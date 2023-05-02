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
            this.Offset = offset;
            this.Line = line;
            bool isValidSemiColon = (line.IndexOf(':') == 2 && line.IndexOf(':', 3) == 5 && line.IndexOf(':', 6) == 12);
            bool isValidDot = (line.IndexOf('.') == 2 && line.IndexOf('.', 3) == 5 && line.IndexOf(':', 6) == 12);
            this.IsValid = isValidSemiColon || isValidDot;
            if (this.IsValid) {
                this.Time = TimeSpan.ParseExact(line.Substring(0, 12), isValidSemiColon ? "hh\\:mm\\:ss\\.fff" : "hh\\.mm\\.ss\\.fff", null);
            }
        }

        public override string ToString() {
            return $"{this.Time}: {this.Line} ({this.Offset})";
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
        public Stats StatsForm { get; set; }
        private string selectedShowId;
        private string sessionId;
        private bool autoChangeProfile, preventMouseCursorBugs;
        public event Action<List<RoundInfo>> OnParsedLogLines;
        public event Action<List<RoundInfo>> OnParsedLogLinesCurrent;
        public event Action<DateTime> OnNewLogFileDate;
        public event Action<string> OnError;

        public void SetAutoChangeProfile(bool option) {
            this.autoChangeProfile = option;
        }
        public void SetPreventMouseCursorBugs(bool option) {
            this.preventMouseCursorBugs = option;
        }

        public void Start(string logDirectory, string fileName) {
            if (this.running) { return; }

            this.filePath = Path.Combine(logDirectory, fileName);
            this.prevFilePath = Path.Combine(logDirectory, Path.GetFileNameWithoutExtension(fileName) + "-prev.log");
            this.stop = false;
            this.watcher = new Thread(this.ReadLogFile) { IsBackground = true };
            this.watcher.Start();
            this.parser = new Thread(this.ParseLines) { IsBackground = true };
            this.parser.Start();
        }

        public async Task Stop() {
            this.stop = true;
            while (this.running || this.watcher == null || this.watcher.ThreadState == ThreadState.Unstarted) {
                await Task.Delay(50);
            }
            this.lines = new List<LogLine>();
            await Task.Factory.StartNew(() => this.watcher?.Join());
            await Task.Factory.StartNew(() => this.parser?.Join());
        }

        private void ReadLogFile() {
            this.running = true;
            List<LogLine> tempLines = new List<LogLine>();
            DateTime lastDate = DateTime.MinValue;
            bool completed = false;
            string currentFilePath = prevFilePath;
            long offset = 0;
            while (!this.stop) {
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
                                            this.OnNewLogFileDate?.Invoke(currentDate);
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
                            if (this.ParseLine(line, round, logRound)) {
                                lastDate = line.Date;
                                offset = line.Offset;
                                lock (this.lines) {
                                    this.lines.AddRange(currentLines);
                                    currentLines.Clear();
                                }
                            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) > 0
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateMainMenu with FGClient.StatePrivateLobby", StringComparison.OrdinalIgnoreCase) > 0
                                       || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState called with reason IngameMenuLeaveMatch", StringComparison.OrdinalIgnoreCase) > 0
                                       || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState called with reason LeaveMatchSquadMode", StringComparison.OrdinalIgnoreCase) > 0
                                       || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) > 0) {
                                offset = i > 0 ? tempLines[i - 1].Offset : offset;
                                lastDate = line.Date;
                            }
                        }

                        if (logRound.LastPing != 0) {
                            Stats.LastServerPing = logRound.LastPing;
                        }
                        this.OnParsedLogLinesCurrent?.Invoke(round);
                    }

                    if (!completed) {
                        completed = true;
                        offset = 0;
                        currentFilePath = filePath;
                    }
                } catch (Exception ex) {
                    this.OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
            this.running = false;
        }
        private void ParseLines() {
            List<RoundInfo> round = new List<RoundInfo>();
            List<RoundInfo> allStats = new List<RoundInfo>();
            LogRound logRound = new LogRound();

            while (!this.stop) {
                try {
                    lock (this.lines) {
                        for (int i = 0; i < this.lines.Count; i++) {
                            LogLine line = this.lines[i];
                            if (this.ParseLine(line, round, logRound)) {
                                allStats.AddRange(round);
                            }
                        }

                        if (allStats.Count > 0) {
                            this.OnParsedLogLines?.Invoke(allStats);
                            allStats.Clear();
                        }

                        this.lines.Clear();
                    }
                } catch (Exception ex) {
                    this.OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
        }
        private readonly Dictionary<string, string> _roundNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "round_follow-the-leader_ss2_launch", "round_follow-the-leader_s6_launch" },
            
            // Finals Marathon
            {"round_kraken_attack_only_finals_v2_r1", "round_kraken_attack"},
            {"round_kraken_attack_only_finals_v2_r2", "round_kraken_attack"},
            {"round_kraken_attack_only_finals_v2_r3_r4", "round_kraken_attack"},
            {"round_kraken_attack_only_finals_v2_final", "round_kraken_attack"},
            
            {"round_blastball_only_finals_v2_r1", "round_blastball_arenasurvival_symphony_launch_show"},
            {"round_blastball_only_finals_v2_r2", "round_blastball_arenasurvival_symphony_launch_show"},
            {"round_blastball_only_finals_v2_r3_r4", "round_blastball_arenasurvival_symphony_launch_show"},
            {"round_blastball_only_finals_v2_final", "round_blastball_arenasurvival_symphony_launch_show"},
            
            {"round_floor_fall_only_finals_v2_r1", "round_floor_fall"},
            {"round_floor_fall_only_finals_v2_r2", "round_floor_fall"},
            {"round_floor_fall_only_finals_v2_r3_r4", "round_floor_fall"},
            {"round_floor_fall_only_finals_v2_final", "round_floor_fall"},

            {"round_hexsnake_only_finals_v2_r1", "round_hexsnake_almond"},
            {"round_hexsnake_only_finals_v2_r2", "round_hexsnake_almond"},
            {"round_hexsnake_only_finals_v2_r3_r4", "round_hexsnake_almond"},
            {"round_hexsnake_only_finals_v2_final", "round_hexsnake_almond"},

            {"round_jump_showdown_only_finals_v2_r1", "round_jump_showdown"},
            {"round_jump_showdown_only_finals_v2_r2", "round_jump_showdown"},
            {"round_jump_showdown_only_finals_v2_r3_r4", "round_jump_showdown"},
            {"round_jump_showdown_only_finals_v2_final", "round_jump_showdown"},

            {"round_hexaring_only_finals_v2_r1", "round_hexaring_symphony_launch_show"},
            {"round_hexaring_only_finals_v2_r2", "round_hexaring_symphony_launch_show"},
            {"round_hexaring_only_finals_v2_r3_r4", "round_hexaring_symphony_launch_show"},
            {"round_hexaring_only_finals_v2_final", "round_hexaring_symphony_launch_show"},
            
            {"round_tunnel_final_only_finals_v2_r1", "round_tunnel_final"},
            {"round_tunnel_final_only_finals_v2_r2", "round_tunnel_final"},
            {"round_tunnel_final_only_finals_v2_r3_r4", "round_tunnel_final"},
            {"round_tunnel_final_only_finals_v2_final", "round_tunnel_final"},
            
            // Hex-a-gone Trials
            {"round_floor_fall_event_only_01", "round_floor_fall"},
            {"round_floor_fall_event_only_02", "round_floor_fall"},
            {"round_floor_fall_event_only_final", "round_floor_fall"},
            
            // Hex-a-Gravity Trials
            {"round_floor_fall_event_only_low_grav_01", "round_floor_fall"},
            {"round_floor_fall_event_only_low_grav_02", "round_floor_fall"},
            {"round_floor_fall_event_only_low_grav_final", "round_floor_fall"},
            
            // Hex-a-thon
            {"round_floor_fall_event_walnut", "round_floor_fall"},
            {"round_floor_fall_event_walnut_r2", "round_floor_fall"},
            {"round_floor_fall_event_walnut_final", "round_floor_fall"},
            
            {"round_hexaring_event_walnut", "round_hexaring_symphony_launch_show"},
            {"round_hexaring_event_walnut_r2", "round_hexaring_symphony_launch_show"},
            {"round_hexaring_event_walnut_final", "round_hexaring_symphony_launch_show"},
            
            {"round_hexsnake_event_walnut", "round_hexsnake_almond"},
            {"round_hexsnake_event_walnut_r2", "round_hexsnake_almond"},
            {"round_hexsnake_event_walnut_final", "round_hexsnake_almond"},
            
            // Blast Ball Trials
            {"round_blastball_arenasurvival_blast_ball_trials_01", "round_blastball_arenasurvival_symphony_launch_show"},
            {"round_blastball_arenasurvival_blast_ball_trials_02", "round_blastball_arenasurvival_symphony_launch_show"},
            {"round_blastball_arenasurvival_blast_ball_trials_fn", "round_blastball_arenasurvival_symphony_launch_show"},
        };
        private readonly Dictionary<string, string> _sceneNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "FallGuy_FollowTheLeader_UNPACKED", "FallGuy_FollowTheLeader" } };

        private bool GetIsRealLastRound(string roundName) {
            return (roundName.IndexOf("round_jinxed", StringComparison.OrdinalIgnoreCase) != -1
                    && roundName.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1)

                   || (roundName.IndexOf("round_fall_ball", StringComparison.OrdinalIgnoreCase) != -1
                       && roundName.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                       && roundName.IndexOf("_cup_only", StringComparison.OrdinalIgnoreCase) == -1)

                   || (roundName.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1
                       && roundName.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1)

                   || (roundName.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) != -1
                       && roundName.Substring(roundName.Length - 6).ToLower() == "_final")

                   || roundName.EndsWith("_xtreme_party_final", StringComparison.OrdinalIgnoreCase);
        }

        private bool GetIsModeException(string sceneName) {
            return sceneName.IndexOf("round_lava_event_only_slime_climb", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_kraken_attack_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_blastball_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_floor_fall_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_hexsnake_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_jump_showdown_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_hexaring_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_tunnel_final_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_floor_fall_event_only", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_floor_fall_event_only_low_grav", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_floor_fall_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_hexaring_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_hexsnake_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_blastball_arenasurvival_blast_ball_trials", StringComparison.OrdinalIgnoreCase) != -1
                   || sceneName.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool GetIsFinalException(string sceneName) {
            return ((sceneName.IndexOf("round_lava_event_only_slime_climb", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_kraken_attack_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_blastball_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_floor_fall_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_hexsnake_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_jump_showdown_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_hexaring_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_tunnel_final_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_floor_fall_event_only", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_floor_fall_event_only_low_grav", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_floor_fall_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_hexaring_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                     || sceneName.IndexOf("round_hexsnake_event_walnut", StringComparison.OrdinalIgnoreCase) != -1)
                         && sceneName.Substring(sceneName.Length - 6).ToLower() == "_final")

                     || (sceneName.IndexOf("round_blastball_arenasurvival_blast_ball_trials", StringComparison.OrdinalIgnoreCase) != -1
                         && sceneName.Substring(sceneName.Length - 3).ToLower() == "_fn")

                     || (sceneName.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1
                         && sceneName.Substring(sceneName.Length - 3) == "_03");
        }
        
        private bool GetIsTeamException(string roundName) {
            return roundName.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1
                   && (roundName.IndexOf("_duos", StringComparison.OrdinalIgnoreCase) != -1
                       || roundName.IndexOf("_squads", StringComparison.OrdinalIgnoreCase) != -1);
        }

        private bool ParseLine(LogLine line, List<RoundInfo> round, LogRound logRound) {
            int index;
            if (Stats.InShow && logRound.Info == null && 
                (index = line.Line.IndexOf("[HandleSuccessfulLogin] Selected show is", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                this.selectedShowId = line.Line.Substring(line.Line.Length - (line.Line.Length - index - 41));
                if (this.autoChangeProfile && !Stats.EndedShow) {
                    this.StatsForm.SetLinkedProfile(this.selectedShowId, logRound.PrivateLobby);
                }
            }
            else if ((index = line.Line.IndexOf("[HandleSuccessfulLogin] Session: ", StringComparison.OrdinalIgnoreCase)) > 0) {
                //Store SessionID to prevent duplicates
                this.sessionId = line.Line.Substring(index + 33);
            }
            else if ((index = line.Line.IndexOf("[StateGameLoading] Loading game level scene", StringComparison.OrdinalIgnoreCase)) > 0) {
                logRound.Info = new RoundInfo { ShowNameId = this.selectedShowId, SessionId = this.sessionId };
                int index2 = line.Line.IndexOf(' ', index + 44);
                if (index2 < 0) { index2 = line.Line.Length; }

                logRound.Info.SceneName = line.Line.Substring(index + 44, index2 - index - 44);
                if (_sceneNameReplacer.TryGetValue(logRound.Info.SceneName, out string newName)) {
                    logRound.Info.SceneName = newName;
                }
                logRound.FindingPosition = false;
                round.Add(logRound.Info);
            }
            else if (logRound.Info != null &&
                       (index = line.Line.IndexOf("[StateGameLoading] Finished loading game level", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                int index2 = line.Line.IndexOf(". ", index + 62);
                if (index2 < 0) { index2 = line.Line.Length; }
                logRound.Info.Name = line.Line.Substring(index + 62, index2 - index - 62);

                bool isRealLastRound = this.GetIsRealLastRound(logRound.Info.Name);
                bool isModeException = this.GetIsModeException(logRound.Info.Name);
                bool isFinalException = this.GetIsFinalException(logRound.Info.Name);
                bool isTeamException = this.GetIsTeamException(logRound.Info.Name);

                if (_roundNameReplacer.TryGetValue(logRound.Info.Name, out string newName)) {
                    logRound.Info.Name = newName;
                }
                logRound.Info.Round = round.Count;
                logRound.Info.Start = line.Date;
                logRound.Info.InParty = logRound.CurrentlyInParty;
                logRound.Info.PrivateLobby = logRound.PrivateLobby;
                logRound.Info.GameDuration = logRound.Duration;
                logRound.CountingPlayers = true;
                
                if (isRealLastRound) {
                    logRound.Info.IsFinal = true;
                } else if (isModeException) {
                    logRound.Info.IsFinal = isFinalException;
                } else {
                    logRound.Info.IsFinal = logRound.IsFinal || 
                                            (!logRound.HasIsFinal && LevelStats.SceneToRound.TryGetValue(logRound.Info.SceneName, out string roundName) && LevelStats.ALL.TryGetValue(roundName, out LevelStats stats) && stats.IsFinal);
                }
                logRound.Info.IsTeam = isTeamException;
            }
            else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) > 0
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateMainMenu with FGClient.StatePrivateLobby", StringComparison.OrdinalIgnoreCase) > 0)
            {
                logRound.PrivateLobby = line.Line.IndexOf("StatePrivateLobby", StringComparison.OrdinalIgnoreCase) > 0;
                logRound.CurrentlyInParty = !logRound.PrivateLobby && (line.Line.IndexOf("solo", StringComparison.OrdinalIgnoreCase) == -1);
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
            }
            else if ((index = line.Line.IndexOf("NetworkGameOptions: durationInSeconds=", StringComparison.OrdinalIgnoreCase)) > 0) { // legacy code // It seems to have been deleted from the log file now.
                int nextIndex = line.Line.IndexOf(" ", index + 38);
                logRound.Duration = int.Parse(line.Line.Substring(index + 38, nextIndex - index - 38));
                index = line.Line.IndexOf("isFinalRound=", StringComparison.OrdinalIgnoreCase);
                logRound.HasIsFinal = index > 0;
                index = line.Line.IndexOf("isFinalRound=True", StringComparison.OrdinalIgnoreCase);
                logRound.IsFinal = index > 0;
            }
            else if (logRound.Info != null && logRound.CountingPlayers &&
                       (line.Line.IndexOf("[ClientGameManager] Finalising spawn", StringComparison.OrdinalIgnoreCase) > 0
                        || line.Line.IndexOf("[ClientGameManager] Added player ", StringComparison.OrdinalIgnoreCase) > 0))
            {
                logRound.Info.Players++;
            }
            else if (logRound.Info != null && logRound.CountingPlayers &&
                       (line.Line.IndexOf("[CameraDirector] Adding Spectator target", StringComparison.OrdinalIgnoreCase) > 0))
            {
                if (line.Line.IndexOf("ps4", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersPs4++;
                } else if (line.Line.IndexOf("ps5", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersPs5++;
                } else if (line.Line.IndexOf("xb1", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersXb1++;
                } else if (line.Line.IndexOf("xsx", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersXsx++;
                } else if (line.Line.IndexOf("switch", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersSw++;
                } else if (line.Line.IndexOf("win", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersPc++;
                } else if (line.Line.IndexOf("bots", StringComparison.OrdinalIgnoreCase) > 0) {
                    logRound.Info.PlayersBots++;
                } else {
                    logRound.Info.PlayersEtc++;
                }
            }
            else if (line.Line.IndexOf("[ClientGameManager] Handling bootstrap for local player FallGuy", StringComparison.OrdinalIgnoreCase) > 0 &&
                       (index = line.Line.IndexOf("playerID = ", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                int prevIndex = line.Line.IndexOf(',', index + 11);
                logRound.CurrentPlayerID = line.Line.Substring(index + 11, prevIndex - index - 11);
            }
            else if (logRound.Info != null && 
                       line.Line.IndexOf($"HandleServerPlayerProgress PlayerId={logRound.CurrentPlayerID} is succeeded=", StringComparison.OrdinalIgnoreCase) > 0)
            {
                index = line.Line.IndexOf("succeeded=True", StringComparison.OrdinalIgnoreCase);
                if (index > 0) {
                    logRound.Info.Finish = logRound.Info.End == DateTime.MinValue ? line.Date : logRound.Info.End;
                }
                logRound.FindingPosition = true;
            }
            else if (logRound.Info != null &&
                       logRound.FindingPosition && (index = line.Line.IndexOf("[ClientGameSession] NumPlayersAchievingObjective=")) > 0)
            {
                int position = int.Parse(line.Line.Substring(index + 49));
                if (position > 0) {
                    logRound.FindingPosition = false;
                    logRound.Info.Position = position;
                }
            }
            else if (logRound.Info != null &&
                       line.Line.IndexOf("Client address: ", StringComparison.OrdinalIgnoreCase) > 0)
            {
                index = line.Line.IndexOf("RTT: ");
                if (index > 0) {
                    int msIndex = line.Line.IndexOf("ms", index);
                    logRound.LastPing = int.Parse(line.Line.Substring(index + 5, msIndex - index - 5));
                }
            //} else if (logRound.Info != null && line.Line.IndexOf("[GameSession] Changing state from Precountdown to Countdown", StringComparison.OrdinalIgnoreCase) > 0) {
            }
            else if (logRound.Info != null &&
                       line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) > 0)
            {
                if (this.preventMouseCursorBugs && Stats.InShow && !Stats.EndedShow) {
                    this.StatsForm.PreventMouseCursorBug();
                }
                logRound.Info.Start = line.Date;
                logRound.Info.Playing = true;
                logRound.CountingPlayers = false;
            //} else if (line.Line.IndexOf("[GameSession] Changing state from GameOver to Results", StringComparison.OrdinalIgnoreCase) > 0
            //           || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateGameInProgress with FGClient.StateQualificationScreen", StringComparison.OrdinalIgnoreCase) > 0) {
            }
            else if (logRound.Info != null &&
                       (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver", StringComparison.OrdinalIgnoreCase) > 0
                        || line.Line.IndexOf("Changing local player state to: SpectatingEliminated", StringComparison.OrdinalIgnoreCase) > 0
                        || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState", StringComparison.OrdinalIgnoreCase) > 0
                        || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) > 0))
            {
                if (line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) > 0) { logRound.PrivateLobby = false; }
                if (logRound.Info.End == DateTime.MinValue) {
                    logRound.Info.End = line.Date;
                }
                logRound.Info.Playing = false;
            }
            else if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) > 0
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateReloadingToMainMenu with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) > 0
                       || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState called with reason IngameMenuLeaveMatch", StringComparison.OrdinalIgnoreCase) > 0
                       || line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState called with reason LeaveMatchSquadMode", StringComparison.OrdinalIgnoreCase) > 0)
            {
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                }
                logRound.FindingPosition = false;
                logRound.CountingPlayers = false;
                Stats.InShow = false;
                Stats.EndedShow = true;
            }
            else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==", StringComparison.OrdinalIgnoreCase) > 0) {
                if (logRound.Info == null) { return false; }

                RoundInfo temp = null;
                StringReader sr = new StringReader(line.Line);
                string detail;
                bool foundRound = false;
                int maxRound = 0;
                DateTime showStart = DateTime.MinValue;
                int questKudos = 0;
                while ((detail = sr.ReadLine()) != null) {
                    if (detail.IndexOf("[Round ", StringComparison.OrdinalIgnoreCase) == 0) {
                        foundRound = true;
                        int roundNum = (int)detail[7] - 0x30 + 1;
                        string roundName = detail.Substring(11, detail.Length - 12);
                        if (_roundNameReplacer.TryGetValue(roundName, out string newName)) {
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
                    } else {
                        if (detail.IndexOf("> Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            questKudos = int.Parse(detail.Substring(9));
                            //> Fame:, > Crowns:, > CurrentCrownShards:
                        }
                    }
                }
                temp.Kudos += questKudos;

                if (round.Count > maxRound) {
                    return false;
                }

                logRound.Info = round[round.Count - 1];
                DateTime showEnd = logRound.Info.End;
                for (int i = 0; i < round.Count; i++) {
                    round[i].ShowEnd = showEnd;
                }
                if (logRound.Info.Qualified) {
                    logRound.Info.Position = 1;
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
