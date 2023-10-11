using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        public bool CurrentlyInParty;
        public bool PrivateLobby;
        public bool FindingPosition;
        public bool IsFinal;
        public bool HasIsFinal;
        public string CurrentPlayerID;
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
        public bool autoChangeProfile;
        public bool preventOverlayMouseClicks;
        public bool isDisplayPing;
        private string selectedShowId;
        private bool useShareCode;
        private string sessionId;
        private bool toggleRequestCountryInfoApi;
        private bool toggleFgdbCreativeApi;
        
        private string creativeShareCode;
        private string creativeOnlinePlatformId;
        private string creativeAuthor;
        private int creativeVersion;
        private string creativeStatus;
        private string creativeTitle;
        private string creativeDescription;
        private int creativeMaxPlayer;
        private string creativePlatformId;
        private DateTime creativeLastModifiedDate;
        private int creativePlayCount;
        private int creativeQualificationPercent;
        private int creativeTimeLimitSeconds;
        
        private Ping pingSender = new Ping();
        private PingReply pingReply;
        private readonly object pingCheckLock = new object();
        private readonly object fgdbCreativeApiLock = new object();
        
        public event Action<List<RoundInfo>> OnParsedLogLines;
        public event Action<List<RoundInfo>> OnParsedLogLinesCurrent;
        public event Action<DateTime> OnNewLogFileDate;
        public event Action<string> OnError;

        public void Start(string logDirectory, string fileName) {
            if (this.running) { return; }

            this.filePath = Path.Combine(logDirectory, fileName);
            this.prevFilePath = Path.Combine(logDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}-prev.log");
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
            string currentFilePath = this.prevFilePath;
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
                                    
                                    if (Stats.OnlineServiceType == Stats.OnlineServiceTypes.None && line.IndexOf("Discovering subsystems at path", StringComparison.OrdinalIgnoreCase) >= 0) {
                                        string subsystemsPath = line.Substring(44);
                                        string[] userInfo;
                                        if (subsystemsPath.IndexOf("steamapps", StringComparison.OrdinalIgnoreCase) >= 0) {
                                            Stats.OnlineServiceType = Stats.OnlineServiceTypes.Steam;
                                            userInfo = this.StatsForm.FindSteamNickname();
                                        } else {
                                            Stats.OnlineServiceType = Stats.OnlineServiceTypes.EpicGames;
                                            userInfo = this.StatsForm.FindEpicGamesNickname();
                                        }
                                        
                                        if (!string.IsNullOrEmpty(userInfo[0]) && !string.IsNullOrEmpty(userInfo[1])) {
                                            Stats.OnlineServiceId = userInfo[0];
                                            Stats.OnlineServiceNickname = userInfo[1];
                                        }
                                    }

                                    if (logLine.IsValid) {
                                        int index;
                                        if ((index = line.IndexOf("[GlobalGameStateClient].PreStart called at ")) >= 0) {
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

                                        if (line.IndexOf(" == [CompletedEpisodeDto] ==") >= 0 || line.IndexOf("[FNMMSClientRemoteService] Message received: ") >= 0 ) {
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
                                    }
                                    // else if (logLine.Line.IndexOf("Client address: ", StringComparison.OrdinalIgnoreCase) >= 0) {
                                    //     tempLines.Add(logLine);
                                    // }
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
                            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) >= 0
                                       //|| line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateMainMenu with FGClient.StatePrivateLobby", StringComparison.OrdinalIgnoreCase) >= 0
                                       //|| line.Line.IndexOf("[StateDisconnectingFromServer] Shutting down game and resetting scene to reconnect", StringComparison.OrdinalIgnoreCase) >= 0
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) >= 0
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateReloadingToMainMenu with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                                       || line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                                       || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) >= 0) {
                                offset = i > 0 ? tempLines[i - 1].Offset : offset;
                                lastDate = line.Date;
                            } else if (line.Line.IndexOf("[HandleSuccessfulLogin] Selected show is", StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (this.autoChangeProfile && Stats.InShow && !Stats.EndedShow) {
                                    this.StatsForm.SetLinkedProfileMenu(this.selectedShowId, logRound.PrivateLobby, this.StatsForm.IsCreativeShow(this.selectedShowId));
                                }
                            } else if (line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (this.preventOverlayMouseClicks && Stats.InShow && !Stats.EndedShow) {
                                    this.StatsForm.PreventOverlayMouseClicks();
                                }
                            }
// #if AllowUpdate
//                             TimeSpan timeDiff = DateTime.UtcNow - this.StatsForm.timeSwitcherForCheckUpdate;
//                             if (timeDiff.TotalMinutes >= 30) {
//                                 Task.Run(() => {
//                                     if (this.StatsForm.IsInternetConnected()) {
//                                         this.StatsForm.timeSwitcherForCheckUpdate = DateTime.UtcNow;
//                                         this.StatsForm.CheckForNewVersion();
//                                     }
//                                 });
//                             }
// #endif
                        }

                        //if (logRound.LastPing > 0) {
                        //    Stats.LastServerPing = logRound.LastPing;
                        //}
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
            {"round_follow-the-leader_ss2_launch", "round_follow-the-leader_s6_launch"},
            {"round_follow-the-leader_ss2_parrot", "round_follow-the-leader_s6_launch"},
            
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
            
            {"round_thin_ice_only_finals_v2_r1", "round_thin_ice"},
            {"round_thin_ice_only_finals_v2_r2", "round_thin_ice"},
            {"round_thin_ice_only_finals_v2_r3_r4", "round_thin_ice"},
            {"round_thin_ice_only_finals_v2_final", "round_thin_ice"},
            
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
            
            // Squad Celebration
            {"s10_pixelperfect_squads_squadcelebration", "round_pixelperfect_almond"},
            {"s10_snowyscrap_squads_squadcelebration", "round_snowy_scrap"},
            {"s10_conveyorarena_squads_squadcelebration", "round_conveyor_arena"},
            {"s10_basketfall_squads_squadcelebration_final", "round_basketfall_s4_show"},
            {"s10_fallball_squads_squadcelebration_final", "round_fall_ball_60_players"},
            {"s10_jinxed_squads_squadcelebration_final", "round_jinxed"},
            
            // Thin Ice Trials
            {"round_thin_ice_event_only_01", "round_thin_ice"},
            {"round_thin_ice_event_only_02", "round_thin_ice"},
            {"round_thin_ice_event_only_final", "round_thin_ice"},
            
            // Digi's Shuffle Selection
            // {"mrs_wle_fp4_06_0_01", "current_wle_fp4_06_0_01"},
            // {"mrs_wle_fp4_06_0_02", "current_wle_fp4_06_0_02"},
            // {"mrs_wle_fp4_06_0_04", "current_wle_fp4_06_0_04"},
            // {"mrs_wle_fp4_06_1_02", "current_wle_fp4_06_1_02"},
            // {"mrs_wle_fp4_06_1_03", "current_wle_fp4_06_1_03"},
            // {"mrs_wle_fp4_06_1_06", "current_wle_fp4_06_1_06"},
            // {"mrs_wle_fp4_07_01", "current_wle_fp4_07_01"},
            // {"mrs_wle_fp4_07_02", "current_wle_fp4_07_02"},
            // {"mrs_wle_fp4_07_04", "current_wle_fp4_07_04"},
            // {"mrs_wle_fp4_07_05", "current_wle_fp4_07_05"},
            // {"mrs_wle_fp4_07_06", "current_wle_fp4_07_06"},
            // {"mrs_wle_fp4_08_0_01", "current_wle_fp4_08_0_01"},
            // {"mrs_wle_fp4_08_0_02", "current_wle_fp4_08_0_02"},
            // {"mrs_wle_fp4_08_0_04", "current_wle_fp4_08_0_04"},
            // {"mrs_wle_fp4_08_0_05", "current_wle_fp4_08_0_05"},
            // {"mrs_wle_fp4_08_1_02", "current_wle_fp4_08_1_02"},
            // {"mrs_wle_fp4_08_1_04", "current_wle_fp4_08_1_04"},
            // {"mrs_wle_fp4_08_3_01", "current_wle_fp4_08_3_01"},
            // {"mrs_wle_fp4_09_01", "current_wle_fp4_09_01"},
            // {"mrs_wle_fp4_09_02", "current_wle_fp4_09_02"},
            // {"mrs_wle_fp4_09_06", "current_wle_fp4_09_06"},
            // {"mrs_wle_fp4_09_1_01", "current_wle_fp4_09_1_01"},
            // {"mrs_wle_fp4_09_1_02", "current_wle_fp4_09_1_02"},
            // {"mrs_wle_fp4_09_2_01", "current_wle_fp4_09_2_01"},
            // {"mrs_wle_fp4_10_04", "current_wle_fp4_10_04"},
            // {"mrs_wle_fp4_10_05", "current_wle_fp4_10_05"},
            // {"mrs_wle_s10_player_round_wk6_14", "wle_s10_player_round_wk6_14"},
            // {"mrs_wle_s10_player_round_wk6_15", "wle_s10_player_round_wk6_15"},
            // {"mrs_wle_fp4_09_03", "current_wle_fp4_09_03"},
            // {"mrs_wle_fp4_10_01", "current_wle_fp4_10_01"},
            // {"mrs_wle_fp4_10_02", "current_wle_fp4_10_02"},
            // {"mrs_wle_fp4_10_03", "current_wle_fp4_10_03"},
            // {"mrs_wle_fp4_09_04", "current_wle_fp4_09_04"},
            // {"mrs_wle_fp4_09_05", "current_wle_fp4_09_05"},
            // {"mrs_wle_fp4_10_06", "current_wle_fp4_10_06"},
            // {"mrs_wle_fp4_10_07", "current_wle_fp4_10_07"},
            // {"mrs_wle_fp4_10_08", "current_wle_fp4_10_08"},
            // {"mrs_wle_fp4_08_0_03", "current_wle_fp4_08_0_03"},
            // {"mrs_wle_fp4_08_0_06", "current_wle_fp4_08_0_06"},
            // {"mrs_wle_fp4_08_0_07", "current_wle_fp4_08_0_07"},
            // {"mrs_wle_fp4_08_1_01", "current_wle_fp4_08_1_01"},
            // {"mrs_wle_fp4_08_1_03", "current_wle_fp4_08_1_03"},
            // {"mrs_wle_fp4_07_03", "current_wle_fp4_07_03"},
            // {"mrs_wle_fp4_07_07", "current_wle_fp4_07_07"},
            // {"mrs_wle_fp4_06_0_03", "current_wle_fp4_06_0_03"},
            // {"mrs_wle_fp4_06_0_05", "current_wle_fp4_06_0_05"},
            // {"mrs_wle_fp4_06_1_01", "current_wle_fp4_06_1_01"},
            // {"mrs_wle_fp4_06_1_04", "current_wle_fp4_06_1_04"},
            // {"mrs_wle_fp4_06_1_05", "current_wle_fp4_06_1_05"},
        };
        private readonly Dictionary<string, string> _sceneNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "FallGuy_FollowTheLeader_UNPACKED", "FallGuy_FollowTheLeader" }, { "FallGuy_BlueJay_UNPACKED", "FallGuy_BlueJay" }
        };
        private string ReplaceCreativeLevel(string showId, string roundId) {
            if (showId.Equals("wle_mrs_shuffle_show")) {
                if (roundId.StartsWith("mrs_wle_fp")) {
                    return $"current{roundId.Substring(3)}";
                } else if (roundId.StartsWith("mrs_wle_s10_player_round_wk")) {
                    return roundId.Substring(4);
                }
            }
            return roundId;
        }
        private bool IsCreativeFinalRound(string showId, string roundId) {
            return (((showId.StartsWith("show_wle_s10_wk") && !showId.EndsWith("_mrs")) ||
                     showId.StartsWith("wle_s10_player_round_wk") ||
                     showId.StartsWith("show_wle_s10_") ||
                     showId.StartsWith("current_wle_fp") ||
                     showId.StartsWith("wle_s10_cf_round_"))
                    
                    || (showId.StartsWith("wle_mrs_bagel") && roundId.StartsWith("wle_mrs_bagel_final"))
                    || showId.StartsWith("wle_mrs_shuffle_show")
                    
                    || (roundId.IndexOf("wle_s10_orig_round_010", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_011", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_017", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_018", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_024", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_025", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_030", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_orig_round_031", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_round_004", StringComparison.OrdinalIgnoreCase) != -1 ||
                        roundId.IndexOf("wle_s10_round_009", StringComparison.OrdinalIgnoreCase) != -1)
                );
        }

        private bool IsRealFinalRound(string roundId) {
            return (roundId.IndexOf("round_jinxed", StringComparison.OrdinalIgnoreCase) != -1
                    && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1)

                   || (roundId.IndexOf("round_fall_ball", StringComparison.OrdinalIgnoreCase) != -1
                       && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                       && roundId.IndexOf("_cup_only", StringComparison.OrdinalIgnoreCase) == -1)

                   || ((roundId.IndexOf("round_basketfall", StringComparison.OrdinalIgnoreCase) != -1
                        || roundId.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1)
                            && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase))

                   || ((roundId.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) != -1
                        || roundId.IndexOf("round_robotrampage", StringComparison.OrdinalIgnoreCase) != -1)
                            && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase))

                   || roundId.EndsWith("_timeattack_final", StringComparison.OrdinalIgnoreCase)

                   || roundId.EndsWith("_xtreme_party_final", StringComparison.OrdinalIgnoreCase)
                   
                   || (roundId.IndexOf("_squads_squadcelebration", StringComparison.OrdinalIgnoreCase) != -1
                       && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase));
        }
        
        private bool IsModeException(string roundId) {
            return roundId.IndexOf("round_lava_event_only_slime_climb", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_kraken_attack_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_blastball_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_floor_fall_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_hexsnake_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_jump_showdown_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_hexaring_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_tunnel_final_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_thin_ice_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_drumtop_event_only", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_floor_fall_event_only", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_floor_fall_event_only_low_grav", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_floor_fall_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_hexaring_event_only", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_hexaring_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_hexsnake_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_kraken_attack_event_only_survival", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_thin_ice_event_only", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_blastball_arenasurvival_blast_ball_trials", StringComparison.OrdinalIgnoreCase) != -1
                   || roundId.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool IsModeFinalException(string roundId) {
            return ((roundId.IndexOf("round_lava_event_only_slime_climb", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_kraken_attack_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_blastball_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_floor_fall_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_hexsnake_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_jump_showdown_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_hexaring_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_tunnel_final_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_thin_ice_only_finals", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_drumtop_event_only", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_floor_fall_event_only", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_floor_fall_event_only_low_grav", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_floor_fall_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_hexaring_event_only", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_hexaring_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_hexsnake_event_walnut", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_kraken_attack_event_only_survival", StringComparison.OrdinalIgnoreCase) != -1
                     || roundId.IndexOf("round_thin_ice_event_only", StringComparison.OrdinalIgnoreCase) != -1)
                         && roundId.EndsWith("_final"))

                     || (roundId.IndexOf("round_blastball_arenasurvival_blast_ball_trials", StringComparison.OrdinalIgnoreCase) != -1
                         && roundId.EndsWith("_fn"))

                     || (roundId.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1
                         && roundId.EndsWith("_03"));
        }
        
        private bool IsTeamException(string roundId) {
            return roundId.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1
                   && (roundId.IndexOf("_duos", StringComparison.OrdinalIgnoreCase) != -1
                       || roundId.IndexOf("_squads", StringComparison.OrdinalIgnoreCase) != -1);
        }

        private void InitStaticVariable() {
            Stats.LastServerPing = 0;
            Stats.IsBadPing = false;
            Stats.LastCountryAlpha2Code = string.Empty;
            Stats.IsPrePlaying = false;
            Stats.IsPlaying = false;
            Stats.PingSwitcher = 10;
            Stats.IsQueued = false;
            Stats.QueuedPlayers = 0;
        }

        private void InitCreativeLevelVariable() {
            this.creativeOnlinePlatformId = null;
            this.creativeAuthor = null;
            this.creativeShareCode = null;
            this.creativeVersion = 0;
            this.creativeStatus = null;
            this.creativeTitle = null;
            this.creativeDescription = null;
            this.creativeMaxPlayer = 0;
            this.creativePlatformId = null;
            this.creativeLastModifiedDate = DateTime.MinValue;
            this.creativePlayCount = 0;
            this.creativeQualificationPercent = 0;
            this.creativeTimeLimitSeconds = 0;
        }

        private bool ParseLine(LogLine line, List<RoundInfo> round, LogRound logRound) {
            int index;
            if (!Stats.IsPrePlaying && line.Line.IndexOf("[FNMMSClientRemoteService] Message received: ", StringComparison.OrdinalIgnoreCase) >= 0) {
                string detail;
                StringReader sr = new StringReader(line.Line);
                while ((detail = sr.ReadLine()) != null) {
                    string content;
                    if ((index = detail.IndexOf("\"name\": ", StringComparison.OrdinalIgnoreCase)) >= 0) {
                        content = Regex.Replace(detail.Substring(index + 8), "[\",]", "");
                        if (string.Equals("Play", content)) {
                            Stats.IsQueued = false;
                        }
                        // else if (string.Equals("StatusUpdate", content)) {
                        //     
                        // }
                    } else if ((index = detail.IndexOf("\"queuedPlayers\": ", StringComparison.OrdinalIgnoreCase)) >= 0) {
                        content = Regex.Replace(detail.Substring(index + 17), "[\",]", "");
                        if (!string.Equals("null", content)) {
                            if (int.TryParse(content, out int queuedPlayers)) {
                                Stats.QueuedPlayers = queuedPlayers;
                            }
                        }
                    } else if ((index = detail.IndexOf("\"state\": ", StringComparison.OrdinalIgnoreCase)) >= 0) {
                        content = Regex.Replace(detail.Substring(index + 9), "[\",]", "");
                        if (string.Equals("Queued", content)) {
                            Stats.IsQueued = true;
                        } else if (string.Equals("SessionAssignment", content)) {
                            Stats.IsQueued = false;
                        }
                    }
                }
            } else if (this.isDisplayPing && Stats.InShow && !Stats.EndedShow && line.Line.IndexOf("[StateConnectToGame] We're connected to the server! Host = ", StringComparison.OrdinalIgnoreCase) >= 0) {
                TimeSpan timeDiff = DateTime.UtcNow - line.Date;
                if (timeDiff.TotalMinutes <= 40) {
                    lock (this.pingCheckLock) {
                        string host = line.Line.Substring(line.Line.IndexOf("Host = ") + 7);
                        string ip = host.Substring(0, host.IndexOf(":"));
                        if (Stats.PingSwitcher++ % 10 == 0) {
                            Stats.PingSwitcher = 1;
                            byte[] bufferArray = new byte[32];
                            int timeout = 1000;
                            try {
                                Task.Run(() => {
                                    if (Stats.IsClientRunning()) {
                                        this.pingReply = pingSender.Send(ip, timeout, bufferArray);
                                        if (this.pingReply.Status == IPStatus.Success) {
                                            Stats.LastServerPing = this.pingReply.RoundtripTime;
                                            Stats.IsBadPing = false;
                                        } else {
                                            Stats.LastServerPing = this.pingReply.RoundtripTime;
                                            Stats.IsBadPing = true;
                                        }
                                    } else {
                                        Stats.LastServerPing = 0;
                                        Stats.IsBadPing = false;
                                    }
                                });

                                if (!this.toggleRequestCountryInfoApi) {
                                    this.toggleRequestCountryInfoApi = true;
                                    Task.Run(() => {
                                        if (Stats.IsClientRunning()) {
                                            try {
                                                Stats.LastCountryAlpha2Code = this.StatsForm.GetIpToCountryCode(ip).ToLower();
                                                
                                                if (this.StatsForm.CurrentSettings.NotifyServerConnected && !string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                                                    this.StatsForm.ShowNotification(Multilingual.GetWord("message_connected_to_server_caption"),
                                                        $"{Multilingual.GetWord("message_connected_to_server_prefix")}{Multilingual.GetCountryName(Stats.LastCountryAlpha2Code)}{Multilingual.GetWord("message_connected_to_server_suffix")}",
                                                        System.Windows.Forms.ToolTipIcon.Info, 2000);
                                                }

                                                if (string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                                                    this.toggleRequestCountryInfoApi = false;
                                                }
                                            } catch {
                                                this.toggleRequestCountryInfoApi = false;
                                                Stats.LastCountryAlpha2Code = string.Empty;
                                            }
                                        } else {
                                            Stats.LastCountryAlpha2Code = string.Empty;
                                        }
                                    });
                                }
                            } catch {
                                Stats.LastServerPing = 0;
                                Stats.IsBadPing = true;
                            }
                        }
                    }
                }
            } else if (line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateConnectToGame with FGClient.StateConnectionAuthentication", StringComparison.OrdinalIgnoreCase) >= 0) {
                Stats.IsPrePlaying = true;
            } else if ((index = line.Line.IndexOf("[HandleSuccessfulLogin] Session: ", StringComparison.OrdinalIgnoreCase)) >= 0) {
                //Store SessionID to prevent duplicates
                this.sessionId = line.Line.Substring(index + 33);
            } else if ((index = line.Line.IndexOf("[HandleSuccessfulLogin] Selected show is", StringComparison.OrdinalIgnoreCase)) >= 0) {
                this.selectedShowId = line.Line.Substring(line.Line.Length - (line.Line.Length - index - 41));
                if (this.selectedShowId.StartsWith("ugc-")) {
                    this.selectedShowId = this.selectedShowId.Substring(4);
                    this.useShareCode = true;
                } else {
                    this.useShareCode = false;
                }
                Stats.IsPrePlaying = true;
            } else if ((index = line.Line.IndexOf("[StateGameLoading] Loading game level scene", StringComparison.OrdinalIgnoreCase)) >= 0) {
                logRound.Info = new RoundInfo { ShowNameId = this.selectedShowId, SessionId = this.sessionId, UseShareCode = this.useShareCode};
                if (logRound.Info.UseShareCode) {
                    logRound.Info.SceneName = "FallGuy_UseShareCode";
                    TimeSpan timeDiff = DateTime.UtcNow - line.Date;
                    if (timeDiff.TotalMinutes <= 15 && this.StatsForm.IsInternetConnected()) {
                        lock (this.fgdbCreativeApiLock) {
                            if (!this.toggleFgdbCreativeApi) {
                                this.toggleFgdbCreativeApi = true;
                                try {
                                    JsonElement resData = this.StatsForm.GetApiData(this.StatsForm.FALLGUYSDB_API_URL, $"creative/{logRound.Info.ShowNameId}.json").GetProperty("data").GetProperty("snapshot");
                                    JsonElement versionMetadata = resData.GetProperty("version_metadata");
                                    this.creativeOnlinePlatformId = this.StatsForm.FindCreativeAuthor(resData.GetProperty("author").GetProperty("name_per_platform"))[0];
                                    this.creativeAuthor = this.StatsForm.FindCreativeAuthor(resData.GetProperty("author").GetProperty("name_per_platform"))[1];
                                    this.creativeShareCode = resData.GetProperty("share_code").GetString();
                                    this.creativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                    this.creativeStatus = versionMetadata.GetProperty("status").GetString();
                                    this.creativeTitle = versionMetadata.GetProperty("title").GetString();
                                    this.creativeDescription = versionMetadata.GetProperty("description").GetString();
                                    this.creativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                    this.creativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                    this.creativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                    this.creativePlayCount = resData.GetProperty("play_count").GetInt32();
                                    this.creativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                    this.creativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                    Task.Run(() => { this.StatsForm.UpdateUserCreativeLevel(logRound.Info.ShowNameId, resData); });
                                } catch (System.Net.WebException we) {
                                    if (we.Status == System.Net.WebExceptionStatus.ProtocolError) {
                                        RoundInfo ri = this.StatsForm.GetRoundInfoFromShareCode(logRound.Info.ShowNameId);
                                        if (ri != null && !string.IsNullOrEmpty(ri.CreativeTitle)) {
                                            this.creativeOnlinePlatformId = ri.CreativePlatformId;
                                            this.creativeAuthor = ri.CreativeAuthor;
                                            this.creativeShareCode = ri.CreativeShareCode;
                                            this.creativeVersion = ri.CreativeVersion;
                                            this.creativeStatus = ri.CreativeStatus;
                                            this.creativeTitle = ri.CreativeTitle;
                                            this.creativeDescription = ri.CreativeDescription;
                                            this.creativeMaxPlayer = ri.CreativeMaxPlayer;
                                            this.creativePlatformId = ri.CreativePlatformId;
                                            this.creativeLastModifiedDate = ri.CreativeLastModifiedDate;
                                            this.creativePlayCount = ri.CreativePlayCount;
                                            this.creativeQualificationPercent = ri.CreativeQualificationPercent;
                                            this.creativeTimeLimitSeconds = ri.CreativeTimeLimitSeconds;
                                        } else {
                                            this.toggleFgdbCreativeApi = false;
                                            this.InitCreativeLevelVariable();
                                        }
                                    } else {
                                        this.toggleFgdbCreativeApi = false;
                                        this.InitCreativeLevelVariable();
                                    }
                                } catch (Exception ex) {
                                    this.toggleFgdbCreativeApi = false;
                                    this.InitCreativeLevelVariable();
                                }
                            }
                        }
                    }
                } else {
                    int index2 = line.Line.IndexOf(' ', index + 44);
                    if (index2 < 0) { index2 = line.Line.Length; }
                    logRound.Info.SceneName = line.Line.Substring(index + 44, index2 - index - 44);
                    if (_sceneNameReplacer.TryGetValue(logRound.Info.SceneName, out string newName)) {
                        logRound.Info.SceneName = newName;
                    }
                }
                logRound.FindingPosition = false;
                round.Add(logRound.Info);
            } else if (logRound.Info != null && (index = line.Line.IndexOf("[StateGameLoading] Finished loading game level", StringComparison.OrdinalIgnoreCase)) >= 0) {
                int index2 = line.Line.IndexOf(". ", index + 62);
                if (index2 < 0) { index2 = line.Line.Length; }
                if (logRound.Info.UseShareCode) {
                    logRound.Info.Name = "user_creative_race_round";
                    logRound.Info.CreativeShareCode = this.creativeShareCode;
                    logRound.Info.CreativeOnlinePlatformId = this.creativeOnlinePlatformId;
                    logRound.Info.CreativeAuthor = this.creativeAuthor;
                    logRound.Info.CreativeVersion = this.creativeVersion;
                    logRound.Info.CreativeStatus = this.creativeStatus;
                    logRound.Info.CreativeTitle = this.creativeTitle;
                    logRound.Info.CreativeDescription = this.creativeDescription;
                    logRound.Info.CreativeMaxPlayer = this.creativeMaxPlayer;
                    logRound.Info.CreativePlatformId = this.creativePlatformId;
                    logRound.Info.CreativeLastModifiedDate = this.creativeLastModifiedDate;
                    logRound.Info.CreativePlayCount = this.creativePlayCount;
                    logRound.Info.CreativeQualificationPercent = this.creativeQualificationPercent;
                    logRound.Info.CreativeTimeLimitSeconds = this.creativeTimeLimitSeconds;
                } else {
                    logRound.Info.Name = line.Line.Substring(index + 62, index2 - index - 62);
                }
                
                if (this.IsCreativeFinalRound(this.selectedShowId, logRound.Info.Name) || logRound.Info.UseShareCode) {
                    logRound.Info.IsFinal = true;
                } else if (this.IsRealFinalRound(logRound.Info.Name)) {
                    logRound.Info.IsFinal = true;
                } else if (this.IsModeException(logRound.Info.Name)) {
                    logRound.Info.IsFinal = this.IsModeFinalException(logRound.Info.Name);
                } else {
                    logRound.Info.IsFinal = logRound.IsFinal || 
                                            (!logRound.HasIsFinal
                                             && LevelStats.SceneToRound.TryGetValue(logRound.Info.SceneName, out string roundName)
                                             && LevelStats.ALL.TryGetValue(roundName, out LevelStats levelStats) && levelStats.IsFinal);
                }
                logRound.Info.IsTeam = this.IsTeamException(logRound.Info.Name);

                if (_roundNameReplacer.TryGetValue(logRound.Info.Name, out string newName)) {
                    logRound.Info.Name = newName;
                } else {
                    logRound.Info.Name = this.ReplaceCreativeLevel(logRound.Info.ShowNameId, logRound.Info.Name);
                }
                logRound.Info.Round = round.Count;
                logRound.Info.Start = line.Date;
                logRound.Info.InParty = logRound.CurrentlyInParty;
                logRound.Info.PrivateLobby = logRound.PrivateLobby;
                logRound.Info.GameDuration = logRound.Duration;
                logRound.CountingPlayers = true;
            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) >= 0 || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) >= 0) {
                logRound.PrivateLobby = line.Line.IndexOf("StatePrivateLobby", StringComparison.OrdinalIgnoreCase) >= 0;
                logRound.CurrentlyInParty = !logRound.PrivateLobby && (line.Line.IndexOf("solo", StringComparison.OrdinalIgnoreCase) == -1);
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                }
                logRound.CountingPlayers = false;
                logRound.FindingPosition = false;
                Stats.InShow = true;
                round.Clear();
                logRound.Info = null;
            } else if ((index = line.Line.IndexOf("NetworkGameOptions: durationInSeconds=", StringComparison.OrdinalIgnoreCase)) >= 0) { // legacy code // It seems to have been deleted from the log file now.
                int nextIndex = line.Line.IndexOf(" ", index + 38);
                logRound.Duration = int.Parse(line.Line.Substring(index + 38, nextIndex - index - 38));
                index = line.Line.IndexOf("isFinalRound=", StringComparison.OrdinalIgnoreCase);
                logRound.HasIsFinal = index > 0;
                index = line.Line.IndexOf("isFinalRound=True", StringComparison.OrdinalIgnoreCase);
                logRound.IsFinal = index > 0;
            } else if (logRound.Info != null && logRound.CountingPlayers &&
                       (line.Line.IndexOf("[ClientGameManager] Finalising spawn", StringComparison.OrdinalIgnoreCase) >= 0 || line.Line.IndexOf("[ClientGameManager] Added player ", StringComparison.OrdinalIgnoreCase) >= 0)) {
                logRound.Info.Players++;
            } else if (logRound.Info != null && logRound.CountingPlayers && (line.Line.IndexOf("[CameraDirector] Adding Spectator target", StringComparison.OrdinalIgnoreCase) >= 0)) {
                if (line.Line.IndexOf("ps4", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersPs4++;
                } else if (line.Line.IndexOf("ps5", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersPs5++;
                } else if (line.Line.IndexOf("xb1", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersXb1++;
                } else if (line.Line.IndexOf("xsx", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersXsx++;
                } else if (line.Line.IndexOf("switch", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersSw++;
                } else if (line.Line.IndexOf("win", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersPc++;
                } else if (line.Line.IndexOf("bots", StringComparison.OrdinalIgnoreCase) >= 0) {
                    logRound.Info.PlayersBots++;
                } else {
                    logRound.Info.PlayersEtc++;
                }
            } else if (line.Line.IndexOf("[ClientGameManager] Handling bootstrap for local player FallGuy", StringComparison.OrdinalIgnoreCase) >= 0 && (index = line.Line.IndexOf("playerID = ", StringComparison.OrdinalIgnoreCase)) >= 0) {
                int prevIndex = line.Line.IndexOf(',', index + 11);
                logRound.CurrentPlayerID = line.Line.Substring(index + 11, prevIndex - index - 11);
            } else if (logRound.Info != null && line.Line.IndexOf($"HandleServerPlayerProgress PlayerId={logRound.CurrentPlayerID} is succeeded=", StringComparison.OrdinalIgnoreCase) >= 0) {
                index = line.Line.IndexOf("succeeded=True", StringComparison.OrdinalIgnoreCase);
                if (index > 0) {
                    logRound.Info.Finish = logRound.Info.End == DateTime.MinValue ? line.Date : logRound.Info.End;
                }
                logRound.FindingPosition = true;
            } else if (logRound.Info != null && logRound.FindingPosition && (index = line.Line.IndexOf("[ClientGameSession] NumPlayersAchievingObjective=")) >= 0) {
                int position = int.Parse(line.Line.Substring(index + 49));
                if (position > 0) {
                    logRound.FindingPosition = false;
                    logRound.Info.Position = position;
                }
            }
            //else if (logRound.Info != null && line.Line.IndexOf("Client address: ", StringComparison.OrdinalIgnoreCase) >= 0) {
            //    index = line.Line.IndexOf("RTT: ");
            //    if (index > 0) {
            //        int msIndex = line.Line.IndexOf("ms", index);
            //        logRound.LastPing = long.Parse(line.Line.Substring(index + 5, msIndex - index - 5));
            //    }
            //} else if (logRound.Info != null && line.Line.IndexOf("[GameSession] Changing state from Precountdown to Countdown", StringComparison.OrdinalIgnoreCase) >= 0) {
            //}
            else if (logRound.Info != null &&
                       line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) >= 0) {
                Stats.IsPlaying = true;
                logRound.Info.Start = line.Date;
                logRound.Info.Playing = true;
                logRound.CountingPlayers = false;
            } else if (logRound.Info != null && (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver", StringComparison.OrdinalIgnoreCase) >= 0)) {
                        //|| line.Line.IndexOf("Changing local player state to: SpectatingEliminated", StringComparison.OrdinalIgnoreCase) >= 0
                        //|| line.Line.IndexOf("[GlobalGameStateClient] SwitchToDisconnectingState", StringComparison.OrdinalIgnoreCase) >= 0
                        //|| line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0))
                        //|| line.Line.IndexOf("[ClientGlobalGameState] Client has been disconnected", StringComparison.OrdinalIgnoreCase) >= 0
                // if (line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0) { logRound.PrivateLobby = false; }
                if (logRound.Info.End == DateTime.MinValue) {
                    logRound.Info.End = line.Date;
                }
                logRound.Info.Playing = false;
            } else if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                       || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) >= 0
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateReloadingToMainMenu with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) >= 0
                       || line.Line.IndexOf("[StateDisconnectingFromServer] Shutting down game and resetting scene to reconnect", StringComparison.OrdinalIgnoreCase) >= 0) {
                logRound.PrivateLobby = false;
                logRound.FindingPosition = false;
                logRound.CountingPlayers = false;
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                    if (!Stats.EndedShow) {
                        DateTime showStart = DateTime.MinValue;
                        DateTime showEnd = logRound.Info.End;
                        for (int i = 0; i < round.Count; i++) {
                            if (string.IsNullOrEmpty(round[i].Name)) {
                                if (i != 0) {
                                    round[i - 1].Qualified = false;
                                }
                                round.RemoveAt(i);
                                logRound.Info = null;
                                Stats.InShow = false;
                                Stats.EndedShow = true;
                                return true;
                            }
                            
                            round[i].VerifyName();
                            if (i == 0) {
                                showStart = round[i].Start;
                            }
                            round[i].ShowStart = showStart;
                            round[i].Playing = false;
                            round[i].Round = i + 1;
                            if (round[i].End == DateTime.MinValue) {
                                round[i].End = line.Date;
                            }
                            if (round[i].Start == DateTime.MinValue) {
                                round[i].Start = round[i].End;
                            }
                            if (i < (round.Count - 1)) {
                                round[i].Qualified = true;
                                round[i].IsAbandon = true;
                            } else if (round[i].UseShareCode && round[i].Finish.HasValue) {
                                if (round[i].Position > 0) {
                                    double rankPercentage = (Convert.ToDouble(round[i].Position) / Convert.ToDouble(round[i].Players)) * 100d;
                                    if (round[i].Position == 1) {
                                        round[i].Tier = 1; //gold
                                    } else if (round[i].Position == 2 || rankPercentage <= 20d) {
                                        round[i].Tier = 2; //silver
                                    } else if (rankPercentage <= 50d) {
                                        round[i].Tier = 3; //bronze
                                    } else if (rankPercentage > 50d) {
                                        round[i].Tier = 0; //pink
                                    }
                                    round[i].Qualified = true;
                                    round[i].Crown = true;
                                } else {
                                    round[i].Tier = 0;
                                    round[i].Qualified = false;
                                    round[i].Crown = false;
                                    round[i].Finish = null;
                                }
                            } else {
                                round[i].IsAbandon = true;
                            }
                            round[i].ShowEnd = showEnd;
                        }
                        logRound.Info = null;
                        this.InitStaticVariable();
                        this.toggleRequestCountryInfoApi = false;
                        this.toggleFgdbCreativeApi = false;
                        Stats.InShow = false;
                        Stats.EndedShow = true;
                        return true;
                    }
                }
                logRound.Info = null;
                this.InitStaticVariable();
                this.toggleRequestCountryInfoApi = false;
                this.toggleFgdbCreativeApi = false;
                Stats.InShow = false;
            } else if (line.Line.IndexOf("[GameSession] Changing state from GameOver to Results", StringComparison.OrdinalIgnoreCase) >= 0) {
                if (logRound.Info == null || !logRound.Info.UseShareCode) { return false; }
                if (round.Count > 0) {
                    foreach (RoundInfo temp in round) {
                        temp.Playing = false;
                        temp.ShowStart = temp.Start;
                        
                        logRound.PrivateLobby = temp.PrivateLobby;
                        logRound.CurrentlyInParty = temp.InParty;
                        
                        if (temp.End == DateTime.MinValue) {
                            temp.End = line.Date;
                        }
                        if (temp.Start == DateTime.MinValue) {
                            temp.Start = temp.End;
                        }
                        if (!temp.Finish.HasValue) {
                            temp.Finish = temp.End;
                        }
                        
                        DateTime showEnd = temp.End;
                        temp.ShowEnd = showEnd;
                        
                        if (temp.Finish.HasValue) {
                            if (temp.Position > 0) {
                                double rankPercentage = (Convert.ToDouble(temp.Position) / Convert.ToDouble(temp.Players)) * 100d;
                                if (temp.Position == 1) {
                                    temp.Tier = 1; //gold
                                } else if (temp.Position == 2 || rankPercentage <= 20d) {
                                    temp.Tier = 2; //silver
                                } else if (rankPercentage <= 50d) {
                                    temp.Tier = 3; //bronze
                                } else if (rankPercentage > 50d) {
                                    temp.Tier = 0; //pink
                                }
                                temp.Qualified = true;
                                temp.Crown = true;
                            } else {
                                temp.Tier = 0;
                                temp.Qualified = false;
                                temp.Crown = false;
                                temp.Finish = null;
                            }
                        } else {
                            temp.Tier = 0;
                            temp.Qualified = false;
                            temp.Crown = false;
                        }
                    }
                    
                    logRound.Info = null;
                    this.InitStaticVariable();
                    this.toggleRequestCountryInfoApi = false;
                    this.toggleFgdbCreativeApi = false;
                    Stats.InShow = false;
                    Stats.EndedShow = true;
                    return true;
                }
            } else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==", StringComparison.OrdinalIgnoreCase) >= 0) {
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
                        } else {
                            roundName = this.ReplaceCreativeLevel(logRound.Info.ShowNameId, logRound.Info.Name);
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
                    logRound.Info.Crown = true;
                }
                logRound.Info = null;
                this.InitStaticVariable();
                Stats.InShow = false;
                Stats.EndedShow = true;
                return true;
            }
            return false;
        }
    }
}
