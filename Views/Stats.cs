using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using MetroFramework;
namespace FallGuysStats {
    public partial class Stats : MetroFramework.Forms.MetroForm {
        [STAThread]
        static void Main() {
            try {
                if (!IsAlreadyRunning()) {
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
                        MessageBox.Show(@"Fall Guys Stats is already running.", @"Already Running");
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, @"Process Exception");
                return true;
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
            new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 11, 22, 0, 0, 0, DateTimeKind.Utc)
        };
        private static DateTime SeasonStart, WeekStart, DayStart;
        private static DateTime SessionStart = DateTime.UtcNow;
        public static bool InShow = false;
        public static bool EndedShow = false;
        public static int LastServerPing = 0;
        public static int CurrentLanguage = 0;
        public static Bitmap ImageOpacity(Image imgData, float opacity) {
            Bitmap bmpTmp = new Bitmap(imgData.Width, imgData.Height);
            Graphics gp = Graphics.FromImage(bmpTmp);
            ColorMatrix clrMatrix = new ColorMatrix();
            clrMatrix.Matrix33 = opacity;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gp.DrawImage(imgData, new Rectangle(0, 0, bmpTmp.Width, bmpTmp.Height), 0, 0, imgData.Width, imgData.Height, GraphicsUnit.Pixel, imgAttribute);
            gp.Dispose();
            return bmpTmp;
        }
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
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
        public ILiteCollection<Profiles> Profiles;
        public List<Profiles> AllProfiles = new List<Profiles>();
        public List<ToolStripMenuItem> ProfileMenuItems = new List<ToolStripMenuItem>();
        public UserSettings CurrentSettings;
        private Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        private DateTime startupTime = DateTime.UtcNow;
        private int askedPreviousShows = 0;
        private TextInfo textInfo;
        private int currentProfile;
        private Color infoStripForeColor;
        
        private Image numberOne = ImageOpacity(Properties.Resources.number_1,   0.5F);
        private Image numberTwo = ImageOpacity(Properties.Resources.number_2,   0.5F);
        private Image numberThree = ImageOpacity(Properties.Resources.number_3, 0.5F);
        private Image numberFour = ImageOpacity(Properties.Resources.number_4,  0.5F);
        private Image numberFive = ImageOpacity(Properties.Resources.number_5,  0.5F);
        private Image numberSix = ImageOpacity(Properties.Resources.number_6,   0.5F);
        private Image numberSeven = ImageOpacity(Properties.Resources.number_7, 0.5F);
        private Image numberEight = ImageOpacity(Properties.Resources.number_8, 0.5F);
        private Image numberNine = ImageOpacity(Properties.Resources.number_9,  0.5F);

        public Stats() {
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
                } catch {
                    this.UserSettings.DeleteAll();
                    this.CurrentSettings = GetDefaultSettings();
                    this.UserSettings.Insert(this.CurrentSettings);
                }
            }
            this.StatsDB.Commit();
            
            this.InitializeComponent();
            
            this.textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            this.logFile.OnParsedLogLines += this.LogFile_OnParsedLogLines;
            this.logFile.OnNewLogFileDate += this.LogFile_OnNewLogFileDate;
            this.logFile.OnError += this.LogFile_OnError;
            this.logFile.OnParsedLogLinesCurrent += this.LogFile_OnParsedLogLinesCurrent;
            this.logFile.StatsForm = this;

            foreach (var entry in LevelStats.ALL) {
                this.StatDetails.Add(entry.Value);
                this.StatLookup.Add(entry.Key, entry.Value);
            }

            this.InitMainDataGridView();
            
            this.RoundDetails = this.StatsDB.GetCollection<RoundInfo>("RoundDetails");
            this.Profiles = this.StatsDB.GetCollection<Profiles>("Profiles");

            this.StatsDB.BeginTrans();

            if (this.Profiles.Count() == 0) {
                using (SelectLanguage initLanguageForm = new SelectLanguage()) {
                    initLanguageForm.Icon = this.Icon;
                    if (initLanguageForm.ShowDialog(this) == DialogResult.OK) {
                        CurrentLanguage = initLanguageForm.selectedLanguage;
                        Overlay.SetDefaultFont(CurrentLanguage, 18);
                        this.CurrentSettings.Multilingual = initLanguageForm.selectedLanguage;
                        this.Profiles.Insert(new Profiles { ProfileId = 3, ProfileName = Multilingual.GetWord("main_profile_custom"), ProfileOrder = 4, LinkedShowId = "private_lobbies" });
                        this.Profiles.Insert(new Profiles { ProfileId = 2, ProfileName = Multilingual.GetWord("main_profile_squad"), ProfileOrder = 3, LinkedShowId = "squads_4player" });
                        this.Profiles.Insert(new Profiles { ProfileId = 1, ProfileName = Multilingual.GetWord("main_profile_duo"), ProfileOrder = 2, LinkedShowId = "squads_2player_template" });
                        this.Profiles.Insert(new Profiles { ProfileId = 0, ProfileName = Multilingual.GetWord("main_profile_solo"), ProfileOrder = 1, LinkedShowId = "main_show" });
                    }
                }
            }
            
            this.ChangeMainLanguage();
            this.Text = $"　  {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.BackImage = this.Icon.ToBitmap();
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(18, 18, 0, 0);
            
            this.UpdateGridRoundName();
            this.UpdateHoopsieLegends();

            this.RoundDetails.EnsureIndex(x => x.Name);
            this.RoundDetails.EnsureIndex(x => x.ShowID);
            this.RoundDetails.EnsureIndex(x => x.Round);
            this.RoundDetails.EnsureIndex(x => x.Start);
            this.RoundDetails.EnsureIndex(x => x.InParty);
            this.StatsDB.Commit();

            this.UpdateDatabaseVersion();
            
            this.CurrentRound = new List<RoundInfo>();

            this.overlay = new Overlay { StatsForm = this, Icon = this.Icon, ShowIcon = true, BackgroundResourceName = this.CurrentSettings.OverlayBackgroundResourceName, TabResourceName = this.CurrentSettings.OverlayTabResourceName};
            string fixedPosition = this.CurrentSettings.OverlayFixedPosition;
            this.overlay.SetFixedPosition(
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("ne"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("nw"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("se"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("sw"),
                    !string.IsNullOrEmpty(fixedPosition) && fixedPosition.Equals("free")
                );
            if (this.overlay.IsFixed()) this.overlay.Cursor = Cursors.Default;
            this.overlay.Show();
            this.overlay.Visible = false;
            this.overlay.StartTimer();

            this.UpdateGameExeLocation();
            if (this.CurrentSettings.AutoLaunchGameOnStartup) {
                this.LaunchGame(true);
            }

            this.RemoveUpdateFiles();
            this.infoStrip.Renderer = new MySr();
            this.ReloadProfileMenuItems();
            
            this.SuspendLayout();
            this.SetTheme(this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;

            foreach (var item in this.gridDetails.CMenu.Items) {
                if (item is ToolStripMenuItem tsi) {
                    tsi.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17,17,17);
                    tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    tsi.MouseEnter += CMenu_MouseEnter;
                    tsi.MouseLeave += CMenu_MouseLeave;
                    if (tsi.Name.Equals("exportItemCSV")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemHTML")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemBBCODE")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemMD")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    }
                }
            }

            if (this.Theme == MetroThemeStyle.Light) {
                this.dataGridViewCellStyle1.BackColor = Color.LightGray;
                this.dataGridViewCellStyle1.ForeColor = Color.Black;
                this.dataGridViewCellStyle1.SelectionBackColor = Color.Cyan;
                //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
                this.dataGridViewCellStyle2.BackColor = Color.White;
                this.dataGridViewCellStyle2.ForeColor = Color.Black;
                this.dataGridViewCellStyle2.SelectionBackColor = Color.DeepSkyBlue;
                this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            } else if (this.Theme == MetroThemeStyle.Dark) {
                this.dataGridViewCellStyle1.BackColor = Color.FromArgb(2,2,2);
                this.dataGridViewCellStyle1.ForeColor = Color.DarkGray;
                this.dataGridViewCellStyle1.SelectionBackColor = Color.DarkSlateBlue;
                //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
                this.dataGridViewCellStyle2.BackColor = Color.FromArgb(49,51,56);
                this.dataGridViewCellStyle2.ForeColor = Color.WhiteSmoke;
                this.dataGridViewCellStyle2.SelectionBackColor = Color.PaleGreen;
                this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            }
            
            if (this.Theme == MetroThemeStyle.Light) {
                foreach (Control c1 in Controls) {
                    if (c1 is MenuStrip ms1) {
                        foreach (ToolStripMenuItem tsmi1 in ms1.Items) {
                            if (tsmi1.Name.Equals("menuSettings")) {
                                tsmi1.Image = Properties.Resources.setting_icon;
                            } else if (tsmi1.Name.Equals("menuFilters")) {
                                tsmi1.Image = Properties.Resources.filter_icon;
                            } else if (tsmi1.Name.Equals("menuProfile")) {
                                tsmi1.Image = Properties.Resources.profile_icon;
                            } else if (tsmi1.Name.Equals("menuOverlay")) {
                            } else if (tsmi1.Name.Equals("menuUpdate")) {
                                tsmi1.Image = Properties.Resources.github_icon;
                            } else if (tsmi1.Name.Equals("menuHelp")) {
                                tsmi1.Image = Properties.Resources.github_icon;
                            } else if (tsmi1.Name.Equals("menuLaunchFallGuys")) {
                            }
                            tsmi1.ForeColor = Color.Black;
                            tsmi1.MouseEnter += this.menu_MouseEnter;
                            tsmi1.MouseLeave += this.menu_MouseLeave;
                            foreach (ToolStripMenuItem tsmi2 in tsmi1.DropDownItems) {
                                if (tsmi2.Name.Equals("menuEditProfiles")) { tsmi2.Image = Properties.Resources.setting_icon; }
                                tsmi2.ForeColor = Color.Black;
                                tsmi2.BackColor = Color.White;
                                tsmi2.MouseEnter += this.menu_MouseEnter;
                                tsmi2.MouseLeave += this.menu_MouseLeave;
                                foreach (ToolStripMenuItem tsmi3 in tsmi2.DropDownItems) {
                                    tsmi3.ForeColor = Color.Black;
                                    tsmi3.BackColor = Color.White;
                                    tsmi3.MouseEnter += this.menu_MouseEnter;
                                    tsmi3.MouseLeave += this.menu_MouseLeave;
                                }
                            }
                        }
                    } else if (c1 is ToolStrip ts1) {
                        ts1.BackColor = Color.Transparent;
                        foreach (ToolStripLabel tsl1 in ts1.Items) {
                            if (tsl1.Name.Equals("lblCurrentProfile")) {
                                tsl1.ForeColor = Color.Red;
                            } else if (tsl1.Name.Equals("lblTotalTime")) {
                                tsl1.Image = Properties.Resources.clock_icon;
                                tsl1.ForeColor = Color.Black;
                            } else if (tsl1.Name.Equals("lblTotalShows")) {
                                tsl1.ForeColor = Color.Blue;
                            } else if (tsl1.Name.Equals("lblTotalRounds")) {
                                tsl1.ForeColor = Color.Blue;
                            } else if (tsl1.Name.Equals("lblTotalWins")) {
                                tsl1.ForeColor = Color.Blue;
                            } else if (tsl1.Name.Equals("lblTotalFinals")) {
                                tsl1.Image = Properties.Resources.final_icon;
                                tsl1.ForeColor = Color.Blue;
                            } else if (tsl1.Name.Equals("lblKudos")) {
                                tsl1.ForeColor = Color.Black;
                            }
                        }
                    }
                }
            } else if (this.Theme == MetroThemeStyle.Dark) {
                foreach (Control c1 in Controls) {
                    if (c1 is MenuStrip ms1) {
                        foreach (ToolStripMenuItem tsmi1 in ms1.Items) {
                            if (tsmi1.Name.Equals("menuSettings")) {
                                tsmi1.Image = Properties.Resources.setting_gray_icon;
                            } else if (tsmi1.Name.Equals("menuFilters")) {
                                tsmi1.Image = Properties.Resources.filter_gray_icon;
                            } else if (tsmi1.Name.Equals("menuProfile")) {
                                tsmi1.Image = Properties.Resources.profile_gray_icon;
                            } else if (tsmi1.Name.Equals("menuOverlay")) {
                            } else if (tsmi1.Name.Equals("menuUpdate")) {
                                tsmi1.Image = Properties.Resources.github_gray_icon;
                            } else if (tsmi1.Name.Equals("menuHelp")) {
                                tsmi1.Image = Properties.Resources.github_gray_icon;
                            } else if (tsmi1.Name.Equals("menuLaunchFallGuys")) {
                            }
                            tsmi1.ForeColor = Color.DarkGray;
                            tsmi1.MouseEnter += this.menu_MouseEnter;
                            tsmi1.MouseLeave += this.menu_MouseLeave;
                            foreach (ToolStripMenuItem tsmi2 in tsmi1.DropDownItems) {
                                if (tsmi2.Name.Equals("menuEditProfiles")) { tsmi2.Image = Properties.Resources.setting_gray_icon; }
                                tsmi2.ForeColor = Color.DarkGray;
                                tsmi2.BackColor = Color.FromArgb(17,17,17);
                                tsmi2.MouseEnter += this.menu_MouseEnter;
                                tsmi2.MouseLeave += this.menu_MouseLeave;
                                foreach (ToolStripMenuItem tsmi3 in tsmi2.DropDownItems) {
                                    tsmi3.ForeColor = Color.DarkGray;
                                    tsmi3.BackColor = Color.FromArgb(17,17,17);
                                    tsmi3.MouseEnter += this.menu_MouseEnter;
                                    tsmi3.MouseLeave += this.menu_MouseLeave;
                                }
                            }
                        }
                    } else if (c1 is ToolStrip ts1) {
                        ts1.BackColor = Color.Transparent;
                        foreach (ToolStripLabel tsl1 in ts1.Items) {
                            if (tsl1.Name.Equals("lblCurrentProfile")) {
                                tsl1.ForeColor = Color.FromArgb(0,192,192);
                            } else if (tsl1.Name.Equals("lblTotalTime")) {
                                tsl1.Image = Properties.Resources.clock_gray_icon;
                                tsl1.ForeColor = Color.DarkGray;
                            } else if (tsl1.Name.Equals("lblTotalShows")) {
                                tsl1.ForeColor = Color.Orange;
                            } else if (tsl1.Name.Equals("lblTotalRounds")) {
                                tsl1.ForeColor = Color.Orange;
                            } else if (tsl1.Name.Equals("lblTotalWins")) {
                                tsl1.ForeColor = Color.Orange;
                            } else if (tsl1.Name.Equals("lblTotalFinals")) {
                                tsl1.Image = Properties.Resources.final_gray_icon;
                                tsl1.ForeColor = Color.Orange;
                            } else if (tsl1.Name.Equals("lblKudos")) {
                                tsl1.ForeColor = Color.DarkGray;
                            }
                        }
                    }
                }
            }
            this.Invalidate(true);
        }
        private void CMenu_MouseEnter(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = Color.Black;
                if (tsi.Name.Equals("exportItemCSV")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemHTML")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemBBCODE")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemMD")) {
                    tsi.Image = Properties.Resources.export;
                }
            }
        }
        private void CMenu_MouseLeave(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                if (tsi.Name.Equals("exportItemCSV")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemHTML")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemBBCODE")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemMD")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                }
            }
        }
        private void menu_MouseEnter(object sender, EventArgs e) {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi.Name.Equals("menuSettings")) {
                tsmi.Image = Properties.Resources.setting_icon;
            } else if (tsmi.Name.Equals("menuFilters")) {
                tsmi.Image = Properties.Resources.filter_icon;
            } else if (tsmi.Name.Equals("menuProfile")) {
                tsmi.Image = Properties.Resources.profile_icon;
            } else if (tsmi.Name.Equals("menuOverlay")) {
            } else if (tsmi.Name.Equals("menuUpdate")) {
                tsmi.Image = Properties.Resources.github_icon;
            } else if (tsmi.Name.Equals("menuHelp")) {
                tsmi.Image = Properties.Resources.github_icon;
            } else if (tsmi.Name.Equals("menuLaunchFallGuys")) {
            } else if (tsmi.Name.Equals("menuEditProfiles")) {
                tsmi.Image = Properties.Resources.setting_icon;
            }
            tsmi.ForeColor = Color.Black;
        }
        private void menu_MouseLeave(object sender, EventArgs e) {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi.Name.Equals("menuSettings")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.setting_gray_icon : Properties.Resources.setting_icon;
            } else if (tsmi.Name.Equals("menuFilters")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.filter_gray_icon : Properties.Resources.filter_icon;
            } else if (tsmi.Name.Equals("menuProfile")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.profile_gray_icon : Properties.Resources.profile_icon;
            } else if (tsmi.Name.Equals("menuOverlay")) {
            } else if (tsmi.Name.Equals("menuUpdate")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.github_gray_icon : Properties.Resources.github_icon;
            } else if (tsmi.Name.Equals("menuHelp")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.github_gray_icon : Properties.Resources.github_icon;
            } else if (tsmi.Name.Equals("menuLaunchFallGuys")) {
            } else if (tsmi.Name.Equals("menuEditProfiles")) {
                tsmi.Image = this.Theme == MetroThemeStyle.Dark ? Properties.Resources.setting_gray_icon : Properties.Resources.setting_icon;
            }
            tsmi.ForeColor = this.Theme == MetroThemeStyle.Dark ? Color.DarkGray : Color.Black;
        }
        private void infoStrip_MouseEnter(object sender, EventArgs e) {
            this.Cursor = Cursors.Hand;
            if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripLabel") {
                var lblInfo = sender as ToolStripLabel;
                if (lblInfo != null) {
                    this.infoStripForeColor = lblInfo.ForeColor;
                    if (lblInfo.Name == "lblCurrentProfile") {
                        lblInfo.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(245, 154, 168) : Color.FromArgb(231, 251, 255);
                    } else {
                        lblInfo.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(147, 174, 248) : Color.FromArgb(255, 250, 244);
                    }
                }
            }
        }
        private void infoStrip_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
            if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripLabel") {
                var lblInfo = sender as ToolStripLabel;
                if (lblInfo != null) {
                    lblInfo.ForeColor = this.infoStripForeColor;
                }
            }
        }

        public void ReloadProfileMenuItems() {
            this.ProfileMenuItems.Clear();
            this.menuProfile.DropDownItems.Clear();
            this.menuProfile.DropDownItems.Add(this.menuEditProfiles);
            this.AllProfiles.Clear();
            this.AllProfiles = this.Profiles.FindAll().ToList();
            int profileNumber = 0; 
            for (int i = this.AllProfiles.Count - 1; i >= 0; i--) {
                Profiles profile = this.AllProfiles[i];
                var menuItem = new ToolStripMenuItem();
                menuItem.Checked = this.CurrentSettings.SelectedProfile == profile.ProfileId;
                menuItem.CheckOnClick = true;
                menuItem.CheckState = this.CurrentSettings.SelectedProfile == profile.ProfileId ? CheckState.Checked : CheckState.Unchecked;
                menuItem.Name = "menuProfile" + profile.ProfileId;
                switch (profileNumber++) {
                    case 0:
                        menuItem.Image = this.numberOne;
                        break;
                    case 1:
                        menuItem.Image = this.numberTwo;
                        break;
                    case 2:
                        menuItem.Image = this.numberThree;
                        break;
                    case 3:
                        menuItem.Image = this.numberFour;
                        break;
                    case 4:
                        menuItem.Image = this.numberFive;
                        break;
                    case 5:
                        menuItem.Image = this.numberSix;
                        break;
                    case 6:
                        menuItem.Image = this.numberSeven;
                        break;
                    case 7:
                        menuItem.Image = this.numberEight;
                        break;
                    case 8:
                        menuItem.Image = this.numberNine;
                        break;
                }
                menuItem.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                menuItem.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17,17,17);
                menuItem.Size = new Size(180, 22);
                menuItem.Text = profile.ProfileName;
                menuItem.Click += this.menuStats_Click;
                menuItem.Paint += this.menuProfile_Paint;
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowCheckMargin = true;
                //((ToolStripDropDownMenu)menuProfile.DropDown).ShowImageMargin = true;
                this.menuProfile.DropDownItems.Add(menuItem);
                this.ProfileMenuItems.Add(menuItem);
                if (this.CurrentSettings.SelectedProfile == profile.ProfileId) {
                    this.SetCurrentProfileIcon(!string.IsNullOrEmpty(profile.LinkedShowId));
                    menuItem.PerformClick();
                }
            }
        }
        
        private void menuProfile_Paint(object sender, PaintEventArgs e) {
            //e.Graphics.DrawRectangle(Pens.Red, ((ToolStripMenuItem)sender).ContentRectangle);
            if (this.AllProfiles.FindIndex(profile => profile.ProfileId.ToString() == ((ToolStripMenuItem)sender).Name.Substring(11) && !string.IsNullOrEmpty(profile.LinkedShowId)) != -1) {
                e.Graphics.DrawImage(this.CurrentSettings.AutoChangeProfile ? Properties.Resources.link_on_icon : this.Theme == MetroThemeStyle.Light ? Properties.Resources.link_icon : Properties.Resources.link_gray_icon, 20, 5, 13, 13);
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

                    int index = 0;
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

                    int index = 0;
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

                    int index = 0;
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

                    int index = 0;
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

                    int index = 0;
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
                        if (this.StatLookup.TryGetValue(info.Name, out LevelStats stats)) {
                            info.IsFinal = stats.IsFinal && (info.Name != "round_floor_fall" || info.Round >= 3 || (i > 0 && this.AllStats[i - 1].Name != "round_floor_fall"));
                        } else {
                            info.IsFinal = false;
                        }
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
        }
        private UserSettings GetDefaultSettings() {
            return new UserSettings {
                ID = 1,
                Theme = 0,
                CycleTimeSeconds = 5,
                FilterType = 0,
                SelectedProfile = 0,
                FlippedDisplay = false,
                LogPath = null,
                OverlayBackground = 0,
                OverlayBackgroundResourceName = string.Empty,
                OverlayTabResourceName = string.Empty,
                IsOverlayBackgroundCustomized = false,
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
                PreviousWins = 0,
                WinsFilter = 1,
                QualifyFilter = 1,
                FastestFilter = 1,
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
                AutoLaunchGameOnStartup = false,
                GameExeLocation = string.Empty,
                GameShortcutLocation = string.Empty,
                IgnoreLevelTypeWhenSorting = false,
                UpdatedDateFormat = true,
                Version = 26
            };
        }
        private void UpdateHoopsieLegends() {
            LevelStats level = this.StatLookup["round_hoops_blockade_solo"];
            string newName = this.CurrentSettings.HoopsieHeros ? Multilingual.GetWord("main_hoopsie_heroes") : Multilingual.GetWord("main_hoopsie_legends");
            if (level.Name != newName) {
                level.Name = newName;
                this.gridDetails.Invalidate();
            }
        }
        private void UpdateGridRoundName() {
            Dictionary<string, string> rounds = Multilingual.GetRoundsDictionary();
            foreach(KeyValuePair<string, string> item in rounds) {
                LevelStats level = StatLookup[item.Key];
                level.Name = item.Value;
            }
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
                LevelStats calculator = StatDetails[i];
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

                            if (info.ShowID == lastAddedShowID || (IsInStatsFilter(info.Start) && IsInPartyFilter(info))) {
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
        private void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                if (!this.overlay.Disposing && !this.overlay.IsDisposed && !this.IsDisposed && !this.Disposing) {
                    if (this.overlay.Visible) {
                        if (!this.overlay.IsFixed()) {
                            this.CurrentSettings.OverlayLocationX = this.overlay.Location.X;
                            this.CurrentSettings.OverlayLocationY = this.overlay.Location.Y;
                            this.CurrentSettings.OverlayWidth = this.overlay.Width;
                            this.CurrentSettings.OverlayHeight = this.overlay.Height;
                        }
                    }
                    this.CurrentSettings.FilterType = this.menuAllStats.Checked ? 0 : this.menuSeasonStats.Checked ? 1 : this.menuWeekStats.Checked ? 2 : this.menuDayStats.Checked ? 3 : 4;
                    this.CurrentSettings.SelectedProfile = this.currentProfile;

                    this.CurrentSettings.FormLocationX = this.Location.X;
                    this.CurrentSettings.FormLocationY = this.Location.Y;
                    this.CurrentSettings.FormWidth = this.ClientSize.Width;
                    this.CurrentSettings.FormHeight = this.ClientSize.Height;
                    this.SaveUserSettings();
                }
                this.StatsDB.Dispose();
            } catch { }
        }
        private void Stats_Load(object sender, EventArgs e) {
            try {
                if (this.CurrentSettings.FormWidth.HasValue) {
                    this.ClientSize = new Size(this.CurrentSettings.FormWidth.Value, this.CurrentSettings.FormHeight.Value);
                }
                if (this.CurrentSettings.FormLocationX.HasValue && IsOnScreen(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value, this.Width)) {
                    this.Location = new Point(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value);
                }

#if AllowUpdate
                if (this.CurrentSettings.AutoUpdate && this.CheckForUpdate(true)) {
                    return;
                }
#endif

                this.menuProfile.DropDownItems["menuProfile" + this.CurrentSettings.SelectedProfile].PerformClick();

                this.UpdateDates();
            } catch { }
        }
        private void Stats_Shown(object sender, EventArgs e) {
            try {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                    logPath = this.CurrentSettings.LogPath;
                }
                this.logFile.Start(logPath, LOGNAME);

                this.overlay.ArrangeDisplay(this.CurrentSettings.FlippedDisplay, this.CurrentSettings.ShowOverlayTabs,this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo, this.CurrentSettings.OverlayColor, this.CurrentSettings.OverlayWidth, this.CurrentSettings.OverlayHeight, this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
                if (this.CurrentSettings.OverlayVisible) {
                    this.ToggleOverlay(this.overlay);
                }

                this.menuAllStats.Checked = false;
                switch (this.CurrentSettings.FilterType) {
                    case 0:
                        this.menuAllStats.Checked = true;
                        this.menuStats_Click(this.menuAllStats, null);
                        break;
                    case 1:
                        this.menuSeasonStats.Checked = true;
                        this.menuStats_Click(this.menuSeasonStats, null);
                        break;
                    case 2: this.menuWeekStats.Checked = true;
                        this.menuStats_Click(this.menuWeekStats, null);
                        break;
                    case 3: this.menuDayStats.Checked = true;
                        this.menuStats_Click(this.menuDayStats, null);
                        break;
                    case 4: this.menuSessionStats.Checked = true;
                        this.menuStats_Click(this.menuSessionStats, null);
                        break;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogFile_OnError(string error) {
            if (!this.Disposing && !this.IsDisposed) {
                try {
                    if (this.InvokeRequired) {
                        Invoke((Action<string>)LogFile_OnError, error);
                    } else {
                        MessageBox.Show(this, error, $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch { }
            }
        }
        private void LogFile_OnNewLogFileDate(DateTime newDate) {
            if (SessionStart != newDate) {
                SessionStart = newDate;
                if (this.menuSessionStats.Checked) {
                    menuStats_Click(this.menuSessionStats, null);
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
                if (InvokeRequired) {
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
                                        editShows.Icon = this.Icon;
                                        editShows.Profiles = this.AllProfiles;
                                        editShows.StatsForm = this;
                                        if (editShows.ShowDialog(this) == DialogResult.OK) {
                                            this.askedPreviousShows = 1;
                                            profile = editShows.SelectedProfileId;
                                            this.CurrentSettings.SelectedProfile = profile;
                                            this.ReloadProfileMenuItems();
                                        } else {
                                            this.askedPreviousShows = 2;
                                        }
                                    }
                                }

                                if (stat.ShowEnd < this.startupTime && this.askedPreviousShows == 2) {
                                    continue;
                                }

                                if (stat.Round == 1) {
                                    this.nextShowID++;
                                    this.lastAddedShow = stat.Start;
                                }
                                stat.ShowID = nextShowID;
                                stat.Profile = profile;
                                this.RoundDetails.Insert(stat);
                                this.AllStats.Add(stat);
                            } else {
                                continue;
                            }
                        }

                        if (!stat.PrivateLobby) {
                            if (stat.Round == 1) {
                                this.Shows++;
                            }
                            this.Rounds++;
                        }
                        this.Duration += stat.End - stat.Start;
                        this.Kudos += stat.Kudos;

                        // add new type of round to the rounds lookup
                        if (!this.StatLookup.ContainsKey(stat.Name)) {
                            string roundName = stat.Name;
                            if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                                roundName = roundName.Substring(6).Replace('_', ' ');
                            }

                            LevelStats newLevel = new LevelStats(this.textInfo.ToTitleCase(roundName), LevelType.Unknown, false, 0, null);
                            this.StatLookup.Add(stat.Name, newLevel);
                            this.StatDetails.Add(newLevel);
                            this.gridDetails.DataSource = null;
                            this.gridDetails.DataSource = this.StatDetails;
                        }

                        stat.ToLocalTime();
                        LevelStats levelStats = this.StatLookup[stat.Name];

                        if (!stat.PrivateLobby) {
                            if (stat.IsFinal || stat.Crown) {
                                this.Finals++;
                                if (stat.Qualified) {
                                    this.Wins++;
                                }
                            }
                        }
                        levelStats.Add(stat);
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

                if (!Disposing && !IsDisposed) {
                    try {
                        UpdateTotals();
                    } catch { }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsInStatsFilter(DateTime showEnd) {
            return this.menuAllStats.Checked ||
                   (this.menuSeasonStats.Checked && showEnd > SeasonStart) ||
                   (this.menuWeekStats.Checked && showEnd > WeekStart) ||
                   (this.menuDayStats.Checked && showEnd > DayStart) ||
                   (this.menuSessionStats.Checked && showEnd > SessionStart);
        }
        private bool IsInPartyFilter(RoundInfo info) {
            return this.menuAllPartyStats.Checked ||
                   (this.menuSoloStats.Checked && !info.InParty) ||
                   (this.menuPartyStats.Checked && info.InParty);
        }
        public string GetCurrentFilter() {
            return this.menuAllStats.Checked ? Multilingual.GetWord("main_all") : this.menuSeasonStats.Checked ? Multilingual.GetWord("main_season") : this.menuWeekStats.Checked ? Multilingual.GetWord("main_week") : this.menuDayStats.Checked ? Multilingual.GetWord("main_day") : Multilingual.GetWord("main_session");
        }
        public string GetCurrentProfile() {
            return this.menuProfile.DropDownItems["menuProfile" + this.CurrentSettings.SelectedProfile].Text;
        }
        public int GetCurrentProfileId() {
            return this.currentProfile;
        }
        public string GetCurrentProfileLinkedShowId() {
            return this.AllProfiles.Find(p => p.ProfileId == this.GetCurrentProfileId()).LinkedShowId;
        }
        public void SetLinkedProfile(string showId, bool isPrivateLobbies) {
            if (string.IsNullOrEmpty(showId) && this.GetCurrentProfileLinkedShowId().Equals(showId)) return;
            for (int i = 0; i < this.AllProfiles.Count; i++) {
                if (isPrivateLobbies) {
                    if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && this.AllProfiles[i].LinkedShowId.Equals("private_lobbies")) {
                        ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                        if (!item.Checked) item.PerformClick();
                        break;
                    }
                } else {
                    if (!string.IsNullOrEmpty(this.AllProfiles[i].LinkedShowId) && showId.IndexOf(this.AllProfiles[i].LinkedShowId, StringComparison.OrdinalIgnoreCase) != -1) {
                        ToolStripMenuItem item = this.ProfileMenuItems[this.AllProfiles.Count - 1 - i];
                        if (!item.Checked) item.PerformClick();
                        break;
                    }
                }
            }
        }
        public void SetCurrentProfileIcon(bool linked) {
            if (this.CurrentSettings.AutoChangeProfile) {
                this.lblCurrentProfile.Image = linked ? Properties.Resources.profile2_linked_icon : Properties.Resources.profile2_unlinked_icon;
                this.overlay.SetCurrentProfileForeColor(linked ? Color.GreenYellow : string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
            } else {
                this.lblCurrentProfile.Image = Properties.Resources.profile2_icon;
                this.overlay.SetCurrentProfileForeColor(string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized) ? Color.White : (Color)new ColorConverter().ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized));
            }
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
            if (!this.StatLookup.TryGetValue(name ?? string.Empty, out currentLevel)) {
                currentLevel = new LevelStats(name, LevelType.Unknown, false, 0, null);
            }
            int profile = this.currentProfile;

            for (int i = 0; i < this.AllStats.Count; i++) {
                RoundInfo info = this.AllStats[i];
                if (info.Profile != profile) { continue; }

                TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                bool hasLevelDetails = StatLookup.TryGetValue(info.Name, out levelDetails);
                bool isCurrentLevel = currentLevel.Name.Equals(hasLevelDetails ? levelDetails.Name : info.Name, StringComparison.OrdinalIgnoreCase);

                int currentShow = info.ShowID;
                RoundInfo endShow = info;
                for (int j = i + 1; j < this.AllStats.Count; j++) {
                    if (this.AllStats[j].ShowID != currentShow) {
                        break;
                    }
                    endShow = this.AllStats[j];
                }

                bool isInWinsFilter = !endShow.PrivateLobby && (this.CurrentSettings.WinsFilter == 0 ||
                    (this.CurrentSettings.WinsFilter == 1 && this.IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info)));
                bool isInQualifyFilter = !endShow.PrivateLobby && (this.CurrentSettings.QualifyFilter == 0 ||
                    (this.CurrentSettings.QualifyFilter == 1 && this.IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info)));
                bool isInFastestFilter = this.CurrentSettings.FastestFilter == 0 ||
                    (this.CurrentSettings.FastestFilter == 1 && this.IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info));

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

            return summary;
        }
        private void ClearTotals() {
            this.Rounds = 0;
            this.Duration = TimeSpan.Zero;
            this.Wins = 0;
            this.Shows = 0;
            this.Finals = 0;
            this.Kudos = 0;
        }
        private void UpdateTotals() {
            try {
                this.lblCurrentProfile.Text = $"{GetCurrentProfile()}";
                this.lblCurrentProfile.ToolTipText = $"{Multilingual.GetWord("profile_change_tooltiptext")}";
                this.lblTotalRounds.Text = $"{Multilingual.GetWord("main_rounds")} : {this.Rounds}{Multilingual.GetWord("main_round")}";
                this.lblTotalRounds.ToolTipText = $"{Multilingual.GetWord("rounds_detail_tooltiptext")}";
                this.lblTotalShows.Text = $"{Multilingual.GetWord("main_shows")} : {this.Shows}{Multilingual.GetWord("main_inning")}";
                this.lblTotalShows.ToolTipText = $"{Multilingual.GetWord("shows_detail_tooltiptext")}";
                this.lblTotalTime.Text = $"{(int)this.Duration.TotalHours}{Multilingual.GetWord("main_hour")}{this.Duration:mm}{Multilingual.GetWord("main_min")}{this.Duration:ss}{Multilingual.GetWord("main_sec")}";
                float winChance = (float)this.Wins * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalWins.Text = $"{this.Wins}{Multilingual.GetWord("main_win")} ({winChance:0.0} %)";
                this.lblTotalWins.ToolTipText = $"{Multilingual.GetWord("wins_detail_tooltiptext")}";
                float finalChance = (float)this.Finals * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalFinals.Text = $"{this.Finals}{Multilingual.GetWord("main_inning")} ({finalChance:0.0} %)";
                this.lblTotalFinals.ToolTipText = $"{Multilingual.GetWord("finals_detail_tooltiptext")}";
                this.lblKudos.Text = $"{this.Kudos}";
                this.gridDetails.Refresh();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            this.SetMainDataGridView();
        }
        private int GetDataGridViewColumnWidth(string columnName, String columnText) {
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
                    sizeOfText += CurrentLanguage == 2 || CurrentLanguage == 4 ? 5 : 0;
                    break;
                case "Gold":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 1 ? 12 : CurrentLanguage == 4 ? 5 : 0;
                    break;
                case "Silver":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 ? 5 : 0;
                    break;
                case "Bronze":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 ? 5 : 0;
                    break;
                case "Kudos":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Fastest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 ? 20 : 0;
                    break;
                case "Longest":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 ? 20 : 0;
                    break;
                case "AveFinish":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    sizeOfText += CurrentLanguage == 4 ? 20 : 0;
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
                this.gridDetails.Columns["AveKudos"].Visible = false;
                this.gridDetails.Columns["AveDuration"].Visible = false;
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
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                LevelStats levelStats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                float fBrightness = 0.7F;
                switch (this.gridDetails.Columns[e.ColumnIndex].Name) {
                    case "RoundIcon":
                        if (levelStats.IsFinal) {
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(255,230,138) : Color.FromArgb((int)(255 * fBrightness),(int)(230 * fBrightness),(int)(138 * fBrightness));
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.Race: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(206,255,228) : Color.FromArgb((int)(206 * fBrightness),(int)(255 * fBrightness),(int)(228 * fBrightness)); break;
                            case LevelType.Survival: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(244,206,250) : Color.FromArgb((int)(244 * fBrightness),(int)(206 * fBrightness),(int)(250 * fBrightness)); break;
                            case LevelType.Team: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(255,238,230) : Color.FromArgb((int)(255 * fBrightness),(int)(238 * fBrightness),(int)(230 * fBrightness)); break;
                            case LevelType.Hunt: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(208,222,244) : Color.FromArgb((int)(208 * fBrightness),(int)(222 * fBrightness),(int)(244 * fBrightness)); break;
                            case LevelType.Unknown: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.DarkGray; break;
                        }
                        break;
                    case "Name":
                        e.CellStyle.ForeColor = Color.Black;
                        this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_tooltiptext");
                        if (levelStats.IsFinal) {
                            e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(255,230,138) : Color.FromArgb((int)(255 * fBrightness),(int)(230 * fBrightness),(int)(138 * fBrightness));
                            break;
                        }
                        switch (levelStats.Type) {
                            case LevelType.Race: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(206,255,228) : Color.FromArgb((int)(206 * fBrightness),(int)(255 * fBrightness),(int)(228 * fBrightness)); break;
                            case LevelType.Survival: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(244,206,250) : Color.FromArgb((int)(244 * fBrightness),(int)(206 * fBrightness),(int)(250 * fBrightness)); break;
                            case LevelType.Team: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(255,238,230) : Color.FromArgb((int)(255 * fBrightness),(int)(238 * fBrightness),(int)(230 * fBrightness)); break;
                            case LevelType.Hunt: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(208,222,244) : Color.FromArgb((int)(208 * fBrightness),(int)(222 * fBrightness),(int)(244 * fBrightness)); break;
                            case LevelType.Unknown: e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.DarkGray; break;
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
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            this.gridDetails.Cursor = Cursors.Default;
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex >= 0 && (this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")) {
                    this.gridDetails.Cursor = Cursors.Hand;
                } else if (e.RowIndex >= 0 && !(this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon")) {
                    this.gridDetails.Cursor = this.Theme == MetroThemeStyle.Light ? new Cursor(Properties.Resources.transform_icon.GetHicon()) : new Cursor(Properties.Resources.transform_gray_icon.GetHicon());
                } else {
                    this.gridDetails.Cursor = Cursors.Default;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }
                if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name" || this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon") {
                    LevelStats stats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                    using (LevelDetails levelDetails = new LevelDetails()) {
                        levelDetails.LevelName = stats.Name;
                        List<RoundInfo> rounds = stats.Stats;
                        rounds.Sort();
                        levelDetails.RoundDetails = rounds;
                        levelDetails.StatsForm = this;
                        levelDetails.ShowDialog(this);
                    }
                } else {
                    this.ToggleWinPercentageDisplay();
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = this.gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridDetails.GetSortOrder(columnName);

            this.StatDetails.Sort(delegate (LevelStats one, LevelStats two) {
                LevelType oneType = one.IsFinal ? LevelType.Final : one.Type;
                LevelType twoType = two.IsFinal ? LevelType.Final : two.Type;

                int typeCompare = this.CurrentSettings.IgnoreLevelTypeWhenSorting && sortOrder != SortOrder.None ? 0 : ((int)oneType).CompareTo((int)twoType);

                if (sortOrder == SortOrder.Descending) {
                    LevelStats temp = one;
                    one = two;
                    two = temp;
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
                        shows.Insert(0,
                            new RoundInfo {
                                Name = isFinal ? "Final" : string.Empty, ShowNameId = info.ShowNameId, IsFinal = isFinal, End = endDate,
                                Start = info.Start, StartLocal = info.StartLocal, Kudos = kudosTotal,
                                Qualified = won, Round = roundCount, ShowID = info.ShowID, Tier = won ? 1 : 0
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

            //if (rounds.Count <= 0) {
            //    MessageBox.Show(this, $"{Multilingual.GetWord("level_detail_no_data")}", Multilingual.GetWord("level_detail_no_data_caption"), MessageBoxButtons.OK);
            //    return;
            //}
            
            using (StatsDisplay display = new StatsDisplay { StatsForm = this, Text = $"{Multilingual.GetWord("level_detail_wins_per_day")} - {this.GetCurrentProfile()}" }) {
                if (rounds.Count > 0) {
                    ArrayList dates = new ArrayList();
                    ArrayList shows = new ArrayList();
                    ArrayList finals = new ArrayList();
                    ArrayList wins = new ArrayList();
                    if (rounds.Count > 0) {
                        DateTime start = rounds[0].StartLocal;
                        int currentShows = 0;
                        int currentFinals = 0;
                        int currentWins = 0;
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
                                dates.Add(start.Date.ToOADate());
                                shows.Add(Convert.ToDouble(currentShows));
                                finals.Add(Convert.ToDouble(currentFinals));
                                wins.Add(Convert.ToDouble(currentWins));

                                int missingCount = (int)(info.StartLocal.Date - start.Date).TotalDays;
                                while (missingCount > 1) {
                                    missingCount--;
                                    start = start.Date.AddDays(1);
                                    dates.Add(start.ToOADate());
                                    shows.Add(0D);
                                    finals.Add(0D);
                                    wins.Add(0D);
                                }

                                currentShows = 0;
                                currentFinals = 0;
                                currentWins = 0;
                                start = info.StartLocal;
                            }
                        }

                        dates.Add(start.Date.ToOADate());
                        shows.Add(Convert.ToDouble(currentShows));
                        finals.Add(Convert.ToDouble(currentFinals));
                        wins.Add(Convert.ToDouble(currentWins));
                    } else {
                        dates.Add(DateTime.Now.Date.ToOADate());
                        shows.Add(0D);
                        finals.Add(0D);
                        wins.Add(0D);
                    }
                    
                    display.manualSpacing = dates.Count / 28;
                    display.dates = (double[])dates.ToArray(typeof(double));
                    display.shows = (double[])shows.ToArray(typeof(double));
                    display.finals = (double[])finals.ToArray(typeof(double));
                    display.wins = (double[])wins.ToArray(typeof(double));
                } else {
                    display.manualSpacing = 1;
                    display.dates = null;
                    display.shows = null;
                    display.finals = null;
                    display.wins = null;
                }

                display.ShowDialog(this);
            }
        }
        private void LaunchHelpInBrowser() {
            try {
                Process.Start(@"https://github.com/ShootMe/FallGuysStats");
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LaunchGame(bool ignoreExisting) {
            try {
                if (CurrentSettings.LaunchPlatform == 0) {
                    if (!string.IsNullOrEmpty(this.CurrentSettings.GameShortcutLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = "FallGuys_client_game";
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MessageBox.Show(this, Multilingual.GetWord("message_already_running"), Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
                        }

                        if (MessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}", Multilingual.GetWord("message_execution_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameShortcutLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MessageBox.Show(this, Multilingual.GetWord("message_register_shortcut"), Multilingual.GetWord("message_register_shortcut_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    this.UpdateGameExeLocation();
                    if (!string.IsNullOrEmpty(CurrentSettings.GameExeLocation) && File.Exists(CurrentSettings.GameExeLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = Path.GetFileNameWithoutExtension(CurrentSettings.GameExeLocation);
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MessageBox.Show(this, Multilingual.GetWord("message_already_running"), Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
                        }

                        if (MessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}", Multilingual.GetWord("message_execution_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameExeLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MessageBox.Show(this, Multilingual.GetWord("message_register_exe"), Multilingual.GetWord("message_register_exe_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void UpdateGameExeLocation() {
            if (string.IsNullOrEmpty(this.CurrentSettings.GameExeLocation)) {
                string fallGuysExeLocation = this.FindGameExeLocation();
                if (!string.IsNullOrEmpty(fallGuysExeLocation)) {
                    this.menuLaunchFallGuys.Image = Properties.Resources.steam_main_icon;
                    this.CurrentSettings.LaunchPlatform = 1;
                    this.CurrentSettings.GameExeLocation = fallGuysExeLocation;
                    this.SaveUserSettings();
                }
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
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        private void lblCurrentProfile_Click(object sender, EventArgs e) {
            for (var i = 0; i < this.ProfileMenuItems.Count; i++) {
                ToolStripItem item = this.ProfileMenuItems[i];
                if (!(item is ToolStripMenuItem menuItem)) { continue; }

                if (menuItem.Checked && i + 1 < this.ProfileMenuItems.Count) {
                    this.ProfileMenuItems[i + 1].PerformClick();
                    break;
                } else if (menuItem.Checked && i + 1 >= this.ProfileMenuItems.Count) {
                    this.ProfileMenuItems[0].PerformClick();
                    break;
                }
            }
        }
        private void lblTotalFinals_Click(object sender, EventArgs e) {
            try {
                this.ShowFinals();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalShows_Click(object sender, EventArgs e) {
            try {
                this.ShowShows();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalRounds_Click(object sender, EventArgs e) {
            try {
                this.ShowRounds();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalWins_Click(object sender, EventArgs e) {
            try {
                this.ShowWinGraph();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuStats_Click(object sender, EventArgs e) {
            try {
                ToolStripMenuItem button = sender as ToolStripMenuItem;
                if (button == this.menuAllStats || button == this.menuSeasonStats || button == this.menuWeekStats || button == this.menuDayStats || button == this.menuSessionStats) {
                    if (!this.menuAllStats.Checked && !this.menuSeasonStats.Checked && !this.menuWeekStats.Checked && !this.menuDayStats.Checked && !this.menuSessionStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in this.menuStatsFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }
                }

                if (button == this.menuAllPartyStats || button == this.menuSoloStats || button == this.menuPartyStats) {
                    if (!this.menuAllPartyStats.Checked && !this.menuSoloStats.Checked && !this.menuPartyStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in this.menuPartyFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }

                    button = this.menuAllStats.Checked ? this.menuAllStats : this.menuSeasonStats.Checked ? this.menuSeasonStats : this.menuWeekStats.Checked ? this.menuWeekStats : this.menuDayStats.Checked ? this.menuDayStats : this.menuSessionStats;
                }

                if(this.ProfileMenuItems.Contains(button)) {
                    for (int i = this.ProfileMenuItems.Count - 1; i >= 0; i--) {
                        if(this.ProfileMenuItems[i].Name == button.Name) this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileName == this.ProfileMenuItems[i].Text && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                        this.ProfileMenuItems[i].Checked = this.ProfileMenuItems[i].Name == button.Name;
                    }
                    this.currentProfile = Int32.Parse(button.Name.Substring(11));
                    button = this.menuAllStats.Checked ? this.menuAllStats : this.menuSeasonStats.Checked ? this.menuSeasonStats : this.menuWeekStats.Checked ? this.menuWeekStats : this.menuDayStats.Checked ? this.menuDayStats : this.menuSessionStats;
                }

                for (int i = 0; i < this.StatDetails.Count; i++) {
                    LevelStats calculator = this.StatDetails[i];
                    calculator.Clear();
                }

                this.ClearTotals();

                int profile = this.currentProfile;
                bool soloOnly = this.menuSoloStats.Checked;
                List<RoundInfo> rounds = new List<RoundInfo>();

                DateTime compareDate = this.menuAllStats.Checked ? DateTime.MinValue : this.menuSeasonStats.Checked ? SeasonStart : this.menuWeekStats.Checked ? WeekStart : this.menuDayStats.Checked ? DayStart : SessionStart;
                for (int i = 0; i < this.AllStats.Count; i++) {
                    RoundInfo round = this.AllStats[i];
                    if (round.Start > compareDate && round.Profile == profile && (this.menuAllPartyStats.Checked || round.InParty == soloOnly)) {
                        rounds.Add(round);
                    }
                }

                rounds.Sort();

                if (rounds.Count > 0 && (button == this.menuWeekStats || button == this.menuDayStats || button == this.menuSessionStats)) {
                    int minShowID = rounds[0].ShowID;

                    for (int i = 0; i < this.AllStats.Count; i++) {
                        RoundInfo round = this.AllStats[i];
                        if (round.ShowID == minShowID && round.Start <= compareDate) {
                            rounds.Add(round);
                        }
                    }
                }

                rounds.Sort();

                this.CurrentSettings.SelectedProfile = profile;
                this.CurrentSettings.FilterType = this.menuAllStats.Checked ? 0 : this.menuSeasonStats.Checked ? 1 : this.menuWeekStats.Checked ? 2 : this.menuDayStats.Checked ? 3 : 4;
                this.SaveUserSettings();

                this.loadingExisting = true;
                this.LogFile_OnParsedLogLines(rounds);
                this.loadingExisting = false;
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuUpdate_Click(object sender, EventArgs e) {
            try {
                this.CheckForUpdate(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_update_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool CheckForUpdate(bool silent) {
#if AllowUpdate
            using (ZipWebClient web = new ZipWebClient()) {
                string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                int index = assemblyInfo.IndexOf("AssemblyVersion(");
                if (index > 0) {
                    int indexEnd = assemblyInfo.IndexOf("\")", index);
                    Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                    Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                    if (newVersion > currentVersion) {
                        if (silent || MessageBox.Show(this, $"{Multilingual.GetWord("message_update_question_prefix")} [ v{newVersion.ToString(2)} ] {Multilingual.GetWord("message_update_question_suffix")}", $"{Multilingual.GetWord("message_update_question_caption")}", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            byte[] data = web.DownloadData($"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuysStats.zip");
                            string exeName = null;
                            using (MemoryStream ms = new MemoryStream(data)) {
                                using (ZipArchive zipFile = new ZipArchive(ms, ZipArchiveMode.Read)) {
                                    foreach (var entry in zipFile.Entries) {
                                        if (entry.Name.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) > 0) {
                                            exeName = entry.Name;
                                        }
                                        if (File.Exists(entry.Name)) File.Move(entry.Name, $"{entry.Name}.bak");
                                        entry.ExtractToFile(entry.Name, true);
                                    }
                                }
                            }

                            Process.Start(new ProcessStartInfo(exeName));
                            this.Visible = false;
                            this.Close();
                            return true;
                        }
                    } else if (!silent) {
                        MessageBox.Show(this, $"{Multilingual.GetWord("message_update_latest_version")}", $"{Multilingual.GetWord("message_update_question_caption")}", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                } else if (!silent) {
                    MessageBox.Show(this, $"{Multilingual.GetWord("message_update_not_determine_version")}", $"{Multilingual.GetWord("message_update_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
#else
            this.LaunchHelpInBrowser();
#endif
            return false;
        }
        private async void menuSettings_Click(object sender, EventArgs e) {
            try {
                using (Settings settings = new Settings()) {
                    settings.Icon = this.Icon;
                    settings.CurrentSettings = this.CurrentSettings;
                    settings.StatsForm = this;
                    string lastLogPath = this.CurrentSettings.LogPath;
                    if (settings.ShowDialog(this) == DialogResult.OK) {
                        this.CurrentSettings = settings.CurrentSettings;
                        this.SuspendLayout();
                        this.SetTheme(this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
                        this.ResumeLayout(false);
                        this.SaveUserSettings();
                        this.ChangeMainLanguage();
                        this.gridDetails.ChangeContextMenuLanguage();
                        this.overlay.SetBackgroundResourcesName(this.CurrentSettings.OverlayBackgroundResourceName, this.CurrentSettings.OverlayTabResourceName);
                        this.overlay.ChangeLanguage();
                        this.UpdateTotals();
                        this.SetCurrentProfileIcon(this.AllProfiles.FindIndex(p => p.ProfileId == this.GetCurrentProfileId() && !string.IsNullOrEmpty(p.LinkedShowId)) != -1);
                        this.UpdateGridRoundName();
                        this.UpdateHoopsieLegends();

                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(this.CurrentSettings.LogPath) || (!string.IsNullOrEmpty(lastLogPath) && lastLogPath.Equals(this.CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await this.logFile.Stop();

                            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                                logPath = this.CurrentSettings.LogPath;
                            }
                            this.logFile.Start(logPath, LOGNAME);
                        }
                        
                        this.overlay.ArrangeDisplay(this.CurrentSettings.FlippedDisplay, this.CurrentSettings.ShowOverlayTabs, this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo, this.CurrentSettings.OverlayColor, this.CurrentSettings.OverlayWidth, this.CurrentSettings.OverlayHeight, this.CurrentSettings.OverlayFontSerialized, this.CurrentSettings.OverlayFontColorSerialized);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuOverlay_Click(object sender, EventArgs e) {
            this.ToggleOverlay(overlay);
        }
        private void ToggleOverlay(Overlay overlay) {
            if (overlay.Visible) {
                overlay.Hide();
                this.menuOverlay.Image = Properties.Resources.stat_gray_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                if (!overlay.IsFixed()) {
                    this.CurrentSettings.OverlayLocationX = overlay.Location.X;
                    this.CurrentSettings.OverlayLocationY = overlay.Location.Y;
                    this.CurrentSettings.OverlayWidth = overlay.Width;
                    this.CurrentSettings.OverlayHeight = overlay.Height;
                }
                this.CurrentSettings.OverlayVisible = false;
                this.SaveUserSettings();
            } else {
                overlay.TopMost = !this.CurrentSettings.OverlayNotOnTop;
                overlay.Show();
                this.menuOverlay.Image = Properties.Resources.stat_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.CurrentSettings.OverlayVisible = true;
                this.SaveUserSettings();

                if (overlay.IsFixed()) {
                    if (this.CurrentSettings.OverlayFixedPositionX.HasValue && this.IsOnScreen(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value, overlay.Width)) {
                        overlay.FlipDisplay(this.CurrentSettings.FixedFlippedDisplay);
                        overlay.Location = new Point(this.CurrentSettings.OverlayFixedPositionX.Value, this.CurrentSettings.OverlayFixedPositionY.Value);
                    } else {
                        overlay.Location = this.Location;
                    }
                } else {
                    if (this.CurrentSettings.OverlayLocationX.HasValue && this.IsOnScreen(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value, overlay.Width)) {
                        overlay.Location = new Point(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value);
                    } else {
                        overlay.Location = this.Location;
                    }
                }
            }
        }
        private void menuHelp_Click(object sender, EventArgs e) {
            this.LaunchHelpInBrowser();
        }
        private void menuEditProfiles_Click(object sender, EventArgs e) {
            try {
                using (EditProfiles editProfiles = new EditProfiles()) {
                    editProfiles.Icon = this.Icon;
                    editProfiles.StatsForm = this;
                    editProfiles.Profiles = this.AllProfiles;
                    editProfiles.AllStats = this.RoundDetails.FindAll().ToList();
                    editProfiles.ShowDialog(this);
                    lock (this.StatsDB) {
                        this.StatsDB.BeginTrans();
                        this.AllProfiles = editProfiles.Profiles;
                        this.Profiles.DeleteAll();
                        this.Profiles.InsertBulk(this.AllProfiles);
                        this.AllStats = editProfiles.AllStats;
                        this.RoundDetails.DeleteAll();
                        this.RoundDetails.InsertBulk(this.AllStats);
                        this.StatsDB.Commit();
                    }
                    this.ReloadProfileMenuItems();
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuLaunchFallGuys_Click(object sender, EventArgs e) {
            try {
                this.LaunchGame(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public Screen GetCurrentScreen(Point location) {
            Screen[] scr = Screen.AllScreens;
            Screen screen = null;
            for (int i = 0; i < scr.Length; i++) {
                if (scr[i].WorkingArea.Contains(location)) {
                    screen = scr[i];
                    break;
                }
            }
            return screen;
        }
        private void ChangeMainLanguage() {
            this.Text = $"　  {Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.menu.Font = Overlay.GetMainFont(12);
            this.menuLaunchFallGuys.Font = Overlay.GetMainFont(12);
            this.infoStrip.Font = Overlay.GetMainFont(13);
            
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(10);
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(12);
            this.SetMainDataGridView();
            
            this.menuSettings.Text = $"{Multilingual.GetWord("main_settings")}";
            this.menuFilters.Text = $"{Multilingual.GetWord("main_filters")}";
            this.menuStatsFilter.Text = Multilingual.GetWord("main_stats");
            this.menuAllStats.Text = Multilingual.GetWord("main_all");
            this.menuSeasonStats.Text = Multilingual.GetWord("main_season");
            this.menuWeekStats.Text = Multilingual.GetWord("main_week");
            this.menuDayStats.Text = Multilingual.GetWord("main_day");
            this.menuSessionStats.Text = Multilingual.GetWord("main_session");
            this.menuPartyFilter.Text = Multilingual.GetWord("main_party_type");
            this.menuAllPartyStats.Text = Multilingual.GetWord("main_all");
            this.menuSoloStats.Text = Multilingual.GetWord("main_solo");
            this.menuPartyStats.Text = Multilingual.GetWord("main_party");
            this.menuProfile.Text = $"{Multilingual.GetWord("main_profile")}";
            this.menuEditProfiles.Text = $"{Multilingual.GetWord("main_profile_setting")}";
            if (!CurrentSettings.OverlayVisible) {
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
            } else {
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
            }
            this.menuUpdate.Text = $"{Multilingual.GetWord("main_update")}";
            this.menuHelp.Text = $"{Multilingual.GetWord("main_help")}";
            this.menuLaunchFallGuys.Text = $"{Multilingual.GetWord("main_launch_fall_guys")}";
            this.menuLaunchFallGuys.Image = this.CurrentSettings.LaunchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
        }
    }
    
    public class MySr : ToolStripSystemRenderer {
        public MySr() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            //base.OnRenderToolStripBorder(e);
        }
    }
}