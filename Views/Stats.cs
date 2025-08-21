using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using Microsoft.Win32;

namespace FallGuysStats {
    public partial class Stats : MetroFramework.Forms.MetroForm {
        [STAThread]
        private static void Main() {
            try {
                bool isAppUpdated = false;
#if AllowUpdate
                if (File.Exists($"{CURRENTDIR}{Path.GetFileName(Assembly.GetEntryAssembly().Location)}.bak")) {
                    isAppUpdated = true;
                }
#endif
                if (isAppUpdated || !IsAlreadyRunning()) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    SplashForm splashWindow = new SplashForm();
                    Screen primaryScreen = Screen.PrimaryScreen;
                    Rectangle workingArea = primaryScreen.WorkingArea;
                    int x = workingArea.Left + (workingArea.Width - splashWindow.Width) / 2;
                    int y = workingArea.Top + (workingArea.Height - splashWindow.Height) / 2;
                    splashWindow.Location = new Point(x, y);
                    splashWindow.Show();
                    splashWindow.Refresh();
                    splashWindow.TopMost = true;
                    splashWindow.TopMost = false;
                    Application.DoEvents();

                    Stats statsForm = new Stats();
                    splashWindow.Close();
                    Application.Run(statsForm);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), @"Run Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SetEventWaitHandle() {
            EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "FallGuysStatsEventWaitHandle", out bool createdNew);
            if (!createdNew) {
                Application.Exit();
            } else {
                Task.Run(() => {
                    while (eventWaitHandle.WaitOne()) {
                        this.Invoke((Action)(() => {
                            this.Visible = true;
                            this.TopMost = true;
                            this.TopMost = false;
                        }));
                    }
                });
            }
        }
        
        private static bool IsAlreadyRunning() {
            try {
                string currentProcessName = Process.GetCurrentProcess().ProcessName;
                foreach (var process in Process.GetProcessesByName(currentProcessName)) {
                    if (process.Id != Process.GetCurrentProcess().Id) {
                        EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting("FallGuysStatsEventWaitHandle");
                        eventWaitHandle.Set();
                        // Utils.ShowWindow(process.MainWindowHandle, 9);
                        // Utils.SetForegroundWindow(process.MainWindowHandle);
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), @"Process Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }
        
        public static readonly string CURRENTDIR = AppDomain.CurrentDomain.BaseDirectory;
        
        private static readonly string LOGFILENAME = "Player.log";
        
        public static readonly (string Name, DateTime StartDate)[] Seasons = new (string Name, DateTime StartDate)[] {
           ("S1", new DateTime(2020, 8, 4, 0, 0, 0, DateTimeKind.Utc)),     // Season 1 (Ultimate Knockout)
           ("S2", new DateTime(2020, 10, 8, 0, 0, 0, DateTimeKind.Utc)),    // Season 2 (Medieval Knockout)
           ("S3", new DateTime(2020, 12, 15, 0, 0, 0, DateTimeKind.Utc)),   // Season 3 (Winter Knockout)
           ("S4", new DateTime(2021, 3, 22, 0, 0, 0, DateTimeKind.Utc)),    // Season 4 (Fall Guys 4041)
           ("S5", new DateTime(2021, 7, 20, 0, 0, 0, DateTimeKind.Utc)),    // Season 5 (Jungle Adventure)
           ("S6", new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc)),   // Season 6 (Party Spectacular)
           ("SS1", new DateTime(2022, 6, 21, 0, 0, 0, DateTimeKind.Utc)),   // Season 1 (Free For All)
           ("SS2", new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc)),   // Season 2 (Satellite Scramble)
           ("SS3", new DateTime(2022, 11, 22, 0, 0, 0, DateTimeKind.Utc)),  // Season 3 (Sunken Secrets)
           ("SS4", new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Utc)),   // Season 4 (Creative Construction)
           ("10.3", new DateTime(2023, 8, 16, 0, 0, 0, DateTimeKind.Utc)),  // Summer Breeze Update
           ("10.4", new DateTime(2023, 9, 27, 0, 0, 0, DateTimeKind.Utc)),  // Fall Force Update
           ("10.5", new DateTime(2023, 11, 7, 0, 0, 0, DateTimeKind.Utc)),  // Tool Up Update
           ("10.6", new DateTime(2023, 12, 6, 0, 0, 0, DateTimeKind.Utc)),  // Power Party Update
           ("10.7", new DateTime(2024, 1, 23, 0, 0, 0, DateTimeKind.Utc)),  // Shapes and Stickers Update
           ("10.8", new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc)),  // Survival Update
           ("10.9", new DateTime(2024, 5, 4, 0, 0, 0, DateTimeKind.Utc)),   // Fall Forever Update
           ("11.0", new DateTime(2024, 6, 11, 0, 0, 0, DateTimeKind.Utc)),  // June '24 Update
           ("11.1", new DateTime(2024, 7, 23, 0, 0, 0, DateTimeKind.Utc)),  // July '24 Update
           ("11.2", new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc)),   // Scrapyard Stumble Update
           ("11.3", new DateTime(2024, 10, 8, 0, 0, 0, DateTimeKind.Utc)),  // Falloween 2024 Update
           ("11.4", new DateTime(2024, 11, 7, 0, 0, 0, DateTimeKind.Utc)),  // November '24 Update
           ("11.5", new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc)), // Winter Update
           ("11.6", new DateTime(2025, 2, 4, 0, 0, 0, DateTimeKind.Utc)),   // Fall and Fantasy Update
           ("18.0", new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc)),   // Ranked Knockout Update
           ("19.0", new DateTime(2025, 5, 27, 0, 0, 0, DateTimeKind.Utc)),  // Yeetropolis Update
           ("20.0", new DateTime(2025, 7, 29, 0, 0, 0, DateTimeKind.Utc)),  // Tropical Tides Update
        };
        private static DateTime SeasonStart, WeekStart, DayStart;
        private static DateTime SessionStart = DateTime.UtcNow;
        public static Language CurrentLanguage;
        public static MetroThemeStyle CurrentTheme = MetroThemeStyle.Light;
        public static bool InstalledEmojiFont;
        
        public static bool InShow = false; 
        public static bool EndedShow = false;
        
        public static bool IsDisplayOverlayTime = true;
        public static bool IsDisplayOverlayPing = true;
        public static bool IsOverlayRoundInfoNeedRefresh;
        
        public static bool IsGameRunning = false;
        public static bool IsClientHasBeenClosed = false;
        
        public static bool IsConnectedToServer = false;
        public static DateTime ConnectedToServerDate = DateTime.MinValue;
        public static string LastServerIp = string.Empty;
        public static string LastCountryAlpha2Code = string.Empty;
        public static string LastCountryRegion = string.Empty;
        public static string LastCountryCity = string.Empty;
        public static long LastServerPing = 0;
        public static bool IsBadServerPing = false;
        
        public static readonly List<string> SucceededPlayerIds = new List<string>();
        public static readonly List<string> EliminatedPlayerIds = new List<string>();
        public static readonly Dictionary<string, string> ReadyPlayerIds = new Dictionary<string, string>();
        
        public static int CasualRoundNum { get; set; }
        public static string SavedSessionId { get; set; }
        public static int SavedRoundCount { get; set; }
        public static int NumPlayersSucceeded { get; set; }
        public static int NumPlayersPsSucceeded { get; set; }
        public static int NumPlayersXbSucceeded { get; set; }
        public static int NumPlayersSwSucceeded { get; set; }
        public static int NumPlayersMbSucceeded { get; set; }
        public static int NumPlayersPcSucceeded { get; set; }
        public static int NumPlayersEliminated { get; set; }
        public static int NumPlayersPsEliminated { get; set; }
        public static int NumPlayersXbEliminated { get; set; }
        public static int NumPlayersSwEliminated { get; set; }
        public static int NumPlayersMbEliminated { get; set; }
        public static int NumPlayersPcEliminated { get; set; }
        public static bool IsLastRoundRunning { get; set; }
        public static bool IsLastPlayedRoundStillPlaying { get; set; }
        public static DateTime LastGameStart { get; set; } = DateTime.MinValue;
        public static DateTime LastRoundLoad { get; set; } = DateTime.MinValue;
        public static DateTime LastCreativeRoundLoad { get; set; } = DateTime.MinValue;
        public static DateTime? LastPlayedRoundStart { get; set; }
        public static DateTime? LastPlayedRoundEnd { get; set; }
        
        public static bool IsQueued = false;
        public static int QueuedPlayers = 0;
        
        private static readonly FallalyticsReporter FallalyticsReporter = new FallalyticsReporter();
        
        public static string OnlineServiceId = string.Empty;
        public static string OnlineServiceNickname = string.Empty;
        public static OnlineServiceTypes OnlineServiceType = OnlineServiceTypes.None;
        public static string HostCountryCode = string.Empty;
        
        public static bool UseWebProxy;
        public static string ProxyAddress = string.Empty;
        public static string ProxyPort = string.Empty;
        public static bool EnableProxyAuthentication;
        public static string ProxyUsername = string.Empty;
        public static string ProxyPassword = string.Empty;
        public static bool SucceededTestProxy;
        
        public static int IpGeolocationService;
        public static string IPinfoToken;
        public static readonly string IPinfoTokenFileName = "IPinfo.io.txt";
        
        readonly DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        readonly DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public List<RoundInfo> CurrentRound = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup;
        public List<LevelStats> StatDetails;
        private readonly LogFileWatcher logFile = new LogFileWatcher();
        private int Shows, Rounds, CustomAndCasualShows, CustomAndCasualRounds;
        private TimeSpan Duration;
        private int Wins, Finals, Kudos;
        private int GoldMedals, SilverMedals, BronzeMedals, PinkMedals, EliminatedMedals;
        private int CustomAndCasualGoldMedals, CustomAndCasualSilverMedals, CustomAndCasualBronzeMedals, CustomAndCasualPinkMedals, CustomAndCasualEliminatedMedals;
        private int nextShowID;
        private bool loadingExisting;
        private bool updateFilterType, updateFilterRange;
        private DateTime customfilterRangeStart = DateTime.MinValue;
        private DateTime customfilterRangeEnd = DateTime.MaxValue;
        private int selectedCustomTemplateSeason = -1;
        private bool updateSelectedProfile, useLinkedProfiles;
        public LiteDatabase StatsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        public ILiteCollection<UserSettings> UserSettings;
        public ILiteCollection<Profiles> Profiles;
        public ILiteCollection<FallalyticsPbLog> FallalyticsPbLog;
        public ILiteCollection<FallalyticsCrownLog> FallalyticsCrownLog;
        public ILiteCollection<ServerConnectionLog> ServerConnectionLog;
        public ILiteCollection<PersonalBestLog> PersonalBestLog;
        public ILiteCollection<UpcomingShow> UpcomingShow;
        public ILiteCollection<LevelTimeLimit> LevelTimeLimit;
        public List<Profiles> AllProfiles = new List<Profiles>();
        public UserSettings CurrentSettings;
        public List<FallalyticsPbLog> FallalyticsPbLogCache = new List<FallalyticsPbLog>();
        public List<FallalyticsCrownLog> FallalyticsCrownLogCache = new List<FallalyticsCrownLog>();
        public List<ServerConnectionLog> ServerConnectionLogCache = new List<ServerConnectionLog>();
        public List<PersonalBestLog> PersonalBestLogCache = new List<PersonalBestLog>();
        public List<UpcomingShow> UpcomingShowCache = new List<UpcomingShow>();
        public List<LevelTimeLimit> LevelTimeLimitCache = new List<LevelTimeLimit>();
        public readonly Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        private readonly DateTime startupTime = DateTime.UtcNow;
        public DateTime overallRankLoadTime {  get; set; }
        public DateTime weeklyCrownLoadTime {  get; set; }
        public int totalOverallRankPlayers, totalWeeklyCrownPlayers;
        private int askedPreviousShows;
        private readonly TextInfo textInfo;
        private int currentProfile, currentLanguage;
        private Color infoStripForeColor;
        public List<ToolStripMenuItem> ProfileMenuItems = new List<ToolStripMenuItem>();
        public List<ToolStripMenuItem> ProfileTrayItems = new List<ToolStripMenuItem>();

        private readonly Image numberOne = Utils.ImageOpacity(Properties.Resources.number_1,   0.5F);
        private readonly Image numberTwo = Utils.ImageOpacity(Properties.Resources.number_2,   0.5F);
        private readonly Image numberThree = Utils.ImageOpacity(Properties.Resources.number_3, 0.5F);
        private readonly Image numberFour = Utils.ImageOpacity(Properties.Resources.number_4,  0.5F);
        private readonly Image numberFive = Utils.ImageOpacity(Properties.Resources.number_5,  0.5F);
        private readonly Image numberSix = Utils.ImageOpacity(Properties.Resources.number_6,   0.5F);
        private readonly Image numberSeven = Utils.ImageOpacity(Properties.Resources.number_7, 0.5F);
        private readonly Image numberEight = Utils.ImageOpacity(Properties.Resources.number_8, 0.5F);
        private readonly Image numberNine = Utils.ImageOpacity(Properties.Resources.number_9,  0.5F);

        private bool maximizedForm;
        private bool isFocused, isFormClosing;
        private bool shiftKeyToggle, ctrlKeyToggle;
        private MetroToolTip mtt = new MetroToolTip();
        private MetroToolTip cmtt = new MetroToolTip();
        private MetroToolTip omtt = new MetroToolTip();
        private DWM_WINDOW_CORNER_PREFERENCE windowConerPreference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;
        private string mainWndTitle;
        private bool isStartingUp = true;
        public bool isUpdate;
        private bool isAvailableNewVersion;
        private string availableNewVersion;
        private int profileWithLinkedCustomShow = -1;
        private Toast toast;
        public List<OverallRank.Player> leaderboardOverallRankList;
        public List<WeeklyCrown.Player> weeklyCrownList;
        public string weeklyCrownNext;
        public string weeklyCrownPrevious;
        public int weeklyCrownCurrentYear;
        public int weeklyCrownCurrentWeek;
        public string weeklyCrownPeriod;
        public Point screenCenter;
        private System.Threading.Timer upcomingShowTimer;
        private System.Threading.Timer levelTimeLimitTimer;

        public event Action OnUpdatedLevelRows;
        private readonly System.Windows.Forms.Timer scrollTimer = new System.Windows.Forms.Timer { Interval = 100 };
        private bool isScrollingStopped = true;
        
        public readonly string[] PublicShowIdList = {
            "ranked_solo_show",
            "main_show",
            "squads_2player_template",
            "squads_3player_template",
            "squads_4player",
            "event_xtreme_fall_guys_template",
            "event_xtreme_fall_guys_squads_template",
            "no_elimination_show",
            "anniversary_fp12_ltm",
            "event_anniversary_season_1_alternate_name",
            "event_blast_ball_banger_template",
            "event_only_button_bashers_template",
            "event_only_finals_v3_template",
            "event_only_races_any_final_template",
            "event_only_fall_ball_template",
            "event_only_hexaring_template",
            "event_only_floor_fall_template",
            "event_only_floor_fall_low_grav",
            "event_only_blast_ball_trials_template",
            "event_only_thin_ice_template",
            "event_only_slime_climb",
            "event_only_slime_climb_2_template",
            "event_only_jump_club_template",
            "event_only_hoverboard_template",
            "event_only_drumtop_template",
            "event_only_skeefall_timetrial_s6_1",
            "event_only_roll_out",
            "event_walnut_template",
            "event_yeetus_template",
            "survival_of_the_fittest",
            "show_robotrampage_ss2_show1_template",
            "event_le_anchovy_template",
            "event_pixel_palooza_template",
            "event_snowday_stumble",
            "mrs_pegwin_winter_2teamsfinal",
            "xtreme_party",
            "invisibeans_mode",
            "timeattack_mode",
            "fall_guys_creative_mode",
            "private_lobbies"
        };
        
        public readonly string[] PublicShowIdList2 = {
            "ranked_show_knockout",
            "knockout_mode",
            // "knockout_duos",
            // "knockout_squads",
            "no_elimination_explore",
            "greatestsquads_ltm",
            "teams_show_ltm",
            "sports_show",
            "showcase_fp19",
            "showcase_fp20",
            "event_day_at_races_squads_template",
            "event_only_ss2_squads_template",
            "squadcelebration",
            "invisibeans_pistachio_template",
            "xtreme_explore"
        };
        
        public string GetUserCreativeLevelTypeId(string gameModeId) {
            switch (gameModeId) {
                case "GAMEMODE_GAUNTLET": return "user_creative_race_round";
                case "GAMEMODE_SURVIVAL": return "user_creative_survival_round";
                case "GAMEMODE_POINTS": return "user_creative_hunt_round";
                case "GAMEMODE_LOGIC": return "user_creative_logic_round";
                case "GAMEMODE_TEAM": return "user_creative_team_round";
                default: return "user_creative_race_round";
            }
        }
        
        private string GetCreativeLevelTypeId(LevelType type, bool isFinal) {
            switch (type) {
                case LevelType.Race: return isFinal ? "creative_race_final_round" : "creative_race_round";
                case LevelType.Survival: return isFinal ? "creative_survival_final_round" : "creative_survival_round";
                case LevelType.Hunt: return isFinal ? "creative_hunt_final_round" : "creative_hunt_round";
                case LevelType.Logic: return isFinal ? "creative_logic_final_round" : "creative_logic_round";
                case LevelType.Team: return isFinal ? "creative_team_final_round" : "creative_team_round";
                default: return "unknown";
            }
        }
        
        private LevelType GetCreativeLevelType(string gameModeId) {
            switch (gameModeId) {
                case "GAMEMODE_GAUNTLET": return LevelType.Race;
                case "GAMEMODE_SURVIVAL": return LevelType.Survival;
                case "GAMEMODE_POINTS": return LevelType.Hunt;
                case "GAMEMODE_LOGIC": return LevelType.Logic;
                case "GAMEMODE_TEAM": return LevelType.Team;
                default: return LevelType.Unknown;
            }
        }
        
        private BestRecordType GetBestRecordType(string gameModeId) {
            switch (gameModeId) {
                case "GAMEMODE_GAUNTLET": return BestRecordType.Fastest;
                case "GAMEMODE_SURVIVAL": return BestRecordType.Longest;
                case "GAMEMODE_POINTS": return BestRecordType.Fastest;
                case "GAMEMODE_LOGIC": return BestRecordType.Fastest; // or Longest
                case "GAMEMODE_TEAM": return BestRecordType.HighScore;
                default: return BestRecordType.Fastest;
            }
        }
        
        private void DatabaseMigration() {
            if (File.Exists($"{CURRENTDIR}data.db")) {
                using (var sourceDb = new LiteDatabase($@"{CURRENTDIR}data.db")) {
                    if (sourceDb.UserVersion != 0) return;

                    using (var targetDb = new LiteDatabase($@"Filename={CURRENTDIR}data_new.db;Upgrade=true")) {
                        string[] tableNames = { "Profiles", "RoundDetails", "UserSettings", "ServerConnectionLog", "PersonalBestLog", "FallalyticsPbLog", "FallalyticsCrownLog" };
                        foreach (var tableName in tableNames) {
                            if (!sourceDb.CollectionExists(tableName)) continue;
                            var sourceData = sourceDb.GetCollection(tableName).FindAll();
                            var targetCollection = targetDb.GetCollection(tableName);
                            targetCollection.InsertBulk(sourceData);
                        }
                        targetDb.UserVersion += 1;
                    }
                }
                File.Move($"{CURRENTDIR}data.db", $"{CURRENTDIR}data.db_bak");
                File.Move($"{CURRENTDIR}data_new.db", $"{CURRENTDIR}data.db");
            }
        }

        private void SetWindowCorner() {
            Utils.DwmSetWindowAttribute(this.menu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuUsefulThings.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuFallGuysDB.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.menuFallalytics.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayCMenu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayUsefulThings.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayFallGuysDB.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            Utils.DwmSetWindowAttribute(this.trayFallalytics.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
        }

        public struct LevelTimeLimitInfo {
            public int version { get; set; }
            public ShowData data { get; set; }
            public struct ShowData {
                public List<Roundpool> roundpools { get; set; }
                public struct Roundpool {
                    public string id { get; set; }
                    public List<Level> levels { get; set; }
                    public struct Level {
                        public string id { get; set; }
                        public int duration { get; set; }
                    }
                }
            }
        }

        public struct FGDB_UpcomingShowInfo {
            public bool ok { get; set; }
            public ShowData data { get; set; }

            public struct ShowData {
                public bool has_more { get; set; }
                public int total_shows { get; set; }
                public List<Show> shows { get; set; }

                public struct Show {
                    public string id { get; set; }
                    public string name { get; set; }
                    public string display_name { get; set; }
                    public string show_image { get; set; }
                    public string show_section_id { get; set; }
                    public string tile_colour { get; set; }
                    public DateTime? starts { get; set; }
                    public DateTime? ends { get; set; }
                    public string description { get; set; }
                    public int min_party_size { get; set; }
                    public int max_party_size { get; set; }
                    public ShowTag show_tag { get; set; }
                    public ShowType show_type { get; set; }
                    public Size size { get; set; }
                    public List<Rewards> rewards { get; set; }
                    public List<Level> rounds { get; set; }

                    public struct ShowTag {
                        public string name { get; set; }
                        public string icon { get; set; }
                    }
                    public struct ShowType {
                        public string type { get; set; }
                        public int squad_size { get; set; }
                    }
                    public struct Size {
                        public string type { get; set; }
                        public int min { get; set; }
                        public int max { get; set; }
                    }
                    public struct Rewards {
                        public string type { get; set; }
                        public int value { get; set; }
                    }

                    public struct Level {
                        public string id { get; set; }
                        public string display_name { get; set; }
                        public string share_code { get; set; }
                        public bool is_creative_level { get; set; }
                        public string creative_game_mode_id { get; set; }
                        public string level_archetype { get; set; }
                        public bool is_final { get; set; }
                        public int max_players { get; set; }
                        public int min_players { get; set; }
                        public int[] can_only_be_on_these_stages { get; set; }
                        public int[] cannot_be_on_these_stages { get; set; }
                    }
                }
            }
        }

        public struct FGA_UpcomingShowInfo {
            public string xstatus { get; set; }
            public ShowData shows { get; set; }

            public struct ShowData {
                public List<LiveShow> live_shows { get; set; }

                public struct LiveShow {

                    [JsonExtensionData]
                    public Dictionary<string, JsonElement> showInfo { get; set; }
                    public string section_name { get; set; }

                    public struct Show {
                        public string id { get; set; }
                        public string show_name { get; set; }
                        public string show_desc { get; set; }
                        public long begins { get; set; }
                        public long ends { get; set; }
                        public string roundpool { get; set; }
                        public string image { get; set; }
                        public Dictionary<string, int> victory_rewards { get; set; }

                        public struct Rewards {
                            public int? fame { get; set; }
                            public int? kudos { get; set; }
                            public int? shards { get; set; }
                            public int? crowns { get; set; }
                        }
                    }
                }
            }
        }

        public struct FGA_RoundpoolInfo {
            public string xstatus { get; set; }
            public LevelData shows { get; set; }

            public struct LevelData {
                public Roundpool roundpool { get; set; }

                public struct Roundpool {

                    [JsonExtensionData]
                    public Dictionary<string, JsonElement> roundpoolInfo { get; set; }

                    public struct Level {
                        public string name { get; set; }
                        public string id { get; set; }
                        public string archetype { get; set; }
                        public string creative_gamemode { get; set; }
                        public string type { get; set; }
                        public int[] cannot_be_on_stages { get; set; }
                        public int[] can_only_be_on_stages { get; set; }
                        public int? min_players { get; set; }
                        public int? max_players { get; set; }
                        public int? time_remaining { get; set; }
                        public string wushu_id { get; set; }

                        [JsonExtensionData]
                        public Dictionary<string, JsonElement> wushu_author { get; set; }
                        public bool is_final { get; set; }
                    }
                }
            }
        }

        private void UpdateUpcomingShow() {
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    string json = web.DownloadString($"{Utils.FALLGUYSDB_API_URL}upcoming-shows");
                    FGDB_UpcomingShowInfo upcomingShow = System.Text.Json.JsonSerializer.Deserialize<FGDB_UpcomingShowInfo>(json);
                    if (upcomingShow.ok) {
                        var temps = new List<UpcomingShow>();
                        foreach (var show in upcomingShow.data.shows.Where(s => s.starts <= DateTime.UtcNow)) {
                            foreach (var level in show.rounds.Where(r => r.is_creative_level)) {
                                if (this.UpcomingShowCache.Exists(u => string.Equals(u.LevelId, level.id)
                                                                       && (string.IsNullOrEmpty(u.DisplayName) || Equals(u.LevelType, LevelType.Unknown)))) {
                                    this.StatsDB.BeginTrans();
                                    this.UpcomingShowCache.RemoveAll(u => string.IsNullOrEmpty(u.DisplayName) || Equals(u.LevelType, LevelType.Unknown));
                                    this.UpcomingShow.DeleteAll();
                                    this.UpcomingShow.InsertBulk(this.UpcomingShowCache);
                                    this.StatsDB.Commit();
                                }
                                if (!this.UpcomingShowCache.Exists(u => string.Equals(u.LevelId, level.id))
                                    && !LevelStats.ALL.ContainsKey(this.ReplaceLevelIdInShuffleShow(show.id, level.id))) {
                                    var temp = new UpcomingShow {
                                        ShowId = show.id,
                                        LevelId = level.id,
                                        DisplayName = level.display_name,
                                        ShareCode = level.share_code,
                                        IsFinal = level.is_final,
                                        IsCreative = true,
                                        LevelType = this.GetCreativeLevelType(level.creative_game_mode_id),
                                        BestRecordType = this.GetBestRecordType(level.creative_game_mode_id),
                                        AddDate = this.startupTime
                                    };
                                    temps.Add(temp);
                                }
                            }
                        }
                        
                        if (temps.Count > 0) {
                            this.StatsDB.BeginTrans();
                            this.UpcomingShow.InsertBulk(temps);
                            this.StatsDB.Commit();
                        }
                    } else {
                        throw new Exception("FallGuysDB API is unavailable => Try FGAnalyst API now");
                    }
                } catch {
                    try {
                        string json = web.DownloadString($"{Utils.FGANALYST_API_URL}show-selector/");
                        FGA_UpcomingShowInfo upcomingShow = System.Text.Json.JsonSerializer.Deserialize<FGA_UpcomingShowInfo>(json);
                        if (string.Equals(upcomingShow.xstatus, "success")) {
                            var selectedShows = new List<FGA_UpcomingShowInfo.ShowData.LiveShow.Show>();
                            foreach (var liveShow in upcomingShow.shows.live_shows) {
                                foreach (var showInfo in liveShow.showInfo.Values) {
                                    var show = showInfo.Deserialize<FGA_UpcomingShowInfo.ShowData.LiveShow.Show>();
                                    if (string.Equals(liveShow.section_name, "CLASSIC GAMES")) {
                                        if (this.IsCreativeShow(show.id) || string.Equals(show.id, "event_snowday_stumble")) {
                                            selectedShows.Add(show);
                                        }
                                    } else if (!string.Equals(show.id, "ftue_uk_show") && show.victory_rewards.Count != 0) {
                                        selectedShows.Add(show);
                                    }
                                }
                            }
                            var temps = new List<UpcomingShow>();
                            foreach (var show in selectedShows) {
                                if (show.begins <= DateTimeOffset.UtcNow.ToUnixTimeSeconds()) {
                                    json = web.DownloadString($"{Utils.FGANALYST_API_URL}show-roundpools/?roundpool={show.roundpool}");
                                    FGA_RoundpoolInfo roundpool = System.Text.Json.JsonSerializer.Deserialize<FGA_RoundpoolInfo>(json);
                                    if (string.Equals(roundpool.xstatus, "success")) {
                                        foreach (var levelInfo in roundpool.shows.roundpool.roundpoolInfo.Values) {
                                            var level = levelInfo.Deserialize<FGA_RoundpoolInfo.LevelData.Roundpool.Level>();
                                            if (string.Equals(level.type, "wushu")) {
                                                if (this.UpcomingShowCache.Exists(u => string.Equals(u.LevelId, level.id) && (string.IsNullOrEmpty(u.DisplayName) || Equals(u.LevelType, LevelType.Unknown)))) {
                                                    this.StatsDB.BeginTrans();
                                                    this.UpcomingShowCache.RemoveAll(u => string.IsNullOrEmpty(u.DisplayName) || Equals(u.LevelType, LevelType.Unknown));
                                                    this.UpcomingShow.DeleteAll();
                                                    this.UpcomingShow.InsertBulk(this.UpcomingShowCache);
                                                    this.StatsDB.Commit();
                                                }
                                                if (!this.UpcomingShowCache.Exists(u => string.Equals(u.LevelId, level.id))
                                                    && !LevelStats.ALL.ContainsKey(this.ReplaceLevelIdInShuffleShow(show.id, level.id))) {
                                                    var temp = new UpcomingShow {
                                                        ShowId = show.id,
                                                        LevelId = level.id,
                                                        DisplayName = level.name,
                                                        ShareCode = level.wushu_id,
                                                        IsFinal = level.is_final,
                                                        IsCreative = true,
                                                        LevelType = this.GetCreativeLevelType(level.creative_gamemode),
                                                        BestRecordType = this.GetBestRecordType(level.creative_gamemode),
                                                        AddDate = this.startupTime
                                                    };
                                                    temps.Add(temp);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                            if (temps.Count > 0) {
                                this.StatsDB.BeginTrans();
                                this.UpcomingShow.InsertBulk(temps);
                                this.StatsDB.Commit();
                            }
                        }
                    } catch {
                        // ignored
                    }
                }
            }
        }

        private void GenerateLevelStats() {
            List<string> removableLevelsInUpcomingShow = new List<string>();
            this.UpcomingShowCache = this.UpcomingShow.FindAll().ToList();
            foreach (var level in this.UpcomingShowCache) {
                if (string.IsNullOrEmpty(level.DisplayName) || Equals(level.LevelType, LevelType.Unknown)) {
                    removableLevelsInUpcomingShow.Add(level.LevelId);
                } else if (!LevelStats.ALL.ContainsKey(level.LevelId)) {
                    LevelStats.ALL.Add(level.LevelId, new LevelStats(level.LevelId, level.ShareCode, level.DisplayName, level.LevelType, level.BestRecordType, level.IsCreative, level.IsFinal,
                        10, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon));
                }
            }
            if (removableLevelsInUpcomingShow.Count > 0) {
                this.StatsDB.BeginTrans();
                this.UpcomingShowCache.RemoveAll(u => removableLevelsInUpcomingShow.Contains(u.LevelId));
                this.UpcomingShow.DeleteAll();
                this.UpcomingShow.InsertBulk(this.UpcomingShowCache);
                this.StatsDB.Commit();
            }
        }

        private void UpdateUpcomingShowJob() {
            DateTime now = DateTime.UtcNow;
            DateTime targetTime = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0);
            if (now > targetTime) {
                targetTime = targetTime.AddDays(1);
            }
            double initialDelay = (targetTime - now).TotalMilliseconds;
            this.upcomingShowTimer = new System.Threading.Timer(state => {
                Task.Run(() => {
                    if (!Utils.IsInternetConnected()) return;

                    this.UpdateUpcomingShow();
                    this.GenerateLevelStats();
                    if (this.UpcomingShowCache.Any()) {
                        lock (this.StatLookup) {
                            this.StatLookup = LevelStats.ALL.ToDictionary(entry => entry.Key, entry => entry.Value);
                        }
                        lock (this.StatDetails) {
                            this.StatDetails = LevelStats.ALL
                                .Where(entry => !string.IsNullOrEmpty(entry.Value.ShareCode))
                                .GroupBy(entry => entry.Value.ShareCode)
                                .Select(group => group.First().Value)
                                .Concat(LevelStats.ALL.Where(entry => string.IsNullOrEmpty(entry.Value.ShareCode)).Select(entry => entry.Value))
                                .ToList();
                        }
                        lock (this.gridDetails) {
                            this.gridDetails.Invoke((MethodInvoker)delegate {
                                this.SortGridDetails(true);
                                IsOverlayRoundInfoNeedRefresh = true;
                            });
                        }
                    }
                });
            }, null, (int)initialDelay, 24 * 60 * 60 * 1000);
        }

        private void UpdateLevelTimeLimit() {
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    string json = web.DownloadString(Utils.FALLGUYSSTATS_LEVEL_TIME_LIMIT_DB_URL);
                    LevelTimeLimitInfo levelTimeLimit = System.Text.Json.JsonSerializer.Deserialize<LevelTimeLimitInfo>(json);
                    if (levelTimeLimit.version > this.CurrentSettings.LevelTimeLimitVersion) {
                        var temps = new List<LevelTimeLimit>();
                        foreach (var roundpool in levelTimeLimit.data.roundpools) {
                            foreach (var level in roundpool.levels) {
                                if (!temps.Exists(l => string.Equals(l.LevelId, level.id))) {
                                    var temp = new LevelTimeLimit {
                                        LevelId = level.id,
                                        Duration = level.duration,
                                    };
                                    temps.Add(temp);
                                }
                            }
                        }

                        this.StatsDB.BeginTrans();
                        this.LevelTimeLimit.DeleteAll();
                        this.LevelTimeLimit.InsertBulk(temps);
                        this.StatsDB.Commit();
                        this.CurrentSettings.LevelTimeLimitVersion = levelTimeLimit.version;
                        this.SaveUserSettings();
                        this.LevelTimeLimitCache = this.LevelTimeLimit.FindAll().ToList();
                    }
                } catch {
                    // ignored
                }
            }
        }

        private void UpdateLevelTimeLimitJob() {
            DateTime now = DateTime.UtcNow;
            DateTime targetTime = new DateTime(now.Year, now.Month, now.Day, 9, 50, 0);
            if (now > targetTime) {
                targetTime = targetTime.AddDays(1);
            }
            double initialDelay = (targetTime - now).TotalMilliseconds;
            this.levelTimeLimitTimer = new System.Threading.Timer(state => {
                Task.Run(() => {
                    if (!Utils.IsInternetConnected()) return;

                    this.UpdateLevelTimeLimit();
                });
            }, null, (int)initialDelay, 24 * 60 * 60 * 1000);
        }

        private void UpdateOnlineDatabases() {
            if (Utils.IsInternetConnected()) {
                this.UpdateLevelTimeLimit();
                this.UpdateUpcomingShow();
                this.GenerateLevelStats();
                if (this.UpcomingShowCache.Any()) {
                    lock (this.StatLookup) {
                        this.StatLookup = LevelStats.ALL.ToDictionary(entry => entry.Key, entry => entry.Value);
                    }
                    lock (this.StatDetails) {
                        this.StatDetails = LevelStats.ALL
                            .Where(entry => !string.IsNullOrEmpty(entry.Value.ShareCode))
                            .GroupBy(entry => entry.Value.ShareCode)
                            .Select(group => group.First().Value)
                            .Concat(LevelStats.ALL.Where(entry => string.IsNullOrEmpty(entry.Value.ShareCode)).Select(entry => entry.Value))
                            .ToList();
                    }
                    lock (this.gridDetails) {
                        this.gridDetails.Invoke((MethodInvoker)delegate {
                            this.SortGridDetails(true);
                            IsOverlayRoundInfoNeedRefresh = true;
                        });
                    }
                }
            }
        }

        private Stats() {
            this.DatabaseMigration();

            this.mainWndTitle = $"     {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.StatsDB = new LiteDatabase($@"{CURRENTDIR}data.db");
            this.StatsDB.Pragma("UTC_DATE", true);
            this.UserSettings = this.StatsDB.GetCollection<UserSettings>("UserSettings");

            if (this.UserSettings.Count() == 0) {
                this.CurrentSettings = this.GetDefaultSettings();
                this.StatsDB.BeginTrans();
                this.UserSettings.Insert(this.CurrentSettings);
                this.StatsDB.Commit();
            } else {
                try {
                    this.CurrentSettings = this.UserSettings.FindAll().First();
                    CurrentLanguage = (Language)this.CurrentSettings.Multilingual;
                    CurrentTheme = this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : MetroThemeStyle.Dark;
                } catch {
                    this.CurrentSettings = GetDefaultSettings();
                    this.StatsDB.BeginTrans();
                    this.UserSettings.DeleteAll();
                    this.UserSettings.Insert(this.CurrentSettings);
                    this.StatsDB.Commit();
                }
            }

            this.RemoveUpdateFiles();

            this.InitializeComponent();

            this.SetEventWaitHandle();

#if !AllowUpdate
            this.menu.Items.Remove(this.menuUpdate);
            this.trayCMenu.Items.Remove(this.trayUpdate);
#endif

            this.ShowInTaskbar = false;
            this.Opacity = 0;
            this.trayCMenu.Opacity = 0;
            this.textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            UseWebProxy = this.CurrentSettings.UseProxyServer;
            ProxyAddress = this.CurrentSettings.ProxyAddress;
            ProxyPort = this.CurrentSettings.ProxyPort;
            EnableProxyAuthentication = this.CurrentSettings.EnableProxyAuthentication;
            ProxyUsername = this.CurrentSettings.ProxyUsername;
            ProxyPassword = this.CurrentSettings.ProxyPassword;
            SucceededTestProxy = this.CurrentSettings.SucceededTestProxy;

            IpGeolocationService = this.CurrentSettings.IpGeolocationService;
            if (File.Exists($"{CURRENTDIR}{IPinfoTokenFileName}")) {
                try {
                    StreamReader sr = new StreamReader($"{CURRENTDIR}{IPinfoTokenFileName}");
                    IPinfoToken = sr.ReadLine();
                    sr.Close();
                } catch {
                    IPinfoToken = string.Empty;
                }
            } else {
                IPinfoToken = string.Empty;
            }

            this.RoundDetails = this.StatsDB.GetCollection<RoundInfo>("RoundDetails");
            this.Profiles = this.StatsDB.GetCollection<Profiles>("Profiles");
            this.ServerConnectionLog = this.StatsDB.GetCollection<ServerConnectionLog>("ServerConnectionLog");
            this.PersonalBestLog = this.StatsDB.GetCollection<PersonalBestLog>("PersonalBestLog");
            this.FallalyticsPbLog = this.StatsDB.GetCollection<FallalyticsPbLog>("FallalyticsPbLog");
            this.FallalyticsCrownLog = this.StatsDB.GetCollection<FallalyticsCrownLog>("FallalyticsCrownLog");
            this.UpcomingShow = this.StatsDB.GetCollection<UpcomingShow>("UpcomingShow");
            this.LevelTimeLimit = this.StatsDB.GetCollection<LevelTimeLimit>("LevelTimeLimit");

            this.StatsDB.BeginTrans();

            this.RoundDetails.EnsureIndex(r => r.Name);
            this.RoundDetails.EnsureIndex(r => r.ShowID);
            this.RoundDetails.EnsureIndex(r => r.Round);
            this.RoundDetails.EnsureIndex(r => r.Start);
            this.RoundDetails.EnsureIndex(r => r.InParty);

            this.Profiles.EnsureIndex(p => p.ProfileId);

            this.ServerConnectionLog.EnsureIndex(f => f.SessionId);
            this.PersonalBestLog.EnsureIndex(f => f.PbDate);

            this.FallalyticsPbLog.EnsureIndex(f => f.PbId);
            this.FallalyticsPbLog.EnsureIndex(f => f.RoundId);
            this.FallalyticsPbLog.EnsureIndex(f => f.ShowId);

            this.FallalyticsCrownLog.EnsureIndex(f => f.Id);
            this.FallalyticsCrownLog.EnsureIndex(f => f.SessionId);

            this.UpcomingShow.EnsureIndex(f => f.LevelId);
            this.LevelTimeLimit.EnsureIndex(f => f.LevelId);

            this.StatsDB.Commit();

            if (this.Profiles.Count() == 0) {
                string sysLang = CultureInfo.CurrentUICulture.Name.StartsWith("zh") ?
                                 CultureInfo.CurrentUICulture.Name :
                                 CultureInfo.CurrentUICulture.Name.Substring(0, 2);
                using (InitLanguage initLanguageForm = new InitLanguage(sysLang)) {
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    if (initLanguageForm.ShowDialog(this) == DialogResult.OK) {
                        CurrentLanguage = initLanguageForm.selectedLanguage;
                        Overlay.SetDefaultFont(18, CurrentLanguage);
                        this.CurrentSettings.Multilingual = (int)initLanguageForm.selectedLanguage;
                        if (initLanguageForm.autoGenerateProfiles) {
                            this.StatsDB.BeginTrans();
                            for (int i = this.PublicShowIdList.Length; i >= 1; i--) {
                                string showId = this.PublicShowIdList[i - 1];
                                this.Profiles.Insert(new Profiles { ProfileId = i - 1, ProfileName = Multilingual.GetShowName(showId), ProfileOrder = i, LinkedShowId = showId, DoNotCombineShows = false });
                            }
                            this.StatsDB.Commit();
                            this.CurrentSettings.AutoChangeProfile = true;
                        } else {
                            this.StatsDB.BeginTrans();
                            this.Profiles.Insert(new Profiles { ProfileId = 5, ProfileName = Multilingual.GetWord("main_profile_creative"), ProfileOrder = 6, LinkedShowId = "fall_guys_creative_mode", DoNotCombineShows = false });
                            this.Profiles.Insert(new Profiles { ProfileId = 4, ProfileName = Multilingual.GetWord("main_profile_custom"), ProfileOrder = 5, LinkedShowId = "private_lobbies", DoNotCombineShows = false });
                            this.Profiles.Insert(new Profiles { ProfileId = 3, ProfileName = Multilingual.GetWord("main_profile_squad"), ProfileOrder = 4, LinkedShowId = "squads_4player", DoNotCombineShows = false });
                            this.Profiles.Insert(new Profiles { ProfileId = 2, ProfileName = Multilingual.GetWord("main_profile_duo"), ProfileOrder = 3, LinkedShowId = "squads_2player_template", DoNotCombineShows = false });
                            this.Profiles.Insert(new Profiles { ProfileId = 1, ProfileName = Multilingual.GetWord("main_profile_ranked_solo"), ProfileOrder = 2, LinkedShowId = "ranked_solo_show", DoNotCombineShows = false });
                            this.Profiles.Insert(new Profiles { ProfileId = 0, ProfileName = Multilingual.GetWord("main_profile_solo"), ProfileOrder = 1, LinkedShowId = "main_show", DoNotCombineShows = false });
                            this.StatsDB.Commit();
                        }
                    }
                    this.EnableInfoStrip(true);
                    this.EnableMainMenu(true);
                }
            }

            this.LevelTimeLimitCache = this.LevelTimeLimit.FindAll().ToList();

            this.GenerateLevelStats();

            this.StatLookup = LevelStats.ALL.ToDictionary(entry => entry.Key, entry => entry.Value);

            this.StatDetails = LevelStats.ALL
                .Where(entry => !string.IsNullOrEmpty(entry.Value.ShareCode))
                .GroupBy(entry => entry.Value.ShareCode)
                .Select(group => group.First().Value)
                .Concat(LevelStats.ALL.Where(entry => string.IsNullOrEmpty(entry.Value.ShareCode)).Select(entry => entry.Value))
                .ToList();

            this.UpdateDatabaseDateFormat();
            this.UpdateDatabaseVersion();
            
            this.BackImage = this.Icon.ToBitmap();
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(18, 18, 0, 0);
            this.SetMinimumSize();
            this.ChangeLanguage();
            this.InitMainDataGridView();
            this.UpdateGridRoundName();
            this.UpdateHoopsieLegends();
            
            Task.Run(() => {
                this.UpdateOnlineDatabases();
                this.UpdateLevelTimeLimitJob();
                this.UpdateUpcomingShowJob();
            });
            
            this.overlay = new Overlay { Text = @"Fall Guys Stats Overlay", StatsForm = this, Icon = this.Icon, ShowIcon = true, BackgroundResourceName = this.CurrentSettings.OverlayBackgroundResourceName, TabResourceName = this.CurrentSettings.OverlayTabResourceName };
            
            Screen screen = Utils.GetCurrentScreen(this.overlay.Location);
            Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
            Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
            this.screenCenter = new Point(screenLocation.X + (screenSize.Width / 2), screenLocation.Y + (screenSize.Height / 2));
            
            this.logFile.OnParsedLogLines += this.LogFile_OnParsedLogLines;
            this.logFile.OnNewLogFileDate += this.LogFile_OnNewLogFileDate;
            this.logFile.OnServerConnectionNotification += this.LogFile_OnServerConnectionNotification;
            this.logFile.OnPersonalBestNotification += this.LogFile_OnPersonalBestNotification;
            this.logFile.OnError += this.LogFile_OnError;
            this.logFile.OnParsedLogLinesCurrent += this.LogFile_OnParsedLogLinesCurrent;
            this.logFile.StatsForm = this;
            
            string fixedPosition = this.CurrentSettings.OverlayFixedPosition;
            this.overlay.SetFixedPosition(
                string.Equals(fixedPosition, "ne"),
                string.Equals(fixedPosition, "nw"),
                string.Equals(fixedPosition, "se"),
                string.Equals(fixedPosition, "sw"),
                string.Equals(fixedPosition, "free")
            );
            if (this.overlay.IsFixed()) this.overlay.Cursor = Cursors.Default;
            this.overlay.Opacity = this.CurrentSettings.OverlayBackgroundOpacity / 100D;
            this.overlay.Show();
            this.overlay.Hide();
            this.overlay.StartTimer();
            
            this.SetSystemTrayIcon(this.CurrentSettings.SystemTrayIcon);
            
            this.UpdateGameExeLocation();
        }
        
        public void cmtt_levelDetails_Draw(object sender, DrawToolTipEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            // Draw the standard background.
            //e.DrawBackground();
            // Draw the custom background.
            e.Graphics.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            
            // Draw the standard border.
            e.DrawBorder();
            // Draw the custom border to appear 3-dimensional.
            //e.Graphics.DrawLines(SystemPens.ControlLightLight, new[] {
            //    new Point (0, e.Bounds.Height - 1), 
            //    new Point (0, 0), 
            //    new Point (e.Bounds.Width - 1, 0)
            //});
            //e.Graphics.DrawLines(SystemPens.ControlDarkDark, new[] {
            //    new Point (0, e.Bounds.Height - 1), 
            //    new Point (e.Bounds.Width - 1, e.Bounds.Height - 1), 
            //    new Point (e.Bounds.Width - 1, 0)
            //});
            
            // Draw the standard text with customized formatting options.
            //e.DrawText(TextFormatFlags.TextBoxControl | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding);
            // Draw the custom text.
            // The using block will dispose the StringFormat automatically.
            //using (StringFormat sf = new StringFormat()) {
            //    sf.Alignment = StringAlignment.Near;
            //    sf.LineAlignment = StringAlignment.Near;
            //    sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            //    sf.FormatFlags = StringFormatFlags.NoWrap;
            //    e.Graphics.DrawString(e.ToolTipText, Overlay.GetMainFont(12), SystemBrushes.ActiveCaptionText, e.Bounds, sf);
            //    //using (Font f = new Font("Tahoma", 9)) {
            //    //    e.Graphics.DrawString(e.ToolTipText, f, SystemBrushes.ActiveCaptionText, e.Bounds, sf);
            //    //}
            //}
            e.Graphics.DrawString(e.ToolTipText, InstalledEmojiFont ? new Font("Segoe UI Emoji", 8.6f) : e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, new PointF(e.Bounds.X + 8, e.Bounds.Y - 8));
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            if (c.Parent != null) {
                Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
                Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
        }
        
        public void cmtt_levelDetails_Draw2(object sender, DrawToolTipEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            e.Graphics.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, new PointF(e.Bounds.X + 8, e.Bounds.Y - 8));
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            if (c.Parent != null) {
                Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
                Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
        }
        
        private void cmtt_overlay_Draw(object sender, DrawToolTipEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            // Draw the custom background.
            e.Graphics.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            
            // Draw the standard border.
            e.DrawBorder();
            
            e.Graphics.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, new PointF(e.Bounds.X + 2, e.Bounds.Y + 2));
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            if (c.Parent != null) {
                Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
                Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
        }
        
        private void cmtt_center_Draw(object sender, DrawToolTipEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            // Draw the custom background.
            e.Graphics.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            
            // Draw the standard border.
            e.DrawBorder();
            
            // Draw the custom text.
            // The using block will dispose the StringFormat automatically.
            using (StringFormat sf = new StringFormat()) {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.HotkeyPrefix = HotkeyPrefix.None;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                e.Graphics.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, e.Bounds, sf);
            }
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            if (c.Parent != null) {
                Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
                Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
        }
        
        public class CustomToolStripSystemRenderer : ToolStripSystemRenderer {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder(e);
            }
        }
        
        public class CustomLightArrowRenderer : ToolStripProfessionalRenderer {
            public CustomLightArrowRenderer() : base(new CustomLightColorTable()) { }
            protected override void OnRenderArrow (ToolStripArrowRenderEventArgs e) {
                //var tsMenuItem = e.Item as ToolStripMenuItem;
                //if (tsMenuItem != null) e.ArrowColor = CurrentTheme == MetroThemeStyle.Dark ? Color.DarkGray : Color.FromArgb(17, 17, 17);
                //Point point = new Point(e.ArrowRectangle.Left + e.ArrowRectangle.Width / 2, e.ArrowRectangle.Top + e.ArrowRectangle.Height / 2);
                //Point[] points = new Point[3]
                //{
                //    new Point(point.X - 2, point.Y - 4),
                //    new Point(point.X - 2, point.Y + 4),
                //    new Point(point.X + 2, point.Y)
                //};
                //e.Graphics.FillPolygon(Brushes.DarkGray, points);
                e.ArrowColor = Color.FromArgb(17, 17, 17);
                base.OnRenderArrow(e);
            }
        }
        
        public class CustomDarkArrowRenderer : ToolStripProfessionalRenderer {
            public CustomDarkArrowRenderer() : base(new CustomDarkColorTable()) { }
            protected override void OnRenderArrow (ToolStripArrowRenderEventArgs e) {
                //var tsMenuItem = e.Item as ToolStripMenuItem;
                //if (tsMenuItem != null) e.ArrowColor = CurrentTheme == MetroThemeStyle.Dark ? Color.DarkGray : Color.FromArgb(17, 17, 17);
                //Point point = new Point(e.ArrowRectangle.Left + e.ArrowRectangle.Width / 2, e.ArrowRectangle.Top + e.ArrowRectangle.Height / 2);
                //Point[] points = new Point[3]
                //{
                //    new Point(point.X - 2, point.Y - 4),
                //    new Point(point.X - 2, point.Y + 4),
                //    new Point(point.X + 2, point.Y)
                //};
                //e.Graphics.FillPolygon(Brushes.DarkGray, points);
                e.ArrowColor = Color.DarkGray;
                base.OnRenderArrow(e);
            }
        }
        
        private class CustomLightColorTable : ProfessionalColorTable {
            public CustomLightColorTable() { UseSystemColors = false; }
            //public override Color ToolStripBorder {
            //    get { return Color.Red; }
            //}
            public override Color MenuBorder {
                get { return Color.White; }
            }
            public override Color ToolStripDropDownBackground {
                get { return Color.White; }
            }
            public override Color MenuItemBorder {
                get { return Color.DarkSeaGreen; }
            }
            public override Color MenuItemSelected {
                get { return Color.LightGreen; }
            }
            //public override Color MenuItemSelectedGradientBegin {
            //    get { return Color.LawnGreen; }
            //}
            //public override Color MenuItemSelectedGradientEnd {
            //    get { return Color.MediumSeaGreen; }
            //}
            //public override Color MenuStripGradientBegin {
            //    get { return Color.AliceBlue; }
            //}
            //public override Color MenuStripGradientEnd {
            //    get { return Color.DodgerBlue; }
            //}
        }
        
        private class CustomDarkColorTable : ProfessionalColorTable {
            public CustomDarkColorTable() { UseSystemColors = false; }
            //public override Color ToolStripBorder {
            //    get { return Color.Red; }
            //}
            public override Color MenuBorder {
                get { return Color.FromArgb(17, 17, 17); }
            }
            public override Color ToolStripDropDownBackground {
                get { return Color.FromArgb(17, 17, 17); }
            }
            public override Color MenuItemBorder {
                get { return Color.DarkSeaGreen; }
            }
            public override Color MenuItemSelected {
                get { return Color.LightGreen; }
            }
            //public override Color MenuItemSelectedGradientBegin {
            //    get { return Color.LawnGreen; }
            //}
            //public override Color MenuItemSelectedGradientEnd {
            //    get { return Color.MediumSeaGreen; }
            //}
            //public override Color MenuStripGradientBegin {
            //    get { return Color.AliceBlue; }
            //}
            //public override Color MenuStripGradientEnd {
            //    get { return Color.DodgerBlue; }
            //}
        }
        
        private TaskbarPosition GetTaskbarPosition() {
            TaskbarPosition taskbarPosition = TaskbarPosition.Bottom;
            Rectangle screenBounds = Screen.GetBounds(Cursor.Position);
            Rectangle workingArea = Screen.GetWorkingArea(Cursor.Position);
            if (workingArea.Width == screenBounds.Width) {
                if (workingArea.Top > 0) { taskbarPosition = TaskbarPosition.Top; }
            } else {
                if (workingArea.Left > screenBounds.Left) {
                    taskbarPosition = TaskbarPosition.Left;
                } else if (workingArea.Right < screenBounds.Right) {
                    taskbarPosition = TaskbarPosition.Right;
                }
            }
            return taskbarPosition;
        }
        
        public void PreventOverlayMouseClicks() {
            this.BeginInvoke((MethodInvoker)delegate {
                if (this.overlay.IsMouseOver() && ActiveForm != this) { this.SetCursorPositionCenter(); }
            });
        }
        
        private void SetCursorPositionCenter() {
            if (this.overlay.Location.X <= this.screenCenter.X && this.overlay.Location.Y <= this.screenCenter.Y) {
                Cursor.Position = new Point(this.screenCenter.X * 2, this.screenCenter.Y * 2); // NW
            } else if (this.overlay.Location.X <= this.screenCenter.X && this.overlay.Location.Y > this.screenCenter.Y) {
                Cursor.Position = new Point(this.screenCenter.X * 2, 0); // SW
            } else if (this.overlay.Location.X > this.screenCenter.X && this.overlay.Location.Y <= this.screenCenter.Y) {
                Cursor.Position = new Point(0, this.screenCenter.Y * 2); // NE
            } else if (this.overlay.Location.X > this.screenCenter.X && this.overlay.Location.Y > this.screenCenter.Y) {
                Cursor.Position = new Point(0, 0); // SE
            }
        }

        public void SetSecretKey() {
            Type type = Type.GetType("FallGuysStats.SecretKey");
            if (type != null) {
                MethodInfo methodInfo = type.GetMethod("VERIFY", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                methodInfo?.Invoke(null, null);
                FieldInfo fieldInfo = type.GetField("FALLALYTICS_KEY", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null) {
                    object value = fieldInfo.GetValue(null);
                    Environment.SetEnvironmentVariable("FALLALYTICS_KEY", value as string);
                } else {
                    Environment.SetEnvironmentVariable("FALLALYTICS_KEY", "");
                }
            } else {
                Environment.SetEnvironmentVariable("FALLALYTICS_KEY", "");
            }
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            this.mtt.Theme = theme;
            this.omtt.Theme = theme;
            this.menu.Renderer = theme == MetroThemeStyle.Light ? new CustomLightArrowRenderer() : new CustomDarkArrowRenderer() as ToolStripRenderer;
            this.trayCMenu.Renderer = theme == MetroThemeStyle.Light ? new CustomLightArrowRenderer() : new CustomDarkArrowRenderer() as ToolStripRenderer;
            foreach (Control c1 in Controls) {
                if (c1 is MenuStrip ms1) {
                    foreach (var item in ms1.Items) {
                        if (item is ToolStripMenuItem tsmi1) {
                            if (Equals(tsmi1, this.menuSettings)) {
                                tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuFilters)) {
                                tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon;
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuProfile)) {
                                tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon;
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuUpdate)) {
                                tsmi1.Image = theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon)
                                                                             : (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon);
                                tsmi1.ForeColor = this.isAvailableNewVersion ? (theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                            } else if (Equals(tsmi1, this.menuHelp)) {
                                tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon;
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuOverlay)) {
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuLaunchFallGuys)) {
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            } else if (Equals(tsmi1, this.menuUsefulThings)) {
                                tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            }
                            
                            tsmi1.MouseEnter += this.menu_MouseEnter;
                            tsmi1.MouseLeave += this.menu_MouseLeave;
                            foreach (var item1 in tsmi1.DropDownItems) {
                                if (item1 is ToolStripMenuItem subTsmi1) {
                                    if (Equals(subTsmi1, this.menuEditProfiles)) { subTsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; }
                                    subTsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    subTsmi1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    subTsmi1.MouseEnter += this.menu_MouseEnter;
                                    subTsmi1.MouseLeave += this.menu_MouseLeave;
                                    foreach (var item2 in subTsmi1.DropDownItems) {
                                        if (item2 is ToolStripMenuItem subTsmi2) {
                                            if (Equals(subTsmi2, this.menuCustomRangeStats)) { subTsmi2.Image = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; }
                                            subTsmi2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                            subTsmi2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                            subTsmi2.MouseEnter += this.menu_MouseEnter;
                                            subTsmi2.MouseLeave += this.menu_MouseLeave;
                                        } else if (item2 is ToolStripSeparator subTss2) {
                                            subTss2.Paint += this.CustomToolStripSeparatorCustom_Paint;
                                            subTss2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                            subTss2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                        }
                                    }
                                } else if (item1 is ToolStripSeparator subTss1) {
                                    subTss1.Paint += this.CustomToolStripSeparatorCustom_Paint;
                                    subTss1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    subTss1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                }
                            }
                        }
                    }
                } else if (c1 is ToolStrip ts1) {
                    ts1.BackColor = Color.Transparent;
                    foreach (var tsi1 in ts1.Items) {
                        if (tsi1 is ToolStripLabel tsl1) {
                            if (Equals(tsl1, this.lblCurrentProfile)) {
                                tsl1.Font = Overlay.GetMainFont(14f);
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192);
                            } else if (Equals(tsl1, this.lblTotalTime)) {
                                tsl1.Font = Overlay.GetMainFont(14f);
                                tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.clock_icon : Properties.Resources.clock_gray_icon;
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                            } else if (Equals(tsl1, this.lblTotalShows) || Equals(tsl1, this.lblTotalWins)) {
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                            } else if (Equals(tsl1, this.lblTotalRounds)) {
                                tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                            } else if (Equals(tsl1, this.lblTotalFinals)) {
                                tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                            } else if (Equals(tsl1, this.lblGoldMedal) || Equals(tsl1, this.lblSilverMedal) ||
                                       Equals(tsl1, this.lblBronzeMedal) || Equals(tsl1, this.lblPinkMedal) ||
                                       Equals(tsl1, this.lblEliminatedMedal) || Equals(tsl1, this.lblKudos)) {
                                tsl1.Font = Overlay.GetMainFont(14f);
                                tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray;
                            }
                        } else if (tsi1 is ToolStripSeparator tss1) {
                            tss1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray; break;
                        }
                    }
                } else if (c1 is MetroToggle mt1) {
                    mt1.Theme = theme;
                } else if (c1 is MetroLink ml1) {
                    ml1.Theme = theme;
                } else if (c1 is Label lbl1) {
                    lbl1.Font = Overlay.GetMainFont(13f);
                    if (Equals(lbl1, this.lblCreativeLevel)) {
                        lbl1.ForeColor = this.mtgCreativeLevel.Checked ? (theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                    } else if (Equals(lbl1, this.lblIgnoreLevelTypeWhenSorting)) {
                        lbl1.ForeColor = this.mtgIgnoreLevelTypeWhenSorting.Checked ? (theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                    }
                }
            }

            this.gridDetails.Theme = theme;
            this.gridDetails.SetContextMenuTheme();
            this.gridDetails.BackgroundColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkSlateBlue;
            //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.PaleGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;

            foreach (var item in this.trayCMenu.Items) {
                if (item is ToolStripMenuItem tsmi) {
                    tsmi.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tsmi.MouseEnter += this.trayMenu_MouseEnter;
                    tsmi.MouseLeave += this.trayMenu_MouseLeave;
                    if (Equals(tsmi, this.traySettings)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayFilters)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon;
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayProfile)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon;
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayUpdate)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon)
                                                                    : (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon);
                        tsmi.ForeColor = this.isAvailableNewVersion ? (theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                    } else if (Equals(tsmi, this.trayHelp)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon;
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayExitProgram)) {
                        tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.shutdown_icon : Properties.Resources.shutdown_gray_icon;
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayOverlay)) {
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayLaunchFallGuys)) {
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.trayUsefulThings)) {
                        tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    }

                    foreach (var subItem1 in tsmi.DropDownItems) {
                        if (subItem1 is ToolStripMenuItem stsmi1) {
                            if (Equals(stsmi1, this.trayEditProfiles)) { stsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; }
                            stsmi1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                            stsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            stsmi1.MouseEnter += this.trayMenu_MouseEnter;
                            stsmi1.MouseLeave += this.trayMenu_MouseLeave;
                            foreach (var subItem2 in stsmi1.DropDownItems) {
                                if (subItem2 is ToolStripMenuItem stsmi2) {
                                    if (Equals(stsmi2, this.trayCustomRangeStats)) { stsmi2.Image = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; }
                                    stsmi2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    stsmi2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    stsmi2.MouseEnter += this.trayMenu_MouseEnter;
                                    stsmi2.MouseLeave += this.trayMenu_MouseLeave;
                                } else if (subItem2 is ToolStripSeparator stss2) {
                                    stss2.Paint += this.CustomToolStripSeparatorCustom_Paint;
                                    stss2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    stss2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                }
                            }
                        } else if (subItem1 is ToolStripSeparator stss1) {
                            stss1.Paint += this.CustomToolStripSeparatorCustom_Paint;
                            stss1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                            stss1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                        }
                    }
                } else if (item is ToolStripSeparator tss) {
                    tss.Paint += this.CustomToolStripSeparatorCustom_Paint;
                    tss.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tss.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                }
            }
            this.Theme = theme;
            this.ResumeLayout();
            this.Invalidate(true);
        }
        
        private void CustomToolStripSeparatorCustom_Paint(Object sender, PaintEventArgs e) {
            ToolStripSeparator separator = (ToolStripSeparator)sender;
            e.Graphics.FillRectangle(new SolidBrush(this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)), 0, 0, separator.Width, separator.Height); // CUSTOM_COLOR_BACKGROUND
            e.Graphics.DrawLine(new Pen(this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray), 30, separator.Height / 2, separator.Width - 4, separator.Height / 2); // CUSTOM_COLOR_FOREGROUND
        }
        
        private void trayMenu_MouseEnter(object sender, EventArgs e) {
            switch (sender) {
                case ToolStripMenuItem tsi: {
                    tsi.ForeColor = Color.Black;
                    if (Equals(tsi, this.traySettings)) {
                        tsi.Image = Properties.Resources.setting_icon;
                    } else if (Equals(tsi, this.trayFilters)) {
                        tsi.Image = Properties.Resources.filter_icon;
                    } else if (Equals(tsi, this.trayCustomRangeStats)) {
                        tsi.Image = Properties.Resources.calendar_icon;
                    } else if (Equals(tsi, this.trayProfile)) {
                        tsi.Image = Properties.Resources.profile_icon;
                    } else if (Equals(tsi, this.trayUpdate)) {
                        tsi.Image = this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon;
                    } else if (Equals(tsi, this.trayHelp)) {
                        tsi.Image = Properties.Resources.github_icon;
                    } else if (Equals(tsi, this.trayEditProfiles)) {
                        tsi.Image = Properties.Resources.setting_icon;
                    } else if (Equals(tsi, this.trayExitProgram)) {
                        tsi.Image = Properties.Resources.shutdown_icon;
                    }
                    break;
                }
            }
        }
        
        private void trayMenu_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
            switch (sender) {
                case ToolStripMenuItem tsi: {
                    if (Equals(tsi, this.traySettings)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayFilters)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayCustomRangeStats)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayProfile)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayUpdate)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon)
                                                                        : (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon);
                        tsi.ForeColor = this.isAvailableNewVersion ? (this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                    } else if (Equals(tsi, this.trayHelp)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayEditProfiles)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsi, this.trayExitProgram)) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.shutdown_icon : Properties.Resources.shutdown_gray_icon;
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else {
                        tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    }
                    break;
                }
            }
        }
        
        private void menu_MouseEnter(object sender, EventArgs e) {
            switch (sender) {
                case ToolStripMenuItem tsmi: {
                    tsmi.ForeColor = Color.Black;
                    if (Equals(tsmi, this.menuSettings)) {
                        tsmi.Image = Properties.Resources.setting_icon;
                    } else if (Equals(tsmi, this.menuFilters)) {
                        tsmi.Image = Properties.Resources.filter_icon;
                    } else if (Equals(tsmi, this.menuCustomRangeStats)) {
                        tsmi.Image = Properties.Resources.calendar_icon;
                    } else if (Equals(tsmi, this.menuProfile)) {
                        tsmi.Image = Properties.Resources.profile_icon;
                    } else if (Equals(tsmi, this.menuUpdate)) {
                        tsmi.Image = this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon;
                    } else if (Equals(tsmi, this.menuHelp)) {
                        tsmi.Image = Properties.Resources.github_icon;
                    } else if (Equals(tsmi, this.menuEditProfiles)) {
                        tsmi.Image = Properties.Resources.setting_icon;
                    }
                    break;
                }
            }
        }
        
        private void menu_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
            switch (sender) {
                case ToolStripMenuItem tsmi: {
                    if (Equals(tsmi, this.menuSettings)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.menuFilters)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.menuCustomRangeStats)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.menuProfile)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.menuUpdate)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon)
                                                                         : (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon);
                        tsmi.ForeColor = this.isAvailableNewVersion ? (this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : (this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray);
                    } else if (Equals(tsmi, this.menuHelp)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else if (Equals(tsmi, this.menuEditProfiles)) {
                        tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    } else {
                        tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    }
                    break;
                }
            }
        }
        
        private void infoStrip_MouseEnter(object sender, EventArgs e) {
            switch (sender) {
                case ToolStripLabel lblInfo: {
                    this.Cursor = Cursors.Hand;
                    this.infoStripForeColor = Equals(lblInfo, this.lblCurrentProfile)
                        ? this.Theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192)
                        : this.Theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                
                    lblInfo.ForeColor = Equals(lblInfo, this.lblCurrentProfile)
                        ? this.Theme == MetroThemeStyle.Light ? Color.FromArgb(245, 154, 168) : Color.FromArgb(231, 251, 255)
                        : this.Theme == MetroThemeStyle.Light ? Color.FromArgb(147, 174, 248) : Color.FromArgb(255, 250, 244);

                    Point cursorPosition = this.PointToClient(Cursor.Position);
                    Point position = new Point(cursorPosition.X + 16, cursorPosition.Y + 16);
                    this.AllocCustomTooltip(this.cmtt_center_Draw);
                    if (Equals(lblInfo, this.lblCurrentProfileIcon)) {
                        this.ShowCustomTooltip(Multilingual.GetWord($"{(this.CurrentSettings.AutoChangeProfile ? "profile_icon_enable_tooltip" : "profile_icon_disable_tooltip")}"), this, position);
                    } else if (Equals(lblInfo, this.lblCurrentProfile)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("profile_change_tooltip"), this, position);
                    } else if (Equals(lblInfo, this.lblTotalShows)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("shows_detail_tooltip"), this, position);
                    } else if (Equals(lblInfo, this.lblTotalRounds)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("rounds_detail_tooltip"), this, position);
                    } else if (Equals(lblInfo, this.lblTotalFinals)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("finals_detail_tooltip"), this, position);
                    } else if (Equals(lblInfo, this.lblTotalWins)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("wins_detail_tooltip"), this, position);
                    } else if (Equals(lblInfo, this.lblTotalTime)) {
                        this.ShowCustomTooltip(Multilingual.GetWord("stats_detail_tooltip"), this, position);
                    }

                    break;
                }
            }
        }
        
        private void infoStrip_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
            this.HideCustomTooltip(this);
            if (sender is ToolStripLabel lblInfo) {
                lblInfo.ForeColor = this.infoStripForeColor;
            }
        }
        
        public void ReloadProfileMenuItems() {
            this.ProfileMenuItems.Clear();
            this.menuProfile.DropDownItems.Clear();
            this.menuProfile.DropDownItems.Add(this.menuEditProfiles);
            this.menuProfile.DropDownItems.Add(this.menuSeparator2);
            this.menuSeparator2.Paint += this.CustomToolStripSeparatorCustom_Paint;
            this.menuSeparator2.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.menuSeparator2.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            
            this.ProfileTrayItems.Clear();
            this.trayProfile.DropDownItems.Clear();
            this.trayProfile.DropDownItems.Add(this.trayEditProfiles);
            this.trayProfile.DropDownItems.Add(this.traySubSeparator2);
            this.traySubSeparator2.Paint += this.CustomToolStripSeparatorCustom_Paint;
            this.traySubSeparator2.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.traySubSeparator2.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            
            this.AllProfiles.Clear();
            this.AllProfiles = this.Profiles.FindAll().ToList();
            this.profileWithLinkedCustomShow = this.AllProfiles.Find(p => string.Equals(p.LinkedShowId, "private_lobbies"))?.ProfileId ?? -1;
            int profileNumber = 0; 
            for (int i = this.AllProfiles.Count - 1; i >= 0; i--) {
                Profiles profile = this.AllProfiles[i];
                ToolStripMenuItem menuItem = new ToolStripMenuItem {
                    Checked = this.CurrentSettings.SelectedProfile == profile.ProfileId,
                    CheckOnClick = true,
                    CheckState = this.CurrentSettings.SelectedProfile == profile.ProfileId ? CheckState.Checked : CheckState.Unchecked,
                    Name = $@"menuProfile{profile.ProfileId}"
                };
                ToolStripMenuItem trayItem = new ToolStripMenuItem {
                    Checked = this.CurrentSettings.SelectedProfile == profile.ProfileId,
                    CheckOnClick = true,
                    CheckState = this.CurrentSettings.SelectedProfile == profile.ProfileId ? CheckState.Checked : CheckState.Unchecked,
                    Name = $@"menuProfile{profile.ProfileId}"
                };
                
                switch (profileNumber++) {
                    case 0: menuItem.Image = this.numberOne; trayItem.Image = this.numberOne; break;
                    case 1: menuItem.Image = this.numberTwo; trayItem.Image = this.numberTwo; break;
                    case 2: menuItem.Image = this.numberThree; trayItem.Image = this.numberThree; break;
                    case 3: menuItem.Image = this.numberFour; trayItem.Image = this.numberFour; break;
                    case 4: menuItem.Image = this.numberFive; trayItem.Image = this.numberFive; break;
                    case 5: menuItem.Image = this.numberSix; trayItem.Image = this.numberSix; break;
                    case 6: menuItem.Image = this.numberSeven; trayItem.Image = this.numberSeven; break;
                    case 7: menuItem.Image = this.numberEight; trayItem.Image = this.numberEight; break;
                    case 8: menuItem.Image = this.numberNine; trayItem.Image = this.numberNine; break;
                }
                menuItem.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                menuItem.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                menuItem.Size = new Size(180, 22);
                menuItem.Text = profile.ProfileName.Replace("&", "&&");
                menuItem.Click += this.menuStats_Click;
                menuItem.Paint += this.menuProfile_Paint;
                menuItem.MouseMove += this.setCursor_MouseMove;
                // menuItem.MouseLeave += this.setCursor_MouseLeave;
                menuItem.MouseEnter += this.menu_MouseEnter;
                menuItem.MouseLeave += this.menu_MouseLeave;
                this.menuProfile.DropDownItems.Add(menuItem);
                this.ProfileMenuItems.Add(menuItem);
                
                trayItem.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                trayItem.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                trayItem.Size = new Size(180, 22);
                trayItem.Text = profile.ProfileName.Replace("&", "&&");
                trayItem.Click += this.menuStats_Click;
                trayItem.Paint += this.menuProfile_Paint;
                trayItem.MouseEnter += this.trayMenu_MouseEnter;
                trayItem.MouseLeave += this.trayMenu_MouseLeave;
                this.trayProfile.DropDownItems.Add(trayItem);
                this.ProfileTrayItems.Add(trayItem);
                
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowCheckMargin = true;
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowImageMargin = true;
                
                if (this.CurrentSettings.SelectedProfile == profile.ProfileId) {
                    if (this.AllProfiles.Count != 0) this.SetCurrentProfileIcon(!string.IsNullOrEmpty(profile.LinkedShowId));
                    this.menuStats_Click(menuItem, EventArgs.Empty);
                }
            }
        }
        
        private void menuProfile_Paint(object sender, PaintEventArgs e) {
            if (this.AllProfiles.FindIndex(p => string.Equals(p.ProfileId.ToString(), ((ToolStripMenuItem)sender).Name.Substring(11)) && !string.IsNullOrEmpty(p.LinkedShowId)) != -1) {
                e.Graphics.DrawImage(this.CurrentSettings.AutoChangeProfile ? Properties.Resources.link_on_icon :
                                     this.Theme == MetroThemeStyle.Light ? Properties.Resources.link_icon : Properties.Resources.link_gray_icon, 21, 5, 11, 11);
            }
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
        
        private void UpdateDatabaseDateFormat() {
            if (!this.CurrentSettings.UpdatedDateFormat) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    info.Start = DateTime.SpecifyKind(info.Start.ToLocalTime(), DateTimeKind.Utc);
                    info.End = DateTime.SpecifyKind(info.End.ToLocalTime(), DateTimeKind.Utc);
                    info.Finish = info.Finish.HasValue ? DateTime.SpecifyKind(info.Finish.Value.ToLocalTime(), DateTimeKind.Utc) : (DateTime?)null;
                    this.RoundDetails.Update(info);
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.UpdatedDateFormat = true;
                this.SaveUserSettings();
            }
        }
        
        private void UpdateDatabaseVersion() {
            int lastVersion = 127;
            for (int version = this.CurrentSettings.Version; version < lastVersion; version++) {
                switch (version) {
                    case 126: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_yeetus_template")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_tip_toe") && ri.Players <= 9) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 125: {
                            DateTime dateCond = new DateTime(2025, 7, 29, 9, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                              where !string.IsNullOrEmpty(ri.ShowNameId) &&
                                                                    ri.Start >= dateCond &&
                                                                    ri.ShowNameId.StartsWith("knockout_")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if ((this.StatLookup.TryGetValue(ri.Name, out LevelStats levelStats) && levelStats.IsFinal)
                                     || string.Equals(ri.Name, "knockout_rotateandeliminate")
                                     || string.Equals(ri.Name, "knockout_gooprope_rodeo")
                                     || string.Equals(ri.Name, "knockout_slimeballshowdown")
                                     || string.Equals(ri.Name, "knockout_blunderblocks")
                                     || string.Equals(ri.Name, "knockout_pier_pressure")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp20")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Round == 3 || string.Equals(ri.Name, "showcase_boats")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList3 = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "anniversary_fp12_ltm")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList3) {
                                if (ri.Round == 10) ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList3);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 124: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_only_slime_climb_2_template")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Round == 3) ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "sports_show")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (ri.Round == 3 || string.Equals(ri.Name, "round_fall_ball_60_players")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 123: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_nature_ltm")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "showcase_frogjet")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 122: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_only_finals_v3_template")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Name.EndsWith("_final")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 121: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_bouncy_bean_time")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "showcase_rollinruins")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp19")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Round == 3 || string.Equals(ri.Name, "fp19_mellowcakes")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 120: {
                            if (this.CurrentSettings.SelectedCustomTemplateSeason == 9) {
                                this.CurrentSettings.SelectedCustomTemplateSeason = -1;
                            }
                            break;
                        }
                    case 119: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "greatestsquads_ltm")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Round == 3 || string.Equals(ri.Name, "gs_slimecycle")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 118: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "squads_2player_template") || string.Equals(ri.ShowNameId, "squads_4player") ||
                                                                   string.Equals(ri.ShowNameId, "classic_duos_show") || string.Equals(ri.ShowNameId, "classic_squads_show")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_1v1_volleyfall_symphony_launch_show") || string.Equals(ri.Name, "round_hoops_revenge_symphony_launch_show")) {
                                    ri.IsTeam = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 117: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp13")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "scrapyard_derrameburbujeante")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 116: {
                            List<Profiles> profileList = this.Profiles.FindAll().ToList();
                            
                            foreach (Profiles p in profileList) {
                                if (string.Equals(p.LinkedShowId, "ranked_show_knockout") && !p.DoNotCombineShows) {
                                    p.LinkedShowId = "ranked_solo_show";
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.Profiles.DeleteAll();
                            this.Profiles.InsertBulk(profileList);
                            this.StatsDB.Commit();
                            
                            DateTime dateCond = new DateTime(2025, 4, 1, 9, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "knockout_mode") && ri.Start >= dateCond
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if ((ri.Name.StartsWith("ranked_") && ri.Name.EndsWith("_final"))
                                     || string.Equals(ri.Name, "round_floor_fall")
                                     || string.Equals(ri.Name, "round_kraken_attack")
                                     || string.Equals(ri.Name, "round_tunnel_final")
                                     || string.Equals(ri.Name, "round_blastball_arenasurvival_symphony_launch_show")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 115: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp18")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (string.Equals(ri.Name, "showcase_bulletfallwoods") || string.Equals(ri.Name, "showcase_treeclimberswoods")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where !string.IsNullOrEmpty(ri.ShowNameId) &&
                                                                    ri.ShowNameId.StartsWith("ranked_")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (ri.Name.StartsWith("ranked_") && ri.Name.EndsWith("_final")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 114: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_nature_ltm")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = string.Equals(ri.Name, "logroll_nature_ltm") || string.Equals(ri.Name, "lilypadlimbo_nature_ltm") || string.Equals(ri.Name, "junglewall_nature_ltm");
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 113: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_bouncy_bean_time")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = ri.Name.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 112: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "knockout_mode")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_fp17_knockout_castlesiege") || string.Equals(ri.Name, "round_fp17_knockout_gardenpardon")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where string.Equals(ri.ShowNameId, "event_snowday_stumble")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (string.Equals(ri.Name, "round_cloudyteacups_final_sds") || string.Equals(ri.Name, "round_goopropegrandslam_final_sds")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 111: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp17")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (string.Equals(ri.Name, "round_fp17_gardenpardon") || string.Equals(ri.Name, "round_fp17_castlesiege")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            //
                            // List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                            //     where string.Equals(ri.ShowNameId, "ftue_uk_show")
                            //     select ri).ToList();
                            //
                            // foreach (RoundInfo ri in roundInfoList2) {
                            //     if (string.Equals(ri.Name, "round_fp17_knockout_castlesiege")
                            //         || string.Equals(ri.Name, "knockout_circleoslime_final_survival")
                            //         || string.Equals(ri.Name, "knockout_goopropegrandslamgoldrush_final_survival")
                            //         || string.Equals(ri.Name, "knockout_rollerderby_final")
                            //         || string.Equals(ri.Name, "knockout_mode_cloudyteacupsgoldrush_final")
                            //         || string.Equals(ri.Name, "round_fp17_knockout_gardenpardon")
                            //         || string.Equals(ri.Name, "round_fp17_knockout_gardenpardon")) {
                            //         ri.IsFinal = true;
                            //     }
                            // }
                            // this.StatsDB.BeginTrans();
                            // this.RoundDetails.Update(roundInfoList2);
                            // this.StatsDB.Commit();
                            break;
                        }
                    case 110: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "fp16_ski_fall_high_scorers")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 109: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp16")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Name.EndsWith("_final") || ri.Name.EndsWith("_goopropegrandslam")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where string.Equals(ri.ShowNameId, "mrs_pegwin_winter_2teamsfinal")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (string.Equals(ri.Name, "round_penguin_solos") || string.Equals(ri.Name, "round_chicken_chase")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 108: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "showcase_fp13")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Name.EndsWith("_final")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 107: {
                            DateTime dateCond = new DateTime(2024, 11, 17, 10, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_animals_template") && ri.Start >= dateCond
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_drumtop") && ri.Players <= 10) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 106: {
                            DateTime dateCond = new DateTime(2024, 10, 25, 12, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_only_button_bashers_template") && ri.Start >= dateCond
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Round < 4) {
                                    ri.IsFinal = false;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 105: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "explore_points")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.ShowNameId = "user_creative_hunt_round";
                                ri.UseShareCode = true;
                                ri.Name = ri.Name.Substring(4, 14);
                                ri.CreativeShareCode = ri.Name;
                                ri.CreativeTitle = ri.Name;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 104: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "explore_points")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsCasualShow = true;
                                ri.Round = 1;
                                ri.Qualified = ri.Finish.HasValue;
                                ri.IsFinal = false;
                                ri.Crown = false;
                                ri.IsAbandon = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 103: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "xtreme_explore")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "event_xtreme_fall_guys_template"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsCasualShow = true;
                                ri.Round = 1;
                                ri.Qualified = ri.Finish.HasValue;
                                ri.IsFinal = false;
                                ri.Crown = false;
                                ri.IsAbandon = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 102: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "teams_show_ltm")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "squads_2player_template"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (string.Equals(ri.Name, "round_territory_control_s4_show") || (string.Equals(ri.Name, "round_fall_ball_60_players") && (ri.Players % 2 == 0) && ri.Round == 1)) {
                                    ri.IsFinal = false;
                                } else if (string.Equals(ri.Name, "round_1v1_volleyfall_symphony_launch_show")) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 101: {
                            this.StatsDB.BeginTrans();
                            this.UpcomingShow.UpdateMany(
                                lv => new UpcomingShow { LevelType = LevelType.Hunt },
                                lv => lv.LevelType == LevelType.Unknown
                            );
                            this.StatsDB.Commit();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            if (profileId != -1) {
                                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                                 where string.Equals(ri.ShowNameId, "showcase_fp13")
                                                                 select ri).ToList();
                                
                                foreach (RoundInfo ri in roundInfoList) {
                                    ri.Profile = profileId;
                                }
                                this.StatsDB.BeginTrans();
                                this.RoundDetails.Update(roundInfoList);
                                this.StatsDB.Commit();
                            }
                            this.UpcomingShowCache = this.UpcomingShow.FindAll().ToList();
                            break;
                        }
                    case 100: {
                            this.CurrentSettings.CountPlayersDuringTheLevel = true;
                            break;
                        }
                    case 99: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "ftue_uk_show")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "main_show"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = string.Equals(ri.Name, "round_snowballsurvival");
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            DateTime dateCond = new DateTime(2024, 5, 15, 12, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where !string.IsNullOrEmpty(ri.ShowNameId) &&
                                                                    ri.Start >= dateCond &&
                                                                    ri.ShowNameId.StartsWith("knockout_")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                ri.IsFinal = string.Equals(ri.Name, "round_blastball_arenasurvival_symphony_launch_show") || string.Equals(ri.Name, "round_floor_fall") ||
                                             string.Equals(ri.Name, "round_hexaring_symphony_launch_show") || string.Equals(ri.Name, "round_hexsnake_almond") || string.Equals(ri.Name, "round_royal_rumble") ||
                                             (!string.Equals(ri.Name, "knockout_fp10_final_8") && ri.Name.StartsWith("knockout_") && (ri.Name.EndsWith("_opener_4") || ri.Name.IndexOf("_final") != -1));
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 98: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "no_elimination_explore")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "main_show"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsCasualShow = true;
                                ri.Round = 1;
                                ri.Qualified = ri.Finish.HasValue;
                                ri.IsFinal = false;
                                ri.Crown = false;
                                ri.IsAbandon = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 97: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("classic_")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_basketfall_s4_show") || string.Equals(ri.Name, "round_territory_control_s4_show")) {
                                    ri.IsFinal = (string.Equals(ri.ShowNameId, "classic_duos_show") && ri.Players <= 4) || (string.Equals(ri.ShowNameId, "classic_squads_show") && ri.Players <= 8);
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 96: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("classic_")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                int profileId = -1;
                                if (string.Equals(ri.ShowNameId, "classic_solo_main_show")) {
                                    Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "main_show"));
                                    profileId = profile?.ProfileId ?? -1;
                                } else if (string.Equals(ri.ShowNameId, "classic_duos_show")) {
                                    Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "squads_2player_template"));
                                    profileId = profile?.ProfileId ?? -1;
                                } else if (string.Equals(ri.ShowNameId, "classic_squads_show")) {
                                    Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "squads_4player"));
                                    profileId = profile?.ProfileId ?? -1;
                                }
                                if (profileId != -1) ri.Profile = profileId;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 95: {
                            this.UpcomingShowCache = this.UpcomingShow.FindAll().ToList();
                            bool isDelete = false;
                            this.StatsDB.BeginTrans();
                            foreach (var level in this.UpcomingShowCache) {
                                if (LevelStats.ALL.ContainsKey(level.LevelId)) {
                                    BsonExpression condition = Query.EQ("LevelId", level.LevelId);
                                    this.UpcomingShow.DeleteMany(condition);
                                }
                            }
                            this.StatsDB.Commit();
                            if (isDelete) this.UpcomingShowCache = this.UpcomingShow.FindAll().ToList();
                            break;
                        }
                    case 94: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) &&
                                                                   ri.ShowNameId.StartsWith("user_creative_") &&
                                                                   ri.IsFinal &&
                                                                   !ri.PrivateLobby
                                                             select ri).ToList();
                            
                            if (roundInfoList.Any()) {
                                int showId = roundInfoList.First().ShowID;
                                
                                foreach (RoundInfo ri in roundInfoList) {
                                    ri.IsCasualShow = true;
                                    ri.Round = 1;
                                    ri.Qualified = ri.Finish.HasValue;
                                    ri.IsFinal = false;
                                    ri.Crown = false;
                                    ri.IsAbandon = false;
                                }
                                this.StatsDB.BeginTrans();
                                this.RoundDetails.Update(roundInfoList);
                                this.StatsDB.Commit();
                                
                                List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                                  where ri.ShowID >= showId
                                                                  select ri).ToList();
                                
                                bool isFirstShow = true;
                                bool isFixRequired = false;
                                bool waitForNextCasualShow = false;
                                int i = 1;
                                foreach (RoundInfo ri in roundInfoList2) {
                                    if (!isFixRequired) {
                                        if (!ri.IsCasualShow) {
                                            if (isFirstShow && ri.ShowID == showId) {
                                                isFixRequired = true;
                                                waitForNextCasualShow = true;
                                            } else {
                                                isFirstShow = false;
                                                if (ri.Round == 1) {
                                                    showId = ri.ShowID;
                                                }
                                            }
                                        } else {
                                            if (isFirstShow) {
                                                isFirstShow = false;
                                            } else if (ri.ShowID == showId) {
                                                isFixRequired = true;
                                                ri.ShowID = showId + i;
                                                i++;
                                            }
                                        }
                                        continue;
                                    }
                                    if (ri.IsCasualShow) {
                                        waitForNextCasualShow = false;
                                        ri.ShowID = showId + i;
                                        i++;
                                        continue;
                                    }
                                    if (waitForNextCasualShow) {
                                        continue;
                                    }
                                    if (ri.Round == 1) {
                                        ri.ShowID = showId + i;
                                        showId = ri.ShowID;
                                        i = 1;
                                    } else {
                                        ri.ShowID = showId;
                                    }
                                }
                                this.StatsDB.BeginTrans();
                                this.RoundDetails.Update(roundInfoList2);
                                this.StatsDB.Commit();
                            }
                            break;
                        }
                    case 93: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("knockout_")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = (ri.Name.StartsWith("knockout_fp") && ri.Name.IndexOf("_final") != -1) || (ri.ShowNameId.StartsWith("knockout_fp") && ri.ShowNameId.EndsWith("_srs"));
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            DateTime dateCond = new DateTime(2024, 5, 15, 12, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where !string.IsNullOrEmpty(ri.ShowNameId) &&
                                                                    ri.Start >= dateCond &&
                                                                    ri.ShowNameId.StartsWith("knockout_")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                ri.IsFinal = string.Equals(ri.Name, "round_blastball_arenasurvival_symphony_launch_show") || string.Equals(ri.Name, "round_kraken_attack") || string.Equals(ri.Name, "round_jump_showdown") ||
                                             string.Equals(ri.Name, "round_crown_maze") || string.Equals(ri.Name, "round_tunnel_final") || string.Equals(ri.Name, "round_fall_mountain_hub_complete") ||
                                             (!string.Equals(ri.Name, "knockout_fp10_final_8") && ri.Name.StartsWith("knockout_fp") && ri.Name.IndexOf("_final") != -1);
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 92: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("knockout_")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_blastball_arenasurvival_symphony_launch_show") || string.Equals(ri.Name, "round_kraken_attack")) {
                                    ri.IsFinal = false;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 91: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("knockout_")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = ri.Name.StartsWith("knockout_fp10_final_") || string.Equals(ri.Name, "round_crown_maze") || string.Equals(ri.Name, "round_tunnel_final") || string.Equals(ri.Name, "round_fall_mountain_hub_complete");
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 90: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where !string.IsNullOrEmpty(ri.ShowNameId) && ri.ShowNameId.StartsWith("knockout_")
                                                             select ri).ToList();
                            
                            Dictionary<string, string> sceneToRound = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                                { "knockout_fp10_filler_9", "round_airtime" },
                                { "knockout_fp10_opener_3", "round_fruitpunch_s4_show" },
                                { "knockout_fp10_opener_4", "round_blastball_arenasurvival_symphony_launch_show" },
                                { "knockout_fp10_opener_9", "round_see_saw_360" },
                                { "knockout_fp10_filler_8", "round_hoops" },
                                { "knockout_fp10_filler_15", "round_hoverboardsurvival_s4_show" },
                                { "knockout_fp10_filler_3", "round_jump_club" },
                                { "knockout_fp10_filler_7", "round_kraken_attack" },
                                { "knockout_fp10_opener_8", "round_follow-the-leader_s6_launch" },
                                { "knockout_fp10_opener_2", "round_snowballsurvival" },
                                { "knockout_fp10_opener_7", "round_tail_tag" },
                                { "knockout_fp10_filler_4", "round_spin_ring_symphony_launch_show" },
                                { "knockout_fp10_opener_17", "round_gauntlet_03" },
                                { "knockout_fp10_filler_6", "round_1v1_volleyfall_symphony_launch_show" },
                                { "knockout_fp10_final_3", "round_fall_mountain_hub_complete" },
                                { "knockout_fp10_final_1", "round_crown_maze" },
                                { "knockout_fp10_final_2", "round_tunnel_final" }
                            };
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (sceneToRound.TryGetValue(ri.Name, out string levelId)) {
                                    int profileId = -1;
                                    if (string.Equals(ri.ShowNameId, "knockout_mode")) {
                                        Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "main_show"));
                                        profileId = profile?.ProfileId ?? -1;
                                    } else if (string.Equals(ri.ShowNameId, "knockout_duos")) {
                                        Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "squads_2player_template"));
                                        profileId = profile?.ProfileId ?? -1;
                                    } else if (string.Equals(ri.ShowNameId, "knockout_squads")) {
                                        Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "squads_4player"));
                                        profileId = profile?.ProfileId ?? -1;
                                    }
                                    if (profileId != -1) ri.Profile = profileId;
                                    ri.Name = levelId;
                                    ri.IsFinal = string.Equals(levelId, "round_crown_maze") || string.Equals(levelId, "round_tunnel_final") || string.Equals(levelId, "round_fall_mountain_hub_complete");
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 89: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.PrivateLobby && string.Equals(ri.Name, "unknown", StringComparison.OrdinalIgnoreCase)
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.Name = ri.ShowNameId;
                                ri.ShowNameId = this.GetUserCreativeLevelTypeId(ri.CreativeGameModeId);
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 88: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.PrivateLobby && (string.Equals(ri.ShowNameId, "unknown", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(ri.CreativeGameModeId) || string.IsNullOrEmpty(ri.CreativeLevelThemeId))
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.ShowNameId, "unknown", StringComparison.OrdinalIgnoreCase)) {
                                    ri.ShowNameId = "user_creative_race_round";
                                }
                                
                                if (!string.IsNullOrEmpty(ri.CreativeShareCode)) {
                                    if (string.IsNullOrEmpty(ri.CreativeGameModeId)) {
                                        ri.CreativeGameModeId = "GAMEMODE_GAUNTLET";
                                    }
                                    if (string.IsNullOrEmpty(ri.CreativeLevelThemeId)) {
                                        ri.CreativeLevelThemeId = "THEME_VANILLA";
                                    }
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 87: {
                            List<Profiles> profileList = this.Profiles.FindAll().ToList();
                            
                            foreach (Profiles p in profileList) {
                                if (string.Equals(p.LinkedShowId, "event_only_finals_v2_template")) {
                                    p.LinkedShowId = "event_only_finals_v3_template";
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.Profiles.DeleteAll();
                            this.Profiles.InsertBulk(profileList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 86: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_april_fools") && ri.IsFinal == false
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            this.StatsDB.BeginTrans();
                            this.UpcomingShowCache.RemoveAll(u => string.Equals(u.ShowId, "event_april_fools") && !u.LevelId.StartsWith("wle_shuffle_falljam_april_"));
                            this.UpcomingShow.DeleteAll();
                            this.UpcomingShow.InsertBulk(this.UpcomingShowCache);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 85: {
                            this.StatsDB.BeginTrans();
                            this.UpcomingShowCache.RemoveAll(u => string.Equals(u.ShowId, "wle_shuffle_discover") && u.LevelId.StartsWith("wle_shuggle_mwk3_"));
                            this.UpcomingShow.DeleteAll();
                            this.UpcomingShow.InsertBulk(this.UpcomingShowCache);
                            this.StatsDB.Commit();
                            if (this.CurrentSettings.NotificationWindowPosition == 0) {
                                this.CurrentSettings.NotificationWindowPosition += 3;
                            }
                            this.CurrentSettings.NotificationWindowAnimation = 0;
                            break;
                        }
                    case 84: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_only_button_bashers_template")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Round > 3) ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where string.Equals(ri.ShowNameId, "wle_mrs_ugc_playful_pioneers") ||
                                                                    string.Equals(ri.ShowNameId, "wle_playful_shuffle")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                if (string.Equals(ri.ShowNameId, "wle_playful_shuffle")) {
                                    ri.IsFinal = true;
                                } else if (string.Equals(ri.ShowNameId, "wle_mrs_ugc_playful_pioneers")) {
                                    if (LevelStats.ALL.TryGetValue(ri.Name, out LevelStats l1)) {
                                        ri.IsFinal = l1.IsFinal;
                                    }
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 83: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_shuffle_survival")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 82: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.Name.StartsWith("user_creative_") && ri.Name.EndsWith("_round")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                (ri.ShowNameId, ri.Name) = (ri.Name, ri.ShowNameId);
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 81: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_survival_showdown")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Name.IndexOf("_showdown_final", StringComparison.OrdinalIgnoreCase) != -1) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            DateTime dateCond = new DateTime(2024, 2, 28, 10, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where ri.Start <= dateCond && string.Equals(ri.Name, "user_creative_race_round")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                ri.CreativeGameModeId = "GAMEMODE_GAUNTLET";
                                ri.SceneName = "GAMEMODE_GAUNTLET";
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 80: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "no_elimination_show")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (string.Equals(ri.Name, "round_snowballsurvival") || string.Equals(ri.Name, "round_robotrampage_arena_2")) {
                                    if (ri.Round == 3) {
                                        ri.IsFinal = true;
                                    } else {
                                        var filteredList = roundInfoList.Where(r => r.ShowID == ri.ShowID);
                                        int maxRound = filteredList.Max(r => r.Round);
                                        if (ri.Round == maxRound && ri.Qualified) {
                                            ri.IsFinal = true;
                                        }
                                    }
                                } else if (ri.Name.StartsWith("wle_main_filler_") || ri.Name.StartsWith("wle_main_opener_")) {
                                    ri.Name = ri.Name.Replace("_noelim", "");
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                                                              where string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show") && ri.Name.StartsWith("digishuffle_feb_")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList2) {
                                ri.Name = $"wle_{ri.Name}";
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList2);
                            this.StatsDB.Commit();
                            
                            List<RoundInfo> roundInfoList3 = (from ri in this.RoundDetails.FindAll()
                                                              where string.Equals(ri.ShowNameId, "wle_shuffle_chill")
                                                              select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList3) {
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList3);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 79: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_bouncy_bean_time")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = ri.Name.IndexOf("_final", StringComparison.OrdinalIgnoreCase) != -1;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 78: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Name.StartsWith("wle_shuffle_squads_2_24_01_")) {
                                    ri.Name = ri.Name.Replace("_squads_", "_").Replace("_24_01_", "_24_");
                                } else if (ri.Name.StartsWith("wle_shuffle_2_24_01_")) {
                                    ri.Name = ri.Name.Replace("_24_01_", "_24_");
                                }
                                if (profileId != -1) ri.Profile = profileId;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 77:
                    case 76: {
                            if (version == 76) ++version;
                            this.CurrentSettings.EnableFallalyticsWeeklyCrownLeague = true;
                            break;
                        }
                    case 75: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Name.StartsWith("wle_shuffle_squads_fp")) {
                                    ri.Name = ri.Name.Replace("_squads_", "_discover_");
                                } else if (ri.Name.StartsWith("wle_shuffle_fp")) {
                                    ri.Name = ri.Name.Replace("wle_shuffle_fp", "wle_shuffle_discover_fp");
                                }
                                if (profileId != -1) ri.Profile = profileId;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 74: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_blast_ball_banger_template")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "event_blast_ball_banger_template"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = string.Equals(ri.Name, "round_blastball_arenasurvival_symphony_launch_show");
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 73: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_winter")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                if (ri.Name.IndexOf("_final_", StringComparison.OrdinalIgnoreCase) != -1) {
                                    ri.IsFinal = true;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 72: {
                            DateTime dateCond = new DateTime(2023, 12, 15, 10, 0, 0, DateTimeKind.Utc);
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.Start >= dateCond &&
                                                                   string.Equals(ri.Name, "user_creative_race_round") &&
                                                                   (ri.PrivateLobby == false || ri.Round > 1)
                                                             select ri).ToList();
                            
                            this.StatsDB.BeginTrans();
                            foreach (RoundInfo info in roundInfoList) {
                                this.RoundDetails.DeleteMany(r => r.ShowID == info.ShowID);
                            }
                            this.StatsDB.Commit();
                            break;
                        }
                    case 71: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "event_anniversary_season_1_alternate_name")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                if (ri.Name.IndexOf("round_fall_ball", StringComparison.OrdinalIgnoreCase) != -1
                                    || ri.Name.IndexOf("round_jinxed", StringComparison.OrdinalIgnoreCase) != -1) {
                                    ri.IsFinal = false;
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 70: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_bouncy_bean_time")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = ri.Name.EndsWith("_final");
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 69: {
                            // List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                            //                                  where string.Equals(ri.ShowNameId, "wle_shuffle_discover") ||
                            //                                        string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads")
                            //                                  select ri).ToList();
                            //
                            // Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            // int profileId = profile?.ProfileId ?? -1;
                            //
                            // this.StatsDB.BeginTrans();
                            // foreach (RoundInfo ri in roundInfoList) {
                            //     if (profileId != -1) ri.Profile = profileId;
                            //     if (string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads") && ri.Name.IndexOf("_squads", StringComparison.OrdinalIgnoreCase) != -1) {
                            //         ri.Name = ri.Name.Replace("_squads", "");
                            //     }
                            //     if (this.LevelIdReplacerInShuffleShow.TryGetValue(ri.Name, out string newName)) {
                            //         ri.Name = newName;
                            //     }
                            //     ri.IsFinal = true;
                            //
                            //     if (string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads") && ri.Round > 1) {
                            //         List<RoundInfo> ril = roundInfoList.FindAll(r => r.ShowID == ri.ShowID);
                            //         foreach (RoundInfo r in ril) {
                            //             if (r.Round != ri.Round) {
                            //                 r.IsFinal = false;
                            //             }
                            //         }
                            //         this.RoundDetails.Update(ril);
                            //     }
                            // }
                            // this.RoundDetails.Update(roundInfoList);
                            // this.StatsDB.Commit();
                            //
                            // Dictionary<string, string> duplicatedKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                            //     { "wle_discover_level_wk2_004", "current_wle_fp4_05_01_05" },
                            //     { "wle_discover_level_wk2_011", "current_wle_fp6_wk4_05_01" },
                            //     { "wle_discover_level_wk2_042", "current_wle_fp6_wk4_02_04" },
                            //     { "wle_discover_level_wk2_044", "current_wle_fp6_wk4_05_02" },
                            //     { "wle_discover_level_wk2_045", "current_wle_fp6_3_04" }
                            // };
                            //
                            // List<RoundInfo> roundInfoList2 = (from ri in this.RoundDetails.FindAll()
                            //                                   where duplicatedKey.ContainsKey(ri.Name)
                            //                                   select ri).ToList();
                            //
                            // foreach (RoundInfo ri in roundInfoList2) {
                            //     if (duplicatedKey.TryGetValue(ri.Name, out string newName)) {
                            //         ri.Name = newName;
                            //     }
                            // }
                            //
                            // this.StatsDB.BeginTrans();
                            // this.RoundDetails.Update(roundInfoList2);
                            // this.StatsDB.Commit();
                            break;
                        }
                    case 68: {
                            // List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                            //                                  where string.Equals(ri.ShowNameId, "wle_shuffle_discover") ||
                            //                                        string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads")
                            //                                  select ri).ToList();
                            //
                            // Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            // int profileId = profile?.ProfileId ?? -1;
                            //
                            // this.StatsDB.BeginTrans();
                            // foreach (RoundInfo ri in roundInfoList) {
                            //     if (profileId != -1) ri.Profile = profileId;
                            //     if (string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads") && ri.Name.EndsWith("_squads")) {
                            //         ri.Name = ri.Name.Substring(0, ri.Name.LastIndexOf("_squads", StringComparison.OrdinalIgnoreCase));
                            //     }
                            //     if (this.LevelIdReplacerInShuffleShow.TryGetValue(ri.Name, out string newName)) {
                            //         ri.Name = newName;
                            //     }
                            //     ri.IsFinal = true;
                            //
                            //     if (string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show_squads") && ri.Round > 1) {
                            //         List<RoundInfo> ril = roundInfoList.FindAll(r => r.ShowID == ri.ShowID);
                            //         foreach (RoundInfo r in ril) {
                            //             if (r.Round != ri.Round) {
                            //                 r.IsFinal = false;
                            //             }
                            //         }
                            //         this.RoundDetails.Update(ril);
                            //     }
                            // }
                            //
                            // this.RoundDetails.Update(roundInfoList);
                            // this.StatsDB.Commit();
                            break;
                        }
                    case 67: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_shuffle_discover")
                                                             select ri).ToList();
                            
                            Profiles profile = this.Profiles.FindOne(Query.EQ("LinkedShowId", "fall_guys_creative_mode"));
                            int profileId = profile?.ProfileId ?? -1;
                            foreach (RoundInfo ri in roundInfoList) {
                                if (profileId != -1) ri.Profile = profileId;
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 66: {
                            if (this.StatsDB.CollectionExists("FallalyticsPbLog")) {
                                this.StatsDB.BeginTrans();
                                this.FallalyticsPbLog.DeleteAll();
                                this.StatsDB.Commit();
                            }
                            this.CurrentSettings.EnableFallalyticsReporting = true;
                            break;
                        }
                    case 65: {
                            this.CurrentSettings.EnableFallalyticsReporting = true;
                            break;
                        }
                    case 64: {
                            this.CurrentSettings.NotifyPersonalBest = true;
                            break;
                        }
                    case 63: {
                            this.CurrentSettings.RecordEscapeDuringAGame = true;
                            break;
                        }
                    case 62: {
                            List<Profiles> profileList = (from p in this.Profiles.FindAll()
                                                          where string.IsNullOrEmpty(p.ProfileName)
                                                          select p).ToList();
                            
                            foreach (Profiles p in profileList) {
                                p.ProfileName = Utils.ComputeHash(BitConverter.GetBytes(DateTime.Now.Ticks), HashTypes.MD5).Substring(0, 20);
                            }
                            this.StatsDB.BeginTrans();
                            this.Profiles.Update(profileList);
                            this.StatsDB.Commit();
                            this.CurrentSettings.NotificationSounds = 0;
                            this.CurrentSettings.NotificationWindowPosition = 0;
                            this.CurrentSettings.NotificationWindowAnimation = 1;
                            break;
                        }
                    case 61: {
                            this.CurrentSettings.NotifyServerConnected = true;
                            this.CurrentSettings.MuteNotificationSounds = false;
                            this.CurrentSettings.NotificationSounds = 0;
                            break;
                        }
                    case 60: {
                            if (this.StatsDB.CollectionExists("FallalyticsPbInfo")) {
                                this.StatsDB.DropCollection("FallalyticsPbInfo");
                            }
                            this.CurrentSettings.NotifyServerConnected = true;
                            this.CurrentSettings.MuteNotificationSounds = false;
                            break;
                        }
                    case 59: {
                            // List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                            //                                  where string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show") &&
                            //                                        ri.Name.StartsWith("shuffle_halloween_")
                            //                                  select ri).ToList();
                            //
                            // foreach (RoundInfo ri in roundInfoList) {
                            //     if (this.LevelIdReplacerInDigisShuffleShow.TryGetValue($"wle_{ri.Name}", out string newName)) {
                            //         ri.Name = newName;
                            //     }
                            // }
                            // this.StatsDB.BeginTrans();
                            // this.RoundDetails.Update(roundInfoList);
                            // this.StatsDB.Commit();
                            break;
                        }
                    case 58: {
                            this.CurrentSettings.DisplayGamePlayedInfo = true;
                            break;
                        }
                    case 57: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.IsFinal &&
                                                                   string.Equals(ri.ShowNameId, "event_only_hexaring_template") &&
                                                                   ri.Round < 3
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 56: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.IsFinal &&
                                                                   string.Equals(ri.ShowNameId, "event_only_thin_ice_template") &&
                                                                   ri.Round < 3
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 55: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where string.Equals(ri.ShowNameId, "wle_mrs_shuffle_show")
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.Name = ri.Name.StartsWith("mrs_wle_fp") ? $"current{ri.Name.Substring(3)}" : ri.Name.Substring(4);
                                ri.IsFinal = true;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 54: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.IsFinal &&
                                                                   string.Equals(ri.ShowNameId, "event_only_hexaring_template") &&
                                                                   ri.Round < 3
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 53: {
                            List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                                                             where ri.IsFinal &&
                                                                   string.Equals(ri.ShowNameId, "survival_of_the_fittest") &&
                                                                   string.Equals(ri.Name, "round_kraken_attack") &&
                                                                   ri.Round != 4
                                                             select ri).ToList();
                            
                            foreach (RoundInfo ri in roundInfoList) {
                                ri.IsFinal = false;
                            }
                            this.StatsDB.BeginTrans();
                            this.RoundDetails.Update(roundInfoList);
                            this.StatsDB.Commit();
                            break;
                        }
                    case 52: {
                            this.CurrentSettings.DisplayCurrentTime = true;
                            break;
                        }
                    case 51: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.Name, "current_wle_fp4_10_08") && info.Start < new DateTime(2023, 8, 22)) {
                                    info.Name = "current_wle_fp4_10_08_m";
                                    info.ShowNameId = "current_wle_fp4_10_08_m";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 50: {
                            this.CurrentSettings.EnableFallalyticsReporting = true;
                            break;
                        }
                    case 49: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("wle_s10_cf_round_"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 48: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.ShowNameId, "main_show") &&
                                    this.IsFinalWithCreativeLevel(info.Name)) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 47: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) &&
                                    ((info.ShowNameId.StartsWith("show_wle_s10_wk") || info.ShowNameId.StartsWith("event_wle_s10_wk")) && info.ShowNameId.EndsWith("_mrs")) &&
                                    !this.IsFinalWithCreativeLevel(info.Name)) {
                                    info.IsFinal = false;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.GroupingCreativeRoundLevels = true;
                            break;
                        }
                    case 46: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 45: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 44: {
                            this.CurrentSettings.ShowChangelog = true;
                            break;
                        }
                    case 43: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.Name, "wle_s10_user_creative_race_round", StringComparison.OrdinalIgnoreCase)) {
                                    info.Name = "user_creative_race_round";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 42: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.NotifyServerConnected = false;
                            break;
                        }
                    case 41: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.NotifyServerConnected = false;
                            break;
                        }
                    case 40: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if ((!string.IsNullOrEmpty(info.ShowNameId) && info.ShowNameId.StartsWith("wle_mrs_bagel")) && info.Name.StartsWith("wle_mrs_bagel_final")) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.NotifyServerConnected = false;
                            break;
                        }
                    case 39: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.NotifyServerConnected = false;
                            break;
                        }
                    case 38: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                                    (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                                     info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                                     info.ShowNameId.StartsWith("current_wle_fp"))) {
                                    info.IsFinal = true;
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            this.CurrentSettings.NotifyServerConnected = false;
                            break;
                        }
                    case 37: {
                            this.AllProfiles.AddRange(this.Profiles.FindAll());
                            for (int i = this.AllProfiles.Count - 1; i >= 0; i--) {
                                Profiles profiles = this.AllProfiles[i];
                                if (string.Equals(profiles.LinkedShowId, "event_only_survival_ss2_3009_0210_2022")) {
                                    profiles.LinkedShowId = "survival_of_the_fittest";
                                }
                            }
                            this.StatsDB.BeginTrans();
                            this.Profiles.DeleteAll();
                            this.Profiles.InsertBulk(this.AllProfiles);
                            this.StatsDB.Commit();
                            this.AllProfiles.Clear();
                            break;
                        }
                    case 36: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.Name, "round_follow-the-leader_ss2_launch", StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(info.Name, "round_follow-the-leader_ss2_parrot", StringComparison.OrdinalIgnoreCase)) {
                                    info.Name = "round_follow_the_line";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 35: {
                            this.CurrentSettings.AutoUpdate = true;
                            break;
                        }
                    case 34: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (info.UseShareCode && info.CreativeLastModifiedDate != DateTime.MinValue && string.IsNullOrEmpty(info.CreativeOnlinePlatformId)) {
                                    info.CreativeOnlinePlatformId = "eos";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.CurrentSettings.FilterType = 1;
                            this.CurrentSettings.SelectedCustomTemplateSeason = -1;
                            this.CurrentSettings.CustomFilterRangeStart = DateTime.MinValue;
                            this.CurrentSettings.CustomFilterRangeEnd = DateTime.MaxValue;
                            break;
                        }
                    case 33: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.Name, "round_bluejay_40", StringComparison.OrdinalIgnoreCase)) {
                                    info.Name = "round_bluejay";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            break;
                        }
                    case 32: {
                            this.CurrentSettings.FilterType += 1;
                            this.CurrentSettings.SelectedCustomTemplateSeason = -1;
                            this.CurrentSettings.CustomFilterRangeStart = DateTime.MinValue;
                            this.CurrentSettings.CustomFilterRangeEnd = DateTime.MaxValue;
                            break;
                        }
                    case 31: {
                            this.CurrentSettings.OverlayColor = this.CurrentSettings.OverlayColor > 0 ? this.CurrentSettings.OverlayColor + 1 : this.CurrentSettings.OverlayColor;
                            break;
                        }
                    case 30: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                if (string.Equals(info.Name, "wle_s10_user_creative_round", StringComparison.OrdinalIgnoreCase)) {
                                    info.Name = "wle_s10_user_creative_race_round";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 29: {
                            this.CurrentSettings.SystemTrayIcon = true;
                            break;
                        }
                    case 28: {
                            this.CurrentSettings.Visible = true;
                            break;
                        }
                    case 27: {
                            this.CurrentSettings.PreventOverlayMouseClicks = false;
                            break;
                        }
                    case 26: {
                            this.CurrentSettings.OverlayBackgroundOpacity = 100;
                            break;
                        }
                    case 25: {
                            this.CurrentSettings.OverlayBackground = 0;
                            this.CurrentSettings.OverlayBackgroundResourceName = string.Empty;
                            this.CurrentSettings.OverlayTabResourceName = string.Empty;
                            this.CurrentSettings.IsOverlayBackgroundCustomized = false;
                            this.CurrentSettings.OverlayFontColorSerialized = string.Empty;
                            break;
                        }
                    case 24: {
                            this.CurrentSettings.WinsFilter = 1;
                            this.CurrentSettings.QualifyFilter = 1;
                            this.CurrentSettings.FastestFilter = 1;
                            break;
                        }
                    case 23: {
                            this.CurrentSettings.OverlayColor = 0;
                            this.CurrentSettings.GameExeLocation = string.Empty;
                            this.CurrentSettings.GameShortcutLocation = string.Empty;
                            this.CurrentSettings.AutoLaunchGameOnStartup = false;
                            break;
                        }
                    case 22: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_slippy_slide", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_slippy_slide";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_follow_the_line", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_follow_the_line";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_slide_chute", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_slide_chute";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_blastballruins", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_blastballruins";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_kraken_attack", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_kraken_attack";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_bluejay", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_bluejay";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 21: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_slippy_slide", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_slippy_slide";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_follow_the_line", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_follow_the_line";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_slide_chute", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_slide_chute";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_blastballruins", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_blastballruins";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_kraken_attack", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_kraken_attack";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_bluejay", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_bluejay";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 20: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_follow-the-leader", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_follow-the-leader_s6_launch";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 19: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_satellitehoppers", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_satellitehoppers_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_ffa_button_bashers", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_ffa_button_bashers_squads_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_hoverboardsurvival2", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_hoverboardsurvival2_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_gauntlet_10", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_gauntlet_10_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_starlink", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_starlink_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_tiptoefinale", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_tiptoefinale_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_pixelperfect_almond";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_hexsnake", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_hexsnake_almond";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 18: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_1v1_volleyfall_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_gauntlet_09", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_gauntlet_09_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_short_circuit_2", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_short_circuit_2_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_hoops_revenge", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_hoops_revenge_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_hexaring", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_hexaring_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_spin_ring", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_spin_ring_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_blastball", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_blastball_arenasurvival_symphony_launch_show";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 17: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = AllStats[i];
                                
                                if (info.Name.IndexOf("round_invisibeans", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_invisibeans";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 16: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = AllStats[i];
                                
                                if (info.Name.IndexOf("round_fruit_bowl", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_fruit_bowl";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 15: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_gauntlet_08", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_gauntlet_08";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_airtime", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_airtime";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_follow-", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_follow-the-leader_s6_launch";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_pipedup", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_pipedup_s6_launch";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_see_saw_360", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_see_saw_360";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 14: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = AllStats[i];
                                
                                if (info.Name.IndexOf("round_king_of", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_king_of_the_hill";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_drumtop", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_drumtop";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_penguin_solos", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_penguin_solos";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_gauntlet_07", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_gauntlet_07";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_robotrampage", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_robotrampage_arena_2";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_crown_maze", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_crown_maze";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 13:
                    case 12: {
                            if (version == 12) ++version;
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (info.Name.IndexOf("round_fruitpunch", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_fruitpunch_s4_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_hoverboardsurvival", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_hoverboardsurvival_s4_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_basketfall", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_basketfall_s4_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_territory", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_territory_control_s4_show";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_shortcircuit", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_shortcircuit";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_gauntlet_06", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_gauntlet_06";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_tunnel_race", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_tunnel_race";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_1v1_button", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_1v1_button_basher";
                                    this.RoundDetails.Update(info);
                                } else if (info.Name.IndexOf("round_slimeclimb", StringComparison.OrdinalIgnoreCase) == 0) {
                                    info.Name = "round_slimeclimb_2";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 11: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.AllStats.Sort();
                            this.StatsDB.BeginTrans();
                            int lastShow = -1;
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (lastShow != info.ShowID) {
                                    lastShow = info.ShowID;
                                    info.IsFinal = this.StatLookup.TryGetValue(info.Name, out LevelStats stats) && stats.IsFinal && (info.Name != "round_floor_fall" || info.Round >= 3 || (i > 0 && this.AllStats[i - 1].Name != "round_floor_fall"));
                                } else {
                                    info.IsFinal = false;
                                }
                                
                                this.RoundDetails.Update(info);
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 10: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                int index;
                                if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0
                                    || (index = info.Name.IndexOf(". D", StringComparison.OrdinalIgnoreCase)) > 0) {
                                    info.Name = info.Name.Substring(0, index);
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 9: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                if (string.Equals(info.Name, "round_fall_mountain", StringComparison.OrdinalIgnoreCase)) {
                                    info.Name = "round_fall_mountain_hub_complete";
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 8: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                int index;
                                if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0) {
                                    info.Name = info.Name.Substring(0, index);
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 7: {
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                int index;
                                if ((index = info.Name.IndexOf("_hard_mode", StringComparison.OrdinalIgnoreCase)) > 0) {
                                    info.Name = info.Name.Substring(0, index);
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 6:
                    case 5: {
                            if (version == 5) ++version;
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                int index;
                                if ((index = info.Name.IndexOf("_northernlion", StringComparison.OrdinalIgnoreCase)) > 0) {
                                    info.Name = info.Name.Substring(0, index);
                                    RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 4:
                    case 3: {
                            if (version == 3) ++version;
                            this.AllStats.AddRange(this.RoundDetails.FindAll());
                            this.StatsDB.BeginTrans();
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo info = this.AllStats[i];
                                
                                int index;
                                if ((index = info.Name.IndexOf("_variation", StringComparison.OrdinalIgnoreCase)) > 0) {
                                    info.Name = info.Name.Substring(0, index);
                                    this.RoundDetails.Update(info);
                                }
                            }
                            this.StatsDB.Commit();
                            this.AllStats.Clear();
                            break;
                        }
                    case 2: {
                            this.CurrentSettings.SwitchBetweenStreaks = this.CurrentSettings.SwitchBetweenLongest;
                            break;
                        }
                    case 1: {
                            this.CurrentSettings.SwitchBetweenPlayers = this.CurrentSettings.SwitchBetweenLongest;
                            break;
                        }
                    case 0: {
                            this.CurrentSettings.SwitchBetweenQualify = this.CurrentSettings.SwitchBetweenLongest;
                            break;
                        }
                }
            }
            if (this.CurrentSettings.Version < lastVersion) {
                this.CurrentSettings.Version = lastVersion;
                this.SaveUserSettings();
            }
        }
        
        private UserSettings GetDefaultSettings() {
            return new UserSettings {
                ID = 1,
                Theme = 0,
                CycleTimeSeconds = 5,
                FilterType = 1,
                CustomFilterRangeStart = DateTime.MinValue,
                CustomFilterRangeEnd = DateTime.MaxValue,
                SelectedCustomTemplateSeason = -1,
                SelectedProfile = 0,
                LogPath = null,
                OverlayBackground = 0,
                OverlayBackgroundResourceName = string.Empty,
                OverlayTabResourceName = string.Empty,
                OverlayBackgroundOpacity = 100,
                IsOverlayBackgroundCustomized = false,
                NotifyServerConnected = false,
                NotifyPersonalBest = false,
                MuteNotificationSounds = false,
                NotificationSounds = 0,
                NotificationWindowPosition = 0,
                NotificationWindowAnimation = 0,
                OverlayColor = 0,
                OverlayLocationX = null,
                OverlayLocationY = null,
                OverlayFixedPosition = string.Empty,
                OverlayFixedPositionX = null,
                OverlayFixedPositionY = null,
                OverlayFixedWidth = null,
                OverlayFixedHeight = null,
                LockButtonLocation = 0,
                FlippedDisplay = false,
                FixedFlippedDisplay = false,
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
                OverlayFontSerialized = string.Empty,
                OverlayFontColorSerialized = string.Empty,
                PlayerByConsoleType = false,
                ColorByRoundType = false,
                AutoChangeProfile = false,
                ShadeTheFlagImage = false,
                DisplayCurrentTime = false,
                DisplayGamePlayedInfo = false,
                CountPlayersDuringTheLevel = true,
                PreviousWins = 0,
                WinsFilter = 1,
                QualifyFilter = 1,
                FastestFilter = 1,
                HideWinsInfo = false,
                HideRoundInfo = false,
                HideTimeInfo = false,
                ShowOverlayTabs = false,
                ShowPercentages = false,
                AutoUpdate = true,
                MaximizedWindowState = false,
                SystemTrayIcon = true,
                PreventOverlayMouseClicks = false,
                FormLocationX = null,
                FormLocationY = null,
                FormWidth = null,
                FormHeight = null,
                OverlayWidth = 786,
                OverlayHeight = 99,
                HideOverlayPercentages = false,
                HoopsieHeros = false,
                AutoLaunchGameOnStartup = false,
                GameExeLocation = string.Empty,
                GameShortcutLocation = string.Empty,
                IgnoreLevelTypeWhenSorting = false,
                GroupingCreativeRoundLevels = false,
                RecordEscapeDuringAGame = false,
                UpdatedDateFormat = true,
                WinPerDayGraphStyle = 0,
                EnableFallalyticsReporting = false,
                EnableFallalyticsWeeklyCrownLeague = false,
                EnableFallalyticsAnonymous = false,
                UseProxyServer = false,
                ProxyAddress = string.Empty,
                ProxyPort = string.Empty,
                EnableProxyAuthentication = false,
                ProxyUsername = string.Empty,
                ProxyPassword = string.Empty,
                SucceededTestProxy = false,
                IpGeolocationService = 0,
                ShowChangelog = true,
                Visible = true,
                LevelTimeLimitVersion = 0,
                Version = 0
            };
        }
        
        public int GetOverlaySetting() {
            return (this.CurrentSettings.HideWinsInfo ? 4 : 0) + (this.CurrentSettings.HideRoundInfo ? 2 : 0) + (this.CurrentSettings.HideTimeInfo ? 1 : 0);
        }
        
        private bool IsFinalWithCreativeLevel(string levelId) {
            return string.Equals(levelId, "wle_s10_orig_round_010") ||
                   string.Equals(levelId, "wle_s10_orig_round_011") ||
                   string.Equals(levelId, "wle_s10_orig_round_017") ||
                   string.Equals(levelId, "wle_s10_orig_round_018") ||
                   string.Equals(levelId, "wle_s10_orig_round_024") ||
                   string.Equals(levelId, "wle_s10_orig_round_025") ||
                   string.Equals(levelId, "wle_s10_orig_round_030") ||
                   string.Equals(levelId, "wle_s10_orig_round_031") ||
                   string.Equals(levelId, "wle_s10_round_004") ||
                   string.Equals(levelId, "wle_s10_round_009");
        }
        
        private void UpdateHoopsieLegends() {
            if (this.StatLookup.TryGetValue("round_hoops_blockade_solo", out LevelStats level)) {
                level.Name = this.CurrentSettings.HoopsieHeros ? Multilingual.GetWord("main_hoopsie_heroes") : Multilingual.GetWord("main_hoopsie_legends");
            }
        }
        
        private void UpdateGridRoundName() {
            foreach (KeyValuePair<string, string> item in Multilingual.GetLevelsDictionary().Where(r => r.Key.StartsWith("round_") || r.Key.StartsWith("user_creative_") || r.Key.StartsWith("creative_"))) {
                if (this.StatLookup.TryGetValue(item.Key, out LevelStats level)) {
                    level.Name = item.Value;
                }
            }
            this.SortGridDetails(true);
            this.gridDetails.Invalidate();
        }
        
        public void UpdateDates() {
            if (DateTime.Now.Date.ToUniversalTime() == DayStart) return;

            DateTime currentUTC = DateTime.UtcNow;
            for (int i = Seasons.Count() - 1; i >= 0; i--) {
                if (currentUTC > Seasons[i].StartDate) {
                    SeasonStart = Seasons[i].StartDate;
                    break;
                }
            }
            WeekStart = DateTime.Now.Date.AddDays(-7).ToUniversalTime();
            DayStart = DateTime.Now.Date.ToUniversalTime();

            this.ResetStats();
        }
        
        public void SaveUserSettings() {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                this.UserSettings.Update(this.CurrentSettings);
                this.StatsDB.Commit();
            }
        }
        
        public void ResetStats() {
            for (int i = 0; i < this.StatDetails.Count; i++) {
                LevelStats calculator = this.StatDetails[i];
                calculator.Clear();
            }

            this.ClearTotals();

            List<RoundInfo> rounds = new List<RoundInfo>();
            int profile = this.GetCurrentProfileId();

            lock (this.StatsDB) {
                this.AllStats.Clear();
                this.nextShowID = 0;
                this.lastAddedShow = DateTime.MinValue;
                if (this.RoundDetails.Count() > 0) {
                    this.AllStats.AddRange(this.RoundDetails.FindAll());
                    this.AllStats.Sort();

                    if (this.AllStats.Count > 0) {
                        this.nextShowID = this.AllStats[this.AllStats.Count - 1].ShowID;

                        int lastAddedShowId = -1;
                        for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                            RoundInfo info = this.AllStats[i];
                            info.ToLocalTime();
                            if (info.Profile != profile) continue;

                            if (info.ShowID == lastAddedShowId || (IsInStatsFilter(info) && IsInPartyFilter(info))) {
                                lastAddedShowId = info.ShowID;
                                rounds.Add(info);
                            }

                            if (info.Start > lastAddedShow && info.Round == 1) {
                                this.lastAddedShow = info.Start;
                            }
                        }
                    }
                }
            }

            lock (this.CurrentRound) {
                this.CurrentRound.Clear();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    if (info.Profile != profile) continue;

                    this.CurrentRound.Insert(0, info);
                    if (info.Round == 1) {
                        break;
                    }
                }
            }

            rounds.Sort();
            this.loadingExisting = true;
            this.LogFile_OnParsedLogLines(rounds);
            this.loadingExisting = false;
        }
        
        private void menuUsefulThings_Click(object sender, EventArgs e) {
            try {
                if (sender.Equals(this.menuFallGuysWiki) || sender.Equals(this.trayFallGuysWiki)) {
                    Process.Start(@"https://fallguysultimateknockout.fandom.com/wiki/Fall_Guys:_Ultimate_Knockout_Wiki");
                } else if (sender.Equals(this.menuFallGuysReddit) || sender.Equals(this.trayFallGuysReddit)) {
                    Process.Start(@"https://www.reddit.com/r/FallGuysGame/");
                } else if (sender.Equals(this.menuFallalyticsMain) || sender.Equals(this.trayFallalyticsMain)) {
                    Process.Start(@"https://fallalytics.com/");
                } else if (sender.Equals(this.menuFallalyticsFindPlayer) || sender.Equals(this.trayFallalyticsFindPlayer)) {
                    Process.Start(@"https://fallalytics.com/player-search");
                } else if (sender.Equals(this.menuFallalyticsOverallRankings) || sender.Equals(this.trayFallalyticsOverallRankings)) {
                    Process.Start(@"https://fallalytics.com/leaderboards/speedrun-total");
                } else if (sender.Equals(this.menuFallalyticsLevelRankings) || sender.Equals(this.trayFallalyticsLevelRankings)) {
                    Process.Start(@"https://fallalytics.com/leaderboards/speedrun");
                } else if (sender.Equals(this.menuFallalyticsWeeklyCrownLeague) || sender.Equals(this.trayFallalyticsWeeklyCrownLeague)) {
                    Process.Start(@"https://fallalytics.com/leaderboards/crowns");
                } else if (sender.Equals(this.menuFallalyticsRoundStatistics) || sender.Equals(this.trayFallalyticsRoundStatistics)) {
                    Process.Start(@"https://fallalytics.com/rounds");
                } else if (sender.Equals(this.menuFallalyticsShowStatistics) || sender.Equals(this.trayFallalyticsShowStatistics)) {
                    Process.Start(@"https://fallalytics.com/shows");
                } else if (sender.Equals(this.menuFallalyticsNews) || sender.Equals(this.trayFallalyticsNews)) {
                    Process.Start(@"https://fallalytics.com/news");
                } else if (sender.Equals(this.menuRollOffClub) || sender.Equals(this.trayRollOffClub)) {
                    if (CurrentLanguage == Language.Korean) {
                        Process.Start(@"https://rolloff.club/ko/");
                    } else if (CurrentLanguage == Language.Japanese) {
                        Process.Start(@"https://rolloff.club/ja/");
                    } else if (CurrentLanguage == Language.SimplifiedChinese) {
                        Process.Start(@"https://rolloff.club/zh/");
                    } else {
                        Process.Start(@"https://rolloff.club/");
                    }
                } else if (sender.Equals(this.menuLostTempleAnalyzer) || sender.Equals(this.trayLostTempleAnalyzer)) {
                    Process.Start(@"https://alexjlockwood.github.io/lost-temple-analyzer/");
                } else if (sender.Equals(this.menuFallGuysDBMain) || sender.Equals(this.trayFallGuysDBMain)) {
                    Process.Start(@"https://fallguys-db.pages.dev/");
                } else if (sender.Equals(this.menuFallGuysDBShows) || sender.Equals(this.trayFallGuysDBShows)) {
                    Process.Start(@"https://fallguys-db.pages.dev/upcoming_shows");
                } else if (sender.Equals(this.menuFallGuysDBDiscovery) || sender.Equals(this.trayFallGuysDBDiscovery)) {
                    Process.Start(@"https://fallguys-db.pages.dev/discovery");
                } else if (sender.Equals(this.menuFallGuysDBShop) || sender.Equals(this.trayFallGuysDBShop)) {
                    Process.Start(@"https://fallguys-db.pages.dev/store");
                } else if (sender.Equals(this.menuFallGuysDBNewsfeeds) || sender.Equals(this.trayFallGuysDBNewsfeeds)) {
                    Process.Start(@"https://fallguys-db.pages.dev/newsfeeds");
                } else if (sender.Equals(this.menuFallGuysDBStrings) || sender.Equals(this.trayFallGuysDBStrings)) {
                    Process.Start(@"https://fallguys-db.pages.dev/strings");
                } else if (sender.Equals(this.menuFallGuysDBCosmetics) || sender.Equals(this.trayFallGuysDBCosmetics)) {
                    Process.Start(@"https://fallguys-db.pages.dev/unlocks");
                } else if (sender.Equals(this.menuFallGuysDBCrownRanks) || sender.Equals(this.trayFallGuysDBCrownRanks)) {
                    Process.Start(@"https://fallguys-db.pages.dev/crown_ranks");
                } else if (sender.Equals(this.menuFallGuysDBLiveEvents) || sender.Equals(this.trayFallGuysDBLiveEvents)) {
                    Process.Start(@"https://fallguys-db.pages.dev/live_events");
                } else if (sender.Equals(this.menuFallGuysDBDailyShop) || sender.Equals(this.trayFallGuysDBDailyShop)) {
                    Process.Start(@"https://fallguys-db.pages.dev/daily_store");
                } else if (sender.Equals(this.menuFallGuysDBCreative) || sender.Equals(this.trayFallGuysDBCreative)) {
                    Process.Start(@"https://fallguys-db.pages.dev/creative");
                } else if (sender.Equals(this.menuFallGuysDBFaq) || sender.Equals(this.trayFallGuysDBFaq)) {
                    Process.Start(@"https://fallguys-db.pages.dev/faq");
                } else if (sender.Equals(this.menuFallGuysOfficial) || sender.Equals(this.trayFallGuysOfficial)) {
                    Process.Start(@"https://fallguys.com/");
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuUsefulThings_MouseEnter(object sender, EventArgs e) {
            Rectangle rectangle = this.menuUsefulThings.Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 260);
            this.AllocCustomTooltip(this.cmtt_center_Draw);
            if (sender.Equals(this.menuFallGuysWiki)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_wiki_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallGuysReddit)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_reddit_tooltip"), this, position);
            } else if (sender.Equals(this.menuRollOffClub)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_roll_off_club_tooltip"), this, position);
            } else if (sender.Equals(this.menuLostTempleAnalyzer)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_lost_temple_analyzer_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallGuysOfficial)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_official_tooltip"), this, position);
            }
            // else if (sender.Equals(this.menuFallGuysDB)) {
            //     this.ShowCustomTooltip(Multilingual.GetWord("main_todays_show_tooltip"), this, position);
            // } else if (sender.Equals(this.menuFallalytics)) {
            //     this.ShowCustomTooltip(Multilingual.GetWord("main_fallalytics_tooltip"), this, position);
            // }
        }

        private void menuUsefulThings_MouseLeave(object sender, EventArgs e) {
            this.HideCustomTooltip(this);
            this.Cursor = Cursors.Default;
        }

        private void menuUpdate_MouseEnter(object sender, EventArgs e) {
            Rectangle rectangle = ((ToolStripMenuItem)sender).Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 68);
            this.AllocTooltip();
            this.ShowTooltip(sender.Equals(this.menuUpdate) && this.isAvailableNewVersion ? $"{Multilingual.GetWord("main_you_can_update_new_version_prefix_tooltip")}v{this.availableNewVersion}{Multilingual.GetWord("main_you_can_update_new_version_suffix_tooltip")}" :
                $"{Multilingual.GetWord("main_update_prefix_tooltip")}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip")}",
                this, position);
        }
        
        private void menuUpdate_MouseLeave(object sender, EventArgs e) {
            this.HideTooltip(this);
            this.Cursor = Cursors.Default;
        }
        
        private void menuOverlay_MouseEnter(object sender, EventArgs e) {
            this.Cursor = Cursors.Hand;
            Rectangle rectangle = this.menuOverlay.Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 68);
            this.AllocCustomTooltip(this.cmtt_overlay_Draw);
            this.ShowCustomTooltip($"{Multilingual.GetWord(this.overlay.Visible ? "main_overlay_hide_tooltip" : "main_overlay_show_tooltip")}{Environment.NewLine}{Multilingual.GetWord("main_overlay_shortcut_tooltip")}", this, position);
        }
        
        private void menuOverlay_MouseLeave(object sender, EventArgs e) {
            this.HideCustomTooltip(this);
            this.Cursor = Cursors.Default;
        }

        private void setCursor_MouseMove(object sender, MouseEventArgs e) {
            this.Cursor = Cursors.Hand;
        }

        private void setCursor_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
        }
        
        private void trayCMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            this.trayCMenu.Opacity = 0;
        }
        
        private void trayCMenu_Opening(object sender, CancelEventArgs e) {
            this.trayCMenu.Opacity = 100;
        }
        
        private void trayIcon_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                int menuPositionX = 0, menuPositionY = 0;
                switch (this.GetTaskbarPosition()) {
                    case TaskbarPosition.Bottom:
                        if (MousePosition.Y >= Screen.GetWorkingArea(MousePosition).Height) {
                            menuPositionX = MousePosition.X;
                            menuPositionY = Screen.GetWorkingArea(MousePosition).Height - this.trayCMenu.Height;
                        } else {
                            menuPositionX = MousePosition.X + 5;
                            menuPositionY = this.trayCMenu.Location.Y - 5;
                        }
                        break;
                    case TaskbarPosition.Left:
                        if (MousePosition.X <= (Screen.GetBounds(MousePosition).Width - Screen.GetWorkingArea(MousePosition).Width)) {
                            menuPositionX = Screen.GetBounds(MousePosition).Width - Screen.GetWorkingArea(MousePosition).Width;
                            menuPositionY = this.trayCMenu.Location.Y;
                        } else {
                            menuPositionX = MousePosition.X + 5;
                            menuPositionY = this.trayCMenu.Location.Y - 5;
                        }
                        break;
                    case TaskbarPosition.Right:
                        if (MousePosition.X >= Screen.GetWorkingArea(MousePosition).Width) {
                            menuPositionX = Screen.GetWorkingArea(MousePosition).Width - this.trayCMenu.Width;
                            menuPositionY = MousePosition.Y - this.trayCMenu.Height;
                        } else {
                            menuPositionX = MousePosition.X - this.trayCMenu.Width - 5;
                            menuPositionY = this.trayCMenu.Location.Y - 5;
                        }
                        break;
                    case TaskbarPosition.Top:
                        if (MousePosition.Y <= (Screen.GetBounds(MousePosition).Height - Screen.GetWorkingArea(MousePosition).Height)) {
                            menuPositionX = MousePosition.X - this.trayCMenu.Width;
                            menuPositionY = Screen.GetBounds(MousePosition).Height - Screen.GetWorkingArea(MousePosition).Height;
                        } else {
                            menuPositionX = MousePosition.X - this.trayCMenu.Width - 5;
                            menuPositionY = this.trayCMenu.Location.Y + 5;
                        }
                        break;
                }
                this.trayCMenu.Location = new Point(menuPositionX, menuPositionY);
            }
        }
        
        private void trayIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                if (this.Visible && this.WindowState == FormWindowState.Minimized) {
                    this.isFocused = true;
                    this.WindowState = this.maximizedForm ? FormWindowState.Maximized : FormWindowState.Normal;
                    this.TopMost = true;
                    this.TopMost = false;
                    Utils.SetForegroundWindow(Utils.FindWindow(null, this.mainWndTitle));
                } else if (this.Visible && this.WindowState != FormWindowState.Minimized) {
                    if (this.isFocused) {
                        this.isFocused = false;
                        this.Hide();
                    } else {
                        this.isFocused = true;
                        this.TopMost = true;
                        this.TopMost = false;
                        Utils.SetForegroundWindow(Utils.FindWindow(null, this.mainWndTitle));
                    }
                } else {
                    this.isFocused = true;
                    this.Show();
                }
            }
        }
        
        private void trayIcon_MouseMove(object sender, MouseEventArgs e) {
            this.isFocused = ActiveForm == this;
        }
        
        private void Stats_VisibleChanged(object sender, EventArgs e) {
            if (this.Visible) {
                this.TopMost = true;
                this.TopMost = false;
                Utils.SetForegroundWindow(Utils.FindWindow(null, this.mainWndTitle));
                this.SetMainDataGridViewOrder();
            }
        }
        
        private void Stats_Resize(object sender, EventArgs e) {
            this.isFocused = true;
            if (this.WindowState == FormWindowState.Maximized) {
                this.maximizedForm = true;
            } else if (this.WindowState == FormWindowState.Normal) {
                this.maximizedForm = false;
            }
        }
        
        public void SaveWindowState() {
            this.CurrentSettings.Visible = this.Visible;
            if (this.overlay.Visible) {
                if (!this.overlay.IsFixed()) {
                    this.CurrentSettings.OverlayLocationX = this.overlay.Location.X;
                    this.CurrentSettings.OverlayLocationY = this.overlay.Location.Y;
                    this.CurrentSettings.OverlayWidth = this.overlay.Width;
                    this.CurrentSettings.OverlayHeight = this.overlay.Height;
                } else {
                    this.CurrentSettings.OverlayFixedPositionX = this.overlay.Location.X;
                    this.CurrentSettings.OverlayFixedPositionY = this.overlay.Location.Y;
                    this.CurrentSettings.OverlayFixedWidth = this.overlay.Width;
                    this.CurrentSettings.OverlayFixedHeight = this.overlay.Height;
                }
            }
            
            if (this.WindowState != FormWindowState.Normal) {
                this.CurrentSettings.FormLocationX = this.RestoreBounds.Location.X;
                this.CurrentSettings.FormLocationY = this.RestoreBounds.Location.Y;
                this.CurrentSettings.FormWidth = this.RestoreBounds.Size.Width;
                this.CurrentSettings.FormHeight = this.RestoreBounds.Size.Height;
                this.CurrentSettings.MaximizedWindowState = this.WindowState == FormWindowState.Maximized;
            } else {
                this.CurrentSettings.FormLocationX = this.Location.X;
                this.CurrentSettings.FormLocationY = this.Location.Y;
                this.CurrentSettings.FormWidth = this.Size.Width;
                this.CurrentSettings.FormHeight = this.Size.Height;
                this.CurrentSettings.MaximizedWindowState = false;
            }
        }
        
        public void Stats_ExitProgram(object sender, EventArgs e) {
            this.isFormClosing = true;
            this.Close();
        }
        
        private async void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.isFormClosing || !this.CurrentSettings.SystemTrayIcon) {
                try {
                    if (!this.isUpdate && !this.overlay.Disposing && !this.overlay.IsDisposed && !this.IsDisposed && !this.Disposing) {
                        this.SaveWindowState();
                        this.SaveUserSettings();
                    }
                    await this.logFile.Stop();
                } catch (Exception ex) {
                    MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Stats_FormClosed(object sender, FormClosedEventArgs e) {
            this.StatsDB?.Dispose();
        }

        private void Stats_Load(object sender, EventArgs e) {
            try {
                if (Utils.IsInternetConnected()) {
                    Task.Run(() => { HostCountryCode = Utils.GetCountryCode(Utils.GetUserPublicIp()); });
                }
                
                this.SetTheme(CurrentTheme);
                this.infoStrip.Renderer = new CustomToolStripSystemRenderer();
                this.infoStrip2.Renderer = new CustomToolStripSystemRenderer();

                this.UpdateDates();
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string TranslateChangelog(string s) {
            string[] lines = s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string rtnStr = string.Empty;
            for (int i = 0; i < lines.Length; i++) {
                if (i > 0) rtnStr += Environment.NewLine;
                rtnStr += CurrentLanguage == Language.English || string.IsNullOrEmpty(Multilingual.GetWord(lines[i].Replace("  - ", "message_changelog_").Replace(" ", "_")))
                          ? lines[i]
                          : $"  - {Multilingual.GetWord(lines[i].Replace("  - ", "message_changelog_").Replace(" ", "_"))}";
            }
            for (int i = 0; i < 5 - lines.Length; i++) {
                rtnStr += Environment.NewLine;
            }
            return rtnStr;
        }

        private void InitLogData() {
            this.ClearServerConnectionLog(5);
            this.ClearPersonalBestLog(15);
            this.ClearWeeklyCrownLog(15);
            this.FallalyticsPbLogCache = this.FallalyticsPbLog.FindAll().ToList();
            this.FallalyticsCrownLogCache = this.FallalyticsCrownLog.FindAll().ToList();
            this.ServerConnectionLogCache = this.ServerConnectionLog.FindAll().ToList();
            this.PersonalBestLogCache = this.PersonalBestLog.FindAll().ToList();
        }
        
        private void Stats_Shown(object sender, EventArgs e) {
            try {
#if AllowUpdate
                if (Utils.IsInternetConnected()) {
                    if (this.CurrentSettings.AutoUpdate) {
                        if (this.CheckForUpdate(true)) {
                            this.Stats_ExitProgram(this, null);
                            return;
                        }
                    } else {
                        this.CheckForNewVersion();
                    }

                    if (this.CurrentSettings.ShowChangelog) {
                        try {
                            string changelog = Utils.GetApiData(Utils.FALLGUYSSTATS_RELEASES_LATEST_INFO_URL).GetProperty("body").GetString();
                            changelog = changelog?.Substring(0, changelog.IndexOf($"{Environment.NewLine}{Environment.NewLine}<br>{Environment.NewLine}{Environment.NewLine}", StringComparison.OrdinalIgnoreCase));
                        
                            MetroMessageBox.Show(this,
                                $"{this.TranslateChangelog(changelog)}{Multilingual.GetWord("main_update_prefix_tooltip").Trim()}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip").Trim()}",
                                $"{Multilingual.GetWord("message_changelog_caption")} - {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        
                            this.CurrentSettings.ShowChangelog = false;
                            this.SaveUserSettings();
                        } catch {
                            // ignored
                        }
                    }
                }
#endif
                this.RemoveUpdateFiles();
                this.InitLogData();
                
                this.scrollTimer.Tick += this.scrollTimer_Tick;
                if (this.CurrentSettings.Visible) {
                    this.Show();
                    this.ShowInTaskbar = true;
                    this.Opacity = 1;
                } else {
                    this.Hide();
                    this.ShowInTaskbar = true;
                    this.Opacity = 1;
                }
                this.SetMainDataGridViewOrder();
                InstalledEmojiFont = Utils.IsFontInstalled("Segoe UI Emoji");

                if (this.WindowState != FormWindowState.Minimized) {
                    this.WindowState = this.CurrentSettings.MaximizedWindowState ? FormWindowState.Maximized : FormWindowState.Normal;
                }
                if (this.CurrentSettings.FormWidth.HasValue) {
                    this.Size = new Size(this.CurrentSettings.FormWidth.Value, this.CurrentSettings.FormHeight.Value);
                }
                if (this.CurrentSettings.FormLocationX.HasValue && Utils.IsOnScreen(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value, this.Width, this.Height)) {
                    this.Location = new Point(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value);
                }
                
                string logFilePath = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low", "Mediatonic", "FallGuys_client");
                if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath) && Directory.Exists(this.CurrentSettings.LogPath)) {
                    logFilePath = this.CurrentSettings.LogPath;
                }
                this.logFile.Start(logFilePath, LOGFILENAME);

                this.overlay.ArrangeDisplay(string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.FlippedDisplay : this.CurrentSettings.FixedFlippedDisplay, this.CurrentSettings.ShowOverlayTabs,
                    this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo,
                    this.CurrentSettings.OverlayColor, this.CurrentSettings.LockButtonLocation,
                    string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayWidth : this.CurrentSettings.OverlayFixedWidth,
                    string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayHeight : this.CurrentSettings.OverlayFixedHeight,
                    this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
                if (this.CurrentSettings.OverlayVisible) { this.ToggleOverlay(this.overlay); }
                
                this.ReloadProfileMenuItems();
                
                this.selectedCustomTemplateSeason = this.CurrentSettings.SelectedCustomTemplateSeason;
                this.customfilterRangeStart = this.CurrentSettings.CustomFilterRangeStart;
                this.customfilterRangeEnd = this.CurrentSettings.CustomFilterRangeEnd;
                this.menuAllStats.Checked = false;
                this.trayAllStats.Checked = false;
                switch (this.CurrentSettings.FilterType) {
                    case 0:
                        this.menuCustomRangeStats.Checked = true;
                        this.trayCustomRangeStats.Checked = true;
                        this.menuStats_Click(this.menuCustomRangeStats, EventArgs.Empty);
                        break;
                    case 1:
                        this.menuAllStats.Checked = true;
                        this.trayAllStats.Checked = true;
                        this.menuStats_Click(this.menuAllStats, EventArgs.Empty);
                        break;
                    case 2:
                        this.menuSeasonStats.Checked = true;
                        this.traySeasonStats.Checked = true;
                        this.menuStats_Click(this.menuSeasonStats, EventArgs.Empty);
                        break;
                    case 3:
                        this.menuWeekStats.Checked = true;
                        this.trayWeekStats.Checked = true;
                        this.menuStats_Click(this.menuWeekStats, EventArgs.Empty);
                        break;
                    case 4:
                        this.menuDayStats.Checked = true;
                        this.trayDayStats.Checked = true;
                        this.menuStats_Click(this.menuDayStats, EventArgs.Empty);
                        break;
                    case 5:
                        this.menuSessionStats.Checked = true;
                        this.traySessionStats.Checked = true;
                        this.menuStats_Click(this.menuSessionStats, EventArgs.Empty);
                        break;
                }
                
                this.SetWindowCorner();
                this.isStartingUp = false;
                
                if (this.CurrentSettings.AutoLaunchGameOnStartup) {
                    this.LaunchGame(true);
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LogFile_OnError(string error) {
            if (!this.Disposing && !this.IsDisposed) {
                try {
                    if (this.InvokeRequired) {
                        this.Invoke((Action<string>)LogFile_OnError, error);
                    } else {
                        MetroMessageBox.Show(this, error, $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch {
                    // ignored
                }
            }
        }

        private void LogFile_OnPersonalBestNotification(string showNameId, string roundId, TimeSpan existingRecord, TimeSpan currentRecord) {
            string timeDiffContent = String.Empty;
            if (existingRecord != TimeSpan.MaxValue) {
                TimeSpan timeDiff = existingRecord - currentRecord;
                timeDiffContent = timeDiff.Minutes > 0 ? $" ⏱️{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_prefix")}{timeDiff.Minutes}{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_infix")} {timeDiff.Seconds}.{timeDiff.Milliseconds}{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_suffix")}"
                                  : $" ⏱️{timeDiff.Seconds}.{timeDiff.Milliseconds}{Multilingual.GetWord("message_new_personal_best_timediff_by_second")}";
            }
            string levelName = this.StatLookup.TryGetValue(roundId, out LevelStats l1) ? l1.Name : roundId;
            string showName = $"{(string.Equals(Multilingual.GetShowName(showNameId), levelName) ? $"({levelName})" : $"({Multilingual.GetShowName(showNameId)} • {levelName})")}";
            string description = $"{Multilingual.GetWord("message_new_personal_best_prefix")}{showName}{Multilingual.GetWord("message_new_personal_best_suffix")}{timeDiffContent}";
            ToastPosition toastPosition = Enum.TryParse(this.CurrentSettings.NotificationWindowPosition.ToString(), out ToastPosition position) ? position : ToastPosition.BottomRight;
            ToastTheme toastTheme = this.Theme == MetroThemeStyle.Light ? ToastTheme.Light : ToastTheme.Dark;
            ToastAnimation toastAnimation = this.CurrentSettings.NotificationWindowAnimation == 0 ? ToastAnimation.FADE : ToastAnimation.SLIDE;
            ToastSound toastSound = Enum.TryParse(this.CurrentSettings.NotificationSounds.ToString(), out ToastSound sound) ? sound : ToastSound.Generic01;
            this.ShowToastNotification(this, null, Multilingual.GetWord("message_new_personal_best_caption"), description, Overlay.GetMainFont(16, FontStyle.Bold, CurrentLanguage),
                null, ToastDuration.MEDIUM, toastPosition, toastAnimation, toastTheme, toastSound, this.CurrentSettings.MuteNotificationSounds, true);
        }

        private void LogFile_OnServerConnectionNotification() {
            string countryFullName;
            if (!string.IsNullOrEmpty(LastCountryAlpha2Code)) {
                countryFullName = Multilingual.GetCountryName(LastCountryAlpha2Code);
                if (!string.IsNullOrEmpty(LastCountryRegion)) {
                    countryFullName += $" ({LastCountryRegion}";
                    if (!string.IsNullOrEmpty(LastCountryCity)) {

                        if (!string.Equals(LastCountryCity, LastCountryRegion)) {
                            countryFullName += $", {LastCountryCity})";
                        } else {
                            countryFullName += ")";
                        }
                    } else {
                        countryFullName += ")";
                    }
                } else {
                    if (!string.IsNullOrEmpty(LastCountryCity)) {
                        countryFullName += $" ({LastCountryCity})";
                    }
                }
            } else {
                countryFullName = "UNKNOWN";
            }
            string description = $"{Multilingual.GetWord("message_connected_to_server_prefix")}{countryFullName}{Multilingual.GetWord("message_connected_to_server_suffix")}";
            Image flagImage = (Image)Properties.Resources.ResourceManager.GetObject($"country_{(string.IsNullOrEmpty(LastCountryAlpha2Code) ? "unknown" : LastCountryAlpha2Code)}{(this.CurrentSettings.ShadeTheFlagImage ? "_shiny" : "")}_icon");
            ToastPosition toastPosition = Enum.TryParse(this.CurrentSettings.NotificationWindowPosition.ToString(), out ToastPosition position) ? position : ToastPosition.BottomRight;
            ToastTheme toastTheme = this.Theme == MetroThemeStyle.Light ? ToastTheme.Light : ToastTheme.Dark;
            ToastAnimation toastAnimation = this.CurrentSettings.NotificationWindowAnimation == 0 ? ToastAnimation.FADE : ToastAnimation.SLIDE;
            ToastSound toastSound = Enum.TryParse(this.CurrentSettings.NotificationSounds.ToString(), out ToastSound sound) ? sound : ToastSound.Generic01;
            this.ShowToastNotification(this, null, Multilingual.GetWord("message_connected_to_server_caption"), description, Overlay.GetMainFont(16, FontStyle.Bold, CurrentLanguage),
                flagImage, ToastDuration.MEDIUM, toastPosition, toastAnimation, toastTheme, toastSound, this.CurrentSettings.MuteNotificationSounds, true);
        }
        
        private void LogFile_OnNewLogFileDate(DateTime newDate) {
            if (SessionStart != newDate) {
                SessionStart = newDate;
                if (this.menuSessionStats.Checked) {
                    this.menuStats_Click(this.menuSessionStats, EventArgs.Empty);
                }
            }
        }
        
        private void LogFile_OnParsedLogLinesCurrent(List<RoundInfo> round) {
            lock (this.CurrentRound) {
                if (this.CurrentRound == null || this.CurrentRound.Count != round.Count) {
                    this.CurrentRound = round;
                } else {
                    for (int i = 0; i < this.CurrentRound.Count; i++) {
                        RoundInfo info = this.CurrentRound[i];
                        if (!info.Equals(round[i])) {
                            this.CurrentRound = round;
                            break;
                        }
                    }
                }
            }
        }
        
        private void LogFile_OnParsedLogLines(List<RoundInfo> round) {
            try {
                if (this.InvokeRequired) {
                    this.Invoke((Action<List<RoundInfo>>)this.LogFile_OnParsedLogLines, round);
                    return;
                }

                lock (this.StatsDB) {
                    if (!this.loadingExisting) { this.StatsDB.BeginTrans(); }

                    int profile = this.currentProfile;
                    foreach (var stat in round) {
                        if (!this.loadingExisting) {
                            RoundInfo info = null;
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo temp = this.AllStats[i];
                                if (temp.Start == stat.Start && temp.Name == stat.Name) {
                                    info = temp;
                                    break;
                                }
                            }

                            if (info == null && stat.Start > this.lastAddedShow) {
                                if (stat.ShowEnd < this.startupTime && this.askedPreviousShows == 0) {
                                    IsDisplayOverlayTime = false;
                                    using (EditShows editShows = new EditShows()) {
                                        editShows.FunctionFlag = "add";
                                        editShows.Profiles = this.AllProfiles;
                                        editShows.StatsForm = this;
                                        this.EnableInfoStrip(false);
                                        this.EnableMainMenu(false);
                                        if (editShows.ShowDialog(this) == DialogResult.OK) {
                                            this.askedPreviousShows = 1;
                                            if (editShows.UseLinkedProfiles) {
                                                this.useLinkedProfiles = true;
                                            } else {
                                                profile = editShows.SelectedProfileId;
                                                this.CurrentSettings.SelectedProfile = profile;
                                                this.SetProfileMenu(profile);
                                            }
                                        } else {
                                            this.askedPreviousShows = 2;
                                        }
                                        this.EnableInfoStrip(true);
                                        this.EnableMainMenu(true);
                                        IsDisplayOverlayTime = true;
                                    }
                                }
                                
                                if (stat.ShowEnd < this.startupTime && this.askedPreviousShows == 2) {
                                    continue;
                                }
                                
                                if (stat.ShowEnd < this.startupTime && this.useLinkedProfiles) {
                                    profile = this.GetLinkedProfileId(stat.ShowNameId, stat.PrivateLobby);
                                    this.CurrentSettings.SelectedProfile = profile;
                                    this.SetProfileMenu(profile);
                                }

                                if (stat.Round == 1) {
                                    this.nextShowID++;
                                    this.lastAddedShow = stat.Start;
                                }
                                stat.ShowID = this.nextShowID;
                                stat.Profile = profile;

                                if (((this.StatLookup.TryGetValue(stat.Name, out LevelStats l1) && l1.IsCreative && !string.IsNullOrEmpty(l1.ShareCode) && string.IsNullOrEmpty(stat.CreativeTitle))
                                      || (stat.UseShareCode && !string.IsNullOrEmpty(stat.CreativeShareCode) && (string.Equals(stat.ShowNameId, "unknown") || string.IsNullOrEmpty(stat.CreativeTitle)))) && Utils.IsInternetConnected()) {
                                    string shareCode = stat.UseShareCode ? stat.CreativeShareCode : l1.ShareCode;
                                    RoundInfo ri = this.GetRoundInfoFromShareCode(shareCode);
                                    if (ri != null) {
                                        if (stat.UseShareCode) { stat.ShowNameId = ri.ShowNameId; }
                                        stat.CreativeOnlinePlatformId = ri.CreativeOnlinePlatformId;
                                        stat.CreativeAuthor = ri.CreativeAuthor;
                                        stat.CreativeShareCode = shareCode;
                                        stat.CreativeVersion = ri.CreativeVersion;
                                        stat.CreativeStatus = ri.CreativeStatus;
                                        stat.CreativeTitle = ri.CreativeTitle;
                                        stat.CreativeDescription = ri.CreativeDescription;
                                        stat.CreativeCreatorTags = ri.CreativeCreatorTags;
                                        stat.CreativeMaxPlayer = ri.CreativeMaxPlayer;
                                        stat.CreativeThumbUrl = ri.CreativeThumbUrl;
                                        stat.CreativePlatformId = ri.CreativePlatformId;
                                        stat.CreativeGameModeId = ri.CreativeGameModeId;
                                        stat.CreativeLevelThemeId = ri.CreativeLevelThemeId;
                                        stat.CreativeLastModifiedDate = ri.CreativeLastModifiedDate;
                                        stat.CreativePlayCount = ri.CreativePlayCount;
                                        stat.CreativeLikes = ri.CreativeLikes;
                                        stat.CreativeDislikes = ri.CreativeDislikes;
                                        stat.CreativeQualificationPercent = ri.CreativeQualificationPercent;
                                        stat.CreativeTimeLimitSeconds = ri.CreativeTimeLimitSeconds;
                                    } else {
                                        try {
                                            JsonElement resData = Utils.GetApiData(Utils.FALLGUYSDB_API_URL, $"creative/{shareCode}.json");
                                            JsonElement je = resData.GetProperty("data");
                                            JsonElement snapshot = je.GetProperty("snapshot");
                                            JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                                            JsonElement stats = snapshot.GetProperty("stats");
                                            if (stat.UseShareCode) { stat.ShowNameId = this.GetUserCreativeLevelTypeId(versionMetadata.GetProperty("game_mode_id").GetString()); }
                                            string[] onlinePlatformInfo = this.FindUserCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                                            // stat.CreativeShareCode = snapshot.GetProperty("share_code").GetString();
                                            stat.CreativeShareCode = shareCode;
                                            stat.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                            stat.CreativeAuthor = onlinePlatformInfo[1];
                                            stat.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                            stat.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                            stat.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                            stat.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                            if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                                                string temps = string.Empty;
                                                foreach (JsonElement t in creatorTags.EnumerateArray()) {
                                                    if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                                                    temps += t.GetString();
                                                }
                                                stat.CreativeCreatorTags = temps;
                                            }
                                            stat.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                            stat.CreativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                                            stat.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                            stat.CreativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString() ?? "GAMEMODE_GAUNTLET";
                                            stat.CreativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString() ?? "THEME_VANILLA";
                                            stat.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                            stat.CreativePlayCount = stats.GetProperty("play_count").GetInt32();
                                            stat.CreativeLikes = stats.GetProperty("likes").GetInt32();
                                            stat.CreativeDislikes = stats.GetProperty("dislikes").GetInt32();
                                            stat.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                            stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                            // var stat1 = stat;
                                            // Task.Run(() => { this.UpdateCreativeLevels(stat1.Name, shareCode, snapshot); });
                                        } catch {
                                            try {
                                                JsonElement resData = Utils.GetApiData(Utils.FGANALYST_API_URL, $"creative/?share_code={shareCode}");
                                                JsonElement je = resData.GetProperty("level_data");
                                                JsonElement levelData = je[0];
                                                JsonElement versionMetadata = levelData.GetProperty("version_metadata");
                                                JsonElement stats = levelData.GetProperty("stats");
                                                if (stat.UseShareCode) { stat.ShowNameId = this.GetUserCreativeLevelTypeId(versionMetadata.GetProperty("game_mode_id").GetString()); }
                                                string[] onlinePlatformInfo = this.FindUserCreativeAuthor(levelData.GetProperty("author").GetProperty("name_per_platform"));
                                                // stat.CreativeShareCode = levelData.GetProperty("share_code").GetString();
                                                stat.CreativeShareCode = shareCode;
                                                stat.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                                stat.CreativeAuthor = onlinePlatformInfo[1];
                                                stat.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                                stat.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                                stat.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                                stat.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                                if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                                                    string temps = string.Empty;
                                                    foreach (JsonElement t in creatorTags.EnumerateArray()) {
                                                        if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                                                        temps += t.GetString();
                                                    }
                                                    stat.CreativeCreatorTags = temps;
                                                }
                                                stat.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                                stat.CreativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                                                stat.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                                stat.CreativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString() ?? "GAMEMODE_GAUNTLET";
                                                stat.CreativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString() ?? "THEME_VANILLA";
                                                stat.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                                stat.CreativePlayCount = stats.GetProperty("play_count").GetInt32();
                                                stat.CreativeLikes = stats.GetProperty("likes").GetInt32();
                                                stat.CreativeDislikes = stats.GetProperty("dislikes").GetInt32();
                                                stat.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                                stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                                // var stat1 = stat;
                                                // Task.Run(() => { this.UpdateCreativeLevels(stat1.Name, shareCode, levelData); });
                                            } catch {
                                                stat.CreativeOnlinePlatformId = string.Empty;
                                                stat.CreativeAuthor = string.Empty;
                                                stat.CreativeShareCode = string.Empty;
                                                stat.CreativeVersion = 0;
                                                stat.CreativeStatus = string.Empty;
                                                stat.CreativeTitle = string.Empty;
                                                stat.CreativeDescription = string.Empty;
                                                stat.CreativeCreatorTags = string.Empty;
                                                stat.CreativeMaxPlayer = 0;
                                                stat.CreativeThumbUrl = string.Empty;
                                                stat.CreativePlatformId = string.Empty;
                                                stat.CreativeGameModeId = string.Empty;
                                                stat.CreativeLevelThemeId = string.Empty;
                                                stat.CreativeLastModifiedDate = DateTime.MinValue;
                                                stat.CreativePlayCount = 0;
                                                stat.CreativeLikes = 0;
                                                stat.CreativeDislikes = 0;
                                                stat.CreativeQualificationPercent = 0;
                                                stat.CreativeTimeLimitSeconds = 0;
                                            }
                                        }
                                    }
                                }

                                this.RoundDetails.Insert(stat);
                                this.AllStats.Add(stat);
                                
                                // Below is where reporting to fallalytics happen
                                // Must have enabled the setting to enable tracking
                                // Must not be a private lobby
                                // Must be a game that is played after FallGuysStats started
                                if ((this.CurrentSettings.EnableFallalyticsReporting || this.CurrentSettings.EnableFallalyticsWeeklyCrownLeague) && !stat.PrivateLobby && !stat.UseShareCode && stat.ShowEnd > this.startupTime) {
                                    if (this.CurrentSettings.EnableFallalyticsReporting) {
                                        Task.Run(() => FallalyticsReporter.Report(stat, this.CurrentSettings.FallalyticsAPIKey));
                                    }

                                    if (OnlineServiceType != OnlineServiceTypes.None) {
                                        string[] userInfo = null;
                                        if (OnlineServiceType == OnlineServiceTypes.Steam) {
                                            userInfo = this.FindSteamUserInfo();
                                        } else if (OnlineServiceType == OnlineServiceTypes.EpicGames) {
                                            userInfo = this.FindEpicGamesUserInfo();
                                        }

                                        if (userInfo != null && !string.IsNullOrEmpty(userInfo[0]) && !string.IsNullOrEmpty(userInfo[1])) {
                                            OnlineServiceId = userInfo[0];
                                            OnlineServiceNickname = userInfo[1];
                                        }
                                        
                                        if (!string.IsNullOrEmpty(OnlineServiceId) && !string.IsNullOrEmpty(OnlineServiceNickname)) {
                                            if (string.IsNullOrEmpty(HostCountryCode)) {
                                                HostCountryCode = Utils.GetCountryCode(Utils.GetUserPublicIp());
                                            }
                                            
                                            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FALLALYTICS_KEY"))) {
                                                if (this.CurrentSettings.EnableFallalyticsReporting) {
                                                    if (stat.Finish.HasValue && (this.StatLookup.TryGetValue(stat.Name, out LevelStats level) && level.Type == LevelType.Race)) {
                                                        Task.Run(() => this.FallalyticsRegisterPb(stat));
                                                    }
                                                }
                                                
                                                if (this.CurrentSettings.EnableFallalyticsWeeklyCrownLeague) {
                                                    if (stat.Crown) {
                                                        Task.Run(() => this.FallalyticsWeeklyCrown(stat)).ContinueWith(prevTask => this.FallalyticsResendWeeklyCrown());
                                                    }
                                                    
                                                    bool existsTransferFailedLogs = this.FallalyticsCrownLogCache.Exists(l => l.IsTransferSuccess == false && l.OnlineServiceType == (int)OnlineServiceType && string.Equals(l.OnlineServiceId, OnlineServiceId));
                                                    if (existsTransferFailedLogs) {
                                                        Task.Run(this.FallalyticsResendWeeklyCrown);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            } else {
                                continue;
                            }
                        }

                        if (stat.PrivateLobby || stat.IsCasualShow) {
                            if (stat.Round == 1) {
                                this.CustomAndCasualShows++;
                            }
                            this.CustomAndCasualRounds++;
                        } else {
                            if (stat.Round == 1) {
                                this.Shows++;
                            }
                            this.Rounds++;
                        }
                        this.Duration += stat.End - stat.Start;

                        if (stat.PrivateLobby || stat.IsCasualShow) {
                            if (stat.Qualified) {
                                switch (stat.Tier) {
                                    case 0:
                                        this.CustomAndCasualPinkMedals++;
                                        break;
                                    case 1:
                                        this.CustomAndCasualGoldMedals++;
                                        break;
                                    case 2:
                                        this.CustomAndCasualSilverMedals++;
                                        break;
                                    case 3:
                                        this.CustomAndCasualBronzeMedals++;
                                        break;
                                }
                            } else {
                                this.CustomAndCasualEliminatedMedals++;
                            }
                        } else {
                            if (stat.Qualified) {
                                switch (stat.Tier) {
                                    case 0:
                                        this.PinkMedals++;
                                        break;
                                    case 1:
                                        this.GoldMedals++;
                                        break;
                                    case 2:
                                        this.SilverMedals++;
                                        break;
                                    case 3:
                                        this.BronzeMedals++;
                                        break;
                                }
                            } else {
                                this.EliminatedMedals++;
                            }
                        }

                        this.Kudos += stat.Kudos;

                        // add new type of round to the rounds lookup
                        if (!stat.UseShareCode && !this.StatLookup.ContainsKey(stat.Name)) {
                            string roundName = stat.Name;
                            roundName = roundName.StartsWith("round_") ? roundName.Substring(6).Replace('_', ' ')
                                                                       : roundName.Replace('_', ' ');

                            LevelStats newLevel = new LevelStats(stat.Name, string.Empty, this.textInfo.ToTitleCase(roundName), LevelType.Unknown, BestRecordType.Fastest, false, false, 10, Properties.Resources.round_unknown_icon, Properties.Resources.round_unknown_big_icon);
                            this.StatLookup.Add(stat.Name, newLevel);
                            this.StatDetails.Add(newLevel);
                            this.gridDetails.DataSource = null;
                            // this.gridDetails.DataSource = this.StatDetails;
                            this.gridDetails.DataSource = this.GetFilteredDataSource(this.CurrentSettings.GroupingCreativeRoundLevels);
                        }

                        stat.ToLocalTime();
                        
                        if (!stat.PrivateLobby && !stat.IsCasualShow) {
                            if (stat.IsFinal || stat.Crown) {
                                this.Finals++;
                                if (stat.Qualified) {
                                    this.Wins++;
                                }
                            }
                        }
                        
                        if (this.StatLookup.TryGetValue(stat.UseShareCode ? stat.ShowNameId : stat.Name, out LevelStats levelStats)) {
                            levelStats.Increase(stat, this.profileWithLinkedCustomShow == stat.Profile);
                            levelStats.Add(stat);

                            if (levelStats.IsCreative && !stat.UseShareCode) {
                                if (!this.StatDetails.Contains(levelStats)) {
                                    LevelStats l1 = this.StatDetails.Find(l => string.Equals(l.ShareCode, levelStats.ShareCode));
                                    l1.Increase(stat, this.profileWithLinkedCustomShow == stat.Profile);
                                    l1.Add(stat);
                                }
                            
                                if (this.StatLookup.TryGetValue(this.GetCreativeLevelTypeId(levelStats.Type, levelStats.IsFinal), out LevelStats creativeLevel)) {
                                    creativeLevel.Increase(stat, this.profileWithLinkedCustomShow == stat.Profile);
                                    creativeLevel.Add(stat);
                                }
                            }
                        }
                    }

                    if (!this.loadingExisting) { this.StatsDB.Commit(); }
                    this.OnUpdatedLevelRows?.Invoke();
                }

                lock (this.CurrentRound) {
                    this.CurrentRound.Clear();
                    for (int i = round.Count - 1; i >= 0; i--) {
                        RoundInfo info = round[i];
                        this.CurrentRound.Insert(0, info);
                        if (info.Round == 1) {
                            break;
                        }
                    }
                }

                if (!this.Disposing && !this.IsDisposed) {
                    try {
                        this.UpdateTotals();
                    } catch {
                        // ignored
                    }
                }

                IsOverlayRoundInfoNeedRefresh = true;
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // public bool ExistsPersonalBestLog(DateTime pbDate) {
        //     return this.PersonalBestLog.Exists(Query.EQ("_id", pbDate));
        // }
        
        public bool ExistsPersonalBestLog(DateTime pbDate) {
            return this.PersonalBestLogCache.Exists(l => l.PbDate == pbDate);
        }
        
        // public PersonalBestLog SelectPersonalBestLog(string sessionId, string showId, string roundId) {
        //     BsonExpression condition = Query.And(
        //         Query.EQ("SessionId", sessionId),
        //         Query.EQ("ShowId", showId),
        //         Query.EQ("RoundId", roundId)
        //     );
        //     return this.PersonalBestLog.FindOne(condition);
        // }
        
        // public void InsertPersonalBestLog(DateTime finish, string sessionId, string showId, string roundId, double record, bool isPb) {
        //     lock (this.StatsDB) {
        //         this.StatsDB.BeginTrans();
        //         this.PersonalBestLog.Insert(new PersonalBestLog { PbDate = finish, SessionId = sessionId, ShowId = showId, RoundId = roundId, Record = record, IsPb = isPb,
        //             CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname
        //         });
        //         this.StatsDB.Commit();
        //     }
        // }
        
        public void InsertPersonalBestLog(DateTime finish, string sessionId, string showId, string roundId, double record, bool isPb) {
            lock (this.StatsDB) {
                PersonalBestLog log = new PersonalBestLog { PbDate = finish, SessionId = sessionId, ShowId = showId, RoundId = roundId, Record = record, IsPb = isPb,
                    CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname
                };
                this.StatsDB.BeginTrans();
                this.PersonalBestLog.Insert(log);
                this.StatsDB.Commit();
                this.PersonalBestLogCache.Add(log);
            }
        }

        // private void UpsertPersonalBestLog(string sessionId, string showId, string roundId, double record, DateTime finish, bool isPb) {
        //     lock (this.StatsDB) {
        //         this.StatsDB.BeginTrans();
        //         this.PersonalBestLog.Upsert(new PersonalBestLog { SessionId = sessionId, ShowId = showId, RoundId = roundId, Record = record, PbDate = finish, IsPb = isPb,
        //             CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname
        //         });
        //         this.StatsDB.Commit();
        //     }
        // }

        private void ClearPersonalBestLog(int days) {
            lock (this.StatsDB) {
                DateTime daysCond = DateTime.Now.AddDays(days * -1);
                BsonExpression condition = Query.LT("_id", daysCond);
                this.StatsDB.BeginTrans();
                this.PersonalBestLog.DeleteMany(condition);
                this.StatsDB.Commit();
            }
        }
        
        private void ClearWeeklyCrownLog(int days) {
            lock (this.StatsDB) {
                DateTime daysCond = DateTime.Now.AddDays(days * -1);
                BsonExpression condition = Query.LT("End", daysCond);
                this.StatsDB.BeginTrans();
                this.FallalyticsCrownLog.DeleteMany(condition);
                this.StatsDB.Commit();
            }
        }

        public bool ExistsServerConnectionLog(string sessionId) {
            if (string.IsNullOrEmpty(sessionId)) return false;
            // BsonExpression condition = Query.And(
            //     Query.EQ("_id", sessionId),
            //     Query.EQ("ShowId", showId)
            // );
            // return this.ServerConnectionLog.Exists(condition);
            return this.ServerConnectionLogCache.Exists(l => string.Equals(l.SessionId, sessionId));
        }
        
        public ServerConnectionLog SelectServerConnectionLog(string sessionId) {
            // BsonExpression condition = Query.And(
            //     Query.EQ("_id", sessionId),
            //     Query.EQ("ShowId", showId)
            // );
            // return this.ServerConnectionLog.FindOne(condition);
            return this.ServerConnectionLogCache.Find(l => string.Equals(l.SessionId, sessionId));
        }
        
        public void InsertServerConnectionLog(string sessionId, string showId, string serverIp, DateTime connectionDate, bool isNotify, bool isPlaying) {
            lock (this.StatsDB) {
                ServerConnectionLog log = new ServerConnectionLog { SessionId = sessionId, ShowId = showId, ServerIp = serverIp, ConnectionDate = connectionDate,
                    CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
                    IsNotify = isNotify, IsPlaying = isPlaying
                };
                this.StatsDB.BeginTrans();
                this.ServerConnectionLog.Insert(log);
                this.StatsDB.Commit();
                this.ServerConnectionLogCache.Add(log);
            }
        }
        
        public void UpdateServerConnectionLog(string sessionId, bool isPlaying) {
            lock (this.StatsDB) {
                ServerConnectionLog log = this.SelectServerConnectionLog(sessionId);
                if (log != null) {
                    log.IsPlaying = isPlaying;
                    this.StatsDB.BeginTrans();
                    this.ServerConnectionLog.Update(log);
                    this.StatsDB.Commit();
                }
            }
        }

        // public void UpsertServerConnectionLog(string sessionId, string showNameId, string serverIp, DateTime connectionDate, bool isNotify, bool isPlaying) {
        //     lock (this.StatsDB) {
        //         this.StatsDB.BeginTrans();
        //         this.ServerConnectionLog.Upsert(new ServerConnectionLog { SessionId = sessionId, ShowId = showNameId, ServerIp = serverIp, ConnectionDate = connectionDate,
        //             CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
        //             IsNotify = isNotify, IsPlaying = isPlaying
        //         });
        //         this.StatsDB.Commit();
        //     }
        // }

        private void ClearServerConnectionLog(int days) {
            lock (this.StatsDB) {
                DateTime daysCond = DateTime.Now.AddDays(days * -1);
                BsonExpression condition = Query.LT("ConnectionDate", daysCond);
                this.StatsDB.BeginTrans();
                this.ServerConnectionLog.DeleteMany(condition);
                this.StatsDB.Commit();
            }
        }

        private async Task FallalyticsResendWeeklyCrown() {
            foreach (FallalyticsCrownLog log in this.FallalyticsCrownLogCache.FindAll(l => l.IsTransferSuccess == false)) {
                RoundInfo stat = new RoundInfo { SessionId = log.SessionId, ShowNameId = log.ShowId, Name = log.RoundId, End = log.End, OnlineServiceType = log.OnlineServiceType, OnlineServiceId = log.OnlineServiceId, OnlineServiceNickname = log.OnlineServiceNickname };
                log.IsTransferSuccess = await FallalyticsReporter.WeeklyCrown(stat, this.CurrentSettings.EnableFallalyticsAnonymous);
                lock (this.StatsDB) {
                    this.StatsDB.BeginTrans();
                    this.FallalyticsCrownLog.Update(log);
                    this.StatsDB.Commit();
                }
            }
        }

        private async Task FallalyticsWeeklyCrown(RoundInfo stat) {
            string currentSessionId = stat.SessionId;
            string currentShowNameId = stat.ShowNameId;
            string currentRoundId = stat.Name;
            DateTime currentEnd = stat.End;
            bool isTransferSuccess = await FallalyticsReporter.WeeklyCrown(stat, this.CurrentSettings.EnableFallalyticsAnonymous);
            
            lock (this.StatsDB) {
                FallalyticsCrownLog log = new FallalyticsCrownLog {
                    SessionId = currentSessionId, RoundId = currentRoundId, ShowId = currentShowNameId, End = currentEnd, CountryCode = HostCountryCode,
                    OnlineServiceType = stat.OnlineServiceType.Value, OnlineServiceId = stat.OnlineServiceId, OnlineServiceNickname = stat.OnlineServiceNickname,
                    IsTransferSuccess = isTransferSuccess
                };
                this.StatsDB.BeginTrans();
                this.FallalyticsCrownLog.Insert(log);
                this.StatsDB.Commit();
                this.FallalyticsCrownLogCache.Add(log);
            }
        }

        private async Task FallalyticsRegisterPb(RoundInfo stat) {
            string currentSessionId = stat.SessionId;
            string currentShowNameId = stat.ShowNameId;
            string currentRoundId = stat.Name;
            TimeSpan currentRecord = stat.Finish.Value - stat.Start;
            DateTime currentFinish = stat.Finish.Value;
            bool isTransferSuccess;

            bool existsPbLog = this.FallalyticsPbLogCache.Exists(l => string.Equals(l.RoundId, currentRoundId) && l.OnlineServiceType == stat.OnlineServiceType.Value && string.Equals(l.OnlineServiceId, stat.OnlineServiceId));
            if (!existsPbLog) {
                // RoundInfo recordInfo = this.AllStats.FindAll(r => r.PrivateLobby == false && r.Finish.HasValue && string.Equals(r.Name, currentRoundId) && !string.IsNullOrEmpty(r.ShowNameId) && !string.IsNullOrEmpty(r.SessionId)).OrderBy(r => r.Finish.Value - r.Start).FirstOrDefault();
                //
                // if (recordInfo != null && currentRecord > recordInfo.Finish.Value - recordInfo.Start) {
                //     currentSessionId = recordInfo.SessionId;
                //     currentShowNameId = recordInfo.ShowNameId;
                //     currentRoundId = recordInfo.Name;
                //     currentRecord = recordInfo.Finish.Value - recordInfo.Start;
                //     currentFinish = recordInfo.Finish.Value;
                // }

                isTransferSuccess = await FallalyticsReporter.RegisterPb(stat, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous);
                
                lock (this.StatsDB) {
                    FallalyticsPbLog log = new FallalyticsPbLog {
                        SessionId = currentSessionId, RoundId = currentRoundId, ShowId = currentShowNameId,
                        Record = currentRecord.TotalMilliseconds, PbDate = currentFinish,
                        CountryCode = HostCountryCode, OnlineServiceType = stat.OnlineServiceType.Value,
                        OnlineServiceId = stat.OnlineServiceId, OnlineServiceNickname = stat.OnlineServiceNickname,
                        IsTransferSuccess = isTransferSuccess
                    };
                    this.StatsDB.BeginTrans();
                    this.FallalyticsPbLog.Insert(log);
                    this.StatsDB.Commit();
                    this.FallalyticsPbLogCache.Add(log);
                }
            } else {
                int logIndex = this.FallalyticsPbLogCache.FindIndex(l => string.Equals(l.RoundId, currentRoundId) && l.OnlineServiceType == stat.OnlineServiceType.Value && string.Equals(l.OnlineServiceId, stat.OnlineServiceId));
                if (logIndex != -1) {
                    FallalyticsPbLog pbLog = this.FallalyticsPbLogCache[logIndex];
                    TimeSpan existingRecord = TimeSpan.FromMilliseconds(pbLog.Record);
                    
                    // RoundInfo missingInfo = this.AllStats.FindAll(r =>
                    //         r.PrivateLobby == false && r.Finish.HasValue && string.Equals(r.Name, currentRoundId) && !string.IsNullOrEmpty(r.ShowNameId) && !string.IsNullOrEmpty(r.SessionId) &&
                    //         string.Equals(r.OnlineServiceId, OnlineServiceId) && string.Equals(r.OnlineServiceNickname, OnlineServiceNickname))
                    //     .OrderBy(r => r.Finish.Value - r.Start).FirstOrDefault();
                    // if (missingInfo != null && (missingInfo.Finish.Value - missingInfo.Start) < currentRecord) {
                    //     currentSessionId = missingInfo.SessionId;
                    //     currentShowNameId = missingInfo.ShowNameId;
                    //     currentRoundId = missingInfo.Name;
                    //     currentRecord = missingInfo.Finish.Value - missingInfo.Start;
                    //     currentFinish = missingInfo.Finish.Value;
                    // }
                    
                    if (pbLog.IsTransferSuccess) {
                        if (currentRecord < existingRecord) {
                            isTransferSuccess = await FallalyticsReporter.RegisterPb(stat, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous);
                            
                            lock (this.StatsDB) {
                                pbLog.SessionId = currentSessionId;
                                pbLog.ShowId = currentShowNameId;
                                pbLog.Record = currentRecord.TotalMilliseconds;
                                pbLog.PbDate = currentFinish;
                                pbLog.IsTransferSuccess = isTransferSuccess;
                                this.StatsDB.BeginTrans();
                                this.FallalyticsPbLog.Update(pbLog);
                                this.StatsDB.Commit();
                            }
                        }
                    } else {
                        currentSessionId = currentRecord < existingRecord ? currentSessionId : pbLog.SessionId;
                        currentShowNameId = currentRecord < existingRecord ? currentShowNameId : pbLog.ShowId;
                        currentRecord = currentRecord < existingRecord ? currentRecord : existingRecord;
                        currentFinish = currentRecord < existingRecord ? currentFinish : pbLog.PbDate;
                        isTransferSuccess = await FallalyticsReporter.RegisterPb(new RoundInfo { SessionId = currentSessionId, ShowNameId = currentShowNameId, Name = currentRoundId, OnlineServiceType = pbLog.OnlineServiceType, OnlineServiceId = pbLog.OnlineServiceId, OnlineServiceNickname = pbLog.OnlineServiceNickname }, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous);
                        
                        lock (this.StatsDB) {
                            pbLog.SessionId = currentSessionId;
                            pbLog.ShowId = currentShowNameId;
                            pbLog.Record = currentRecord.TotalMilliseconds;
                            pbLog.PbDate = currentFinish;
                            pbLog.IsTransferSuccess = isTransferSuccess;
                            this.StatsDB.BeginTrans();
                            this.FallalyticsPbLog.Update(pbLog);
                            this.StatsDB.Commit();
                        }
                    }
                } else {
                    isTransferSuccess = await FallalyticsReporter.RegisterPb(stat, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous);
                    
                    lock (this.StatsDB) {
                        FallalyticsPbLog log = new FallalyticsPbLog {
                            SessionId = currentSessionId, RoundId = currentRoundId, ShowId = currentShowNameId,
                            Record = currentRecord.TotalMilliseconds, PbDate = currentFinish,
                            CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType,
                            OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
                            IsTransferSuccess = isTransferSuccess
                        };
                        this.StatsDB.BeginTrans();
                        this.FallalyticsPbLog.Insert(log);
                        this.StatsDB.Commit();
                        this.FallalyticsPbLogCache.Add(log);
                    }
                }   
            }
        }
        
        private bool IsInStatsFilter(RoundInfo info) {
            return (this.menuCustomRangeStats.Checked && info.Start >= this.customfilterRangeStart && info.Start <= this.customfilterRangeEnd)
                   || this.menuAllStats.Checked
                   || (this.menuSeasonStats.Checked && info.Start > SeasonStart)
                   || (this.menuWeekStats.Checked && info.Start > WeekStart)
                   || (this.menuDayStats.Checked && info.Start > DayStart)
                   || (this.menuSessionStats.Checked && info.Start > SessionStart);
        }
        
        private bool IsInPartyFilter(RoundInfo info) {
            return this.menuAllPartyStats.Checked
                   || (this.menuSoloStats.Checked && !info.InParty)
                   || (this.menuPartyStats.Checked && info.InParty);
        }
        
        public string GetCurrentFilterName() {
            if (this.menuCustomRangeStats.Checked && this.selectedCustomTemplateSeason > -1) {
                return Seasons[this.selectedCustomTemplateSeason].Name;
            } else {
                return this.menuCustomRangeStats.Checked ? Multilingual.GetWord("main_custom_range") :
                       this.menuAllStats.Checked ? Multilingual.GetWord("main_all") :
                       this.menuSeasonStats.Checked ? Multilingual.GetWord("main_season") :
                       this.menuWeekStats.Checked ? Multilingual.GetWord("main_week") :
                       this.menuDayStats.Checked ? Multilingual.GetWord("main_day") : Multilingual.GetWord("main_session");
            }
        }
        
        public string GetCurrentProfileName() {
            if (this.AllProfiles.Count == 0) return String.Empty;
            return this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).ProfileName;
        }
        
        public int GetCurrentProfileId() {
            return this.currentProfile;
        }
        
        private int GetProfileIdByName(string profileName) {
            if (this.AllProfiles.Count == 0 || string.IsNullOrEmpty(profileName)) return 0;
            return this.AllProfiles.Find(p => string.Equals(p.ProfileName, profileName)).ProfileId;
        }
        
        private string GetCurrentProfileLinkedShowId() {
            if (this.AllProfiles.Count == 0) return String.Empty;
            string currentProfileLinkedShowId = this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).LinkedShowId;
            return currentProfileLinkedShowId ?? string.Empty;
        }
        
        public string GetAlternateShowId(string showId) {
            switch (showId) {
                case "event_day_at_the_races_ltm":
                    return "event_only_races_any_final_template";
                case "event_le_anchovy_private_lobbies":
                    return "event_le_anchovy_template";
                case "event_only_jump_club_custom_lobby":
                    return "event_only_jump_club_template";
                case "event_only_roll_out_custom_lobby":
                    return "event_only_roll_out";
                case "knockout_mode_pl":
                    return "knockout_mode";
                case "live_event_timeattack_shuffle_pl":
                    return "live_event_timeattack_shuffle";
                default:
                    return showId;
            }
        }
        
        public string GetMainGroupShowId(string showId) {
            switch (showId) {
                case "ranked_show_knockout":
                    return "ranked_solo_show";
                // case "anniversary_fp12_ltm":
                case "classic_solo_main_show":
                case "ftue_uk_show":
                case "knockout_mode":
                case "no_elimination_explore":
                case "turbo_2_show":
                case "turbo_show":
                    return "main_show";
                case "classic_duos_show":
                case "knockout_duos":
                case "teams_show_ltm":
                    return "squads_2player_template";
                case "sports_show":
                    return "squads_3player_template";
                case "classic_squads_show":
                case "event_day_at_races_squads_template":
                case "event_only_ss2_squads_template":
                case "knockout_squads":
                case "squadcelebration":
                    return "squads_4player";
                case "fp16_ski_fall_high_scorers":
                    return "event_only_skeefall_timetrial_s6_1";
                case "invisibeans_pistachio_template":
                case "invisibeans_template":
                    return "invisibeans_mode";
                case "live_event_timeattack_dizzyheights":
                case "live_event_timeattack_lilyleapers":
                case "live_event_timeattack_partyprom":
                case "live_event_timeattack_shuffle":
                case "live_event_timeattack_trackattack":
                case "live_event_timeattack_treetoptumble":
                case "live_event_timeattack_tundrarun":
                    return "timeattack_mode";
                case "xtreme_explore":
                    return "event_xtreme_fall_guys_template";
                default:
                    return showId;
            }
        }
        
        public string ReplaceLevelIdInShuffleShow(string showId, string roundId) {
            if (string.Equals(showId, "no_elimination_show") && (roundId.StartsWith("wle_main_filler_") || roundId.StartsWith("wle_main_opener_"))) {
                roundId = roundId.Replace("_noelim", "");
            }
            return roundId;
        }
        
        private bool IsCreativeShow(string showId) {
            return string.Equals(showId, "casual_show")
                   || string.Equals(showId, "spotlight_mode")
                   || string.Equals(showId, "explore_points")
                   || string.Equals(showId, "showcase_fp13")
                   || string.Equals(showId, "showcase_fp16")
                   || string.Equals(showId, "showcase_fp17")
                   || string.Equals(showId, "showcase_fp18")
                   || string.Equals(showId, "showcase_fp19")
                   || string.Equals(showId, "showcase_fp20")
                   || string.Equals(showId, "greatestsquads_ltm")
                   || showId.StartsWith("user_creative_")
                   || showId.StartsWith("creative_")
                   || showId.StartsWith("event_wle_")
                   || showId.StartsWith("show_wle")
                   || showId.StartsWith("wle_")
                   || showId.StartsWith("current_wle_")
                   || (showId.StartsWith("event_") && showId.EndsWith("_fools"));
        }
        
        private int GetLinkedProfileId(string realShowId, bool isPrivateLobbies) {
            if (this.AllProfiles.Count == 0 || string.IsNullOrEmpty(realShowId)) return 0;
            realShowId = this.GetAlternateShowId(realShowId);
            string showId = this.GetMainGroupShowId(realShowId);
            foreach (Profiles profiles in this.AllProfiles.OrderBy(p => p.DoNotCombineShows ? 0 : 1)) {
                if (profiles.DoNotCombineShows) {
                    if (!isPrivateLobbies && !string.IsNullOrEmpty(profiles.LinkedShowId) && realShowId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                        return profiles.ProfileId;
                    }
                } else {
                    if (isPrivateLobbies) {
                        if (!string.IsNullOrEmpty(profiles.LinkedShowId) && string.Equals(profiles.LinkedShowId, "private_lobbies")) {
                            return profiles.ProfileId;
                        }
                    } else {
                        if (this.IsCreativeShow(showId)) {
                            if (!string.IsNullOrEmpty(profiles.LinkedShowId) && string.Equals(profiles.LinkedShowId, "fall_guys_creative_mode")) {
                                return profiles.ProfileId;
                            }
                        } else {
                            if (!string.IsNullOrEmpty(profiles.LinkedShowId) && showId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                                return profiles.ProfileId;
                            }
                        }
                    }
                }
            }
            if (isPrivateLobbies) {
                // return corresponding linked profile when possible if no linked "private_lobbies" profile was found
                return (from profiles in this.AllProfiles.OrderBy(p => p.DoNotCombineShows ? 0 : 1) where !string.IsNullOrEmpty(profiles.LinkedShowId) && showId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1 select profiles.ProfileId).FirstOrDefault();
            }
            // return ProfileId 0 if no linked profile was found/matched
            return 0;
        }
        
        public void SetLinkedProfileMenu(string realShowId, bool isPrivateLobbies) {
            if (this.AllProfiles.Count == 0 || string.IsNullOrEmpty(realShowId)) return;

            realShowId = this.GetAlternateShowId(realShowId);

            string currentProfileLinkedShowId = this.GetCurrentProfileLinkedShowId();
            bool isCurrentProfileIsDNCS = this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).DoNotCombineShows;
            if (isCurrentProfileIsDNCS && string.Equals(currentProfileLinkedShowId, realShowId)) return;

            string showId = this.GetMainGroupShowId(realShowId);
            int linkedDNCSProfileId = this.AllProfiles.Find(p => p.DoNotCombineShows && string.Equals(p.LinkedShowId, realShowId))?.ProfileId ?? -1;
            if (linkedDNCSProfileId == -1 && string.Equals(currentProfileLinkedShowId, showId)) return;

            this.BeginInvoke((MethodInvoker)delegate {
                int profileId = -1;
                bool isLinkedProfileFound = false;
                foreach (Profiles profiles in this.AllProfiles.FindAll(p => p.DoNotCombineShows)) {
                    if (!isPrivateLobbies && !string.IsNullOrEmpty(profiles.LinkedShowId) && realShowId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                        profileId = profiles.ProfileId;
                        isLinkedProfileFound = true;
                        break;
                    }
                }
                for (int i = 0; i < this.AllProfiles.Count; i++) {
                    if (isLinkedProfileFound) {
                        if (this.AllProfiles[i].ProfileId == profileId) {
                            ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                            if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                            return;
                        } else {
                            continue;
                        }
                    }
                    if (this.AllProfiles[i].DoNotCombineShows) continue;
                    if (isPrivateLobbies) {
                        if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && string.Equals(this.AllProfiles[i].LinkedShowId, "private_lobbies")) {
                            ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                            if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                            return;
                        }
                    } else {
                        if (this.IsCreativeShow(showId)) {
                            if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && string.Equals(this.AllProfiles[i].LinkedShowId, "fall_guys_creative_mode")) {
                                ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                                if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                                return;
                            }
                        } else {
                            if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && showId.IndexOf(this.AllProfiles[i].LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                                ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                                if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                                return;
                            }
                        }
                    }
                }
                if (isPrivateLobbies) { // select corresponding linked profile when possible if no linked "private_lobbies" profile was found
                    foreach (Profiles profiles in this.AllProfiles.FindAll(p => p.DoNotCombineShows)) {
                        if (!string.IsNullOrEmpty(profiles.LinkedShowId) && realShowId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                            profileId = profiles.ProfileId;
                            isLinkedProfileFound = true;
                            break;
                        }
                    }
                    for (int j = 0; j < this.AllProfiles.Count; j++) {
                        if (isLinkedProfileFound) {
                            if (this.AllProfiles[j].ProfileId == profileId) {
                                ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - j];
                                if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                                return;
                            } else {
                                continue;
                            }
                        } else {
                            if (this.AllProfiles[j].DoNotCombineShows || string.IsNullOrEmpty(this.AllProfiles[j].LinkedShowId) || showId.IndexOf(this.AllProfiles[j].LinkedShowId, StringComparison.OrdinalIgnoreCase) == -1) continue;
                            
                            ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - j];
                            if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                            return;
                        }
                    }
                }
                // select ProfileId 0 if no linked profile was found/matched
                for (int k = 0; k < this.AllProfiles.Count; k++) {
                    if (this.AllProfiles[k].ProfileId != 0) continue;
                    
                    ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - k];
                    if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                    return;
                }
            });
        }
        
        private void SetProfileMenu(int profile) {
            if (profile == -1 || this.AllProfiles.Count == 0) return;

            this.Invoke((MethodInvoker)delegate {
                ToolStripMenuItem tsmi = this.menuProfile.DropDownItems[$"menuProfile{profile}"] as ToolStripMenuItem;
                if (tsmi.Checked) return;

                this.menuStats_Click(tsmi, EventArgs.Empty);
            });
        }
        
        private void SetCurrentProfileIcon(bool linked) {
            this.BeginInvoke((MethodInvoker)delegate {
                if (this.CurrentSettings.AutoChangeProfile) {
                    this.lblCurrentProfileIcon.Image = linked ? Properties.Resources.profile2_linked_icon : Properties.Resources.profile2_unlinked_icon;
                    this.overlay.SetCurrentProfileForeColor(linked ? Color.GreenYellow
                                                            : string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White
                                                              : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
                } else {
                    this.lblCurrentProfileIcon.Image = Properties.Resources.profile2_icon;
                    this.overlay.SetCurrentProfileForeColor(string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White
                                                            : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
                }
            });
        }
        
        public RoundInfo GetRoundInfoFromShareCode(string shareCode) {
            return this.AllStats.FindLast(r => r.UseShareCode && string.Equals(r.CreativeShareCode, shareCode) && !string.IsNullOrEmpty(r.CreativeTitle));
        }
        
        public void UpdateCreativeLevels(string levelId, string shareCode, JsonElement levelData) {
            List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => string.Equals(r.Name, levelId) || string.Equals(r.Name, shareCode) || string.Equals(r.CreativeShareCode, shareCode));
            if (filteredInfo.Count <= 0) return;
            
            JsonElement versionMetadata = levelData.GetProperty("version_metadata");
            JsonElement stats = levelData.GetProperty("stats");
            string[] onlinePlatformInfo = this.FindUserCreativeAuthor(levelData.GetProperty("author").GetProperty("name_per_platform"));
            foreach (RoundInfo info in filteredInfo) {
                // info.CreativeShareCode = levelData.GetProperty("share_code").GetString();
                info.CreativeShareCode = shareCode;
                info.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                info.CreativeAuthor = onlinePlatformInfo[1];
                info.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                info.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                info.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                info.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                    string temps = string.Empty;
                    foreach (JsonElement t in creatorTags.EnumerateArray()) {
                        if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                        temps += t.GetString();
                    }
                    info.CreativeCreatorTags = temps;
                }
                info.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                info.CreativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                info.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                info.CreativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString() ?? "GAMEMODE_GAUNTLET";
                info.CreativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString() ?? "THEME_VANILLA";
                info.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                info.CreativePlayCount = stats.GetProperty("play_count").GetInt32();
                info.CreativeLikes = stats.GetProperty("likes").GetInt32();
                info.CreativeDislikes = stats.GetProperty("dislikes").GetInt32();
                info.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                info.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
            }
            
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                this.RoundDetails.Update(filteredInfo);
                this.StatsDB.Commit();
            }
        }
        
        public StatSummary GetLevelInfo(string levelId, LevelType type, BestRecordType record, bool useShareCode) {
            StatSummary summary = new StatSummary {
                CurrentStreak = 0,
                CurrentFinalStreak = 0,
                BestStreak = 0,
                BestFinalStreak = 0,
                AllWins = 0,
                TotalWins = 0,
                TotalShows = 0,
                TotalFinals = 0,
                TotalPlays = 0,
                TotalQualify = 0,
                TotalGolds = 0,
                FastestFinish = null,
                FastestFinishOverall = null,
                LongestFinish = null,
                LongestFinishOverall = null,
                HighScore = null,
                LowScore = null
            };

            int lastShow = -1;
            if (!this.StatLookup.TryGetValue(useShareCode ? type.UserCreativeLevelTypeId() : levelId, out LevelStats currentLevel)) {
                currentLevel = new LevelStats(levelId, string.Empty, levelId, LevelType.Unknown, BestRecordType.Fastest, false, false, 10, Properties.Resources.round_unknown_icon, Properties.Resources.round_unknown_big_icon);
            }

            List<RoundInfo> roundInfo = useShareCode ? this.AllStats.FindAll(r => r.Profile == this.GetCurrentProfileId() && string.Equals(r.Name, levelId) && string.Equals(r.ShowNameId, type.UserCreativeLevelTypeId()))
                                                     : this.AllStats.FindAll(r => r.Profile == this.GetCurrentProfileId());

            for (int i = 0; i < roundInfo.Count; i++) {
                RoundInfo info = roundInfo[i];
                TimeSpan finishTime = info.Finish.GetValueOrDefault(info.Start) - info.Start;
                bool hasFinishTime = finishTime.TotalSeconds > 1.1;
                bool hasLevelDetails = this.StatLookup.ContainsKey(info.UseShareCode ? info.ShowNameId : info.Name);
                bool isCurrentLevel = false;
                if (useShareCode) {
                    isCurrentLevel = true;
                } else {
                    if (currentLevel.IsCreative && !string.IsNullOrEmpty(currentLevel.ShareCode)) {
                        if (this.StatLookup.TryGetValue(info.Name, out LevelStats l1) && string.Equals(l1.ShareCode, currentLevel.ShareCode)) {
                            isCurrentLevel = true;
                        }
                    } else if (string.Equals(info.Name, currentLevel.Id)) {
                        isCurrentLevel = true;
                    }
                }

                int startRoundShowId = info.ShowID;
                RoundInfo endRound = info;
                for (int j = i + 1; j < roundInfo.Count; j++) {
                    if (roundInfo[j].ShowID != startRoundShowId) {
                        break;
                    }
                    endRound = roundInfo[j];
                }

                bool isShareCodeUsedOrIsNotPrivateLobby = useShareCode || !endRound.PrivateLobby;

                bool isInWinsFilter = isShareCodeUsedOrIsNotPrivateLobby && !endRound.IsCasualShow
                                      && (this.CurrentSettings.WinsFilter == 0
                                          || (this.CurrentSettings.WinsFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info))
                                          || (this.CurrentSettings.WinsFilter == 2 && endRound.Start > SeasonStart)
                                          || (this.CurrentSettings.WinsFilter == 3 && endRound.Start > WeekStart)
                                          || (this.CurrentSettings.WinsFilter == 4 && endRound.Start > DayStart)
                                          || (this.CurrentSettings.WinsFilter == 5 && endRound.Start > SessionStart));
                bool isInQualifyFilter = isShareCodeUsedOrIsNotPrivateLobby
                                         && (this.CurrentSettings.QualifyFilter == 0
                                             || (this.CurrentSettings.QualifyFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info))
                                             || (this.CurrentSettings.QualifyFilter == 2 && endRound.Start > SeasonStart)
                                             || (this.CurrentSettings.QualifyFilter == 3 && endRound.Start > WeekStart)
                                             || (this.CurrentSettings.QualifyFilter == 4 && endRound.Start > DayStart)
                                             || (this.CurrentSettings.QualifyFilter == 5 && endRound.Start > SessionStart));
                bool isInFastestFilter = this.CurrentSettings.FastestFilter == 0
                                         || (this.CurrentSettings.FastestFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info))
                                         || (this.CurrentSettings.FastestFilter == 2 && endRound.Start > SeasonStart)
                                         || (this.CurrentSettings.FastestFilter == 3 && endRound.Start > WeekStart)
                                         || (this.CurrentSettings.FastestFilter == 4 && endRound.Start > DayStart)
                                         || (this.CurrentSettings.FastestFilter == 5 && endRound.Start > SessionStart);

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

                    if (hasFinishTime && (!summary.FastestFinishOverall.HasValue || summary.FastestFinishOverall.Value > finishTime)) {
                        summary.FastestFinishOverall = finishTime;
                    }

                    if (hasFinishTime && (!summary.LongestFinishOverall.HasValue || summary.LongestFinishOverall.Value < finishTime)) {
                        summary.LongestFinishOverall = finishTime;
                    }

                    if (isInFastestFilter) {
                        if (hasFinishTime && (!summary.FastestFinish.HasValue || summary.FastestFinish.Value > finishTime)) {
                            summary.FastestFinish = finishTime;
                        }

                        if (hasFinishTime && (!summary.LongestFinish.HasValue || summary.LongestFinish.Value < finishTime)) {
                            summary.LongestFinish = finishTime;
                        }

                        if ((!hasLevelDetails || record == BestRecordType.HighScore) && info.Score.HasValue && (!summary.HighScore.HasValue || info.Score.Value > summary.HighScore.Value)) {
                            summary.HighScore = info.Score;
                        }
                        
                        if ((!hasLevelDetails || record == BestRecordType.HighScore) && info.Score.HasValue && (!summary.LowScore.HasValue || info.Score.Value < summary.LowScore.Value)) {
                            summary.LowScore = info.Score;
                        }
                    }
                }

                bool isFinalRound = useShareCode ? (info.IsFinal || info.Crown) : ((info.IsFinal || info.Crown) && !endRound.PrivateLobby);

                if (ReferenceEquals(info, endRound) && isFinalRound) {
                    summary.CurrentFinalStreak++;
                    if (summary.BestFinalStreak < summary.CurrentFinalStreak) {
                        summary.BestFinalStreak = summary.CurrentFinalStreak;
                    }
                }

                isShareCodeUsedOrIsNotPrivateLobby = useShareCode || !info.PrivateLobby;

                if (info.Qualified) {
                    if (hasLevelDetails && (info.IsFinal || info.Crown)) {
                        if (isShareCodeUsedOrIsNotPrivateLobby && !info.IsCasualShow) {
                            summary.AllWins++;
                        }

                        if (isInWinsFilter) {
                            summary.TotalWins++;
                            summary.TotalFinals++;
                        }

                        if (isShareCodeUsedOrIsNotPrivateLobby && !info.IsCasualShow) {
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
                    }
                } else if (isShareCodeUsedOrIsNotPrivateLobby && !info.IsCasualShow) {
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
            this.Wins = 0;
            this.Shows = 0;
            this.Rounds = 0;
            this.CustomAndCasualRounds = 0;
            this.Duration = TimeSpan.Zero;
            this.CustomAndCasualShows = 0;
            this.Finals = 0;
            this.GoldMedals = 0;
            this.SilverMedals = 0;
            this.BronzeMedals = 0;
            this.PinkMedals = 0;
            this.EliminatedMedals = 0;
            this.CustomAndCasualGoldMedals = 0;
            this.CustomAndCasualSilverMedals = 0;
            this.CustomAndCasualBronzeMedals = 0;
            this.CustomAndCasualPinkMedals = 0;
            this.CustomAndCasualEliminatedMedals = 0;
            this.Kudos = 0;
        }
        
        private void UpdateTotals() {
            try {
                this.lblCurrentProfile.Text = $"{this.GetCurrentProfileName().Replace("&", "&&")}";
                //this.lblCurrentProfile.ToolTipText = $"{Multilingual.GetWord("profile_change_tooltiptext")}";
                this.lblTotalShows.Text = $"{this.Shows:N0}{Multilingual.GetWord("main_inning")}";
                if (this.CustomAndCasualShows > 0) this.lblTotalShows.Text += $" ({Multilingual.GetWord("main_custom_and_casual_shows")} : {this.CustomAndCasualShows:N0}{Multilingual.GetWord("main_inning")})";
                //this.lblTotalShows.ToolTipText = $"{Multilingual.GetWord("shows_detail_tooltiptext")}";
                this.lblTotalRounds.Text = $"{this.Rounds:N0}{Multilingual.GetWord("main_round")}";
                if (this.CustomAndCasualRounds > 0) this.lblTotalRounds.Text += $" ({Multilingual.GetWord("main_custom_and_casual_shows")} : {this.CustomAndCasualRounds:N0}{Multilingual.GetWord("main_round")})";
                //this.lblTotalRounds.ToolTipText = $"{Multilingual.GetWord("rounds_detail_tooltiptext")}";
                this.lblTotalTime.Text = $"{(int)this.Duration.TotalHours}{Multilingual.GetWord("main_hour")}{this.Duration:mm}{Multilingual.GetWord("main_min")}{this.Duration:ss}{Multilingual.GetWord("main_sec")}";
                //this.lblTotalTime.ToolTipText = $"{Multilingual.GetWord("stats_detail_tooltiptext")}";
                float winChance = (float)this.Wins * 100 / Math.Max(1, this.Shows);
                this.lblTotalWins.Text = $"{this.Wins:N0}{Multilingual.GetWord("main_win")} ({Math.Truncate(winChance * 10) / 10} %)";
                //this.lblTotalWins.ToolTipText = $"{Multilingual.GetWord("wins_detail_tooltiptext")}";
                float finalChance = (float)this.Finals * 100 / Math.Max(1, this.Shows);
                this.lblTotalFinals.Text = $"{this.Finals:N0}{Multilingual.GetWord("main_inning")} ({Math.Truncate(finalChance * 10) / 10} %)";
                //this.lblTotalFinals.ToolTipText = $"{Multilingual.GetWord("finals_detail_tooltiptext")}";
                this.lblGoldMedal.Text = $"{this.GoldMedals:N0}";
                if (this.CustomAndCasualGoldMedals > 0) this.lblGoldMedal.Text += $" ({this.CustomAndCasualGoldMedals:N0})";
                this.lblSilverMedal.Text = $"{this.SilverMedals:N0}";
                if (this.CustomAndCasualSilverMedals > 0) this.lblSilverMedal.Text += $" ({this.CustomAndCasualSilverMedals:N0})";
                this.lblBronzeMedal.Text = $"{this.BronzeMedals:N0}";
                if (this.CustomAndCasualBronzeMedals > 0) this.lblBronzeMedal.Text += $" ({this.CustomAndCasualBronzeMedals:N0})";
                this.lblPinkMedal.Text = $"{this.PinkMedals:N0}";
                if (this.CustomAndCasualPinkMedals > 0) this.lblPinkMedal.Text += $" ({this.CustomAndCasualPinkMedals:N0})";
                this.lblEliminatedMedal.Text = $"{this.EliminatedMedals:N0}";
                if (this.CustomAndCasualEliminatedMedals > 0) this.lblEliminatedMedal.Text += $" ({this.CustomAndCasualEliminatedMedals:N0})";
                this.lblGoldMedal.Visible = this.GoldMedals != 0 || this.CustomAndCasualGoldMedals != 0;
                this.lblSilverMedal.Visible = this.SilverMedals != 0 || this.CustomAndCasualSilverMedals != 0;
                this.lblBronzeMedal.Visible = this.BronzeMedals != 0 || this.CustomAndCasualBronzeMedals != 0;
                this.lblPinkMedal.Visible = this.PinkMedals != 0 || this.CustomAndCasualPinkMedals != 0;
                this.lblEliminatedMedal.Visible = this.EliminatedMedals != 0 || this.CustomAndCasualEliminatedMedals != 0;
                this.lblKudos.Text = $"{this.Kudos:N0}";
                this.lblKudos.Visible = this.Kudos != 0;
                this.gridDetails.Invalidate();
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowToastNotification(IWin32Window window, Image thumbNail, string caption, string description, Font font, Image appOwnerIcon, ToastDuration duration, ToastPosition position, ToastAnimation animation, ToastTheme theme, ToastSound toastSound, bool muting, bool isAsync) {
            this.BeginInvoke((MethodInvoker)delegate {
                this.toast = Toast.Build(window, caption, description, font, thumbNail, appOwnerIcon,
                    duration, position, animation, ToastCloseStyle.ButtonAndClickEntire, theme, toastSound, muting);
                if (isAsync) {
                    this.toast.ShowAsync();
                } else {
                    this.toast.Show();
                }
            });
        }
        
        public void ShowNotification(string title, string text, ToolTipIcon toolTipIcon, int timeout) {
            if (this.trayIcon.Visible) {
                this.trayIcon.BalloonTipTitle = title;
                this.trayIcon.BalloonTipText = text;
                this.trayIcon.BalloonTipIcon = toolTipIcon;
                this.trayIcon.ShowBalloonTip(timeout);
            }
            // else {
            //     MetroMessageBox.Show(this, text, title, MessageBoxButtons.OK, toolTipIcon == ToolTipIcon.None ? MessageBoxIcon.None :
            //                                                                                  toolTipIcon == ToolTipIcon.Error ? MessageBoxIcon.Error :
            //                                                                                  toolTipIcon == ToolTipIcon.Info ? MessageBoxIcon.Information :
            //                                                                                  toolTipIcon == ToolTipIcon.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.None);
            // }
        }
        
        public void AllocOverlayTooltip() {
            this.omtt = new MetroToolTip {
                Theme = this.Theme
            };
        }
        
        public void ShowOverlayTooltip(string message, IWin32Window window, Point position, int duration = -1) {
            if (duration == -1) {
                this.omtt.Show(message, window, position);
            } else {
                this.omtt.Show(message, window, position, duration);
            }
        }
        
        public void HideOverlayTooltip(IWin32Window window) {
            this.omtt.Hide(window);
        }
        
        public void AllocCustomTooltip(DrawToolTipEventHandler drawFunc) {
            this.cmtt = new MetroToolTip {
                OwnerDraw = true
            };
            this.cmtt.Draw += drawFunc;
        }
        
        public void ShowCustomTooltip(string message, IWin32Window window, Point position, int duration = -1) {
            if (duration == -1) {
                this.cmtt.Show(message, window, position);
            } else {
                this.cmtt.Show(message, window, position, duration);
            }
        }
        
        public void HideCustomTooltip(IWin32Window window) {
            this.cmtt.Hide(window);
        }
        
        public void AllocTooltip() {
            this.mtt = new MetroToolTip {
                Theme = this.Theme
            };
        }
        
        public void ShowTooltip(string message, IWin32Window window, Point position, int duration = -1) {
            if (duration == -1) {
                this.mtt.Show(message, window, position);
            } else {
                this.mtt.Show(message, window, position, duration);
            }
        }
        
        public void HideTooltip(IWin32Window window) {
            this.mtt.Hide(window);
        }
        
        private void Toggle_MouseEnter(object sender, EventArgs e) {
            if (sender.Equals(this.mtgCreativeLevel) || sender.Equals(this.lblCreativeLevel)) {
                if (!this.mtgCreativeLevel.Checked) this.lblCreativeLevel.ForeColor = Color.DimGray;
            } else if (sender.Equals(this.mtgIgnoreLevelTypeWhenSorting) || sender.Equals(this.lblIgnoreLevelTypeWhenSorting)) {
                if (!this.mtgIgnoreLevelTypeWhenSorting.Checked) this.lblIgnoreLevelTypeWhenSorting.ForeColor = Color.DimGray;
            }
        }
        
        private void Toggle_MouseLeave(object sender, EventArgs e) {
            if (sender.Equals(this.mtgCreativeLevel) || sender.Equals(this.lblCreativeLevel)) {
                if (!this.mtgCreativeLevel.Checked) this.lblCreativeLevel.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            } else if (sender.Equals(this.mtgIgnoreLevelTypeWhenSorting) || sender.Equals(this.lblIgnoreLevelTypeWhenSorting)) {
                if (!this.mtgIgnoreLevelTypeWhenSorting.Checked) this.lblIgnoreLevelTypeWhenSorting.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
        }
        
        private void lblCreativeLevel_Click(object sender, EventArgs e) {
            this.mtgCreativeLevel.Checked = !this.mtgCreativeLevel.Checked;
        }
        
        private void mtgCreativeLevel_CheckedChanged(object sender, EventArgs e) {
            bool mtgChecked = ((MetroToggle)sender).Checked; 
            // this.VisibleGridRowOfCreativeLevel(mtgChecked);
            this.lblCreativeLevel.ForeColor = mtgChecked ? (this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : Color.DimGray;
            this.CurrentSettings.GroupingCreativeRoundLevels = mtgChecked;
            this.SaveUserSettings();
            this.gridDetails.DataSource = null;
            this.gridDetails.DataSource = this.GetFilteredDataSource(this.CurrentSettings.GroupingCreativeRoundLevels);
        }
        
        private void lblIgnoreLevelTypeWhenSorting_Click(object sender, EventArgs e) {
            this.mtgIgnoreLevelTypeWhenSorting.Checked = !this.mtgIgnoreLevelTypeWhenSorting.Checked;
        }
        
        private void mtgIgnoreLevelTypeWhenSorting_CheckedChanged(object sender, EventArgs e) {
            bool mtgChecked = ((MetroToggle)sender).Checked; 
            this.lblIgnoreLevelTypeWhenSorting.ForeColor = mtgChecked ? (this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : Color.DimGray;
            this.CurrentSettings.IgnoreLevelTypeWhenSorting = mtgChecked;
            this.SortGridDetails(true);
            this.SaveUserSettings();
        }
        
        private void scrollTimer_Tick(object sender, EventArgs e) {
            this.scrollTimer.Stop();
            this.isScrollingStopped = true;
        }

        private void gridDetails_Scroll(object sender, ScrollEventArgs e) {
            this.isScrollingStopped = false;
            this.scrollTimer.Stop();
            this.scrollTimer.Start();
        }

        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            try {
                if (((Grid)sender).Columns.Count == 0) return;
                
                int pos = 0;
                ((Grid)sender).Columns["RoundBigIcon"].Visible = false;
                ((Grid)sender).Columns["AveKudos"].Visible = false;
                ((Grid)sender).Columns["AveDuration"].Visible = false;
                ((Grid)sender).Columns["Id"].Visible = false;
                ((Grid)sender).Columns["ShareCode"].Visible = false;
                ((Grid)sender).Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["RoundIcon"].Resizable = DataGridViewTriState.False;
                ((Grid)sender).Setup("Name",      pos++, this.GetDataGridViewColumnWidth("Name", Multilingual.GetWord("main_round_name")), Multilingual.GetWord("main_round_name"), DataGridViewContentAlignment.MiddleLeft);
                ((Grid)sender).Columns["Name"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                ((Grid)sender).Setup("Played",    pos++, this.GetDataGridViewColumnWidth("Played", Multilingual.GetWord("main_played")), Multilingual.GetWord("main_played"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Qualified", pos++, this.GetDataGridViewColumnWidth("Qualified", Multilingual.GetWord("main_qualified")), Multilingual.GetWord("main_qualified"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Gold",      pos++, this.GetDataGridViewColumnWidth("Gold", Multilingual.GetWord("main_gold")), Multilingual.GetWord("main_gold"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Silver",    pos++, this.GetDataGridViewColumnWidth("Silver", Multilingual.GetWord("main_silver")), Multilingual.GetWord("main_silver"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Bronze",    pos++, this.GetDataGridViewColumnWidth("Bronze", Multilingual.GetWord("main_bronze")), Multilingual.GetWord("main_bronze"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Kudos",     pos++, this.GetDataGridViewColumnWidth("Kudos", Multilingual.GetWord("main_kudos")), Multilingual.GetWord("main_kudos"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Fastest",   pos++, this.GetDataGridViewColumnWidth("Fastest", Multilingual.GetWord("main_fastest")), Multilingual.GetWord("main_fastest"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Longest",   pos++, this.GetDataGridViewColumnWidth("Longest", Multilingual.GetWord("main_longest")), Multilingual.GetWord("main_longest"), DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("AveFinish", pos,   this.GetDataGridViewColumnWidth("AveFinish", Multilingual.GetWord("main_ave_finish")), Multilingual.GetWord("main_ave_finish"), DataGridViewContentAlignment.MiddleRight);
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private int GetDataGridViewColumnWidth(string columnName, string columnText) {
            int sizeOfText;
            switch (columnName) {
                case "RoundIcon":
                    sizeOfText = 13;
                    break;
                case "Name":
                    return 0;
                case "Played":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Qualified":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.English || CurrentLanguage == Language.French ? 0 : 5;
                    break;
                case "Gold":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.French ? 12 : CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 5 : 0;
                    break;
                case "Silver":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 5 : 0;
                    break;
                case "Bronze":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 5 : 0;
                    break;
                case "Kudos":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Fastest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 20 : 0;
                    break;
                case "Longest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 20 : 0;
                    break;
                case "AveFinish":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == Language.SimplifiedChinese || CurrentLanguage == Language.TraditionalChinese ? 20 : 0;
                    break;
                default:
                    return 0;
            }
            
            return sizeOfText + 24;
        }
        
        private void InitMainDataGridView() {
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.dataGridViewCellStyle1.BackColor = Color.LightGray;
            //this.dataGridViewCellStyle1.ForeColor = Color.Black;
            //this.dataGridViewCellStyle1.SelectionBackColor = Color.Cyan;
            //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridDetails.ColumnHeadersHeight = 20;
                
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(14);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.dataGridViewCellStyle2.BackColor = Color.White;
            //this.dataGridViewCellStyle2.ForeColor = Color.Black;
            //this.dataGridViewCellStyle2.SelectionBackColor = Color.DeepSkyBlue;
            //this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            this.gridDetails.RowTemplate.Height = 25;

            // this.gridDetails.DataSource = this.StatDetails;
            this.gridDetails.DataSource = this.GetFilteredDataSource(this.CurrentSettings.GroupingCreativeRoundLevels);
        }
        
        private void SetMainDataGridViewOrder() {
            int pos = 0;
            this.gridDetails.Columns["RoundIcon"].DisplayIndex = pos++;
            this.gridDetails.Columns["Name"].DisplayIndex = pos++;
            this.gridDetails.Columns["Played"].DisplayIndex = pos++;
            this.gridDetails.Columns["Qualified"].DisplayIndex = pos++;
            this.gridDetails.Columns["Gold"].DisplayIndex = pos++;
            this.gridDetails.Columns["Silver"].DisplayIndex = pos++;
            this.gridDetails.Columns["Bronze"].DisplayIndex = pos++;
            this.gridDetails.Columns["Kudos"].DisplayIndex = pos++;
            this.gridDetails.Columns["Fastest"].DisplayIndex = pos++;
            this.gridDetails.Columns["Longest"].DisplayIndex = pos++;
            this.gridDetails.Columns["AveFinish"].DisplayIndex = pos;
        }
        
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                if (e.RowIndex < 0) return;
                if (!((Grid)sender).Rows[e.RowIndex].Visible) return;
                
                LevelStats levelStats = ((Grid)sender).Rows[e.RowIndex].DataBoundItem as LevelStats;
                float fBrightness = 0.85f;
                Color cellColor;
                switch (((Grid)sender).Columns[e.ColumnIndex].Name) {
                    case "RoundIcon":
                        if (levelStats.IsFinal) {
                            cellColor = Color.FromArgb(255, 240, 200);
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            break;
                        }
                        switch (levelStats.Type) {
                            // case LevelType.CreativeRace:
                            //     cellColor = Color.FromArgb(122, 201, 241);
                            //     e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            //     break;
                            case LevelType.CreativeRace:
                            case LevelType.Race:
                                cellColor = Color.FromArgb(210, 255, 220);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeSurvival:
                            case LevelType.Survival:
                                cellColor = Color.FromArgb(250, 205, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeHunt:
                            case LevelType.Hunt:
                                cellColor = Color.FromArgb(200, 220, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeLogic:
                            case LevelType.Logic:
                                cellColor = Color.FromArgb(230, 250, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeTeam:
                            case LevelType.Team:
                                cellColor = Color.FromArgb(255, 220, 205);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Invisibeans:
                                cellColor = Color.FromArgb(255, 255, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Unknown:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.DarkGray;
                                break;
                        }
                        break;
                    case "Name":
                        e.CellStyle.ForeColor = Color.Black;
                        if (levelStats.IsCreative) e.Value = $"🔧 {e.Value}";
                        if (levelStats.IsFinal) {
                            cellColor = Color.FromArgb(255, 240, 200);
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            break;
                        }
                        switch (levelStats.Type) {
                            // case LevelType.CreativeRace:
                            //     cellColor = Color.FromArgb(122, 201, 241);
                            //     e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            //     break;
                            case LevelType.CreativeRace:
                            case LevelType.Race:
                                cellColor = Color.FromArgb(210, 255, 220);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeSurvival:
                            case LevelType.Survival:
                                cellColor = Color.FromArgb(250, 205, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeHunt:
                            case LevelType.Hunt:
                                cellColor = Color.FromArgb(200, 220, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeLogic:
                            case LevelType.Logic:
                                cellColor = Color.FromArgb(230, 250, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.CreativeTeam:
                            case LevelType.Team:
                                cellColor = Color.FromArgb(255, 220, 205);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Invisibeans:
                                cellColor = Color.FromArgb(255, 255, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Unknown:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.DarkGray;
                                break;
                        }
                        break;
                    case "Played":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 126, 222);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Played == 0 ? "-" : $"{e.Value:N0}";
                        break;
                    case "Qualified":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(255, 20, 147);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        if (levelStats.Qualified == 0) {
                            e.Value = "-";
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                        } else {
                            float qualifyChance = levelStats.Qualified * 100f / Math.Max(1, levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{Math.Truncate(qualifyChance * 10) / 10}%";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Qualified:N0}";
                            } else {
                                e.Value = $"{levelStats.Qualified:N0}";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(qualifyChance * 10) / 10}%";
                            }
                        }
                        break;
                    case "Gold":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(255, 215, 0);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        if (levelStats.Gold == 0) {
                            e.Value = "-";
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                        } else {
                            float goldChance = levelStats.Gold * 100f / Math.Max(1, levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{Math.Truncate(goldChance * 10) / 10}%";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Gold:N0}";
                            } else {
                                e.Value = $"{levelStats.Gold:N0}";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(goldChance * 10) / 10}%";
                            }
                        }
                        break;
                    case "Silver":
                        fBrightness -= 0.3f;
                        cellColor = Color.FromArgb(192, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        if (levelStats.Silver == 0) {
                            e.Value = "-";
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                        } else {
                            float silverChance = levelStats.Silver * 100f / Math.Max(1, levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{Math.Truncate(silverChance * 10) / 10}%";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Silver:N0}";
                            } else {
                                e.Value = $"{levelStats.Silver:N0}";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(silverChance * 10) / 10}%";
                            }
                        }
                        break;
                    case "Bronze":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(205, 127, 50);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        if (levelStats.Bronze == 0) {
                            e.Value = "-";
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                        } else {
                            float bronzeChance = levelStats.Bronze * 100f / Math.Max(1, levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{Math.Truncate(bronzeChance * 10) / 10}%";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Bronze:N0}";
                            } else {
                                e.Value = $"{levelStats.Bronze:N0}";
                                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(bronzeChance * 10) / 10}%";
                            }
                        }
                        break;
                    case "Kudos":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(218, 112, 214);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Kudos == 0 ? "-" : $"{e.Value:N0}";
                        break;
                    case "AveFinish":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.AveFinish == TimeSpan.Zero ? "-" : levelStats.AveFinish.ToString("m\\:ss\\.fff");
                        break;
                    case "Fastest":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Fastest == TimeSpan.Zero ? "-" : levelStats.Fastest.ToString("m\\:ss\\.fff");
                        break;
                    case "Longest":
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Longest == TimeSpan.Zero ? "-" : levelStats.Longest.ToString("m\\:ss\\.fff");
                        break;
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                ((Grid)sender).SuspendLayout();
                ((Grid)sender).Cursor = Cursors.Default;
                this.HideCustomTooltip(this);
                ((Grid)sender).ResumeLayout();
            }
        }
        
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (!this.isScrollingStopped) return;

            try {
                ((Grid)sender).SuspendLayout();
                if (e.RowIndex >= 0 && (((Grid)sender).Columns[e.ColumnIndex].Name == "Name" || ((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon")) {
                    ((Grid)sender).ShowCellToolTips = false;
                    ((Grid)sender).Cursor = Cursors.Hand;
                    Point cursorPosition = this.PointToClient(Cursor.Position);
                    Point position = new Point(cursorPosition.X + 16, cursorPosition.Y + 16);
                    this.AllocCustomTooltip(this.cmtt_center_Draw);
                    this.ShowCustomTooltip($"{Multilingual.GetWord("level_detail_tooltiptext_prefix")}{((Grid)sender).Rows[e.RowIndex].Cells["Name"].Value}{Multilingual.GetWord("level_detail_tooltiptext_suffix")}", this, position);
                } else if (e.RowIndex >= 0) {
                    ((Grid)sender).ShowCellToolTips = true;
                    ((Grid)sender).Cursor = e.RowIndex >= 0 && !(((Grid)sender).Columns[e.ColumnIndex].Name == "Name" || ((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon")
                                              ? this.Theme == MetroThemeStyle.Light
                                                ? new Cursor(Properties.Resources.transform_icon.GetHicon())
                                                : new Cursor(Properties.Resources.transform_gray_icon.GetHicon())
                                              : Cursors.Default;
                }
                ((Grid)sender).ResumeLayout();
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            this.mtgCreativeLevel.Checked = this.CurrentSettings.GroupingCreativeRoundLevels;
            this.mtgIgnoreLevelTypeWhenSorting.Checked = this.CurrentSettings.IgnoreLevelTypeWhenSorting;
        }

        public List<LevelStats> GetFilteredDataSource(bool isFilter) {
            return isFilter ? this.StatDetails.Where(l => l.IsCreative != true || ((l.Id.StartsWith("creative_") || l.Id.StartsWith("user_creative_")) && l.Id.EndsWith("_round"))).ToList()
                            : this.StatDetails.Where(l => !(l.Id.StartsWith("creative_") && l.Id.EndsWith("_round"))).ToList();
        }
        
        private void SortGridDetails(bool isInitialize, int columnIndex = 0) {
            if (this.StatDetails == null) return;

            string columnName = this.gridDetails.Columns[columnIndex].Name;
            SortOrder sortOrder = isInitialize ? SortOrder.None : this.gridDetails.GetSortOrder(columnName);

            this.StatDetails.Sort((one, two) => {
                LevelType oneType = one.IsFinal ? LevelType.Final : one.Type;
                LevelType twoType = two.IsFinal ? LevelType.Final : two.Type;

                int typeCompare = this.CurrentSettings.IgnoreLevelTypeWhenSorting && sortOrder != SortOrder.None ? 0 : ((int)oneType).CompareTo((int)twoType);

                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                int nameCompare = $"{(one.IsCreative ? "#" : "")}{one.Name}".CompareTo($"{(two.IsCreative ? "#" : "")}{two.Name}");
                bool percents = this.CurrentSettings.ShowPercentages;
                if (typeCompare == 0 && sortOrder != SortOrder.None) {
                    switch (columnName) {
                        case "Played": typeCompare = one.Played.CompareTo(two.Played); break;
                        case "Qualified": typeCompare = ((double)one.Qualified / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Qualified / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Gold": typeCompare = ((double)one.Gold / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Gold / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Silver": typeCompare = ((double)one.Silver / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Silver / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Bronze": typeCompare = ((double)one.Bronze / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Bronze / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Kudos": typeCompare = one.Kudos.CompareTo(two.Kudos); break;
                        case "Fastest": typeCompare = one.Fastest.CompareTo(two.Fastest); break;
                        case "Longest": typeCompare = one.Longest.CompareTo(two.Longest); break;
                        case "AveFinish": typeCompare = one.AveFinish.CompareTo(two.AveFinish); break;
                        case "AveKudos": typeCompare = one.AveKudos.CompareTo(two.AveKudos); break;
                        default: typeCompare = nameCompare; break;
                    }
                }

                if (typeCompare == 0) {
                    typeCompare = nameCompare;
                }

                return typeCompare;
            });

            this.gridDetails.DataSource = null;
            // this.gridDetails.DataSource = this.StatDetails;
            this.gridDetails.DataSource = this.GetFilteredDataSource(this.CurrentSettings.GroupingCreativeRoundLevels);
            this.gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            this.SortGridDetails(false, e.ColumnIndex);
        }
        
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (((Grid)sender).SelectedCells.Count > 0) {
                ((Grid)sender).ClearSelection();
            }
        }
        
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) return;
                if (((Grid)sender).Columns[e.ColumnIndex].Name == "Name" || ((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon") {
                    LevelStats levelStats = ((Grid)sender).Rows[e.RowIndex].DataBoundItem as LevelStats;
                    using (LevelDetails levelDetails = new LevelDetails {
                               StatsForm = this,
                               LevelId = levelStats.Id,
                               LevelName = levelStats.Name,
                               RoundIcon = levelStats.RoundBigIcon,
                               IsCreative = levelStats.IsCreative,
                               RoundDetails = levelStats.Stats
                           }) {
                        this.EnableInfoStrip(false);
                        this.EnableMainMenu(false);
                        this.OnUpdatedLevelRows += levelDetails.LevelDetails_OnUpdatedLevelRows;
                        levelDetails.ShowDialog(this);
                        this.OnUpdatedLevelRows -= levelDetails.LevelDetails_OnUpdatedLevelRows;
                        this.EnableInfoStrip(true);
                        this.EnableMainMenu(true);
                    }
                } else {
                    this.CurrentSettings.ShowPercentages = !this.CurrentSettings.ShowPercentages;
                    this.SaveUserSettings();
                    this.gridDetails.Invalidate();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }

        public List<RoundInfo> GetShowsForDisplay() {
            return this.AllStats
                    .Where(r => r.Profile == this.GetCurrentProfileId()
                                && this.IsInStatsFilter(r)
                                && this.IsInPartyFilter(r))
                    .GroupBy(r => r.ShowID)
                    .Select(g => new {
                        ShowID = g.Key,
                        SortedRounds = g.OrderBy(r => r.Round).ToList()
                    })
                    .Select(g => new RoundInfo {
                        ShowID = g.ShowID,
                        // Name = g.SortedRounds.LastOrDefault().IsFinal || g.SortedRounds.LastOrDefault().Crown ? "Final" : string.Empty,
                        Name = string.Join(";", g.SortedRounds.Select(r => !string.IsNullOrEmpty(r.ShowNameId) && r.ShowNameId.StartsWith("user_creative_") ? (string.IsNullOrEmpty(r.CreativeTitle) ? r.Name : r.CreativeTitle) : r.Name)),
                        ShowNameId = string.Join(";", g.SortedRounds.Select(r => r.ShowNameId)),
                        IsFinal = g.SortedRounds.LastOrDefault().IsFinal,
                        End = g.SortedRounds.Max(r => r.End),
                        Start = g.SortedRounds.Min(r => r.Start),
                        StartLocal = g.SortedRounds.Min(r => r.StartLocal),
                        Kudos = g.SortedRounds.Sum(r => r.Kudos),
                        Qualified = g.SortedRounds.LastOrDefault().Qualified,
                        Round = g.SortedRounds.Max(r => r.Round),
                        Tier = g.SortedRounds.LastOrDefault().Qualified ? 1 : 0,
                        PrivateLobby = g.SortedRounds.LastOrDefault().PrivateLobby,
                        UseShareCode = g.SortedRounds.LastOrDefault().UseShareCode,
                        IsCasualShow = g.SortedRounds.LastOrDefault().IsCasualShow,
                        CreativeOnlinePlatformId = g.SortedRounds.LastOrDefault().CreativeOnlinePlatformId,
                        CreativeAuthor = g.SortedRounds.LastOrDefault().CreativeAuthor,
                        CreativeShareCode = g.SortedRounds.LastOrDefault().CreativeShareCode,
                        CreativeVersion = g.SortedRounds.LastOrDefault().CreativeVersion,
                        CreativeStatus = g.SortedRounds.LastOrDefault().CreativeStatus,
                        CreativeTitle = g.SortedRounds.LastOrDefault().CreativeTitle,
                        CreativeDescription = g.SortedRounds.LastOrDefault().CreativeDescription,
                        CreativeCreatorTags = g.SortedRounds.LastOrDefault().CreativeCreatorTags,
                        CreativeMaxPlayer = g.SortedRounds.LastOrDefault().CreativeMaxPlayer,
                        CreativeThumbUrl = g.SortedRounds.LastOrDefault().CreativeThumbUrl,
                        CreativePlatformId = g.SortedRounds.LastOrDefault().CreativePlatformId,
                        CreativeGameModeId = g.SortedRounds.LastOrDefault().CreativeGameModeId,
                        CreativeLevelThemeId = g.SortedRounds.LastOrDefault().CreativeLevelThemeId,
                        CreativeLastModifiedDate = g.SortedRounds.LastOrDefault().CreativeLastModifiedDate,
                        CreativePlayCount = g.SortedRounds.LastOrDefault().CreativePlayCount,
                        CreativeLikes = g.SortedRounds.LastOrDefault().CreativeLikes,
                        CreativeDislikes = g.SortedRounds.LastOrDefault().CreativeDislikes,
                        CreativeQualificationPercent = g.SortedRounds.LastOrDefault().CreativeQualificationPercent,
                        CreativeTimeLimitSeconds = g.SortedRounds.LastOrDefault().CreativeTimeLimitSeconds
                    }).ToList();
        }
        
        private void DisplayShows() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.StatsForm = this;
                levelDetails.LevelName = "Shows";
                levelDetails.RoundDetails = this.GetShowsForDisplay();
                this.OnUpdatedLevelRows += levelDetails.LevelDetails_OnUpdatedLevelRows;
                levelDetails.ShowDialog(this);
                this.OnUpdatedLevelRows -= levelDetails.LevelDetails_OnUpdatedLevelRows;
            }
        }

        public List<RoundInfo> GetRoundsForDisplay() {
            return this.AllStats
                .Where(r => r.Profile == this.GetCurrentProfileId() &&
                            this.IsInStatsFilter(r) &&
                            this.IsInPartyFilter(r))
                .OrderBy(r => r.ShowID)
                .ThenBy(r => r.Round)
                .ToList();
        }

        private void DisplayRounds() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.StatsForm = this;
                levelDetails.LevelName = "Rounds";
                levelDetails.RoundDetails = this.GetRoundsForDisplay();
                this.OnUpdatedLevelRows += levelDetails.LevelDetails_OnUpdatedLevelRows;
                levelDetails.ShowDialog(this);
                this.OnUpdatedLevelRows -= levelDetails.LevelDetails_OnUpdatedLevelRows;
            }
        }

        public List<RoundInfo> GetFinalsForDisplay() {
            return this.AllStats
                .Where(r => r.Profile == this.GetCurrentProfileId() &&
                            this.IsInStatsFilter(r) &&
                            this.IsInPartyFilter(r))
                .GroupBy(r => r.ShowID)
                .Where(g => g.Any(r => (r.Round == g.Max(x => x.Round)) && (r.IsFinal || r.Crown)))
                .SelectMany(g => g)
                .ToList();
        }

        private void DisplayFinals() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.StatsForm = this;
                levelDetails.LevelName = "Finals";
                levelDetails.RoundDetails = this.GetFinalsForDisplay();
                this.OnUpdatedLevelRows += levelDetails.LevelDetails_OnUpdatedLevelRows;
                levelDetails.ShowDialog(this);
                this.OnUpdatedLevelRows -= levelDetails.LevelDetails_OnUpdatedLevelRows;
            }
        }
        
        private void DisplayWinsGraph() {
            using (WinStatsDisplay display = new WinStatsDisplay {
                       StatsForm = this,
                       Text = $@"     {Multilingual.GetWord("level_detail_wins_per_day")} - {this.GetCurrentProfileName().Replace("&", "&&")} ({this.GetCurrentFilterName()})",
                       BackImage = Properties.Resources.crown_icon,
                       BackMaxSize = 32,
                       BackImagePadding = new Padding(20, 20, 0, 0)
                   }) {
                List<RoundInfo> rounds = this.AllStats
                    .Where(r => r.Profile == this.GetCurrentProfileId() &&
                                this.IsInStatsFilter(r) &&
                                this.IsInPartyFilter(r))
                    .OrderBy(r => r.End).ToList();
                
                var dates = new ArrayList();
                var shows = new ArrayList();
                var finals = new ArrayList();
                var wins = new ArrayList();
                var winsInfo = new Dictionary<double, SortedList<string, int>>();
                if (rounds.Count > 0) {
                    DateTime start = rounds[0].StartLocal;
                    int currentShows = 0;
                    int currentFinals = 0;
                    int currentWins = 0;
                    bool isIncrementedShows = false;
                    bool isIncrementedFinals = false;
                    bool isIncrementedWins = false;
                    bool isOverDate = false;
                    foreach (RoundInfo info in rounds.Where(info => !info.PrivateLobby &&
                                                                    !info.IsCasualShow)) {
                        if (info.Round == 1) {
                            currentShows += isOverDate ? 2 : 1;
                            isIncrementedShows = true;
                        }

                        if (info.Crown || info.IsFinal) {
                            isOverDate = start.Date < info.StartLocal.Date;
                            currentFinals++;
                            isIncrementedFinals = true;
                            
                            if (info.Qualified) {
                                currentWins++;
                                isIncrementedWins = true;
                                
                                string levelName = info.UseShareCode ? (string.IsNullOrEmpty(info.CreativeTitle) ? info.Name : info.CreativeTitle) : this.StatLookup.TryGetValue(info.Name, out LevelStats l1) ? l1.Name : info.Name;
                                if (winsInfo.TryGetValue(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), out SortedList<string, int> wi)) {
                                    if (wi.ContainsKey($"{levelName};crown")) {
                                        wi[$"{levelName};crown"] += 1;
                                    } else {
                                        wi[$"{levelName};crown"] = 1;
                                    }
                                } else {
                                    winsInfo.Add(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), new SortedList<string, int> {{$"{levelName};crown", 1}});
                                }

                                if (isOverDate) {
                                    currentShows--;
                                    isIncrementedShows = false;
                                }
                            } else {
                                string levelName = info.UseShareCode ? (string.IsNullOrEmpty(info.CreativeTitle) ? info.Name : info.CreativeTitle) : this.StatLookup.TryGetValue(info.Name, out LevelStats l1) ? l1.Name : info.Name;
                                if (winsInfo.TryGetValue(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), out SortedList<string, int> wi)) {
                                    if (wi.ContainsKey($"{levelName};eliminated")) {
                                        wi[$"{levelName};eliminated"] += 1;
                                    } else {
                                        wi[$"{levelName};eliminated"] = 1;
                                    }
                                } else {
                                    winsInfo.Add(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), new SortedList<string, int> {{$"{levelName};eliminated", 1}});
                                }
                                
                                if (isOverDate) {
                                    currentShows--;
                                    isIncrementedShows = false;
                                }
                            }
                        }

                        if (info.StartLocal.Date > start.Date && (isIncrementedShows || isIncrementedFinals)) {
                            dates.Add(start.Date.ToOADate());
                            shows.Add(Convert.ToDouble(isIncrementedShows ? --currentShows : currentShows));
                            finals.Add(Convert.ToDouble(isIncrementedFinals ? --currentFinals : currentFinals));
                            wins.Add(Convert.ToDouble(isIncrementedWins ? --currentWins : currentWins));

                            int daysWithoutStats = (int)(info.StartLocal.Date - start.Date).TotalDays - 1;
                            while (daysWithoutStats > 0) {
                                daysWithoutStats--;
                                start = start.Date.AddDays(1);
                                dates.Add(start.ToOADate());
                                shows.Add(0d);
                                finals.Add(0d);
                                wins.Add(0d);
                            }

                            currentShows = isIncrementedShows ? 1 : 0;
                            currentFinals = isIncrementedFinals ? 1 : 0;
                            currentWins = isIncrementedWins ? 1 : 0;
                            start = info.StartLocal;
                        }

                        isIncrementedShows = false;
                        isIncrementedFinals = false;
                        isIncrementedWins = false;
                    }

                    if (isOverDate) currentShows += 1;

                    dates.Add(start.Date.ToOADate());
                    shows.Add(Convert.ToDouble(currentShows));
                    finals.Add(Convert.ToDouble(currentFinals));
                    wins.Add(Convert.ToDouble(currentWins));

                    display.manualSpacing = Math.Ceiling(dates.Count / 28d);
                } else {
                    dates.Add(DateTime.Now.Date.ToOADate());
                    shows.Add(0d);
                    finals.Add(0d);
                    wins.Add(0d);

                    display.manualSpacing = 1.0;
                }
                display.dates = (double[])dates.ToArray(typeof(double));
                display.shows = (double[])shows.ToArray(typeof(double));
                display.finals = (double[])finals.ToArray(typeof(double));
                display.wins = (double[])wins.ToArray(typeof(double));
                display.winsInfo = winsInfo;
                
                display.ShowDialog(this);
            }
        }
        
        private void DisplayLevelGraph() {
            using (LevelStatsDisplay levelStatsDisplay = new LevelStatsDisplay()) {
                levelStatsDisplay.StatsForm = this;
                levelStatsDisplay.Text = $@"     {Multilingual.GetWord("level_detail_stats_by_round")} - {this.GetCurrentProfileName().Replace("&", "&&")} ({this.GetCurrentFilterName()})";
                levelStatsDisplay.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                levelStatsDisplay.BackMaxSize = 32;
                levelStatsDisplay.BackImagePadding = new Padding(20, 20, 0, 0);
                List<RoundInfo> rounds = this.AllStats.Where(r => r.Profile == this.GetCurrentProfileId()
                                                                  && this.IsInStatsFilter(r)
                                                                  && this.IsInPartyFilter(r)
                                                                  && !r.UseShareCode)
                    .OrderBy(r => (this.StatLookup.TryGetValue(r.Name, out LevelStats l1) && l1.IsCreative && !string.IsNullOrEmpty(l1.ShareCode)) ? l1.ShareCode : r.Name)
                    .ThenBy(r => r.Name).ToList();
                List<RoundInfo> useShareCodeRounds = this.AllStats.Where(r => r.Profile == this.GetCurrentProfileId()
                                                                              && this.IsInStatsFilter(r)
                                                                              && this.IsInPartyFilter(r)
                                                                              && r.UseShareCode)
                    .OrderBy(r => r.Name).ToList();
                
                if (rounds.Count == 0 && useShareCodeRounds.Count == 0) return;
                
                var levelMedalInfo = new Dictionary<string, double[]>();
                var levelTotalPlayTime = new Dictionary<string, TimeSpan>();
                var levelTimeInfo = new Dictionary<string, string[]>();
                var levelScoreInfo = new Dictionary<string, string[]>();
                var levelList = new Dictionary<string, string>();
                
                double p = 0, gm = 0, sm = 0, bm = 0, pm = 0, em = 0;
                int hs = -1, ls = int.MaxValue;
                TimeSpan pt = TimeSpan.Zero, ft = TimeSpan.MaxValue, lt = TimeSpan.Zero;
                for (int i = 0; i < rounds.Count; i++) {
                    bool isCurrentRoundInfoAvailable = this.StatLookup.TryGetValue(rounds[i].Name, out LevelStats l1);
                    if (i > 0) {
                        bool isCurrentRoundIsCreative = !isCurrentRoundInfoAvailable || l1.IsCreative;
                        bool isPreviousRoundInfoAvailable = this.StatLookup.TryGetValue(rounds[i - 1].Name, out LevelStats l2);
                        bool isPreviousRoundIsCreative = !isPreviousRoundInfoAvailable || l2.IsCreative;
                        if ((isCurrentRoundIsCreative && isPreviousRoundIsCreative && isCurrentRoundInfoAvailable && isPreviousRoundInfoAvailable && !string.Equals(l1.ShareCode, l2.ShareCode))
                             || (!isCurrentRoundIsCreative && isPreviousRoundIsCreative)
                             || (!isCurrentRoundIsCreative && !isPreviousRoundIsCreative && !string.Equals(rounds[i].Name, rounds[i - 1].Name))
                             || (isCurrentRoundInfoAvailable && !isPreviousRoundInfoAvailable)
                             || (!isCurrentRoundInfoAvailable && isPreviousRoundInfoAvailable)
                             || (!isCurrentRoundInfoAvailable && !isPreviousRoundInfoAvailable && !string.Equals(rounds[i].Name, rounds[i - 1].Name))) {
                            string levelId = isPreviousRoundInfoAvailable && l2.IsCreative ? l2.ShareCode : rounds[i - 1].Name;
                            string levelName = isPreviousRoundInfoAvailable ? l2.Name : rounds[i - 1].Name;
                            levelTotalPlayTime.Add(levelId, pt);
                            levelMedalInfo.Add(levelId, new[] { p, gm, sm, bm, pm, em });
                            levelTimeInfo.Add(levelId, new[] { ft < TimeSpan.MaxValue ? $"{ft:m\\:ss\\.fff}" : @"-", lt > TimeSpan.Zero ? $"{lt:m\\:ss\\.fff}" : @"-" });
                            levelScoreInfo.Add(levelId, new[] { hs >= 0 ? $"{hs}" : @"-", ls < int.MaxValue ? $"{ls}" : @"-" });
                            levelList.Add(rounds[i - 1].Name, levelName.Replace("&", "&&"));
                            pt = TimeSpan.Zero; ft = TimeSpan.MaxValue; lt = TimeSpan.Zero;
                            hs = -1; ls = int.MaxValue;
                            p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                        }
                    }
                    TimeSpan rft = rounds[i].Finish.GetValueOrDefault(rounds[i].Start) - rounds[i].Start;
                    if (rounds[i].Finish.HasValue && rft.TotalSeconds > 1.1) {
                        ft = rft < ft ? rft : ft;
                        lt = rft > lt ? rft : lt;
                    }
                    if (rounds[i].Score.HasValue) {
                        hs = (int)(rounds[i].Score > hs ? rounds[i].Score : hs);
                        ls = (int)(rounds[i].Score < ls ? rounds[i].Score : ls);
                    }
                    
                    pt += rounds[i].End - rounds[i].Start;
                    ++p;
                    if (rounds[i].Qualified) {
                        switch (rounds[i].Tier) {
                            case 0: ++pm; break;
                            case 1: ++gm; break;
                            case 2: ++sm; break;
                            case 3: ++bm; break;
                        }
                    } else {
                        ++em;
                    }
                    
                    if (i == rounds.Count - 1) {
                        string levelId = isCurrentRoundInfoAvailable && l1.IsCreative ? l1.ShareCode : rounds[i].Name;
                        string levelName = isCurrentRoundInfoAvailable ? l1.Name : rounds[i].Name;
                        levelTotalPlayTime.Add(levelId, pt);
                        levelMedalInfo.Add(levelId, new[] { p, gm, sm, bm, pm, em });
                        levelTimeInfo.Add(levelId, new[] { ft < TimeSpan.MaxValue ? $"{ft:m\\:ss\\.fff}" : @"-", lt > TimeSpan.Zero ? $"{lt:m\\:ss\\.fff}" : @"-" });
                        levelScoreInfo.Add(levelId, new[] { hs >= 0 ? $"{hs}" : @"-", ls < int.MaxValue ? $"{ls}" : @"-" });
                        levelList.Add(rounds[i].Name, levelName.Replace("&", "&&"));
                    }
                }
                
                pt = TimeSpan.Zero; ft = TimeSpan.MaxValue; lt = TimeSpan.Zero;
                hs = -1; ls = int.MaxValue;
                p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                for (int i = 0; i < useShareCodeRounds.Count; i++) {
                    if (i > 0 && !string.Equals(useShareCodeRounds[i].Name, useShareCodeRounds[i - 1].Name)) {
                        string levelId = $"{useShareCodeRounds[i - 1].Name}.";
                        levelTotalPlayTime.Add(levelId, pt);
                        levelMedalInfo.Add(levelId, new[] { p, gm, sm, bm, pm, em });
                        levelTimeInfo.Add(levelId, new[] { ft < TimeSpan.MaxValue ? $"{ft:m\\:ss\\.fff}" : @"-", lt > TimeSpan.Zero ? $"{lt:m\\:ss\\.fff}" : @"-" });
                        levelScoreInfo.Add(levelId, new[] { hs >= 0 ? $"{hs}" : @"-", ls < int.MaxValue ? $"{ls}" : @"-" });
                        levelList.Add(levelId, $@"{this.GetUserCreativeLevelTitle(useShareCodeRounds[i - 1].Name).Replace("&", "&&")} ({Multilingual.GetWord("main_custom_and_casual_shows")})");
                        
                        pt = TimeSpan.Zero; ft = TimeSpan.MaxValue; lt = TimeSpan.Zero;
                        hs = -1; ls = int.MaxValue;
                        p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                    }
                    TimeSpan rft = useShareCodeRounds[i].Finish.GetValueOrDefault(useShareCodeRounds[i].Start) - useShareCodeRounds[i].Start;
                    if (useShareCodeRounds[i].Finish.HasValue && rft.TotalSeconds > 1.1) {
                        ft = rft < ft ? rft : ft;
                        lt = rft > lt ? rft : lt;
                    }
                    if (useShareCodeRounds[i].Score.HasValue) {
                        hs = (int)(useShareCodeRounds[i].Score > hs ? useShareCodeRounds[i].Score : hs);
                        ls = (int)(useShareCodeRounds[i].Score < ls ? useShareCodeRounds[i].Score : ls);
                    }
                    
                    pt += useShareCodeRounds[i].End - useShareCodeRounds[i].Start;
                    ++p;
                    if (useShareCodeRounds[i].Qualified) {
                        switch (useShareCodeRounds[i].Tier) {
                            case 0: ++pm; break;
                            case 1: ++gm; break;
                            case 2: ++sm; break;
                            case 3: ++bm; break;
                        }
                    } else {
                        ++em;
                    }
                    
                    if (i == useShareCodeRounds.Count - 1) {
                        string levelId = $"{useShareCodeRounds[i].Name}.";
                        levelTotalPlayTime.Add(levelId, pt);
                        levelMedalInfo.Add(levelId, new[] { p, gm, sm, bm, pm, em });
                        levelTimeInfo.Add(levelId, new[] { ft < TimeSpan.MaxValue ? $"{ft:m\\:ss\\.fff}" : @"-", lt > TimeSpan.Zero ? $"{lt:m\\:ss\\.fff}" : @"-" });
                        levelScoreInfo.Add(levelId, new[] { hs >= 0 ? $"{hs}" : @"-", ls < int.MaxValue ? $"{ls}" : @"-" });
                        levelList.Add(levelId, $@"{this.GetUserCreativeLevelTitle(useShareCodeRounds[i].Name).Replace("&", "&&")} ({Multilingual.GetWord("main_custom_and_casual_shows")})");
                    }
                }
                
                levelStatsDisplay.levelList = from pair in levelList orderby pair.Value.Trim() ascending select pair;
                levelStatsDisplay.levelTotalPlayTime = levelTotalPlayTime;
                levelStatsDisplay.levelTimeInfo = levelTimeInfo;
                levelStatsDisplay.levelScoreInfo = levelScoreInfo;
                levelStatsDisplay.levelMedalInfo = levelMedalInfo;
                
                levelStatsDisplay.ShowDialog(this);
            }
        }
        
        private void LaunchHelpInBrowser() {
            try {
                if (CurrentLanguage == Language.French) {
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/fr/README.md#table-des-mati%C3%A8res");
                } else if (CurrentLanguage == Language.Korean) {
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/ko/README.md#%EB%AA%A9%EC%B0%A8");
                } else if (CurrentLanguage == Language.Japanese) {
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/ja/README.md#%E7%9B%AE%E6%AC%A1");
                } else {
                    Process.Start("https://github.com/ShootMe/FallGuysStats#table-of-contents");
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LaunchGame(bool ignoreExisting) {
            try {
                //this.UpdateGameExeLocation();
                if (this.CurrentSettings.LaunchPlatform == 0) {
                    if (!string.IsNullOrEmpty(this.CurrentSettings.GameShortcutLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = "FallGuys_client_game";
                        if (processes.Select(t => t.ProcessName).Any(name => name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0)) {
                            if (!ignoreExisting) {
                                MetroMessageBox.Show(this, Multilingual.GetWord("message_fall_guys_already_running"),
                                    Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }

                        if (MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}",
                                $"[{Multilingual.GetWord("level_detail_online_platform_eos")}] {Multilingual.GetWord("message_execution_caption")}",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameShortcutLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MetroMessageBox.Show(this, Multilingual.GetWord("message_register_shortcut"),
                            Multilingual.GetWord("message_register_shortcut_caption"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    if (!string.IsNullOrEmpty(this.CurrentSettings.GameExeLocation) && File.Exists(this.CurrentSettings.GameExeLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = Path.GetFileNameWithoutExtension(this.CurrentSettings.GameExeLocation);
                        if (processes.Select(t => t.ProcessName).Any(name => name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0)) {
                            if (!ignoreExisting) {
                                MetroMessageBox.Show(this, Multilingual.GetWord("message_fall_guys_already_running"),
                                    Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }

                        if (MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}",
                                $"[{Multilingual.GetWord("level_detail_online_platform_steam")}] {Multilingual.GetWord("message_execution_caption")}",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameExeLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MetroMessageBox.Show(this, Multilingual.GetWord("message_register_exe"),
                            Multilingual.GetWord("message_register_exe_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public void UpdateGameExeLocation() {
            string fallGuysShortcutLocation = this.FindEpicGamesShortcutLocation();
            string fallGuysExeLocation = this.FindSteamExeLocation();

            if (string.IsNullOrEmpty(fallGuysShortcutLocation) && !string.IsNullOrEmpty(fallGuysExeLocation)) {
                this.menuLaunchFallGuys.Image = Properties.Resources.steam_main_icon;
                this.trayLaunchFallGuys.Image = Properties.Resources.steam_main_icon;
                this.CurrentSettings.LaunchPlatform = 1;
            } else if (!string.IsNullOrEmpty(fallGuysShortcutLocation) && !string.IsNullOrEmpty(fallGuysExeLocation)) {
                this.menuLaunchFallGuys.Image = this.CurrentSettings.LaunchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
                this.trayLaunchFallGuys.Image = this.CurrentSettings.LaunchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
            } else {
                this.menuLaunchFallGuys.Image = Properties.Resources.epic_main_icon;
                this.trayLaunchFallGuys.Image = Properties.Resources.epic_main_icon;
                this.CurrentSettings.LaunchPlatform = 0;
            }

            this.CurrentSettings.GameShortcutLocation = fallGuysShortcutLocation;
            this.CurrentSettings.GameExeLocation = fallGuysExeLocation;
            this.SaveUserSettings();
        }
        
        public string[] FindEpicGamesUserInfo() {
            string[] userInfo = { string.Empty, string.Empty };
            try {
                string logsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Logs");
                if (Directory.Exists(logsDir)) {
                    FileInfo[] logFiles = new DirectoryInfo(logsDir).GetFiles("EpicGamesLauncher*").OrderByDescending(p => p.LastWriteTime).ToArray();
                    if (logFiles.Length > 0) {
                        foreach (FileInfo file in logFiles) {
                            using (FileStream fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                                using (StreamReader sr = new StreamReader(fs)) {
                                    string line;
                                    List<string> lines = new List<string>();
                                    while ((line = sr.ReadLine()) != null) {
                                        if (line.IndexOf("FCommunityPortalLaunchAppTask: Launching app", StringComparison.OrdinalIgnoreCase) != -1
                                            && line.IndexOf("RunFallGuys.exe", StringComparison.OrdinalIgnoreCase) != -1) {
                                            lines.Add(line);
                                        }
                                    }

                                    if (lines.Count > 0) {
                                        line = lines.Last();
                                        int index = line.IndexOf("-epicuserid=", StringComparison.OrdinalIgnoreCase) + 12;
                                        int index2 = line.IndexOf("-epicusername=", StringComparison.OrdinalIgnoreCase) + 15;
                                        userInfo[0] = line.Substring(index, line.IndexOf(" -epiclocale=", StringComparison.OrdinalIgnoreCase) - index);
                                        userInfo[1] = line.Substring(index2, line.IndexOf("\" -epicuserid=", StringComparison.OrdinalIgnoreCase) - index2);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return userInfo;
        }
        
        public string[] FindSteamUserInfo() {
            string[] userInfo = { string.Empty, string.Empty };
            try {
                string steamPath;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
                } else {
                    object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                    if (regValue == null) { return userInfo; }

                    steamPath = (string)regValue;
                }

                string steamConfigPath = Path.Combine(steamPath, "config", "loginusers.vdf");
                if (File.Exists(steamConfigPath)) {
                    var kv = SteamKit2.KeyValue.LoadAsText(steamConfigPath);
                    if (kv != null && string.Equals(kv.Name, "users", StringComparison.OrdinalIgnoreCase)) {
                        bool isFind = false;
                        foreach (var kvc in kv.Children) {
                            foreach (var kvcc in kvc.Children) {
                                if (string.Equals(kvcc.Name, "mostrecent", StringComparison.OrdinalIgnoreCase) && string.Equals(kvcc.Value, "1")) {
                                    isFind = true;
                                } else if (string.Equals(kvcc.Name, "accountname", StringComparison.OrdinalIgnoreCase)) {
                                    userInfo[0] = kvcc.Value;
                                } else if (string.Equals(kvcc.Name, "personaname", StringComparison.OrdinalIgnoreCase)) {
                                    userInfo[1] = kvcc.Value;
                                }
                            }
                            if (isFind) break;
                        }

                        if (!isFind) {
                            userInfo[0] = string.Empty;
                            userInfo[1] = string.Empty;
                        }
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return userInfo;
        }
        
        public string FindEpicGamesShortcutLocation() {
            try {
                object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher", "AppDataPath", null);
                if (regValue == null) { return string.Empty; }
                
                string epicGamesPath = Path.Combine((string)regValue, "Manifests");
                
                if (Directory.Exists(epicGamesPath)) {
                    DirectoryInfo di = new DirectoryInfo(epicGamesPath);
                    foreach (FileInfo file in di.GetFiles()) {
                        if (!string.Equals(file.Extension, ".item")) continue;
                        JsonClass json = Json.Read(File.ReadAllText(file.FullName)) as JsonClass;
                        if (string.Equals(json["MainGameCatalogNamespace"].AsString(), "50118b7f954e450f8823df1614b24e80") || string.Equals(json["CatalogNamespace"].AsString(), "50118b7f954e450f8823df1614b24e80")) {
                            return "com.epicgames.launcher://apps/50118b7f954e450f8823df1614b24e80%3A38ec4849ea4f4de6aa7b6fb0f2d278e1%3A0a2d9f6403244d12969e11da6713137b?action=launch&silent=true";
                        }
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        
        public string FindSteamExeLocation() {
            try {
                string steamPath;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
                } else {
                    // get steam install folder
                    object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                    if (regValue == null) { return string.Empty; }

                    steamPath = (string)regValue;
                }

                string fallGuysSteamPath = Path.Combine(steamPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                
                if (File.Exists(fallGuysSteamPath)) { return fallGuysSteamPath; }
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
                                fallGuysSteamPath = Path.Combine(libraryPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                                
                                if (File.Exists(fallGuysSteamPath)) { return fallGuysSteamPath; }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        
        private void EnableMainMenu(bool enable) {
            this.menuSettings.Enabled = enable;
            this.menuFilters.Enabled = enable;
            this.menuProfile.Enabled = enable;
            this.mlLeaderboard.Enabled = enable;
            if (enable) {
                this.menuSettings.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                this.menuSettings.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
            }
            if (this.trayIcon.Visible) {
                this.traySettings.Enabled = enable;
                this.trayFilters.Enabled = enable;
                this.trayProfile.Enabled = enable;
                if (enable) {
                    this.traySettings.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    this.traySettings.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
                }
            }
        }
        
        private void EnableInfoStrip(bool enable) {
            this.infoStrip.Enabled = enable;
            this.infoStrip2.Enabled = enable;
            this.lblTotalTime.Enabled = enable;
            if (enable) this.lblTotalTime.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
            foreach (var tsi in this.infoStrip.Items) {
                if (tsi is ToolStripLabel tsl) {
                    tsl.Enabled = enable;
                    if (enable) {
                        this.Cursor = Cursors.Default;
                        tsl.ForeColor = tsl.Equals(this.lblCurrentProfile)
                            ? this.Theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192)
                            : this.Theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                    }
                }
            }
        }
        
        private void Stats_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    this.shiftKeyToggle = false;
                    break;
                case Keys.ControlKey:
                    this.ctrlKeyToggle = false;
                    break;
            }
        }
        
        private void Stats_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    this.shiftKeyToggle = true;
                    break;
                case Keys.ControlKey:
                    this.ctrlKeyToggle = true;
                    break;
            }
            
            switch (e.Control) {
                case true when e.KeyCode == Keys.M:
                    this.CurrentSettings.OverlayNotOnTop = !this.CurrentSettings.OverlayNotOnTop;
                    this.SetOverlayTopMost(!this.CurrentSettings.OverlayNotOnTop);
                    this.SaveUserSettings();
                    break;
                case true when e.KeyCode == Keys.T:
                    int colorOption = 0;
                    if (this.overlay.BackColor.ToArgb() == Color.FromArgb(224, 224, 224).ToArgb()) {
                        colorOption = 1;
                    } else if (this.overlay.BackColor.ToArgb() == Color.White.ToArgb()) {
                        colorOption = 2;
                    } else if (this.overlay.BackColor.ToArgb() == Color.Black.ToArgb()) {
                        colorOption = 3;
                    } else if (this.overlay.BackColor.ToArgb() == Color.Magenta.ToArgb()) {
                        colorOption = 4;
                    } else if (this.overlay.BackColor.ToArgb() == Color.Red.ToArgb()) {
                        colorOption = 5;
                    } else if (this.overlay.BackColor.ToArgb() == Color.Green.ToArgb()) {
                        colorOption = 6;
                    } else if (this.overlay.BackColor.ToArgb() == Color.Blue.ToArgb()) {
                        colorOption = 0;
                    }
                    this.overlay.SetBackgroundColor(colorOption);
                    this.CurrentSettings.OverlayColor = colorOption;
                    this.SaveUserSettings();
                    this.overlay.ResetBackgroundImage();
                    break;
                case true when e.KeyCode == Keys.F:
                    if (!this.overlay.IsFixed()) {
                        this.overlay.FlipDisplay(!this.overlay.flippedImage);
                        this.CurrentSettings.FlippedDisplay = this.overlay.flippedImage;
                        this.SaveUserSettings();
                        this.overlay.ResetBackgroundImage();
                    }
                    break;
                case true when e.KeyCode == Keys.R:
                    this.CurrentSettings.ColorByRoundType = !this.CurrentSettings.ColorByRoundType;
                    this.SaveUserSettings();
                    this.overlay.ResetBackgroundImage();
                    break;
                case true when e.Shift && e.KeyCode == Keys.Z:
                    this.SetAutoChangeProfile(!this.CurrentSettings.AutoChangeProfile);
                    break;
                case true when e.Shift && e.KeyCode == Keys.X:
                    this.overlay.ResetOverlaySize();
                    break;
                case true when e.Shift && e.KeyCode == Keys.C:
                    this.overlay.ResetOverlayLocation(true);
                    break;
                case true when e.Shift && e.KeyCode == Keys.Up:
                    this.SetOverlayBackgroundOpacity(this.CurrentSettings.OverlayBackgroundOpacity + 5);
                    break;
                case true when e.Shift && e.KeyCode == Keys.Down:
                    this.SetOverlayBackgroundOpacity(this.CurrentSettings.OverlayBackgroundOpacity - 5);
                    break;
                case true when e.KeyCode == Keys.C:
                    this.CurrentSettings.PlayerByConsoleType = !this.CurrentSettings.PlayerByConsoleType;
                    this.SaveUserSettings();
                    this.overlay.ResetBackgroundImage();
                    break;
                case false when e.KeyCode == Keys.ShiftKey:
                    this.shiftKeyToggle = true;
                    break;
                case false when e.KeyCode == Keys.ControlKey:
                    this.ctrlKeyToggle = true;
                    break;
            }
            e.SuppressKeyPress = true;
        }
        
        private void lblCurrentProfileIcon_Click(object sender, EventArgs e) {
            this.SetAutoChangeProfile(!this.CurrentSettings.AutoChangeProfile);
            this.HideCustomTooltip(this);
        }
        
        private void lblCurrentProfile_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                for (int i = 0; i < this.ProfileMenuItems.Count; i++) {
                    if (!(this.ProfileMenuItems[i] is ToolStripMenuItem menuItem)) continue;
                    if (this.shiftKeyToggle) {
                        if (menuItem.Checked && i - 1 >= 0) {
                            this.menuStats_Click(this.ProfileMenuItems[i - 1], EventArgs.Empty);
                            break;
                        }
                        if (menuItem.Checked && i - 1 < 0) {
                            this.menuStats_Click(this.ProfileMenuItems[this.ProfileMenuItems.Count - 1], EventArgs.Empty);
                            break;
                        }
                    } else {
                        if (menuItem.Checked && i + 1 < this.ProfileMenuItems.Count) {
                            this.menuStats_Click(this.ProfileMenuItems[i + 1], EventArgs.Empty);
                            break;
                        }
                        if (menuItem.Checked && i + 1 >= this.ProfileMenuItems.Count) {
                            this.menuStats_Click(this.ProfileMenuItems[0], EventArgs.Empty);
                            break;
                        }
                    }
                }
            } else if (e.Button == MouseButtons.Right) {
                for (int i = 0; i < this.ProfileMenuItems.Count; i++) {
                    if (!(this.ProfileMenuItems[i] is ToolStripMenuItem menuItem)) continue;
                    if (menuItem.Checked && i - 1 >= 0) {
                        this.menuStats_Click(this.ProfileMenuItems[i - 1], EventArgs.Empty);
                        break;
                    }
                    if (menuItem.Checked && i - 1 < 0) {
                        this.menuStats_Click(this.ProfileMenuItems[this.ProfileMenuItems.Count - 1], EventArgs.Empty);
                        break;
                    }
                }
            }
        }
        
        private void lblTotalTime_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                this.DisplayLevelGraph();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblTotalFinals_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                this.DisplayFinals();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblTotalShows_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                this.DisplayShows();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblTotalRounds_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                this.DisplayRounds();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblTotalWins_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                this.DisplayWinsGraph();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool InitializeWeeklyCrownList(string date) {
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    string weeklyCrownApiUrl = "https://data.fallalytics.com/api/crown-leaderboard";
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    string json = web.DownloadString($"{weeklyCrownApiUrl}?page=1{(!string.IsNullOrEmpty(date) ? $"&date={date}" : "")}");
                    WeeklyCrown weeklyCrown = System.Text.Json.JsonSerializer.Deserialize<WeeklyCrown>(json);
                    if (weeklyCrown.found) {
                        this.weeklyCrownPrevious = weeklyCrown.previous;
                        this.weeklyCrownNext = weeklyCrown.next;
                        this.weeklyCrownCurrentYear = (int)weeklyCrown.year;
                        this.weeklyCrownCurrentWeek = (int)weeklyCrown.week;
                        this.weeklyCrownPeriod = Utils.GetStartAndEndDates(this.weeklyCrownCurrentYear, this.weeklyCrownCurrentWeek);
                        int totalPlayers = weeklyCrown.total;
                        int totalPages = (int)Math.Ceiling(Math.Min(1000, totalPlayers) / 100f);
                        for (int i = 0; i < weeklyCrown.users.Count; i++) {
                            WeeklyCrown.Player temp = weeklyCrown.users[i];
                            temp.rank = i + 1;
                            weeklyCrown.users[i] = temp;
                        }
                        this.totalWeeklyCrownPlayers = totalPlayers;
                        this.weeklyCrownList = weeklyCrown.users;
                        if (totalPages > 1) {
                            var tasks = new List<Task>();
                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                            for (int i = 2; i <= totalPages; i++) {
                                int page = i;
                                tasks.Add(Task.Run(async () => {
                                    HttpResponseMessage response = await client.GetAsync($"{weeklyCrownApiUrl}?page={page}{(!string.IsNullOrEmpty(date) ? $"&date={date}" : "")}");
                                    if (response.IsSuccessStatusCode) {
                                        json = await response.Content.ReadAsStringAsync();
                                        weeklyCrown = System.Text.Json.JsonSerializer.Deserialize<WeeklyCrown>(json);
                                        for (int j = 0; j < weeklyCrown.users.Count; j++) {
                                            WeeklyCrown.Player temp = weeklyCrown.users[j];
                                            temp.rank = j + 1 + ((page - 1) * 100);
                                            weeklyCrown.users[j] = temp;
                                        }
                                        this.weeklyCrownList.AddRange(weeklyCrown.users);
                                    }
                                }));
                            }
                            Task.WhenAll(tasks).Wait();
                        }
                        this.weeklyCrownList.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                        this.weeklyCrownLoadTime = DateTime.UtcNow;
                        return true;
                    }
                    return false;
                } catch {
                    this.weeklyCrownList = null;
                    return false;
                }
            }
        }

        public bool InitializeOverallRankList() {
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    string overallRankApiUrl = "https://data.fallalytics.com/api/speedrun-total";
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    string json = web.DownloadString($"{overallRankApiUrl}?page=1");
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new LevelRankInfoConverter());
                    OverallRank overallRank = System.Text.Json.JsonSerializer.Deserialize<OverallRank>(json);
                    if (overallRank.found) {
                        int totalPlayers = overallRank.total;
                        int totalPages = (int)Math.Ceiling(Math.Min(1000, totalPlayers) / 100f);
                        for (int i = 0; i < overallRank.users.Count; i++) {
                            OverallRank.Player temp = overallRank.users[i];
                            temp.rank = i + 1;
                            overallRank.users[i] = temp;
                        }
                        this.totalOverallRankPlayers = totalPlayers;
                        this.leaderboardOverallRankList = overallRank.users;
                        if (totalPages > 1) {
                            var tasks = new List<Task>();
                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                            for (int i = 2; i <= totalPages; i++) {
                                int page = i;
                                tasks.Add(Task.Run(async () => {
                                    HttpResponseMessage response = await client.GetAsync($"{overallRankApiUrl}?page={page}");
                                    if (response.IsSuccessStatusCode) {
                                        json = await response.Content.ReadAsStringAsync();
                                        overallRank = System.Text.Json.JsonSerializer.Deserialize<OverallRank>(json, options);
                                        for (int j = 0; j < overallRank.users.Count; j++) {
                                            OverallRank.Player temp = overallRank.users[j];
                                            temp.rank = j + 1 + ((page - 1) * 100);
                                            overallRank.users[j] = temp;
                                        }
                                        this.leaderboardOverallRankList.AddRange(overallRank.users);
                                    }
                                }));
                            }
                            Task.WhenAll(tasks).Wait();
                        }
                        this.leaderboardOverallRankList.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                        this.overallRankLoadTime = DateTime.UtcNow;
                        return true;
                    }
                    return false;
                } catch {
                    this.leaderboardOverallRankList = null;
                    return false;
                }
            }
        }
        
        private void mlReportCheater_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/ShootMe/FallGuysStats/issues/332#issuecomment-2042482371");
        }
        
        private void mlLeaderboard_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                using (LeaderboardDisplay leaderboard = new LeaderboardDisplay()) {
                    leaderboard.StatsForm = this;
                    leaderboard.Text = $@"      {Multilingual.GetWord("leaderboard_menu_title")}";
                    leaderboard.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.leaderboard_icon : Properties.Resources.leaderboard_gray_icon;
                    leaderboard.BackMaxSize = 32;
                    leaderboard.BackImagePadding = new Padding(20, 21, 0, 0);
                    this.leaderboardOverallRankList?.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                    this.weeklyCrownList?.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                    leaderboard.ShowDialog(this);
                }
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public void menuStats_Click(object sender, EventArgs e) {
            try {
                ToolStripMenuItem button = sender as ToolStripMenuItem;
                if (Equals(button, this.menuCustomRangeStats) || Equals(button, this.trayCustomRangeStats)) {
                    if (this.isStartingUp) {
                        this.updateFilterRange = true;
                    } else {
                        using (FilterCustomRange filterCustomRange = new FilterCustomRange()) {
                            filterCustomRange.StatsForm = this;
                            filterCustomRange.startDate = this.customfilterRangeStart;
                            filterCustomRange.endDate = this.customfilterRangeEnd;
                            filterCustomRange.selectedCustomTemplateSeason = this.selectedCustomTemplateSeason;
                            this.EnableInfoStrip(false);
                            this.EnableMainMenu(false);
                            if (filterCustomRange.ShowDialog(this) == DialogResult.OK) {
                                this.menuCustomRangeStats.Checked = true;
                                this.menuAllStats.Checked = false;
                                this.menuSeasonStats.Checked = false;
                                this.menuWeekStats.Checked = false;
                                this.menuDayStats.Checked = false;
                                this.menuSessionStats.Checked = false;
                                this.trayCustomRangeStats.Checked = true;
                                this.trayAllStats.Checked = false;
                                this.traySeasonStats.Checked = false;
                                this.trayWeekStats.Checked = false;
                                this.trayDayStats.Checked = false;
                                this.traySessionStats.Checked = false;
                                this.selectedCustomTemplateSeason = filterCustomRange.selectedCustomTemplateSeason;
                                this.customfilterRangeStart = filterCustomRange.startDate;
                                this.customfilterRangeEnd = filterCustomRange.endDate;
                                this.updateFilterRange = true;
                            } else {
                                this.EnableInfoStrip(true);
                                this.EnableMainMenu(true);
                                return;
                            }
                            this.EnableInfoStrip(true);
                            this.EnableMainMenu(true);
                        }
                    }
                } else if (Equals(button, this.menuAllStats) || Equals(button, this.menuSeasonStats) || Equals(button, this.menuWeekStats) || Equals(button, this.menuDayStats) || Equals(button, this.menuSessionStats)) {
                    if (!this.menuAllStats.Checked && !this.menuSeasonStats.Checked && !this.menuWeekStats.Checked && !this.menuDayStats.Checked && !this.menuSessionStats.Checked) {
                        button.Checked = true;
                        switch (button.Name) {
                            case "menuCustomRangeStats":
                                this.trayCustomRangeStats.Checked = true; break;
                            case "menuAllStats":
                                this.trayAllStats.Checked = true; break;
                            case "menuSeasonStats":
                                this.traySeasonStats.Checked = true; break;
                            case "menuWeekStats":
                                this.trayWeekStats.Checked = true; break;
                            case "menuDayStats":
                                this.trayDayStats.Checked = true; break;
                            case "menuSessionStats":
                                this.traySessionStats.Checked = true; break;
                        }
                        return;
                    }
                    this.updateFilterType = true;
                    this.updateFilterRange = false;

                    foreach (var item in this.menuStatsFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem) {
                            if (menuItem != null && menuItem.Checked && menuItem != button) {
                                menuItem.Checked = false;
                                switch (menuItem.Name) {
                                    case "menuCustomRangeStats":
                                        this.trayCustomRangeStats.Checked = false; break;
                                    case "menuAllStats":
                                        this.trayAllStats.Checked = false; break;
                                    case "menuSeasonStats":
                                        this.traySeasonStats.Checked = false; break;
                                    case "menuWeekStats":
                                        this.trayWeekStats.Checked = false; break;
                                    case "menuDayStats":
                                        this.trayDayStats.Checked = false; break;
                                    case "menuSessionStats":
                                        this.traySessionStats.Checked = false; break;
                                }
                            }
                            
                            if (menuItem.Checked) {
                                switch (menuItem.Name) {
                                    case "menuCustomRangeStats":
                                        this.trayCustomRangeStats.Checked = true; break;
                                    case "menuAllStats":
                                        this.trayAllStats.Checked = true; break;
                                    case "menuSeasonStats":
                                        this.traySeasonStats.Checked = true; break;
                                    case "menuWeekStats":
                                        this.trayWeekStats.Checked = true; break;
                                    case "menuDayStats":
                                        this.trayDayStats.Checked = true; break;
                                    case "menuSessionStats":
                                        this.traySessionStats.Checked = true; break;
                                }
                            }
                        }
                    }
                } else if (Equals(button, this.menuAllPartyStats) || Equals(button, this.menuSoloStats) || Equals(button, this.menuPartyStats)) {
                    if (!this.menuAllPartyStats.Checked && !this.menuSoloStats.Checked && !this.menuPartyStats.Checked) {
                        button.Checked = true;
                        switch (button.Name) {
                            case "menuAllPartyStats":
                                this.trayAllPartyStats.Checked = true; break;
                            case "menuSoloStats":
                                this.traySoloStats.Checked = true; break;
                            case "menuPartyStats":
                                this.trayPartyStats.Checked = true; break;
                        }
                        return;
                    }

                    foreach (var item in this.menuPartyFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem) {
                            if (menuItem != null && menuItem.Checked && menuItem != button) {
                                menuItem.Checked = false;
                                switch (menuItem.Name) {
                                    case "menuAllPartyStats":
                                        this.trayAllPartyStats.Checked = false; break;
                                    case "menuSoloStats":
                                        this.traySoloStats.Checked = false; break;
                                    case "menuPartyStats":
                                        this.trayPartyStats.Checked = false; break;
                                }
                            }
                            
                            if (menuItem.Checked) {
                                switch (menuItem.Name) {
                                    case "menuAllPartyStats":
                                        this.trayAllPartyStats.Checked = true; break;
                                    case "menuSoloStats":
                                        this.traySoloStats.Checked = true; break;
                                    case "menuPartyStats":
                                        this.trayPartyStats.Checked = true; break;
                                }
                            }
                        }
                    }
                } else if (this.ProfileMenuItems.Contains(button)) {
                    if (this.AllProfiles.Count != 0) {
                        for (int i = this.ProfileMenuItems.Count - 1; i >= 0; i--) {
                            if (this.ProfileMenuItems[i].Name == button.Name) {
                                this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => string.Equals(p.ProfileName, this.ProfileMenuItems[i].Text.Replace("&&", "&")) && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                            }
                            this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                            this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                        }
                    }
                    this.currentProfile = this.GetProfileIdByName(button.Text.Replace("&&", "&"));
                    this.updateSelectedProfile = true;
                } else if (Equals(button, this.trayAllStats) || Equals(button, this.traySeasonStats) || Equals(button, this.trayWeekStats) || Equals(button, this.trayDayStats) || Equals(button, this.traySessionStats)) {
                    if (!this.trayAllStats.Checked && !this.traySeasonStats.Checked && !this.trayWeekStats.Checked && !this.trayDayStats.Checked && !this.traySessionStats.Checked) {
                        button.Checked = true;
                        switch (button.Name) {
                            case "trayCustomRangeStats":
                                this.menuCustomRangeStats.Checked = true; break;
                            case "trayAllStats":
                                this.menuAllStats.Checked = true; break;
                            case "traySeasonStats":
                                this.menuSeasonStats.Checked = true; break;
                            case "trayWeekStats":
                                this.menuWeekStats.Checked = true; break;
                            case "trayDayStats":
                                this.menuDayStats.Checked = true; break;
                            case "traySessionStats":
                                this.menuSessionStats.Checked = true; break;
                        }
                        return;
                    }
                    this.updateFilterType = true;
                    this.updateFilterRange = false;

                    foreach (var item in this.trayStatsFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem) {
                            if (menuItem != null && menuItem.Checked && menuItem != button) {
                                menuItem.Checked = false;
                                switch (menuItem.Name) {
                                    case "trayCustomRangeStats":
                                        this.menuCustomRangeStats.Checked = false; break;
                                    case "trayAllStats":
                                        this.menuAllStats.Checked = false; break;
                                    case "traySeasonStats":
                                        this.menuSeasonStats.Checked = false; break;
                                    case "trayWeekStats":
                                        this.menuWeekStats.Checked = false; break;
                                    case "trayDayStats":
                                        this.menuDayStats.Checked = false; break;
                                    case "traySessionStats":
                                        this.menuSessionStats.Checked = false; break;
                                }
                            }
                            
                            if (menuItem.Checked) {
                                switch (menuItem.Name) {
                                    case "trayCustomRangeStats":
                                        this.menuCustomRangeStats.Checked = true; break;
                                    case "trayAllStats":
                                        this.menuAllStats.Checked = true; break;
                                    case "traySeasonStats":
                                        this.menuSeasonStats.Checked = true; break;
                                    case "trayWeekStats":
                                        this.menuWeekStats.Checked = true; break;
                                    case "trayDayStats":
                                        this.menuDayStats.Checked = true; break;
                                    case "traySessionStats":
                                        this.menuSessionStats.Checked = true; break;
                                }
                            }
                        }
                    }
                } else if (Equals(button, this.trayAllPartyStats) || Equals(button, this.traySoloStats) || Equals(button, this.trayPartyStats)) {
                    if (!this.trayAllPartyStats.Checked && !this.traySoloStats.Checked && !this.trayPartyStats.Checked) {
                        button.Checked = true;
                        switch (button.Name) {
                            case "trayAllPartyStats":
                                this.menuAllPartyStats.Checked = true; break;
                            case "traySoloStats":
                                this.menuSoloStats.Checked = true; break;
                            case "trayPartyStats":
                                this.menuPartyStats.Checked = true; break;
                        }
                        return;
                    }
                    
                    foreach (var item in this.trayPartyFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem) {
                            if (menuItem != null && menuItem.Checked && menuItem != button) {
                                menuItem.Checked = false;
                                switch (menuItem.Name) {
                                    case "trayAllPartyStats":
                                        this.menuAllPartyStats.Checked = false; break;
                                    case "traySoloStats":
                                        this.menuSoloStats.Checked = false; break;
                                    case "trayPartyStats":
                                        this.menuPartyStats.Checked = false; break;
                                }
                            }

                            if (menuItem.Checked) {
                                switch (menuItem.Name) {
                                    case "trayAllPartyStats":
                                        this.menuAllPartyStats.Checked = true; break;
                                    case "traySoloStats":
                                        this.menuSoloStats.Checked = true; break;
                                    case "trayPartyStats":
                                        this.menuPartyStats.Checked = true; break;
                                }
                            }
                        }
                    }
                } else if (this.ProfileTrayItems.Contains(button)) {
                    if (this.AllProfiles.Count != 0) {
                        for (int i = this.ProfileTrayItems.Count - 1; i >= 0; i--) {
                            if (this.ProfileTrayItems[i].Name == button.Name) {
                                this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => string.Equals(p.ProfileName, this.ProfileTrayItems[i].Text.Replace("&&", "&")) && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                            }
                            this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                            this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                        }
                    }
                    this.currentProfile = this.GetProfileIdByName(button.Text.Replace("&&", "&"));
                    this.updateSelectedProfile = true;
                }
                
                foreach (LevelStats calculator in this.StatDetails) {
                    calculator.Clear();
                }

                this.ClearTotals();

                int profile = this.currentProfile;

                List<RoundInfo> rounds;
                if (this.menuCustomRangeStats.Checked) {
                    rounds = this.AllStats.Where(roundInfo => roundInfo.Start >= this.customfilterRangeStart &&
                                                              roundInfo.Start <= this.customfilterRangeEnd &&
                                                              roundInfo.Profile == profile && this.IsInPartyFilter(roundInfo)).ToList();
                } else {
                    DateTime compareDate = this.menuAllStats.Checked ? DateTime.MinValue :
                                           this.menuSeasonStats.Checked ? SeasonStart :
                                           this.menuWeekStats.Checked ? WeekStart :
                                           this.menuDayStats.Checked ? DayStart : SessionStart;
                    rounds = this.AllStats.Where(roundInfo => roundInfo.Start > compareDate && roundInfo.Profile == profile && this.IsInPartyFilter(roundInfo)).ToList();
                }

                rounds.Sort();

                if (!this.isStartingUp && this.updateFilterType) {
                    this.updateFilterType = false;
                    this.CurrentSettings.FilterType = this.menuSeasonStats.Checked ? 2 :
                                                      this.menuWeekStats.Checked ? 3 :
                                                      this.menuDayStats.Checked ? 4 :
                                                      this.menuSessionStats.Checked ? 5 : 1;
                    this.CurrentSettings.SelectedCustomTemplateSeason = -1;
                    this.CurrentSettings.CustomFilterRangeStart = DateTime.MinValue;
                    this.CurrentSettings.CustomFilterRangeEnd = DateTime.MaxValue;
                    this.SaveUserSettings();
                } else if (!this.isStartingUp && this.updateFilterRange) {
                    this.updateFilterRange = false;
                    this.CurrentSettings.FilterType = 0;
                    this.CurrentSettings.SelectedCustomTemplateSeason = this.selectedCustomTemplateSeason;
                    this.CurrentSettings.CustomFilterRangeStart = this.customfilterRangeStart;
                    this.CurrentSettings.CustomFilterRangeEnd = this.customfilterRangeEnd;
                    this.SaveUserSettings();
                } else if (!this.isStartingUp && this.updateSelectedProfile) {
                    this.updateSelectedProfile = false;
                    this.CurrentSettings.SelectedProfile = profile;
                    this.SaveUserSettings();
                }
                
                this.overlay.ResetBackgroundImage();
                
                this.loadingExisting = true;
                this.LogFile_OnParsedLogLines(rounds);
                this.loadingExisting = false;
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void menuUpdate_Click(object sender, EventArgs e) {
            try {
                if (Utils.IsInternetConnected()) {
                    if (this.CheckForUpdate(false)) {
                        this.Stats_ExitProgram(this, null);
                    }
                } else {
                    MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_check_internet_connection")}", $"{Multilingual.GetWord("message_check_internet_connection_caption")}",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_update_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public string GetUserCreativeLevelTitle(string shareCode) {
            string levelName = this.AllStats.FindLast(r => r.UseShareCode && string.Equals(r.Name, shareCode))?.CreativeTitle;
            return string.IsNullOrEmpty(levelName) ? shareCode : levelName;
        }
        
        public string[] FindUserCreativeAuthor(JsonElement authorData) {
            string[] onlinePlatformInfo = { "N/A", "N/A" };
            string onlinePlatformId = string.Empty;
            string onlinePlatformNickname = string.Empty;
            using (JsonElement.ObjectEnumerator objectEnumerator = authorData.EnumerateObject()) {
                while (objectEnumerator.MoveNext()) {
                    JsonProperty current = objectEnumerator.Current;
                    if (!string.IsNullOrEmpty(onlinePlatformId)) onlinePlatformId += ";";
                    onlinePlatformId += current.Name;
                    if (!string.IsNullOrEmpty(onlinePlatformNickname)) onlinePlatformNickname += ";";
                    onlinePlatformNickname += current.Value.GetString();
                }
                onlinePlatformInfo[0] = onlinePlatformId;
                onlinePlatformInfo[1] = onlinePlatformNickname;
            }
            return onlinePlatformInfo;
        }
        
#if AllowUpdate
        public void ChangeStateForAvailableNewVersion(string newVersion) {
            this.isAvailableNewVersion = true;
            this.availableNewVersion = newVersion;
            this.menuUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
            this.trayUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
            this.menuUpdate.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow;
            this.trayUpdate.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow;
        }
        
        private bool CheckForNewVersion() {
            using (ZipWebClient web = new ZipWebClient()) {
                try {
                    string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");
                    int index = assemblyInfo.IndexOf("AssemblyVersion(");
                    if (index > 0) {
                        int indexEnd = assemblyInfo.IndexOf("\")", index);
                        Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                        Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                        if (newVersion > currentVersion) {
                            this.ChangeStateForAvailableNewVersion(newVersion.ToString(2));
                            return true;
                        }
                    }
                } catch {
                    return false;
                }
            }
            return false;
        }
#endif
        
        private bool CheckForUpdate(bool isSilent) {
#if AllowUpdate
            using (ZipWebClient web = new ZipWebClient()) {
                try {
                    string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");
                    int index = assemblyInfo.IndexOf("AssemblyVersion(");
                    if (index > 0) {
                        int indexEnd = assemblyInfo.IndexOf("\")", index);
                        Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                        Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                        if (newVersion > currentVersion) {
                            this.ChangeStateForAvailableNewVersion(newVersion.ToString(2));
                            if (MetroMessageBox.Show(this,
                                    $"{Multilingual.GetWord("message_update_question_prefix")} [ v{newVersion.ToString(2)} ] {Multilingual.GetWord("message_update_question_suffix")}",
                                    $"{Multilingual.GetWord("message_update_question_caption")}",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                if (!isSilent) {
                                    if (!this.overlay.Disposing && !this.overlay.IsDisposed && !this.IsDisposed && !this.Disposing) {
                                        this.SaveWindowState();
                                    }
                                }
                                this.CurrentSettings.ShowChangelog = true;
                                this.SaveUserSettings();
                                this.Hide();
                                this.overlay?.Hide();
                                
                                using (DownloadProgress progress = new DownloadProgress()) {
                                    this.StatsDB?.Dispose();
                                    progress.ZipWebClient = web;
                                    progress.DownloadUrl = Utils.FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL;
                                    progress.FileName = $"{CURRENTDIR}FallGuysStats.zip";
                                    progress.ShowDialog(this);
                                }

                                this.isUpdate = true;
                                return true;
                            }
                        } else if (!isSilent) {
                            MetroMessageBox.Show(this,
                                $"{Multilingual.GetWord("message_update_latest_version")}" +
                                $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}" +
                                $"{Multilingual.GetWord("main_update_prefix_tooltip").Trim()}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip").Trim()}",
                                $"{Multilingual.GetWord("message_update_question_caption")}",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    } else if (!isSilent) {
                        MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_not_determine_version")}",
                            $"{Multilingual.GetWord("message_update_error_caption")}",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch {
                    return false;
                }
            }
#else
            this.LaunchHelpInBrowser();
#endif
            return false;
        }
        
        private void SetSystemTrayIcon(bool enable) {
            this.trayIcon.Visible = enable;
            if (!enable && !this.Visible) {
                this.Visible = true;
                this.CurrentSettings.Visible = true;
            }
        }
        
        public void SetOverlayTopMost(bool topMost) {
            this.overlay.TopMost = topMost;
            if (this.overlay.Visible) {
                this.overlay.Hide();
                this.overlay.ShowInTaskbar = !topMost;
                this.overlay.Show();
            } else {
                this.overlay.ShowInTaskbar = !topMost;
            }
            this.overlay.ResetBackgroundImage();
        }
        
        public void SetAutoChangeProfile(bool autoChangeProfile) {
            this.CurrentSettings.AutoChangeProfile = autoChangeProfile;
            if (this.AllProfiles.Count != 0) {
                this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileId == this.GetCurrentProfileId() && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
            }
            this.SaveUserSettings();
        }
        
        public void SetOverlayBackgroundOpacity(int opacity) {
            if (opacity > 100) { opacity = 100; }
            if (opacity < 0) { opacity = 0; }
            this.CurrentSettings.OverlayBackgroundOpacity = opacity;
            this.overlay.Opacity = opacity / 100d;
            this.SaveUserSettings();
            this.overlay.ResetBackgroundImage();
        }
        
        private void SetMinimumSize() {
            this.MinimumSize = new Size(CurrentLanguage == Language.English ? 730 :
                                        CurrentLanguage == Language.French ? 860 :
                                        CurrentLanguage == Language.Korean ? 710 :
                                        CurrentLanguage == Language.Japanese ? 805 : 685
                                        , 350);
        }
        
        private async void menuSettings_Click(object sender, EventArgs e) {
            try {
                using (Settings settings = new Settings()) {
                    //settings.Icon = this.Icon;
                    settings.CurrentSettings = this.CurrentSettings;
                    settings.BackMaxSize = 32;
                    settings.BackImagePadding = new Padding(20, 19, 0, 0);
                    settings.StatsForm = this;
                    settings.Overlay = this.overlay;
                    string lastLogPath = this.CurrentSettings.LogPath;
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    if (settings.ShowDialog(this) == DialogResult.OK) {
                        this.CurrentSettings = settings.CurrentSettings;
                        UseWebProxy = this.CurrentSettings.UseProxyServer;
                        ProxyAddress = this.CurrentSettings.ProxyAddress;
                        ProxyPort = this.CurrentSettings.ProxyPort;
                        EnableProxyAuthentication = this.CurrentSettings.EnableProxyAuthentication;
                        ProxyUsername = this.CurrentSettings.ProxyUsername;
                        ProxyPassword = this.CurrentSettings.ProxyPassword;
                        SucceededTestProxy = this.CurrentSettings.SucceededTestProxy;
                        
                        IpGeolocationService = this.CurrentSettings.IpGeolocationService;
                        
                        this.SetSystemTrayIcon(this.CurrentSettings.SystemTrayIcon);
                        this.SetTheme(CurrentTheme);
                        this.SaveUserSettings();
                        if (this.currentLanguage != (int)CurrentLanguage) {
                            this.SetMinimumSize();
                            this.ChangeLanguage();
                            this.UpdateTotals();
                            this.gridDetails.ChangeContextMenuLanguage();
                            this.UpdateGridRoundName();
                            this.overlay.ChangeLanguage();
                        }
                        this.SortGridDetails(true);
                        this.ChangeLaunchPlatformLogo(this.CurrentSettings.LaunchPlatform);
                        this.UpdateHoopsieLegends();
                        this.SetOverlayTopMost(!this.CurrentSettings.OverlayNotOnTop);
                        this.SetOverlayBackgroundOpacity(this.CurrentSettings.OverlayBackgroundOpacity);
                        this.overlay.SetBackgroundResourcesName(this.CurrentSettings.OverlayBackgroundResourceName, this.CurrentSettings.OverlayTabResourceName);
                        if (this.AllProfiles.Count != 0) {
                            this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileId == this.GetCurrentProfileId() && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                        }
                        this.Invalidate();
                        
                        IsDisplayOverlayPing = this.CurrentSettings.OverlayVisible && !this.CurrentSettings.HideRoundInfo && (this.CurrentSettings.SwitchBetweenPlayers || this.CurrentSettings.OnlyShowPing);
                        IsOverlayRoundInfoNeedRefresh = true;
                        
                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(this.CurrentSettings.LogPath) ||
                            (!string.IsNullOrEmpty(lastLogPath) && string.Equals(lastLogPath, this.CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await this.logFile.Stop();
                            string logFilePath = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath) && Directory.Exists(this.CurrentSettings.LogPath)) {
                                logFilePath = this.CurrentSettings.LogPath;
                            }
                            this.logFile.Start(logFilePath, LOGFILENAME);
                        }
                        
                        this.overlay.ResetBackgroundImage();
                    } else {
                        this.overlay.Opacity = this.CurrentSettings.OverlayBackgroundOpacity / 100D;
                    }
                    this.EnableInfoStrip(true);
                    this.EnableMainMenu(true);
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }

        private void menuOverlay_Click(object sender, EventArgs e) {
            this.ToggleOverlay(this.overlay);
        }
        
        public void ToggleOverlay(Overlay overlay) {
            if (overlay.Visible) {
                IsDisplayOverlayPing = false;
                overlay.Hide();
                this.menuOverlay.Image = Properties.Resources.stat_gray_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                this.trayOverlay.Image = Properties.Resources.stat_gray_icon;
                this.trayOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                if (!overlay.IsFixed()) {
                    this.CurrentSettings.OverlayLocationX = overlay.Location.X;
                    this.CurrentSettings.OverlayLocationY = overlay.Location.Y;
                    this.CurrentSettings.OverlayWidth = overlay.Width;
                    this.CurrentSettings.OverlayHeight = overlay.Height;
                }
                this.CurrentSettings.OverlayVisible = false;
                this.SaveUserSettings();
            } else {
                IsDisplayOverlayPing = !this.CurrentSettings.HideRoundInfo && (this.CurrentSettings.SwitchBetweenPlayers || this.CurrentSettings.OnlyShowPing);
                overlay.TopMost = !this.CurrentSettings.OverlayNotOnTop;
                overlay.Show();
                overlay.ShowInTaskbar = this.CurrentSettings.OverlayNotOnTop;
                this.menuOverlay.Image = Properties.Resources.stat_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.trayOverlay.Image = Properties.Resources.stat_icon;
                this.trayOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.CurrentSettings.OverlayVisible = true;
                this.SaveUserSettings();

                if (overlay.IsFixed()) {
                    if (this.CurrentSettings.OverlayFixedPositionX.HasValue &&
                        Utils.IsOnScreen(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value, overlay.Width, overlay.Height))
                    {
                        overlay.FlipDisplay(this.CurrentSettings.FixedFlippedDisplay);
                        overlay.Location = new Point(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value);
                    } else {
                        overlay.Location = this.Location;
                    }
                } else {
                    overlay.Location = this.CurrentSettings.OverlayLocationX.HasValue && Utils.IsOnScreen(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value, overlay.Width, overlay.Height)
                                       ? new Point(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value)
                                       : this.Location;
                }
                this.overlay.ResetBackgroundImage();
            }
        }
        
        private void menuHelp_Click(object sender, EventArgs e) {
            this.LaunchHelpInBrowser();
        }
        
        private void menuEditProfiles_Click(object sender, EventArgs e) {
            try {
                using (EditProfiles editProfiles = new EditProfiles()) {
                    editProfiles.StatsForm = this;
                    editProfiles.Profiles = this.AllProfiles;
                    editProfiles.AllStats = this.AllStats;
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    editProfiles.ShowDialog(this);
                    this.EnableInfoStrip(true);
                    this.EnableMainMenu(true);
                    if (editProfiles.IsUpdate || editProfiles.IsDelete) {
                        lock (this.StatsDB) {
                            this.StatsDB.BeginTrans();
                            this.AllProfiles = editProfiles.Profiles;
                            this.Profiles.DeleteAll();
                            this.Profiles.InsertBulk(this.AllProfiles);
                            this.AllStats = editProfiles.AllStats;
                            if (editProfiles.IsUpdate) this.RoundDetails.Update(this.AllStats);
                            if (editProfiles.IsDelete) {
                                foreach (int p in editProfiles.DeleteList) {
                                    this.RoundDetails.DeleteMany(r => r.Profile == p);
                                }
                            }
                            this.StatsDB.Commit();
                        }
                        this.ReloadProfileMenuItems();
                        IsOverlayRoundInfoNeedRefresh = true;
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }
        
        private void menuLaunchFallGuys_Click(object sender, EventArgs e) {
            try {
                this.LaunchGame(false);
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ChangeLaunchPlatformLogo(int launchPlatform) {
            this.trayLaunchFallGuys.Image = launchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
            this.menuLaunchFallGuys.Image = launchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
        }
        
        public void SetLeaderboardTitle() {
            this.BeginInvoke((MethodInvoker)delegate {
                if (OnlineServiceType != OnlineServiceTypes.None && !string.IsNullOrEmpty(OnlineServiceId) && !string.IsNullOrEmpty(OnlineServiceNickname)) {
                    this.mlLeaderboard.Image = OnlineServiceType == OnlineServiceTypes.EpicGames ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
                    this.mlLeaderboard.Text = OnlineServiceNickname;
                } else {
                    this.mlLeaderboard.Text = Multilingual.GetWord("leaderboard_menu_title");
                }
                this.mlLeaderboard.Location = new Point(this.Width - this.mlLeaderboard.Width - 10, this.mlLeaderboard.Location.Y);
                this.mlLeaderboard.Enabled = true;
            });
        }
        
        private void ChangeLanguage() {
            this.SuspendLayout();
            this.currentLanguage = (int)CurrentLanguage;
            this.mainWndTitle = $@"     {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.trayIcon.Text = this.mainWndTitle.Trim();
            this.Text = this.mainWndTitle;
            this.menu.Font = Overlay.GetMainFont(12);
            this.menuLaunchFallGuys.Font = Overlay.GetMainFont(12);
            this.infoStrip.Font = Overlay.GetMainFont(13);
            this.infoStrip2.Font = Overlay.GetMainFont(13);
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(14);
            this.lblCreativeLevel.Text = Multilingual.GetWord("settings_grouping_creative_round_levels");
            this.lblIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            this.mlReportCheater.Text = Multilingual.GetWord("leaderboard_report_cheater");
            this.mlReportCheater.Location = new Point(this.Width - this.mlReportCheater.Width - 10, this.mlReportCheater.Location.Y);
            
            this.traySettings.Text = Multilingual.GetWord("main_settings");
            this.trayFilters.Text = Multilingual.GetWord("main_filters");
            this.trayStatsFilter.Text = Multilingual.GetWord("main_stats");
            this.trayCustomRangeStats.Text = Multilingual.GetWord("main_custom_range");
            this.trayAllStats.Text = Multilingual.GetWord("main_all");
            this.traySeasonStats.Text = Multilingual.GetWord("main_season");
            this.trayWeekStats.Text = Multilingual.GetWord("main_week");
            this.trayDayStats.Text = Multilingual.GetWord("main_day");
            this.traySessionStats.Text = Multilingual.GetWord("main_session");
            this.trayPartyFilter.Text = Multilingual.GetWord("main_party_type");
            this.trayAllPartyStats.Text = Multilingual.GetWord("main_all");
            this.traySoloStats.Text = Multilingual.GetWord("main_solo");
            this.trayPartyStats.Text = Multilingual.GetWord("main_party");
            this.trayProfile.Text = Multilingual.GetWord("main_profile");
            this.trayEditProfiles.Text = Multilingual.GetWord("main_profile_setting");
            this.trayOverlay.Text = Multilingual.GetWord(CurrentSettings.OverlayVisible ? "main_hide_overlay" : "main_show_overlay");
            this.trayUsefulThings.Text = Multilingual.GetWord("main_useful_things");
            this.trayFallGuysWiki.Text = Multilingual.GetWord("main_fall_guys_wiki");
            this.trayFallGuysReddit.Text = Multilingual.GetWord("main_fall_guys_reddit");
            this.trayFallalytics.Text = Multilingual.GetWord("main_fallalytics");
            this.trayFallalyticsMain.Text = Multilingual.GetWord("main_fallalytics_main");
            this.trayFallalyticsFindPlayer.Text = Multilingual.GetWord("main_fallalytics_find_player");
            this.trayFallalyticsOverallRankings.Text = Multilingual.GetWord("main_fallalytics_overall_rankings");
            this.trayFallalyticsLevelRankings.Text = Multilingual.GetWord("main_fallalytics_level_rankings");
            this.trayFallalyticsWeeklyCrownLeague.Text = Multilingual.GetWord("main_fallalytics_weekly_crown_league");
            this.trayFallalyticsRoundStatistics.Text = Multilingual.GetWord("main_fallalytics_round_statistics");
            this.trayFallalyticsShowStatistics.Text = Multilingual.GetWord("main_fallalytics_show_statistics");
            this.trayFallalyticsNews.Text = Multilingual.GetWord("main_fallalytics_news");
            this.trayRollOffClub.Text = Multilingual.GetWord("main_roll_off_club");
            this.trayLostTempleAnalyzer.Text = Multilingual.GetWord("main_lost_temple_analyzer");
            this.trayFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
            this.trayFallGuysDBMain.Text = Multilingual.GetWord("main_fall_guys_db_main");
            this.trayFallGuysDBShows.Text = Multilingual.GetWord("main_fall_guys_db_shows");
            this.trayFallGuysDBDiscovery.Text = Multilingual.GetWord("main_fall_guys_db_discovery");
            this.trayFallGuysDBShop.Text = Multilingual.GetWord("main_fall_guys_db_shop");
            this.trayFallGuysDBNewsfeeds.Text = Multilingual.GetWord("main_fall_guys_db_newsfeeds");
            this.trayFallGuysDBStrings.Text = Multilingual.GetWord("main_fall_guys_db_strings");
            this.trayFallGuysDBCosmetics.Text = Multilingual.GetWord("main_fall_guys_db_cosmetics");
            this.trayFallGuysDBCrownRanks.Text = Multilingual.GetWord("main_fall_guys_db_crown_ranks");
            this.trayFallGuysDBLiveEvents.Text = Multilingual.GetWord("main_fall_guys_db_live_events");
            this.trayFallGuysDBDailyShop.Text = Multilingual.GetWord("main_fall_guys_db_daily_shop");
            this.trayFallGuysDBCreative.Text = Multilingual.GetWord("main_fall_guys_db_creative");
            this.trayFallGuysDBFaq.Text = Multilingual.GetWord("main_fall_guys_db_faq");
            this.trayFallGuysOfficial.Text = Multilingual.GetWord("main_fall_guys_official");
            this.trayUpdate.Text = Multilingual.GetWord("main_update");
            this.trayHelp.Text = Multilingual.GetWord("main_help");
            this.trayLaunchFallGuys.Text = Multilingual.GetWord("main_launch_fall_guys");
            this.trayExitProgram.Text = Multilingual.GetWord("main_exit_program");
            
            this.menuSettings.Text = Multilingual.GetWord("main_settings");
            this.menuFilters.Text = Multilingual.GetWord("main_filters");
            this.menuStatsFilter.Text = Multilingual.GetWord("main_stats");
            this.menuCustomRangeStats.Text = Multilingual.GetWord("main_custom_range");
            this.menuAllStats.Text = Multilingual.GetWord("main_all");
            this.menuSeasonStats.Text = Multilingual.GetWord("main_season");
            this.menuWeekStats.Text = Multilingual.GetWord("main_week");
            this.menuDayStats.Text = Multilingual.GetWord("main_day");
            this.menuSessionStats.Text = Multilingual.GetWord("main_session");
            this.menuPartyFilter.Text = Multilingual.GetWord("main_party_type");
            this.menuAllPartyStats.Text = Multilingual.GetWord("main_all");
            this.menuSoloStats.Text = Multilingual.GetWord("main_solo");
            this.menuPartyStats.Text = Multilingual.GetWord("main_party");
            this.menuProfile.Text = Multilingual.GetWord("main_profile");
            this.menuEditProfiles.Text = Multilingual.GetWord("main_profile_setting");
            this.menuOverlay.Text = Multilingual.GetWord(CurrentSettings.OverlayVisible ? "main_hide_overlay" : "main_show_overlay");
            this.menuUpdate.Text = Multilingual.GetWord("main_update");
            this.menuHelp.Text = Multilingual.GetWord("main_help");
            this.menuLaunchFallGuys.Text = Multilingual.GetWord("main_launch_fall_guys");
            this.menuUsefulThings.Text = Multilingual.GetWord("main_useful_things");
            this.menuFallGuysWiki.Text = Multilingual.GetWord("main_fall_guys_wiki");
            this.menuFallGuysReddit.Text = Multilingual.GetWord("main_fall_guys_reddit");
            this.menuFallalytics.Text = Multilingual.GetWord("main_fallalytics");
            this.menuFallalyticsMain.Text = Multilingual.GetWord("main_fallalytics_main");
            this.menuFallalyticsFindPlayer.Text = Multilingual.GetWord("main_fallalytics_find_player");
            this.menuFallalyticsOverallRankings.Text = Multilingual.GetWord("main_fallalytics_overall_rankings");
            this.menuFallalyticsLevelRankings.Text = Multilingual.GetWord("main_fallalytics_level_rankings");
            this.menuFallalyticsWeeklyCrownLeague.Text = Multilingual.GetWord("main_fallalytics_weekly_crown_league");
            this.menuFallalyticsRoundStatistics.Text = Multilingual.GetWord("main_fallalytics_round_statistics");
            this.menuFallalyticsShowStatistics.Text = Multilingual.GetWord("main_fallalytics_show_statistics");
            this.menuFallalyticsNews.Text = Multilingual.GetWord("main_fallalytics_news");
            this.menuRollOffClub.Text = Multilingual.GetWord("main_roll_off_club");
            this.menuLostTempleAnalyzer.Text = Multilingual.GetWord("main_lost_temple_analyzer");
            this.menuFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
            this.menuFallGuysDBMain.Text = Multilingual.GetWord("main_fall_guys_db_main");
            this.menuFallGuysDBShows.Text = Multilingual.GetWord("main_fall_guys_db_shows");
            this.menuFallGuysDBDiscovery.Text = Multilingual.GetWord("main_fall_guys_db_discovery");
            this.menuFallGuysDBShop.Text = Multilingual.GetWord("main_fall_guys_db_shop");
            this.menuFallGuysDBNewsfeeds.Text = Multilingual.GetWord("main_fall_guys_db_newsfeeds");
            this.menuFallGuysDBStrings.Text = Multilingual.GetWord("main_fall_guys_db_strings");
            this.menuFallGuysDBCosmetics.Text = Multilingual.GetWord("main_fall_guys_db_cosmetics");
            this.menuFallGuysDBCrownRanks.Text = Multilingual.GetWord("main_fall_guys_db_crown_ranks");
            this.menuFallGuysDBLiveEvents.Text = Multilingual.GetWord("main_fall_guys_db_live_events");
            this.menuFallGuysDBDailyShop.Text = Multilingual.GetWord("main_fall_guys_db_daily_shop");
            this.menuFallGuysDBCreative.Text = Multilingual.GetWord("main_fall_guys_db_creative");
            this.menuFallGuysDBFaq.Text = Multilingual.GetWord("main_fall_guys_db_faq");
            this.menuFallGuysOfficial.Text = Multilingual.GetWord("main_fall_guys_official");
            // this.SetLeaderboardTitle();
            this.ResumeLayout();
        }
    }
}
