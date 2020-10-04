using NewTek.NDI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
        private Sender NDISender;
        private VideoFrame NDIFrame;
        private Bitmap NDIImage, DrawImage, Background;
        private Graphics NDIGraphics, DrawGraphics;
        private RoundInfo lastRound;
        private int triesToDownload, drawWidth, drawHeight;
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
        public void Cleanup() {
            try {
                lock (GlobalFont) {
                    if (NDIGraphics != null) {
                        NDIGraphics.Dispose();
                        NDIGraphics = null;
                    }
                    if (NDIImage != null) {
                        NDIImage.Dispose();
                        NDIImage = null;
                    }
                    if (NDIFrame != null) {
                        NDIFrame.Dispose();
                        NDIFrame = null;
                    }
                    if (NDISender != null) {
                        NDISender.Dispose();
                        NDISender = null;
                    }
                }
            } catch { }
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

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x84) {
                Point pos = this.PointToClient(new Point(m.LParam.ToInt32()));
                int hitSize = 16;
                if (pos.X >= this.ClientSize.Width - hitSize && pos.Y >= this.ClientSize.Height - hitSize) {
                    m.Result = (IntPtr)17;
                    return;
                } else if (pos.X <= hitSize && pos.Y >= this.ClientSize.Height - hitSize) {
                    m.Result = (IntPtr)16;
                    return;
                } else if (pos.X <= hitSize && pos.Y <= hitSize) {
                    m.Result = (IntPtr)13;
                    return;
                } else if (pos.X >= this.ClientSize.Width - hitSize && pos.Y <= hitSize) {
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
                    if (this.IsHandleCreated && !this.Disposing && !this.IsDisposed) {
                        frameCount++;
                        isTimeToSwitch = frameCount % (StatsForm.CurrentSettings.CycleTimeSeconds * 20) == 0;
                        this.Invoke((Action)UpdateInfo);
                    }

                    StatsForm.UpdateDates();
                    Thread.Sleep(50);
                } catch { }
            }
        }
        private void SetQualifyChanceLabel(StatSummary levelInfo) {
            if (!StatsForm.CurrentSettings.SwitchBetweenQualify) {
                return;
            }
            float qualifyChance;
            switch (switchCount % 2) {
                case 0:
                    lblQualifyChance.Text = "QUALIFY:";
                    qualifyChance = (float)levelInfo.TotalQualify * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    lblQualifyChance.TextRight = $"{levelInfo.TotalQualify} / {levelInfo.TotalPlays} - {qualifyChance:0.0}%";
                    break;
                case 1:
                    lblQualifyChance.Text = "GOLD:";
                    qualifyChance = (float)levelInfo.TotalGolds * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    lblQualifyChance.TextRight = $"{levelInfo.TotalGolds} / {levelInfo.TotalPlays} - {qualifyChance:0.0}%";
                    break;
            }
        }
        private void SetFastestLabel(StatSummary levelInfo, LevelStats level) {
            if (!StatsForm.CurrentSettings.SwitchBetweenLongest) {
                switchCount = level.Type.FastestLabel();
            }
            switch (switchCount % (levelInfo.BestScore.HasValue ? 3 : 2)) {
                case 0:
                    lblFastest.Text = "FASTEST:";
                    lblFastest.TextRight = levelInfo.BestFinish.HasValue ? $"{levelInfo.BestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 1:
                    lblFastest.Text = "LONGEST:";
                    lblFastest.TextRight = levelInfo.LongestFinish.HasValue ? $"{levelInfo.LongestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 2:
                    lblFastest.Text = "HIGH SCORE:";
                    lblFastest.TextRight = levelInfo.BestScore.Value.ToString();
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

                StatSummary levelInfo = StatsForm.GetLevelInfo(lastRound?.Name);
                lblFilter.Text = levelInfo.CurrentFilter;

                if (lastRound != null) {
                    lblName.Text = $"ROUND {lastRound.Round}:";
                    
                    lblName.TextRight = LevelStats.ALL.TryGetValue(lastRound.Name, out var level) ? level.Name.ToUpper() : String.Empty;
                    lblPlayers.TextRight = lastRound.Players.ToString();

                    float winChance = (float)levelInfo.TotalWins * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    if (StatsForm.CurrentSettings.PreviousWins > 0) {
                        lblWins.TextRight = $"{levelInfo.TotalWins} ({levelInfo.AllWins + StatsForm.CurrentSettings.PreviousWins}) - {winChance:0.0}%";
                    } else if (StatsForm.CurrentSettings.FilterType != 0) {
                        lblWins.TextRight = $"{levelInfo.TotalWins} ({levelInfo.AllWins}) - {winChance:0.0}%";
                    } else {
                        lblWins.TextRight = $"{levelInfo.TotalWins} - {winChance:0.0}%";
                    }

                    float finalChance = (float)levelInfo.TotalFinals * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    lblFinals.TextRight = $"{levelInfo.TotalFinals} - {finalChance:0.0}%";

                    lblStreak.TextRight = $"{levelInfo.CurrentStreak} (BEST {levelInfo.BestStreak})";

                    if (isTimeToSwitch) {
                        SetQualifyChanceLabel(levelInfo);
                        SetFastestLabel(levelInfo, level);
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
                        if (lastRound.Position > 0) {
                            lblFinish.TextRight = $"# {lastRound.Position} - {Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                        } else {
                            lblFinish.TextRight = $"{Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                        }
                    } else if (lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            lblFinish.TextRight = $"{DateTime.UtcNow - startTime:m\\:ss}";
                        } else {
                            lblFinish.TextRight = $"{DateTime.UtcNow - Start:m\\:ss}";
                        }
                    } else {
                        lblFinish.TextRight = "-";
                    }

                    if (End != DateTime.MinValue) {
                        lblDuration.TextRight = (End - Start).ToString("m\\:ss");
                    } else if (lastRound.Playing) {
                        if (Start > DateTime.UtcNow) {
                            lblDuration.TextRight = (DateTime.UtcNow - startTime).ToString("m\\:ss");
                        } else {
                            lblDuration.TextRight = (DateTime.UtcNow - Start).ToString("m\\:ss");
                        }
                    } else {
                        lblDuration.TextRight = "-";
                    }
                }
                this.Invalidate();
            }

            if (StatsForm.CurrentSettings.UseNDI) {
                SendNDI();
            }
        }
        private void SendNDI() {
            try {
                lock (GlobalFont) {
                    if (NDISender == null) {
                        NDISender = new Sender("Fall Guys Stats Overlay", true, false, null, "Fall Guys Stats Overlay");
                    }
                    if (NDIFrame == null) {
                        NDIFrame = new VideoFrame(Background.Width, Background.Height, (float)Background.Width / Background.Height, 20, 1);
                    }
                    if (NDIImage == null) {
                        NDIImage = new Bitmap(NDIFrame.Width, NDIFrame.Height, NDIFrame.Stride, PixelFormat.Format32bppPArgb, NDIFrame.BufferPtr);
                    }
                    if (NDIGraphics == null) {
                        NDIGraphics = Graphics.FromImage(NDIImage);
                        NDIGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    NDIGraphics.Clear(Color.Transparent);
                    NDIGraphics.DrawImage(Background, 0, 0);

                    foreach (Control control in Controls) {
                        if (control is TransparentLabel label) {
                            NDIGraphics.TranslateTransform(label.Location.X, label.Location.Y);
                            label.Draw(NDIGraphics);
                            NDIGraphics.TranslateTransform(-label.Location.X, -label.Location.Y);
                        }
                    }

                    NDISender.Send(NDIFrame);
                }
            } catch (Exception ex) {
                if (ex.Message.IndexOf("Unable to load dll", StringComparison.OrdinalIgnoreCase) >= 0) {
                    StatsForm.CurrentSettings.UseNDI = false;
                    Cleanup();
                    if (triesToDownload < 5) {
                        triesToDownload++;
                        Task.Run(delegate () {
                            try {
                                using (ZipWebClient web = new ZipWebClient()) {
                                    byte[] data = web.DownloadData($"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/NDI.zip");
                                    using (MemoryStream ms = new MemoryStream(data)) {
                                        using (ZipArchive zipFile = new ZipArchive(ms, ZipArchiveMode.Read)) {
                                            foreach (var entry in zipFile.Entries) {
                                                entry.ExtractToFile(entry.Name, true);
                                            }
                                        }
                                    }
                                }
                                StatsForm.CurrentSettings.UseNDI = true;
                            } catch {
                                Thread.Sleep(10000);
                            }
                        });
                    }
                }
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
        public void ArrangeDisplay(bool flipDisplay, bool showTabs, bool hideWins, bool hideRound, bool hideTime, int colorOption, int? width, int? height) {
            FlipDisplay(false);

            int heightOffset = showTabs ? 35 : 0;
            lblWins.Location = new Point(22, 9 + heightOffset);
            lblFinals.Location = new Point(22, 32 + heightOffset);
            lblStreak.Location = new Point(22, 55 + heightOffset);

            int overlaySetting = (hideWins ? 4 : 0) + (hideRound ? 2 : 0) + (hideTime ? 1 : 0);
            switch (overlaySetting) {
                case 0:
                    drawWidth = 786;

                    lblWins.DrawVisible = true;
                    lblFinals.DrawVisible = true;
                    lblStreak.DrawVisible = true;

                    lblName.Location = new Point(268, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblQualifyChance.Location = new Point(268, 32 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    lblFastest.Location = new Point(268, 55 + heightOffset);
                    lblFastest.Size = new Size(281, 22);
                    lblFastest.DrawVisible = true;

                    lblPlayers.Location = new Point(557, 9 + heightOffset);
                    lblPlayers.Size = new Size(225, 22);
                    lblPlayers.DrawVisible = true;
                    lblDuration.Location = new Point(557, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(557, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 1:
                    drawWidth = 786 - 225 - 6;

                    lblWins.DrawVisible = true;
                    lblFinals.DrawVisible = true;
                    lblStreak.DrawVisible = true;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;

                    lblName.Location = new Point(268, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblPlayers.Location = new Point(268, 32 + heightOffset);
                    lblPlayers.Size = new Size(281, 22);
                    lblPlayers.DrawVisible = true;
                    lblQualifyChance.Location = new Point(268, 55 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    break;
                case 2:
                    drawWidth = 786 - 281 - 6;

                    lblWins.DrawVisible = true;
                    lblFinals.DrawVisible = true;
                    lblStreak.DrawVisible = true;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.Location = new Point(268, 9 + heightOffset);
                    lblFastest.Size = new Size(225, 22);
                    lblFastest.DrawVisible = true;
                    lblDuration.Location = new Point(268, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(268, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 3:
                    drawWidth = 786 - 281 - 225 - 12;

                    lblWins.DrawVisible = true;
                    lblFinals.DrawVisible = true;
                    lblStreak.DrawVisible = true;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;
                    break;
                case 4:
                    drawWidth = 786 - 238 - 6;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.Location = new Point(22, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblQualifyChance.Location = new Point(22, 32 + heightOffset);
                    lblQualifyChance.DrawVisible = true;
                    lblFastest.Location = new Point(22, 55 + heightOffset);
                    lblFastest.Size = new Size(281, 22);
                    lblFastest.DrawVisible = true;

                    lblPlayers.Location = new Point(311, 9 + heightOffset);
                    lblPlayers.Size = new Size(225, 22);
                    lblPlayers.DrawVisible = true;
                    lblDuration.Location = new Point(311, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(311, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
                case 5:
                    drawWidth = 786 - 238 - 225 - 12;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.Location = new Point(22, 9 + heightOffset);
                    lblName.DrawVisible = true;
                    lblPlayers.Location = new Point(22, 32 + heightOffset);
                    lblPlayers.Size = new Size(281, 22);
                    lblPlayers.DrawVisible = true;
                    lblQualifyChance.Location = new Point(22, 55 + heightOffset);
                    lblQualifyChance.DrawVisible = true;

                    lblFastest.DrawVisible = false;
                    lblDuration.DrawVisible = false;
                    lblFinish.DrawVisible = false;
                    break;
                case 6:
                    drawWidth = 786 - 238 - 281 - 12;

                    lblWins.DrawVisible = false;
                    lblFinals.DrawVisible = false;
                    lblStreak.DrawVisible = false;

                    lblName.DrawVisible = false;
                    lblQualifyChance.DrawVisible = false;
                    lblPlayers.DrawVisible = false;

                    lblFastest.Location = new Point(22, 9 + heightOffset);
                    lblFastest.Size = new Size(225, 22);
                    lblFastest.DrawVisible = true;
                    lblDuration.Location = new Point(22, 32 + heightOffset);
                    lblDuration.DrawVisible = true;
                    lblFinish.Location = new Point(22, 55 + heightOffset);
                    lblFinish.DrawVisible = true;
                    break;
            }

            DisplayTabs(showTabs);
            FlipDisplay(flipDisplay);
            SetBackgroundColor(colorOption);
            Cleanup();

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