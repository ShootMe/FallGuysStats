using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace FallGuysStats {
    public class LogLine {
        public TimeSpan Time { get; } = TimeSpan.Zero;
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string Line { get; set; }
        public bool IsValid { get; set; }
        public long Offset { get; set; }

        public LogLine(string line, long offset) {
            Offset = offset;
            Line = line;
            bool isValidSemiColon = (line.IndexOf(':') == 2 && line.IndexOf(':', 3) == 5 && line.IndexOf(':', 6) == 12);
            bool isValidDot = (line.IndexOf('.') == 2 && line.IndexOf('.', 3) == 5 && line.IndexOf(':', 6) == 12);
            IsValid = isValidSemiColon || isValidDot;
            if (IsValid) {
                Time = TimeSpan.ParseExact(line.Substring(0, 12), isValidSemiColon ? "hh\\:mm\\:ss\\.fff" : "hh\\.mm\\.ss\\.fff", null);
            }
        }

        public override string ToString() {
            return $"{Time}: {Line} ({Offset})";
        }
    }
    public class LogRound {
        public bool CountingPlayers;
        public int Players;
        public bool CurrentlyInParty;
        public bool PrivateLobby;
        public bool FindingPosition;
        public bool IsFinal;
        public bool HasIsFinal;
        public string CurrentPlayerID;
        public int LastPing;
        public int Duration;
        public RoundInfo Info;
    }
    public class LogFileWatcher {
        const int UpdateDelay = 500;

        private string filePath;
        private string prevFilePath;
        private List<LogLine> lines = new List<LogLine>();
        private bool running;
        private bool stop;
        private Thread watcher, parser;

        //TODO Dynamically determine if (real) logs have timestamps
        public bool LogHasTimestamps = false;

        public event Action<List<RoundInfo>> OnParsedLogLines;
        public event Action<List<RoundInfo>> OnParsedLogLinesCurrent;
        public event Action<DateTime> OnNewLogFileDate;
        public event Action<string> OnError;

        public void Start(string logDirectory, string fileName) {
            if (running) { return; }

            filePath = Path.Combine(logDirectory, fileName);
            if (!LogHasTimestamps) {
                filePath = Path.Combine(logDirectory, "player-withtime.log");
            }
            prevFilePath = Path.Combine(logDirectory, Path.GetFileNameWithoutExtension(fileName) + "-prev.log");
            stop = false;
            watcher = new Thread(ReadLogFile) { IsBackground = true };
            watcher.Start();
            parser = new Thread(ParseLines) { IsBackground = true };
            parser.Start();
        }

        public async Task Stop() {
            stop = true;
            while (running || watcher == null || watcher.ThreadState == ThreadState.Unstarted) {
                await Task.Delay(50);
            }
            lines = new List<LogLine>();
            await Task.Factory.StartNew(() => watcher?.Join());
            await Task.Factory.StartNew(() => parser?.Join());
        }

        private void ReadLogFile() {
            running = true;
            List<LogLine> tempLines = new List<LogLine>();
            DateTime lastDate = DateTime.MinValue;
            bool completed = false;
            
            string currentFilePath = prevFilePath;
            if (LogHasTimestamps) {
                currentFilePath = filePath;
            }
            
            long offset = 0;
            while (!stop) {
                try {
                    if (File.Exists(currentFilePath)) {
                        using (FileStream fs = new FileStream(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                            tempLines.Clear();

                            if (fs.Length > offset) {
                                fs.Seek(offset, SeekOrigin.Begin);

                                LineReader sr = new LineReader(fs);
                                string line;
                                DateTime currentDate = lastDate;
                                while ((line = sr.ReadLine()) != null) {
                                    LogLine logLine = new LogLine(line, sr.Position);

                                    if (logLine.IsValid) {
                                        int index;
                                        if ((index = line.IndexOf("[GlobalGameStateClient].PreStart called at ")) > 0) {
                                            currentDate = DateTime.SpecifyKind(DateTime.Parse(line.Substring(index + 43, 19)), DateTimeKind.Utc);
                                            OnNewLogFileDate?.Invoke(currentDate);
                                        }

                                        if (currentDate != DateTime.MinValue) {
                                            if (currentDate.TimeOfDay.TotalSeconds - logLine.Time.TotalSeconds > 60000) {
                                                currentDate = currentDate.AddDays(1);
                                            }
                                            currentDate = currentDate.AddSeconds(logLine.Time.TotalSeconds - currentDate.TimeOfDay.TotalSeconds);
                                            logLine.Date = currentDate;
                                        }

                                        if (line.IndexOf(" == [CompletedEpisodeDto] ==") > 0) {
                                            StringBuilder sb = new StringBuilder(line);
                                            sb.AppendLine();
                                            while ((line = sr.ReadLine()) != null) {
                                                LogLine temp = new LogLine(line, fs.Position);
                                                if (temp.IsValid) {
                                                    logLine.Line = sb.ToString();
                                                    logLine.Offset = sr.Position;
                                                    tempLines.Add(logLine);
                                                    tempLines.Add(temp);
                                                    break;
                                                } else if (!string.IsNullOrEmpty(line)) {
                                                    sb.AppendLine(line);
                                                }
                                            }
                                        } else {
                                            tempLines.Add(logLine);
                                        }
                                    } else if (logLine.Line.IndexOf("Client address: ", StringComparison.OrdinalIgnoreCase) > 0) {
                                        tempLines.Add(logLine);
                                    }
                                }
                            } else if (offset > fs.Length) {
                                offset = 0;
                            }
                        }
                    }

                    if (tempLines.Count > 0) {
                        List<RoundInfo> round = new List<RoundInfo>();
                        LogRound logRound = new LogRound();
                        List<LogLine> currentLines = new List<LogLine>();

                        for (int i = 0; i < tempLines.Count; i++) {
                            LogLine line = tempLines[i];
                            currentLines.Add(line);
                            if (ParseLine(line, round, logRound)) {
                                lastDate = line.Date;
                                offset = line.Offset;
                                lock (lines) {
                                    lines.AddRange(currentLines);
                                    currentLines.Clear();
                                }
                            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) > 0 ||
                                line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateMainMenu with FGClient.StatePrivateLobby", StringComparison.OrdinalIgnoreCase) > 0) {
                                offset = i > 0 ? tempLines[i - 1].Offset : offset;
                                lastDate = line.Date;
                            }
                        }

                        if (logRound.LastPing != 0) {
                            Stats.LastServerPing = logRound.LastPing;
                        }
                        OnParsedLogLinesCurrent?.Invoke(round);
                    }

                    if (!completed) {
                        completed = true;
                        offset = 0;
                        currentFilePath = filePath;
                    }
                } catch (Exception ex) {
                    OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
            running = false;
        }
        private void ParseLines() {
            List<RoundInfo> round = new List<RoundInfo>();
            List<RoundInfo> allStats = new List<RoundInfo>();
            LogRound logRound = new LogRound();

            while (!stop) {
                try {
                    lock (lines) {
                        for (int i = 0; i < lines.Count; i++) {
                            LogLine line = lines[i];
                            if (ParseLine(line, round, logRound)) {
                                allStats.AddRange(round);
                            }
                        }

                        if (allStats.Count > 0) {
                            OnParsedLogLines?.Invoke(allStats);
                            allStats.Clear();
                        }

                        lines.Clear();
                    }
                } catch (Exception ex) {
                    OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
        }
        private readonly Dictionary<string, string> RoundNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "round_follow-the-leader_ss2_launch", "round_follow-the-leader_s6_launch" } };
        private readonly Dictionary<string, string> SceneNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "FallGuy_FollowTheLeader_UNPACKED", "FallGuy_FollowTheLeader" } };
        private bool ParseLine(LogLine line, List<RoundInfo> round, LogRound logRound) {
            int index;
            if ((index = line.Line.IndexOf("[StateGameLoading] Loading game level scene", StringComparison.OrdinalIgnoreCase)) > 0) {
                logRound.Info = new RoundInfo();
                int index2 = line.Line.IndexOf(' ', index + 44);
                if (index2 < 0) { index2 = line.Line.Length; }

                logRound.Info.SceneName = line.Line.Substring(index + 44, index2 - index - 44);
                if (SceneNameReplacer.TryGetValue(logRound.Info.SceneName, out string newName)) {
                    logRound.Info.SceneName = newName;
                }
                logRound.FindingPosition = false;
                round.Add(logRound.Info);
            } else if (logRound.Info != null && (index = line.Line.IndexOf("[StateGameLoading] Finished loading game level", StringComparison.OrdinalIgnoreCase)) > 0) {
                int index2 = line.Line.IndexOf(". ", index + 62);
                if (index2 < 0) { index2 = line.Line.Length; }

                logRound.Info.Name = line.Line.Substring(index + 62, index2 - index - 62);
                if (RoundNameReplacer.TryGetValue(logRound.Info.Name, out string newName)) {
                    logRound.Info.Name = newName;
                }
                logRound.Info.Round = round.Count;
                logRound.Info.Start = line.Date;
                logRound.Info.InParty = logRound.CurrentlyInParty;
                logRound.Info.PrivateLobby = logRound.PrivateLobby;
                logRound.Info.GameDuration = logRound.Duration;
                logRound.CountingPlayers = true;
                logRound.Info.IsFinal = logRound.IsFinal || (!logRound.HasIsFinal && LevelStats.SceneToRound.TryGetValue(logRound.Info.SceneName, out string roundName) && LevelStats.ALL.TryGetValue(roundName, out LevelStats stats) && stats.IsFinal);
            } else if ((index = line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase)) > 0 ||
                (index = line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateMainMenu with FGClient.StatePrivateLobby", StringComparison.OrdinalIgnoreCase)) > 0) {
                logRound.PrivateLobby = line.Line.IndexOf("StatePrivateLobby") > 0;
                logRound.CurrentlyInParty = logRound.PrivateLobby || (line.Line.IndexOf("solo", StringComparison.OrdinalIgnoreCase) > 0);
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                }
                logRound.FindingPosition = false;
                Stats.InShow = true;
                round.Clear();
                logRound.Info = null;
            } else if ((index = line.Line.IndexOf("NetworkGameOptions: durationInSeconds=", StringComparison.OrdinalIgnoreCase)) > 0) {
                int nextIndex = line.Line.IndexOf(" ", index + 38);
                logRound.Duration = int.Parse(line.Line.Substring(index + 38, nextIndex - index - 38));
                index = line.Line.IndexOf("isFinalRound=", StringComparison.OrdinalIgnoreCase);
                logRound.HasIsFinal = index > 0;
                index = line.Line.IndexOf("isFinalRound=True", StringComparison.OrdinalIgnoreCase);
                logRound.IsFinal = index > 0;
            } else if (logRound.Info != null && logRound.CountingPlayers && (line.Line.IndexOf("[ClientGameManager] Finalising spawn", StringComparison.OrdinalIgnoreCase) > 0 || line.Line.IndexOf("[ClientGameManager] Added player ", StringComparison.OrdinalIgnoreCase) > 0)) {
                logRound.Info.Players++;
            } else if ((index = line.Line.IndexOf("[ClientGameManager] Handling bootstrap for local player FallGuy [", StringComparison.OrdinalIgnoreCase)) > 0) {
                int prevIndex = line.Line.IndexOf(']', index + 65);
                logRound.CurrentPlayerID = line.Line.Substring(index + 65, prevIndex - index - 65);
            } else if (logRound.Info != null && line.Line.IndexOf($"[ClientGameManager] Handling unspawn for player FallGuy [{logRound.CurrentPlayerID}]", StringComparison.OrdinalIgnoreCase) > 0) {
                if (logRound.Info.End == DateTime.MinValue) {
                    logRound.Info.Finish = line.Date;
                } else {
                    logRound.Info.Finish = logRound.Info.End;
                }
                logRound.FindingPosition = true;
            } else if (logRound.Info != null && logRound.FindingPosition && (index = line.Line.IndexOf("[ClientGameSession] NumPlayersAchievingObjective=")) > 0) {
                int position = int.Parse(line.Line.Substring(index + 49));
                if (position > 0) {
                    logRound.FindingPosition = false;
                    logRound.Info.Position = position;
                }
            } else if (logRound.Info != null && line.Line.IndexOf("Client address: ", StringComparison.OrdinalIgnoreCase) > 0) {
                index = line.Line.IndexOf("RTT: ");
                if (index > 0) {
                    int msIndex = line.Line.IndexOf("ms", index);
                    logRound.LastPing = int.Parse(line.Line.Substring(index + 5, msIndex - index - 5));
                }
            } else if (logRound.Info != null && line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) > 0) {
                logRound.Info.Start = line.Date;
                logRound.Info.Playing = true;
                logRound.CountingPlayers = false;
            } else if (logRound.Info != null &&
                (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver", StringComparison.OrdinalIgnoreCase) > 0
                || line.Line.IndexOf("Changing local player state to: SpectatingEliminated", StringComparison.OrdinalIgnoreCase) > 0
                || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState", StringComparison.OrdinalIgnoreCase) > 0
                || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) > 0)) {
                if (logRound.Info.End == DateTime.MinValue) {
                    logRound.Info.End = line.Date;
                }
                logRound.Info.Playing = false;
            } else if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) > 0) {
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                }
                logRound.FindingPosition = false;
                logRound.CountingPlayers = false;
                Stats.InShow = false;
            } else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==", StringComparison.OrdinalIgnoreCase) > 0) {
                if (logRound.Info == null) { return false; }

                RoundInfo temp = null;
                StringReader sr = new StringReader(line.Line);
                string detail;
                bool foundRound = false;
                int maxRound = 0;
                DateTime showStart = DateTime.MinValue;
                while ((detail = sr.ReadLine()) != null) {
                    if (detail.IndexOf("[Round ", StringComparison.OrdinalIgnoreCase) == 0) {
                        foundRound = true;
                        int roundNum = (int)detail[7] - 0x30 + 1;
                        string roundName = detail.Substring(11, detail.Length - 12);
                        if (RoundNameReplacer.TryGetValue(roundName, out string newName)) {
                            roundName = newName;
                        }

                        if (roundNum - 1 < round.Count) {
                            if (roundNum > maxRound) {
                                maxRound = roundNum;
                            }

                            temp = round[roundNum - 1];
                            if (string.IsNullOrEmpty(temp.Name) || !temp.Name.Equals(roundName, StringComparison.OrdinalIgnoreCase)) {
                                return false;
                            }

                            temp.VerifyName();
                            if (roundNum == 1) {
                                showStart = temp.Start;
                            }
                            temp.ShowStart = showStart;
                            temp.Playing = false;
                            temp.Round = roundNum;
                            logRound.PrivateLobby = temp.PrivateLobby;
                            logRound.CurrentlyInParty = temp.InParty;
                        } else {
                            return false;
                        }

                        if (temp.End == DateTime.MinValue) {
                            temp.End = line.Date;
                        }
                        if (temp.Start == DateTime.MinValue) {
                            temp.Start = temp.End;
                        }
                        if (!temp.Finish.HasValue) {
                            temp.Finish = temp.End;
                        }
                    } else if (foundRound) {
                        if (detail.IndexOf("> Position: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            temp.Position = int.Parse(detail.Substring(12));
                        } else if (detail.IndexOf("> Team Score: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            temp.Score = int.Parse(detail.Substring(14));
                        } else if (detail.IndexOf("> Qualified: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            char qualified = detail[13];
                            temp.Qualified = qualified == 'T';
                            temp.Finish = temp.Qualified ? temp.Finish : null;
                        } else if (detail.IndexOf("> Bonus Tier: ", StringComparison.OrdinalIgnoreCase) == 0 && detail.Length == 15) {
                            char tier = detail[14];
                            temp.Tier = (int)tier - 0x30 + 1;
                        } else if (detail.IndexOf("> Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            temp.Kudos += int.Parse(detail.Substring(9));
                        } else if (detail.IndexOf("> Bonus Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            temp.Kudos += int.Parse(detail.Substring(15));
                        }
                    }
                }

                if (round.Count > maxRound) {
                    return false;
                }

                logRound.Info = round[round.Count - 1];
                DateTime showEnd = logRound.Info.End;
                for (int i = 0; i < round.Count; i++) {
                    round[i].ShowEnd = showEnd;
                }
                if (logRound.Info.Qualified) {
                    logRound.Info.Crown = true;
                }
                logRound.Info = null;
                Stats.InShow = false;
                Stats.EndedShow = true;
                return true;
            }
            return false;
        }
    }
}