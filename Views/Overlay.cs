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
        private int labelToShow;
        private Sender NDISender;
        private VideoFrame NDIFrame;
        private Bitmap NDIImage;
        private Graphics NDIGraphics;
        private RoundInfo lastRound;
        private int triesToDownload;
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

            Bitmap newImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.DrawImage(Properties.Resources.background, 0, 0);
            }
            BackgroundImage = newImage;

            foreach (Control c in Controls) {
                if (c is TransparentLabel label) {
                    label.Parent = this;
                    label.BackColor = Color.Transparent;
                }
                c.MouseDown += Overlay_MouseDown;
            }

            SetFonts(this);
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
                        if (StatsForm.CurrentSettings.SwitchBetweenLongest) {
                            if ((frameCount % (StatsForm.CurrentSettings.CycleTimeSeconds * 20)) == 0) {
                                labelToShow++;
                            }
                        } else {
                            labelToShow = 0;
                        }

                        this.Invoke((Action)UpdateInfo);
                    }

                    Thread.Sleep(50);
                } catch { }
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

                    string displayName = string.Empty;
                    LevelStats.DisplayNameLookup.TryGetValue(lastRound.Name, out displayName);
                    lblName.TextRight = displayName.ToUpper();
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
                    lblFinalChance.TextRight = $"{levelInfo.TotalFinals} - {finalChance:0.0}%";

                    lblStreak.TextRight = $"{levelInfo.CurrentStreak} (BEST {levelInfo.BestStreak})";

                    if ((labelToShow % 2) == 0) {
                        lblQualifyChance.Text = "QUALIFY:";
                        float qualifyChance = (float)levelInfo.TotalQualify * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                        lblQualifyChance.TextRight = $"{levelInfo.TotalQualify} / {levelInfo.TotalPlays} - {qualifyChance:0.0}%";
                    } else {
                        lblQualifyChance.Text = "GOLD:";
                        float qualifyChance = (float)levelInfo.TotalGolds * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                        lblQualifyChance.TextRight = $"{levelInfo.TotalGolds} / {levelInfo.TotalPlays} - {qualifyChance:0.0}%";
                    }

                    int modCount = levelInfo.BestScore.HasValue ? 3 : 2;
                    if ((labelToShow % modCount) == 1) {
                        lblFastest.Text = "FASTEST:";
                        lblFastest.TextRight = levelInfo.BestFinish.HasValue ? $"{levelInfo.BestFinish:m\\:ss\\.ff}" : "-";
                    } else if ((labelToShow % modCount) == 2) {
                        lblFastest.Text = "HIGH SCORE:";
                        lblFastest.TextRight = levelInfo.BestScore.Value.ToString();
                    } else {
                        lblFastest.Text = "LONGEST:";
                        lblFastest.TextRight = levelInfo.LongestFinish.HasValue ? $"{levelInfo.LongestFinish:m\\:ss\\.ff}" : "-";
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
                        NDIFrame = new VideoFrame(ClientSize.Width, ClientSize.Height, (float)ClientSize.Width / ClientSize.Height, 20, 1);
                    }
                    if (NDIImage == null) {
                        NDIImage = new Bitmap(NDIFrame.Width, NDIFrame.Height, NDIFrame.Stride, PixelFormat.Format32bppPArgb, NDIFrame.BufferPtr);
                    }
                    if (NDIGraphics == null) {
                        NDIGraphics = Graphics.FromImage(NDIImage);
                        NDIGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    NDIGraphics.Clear(Color.Transparent);
                    NDIGraphics.DrawImage(BackgroundImage, 0, 0);

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
        private void Overlay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.T) {
                int colorOption = 0;
                if (BackColor.ToArgb() == Color.Black.ToArgb()) {
                    colorOption = 5;
                    BackColor = Color.Green;
                } else if (BackColor.ToArgb() == Color.Green.ToArgb()) {
                    colorOption = 0;
                    BackColor = Color.Magenta;
                } else if (BackColor.ToArgb() == Color.Magenta.ToArgb()) {
                    colorOption = 1;
                    BackColor = Color.Blue;
                } else if (BackColor.ToArgb() == Color.Blue.ToArgb()) {
                    colorOption = 2;
                    BackColor = Color.Red;
                } else if (BackColor.ToArgb() == Color.Red.ToArgb()) {
                    colorOption = 3;
                    BackColor = Color.FromArgb(224, 224, 224);
                } else if (BackColor.ToArgb() == Color.FromArgb(224, 224, 224).ToArgb()) {
                    colorOption = 4;
                    BackColor = Color.Black;
                }

                StatsForm.CurrentSettings.OverlayColor = colorOption;
                StatsForm.SaveUserSettings();
            } else if (e.KeyCode == Keys.F) {
                FlipDisplay(!flippedImage);
                BackgroundImage = RecreateBackground();

                StatsForm.CurrentSettings.FlippedDisplay = flippedImage;
                StatsForm.SaveUserSettings();
            }
        }
        public void ArrangeDisplay(bool flipDisplay, bool showTabs, bool hideRound, bool hideTime) {
            FlipDisplay(false);

            int heightOffset = showTabs ? 35 : 0;
            lblWins.Location = new Point(22, 9 + heightOffset);
            lblFinalChance.Location = new Point(22, 32 + heightOffset);
            lblStreak.Location = new Point(22, 55 + heightOffset);

            if (!hideRound && !hideTime) {
                ClientSize = new Size(786, Height);

                lblName.Location = new Point(268, 9 + heightOffset);
                lblName.Visible = true;
                lblQualifyChance.Location = new Point(268, 32 + heightOffset);
                lblQualifyChance.Visible = true;
                lblFastest.Location = new Point(268, 55 + heightOffset);
                lblFastest.Size = new Size(281, 22);
                lblFastest.Visible = true;

                lblPlayers.Location = new Point(557, 9 + heightOffset);
                lblPlayers.Size = new Size(225, 22);
                lblPlayers.Visible = true;
                lblDuration.Location = new Point(557, 32 + heightOffset);
                lblDuration.Visible = true;
                lblFinish.Location = new Point(557, 55 + heightOffset);
                lblFinish.Visible = true;
            } else if (!hideRound) {
                ClientSize = new Size(555, Height);

                lblFastest.Visible = false;
                lblDuration.Visible = false;
                lblFinish.Visible = false;

                lblName.Location = new Point(268, 9 + heightOffset);
                lblName.Visible = true;
                lblPlayers.Location = new Point(268, 32 + heightOffset);
                lblPlayers.Size = new Size(281, 22);
                lblPlayers.Visible = true;
                lblQualifyChance.Location = new Point(268, 55 + heightOffset);
                lblQualifyChance.Visible = true;
            } else if (!hideTime) {
                ClientSize = new Size(499, Height);

                lblName.Visible = false;
                lblQualifyChance.Visible = false;
                lblPlayers.Visible = false;

                lblFastest.Location = new Point(268, 9 + heightOffset);
                lblFastest.Size = new Size(225, 22);
                lblFastest.Visible = true;
                lblDuration.Location = new Point(268, 32 + heightOffset);
                lblDuration.Visible = true;
                lblFinish.Location = new Point(268, 55 + heightOffset);
                lblFinish.Visible = true;
            } else {
                ClientSize = new Size(266, Height);

                lblFastest.Visible = false;
                lblDuration.Visible = false;
                lblFinish.Visible = false;
                lblName.Visible = false;
                lblQualifyChance.Visible = false;
                lblPlayers.Visible = false;
            }

            DisplayTabs(showTabs);
            FlipDisplay(flipDisplay);
            Cleanup();

            BackgroundImage = RecreateBackground();
        }
        public void FlipDisplay(bool flipped) {
            if (flipped == flippedImage) { return; }
            flippedImage = flipped;

            foreach (Control ctr in Controls) {
                if (ctr is TransparentLabel label && label != lblFilter) {
                    label.Location = new Point(label.Location.X + (flippedImage ? -18 : 18), label.Location.Y);
                }
            }

            DisplayTabs(Height > 99);
        }
        private Bitmap RecreateBackground() {
            if (BackgroundImage != null) {
                BackgroundImage.Dispose();
            }

            bool tabsDisplayed = Height > 99;
            Bitmap newImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.DrawImage(Properties.Resources.background, 0, tabsDisplayed ? 35 : 0);
                if (tabsDisplayed) {
                    g.DrawImage(Properties.Resources.tab_unselected, Width - 110, 0);
                }
            }

            if (flippedImage) {
                newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return newImage;
        }
        public void DisplayTabs(bool showTabs) {
            if (showTabs) {
                ClientSize = new Size(Width, 134);
                lblFilter.Location = new Point(flippedImage ? -5 : Width - 105, 11);
                lblFilter.Visible = true;
            } else {
                ClientSize = new Size(Width, 99);
                lblFilter.Visible = false;
            }
        }
    }
}