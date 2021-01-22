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

        public static PrivateFontCollection CustomFont;
        public static Font GlobalFont;
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
        static Overlay() {
            if (!File.Exists("TitanOne-Regular.ttf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.TitanOne-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("TitanOne-Regular.ttf", fontdata);
                }
            }

            CustomFont = new PrivateFontCollection();
            CustomFont.AddFontFile("TitanOne-Regular.ttf");
            GlobalFont = new Font(CustomFont.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
        }
        public Overlay() {
            InitializeComponent();

            Bitmap background = Properties.Resources.background;
            Bitmap newImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.DrawImage(background, 0, 0);
            }
            Background = newImage;

            DrawImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            DrawGraphics = Graphics.FromImage(DrawImage);
            drawWidth = background.Width;
            drawHeight = background.Height;

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
            timer = new Thread(UpdateTimer);
            timer.IsBackground = true;
            timer.Start();
        }
        public static void SetFonts(Control control, float customSize = -1, Font font = null) {
            if (font == null) {
                font = customSize <= 0 ? GlobalFont : new Font(CustomFont.Families[0], customSize, FontStyle.Regular, GraphicsUnit.Pixel);
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
            while (StatsForm != null && !StatsForm.IsDisposed && !StatsForm.Disposing) {
                try {
                    if (IsHandleCreated && !Disposing && !IsDisposed) {
                        frameCount++;
                        isTimeToSwitch = frameCount % (StatsForm.CurrentSettings.CycleTimeSeconds * 20) == 0;
                        Invoke((Action)UpdateInfo);
                    }

                    StatsForm.UpdateDates();
                    Thread.Sleep(50);
                } catch { }
            }
        }
        private void SetQualifyChanceLabel(StatSummary levelInfo) {
            int qualifySwitchCount = switchCount;
            if (!StatsForm.CurrentSettings.SwitchBetweenQualify) {
                qualifySwitchCount = StatsForm.CurrentSettings.OnlyShowGold ? 1 : 0;
            }
            float qualifyChance;
            string qualifyChanceDisplay;
            switch (qualifySwitchCount % 2) {
                case 0:
                    lblQualifyChance.Text = "QUALIFY:";
                    qualifyChance = levelInfo.TotalQualify * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    qualifyChanceDisplay = StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {qualifyChance:0.0}%";
                    lblQualifyChance.TextRight = $"{levelInfo.TotalQualify} / {levelInfo.TotalPlays}{qualifyChanceDisplay}";
                    break;
                case 1:
                    lblQualifyChance.Text = "GOLD:";
                    qualifyChance = levelInfo.TotalGolds * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    qualifyChanceDisplay = StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {qualifyChance:0.0}%";
                    lblQualifyChance.TextRight = $"{levelInfo.TotalGolds} / {levelInfo.TotalPlays}{qualifyChanceDisplay}";
                    break;
            }
        }
        private void SetFastestLabel(StatSummary levelInfo, LevelType type) {
            int fastestSwitchCount = switchCount;
            if (!StatsForm.CurrentSettings.SwitchBetweenLongest) {
                fastestSwitchCount = StatsForm.CurrentSettings.OnlyShowLongest ? 0 : type.FastestLabel();
            }
            switch (fastestSwitchCount % (levelInfo.BestScore.HasValue ? 3 : 2)) {
                case 0:
                    lblFastest.Text = "LONGEST:";
                    lblFastest.TextRight = levelInfo.LongestFinish.HasValue ? $"{levelInfo.LongestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 1:
                    lblFastest.Text = "FASTEST:";
                    lblFastest.TextRight = levelInfo.BestFinish.HasValue ? $"{levelInfo.BestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 2:
                    lblFastest.Text = "HIGH SCORE:";
                    lblFastest.TextRight = levelInfo.BestScore.Value.ToString();
                    break;
            }
        }
        private void SetPlayersLabel() {
            int playersSwitchCount = switchCount;
            if (!StatsForm.CurrentSettings.SwitchBetweenPlayers) {
                playersSwitchCount = StatsForm.CurrentSettings.OnlyShowPing ? 1 : 0;
            }
            switch (playersSwitchCount % 2) {
                case 0:
                    lblPlayers.Text = "PLAYERS:";
                    lblPlayers.TextRight = lastRound?.Players.ToString();
                    break;
                case 1:
                    lblPlayers.Text = "PING:";
                    lblPlayers.TextRight = Stats.InShow && Stats.LastServerPing != 0 ? $"{Stats.LastServerPing} ms" : "-";
                    break;
            }
        }
        private void SetStreakInfo(StatSummary levelInfo) {
            int streakSwitchCount = switchCount;
            if (!StatsForm.CurrentSettings.SwitchBetweenStreaks) {
                streakSwitchCount = StatsForm.CurrentSettings.OnlyShowFinalStreak ? 1 : 0;
            }
            switch (streakSwitchCount % 2) {
                case 0:
                    lblStreak.Text = "WIN STREAK:";
                    lblStreak.TextRight = $"{levelInfo.CurrentStreak} ({levelInfo.BestStreak})";
                    break;
                case 1:
                    lblStreak.Text = "FINAL STREAK:";
                    lblStreak.TextRight = $"{levelInfo.CurrentFinalStreak} ({levelInfo.BestFinalStreak})";
                    break;
            }
        }
        private void UpdateInfo() {
            if (StatsForm == null) { return; }

            lock (StatsForm.CurrentRound) {
                bool hasCurrentRound = StatsForm.CurrentRound != null && StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound && (lastRound == null || Stats.InShow || Stats.EndedShow)) {
                    if (Stats.EndedShow) {
                        Stats.EndedShow = false;
                    }
                    lastRound = StatsForm.CurrentRound[StatsForm.CurrentRound.Count - 1];
                }

                lblFilter.Text = StatsForm.GetCurrentFilter();

                if (lastRound != null && !string.IsNullOrEmpty(lastRound.Name)) {
                    string roundName = lastRound.VerifiedName();
                    lblName.Text = $"ROUND {lastRound.Round}:";

                    if (StatsForm.StatLookup.TryGetValue(roundName, out var level)) {
                        roundName = level.Name.ToUpper();
                    }

                    if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                        roundName = roundName.Substring(6).Replace('_', ' ').ToUpper();
                    }
                    if (roundName.Length > 15) { roundName = roundName.Substring(0, 15); }

                    StatSummary levelInfo = StatsForm.GetLevelInfo(roundName);
                    lblName.TextRight = roundName;

                    float winChance = levelInfo.TotalWins * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    string winChanceDisplay = StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {winChance:0.0}%";
                    if (StatsForm.CurrentSettings.PreviousWins > 0) {
                        lblWins.TextRight = $"{levelInfo.TotalWins} ({levelInfo.AllWins + StatsForm.CurrentSettings.PreviousWins}){winChanceDisplay}";
                    } else if (StatsForm.CurrentSettings.FilterType != 0) {
                        lblWins.TextRight = $"{levelInfo.TotalWins} ({levelInfo.AllWins}){winChanceDisplay}";
                    } else {
                        lblWins.TextRight = $"{levelInfo.TotalWins}{winChanceDisplay}";
                    }

                    float finalChance = levelInfo.TotalFinals * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    string finalText = $"{levelInfo.TotalFinals} / {levelInfo.TotalShows}";
                    string finalChanceDisplay = StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : finalText.Length > 9 ? $" - {finalChance:0}%" : $" - {finalChance:0.0}%";
                    lblFinals.TextRight = $"{finalText}{finalChanceDisplay}";

                    SetQualifyChanceLabel(levelInfo);
                    LevelType levelType = (level?.Type).GetValueOrDefault();
                    SetFastestLabel(levelInfo, levelType);
                    SetPlayersLabel();
                    SetStreakInfo(levelInfo);
                    if (isTimeToSwitch) {
                        switchCount++;
                    }

                    DateTime Start = lastRound.Start;
                    DateTime End = lastRound.End;
                    DateTime? Finish = lastRound.Finish;

                    if (lastRound.Playing != startedPlaying) {
                        if (lastRound.Playing) {
                            startTime = DateTime.UtcNow;
                        }
                        startedPlaying = lastRound.Playing;
                    }

                    if (Finish.HasValue) {
                        TimeSpan Time = Finish.GetValueOrDefault(End) - Start;
                        if (lastRound.Position > 0) {
                            lblFinish.TextRight = $"# {lastRound.Position} - {Time:m\\:ss\\.ff}";
                        } else {
                            lblFinish.TextRight = $"{Time:m\\:ss\\.ff}";
                        }

                        if (levelType == LevelType.Race || levelType == LevelType.Hunt || roundName == "ROCK'N'ROLL" || roundName == "SNOWY SCRAP") {
                            if (Time < levelInfo.BestFinish.GetValueOrDefault(TimeSpan.MaxValue) && Time > levelInfo.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                lblFinish.ForeColor = Color.LightGreen;
                            } else if (Time < levelInfo.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                lblFinish.ForeColor = Color.Gold;
                            }
                        } else if (Time > levelInfo.LongestFinish && Time < levelInfo.LongestFinishOverall) {
                            lblFinish.ForeColor = Color.LightGreen;
                        } else if (Time > levelInfo.LongestFinishOverall) {
                            lblFinish.ForeColor = Color.Gold;
                        }
                    } else if (lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            lblFinish.TextRight = $"{DateTime.UtcNow - startTime:m\\:ss}";
                        } else {
                            lblFinish.TextRight = $"{DateTime.UtcNow - Start:m\\:ss}";
                        }
                    } else {
                        lblFinish.TextRight = "-";
                        lblFinish.ForeColor = Color.White;
                    }

                    if (lastRound.GameDuration > 0) {
                        lblDuration.Text = $"TIME ({TimeSpan.FromSeconds(lastRound.GameDuration):m\\:ss}):";
                    } else {
                        lblDuration.Text = "TIME:";
                    }

                    if (End != DateTime.MinValue) {
                        lblDuration.TextRight = $"{End - Start:m\\:ss\\.ff}";
                    } else if (lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            lblDuration.TextRight = $"{DateTime.UtcNow - startTime:m\\:ss}";
                        } else {
                            lblDuration.TextRight = $"{DateTime.UtcNow - Start:m\\:ss}";
                        }
                    } else {
                        lblDuration.TextRight = "-";
                    }
                }
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            lock (GlobalFont) {
                DrawGraphics.Clear(Color.Transparent);
                DrawGraphics.DrawImage(Background, 0, 0);

                foreach (Control control in Controls) {
                    if (control is TransparentLabel label) {
                        DrawGraphics.TranslateTransform(label.Location.X, label.Location.Y);
                        label.Draw(DrawGraphics);
                        DrawGraphics.TranslateTransform(-label.Location.X, -label.Location.Y);
                    }
                }

                e.Graphics.InterpolationMode = InterpolationMode.Default;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.DrawImage(DrawImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), new Rectangle(0, 0, DrawImage.Width, DrawImage.Height), GraphicsUnit.Pixel);
            }
        }
        private void Overlay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.T) {
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

                StatsForm.CurrentSettings.OverlayColor = colorOption;
                StatsForm.SaveUserSettings();
            } else if (e.KeyCode == Keys.F) {
                FlipDisplay(!flippedImage);
                Background = RecreateBackground();

                StatsForm.CurrentSettings.FlippedDisplay = flippedImage;
                StatsForm.SaveUserSettings();
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
            lblWins.Location = new Point(22, 9 + heightOffset);
            lblFinals.Location = new Point(22, 32 + heightOffset);
            lblStreak.Location = new Point(22, 55 + heightOffset);

            int spacerWidth = 6;
            int firstColumnX = 22;
            int firstColumnWidth = 242;
            int secondColumnX = firstColumnX + firstColumnWidth + spacerWidth;
            int secondColumnWidth = 281;
            int thirdColumnX = secondColumnX + secondColumnWidth + spacerWidth;
            int thirdColumnWidth = 225;

            lblWins.Size = new Size(firstColumnWidth, 22);
            lblFinals.Size = new Size(firstColumnWidth, 22);
            lblStreak.Size = new Size(firstColumnWidth, 22);

            drawWidth = firstColumnWidth + secondColumnWidth + thirdColumnWidth + spacerWidth * 3 + firstColumnX - 2;

            int overlaySetting = (hideWins ? 4 : 0) + (hideRound ? 2 : 0) + (hideTime ? 1 : 0);
            switch (overlaySetting) {
                case 0:
                    lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblWins.DrawVisible = true;
                    lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblFinals.DrawVisible = true;
                    lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblStreak.DrawVisible = true;

                    lblName.Location = new Point(secondColumnX, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblQualifyChance.Location = new Point(secondColumnX, 32 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    lblFastest.Location = new Point(secondColumnX, 55 + heightOffset);
                    lblFastest.Size = new Size(secondColumnWidth, 22);
                    lblFastest.DrawVisible = true;

                    lblPlayers.Location = new Point(thirdColumnX, 9 + heightOffset);
                    lblPlayers.Size = new Size(thirdColumnWidth, 22);
                    lblPlayers.DrawVisible = true;
                    lblDuration.Location = new Point(thirdColumnX, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(thirdColumnX, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 1:
                    drawWidth -= thirdColumnWidth + spacerWidth;

                    lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblWins.DrawVisible = true;
                    lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblFinals.DrawVisible = true;
                    lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblStreak.DrawVisible = true;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;

                    lblName.Location = new Point(secondColumnX, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblPlayers.Location = new Point(secondColumnX, 32 + heightOffset);
                    lblPlayers.Size = new Size(secondColumnWidth, 22);
                    lblPlayers.DrawVisible = true;
                    lblQualifyChance.Location = new Point(secondColumnX, 55 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    break;
                case 2:
                    drawWidth -= secondColumnWidth + spacerWidth;

                    lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblWins.DrawVisible = true;
                    lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblFinals.DrawVisible = true;
                    lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblStreak.DrawVisible = true;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.Location = new Point(secondColumnX, 9 + heightOffset);
                    lblFastest.Size = new Size(thirdColumnWidth, 22);
                    lblFastest.DrawVisible = true;
                    lblDuration.Location = new Point(secondColumnX, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(secondColumnX, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 3:
                    drawWidth -= secondColumnWidth + thirdColumnWidth + spacerWidth * 2;

                    lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblWins.DrawVisible = true;
                    lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblFinals.DrawVisible = true;
                    lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblStreak.DrawVisible = true;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;
                    break;
                case 4:
                    drawWidth -= firstColumnWidth + spacerWidth;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblQualifyChance.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    lblFastest.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblFastest.Size = new Size(secondColumnWidth, 22);
                    lblFastest.DrawVisible = true;

                    lblPlayers.Location = new Point(firstColumnX + secondColumnWidth + 6, 9 + heightOffset);
                    lblPlayers.Size = new Size(thirdColumnWidth, 22);
                    lblPlayers.DrawVisible = true;
                    lblDuration.Location = new Point(firstColumnX + secondColumnWidth + 6, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(firstColumnX + secondColumnWidth + 6, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 5:
                    drawWidth -= firstColumnWidth + thirdColumnWidth + spacerWidth * 2;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblPlayers.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblPlayers.Size = new Size(secondColumnWidth, 22);
                    lblPlayers.DrawVisible = true;
                    lblQualifyChance.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblQualifyChance.DrawVisible = true;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;
                    break;
                case 6:
                    drawWidth -= firstColumnWidth + secondColumnWidth + spacerWidth * 2;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.Location = new Point(firstColumnX, 9 + heightOffset);
                    lblFastest.Size = new Size(thirdColumnWidth, 22);
                    lblFastest.DrawVisible = true;
                    lblDuration.Location = new Point(firstColumnX, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(firstColumnX, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
            }

            if (!string.IsNullOrEmpty(serializedFont)) {
                FontConverter fontConverter = new FontConverter();
                SetFonts(this, -1, fontConverter.ConvertFromString(serializedFont) as Font);
            } else {
                SetFonts(this);
            }

            DisplayTabs(showTabs);
            FlipDisplay(flipDisplay);
            SetBackgroundColor(colorOption);

            Background = RecreateBackground();
            if (width.HasValue) {
                Width = width.Value;
            }
            if (height.HasValue) {
                Height = height.Value;
            }
        }
        public void FlipDisplay(bool flipped) {
            if (flipped == flippedImage) { return; }
            flippedImage = flipped;

            foreach (Control ctr in Controls) {
                if (ctr is TransparentLabel label && label != lblFilter) {
                    label.Location = new Point(label.Location.X + (flippedImage ? -18 : 18), label.Location.Y);
                }
            }

            DisplayTabs(drawHeight > 99);
        }
        private Bitmap RecreateBackground() {
            lock (GlobalFont) {
                if (Background != null) {
                    Background.Dispose();
                }

                bool tabsDisplayed = StatsForm.CurrentSettings.ShowOverlayTabs;
                Bitmap newImage = new Bitmap(drawWidth, drawHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage)) {
                    g.DrawImage(Properties.Resources.background, 0, tabsDisplayed ? 35 : 0);
                    if (tabsDisplayed) {
                        g.DrawImage(Properties.Resources.tab_unselected, drawWidth - 110, 0);
                    }
                }

                if (flippedImage) {
                    newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }

                DrawGraphics.Dispose();
                DrawImage.Dispose();
                DrawImage = new Bitmap(newImage.Width, newImage.Height, PixelFormat.Format32bppArgb);
                DrawGraphics = Graphics.FromImage(DrawImage);

                return newImage;
            }
        }
        public void DisplayTabs(bool showTabs) {
            if (showTabs) {
                drawHeight = 134;
                lblFilter.Location = new Point(flippedImage ? -5 : drawWidth - 105, 11);
                lblFilter.DrawVisible = true;
            } else {
                drawHeight = 99;
                lblFilter.DrawVisible = false;
            }
        }
    }
}