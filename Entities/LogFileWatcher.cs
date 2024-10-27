using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            bool isValidSemiColon = line.IndexOf(":") == 2 && line.IndexOf(":", 3) == 5 && line.IndexOf(":", 6) == 12;
            bool isValidDot = line.IndexOf(".") == 2 && line.IndexOf(".", 3) == 5 && line.IndexOf(":", 6) == 12;
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
        public bool CurrentlyInParty;
        public bool PrivateLobby;
        public bool InLoadingGameScreen;
        public bool IsRoundPreloaded;
        public bool CountingPlayers;
        public bool GetCurrentPlayerID;
        public bool FindingPosition;
        public string CurrentPlayerID;

        public RoundInfo Info;
    }

    public class ThreadLocalData {
        public string selectedShowId;
        public bool useShareCode;
        public string currentSessionId;
        public string sceneName;

        public bool toggleCountryInfoApi;
        public string creativeShareCode;
        public string creativeOnlinePlatformId;
        public string creativeAuthor;
        public int creativeVersion;
        public string creativeStatus;
        public string creativeTitle;
        public string creativeDescription;
        public string creativeCreatorTags;
        public int creativeMaxPlayer;
        public string creativeThumbUrl;
        public string creativePlatformId;
        public string creativeGameModeId;
        public string creativeLevelThemeId;
        public DateTime creativeLastModifiedDate;
        public int creativePlayCount;
        public int creativeLikes;
        public int creativeDislikes;
        public int creativeQualificationPercent;
        public int creativeTimeLimitSeconds;
    }

    public class LogFileWatcher {
        private const int UpdateDelay = 500;

        private string filePath;
        private string prevFilePath;
        private List<LogLine> logLines = new List<LogLine>();
        private bool running;
        private bool stop;
        private Task logFileWatcher, logLineParser;

        public Stats StatsForm { get; set; }
        
        private readonly ThreadLocal<ThreadLocalData> threadLocalVariable = new ThreadLocal<ThreadLocalData>(() => new ThreadLocalData());
        public event Action<List<RoundInfo>> OnParsedLogLines;
        public event Action<List<RoundInfo>> OnParsedLogLinesCurrent;
        public event Action<DateTime> OnNewLogFileDate;
        public event Action OnServerConnectionNotification;
        public event Action<string, string, TimeSpan, TimeSpan> OnPersonalBestNotification;
        public event Action<string> OnError;

        private readonly ServerPingWatcher serverPingWatcher = new ServerPingWatcher();
        private readonly GameStateWatcher gameStateWatcher = new GameStateWatcher();

        public void Start(string logDirectory, string fileName) {
            if (this.running) { return; }

            this.filePath = Path.Combine(logDirectory, fileName);
            this.prevFilePath = Path.Combine(logDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}-prev.log");
            this.stop = false;
            this.logFileWatcher = new Task(this.ReadLogFile);
            this.logFileWatcher.Start();
            this.logLineParser = new Task(this.ParseLines);
            this.logLineParser.Start();
        }

        public async Task Stop() {
            this.stop = true;
            while (this.running || this.logFileWatcher == null || this.logFileWatcher.Status == TaskStatus.Created) {
                await Task.Delay(50);
            }
            lock (this.logLines) {
                this.logLines = new List<LogLine>();
            }
            
            await Task.Run(() => this.logFileWatcher?.Wait());
            await Task.Run(() => this.logLineParser?.Wait());
            
            // await this.gameStateWatcher.Stop();
            // await this.serverPingWatcher.Stop();
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

                                    if (line.IndexOf("Discovering subsystems at path", StringComparison.OrdinalIgnoreCase) != -1) {
                                        string subsystemsPath = line.Substring(44);
                                        string[] userInfo;
                                        if (subsystemsPath.IndexOf("steamapps", StringComparison.OrdinalIgnoreCase) != -1) {
                                            Stats.OnlineServiceType = OnlineServiceTypes.Steam;
                                            userInfo = this.StatsForm.FindSteamUserInfo();
                                        } else {
                                            Stats.OnlineServiceType = OnlineServiceTypes.EpicGames;
                                            userInfo = this.StatsForm.FindEpicGamesUserInfo();
                                        }
                                        
                                        Stats.OnlineServiceId = userInfo[0];
                                        Stats.OnlineServiceNickname = userInfo[1];
                                        this.StatsForm.SetSecretKey();
                                        this.StatsForm.SetLeaderboardTitle();
                                    }

                                    if (logLine.IsValid) {
                                        int index;
                                        if ((index = line.IndexOf("[GlobalGameStateClient].PreStart called at ")) != -1) {
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

                                        if (line.IndexOf(" == [CompletedEpisodeDto] ==") != -1) {
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
                                        } else if (line.IndexOf("[FNMMSClientRemoteService] Message received: {") != -1) {
                                            while ((line = sr.ReadLine()) != null) {
                                                if (line.IndexOf("\"queuedPlayers\": ") != -1) {
                                                    string content = Regex.Replace(line.Substring(21), "[\",]", "");
                                                    if (!string.Equals(content, "null") &&
                                                        int.TryParse(content, out int queuedPlayers)) {
                                                        Stats.IsQueued = true;
                                                        Stats.QueuedPlayers = queuedPlayers;
                                                        break;
                                                    }
                                                } else if (line.IndexOf("\"name\": ") != -1) {
                                                    string content = Regex.Replace(line.Substring(10), "[\",]", "");
                                                    if (string.Equals(content, "Play")) {
                                                        Stats.IsQueued = false;
                                                        Stats.QueuedPlayers = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                        } else {
                                            tempLines.Add(logLine);
                                        }
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
                                Stats.SavedRoundCount = 0;
                                lastDate = line.Date;
                                offset = line.Offset;
                                lock (this.logLines) {
                                    this.logLines.AddRange(currentLines);
                                    currentLines.Clear();
                                }
                            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobbyMinimal with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateReloadingToMainMenu with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateDisconnectingFromServer with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) != -1
                                       || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) != -1
                                       || (this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId) && (line.Line.IndexOf("[GameplaySpectatorUltimatePartyFlowViewModel] Begin countdown", StringComparison.OrdinalIgnoreCase) != -1
                                                                                                                           || line.Line.IndexOf("[GlobalGameStateClient] Received instruction that server is ending a round", StringComparison.OrdinalIgnoreCase) != -1))) {
                                if (line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) != -1
                                    || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) != -1) {
                                    Stats.CasualRoundNum = 0;
                                }
                                offset = i > 0 ? tempLines[i - 1].Offset : offset;
                                lastDate = line.Date;
                            } else if (this.StatsForm.CurrentSettings.AutoChangeProfile && line.Line.IndexOf("[HandleSuccessfulLogin] Selected show is", StringComparison.OrdinalIgnoreCase) != -1) {
                                if (Stats.InShow && !Stats.EndedShow) {
                                    this.StatsForm.SetLinkedProfileMenu(this.threadLocalVariable.Value.selectedShowId, logRound.PrivateLobby);
                                }
                            } else if (this.StatsForm.CurrentSettings.PreventOverlayMouseClicks && line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) != -1) {
                                if (Stats.InShow && !Stats.EndedShow) {
                                    this.StatsForm.PreventOverlayMouseClicks();
                                }
                            }
                        }

                        this.OnParsedLogLinesCurrent?.Invoke(round);
                    }

                    if (!completed) {
                        completed = true;
                        offset = 0;
                        currentFilePath = this.filePath;
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
                    lock (this.logLines) {
                        foreach (LogLine line in this.logLines) {
                            if (this.ParseLine(line, round, logRound)) {
                                allStats.AddRange(round);
                            }
                        }

                        if (allStats.Count > 0) {
                            this.OnParsedLogLines?.Invoke(allStats);
                            allStats.Clear();
                        }

                        this.logLines.Clear();
                    }
                } catch (Exception ex) {
                    this.OnError?.Invoke(ex.ToString());
                }
                Thread.Sleep(UpdateDelay);
            }
        }

        private void AddLineAfterClientShutdown() {
            try {
                bool isValidSemiColon, isValidDot, isValid;
                string lastTime = DateTime.UtcNow.ToString("hh:mm:ss.fff");
                foreach (string line in File.ReadAllLines(this.filePath)) {
                    isValidSemiColon = line.IndexOf(":") == 2 && line.IndexOf(":", 3) == 5 && line.IndexOf(":", 6) == 12;
                    isValidDot = line.IndexOf(".") == 2 && line.IndexOf(".", 3) == 5 && line.IndexOf(":", 6) == 12;
                    isValid = isValidSemiColon || isValidDot;
                    if (isValid) {
                        lastTime = line.Substring(0, 12);
                    }
                }
                TextWriter tw = new StreamWriter(this.filePath, true);
                tw.WriteLine();
                tw.WriteLine($"{lastTime}: [EOSPartyPlatformService.Base] Reset, reason: Shutdown");
                tw.WriteLine();
                tw.Close();
            } catch {
                // ignored
            }
        }

        private readonly Dictionary<string, string> _sceneNameReplacer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "FallGuy_FollowTheLeader_UNPACKED", "FallGuy_FollowTheLeader" },
            { "FallGuy_BlueJay_UNPACKED", "FallGuy_BlueJay" }
        };

        private bool IsShowIsCasualShow(string showId) {
            return string.Equals(showId, "casual_show")
                   || string.Equals(showId, "explore_points")
                   || string.Equals(showId, "no_elimination_explore")
                   || string.Equals(showId, "xtreme_explore");
        }

        private bool IsRealFinalRound(int roundNum, string roundId, string showId) {
            if ((showId.StartsWith("knockout_fp") && showId.EndsWith("_srs"))
                 || (showId.StartsWith("show_wle_s10_") && showId.IndexOf("_srs", StringComparison.OrdinalIgnoreCase) != -1)
                 || showId.IndexOf("wle_s10_player_round_", StringComparison.OrdinalIgnoreCase) != -1
                 || showId.StartsWith("wle_mrs_shuffle_")
                 || showId.StartsWith("wle_shuffle_")
                 || showId.StartsWith("current_wle_fp")
                 || showId.StartsWith("wle_s10_cf_round_")
                 || string.Equals(showId, "wle_playful_shuffle")
                 || (showId.StartsWith("event_") && showId.EndsWith("_fools") && roundId.StartsWith("wle_shuffle_"))) {
                return true;
            }

            return (roundId.IndexOf("round_jinxed", StringComparison.OrdinalIgnoreCase) != -1
                    && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                    && !string.Equals(showId, "event_anniversary_season_1_alternate_name"))

                    || (roundId.IndexOf("round_fall_ball", StringComparison.OrdinalIgnoreCase) != -1
                        && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                        && roundId.IndexOf("_cup_only", StringComparison.OrdinalIgnoreCase) == -1
                        && roundId.IndexOf("_teamgames", StringComparison.OrdinalIgnoreCase) == -1
                        && !string.Equals(showId, "event_anniversary_season_1_alternate_name"))

                    || (roundId.IndexOf("round_territory", StringComparison.OrdinalIgnoreCase) != -1
                        && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                        && roundId.IndexOf("_teamgames", StringComparison.OrdinalIgnoreCase) == -1)

                    || (roundId.IndexOf("round_basketfall", StringComparison.OrdinalIgnoreCase) != -1
                        && roundId.IndexOf("_non_final", StringComparison.OrdinalIgnoreCase) == -1
                        && (roundId.EndsWith("_duos", StringComparison.OrdinalIgnoreCase)
                            || roundId.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1))

                    || (roundId.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1
                        && (roundId.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1
                            || roundId.IndexOf("_teamgames", StringComparison.OrdinalIgnoreCase) != -1))

                    || ((roundId.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) != -1
                         || roundId.IndexOf("round_robotrampage", StringComparison.OrdinalIgnoreCase) != -1)
                            && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase))

                    || roundId.EndsWith("_timeattack_final", StringComparison.OrdinalIgnoreCase)

                    || roundId.EndsWith("_xtreme_party_final", StringComparison.OrdinalIgnoreCase)

                    || (roundId.IndexOf("_squads_squadcelebration", StringComparison.OrdinalIgnoreCase) != -1
                        && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase))

                    || (string.Equals(showId, "event_only_hoverboard_template")
                        && string.Equals(roundId, "round_hoverboardsurvival_final"))

                    || (string.Equals(showId, "event_only_button_bashers_template")
                        && roundNum == 4)

                    || (string.Equals(showId, "no_elimination_show")
                        && (string.Equals(roundId, "round_snowballsurvival_final_noelim") || string.Equals(roundId, "round_robotrampage_arena_2_final_noelim")));
        }

        private bool IsModeException(string roundId, string showId) {
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
                   || roundId.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1
                   || string.Equals(showId, "event_blast_ball_banger_template")
                   || string.Equals(showId, "ftue_uk_show")
                   || showId.StartsWith("knockout_");
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
                        && roundId.EndsWith("_final", StringComparison.OrdinalIgnoreCase))

                     || (roundId.IndexOf("round_blastball_arenasurvival_blast_ball_trials", StringComparison.OrdinalIgnoreCase) != -1
                         && roundId.EndsWith("_fn", StringComparison.OrdinalIgnoreCase))

                     || (roundId.IndexOf("round_robotrampage_arena_2_ss2_show1", StringComparison.OrdinalIgnoreCase) != -1
                         && roundId.EndsWith("_03", StringComparison.OrdinalIgnoreCase))

                     || string.Equals(roundId, "round_blastball_arenasurvival_blast_ball_banger")

                     || string.Equals(roundId, "round_snowballsurvival_noelim_ftue_s2")

                     || (!string.Equals(roundId, "knockout_fp10_final_8")
                         && roundId.StartsWith("knockout_", StringComparison.OrdinalIgnoreCase)
                         && (roundId.EndsWith("_opener_4", StringComparison.OrdinalIgnoreCase)
                             || roundId.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1));
        }

        private bool IsTeamException(string roundId) {
            return roundId.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) != -1
                   && (roundId.IndexOf("_duos", StringComparison.OrdinalIgnoreCase) != -1
                       || roundId.IndexOf("_squads", StringComparison.OrdinalIgnoreCase) != -1);
        }

        private void SetDefaultCreativeLevelVariables() {
            this.threadLocalVariable.Value.creativeOnlinePlatformId = string.Empty;
            this.threadLocalVariable.Value.creativeAuthor = string.Empty;
            this.threadLocalVariable.Value.creativeShareCode = string.Empty;
            this.threadLocalVariable.Value.creativeVersion = 0;
            this.threadLocalVariable.Value.creativeStatus = string.Empty;
            this.threadLocalVariable.Value.creativeTitle = string.Empty;
            this.threadLocalVariable.Value.creativeDescription = string.Empty;
            this.threadLocalVariable.Value.creativeCreatorTags = string.Empty;
            this.threadLocalVariable.Value.creativeMaxPlayer = 0;
            this.threadLocalVariable.Value.creativeThumbUrl = string.Empty;
            this.threadLocalVariable.Value.creativePlatformId = string.Empty;
            this.threadLocalVariable.Value.creativeGameModeId = string.Empty;
            this.threadLocalVariable.Value.creativeLevelThemeId = string.Empty;
            this.threadLocalVariable.Value.creativeLastModifiedDate = DateTime.MinValue;
            this.threadLocalVariable.Value.creativePlayCount = 0;
            this.threadLocalVariable.Value.creativeLikes = 0;
            this.threadLocalVariable.Value.creativeDislikes = 0;
            this.threadLocalVariable.Value.creativeQualificationPercent = 0;
            this.threadLocalVariable.Value.creativeTimeLimitSeconds = 0;
        }

        private void SetCreativeLevelVariables(string shareCode) {
            RoundInfo ri = this.StatsForm.GetRoundInfoFromShareCode(shareCode);
            if (ri != null) {
                this.threadLocalVariable.Value.creativeOnlinePlatformId = ri.CreativePlatformId;
                this.threadLocalVariable.Value.creativeAuthor = ri.CreativeAuthor;
                // this.threadLocalVariable.Value.creativeShareCode = ri.CreativeShareCode;
                this.threadLocalVariable.Value.creativeVersion = ri.CreativeVersion;
                this.threadLocalVariable.Value.creativeStatus = ri.CreativeStatus;
                this.threadLocalVariable.Value.creativeTitle = ri.CreativeTitle;
                this.threadLocalVariable.Value.creativeDescription = ri.CreativeDescription;
                this.threadLocalVariable.Value.creativeCreatorTags = ri.CreativeCreatorTags;
                this.threadLocalVariable.Value.creativeMaxPlayer = ri.CreativeMaxPlayer;
                this.threadLocalVariable.Value.creativeThumbUrl = ri.CreativeThumbUrl;
                this.threadLocalVariable.Value.creativePlatformId = ri.CreativePlatformId;
                this.threadLocalVariable.Value.creativeGameModeId = ri.CreativeGameModeId;
                this.threadLocalVariable.Value.creativeLevelThemeId = ri.CreativeLevelThemeId;
                this.threadLocalVariable.Value.creativeLastModifiedDate = ri.CreativeLastModifiedDate;
                this.threadLocalVariable.Value.creativePlayCount = ri.CreativePlayCount;
                this.threadLocalVariable.Value.creativeLikes = ri.CreativeLikes;
                this.threadLocalVariable.Value.creativeDislikes = ri.CreativeDislikes;
                this.threadLocalVariable.Value.creativeQualificationPercent = ri.CreativeQualificationPercent;
                this.threadLocalVariable.Value.creativeTimeLimitSeconds = ri.CreativeTimeLimitSeconds;
                return;
            }
            bool isSucceed = false;
            TimeSpan timeDiff = DateTime.UtcNow - Stats.ConnectedToServerDate;
            if (timeDiff.TotalMinutes <= 15 && Utils.IsInternetConnected()) {
                try {
                    JsonElement resData = Utils.GetApiData(Utils.FALLGUYSDB_API_URL, $"creative/{shareCode}.json");
                    JsonElement je = resData.GetProperty("data");
                    JsonElement snapshot = je.GetProperty("snapshot");
                    JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                    JsonElement stats = snapshot.GetProperty("stats");
                    string[] onlinePlatformInfo = this.StatsForm.FindUserCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                    this.threadLocalVariable.Value.creativeOnlinePlatformId = onlinePlatformInfo[0];
                    this.threadLocalVariable.Value.creativeAuthor = onlinePlatformInfo[1];
                    // this.threadLocalVariable.Value.creativeShareCode = snapshot.GetProperty("share_code").GetString();
                    this.threadLocalVariable.Value.creativeVersion = versionMetadata.GetProperty("version").GetInt32();
                    this.threadLocalVariable.Value.creativeStatus = versionMetadata.GetProperty("status").GetString();
                    this.threadLocalVariable.Value.creativeTitle = versionMetadata.GetProperty("title").GetString();
                    this.threadLocalVariable.Value.creativeDescription = versionMetadata.GetProperty("description").GetString();
                    if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                        string temps = string.Empty;
                        foreach (JsonElement t in creatorTags.EnumerateArray()) {
                            if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                            temps += t.GetString();
                        }
                        this.threadLocalVariable.Value.creativeCreatorTags = temps;
                    }
                    this.threadLocalVariable.Value.creativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                    this.threadLocalVariable.Value.creativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                    this.threadLocalVariable.Value.creativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                    this.threadLocalVariable.Value.creativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString() ?? "GAMEMODE_GAUNTLET";
                    this.threadLocalVariable.Value.creativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString() ?? "THEME_VANILLA";
                    this.threadLocalVariable.Value.creativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                    this.threadLocalVariable.Value.creativePlayCount = stats.GetProperty("play_count").GetInt32();
                    this.threadLocalVariable.Value.creativeLikes = stats.GetProperty("likes").GetInt32();
                    this.threadLocalVariable.Value.creativeDislikes = stats.GetProperty("dislikes").GetInt32();
                    this.threadLocalVariable.Value.creativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                    this.threadLocalVariable.Value.creativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                    // Task.Run(() => { this.StatsForm.UpdateCreativeLevels(string.Empty, shareCode, snapshot); });
                    isSucceed = true;
                } catch {
                    try {
                        JsonElement resData = Utils.GetApiData(Utils.FGANALYST_API_URL, $"creative/?share_code={shareCode}");
                        JsonElement je = resData.GetProperty("level_data");
                        JsonElement levelData = je[0];
                        JsonElement versionMetadata = levelData.GetProperty("version_metadata");
                        JsonElement stats = levelData.GetProperty("stats");
                        string[] onlinePlatformInfo = this.StatsForm.FindUserCreativeAuthor(levelData.GetProperty("author").GetProperty("name_per_platform"));
                        this.threadLocalVariable.Value.creativeOnlinePlatformId = onlinePlatformInfo[0];
                        this.threadLocalVariable.Value.creativeAuthor = onlinePlatformInfo[1];
                        // this.threadLocalVariable.Value.creativeShareCode = levelData.GetProperty("share_code").GetString();
                        this.threadLocalVariable.Value.creativeVersion = versionMetadata.GetProperty("version").GetInt32();
                        this.threadLocalVariable.Value.creativeStatus = versionMetadata.GetProperty("status").GetString();
                        this.threadLocalVariable.Value.creativeTitle = versionMetadata.GetProperty("title").GetString();
                        this.threadLocalVariable.Value.creativeDescription = versionMetadata.GetProperty("description").GetString();
                        if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                            string temps = string.Empty;
                            foreach (JsonElement t in creatorTags.EnumerateArray()) {
                                if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                                temps += t.GetString();
                            }
                            this.threadLocalVariable.Value.creativeCreatorTags = temps;
                        }
                        this.threadLocalVariable.Value.creativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                        this.threadLocalVariable.Value.creativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                        this.threadLocalVariable.Value.creativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                        this.threadLocalVariable.Value.creativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString() ?? "GAMEMODE_GAUNTLET";
                        this.threadLocalVariable.Value.creativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString() ?? "THEME_VANILLA";
                        this.threadLocalVariable.Value.creativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                        this.threadLocalVariable.Value.creativePlayCount = stats.GetProperty("play_count").GetInt32();
                        this.threadLocalVariable.Value.creativeLikes = stats.GetProperty("likes").GetInt32();
                        this.threadLocalVariable.Value.creativeDislikes = stats.GetProperty("dislikes").GetInt32();
                        this.threadLocalVariable.Value.creativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                        this.threadLocalVariable.Value.creativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                        // Task.Run(() => { this.StatsForm.UpdateCreativeLevels(string.Empty, shareCode, levelData); });
                        isSucceed = true;
                    } catch {
                        isSucceed = false;
                    }
                }
            }
            
            if (!isSucceed) {
                this.SetDefaultCreativeLevelVariables();
            }
        }

        private void SetCreativeLevelInfo(RoundInfo info) {
            info.CreativeOnlinePlatformId = this.threadLocalVariable.Value.creativeOnlinePlatformId;
            info.CreativeAuthor = this.threadLocalVariable.Value.creativeAuthor;
            info.CreativeShareCode = this.threadLocalVariable.Value.creativeShareCode;
            info.CreativeVersion = this.threadLocalVariable.Value.creativeVersion;
            info.CreativeStatus = this.threadLocalVariable.Value.creativeStatus;
            info.CreativeTitle = this.threadLocalVariable.Value.creativeTitle;
            info.CreativeDescription = this.threadLocalVariable.Value.creativeDescription;
            info.CreativeCreatorTags = this.threadLocalVariable.Value.creativeCreatorTags;
            info.CreativeMaxPlayer = this.threadLocalVariable.Value.creativeMaxPlayer;
            info.CreativeThumbUrl = this.threadLocalVariable.Value.creativeThumbUrl;
            info.CreativePlatformId = this.threadLocalVariable.Value.creativePlatformId;
            info.CreativeGameModeId = this.threadLocalVariable.Value.creativeGameModeId;
            info.CreativeLevelThemeId = this.threadLocalVariable.Value.creativeLevelThemeId;
            info.CreativeLastModifiedDate = this.threadLocalVariable.Value.creativeLastModifiedDate;
            info.CreativePlayCount = this.threadLocalVariable.Value.creativePlayCount;
            info.CreativeLikes = this.threadLocalVariable.Value.creativeLikes;
            info.CreativeDislikes = this.threadLocalVariable.Value.creativeDislikes;
            info.CreativeQualificationPercent = this.threadLocalVariable.Value.creativeQualificationPercent;
            info.CreativeTimeLimitSeconds = this.threadLocalVariable.Value.creativeTimeLimitSeconds;
        }

        private void SetCountryCodeByIp(string ip) {
            if (this.threadLocalVariable.Value.toggleCountryInfoApi || !Utils.IsProcessRunning("FallGuys_client_game")) { return; }
            this.threadLocalVariable.Value.toggleCountryInfoApi = true;
            Stats.LastCountryAlpha2Code = string.Empty;
            Stats.LastCountryRegion = string.Empty;
            Stats.LastCountryCity = string.Empty;
            try {
                string ci = Utils.GetCountryInfo(ip);
                if (!string.IsNullOrEmpty(ci)) {
                    string[] countryInfo = ci.Split(';');
                    Stats.LastCountryAlpha2Code = countryInfo[0].ToLower();
                    Stats.LastCountryRegion = !string.Equals(countryInfo[1].ToLower(), "unknown") ? countryInfo[1] : string.Empty;
                    Stats.LastCountryCity = !string.Equals(countryInfo[2].ToLower(), "unknown") ? countryInfo[2] : string.Empty;
                } else {
                    string countryCode = Utils.GetCountryCode(ip);
                    Stats.LastCountryAlpha2Code = !string.IsNullOrEmpty(countryCode) ? countryCode.ToLower() : string.Empty;
                    Stats.LastCountryRegion = string.Empty;
                    Stats.LastCountryCity = string.Empty;
                }
            } catch {
                this.threadLocalVariable.Value.toggleCountryInfoApi = false;
                Stats.LastCountryAlpha2Code = string.Empty;
                Stats.LastCountryRegion = string.Empty;
                Stats.LastCountryCity = string.Empty;
            }
        }

        private void ResetVariablesUsedForOverlay() {
            Stats.QueuedPlayers = 0;
            Stats.IsQueued = false;
            Stats.IsConnectedToServer = false;
            Stats.LastServerPing = 0;
            Stats.IsBadServerPing = false;
            Stats.LastCountryAlpha2Code = string.Empty;
            Stats.LastCountryRegion = string.Empty;
            Stats.LastCountryCity = string.Empty;
            this.threadLocalVariable.Value.toggleCountryInfoApi = false;
            this.SetDefaultCreativeLevelVariables();
        }

        private void UpdateServerConnectionLog(string session, string show) {
            if (!this.StatsForm.ExistsServerConnectionLog(session)) {
                this.StatsForm.InsertServerConnectionLog(session, show, Stats.LastServerIp, Stats.ConnectedToServerDate, true, true);
                this.serverPingWatcher.Start();
                this.SetCountryCodeByIp(Stats.LastServerIp);
                if (!Stats.IsClientHasBeenClosed && this.StatsForm.CurrentSettings.NotifyServerConnected && !string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                    this.OnServerConnectionNotification?.Invoke();
                }
            } else {
                ServerConnectionLog serverConnectionLog = this.StatsForm.SelectServerConnectionLog(session);
                if (!serverConnectionLog.IsNotify) {
                    if (!Stats.IsClientHasBeenClosed && this.StatsForm.CurrentSettings.NotifyServerConnected && !string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                        this.OnServerConnectionNotification?.Invoke();
                    }
                }

                if (serverConnectionLog.IsPlaying) {
                    this.serverPingWatcher.Start();
                    this.SetCountryCodeByIp(Stats.LastServerIp);
                }
            }
        }

        private void UpdatePersonalBestLog(RoundInfo info) {
            if (info.PrivateLobby || (!info.IsCasualShow && info.UseShareCode) || !info.Finish.HasValue) { return; }

            if (info.IsCasualShow && string.Equals(info.ShowNameId, "user_creative_race_round")) {
                if (string.IsNullOrEmpty(info.Name)) { return; }

                if (!this.StatsForm.ExistsPersonalBestLog(info.Finish.Value)) {
                    string levelName = string.IsNullOrEmpty(info.CreativeTitle) ? this.StatsForm.GetUserCreativeLevelTitle(info.Name) : info.CreativeTitle;
                    List<RoundInfo> roundInfoList = this.StatsForm.AllStats.FindAll(r => r.PrivateLobby == false && r.Finish.HasValue && !string.IsNullOrEmpty(r.ShowNameId) && !string.IsNullOrEmpty(r.SessionId) && string.Equals(r.Name, info.Name));

                    TimeSpan currentRecord = info.Finish.Value - info.Start;
                    TimeSpan existingRecord = roundInfoList.Count > 0 ? roundInfoList.Min(r => r.Finish.Value - r.Start) : TimeSpan.MaxValue;

                    this.StatsForm.InsertPersonalBestLog(info.Finish.Value, info.SessionId, "casual_show", info.Name, currentRecord.TotalMilliseconds, currentRecord < existingRecord);
                    if (this.StatsForm.CurrentSettings.NotifyPersonalBest && currentRecord < existingRecord) {
                        this.OnPersonalBestNotification?.Invoke("casual_show", levelName, existingRecord, currentRecord);
                    }
                }
            } else {
                string levelId = info.VerifiedName();
                if (!this.StatsForm.StatLookup.TryGetValue(levelId, out LevelStats currentLevel) || currentLevel.Type != LevelType.Race) {
                    return;
                }

                if (!this.StatsForm.ExistsPersonalBestLog(info.Finish.Value)) {
                    List<RoundInfo> roundInfoList;
                    if (currentLevel.IsCreative && !string.IsNullOrEmpty(currentLevel.ShareCode)) {
                        string[] ids = this.StatsForm.StatDetails.Where(l => string.Equals(l.ShareCode, currentLevel.ShareCode)).Select(l => l.Id).ToArray();
                        roundInfoList = this.StatsForm.AllStats.FindAll(r => r.PrivateLobby == false && r.Finish.HasValue && !string.IsNullOrEmpty(r.ShowNameId) && !string.IsNullOrEmpty(r.SessionId) && ids.Contains(r.Name));
                    } else {
                        roundInfoList = this.StatsForm.AllStats.FindAll(r => r.PrivateLobby == false && r.Finish.HasValue && !string.IsNullOrEmpty(r.ShowNameId) && !string.IsNullOrEmpty(r.SessionId) && string.Equals(r.Name, levelId));
                    }

                    TimeSpan currentRecord = info.Finish.Value - info.Start;
                    TimeSpan existingRecord = roundInfoList.Count > 0 ? roundInfoList.Min(r => r.Finish.Value - r.Start) : TimeSpan.MaxValue;

                    this.StatsForm.InsertPersonalBestLog(info.Finish.Value, info.SessionId, info.ShowNameId, levelId, currentRecord.TotalMilliseconds, currentRecord < existingRecord);
                    if (this.StatsForm.CurrentSettings.NotifyPersonalBest && currentRecord < existingRecord) {
                        this.OnPersonalBestNotification?.Invoke(info.ShowNameId, levelId, existingRecord, currentRecord);
                    }
                }
            }
        }

        private bool ParseLine(LogLine line, List<RoundInfo> round, LogRound logRound) {
            int index;
            if (this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId) && (line.Line.IndexOf("[MatchmakeWhileInGameHandler] Cancel matchmaking, reason: Cancel", StringComparison.OrdinalIgnoreCase) != -1
                                                                                           || line.Line.IndexOf("[GameplaySpectatorUltimatePartyFlowViewModel] Stop previous countdown", StringComparison.OrdinalIgnoreCase) != -1)) {
                Stats.InShow = false;
                this.ResetVariablesUsedForOverlay();
            } else if ((!this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId) && line.Line.IndexOf("[StateDisconnectingFromServer] Shutting down game and resetting scene to reconnect", StringComparison.OrdinalIgnoreCase) != -1)
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateDisconnectingFromServer with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1) {
                this.StatsForm.UpdateServerConnectionLog(this.threadLocalVariable.Value.currentSessionId, false);
                Stats.InShow = false;
                this.ResetVariablesUsedForOverlay();
            } else if (line.Line.IndexOf("[StateMatchmaking] Begin", StringComparison.OrdinalIgnoreCase) != -1
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) != -1
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobbyMinimal with FGClient.StateConnectToGame", StringComparison.OrdinalIgnoreCase) != -1) {
                if (line.Date > Stats.LastGameStart) {
                    Stats.LastGameStart = line.Date;
                    if (logRound.Info != null) {
                        if (logRound.Info.End == DateTime.MinValue) {
                            logRound.Info.End = line.Date;
                        }
                        logRound.Info.Playing = false;
                        logRound.Info = null;
                    }
                }
                Stats.EndedShow = false;

                logRound.PrivateLobby = line.Line.IndexOf("StatePrivateLobby", StringComparison.OrdinalIgnoreCase) != -1;
                logRound.CurrentlyInParty = !logRound.PrivateLobby && (line.Line.IndexOf("solo", StringComparison.OrdinalIgnoreCase) == -1);
                logRound.InLoadingGameScreen = false;
                logRound.CountingPlayers = false;
                logRound.GetCurrentPlayerID = false;
                logRound.FindingPosition = false;

                round.Clear();
            } else if (!Stats.EndedShow && line.Line.IndexOf("[FG_UnityInternetNetworkManager] Client connected to Server", StringComparison.OrdinalIgnoreCase) != -1) {
                if (!Stats.IsConnectedToServer) {
                    Stats.IsConnectedToServer = true;
                    Stats.ConnectedToServerDate = line.Date;
                    int ipIndex = line.Line.IndexOf("IP:", StringComparison.OrdinalIgnoreCase) + 3;
                    Stats.LastServerIp = line.Line.Substring(ipIndex);

                }
            } else if ((index = line.Line.IndexOf("[HandleSuccessfulLogin] Selected show is ", StringComparison.OrdinalIgnoreCase)) != -1) {
                int index2 = line.Line.IndexOf(" IsUltimatePartyEpisode:");
                this.threadLocalVariable.Value.selectedShowId = index2 != -1 ? line.Line.Substring(index + 41, index2 - (index + 41)) : line.Line.Substring(index + 41);
                if (string.Equals(this.threadLocalVariable.Value.selectedShowId, "casual_show")) {
                    this.threadLocalVariable.Value.useShareCode = true;
                } else if (this.threadLocalVariable.Value.selectedShowId.StartsWith("ugc-")) {
                    this.threadLocalVariable.Value.selectedShowId = this.threadLocalVariable.Value.selectedShowId.Substring(4);
                    this.threadLocalVariable.Value.useShareCode = true;
                } else {
                    this.threadLocalVariable.Value.useShareCode = false;
                }
            } else if ((index = line.Line.IndexOf("[HandleSuccessfulLogin] Session: ", StringComparison.OrdinalIgnoreCase)) != -1) {
                this.threadLocalVariable.Value.currentSessionId = line.Line.Substring(index + 33);
                if ((DateTime.UtcNow - Stats.ConnectedToServerDate).TotalMinutes <= 40) {
                    this.UpdateServerConnectionLog(this.threadLocalVariable.Value.currentSessionId, this.threadLocalVariable.Value.selectedShowId);
                }
            } else if ((index = line.Line.IndexOf("[StateGameLoading] Created UGC round: ", StringComparison.OrdinalIgnoreCase)) != -1) {
                this.threadLocalVariable.Value.creativeShareCode = line.Line.Substring(index + 42, 14);
                this.threadLocalVariable.Value.useShareCode = true;
            } else if ((index = line.Line.IndexOf("[RoundLoader] LoadGameLevelSceneASync COMPLETE for scene ", StringComparison.OrdinalIgnoreCase)) != -1) {
                if (line.Date > Stats.LastRoundLoad) {
                    Stats.LastRoundLoad = line.Date;
                    Stats.InShow = true;
                    Stats.SucceededPlayerIds.Clear();
                    Stats.EliminatedPlayerIds.Clear();
                    Stats.ReadyPlayerIds.Clear();
                    Stats.NumPlayersSucceeded = 0;
                    Stats.NumPlayersPsSucceeded = 0;
                    Stats.NumPlayersXbSucceeded = 0;
                    Stats.NumPlayersSwSucceeded = 0;
                    Stats.NumPlayersPcSucceeded = 0;
                    Stats.NumPlayersMbSucceeded = 0;
                    Stats.NumPlayersEliminated = 0;
                    Stats.NumPlayersPsEliminated = 0;
                    Stats.NumPlayersXbEliminated = 0;
                    Stats.NumPlayersSwEliminated = 0;
                    Stats.NumPlayersPcEliminated = 0;
                    Stats.NumPlayersMbEliminated = 0;
                    Stats.IsLastRoundRunning = true;
                    Stats.IsLastPlayedRoundStillPlaying = false;
                    Stats.LastPlayedRoundStart = null;
                    Stats.LastPlayedRoundEnd = null;

                    if ((DateTime.UtcNow - Stats.ConnectedToServerDate).TotalMinutes <= 40) {
                        this.gameStateWatcher.Start();
                    }
                }

                int index2 = line.Line.IndexOf(" on frame ");
                this.threadLocalVariable.Value.sceneName = line.Line.Substring(index + 57, index2 - (index + 57));

                if (logRound.InLoadingGameScreen) {
                    logRound.Info = new RoundInfo {
                        ShowNameId = this.threadLocalVariable.Value.selectedShowId, SessionId = this.threadLocalVariable.Value.currentSessionId,
                        UseShareCode = this.threadLocalVariable.Value.useShareCode, IsCasualShow = this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId),
                        OnlineServiceType = (int)Stats.OnlineServiceType, OnlineServiceId = Stats.OnlineServiceId, OnlineServiceNickname = Stats.OnlineServiceNickname
                    };

                    if (logRound.Info.UseShareCode) {
                        if (line.Date > Stats.LastCreativeRoundLoad) {
                            Stats.LastCreativeRoundLoad = line.Date;
                            this.threadLocalVariable.Value.creativeShareCode = logRound.Info.IsCasualShow ? this.threadLocalVariable.Value.creativeShareCode : logRound.Info.ShowNameId;
                            this.SetCreativeLevelVariables(this.threadLocalVariable.Value.creativeShareCode);
                        }
                        logRound.Info.SceneName = this.threadLocalVariable.Value.creativeGameModeId;
                    } else {
                        logRound.Info.SceneName = this._sceneNameReplacer.TryGetValue(this.threadLocalVariable.Value.sceneName, out string sceneName)
                                                  ? sceneName
                                                  : this.threadLocalVariable.Value.sceneName;
                    }
                    logRound.FindingPosition = false;

                    round.Add(logRound.Info);
                } else {
                    logRound.IsRoundPreloaded = true;
                }
            } else if (line.Line.IndexOf("[StateGameLoading] ShowLoadingGameScreenAndLoadLevel", StringComparison.OrdinalIgnoreCase) != -1) {
                logRound.InLoadingGameScreen = true;
                if (logRound.IsRoundPreloaded) {
                    logRound.IsRoundPreloaded = false;
                    logRound.Info = new RoundInfo {
                        ShowNameId = this.threadLocalVariable.Value.selectedShowId, SessionId = this.threadLocalVariable.Value.currentSessionId,
                        UseShareCode = this.threadLocalVariable.Value.useShareCode, IsCasualShow = this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId),
                        OnlineServiceType = (int)Stats.OnlineServiceType, OnlineServiceId = Stats.OnlineServiceId, OnlineServiceNickname = Stats.OnlineServiceNickname
                    };

                    if (logRound.Info.UseShareCode) {
                        if (line.Date > Stats.LastCreativeRoundLoad) {
                            Stats.LastCreativeRoundLoad = line.Date;
                            this.threadLocalVariable.Value.creativeShareCode = logRound.Info.IsCasualShow ? this.threadLocalVariable.Value.creativeShareCode : logRound.Info.ShowNameId;
                            this.SetCreativeLevelVariables(this.threadLocalVariable.Value.creativeShareCode);
                        }
                        logRound.Info.SceneName = this.threadLocalVariable.Value.creativeGameModeId;
                    } else {
                        logRound.Info.SceneName = this._sceneNameReplacer.TryGetValue(this.threadLocalVariable.Value.sceneName, out string sceneName)
                                                  ? sceneName
                                                  : this.threadLocalVariable.Value.sceneName;
                    }
                    logRound.FindingPosition = false;

                    round.Add(logRound.Info);
                }
            } else if (logRound.Info != null && (index = line.Line.IndexOf("[StateGameLoading] Finished loading game level", StringComparison.OrdinalIgnoreCase)) != -1) {
                if (logRound.Info.IsCasualShow) {
                    if (this.threadLocalVariable.Value.currentSessionId != Stats.SavedSessionId) {
                        Stats.SavedSessionId = this.threadLocalVariable.Value.currentSessionId;
                        Stats.CasualRoundNum++;
                    }
                    logRound.Info.Round = 1;
                } else {
                    logRound.Info.Round = !Stats.EndedShow ? round.Count : Stats.SavedRoundCount + round.Count;
                }

                if (logRound.Info.UseShareCode) {
                    logRound.Info.Name = logRound.Info.IsCasualShow ? this.threadLocalVariable.Value.creativeShareCode : logRound.Info.ShowNameId;
                    logRound.Info.ShowNameId = this.StatsForm.GetUserCreativeLevelTypeId(this.threadLocalVariable.Value.creativeGameModeId);
                    this.SetCreativeLevelInfo(logRound.Info);
                } else {
                    int index2 = line.Line.IndexOf(". ", index + 62);
                    if (index2 < 0) { index2 = line.Line.Length; }
                    logRound.Info.Name = this.StatsForm.ReplaceLevelIdInShuffleShow(logRound.Info.ShowNameId ?? this.threadLocalVariable.Value.selectedShowId, line.Line.Substring(index + 62, index2 - index - 62));
                }

                if (logRound.Info.IsCasualShow) {
                    logRound.Info.IsFinal = false;
                } else if (logRound.Info.UseShareCode || this.IsRealFinalRound(logRound.Info.Round, logRound.Info.Name, logRound.Info.ShowNameId)) {
                    logRound.Info.IsFinal = true;
                } else if (this.IsModeException(logRound.Info.Name, logRound.Info.ShowNameId)) {
                    logRound.Info.IsFinal = this.IsModeFinalException(logRound.Info.Name);
                } else if (logRound.Info.Name.StartsWith("wle_s10_") || logRound.Info.Name.StartsWith("wle_mrs_")) {
                    logRound.Info.IsFinal = this.StatsForm.StatLookup.TryGetValue(logRound.Info.Name, out LevelStats levelStats) && levelStats.IsFinal;
                } else {
                    logRound.Info.IsFinal = LevelStats.SceneToRound.TryGetValue(logRound.Info.SceneName, out string levelId) && this.StatsForm.StatLookup.TryGetValue(levelId, out LevelStats levelStats) && levelStats.IsFinal;
                }

                logRound.Info.IsTeam = this.IsTeamException(logRound.Info.Name);

                logRound.Info.Start = line.Date;
                logRound.Info.InParty = logRound.CurrentlyInParty;
                logRound.Info.PrivateLobby = logRound.PrivateLobby;

                logRound.CountingPlayers = true;
                logRound.GetCurrentPlayerID = true;
            } else if (logRound.Info != null && logRound.CountingPlayers && (line.Line.IndexOf("[ClientGameManager] Finalising spawn", StringComparison.OrdinalIgnoreCase) != -1 || line.Line.IndexOf("[ClientGameManager] Added player ", StringComparison.OrdinalIgnoreCase) != -1)) {
                logRound.Info.Players++;
            } else if (logRound.Info != null && logRound.CountingPlayers && line.Line.IndexOf("[CameraDirector] Adding Spectator target", StringComparison.OrdinalIgnoreCase) != -1) {
                string playerId = line.Line.Substring(line.Line.IndexOf(" and playerID: ", StringComparison.OrdinalIgnoreCase) + 15);
                if (line.Line.IndexOf("ps4", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersPs4++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "ps4");
                    }
                } else if (line.Line.IndexOf("ps5", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersPs5++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "ps5");
                    }
                } else if (line.Line.IndexOf("xb1", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersXb1++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "xb1");
                    }
                } else if (line.Line.IndexOf("xsx", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersXsx++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "xsx");
                    }
                } else if (line.Line.IndexOf("switch", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersSw++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "switch");
                    }
                } else if (line.Line.IndexOf("pc", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersPc++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "pc");
                    }
                } else if (line.Line.IndexOf("android", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersAndroid++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "android");
                    }
                } else if (line.Line.IndexOf("ios", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersIos++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "ios");
                    }
                } else if (line.Line.IndexOf("bots", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.PlayersBots++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "bots");
                    }
                } else {
                    logRound.Info.PlayersEtc++;
                    if (!Stats.ReadyPlayerIds.ContainsKey(playerId)) {
                        Stats.ReadyPlayerIds.Add(playerId, "etc");
                    }
                }
            } else if (logRound.Info != null && logRound.GetCurrentPlayerID && line.Line.IndexOf("[ClientGameManager] Handling bootstrap for local player FallGuy", StringComparison.OrdinalIgnoreCase) != -1 && (index = line.Line.IndexOf("playerID = ", StringComparison.OrdinalIgnoreCase)) != -1) {
                logRound.GetCurrentPlayerID = false;
                int prevIndex = line.Line.IndexOf(",", index + 11);
                logRound.CurrentPlayerID = line.Line.Substring(index + 11, prevIndex - index - 11);
            } else if (logRound.Info != null && line.Line.IndexOf("[GameSession] Changing state from Countdown to Playing", StringComparison.OrdinalIgnoreCase) != -1) {
                logRound.Info.Start = line.Date;
                logRound.Info.Playing = true;
                logRound.InLoadingGameScreen = false;
                logRound.CountingPlayers = false;
                logRound.GetCurrentPlayerID = false;
            } else if (logRound.Info != null && line.Line.IndexOf($"HandleServerPlayerProgress PlayerId={logRound.CurrentPlayerID} is succeeded=", StringComparison.OrdinalIgnoreCase) != -1) {
                if (line.Line.IndexOf("succeeded=True", StringComparison.OrdinalIgnoreCase) != -1) {
                    logRound.Info.Finish = logRound.Info.End == DateTime.MinValue ? line.Date : logRound.Info.End;
                    if (line.Date > Stats.LastRoundLoad && !Stats.SucceededPlayerIds.Contains(logRound.CurrentPlayerID)) {
                        Stats.SucceededPlayerIds.Add(logRound.CurrentPlayerID);
                        Stats.NumPlayersSucceeded++;
                        if (Stats.ReadyPlayerIds.TryGetValue(logRound.CurrentPlayerID, out string platformId)) {
                            switch (platformId) {
                                case "ps4":
                                case "ps5":
                                    Stats.NumPlayersPsSucceeded++; break;
                                case "xb1":
                                case "xsx":
                                    Stats.NumPlayersXbSucceeded++; break;
                                case "switch":
                                    Stats.NumPlayersSwSucceeded++; break;
                                case "pc":
                                    Stats.NumPlayersPcSucceeded++; break;
                                case "android":
                                case "ios":
                                    Stats.NumPlayersMbSucceeded++; break;
                            }
                        }
                        this.UpdatePersonalBestLog(logRound.Info);
                    }
                    logRound.FindingPosition = true;
                } else if (line.Line.IndexOf("succeeded=False", StringComparison.OrdinalIgnoreCase) != -1) {
                    if (line.Date > Stats.LastRoundLoad && !Stats.EliminatedPlayerIds.Contains(logRound.CurrentPlayerID)) {
                        Stats.EliminatedPlayerIds.Add(logRound.CurrentPlayerID);
                        Stats.NumPlayersEliminated++;
                        if (Stats.ReadyPlayerIds.TryGetValue(logRound.CurrentPlayerID, out string platformId)) {
                            switch (platformId) {
                                case "ps4":
                                case "ps5":
                                    Stats.NumPlayersPsEliminated++; break;
                                case "xb1":
                                case "xsx":
                                    Stats.NumPlayersXbEliminated++; break;
                                case "switch":
                                    Stats.NumPlayersSwEliminated++; break;
                                case "pc":
                                    Stats.NumPlayersPcEliminated++; break;
                                case "android":
                                case "ios":
                                    Stats.NumPlayersMbEliminated++; break;
                            }
                        }
                    }
                    logRound.FindingPosition = true;
                }
            } else if (logRound.Info != null && !Stats.EndedShow && logRound.FindingPosition && (index = line.Line.IndexOf("[ClientGameSession] NumPlayersAchievingObjective=")) != -1) {
                int position = int.Parse(line.Line.Substring(index + 49));
                if (position > 0) {
                    logRound.FindingPosition = false;
                    logRound.Info.Position = position;
                }
            } else if (line.Date > Stats.LastRoundLoad && (index = line.Line.IndexOf("HandleServerPlayerProgress PlayerId=", StringComparison.OrdinalIgnoreCase)) != -1) {
                if (line.Line.IndexOf("succeeded=True", StringComparison.OrdinalIgnoreCase) != -1) {
                    int prevIndex = line.Line.IndexOf(" ", index + 36);
                    string playerId = line.Line.Substring(index + 36, prevIndex - index - 36);
                    if (!Stats.SucceededPlayerIds.Contains(playerId)) {
                        Stats.SucceededPlayerIds.Add(playerId);
                        Stats.NumPlayersSucceeded++;
                        if (Stats.ReadyPlayerIds.TryGetValue(playerId, out string platformId)) {
                            switch (platformId) {
                                case "ps4":
                                case "ps5":
                                    Stats.NumPlayersPsSucceeded++; break;
                                case "xb1":
                                case "xsx":
                                    Stats.NumPlayersXbSucceeded++; break;
                                case "switch":
                                    Stats.NumPlayersSwSucceeded++; break;
                                case "pc":
                                    Stats.NumPlayersPcSucceeded++; break;
                                case "android":
                                case "ios":
                                    Stats.NumPlayersMbSucceeded++; break;
                            }
                        }
                    }
                } else if (line.Line.IndexOf("succeeded=False", StringComparison.OrdinalIgnoreCase) != -1) {
                    int prevIndex = line.Line.IndexOf(" ", index + 36);
                    string playerId = line.Line.Substring(index + 36, prevIndex - index - 36);
                    if (!Stats.EliminatedPlayerIds.Contains(playerId)) {
                        Stats.EliminatedPlayerIds.Add(playerId);
                        Stats.NumPlayersEliminated++;
                        if (Stats.ReadyPlayerIds.TryGetValue(playerId, out string platformId)) {
                            switch (platformId) {
                                case "ps4":
                                case "ps5":
                                    Stats.NumPlayersPsEliminated++; break;
                                case "xb1":
                                case "xsx":
                                    Stats.NumPlayersXbEliminated++; break;
                                case "switch":
                                    Stats.NumPlayersSwEliminated++; break;
                                case "pc":
                                    Stats.NumPlayersPcEliminated++; break;
                                case "android":
                                case "ios":
                                    Stats.NumPlayersMbEliminated++; break;
                            }
                        }
                    }
                }
            } else if (line.Line.IndexOf("[GameSession] Changing state from Playing to GameOver", StringComparison.OrdinalIgnoreCase) != -1) {
                if (line.Date > Stats.LastRoundLoad) {
                    if (Stats.InShow && Stats.LastPlayedRoundStart.HasValue && !Stats.LastPlayedRoundEnd.HasValue) {
                        Stats.LastPlayedRoundEnd = line.Date;
                    }
                    Stats.IsLastRoundRunning = false;
                    Stats.IsLastPlayedRoundStillPlaying = false;
                }
                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                }
            } else if (line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StatePrivateLobby with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1
                       || line.Line.IndexOf("[GameStateMachine] Replacing FGClient.StateReloadingToMainMenu with FGClient.StateMainMenu", StringComparison.OrdinalIgnoreCase) != -1
                       || line.Line.IndexOf("[StateMainMenu] Loading scene MainMenu", StringComparison.OrdinalIgnoreCase) != -1
                       || line.Line.IndexOf("[EOSPartyPlatformService.Base] Reset, reason: Shutdown", StringComparison.OrdinalIgnoreCase) != -1
                       || (this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId) && (line.Line.IndexOf("[GameplaySpectatorUltimatePartyFlowViewModel] Begin countdown", StringComparison.OrdinalIgnoreCase) != -1
                                                                                                           || line.Line.IndexOf("[GlobalGameStateClient] Received instruction that server is ending a round", StringComparison.OrdinalIgnoreCase) != -1))
                       || Stats.IsClientHasBeenClosed) {
                this.ResetVariablesUsedForOverlay();

                if (Stats.IsClientHasBeenClosed) {
                    Stats.IsClientHasBeenClosed = false;
                    this.AddLineAfterClientShutdown();
                    return false;
                }

                if (Stats.InShow && Stats.LastPlayedRoundStart.HasValue && !Stats.LastPlayedRoundEnd.HasValue) {
                    Stats.LastPlayedRoundEnd = line.Date;
                }
                Stats.IsLastRoundRunning = false;
                Stats.IsLastPlayedRoundStillPlaying = false;

                logRound.InLoadingGameScreen = false;
                logRound.CountingPlayers = false;
                logRound.GetCurrentPlayerID = false;
                logRound.FindingPosition = false;

                if (logRound.Info != null) {
                    if (logRound.Info.End == DateTime.MinValue) {
                        logRound.Info.End = line.Date;
                    }
                    logRound.Info.Playing = false;
                    if (this.IsShowIsCasualShow(this.threadLocalVariable.Value.selectedShowId)) {
                        if (!Stats.EndedShow) {
                            DateTime showEnd = logRound.Info.End;
                            for (int i = 0; i < round.Count; i++) {
                                if (string.IsNullOrEmpty(round[i].Name)) {
                                    round.RemoveAt(i);
                                    logRound.Info = null;
                                    Stats.InShow = false;
                                    Stats.EndedShow = true;
                                    return true;
                                }
                                round[i].VerifyName();
                                round[i].ShowStart = round[i].Start;
                                round[i].Playing = false;
                                round[i].Round = 1;
                                if (round[i].End == DateTime.MinValue) {
                                    round[i].End = line.Date;
                                }
                                if (round[i].Start == DateTime.MinValue) {
                                    round[i].Start = round[i].End;
                                }
                                if (round[i].Finish.HasValue) {
                                    round[i].Qualified = true;
                                }
                                round[i].ShowEnd = showEnd;
                            }
                            this.StatsForm.UpdateServerConnectionLog(logRound.Info.SessionId, false);
                            logRound.Info = null;
                            Stats.InShow = false;
                            Stats.EndedShow = true;
                            return true;
                        } else {
                            Stats.CasualRoundNum = 0;
                        }
                    } else if (logRound.Info.UseShareCode || (this.StatsForm.CurrentSettings.RecordEscapeDuringAGame && !Stats.EndedShow)) {
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
                                round[i].Qualified = true;
                                round[i].Crown = true;
                            } else {
                                round[i].IsAbandon = true;
                            }
                            round[i].ShowEnd = showEnd;
                            if (round.Count > 1 && (i + 1) != round.Count) {
                                round[i].IsFinal = false;
                            }
                        }
                        this.StatsForm.UpdateServerConnectionLog(logRound.Info.SessionId, false);
                        logRound.Info = null;
                        Stats.InShow = false;
                        Stats.EndedShow = true;
                        return true;
                    }
                }
                logRound.Info = null;
                Stats.InShow = false;
                Stats.EndedShow = true;
            } else if (line.Line.IndexOf(" == [CompletedEpisodeDto] ==", StringComparison.OrdinalIgnoreCase) != -1) {
                if (logRound.Info == null || Stats.EndedShow) { return false; }

                Stats.SavedRoundCount = logRound.Info.Round;
                Stats.EndedShow = true;

                if (logRound.Info.End == DateTime.MinValue) {
                    Stats.LastPlayedRoundStart = logRound.Info.Start;
                    Stats.IsLastPlayedRoundStillPlaying = true;
                    logRound.Info.End = line.Date;
                }
                logRound.Info.Playing = false;

                RoundInfo roundInfo = null;
                StringReader sr = new StringReader(line.Line);
                string detail;
                bool foundRound = false;
                int maxRound = 0;
                DateTime showStart = DateTime.MinValue;
                int questKudos = 0;
                while ((detail = sr.ReadLine()) != null) {
                    if (detail.IndexOf("[Round ", StringComparison.OrdinalIgnoreCase) == 0) {
                        foundRound = true;
                        int roundNum = detail[7] - 0x30 + 1;
                        string roundName = detail.Substring(11, detail.Length - 12);

                        if (roundNum - 1 < round.Count) {
                            if (roundNum > maxRound) {
                                maxRound = roundNum;
                            }

                            roundInfo = round[roundNum - 1];

                            if (string.IsNullOrEmpty(roundInfo.Name)) {
                                return false;
                            }

                            if (string.Equals(roundInfo.ShowNameId, "wle_mrs_shuffle_show") || string.Equals(roundInfo.ShowNameId, "wle_shuffle_discover") || string.Equals(roundInfo.ShowNameId, "wle_mrs_shuffle_show_squads")) {
                                if (round.Count > 1 && roundNum != round.Count) {
                                    roundInfo.IsFinal = false;
                                }
                                roundName = this.StatsForm.ReplaceLevelIdInShuffleShow(roundInfo.ShowNameId, roundName);
                            }

                            if (!string.Equals(roundInfo.Name, roundName, StringComparison.OrdinalIgnoreCase)) {
                                return false;
                            }

                            roundInfo.VerifyName();

                            if (roundNum == 1) {
                                showStart = roundInfo.Start;
                            }
                            roundInfo.ShowStart = showStart;
                            roundInfo.Playing = false;
                            roundInfo.Round = roundNum;
                        } else {
                            return false;
                        }

                        if (roundInfo.End == DateTime.MinValue) {
                            roundInfo.End = line.Date;
                        }
                        if (roundInfo.Start == DateTime.MinValue) {
                            roundInfo.Start = roundInfo.End;
                        }
                    } else if (foundRound) {
                        if (detail.IndexOf("> Qualified: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            char qualified = detail[13];
                            roundInfo.Qualified = qualified == 'T';
                        } else if (detail.IndexOf("> Position: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            roundInfo.Position = int.Parse(detail.Substring(12));
                        } else if (detail.IndexOf("> Team Score: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            roundInfo.Score = int.Parse(detail.Substring(14));
                        } else if (detail.IndexOf("> Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            roundInfo.Kudos += int.Parse(detail.Substring(9));
                        } else if (detail.IndexOf("> Bonus Tier: ", StringComparison.OrdinalIgnoreCase) == 0 && detail.Length == 15) {
                            char tier = detail[14];
                            roundInfo.Tier = roundInfo.Qualified ? tier - 0x30 + 1 : 0;
                        } else if (detail.IndexOf("> Bonus Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            roundInfo.Kudos += int.Parse(detail.Substring(15));
                        }
                    } else {
                        if (detail.IndexOf("> Kudos: ", StringComparison.OrdinalIgnoreCase) == 0) {
                            questKudos = int.Parse(detail.Substring(9));
                            //> Fame:, > Crowns:, > CurrentCrownShards:
                        }
                    }
                }

                if (round.Count > maxRound) {
                    return false;
                }

                if (roundInfo != null) {
                    roundInfo.Kudos += questKudos;
                }

                DateTime showEnd = logRound.Info.End;
                for (int i = 0; i < round.Count; i++) {
                    round[i].ShowEnd = showEnd;
                }

                if (logRound.Info.Qualified) {
                    logRound.Info.Crown = true;
                    logRound.InLoadingGameScreen = false;
                    logRound.CountingPlayers = false;
                    logRound.GetCurrentPlayerID = false;
                    logRound.FindingPosition = false;
                }
                this.StatsForm.UpdateServerConnectionLog(logRound.Info.SessionId, false);
                logRound.Info = null;
                return true;
            }
            return false;
        }
    }
}
