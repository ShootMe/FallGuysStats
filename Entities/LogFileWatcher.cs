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

        public LogLine(string line) {
            Line = line;
            if (line.IndexOf(':') == 2 && line.IndexOf(':', 3) == 5 && line.IndexOf(':', 6) == 12) {
                Time = TimeSpan.Parse(line.Substring(0, 12));
            }
        }

        public override string ToString() {
            return $"{Time}: {Line}";
        }
    }
    public class LogFileWatcher {
        const int UpdateDelay = 500;

        private string filePath;
        private string prevFilePath;
        private List<LogLine> lines = new List<LogLine>();
        private bool running;
        private bool stop;
        private Thread watcher, parser;

        public event Action<List<RoundInfo>> OnParsedLogLines;
        public event Action<List<RoundInfo>> OnParsedLogLinesCurrent;
        public event Action<DateTime> OnNewLogFileDate;
        public event Action<string> OnError;

        public void Start(string logDirectory, string fileName) {
            if (running) { return; }

            filePath = Path.Combine(logDirectory, fileName);
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
            List<LogLine> currentLines = new List<LogLine>();
            List<LogLine> tempLines = new List<LogLine>();
            DateTime lastDate = DateTime.MinValue;
            bool completed = false;
            string currentFilePath = prevFilePath;
            long offset = 0;
            while (!stop) {
                try {
                    if (File.Exists(currentFilePath)) {
                        using (FileStream fs = new FileStream(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                            tempLines.Clear();

                            if (fs.Length > offset) {
                                fs.Seek(offset, SeekOrigin.Begin);

                                using (StreamReader sr = new StreamReader(fs)) {
                                    string line;
                                    DateTime currentDate = lastDate;
                                    while (!sr.EndOfStream && (line = sr.ReadLine()) != null) {
                                        LogLine logLine = new LogLine(line);

                                        if (logLine.Time != TimeSpan.Zero) {
                                            int index;
                                            if ((index = line.IndexOf("[GlobalGameStateClient].PreStart called at ")) > 0) {
                                                currentDate = DateTime.SpecifyKind(DateTime.Parse(line.Substring(index + 43, 19)), DateTimeKind.Local);
                                                OnNewLogFileDate?.Invoke(currentDate);
                                            }

                                            if (currentDate != DateTime.MinValue) {
                                                if (currentDate.TimeOfDay > logLine.Time) {
                                                    currentDate = currentDate.AddDays(1);
                                                }
                                                currentDate = currentDate.AddSeconds(logLine.Time.TotalSeconds - currentDate.TimeOfDay.TotalSeconds);
                                                logLine.Date = currentDate;
                                            }

                                            if (line.IndexOf(" == [CompletedEpisodeDto] ==") > 0) {
                                                StringBuilder sb = new StringBuilder(line);
                                                sb.AppendLine();
                                                while (!sr.EndOfStream && (line = sr.ReadLine()) != null) {
                                                    LogLine temp = new LogLine(line);
                                                    if (temp.Time != TimeSpan.Zero) {
                                                        logLine.Line = sb.ToString();
                                                        currentLines.AddRange(tempLines);
                                                        currentLines.Add(logLine);
                                                        currentLines.Add(temp);
                                                        lastDate = currentDate;
                                                        offset = fs.Position;
                                                        tempLines.Clear();
                                                        break;
                                                    } else if (!string.IsNullOrEmpty(line)) {
                                                        sb.AppendLine(line);
                                                    }
                                                }
                                            } else {
                                                tempLines.Add(logLine);
                                            }
                                        }
                                    }
                                }
                            } else if (offset > fs.Length) {
                                offset = 0;
                            }
                        }
                    }

                    if (!completed) {
                        completed = true;
                        offset = 0;
                        currentFilePath = filePath;
                    }

                    if (currentLines.Count > 0) {
                        lock (lines) {
                            lines.AddRange(currentLines);
                            currentLines.Clear();
                        }
                    } else if (tempLines.Count > 0) {
                        RoundInfo stat = null;
                        List<RoundInfo> round = new List<RoundInfo>();
                        int players = 0;
                        bool countPlayers = false;
                        bool currentlyInParty = false;
                        bool findPosition = false;
                        string currentPlayerID = string.Empty;
                        for (int i = 0; i < tempLines.Count; i++) {
                            LogLine line = tempLines[i];
                            ParseLine(line, round, ref currentPlayerID, ref countPlayers, ref currentlyInParty, ref findPosition, ref players, ref stat);
                        }

                        OnParsedLogLinesCurrent?.Invoke(round);
                    }
                } catch (Exception ex) {
                    OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
            running = false;
        }
        private void ParseLines() {
            RoundInfo stat = null;
            List<RoundInfo> round = new List<RoundInfo>();
            List<RoundInfo> allStats = new List<RoundInfo>();
            int players = 0;
            bool countPlayers = false;
            bool currentlyInParty = false;
            bool findPosition = false;
            string currentPlayerID = string.Empty;
            while (!stop) {
                try {
                    lock (lines) {
                        for (int i = 0; i < lines.Count; i++) {
                            LogLine line = lines[i];
                            if (ParseLine(line, round, ref currentPlayerID, ref countPlayers, ref currentlyInParty, ref findPosition, ref players, ref stat)) {
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
        private bool ParseLine(LogLine line, List<RoundInfo> round, ref string currentPlayerID, ref bool countPlayers, ref bool currentlyInParty, ref bool findPosition, ref int players, ref RoundInfo stat) {
            int index;
            if ((index = line.Line.IndexOf("[StateGameLoading] Finished loading game level", StringComparison.OrdinalIgnoreCase)) > 0) {
                stat = new RoundInfo();
                round.Add(stat);
                stat.Name = line.Line.Substring(index + 62);
                stat.Round = round.Count;
                stat.Start = line.Date;
                stat.InParty = currentlyInParty;
                countPlayers = true;
            } else if ((index = line.Line.IndexOf("[StateMatchmaking] Begin matchmaking", StringComparison.OrdinalIgnoreCase)) > 0) {
                currentlyInParty = !line.Line.Substring(index + 37).Equals("solo", StringComparison.OrdinalIgnoreCase);
                if (stat != null) {
                    if (stat.End == DateTime.MinValue) {
                        stat.End = line.Date;
                    }
                    stat.Playing = false;
                }
                Stats.InShow = true;
                round.Clear();
                stat = null;
            } else if (stat != null && countPlayers && line.Line.IndexOf("[ClientGameManager] Added player ", StringComparison.OrdinalIgnoreCase) > 0 && (index = line.Line.IndexOf(" players in system.", StringComparison.OrdinalIgnoreCase)) > 0) {
                int prevIndex = line.Line.LastIndexOf(' ', index - 1);
                if (int.TryParse(line.Line.Substring(prevIndex, index - prevIndex), out players)) {
                    stat.Players = players;
                }
            } else if ((index = line.Line.IndexOf("[ClientGameManager] Handling bootstrap for local player FallGuy [", StringComparison.OrdinalIgnoreCase)) > 0) {
                int prevIndex = line.Line.IndexOf(']', index + 65);
                currentPlayerID = line.Line.Substring(index + 65, prevIndex - index - 65);
            } else if (stat != null && line.Line.IndexOf($"[ClientGameManager] Handling unspawn for player FallGuy [{currentPlayerID}]", StringComparison.OrdinalIgnoreCase) > 0) {
                if (stat.End == DateTime.MinValue) {
                    stat.Finish = line.Date;
                } else {
                    stat.Finish = stat.End;
                }
                findPosition = true;
            } else if (stat != null && findPosition && (index = line.Line.IndexOf("[ClientGameSession] NumPlayersAchievingObjective=")) > 0) {
                int position = int.Parse(line.Line.Substring(index + 49));
                if (position > 0) {
                    findPosition = false;
                    stat.Position = position;
                }
            } else if (stat != null && line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) > 0) {
                stat.Start = line.Date;
                stat.Playing = true;
                countPlayers = false;
            } else if (stat != null &&
                (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver", StringComparison.OrdinalIgnoreCase) > 0
                || line.Line.IndexOf("Changing local player state to: SpectatingEliminated", StringComparison.OrdinalIgnoreCase) > 0
                || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState", StringComparison.OrdinalIgnoreCase) > 0)) {
                if (stat.End == DateTime.MinValue) {
                    stat.End = line.Date;
                }
                stat.Playing = false;
                findPosition = false;
            } else if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) > 0) {
                if (stat != null) {
                    if (stat.End == DateTime.MinValue) {
                        stat.End = line.Date;
                    }
                    stat.Playing = false;
                }
                findPosition = false;
                countPlayers = false;
                Stats.InShow = false;
            } else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==", StringComparison.OrdinalIgnoreCase) > 0) {
                if (stat == null) { return false; }

                RoundInfo temp = null;
                StringReader sr = new StringReader(line.Line);
                string detail;
                bool foundRound = false;
                int maxRound = 0;
                while ((detail = sr.ReadLine()) != null) {
                    if (detail.IndexOf("[Round ", StringComparison.OrdinalIgnoreCase) == 0) {
                        foundRound = true;
                        int roundNum = (int)detail[7] - 0x30 + 1;
                        string roundName = detail.Substring(11, detail.Length - 12);
                        if (roundNum - 1 < round.Count) {
                            if (roundNum > maxRound) {
                                maxRound = roundNum;
                            }

                            temp = round[roundNum - 1];
                            if (!temp.Name.Equals(roundName, StringComparison.OrdinalIgnoreCase)) {
                                return false;
                            }

                            temp.Playing = false;
                            temp.Round = roundNum;
                            currentlyInParty = temp.InParty;
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

                stat = null;
                Stats.InShow = false;
                Stats.EndedShow = true;
                return true;
            }
            return false;
        }
    }
}