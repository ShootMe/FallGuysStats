using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace FallGuysStats {
    public class RewriteLogLine {
        //public TimeSpan Time { get; } = TimeSpan.Zero;
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string Line { get; set; }
        public bool IsValid { get; set; }
        public long Offset { get; set; }

        public RewriteLogLine(string line, long offset) {
            Offset = offset;
            Line = line;
            
            DateTime dateStamp;
            bool isValidDate = false;

            try {
                var colonIndex = line.IndexOf(':');
                isValidDate = !string.IsNullOrWhiteSpace(line) && colonIndex > 0 && DateTime.TryParse(line.Substring(0, colonIndex), out dateStamp);
            } catch { }

            if (isValidDate) {
                IsValid = true;

                // Ignoring that some lines have a timestamp after the date,
                // could cause problems with timestamp-less lines around it 
                // being presented out of time order

                // Rounding time to 1ms to faithfully mimic the old log format
                var timeStamp = DateTime.UtcNow.AddTicks(-(DateTime.UtcNow.Ticks % 10));
                var timeStampString = timeStamp.ToString("HH\\:mm\\:ss\\.fff");

                Date = timeStamp;

                // Modify line to remove leading date
                Line = Line.Substring(Line.IndexOf(":"));
                Line = timeStampString + Line;
                
            }
        }

        public override string ToString() {
            return $"{Line}";
        }
    }
    public class RewriteLogRound {
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
    public class LogFileRewriter {
        const int UpdateDelay = 50;

        private string filePath;
        private string newFilePath;
        private bool running;
        private bool stop;
        private Thread watcher;

        public event Action<string> OnError;

        public void Start(string logDirectory, string fileName) {
            if (running) { return; }

            filePath = Path.Combine(logDirectory, fileName);
            newFilePath = Path.Combine(logDirectory, "player-withtime.log");

            if (!File.Exists(newFilePath)) {
                File.Create(newFilePath);
            } else {
                // Empty out the player-withtime.log file
                System.IO.File.WriteAllText(newFilePath, string.Empty);
            }

            stop = false;
            watcher = new Thread(ReadLogFile) { IsBackground = true };
            watcher.Start();
        }

        public async Task Stop() {
            stop = true;
            while (running || watcher == null || watcher.ThreadState == ThreadState.Unstarted) {
                await Task.Delay(50);
            }
            await Task.Factory.StartNew(() => watcher?.Join());
        }

        private void ReadLogFile() {
            running = true;
            List<string> tempLines = new List<string>();
            DateTime lastDate = DateTime.MinValue;
            bool completed = false;
            string currentFilePath = filePath;
            long offset = 0;

            while (!stop) {
                try {
                    if (File.Exists(currentFilePath)) {
                        using (FileStream fs = new FileStream(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                            tempLines.Clear();

                            if (offset == 0) {
                                offset = fs.Length;
                            }

                            if (fs.Length > offset) {
                                fs.Seek(offset, SeekOrigin.Begin);

                                LineReader sr = new LineReader(fs);
                                string line;
                                DateTime currentDate = lastDate;
                                while ((line = sr.ReadLine()) != null) {
                                    
                                    RewriteLogLine logLine = new RewriteLogLine(line, sr.Position);
                                    tempLines.Add(logLine.Line);

                                    // [StateGameLoading] Loading game level scene
                                    int index;
                                    if ((index = logLine.Line.IndexOf("[StateGameLoading] Loading game level scene", StringComparison.OrdinalIgnoreCase)) > 0) {

                                        // Search player-withtime.log for the "[GlobalGameStateClient].PreStart called at" message. if none found, inject it
                                        string contentsWithTimeLog = File.ReadAllText(newFilePath);
                                        if (contentsWithTimeLog.IndexOf("[GlobalGameStateClient].PreStart called at", StringComparison.OrdinalIgnoreCase) < 0) {
                                            tempLines.Add(String.Format("{0}: [GlobalGameStateClient].PreStart called at {1}  UTC", logLine.Date.ToString("HH\\:mm\\:ss\\.fff"), logLine.Date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")));
                                        }
                                    }

                                    offset = sr.Position;
                                }
                            } else if (offset > fs.Length) {
                                offset = fs.Length;
                            }
                        }
                    }

                    if (tempLines.Count > 0) {
                        // Write to the new file
                        File.AppendAllLines(newFilePath, tempLines, Encoding.UTF8);
                    }

                } catch (Exception ex) {
                    OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
            running = false;
        }
    }
}