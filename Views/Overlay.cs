using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Overlay : Form {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        
        public Stats StatsForm { get; set; }
        private Thread timer;
        private bool flippedImage;
        private int frameCount;
        private bool isTimeToSwitch;
        private int switchCount;
        private Bitmap Background, DrawImage;
        private Graphics DrawGraphics;
        private RoundInfo lastRound;
        private int drawWidth, drawHeight;
        private bool startedPlaying;
        private DateTime startTime;
        private bool shiftKeyToggle;
        
        public static PrivateFontCollection DefaultFontCollection;
        public static Font DefaultFont;
        
        static Overlay() {
            if (!File.Exists("TitanOne-Regular.ttf") ) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.TitanOne-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("TitanOne-Regular.ttf", fontdata);
                }
            }
            
            if (!File.Exists("NotoSans-Regular.ttf") ) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.NotoSans-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("NotoSans-Regular.ttf", fontdata);
                }
            }

            DefaultFontCollection = new PrivateFontCollection();
            DefaultFontCollection.AddFontFile("NotoSans-Regular.ttf");
            DefaultFontCollection.AddFontFile("TitanOne-Regular.ttf");
            if (Stats.CurrentLanguage == 0) {
                DefaultFont = new Font(DefaultFontCollection.Families[1], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            } else {
                DefaultFont = new Font(DefaultFontCollection.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }
        
        public Overlay() {
            this.InitializeComponent();
            this.ChangeLanguage();

            Bitmap background = Properties.Resources.background;
            Bitmap newImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.DrawImage(background, 0, 0);
            }
            this.Background = newImage;

            this.DrawImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            this.DrawGraphics = Graphics.FromImage(this.DrawImage);
            this.drawWidth = background.Width;
            this.drawHeight = background.Height;

            foreach (Control c in Controls) {
                if (c is TransparentLabel label) {
                    label.Parent = this;
                    label.BackColor = Color.Transparent;
                }
                c.MouseDown += Overlay_MouseDown;
            }

            SetFonts(this);

            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x84) {
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                int hitSize = 16;
                if (pos.X >= ClientSize.Width - hitSize && pos.Y >= ClientSize.Height - hitSize) {
                    m.Result = (IntPtr)17;
                    return;
                } else if (pos.X <= hitSize && pos.Y >= ClientSize.Height - hitSize) {
                    m.Result = (IntPtr)16;
                    return;
                } else if (pos.X <= hitSize && pos.Y <= hitSize) {
                    m.Result = (IntPtr)13;
                    return;
                } else if (pos.X >= ClientSize.Width - hitSize && pos.Y <= hitSize) {
                    m.Result = (IntPtr)14;
                    return;
                }
            }
            base.WndProc(ref m);
        }
        public void StartTimer() {
            this.timer = new Thread(UpdateTimer);
            this.timer.IsBackground = true;
            this.timer.Start();
        }
        public static void SetFonts(Control control, float customSize = -1, Font font = null) {
            if (font == null) {
                font = customSize <= 0 ? DefaultFont : new Font(Stats.CurrentLanguage == 0 ? DefaultFontCollection.Families[1] : DefaultFontCollection.Families[0], customSize, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            control.Font = font;
            foreach (Control ctr in control.Controls) {
                ctr.Font = font;
                if (ctr.HasChildren) {
                    SetFonts(ctr, customSize, font);
                }
            }
        }
        private void Overlay_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void UpdateTimer() {
            while (this.StatsForm != null && !this.StatsForm.IsDisposed && !this.StatsForm.Disposing) {
                try {
                    if (IsHandleCreated && !Disposing && !IsDisposed) {
                        this.frameCount++;
                        this.isTimeToSwitch = this.frameCount % (this.StatsForm.CurrentSettings.CycleTimeSeconds * 20) == 0;
                        Invoke((Action)UpdateInfo);
                    }

                    StatsForm.UpdateDates();
                    Thread.Sleep(50);
                } catch { }
            }
        }
        private void SetQualifyChanceLabel(StatSummary levelInfo) {
            int qualifySwitchCount = switchCount;
            if (!this.StatsForm.CurrentSettings.SwitchBetweenQualify) {
                qualifySwitchCount = this.StatsForm.CurrentSettings.OnlyShowGold ? 1 : 0;
            }
            float qualifyChance;
            string qualifyChanceDisplay;
            string qualifyDisplay;
            switch (qualifySwitchCount % 2) {
                case 0:
                    this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_chance")} :";
                    qualifyChance = levelInfo.TotalQualify * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {qualifyChance:0.0}%";
                    qualifyDisplay = $"{levelInfo.TotalQualify}{(levelInfo.TotalPlays < 1000 ?  " / " + levelInfo.TotalPlays : Multilingual.GetWord("overlay_inning"))}";
                    this.lblQualifyChance.TextRight = $"　{qualifyDisplay}{qualifyChanceDisplay}";
                    break;
                case 1:
                    this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_gold")} :";
                    qualifyChance = levelInfo.TotalGolds * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {qualifyChance:0.0}%";
                    qualifyDisplay = $"{levelInfo.TotalGolds}{(levelInfo.TotalPlays < 1000 ?  " / " + levelInfo.TotalPlays : Multilingual.GetWord("overlay_inning"))}";
                    this.lblQualifyChance.TextRight = $"　{qualifyDisplay}{qualifyChanceDisplay}";
                    break;
            }
        }
        private void SetFastestLabel(StatSummary levelInfo, LevelType type) {
            int fastestSwitchCount = this.switchCount;
            if (!this.StatsForm.CurrentSettings.SwitchBetweenLongest) {
                fastestSwitchCount = StatsForm.CurrentSettings.OnlyShowLongest ? 0 : type.FastestLabel();
            }
            switch (fastestSwitchCount % (levelInfo.BestScore.HasValue ? 3 : 2)) {
                case 0:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_longest")} :";
                    this.lblFastest.TextRight = levelInfo.LongestFinish.HasValue ? $"　{levelInfo.LongestFinish:m\\:ss\\.ff}" : "　-";
                    break;
                case 1:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_fastest")} :";
                    this.lblFastest.TextRight = levelInfo.BestFinish.HasValue ? $"　{levelInfo.BestFinish:m\\:ss\\.ff}" : "　-";
                    break;
                case 2:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_best_score")} :";
                    this.lblFastest.TextRight = $"　{levelInfo.BestScore?.ToString()}";
                    break;
            }
        }
        private void SetPlayersLabel() {
            int playersSwitchCount = switchCount;
            if (!this.StatsForm.CurrentSettings.SwitchBetweenPlayers) {
                playersSwitchCount = this.StatsForm.CurrentSettings.OnlyShowPing ? 1 : 0;
            }
            switch (playersSwitchCount % 2) {
                case 0:
                    this.lblPlayers.TextRight = $"　{this.lastRound?.Players.ToString()}";
                    if (StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayers.Image = Properties.Resources.player_icon;
                        this.lblPlayers.Text = @"ㅤ    :";
                        int psCount = this.lastRound.PlayersPs4 + this.lastRound.PlayersPs5;
                        int xbCount = this.lastRound.PlayersXb1 + this.lastRound.PlayersXsx;
                        int swCount = this.lastRound.PlayersSw;
                        int pcCount = this.lastRound.PlayersPc;
                        this.lblPlayersPs.TextRight = (psCount == 0 ? "　-" : $"　{psCount.ToString()}");
                        this.lblPlayersPs.Size = new Size((psCount > 9 ? 31 : 26), 16);
                        this.lblPlayersPs.DrawVisible = true;
                        this.lblPlayersXbox.TextRight = (xbCount == 0 ? "　-" : $"　{xbCount.ToString()}");
                        this.lblPlayersXbox.Size = new Size((xbCount > 9 ? 31 : 26), 16);
                        this.lblPlayersXbox.DrawVisible = true;
                        this.lblPlayersSwitch.TextRight = (swCount == 0 ? "　-" : $"　{swCount.ToString()}");
                        this.lblPlayersSwitch.Size = new Size((swCount > 9 ? 31 : 26), 16);
                        this.lblPlayersSwitch.DrawVisible = true;
                        this.lblPlayersPc.TextRight = (pcCount == 0 ? "　-" : $"　{pcCount.ToString()}");
                        this.lblPlayersPc.Size = new Size((pcCount > 9 ? 31 : 26), 16);
                        this.lblPlayersPc.DrawVisible = true;
                    } else {
                        this.lblPlayers.Image = null;
                        this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_players")} :";
                        this.lblPlayersPs.DrawVisible = false;
                        this.lblPlayersXbox.DrawVisible = false;
                        this.lblPlayersSwitch.DrawVisible = false;
                        this.lblPlayersPc.DrawVisible = false;
                    }
                    break;
                case 1:
                    this.lblPlayers.Image = null;
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersPc.DrawVisible = false;
                    this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_ping")} :";
                    this.lblPlayers.TextRight = Stats.InShow && Stats.LastServerPing != 0 ? $"　{Stats.LastServerPing} ms" : "　-";
                    break;
            }
        }
        private void SetStreakInfo(StatSummary levelInfo) {
            int streakSwitchCount = this.switchCount;
            if (!this.StatsForm.CurrentSettings.SwitchBetweenStreaks) {
                streakSwitchCount = this.StatsForm.CurrentSettings.OnlyShowFinalStreak ? 1 : 0;
            }
            switch (streakSwitchCount % 2) {
                case 0:
                    this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak")} :";
                    this.lblStreak.TextRight = $"　{levelInfo.CurrentStreak}{Multilingual.GetWord("overlay_streak_suffix")} ({Multilingual.GetWord("overlay_best")}{levelInfo.BestStreak}{Multilingual.GetWord("overlay_streak_suffix")})";
                    break;
                case 1:
                    this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak_finals")} :";
                    this.lblStreak.TextRight = $"　{levelInfo.CurrentFinalStreak}{Multilingual.GetWord("overlay_inning")} ({Multilingual.GetWord("overlay_best")}{levelInfo.BestFinalStreak}{Multilingual.GetWord("overlay_inning")})";
                    break;
            }
        }
        private void UpdateInfo() {
            if (this.StatsForm == null) { return; }

            lock (this.StatsForm.CurrentRound) {
                bool hasCurrentRound = this.StatsForm.CurrentRound != null && this.StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound && (lastRound == null || Stats.InShow || Stats.EndedShow)) {
                    if (Stats.EndedShow) {
                        Stats.EndedShow = false;
                    }
                    lastRound = this.StatsForm.CurrentRound[this.StatsForm.CurrentRound.Count - 1];
                }

                lblFilter.Text = this.StatsForm.GetCurrentFilter();
                lblProfile.Text = this.StatsForm.GetCurrentProfile();
                Background = RecreateBackground();
                lblProfile.Location = new Point(flippedImage ? 125 : drawWidth - (145 + this.GetOverlayProfileOffset(this.lblProfile.Text)), 9);

                if (lastRound != null && !string.IsNullOrEmpty(lastRound.Name)) {
                    string roundName = lastRound.VerifiedName();
                    lblName.Text = $"{Multilingual.GetWord("overlay_name_prefix")}{lastRound.Round}{Multilingual.GetWord("overlay_name_suffix")} :";

                    if (this.StatsForm.StatLookup.TryGetValue(roundName, out var level)) {
                        roundName = level.Name.ToUpper();
                    }

                    if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                        roundName = roundName.Substring(6).Replace('_', ' ').ToUpper();
                    }
                    
                    StatSummary levelInfo = this.StatsForm.GetLevelInfo(roundName);
                    if (roundName.Length > 15) { roundName = roundName.Substring(0, 15); }

                    LevelType levelType = (level?.Type).GetValueOrDefault();

                    if (StatsForm.CurrentSettings.ColorByRoundType) {
                        this.lblName.LevelColor = levelType.LevelBackColor(lastRound.IsFinal, 223);
                    } else {
                        this.lblName.LevelColor = Color.Empty;
                    }
                    this.lblName.TextRight = roundName;

                    this.lblWins.Text = $"{Multilingual.GetWord("overlay_wins")} :";
                    float winChance = levelInfo.TotalWins * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    string winChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $"{Multilingual.GetWord("overlay_win")} - {winChance:0.0}%";
                    if (this.StatsForm.CurrentSettings.PreviousWins > 0) {
                        this.lblWins.TextRight = $"　{levelInfo.TotalWins} ({levelInfo.AllWins + this.StatsForm.CurrentSettings.PreviousWins}){winChanceDisplay}";
                    } else if (this.StatsForm.CurrentSettings.FilterType != 0) {
                        this.lblWins.TextRight = $"　{levelInfo.TotalWins} ({levelInfo.AllWins}){winChanceDisplay}";
                    } else {
                        this.lblWins.TextRight = $"　{levelInfo.TotalWins}{winChanceDisplay}";
                    }

                    this.lblFinals.Text = $"{Multilingual.GetWord("overlay_finals")} :";
                    float finalChance = levelInfo.TotalFinals * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    
                    string finalText = $"　{levelInfo.TotalFinals}{(levelInfo.TotalShows < 1000 ?  " / " + levelInfo.TotalShows : Multilingual.GetWord("overlay_inning"))}";
                    
                    string finalChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : finalText.Length > 9 ? $" - {finalChance:0}%" : $" - {finalChance:0.0}%";
                    this.lblFinals.TextRight = $"　{finalText}{finalChanceDisplay}";

                    SetQualifyChanceLabel(levelInfo);
                    SetFastestLabel(levelInfo, levelType);
                    SetPlayersLabel();
                    SetStreakInfo(levelInfo);
                    if (this.isTimeToSwitch) {
                        this.switchCount++;
                    }

                    DateTime Start = this.lastRound.Start;
                    DateTime End = this.lastRound.End;
                    DateTime? Finish = this.lastRound.Finish;

                    if (this.lastRound.Playing != this.startedPlaying) {
                        if (this.lastRound.Playing) {
                            this.startTime = DateTime.UtcNow;
                        }
                        this.startedPlaying = this.lastRound.Playing;
                    }

                    this.lblFinish.Text = $"{Multilingual.GetWord("overlay_finish")} :";
                    if (Finish.HasValue) {
                        TimeSpan Time = Finish.GetValueOrDefault(End) - Start;
                        //lblFinish.Text = $"{Multilingual.GetWord("overlay_finish")} :";
                        if (this.lastRound.Position > 0) {
                            this.lblFinish.TextRight = $"　# {this.lastRound.Position} - {Time:m\\:ss\\.ff}";
                        } else {
                            this.lblFinish.TextRight = $"　{Time:m\\:ss\\.ff}";
                        }

                        if (levelType == LevelType.Race || levelType == LevelType.Hunt || roundName == "ROCK'N'ROLL" || roundName == "SNOWY SCRAP") {
                            if (Time < levelInfo.BestFinish.GetValueOrDefault(TimeSpan.MaxValue) && Time > levelInfo.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.LightGreen;
                            } else if (Time < levelInfo.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.Gold;
                            }
                        } else if (Time > levelInfo.LongestFinish && Time < levelInfo.LongestFinishOverall) {
                            this.lblFinish.ForeColor = Color.LightGreen;
                        } else if (Time > levelInfo.LongestFinishOverall) {
                            this.lblFinish.ForeColor = Color.Gold;
                        }
                    } else if (this.lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            this.lblFinish.TextRight = $"　{DateTime.UtcNow - startTime:m\\:ss}";
                        } else {
                            this.lblFinish.TextRight = $"　{DateTime.UtcNow - Start:m\\:ss}";
                        }
                    } else {
                        this.lblFinish.TextRight = "-";
                        this.lblFinish.ForeColor = Color.White;
                    }

                    if (lastRound.GameDuration > 0) {
                        this.lblDuration.Text = $"{Multilingual.GetWord("overlay_duration")} ({TimeSpan.FromSeconds(lastRound.GameDuration):m\\:ss}):";
                    } else {
                        this.lblDuration.Text = $"{Multilingual.GetWord("overlay_duration")} :";
                    }

                    if (End != DateTime.MinValue) {
                        this.lblDuration.TextRight = $"　{End - Start:m\\:ss\\.ff}";
                    } else if (lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            this.lblDuration.TextRight = $"　{DateTime.UtcNow - startTime:m\\:ss}";
                        } else {
                            this.lblDuration.TextRight = $"　{DateTime.UtcNow - Start:m\\:ss}";
                        }
                    } else {
                        this.lblDuration.TextRight = "　-";
                    }
                }
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            lock (DefaultFont) {
                this.DrawGraphics.Clear(Color.Transparent);
                this.DrawGraphics.DrawImage(Background, 0, 0);

                foreach (Control control in Controls) {
                    if (control is TransparentLabel label) {
                        this.DrawGraphics.TranslateTransform(label.Location.X, label.Location.Y);
                        label.Draw(this.DrawGraphics);
                        this.DrawGraphics.TranslateTransform(-label.Location.X, -label.Location.Y);
                    }
                }

                e.Graphics.InterpolationMode = InterpolationMode.Default;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.DrawImage(this.DrawImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), new Rectangle(0, 0, this.DrawImage.Width, this.DrawImage.Height), GraphicsUnit.Pixel);
            }
        }
        private void Overlay_MouseWheel(object sender, MouseEventArgs e) {
            if (this.shiftKeyToggle == false) { return; }
            if (this.StatsForm.ProfileMenuItems.Count <= 1) { return; }
            if ((e.Delta / 120) > 0) {
                for (var i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                    ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                    if (!(item is ToolStripMenuItem menuItem)) { continue; }
                    if (menuItem.Checked && i > 0) {
                        this.StatsForm.ProfileMenuItems[i-1].PerformClick();
                        break;
                    } else if (menuItem.Checked && i == 0) {
                        this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count-1].PerformClick();
                        break;
                    }
                }
            } else {
                for (var i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                    ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                    if (!(item is ToolStripMenuItem menuItem)) { continue; }
                    if (menuItem.Checked && i+1 < this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.ProfileMenuItems[i+1].PerformClick();
                        break;
                    } else if (menuItem.Checked && i+1 >= this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.ProfileMenuItems[0].PerformClick();
                        break;
                    }
                }
            }
        }
        private void Overlay_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ShiftKey) {
                this.shiftKeyToggle = false;
            }
        }
        private void Overlay_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    this.shiftKeyToggle = true;
                    break;
                case Keys.T: {
                    int colorOption = 0;
                    if (BackColor.ToArgb() == Color.Black.ToArgb()) {
                        colorOption = 5;
                    } else if (BackColor.ToArgb() == Color.Green.ToArgb()) {
                        colorOption = 0;
                    } else if (BackColor.ToArgb() == Color.Magenta.ToArgb()) {
                        colorOption = 1;
                    } else if (BackColor.ToArgb() == Color.Blue.ToArgb()) {
                        colorOption = 2;
                    } else if (BackColor.ToArgb() == Color.Red.ToArgb()) {
                        colorOption = 3;
                    } else if (BackColor.ToArgb() == Color.FromArgb(224, 224, 224).ToArgb()) {
                        colorOption = 4;
                    }
                    SetBackgroundColor(colorOption);
                    this.StatsForm.CurrentSettings.OverlayColor = colorOption;
                    this.StatsForm.SaveUserSettings();
                    break;
                }
                case Keys.F:
                    this.FlipDisplay(!this.flippedImage);
                    this.Background = RecreateBackground();
                    this.StatsForm.CurrentSettings.FlippedDisplay = this.flippedImage;
                    this.StatsForm.SaveUserSettings();
                    break;
                case Keys.P when this.StatsForm.ProfileMenuItems.Count <= 1:
                    return;
                case Keys.P: {
                    for (var i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                        ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                        if (!(item is ToolStripMenuItem menuItem)) { continue; }

                        if (menuItem.Checked && i + 1 < this.StatsForm.ProfileMenuItems.Count) {
                            this.StatsForm.ProfileMenuItems[i + 1].PerformClick();
                            break;
                        } else if (menuItem.Checked && i + 1 >= this.StatsForm.ProfileMenuItems.Count) {
                            this.StatsForm.ProfileMenuItems[0].PerformClick();
                            break;
                        }
                    }

                    break;
                }
                case Keys.Up:
                    if (this.shiftKeyToggle == false) { return; }
                    for (var i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                        ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                        if (!(item is ToolStripMenuItem menuItem)) { continue; }
                        if (menuItem.Checked && i > 0) {
                            this.StatsForm.ProfileMenuItems[i-1].PerformClick();
                            break;
                        } else if (menuItem.Checked && i == 0) {
                            this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count-1].PerformClick();
                            break;
                        }
                    }
                    break;
                case Keys.Down:
                    if (this.shiftKeyToggle == false) { return; }
                    for (var i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                        ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                        if (!(item is ToolStripMenuItem menuItem)) { continue; }
                        if (menuItem.Checked && i+1 < this.StatsForm.ProfileMenuItems.Count) {
                            this.StatsForm.ProfileMenuItems[i+1].PerformClick();
                            break;
                        } else if (menuItem.Checked && i+1 >= this.StatsForm.ProfileMenuItems.Count) {
                            this.StatsForm.ProfileMenuItems[0].PerformClick();
                            break;
                        }
                    }
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                {
                    if (this.StatsForm.ProfileMenuItems.Count <= 1) { return; }
                    int i = Convert.ToInt32(((char)e.KeyValue).ToString());
                    if (i <= this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.ProfileMenuItems[i-1].PerformClick();
                    }

                    break;
                }
            }
        }
        public void SetBackgroundColor(int colorOption) {
            switch (colorOption) {
                case 0: BackColor = Color.Magenta; break;
                case 1: BackColor = Color.Blue; break;
                case 2: BackColor = Color.Red; break;
                case 3: BackColor = Color.FromArgb(224, 224, 224); break;
                case 4: BackColor = Color.Black; break;
                case 5: BackColor = Color.Green; break;
            }
        }
        public void ArrangeDisplay(bool flipDisplay, bool showTabs, bool hideWins, bool hideRound, bool hideTime, int colorOption, int? width, int? height, string serializedFont) {
            FlipDisplay(false);

            int heightOffset = showTabs ? 35 : 0;
            this.lblWins.Location = new Point(22, 9 + heightOffset);
            this.lblFinals.Location = new Point(22, 32 + heightOffset);
            this.lblStreak.Location = new Point(22, 55 + heightOffset);

            int spacerWidth = 6;
            int firstColumnX = 22;
            int firstColumnWidth = 242;
            int secondColumnX = firstColumnX + firstColumnWidth + spacerWidth;
            int secondColumnWidth = 281;
            int thirdColumnX = secondColumnX + secondColumnWidth + spacerWidth;
            int thirdColumnWidth = 225;

            this.lblWins.Size = new Size(firstColumnWidth, 22);
            this.lblFinals.Size = new Size(firstColumnWidth, 22);
            this.lblStreak.Size = new Size(firstColumnWidth, 22);

            this.drawWidth = firstColumnWidth + secondColumnWidth + thirdColumnWidth + spacerWidth * 3 + firstColumnX - 2;

            int overlaySetting = (hideWins ? 4 : 0) + (hideRound ? 2 : 0) + (hideTime ? 1 : 0);
            switch (overlaySetting) {
                case 0:
                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblName.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblName.DrawVisible = true;
                    this.lblQualifyChance.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    this.lblFastest.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblFastest.Size = new Size(secondColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;

                    this.lblPlayers.Location = new Point(thirdColumnX, 10 + heightOffset);
                    this.lblPlayers.Size = new Size(thirdColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(thirdColumnX+52, 13 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 13;
                        this.lblPlayersPs.ImageHeight = 13;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(thirdColumnX+90, 13 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 13;
                        this.lblPlayersXbox.ImageHeight = 13;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(thirdColumnX+128, 13 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 13;
                        this.lblPlayersSwitch.ImageHeight = 13;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(thirdColumnX+166, 13 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageWidth = 13;
                        this.lblPlayersPc.ImageHeight = 13;
                        this.lblPlayersPc.DrawVisible = true;
                    }
                    
                    this.lblDuration.Location = new Point(thirdColumnX, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(thirdColumnX, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 1:
                    this.drawWidth -= thirdColumnWidth + spacerWidth;

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblFastest.DrawVisible = false;
                    this.lblDuration.DrawVisible = false;
                    this.lblFinish.DrawVisible = false;

                    this.lblName.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblName.DrawVisible = true;
                    
                    this.lblPlayers.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblPlayers.Size = new Size(secondColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(secondColumnX+49, 35 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 18);
                        this.lblPlayersPs.ImageWidth = 12;
                        this.lblPlayersPs.ImageHeight = 12;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(secondColumnX+86, 35 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 18);
                        this.lblPlayersXbox.ImageWidth = 12;
                        this.lblPlayersXbox.ImageHeight = 12;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(secondColumnX+123, 35 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 18);
                        this.lblPlayersSwitch.ImageWidth = 12;
                        this.lblPlayersSwitch.ImageHeight = 12;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(secondColumnX+160, 35 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 18);
                        this.lblPlayersPc.ImageWidth = 12;
                        this.lblPlayersPc.ImageHeight = 12;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblQualifyChance.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    break;
                case 2:
                    this.drawWidth -= secondColumnWidth + spacerWidth;

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblName.DrawVisible = false;
                    this.lblQualifyChance.DrawVisible = false;
                    this.lblPlayers.DrawVisible = false;
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersPc.DrawVisible = false;

                    this.lblFastest.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblFastest.Size = new Size(thirdColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;
                    this.lblDuration.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 3:
                    this.drawWidth -= secondColumnWidth + thirdColumnWidth + spacerWidth * 2;

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblName.DrawVisible = false;
                    this.lblQualifyChance.DrawVisible = false;
                    this.lblPlayers.DrawVisible = false;
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersPc.DrawVisible = false;

                    this.lblFastest.DrawVisible = false;
                    this.lblDuration.DrawVisible = false;
                    this.lblFinish.DrawVisible = false;
                    break;
                case 4:
                    this.drawWidth -= firstColumnWidth + spacerWidth;

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblName.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblName.DrawVisible = true;
                    this.lblQualifyChance.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    this.lblFastest.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblFastest.Size = new Size(secondColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;

                    this.lblPlayers.Location = new Point(firstColumnX + secondColumnWidth + 6, 9 + heightOffset);
                    this.lblPlayers.Size = new Size(thirdColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + secondColumnWidth + 6 + 49, 12 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 18);
                        this.lblPlayersPs.ImageWidth = 12;
                        this.lblPlayersPs.ImageHeight = 12;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + secondColumnWidth + 6 + 86, 12 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 18);
                        this.lblPlayersXbox.ImageWidth = 12;
                        this.lblPlayersXbox.ImageHeight = 12;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + secondColumnWidth + 6 + 123, 12 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 18);
                        this.lblPlayersSwitch.ImageWidth = 12;
                        this.lblPlayersSwitch.ImageHeight = 12;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + secondColumnWidth + 6 + 160, 12 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 18);
                        this.lblPlayersPc.ImageWidth = 12;
                        this.lblPlayersPc.ImageHeight = 12;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblDuration.Location = new Point(firstColumnX + secondColumnWidth + 6, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(firstColumnX + secondColumnWidth + 6, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 5:
                    this.drawWidth -= firstColumnWidth + thirdColumnWidth + spacerWidth * 2;

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblName.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblName.DrawVisible = true;
                    
                    this.lblPlayers.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblPlayers.Size = new Size(secondColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX+49, 35 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 18);
                        this.lblPlayersPs.ImageWidth = 12;
                        this.lblPlayersPs.ImageHeight = 12;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX+86, 35 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 18);
                        this.lblPlayersXbox.ImageWidth = 12;
                        this.lblPlayersXbox.ImageHeight = 12;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX+123, 35 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 18);
                        this.lblPlayersSwitch.ImageWidth = 12;
                        this.lblPlayersSwitch.ImageHeight = 12;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX+160, 35 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 18);
                        this.lblPlayersPc.ImageWidth = 12;
                        this.lblPlayersPc.ImageHeight = 12;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblQualifyChance.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;

                    this.lblFastest.DrawVisible = false;
                    this.lblDuration.DrawVisible = false;
                    this.lblFinish.DrawVisible = false;
                    break;
                case 6:
                    this.drawWidth -= firstColumnWidth + secondColumnWidth + spacerWidth * 2;

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblName.DrawVisible = false;
                    this.lblQualifyChance.DrawVisible = false;
                    this.lblPlayers.DrawVisible = false;
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersPc.DrawVisible = false;

                    this.lblFastest.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblFastest.Size = new Size(thirdColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;
                    this.lblDuration.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
            }

            if (!string.IsNullOrEmpty(serializedFont)) {
                FontConverter fontConverter = new FontConverter();
                SetFonts(this, -1, fontConverter.ConvertFromString(serializedFont) as Font);
            } else {
                SetFonts(this);
            }

            this.DisplayTabs(showTabs);
            this.DisplayProfile(showTabs);
            this.FlipDisplay(flipDisplay);
            this.SetBackgroundColor(colorOption);

            this.Background = RecreateBackground();
            if (width.HasValue) {
                this.Width = width.Value;
            }
            if (height.HasValue) {
                this.Height = height.Value;
            }
        }
        public void FlipDisplay(bool flipped) {
            if (flipped == this.flippedImage) { return; }
            this.flippedImage = flipped;

            foreach (Control ctr in Controls) {
                if (ctr is TransparentLabel label && label != this.lblFilter && label != this.lblProfile) {
                    label.Location = new Point(label.Location.X + (this.flippedImage ? -18 : 18), label.Location.Y);
                }
            }

            this.DisplayTabs(this.drawHeight > 99);
            this.DisplayProfile(this.drawHeight > 99);
        }
        private int GetCountNumeric(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if (0x30 <= ch && ch <= 0x39) count++;
            }
            return count;
        }
        private int GetCountSpace(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if (0x20 == ch) count++;
            }
            return count;
        }
        private int GetCountBigSignCharacter(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if ((0x23 <= ch && ch <= 0x26)
                    || 0x2B == ch
                    || (0x3C <= ch && ch <= 0x40)
                    || 0x5C == ch
                    || 0x7E == ch) count++;
            }
            return count;
        }
        private int GetCountSmallSignCharacter(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if ((0x21 <= ch && ch <= 0x22)
                    || (0x27 <= ch && ch <= 0x2A)
                    || (0x2C <= ch && ch <= 0x2F)
                    || (0x3A <= ch && ch <= 0x3B)
                    || (0x60 <= ch)
                    || (0x7B <= ch && ch <= 0x7D)
                   ) count++;
            }
            return count;
        }
        private int GetCountKorAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if ((0xAC00 <= ch && ch <= 0xD7A3) //Korean
                    || (0x3131 <= ch && ch <= 0x318E) //Korean
                   ) count++;
            }
            return count;
        }
        private int GetCountJpnAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if ((0x3040 <= ch && ch <= 0x309F) //Japanese
                    || (0x30A0 <= ch && ch <= 0x30FF) //Japanese
                    || (0x3400 <= ch && ch <= 0x4DBF) //Japanese
                    || (0x4E00 <= ch && ch <= 0x9FBF) //Japanese
                    || (0xF900 <= ch && ch <= 0xFAFF) //Japanese
                   ) count++;
            }
            return count;
        }
        private int GetCountEnglishlowercase(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach(char ch in charArr) {
                if ((0x61 <= ch && ch <= 0x7A)) count++;
            }
            return count;
        }
        private int GetOverlayProfileOffset(string s) {
            int sizeOfText = TextRenderer.MeasureText(s, this.lblProfile.Font).Width;
            int offset;
            if (this.lblProfile.Font.FontFamily.Name.Equals(DefaultFontCollection.Families[1].Name)) {
                offset = 22 - (int)(this.GetCountEnglishlowercase(s) * (-0.3F)) - 
                         (int)(this.GetCountKorAlphabet(s) * (6.7F)) - 
                         (int)(this.GetCountJpnAlphabet(s) * (0.8F)) - 
                         (int)(this.GetCountBigSignCharacter(s) * (0.1F)) - 
                         (int)(this.GetCountSmallSignCharacter(s) * (0.2F));
            } else if (this.lblProfile.Font.FontFamily.Name.Equals(DefaultFontCollection.Families[0].Name)) {
                offset = 22 - (int)(this.GetCountBigSignCharacter(s) * (0.1F)) - 
                         (int)(this.GetCountSmallSignCharacter(s) * (0.2F));
            } else {
                offset = 22 - (int)(this.GetCountEnglishlowercase(s) * (-0.3F)) - 
                         (int)(this.GetCountKorAlphabet(s) * (1.8F)) - 
                         (int)(this.GetCountJpnAlphabet(s) * (1.8F)) - 
                         (int)(this.GetCountBigSignCharacter(s) * (0.1F)) - 
                         (int)(this.GetCountSmallSignCharacter(s) * (0.2F)) - 
                         (int)(this.GetCountNumeric(s) * (-0.1F));
            }
            return sizeOfText - offset;
        }
        private Bitmap RecreateBackground() {
            lock (DefaultFont) {
                if (this.Background != null) {
                    this.Background.Dispose();
                }

                bool tabsDisplayed = this.StatsForm.CurrentSettings.ShowOverlayTabs;
                //bool profileDisplayed = StatsForm.CurrentSettings.ShowOverlayProfile;
                Bitmap newImage = new Bitmap(this.drawWidth, this.drawHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage)) {
                    g.DrawImage(Properties.Resources.background, 0, tabsDisplayed ? 35 : 0);
                    if (tabsDisplayed) {
                        g.DrawImage(Properties.Resources.tab_unselected, this.drawWidth - 170 - this.GetOverlayProfileOffset(this.lblProfile.Text), 0);
                        g.DrawImage(Properties.Resources.tab_unselected, this.drawWidth - 110, 0);
                    }
                }

                if (this.flippedImage) {
                    newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }

                this.DrawGraphics.Dispose();
                this.DrawImage.Dispose();
                this.DrawImage = new Bitmap(newImage.Width, newImage.Height, PixelFormat.Format32bppArgb);
                this.DrawGraphics = Graphics.FromImage(this.DrawImage);

                return newImage;
            }
        }
        public void DisplayTabs(bool showTabs) {
            if (showTabs) {
                this.drawHeight = 134;
                this.lblFilter.Location = new Point(this.flippedImage ? -5 : this.drawWidth - 105, 9);
                this.lblFilter.DrawVisible = true;
            } else {
                this.drawHeight = 99;
                this.lblFilter.DrawVisible = false;
            }
        }
        public void DisplayProfile(bool showProfile) {
            if (showProfile) {
                this.drawHeight = 134;
                this.lblProfile.Location = new Point(this.flippedImage ? 125 : this.drawWidth - (145 + this.GetOverlayProfileOffset(this.lblProfile.Text)), 9);
                
                this.lblProfile.DrawVisible = true;
            } else {
                this.drawHeight = 99;
                this.lblProfile.DrawVisible = false;
            }
        }
        public void ChangeLanguage() {
            this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak")} :";
            this.lblStreak.TextRight = $"0{Multilingual.GetWord("overlay_streak_suffix")} ({Multilingual.GetWord("overlay_best")} 0{Multilingual.GetWord("overlay_streak_suffix")})";
            this.lblFinals.Text = $"{Multilingual.GetWord("overlay_finals")} :";
            this.lblFinals.TextRight = $"0{Multilingual.GetWord("overlay_inning")} - 0.0%";
            this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_chance")} :";
            this.lblFastest.Text = $"{Multilingual.GetWord("overlay_fastest")} :";
            this.lblDuration.Text = $"{Multilingual.GetWord("overlay_duration")} :";
            this.lblName.Text = $"{Multilingual.GetWord("overlay_name_prefix")}1{Multilingual.GetWord("overlay_name_suffix")} :";
            this.lblWins.Text = $"{Multilingual.GetWord("overlay_wins")} :";
            this.lblWins.TextRight = $"0{Multilingual.GetWord("overlay_inning")} - 0.0%";
            this.lblFinish.Text = $"{Multilingual.GetWord("overlay_finish")} :";
            this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_players")} :";
        }
    }
}