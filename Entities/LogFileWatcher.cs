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
        public event Action<DateTime> OnNewLogFileDate;

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
                                                currentDate = DateTime.Parse(line.Substring(index + 43, 19));
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
                    }
                } catch { }
                Thread.Sleep(UpdateDelay);
            }
            running = false;
        }
        private void ParseLines() {
            RoundInfo stat = null;
            List<RoundInfo> round = new List<RoundInfo>();
            List<RoundInfo> allStats = new List<RoundInfo>();
            int players;
            bool countPlayers = false;
            bool currentlyInParty = false;
            while (!stop) {
                try {
                    lock (lines) {
                        for (int i = 0; i < lines.Count; i++) {
                            LogLine line = lines[i];
                            int index;
                            if ((index = line.Line.IndexOf("[StateGameLoading] Finished loading game level")) > 0) {
                                stat = new RoundInfo();
                                round.Add(stat);
                                stat.Name = line.Line.Substring(index + 62);
                                stat.Start = line.Date;
                                stat.InParty = currentlyInParty;
                                countPlayers = true;
                            } else if ((index = line.Line.IndexOf("[StateMatchmaking] Begin matchmaking")) > 0) {
                                currentlyInParty = !line.Line.Substring(index + 37).Equals("solo");
                            } else if (stat != null && countPlayers && line.Line.IndexOf("[ClientGameManager] Added player ") > 0 && (index = line.Line.IndexOf(" players in system.")) > 0) {
                                int prevIndex = line.Line.LastIndexOf(' ', index - 1);
                                if (int.TryParse(line.Line.Substring(prevIndex, index - prevIndex), out players)) {
                                    stat.Players = players;
                                }
                            } else if (stat != null && line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing") > 0) {
                                stat.Start = line.Date;
                                countPlayers = false;
                            } else if (stat != null &&
                                (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver") > 0
                                || line.Line.IndexOf("Changing local player state to: SpectatingEliminated") > 0)) {
                                stat.End = line.Date;
                            } else if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu") > 0) {
                                round.Clear();
                                stat = null;
                            } else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==") > 0) {
                                if (stat.End == DateTime.MinValue) {
                                    stat.End = line.Date;
                                }

                                StringReader sr = new StringReader(line.Line);
                                string detail;
                                bool foundRound = false;
                                while ((detail = sr.ReadLine()) != null) {
                                    if (detail.IndexOf("[Round ") == 0) {
                                        foundRound = true;
                                        int roundNum = (int)detail[7] - 0x30 + 1;
                                        if (roundNum - 1 < round.Count) {
                                            stat = round[roundNum - 1];
                                            stat.Round = roundNum;
                                            currentlyInParty = stat.InParty;
                                            if (stat.End == DateTime.MinValue) {
                                                stat.End = DateTime.Now;
                                            }
                                            if (stat.Start == DateTime.MinValue) {
                                                stat.Start = stat.End.AddSeconds(-1);
                                            }
                                        } else {
                                            stat = round[roundNum - 2];
                                            stat = new RoundInfo() { Start = stat.End, End = stat.End.AddSeconds(1), Name = detail.Substring(11, detail.Length - 12), Round = roundNum, InParty = currentlyInParty };
                                            round.Add(stat);
                                        }
                                    } else if (foundRound) {
                                        if (detail.IndexOf("> Position: ") == 0) {
                                            stat.Position = int.Parse(detail.Substring(12));
                                        } else if (detail.IndexOf("> Team Score: ") == 0) {
                                            stat.Score = int.Parse(detail.Substring(14));
                                        } else if (detail.IndexOf("> Qualified: ") == 0) {
                                            char qualified = detail[13];
                                            stat.Qualified = qualified == 'T';
                                        } else if (detail.IndexOf("> Bonus Tier: ") == 0 && detail.Length == 15) {
                                            char tier = detail[14];
                                            stat.Tier = (int)tier - 0x30 + 1;
                                        } else if (detail.IndexOf("> Kudos: ") == 0) {
                                            stat.Kudos += int.Parse(detail.Substring(9));
                                        } else if (detail.IndexOf("> Bonus Kudos: ") == 0) {
                                            stat.Kudos += int.Parse(detail.Substring(15));
                                        }
                                    }
                                }

                                allStats.AddRange(round);
                                round.Clear();
                                stat = null;
                            }
                        }

                        if (allStats.Count > 0) {
                            OnParsedLogLines?.Invoke(allStats);
                            allStats.Clear();
                        }

                        lines.Clear();
                    }
                } catch { }
                Thread.Sleep(UpdateDelay);
            }
        }
    }
}