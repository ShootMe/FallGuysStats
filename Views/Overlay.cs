using System;
using System.Drawing;
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
            GlobalFont = new Font(CustomFont.Families[0], 10, FontStyle.Regular, GraphicsUnit.Point);
        }
        public Overlay() {
            InitializeComponent();

            foreach (Control c in Controls) {
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
                font = customSize <= 0 ? GlobalFont : new Font(CustomFont.Families[0], customSize, FontStyle.Regular, GraphicsUnit.Point);
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
                    if (this.Visible && this.IsHandleCreated && !this.Disposing && !this.IsDisposed) {
                        this.Invoke((Action)UpdateInfo);
                    }
                    Thread.Sleep(50);
                } catch { }
            }
        }
        private void UpdateInfo() {
            if (StatsForm == null) { return; }

            float winChance = (float)StatsForm.Wins * 100 / (StatsForm.Shows == 0 ? 1 : StatsForm.Shows);
            lblWins.Text = $"{StatsForm.Wins} - {winChance:0.0}%";
            float finalChance = (float)StatsForm.Finals * 100 / (StatsForm.Shows == 0 ? 1 : StatsForm.Shows);
            lblFinalChance.Text = $"{StatsForm.Finals} - {finalChance:0.0}%";

            bool hasCurrentRound = StatsForm.CurrentRound != null && StatsForm.CurrentRound.Count > 0;
            if (hasCurrentRound) {
                RoundInfo info = StatsForm.CurrentRound[StatsForm.CurrentRound.Count - 1];
                if (StatsForm.RoundChanged) {
                    StatsForm.RoundChanged = false;
                    lblRound.Text = string.Concat((info.End != DateTime.MinValue ? "Last" : "Current"), $" Round {StatsForm.CurrentRound.Count}");
                    string displayName = string.Empty;
                    LevelStats.DisplayNameLookup.TryGetValue(info.Name, out displayName);
                    lblName.Text = displayName;
                    lblPlayers.Text = info.Players.ToString();
                    Tuple<int, int, TimeSpan?> levelInfo = StatsForm.GetLevelInfo(info.Name);
                    float qualifyChance = (float)levelInfo.Item2 * 100 / (levelInfo.Item1 == 0 ? 1 : levelInfo.Item1);
                    lblQualifyChance.Text = $"{levelInfo.Item2} / {levelInfo.Item1} - {qualifyChance:0.0}%";
                    lblFastest.Text = levelInfo.Item3.HasValue ? $"{levelInfo.Item3:m\\:ss\\.ff}" : "-";
                }

                DateTime Start = DateTime.MinValue;
                if (info.Start != DateTime.MinValue) { Start = info.Start.Add(info.Start - info.Start.ToUniversalTime()); }
                DateTime End = DateTime.MinValue;
                if (info.End != DateTime.MinValue) { End = info.End.Add(info.End - info.End.ToUniversalTime()); }
                DateTime? Finish = null;
                if (info.Finish.HasValue) { Finish = info.Finish.Value.Add(info.Finish.Value - info.Finish.Value.ToUniversalTime()); }

                if (Finish.HasValue) {
                    if (info.Position > 0) {
                        lblFinish.Text = $"# {info.Position} - {Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                    } else {
                        lblFinish.Text = $"{Finish.GetValueOrDefault(End) - Start:m\\:ss\\.ff}";
                    }
                } else if (info.Playing) {
                    lblFinish.Text = $"{DateTime.Now - Start:m\\:ss\\.ff}";
                } else {
                    lblFinish.Text = "-";
                }

                if (End != DateTime.MinValue) {
                    lblDuration.Text = (End - Start).ToString("m\\:ss");
                } else if (info.Playing) {
                    lblDuration.Text = (DateTime.Now - Start).ToString("m\\:ss");
                } else {
                    lblDuration.Text = "-";
                }
            }
        }
    }
}