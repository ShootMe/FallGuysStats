using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

namespace FallGuysStats {
    public partial class Stats : MetroFramework.Forms.MetroForm {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        //[DllImport("user32.dll")]
        //private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetForegroundWindow();
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetActiveWindow();
        
        public enum DWMWINDOWATTRIBUTE {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        public enum DWM_WINDOW_CORNER_PREFERENCE {
            DWMWCP_DEFAULT      = 0,
            DWMWCP_DONOTROUND   = 1,
            DWMWCP_ROUND        = 2,
            DWMWCP_ROUNDSMALL   = 3
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern long DwmSetWindowAttribute(IntPtr hWnd,
            DWMWINDOWATTRIBUTE attribute,
            ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
            uint cbAttribute);
        
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
                        CurrentLanguage = "fr".Equals(sysLang, StringComparison.Ordinal) ? 1 :
                                          "ko".Equals(sysLang, StringComparison.Ordinal) ? 2 :
                                          "ja".Equals(sysLang, StringComparison.Ordinal) ? 3 :
                                          "zh-chs".Equals(sysLang, StringComparison.Ordinal) ? 4 :
                                          "zh-cht".Equals(sysLang, StringComparison.Ordinal) ? 5 : 0;
                        MessageBox.Show(Multilingual.GetWord("message_tracker_already_running"), Multilingual.GetWord("message_already_running_caption"),
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
        private static string LOGNAME = "Player.log";
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
        public static bool InShow = false; 
        public static bool EndedShow = false;
        public static bool IsPlaying = false;
        public static bool IsPrePlaying = false;
        public static bool IsQueued = false;
        public static int QueuedPlayers = 0;
        public static int PingSwitcher = 10;
        public static long LastServerPing = 0;
        public static bool IsBadPing = false;
        public static string LastCountryAlpha2Code = string.Empty;
        public static string LastCountryAlpha3Code = string.Empty;
        public static string LastCountryDefaultName = string.Empty;
        public static int CurrentLanguage;
        public static MetroThemeStyle CurrentTheme = MetroThemeStyle.Light;
        private static FallalyticsReporter FallalyticsReporter = new FallalyticsReporter();
        public static Bitmap ImageOpacity(Image sourceImage, float opacity = 1F) {
            Bitmap bmp = new Bitmap(sourceImage.Width, sourceImage.Height);
            Graphics gp = Graphics.FromImage(bmp);
            ColorMatrix clrMatrix = new ColorMatrix { Matrix33 = opacity };
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gp.DrawImage(sourceImage, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, imgAttribute);
            gp.Dispose();
            return bmp;
        }
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        private int Shows, Rounds;
        private int CustomShows, CustomRounds;
        private TimeSpan Duration;
        private int Wins;
        private int Finals;
        private int Kudos;
        private int GoldMedals, SilverMedals, BronzeMedals, PinkMedals, EliminatedMedals;
        private int CustomGoldMedals, CustomSilverMedals, CustomBronzeMedals, CustomPinkMedals, CustomEliminatedMedals;
        private int nextShowID;
        private bool loadingExisting;
        private bool updateFilterType;
        private bool updateFilterRange;
        private DateTime customfilterRangeStart = DateTime.MinValue;
        private DateTime customfilterRangeEnd = DateTime.MaxValue;
        private int selectedCustomTemplateSeason;
        private bool updateSelectedProfile;
        private bool useLinkedProfiles;
        public LiteDatabase StatsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        public ILiteCollection<UserSettings> UserSettings;
        public ILiteCollection<Profiles> Profiles;
        public List<Profiles> AllProfiles = new List<Profiles>();
        public List<ToolStripMenuItem> ProfileMenuItems = new List<ToolStripMenuItem>();
        public List<ToolStripMenuItem> ProfileTrayItems = new List<ToolStripMenuItem>();
        public UserSettings CurrentSettings;
        public Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        private DateTime startupTime = DateTime.UtcNow;
        private int askedPreviousShows = 0;
        private TextInfo textInfo;
        private int currentProfile;
        private int currentLanguage;
        private Color infoStripForeColor;

        private readonly Image numberOne = ImageOpacity(Properties.Resources.number_1,   0.5F);
        private readonly Image numberTwo = ImageOpacity(Properties.Resources.number_2,   0.5F);
        private readonly Image numberThree = ImageOpacity(Properties.Resources.number_3, 0.5F);
        private readonly Image numberFour = ImageOpacity(Properties.Resources.number_4,  0.5F);
        private readonly Image numberFive = ImageOpacity(Properties.Resources.number_5,  0.5F);
        private readonly Image numberSix = ImageOpacity(Properties.Resources.number_6,   0.5F);
        private readonly Image numberSeven = ImageOpacity(Properties.Resources.number_7, 0.5F);
        private readonly Image numberEight = ImageOpacity(Properties.Resources.number_8, 0.5F);
        private readonly Image numberNine = ImageOpacity(Properties.Resources.number_9,  0.5F);

        private bool maximizedForm;
        private bool isFocused;
        private bool isFormClosing;
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
// #if AllowUpdate
//         public DateTime timeSwitcherForCheckUpdate;
// #endif
        
        public Point screenCenter;
        public readonly string FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL = "https://github.com/ShootMe/FallGuysStats/releases/latest/download/FallGuysStats.zip";
        public readonly string FALLGUYSDB_API_URL = "https://api2.fallguysdb.info/api/";
        private readonly string IP2C_ORG_URL = "https://ip2c.org/";

        private int profileIdWithLinkedCustomShow;
        public readonly string[] publicShowIdList = {
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
            "event_only_slime_climb",
            "event_only_jump_club_template",
            "event_walnut_template",
            "survival_of_the_fittest",
            "show_robotrampage_ss2_show1_template",
            "event_le_anchovy_template",
            "event_pixel_palooza_template",
            "xtreme_party",
            "fall_guys_creative_mode",
            "private_lobbies"
        };

        private Stats() {
// #if AllowUpdate
//             this.timeSwitcherForCheckUpdate = DateTime.UtcNow;
// #endif
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
                    CurrentLanguage = this.CurrentSettings.Multilingual;
                    CurrentTheme = this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light :
                                   this.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default;
                } catch {
                    this.UserSettings.DeleteAll();
                    this.CurrentSettings = GetDefaultSettings();
                    this.UserSettings.Insert(this.CurrentSettings);
                }
            }
            this.StatsDB.Commit();
            
            this.RemoveUpdateFiles();
            
            this.InitializeComponent();
            
            this.infoStrip.Renderer = new CustomToolStripSystemRenderer();
            this.infoStrip2.Renderer = new CustomToolStripSystemRenderer();
            DwmSetWindowAttribute(this.menu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.menuFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.menuStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.menuPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.menuProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.menuLookHere.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayCMenu.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayFilters.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayStatsFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayPartyFilter.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayProfile.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            DwmSetWindowAttribute(this.trayLookHere.DropDown.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference, sizeof(uint));
            
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

            this.StatsDB.BeginTrans();

            if (this.Profiles.Count() == 0) {
                string sysLang = CultureInfo.CurrentUICulture.Name.StartsWith("zh") ?
                                 CultureInfo.CurrentUICulture.Name :
                                 CultureInfo.CurrentUICulture.Name.Substring(0, 2);
                using (SelectLanguage initLanguageForm = new SelectLanguage(sysLang)) {
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    if (initLanguageForm.ShowDialog(this) == DialogResult.OK) {
                        CurrentLanguage = initLanguageForm.selectedLanguage;
                        Overlay.SetDefaultFont(CurrentLanguage, 18);
                        this.CurrentSettings.Multilingual = initLanguageForm.selectedLanguage;
                        if (initLanguageForm.autoGenerateProfiles) {
                            for (int i = this.publicShowIdList.Length; i >= 1; i--) {
                                string showId = this.publicShowIdList[i - 1];
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

            this.BackImage = this.Icon.ToBitmap();
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(18, 18, 0, 0);
            
            this.RoundDetails.EnsureIndex(x => x.Name);
            this.RoundDetails.EnsureIndex(x => x.ShowID);
            this.RoundDetails.EnsureIndex(x => x.Round);
            this.RoundDetails.EnsureIndex(x => x.Start);
            this.RoundDetails.EnsureIndex(x => x.InParty);
            this.StatsDB.Commit();
            
            this.UpdateDatabaseVersion();
            
            foreach (KeyValuePair<string, LevelStats> entry in LevelStats.ALL) {
                this.StatLookup.Add(entry.Key, entry.Value);
                this.StatDetails.Add(entry.Value);
            }
            
            this.SetMinimumSize();
            this.ChangeMainLanguage();
            this.InitMainDataGridView();
            this.UpdateGridRoundName();
            this.UpdateHoopsieLegends();
            
            this.CurrentRound = new List<RoundInfo>();
            
            this.overlay = new Overlay { Text = @"Fall Guys Stats Overlay", StatsForm = this, Icon = this.Icon, ShowIcon = true, BackgroundResourceName = this.CurrentSettings.OverlayBackgroundResourceName, TabResourceName = this.CurrentSettings.OverlayTabResourceName };
            
            Screen screen = this.GetCurrentScreen(this.overlay.Location);
            Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
            Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
            this.screenCenter = new Point(screenLocation.X + (screenSize.Width / 2), screenLocation.Y + (screenSize.Height / 2));
            
            this.logFile.OnParsedLogLines += this.LogFile_OnParsedLogLines;
            this.logFile.OnNewLogFileDate += this.LogFile_OnNewLogFileDate;
            this.logFile.OnError += this.LogFile_OnError;
            this.logFile.OnParsedLogLinesCurrent += this.LogFile_OnParsedLogLinesCurrent;
            this.logFile.StatsForm = this;
            this.logFile.autoChangeProfile = this.CurrentSettings.AutoChangeProfile;
            this.logFile.preventOverlayMouseClicks = this.CurrentSettings.PreventOverlayMouseClicks;
            this.logFile.isDisplayPing = !this.CurrentSettings.HideRoundInfo && (this.CurrentSettings.SwitchBetweenPlayers || this.CurrentSettings.OnlyShowPing);
            
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
            
            this.ReloadProfileMenuItems();
            
            this.SetSystemTrayIcon(this.CurrentSettings.SystemTrayIcon);
            this.UpdateGameExeLocation();
            this.SaveUserSettings();

            this.SetTheme(CurrentTheme);
        }
        
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr h, int x, int y, int width, int height, bool redraw);
        
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
            MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
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
            MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
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
            MoveWindow(handle, location.X, location.Y, e.Bounds.Width, e.Bounds.Height, false);
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
        
        public sealed override string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }
        
        private enum TaskbarPosition { Top, Bottom, Left, Right }
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
            if (this.overlay.IsMouseEnter() && ActiveForm != this) { this.SetCursorPositionCenter(); }
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
        
        public bool IsInternetConnected() {
            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try {
                // Check NCSI test link
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT){
                    return false;
                }

                // Check NCSI DNS IP
                IPHostEntry dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() < 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS) {
                    return false;
                }
            } catch (Exception ex) {
                return false;
            }

            return true;
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            //if (this.Theme == theme) return;
            this.SuspendLayout();
            this.mtt.Theme = theme;
            this.omtt.Theme = theme;
            this.menu.Renderer = theme == MetroThemeStyle.Light ? (ToolStripRenderer)new CustomLightArrowRenderer() : new CustomDarkArrowRenderer();
            this.trayCMenu.Renderer = theme == MetroThemeStyle.Light ? (ToolStripRenderer)new CustomLightArrowRenderer() : new CustomDarkArrowRenderer();
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
                                case "lblCurrentProfile": tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Red : Color.FromArgb(0, 192, 192); break;
                                case "lblTotalTime":
                                    tsl1.Image = theme == MetroThemeStyle.Light ? Properties.Resources.clock_icon : Properties.Resources.clock_gray_icon;
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.Blue : Color.Orange;
                                    //tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray;
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
                                case "lblGoldMedal":
                                case "lblSilverMedal":
                                case "lblBronzeMedal":
                                case "lblPinkMedal":
                                case "lblEliminatedMedal":
                                case "lblKudos":
                                    tsl1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray; break;
                            }
                        } else if (tsi1 is ToolStripSeparator tss1) {
                            tss1.ForeColor = theme == MetroThemeStyle.Light ? Color.DarkSlateGray : Color.DarkGray; break;
                        }
                    }
                }
            }
            
            foreach (object item in this.gridDetails.CMenu.Items) {
                if (item is ToolStripMenuItem tsi) {
                    tsi.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tsi.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    tsi.MouseEnter += this.CMenu_MouseEnter;
                    tsi.MouseLeave += this.CMenu_MouseLeave;
                    switch (tsi.Name) {
                        case "exportItemCSV":
                        case "exportItemHTML":
                        case "exportItemBBCODE":
                        case "exportItemMD":
                            tsi.Image = theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray; break;
                    }
                }
            }

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
                    tsmi.MouseEnter += this.CMenu_MouseEnter;
                    tsmi.MouseLeave += this.CMenu_MouseLeave;
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
                            stsmi1.MouseEnter += this.CMenu_MouseEnter;
                            stsmi1.MouseLeave += this.CMenu_MouseLeave;
                            foreach (var subItem2 in stsmi1.DropDownItems) {
                                if (subItem2 is ToolStripMenuItem stsmi2) {
                                    if (stsmi2.Name.Equals("trayCustomRangeStats")) { stsmi2.Image = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon; }
                                    stsmi2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    stsmi2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    stsmi2.MouseEnter += this.CMenu_MouseEnter;
                                    stsmi2.MouseLeave += this.CMenu_MouseLeave;
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
            this.ResumeLayout(false);
            this.Refresh();
        }
        private void CustomToolStripSeparatorCustom_Paint(Object sender, PaintEventArgs e) {
            ToolStripSeparator separator = (ToolStripSeparator)sender;
            e.Graphics.FillRectangle(new SolidBrush(this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)), 0, 0, separator.Width, separator.Height); // CUSTOM_COLOR_BACKGROUND
            e.Graphics.DrawLine(new Pen(this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray), 30, separator.Height / 2, separator.Width - 4, separator.Height / 2); // CUSTOM_COLOR_FOREGROUND
        }
        private void CMenu_MouseEnter(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = Color.Black;
                switch (tsi.Name) {
                    case "exportItemCSV":
                    case "exportItemHTML":
                    case "exportItemBBCODE":
                    case "exportItemMD":
                        tsi.Image = Properties.Resources.export; break;
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
        private void CMenu_MouseLeave(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                switch (tsi.Name) {
                    case "exportItemCSV":
                    case "exportItemHTML":
                    case "exportItemBBCODE":
                    case "exportItemMD":
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray; break;
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
                if (lblInfo.Name.Equals("lblCurrentProfile")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("profile_change_tooltiptext"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalShows")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("shows_detail_tooltiptext"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalRounds")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("rounds_detail_tooltiptext"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalFinals")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("finals_detail_tooltiptext"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalWins")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("wins_detail_tooltiptext"), this, position);
                } else if (lblInfo.Name.Equals("lblTotalTime")) {
                    this.ShowCustomTooltip(Multilingual.GetWord("stats_detail_tooltiptext"), this, position);
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
                menuItem.Text = profile.ProfileName;
                menuItem.Click += this.menuStats_Click;
                menuItem.Paint += this.menuProfile_Paint;
                menuItem.MouseMove += this.setCursor_MouseMove;
                menuItem.MouseLeave += this.setCursor_MouseLeave;
                this.menuProfile.DropDownItems.Add(menuItem);
                this.ProfileMenuItems.Add(menuItem);
                
                trayItem.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                trayItem.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                trayItem.Size = new Size(180, 22);
                trayItem.Text = profile.ProfileName;
                trayItem.Click += this.menuStats_Click;
                trayItem.Paint += this.menuProfile_Paint;
                this.trayProfile.DropDownItems.Add(trayItem);
                this.ProfileTrayItems.Add(trayItem);
                
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowCheckMargin = true;
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowImageMargin = true;
                
                if (this.CurrentSettings.SelectedProfile == profile.ProfileId) {
                    this.SetCurrentProfileIcon(!string.IsNullOrEmpty(profile.LinkedShowId));
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
                this.CurrentSettings.GroupingCreativeRoundLevels = true;
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
                UpdatedDateFormat = true,
                WinPerDayGraphStyle = 0,
                ShowChangelog = true,
                Visible = true,
                Version = 51
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
            return levelId.StartsWith("wle_s10_round_") || levelId.StartsWith("wle_s10_orig_round_") ||
                   levelId.StartsWith("wle_mrs_bagel_") || levelId.StartsWith("wle_s10_bt_round_") ||
                   levelId.StartsWith("current_wle_fp") || levelId.StartsWith("wle_s10_player_round_wk") ||
                   levelId.StartsWith("wle_s10_cf_round_") || levelId.StartsWith("wle_s10_long_round_") || levelId.Equals("wle_fp2_wk6_01");
        }
        private void UpdateGridRoundName() {
            foreach (KeyValuePair<string, string> item in Multilingual.GetRoundsDictionary()) {
                if (this.IsCreativeLevel(item.Key)) { continue; }
                LevelStats level = this.StatLookup[item.Key];
                level.Name = item.Value;
            }
            this.SortGridDetails(0, true);
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
                if (((ToolStripMenuItem)sender).Name.IndexOf("FallGuysWiki") != -1) {
                    Process.Start(@"https://fallguysultimateknockout.fandom.com/wiki/Fall_Guys:_Ultimate_Knockout_Wiki");
                } else if (((ToolStripMenuItem)sender).Name.IndexOf("FallGuysReddit") != -1) {
                    Process.Start(@"https://www.reddit.com/r/FallGuysGame/");
                } else if (((ToolStripMenuItem)sender).Name.IndexOf("Fallalytics") != -1) {
                    Process.Start(@"https://fallalytics.com/");
                } else if (((ToolStripMenuItem)sender).Name.IndexOf("RollOffClub") != -1) {
                    if (CurrentLanguage == 2) {
                        Process.Start(@"https://rolloff.club/ko/");
                    } else if (CurrentLanguage == 4) {
                        Process.Start(@"https://rolloff.club/zh/");
                    } else {
                        Process.Start(@"https://rolloff.club/");
                    }
                } else if (((ToolStripMenuItem)sender).Name.IndexOf("FallGuysDB") != -1) {
                    Process.Start(@"https://fallguys-db.pages.dev/upcoming_shows");
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuLookHere_MouseEnter(object sender, EventArgs e) {
            Rectangle rectangle = this.menuLookHere.Bounds;
            Point position = new Point(rectangle.Left, rectangle.Bottom + 204);
            this.AllocCustomTooltip(this.cmtt_center_Draw);
            if (((ToolStripMenuItem)sender).Name.Equals("menuFallGuysWiki")) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_wiki_tooltip"), this, position);
            } else if (((ToolStripMenuItem)sender).Name.Equals("menuFallGuysReddit")) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fall_guys_reddit_tooltip"), this, position);
            } else if (((ToolStripMenuItem)sender).Name.Equals("menuFallalytics")) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_fallalytics_tooltip"), this, position);
            } else if (((ToolStripMenuItem)sender).Name.Equals("menuRollOffClub")) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_roll_off_club_tooltip"), this, position);
            } else if (((ToolStripMenuItem)sender).Name.Equals("menuFallGuysDB")) {
                this.ShowCustomTooltip(Multilingual.GetWord("main_todays_show_tooltip"), this, position);
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
                    SetForegroundWindow(FindWindow(null, this.mainWndTitle));
                } else if (this.Visible && this.WindowState != FormWindowState.Minimized) {
                    if (this.isFocused) {
                        this.isFocused = false;
                        this.Hide();
                        //SetForegroundWindow(FindWindow(null, "Fall Guys Stats Overlay"));
                    } else {
                        this.isFocused = true;
                        SetForegroundWindow(FindWindow(null, this.mainWndTitle));
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
                SetForegroundWindow(FindWindow(null, this.mainWndTitle));
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
                if (this.CurrentSettings.AutoLaunchGameOnStartup) {
                    this.LaunchGame(true);
                }
                
                this.menuStats_Click(this.menuProfile.DropDownItems[$@"menuProfile{this.CurrentSettings.SelectedProfile}"], EventArgs.Empty);

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
                rtnStr += CurrentLanguage == 0 || string.IsNullOrEmpty(Multilingual.GetWord(lines[i].Replace("  - ", "message_changelog_").Replace(" ", "_")))
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
                if (this.IsInternetConnected()) {
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
                            string changelog = this.GetApiData("https://api.github.com", "/repos/ShootMe/FallGuysStats/releases/latest").GetProperty("body").GetString();
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
                    this.Opacity = 100;
                } else {
                    this.Hide();
                    this.ShowInTaskbar = true;
                    this.Opacity = 100;
                }
                this.SetMainDataGridViewOrder();

                if (this.WindowState != FormWindowState.Minimized) {
                    this.WindowState = this.CurrentSettings.MaximizedWindowState ? FormWindowState.Maximized : FormWindowState.Normal;
                }
                if (this.CurrentSettings.FormWidth.HasValue) {
                    this.Size = new Size(this.CurrentSettings.FormWidth.Value, this.CurrentSettings.FormHeight.Value);
                }
                if (this.CurrentSettings.FormLocationX.HasValue && IsOnScreen(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value, this.Width, this.Height)) {
                    this.Location = new Point(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value);
                }
                
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                    logPath = this.CurrentSettings.LogPath;
                }
                this.logFile.Start(logPath, LOGNAME);

                this.overlay.ArrangeDisplay(string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.FlippedDisplay : this.CurrentSettings.FixedFlippedDisplay, this.CurrentSettings.ShowOverlayTabs,
                    this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo,
                    this.CurrentSettings.OverlayColor, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayWidth : this.CurrentSettings.OverlayFixedWidth, string.IsNullOrEmpty(this.CurrentSettings.OverlayFixedPosition) ? this.CurrentSettings.OverlayHeight : this.CurrentSettings.OverlayFixedHeight,
                    this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
                if (this.CurrentSettings.OverlayVisible) { this.ToggleOverlay(this.overlay); }
                
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
                                stat.ShowID = nextShowID;
                                stat.Profile = profile;

                                if (stat.UseShareCode && string.IsNullOrEmpty(stat.CreativeShareCode)) {
                                    try {
                                        JsonElement resData = this.GetApiData(this.FALLGUYSDB_API_URL, $"creative/{stat.ShowNameId}.json").GetProperty("data").GetProperty("snapshot");
                                        JsonElement versionMetadata = resData.GetProperty("version_metadata");
                                        string[] onlinePlatformInfo = this.FindCreativeAuthor(resData.GetProperty("author").GetProperty("name_per_platform"));
                                        stat.CreativeShareCode = resData.GetProperty("share_code").GetString();
                                        stat.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                        stat.CreativeAuthor = onlinePlatformInfo[1];
                                        stat.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                        stat.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                        stat.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                        stat.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                        stat.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                        stat.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                        stat.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                        stat.CreativePlayCount = resData.GetProperty("play_count").GetInt32();
                                        stat.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                        //stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").GetProperty("time_limit_seconds").GetInt32();
                                        stat.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                    } catch {
                                        // ignored
                                    }
                                }

                                this.RoundDetails.Insert(stat);
                                this.AllStats.Add(stat);
                                
                                //Below is where reporting to fallaytics happen
                                //Must have enabled the setting to enable tracking
                                //Must not be a private lobby
                                //Must be a game that is played after FallGuysStats started
                                if (this.CurrentSettings.EnableFallalyticsReporting && !stat.PrivateLobby && stat.ShowEnd > this.startupTime) {
                                    Task.Run(() => {
                                        FallalyticsReporter.Report(stat, this.CurrentSettings.FallalyticsAPIKey);
                                    });
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

                            LevelStats newLevel = new LevelStats(stat.Name, this.textInfo.ToTitleCase(roundName), LevelType.Unknown, false, false, 0, 0, 0, null, null);
                            this.StatLookup.Add(stat.Name, newLevel);
                            this.StatDetails.Add(newLevel);
                            this.gridDetails.DataSource = null;
                            this.gridDetails.DataSource = this.StatDetails;
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
        public bool IsCreativeShow(string showId) {
            return showId.StartsWith("show_wle_s10_") ||
                   showId.IndexOf("wle_s10_player_round_wk", StringComparison.OrdinalIgnoreCase) != -1 ||
                   showId.Equals("wle_mrs_bagel") ||
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
        private int GetProfileIdFromName(string profileName) {
            return this.AllProfiles.Find(p => p.ProfileName.Equals(profileName)).ProfileId;
        }
        private string GetCurrentProfileLinkedShowId() {
            string currentProfileLinkedShowId = this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).LinkedShowId;
            return currentProfileLinkedShowId ?? string.Empty;
        }
        private int GetLinkedProfileId(string showId, bool isPrivateLobbies, bool isCreativeShow) {
            if (string.IsNullOrEmpty(showId)) return 0;
            if ("squadcelebration".Equals(showId)) { showId = "squads_4player"; }
            for (int i = 0; i < this.AllProfiles.Count; i++) {
                if (isPrivateLobbies) {
                    if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && this.AllProfiles[i].LinkedShowId.Equals("private_lobbies")) {
                        return this.AllProfiles[i].ProfileId;
                    }
                } else {
                    if (isCreativeShow) {
                        if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && this.AllProfiles[i].LinkedShowId.Equals("fall_guys_creative_mode")) {
                            return this.AllProfiles[i].ProfileId;
                        }
                    } else {
                        if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && showId.IndexOf(this.AllProfiles[i].LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                            return this.AllProfiles[i].ProfileId;
                        }
                    }
                }
            }
            if (isPrivateLobbies) { // return corresponding linked profile when possible if no linked "private_lobbies" profile was found
                for (int j = 0; j < this.AllProfiles.Count; j++) {
                    if (!string.IsNullOrEmpty(this.AllProfiles[j].LinkedShowId) && showId.IndexOf(this.AllProfiles[j].LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                        return this.AllProfiles[j].ProfileId;
                    }
                }
            }
            // return ProfileId 0 if no linked profile was found/matched
            return 0;
        }
        public void SetLinkedProfileMenu(string showId, bool isPrivateLobbies, bool isCreativeShow) {
            if (this.AllProfiles.Count == 0) return;
            if ("squadcelebration".Equals(showId)) showId = "squads_4player";
            if (string.IsNullOrEmpty(showId) && this.GetCurrentProfileLinkedShowId().Equals(showId)) return;
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
                    if (!string.IsNullOrEmpty(this.AllProfiles[j].LinkedShowId) && showId.IndexOf(this.AllProfiles[j].LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                        ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - j];
                        if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                        return;
                    }
                }
            }
            // select ProfileId 0 if no linked profile was found/matched
            for (int k = 0; k < this.AllProfiles.Count; k++) {
                if (this.AllProfiles[k].ProfileId == 0) {
                    ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - k];
                    if (!item.Checked) { this.menuStats_Click(item, EventArgs.Empty); }
                    return;
                }
            }
        }
        private void SetProfileMenu(int profile) {
            ToolStripMenuItem tsmi = this.menuProfile.DropDownItems[$"menuProfile{profile}"] as ToolStripMenuItem;
            if (tsmi.Checked) return;
            this.menuStats_Click(tsmi, EventArgs.Empty);
        }
        private void SetCurrentProfileIcon(bool linked) {
            if (this.CurrentSettings.AutoChangeProfile) {
                this.lblCurrentProfile.Image = linked ? Properties.Resources.profile2_linked_icon : Properties.Resources.profile2_unlinked_icon;
                this.overlay.SetCurrentProfileForeColor(linked ? Color.GreenYellow
                    : string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White
                    : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
            } else {
                this.lblCurrentProfile.Image = Properties.Resources.profile2_icon;
                this.overlay.SetCurrentProfileForeColor(string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White
                    : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
            }
        }
        public string GetRoundNameFromShareCode(string shareCode, LevelType levelType) {
            List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => levelType.CreativeLevelTypeId().Equals(r.Name) && shareCode.Equals(r.ShowNameId));
            return filteredInfo.Count > 0 ? (string.IsNullOrEmpty(filteredInfo[filteredInfo.Count - 1].CreativeTitle) ? shareCode : filteredInfo[filteredInfo.Count - 1].CreativeTitle)
                                          : shareCode;
        }
        public int GetTimeLimitSecondsFromShareCode(string shareCode, LevelType levelType) {
            List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => levelType.CreativeLevelTypeId().Equals(r.Name) && shareCode.Equals(r.ShowNameId));
            return filteredInfo.Count > 0 ? filteredInfo[filteredInfo.Count - 1].CreativeTimeLimitSeconds : 0;
        }
        public StatSummary GetLevelInfo(string name, int levelException, bool useShareCode, LevelType levelType) {
            StatSummary summary = new StatSummary {
                AllWins = 0,
                TotalShows = 0,
                TotalPlays = 0,
                TotalWins = 0,
                TotalFinals = 0
            };
            
            //MatchCollection matches = Regex.Matches(name, @"^\d{4}-\d{4}-\d{4}$");
            //if (matches.Count > 0) { // user creative round
            if (useShareCode) { // user creative round
                List<RoundInfo> filteredInfo = this.AllStats.FindAll(r => r.Profile == this.currentProfile && levelType.CreativeLevelTypeId().Equals(r.Name) && name.Equals(r.ShowNameId));
                int lastShow = -1;
                if (!this.StatLookup.TryGetValue(levelType.CreativeLevelTypeId(), out LevelStats currentLevel)) {
                    currentLevel = new LevelStats(name, name, LevelType.Unknown, false, false, 0, 0, 0, null, null);
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
                            if ((!hasLevelDetails || levelDetails.Type == LevelType.Team || levelException == 2)
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
                    currentLevel = new LevelStats(name, name, LevelType.Unknown, false, false, 0, 0, 0, null, null);
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
                            if ((!hasLevelDetails || levelDetails.Type == LevelType.Team || levelException == 2)
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
                this.lblCurrentProfile.Text = $"{this.GetCurrentProfileName()}";
                //this.lblCurrentProfile.ToolTipText = $"{Multilingual.GetWord("profile_change_tooltiptext")}";
                this.lblTotalShows.Text = $"{this.Shows}{Multilingual.GetWord("main_inning")}";
                if (this.CustomShows > 0) this.lblTotalShows.Text += $" ({Multilingual.GetWord("main_profile_custom")} : {this.CustomShows}{Multilingual.GetWord("main_inning")})";
                //this.lblTotalShows.ToolTipText = $"{Multilingual.GetWord("shows_detail_tooltiptext")}";
                this.lblTotalRounds.Text = $"{this.Rounds}{Multilingual.GetWord("main_round")}";
                if (this.CustomRounds > 0) this.lblTotalRounds.Text += $" ({Multilingual.GetWord("main_profile_custom")} : {this.CustomRounds}{Multilingual.GetWord("main_round")})";
                //this.lblTotalRounds.ToolTipText = $"{Multilingual.GetWord("rounds_detail_tooltiptext")}";
                this.lblTotalTime.Text = $"{(int)this.Duration.TotalHours}{Multilingual.GetWord("main_hour")}{this.Duration:mm}{Multilingual.GetWord("main_min")}{this.Duration:ss}{Multilingual.GetWord("main_sec")}";
                //this.lblTotalTime.ToolTipText = $"{Multilingual.GetWord("stats_detail_tooltiptext")}";
                float winChance = (float)this.Wins * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalWins.Text = $"{this.Wins}{Multilingual.GetWord("main_win")} ({winChance:0.0} %)";
                //this.lblTotalWins.ToolTipText = $"{Multilingual.GetWord("wins_detail_tooltiptext")}";
                float finalChance = (float)this.Finals * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalFinals.Text = $"{this.Finals}{Multilingual.GetWord("main_inning")} ({finalChance:0.0} %)";
                //this.lblTotalFinals.ToolTipText = $"{Multilingual.GetWord("finals_detail_tooltiptext")}";
                this.lblGoldMedal.Text = $"{this.GoldMedals}";
                if (this.CustomGoldMedals > 0) this.lblGoldMedal.Text += $" ({this.CustomGoldMedals})";
                this.lblSilverMedal.Text = $"{this.SilverMedals}";
                if (this.CustomSilverMedals > 0) this.lblSilverMedal.Text += $" ({this.CustomSilverMedals})";
                this.lblBronzeMedal.Text = $"{this.BronzeMedals}";
                if (this.CustomBronzeMedals > 0) this.lblBronzeMedal.Text += $" ({this.CustomBronzeMedals})";
                this.lblPinkMedal.Text = $"{this.PinkMedals}";
                if (this.CustomPinkMedals > 0) this.lblPinkMedal.Text += $" ({this.CustomPinkMedals})";
                this.lblEliminatedMedal.Text = $"{this.EliminatedMedals}";
                if (this.CustomEliminatedMedals > 0) this.lblEliminatedMedal.Text += $" ({this.CustomEliminatedMedals})";
                this.lblGoldMedal.Visible = this.GoldMedals != 0 || this.CustomGoldMedals != 0;
                this.lblSilverMedal.Visible = this.SilverMedals != 0 || this.CustomSilverMedals != 0;
                this.lblBronzeMedal.Visible = this.BronzeMedals != 0 || this.CustomBronzeMedals != 0;
                this.lblPinkMedal.Visible = this.PinkMedals != 0 || this.CustomPinkMedals != 0;
                this.lblEliminatedMedal.Visible = this.EliminatedMedals != 0 || this.CustomEliminatedMedals != 0;
                this.lblKudos.Text = $"{this.Kudos}";
                this.lblKudos.Visible = this.Kudos != 0;
                this.gridDetails.Refresh();
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ShowNotification(string title, string text, ToolTipIcon toolTipIcon, int timeout) {
            if (this.trayIcon.Visible) {
                this.trayIcon.BalloonTipTitle = title;
                this.trayIcon.BalloonTipText = text;
                this.trayIcon.BalloonTipIcon = toolTipIcon;
                this.trayIcon.ShowBalloonTip(timeout);
            } else {
                MetroMessageBox.Show(this, text, title, MessageBoxButtons.OK, toolTipIcon == ToolTipIcon.None ? MessageBoxIcon.None :
                                                                                            toolTipIcon == ToolTipIcon.Error ? MessageBoxIcon.Error :
                                                                                            toolTipIcon == ToolTipIcon.Info ? MessageBoxIcon.Information :
                                                                                            toolTipIcon == ToolTipIcon.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.None);
            }
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
                    sizeOfText += CurrentLanguage == 2 || CurrentLanguage == 4 || CurrentLanguage == 5 ? 5 : 0;
                    break;
                case "Gold":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 1 ? 12 : CurrentLanguage == 4 || CurrentLanguage == 5 ? 5 : 0;
                    break;
                case "Silver":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 || CurrentLanguage == 5 ? 5 : 0;
                    break;
                case "Bronze":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 || CurrentLanguage == 5 ? 5 : 0;
                    break;
                case "Kudos":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Fastest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 || CurrentLanguage == 5 ? 20 : 0;
                    break;
                case "Longest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 || CurrentLanguage == 5 ? 20 : 0;
                    break;
                case "AveFinish":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 || CurrentLanguage == 5 ? 20 : 0;
                    break;
                default:
                    return 0;
            }
            
            return sizeOfText + 24;
        }
        private void InitMainDataGridView() {
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(10);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.dataGridViewCellStyle1.BackColor = Color.LightGray;
            //this.dataGridViewCellStyle1.ForeColor = Color.Black;
            //this.dataGridViewCellStyle1.SelectionBackColor = Color.Cyan;
            //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridDetails.ColumnHeadersHeight = 20;
                
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.dataGridViewCellStyle2.BackColor = Color.White;
            //this.dataGridViewCellStyle2.ForeColor = Color.Black;
            //this.dataGridViewCellStyle2.SelectionBackColor = Color.DeepSkyBlue;
            //this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            this.gridDetails.RowTemplate.Height = 25;

            this.gridDetails.DataSource = this.StatDetails;
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
                LevelStats levelStats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                if (this.CurrentSettings.GroupingCreativeRoundLevels) {
                    if (levelStats.IsCreative && !(levelStats.Id.Equals("creative_race_round") || levelStats.Id.Equals("creative_race_final_round") || levelStats.Id.Equals("user_creative_race_round"))) {
                        return;
                    }
                } else {
                    if (levelStats.IsCreative && (levelStats.Id.Equals("creative_race_round") || levelStats.Id.Equals("creative_race_final_round"))) {
                        return;
                    }
                }
                float fBrightness = 0.85F;
                switch (this.gridDetails.Columns[e.ColumnIndex].Name) {
                    case "RoundIcon":
                        if (levelStats.IsFinal) {
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                ? Color.FromArgb(255, 240, 200)
                                : Color.FromArgb((int)(255 * fBrightness), (int)(240 * fBrightness), (int)(200 * fBrightness));
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.CreativeRace:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(122, 201, 241)
                                    : Color.FromArgb((int)(122 * fBrightness), (int)(201 * fBrightness), (int)(241 * fBrightness));
                                break;
                            case LevelType.Race:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(210, 255, 220)
                                    : Color.FromArgb((int)(210 * fBrightness), (int)(255 * fBrightness), (int)(220 * fBrightness));
                                break;
                            case LevelType.Survival:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(250, 205, 255)
                                    : Color.FromArgb((int)(250 * fBrightness), (int)(205 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Hunt:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(200, 220, 255)
                                    : Color.FromArgb((int)(220 * fBrightness), (int)(220 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Logic:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(230, 250, 255)
                                    : Color.FromArgb((int)(230 * fBrightness), (int)(250 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Team:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(255, 220, 205)
                                    : Color.FromArgb((int)(255 * fBrightness), (int)(220 * fBrightness), (int)(205 * fBrightness));
                                break;
                            case LevelType.Invisibeans:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(255, 255, 255)
                                    : Color.FromArgb((int)(255 * fBrightness), (int)(255 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Unknown:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.LightGray
                                    : Color.DarkGray;
                                break;
                        }
                        break;
                    case "Name":
                        if (levelStats.IsCreative) e.Value = $"🔧 {e.Value}";
                        e.CellStyle.ForeColor = Color.Black;
                        //this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_tooltiptext");
                        if (levelStats.IsFinal) {
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                ? Color.FromArgb(255, 240, 200)
                                : Color.FromArgb((int)(255 * fBrightness), (int)(240 * fBrightness), (int)(200 * fBrightness));
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.CreativeRace:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(122, 201, 241)
                                    : Color.FromArgb((int)(122 * fBrightness), (int)(201 * fBrightness), (int)(241 * fBrightness));
                                break;
                            case LevelType.Race:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(210, 255, 220)
                                    : Color.FromArgb((int)(210 * fBrightness), (int)(255 * fBrightness), (int)(220 * fBrightness));
                                break;
                            case LevelType.Survival:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(250, 205, 255)
                                    : Color.FromArgb((int)(250 * fBrightness), (int)(205 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Hunt:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(200, 220, 255)
                                    : Color.FromArgb((int)(220 * fBrightness), (int)(220 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Logic:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(230, 250, 255)
                                    : Color.FromArgb((int)(230 * fBrightness), (int)(250 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Team:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(255, 220, 205)
                                    : Color.FromArgb((int)(255 * fBrightness), (int)(220 * fBrightness), (int)(205 * fBrightness));
                                break;
                            case LevelType.Invisibeans:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.FromArgb(255, 255, 255)
                                    : Color.FromArgb((int)(255 * fBrightness), (int)(255 * fBrightness), (int)(255 * fBrightness));
                                break;
                            case LevelType.Unknown:
                                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light
                                    ? Color.LightGray
                                    : Color.DarkGray;
                                break;
                        }
                        break;
                    case "Qualified": {
                            float qualifyChance = levelStats.Qualified * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Qualified}";
                            } else {
                                e.Value = levelStats.Qualified;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Gold": {
                            float qualifyChance = levelStats.Gold * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Gold}";
                            } else {
                                e.Value = levelStats.Gold;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Silver": {
                            float qualifyChance = levelStats.Silver * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Silver}";
                            } else {
                                e.Value = levelStats.Silver;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Bronze": {
                            float qualifyChance = levelStats.Bronze * 100f / (levelStats.Played == 0 ? 1 : levelStats.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{levelStats.Bronze}";
                            } else {
                                e.Value = levelStats.Bronze;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "AveFinish":
                        e.Value = levelStats.AveFinish.ToString("m\\:ss\\.ff");
                        break;
                    case "Fastest":
                        e.Value = levelStats.Fastest.ToString("m\\:ss\\.ff");
                        break;
                    case "Longest":
                        e.Value = levelStats.Longest.ToString("m\\:ss\\.ff");
                        break;
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            this.gridDetails.Cursor = Cursors.Default;
            this.HideCustomTooltip(this);
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex >= 0 && (this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")) {
                    this.gridDetails.Cursor = Cursors.Hand;
                    Point cursorPosition = this.PointToClient(Cursor.Position);
                    Point position = new Point(cursorPosition.X + 16, cursorPosition.Y + 16);
                    this.AllocCustomTooltip(this.cmtt_center_Draw);
                    this.ShowCustomTooltip($"{Multilingual.GetWord("level_detail_tooltiptext_prefix")}{this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value}{Multilingual.GetWord("level_detail_tooltiptext_suffix")}", this, position);
                } else {
                    this.gridDetails.Cursor = e.RowIndex >= 0 && !(this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")
                        ? this.Theme == MetroThemeStyle.Light
                            ? new Cursor(Properties.Resources.transform_icon.GetHicon())
                            : new Cursor(Properties.Resources.transform_gray_icon.GetHicon())
                        : Cursors.Default;
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }
        private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            this.VisibleGridRowOfCreativeLevel(this.CurrentSettings.GroupingCreativeRoundLevels);
        }
        private void VisibleGridRowOfCreativeLevel(bool visible) {
            List<LevelStats> levelStatsList = this.gridDetails.DataSource as List<LevelStats>;
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[this.gridDetails.DataSource];  
            currencyManager.SuspendBinding();
            for (var i = 0; i < levelStatsList.Count; i++) {
                LevelStats levelStats = levelStatsList[i];
                if (levelStats.IsCreative && !levelStats.Id.Equals("user_creative_race_round")) {
                    this.gridDetails.Rows[i].Visible = !visible;
                }
                if (levelStats.Id.Equals("creative_race_round") || levelStats.Id.Equals("creative_race_final_round")) {
                    this.gridDetails.Rows[i].Visible = visible;
                }
            }
            currencyManager.ResumeBinding();
        }
        private void SortGridDetails(int columnIndex, bool isInitialize) {
            string columnName = this.gridDetails.Columns[columnIndex].Name;
            SortOrder sortOrder = isInitialize ? SortOrder.None : this.gridDetails.GetSortOrder(columnName);

            this.StatDetails.Sort(delegate (LevelStats one, LevelStats two) {
                LevelType oneType = one.IsFinal ? LevelType.Final : one.Type;
                LevelType twoType = two.IsFinal ? LevelType.Final : two.Type;

                int typeCompare = this.CurrentSettings.IgnoreLevelTypeWhenSorting && sortOrder != SortOrder.None
                    ? 0
                    : ((int)oneType).CompareTo((int)twoType);

                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                int nameCompare = one.Name.CompareTo(two.Name);
                bool percents = this.CurrentSettings.ShowPercentages;
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

            this.gridDetails.DataSource = null;
            this.gridDetails.DataSource = this.StatDetails;
            this.gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            this.SortGridDetails(e.ColumnIndex, false);
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
                for (int i = 0; i < this.StatDetails.Count; i++) {
                    if (this.StatDetails[i].Id.Equals("creative_race_round") || this.StatDetails[i].Id.Equals("creative_race_final_round")) continue;
                    rounds.AddRange(this.StatDetails[i].Stats);
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
                       Text = $@"     {Multilingual.GetWord("level_detail_wins_per_day")} - {this.GetCurrentProfileName()} ({this.GetCurrentFilterName()})",
                       BackImage = Properties.Resources.crown_icon,
                       BackMaxSize = 32,
                       BackImagePadding = new Padding(20, 20, 0, 0)
                   })
            {
                ArrayList dates = new ArrayList();
                ArrayList shows = new ArrayList();
                ArrayList finals = new ArrayList();
                ArrayList wins = new ArrayList();
                if (rounds.Count > 0) {
                    DateTime start = rounds[0].StartLocal;
                    int currentShows = 0;
                    int currentFinals = 0;
                    int currentWins = 0;
                    bool incrementedShows = false;
                    bool incrementedFinals = false;
                    bool incrementedWins = false;
                    foreach (RoundInfo info in rounds.Where(info => !info.PrivateLobby)) {
                        if (info.Round == 1) {
                            currentShows++;
                            incrementedShows = true;
                        }

                        if (info.Crown || info.IsFinal) {
                            currentFinals++;
                            incrementedFinals = true;
                            if (info.Qualified) {
                                currentWins++;
                                incrementedWins = true;
                            }
                        }

                        if (info.StartLocal.Date > start.Date && (incrementedShows || incrementedFinals)) {
                            dates.Add(start.Date.ToOADate());
                            shows.Add(Convert.ToDouble(incrementedShows ? --currentShows : currentShows));
                            finals.Add(Convert.ToDouble(incrementedFinals ? --currentFinals : currentFinals));
                            wins.Add(Convert.ToDouble(incrementedWins ? --currentWins : currentWins));

                            int daysWithoutStats = (int)(info.StartLocal.Date - start.Date).TotalDays - 1;
                            while (daysWithoutStats > 0) {
                                daysWithoutStats--;
                                start = start.Date.AddDays(1);
                                dates.Add(start.ToOADate());
                                shows.Add(0D);
                                finals.Add(0D);
                                wins.Add(0D);
                            }

                            currentShows = incrementedShows ? 1 : 0;
                            currentFinals = incrementedFinals ? 1 : 0;
                            currentWins = incrementedWins ? 1 : 0;
                            start = info.StartLocal;
                        }

                        incrementedShows = false;
                        incrementedFinals = false;
                        incrementedWins = false;
                    }

                    dates.Add(start.Date.ToOADate());
                    shows.Add(Convert.ToDouble(currentShows));
                    finals.Add(Convert.ToDouble(currentFinals));
                    wins.Add(Convert.ToDouble(currentWins));

                    display.manualSpacing = (int)Math.Ceiling(dates.Count / 28D);
                    display.dates = (double[])dates.ToArray(typeof(double));
                    display.shows = (double[])shows.ToArray(typeof(double));
                    display.finals = (double[])finals.ToArray(typeof(double));
                    display.wins = (double[])wins.ToArray(typeof(double));
                } else {
                    dates.Add(DateTime.Now.Date.ToOADate());
                    shows.Add(0D);
                    finals.Add(0D);
                    wins.Add(0D);

                    display.manualSpacing = 1;
                    display.dates = (double[])dates.ToArray(typeof(double));
                    display.shows = (double[])shows.ToArray(typeof(double));
                    display.finals = (double[])finals.ToArray(typeof(double));
                    display.wins = (double[])wins.ToArray(typeof(double));
                }
                
                display.ShowDialog(this);
            }
        }
        private void ShowRoundGraph() {
            using (RoundStatsDisplay roundStatsDisplay = new RoundStatsDisplay {
                       StatsForm = this,
                       Text = $@"     {Multilingual.GetWord("level_detail_stats_by_round")} - {this.GetCurrentProfileName()} ({this.GetCurrentFilterName()})",
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
                if (CurrentLanguage == 1) { // French
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/fr/README.md#table-des-mati%C3%A8res");
                } else if (CurrentLanguage == 2) { // Korean
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/ko/README.md#%EB%AA%A9%EC%B0%A8");
                } else if (CurrentLanguage == 3) { // Japanese
                    Process.Start("https://github.com/ShootMe/FallGuysStats/blob/master/docs/ja/README.md#%E7%9B%AE%E6%AC%A1");
                } else {
                    Process.Start("https://github.com/ShootMe/FallGuysStats#table-of-contents");
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MetroMessageBox.Show(this,
                                        Multilingual.GetWord("message_fall_guys_already_running"),
                                        Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
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
                                        Multilingual.GetWord("message_already_running_caption"),
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            Multilingual.GetWord("message_register_exe_caption"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private string FindEpicGamesShortcutLocation() {
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        private string FindSteamExeLocation() {
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    for (int i = this.ProfileMenuItems.Count - 1; i >= 0; i--) {
                        if (this.ProfileMenuItems[i].Name == button.Name) {
                            this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => {
                                return p.ProfileName == this.ProfileMenuItems[i].Text && !string.IsNullOrEmpty(p.LinkedShowId);
                            }) != -1);
                        }
                        this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                        this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                    }

                    this.currentProfile = this.GetProfileIdFromName(button.Text);
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
                    for (int i = this.ProfileTrayItems.Count - 1; i >= 0; i--) {
                        if (this.ProfileTrayItems[i].Name == button.Name) {
                            this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => {
                                return p.ProfileName == this.ProfileTrayItems[i].Text && !string.IsNullOrEmpty(p.LinkedShowId);
                            }) != -1);
                        }
                        this.ProfileTrayItems[i].Checked = this.ProfileTrayItems[i].Name == button.Name;
                        this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                    }
                    
                    this.currentProfile = this.GetProfileIdFromName(button.Text);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuUpdate_Click(object sender, EventArgs e) {
            try {
                if (this.IsInternetConnected()) {
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
        public string[] GetCountryCode(string host) {
            string[] code = { string.Empty, string.Empty, string.Empty };
            using (ApiWebClient web = new ApiWebClient()) {
                string resStr = web.DownloadString($"{this.IP2C_ORG_URL}{host}");
                string[] resArr = resStr.Split(';');
                if ("1".Equals(resArr[0])) {
                    code[0] = resArr[1]; // alpha-2 code
                    code[1] = resArr[2]; // alpha-3 code
                    code[2] = resArr[3]; // a full country name
                } else if ("2".Equals(resArr[0])) {
                    code[0] = "UNKNOWN";
                    code[1] = "UNKNOWN";
                    code[2] = "Unknown";
                }
            }
            return code;
        }
        public JsonElement GetApiData(string apiUrl, string apiEndPoint) {
            JsonElement resJroot;
            using (ApiWebClient web = new ApiWebClient()) {
                string responseJsonString = web.DownloadString($"{apiUrl}{apiEndPoint}");
                JsonDocument jdom = JsonDocument.Parse(responseJsonString);
                resJroot = jdom.RootElement;
            }
            return resJroot;
        }
#if AllowUpdate
        public void ChangeStateForAvailableNewVersion(string newVersion) {
            // this.timeSwitcherForCheckUpdate = DateTime.UtcNow;
            this.isAvailableNewVersion = true;
            this.availableNewVersion = newVersion;
            this.menuUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
            this.trayUpdate.Image = CurrentTheme == MetroThemeStyle.Light ? Properties.Resources.github_update_icon : Properties.Resources.github_update_gray_icon;
        }
        public void CheckForNewVersion() {
            using (ZipWebClient web = new ZipWebClient()) {
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
            }
        }
#endif
        private bool CheckForUpdate(bool isSilent) {
#if AllowUpdate
            using (ZipWebClient web = new ZipWebClient()) {
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
                                progress.DownloadUrl = this.FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL;
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
            this.overlay.Hide();
            this.overlay.ShowInTaskbar = !topMost;
            this.overlay.Show();
        }
        public void SetAutoChangeProfile(bool autoChangeProfile) {
            this.CurrentSettings.AutoChangeProfile = autoChangeProfile;
            this.logFile.autoChangeProfile = autoChangeProfile;
            this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileId == this.GetCurrentProfileId() && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
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
            this.MinimumSize = new Size(CurrentLanguage == 0 ? 720 :
                                        CurrentLanguage == 1 ? 845 :
                                        CurrentLanguage == 2 ? 650 :
                                        CurrentLanguage == 3 ? 795 : 600, 350);
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
                        if (this.currentLanguage != CurrentLanguage) {
                            this.SetMinimumSize();
                            this.ChangeMainLanguage();
                            this.UpdateTotals();
                            this.gridDetails.ChangeContextMenuLanguage();
                            this.UpdateGridRoundName();
                            this.overlay.ChangeLanguage();
                        }
                        this.SortGridDetails(0, true);
                        this.ChangeLaunchPlatformLogo(this.CurrentSettings.LaunchPlatform);
                        this.UpdateHoopsieLegends();
                        this.SetOverlayTopMost(!this.CurrentSettings.OverlayNotOnTop);
                        this.SetOverlayBackgroundOpacity(this.CurrentSettings.OverlayBackgroundOpacity);
                        this.overlay.SetBackgroundResourcesName(this.CurrentSettings.OverlayBackgroundResourceName, this.CurrentSettings.OverlayTabResourceName);
                        this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileId == this.GetCurrentProfileId() && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                        this.Refresh();
                        this.logFile.autoChangeProfile = this.CurrentSettings.AutoChangeProfile;
                        this.logFile.preventOverlayMouseClicks = this.CurrentSettings.PreventOverlayMouseClicks;
                        this.logFile.isDisplayPing = !this.CurrentSettings.HideRoundInfo && (this.CurrentSettings.SwitchBetweenPlayers || this.CurrentSettings.OnlyShowPing);
                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(this.CurrentSettings.LogPath) ||
                            (!string.IsNullOrEmpty(lastLogPath) && lastLogPath.Equals(this.CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await this.logFile.Stop();
                            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                                logPath = this.CurrentSettings.LogPath;
                            }
                            this.logFile.Start(logPath, LOGNAME);
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
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }
        private void menuOverlay_Click(object sender, EventArgs e) {
            this.ToggleOverlay(this.overlay);
        }
        public void ToggleOverlay(Overlay ol) {
            if (ol.Visible) {
                ol.Hide();
                this.menuOverlay.Image = Properties.Resources.stat_gray_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                this.trayOverlay.Image = Properties.Resources.stat_gray_icon;
                this.trayOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                if (!ol.IsFixed()) {
                    this.CurrentSettings.OverlayLocationX = ol.Location.X;
                    this.CurrentSettings.OverlayLocationY = ol.Location.Y;
                    this.CurrentSettings.OverlayWidth = ol.Width;
                    this.CurrentSettings.OverlayHeight = ol.Height;
                }
                this.CurrentSettings.OverlayVisible = false;
                this.SaveUserSettings();
            } else {
                ol.Show();
                ol.TopMost = !this.CurrentSettings.OverlayNotOnTop;
                ol.ShowInTaskbar = this.CurrentSettings.OverlayNotOnTop;
                this.menuOverlay.Image = Properties.Resources.stat_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.trayOverlay.Image = Properties.Resources.stat_icon;
                this.trayOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.CurrentSettings.OverlayVisible = true;
                this.SaveUserSettings();

                if (ol.IsFixed()) {
                    if (this.CurrentSettings.OverlayFixedPositionX.HasValue &&
                        this.IsOnScreen(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value, ol.Width, ol.Height))
                    {
                        ol.FlipDisplay(this.CurrentSettings.FixedFlippedDisplay);
                        ol.Location = new Point(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value);
                    } else {
                        ol.Location = this.Location;
                    }
                } else {
                    ol.Location = this.CurrentSettings.OverlayLocationX.HasValue && this.IsOnScreen(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value, ol.Width, ol.Height)
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
                    //editProfiles.Icon = this.Icon;
                    editProfiles.StatsForm = this;
                    editProfiles.Profiles = this.AllProfiles;
                    editProfiles.AllStats = this.RoundDetails.FindAll().ToList();
                    this.EnableInfoStrip(false);
                    this.EnableMainMenu(false);
                    editProfiles.ShowDialog(this);
                    this.EnableInfoStrip(true);
                    this.EnableMainMenu(true);
                    lock (this.StatsDB) {
                        this.StatsDB.BeginTrans();
                        this.AllProfiles = editProfiles.Profiles;
                        this.Profiles.DeleteAll();
                        this.Profiles.InsertBulk(this.AllProfiles);
                        if (editProfiles.AllStats.Count != this.RoundDetails.Count()) {
                            this.AllStats = editProfiles.AllStats;
                            this.RoundDetails.DeleteAll();
                            this.RoundDetails.InsertBulk(this.AllStats);
                            this.AllStats.Clear();
                        }
                        this.StatsDB.Commit();
                    }
                    this.ReloadProfileMenuItems();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableInfoStrip(true);
                this.EnableMainMenu(true);
            }
        }
        private void menuLaunchFallGuys_Click(object sender, EventArgs e) {
            try {
                this.LaunchGame(false);
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool IsOnScreen(int x, int y, int w, int h) {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens) {
                if (screen.WorkingArea.Contains(new Point(x, y)) || screen.WorkingArea.Contains(new Point(x + w, y + h))) {
                    return true;
                }
            }
            return false;
        }
        public Screen GetCurrentScreen(Point location) {
            Screen[] scr = Screen.AllScreens;
            Screen screen = Screen.PrimaryScreen;
            foreach (Screen s in scr) {
                if (s.WorkingArea.Contains(location)) {
                    screen = s;
                    break;
                }
            }
            return screen;
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
            this.currentLanguage = CurrentLanguage;
            this.mainWndTitle = $@"     {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.trayIcon.Text = this.mainWndTitle.Trim();
            this.Text = this.mainWndTitle;
            this.menu.Font = Overlay.GetMainFont(12);
            this.menuLaunchFallGuys.Font = Overlay.GetMainFont(12);
            this.infoStrip.Font = Overlay.GetMainFont(13);
            this.infoStrip2.Font = Overlay.GetMainFont(13);
            
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(10);
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(12);
            
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
            this.trayFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
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
            this.menuFallGuysDB.Text = Multilingual.GetWord("main_fall_guys_db");
        }
    }
}