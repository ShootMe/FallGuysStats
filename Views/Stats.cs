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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using Microsoft.Win32;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class Stats : MetroFramework.Forms.MetroForm {
        [STAThread]
        static void Main() {
            try {
                bool isAppUpdated = false;
#if AllowUpdate
                if (File.Exists(Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".bak")) {
                    isAppUpdated = true;
                }
#endif
                if (isAppUpdated || !IsAlreadyRunning()) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Stats());
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), @"Run Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private static bool IsAlreadyRunning() {
            try {
                int processCount = 0;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++) {
                    if (AppDomain.CurrentDomain.FriendlyName.Equals(processes[i].ProcessName + ".exe")) processCount++;
                    if (processCount > 1) {
                        string sysLang = CultureInfo.CurrentUICulture.Name.StartsWith("zh") ?
                                         CultureInfo.CurrentUICulture.Name :
                                         CultureInfo.CurrentUICulture.Name.Substring(0, 2);
                        CurrentLanguage = "fr".Equals(sysLang, StringComparison.Ordinal) ? Language.French :
                                          "ko".Equals(sysLang, StringComparison.Ordinal) ? Language.Korean :
                                          "ja".Equals(sysLang, StringComparison.Ordinal) ? Language.Japanese :
                                          "zh-chs".Equals(sysLang, StringComparison.Ordinal) ? Language.SimplifiedChinese :
                                          "zh-cht".Equals(sysLang, StringComparison.Ordinal) ? Language.TraditionalChinese : Language.English;
                        MessageBox.Show(Multilingual.GetWord("message_tracker_already_running"), Multilingual.GetWord("main_fall_guys_stats"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), @"Process Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }
        
        private static string LOGFILENAME = "Player.log";
        public static List<DateTime> Seasons = new List<DateTime> {
            new DateTime(2020, 8, 4, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 10, 8, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 3, 22, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 6, 21, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 11, 22, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Utc)
        };
        private static DateTime SeasonStart, WeekStart, DayStart;
        private static DateTime SessionStart = DateTime.UtcNow;
        public static Language CurrentLanguage;
        public static MetroThemeStyle CurrentTheme = MetroThemeStyle.Light;
        
        public static bool InShow = false; 
        public static bool EndedShow = false;
        
        public static bool IsDisplayOverlayTime = true;
        public static bool IsDisplayOverlayPing = false;

        public static bool IsClientRunning = false;
        public static bool IsClientHasBeenClosed = false;
        
        public static bool ToggleServerInfo = false;
        public static DateTime ConnectedToServerDate = DateTime.MinValue;
        public static string LastServerIp = string.Empty;
        public static string LastCountryAlpha2Code = string.Empty;
        public static string LastCountryRegion = string.Empty;
        public static string LastCountryCity = string.Empty;
        public static long LastServerPing = 0;
        public static bool IsBadServerPing = false;
        
        public static List<string> SucceededPlayerIds = new List<string>();
        
        public static int SavedRoundCount { get; set; }
        public static int NumPlayersSucceeded { get; set; }
        public static bool IsLastRoundRunning { get; set; }
        public static bool IsLastPlayedRoundStillPlaying { get; set; }
        public static DateTime LastGameStart { get; set; } = DateTime.MinValue;
        public static DateTime LastRoundLoad { get; set; } = DateTime.MinValue;
        public static DateTime? LastPlayedRoundStart { get; set; } = null;
        public static DateTime? LastPlayedRoundEnd { get; set; } = null;
        
        public static bool IsQueued = false;
        public static int QueuedPlayers = 0;
        
        private static FallalyticsReporter FallalyticsReporter = new FallalyticsReporter();
        
        public static string OnlineServiceId = string.Empty;
        public static string OnlineServiceNickname = string.Empty;
        public static OnlineServiceTypes OnlineServiceType = OnlineServiceTypes.None;
        public static string HostCountryCode = string.Empty;
        public static string HostCountryRegion = string.Empty;
        public static string HostCountryCity = string.Empty;
        
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        private int Shows, Rounds, CustomShows, CustomRounds;
        private TimeSpan Duration;
        private int Wins, Finals, Kudos;
        private int GoldMedals, SilverMedals, BronzeMedals, PinkMedals, EliminatedMedals;
        private int CustomGoldMedals, CustomSilverMedals, CustomBronzeMedals, CustomPinkMedals, CustomEliminatedMedals;
        private int nextShowID;
        private bool loadingExisting;
        private bool updateFilterType, updateFilterRange;
        private DateTime customfilterRangeStart = DateTime.MinValue;
        private DateTime customfilterRangeEnd = DateTime.MaxValue;
        private int selectedCustomTemplateSeason;
        private bool updateSelectedProfile, useLinkedProfiles;
        public LiteDatabase StatsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        public ILiteCollection<UserSettings> UserSettings;
        public ILiteCollection<Profiles> Profiles;
        public ILiteCollection<FallalyticsPbLog> FallalyticsPbLog;
        public ILiteCollection<ServerConnectionLog> ServerConnectionLog;
        public ILiteCollection<PersonalBestLog> PersonalBestLog;
        public List<Profiles> AllProfiles = new List<Profiles>();
        public UserSettings CurrentSettings;
        public Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        public DateTime startupTime = DateTime.UtcNow;
        private int askedPreviousShows = 0;
        private TextInfo textInfo;
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
        private int profileIdWithLinkedCustomShow = -1;
        private Toast toast;
        public Point screenCenter;
        
        public readonly string[] PublicShowIdList = {
            "main_show",
            "squads_2player_template",
            "squads_4player",
            "event_xtreme_fall_guys_template",
            "event_xtreme_fall_guys_squads_template",
            "event_only_finals_v2_template",
            "event_only_races_any_final_template",
            "event_only_fall_ball_template",
            "event_only_hexaring_template",
            "event_only_floor_fall_template",
            "event_only_floor_fall_low_grav",
            "event_only_blast_ball_trials_template",
            "event_only_thin_ice_template",
            "event_only_slime_climb",
            "event_only_jump_club_template",
            "event_only_hoverboard_template",
            "event_walnut_template",
            "survival_of_the_fittest",
            "show_robotrampage_ss2_show1_template",
            "event_le_anchovy_template",
            "event_pixel_palooza_template",
            "xtreme_party",
            "invisibeans_mode",
            "fall_guys_creative_mode",
            "private_lobbies"
        };
        
        public readonly Dictionary<string, string> LevelIdReplacerInDigisShuffleShow = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "wle_shuffle_halloween_1", "current_wle_fp5_falloween_7_01_01" },
            { "wle_shuffle_halloween_2", "current_wle_fp5_falloween_7_01_02" },
            { "wle_shuffle_halloween_3", "current_wle_fp5_falloween_7_02_01" },
            { "wle_shuffle_halloween_4", "current_wle_fp5_falloween_7_01_03" },
            { "wle_shuffle_halloween_5", "current_wle_fp5_falloween_7_01_05" },
            { "wle_shuffle_halloween_6", "current_wle_fp5_falloween_7_01_06" },
            { "wle_shuffle_halloween_7", "current_wle_fp5_falloween_7_01_07" },
            { "wle_shuffle_halloween_8", "current_wle_fp5_falloween_7_01_08" },
            { "wle_shuffle_halloween_9", "current_wle_fp5_falloween_7_01_09" },
            { "wle_shuffle_halloween_10", "current_wle_fp5_falloween_7_04_01" },
            { "wle_shuffle_halloween_12", "current_wle_fp5_falloween_7_10_01" },
            { "wle_shuffle_halloween_13", "current_wle_fp5_falloween_7_04_03" },
            { "wle_shuffle_halloween_14", "current_wle_fp5_falloween_7_04_04" },
            { "wle_shuffle_halloween_15", "current_wle_fp5_falloween_7_03_01" },
            { "wle_shuffle_halloween_16", "current_wle_fp5_falloween_7_05_01" },
            { "wle_shuffle_halloween_17", "current_wle_fp5_falloween_7_06_01" },
            { "wle_shuffle_halloween_18", "current_wle_fp5_falloween_7_05_02" },
            { "wle_shuffle_halloween_19", "current_wle_fp5_falloween_7_05_03" },
            { "wle_shuffle_halloween_20", "current_wle_fp5_falloween_7_06_02" },
            { "wle_shuffle_halloween_22", "current_wle_fp5_falloween_7_06_04" },
            { "wle_shuffle_halloween_23", "current_wle_fp5_falloween_7_06_05" },
            { "wle_shuffle_halloween_25", "current_wle_fp5_falloween_1_04" },
            { "wle_shuffle_halloween_26", "current_wle_fp5_falloween_1_12" },
            { "wle_shuffle_halloween_27", "current_wle_fp5_falloween_1_14" },
            { "wle_shuffle_halloween_29", "current_wle_fp5_falloween_1_11" },
            { "wle_shuffle_halloween_30", "current_wle_fp5_falloween_1_01" },
            { "wle_shuffle_halloween_31", "current_wle_fp5_falloween_1_05" }, 
            { "wle_shuffle_halloween_32", "current_wle_fp5_falloween_1_06" },
            { "wle_shuffle_halloween_33", "current_wle_fp5_falloween_2_01" },
            { "wle_shuffle_halloween_34", "current_wle_fp5_falloween_6_02" },
            { "wle_shuffle_halloween_35", "current_wle_fp5_falloween_6_03" },
            { "wle_shuffle_halloween_36", "current_wle_fp5_falloween_11_01" },
            { "wle_shuffle_halloween_37", "current_wle_fp5_falloween_2_02" },
            { "wle_shuffle_halloween_38", "current_wle_fp5_falloween_1_09" },
            { "wle_shuffle_halloween_40", "current_wle_fp5_falloween_1_08" },
            { "wle_shuffle_halloween_41", "current_wle_fp5_falloween_2_03" },
            { "wle_shuffle_halloween_42", "current_wle_fp5_falloween_4_03" },
            { "wle_shuffle_halloween_43", "current_wle_fp5_falloween_4_11" },
            { "wle_shuffle_halloween_44", "current_wle_fp5_falloween_4_08" },
            { "wle_shuffle_halloween_45", "current_wle_fp5_falloween_4_13" },
            { "wle_shuffle_halloween_47", "current_wle_fp5_falloween_4_12" },
            { "wle_shuffle_halloween_48", "current_wle_fp5_falloween_9_01" },
            { "wle_shuffle_halloween_49", "current_wle_fp5_falloween_2_03_01" },
            { "wle_shuffle_halloween_50", "current_wle_fp5_falloween_9_03" },
            { "wle_shuffle_halloween_51", "current_wle_fp5_falloween_9_02" },
            { "wle_shuffle_halloween_52", "current_wle_fp5_falloween_9_04" },
            { "wle_shuffle_halloween_53", "current_wle_fp5_falloween_2_03_02" },
            { "wle_shuffle_halloween_54", "current_wle_fp5_falloween_9_05" },
            { "wle_shuffle_halloween_55", "current_wle_fp5_falloween_2_03_03" },
            { "wle_shuffle_halloween_56", "current_wle_fp5_falloween_2_03_04" },
            { "wle_shuffle_halloween_57", "current_wle_fp5_falloween_2_03_05" },
            { "wle_shuffle_halloween_58", "current_wle_fp5_falloween_2_03_06" },
            { "wle_shuffle_halloween_59", "current_wle_fp5_falloween_10_01" },
            { "wle_shuffle_halloween_60", "current_wle_fp5_falloween_14_01" },
            { "wle_shuffle_halloween_61", "current_wle_fp5_falloween_10_02" },
            { "wle_shuffle_halloween_64", "current_wle_fp5_falloween_12_01" },
            { "wle_shuffle_halloween_65", "current_wle_fp5_falloween_12_02" },
            { "wle_shuffle_halloween_66", "current_wle_fp5_falloween_13_01" },
            { "wle_shuffle_halloween_67", "current_wle_fp5_falloween_4_02" },
            { "wle_shuffle_halloween_68", "current_wle_fp5_falloween_4_09" },
            { "wle_shuffle_halloween_69", "current_wle_fp5_falloween_4_19" },
            { "wle_shuffle_halloween_70", "current_wle_fp5_falloween_5_05" },
            { "wle_shuffle_halloween_71", "current_wle_fp5_falloween_5_04" },
            { "wle_shuffle_halloween_72", "current_wle_fp5_falloween_5_03" },
            { "wle_shuffle_halloween_73", "current_wle_fp5_falloween_5_07" },
            { "wle_shuffle_halloween_74", "current_wle_fp5_falloween_5_06" },
            { "wle_shuffle_halloween_75", "current_wle_fp5_falloween_4_10" },
            { "wle_shuffle_halloween_76", "current_wle_fp5_falloween_4_05" },
            { "wle_shuffle_halloween_77", "current_wle_fp5_falloween_4_15" },
            { "wle_shuffle_halloween_78", "current_wle_fp5_falloween_4_06" },
            { "wle_shuffle_halloween_79", "current_wle_fp5_falloween_4_17" },
            { "wle_shuffle_halloween_80", "current_wle_fp5_falloween_4_07" },
            { "wle_shuffle_halloween_81", "current_wle_fp5_falloween_4_14" },
            { "wle_shuffle_halloween_82", "current_wle_fp5_falloween_4_01" },
            { "wle_shuffle_halloween_83", "current_wle_fp5_falloween_4_16" },
            { "wle_shuffle_halloween_84", "current_wle_fp5_falloween_4_04" },
            { "wle_shuffle_halloween_85", "current_wle_fp5_falloween_4_18" },
            { "wle_shuffle_halloween_86", "current_wle_fp5_falloween_1_03" },
            { "wle_shuffle_halloween_87", "current_wle_fp5_falloween_3_04" },
            { "wle_shuffle_halloween_88", "current_wle_fp5_falloween_3_03" },
            { "wle_shuffle_halloween_90", "current_wle_fp5_falloween_3_02" }
        };
        
        private void DatabaseMigration() {
            if (File.Exists("data.db")) {
                using (var sourceDb = new LiteDatabase(@"data.db")) {
                    if (sourceDb.UserVersion != 0) { return; }
                    using (var targetDb = new LiteDatabase(@"Filename=data_new.db;Upgrade=true")) {
                        string[] tableNames = { "Profiles", "RoundDetails", "UserSettings", "ServerConnectionLog", "PersonalBestLog", "FallalyticsPbLog" };
                        foreach (var tableName in tableNames) {
                            if (!sourceDb.CollectionExists(tableName)) continue;
                            var sourceData = sourceDb.GetCollection(tableName).FindAll();
                            var targetCollection = targetDb.GetCollection(tableName);
                            targetCollection.InsertBulk(sourceData);
                        }
                        targetDb.UserVersion += 1;
                    }
                }
                File.Move("data.db", "data.db_bak");
                File.Move("data_new.db", "data.db");
            }
        }

        private Stats() {
            this.DatabaseMigration();
            Task.Run(() => {
                if (Utils.IsInternetConnected()) {
                    HostCountryCode = Utils.GetCountryInfoByIp(Utils.GetUserPublicIp(), true).Split(';')[0];
                }
            });
            this.SetSecretKey();
            
            this.mainWndTitle = $"     {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.StatsDB = new LiteDatabase(@"data.db");
            this.StatsDB.Pragma("UTC_DATE", true);
            this.UserSettings = this.StatsDB.GetCollection<UserSettings>("UserSettings");
            
            this.StatsDB.BeginTrans();
            if (this.UserSettings.Count() == 0) {
                this.CurrentSettings = this.GetDefaultSettings();
                this.UserSettings.Insert(this.CurrentSettings);
            } else {
                try {
                    this.CurrentSettings = this.UserSettings.FindAll().First();
                    CurrentLanguage = (Language)this.CurrentSettings.Multilingual;
                    CurrentTheme = this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : MetroThemeStyle.Dark;
                } catch {
                    this.UserSettings.DeleteAll();
                    this.CurrentSettings = GetDefaultSettings();
                    this.UserSettings.Insert(this.CurrentSettings);
                }
            }
            this.StatsDB.Commit();
            
            this.RemoveUpdateFiles();
            
            this.InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            
#if !AllowUpdate
            this.menu.Items.Remove(this.menuUpdate);
            this.trayCMenu.Items.Remove(this.trayUpdate);
#endif

            this.ShowInTaskbar = false;
            this.Opacity = 0;
            this.trayCMenu.Opacity = 0;
            
            this.textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            
            this.RoundDetails = this.StatsDB.GetCollection<RoundInfo>("RoundDetails");
            this.Profiles = this.StatsDB.GetCollection<Profiles>("Profiles");
            this.ServerConnectionLog = this.StatsDB.GetCollection<ServerConnectionLog>("ServerConnectionLog");
            this.PersonalBestLog = this.StatsDB.GetCollection<PersonalBestLog>("PersonalBestLog");
            this.FallalyticsPbLog = this.StatsDB.GetCollection<FallalyticsPbLog>("FallalyticsPbLog");
            
            this.StatsDB.BeginTrans();
            this.RoundDetails.EnsureIndex(r => r.Name);
            this.RoundDetails.EnsureIndex(r => r.ShowID);
            this.RoundDetails.EnsureIndex(r => r.Round);
            this.RoundDetails.EnsureIndex(r => r.Start);
            this.RoundDetails.EnsureIndex(r => r.InParty);
            
            this.Profiles.EnsureIndex(p => p.ProfileId);
            
            this.ServerConnectionLog.EnsureIndex(f => f.SessionId);
            this.PersonalBestLog.EnsureIndex(f => f.SessionId);
            
            this.FallalyticsPbLog.EnsureIndex(f => f.PbId);
            this.FallalyticsPbLog.EnsureIndex(f => f.RoundId);
            this.FallalyticsPbLog.EnsureIndex(f => f.ShowId);
            
            if (this.Profiles.Count() == 0) {
                string sysLang = CultureInfo.CurrentUICulture.Name.StartsWith("zh") ?
                                 CultureInfo.CurrentUICulture.Name :
                                 CultureInfo.CurrentUICulture.Name.Substring(0, 2);
                using (SelectLanguage initLanguageForm = new SelectLanguage(sysLang)) {
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    if (initLanguageForm.ShowDialog(this) == DialogResult.OK) {
                        CurrentLanguage = initLanguageForm.selectedLanguage;
                        Overlay.SetDefaultFont(18, CurrentLanguage);
                        this.CurrentSettings.Multilingual = (int)initLanguageForm.selectedLanguage;
                        if (initLanguageForm.autoGenerateProfiles) {
                            for (int i = this.PublicShowIdList.Length; i >= 1; i--) {
                                string showId = this.PublicShowIdList[i - 1];
                                this.Profiles.Insert(new Profiles { ProfileId = i - 1, ProfileName = Multilingual.GetShowName(showId), ProfileOrder = i, LinkedShowId = showId });
                            }
                            this.CurrentSettings.AutoChangeProfile = true;
                        } else {
                            this.Profiles.Insert(new Profiles { ProfileId = 3, ProfileName = Multilingual.GetWord("main_profile_custom"), ProfileOrder = 4, LinkedShowId = "private_lobbies" });
                            this.Profiles.Insert(new Profiles { ProfileId = 2, ProfileName = Multilingual.GetWord("main_profile_squad"), ProfileOrder = 3, LinkedShowId = "squads_4player" });
                            this.Profiles.Insert(new Profiles { ProfileId = 1, ProfileName = Multilingual.GetWord("main_profile_duo"), ProfileOrder = 2, LinkedShowId = "squads_2player_template" });
                            this.Profiles.Insert(new Profiles { ProfileId = 0, ProfileName = Multilingual.GetWord("main_profile_solo"), ProfileOrder = 1, LinkedShowId = "main_show" });
                        }
                    }
                    this.EnableInfoStrip(true);
                    this.EnableMainMenu(true);
                }
            }
            this.StatsDB.Commit();
            
            this.UpdateDatabaseVersion();
            
            foreach (KeyValuePair<string, LevelStats> entry in LevelStats.ALL) {
                this.StatLookup.Add(entry.Key, entry.Value);
                this.StatDetails.Add(entry.Value);
            }

            this.BackImage = this.Icon.ToBitmap();
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(18, 18, 0, 0);
            this.SetMinimumSize();
            this.ChangeMainLanguage();
            this.InitMainDataGridView();
            this.UpdateGridRoundName();
            this.UpdateHoopsieLegends();
            
            this.ClearServerConnectionLog();
            this.ClearPersonalBestLog();

            this.CurrentRound = new List<RoundInfo>();
            
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
            this.logFile.autoChangeProfile = this.CurrentSettings.AutoChangeProfile;
            this.logFile.preventOverlayMouseClicks = this.CurrentSettings.PreventOverlayMouseClicks;
            
            string fixedPosition = this.CurrentSettings.OverlayFixedPosition;
            this.overlay.SetFixedPosition(
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("ne"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("nw"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("se"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("sw"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("free")
                );
            if (this.overlay.IsFixed()) this.overlay.Cursor = Cursors.Default;
            this.overlay.Opacity = this.CurrentSettings.OverlayBackgroundOpacity / 100D;
            this.overlay.Show();
            this.overlay.Hide();
            this.overlay.StartTimer();
            
            this.SetSystemTrayIcon(this.CurrentSettings.SystemTrayIcon);
            this.UpdateGameExeLocation();
            this.SaveUserSettings();
        }
        
        public void cmtt_levelDetails_Draw(object sender, DrawToolTipEventArgs e) {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
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
            g.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, new PointF(e.Bounds.X + 8, e.Bounds.Y - 8));
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
            Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
        }
        
        private void cmtt_overlay_Draw(object sender, DrawToolTipEventArgs e) {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            // Draw the custom background.
            //g.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            e.Graphics.FillRectangle(CurrentTheme == MetroThemeStyle.Light ? Brushes.Black : Brushes.WhiteSmoke, e.Bounds);
            
            // Draw the standard border.
            e.DrawBorder();
            
            g.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, new PointF(e.Bounds.X + 2, e.Bounds.Y + 2));
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
            Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
        }
        
        public void cmtt_center_Draw(object sender, DrawToolTipEventArgs e) {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
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
                g.DrawString(e.ToolTipText, e.Font, CurrentTheme == MetroThemeStyle.Light ? Brushes.DarkGray : Brushes.Black, e.Bounds, sf);
            }
            
            MetroToolTip t = (MetroToolTip)sender;
            PropertyInfo h = t.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            IntPtr handle = (IntPtr)h.GetValue(t);
            Control c = e.AssociatedControl;
            Point location = c.Parent.PointToScreen(new Point(c.Right - e.Bounds.Width, c.Bottom));
            Utils.MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
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
                if (this.overlay.IsMouseEnter() && ActiveForm != this) { this.SetCursorPositionCenter(); }
            });
        }
        
        public void SetCursorPositionCenter() {
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

        private void SetSecretKey() {
            Type type = Type.GetType("SecretKey");
            if (type != null) {
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
                            switch (tsmi1.Name) {
                                case "menuSettings":
                                    tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
                                case "menuFilters":
                                    tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon; break;
                                case "menuProfile":
                                    tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon; break;
                                //case "menuOverlay": break;
                                case "menuUpdate":
                                    tsmi1.Image = theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon) :
                                                                                   (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon); break;
                                case "menuHelp":
                                    tsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon; break;
                                //case "menuLaunchFallGuys": break;
                            }
                            tsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            tsmi1.MouseEnter += this.menu_MouseEnter;
                            tsmi1.MouseLeave += this.menu_MouseLeave;
                            foreach (var item1 in tsmi1.DropDownItems) {
                                if (item1 is ToolStripMenuItem subTsmi1) {
                                    if (subTsmi1.Name.Equals("menuEditProfiles")) { subTsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; }
                                    subTsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    subTsmi1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    subTsmi1.MouseEnter += this.menu_MouseEnter;
                                    subTsmi1.MouseLeave += this.menu_MouseLeave;
                                    foreach (var item2 in subTsmi1.DropDownItems) {
                                        if (item2 is ToolStripMenuItem subTsmi2) {
                                            if (subTsmi2.Name.Equals("menuCustomRangeStats")) { subTsmi2.Image = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; }
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
                            switch (tsl1.Name) {
                                case "lblCurrentProfile":
                                    tsl1.Font = Overlay.GetMainFont(14f);
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192);
                                    break;
                                case "lblTotalTime":
                                    tsl1.Font = Overlay.GetMainFont(14f);
                                    tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.clock_icon : Properties.Resources.clock_gray_icon;
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    break;
                                case "lblTotalShows":
                                case "lblTotalWins":
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    break;
                                case "lblTotalRounds":
                                    tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    break;
                                case "lblTotalFinals":
                                    tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    break;
                                case "lblLeaderboard":
                                    tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.leaderboard_icon : Properties.Resources.leaderboard_gray_icon;
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    break;
                                case "lblGoldMedal":
                                case "lblSilverMedal":
                                case "lblBronzeMedal":
                                case "lblPinkMedal":
                                case "lblEliminatedMedal":
                                case "lblKudos":
                                    tsl1.Font = Overlay.GetMainFont(14f);
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray; break;
                            }
                        } else if (tsi1 is ToolStripSeparator tss1) {
                            tss1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray; break;
                        }
                    }
                } else if (c1 is MetroToggle mt1) {
                    mt1.Theme = theme;
                } else if (c1 is Label lbl1) {
                    if (lbl1.Name.Equals("lblCreativeLevel") || lbl1.Name.Equals("lblIgnoreLevelTypeWhenSorting")) {
                        lbl1.Font = Overlay.GetMainFont(13f);
                        lbl1.ForeColor = Color.DimGray;
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
            this.dataGridViewCellStyle2.SelectionForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.Black;

            foreach (var item in this.trayCMenu.Items) {
                if (item is ToolStripMenuItem tsmi) {
                    tsmi.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tsmi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    tsmi.MouseEnter += this.TrayMenu_MouseEnter;
                    tsmi.MouseLeave += this.TrayMenu_MouseLeave;
                    switch (tsmi.Name) {
                        case "traySettings": tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
                        case "trayFilters": tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon; break;
                        case "trayProfile": tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon; break;
                        case "trayUpdate":
                            tsmi.Image = theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon) :
                                                                          (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon); break;
                        case "trayHelp":
                            tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon; break;
                        case "trayExitProgram": tsmi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.shutdown_icon : Properties.Resources.shutdown_gray_icon; break;
                    }
                    foreach (var subItem1 in tsmi.DropDownItems) {
                        if (subItem1 is ToolStripMenuItem stsmi1) {
                            if (stsmi1.Name.Equals("trayEditProfiles")) { stsmi1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; }
                            stsmi1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                            stsmi1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            stsmi1.MouseEnter += this.TrayMenu_MouseEnter;
                            stsmi1.MouseLeave += this.TrayMenu_MouseLeave;
                            foreach (var subItem2 in stsmi1.DropDownItems) {
                                if (subItem2 is ToolStripMenuItem stsmi2) {
                                    if (stsmi2.Name.Equals("trayCustomRangeStats")) { stsmi2.Image = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; }
                                    stsmi2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    stsmi2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    stsmi2.MouseEnter += this.TrayMenu_MouseEnter;
                                    stsmi2.MouseLeave += this.TrayMenu_MouseLeave;
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
        
        private void TrayMenu_MouseEnter(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = Color.Black;
                switch (tsi.Name) {
                    case "traySettings": tsi.Image = Properties.Resources.setting_icon; break;
                    case "trayFilters": tsi.Image = Properties.Resources.filter_icon; break;
                    case "trayCustomRangeStats": tsi.Image = Properties.Resources.calendar_icon; break;
                    case "trayProfile": tsi.Image = Properties.Resources.profile_icon; break;
                    case "trayUpdate": tsi.Image = this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon; break;
                    case "trayHelp": tsi.Image = Properties.Resources.github_icon; break;
                    case "trayEditProfiles": tsi.Image = Properties.Resources.setting_icon; break;
                    case "trayExitProgram": tsi.Image = Properties.Resources.shutdown_icon; break;
                }
            }
        }
        
        private void TrayMenu_MouseLeave(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                switch (tsi.Name) {
                    case "traySettings": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
                    case "trayFilters": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon; break;
                    case "trayCustomRangeStats": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; break;
                    case "trayProfile": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon; break;
                    case "trayUpdate": tsi.Image = this.Theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon) :
                                                                                         (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon); break;
                    case "trayHelp": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon; break;
                    case "trayEditProfiles": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
                    case "trayExitProgram": tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.shutdown_icon : Properties.Resources.shutdown_gray_icon; break;
                }
            }
        }
        
        private void menu_MouseEnter(object sender, EventArgs e) {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            switch (tsmi.Name) {
                case "menuSettings": tsmi.Image = Properties.Resources.setting_icon; break;
                case "menuFilters": tsmi.Image = Properties.Resources.filter_icon; break;
                case "menuCustomRangeStats": tsmi.Image = Properties.Resources.calendar_icon; break;
                case "menuProfile": tsmi.Image = Properties.Resources.profile_icon; break;
                //case "menuOverlay": break;
                case "menuUpdate": tsmi.Image = this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon; break;
                case "menuHelp": tsmi.Image = Properties.Resources.github_icon; break;
                //case "menuLaunchFallGuys": break;
                case "menuEditProfiles": tsmi.Image = Properties.Resources.setting_icon; break;
            }
            tsmi.ForeColor = Color.Black;
        }
        
        private void menu_MouseLeave(object sender, EventArgs e) {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            switch (tsmi.Name) {
                case "menuSettings": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
                case "menuFilters": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.filter_icon : Properties.Resources.filter_gray_icon; break;
                case "menuCustomRangeStats": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; break;
                case "menuProfile": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon; break;
                //case "menuOverlay": break;
                case "menuUpdate": tsmi.Image = this.Theme == MetroThemeStyle.Light ? (this.isAvailableNewVersion ? Properties.Resources.github_update_icon : Properties.Resources.github_icon) :
                                                                                      (this.isAvailableNewVersion ? Properties.Resources.github_update_gray_icon : Properties.Resources.github_gray_icon); break;
                case "menuHelp": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.github_icon : Properties.Resources.github_gray_icon; break;
                //case "menuLaunchFallGuys": break;
                case "menuEditProfiles": tsmi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon; break;
            }
            tsmi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
        }
        
        private void infoStrip_MouseEnter(object sender, EventArgs e) {
            if (sender is ToolStripLabel lblInfo) {
                //this.infoStripForeColor = lblInfo.ForeColor;
                this.Cursor = Cursors.Hand;
                this.infoStripForeColor = lblInfo.Name.Equals("lblCurrentProfile")
                    ? this.Theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192)
                    : this.Theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                
                lblInfo.ForeColor = lblInfo.Name.Equals("lblCurrentProfile")
                    ? this.Theme == MetroThemeStyle.Light ? Color.FromArgb(245, 154, 168) : Color.FromArgb(231, 251, 255)
                    : this.Theme == MetroThemeStyle.Light ? Color.FromArgb(147, 174, 248) : Color.FromArgb(255, 250, 244);

                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 16, cursorPosition.Y + 16);
                this.AllocCustomTooltip(this.cmtt_center_Draw);
                if (lblInfo.Name.Equals("lblCurrentProfileIcon")) {
                    this.ShowCustomTooltip(Multilingual.GetWord($"{(this.CurrentSettings.AutoChangeProfile ? "profile_icon_enable_tooltip" : "profile_icon_disable_tooltip")}"), this, position);
                } else if (lblInfo.Name.Equals("lblCurrentProfile")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("profile_change_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalShows")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("shows_detail_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalRounds")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("rounds_detail_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalFinals")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("finals_detail_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalWins")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("wins_detail_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalTime")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("stats_detail_tooltip"), this, position);
                } else if (lblInfo.Name.Equals("lblLeaderboard")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("leaderboard_detail_tooltip"), this, position);
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
            //this.AllProfiles = this.Profiles.FindAll().ToList();
            this.AllProfiles.AddRange(this.Profiles.FindAll());
            this.profileIdWithLinkedCustomShow = this.AllProfiles.Find(p => "private_lobbies".Equals(p.LinkedShowId))?.ProfileId ?? -1;
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
                menuItem.MouseLeave += this.setCursor_MouseLeave;
                this.menuProfile.DropDownItems.Add(menuItem);
                this.ProfileMenuItems.Add(menuItem);
                
                trayItem.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                trayItem.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                trayItem.Size = new Size(180, 22);
                trayItem.Text = profile.ProfileName.Replace("&", "&&");
                trayItem.Click += this.menuStats_Click;
                trayItem.Paint += this.menuProfile_Paint;
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
            if (this.AllProfiles.FindIndex(profile => profile.ProfileId.ToString().Equals(((ToolStripMenuItem)sender).Name.Substring(11)) && !string.IsNullOrEmpty(profile.LinkedShowId)) != -1) {
                e.Graphics.DrawImage(this.CurrentSettings.AutoChangeProfile ? Properties.Resources.link_on_icon :
                                     this.Theme == MetroThemeStyle.Light ? Properties.Resources.link_icon :
                                     Properties.Resources.link_gray_icon, 20, 5, 11, 11);
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
        
        private void UpdateDatabaseVersion() {
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

            if (this.CurrentSettings.Version == 0) {
                this.CurrentSettings.SwitchBetweenQualify = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 1;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 1) {
                this.CurrentSettings.SwitchBetweenPlayers = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 2;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 2) {
                this.CurrentSettings.SwitchBetweenStreaks = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 3;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 3 || this.CurrentSettings.Version == 4) {
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
                this.CurrentSettings.Version = 5;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 5 || this.CurrentSettings.Version == 6) {
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
                this.CurrentSettings.Version = 7;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 7) {
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
                this.CurrentSettings.Version = 8;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 8) {
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
                this.CurrentSettings.Version = 9;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 9) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.Equals("round_fall_mountain", StringComparison.OrdinalIgnoreCase)) {
                        info.Name = "round_fall_mountain_hub_complete";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 10;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 10) {
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
                this.CurrentSettings.Version = 11;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 11) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.AllStats.Sort();
                this.StatsDB.BeginTrans();
                int lastShow = -1;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (lastShow != info.ShowID) {
                        lastShow = info.ShowID;
                        info.IsFinal = this.StatLookup.TryGetValue(info.Name, out LevelStats stats)
                            ? stats.IsFinal && (info.Name != "round_floor_fall" || info.Round >= 3 || (i > 0 && this.AllStats[i - 1].Name != "round_floor_fall"))
                            : false;
                    } else {
                        info.IsFinal = false;
                    }

                    this.RoundDetails.Update(info);
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 12;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 12 || this.CurrentSettings.Version == 13) {
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
                this.CurrentSettings.Version = 14;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 14) {
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
                this.CurrentSettings.Version = 15;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 15) {
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
                this.CurrentSettings.Version = 16;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 16) {
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
                this.CurrentSettings.Version = 17;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 17) {
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
                this.CurrentSettings.Version = 18;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 18) {
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
                this.CurrentSettings.Version = 19;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 19) {
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
                this.CurrentSettings.Version = 20;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 20) {
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
                this.CurrentSettings.Version = 21;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 21) {
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
                this.CurrentSettings.Version = 22;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 22) {
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
                this.CurrentSettings.Version = 23;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 23) {
                this.CurrentSettings.OverlayColor = 0;
                this.CurrentSettings.GameExeLocation = string.Empty;
                this.CurrentSettings.GameShortcutLocation = string.Empty;
                this.CurrentSettings.AutoLaunchGameOnStartup = false;
                this.CurrentSettings.Version = 24;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 24) {
                this.CurrentSettings.WinsFilter = 1;
                this.CurrentSettings.QualifyFilter = 1;
                this.CurrentSettings.FastestFilter = 1;
                this.CurrentSettings.Version = 25;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 25) {
                this.CurrentSettings.OverlayBackground = 0;
                this.CurrentSettings.OverlayBackgroundResourceName = string.Empty;
                this.CurrentSettings.OverlayTabResourceName = string.Empty;
                this.CurrentSettings.IsOverlayBackgroundCustomized = false;
                this.CurrentSettings.OverlayFontColorSerialized = string.Empty;
                this.CurrentSettings.Version = 26;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 26) {
                this.CurrentSettings.OverlayBackgroundOpacity = 100;
                this.CurrentSettings.Version = 27;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 27) {
                this.CurrentSettings.PreventOverlayMouseClicks = false;
                this.CurrentSettings.Version = 28;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 28) {
                this.CurrentSettings.Visible = true;
                this.CurrentSettings.Version = 29;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 29) {
                this.CurrentSettings.SystemTrayIcon = true;
                this.CurrentSettings.Version = 30;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 30) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Name.Equals("wle_s10_user_creative_round")) {
                        info.Name = "wle_s10_user_creative_race_round";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 31;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 31) {
                this.CurrentSettings.OverlayColor = this.CurrentSettings.OverlayColor > 0 ? this.CurrentSettings.OverlayColor + 1 : this.CurrentSettings.OverlayColor;
                this.CurrentSettings.Version = 32;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 32) {
                this.CurrentSettings.FilterType += 1;
                this.CurrentSettings.SelectedCustomTemplateSeason = -1;
                this.CurrentSettings.CustomFilterRangeStart = DateTime.MinValue;
                this.CurrentSettings.CustomFilterRangeEnd = DateTime.MaxValue;
                this.CurrentSettings.Version = 33;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 33) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Name.Equals("round_bluejay_40")) {
                        info.Name = "round_bluejay";
                        this.RoundDetails.Update(info);
                    }
                }
                this.CurrentSettings.Version = 34;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 34) {
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
                this.CurrentSettings.Version = 35;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 35) {
                this.CurrentSettings.AutoUpdate = true;
                this.CurrentSettings.Version = 36;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 36) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Name.Equals("round_follow-the-leader_ss2_launch") || info.Name.Equals("round_follow-the-leader_ss2_parrot")) {
                        info.Name = "round_follow_the_line";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 37;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 37) {
                this.StatsDB.BeginTrans();
                this.AllProfiles.AddRange(this.Profiles.FindAll());
                for (int i = this.AllProfiles.Count - 1; i >= 0; i--) {
                    Profiles profiles = this.AllProfiles[i];
                    if (!string.IsNullOrEmpty(profiles.LinkedShowId) && profiles.LinkedShowId.Equals("event_only_survival_ss2_3009_0210_2022")) {
                        profiles.LinkedShowId = "survival_of_the_fittest";
                    }
                }
                this.Profiles.DeleteAll();
                this.Profiles.InsertBulk(this.AllProfiles);
                this.StatsDB.Commit();
                this.AllProfiles.Clear();
                this.CurrentSettings.Version = 38;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 38) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                this.CurrentSettings.NotifyServerConnected = false;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 39;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 39) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                this.CurrentSettings.NotifyServerConnected = false;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 40;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 40) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                this.CurrentSettings.NotifyServerConnected = false;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if ((!string.IsNullOrEmpty(info.ShowNameId) && info.ShowNameId.StartsWith("wle_mrs_bagel")) && info.Name.StartsWith("wle_mrs_bagel_final")) {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 41;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 41) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                this.CurrentSettings.NotifyServerConnected = false;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 42;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 42) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                this.CurrentSettings.NotifyServerConnected = false;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 43;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 43) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Name.Equals("wle_s10_user_creative_race_round")) {
                        info.Name = "user_creative_race_round";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 44;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 44) {
                this.CurrentSettings.ShowChangelog = true;
                this.CurrentSettings.Version = 45;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 45) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 46;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 46) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("show_wle_s10_wk") ||
                         info.ShowNameId.StartsWith("wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("show_wle_s10_player_round_wk") ||
                         info.ShowNameId.StartsWith("current_wle_fp")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 47;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 47) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) &&
                        ((info.ShowNameId.StartsWith("show_wle_s10_wk") || info.ShowNameId.StartsWith("event_wle_s10_wk")) && info.ShowNameId.EndsWith("_mrs")) &&
                        !this.IsFinalWithCreativeLevel(info.Name))
                    {
                        info.IsFinal = false;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.GroupingCreativeRoundLevels = true;
                this.CurrentSettings.Version = 48;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 48) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) &&
                        info.ShowNameId.Equals("main_show") &&
                        this.IsFinalWithCreativeLevel(info.Name))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 49;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 49) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.ShowNameId) && !info.IsFinal &&
                        (info.ShowNameId.StartsWith("wle_s10_cf_round_")))
                    {
                        info.IsFinal = true;
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 50;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 50) {
                this.CurrentSettings.EnableFallalyticsReporting = true;
                this.CurrentSettings.Version = 51;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 51) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (!string.IsNullOrEmpty(info.Name) && info.Name.Equals("current_wle_fp4_10_08") && info.Start < new DateTime(2023, 8, 22)) {
                        info.Name = "current_wle_fp4_10_08_m";
                        info.ShowNameId = "current_wle_fp4_10_08_m";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 52;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 52) {
                this.CurrentSettings.DisplayCurrentTime = true;
                this.CurrentSettings.Version = 53;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 53) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.IsFinal &&
                          ri.ShowNameId.Equals("survival_of_the_fittest") &&
                          ri.Name.Equals("round_kraken_attack") &&
                          ri.Round != 4
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    ri.IsFinal = false;
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 54;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 54) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.IsFinal &&
                          ri.ShowNameId.Equals("event_only_hexaring_template") &&
                          ri.Round < 3
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    ri.IsFinal = false;
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 55;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 55) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.ShowNameId.Equals("wle_mrs_shuffle_show")
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    ri.Name = ri.Name.StartsWith("mrs_wle_fp") ? $"current{ri.Name.Substring(3)}" : ri.Name.Substring(4);
                    ri.IsFinal = true;
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 56;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 56) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.IsFinal &&
                          ri.ShowNameId.Equals("event_only_thin_ice_template") &&
                          ri.Round < 3
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    ri.IsFinal = false;
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 57;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 57) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.IsFinal &&
                          ri.ShowNameId.Equals("event_only_hexaring_template") &&
                          ri.Round < 3
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    ri.IsFinal = false;
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 58;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 58) {
                this.CurrentSettings.DisplayGamePlayedInfo = true;
                this.CurrentSettings.Version = 59;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 59) {
                this.StatsDB.BeginTrans();
                List<RoundInfo> roundInfoList = (from ri in this.RoundDetails.FindAll()
                    where !string.IsNullOrEmpty(ri.ShowNameId) &&
                          ri.ShowNameId.Equals("wle_mrs_shuffle_show") &&
                          ri.Name.StartsWith("shuffle_halloween_")
                    select ri).ToList();
                foreach (RoundInfo ri in roundInfoList) {
                    if (this.LevelIdReplacerInDigisShuffleShow.TryGetValue($"wle_{ri.Name}", out string newName)) {
                        ri.Name = newName;
                    }
                }
                this.RoundDetails.Update(roundInfoList);
                this.StatsDB.Commit();
                this.CurrentSettings.Version = 60;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 60) {
                this.StatsDB.DropCollection("FallalyticsPbInfo");
                this.CurrentSettings.NotifyServerConnected = true;
                this.CurrentSettings.MuteNotificationSounds = false;
                this.CurrentSettings.Version = 61;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 61) {
                this.CurrentSettings.NotifyServerConnected = true;
                this.CurrentSettings.MuteNotificationSounds = false;
                this.CurrentSettings.NotificationSounds = 0;
                this.CurrentSettings.Version = 62;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 62) {
                this.StatsDB.BeginTrans();
                List<Profiles> profileList = (from p in this.Profiles.FindAll()
                    where string.IsNullOrEmpty(p.ProfileName)
                    select p).ToList();
                foreach (Profiles p in profileList) {
                    p.ProfileName = Utils.ComputeHash(BitConverter.GetBytes(DateTime.Now.Ticks), HashTypes.MD5).Substring(0, 20);
                }
                this.Profiles.Update(profileList);
                this.StatsDB.Commit();
                this.CurrentSettings.NotificationSounds = 0;
                this.CurrentSettings.NotificationWindowPosition = 0;
                this.CurrentSettings.NotificationWindowAnimation = 1;
                this.CurrentSettings.Version = 63;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 63) {
                this.CurrentSettings.RecordEscapeDuringAGame = true;
                this.CurrentSettings.Version = 64;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 64) {
                this.CurrentSettings.NotifyPersonalBest = true;
                this.CurrentSettings.Version = 65;
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
                FlippedDisplay = false,
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
                EnableFallalyticsAnonymous = false,
                ShowChangelog = true,
                Visible = true,
                Version = 0
            };
        }
        
        private bool IsFinalWithCreativeLevel(string levelId) {
            return levelId.Equals("wle_s10_orig_round_010") ||
                   levelId.Equals("wle_s10_orig_round_011") ||
                   levelId.Equals("wle_s10_orig_round_017") ||
                   levelId.Equals("wle_s10_orig_round_018") ||
                   levelId.Equals("wle_s10_orig_round_024") ||
                   levelId.Equals("wle_s10_orig_round_025") ||
                   levelId.Equals("wle_s10_orig_round_030") ||
                   levelId.Equals("wle_s10_orig_round_031") ||
                   levelId.Equals("wle_s10_round_004") ||
                   levelId.Equals("wle_s10_round_009");
        }
        
        private void UpdateHoopsieLegends() {
            LevelStats level = this.StatLookup["round_hoops_blockade_solo"];
            string newName = this.CurrentSettings.HoopsieHeros ? Multilingual.GetWord("main_hoopsie_heroes") : Multilingual.GetWord("main_hoopsie_legends");
            if (level.Name != newName) {
                level.Name = newName;
            }
        }
        
        private bool IsCreativeLevel(string levelId) {
            return levelId.StartsWith("wle_s10_round_") || levelId.StartsWith("wle_s10_orig_round_")
                   || levelId.StartsWith("wle_mrs_bagel_") || levelId.StartsWith("wle_s10_bt_round_")
                   || levelId.StartsWith("current_wle_fp") || levelId.StartsWith("wle_s10_player_round_wk")
                   || levelId.StartsWith("wle_s10_cf_round_") || levelId.StartsWith("wle_s10_long_round_")
                   || levelId.Equals("wle_fp2_wk6_01");
        }
        
        private void UpdateGridRoundName() {
            foreach (KeyValuePair<string, string> item in Multilingual.GetRoundsDictionary()) {
                if (this.IsCreativeLevel(item.Key)) { continue; }
                LevelStats level = this.StatLookup[item.Key];
                level.Name = item.Value;
            }
            this.SortGridDetails(true);
            this.gridDetails.Invalidate();
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
            int profile = this.currentProfile;

            lock (this.StatsDB) {
                this.AllStats.Clear();
                this.nextShowID = 0;
                this.lastAddedShow = DateTime.MinValue;
                if (this.RoundDetails.Count() > 0) {
                    this.AllStats.AddRange(this.RoundDetails.FindAll());
                    this.AllStats.Sort();

                    if (this.AllStats.Count > 0) {
                        this.nextShowID = this.AllStats[this.AllStats.Count - 1].ShowID;

                        int lastAddedShowID = -1;
                        for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                            RoundInfo info = this.AllStats[i];
                            info.ToLocalTime();
                            if (info.Profile != profile) { continue; }

                            if (info.ShowID == lastAddedShowID || (IsInStatsFilter(info) && IsInPartyFilter(info))) {
                                lastAddedShowID = info.ShowID;
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
                    if (info.Profile != profile) { continue; }

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
        
        private void menuLookHere_Click(object sender, EventArgs e) {
            try {
                if (sender.Equals(this.menuFallGuysWiki) || sender.Equals(this.trayFallGuysWiki)) {
                    Process.Start(@"https://fallguysultimateknockout.fandom.com/wiki/Fall_Guys:_Ultimate_Knockout_Wiki");
                } else if (sender.Equals(this.menuFallGuysReddit) || sender.Equals(this.trayFallGuysReddit)) {
                    Process.Start(@"https://www.reddit.com/r/FallGuysGame/");
                } else if (sender.Equals(this.menuFallalytics) || sender.Equals(this.trayFallalytics)) {
                    Process.Start(@"https://fallalytics.com/");
                } else if (sender.Equals(this.menuRollOffClub) || sender.Equals(this.trayRollOffClub)) {
                    if (CurrentLanguage == Language.Korean) {
                        Process.Start(@"https://rolloff.club/ko/");
                    } else if (CurrentLanguage == Language.SimplifiedChinese) {
                        Process.Start(@"https://rolloff.club/zh/");
                    } else {
                        Process.Start(@"https://rolloff.club/");
                    }
                } else if (sender.Equals(this.menuLostTempleAnalyzer) || sender.Equals(this.trayLostTempleAnalyzer)) {
                    Process.Start(@"https://alexjlockwood.github.io/lost-temple-analyzer/");
                } else if (sender.Equals(this.menuFallGuysDB) || sender.Equals(this.trayFallGuysDB)) {
                    Process.Start(@"https://fallguys-db.pages.dev/upcoming_shows");
                } else if (sender.Equals(this.menuFallGuysOfficial) || sender.Equals(this.trayFallGuysOfficial)) {
                    Process.Start(@"https://fallguys.com/");
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void menuLookHere_MouseEnter(object sender, EventArgs e) {
            Rectangle rectangle = this.menuLookHere.Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 260);
            this.AllocCustomTooltip(this.cmtt_center_Draw);
            if (sender.Equals(this.menuFallGuysWiki)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_wiki_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallGuysReddit)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_reddit_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallalytics)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fallalytics_tooltip"), this, position);
            } else if (sender.Equals(this.menuRollOffClub)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_roll_off_club_tooltip"), this, position);
            } else if (sender.Equals(this.menuLostTempleAnalyzer)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_lost_temple_analyzer_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallGuysDB)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_todays_show_tooltip"), this, position);
            } else if (sender.Equals(this.menuFallGuysOfficial)) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_official_tooltip"), this, position);
            }
        }
        
        private void menuLookHere_MouseLeave(object sender, EventArgs e) {
            this.HideCustomTooltip(this);
            this.Cursor = Cursors.Default;
        }

        private void menuUpdate_MouseEnter(object sender, EventArgs e) {
            Rectangle rectangle = ((ToolStripMenuItem)sender).Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 68);
            this.AllocTooltip();
            this.ShowTooltip("menuUpdate".Equals(((ToolStripMenuItem)sender).Name) && this.isAvailableNewVersion ? $"{Multilingual.GetWord("main_you_can_update_new_version_prefix_tooltip")}v{this.availableNewVersion}{Multilingual.GetWord("main_you_can_update_new_version_suffix_tooltip")}" :
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
                    Utils.SetForegroundWindow(Utils.FindWindow(null, this.mainWndTitle));
                } else if (this.Visible && this.WindowState != FormWindowState.Minimized) {
                    if (this.isFocused) {
                        this.isFocused = false;
                        this.Hide();
                        //Utils.SetForegroundWindow(Utils.FindWindow(null, "Fall Guys Stats Overlay"));
                    } else {
                        this.isFocused = true;
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
        
        private void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.isFormClosing || !this.CurrentSettings.SystemTrayIcon) {
                try {
                    if (!this.isUpdate && !this.overlay.Disposing && !this.overlay.IsDisposed && !this.IsDisposed && !this.Disposing) {
                        this.SaveWindowState();
                        this.SaveUserSettings();
                    }
                    this.StatsDB?.Dispose();
                } catch (Exception ex) {
                    MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                e.Cancel = true;
                this.Hide();
            }
        }
        
        private void Stats_Load(object sender, EventArgs e) {
            try {
                this.SetTheme(CurrentTheme);
                
                this.infoStrip.Renderer = new CustomToolStripSystemRenderer();
                this.infoStrip2.Renderer = new CustomToolStripSystemRenderer();
                Utils.DwmSetWindowAttribute(this.menu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.menuFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.menuStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.menuPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.menuProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.menuLookHere.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayCMenu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
                Utils.DwmSetWindowAttribute(this.trayLookHere.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));

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
                            string changelog = Utils.GetApiData(Utils.GITHUB_API_URL, "repos/ShootMe/FallGuysStats/releases/latest").GetProperty("body").GetString();
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
                if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                    logFilePath = this.CurrentSettings.LogPath;
                }
                this.logFile.Start(logFilePath, LOGFILENAME);

                this.overlay.ArrangeDisplay(string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.FlippedDisplay : this.CurrentSettings.FixedFlippedDisplay, this.CurrentSettings.ShowOverlayTabs,
                    this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo,
                    this.CurrentSettings.OverlayColor, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayWidth : this.CurrentSettings.OverlayFixedWidth, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayHeight : this.CurrentSettings.OverlayFixedHeight,
                    this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
                if (this.CurrentSettings.OverlayVisible) { this.ToggleOverlay(this.overlay); }
                
                this.ReloadProfileMenuItems();
                // this.menuStats_Click(this.menuProfile.DropDownItems[$@"menuProfile{this.CurrentSettings.SelectedProfile}"], EventArgs.Empty);
                
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

        private void LogFile_OnPersonalBestNotification(RoundInfo info, TimeSpan record, TimeSpan currentRecord) {
            string timeDiffContent = String.Empty;
            if (record != TimeSpan.MaxValue) {
                TimeSpan timeDiff = record - currentRecord;
                timeDiffContent = timeDiff.Minutes > 0 ? $" ⏱️{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_prefix")}{timeDiff.Minutes}{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_infix")} {timeDiff.Seconds}.{timeDiff.Milliseconds}{Multilingual.GetWord("message_new_personal_best_timediff_by_minute_suffix")}"
                    : $" ⏱️{timeDiff.Seconds}.{timeDiff.Milliseconds}{Multilingual.GetWord("message_new_personal_best_timediff_by_second")}";
            }
            string showName = $" {(Multilingual.GetShowName(this.GetAlternateShowId(info.ShowNameId)).Equals(Multilingual.GetRoundName(info.Name)) ? $"({Multilingual.GetRoundName(info.Name)})" : $"({Multilingual.GetShowName(this.GetAlternateShowId(info.ShowNameId))} • {Multilingual.GetRoundName(info.Name)})")}";
            string description = $"{Multilingual.GetWord("message_new_personal_best_prefix")}{showName}{Multilingual.GetWord("message_new_personal_best_suffix")}{timeDiffContent}";
            ToastPosition toastPosition = this.CurrentSettings.NotificationWindowPosition == 0 ? ToastPosition.BottomRight : ToastPosition.TopRight;
            ToastTheme toastTheme = this.Theme == MetroThemeStyle.Light ? ToastTheme.Light : ToastTheme.Dark;
            ToastAnimation toastAnimation = this.CurrentSettings.NotificationWindowAnimation == 0 ? ToastAnimation.FADE : ToastAnimation.SLIDE;
            ToastSound toastSound;
            switch (this.CurrentSettings.NotificationSounds) {
                case 1: toastSound = ToastSound.Generic02; break;
                case 2: toastSound = ToastSound.Generic03; break;
                case 3: toastSound = ToastSound.Generic04; break;
                default: toastSound = ToastSound.Generic01; break;
            }
            this.ShowToastNotification(this, Properties.Resources.main_120_icon, Multilingual.GetWord("message_new_personal_best_caption"), description, Overlay.GetMainFont(16, FontStyle.Bold, CurrentLanguage),
                null, ToastDuration.MEDIUM, toastPosition, toastAnimation, toastTheme, toastSound, this.CurrentSettings.MuteNotificationSounds, true);
        }

        private void LogFile_OnServerConnectionNotification() {
            string countryFullName;
            if (!string.IsNullOrEmpty(LastCountryAlpha2Code)) {
                countryFullName = Multilingual.GetCountryName(LastCountryAlpha2Code);
                if (!string.IsNullOrEmpty(LastCountryRegion)) {
                    countryFullName += $" ({LastCountryRegion}";
                    if (!string.IsNullOrEmpty(LastCountryCity)) {

                        if (!LastCountryCity.Equals(LastCountryRegion)) {
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
            ToastPosition toastPosition = this.CurrentSettings.NotificationWindowPosition == 0 ? ToastPosition.BottomRight : ToastPosition.TopRight;
            ToastTheme toastTheme = this.Theme == MetroThemeStyle.Light ? ToastTheme.Light : ToastTheme.Dark;
            ToastAnimation toastAnimation = this.CurrentSettings.NotificationWindowAnimation == 0 ? ToastAnimation.FADE : ToastAnimation.SLIDE;
            ToastSound toastSound;
            switch (this.CurrentSettings.NotificationSounds) {
                case 1: toastSound = ToastSound.Generic02; break;
                case 2: toastSound = ToastSound.Generic03; break;
                case 3: toastSound = ToastSound.Generic04; break;
                default: toastSound = ToastSound.Generic01; break;
            }
            this.ShowToastNotification(this, Properties.Resources.main_120_icon, Multilingual.GetWord("message_connected_to_server_caption"), description, Overlay.GetMainFont(16, FontStyle.Bold, CurrentLanguage),
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
                    for (int k = 0; k < round.Count; k++) {
                        RoundInfo stat = round[k];

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
                                        //editShows.Icon = this.Icon;
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
                                                //this.ReloadProfileMenuItems();
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
                                    profile = this.GetLinkedProfileId(stat.ShowNameId, stat.PrivateLobby, this.IsCreativeShow(stat.ShowNameId));
                                    this.CurrentSettings.SelectedProfile = profile;
                                    //this.ReloadProfileMenuItems();
                                    this.SetProfileMenu(profile);
                                }

                                if (stat.Round == 1) {
                                    this.nextShowID++;
                                    this.lastAddedShow = stat.Start;
                                }
                                stat.ShowID = this.nextShowID;
                                stat.Profile = profile;

                                if (stat.UseShareCode && string.IsNullOrEmpty(stat.CreativeShareCode) && Utils.IsInternetConnected()) {
                                    bool isSucceed = false;
                                    try {
                                        JsonElement resData = Utils.GetApiData(Utils.FALLGUYSDB_API_URL, $"creative/{stat.ShowNameId}.json");
                                        if (resData.TryGetProperty("data", out JsonElement je)) {
                                            JsonElement snapshot = je.GetProperty("snapshot");
                                            JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                                            string[] onlinePlatformInfo = this.FindCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                                            stat.CreativeShareCode = snapshot.GetProperty("share_code").GetString();
                                            stat.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                            stat.CreativeAuthor = onlinePlatformInfo[1];
                                            stat.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                            stat.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                            stat.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                            stat.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                            stat.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                            stat.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                            stat.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                            stat.CreativePlayCount = snapshot.GetProperty("play_count").GetInt32();
                                            stat.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                            //stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").GetProperty("time_limit_seconds").GetInt32();
                                            stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                            isSucceed = true;
                                        }
                                    } catch {
                                        isSucceed = false;
                                    }
                                    
                                    if (!isSucceed) {
                                        RoundInfo ri = this.GetRoundInfoFromShareCode(stat.ShowNameId);
                                        if (ri != null && !string.IsNullOrEmpty(ri.CreativeTitle)) {
                                            stat.CreativeOnlinePlatformId = ri.CreativePlatformId;
                                            stat.CreativeAuthor = ri.CreativeAuthor;
                                            stat.CreativeShareCode = ri.CreativeShareCode;
                                            stat.CreativeVersion = ri.CreativeVersion;
                                            stat.CreativeStatus = ri.CreativeStatus;
                                            stat.CreativeTitle = ri.CreativeTitle;
                                            stat.CreativeDescription = ri.CreativeDescription;
                                            stat.CreativeMaxPlayer = ri.CreativeMaxPlayer;
                                            stat.CreativePlatformId = ri.CreativePlatformId;
                                            stat.CreativeLastModifiedDate = ri.CreativeLastModifiedDate;
                                            stat.CreativePlayCount = ri.CreativePlayCount;
                                            stat.CreativeQualificationPercent = ri.CreativeQualificationPercent;
                                            stat.CreativeTimeLimitSeconds = ri.CreativeTimeLimitSeconds;
                                        } else {
                                            stat.CreativeShareCode = null;
                                            stat.CreativeOnlinePlatformId = null;
                                            stat.CreativeAuthor = null;
                                            stat.CreativeVersion = 0;
                                            stat.CreativeStatus = null;
                                            stat.CreativeTitle = null;
                                            stat.CreativeDescription = null;
                                            stat.CreativeMaxPlayer = 0;
                                            stat.CreativePlatformId = null;
                                            stat.CreativeLastModifiedDate = DateTime.MinValue;
                                            stat.CreativePlayCount = 0;
                                            stat.CreativeQualificationPercent = 0;
                                            stat.CreativeTimeLimitSeconds = 0;
                                        }
                                    }
                                }

                                this.RoundDetails.Insert(stat);
                                this.AllStats.Add(stat);
                                
                                // Below is where reporting to fallaytics happen
                                // Must have enabled the setting to enable tracking
                                // Must not be a private lobby
                                // Must be a game that is played after FallGuysStats started
                                if (this.CurrentSettings.EnableFallalyticsReporting && !stat.PrivateLobby && stat.ShowEnd > this.startupTime) {
                                    Task.Run(() => FallalyticsReporter.Report(stat, this.CurrentSettings.FallalyticsAPIKey));
                                    
                                    if (OnlineServiceType != OnlineServiceTypes.None && stat.Qualified && stat.Finish.HasValue &&
                                        (LevelStats.ALL.TryGetValue(stat.Name, out LevelStats level) && level.Type == LevelType.Race)) {
                                        string apiKey = Environment.GetEnvironmentVariable("FALLALYTICS_KEY");
                                        if (!string.IsNullOrEmpty(apiKey)) { Task.Run(() => this.FallalyticsRegisterPb(stat, apiKey)); }
                                    }
                                }
                            } else {
                                continue;
                            }
                        }

                        if (!stat.PrivateLobby) {
                            if (stat.Round == 1) {
                                this.Shows++;
                            }
                            this.Rounds++;
                        } else {
                            if (stat.Round == 1) {
                                this.CustomShows++;
                            }
                            this.CustomRounds++;
                        }
                        this.Duration += stat.End - stat.Start;

                        if (!stat.PrivateLobby) {
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
                        } else {
                            if (stat.Qualified) {
                                switch (stat.Tier) {
                                    case 0:
                                        this.CustomPinkMedals++;
                                        break;
                                    case 1:
                                        this.CustomGoldMedals++;
                                        break;
                                    case 2:
                                        this.CustomSilverMedals++;
                                        break;
                                    case 3:
                                        this.CustomBronzeMedals++;
                                        break;
                                }
                            } else {
                                this.CustomEliminatedMedals++;
                            }
                        }

                        this.Kudos += stat.Kudos;

                        // add new type of round to the rounds lookup
                        if (!this.StatLookup.ContainsKey(stat.Name)) {
                            string roundName = stat.Name;
                            if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                                roundName = roundName.Substring(6).Replace('_', ' ');
                            }

                            LevelStats newLevel = new LevelStats(stat.Name, this.textInfo.ToTitleCase(roundName), LevelType.Unknown, BestRecordType.Fastest, false, false, 0, 0, 0, Properties.Resources.round_unknown_icon, Properties.Resources.round_unknown_big_icon);
                            this.StatLookup.Add(stat.Name, newLevel);
                            this.StatDetails.Add(newLevel);
                            this.gridDetails.DataSource = null;
                            // this.gridDetails.DataSource = this.StatDetails;
                            this.gridDetails.DataSource = this.GetFilteredDataSource(this.CurrentSettings.GroupingCreativeRoundLevels);
                        }

                        stat.ToLocalTime();
                        
                        if (!stat.PrivateLobby) {
                            if (stat.IsFinal || stat.Crown) {
                                this.Finals++;
                                if (stat.Qualified) {
                                    this.Wins++;
                                }
                            }
                        }
                        
                        LevelStats levelStats = this.StatLookup[stat.Name];
                        levelStats.Increase(stat, this.profileIdWithLinkedCustomShow == stat.Profile);
                        levelStats.Add(stat);

                        if (levelStats.IsCreative && levelStats.Type == LevelType.Race) {
                            LevelStats creativeLevel = this.StatLookup[levelStats.IsFinal ? "creative_race_final_round" : "creative_race_round"];
                            creativeLevel.Increase(stat, this.profileIdWithLinkedCustomShow == stat.Profile);
                            creativeLevel.Add(stat);
                        }
                    }

                    if (!this.loadingExisting) { this.StatsDB.Commit(); }
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
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public bool ExistsPersonalBestLog(string sessionId, string showId, string roundId) {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(showId)) return false;
            BsonExpression condition = Query.And(
                Query.EQ("_id", sessionId),
                Query.EQ("ShowId", showId),
                Query.EQ("RoundId", roundId)
            );
            return this.PersonalBestLog.Exists(condition);
        }
        
        public PersonalBestLog SelectPersonalBestLog(string sessionId, string showId, string roundId) {
            BsonExpression condition = Query.And(
                Query.EQ("_id", sessionId),
                Query.EQ("ShowId", showId),
                Query.EQ("RoundId", roundId)
            );
            return this.PersonalBestLog.FindOne(condition);
        }
        
        public void UpsertPersonalBestLog(string sessionId, string showId, string roundId, double record, DateTime finish, bool isPb) {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                this.PersonalBestLog.Upsert(new PersonalBestLog { SessionId = sessionId, ShowId = showId, RoundId = roundId, Record = record, PbDate = finish, IsPb = isPb,
                    CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname
                });
                this.StatsDB.Commit();
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

        private void ClearPersonalBestLog() {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                DateTime fiveDaysAgo = DateTime.Now.AddDays(-5);
                BsonExpression condition = Query.LT("PbDate", fiveDaysAgo);
                this.PersonalBestLog.DeleteMany(condition);
                this.StatsDB.Commit();
            }
        }

        public bool ExistsServerConnectionLog(string sessionId, string showId) {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(showId)) return false;
            BsonExpression condition = Query.And(
                Query.EQ("_id", sessionId),
                Query.EQ("ShowId", showId)
            );
            return this.ServerConnectionLog.Exists(condition);
        }
        
        public ServerConnectionLog SelectServerConnectionLog(string sessionId, string showId) {
            BsonExpression condition = Query.And(
                Query.EQ("_id", sessionId),
                Query.EQ("ShowId", showId)
            );
            return this.ServerConnectionLog.FindOne(condition);
        }
        
        public void UpsertServerConnectionLog(string sessionId, string showId, string serverIp, DateTime connectionDate, bool isNotify, bool isPlaying) {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                this.ServerConnectionLog.Upsert(new ServerConnectionLog { SessionId = sessionId, ShowId = showId, ServerIp = serverIp, ConnectionDate = connectionDate,
                    CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType, OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
                    IsNotify = isNotify, IsPlaying = isPlaying
                });
                this.StatsDB.Commit();
            }
        }
        
        public void UpdateServerConnectionLog(string sessionId, string showId, bool isPlaying) {
            lock (this.StatsDB) {
                ServerConnectionLog serverConnectionLog = this.SelectServerConnectionLog(sessionId, showId);
                if (serverConnectionLog != null) {
                    this.StatsDB.BeginTrans();
                    serverConnectionLog.IsPlaying = isPlaying;
                    this.ServerConnectionLog.Update(serverConnectionLog);
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

        private void ClearServerConnectionLog() {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                DateTime fiveDaysAgo = DateTime.Now.AddDays(-5);
                BsonExpression condition = Query.LT("ConnectionDate", fiveDaysAgo);
                this.ServerConnectionLog.DeleteMany(condition);
                this.StatsDB.Commit();
            }
        }

        private async Task FallalyticsRegisterPb(RoundInfo stat, string apiKey) {
            if (string.IsNullOrEmpty(OnlineServiceId) || string.IsNullOrEmpty(OnlineServiceNickname)) {
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
            }

            if (string.IsNullOrEmpty(OnlineServiceId) || string.IsNullOrEmpty(OnlineServiceNickname)) return;
            
            if (string.IsNullOrEmpty(HostCountryCode)) {
                HostCountryCode = Utils.GetCountryInfoByIp(Utils.GetUserPublicIp(), true).Split(';')[0];
            }

            string currentSessionId = stat.SessionId;
            string currentShowNameId = stat.ShowNameId;
            string currentRoundId = stat.Name;
            TimeSpan currentRecord = stat.Finish.Value - stat.Start;
            DateTime currentFinish = stat.Finish.Value;
            bool isTransferSuccess = false;
            if (!this.FallalyticsPbLog.Exists(Query.And(Query.EQ("RoundId", stat.Name)))) {
                List<RoundInfo> existingRecords = this.RoundDetails.Find(Query.And(
                    Query.EQ("PrivateLobby", false)
                    , Query.EQ("Name", stat.Name)
                    , Query.Not("Finish", null)
                )).ToList();
                RoundInfo recordInfo = existingRecords.OrderBy(r => r.Finish.Value - r.Start).FirstOrDefault();
            
                if (recordInfo != null && currentRecord > recordInfo.Finish.Value - recordInfo.Start) {
                    currentSessionId = recordInfo.SessionId;
                    currentShowNameId = recordInfo.ShowNameId;
                    currentRoundId = recordInfo.Name;
                    currentRecord = recordInfo.Finish.Value - recordInfo.Start;
                    currentFinish = recordInfo.Finish.Value;
                }

                try {
                    if (Utils.IsEndpointValid(FallalyticsReporter.RegisterPbAPIEndpoint)) {
                        await FallalyticsReporter.RegisterPb(new RoundInfo { SessionId = currentSessionId, Name = currentRoundId, ShowNameId = currentShowNameId }, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous, apiKey);
                        isTransferSuccess = true;
                    }
                } catch {
                    isTransferSuccess = false;
                }
                
                lock (this.StatsDB) {
                    this.StatsDB.BeginTrans();
                    this.FallalyticsPbLog.Insert(new FallalyticsPbLog {
                        SessionId = currentSessionId, RoundId = currentRoundId, ShowId = currentShowNameId,
                        Record = currentRecord.TotalMilliseconds, PbDate = currentFinish,
                        CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType,
                        OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
                        IsTransferSuccess = isTransferSuccess
                    });
                    this.StatsDB.Commit();
                }
            } else {
                FallalyticsPbLog pbLog = this.FallalyticsPbLog.FindOne(Query.And(
                    Query.EQ("RoundId", stat.Name)
                    , Query.EQ("OnlineServiceType", (int)OnlineServiceType)
                    , Query.EQ("OnlineServiceId", OnlineServiceId)
                ));
                
                if (pbLog != null) {
                    TimeSpan existingRecord = TimeSpan.FromMilliseconds(pbLog.Record);
                    if (pbLog.IsTransferSuccess) {
                        if (currentRecord < existingRecord) {
                            try {
                                if (Utils.IsEndpointValid(FallalyticsReporter.RegisterPbAPIEndpoint)) {
                                    await FallalyticsReporter.RegisterPb(stat, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous, apiKey);
                                    isTransferSuccess = true;
                                }
                            } catch {
                                isTransferSuccess = false;
                            }
                            
                            lock (this.StatsDB) {
                                this.StatsDB.BeginTrans();
                                pbLog.Record = currentRecord.TotalMilliseconds;
                                pbLog.PbDate = currentFinish;
                                pbLog.IsTransferSuccess = isTransferSuccess;
                                this.FallalyticsPbLog.Update(pbLog);
                                this.StatsDB.Commit();
                            }
                        }
                    } else {
                        double record = currentRecord < existingRecord ? currentRecord.TotalMilliseconds : existingRecord.TotalMilliseconds;
                        DateTime finish = currentRecord < existingRecord ? currentFinish : pbLog.PbDate;
                        try {
                            if (Utils.IsEndpointValid(FallalyticsReporter.RegisterPbAPIEndpoint)) {
                                await FallalyticsReporter.RegisterPb(stat, record, finish, this.CurrentSettings.EnableFallalyticsAnonymous, apiKey);
                                isTransferSuccess = true;
                            }
                        } catch {
                            isTransferSuccess = false;
                        }
                        
                        lock (this.StatsDB) {
                            this.StatsDB.BeginTrans();
                            pbLog.Record = record;
                            pbLog.PbDate = finish;
                            pbLog.IsTransferSuccess = isTransferSuccess;
                            this.FallalyticsPbLog.Update(pbLog);
                            this.StatsDB.Commit();
                        }
                    }
                } else {
                    try {
                        if (Utils.IsEndpointValid(FallalyticsReporter.RegisterPbAPIEndpoint)) {
                            await FallalyticsReporter.RegisterPb(stat, currentRecord.TotalMilliseconds, currentFinish, this.CurrentSettings.EnableFallalyticsAnonymous, apiKey);
                            isTransferSuccess = true;
                        }
                    } catch {
                        isTransferSuccess = false;
                    }
                    
                    lock (this.StatsDB) {
                        this.StatsDB.BeginTrans();
                        this.FallalyticsPbLog.Insert(new FallalyticsPbLog {
                            SessionId = currentSessionId, RoundId = currentRoundId, ShowId = currentShowNameId,
                            Record = currentRecord.TotalMilliseconds, PbDate = currentFinish,
                            CountryCode = HostCountryCode, OnlineServiceType = (int)OnlineServiceType,
                            OnlineServiceId = OnlineServiceId, OnlineServiceNickname = OnlineServiceNickname,
                            IsTransferSuccess = isTransferSuccess
                        });
                        this.StatsDB.Commit();
                    }
                }   
            }
        }
        
        public bool IsCreativeShow(string showId) {
            return showId.StartsWith("show_wle_s10_") ||
                   showId.StartsWith("event_wle_s10_") ||
                   showId.IndexOf("wle_s10_player_round_wk", StringComparison.OrdinalIgnoreCase) != -1 ||
                   showId.Equals("wle_mrs_bagel") ||
                   showId.Equals("wle_mrs_shuffle_show") ||
                   showId.StartsWith("current_wle_fp") ||
                   showId.StartsWith("wle_s10_cf_round_");
        }
        
        private bool IsInStatsFilter(RoundInfo info) {
            return (this.menuCustomRangeStats.Checked && info.Start >= this.customfilterRangeStart && info.Start <= this.customfilterRangeEnd) ||
                    this.menuAllStats.Checked ||
                   (this.menuSeasonStats.Checked && info.Start > SeasonStart) ||
                   (this.menuWeekStats.Checked && info.Start > WeekStart) ||
                   (this.menuDayStats.Checked && info.Start > DayStart) ||
                   (this.menuSessionStats.Checked && info.Start > SessionStart);
        }
        
        private bool IsInPartyFilter(RoundInfo info) {
            return this.menuAllPartyStats.Checked ||
                   (this.menuSoloStats.Checked && !info.InParty) ||
                   (this.menuPartyStats.Checked && info.InParty);
        }
        
        public string GetCurrentFilterName() {
            if (this.menuCustomRangeStats.Checked && this.selectedCustomTemplateSeason > -1) {
                return (this.selectedCustomTemplateSeason >= 0 && this.selectedCustomTemplateSeason <= 5) ? $"S{this.selectedCustomTemplateSeason + 1}" :
                        (this.selectedCustomTemplateSeason > 5) ? $"SS{this.selectedCustomTemplateSeason - 5}" :
                        Multilingual.GetWord("main_custom_range");
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
            return this.AllProfiles.Find(p => p.ProfileName.Equals(profileName)).ProfileId;
        }
        
        private string GetCurrentProfileLinkedShowId() {
            if (this.AllProfiles.Count == 0) return String.Empty;
            string currentProfileLinkedShowId = this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).LinkedShowId;
            return currentProfileLinkedShowId ?? string.Empty;
        }
        
        private string GetAlternateShowId(string showId) {
            switch (showId) {
                case "turbo_show": return "main_show";
                case "squadcelebration":
                case "event_day_at_races_squads_template":
                    return "squads_4player";
                case "invisibeans_template":
                case "invisibeans_pistachio_template":
                    return "invisibeans_mode";
            }
            return showId;
        }
        
        private int GetLinkedProfileId(string showId, bool isPrivateLobbies, bool isCreativeShow) {
            if (this.AllProfiles.Count == 0 || string.IsNullOrEmpty(showId)) return 0;
            showId = this.GetAlternateShowId(showId);
            foreach (Profiles profiles in this.AllProfiles) {
                if (isPrivateLobbies) {
                    if (!string.IsNullOrEmpty(profiles.LinkedShowId) && profiles.LinkedShowId.Equals("private_lobbies")) {
                        return profiles.ProfileId;
                    }
                } else {
                    if (isCreativeShow) {
                        if (!string.IsNullOrEmpty(profiles.LinkedShowId) && profiles.LinkedShowId.Equals("fall_guys_creative_mode")) {
                            return profiles.ProfileId;
                        }
                    } else {
                        if (!string.IsNullOrEmpty(profiles.LinkedShowId) && showId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                            return profiles.ProfileId;
                        }
                    }
                }
            }
            if (isPrivateLobbies) {
                // return corresponding linked profile when possible if no linked "private_lobbies" profile was found
                return (from profiles in this.AllProfiles where !string.IsNullOrEmpty(profiles.LinkedShowId) && showId.IndexOf(profiles.LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1 select profiles.ProfileId).FirstOrDefault();
            }
            // return ProfileId 0 if no linked profile was found/matched
            return 0;
        }
        
        public void SetLinkedProfileMenu(string showId, bool isPrivateLobbies, bool isCreativeShow) {
            if (this.AllProfiles.Count == 0 || string.IsNullOrEmpty(showId)) return;
            showId = this.GetAlternateShowId(showId);
            if (this.GetCurrentProfileLinkedShowId().Equals(showId)) return;
            this.BeginInvoke((MethodInvoker)delegate {
                for (int i = 0; i < this.AllProfiles.Count; i++) {
                    if (isPrivateLobbies) {
                        if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && this.AllProfiles[i].LinkedShowId.Equals("private_lobbies")) {
                            ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                            if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                            return;
                        }
                    } else {
                        if (isCreativeShow) {
                            if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && this.AllProfiles[i].LinkedShowId.Equals("fall_guys_creative_mode")) {
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
                    for (int j = 0; j < this.AllProfiles.Count; j++) {
                        if (string.IsNullOrEmpty(this.AllProfiles[j].LinkedShowId) || showId.IndexOf(this.AllProfiles[j].LinkedShowId, StringComparison.OrdinalIgnoreCase) == -1) {
                            continue;
                        }

                        ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - j];
                        if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                        return;
                    }
                }
                // select ProfileId 0 if no linked profile was found/matched
                for (int k = 0; k < this.AllProfiles.Count; k++) {
                    if (this.AllProfiles[k].ProfileId != 0) { continue; }

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
                if (tsmi.Checked) { return; }

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
        
        // public string GetRoundNameFromShareCode(string shareCode, LevelType levelType) {
        //     RoundInfo filteredInfo = this.AllStats.FindLast(r => levelType.CreativeLevelTypeId().Equals(r.Name) && shareCode.Equals(r.ShowNameId) && !string.IsNullOrEmpty(r.CreativeTitle));
        //     return filteredInfo != null ? (string.IsNullOrEmpty(filteredInfo.CreativeTitle) ? shareCode : filteredInfo.CreativeTitle) : shareCode;
        // }
        
        // public int GetTimeLimitSecondsFromShareCode(string shareCode, LevelType levelType) {
        //     RoundInfo filteredInfo = this.AllStats.FindLast(r => levelType.CreativeLevelTypeId().Equals(r.Name) && shareCode.Equals(r.ShowNameId) && !string.IsNullOrEmpty(r.CreativeTitle));
        //     return filteredInfo?.CreativeTimeLimitSeconds ?? 0;
        // }
        
        public RoundInfo GetRoundInfoFromShareCode(string shareCode) {
            return this.AllStats.FindLast(r => shareCode.Equals(r.ShowNameId) && !string.IsNullOrEmpty(r.CreativeTitle));
        }
        
        public void UpdateUserCreativeLevel(string shareCode, JsonElement snapshot) {
            List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => shareCode.Equals(r.ShowNameId) && (string.IsNullOrEmpty(r.CreativeTitle) || string.IsNullOrEmpty(r.CreativeShareCode)));
            if (filteredInfo.Count <= 0) { return; }

            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                string[] onlinePlatformInfo = this.FindCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                foreach (RoundInfo info in filteredInfo) {
                    info.CreativeShareCode = snapshot.GetProperty("share_code").GetString();
                    info.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                    info.CreativeAuthor = onlinePlatformInfo[1];
                    info.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                    info.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                    info.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                    info.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                    info.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                    info.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                    info.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                    info.CreativePlayCount = snapshot.GetProperty("play_count").GetInt32();
                    info.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                    //info.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").GetProperty("time_limit_seconds").GetInt32();
                    info.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                    this.RoundDetails.Update(info);
                }
                this.StatsDB.Commit();
            }
        }
        
        public StatSummary GetLevelInfo(string name, LevelType levelType, BestRecordType recordType, bool useShareCode) {
            StatSummary summary = new StatSummary {
                AllWins = 0,
                TotalShows = 0,
                TotalPlays = 0,
                TotalWins = 0,
                TotalFinals = 0
            };
            
            if (useShareCode) { // user creative round
                List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => r.Profile == this.currentProfile && levelType.CreativeLevelTypeId().Equals(r.Name) && name.Equals(r.ShowNameId));
                int lastShow = -1;
                if (!this.StatLookup.TryGetValue(levelType.CreativeLevelTypeId(), out LevelStats currentLevel)) {
                    currentLevel = new LevelStats(name, name, LevelType.Unknown, BestRecordType.Fastest, false, false, 0, 0, 0, Properties.Resources.round_unknown_icon, Properties.Resources.round_unknown_big_icon);
                }
                
                for (int i = 0; i < filteredInfo.Count; i++) {
                    RoundInfo info = filteredInfo[i];
                    
                    TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                    bool hasLevelDetails = this.StatLookup.TryGetValue(info.Name, out LevelStats levelDetails);
                    bool isCurrentLevel = currentLevel.Name.Equals(hasLevelDetails ? levelDetails.Name : info.Name, StringComparison.OrdinalIgnoreCase);
                    
                    int startRoundShowId = info.ShowID;
                    RoundInfo endRound = info;
                    for (int j = i + 1; j < filteredInfo.Count; j++) {
                        if (filteredInfo[j].ShowID != startRoundShowId) {
                            break;
                        }
                        endRound = filteredInfo[j];
                    }
                    
                    bool isInWinsFilter = (this.CurrentSettings.WinsFilter == 0 ||
                                            (this.CurrentSettings.WinsFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                            (this.CurrentSettings.WinsFilter == 2 && endRound.Start > SeasonStart) ||
                                            (this.CurrentSettings.WinsFilter == 3 && endRound.Start > WeekStart) ||
                                            (this.CurrentSettings.WinsFilter == 4 && endRound.Start > DayStart) ||
                                            (this.CurrentSettings.WinsFilter == 5 && endRound.Start > SessionStart));
                    bool isInQualifyFilter = (this.CurrentSettings.QualifyFilter == 0 ||
                                               (this.CurrentSettings.QualifyFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                               (this.CurrentSettings.QualifyFilter == 2 && endRound.Start > SeasonStart) ||
                                               (this.CurrentSettings.QualifyFilter == 3 && endRound.Start > WeekStart) ||
                                               (this.CurrentSettings.QualifyFilter == 4 && endRound.Start > DayStart) ||
                                               (this.CurrentSettings.QualifyFilter == 5 && endRound.Start > SessionStart));
                    bool isInFastestFilter = this.CurrentSettings.FastestFilter == 0 ||
                                             (this.CurrentSettings.FastestFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                             (this.CurrentSettings.FastestFilter == 2 && endRound.Start > SeasonStart) ||
                                             (this.CurrentSettings.FastestFilter == 3 && endRound.Start > WeekStart) ||
                                             (this.CurrentSettings.FastestFilter == 4 && endRound.Start > DayStart) ||
                                             (this.CurrentSettings.FastestFilter == 5 && endRound.Start > SessionStart);
                    
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
                            if ((!hasLevelDetails || recordType == BestRecordType.HighScore)
                                && info.Score.HasValue && (!summary.BestScore.HasValue || info.Score.Value > summary.BestScore.Value)) {
                                summary.BestScore = info.Score;
                            }
                        }
                    }

                    if (ReferenceEquals(info, endRound) && (info.IsFinal || info.Crown)) {
                        if (info.IsFinal) {
                            summary.CurrentFinalStreak++;
                            if (summary.BestFinalStreak < summary.CurrentFinalStreak) {
                                summary.BestFinalStreak = summary.CurrentFinalStreak;
                            }
                        }
                    }
                    
                    if (info.Qualified) {
                        if (hasLevelDetails && (info.IsFinal || info.Crown)) {
                            summary.AllWins++;

                            if (isInWinsFilter) {
                                summary.TotalWins++;
                                summary.TotalFinals++;
                            }

                            summary.CurrentStreak++;
                            if (summary.CurrentStreak > summary.BestStreak) {
                                summary.BestStreak = summary.CurrentStreak;
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
                    } else {
                        if (!info.IsFinal && !info.Crown) {
                            summary.CurrentFinalStreak = 0;
                        }
                        summary.CurrentStreak = 0;
                        if (isInWinsFilter && hasLevelDetails && (info.IsFinal || info.Crown)) {
                            summary.TotalFinals++;
                        }
                    }
                }
            } else {
                int lastShow = -1;
                if (!this.StatLookup.TryGetValue(name, out LevelStats currentLevel)) {
                    currentLevel = new LevelStats(name, name, LevelType.Unknown, BestRecordType.Fastest, false, false, 0, 0, 0, Properties.Resources.round_unknown_icon, Properties.Resources.round_unknown_big_icon);
                }

                for (int i = 0; i < this.AllStats.Count; i++) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Profile != this.currentProfile) { continue; }

                    TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                    bool hasLevelDetails = this.StatLookup.TryGetValue(info.Name, out LevelStats levelDetails);
                    bool isCurrentLevel = currentLevel.Name.Equals(hasLevelDetails ? levelDetails.Name : info.Name, StringComparison.OrdinalIgnoreCase);

                    int startRoundShowId = info.ShowID;
                    RoundInfo endRound = info;
                    for (int j = i + 1; j < this.AllStats.Count; j++) {
                        if (this.AllStats[j].ShowID != startRoundShowId) {
                            break;
                        }
                        endRound = this.AllStats[j];
                    }

                    bool isInWinsFilter = !endRound.PrivateLobby &&
                                          (this.CurrentSettings.WinsFilter == 0 ||
                                          (this.CurrentSettings.WinsFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                          (this.CurrentSettings.WinsFilter == 2 && endRound.Start > SeasonStart) ||
                                          (this.CurrentSettings.WinsFilter == 3 && endRound.Start > WeekStart) ||
                                          (this.CurrentSettings.WinsFilter == 4 && endRound.Start > DayStart) ||
                                          (this.CurrentSettings.WinsFilter == 5 && endRound.Start > SessionStart));
                    bool isInQualifyFilter = !endRound.PrivateLobby &&
                                             (this.CurrentSettings.QualifyFilter == 0 ||
                                             (this.CurrentSettings.QualifyFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                             (this.CurrentSettings.QualifyFilter == 2 && endRound.Start > SeasonStart) ||
                                             (this.CurrentSettings.QualifyFilter == 3 && endRound.Start > WeekStart) ||
                                             (this.CurrentSettings.QualifyFilter == 4 && endRound.Start > DayStart) ||
                                             (this.CurrentSettings.QualifyFilter == 5 && endRound.Start > SessionStart));
                    bool isInFastestFilter = this.CurrentSettings.FastestFilter == 0 ||
                                             (this.CurrentSettings.FastestFilter == 1 && this.IsInStatsFilter(endRound) && this.IsInPartyFilter(info)) ||
                                             (this.CurrentSettings.FastestFilter == 2 && endRound.Start > SeasonStart) ||
                                             (this.CurrentSettings.FastestFilter == 3 && endRound.Start > WeekStart) ||
                                             (this.CurrentSettings.FastestFilter == 4 && endRound.Start > DayStart) ||
                                             (this.CurrentSettings.FastestFilter == 5 && endRound.Start > SessionStart);

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
                            if ((!hasLevelDetails || recordType == BestRecordType.HighScore)
                                && info.Score.HasValue && (!summary.BestScore.HasValue || info.Score.Value > summary.BestScore.Value)) {
                                summary.BestScore = info.Score;
                            }
                        }
                    }

                    if (ReferenceEquals(info, endRound) && (levelDetails.IsFinal || info.Crown) && !endRound.PrivateLobby) {
                        if (info.IsFinal) {
                            summary.CurrentFinalStreak++;
                            if (summary.BestFinalStreak < summary.CurrentFinalStreak) {
                                summary.BestFinalStreak = summary.CurrentFinalStreak;
                            }
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
            }

            return summary;
        }
        
        private void ClearTotals() {
            this.Wins = 0;
            this.Shows = 0;
            this.Rounds = 0;
            this.CustomRounds = 0;
            this.Duration = TimeSpan.Zero;
            this.CustomShows = 0;
            this.Finals = 0;
            this.GoldMedals = 0;
            this.SilverMedals = 0;
            this.BronzeMedals = 0;
            this.PinkMedals = 0;
            this.EliminatedMedals = 0;
            this.CustomGoldMedals = 0;
            this.CustomSilverMedals = 0;
            this.CustomBronzeMedals = 0;
            this.CustomPinkMedals = 0;
            this.CustomEliminatedMedals = 0;
            this.Kudos = 0;
        }
        
        private void UpdateTotals() {
            try {
                this.lblCurrentProfile.Text = $"{this.GetCurrentProfileName().Replace("&", "&&")}";
                //this.lblCurrentProfile.ToolTipText = $"{Multilingual.GetWord("profile_change_tooltiptext")}";
                this.lblTotalShows.Text = $"{this.Shows:N0}{Multilingual.GetWord("main_inning")}";
                if (this.CustomShows > 0) this.lblTotalShows.Text += $" ({Multilingual.GetWord("main_profile_custom")} : {this.CustomShows:N0}{Multilingual.GetWord("main_inning")})";
                //this.lblTotalShows.ToolTipText = $"{Multilingual.GetWord("shows_detail_tooltiptext")}";
                this.lblTotalRounds.Text = $"{this.Rounds:N0}{Multilingual.GetWord("main_round")}";
                if (this.CustomRounds > 0) this.lblTotalRounds.Text += $" ({Multilingual.GetWord("main_profile_custom")} : {this.CustomRounds:N0}{Multilingual.GetWord("main_round")})";
                //this.lblTotalRounds.ToolTipText = $"{Multilingual.GetWord("rounds_detail_tooltiptext")}";
                this.lblTotalTime.Text = $"{(int)this.Duration.TotalHours}{Multilingual.GetWord("main_hour")}{this.Duration:mm}{Multilingual.GetWord("main_min")}{this.Duration:ss}{Multilingual.GetWord("main_sec")}";
                //this.lblTotalTime.ToolTipText = $"{Multilingual.GetWord("stats_detail_tooltiptext")}";
                float winChance = (float)this.Wins * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalWins.Text = $"{this.Wins:N0}{Multilingual.GetWord("main_win")} ({Math.Truncate(winChance * 10) / 10} %)";
                //this.lblTotalWins.ToolTipText = $"{Multilingual.GetWord("wins_detail_tooltiptext")}";
                float finalChance = (float)this.Finals * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalFinals.Text = $"{this.Finals:N0}{Multilingual.GetWord("main_inning")} ({Math.Truncate(finalChance * 10) / 10} %)";
                //this.lblTotalFinals.ToolTipText = $"{Multilingual.GetWord("finals_detail_tooltiptext")}";
                this.lblGoldMedal.Text = $"{this.GoldMedals:N0}";
                if (this.CustomGoldMedals > 0) this.lblGoldMedal.Text += $" ({this.CustomGoldMedals:N0})";
                this.lblSilverMedal.Text = $"{this.SilverMedals:N0}";
                if (this.CustomSilverMedals > 0) this.lblSilverMedal.Text += $" ({this.CustomSilverMedals:N0})";
                this.lblBronzeMedal.Text = $"{this.BronzeMedals:N0}";
                if (this.CustomBronzeMedals > 0) this.lblBronzeMedal.Text += $" ({this.CustomBronzeMedals:N0})";
                this.lblPinkMedal.Text = $"{this.PinkMedals:N0}";
                if (this.CustomPinkMedals > 0) this.lblPinkMedal.Text += $" ({this.CustomPinkMedals:N0})";
                this.lblEliminatedMedal.Text = $"{this.EliminatedMedals:N0}";
                if (this.CustomEliminatedMedals > 0) this.lblEliminatedMedal.Text += $" ({this.CustomEliminatedMedals:N0})";
                this.lblGoldMedal.Visible = this.GoldMedals != 0 || this.CustomGoldMedals != 0;
                this.lblSilverMedal.Visible = this.SilverMedals != 0 || this.CustomSilverMedals != 0;
                this.lblBronzeMedal.Visible = this.BronzeMedals != 0 || this.CustomBronzeMedals != 0;
                this.lblPinkMedal.Visible = this.PinkMedals != 0 || this.CustomPinkMedals != 0;
                this.lblEliminatedMedal.Visible = this.EliminatedMedals != 0 || this.CustomEliminatedMedals != 0;
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
            //                                                                                 toolTipIcon == ToolTipIcon.Error ? MessageBoxIcon.Error :
            //                                                                                 toolTipIcon == ToolTipIcon.Info ? MessageBoxIcon.Information :
            //                                                                                 toolTipIcon == ToolTipIcon.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.None);
            // }
        }
        
        public void AllocOverlayTooltip() {
            this.omtt = new MetroToolTip();
            this.omtt.Theme = this.Theme;
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
            this.cmtt = new MetroToolTip();
            this.cmtt.OwnerDraw = true;
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
            this.mtt = new MetroToolTip();
            this.mtt.Theme = this.Theme;
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
        
        private void lblCreativeLevel_Click(object sender, EventArgs e) {
            this.mtgCreativeLevel.Checked = !this.mtgCreativeLevel.Checked;
        }
        
        private void mtgCreativeLevel_CheckedChanged(object sender, EventArgs e) {
            bool mtgChecked = ((MetroToggle)sender).Checked; 
            // this.VisibleGridRowOfCreativeLevel(mtgChecked);
            this.lblCreativeLevel.ForeColor = mtgChecked ? Color.FromArgb(0, 174, 219) : Color.DimGray;
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
            this.lblIgnoreLevelTypeWhenSorting.ForeColor = mtgChecked ? Color.FromArgb(0, 174, 219) : Color.DimGray;
            this.CurrentSettings.IgnoreLevelTypeWhenSorting = mtgChecked;
            this.SortGridDetails(true);
            this.SaveUserSettings();
        }
        
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            this.SetMainDataGridView();
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
        
        private void SetMainDataGridView() {
            try {
                if (this.gridDetails.Columns.Count == 0) { return; }
                
                int pos = 0;
                this.gridDetails.Columns["RoundBigIcon"].Visible = false;
                this.gridDetails.Columns["AveKudos"].Visible = false;
                this.gridDetails.Columns["AveDuration"].Visible = false;
                this.gridDetails.Columns["Id"].Visible = false;
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Columns["RoundIcon"].Resizable = DataGridViewTriState.False;
                this.gridDetails.Setup("Name",      pos++, this.GetDataGridViewColumnWidth("Name", Multilingual.GetWord("main_round_name")), Multilingual.GetWord("main_round_name"), DataGridViewContentAlignment.MiddleLeft);
                this.gridDetails.Setup("Played",    pos++, this.GetDataGridViewColumnWidth("Played", Multilingual.GetWord("main_played")), Multilingual.GetWord("main_played"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Qualified", pos++, this.GetDataGridViewColumnWidth("Qualified", Multilingual.GetWord("main_qualified")), Multilingual.GetWord("main_qualified"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Gold",      pos++, this.GetDataGridViewColumnWidth("Gold", Multilingual.GetWord("main_gold")), Multilingual.GetWord("main_gold"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Silver",    pos++, this.GetDataGridViewColumnWidth("Silver", Multilingual.GetWord("main_silver")), Multilingual.GetWord("main_silver"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Bronze",    pos++, this.GetDataGridViewColumnWidth("Bronze", Multilingual.GetWord("main_bronze")), Multilingual.GetWord("main_bronze"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Kudos",     pos++, this.GetDataGridViewColumnWidth("Kudos", Multilingual.GetWord("main_kudos")), Multilingual.GetWord("main_kudos"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Fastest",   pos++, this.GetDataGridViewColumnWidth("Fastest", Multilingual.GetWord("main_fastest")), Multilingual.GetWord("main_fastest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Longest",   pos++, this.GetDataGridViewColumnWidth("Longest", Multilingual.GetWord("main_longest")), Multilingual.GetWord("main_longest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("AveFinish", pos,   this.GetDataGridViewColumnWidth("AveFinish", Multilingual.GetWord("main_ave_finish")), Multilingual.GetWord("main_ave_finish"), DataGridViewContentAlignment.MiddleRight);
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                if (e.RowIndex < 0) { return; }
                if (!this.gridDetails.Rows[e.RowIndex].Visible) { return; }
                
                LevelStats levelStats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                float fBrightness = 0.85F;
                Color cellColor;
                switch (this.gridDetails.Columns[e.ColumnIndex].Name) {
                    case "RoundIcon":
                        if (levelStats.IsFinal) {
                            cellColor = Color.FromArgb(255, 240, 200);
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                ? cellColor
                                : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.CreativeRace:
                                cellColor = Color.FromArgb(122, 201, 241);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Race:
                                cellColor = Color.FromArgb(210, 255, 220);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Survival:
                                cellColor = Color.FromArgb(250, 205, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Hunt:
                                cellColor = Color.FromArgb(200, 220, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Logic:
                                cellColor = Color.FromArgb(230, 250, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Team:
                                cellColor = Color.FromArgb(255, 220, 205);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Invisibeans:
                                cellColor = Color.FromArgb(255, 255, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Unknown:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.LightGray
                                    : Color.DarkGray;
                                break;
                        }
                        break;
                    case "Name":
                        e.CellStyle.Font = Overlay.GetMainFont(14f, FontStyle.Regular, CurrentLanguage);
                        e.CellStyle.ForeColor = Color.Black;
                        if (levelStats.IsCreative) e.Value = $"🔧 {e.Value}";
                        if (levelStats.IsFinal) {
                            cellColor = Color.FromArgb(255, 240, 200);
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.CreativeRace:
                                cellColor = Color.FromArgb(122, 201, 241);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Race:
                                cellColor = Color.FromArgb(210, 255, 220);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Survival:
                                cellColor = Color.FromArgb(250, 205, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Hunt:
                                cellColor = Color.FromArgb(200, 220, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
                            case LevelType.Logic:
                                cellColor = Color.FromArgb(230, 250, 255);
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? cellColor : Utils.GetColorBrightnessAdjustment(cellColor, fBrightness);
                                break;
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
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 126, 222);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = $"{e.Value:N0}";
                        break;
                    case "Qualified":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(255, 20, 147);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        float qualifyChance = levelStats.Qualified * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                        if (this.CurrentSettings.ShowPercentages) {
                            e.Value = $"{Math.Truncate(qualifyChance * 10) / 10}%";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Qualified:N0}";
                        } else {
                            e.Value = $"{levelStats.Qualified:N0}";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(qualifyChance * 10) / 10}%";
                        }
                        break;
                    case "Gold":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(255, 215, 0);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        float goldChance = levelStats.Gold * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                        if (this.CurrentSettings.ShowPercentages) {
                            e.Value = $"{Math.Truncate(goldChance * 10) / 10}%";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Gold:N0}";
                        } else {
                            e.Value = $"{levelStats.Gold:N0}";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(goldChance * 10) / 10}%";
                        }
                        break;
                    case "Silver":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.3f;
                        cellColor = Color.FromArgb(192, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        float silverChance = levelStats.Silver * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                        if (this.CurrentSettings.ShowPercentages) {
                            e.Value = $"{Math.Truncate(silverChance * 10) / 10}%";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Silver:N0}";
                        } else {
                            e.Value = $"{levelStats.Silver:N0}";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(silverChance * 10) / 10}%";
                        }
                        break;
                    case "Bronze":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(205, 127, 50);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        float bronzeChance = levelStats.Bronze * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                        if (this.CurrentSettings.ShowPercentages) {
                            e.Value = $"{Math.Truncate(bronzeChance * 10) / 10}%";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Bronze:N0}";
                        } else {
                            e.Value = $"{levelStats.Bronze:N0}";
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{Math.Truncate(bronzeChance * 10) / 10}%";
                        }
                        break;
                    case "Kudos":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(218, 112, 214);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = $"{e.Value:N0}";
                        break;
                    case "AveFinish":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.AveFinish.ToString("m\\:ss\\.ff");
                        break;
                    case "Fastest":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Fastest.ToString("m\\:ss\\.ff");
                        break;
                    case "Longest":
                        e.CellStyle.Font = Overlay.GetMainFont(14f);
                        fBrightness -= 0.2f;
                        cellColor = Color.FromArgb(0, 192, 192);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(cellColor, fBrightness) : cellColor;
                        e.Value = levelStats.Longest.ToString("m\\:ss\\.ff");
                        break;
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                this.gridDetails.SuspendLayout();
                this.gridDetails.Cursor = Cursors.Default;
                this.HideCustomTooltip(this);
                this.gridDetails.ResumeLayout();
            }
        }
        
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                this.gridDetails.SuspendLayout();
                if (e.RowIndex >= 0 && (this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")) {
                    this.gridDetails.ShowCellToolTips = false;
                    this.gridDetails.Cursor = Cursors.Hand;
                    Point cursorPosition = this.PointToClient(Cursor.Position);
                    Point position = new Point(cursorPosition.X + 16, cursorPosition.Y + 16);
                    this.AllocCustomTooltip(this.cmtt_center_Draw);
                    this.ShowCustomTooltip($"{Multilingual.GetWord("level_detail_tooltiptext_prefix")}{this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value}{Multilingual.GetWord("level_detail_tooltiptext_suffix")}", this, position);
                } else if (e.RowIndex >= 0) {
                    this.gridDetails.ShowCellToolTips = true;
                    this.gridDetails.Cursor = e.RowIndex >= 0 && !(this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")
                        ? this.Theme == MetroThemeStyle.Light
                            ? new Cursor(Properties.Resources.transform_icon.GetHicon())
                            : new Cursor(Properties.Resources.transform_gray_icon.GetHicon())
                        : Cursors.Default;
                }
                this.gridDetails.ResumeLayout();
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }
                if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon") {
                    LevelStats levelStats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                    using (LevelDetails levelDetails = new LevelDetails {
                               LevelName = levelStats.Name,
                               RoundIcon = levelStats.RoundBigIcon,
                               StatsForm = this,
                               IsCreative = levelStats.IsCreative
                           }) {
                        List<RoundInfo> rounds = levelStats.Stats;
                        rounds.Sort();
                        levelDetails.RoundDetails = rounds;
                        this.EnableInfoStrip(false);
                        this.EnableMainMenu(false);
                        levelDetails.ShowDialog(this);
                        this.EnableInfoStrip(true);
                        this.EnableMainMenu(true);
                    }
                } else {
                    this.ToggleWinPercentageDisplay();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }
        
        private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            // this.VisibleGridRowOfCreativeLevel(this.CurrentSettings.GroupingCreativeRoundLevels);
            // this.gridDetails.Invalidate();
            this.mtgCreativeLevel.Checked = this.CurrentSettings.GroupingCreativeRoundLevels;
            this.lblCreativeLevel.ForeColor = this.mtgCreativeLevel.Checked ? Color.FromArgb(0, 174, 219) : Color.DimGray;
            this.mtgIgnoreLevelTypeWhenSorting.Checked = this.CurrentSettings.IgnoreLevelTypeWhenSorting;
            this.lblIgnoreLevelTypeWhenSorting.ForeColor = this.mtgIgnoreLevelTypeWhenSorting.Checked ? Color.FromArgb(0, 174, 219) : Color.DimGray;
        }

        private List<LevelStats> GetFilteredDataSource(bool isFilter) {
            if (isFilter) {
                return this.StatDetails.Where(l => l.IsCreative != true || (l.Id.Equals("creative_race_round") || l.Id.Equals("creative_race_final_round") || l.Id.Equals("user_creative_race_round"))).ToList();   
            } else {
                return this.StatDetails.Where(l => !(l.Id.Equals("creative_race_round") || l.Id.Equals("creative_race_final_round"))).ToList();
            }
        }
        
        // private void VisibleGridRowOfCreativeLevel(bool visible) {
            // List<LevelStats> levelStatsList = this.gridDetails.DataSource as List<LevelStats>;
            // CurrencyManager currencyManager = (CurrencyManager)BindingContext[this.gridDetails.DataSource];  
            // currencyManager.SuspendBinding();
            // for (var i = 0; i < levelStatsList.Count; i++) {
            //     LevelStats levelStats = levelStatsList[i];
            //     if (levelStats.IsCreative && !levelStats.Id.Equals("user_creative_race_round")) {
            //         this.gridDetails.Rows[i].Visible = !visible;
            //     }
            //     if (levelStats.Id.Equals("creative_race_round") || levelStats.Id.Equals("creative_race_final_round")) {
            //         this.gridDetails.Rows[i].Visible = visible;
            //     }
            // }
            // currencyManager.ResumeBinding();
        // }
        
        private void SortGridDetails(bool isInitialize, int columnIndex = 0) {
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
            if (this.gridDetails.SelectedCells.Count > 0) {
                this.gridDetails.ClearSelection();
            }
        }
        
        private void ToggleWinPercentageDisplay() {
            this.CurrentSettings.ShowPercentages = !this.CurrentSettings.ShowPercentages;
            this.SaveUserSettings();
            this.gridDetails.Invalidate();
        }
        
        private void ShowShows() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Shows";
                List<RoundInfo> rounds = new List<RoundInfo>();
                foreach (LevelStats ls in this.StatDetails) {
                    if (ls.Id.Equals("creative_race_round") || ls.Id.Equals("creative_race_final_round")) continue;
                    rounds.AddRange(ls.Stats);
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
                        shows.Insert(0,
                            new RoundInfo {
                                Name = isFinal ? "Final" : string.Empty,
                                ShowNameId = info.ShowNameId,
                                IsFinal = isFinal,
                                End = endDate,
                                Start = info.Start,
                                StartLocal = info.StartLocal,
                                Kudos = kudosTotal,
                                Qualified = won,
                                Round = roundCount,
                                ShowID = info.ShowID,
                                Tier = won ? 1 : 0,
                                PrivateLobby = info.PrivateLobby,
                                UseShareCode = info.UseShareCode,
                                CreativeAuthor = info.CreativeAuthor,
                                CreativeOnlinePlatformId = info.CreativeOnlinePlatformId,
                                CreativeShareCode = info.CreativeShareCode,
                                CreativeTitle = info.CreativeTitle,
                                CreativeDescription = info.CreativeDescription,
                                CreativeVersion = info.CreativeVersion,
                                CreativeMaxPlayer = info.CreativeMaxPlayer,
                                CreativePlatformId = info.CreativePlatformId,
                                CreativePlayCount = info.CreativePlayCount,
                                CreativeLastModifiedDate = info.CreativeLastModifiedDate,
                            });
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
                for (int i = 0; i < this.StatDetails.Count; i++) {
                    if (this.StatDetails[i].Id.Equals("creative_race_round") || this.StatDetails[i].Id.Equals("creative_race_final_round")) continue;
                    rounds.AddRange(this.StatDetails[i].Stats);
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
                for (int i = 0; i < this.StatDetails.Count; i++) {
                    if (this.StatDetails[i].Id.Equals("creative_race_round") || this.StatDetails[i].Id.Equals("creative_race_final_round")) continue;
                    rounds.AddRange(this.StatDetails[i].Stats);
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
            for (int i = 0; i < this.StatDetails.Count; i++) {
                if (this.StatDetails[i].Id.Equals("creative_race_round") || this.StatDetails[i].Id.Equals("creative_race_final_round")) continue;
                rounds.AddRange(this.StatDetails[i].Stats);
            }
            rounds.Sort();
            
            using (WinStatsDisplay display = new WinStatsDisplay {
                       StatsForm = this,
                       Text = $@"     {Multilingual.GetWord("level_detail_wins_per_day")} - {this.GetCurrentProfileName().Replace("&", "&&")} ({this.GetCurrentFilterName()})",
                       BackImage = Properties.Resources.crown_icon,
                       BackMaxSize = 32,
                       BackImagePadding = new Padding(20, 20, 0, 0)
                   }) {
                ArrayList dates = new ArrayList();
                ArrayList shows = new ArrayList();
                ArrayList finals = new ArrayList();
                ArrayList wins = new ArrayList();
                Dictionary<double, SortedList<string, int>> winsInfo = new Dictionary<double, SortedList<string, int>>();
                if (rounds.Count > 0) {
                    DateTime start = rounds[0].StartLocal;
                    int currentShows = 0;
                    int currentFinals = 0;
                    int currentWins = 0;
                    bool isIncrementedShows = false;
                    bool isIncrementedFinals = false;
                    bool isIncrementedWins = false;
                    bool isOverDate = false;
                    foreach (RoundInfo info in rounds.Where(info => !info.PrivateLobby)) {
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
                                
                                if (winsInfo.TryGetValue(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), out SortedList<string, int> wi)) {
                                    if (wi.ContainsKey($"{Multilingual.GetRoundName(info.Name)};crown")) {
                                        wi[$"{Multilingual.GetRoundName(info.Name)};crown"] += 1;
                                    } else {
                                        wi[$"{Multilingual.GetRoundName(info.Name)};crown"] = 1;
                                    }
                                } else {
                                    winsInfo.Add(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), new SortedList<string, int> {{$"{Multilingual.GetRoundName(info.Name)};crown", 1}});
                                }

                                if (isOverDate) {
                                    currentShows--;
                                    isIncrementedShows = false;
                                }
                            } else {
                                if (winsInfo.TryGetValue(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), out SortedList<string, int> wi)) {
                                    if (wi.ContainsKey($"{Multilingual.GetRoundName(info.Name)};eliminated")) {
                                        wi[$"{Multilingual.GetRoundName(info.Name)};eliminated"] += 1;
                                    } else {
                                        wi[$"{Multilingual.GetRoundName(info.Name)};eliminated"] = 1;
                                    }
                                } else {
                                    winsInfo.Add(isOverDate ? info.StartLocal.Date.ToOADate() : start.Date.ToOADate(), new SortedList<string, int> {{$"{Multilingual.GetRoundName(info.Name)};eliminated", 1}});
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
                                shows.Add(0D);
                                finals.Add(0D);
                                wins.Add(0D);
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

                    display.manualSpacing = (int)Math.Ceiling(dates.Count / 28D);
                } else {
                    dates.Add(DateTime.Now.Date.ToOADate());
                    shows.Add(0D);
                    finals.Add(0D);
                    wins.Add(0D);

                    display.manualSpacing = 1;
                }
                display.dates = (double[])dates.ToArray(typeof(double));
                display.shows = (double[])shows.ToArray(typeof(double));
                display.finals = (double[])finals.ToArray(typeof(double));
                display.wins = (double[])wins.ToArray(typeof(double));
                display.winsInfo = winsInfo;
                
                display.ShowDialog(this);
            }
        }
        
        private void ShowRoundGraph() {
            using (RoundStatsDisplay roundStatsDisplay = new RoundStatsDisplay {
                       StatsForm = this,
                       Text = $@"     {Multilingual.GetWord("level_detail_stats_by_round")} - {this.GetCurrentProfileName().Replace("&", "&&")} ({this.GetCurrentFilterName()})",
                       BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon,
                       BackMaxSize = 32,
                       BackImagePadding = new Padding(20, 20, 0, 0)
                   })
            {
                List<RoundInfo> rounds;
                List<RoundInfo> userCreativeRounds;
                if (this.menuCustomRangeStats.Checked) {
                    rounds = this.AllStats.Where(ri => {
                        return ri.Start >= this.customfilterRangeStart &&
                               ri.Start <= this.customfilterRangeEnd &&
                               ri.Profile == this.GetCurrentProfileId() &&
                               //!"user_creative_race_round".Equals(ri.Name) &&
                               !ri.UseShareCode &&
                               this.IsInPartyFilter(ri);
                    }).OrderBy(ri => ri.Name).ToList();
                    userCreativeRounds = this.AllStats.Where(ri => {
                        return ri.Start >= this.customfilterRangeStart &&
                               ri.Start <= this.customfilterRangeEnd &&
                               ri.Profile == this.GetCurrentProfileId() &&
                               //"user_creative_race_round".Equals(ri.Name) &&
                               ri.UseShareCode &&
                               this.IsInPartyFilter(ri);
                    }).OrderBy(ri => ri.ShowNameId).ToList();
                } else {
                    DateTime compareDate = this.menuAllStats.Checked ? DateTime.MinValue :
                        this.menuSeasonStats.Checked ? SeasonStart :
                        this.menuWeekStats.Checked ? WeekStart :
                        this.menuDayStats.Checked ? DayStart : SessionStart;
                    rounds = this.AllStats.Where(ri => {
                        return ri.Start > compareDate &&
                               ri.Profile == this.GetCurrentProfileId() &&
                               //!"user_creative_race_round".Equals(ri.Name) &&
                               !ri.UseShareCode &&
                               this.IsInPartyFilter(ri);
                    }).OrderBy(ri => ri.Name).ToList();
                    userCreativeRounds = this.AllStats.Where(ri => {
                        return ri.Start > compareDate &&
                               ri.Profile == this.GetCurrentProfileId() &&
                               //"user_creative_race_round".Equals(ri.Name) &&
                               ri.UseShareCode &&
                               this.IsInPartyFilter(ri);
                    }).OrderBy(ri => ri.ShowNameId).ToList();
                }
                if (rounds.Count == 0 && userCreativeRounds.Count == 0) { return; }
                
                Dictionary<string, double[]> roundGraphData = new Dictionary<string, double[]>();
                Dictionary<string, TimeSpan> roundDurationData = new Dictionary<string, TimeSpan>();
                Dictionary<string, int[]> roundScoreData = new Dictionary<string, int[]>();
                Dictionary<string, string> roundList = new Dictionary<string, string>();
                
                double p = 0, gm = 0, sm = 0, bm = 0, pm = 0, em = 0;
                int hs = 0, ls = 0;
                TimeSpan d = TimeSpan.Zero;
                for (int i = 0; i < rounds.Count; i++) {
                    if (i > 0 && !rounds[i].Name.Equals(rounds[i - 1].Name)) {
                        roundDurationData.Add(rounds[i - 1].Name, d);
                        roundGraphData.Add(rounds[i - 1].Name, new[] { p, gm, sm, bm, pm, em });
                        roundScoreData.Add(rounds[i - 1].Name, new[] { hs, ls });
                        roundList.Add(rounds[i - 1].Name, Multilingual.GetRoundName(rounds[i - 1].Name).Replace("&", "&&"));
                        d = TimeSpan.Zero;
                        hs = 0; ls = 0;
                        p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                    }
                    hs = (int)(rounds[i].Score > hs ? rounds[i].Score : hs);
                    ls = (int)(rounds[i].Score < ls ? rounds[i].Score : ls);
                    
                    d += rounds[i].End - rounds[i].Start;
                    p++;
                    if (rounds[i].Qualified) {
                        switch (rounds[i].Tier) {
                            case (int)QualifyTier.Pink:
                                pm++; break;
                            case (int)QualifyTier.Gold:
                                gm++; break;
                            case (int)QualifyTier.Silver:
                                sm++; break;
                            case (int)QualifyTier.Bronze:
                                bm++; break;
                        }
                    } else {
                        em++;
                    }

                    if (i == rounds.Count - 1) {
                        roundDurationData.Add(rounds[i].Name, d);
                        roundGraphData.Add(rounds[i].Name, new[] { p, gm, sm, bm, pm, em });
                        roundScoreData.Add(rounds[i].Name, new[] { hs, ls });
                        roundList.Add(rounds[i].Name, Multilingual.GetRoundName(rounds[i].Name).Replace("&", "&&"));
                    }
                }

                d = TimeSpan.Zero;
                hs = 0; ls = 0;
                p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                for (int i = 0; i < userCreativeRounds.Count; i++) {
                    if (i > 0 && !userCreativeRounds[i].ShowNameId.Equals(userCreativeRounds[i - 1].ShowNameId)) {
                        roundDurationData.Add(userCreativeRounds[i - 1].ShowNameId, d);
                        roundGraphData.Add(userCreativeRounds[i - 1].ShowNameId, new[] { p, gm, sm, bm, pm, em });
                        roundScoreData.Add(userCreativeRounds[i - 1].ShowNameId, new[] { hs, ls });
                        List<RoundInfo> userCreativeRoundsTitle = userCreativeRounds.FindAll(r => userCreativeRounds[i - 1].ShowNameId.Equals(r.ShowNameId) && !string.IsNullOrEmpty(r.CreativeTitle));
                        roundList.Add(userCreativeRounds[i - 1].ShowNameId, userCreativeRoundsTitle.Count == 0 ? userCreativeRounds[i - 1].ShowNameId : userCreativeRoundsTitle[userCreativeRoundsTitle.Count - 1].CreativeTitle.Replace("&", "&&"));
                        d = TimeSpan.Zero;
                        hs = 0; ls = 0;
                        p = 0; gm = 0; sm = 0; bm = 0; pm = 0; em = 0;
                    }
                    hs = (int)(userCreativeRounds[i].Score > hs ? userCreativeRounds[i].Score : hs);
                    ls = (int)(userCreativeRounds[i].Score < ls ? userCreativeRounds[i].Score : ls);
                    
                    d += userCreativeRounds[i].End - userCreativeRounds[i].Start;
                    p++;
                    if (userCreativeRounds[i].Qualified) {
                        switch (userCreativeRounds[i].Tier) {
                            case (int)QualifyTier.Pink:
                                pm++; break;
                            case (int)QualifyTier.Gold:
                                gm++; break;
                            case (int)QualifyTier.Silver:
                                sm++; break;
                            case (int)QualifyTier.Bronze:
                                bm++; break;
                        }
                    } else {
                        em++;
                    }

                    if (i == userCreativeRounds.Count - 1) {
                        roundDurationData.Add(userCreativeRounds[i].ShowNameId, d);
                        roundGraphData.Add(userCreativeRounds[i].ShowNameId, new[] { p, gm, sm, bm, pm, em });
                        roundScoreData.Add(userCreativeRounds[i].ShowNameId, new[] { hs, ls });
                        List<RoundInfo> userCreativeRoundsTitle = userCreativeRounds.FindAll(r => userCreativeRounds[i].ShowNameId.Equals(r.ShowNameId) && !string.IsNullOrEmpty(r.CreativeTitle));
                        roundList.Add(userCreativeRounds[i].ShowNameId, userCreativeRoundsTitle.Count == 0 ? userCreativeRounds[i].ShowNameId : userCreativeRoundsTitle[userCreativeRoundsTitle.Count - 1].CreativeTitle.Replace("&", "&&"));
                    }
                }
                
                roundStatsDisplay.roundList = from pair in roundList orderby pair.Value ascending select pair; // use LINQ
                roundStatsDisplay.roundDurationData = roundDurationData;
                roundStatsDisplay.roundScoreData = roundScoreData;
                roundStatsDisplay.roundGraphData = roundGraphData;
                
                roundStatsDisplay.ShowDialog(this);
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
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MetroMessageBox.Show(this, Multilingual.GetWord("message_fall_guys_already_running"),
                                        Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
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
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MetroMessageBox.Show(this, Multilingual.GetWord("message_fall_guys_already_running"),
                                        Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
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
        }
        
        public string[] FindEpicGamesUserInfo() {
            string[] userInfo = { string.Empty, string.Empty };
            try {
                string launcherLogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Logs", "EpicGamesLauncher.log");
                if (File.Exists(launcherLogFilePath)) {
                    using (FileStream fs = File.Open(launcherLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        using (StreamReader sr = new StreamReader(fs)) {
                            string line;
                            List<string> lines = new List<string>();
                            while ((line = sr.ReadLine()) != null) {
                                if (line.IndexOf("FCommunityPortalLaunchAppTask: Launching app ") >= 0) {
                                    lines.Add(line);
                                }
                            }

                            if (lines.Count > 0) {
                                line = lines.Last();
                                int index = line.IndexOf("-epicuserid=") + 12;
                                int index2 = line.IndexOf("-epicusername=") + 15;
                                userInfo[0] = line.Substring(index, line.IndexOf(" -epiclocale=") - index);
                                userInfo[1] = line.Substring(index2, line.IndexOf("\" -epicuserid=") - index2);
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
                object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                if (regValue == null) {
                    return userInfo;
                }
                string steamPath = (string)regValue;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
                }

                string steamConfigPath = Path.Combine(steamPath, "config", "loginusers.vdf");
                if (File.Exists(steamConfigPath)) {
                    FileInfo steamConfigFileInfo = new FileInfo(steamConfigPath);
                    JsonClass json = Json.Read(File.ReadAllText(steamConfigFileInfo.FullName)) as JsonClass;
                    foreach (JsonObject obj in json) {
                        if (obj is JsonClass node) {
                            if (!string.IsNullOrEmpty(node["AccountName"].AsString())) userInfo[0] = node["AccountName"].AsString();
                            if (!string.IsNullOrEmpty(node["PersonaName"].AsString())) userInfo[1] = node["PersonaName"].AsString();
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
                if (regValue == null) {
                    return string.Empty;
                }
                string epicGamesPath = Path.Combine((string)regValue, "Manifests");
                
                if (Directory.Exists(epicGamesPath)) {
                    DirectoryInfo di = new DirectoryInfo(epicGamesPath);
                    foreach (FileInfo file in di.GetFiles()) {
                        if (!".item".Equals(file.Extension)) continue;
                        JsonClass json = Json.Read(File.ReadAllText(file.FullName)) as JsonClass;
                        string displayName = json["DisplayName"].AsString();
                        if ("Fall Guys".Equals(displayName)) {
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
                // get steam install folder
                object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                if (regValue == null) {
                    return string.Empty;
                }
                string steamPath = (string)regValue;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
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
                        tsl.ForeColor = tsl.Name.Equals("lblCurrentProfile")
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
                    break;
                case true when e.KeyCode == Keys.F:
                    if (!this.overlay.IsFixed()) {
                        this.overlay.FlipDisplay(!this.overlay.flippedImage);
                        this.CurrentSettings.FlippedDisplay = this.overlay.flippedImage;
                        this.SaveUserSettings();
                    }
                    break;
                case true when e.KeyCode == Keys.R:
                    this.CurrentSettings.ColorByRoundType = !this.CurrentSettings.ColorByRoundType;
                    this.SaveUserSettings();
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
                    this.overlay.ArrangeDisplay(this.CurrentSettings.FlippedDisplay, this.CurrentSettings.ShowOverlayTabs,
                        this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo,
                        this.CurrentSettings.OverlayColor, this.CurrentSettings.OverlayWidth, this.CurrentSettings.OverlayHeight,
                        this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
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
                    if (!(this.ProfileMenuItems[i] is ToolStripMenuItem menuItem)) { continue; }
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
                    if (!(this.ProfileMenuItems[i] is ToolStripMenuItem menuItem)) { continue; }
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
                this.ShowRoundGraph();
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
                this.ShowFinals();
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
                this.ShowShows();
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
                this.ShowRounds();
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
                this.ShowWinGraph();
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            } catch (Exception ex) {
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblLeaderboard_Click(object sender, EventArgs e) {
            try {
                this.EnableInfoStrip(false);
                this.EnableMainMenu(false);
                using (LeaderboardDisplay leaderboard = new LeaderboardDisplay {
                           StatsForm = this,
                           Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}",
                           BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.leaderboard_icon : Properties.Resources.leaderboard_gray_icon,
                           BackMaxSize = 32,
                           BackImagePadding = new Padding(20, 20, 0, 0)
                       }) {
                    // Dictionary<string, string> roundList = new Dictionary<string, string>();
                    // foreach (KeyValuePair<string, LevelStats> entry in LevelStats.ALL) {
                    //     if (entry.Value.Type == LevelType.Race) {
                    //         roundList.Add(entry.Key, entry.Value.Name);
                    //     }
                    // }
                    // leaderboard.RoundList = from pair in roundList orderby pair.Value ascending select pair; // use LINQ
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
                if (button == this.menuCustomRangeStats || button == this.trayCustomRangeStats) {
                    if (this.isStartingUp) {
                        this.updateFilterRange = true;
                    } else {
                        using (FilterCustomRange filterCustomRange = new FilterCustomRange()) {
                        //filterCustomRange.Icon = this.Icon;
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
                } else if (button == this.menuAllStats || button == this.menuSeasonStats || button == this.menuWeekStats || button == this.menuDayStats || button == this.menuSessionStats) {
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
                } else if (button == this.menuAllPartyStats || button == this.menuSoloStats || button == this.menuPartyStats) {
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
                                this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileName.Equals(this.ProfileMenuItems[i].Text.Replace("&&", "&")) && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                            }
                            this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                            this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                        }
                    }
                    this.currentProfile = this.GetProfileIdByName(button.Text.Replace("&&", "&"));
                    this.updateSelectedProfile = true;
                } else if (button == this.trayAllStats || button == this.traySeasonStats || button == this.trayWeekStats || button == this.trayDayStats || button == this.traySessionStats) {
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
                } else if (button == this.trayAllPartyStats || button == this.traySoloStats || button == this.trayPartyStats) {
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
                                this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileName.Equals(this.ProfileTrayItems[i].Text.Replace("&&", "&")) && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                            }
                            this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                            this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                        }
                    }
                    this.currentProfile = this.GetProfileIdByName(button.Text.Replace("&&", "&"));
                    this.updateSelectedProfile = true;
                }
                
                for (int i = 0; i < this.StatDetails.Count; i++) {
                    LevelStats calculator = this.StatDetails[i];
                    calculator.Clear();
                }

                this.ClearTotals();

                int profile = this.currentProfile;

                List<RoundInfo> rounds;
                if (this.menuCustomRangeStats.Checked) {
                    rounds = this.AllStats.Where(roundInfo => {
                        return roundInfo.Start >= this.customfilterRangeStart &&
                               roundInfo.Start <= this.customfilterRangeEnd &&
                               roundInfo.Profile == profile && this.IsInPartyFilter(roundInfo);
                    }).ToList();
                } else {
                    DateTime compareDate = this.menuAllStats.Checked ? DateTime.MinValue :
                                            this.menuSeasonStats.Checked ? SeasonStart :
                                            this.menuWeekStats.Checked ? WeekStart :
                                            this.menuDayStats.Checked ? DayStart : SessionStart;
                    rounds = this.AllStats.Where(roundInfo => {
                        return roundInfo.Start > compareDate && roundInfo.Profile == profile && this.IsInPartyFilter(roundInfo);
                    }).ToList();
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
        
        public string FindCreativeLevelInfo(string code) {
            string levelName = this.AllStats.FindLast(r => !string.IsNullOrEmpty(r.ShowNameId) && r.ShowNameId.Equals(code) && r.Name.Equals("user_creative_race_round")).CreativeTitle;
            return string.IsNullOrEmpty(levelName) ? code : levelName;
        }
        
        public string[] FindCreativeAuthor(JsonElement authorData) {
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
            // this.timeSwitcherForCheckUpdate = DateTime.UtcNow;
            this.isAvailableNewVersion = true;
            this.availableNewVersion = newVersion;
            this.menuUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
            this.trayUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
        }
        
        public bool CheckForNewVersion() {
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
                        }
                    }
                } catch {
                    return false;
                }
            }
            return true;
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
                                    progress.FileName = "FallGuysStats.zip";
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
        }
        
        public void SetAutoChangeProfile(bool autoChangeProfile) {
            this.CurrentSettings.AutoChangeProfile = autoChangeProfile;
            this.logFile.autoChangeProfile = autoChangeProfile;
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
        }
        
        private void SetMinimumSize() {
            this.MinimumSize = new Size(CurrentLanguage == Language.English ? 720 :
                                        CurrentLanguage == Language.French ? 845 :
                                        CurrentLanguage == Language.Korean ? 650 :
                                        CurrentLanguage == Language.Japanese ? 795 : 600
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
                        this.SetSystemTrayIcon(this.CurrentSettings.SystemTrayIcon);
                        this.SetTheme(CurrentTheme);
                        this.SaveUserSettings();
                        if (this.currentLanguage != (int)CurrentLanguage) {
                            this.SetMinimumSize();
                            this.ChangeMainLanguage();
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
                        this.logFile.autoChangeProfile = this.CurrentSettings.AutoChangeProfile;
                        this.logFile.preventOverlayMouseClicks = this.CurrentSettings.PreventOverlayMouseClicks;
                        
                        IsDisplayOverlayPing = this.CurrentSettings.OverlayVisible && !this.CurrentSettings.HideRoundInfo && (this.CurrentSettings.SwitchBetweenPlayers || this.CurrentSettings.OnlyShowPing);
                        
                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(this.CurrentSettings.LogPath) ||
                            (!string.IsNullOrEmpty(lastLogPath) && lastLogPath.Equals(this.CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await this.logFile.Stop();
                            string logFilePath = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                                logFilePath = this.CurrentSettings.LogPath;
                            }
                            this.logFile.Start(logFilePath, LOGFILENAME);
                        }
                        
                        this.overlay.ArrangeDisplay(string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.FlippedDisplay : this.CurrentSettings.FixedFlippedDisplay, this.CurrentSettings.ShowOverlayTabs,
                            this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo,
                            this.CurrentSettings.OverlayColor, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayWidth : this.CurrentSettings.OverlayFixedWidth, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayHeight : this.CurrentSettings.OverlayFixedHeight,
                            this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
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
                    editProfiles.AllStats = this.RoundDetails.FindAll().ToList();
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
            this.trayLaunchFallGuys.Image = launchPlatform == 0
                ? Properties.Resources.epic_main_icon
                : Properties.Resources.steam_main_icon;
            this.menuLaunchFallGuys.Image = launchPlatform == 0
                ? Properties.Resources.epic_main_icon
                : Properties.Resources.steam_main_icon;
        }
        
        private void ChangeMainLanguage() {
            this.currentLanguage = (int)CurrentLanguage;
            this.mainWndTitle = $@"     {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.trayIcon.Text = this.mainWndTitle.Trim();
            this.Text = this.mainWndTitle;
            this.menu.Font = Overlay.GetMainFont(12);
            this.menuLaunchFallGuys.Font = Overlay.GetMainFont(12);
            this.infoStrip.Font = Overlay.GetMainFont(13);
            this.infoStrip2.Font = Overlay.GetMainFont(13);
            this.lblLeaderboard.Text = Multilingual.GetWord("leaderboard_menu_title");
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(14);
            this.lblCreativeLevel.Text = Multilingual.GetWord("settings_grouping_creative_round_levels");
            this.lblIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            
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
            if (!CurrentSettings.OverlayVisible) {
                this.trayOverlay.Text = Multilingual.GetWord("main_show_overlay");
            } else {
                this.trayOverlay.Text = Multilingual.GetWord("main_hide_overlay");
            }
            this.trayLookHere.Text = Multilingual.GetWord("main_look_here");
            this.trayFallGuysWiki.Text = Multilingual.GetWord("main_fall_guys_wiki");
            this.trayFallGuysReddit.Text = Multilingual.GetWord("main_fall_guys_reddit");
            this.trayFallalytics.Text = Multilingual.GetWord("main_fallalytics");
            this.trayRollOffClub.Text = Multilingual.GetWord("main_roll_off_club");
            this.trayLostTempleAnalyzer.Text = Multilingual.GetWord("main_lost_temple_analyzer");
            this.trayFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
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
            if (!CurrentSettings.OverlayVisible) {
                this.menuOverlay.Text = Multilingual.GetWord("main_show_overlay");
            } else {
                this.menuOverlay.Text = Multilingual.GetWord("main_hide_overlay");
            }
            this.menuUpdate.Text = Multilingual.GetWord("main_update");
            this.menuHelp.Text = Multilingual.GetWord("main_help");
            this.menuLaunchFallGuys.Text = Multilingual.GetWord("main_launch_fall_guys");
            this.menuLookHere.Text = Multilingual.GetWord("main_look_here");
            this.menuFallGuysWiki.Text = Multilingual.GetWord("main_fall_guys_wiki");
            this.menuFallGuysReddit.Text = Multilingual.GetWord("main_fall_guys_reddit");
            this.menuFallalytics.Text = Multilingual.GetWord("main_fallalytics");
            this.menuRollOffClub.Text = Multilingual.GetWord("main_roll_off_club");
            this.menuLostTempleAnalyzer.Text = Multilingual.GetWord("main_lost_temple_analyzer");
            this.menuFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
            this.menuFallGuysOfficial.Text = Multilingual.GetWord("main_fall_guys_official");
        }
    }
}