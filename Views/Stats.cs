using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LiteDB;
using Microsoft.Win32;
namespace FallGuysStats {
    public partial class Stats : Form {
        [STAThread]
        static void Main(string[] args) {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Stats());
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static string LOGNAME = "Player.log";
        private static List<DateTime> Seasons = new List<DateTime> {
            new DateTime(2020, 8, 4, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 10, 8, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 3, 22, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 6, 21, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc)
        };
        private static DateTime SeasonStart, WeekStart, DayStart;
        private static DateTime SessionStart = DateTime.UtcNow;
        public static bool InShow = false;
        public static bool EndedShow = false;
        public static int LastServerPing = 0;

        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        private LogFileRewriter logFileRewriter = new LogFileRewriter();
        public int Shows;
        public int Rounds;
        public TimeSpan Duration;
        public int Wins;
        public int Finals;
        public int Kudos;
        private int nextShowID;
        private bool loadingExisting;
        public LiteDatabase StatsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        public ILiteCollection<UserSettings> UserSettings;
        public UserSettings CurrentSettings;
        private Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        private DateTime startupTime = DateTime.UtcNow;
        private int askedPreviousShows = 0;
        private TextInfo textInfo;
        public Stats() {
            InitializeComponent();

            Text = $"Fall Guys Stats v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            logFile.OnParsedLogLines += LogFile_OnParsedLogLines;
            logFile.OnNewLogFileDate += LogFile_OnNewLogFileDate;
            logFile.OnError += LogFile_OnError;
            logFile.OnParsedLogLinesCurrent += LogFile_OnParsedLogLinesCurrent;

            foreach (var entry in LevelStats.ALL) {
                StatDetails.Add(entry.Value);
                StatLookup.Add(entry.Key, entry.Value);
            }

            gridDetails.DataSource = StatDetails;

            if (!File.Exists("data-pre8_2_0-backup.db") || !File.Exists("data.db")) {
                
                string initial8_2Message = "As of Fall Guys 8.2.0, essential timing info has been removed from the logs. This version of the stat tracker reconstructs the timestamps. Round completion times should be close to correct (within ~0.1 seconds)" + Environment.NewLine + Environment.NewLine + "Please make sure the stat tracker is running BEFORE starting a show!";

                if (File.Exists("data.db")) {
                    File.Copy("data.db", "data-pre8_2_0-backup.db");
                    initial8_2Message += Environment.NewLine + Environment.NewLine + "A backup of the existing database has been written to data-pre8_2_0-backup.db";
                } else {
                    // Create blank data-pre8_2_0-backup.db to prevent additional messages
                    File.Create("data-pre8_2_0-backup.db").Dispose();
                }

                MessageBox.Show(this, initial8_2Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            StatsDB = new LiteDatabase(@"data.db");
            StatsDB.Pragma("UTC_DATE", true);
            RoundDetails = StatsDB.GetCollection<RoundInfo>("RoundDetails");
            UserSettings = StatsDB.GetCollection<UserSettings>("UserSettings");

            StatsDB.BeginTrans();
            if (UserSettings.Count() == 0) {
                CurrentSettings = GetDefaultSettings();
                UserSettings.Insert(CurrentSettings);
            } else {
                try {
                    CurrentSettings = UserSettings.FindAll().First();
                } catch {
                    UserSettings.DeleteAll();
                    CurrentSettings = GetDefaultSettings();
                    UserSettings.Insert(CurrentSettings);
                }
            }

            UpdateHoopsieLegends();

            RoundDetails.EnsureIndex(x => x.Name);
            RoundDetails.EnsureIndex(x => x.ShowID);
            RoundDetails.EnsureIndex(x => x.Round);
            RoundDetails.EnsureIndex(x => x.Start);
            RoundDetails.EnsureIndex(x => x.InParty);
            StatsDB.Commit();

            UpdateDatabaseVersion();

            CurrentRound = new List<RoundInfo>();

            overlay = new Overlay() { StatsForm = this, Icon = Icon, ShowIcon = true };
            overlay.Show();
            overlay.Visible = false;
            overlay.StartTimer();

            UpdateGameExeLocation();
            if (CurrentSettings.AutoLaunchGameOnStartup) {
                LaunchGame(true);
            }

            RemoveUpdateFiles();
        }
        private void RemoveUpdateFiles() {
#if AllowUpdate
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string file in Directory.EnumerateFiles(filePath, "*.bak")) {
                try {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                } catch { }
            }
#endif
        }
        private void UpdateDatabaseVersion() {
            if (!CurrentSettings.UpdatedDateFormat) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    info.Start = DateTime.SpecifyKind(info.Start.ToLocalTime(), DateTimeKind.Utc);
                    info.End = DateTime.SpecifyKind(info.End.ToLocalTime(), DateTimeKind.Utc);
                    info.Finish = info.Finish.HasValue ? DateTime.SpecifyKind(info.Finish.Value.ToLocalTime(), DateTimeKind.Utc) : (DateTime?)null;
                    RoundDetails.Update(info);
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.UpdatedDateFormat = true;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 0) {
                CurrentSettings.SwitchBetweenQualify = CurrentSettings.SwitchBetweenLongest;
                CurrentSettings.Version = 1;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 1) {
                CurrentSettings.SwitchBetweenPlayers = CurrentSettings.SwitchBetweenLongest;
                CurrentSettings.Version = 2;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 2) {
                CurrentSettings.SwitchBetweenStreaks = CurrentSettings.SwitchBetweenLongest;
                CurrentSettings.Version = 3;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 3 || CurrentSettings.Version == 4) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_variation", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 5;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 5 || CurrentSettings.Version == 6) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_northernlion", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 7;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 7) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_hard_mode", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 8;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 8) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 9;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 9) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    if (info.Name.Equals("round_fall_mountain", StringComparison.OrdinalIgnoreCase)) {
                        info.Name = "round_fall_mountain_hub_complete";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 10;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 10) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0
                        || (index = info.Name.IndexOf(". D", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 11;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 11) {
                AllStats.AddRange(RoundDetails.FindAll());
                AllStats.Sort();
                StatsDB.BeginTrans();
                int lastShow = -1;
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (lastShow != info.ShowID) {
                        lastShow = info.ShowID;

                        if (StatLookup.TryGetValue(info.Name, out LevelStats stats)) {
                            info.IsFinal = stats.IsFinal && (info.Name != "round_floor_fall" || info.Round >= 3 || (i > 0 && AllStats[i - 1].Name != "round_floor_fall"));
                        } else {
                            info.IsFinal = false;
                        }
                    } else {
                        info.IsFinal = false;
                    }

                    RoundDetails.Update(info);
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 12;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 12 || CurrentSettings.Version == 13) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_fruitpunch", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_fruitpunch_s4_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoverboardsurvival", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoverboardsurvival_s4_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_basketfall", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_basketfall_s4_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_territory", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_territory_control_s4_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_shortcircuit", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_shortcircuit";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_06", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_06";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_tunnel_race", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_tunnel_race";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_1v1_button", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_1v1_button_basher";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_slimeclimb", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slimeclimb_2";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 14;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 14) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_king_of", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_king_of_the_hill";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_drumtop", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_drumtop";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_penguin_solos", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_penguin_solos";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_07", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_07";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_robotrampage", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_robotrampage_arena_2";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_crown_maze", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_crown_maze";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 15;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 15) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_gauntlet_08", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_08";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_airtime", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_airtime";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_follow-", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow-the-leader_s6_launch";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_pipedup", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_pipedup_s6_launch";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_see_saw_360", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_see_saw_360";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 16;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 16) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_fruit_bowl", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_fruit_bowl";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 17;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 17) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_invisibeans", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_invisibeans";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 18;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 18) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_1v1_volleyfall_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_09", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_09_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_short_circuit_2", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_short_circuit_2_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoops_revenge", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoops_revenge_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hexaring", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hexaring_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_spin_ring", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_spin_ring_symphony_launch_show";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_blastball", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_blastball_arenasurvival_symphony_launch_show";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 19;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 19) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_satellitehoppers", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_satellitehoppers_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_ffa_button_bashers", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_ffa_button_bashers_squads_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoverboardsurvival2", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoverboardsurvival2_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_10", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_10_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_starlink", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_starlink_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_tiptoefinale", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_tiptoefinale_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_pixelperfect_almond";
                        RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hexsnake", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hexsnake_almond";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 20;
                SaveUserSettings();
            }

            if (CurrentSettings.Version == 20) {
                AllStats.AddRange(RoundDetails.FindAll());
                StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_follow-the-leader", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow-the-leader_s6_launch";
                        RoundDetails.Update(info);
                    }
                }
                StatsDB.Commit();
                AllStats.Clear();
                CurrentSettings.Version = 21;
                SaveUserSettings();
            }
        }
        private UserSettings GetDefaultSettings() {
            return new UserSettings() {
                ID = 1,
                CycleTimeSeconds = 5,
                FilterType = 0,
                SelectedProfile = 0,
                FlippedDisplay = false,
                LogPath = null,
                OverlayColor = 3,
                OverlayLocationX = null,
                OverlayLocationY = null,
                SwitchBetweenLongest = true,
                SwitchBetweenQualify = true,
                SwitchBetweenPlayers = true,
                SwitchBetweenStreaks = true,
                OnlyShowLongest = false,
                OnlyShowGold = false,
                OnlyShowPing = false,
                OnlyShowFinalStreak = false,
                OverlayVisible = false,
                OverlayNotOnTop = false,
                PreviousWins = 0,
                WinsFilter = 0,
                QualifyFilter = 0,
                FastestFilter = 0,
                HideWinsInfo = false,
                HideRoundInfo = false,
                HideTimeInfo = false,
                ShowOverlayTabs = false,
                ShowPercentages = false,
                AutoUpdate = false,
                FormLocationX = null,
                FormLocationY = null,
                FormWidth = null,
                FormHeight = null,
                OverlayWidth = 786,
                OverlayHeight = 99,
                HideOverlayPercentages = false,
                HoopsieHeros = false,
                Version = 21,
                AutoLaunchGameOnStartup = false,
                GameExeLocation = string.Empty,
                IgnoreLevelTypeWhenSorting = false,
                UpdatedDateFormat = true
            };
        }
        private void UpdateHoopsieLegends() {
            LevelStats level = StatLookup["round_hoops_blockade_solo"];
            string newName = CurrentSettings.HoopsieHeros ? "Hoopsie Heroes" : "Hoopsie Legends";
            if (level.Name != newName) {
                level.Name = newName;
                gridDetails.Invalidate();
            }
        }
        public void UpdateDates() {
            if (DateTime.Now.Date.ToUniversalTime() == DayStart) { return; }

            DateTime currentUTC = DateTime.UtcNow;
            for (int i = Seasons.Count - 1; i >= 0; i--) {
                if (currentUTC > Seasons[i]) {
                    SeasonStart = Seasons[i];
                    break;
                }
            }
            WeekStart = DateTime.Now.Date.AddDays(-7).ToUniversalTime();
            DayStart = DateTime.Now.Date.ToUniversalTime();

            ResetStats();
        }
        public void SaveUserSettings() {
            lock (StatsDB) {
                StatsDB.BeginTrans();
                UserSettings.Update(CurrentSettings);
                StatsDB.Commit();
            }
        }
        public void ResetStats() {
            for (int i = 0; i < StatDetails.Count; i++) {
                LevelStats calculator = StatDetails[i];
                calculator.Clear();
            }

            ClearTotals();

            List<RoundInfo> rounds = new List<RoundInfo>();
            int profile = menuProfileMain.Checked ? 0 : 1;

            lock (StatsDB) {
                AllStats.Clear();
                nextShowID = 0;
                lastAddedShow = DateTime.MinValue;
                if (RoundDetails.Count() > 0) {
                    AllStats.AddRange(RoundDetails.FindAll());
                    AllStats.Sort();

                    if (AllStats.Count > 0) {
                        nextShowID = AllStats[AllStats.Count - 1].ShowID;

                        int lastAddedShowID = -1;
                        for (int i = AllStats.Count - 1; i >= 0; i--) {
                            RoundInfo info = AllStats[i];
                            info.ToLocalTime();
                            if (info.Profile != profile) { continue; }

                            if (info.ShowID == lastAddedShowID || (IsInStatsFilter(info.Start) && IsInPartyFilter(info))) {
                                lastAddedShowID = info.ShowID;
                                rounds.Add(info);
                            }

                            if (info.Start > lastAddedShow && info.Round == 1) {
                                lastAddedShow = info.Start;
                            }
                        }
                    }
                }
            }

            lock (CurrentRound) {
                CurrentRound.Clear();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    if (info.Profile != profile) { continue; }

                    CurrentRound.Insert(0, info);
                    if (info.Round == 1) {
                        break;
                    }
                }
            }

            rounds.Sort();
            loadingExisting = true;
            LogFile_OnParsedLogLines(rounds);
            loadingExisting = false;
        }
        private void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                if (!overlay.Disposing && !overlay.IsDisposed && !IsDisposed && !Disposing) {
                    if (overlay.Visible) {
                        CurrentSettings.OverlayLocationX = overlay.Location.X;
                        CurrentSettings.OverlayLocationY = overlay.Location.Y;

                        CurrentSettings.OverlayWidth = overlay.Width;
                        CurrentSettings.OverlayHeight = overlay.Height;
                    }
                    CurrentSettings.FilterType = menuAllStats.Checked ? 0 : menuSeasonStats.Checked ? 1 : menuWeekStats.Checked ? 2 : menuDayStats.Checked ? 3 : 4;
                    CurrentSettings.SelectedProfile = menuProfileMain.Checked ? 0 : 1;

                    CurrentSettings.FormLocationX = Location.X;
                    CurrentSettings.FormLocationY = Location.Y;
                    CurrentSettings.FormWidth = ClientSize.Width;
                    CurrentSettings.FormHeight = ClientSize.Height;
                    SaveUserSettings();
                }
                StatsDB.Dispose();
            } catch { }
        }
        private void Stats_Load(object sender, EventArgs e) {
            try {
                if (CurrentSettings.FormWidth.HasValue) {
                    ClientSize = new Size(CurrentSettings.FormWidth.Value, CurrentSettings.FormHeight.Value);
                }
                if (CurrentSettings.FormLocationX.HasValue && IsOnScreen(CurrentSettings.FormLocationX.Value, CurrentSettings.FormLocationY.Value, Width)) {
                    Location = new Point(CurrentSettings.FormLocationX.Value, CurrentSettings.FormLocationY.Value);
                }

#if AllowUpdate
                if (CurrentSettings.AutoUpdate && CheckForUpdate(true)) {
                    return;
                }
#endif

                if (CurrentSettings.SelectedProfile != 0) {
                    menuProfileMain.Checked = false;
                    switch (CurrentSettings.SelectedProfile) {
                        case 1: menuProfilePractice.Checked = true; break;
                    }
                }

                UpdateDates();
            } catch { }
        }
        private void Stats_Shown(object sender, EventArgs e) {
            try {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                if (!string.IsNullOrEmpty(CurrentSettings.LogPath)) {
                    logPath = CurrentSettings.LogPath;
                }
                logFileRewriter.Start(logPath, LOGNAME);
                logFile.Start(logPath, LOGNAME);

                overlay.ArrangeDisplay(CurrentSettings.FlippedDisplay, CurrentSettings.ShowOverlayTabs, CurrentSettings.HideWinsInfo, CurrentSettings.HideRoundInfo, CurrentSettings.HideTimeInfo, CurrentSettings.OverlayColor, CurrentSettings.OverlayWidth, CurrentSettings.OverlayHeight, CurrentSettings.OverlayFontSerialized);
                if (CurrentSettings.OverlayVisible) {
                    ToggleOverlay(overlay);
                }

                menuAllStats.Checked = false;
                switch (CurrentSettings.FilterType) {
                    case 0: menuAllStats.Checked = true; menuStats_Click(menuAllStats, null); break;
                    case 1: menuSeasonStats.Checked = true; menuStats_Click(menuSeasonStats, null); break;
                    case 2: menuWeekStats.Checked = true; menuStats_Click(menuWeekStats, null); break;
                    case 3: menuDayStats.Checked = true; menuStats_Click(menuDayStats, null); break;
                    case 4: menuSessionStats.Checked = true; menuStats_Click(menuSessionStats, null); break;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogFile_OnError(string error) {
            if (!Disposing && !IsDisposed) {
                try {
                    if (InvokeRequired) {
                        Invoke((Action<string>)LogFile_OnError, error);
                    } else {
                        MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch { }
            }
        }
        private void LogFile_OnNewLogFileDate(DateTime newDate) {
            if (SessionStart != newDate) {
                SessionStart = newDate;
                if (menuSessionStats.Checked) {
                    menuStats_Click(menuSessionStats, null);
                }
            }
        }
        private void LogFile_OnParsedLogLinesCurrent(List<RoundInfo> round) {
            lock (CurrentRound) {
                if (CurrentRound == null || CurrentRound.Count != round.Count) {
                    CurrentRound = round;
                } else {
                    for (int i = 0; i < CurrentRound.Count; i++) {
                        RoundInfo info = CurrentRound[i];
                        if (!info.Equals(round[i])) {
                            CurrentRound = round;
                            break;
                        }
                    }
                }
            }
        }
        private void LogFile_OnParsedLogLines(List<RoundInfo> round) {
            try {
                if (InvokeRequired) {
                    Invoke((Action<List<RoundInfo>>)LogFile_OnParsedLogLines, round);
                    return;
                }

                lock (StatsDB) {
                    if (!loadingExisting) { StatsDB.BeginTrans(); }

                    int profile = menuProfileMain.Checked ? 0 : 1;
                    for (int k = 0; k < round.Count; k++) {
                        RoundInfo stat = round[k];

                        if (!loadingExisting) {
                            RoundInfo info = null;
                            for (int i = AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo temp = AllStats[i];
                                if (temp.Start == stat.Start && temp.Name == stat.Name) {
                                    info = temp;
                                    break;
                                }
                            }

                            if (info == null && stat.Start > lastAddedShow) {
                                
                                // Commenting out because with new log only using logs read at realtime,
                                // We always want the data to import

                                //if (stat.ShowEnd < startupTime && askedPreviousShows == 0) {
                                //    if (MessageBox.Show(this, "There are previous shows not in your current stats. Do you wish to add these to your stats?", "Previous Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                                //        askedPreviousShows = 1;
                                //    } else {
                                //        askedPreviousShows = 2;
                                //    }
                                //}

                                //if (stat.ShowEnd < startupTime && askedPreviousShows == 2) {
                                //    continue;
                                //}

                                if (stat.Round == 1) {
                                    nextShowID++;
                                    lastAddedShow = stat.Start;
                                }
                                stat.ShowID = nextShowID;
                                stat.Profile = profile;
                                RoundDetails.Insert(stat);
                                AllStats.Add(stat);
                            } else {
                                continue;
                            }
                        }

                        if (!stat.PrivateLobby) {
                            if (stat.Round == 1) {
                                Shows++;
                            }
                            Rounds++;
                        }
                        Duration += stat.End - stat.Start;
                        Kudos += stat.Kudos;

                        // add new type of round to the rounds lookup
                        if (!StatLookup.ContainsKey(stat.Name)) {
                            string roundName = stat.Name;
                            if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                                roundName = roundName.Substring(6).Replace('_', ' ');
                            }

                            LevelStats newLevel = new LevelStats(textInfo.ToTitleCase(roundName), LevelType.Unknown, false, 0);
                            StatLookup.Add(stat.Name, newLevel);
                            StatDetails.Add(newLevel);
                            gridDetails.DataSource = null;
                            gridDetails.DataSource = StatDetails;
                        }

                        stat.ToLocalTime();
                        LevelStats levelStats = StatLookup[stat.Name];

                        if (!stat.PrivateLobby) {
                            if (stat.IsFinal || stat.Crown) {
                                Finals++;
                                if (stat.Qualified) {
                                    Wins++;
                                }
                            }
                        }
                        levelStats.Add(stat);
                    }

                    if (!loadingExisting) { StatsDB.Commit(); }
                }

                lock (CurrentRound) {
                    CurrentRound.Clear();
                    for (int i = round.Count - 1; i >= 0; i--) {
                        RoundInfo info = round[i];
                        CurrentRound.Insert(0, info);
                        if (info.Round == 1) {
                            break;
                        }
                    }
                }

                if (!Disposing && !IsDisposed) {
                    try {
                        UpdateTotals();
                    } catch { }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsInStatsFilter(DateTime showEnd) {
            return menuAllStats.Checked ||
                (menuSeasonStats.Checked && showEnd > SeasonStart) ||
                (menuWeekStats.Checked && showEnd > WeekStart) ||
                (menuDayStats.Checked && showEnd > DayStart) ||
                (menuSessionStats.Checked && showEnd > SessionStart);
        }
        private bool IsInPartyFilter(RoundInfo info) {
            return menuAllPartyStats.Checked ||
                (menuSoloStats.Checked && !info.InParty) ||
                (menuPartyStats.Checked && info.InParty);
        }
        public string GetCurrentFilter() {
            return menuAllStats.Checked ? "ALL TIME" : menuSeasonStats.Checked ? "SEASON" : menuWeekStats.Checked ? "WEEK" : menuDayStats.Checked ? "DAY" : "SESSION";
        }
        public StatSummary GetLevelInfo(string name) {
            StatSummary summary = new StatSummary();
            LevelStats levelDetails;

            summary.AllWins = 0;
            summary.TotalShows = 0;
            summary.TotalPlays = 0;
            summary.TotalWins = 0;
            summary.TotalFinals = 0;
            int lastShow = -1;
            LevelStats currentLevel;
            if (!StatLookup.TryGetValue(name ?? string.Empty, out currentLevel)) {
                currentLevel = new LevelStats(name, LevelType.Unknown, false, 0);
            }
            int profile = menuProfileMain.Checked ? 0 : 1;

            for (int i = 0; i < AllStats.Count; i++) {
                RoundInfo info = AllStats[i];
                if (info.Profile != profile) { continue; }

                TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                bool hasLevelDetails = StatLookup.TryGetValue(info.Name, out levelDetails);
                bool isCurrentLevel = currentLevel.Name.Equals(hasLevelDetails ? levelDetails.Name : info.Name, StringComparison.OrdinalIgnoreCase);

                int currentShow = info.ShowID;
                RoundInfo endShow = info;
                for (int j = i + 1; j < AllStats.Count; j++) {
                    if (AllStats[j].ShowID != currentShow) {
                        break;
                    }
                    endShow = AllStats[j];
                }

                bool isInQualifyFilter = !endShow.PrivateLobby && (CurrentSettings.QualifyFilter == 0 ||
                    (CurrentSettings.QualifyFilter == 1 && IsInStatsFilter(endShow.Start) && IsInPartyFilter(info)) ||
                    (CurrentSettings.QualifyFilter == 2 && endShow.Start > SeasonStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.QualifyFilter == 3 && endShow.Start > WeekStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.QualifyFilter == 4 && endShow.Start > DayStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.QualifyFilter == 5 && endShow.Start > SessionStart && IsInPartyFilter(info)));
                bool isInFastestFilter = CurrentSettings.FastestFilter == 0 ||
                    (CurrentSettings.FastestFilter == 1 && IsInStatsFilter(endShow.Start) && IsInPartyFilter(info)) ||
                    (CurrentSettings.FastestFilter == 2 && endShow.Start > SeasonStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.FastestFilter == 3 && endShow.Start > WeekStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.FastestFilter == 4 && endShow.Start > DayStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.FastestFilter == 5 && endShow.Start > SessionStart && IsInPartyFilter(info));
                bool isInWinsFilter = !endShow.PrivateLobby && (CurrentSettings.WinsFilter == 3 ||
                    (CurrentSettings.WinsFilter == 0 && IsInStatsFilter(endShow.Start) && IsInPartyFilter(info)) ||
                    (CurrentSettings.WinsFilter == 1 && endShow.Start > SeasonStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.WinsFilter == 2 && endShow.Start > WeekStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.WinsFilter == 4 && endShow.Start > DayStart && IsInPartyFilter(info)) ||
                    (CurrentSettings.WinsFilter == 5 && endShow.Start > SessionStart && IsInPartyFilter(info)));

                if (info.ShowID != lastShow) {
                    lastShow = info.ShowID;
                    if (isInWinsFilter) {
                        summary.TotalShows++;
                    }
                }

                if (isCurrentLevel) {
                    if (isInQualifyFilter) {
                        summary.TotalPlays++;
                    }

                    if (isInFastestFilter) {
                        if ((!hasLevelDetails || levelDetails.Type == LevelType.Team) && info.Score.HasValue && (!summary.BestScore.HasValue || info.Score.Value > summary.BestScore.Value)) {
                            summary.BestScore = info.Score;
                        }
                    }
                }

                if (info == endShow && (levelDetails.IsFinal || info.Crown) && !endShow.PrivateLobby) {
                    summary.CurrentFinalStreak++;
                    if (summary.BestFinalStreak < summary.CurrentFinalStreak) {
                        summary.BestFinalStreak = summary.CurrentFinalStreak;
                    }
                }

                if (info.Qualified) {
                    if (hasLevelDetails && (info.IsFinal || info.Crown)) {
                        if (!info.PrivateLobby) {
                            summary.AllWins++;
                        }

                        if (isInWinsFilter) {
                            summary.TotalWins++;
                            summary.TotalFinals++;
                        }

                        if (!info.PrivateLobby) {
                            summary.CurrentStreak++;
                            if (summary.CurrentStreak > summary.BestStreak) {
                                summary.BestStreak = summary.CurrentStreak;
                            }
                        }
                    }

                    if (isCurrentLevel) {
                        if (isInQualifyFilter) {
                            if (info.Tier == (int)QualifyTier.Gold) {
                                summary.TotalGolds++;
                            }
                            summary.TotalQualify++;
                        }

                        if (isInFastestFilter) {
                            if (finishTime.TotalSeconds > 1.1 && (!summary.BestFinish.HasValue || summary.BestFinish.Value > finishTime)) {
                                summary.BestFinish = finishTime;
                            }
                            if (finishTime.TotalSeconds > 1.1 && info.Finish.HasValue && (!summary.LongestFinish.HasValue || summary.LongestFinish.Value < finishTime)) {
                                summary.LongestFinish = finishTime;
                            }
                        }

                        if (finishTime.TotalSeconds > 1.1 && (!summary.BestFinishOverall.HasValue || summary.BestFinishOverall.Value > finishTime)) {
                            summary.BestFinishOverall = finishTime;
                        }
                        if (finishTime.TotalSeconds > 1.1 && info.Finish.HasValue && (!summary.LongestFinishOverall.HasValue || summary.LongestFinishOverall.Value < finishTime)) {
                            summary.LongestFinishOverall = finishTime;
                        }
                    }
                } else if (!info.PrivateLobby) {
                    if (!info.IsFinal && !info.Crown) {
                        summary.CurrentFinalStreak = 0;
                    }
                    summary.CurrentStreak = 0;
                    if (isInWinsFilter && hasLevelDetails && (info.IsFinal || info.Crown)) {
                        summary.TotalFinals++;
                    }
                }
            }

            return summary;
        }
        private void ClearTotals() {
            Rounds = 0;
            Duration = TimeSpan.Zero;
            Wins = 0;
            Shows = 0;
            Finals = 0;
            Kudos = 0;
        }
        private void UpdateTotals() {
            try {
                lblTotalRounds.Text = $"Rounds: {Rounds}";
                lblTotalShows.Text = $"Shows: {Shows}";
                lblTotalTime.Text = $"Time Played: {(int)Duration.TotalHours}:{Duration:mm\\:ss}";
                float winChance = (float)Wins * 100 / (Shows == 0 ? 1 : Shows);
                lblTotalWins.Text = $"Wins: {Wins} ({winChance:0.0} %)";
                float finalChance = (float)Finals * 100 / (Shows == 0 ? 1 : Shows);
                lblTotalFinals.Text = $"Finals: {Finals} ({finalChance:0.0} %)";
                lblKudos.Text = $"Kudos: {Kudos}";
                gridDetails.Refresh();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            try {
                if (gridDetails.Columns.Count == 0) { return; }
                int pos = 0;

                gridDetails.Columns["AveKudos"].Visible = false;
                gridDetails.Columns["AveDuration"].Visible = false;
                gridDetails.Setup("Name", pos++, 0, "Level Name", DataGridViewContentAlignment.MiddleLeft);
                gridDetails.Setup("Played", pos++, 55, "Played", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Qualified", pos++, 65, "Qualified", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Gold", pos++, 50, "Gold", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Silver", pos++, 50, "Silver", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Bronze", pos++, 50, "Bronze", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Kudos", pos++, 60, "Kudos", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Fastest", pos++, 60, "Fastest", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Longest", pos++, 60, "Longest", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("AveFinish", pos++, 60, "Average", DataGridViewContentAlignment.MiddleRight);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                LevelStats info = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;

                switch (gridDetails.Columns[e.ColumnIndex].Name) {
                    case "Name":
                        gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Click to view level stats";
                        if (info.IsFinal) {
                            e.CellStyle.BackColor = Color.Pink;
                            break;
                        }
                        switch (info.Type) {
                            case LevelType.Race: e.CellStyle.BackColor = Color.LightGoldenrodYellow; break;
                            case LevelType.Survival: e.CellStyle.BackColor = Color.LightBlue; break;
                            case LevelType.Team: e.CellStyle.BackColor = Color.LightGreen; break;
                            case LevelType.Hunt: e.CellStyle.BackColor = Color.LightGoldenrodYellow; break;
                            case LevelType.Unknown: e.CellStyle.BackColor = Color.LightGray; break;
                        }
                        break;
                    case "Qualified": {
                            float qualifyChance = info.Qualified * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Qualified}";
                            } else {
                                e.Value = info.Qualified;
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Gold": {
                            float qualifyChance = info.Gold * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Gold}";
                            } else {
                                e.Value = info.Gold;
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Silver": {
                            float qualifyChance = info.Silver * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Silver}";
                            } else {
                                e.Value = info.Silver;
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Bronze": {
                            float qualifyChance = (float)info.Bronze * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Bronze}";
                            } else {
                                e.Value = info.Bronze;
                                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "AveFinish":
                        e.Value = info.AveFinish.ToString("m\\:ss\\.ff");
                        break;
                    case "Fastest":
                        e.Value = info.Fastest.ToString("m\\:ss\\.ff");
                        break;
                    case "Longest":
                        e.Value = info.Longest.ToString("m\\:ss\\.ff");
                        break;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex >= 0 && gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                    gridDetails.Cursor = Cursors.Hand;
                } else {
                    gridDetails.Cursor = Cursors.Default;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                    using (LevelDetails levelDetails = new LevelDetails()) {
                        LevelStats stats = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                        levelDetails.LevelName = stats.Name;
                        List<RoundInfo> rounds = stats.Stats;
                        rounds.Sort();
                        levelDetails.RoundDetails = rounds;
                        levelDetails.StatsForm = this;
                        levelDetails.ShowDialog(this);
                    }
                } else {
                    ToggleWinPercentageDisplay();
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = gridDetails.GetSortOrder(columnName);

            StatDetails.Sort(delegate (LevelStats one, LevelStats two) {
                LevelType oneType = one.IsFinal ? LevelType.Hunt : one.Type == LevelType.Hunt ? LevelType.Race : one.Type;
                LevelType twoType = two.IsFinal ? LevelType.Hunt : two.Type == LevelType.Hunt ? LevelType.Race : two.Type;

                int typeCompare = CurrentSettings.IgnoreLevelTypeWhenSorting && sortOrder != SortOrder.None ? 0 : ((int)oneType).CompareTo((int)twoType);

                if (sortOrder == SortOrder.Descending) {
                    LevelStats temp = one;
                    one = two;
                    two = temp;
                }

                int nameCompare = one.Name.CompareTo(two.Name);
                bool percents = CurrentSettings.ShowPercentages;
                if (typeCompare == 0 && sortOrder != SortOrder.None) {
                    switch (columnName) {
                        case "Gold": typeCompare = ((double)one.Gold / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Gold / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Silver": typeCompare = ((double)one.Silver / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Silver / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Bronze": typeCompare = ((double)one.Bronze / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Bronze / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Played": typeCompare = one.Played.CompareTo(two.Played); break;
                        case "Qualified": typeCompare = ((double)one.Qualified / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Qualified / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Kudos": typeCompare = one.Kudos.CompareTo(two.Kudos); break;
                        case "AveKudos": typeCompare = one.AveKudos.CompareTo(two.AveKudos); break;
                        case "AveFinish": typeCompare = one.AveFinish.CompareTo(two.AveFinish); break;
                        case "Fastest": typeCompare = one.Fastest.CompareTo(two.Fastest); break;
                        case "Longest": typeCompare = one.Longest.CompareTo(two.Longest); break;
                        default: typeCompare = nameCompare; break;
                    }
                }

                if (typeCompare == 0) {
                    typeCompare = nameCompare;
                }

                return typeCompare;
            });

            gridDetails.DataSource = null;
            gridDetails.DataSource = StatDetails;
            gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (gridDetails.SelectedCells.Count > 0) {
                gridDetails.ClearSelection();
            }
        }
        private void ToggleWinPercentageDisplay() {
            CurrentSettings.ShowPercentages = !CurrentSettings.ShowPercentages;
            SaveUserSettings();
            gridDetails.Invalidate();
        }
        private void ShowShows() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Shows";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }
                rounds.Sort();

                List<RoundInfo> shows = new List<RoundInfo>();
                int roundCount = 0;
                int kudosTotal = 0;
                bool won = false;
                bool isFinal = false;
                DateTime endDate = DateTime.MinValue;

                for (int i = rounds.Count - 1; i >= 0; i--) {
                    RoundInfo info = rounds[i];
                    if (roundCount == 0) {
                        endDate = info.End;
                        won = info.Qualified;
                        isFinal = info.IsFinal || info.Crown;
                    }
                    roundCount++;
                    kudosTotal += info.Kudos;
                    if (info.Round == 1) {
                        shows.Insert(0, new RoundInfo() { Name = isFinal ? "Final" : string.Empty, IsFinal = isFinal, End = endDate, Start = info.Start, StartLocal = info.StartLocal, Kudos = kudosTotal, Qualified = won, Round = roundCount, ShowID = info.ShowID, Tier = won ? 1 : 0 });
                        roundCount = 0;
                        kudosTotal = 0;
                    }
                }
                levelDetails.RoundDetails = shows;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowRounds() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Rounds";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }
                rounds.Sort();
                levelDetails.RoundDetails = rounds;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowFinals() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Finals";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }
                rounds.Sort();

                int keepShow = -1;
                for (int i = rounds.Count - 1; i >= 0; i--) {
                    RoundInfo info = rounds[i];
                    if (info.ShowID != keepShow && (info.Crown || info.IsFinal)) {
                        keepShow = info.ShowID;
                    } else if (info.ShowID != keepShow) {
                        rounds.RemoveAt(i);
                    }
                }
                levelDetails.RoundDetails = rounds;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowWinGraph() {
            List<RoundInfo> rounds = new List<RoundInfo>();
            for (int i = 0; i < StatDetails.Count; i++) {
                rounds.AddRange(StatDetails[i].Stats);
            }
            rounds.Sort();

            using (StatsDisplay display = new StatsDisplay() { Text = "Wins Per Day" }) {
                DataTable dt = new DataTable();
                dt.Columns.Add("Date", typeof(DateTime));
                dt.Columns.Add("Wins", typeof(int));
                dt.Columns.Add("Finals", typeof(int));
                dt.Columns.Add("Shows", typeof(int));

                if (rounds.Count > 0) {
                    DateTime start = rounds[0].StartLocal;
                    int currentWins = 0;
                    int currentFinals = 0;
                    int currentShows = 0;
                    for (int i = 0; i < rounds.Count; i++) {
                        RoundInfo info = rounds[i];
                        if (info.PrivateLobby) { continue; }

                        if (info.Round == 1) {
                            currentShows++;
                        }

                        if (info.Crown || info.IsFinal) {
                            currentFinals++;
                            if (info.Qualified) {
                                currentWins++;
                            }
                        }

                        if (info.StartLocal.Date != start.Date) {
                            dt.Rows.Add(start.Date, currentWins, currentFinals, currentShows);

                            int missingCount = (int)(info.StartLocal.Date - start.Date).TotalDays;
                            while (missingCount > 1) {
                                missingCount--;
                                start = start.Date.AddDays(1);
                                dt.Rows.Add(start, 0, 0, 0);
                            }

                            currentWins = 0;
                            currentFinals = 0;
                            currentShows = 0;
                            start = info.StartLocal;
                        }
                    }

                    dt.Rows.Add(start.Date, currentWins, currentFinals, currentShows);
                } else {
                    dt.Rows.Add(DateTime.Now.Date, 0, 0, 0);
                }

                display.Details = dt;
                display.ShowDialog(this);
            }
        }
        private void LaunchHelpInBrowser() {
            try {
                Process.Start(@"https://github.com/ShootMe/FallGuysStats");
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateGameExeLocation() {
            if (string.IsNullOrEmpty(CurrentSettings.GameExeLocation)) {
                string fallGuys = FindGameExeLocation();
                if (!string.IsNullOrEmpty(fallGuys)) {
                    CurrentSettings.GameExeLocation = fallGuys;
                    SaveUserSettings();
                }
            }
        }
        private void LaunchGame(bool ignoreExisting) {
            try {
                UpdateGameExeLocation();

                if (!string.IsNullOrEmpty(CurrentSettings.GameExeLocation) && File.Exists(CurrentSettings.GameExeLocation)) {
                    Process[] processes = Process.GetProcesses();
                    string fallGuys = Path.GetFileNameWithoutExtension(CurrentSettings.GameExeLocation);
                    for (int i = 0; i < processes.Length; i++) {
                        string name = processes[i].ProcessName;
                        if (name.IndexOf(fallGuys, StringComparison.OrdinalIgnoreCase) >= 0) {
                            if (!ignoreExisting) {
                                MessageBox.Show(this, "Fall Guys is already running.", "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }
                    }

                    Process.Start(CurrentSettings.GameExeLocation);
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string FindGameExeLocation() {
            try {
                // get steam install folder
                object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                string steamPath = (string)regValue;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
                }

                string fallGuys = Path.Combine(steamPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                if (File.Exists(fallGuys)) {
                    return fallGuys;
                }
                // read libraryfolders.vdf from install folder to get games installation folder
                // note: this parsing is terrible, but does technically work fine. There's a better way by specifying a schema and
                // fully parsing the file or something like that. This is quick and dirty, for sure.
                FileInfo libraryFoldersFile = new FileInfo(Path.Combine(steamPath, "steamapps", "libraryfolders.vdf"));
                if (libraryFoldersFile.Exists) {
                    JsonClass json = Json.Read(File.ReadAllText(libraryFoldersFile.FullName)) as JsonClass;
                    foreach (JsonObject obj in json) {
                        if (obj is JsonClass library) {
                            string libraryPath = library["path"].AsString();
                            if (!string.IsNullOrEmpty(libraryPath)) {
                                // look for exe in standard location under library
                                fallGuys = Path.Combine(libraryPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                                if (File.Exists(fallGuys)) {
                                    return fallGuys;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        private void lblTotalFinals_Click(object sender, EventArgs e) {
            try {
                ShowFinals();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalShows_Click(object sender, EventArgs e) {
            try {
                ShowShows();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalRounds_Click(object sender, EventArgs e) {
            try {
                ShowRounds();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalWins_Click(object sender, EventArgs e) {
            try {
                ShowWinGraph();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuStats_Click(object sender, EventArgs e) {
            try {
                ToolStripMenuItem button = sender as ToolStripMenuItem;
                if (button == menuAllStats || button == menuSeasonStats || button == menuWeekStats || button == menuDayStats || button == menuSessionStats) {
                    if (!menuAllStats.Checked && !menuSeasonStats.Checked && !menuWeekStats.Checked && !menuDayStats.Checked && !menuSessionStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in menuStatsFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }
                }

                if (button == menuAllPartyStats || button == menuSoloStats || button == menuPartyStats) {
                    if (!menuAllPartyStats.Checked && !menuSoloStats.Checked && !menuPartyStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in menuPartyFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }

                    button = menuAllStats.Checked ? menuAllStats : menuSeasonStats.Checked ? menuSeasonStats : menuWeekStats.Checked ? menuWeekStats : menuDayStats.Checked ? menuDayStats : menuSessionStats;
                }

                if (button == menuProfileMain || button == menuProfilePractice) {
                    if (!menuProfileMain.Checked && !menuProfilePractice.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in menuProfile.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }

                    button = menuAllStats.Checked ? menuAllStats : menuSeasonStats.Checked ? menuSeasonStats : menuWeekStats.Checked ? menuWeekStats : menuDayStats.Checked ? menuDayStats : menuSessionStats;
                }

                for (int i = 0; i < StatDetails.Count; i++) {
                    LevelStats calculator = StatDetails[i];
                    calculator.Clear();
                }

                ClearTotals();

                int profile = menuProfileMain.Checked ? 0 : 1;
                bool soloOnly = menuSoloStats.Checked;
                List<RoundInfo> rounds = new List<RoundInfo>();

                DateTime compareDate = menuAllStats.Checked ? DateTime.MinValue : menuSeasonStats.Checked ? SeasonStart : menuWeekStats.Checked ? WeekStart : menuDayStats.Checked ? DayStart : SessionStart;
                for (int i = 0; i < AllStats.Count; i++) {
                    RoundInfo round = AllStats[i];
                    if (round.Start > compareDate && round.Profile == profile && (menuAllPartyStats.Checked || round.InParty == !soloOnly)) {
                        rounds.Add(round);
                    }
                }

                rounds.Sort();

                if (rounds.Count > 0 && (button == menuWeekStats || button == menuDayStats || button == menuSessionStats)) {
                    int minShowID = rounds[0].ShowID;

                    for (int i = 0; i < AllStats.Count; i++) {
                        RoundInfo round = AllStats[i];
                        if (round.ShowID == minShowID && round.Start <= compareDate) {
                            rounds.Add(round);
                        }
                    }
                }

                rounds.Sort();

                CurrentSettings.SelectedProfile = profile;
                CurrentSettings.FilterType = menuAllStats.Checked ? 0 : menuSeasonStats.Checked ? 1 : menuWeekStats.Checked ? 2 : menuDayStats.Checked ? 3 : 4;
                SaveUserSettings();

                loadingExisting = true;
                LogFile_OnParsedLogLines(rounds);
                loadingExisting = false;
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuUpdate_Click(object sender, EventArgs e) {
            try {
                CheckForUpdate(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool CheckForUpdate(bool silent) {
#if AllowUpdate
            using (ZipWebClient web = new ZipWebClient()) {
                string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                int index = assemblyInfo.IndexOf("AssemblyVersion(");
                if (index > 0) {
                    int indexEnd = assemblyInfo.IndexOf("\")", index);
                    Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                    if (newVersion > Assembly.GetEntryAssembly().GetName().Version) {
                        if (silent || MessageBox.Show(this, $"There is a new version of Fall Guy Stats available (v{newVersion.ToString(2)}). Do you wish to update now?", "Update Program", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            byte[] data = web.DownloadData($"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStats.zip");
                            string exeName = null;
                            using (MemoryStream ms = new MemoryStream(data)) {
                                using (ZipArchive zipFile = new ZipArchive(ms, ZipArchiveMode.Read)) {
                                    foreach (var entry in zipFile.Entries) {
                                        if (entry.Name.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) > 0) {
                                            exeName = entry.Name;
                                        }
                                        File.Move(entry.Name, $"{entry.Name}.bak");
                                        entry.ExtractToFile(entry.Name, true);
                                    }
                                }
                            }

                            Process.Start(new ProcessStartInfo(exeName));
                            Visible = false;
                            Close();
                            return true;
                        }
                    } else if (!silent) {
                        MessageBox.Show(this, "You are at the latest version.", "Updater", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                } else if (!silent) {
                    MessageBox.Show(this, "Could not determine version.", "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
#else
            LaunchHelpInBrowser();
#endif
            return false;
        }
        private async void menuSettings_Click(object sender, EventArgs e) {
            try {
                using (Settings settings = new Settings()) {
                    settings.Icon = Icon;
                    settings.CurrentSettings = CurrentSettings;
                    string lastLogPath = CurrentSettings.LogPath;

                    if (settings.ShowDialog(this) == DialogResult.OK) {
                        CurrentSettings = settings.CurrentSettings;
                        SaveUserSettings();

                        UpdateHoopsieLegends();

                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(CurrentSettings.LogPath) || (!string.IsNullOrEmpty(lastLogPath) && lastLogPath.Equals(CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await logFileRewriter.Stop();
                            await logFile.Stop();

                            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(CurrentSettings.LogPath)) {
                                logPath = CurrentSettings.LogPath;
                            }
                            logFileRewriter.Start(logPath, LOGNAME);
                            logFile.Start(logPath, LOGNAME);
                        }

                        overlay.ArrangeDisplay(CurrentSettings.FlippedDisplay, CurrentSettings.ShowOverlayTabs, CurrentSettings.HideWinsInfo, CurrentSettings.HideRoundInfo, CurrentSettings.HideTimeInfo, CurrentSettings.OverlayColor, CurrentSettings.OverlayWidth, CurrentSettings.OverlayHeight, CurrentSettings.OverlayFontSerialized);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuOverlay_Click(object sender, EventArgs e) {
            ToggleOverlay(overlay);
        }
        private void ToggleOverlay(Overlay overlay) {
            if (overlay.Visible) {
                overlay.Hide();
                CurrentSettings.OverlayLocationX = overlay.Location.X;
                CurrentSettings.OverlayLocationY = overlay.Location.Y;
                CurrentSettings.OverlayWidth = overlay.Width;
                CurrentSettings.OverlayHeight = overlay.Height;
                CurrentSettings.OverlayVisible = false;
                SaveUserSettings();
            } else {
                overlay.TopMost = !CurrentSettings.OverlayNotOnTop;
                overlay.Show();

                CurrentSettings.OverlayVisible = true;
                SaveUserSettings();

                if (CurrentSettings.OverlayLocationX.HasValue && IsOnScreen(CurrentSettings.OverlayLocationX.Value, CurrentSettings.OverlayLocationY.Value, overlay.Width)) {
                    overlay.Location = new Point(CurrentSettings.OverlayLocationX.Value, CurrentSettings.OverlayLocationY.Value);
                } else {
                    overlay.Location = Location;
                }
            }
        }
        private void menuHelp_Click(object sender, EventArgs e) {
            LaunchHelpInBrowser();
        }
        private void infoStrip_MouseEnter(object sender, EventArgs e) {
            Cursor = Cursors.Hand;
        }
        private void infoStrip_MouseLeave(object sender, EventArgs e) {
            Cursor = Cursors.Default;
        }
        private void menuLaunchFallGuys_Click(object sender, EventArgs e) {
            try {
                LaunchGame(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool IsOnScreen(int x, int y, int w) {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens) {
                if (screen.WorkingArea.Contains(new Point(x, y)) || screen.WorkingArea.Contains(new Point(x + w, y))) {
                    return true;
                }
            }

            return false;
        }
    }
}