using NewTek.NDI;
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
        private int labelToShow;
        private Sender NDISender;
        private VideoFrame NDIFrame;
        private Bitmap NDIImage;
        private Graphics NDIGraphics;
        private RoundInfo lastRound;
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

            float winChance = (float)StatsForm.Wins * 100 / (StatsForm.Shows == 0 ? 1 : StatsForm.Shows);
            if (StatsForm.CurrentSettings.PreviousWins > 0) {
                lblWins.Text = $"{StatsForm.Wins} ({StatsForm.AllWins + StatsForm.CurrentSettings.PreviousWins}) - {winChance:0.0}%";
            } else if (StatsForm.CurrentSettings.FilterType != 0) {
                lblWins.Text = $"{StatsForm.Wins} ({StatsForm.AllWins}) - {winChance:0.0}%";
            } else {
                lblWins.Text = $"{StatsForm.Wins} - {winChance:0.0}%";
            }

            float finalChance = (float)StatsForm.Finals * 100 / (StatsForm.Shows == 0 ? 1 : StatsForm.Shows);
            lblFinalChance.Text = $"{StatsForm.Finals} - {finalChance:0.0}%";

            lock (StatsForm.CurrentRound) {
                bool hasCurrentRound = StatsForm.CurrentRound != null && StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound) {
                    lastRound = StatsForm.CurrentRound[StatsForm.CurrentRound.Count - 1];
                } else if (lastRound != null) {
                    lastRound.Playing = false;
                }

                if (lastRound != null) {
                    lblNameDesc.Text = $"ROUND {StatsForm.CurrentRound.Count}:";

                    string displayName = string.Empty;
                    LevelStats.DisplayNameLookup.TryGetValue(lastRound.Name, out displayName);
                    lblName.Text = displayName.ToUpper();
                    lblPlayers.Text = lastRound.Players.ToString();

                    StatSummary levelInfo = StatsForm.GetLevelInfo(lastRound.Name);
                    lblStreak.Text = $"{levelInfo.CurrentStreak} (BEST {levelInfo.BestStreak})";
                    float qualifyChance = (float)levelInfo.TotalQualify * 100 / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    lblQualifyChance.Text = $"{levelInfo.TotalQualify} / {levelInfo.TotalPlays} - {qualifyChance:0.0}%";
                    int modCount = levelInfo.BestScore.HasValue ? 3 : 2;
                    if ((labelToShow % modCount) == 1) {
                        lblFastestDesc.Text = "LONGEST:";
                        lblFastest.Text = levelInfo.LongestFinish.HasValue ? $"{levelInfo.LongestFinish:m\\:ss\\.ff}" : "-";
                    } else if ((labelToShow % modCount) == 2) {
                        lblFastestDesc.Text = "H SCORE:";
                        lblFastest.Text = levelInfo.BestScore.Value.ToString();
                    } else {
                        lblFastestDesc.Text = "FASTEST:";
                        lblFastest.Text = levelInfo.BestFinish.HasValue ? $"{levelInfo.BestFinish:m\\:ss\\.ff}" : "-";
                    }

                    DateTime Start = DateTime.MinValue;
                    if (lastRound.Start != DateTime.MinValue) { Start = lastRound.Start.Add(lastRound.Start - lastRound.Start.ToUniversalTime()); }
                    DateTime End = DateTime.MinValue;
                    if (lastRound.End != DateTime.MinValue) { End = lastRound.End.Add(lastRound.End - lastRound.End.ToUniversalTime()); }
                    DateTime? Finish = null;
                    if (lastRound.Finish.HasValue) { Finish = lastRound.Finish.Value.Add(lastRound.Finish.Value - lastRound.Finish.Value.ToUniversalTime()); }

                    if (Finish.HasValue) {
                        if (lastRound.Position > 0) {
                            lblFinish.Text = $"# {lastRound.Position} - {Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                        } else {
                            lblFinish.Text = $"{Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                        }
                    } else if (lastRound.Playing) {
                        lblFinish.Text = $"{DateTime.Now - Start:m\\:ss}";
                    } else {
                        lblFinish.Text = "-";
                    }

                    if (End != DateTime.MinValue) {
                        lblDuration.Text = (End - Start).ToString("m\\:ss");
                    } else if (lastRound.Playing) {
                        lblDuration.Text = (DateTime.Now - Start).ToString("m\\:ss");
                    } else {
                        lblDuration.Text = "-";
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
                StatsForm.UserSettings.Update(StatsForm.CurrentSettings);
            } else if (e.KeyCode == Keys.F) {
                FlipDisplay(!flippedImage);

                StatsForm.CurrentSettings.FlippedDisplay = flippedImage;
                StatsForm.UserSettings.Update(StatsForm.CurrentSettings);
            }
        }
        public void FlipDisplay(bool flipped) {
            if (flipped == flippedImage) { return; }

            if (flipped) {
                flippedImage = true;
                BackgroundImage = Properties.Resources.backgroundFlip;
                foreach (Control ctr in Controls) {
                    if (ctr is TransparentLabel label) {
                        label.Location = new Point(label.Location.X - 18, label.Location.Y);
                    }
                }
            } else {
                flippedImage = false;
                BackgroundImage = Properties.Resources.background;
                foreach (Control ctr in Controls) {
                    if (ctr is TransparentLabel label) {
                        label.Location = new Point(label.Location.X + 18, label.Location.Y);
                    }
                }
            }
        }
    }
}