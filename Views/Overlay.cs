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
        public string BackgroundResourceName;
        public string TabResourceName;
        private Thread timer;
        private bool flippedImage;
        private int frameCount;
        private bool isTimeToSwitch;
        private int switchCount;
        private Bitmap Background, DrawImage;
        private Graphics DrawGraphics;
        private RoundInfo lastRound;
        private int levelException;
        private int drawWidth, drawHeight;
        private bool startedPlaying;
        private DateTime startTime;
        private bool shiftKeyToggle, ctrlKeyToggle;
        private new Size DefaultSize;

        private bool isPositionButtonMouseEnter;
        private readonly Image positionNeOffBlur = Stats.ImageOpacity(Properties.Resources.position_ne_off_icon, 0.4F);
        private readonly Image positionNwOffBlur = Stats.ImageOpacity(Properties.Resources.position_nw_off_icon, 0.4F);
        private readonly Image positionSeOffBlur = Stats.ImageOpacity(Properties.Resources.position_se_off_icon, 0.4F);
        private readonly Image positionSwOffBlur = Stats.ImageOpacity(Properties.Resources.position_sw_off_icon, 0.4F);
        private readonly Image positionNeOffFocus = Stats.ImageOpacity(Properties.Resources.position_ne_off_icon, 0.8F);
        private readonly Image positionNwOffFocus = Stats.ImageOpacity(Properties.Resources.position_nw_off_icon, 0.8F);
        private readonly Image positionSeOffFocus = Stats.ImageOpacity(Properties.Resources.position_se_off_icon, 0.8F);
        private readonly Image positionSwOffFocus = Stats.ImageOpacity(Properties.Resources.position_sw_off_icon, 0.8F);
        private readonly Image positionNeOnBlur = Stats.ImageOpacity(Properties.Resources.position_ne_on_icon, 0.4F);
        private readonly Image positionNwOnBlur = Stats.ImageOpacity(Properties.Resources.position_nw_on_icon, 0.4F);
        private readonly Image positionSeOnBlur = Stats.ImageOpacity(Properties.Resources.position_se_on_icon, 0.4F);
        private readonly Image positionSwOnBlur = Stats.ImageOpacity(Properties.Resources.position_sw_on_icon, 0.4F);
        private readonly Image positionNeOnFocus = Stats.ImageOpacity(Properties.Resources.position_ne_on_icon, 0.8F);
        private readonly Image positionNwOnFocus = Stats.ImageOpacity(Properties.Resources.position_nw_on_icon, 0.8F);
        private readonly Image positionSeOnFocus = Stats.ImageOpacity(Properties.Resources.position_se_on_icon, 0.8F);
        private readonly Image positionSwOnFocus = Stats.ImageOpacity(Properties.Resources.position_sw_on_icon, 0.8F);
        private readonly Image positionLockBlur = Stats.ImageOpacity(Properties.Resources.switch_lock_icon, 0.4F);
        private readonly Image positionLockFocus = Stats.ImageOpacity(Properties.Resources.switch_lock_icon, 0.8F);
        private readonly Image positionUnlockBlur = Stats.ImageOpacity(Properties.Resources.switch_unlock_icon, 0.4F);
        private readonly Image positionUnlockFocus = Stats.ImageOpacity(Properties.Resources.switch_unlock_icon, 0.8F);
        public bool isFixedPositionNe, isFixedPositionNw, isFixedPositionSe, isFixedPositionSw, isPositionLock;
        private bool isFocused, isMouseEnter;
        
        private static PrivateFontCollection DefaultFontCollection;
        public static new Font DefaultFont;
        
        static Overlay() {
            if (!File.Exists("TitanOne-Regular.ttf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.TitanOne-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("TitanOne-Regular.ttf", fontdata);
                }
            }
            
            if (!File.Exists("NotoSans-Regular.ttf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.NotoSans-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("NotoSans-Regular.ttf", fontdata);
                }
            }
            
            if (!File.Exists("NotoSansSC-Regular.otf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.NotoSansSC-Regular.otf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes("NotoSansSC-Regular.otf", fontdata);
                }
            }

            DefaultFontCollection = new PrivateFontCollection();
            DefaultFontCollection.AddFontFile("TitanOne-Regular.ttf");
            DefaultFontCollection.AddFontFile("NotoSans-Regular.ttf");
            DefaultFontCollection.AddFontFile("NotoSansSC-Regular.otf");
            SetDefaultFont(Stats.CurrentLanguage, 18);
            
            if (!Directory.Exists("Overlay")) {
                Directory.CreateDirectory("Overlay");
                using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.background.png")) {
                    byte[] overlaydata = new byte[overlayStream.Length];
                    overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                    File.WriteAllBytes("Overlay/background.png", overlaydata);
                }
                using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.tab_unselected.png")) {
                    byte[] overlaydata = new byte[overlayStream.Length];
                    overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                    File.WriteAllBytes("Overlay/tab.png", overlaydata);
                }
            } else {
                if (!File.Exists("Overlay/background.png")) {
                    using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.background.png")) {
                        byte[] overlaydata = new byte[overlayStream.Length];
                        overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                        File.WriteAllBytes("Overlay/background.png", overlaydata);
                    }
                }
                if (!File.Exists("Overlay/tab.png")) {
                    using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.tab_unselected.png")) {
                        byte[] overlaydata = new byte[overlayStream.Length];
                        overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                        File.WriteAllBytes("Overlay/tab.png", overlaydata);
                    }
                }
            }
        }
        
        public Overlay() {
            this.InitializeComponent();
            this.ChangeLanguage();

            this.picPositionNE.Image = this.positionNeOffBlur;
            this.picPositionNW.Image = this.positionNwOffBlur;
            this.picPositionSE.Image = this.positionSeOffBlur;
            this.picPositionSW.Image = this.positionSwOffBlur;
            this.picPositionLock.Image = this.positionUnlockBlur;
            
            this.picPositionNE.Location = new Point((this.Width / 2) - (this.picPositionNE.Size.Width + 2), (this.Height / 2) - (this.picPositionNE.Size.Height + 2));
            this.picPositionNW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) - (this.picPositionNW.Size.Height + 2));
            this.picPositionSE.Location = new Point((this.Width / 2) - (this.picPositionSE.Size.Width + 2), (this.Height / 2) + 2);
            this.picPositionSW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) + 2);
            this.picPositionLock.Location = new Point(this.Location.X - 2, this.Location.Y - 8);
            
            this.SetBackground();

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
        public void SetFixedPosition(bool positonNE, bool positonNW, bool positonSE, bool positonSW, bool positonFree) {
            this.isFixedPositionNe = positonNE;
            this.isFixedPositionNw = positonNW;
            this.isFixedPositionSe = positonSE;
            this.isFixedPositionSw = positonSW;
            this.isPositionLock = positonFree;
        }
        public void SetBackgroundResourcesName(string backgound, string tab) {
            this.BackgroundResourceName = backgound;
            this.TabResourceName = tab;
        }
        public void SetBackground(string BackgroundResourceName = null) {
            Bitmap background = string.IsNullOrEmpty(BackgroundResourceName)
                ? Properties.Resources.background
                : (Bitmap)Properties.Resources.ResourceManager.GetObject(BackgroundResourceName) ?? Properties.Resources.background;
            
            Bitmap newImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.DrawImage(background, 0, 0);
            }
            this.Background = newImage;
            this.DrawImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
            this.DrawGraphics = Graphics.FromImage(this.DrawImage);
            this.drawWidth = background.Width;
            this.drawHeight = background.Height;
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
            this.timer = new Thread(this.UpdateTimer) { IsBackground = true };
            this.timer.Start();
        }
        public static void SetFonts(Control control, float customSize = -1, Font font = null) {
            if (font == null) {
                font = customSize <= 0 ? DefaultFont : new Font(GetDefaultFontFamilies(), customSize, FontStyle.Regular, GraphicsUnit.Pixel);
            }

            control.Font = font;
            foreach (Control ctr in control.Controls) {
                ctr.Font = font;
                if (ctr.HasChildren) {
                    SetFonts(ctr, customSize, font);
                }
            }
        }

        public void SetDefaultFontColor() {
            this.ForeColor = Color.White;
        }
        public void SetFontColor(Color color) {
            this.ForeColor = color;
        }
        public static void SetDefaultFont(int language, float emSize) {
            DefaultFont = new Font(GetDefaultFontFamilies(language), emSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }
        public static Font GetDefaultFont(int language, float emSize) {
            return new Font(GetDefaultFontFamilies(language), emSize, language == 4 ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        }
        public static FontFamily GetDefaultFontFamilies(int language = 0) {
            return language <= 1 ? DefaultFontCollection.Families[2] :
                language == 4 ? DefaultFontCollection.Families[1] : DefaultFontCollection.Families[0];
        }
        public static Font GetMainFont(float emSize, FontStyle fontStyle = FontStyle.Regular, int language = 0) {
            return new Font(GetMainFontFamilies(language), emSize, fontStyle, GraphicsUnit.Pixel);
        }
        public static FontFamily GetMainFontFamilies(int language) {
            return language == 4 ? DefaultFontCollection.Families[1] : DefaultFontCollection.Families[0];
        }
        public bool IsFocused() {
            return this.isFocused;
        }
        public bool IsMouseEnter() {
            return this.isMouseEnter;
        }
        public bool IsFixed() {
            return this.isFixedPositionNe || this.isFixedPositionNw || this.isFixedPositionSe || this.isFixedPositionSw || this.isPositionLock;
        }
        private void Position_MouseClick(object sender, MouseEventArgs e) {
            switch (e.Button) {
                case MouseButtons.Left:
                    string iconName = ((PictureBox)sender).Name;
                    Screen screen = this.StatsForm.GetCurrentScreen(this.Location);
                    Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                    Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                    if (iconName.Equals("picPositionNE")) {
                        if (this.isFixedPositionNe) {
                            if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && this.StatsForm.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width)) {
                                this.Location = new Point(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value);
                            }
                            this.FlipDisplay(this.StatsForm.CurrentSettings.FlippedDisplay);
                            this.picPositionNE.Image = this.positionNeOffFocus;
                            this.isFixedPositionNe = false;
                            this.Cursor = Cursors.SizeAll;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Show();
                        } else {
                            if (!this.IsFixed()) {
                                this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                                this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                            }
                            this.FlipDisplay(true);
                            this.Background = this.RecreateBackground();
                            this.Location = new Point(screenLocation.X, 0);
                            this.picPositionNE.Image = this.positionNeOnFocus;
                            this.picPositionNW.Image = this.positionNwOffFocus;
                            this.picPositionSE.Image = this.positionSeOffFocus;
                            this.picPositionSW.Image = this.positionSwOffFocus;
                            this.isFixedPositionNe = true;
                            this.isFixedPositionNw = false;
                            this.isFixedPositionSe = false;
                            this.isFixedPositionSw = false;
                            this.Cursor = Cursors.Default;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                            this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                            this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                            //this.StatsForm.CurrentSettings.FlippedDisplay = true;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = "ne";
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                            this.picPositionLock.Hide();
                        }
                    } else if (iconName.Equals("picPositionNW")) {
                        if (this.isFixedPositionNw) {
                            if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && this.StatsForm.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width)) {
                                this.Location = new Point(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value);
                            }
                            this.FlipDisplay(this.StatsForm.CurrentSettings.FlippedDisplay);
                            this.picPositionNW.Image = this.positionNwOffFocus;
                            this.isFixedPositionNw = false;
                            this.Cursor = Cursors.SizeAll;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Show();
                        } else {
                            if (!this.IsFixed()) {
                                this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                                this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                            }
                            this.FlipDisplay(false);
                            this.Background = this.RecreateBackground();
                            this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, 0);
                            this.picPositionNE.Image = this.positionNeOffFocus;
                            this.picPositionNW.Image = this.positionNwOnFocus;
                            this.picPositionSE.Image = this.positionSeOffFocus;
                            this.picPositionSW.Image = this.positionSwOffFocus;
                            this.isFixedPositionNe = false;
                            this.isFixedPositionNw = true;
                            this.isFixedPositionSe = false;
                            this.isFixedPositionSw = false;
                            this.Cursor = Cursors.Default;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                            this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                            this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                            //this.StatsForm.CurrentSettings.FlippedDisplay = false;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = "nw";
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Hide();
                        }
                    } else if (iconName.Equals("picPositionSE")) {
                        if (this.isFixedPositionSe) {
                            if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && this.StatsForm.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width)) {
                                this.Location = new Point(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value);
                            }
                            this.FlipDisplay(this.StatsForm.CurrentSettings.FlippedDisplay);
                            this.picPositionSE.Image = this.positionSeOffFocus;
                            this.isFixedPositionSe = false;
                            this.Cursor = Cursors.SizeAll;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Show();
                        } else {
                            if (!this.IsFixed()) {
                                this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                                this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                            }
                            this.FlipDisplay(true);
                            this.Background = this.RecreateBackground();
                            this.Location = new Point(screenLocation.X, screenLocation.Y + screenSize.Height - this.Height);
                            this.picPositionNE.Image = this.positionNeOffFocus;
                            this.picPositionNW.Image = this.positionNwOffFocus;
                            this.picPositionSE.Image = this.positionSeOnFocus;
                            this.picPositionSW.Image = this.positionSwOffFocus;
                            this.isFixedPositionNe = false;
                            this.isFixedPositionNw = false;
                            this.isFixedPositionSe = true;
                            this.isFixedPositionSw = false;
                            this.Cursor = Cursors.Default;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                            this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                            this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                            //this.StatsForm.CurrentSettings.FlippedDisplay = true;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = "se";
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                            this.picPositionLock.Hide();
                        }
                    } else if (iconName.Equals("picPositionSW")) {
                        if (this.isFixedPositionSw) {
                            if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && this.StatsForm.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width)) {
                                this.Location = new Point(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value);
                            }
                            this.FlipDisplay(this.StatsForm.CurrentSettings.FlippedDisplay);
                            this.picPositionSW.Image = this.positionSwOffFocus;
                            this.isFixedPositionSw = false;
                            this.Cursor = Cursors.SizeAll;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Show();
                        } else {
                            if (!this.IsFixed()) {
                                this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                                this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                            }
                            this.FlipDisplay(false);
                            this.Background = this.RecreateBackground();
                            this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, screenLocation.Y + screenSize.Height - this.Height);
                            this.picPositionNE.Image = this.positionNeOffFocus;
                            this.picPositionNW.Image = this.positionNwOffFocus;
                            this.picPositionSE.Image = this.positionSeOffFocus;
                            this.picPositionSW.Image = this.positionSwOnFocus;
                            this.isFixedPositionNe = false;
                            this.isFixedPositionNw = false;
                            this.isFixedPositionSe = false;
                            this.isFixedPositionSw = true;
                            this.Cursor = Cursors.Default;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                            this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                            this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                            //this.StatsForm.CurrentSettings.FlippedDisplay = false;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = "sw";
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionLock.Hide();
                        }
                    } else if (iconName.Equals("picPositionLock")) {
                        if (this.isPositionLock) {
                            this.picPositionLock.Image = this.positionUnlockFocus;
                            this.isPositionLock = false;
                            this.Cursor = Cursors.SizeAll;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                            this.picPositionNE.Show();
                            this.picPositionNW.Show();
                            this.picPositionSE.Show();
                            this.picPositionSW.Show();
                        } else {
                            this.picPositionLock.Image = this.positionLockFocus;
                            this.isPositionLock = true;
                            this.Cursor = Cursors.Default;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                            this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                            this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                            this.StatsForm.CurrentSettings.OverlayFixedPosition = "free";
                            this.StatsForm.CurrentSettings.FixedFlippedDisplay = this.flippedImage;
                            this.picPositionNE.Hide();
                            this.picPositionNW.Hide();
                            this.picPositionSE.Hide();
                            this.picPositionSW.Hide();
                        }
                    }
                    this.StatsForm.SaveUserSettings();
                    break;
            }
        }
        private void Position_MouseEnter(object sender, EventArgs e) {
            if (this.isPositionButtonMouseEnter) return;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.picPositionLock.Image = this.isPositionLock ? this.positionLockFocus : this.positionUnlockFocus;
                } else {
                    this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnFocus : this.positionNeOffFocus;
                    this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnFocus : this.positionNwOffFocus;
                    this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnFocus : this.positionSeOffFocus;
                    this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnFocus : this.positionSwOffFocus;
                }
            } else {
                this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnFocus : this.positionNeOffFocus;
                this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnFocus : this.positionNwOffFocus;
                this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnFocus : this.positionSeOffFocus;
                this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnFocus : this.positionSwOffFocus;
                this.picPositionLock.Image = this.isPositionLock ? this.positionLockFocus : this.positionUnlockFocus;
            }
            this.isPositionButtonMouseEnter = true;
        }
        private void Position_MouseLeave(object sender, EventArgs e) {
            if (!this.isPositionButtonMouseEnter) return;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
                } else {
                    this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnBlur : this.positionNeOffBlur;
                    this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnBlur : this.positionNwOffBlur;
                    this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnBlur : this.positionSeOffBlur;
                    this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnBlur : this.positionSwOffBlur;
                }
            } else {
                this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnBlur : this.positionNeOffBlur;
                this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnBlur : this.positionNwOffBlur;
                this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnBlur : this.positionSeOffBlur;
                this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnBlur : this.positionSwOffBlur;
                this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
            }
            this.isPositionButtonMouseEnter = false;
        }
        private void Overlay_GotFocus(object sender, EventArgs e) {
            this.isFocused = true;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.picPositionLock.Show();
                    this.picPositionNE.Hide();
                    this.picPositionNW.Hide();
                    this.picPositionSE.Hide();
                    this.picPositionSW.Hide();
                } else {
                    this.picPositionLock.Hide();
                    this.picPositionNE.Show();
                    this.picPositionNW.Show();
                    this.picPositionSE.Show();
                    this.picPositionSW.Show();
                    
                }
            } else {
                if (this.picPositionNE.Visible && this.picPositionNW.Visible &&
                    this.picPositionSE.Visible && this.picPositionSW.Visible && this.picPositionLock.Visible) return;
                this.picPositionNE.Show();
                this.picPositionNW.Show();
                this.picPositionSE.Show();
                this.picPositionSW.Show();
                this.picPositionLock.Show();
            }
        }
        private void Overlay_LostFocus(object sender, EventArgs e) {
            this.isFocused = false;
            if (!this.picPositionNE.Visible && !this.picPositionNW.Visible &&
                    !this.picPositionSE.Visible && !this.picPositionSW.Visible && !this.picPositionLock.Visible) return;
            this.picPositionNE.Hide();
            this.picPositionNW.Hide();
            this.picPositionSE.Hide();
            this.picPositionSW.Hide();
            this.picPositionLock.Hide();
        }
        private void Overlay_MouseEnter(object sender, EventArgs e) {
            this.isMouseEnter = true;
        }
        private void Overlay_MouseLeave(object sender, EventArgs e) {
            this.isMouseEnter = false;
        }
        private void Overlay_Resize(object sender, EventArgs e) {
            this.picPositionNE.Location = new Point((this.Width / 2) - (this.picPositionNE.Size.Width + 2), (this.Height / 2) - (this.picPositionNE.Size.Height + 2) + (this.drawHeight > 99 ? 11 : -6));
            this.picPositionNW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) - (this.picPositionNE.Size.Height + 2) + (this.drawHeight > 99 ? 11 : -6));
            this.picPositionSE.Location = new Point((this.Width / 2) - (this.picPositionSE.Size.Width + 2), (this.Height / 2) + 2 + (this.drawHeight > 99 ? 11 : -6));
            this.picPositionSW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) + 2 + (this.drawHeight > 99 ? 11 : -6));
            this.picPositionLock.Location = new Point(this.StatsForm.CurrentSettings.FlippedDisplay ? (this.Width - this.picPositionLock.Width - 14) : 14, (this.Height / 2) - (this.picPositionLock.Size.Height + 6) + (this.drawHeight > 99 ? 11 : -6));
        }
        private void Overlay_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;

            if ((sender.GetType() == this.picPositionNE.GetType()) ||
                (sender.GetType() == this.picPositionNW.GetType()) ||
                (sender.GetType() == this.picPositionSE.GetType()) ||
                (sender.GetType() == this.picPositionSW.GetType()) ||
                (sender.GetType() == this.picPositionLock.GetType())) return;

            if (this.isFixedPositionNe || this.isFixedPositionNw || this.isFixedPositionSe || this.isFixedPositionSw || this.isPositionLock) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        private void UpdateTimer() {
            while (this.StatsForm != null && !this.StatsForm.IsDisposed && !this.StatsForm.Disposing) {
                try {
                    if (this.IsHandleCreated && !this.StatsForm.Disposing && !this.StatsForm.IsDisposed) {
                        this.frameCount++;
                        this.isTimeToSwitch = this.frameCount % (this.StatsForm.CurrentSettings.CycleTimeSeconds * 20) == 0;
                        Invoke((Action)this.UpdateInfo);
                    }

                    this.StatsForm.UpdateDates();
                    Thread.Sleep(50);
                } catch {
                    // ignored
                }
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
                    qualifyDisplay = $"{levelInfo.TotalQualify}{(levelInfo.TotalPlays < 1000 ? " / " + levelInfo.TotalPlays : Multilingual.GetWord("overlay_inning"))}";
                    this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                    break;
                case 1:
                    this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_gold")} :";
                    qualifyChance = levelInfo.TotalGolds * 100f / (levelInfo.TotalPlays == 0 ? 1 : levelInfo.TotalPlays);
                    qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {qualifyChance:0.0}%";
                    qualifyDisplay = $"{levelInfo.TotalGolds}{(levelInfo.TotalPlays < 1000 ? " / " + levelInfo.TotalPlays : Multilingual.GetWord("overlay_inning"))}";
                    this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                    break;
            }
        }
        private void SetFastestLabel(StatSummary levelInfo, LevelType type) {
            int fastestSwitchCount = this.switchCount;
            if (!this.StatsForm.CurrentSettings.SwitchBetweenLongest) {
                fastestSwitchCount = this.StatsForm.CurrentSettings.OnlyShowLongest ? 0
                    : this.levelException == 1 ? 1 : this.levelException == 2 ? 2 : type.FastestLabel();
            }
            switch (fastestSwitchCount % ((levelInfo.BestScore.HasValue && this.levelException != 1) ? 3 : 2)) {
                case 0:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_longest")} :";
                    this.lblFastest.TextRight = levelInfo.LongestFinish.HasValue ? $"{levelInfo.LongestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 1:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_fastest")} :";
                    this.lblFastest.TextRight = levelInfo.BestFinish.HasValue ? $"{levelInfo.BestFinish:m\\:ss\\.ff}" : "-";
                    break;
                case 2:
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_best_score")} :";
                    this.lblFastest.TextRight = $"{levelInfo.BestScore?.ToString()}";
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
                    this.lblPlayers.TextRight = $"{this.lastRound?.Players.ToString()}";
                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayers.Image = Properties.Resources.player_icon;
                        this.lblPlayers.Text = @"ㅤ    :";
                        int psCount = this.lastRound.PlayersPs4 + this.lastRound.PlayersPs5;
                        int xbCount = this.lastRound.PlayersXb1 + this.lastRound.PlayersXsx;
                        int swCount = this.lastRound.PlayersSw;
                        int pcCount = this.lastRound.PlayersPc;
                        this.lblPlayersPs.TextRight = (psCount == 0 ? "-" : $"{psCount}");
                        this.lblPlayersPs.Size = new Size((psCount > 9 ? 32 : 26), 16);
                        this.lblPlayersPs.DrawVisible = true;
                        this.lblPlayersXbox.TextRight = (xbCount == 0 ? "-" : $"{xbCount}");
                        this.lblPlayersXbox.Size = new Size((xbCount > 9 ? 32 : 26), 16);
                        this.lblPlayersXbox.DrawVisible = true;
                        this.lblPlayersSwitch.TextRight = (swCount == 0 ? "-" : $"{swCount}");
                        this.lblPlayersSwitch.Size = new Size((swCount > 9 ? 32 : 26), 16);
                        this.lblPlayersSwitch.DrawVisible = true;
                        this.lblPlayersPc.TextRight = (pcCount == 0 ? "-" : $"{pcCount}");
                        this.lblPlayersPc.Size = new Size((pcCount > 9 ? 32 : 26), 16);
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
                    this.lblPlayers.TextRight = Stats.InShow && Stats.LastServerPing != 0 ? $"{Stats.LastServerPing} ms" : "-";
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
                    this.lblStreak.TextRight = $"{levelInfo.CurrentStreak}{Multilingual.GetWord("overlay_streak_suffix")} ({Multilingual.GetWord("overlay_best")}{levelInfo.BestStreak}{Multilingual.GetWord("overlay_streak_suffix")})";
                    break;
                case 1:
                    this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak_finals")} :";
                    this.lblStreak.TextRight = $"{levelInfo.CurrentFinalStreak}{Multilingual.GetWord("overlay_inning")} ({Multilingual.GetWord("overlay_best")}{levelInfo.BestFinalStreak}{Multilingual.GetWord("overlay_inning")})";
                    break;
            }
        }
        private void UpdateInfo() {
            if (this.StatsForm == null) { return; }

            lock (this.StatsForm.CurrentRound) {
                bool hasCurrentRound = this.StatsForm.CurrentRound != null && this.StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound && (this.lastRound == null || Stats.InShow || Stats.EndedShow)) {
                    if (Stats.EndedShow) {
                        Stats.EndedShow = false;
                    }
                    this.lastRound = this.StatsForm.CurrentRound[this.StatsForm.CurrentRound.Count - 1];
                }

                this.lblFilter.Text = this.StatsForm.GetCurrentFilter();
                this.lblProfile.Text = this.StatsForm.GetCurrentProfile();
                this.Background = this.RecreateBackground();
                this.lblProfile.Location = new Point(this.flippedImage ? 125 : this.drawWidth - (145 + this.GetOverlayProfileOffset(this.lblProfile.Text)), 9);

                if (this.lastRound != null && !string.IsNullOrEmpty(this.lastRound.Name)) {
                    string roundName = this.lastRound.VerifiedName();
                    this.levelException = 0;
                    if (roundName == "round_hoverboardsurvival_s4_show" || roundName == "round_hoverboardsurvival2_almond" || roundName == "round_snowy_scrap" || roundName == "round_jinxed" || roundName == "round_rocknroll" || roundName == "round_conveyor_arena") {
                        this.levelException = 1; // Level is like a "Race" level type (fastest time info is most important - also hide high-score info)
                    } else if (roundName == "round_1v1_button_basher" || roundName == "round_1v1_volleyfall_symphony_launch_show") {
                        this.levelException = 2; // Level is like a "Team" level type (score info is most important)
                    }
                    if (this.StatsForm.StatLookup.TryGetValue(roundName, out LevelStats level)) {
                        roundName = this.lastRound.UseShareCode ? this.lastRound.ShowNameId : level.Name.ToUpper();
                    } else if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                        roundName = roundName.Substring(6).Replace('_', ' ').ToUpper();
                    }
                    
                    StatSummary levelInfo = this.StatsForm.GetLevelInfo(roundName, this.levelException);

                    //if (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) { // English, French
                    //    if (roundName.Length > 29) { roundName = roundName.Substring(0, 29); }
                    //} else {
                    //    if (roundName.Length > 15) { roundName = roundName.Substring(0, 15); }
                    //}
                    if (roundName.Length > 29) { roundName = roundName.Substring(0, 29); }

                    LevelType levelType = (level?.Type).GetValueOrDefault();
                    this.lblRound.IsCreativeRound = level.isCreative;
                    if (this.StatsForm.CurrentSettings.ColorByRoundType) {
                        this.lblRound.Text = $"{Multilingual.GetWord("overlay_round_abbreviation_prefix")}{this.lastRound.Round}{Multilingual.GetWord("overlay_round_abbreviation_suffix")} :";
                        this.lblRound.LevelColor = levelType.LevelBackColor(this.lastRound.IsFinal, this.lastRound.IsTeam, 223);
                        this.lblRound.RoundIcon = level?.RoundIcon;
                        if (this.lblRound.RoundIcon.Height != 23) {
                            this.lblRound.ImageHeight = 23;
                            this.lblRound.ImageWidth = (int)Math.Ceiling(Convert.ToDouble(this.lblRound.ImageHeight) / this.lblRound.RoundIcon.Height * this.lblRound.RoundIcon.Width);
                        } else {
                            this.lblRound.ImageHeight = this.lblRound.RoundIcon.Height;
                            this.lblRound.ImageWidth = this.lblRound.RoundIcon.Width;
                        }
                    } else {
                        this.lblRound.Text = $"{Multilingual.GetWord("overlay_round_prefix")}{this.lastRound.Round}{Multilingual.GetWord("overlay_round_suffix")} :";
                        this.lblRound.LevelColor = Color.Empty;
                        this.lblRound.RoundIcon = null;
                        this.lblRound.ImageWidth = 0;
                        this.lblRound.ImageHeight = 0;
                    }
                    this.lblRound.TextRight = roundName;

                    this.lblWins.Text = $"{Multilingual.GetWord("overlay_wins")} :";
                    float winChance = levelInfo.TotalWins * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    string winChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $"{Multilingual.GetWord("overlay_win")} - {winChance:0.0}%";
                    if (this.StatsForm.CurrentSettings.PreviousWins > 0) {
                        this.lblWins.TextRight = $"{levelInfo.TotalWins} ({levelInfo.AllWins + this.StatsForm.CurrentSettings.PreviousWins}){winChanceDisplay}";
                    } else {
                        this.lblWins.TextRight = this.StatsForm.CurrentSettings.FilterType != 0
                            ? $"　{levelInfo.TotalWins} ({levelInfo.AllWins}){winChanceDisplay}"
                            : $"　{levelInfo.TotalWins}{winChanceDisplay}";
                    }

                    this.lblFinals.Text = $"{Multilingual.GetWord("overlay_finals")} :";
                    string finalText = $"{levelInfo.TotalFinals}{(levelInfo.TotalShows < 1000 ? " / " + levelInfo.TotalShows : Multilingual.GetWord("overlay_inning"))}";
                    float finalChance = levelInfo.TotalFinals * 100f / (levelInfo.TotalShows == 0 ? 1 : levelInfo.TotalShows);
                    string finalChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : finalText.Length > 9 ? $" - {finalChance:0}%" : $" - {finalChance:0.0}%";
                    this.lblFinals.TextRight = $"{finalText}{finalChanceDisplay}";

                    this.SetQualifyChanceLabel(levelInfo);
                    this.SetFastestLabel(levelInfo, levelType);
                    this.SetPlayersLabel();
                    this.SetStreakInfo(levelInfo);
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
                        if (this.lastRound.Crown) {
                            this.lblFinish.TextRight = $"# {Multilingual.GetWord("overlay_position_win")}! - {Time:m\\:ss\\.ff}";
                        } else {
                            this.lblFinish.TextRight = this.lastRound.Position > 0 ? $"# {Multilingual.GetWord("overlay_position_prefix")}{this.lastRound.Position}{Multilingual.GetWord("overlay_position_suffix")} - {Time:m\\:ss\\.ff}" : $"{Time:m\\:ss\\.ff}";
                        }

                        if (levelType == LevelType.Race || levelType == LevelType.Hunt || levelType == LevelType.Invisibeans || this.levelException == 1) {
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
                    } else if (this.lastRound.Playing && Stats.IsPlaying) {
                        this.lblFinish.TextRight = Start > DateTime.UtcNow ? $"{DateTime.UtcNow - startTime:m\\:ss}" : $"{DateTime.UtcNow - Start:m\\:ss}";
                    } else {
                        this.lblFinish.TextRight = "-";
                        this.lblFinish.ForeColor = this.ForeColor;
                    }

                    this.lblDuration.Text = this.lastRound.GameDuration > 0
                        ? $"{Multilingual.GetWord("overlay_duration")} ({TimeSpan.FromSeconds(this.lastRound.GameDuration):m\\:ss}):"
                        : $"{Multilingual.GetWord("overlay_duration")} :";

                    if (End != DateTime.MinValue) {
                        this.lblDuration.TextRight = $"{End - Start:m\\:ss\\.ff}";
                    } else if (this.lastRound.Playing && Stats.IsPlaying) {
                        this.lblDuration.TextRight = Start > DateTime.UtcNow ? $"{DateTime.UtcNow - startTime:m\\:ss}" : $"{DateTime.UtcNow - Start:m\\:ss}";
                    } else {
                        this.lblDuration.TextRight = "-";
                    }
                }
                this.Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            lock (DefaultFont) {
                this.DrawGraphics.Clear(Color.Transparent);
                this.DrawGraphics.DrawImage(this.Background, 0, 0);

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
                e.Graphics.DrawImage(this.DrawImage, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height), new Rectangle(0, 0, this.DrawImage.Width, this.DrawImage.Height), GraphicsUnit.Pixel);
            }
        }
        private void Overlay_MouseWheel(object sender, MouseEventArgs e) {
            if (this.shiftKeyToggle == false) { return; }
            if (this.StatsForm.ProfileMenuItems.Count <= 1) { return; }
            if ((e.Delta / 120) > 0) {
                for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                    ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                    if (!(item is ToolStripMenuItem menuItem)) { continue; }
                    if (menuItem.Checked && i > 0) {
                        this.StatsForm.ProfileMenuItems[i - 1].PerformClick();
                        break;
                    } else if (menuItem.Checked && i == 0) {
                        this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count - 1].PerformClick();
                        break;
                    }
                }
            } else {
                for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
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
            }
        }
        private void Overlay_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    this.shiftKeyToggle = false;
                    break;
                case Keys.ControlKey:
                    this.ctrlKeyToggle = false;
                    break;
            }
        }
        private void Overlay_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    this.shiftKeyToggle = true;
                    break;
                case Keys.ControlKey:
                    this.ctrlKeyToggle = true;
                    break;
                case Keys.D:
                    if (this.shiftKeyToggle && this.ctrlKeyToggle) this.Size = this.DefaultSize;
                    break;
                case Keys.T: {
                        int colorOption = 0;
                        if (BackColor.ToArgb() == Color.FromArgb(224, 224, 224).ToArgb()) {
                            colorOption = 1;
                        } else if (BackColor.ToArgb() == Color.Black.ToArgb()) {
                            colorOption = 2;
                        } else if (BackColor.ToArgb() == Color.Magenta.ToArgb()) {
                            colorOption = 3;
                        } else if (BackColor.ToArgb() == Color.Red.ToArgb()) {
                            colorOption = 4;
                        } else if (BackColor.ToArgb() == Color.Green.ToArgb()) {
                            colorOption = 5;
                        } else if (BackColor.ToArgb() == Color.Blue.ToArgb()) {
                            colorOption = 0;
                        }
                        SetBackgroundColor(colorOption);
                        this.StatsForm.CurrentSettings.OverlayColor = colorOption;
                        this.StatsForm.SaveUserSettings();
                        break;
                    }
                case Keys.F:
                    this.FlipDisplay(!this.flippedImage);
                    this.Background = this.RecreateBackground();
                    this.StatsForm.CurrentSettings.FlippedDisplay = this.flippedImage;
                    this.StatsForm.SaveUserSettings();
                    break;
                case Keys.C:
                    this.StatsForm.CurrentSettings.PlayerByConsoleType = !this.StatsForm.CurrentSettings.PlayerByConsoleType;
                    this.StatsForm.SaveUserSettings();
                    this.ArrangeDisplay(this.StatsForm.CurrentSettings.FlippedDisplay, this.StatsForm.CurrentSettings.ShowOverlayTabs,
                        this.StatsForm.CurrentSettings.HideWinsInfo, this.StatsForm.CurrentSettings.HideRoundInfo, this.StatsForm.CurrentSettings.HideTimeInfo,
                        this.StatsForm.CurrentSettings.OverlayColor, this.StatsForm.CurrentSettings.OverlayWidth, this.StatsForm.CurrentSettings.OverlayHeight,
                        this.StatsForm.CurrentSettings.OverlayFontSerialized, this.StatsForm.CurrentSettings.OverlayFontColorSerialized);
                    return;
                case Keys.R:
                    this.StatsForm.CurrentSettings.ColorByRoundType = !this.StatsForm.CurrentSettings.ColorByRoundType;
                    this.StatsForm.SaveUserSettings();
                    this.ArrangeDisplay(this.StatsForm.CurrentSettings.FlippedDisplay, this.StatsForm.CurrentSettings.ShowOverlayTabs,
                        this.StatsForm.CurrentSettings.HideWinsInfo, this.StatsForm.CurrentSettings.HideRoundInfo, this.StatsForm.CurrentSettings.HideTimeInfo,
                        this.StatsForm.CurrentSettings.OverlayColor, this.StatsForm.CurrentSettings.OverlayWidth, this.StatsForm.CurrentSettings.OverlayHeight,
                        this.StatsForm.CurrentSettings.OverlayFontSerialized, this.StatsForm.CurrentSettings.OverlayFontColorSerialized);
                    return;
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
                case Keys.Left:
                    if (this.shiftKeyToggle == false) { return; }
                    for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                        ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                        if (!(item is ToolStripMenuItem menuItem)) { continue; }
                        if (menuItem.Checked && i > 0) {
                            this.StatsForm.ProfileMenuItems[i - 1].PerformClick();
                            break;
                        } else if (menuItem.Checked && i == 0) {
                            this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count - 1].PerformClick();
                            break;
                        }
                    }
                    break;
                case Keys.Down:
                case Keys.Right:
                    if (this.shiftKeyToggle == false) { return; }
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
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9: {
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
                case 0: BackColor = Color.FromArgb(224, 224, 224); break;
                case 1: BackColor = Color.Black; break;
                case 2: BackColor = Color.Magenta; break;
                case 3: BackColor = Color.Red; break;
                case 4: BackColor = Color.Green; break;
                case 5: BackColor = Color.Blue; break;
            }
        }
        public void ArrangeDisplay(bool flipDisplay, bool showTabs, bool hideWins, bool hideRound, bool hideTime, int colorOption, int? width, int? height, string serializedFont, string serializedFontColor) {
            this.FlipDisplay(false);
            
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

            this.drawWidth = firstColumnWidth + secondColumnWidth + thirdColumnWidth + (spacerWidth * 3) + firstColumnX - 2;

            int overlaySetting = (hideWins ? 4 : 0) + (hideRound ? 2 : 0) + (hideTime ? 1 : 0);
            switch (overlaySetting) {
                case 0:
                    //this.MinimumSize = new Size(786, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(786, showTabs ? 134 : 99);

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblRound.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblRound.DrawVisible = true;
                    this.lblQualifyChance.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    this.lblFastest.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblFastest.Size = new Size(secondColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;

                    this.lblPlayers.Location = new Point(thirdColumnX, 10 + heightOffset);
                    this.lblPlayers.Size = new Size(thirdColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(thirdColumnX + 52, 13 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 13;
                        this.lblPlayersPs.ImageHeight = 13;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(thirdColumnX + 90, 13 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 13;
                        this.lblPlayersXbox.ImageHeight = 13;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(thirdColumnX + 128, 13 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 13;
                        this.lblPlayersSwitch.ImageHeight = 13;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(thirdColumnX + 166, 13 + heightOffset);
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
                    //this.MinimumSize = new Size(538, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(555, showTabs ? 134 : 99);

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

                    this.lblRound.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblRound.DrawVisible = true;
                    
                    this.lblPlayers.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblPlayers.Size = new Size(secondColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(secondColumnX + 49, 35 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 13;
                        this.lblPlayersPs.ImageHeight = 13;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(secondColumnX + 86, 35 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 13;
                        this.lblPlayersXbox.ImageHeight = 13;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(secondColumnX + 123, 35 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 13;
                        this.lblPlayersSwitch.ImageHeight = 13;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(secondColumnX + 160, 35 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageWidth = 13;
                        this.lblPlayersPc.ImageHeight = 13;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblQualifyChance.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    break;
                case 2:
                    //this.MinimumSize = new Size(538, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(499, showTabs ? 134 : 99);

                    this.drawWidth -= secondColumnWidth + spacerWidth;

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblRound.DrawVisible = false;
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
                    //this.MinimumSize = new Size(251, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(268, showTabs ? 134 : 99);

                    this.drawWidth -= secondColumnWidth + thirdColumnWidth + (spacerWidth * 2);

                    this.lblWins.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblWins.DrawVisible = true;
                    this.lblFinals.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblFinals.DrawVisible = true;
                    this.lblStreak.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblStreak.DrawVisible = true;

                    this.lblRound.DrawVisible = false;
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
                    //this.MinimumSize = new Size(251, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(538, showTabs ? 134 : 99);

                    this.drawWidth -= firstColumnWidth + spacerWidth;

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblRound.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblRound.DrawVisible = true;
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
                    //this.MinimumSize = new Size(251, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(307, showTabs ? 134 : 99);

                    this.drawWidth -= firstColumnWidth + thirdColumnWidth + (spacerWidth * 2);

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblRound.Location = new Point(firstColumnX, 9 + heightOffset);
                    this.lblRound.DrawVisible = true;
                    
                    this.lblPlayers.Location = new Point(firstColumnX, 32 + heightOffset);
                    this.lblPlayers.Size = new Size(secondColumnWidth, 22);
                    this.lblPlayers.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + 49, 35 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 18);
                        this.lblPlayersPs.ImageWidth = 12;
                        this.lblPlayersPs.ImageHeight = 12;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + 86, 35 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 18);
                        this.lblPlayersXbox.ImageWidth = 12;
                        this.lblPlayersXbox.ImageHeight = 12;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + 123, 35 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 18);
                        this.lblPlayersSwitch.ImageWidth = 12;
                        this.lblPlayersSwitch.ImageHeight = 12;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + 160, 35 + heightOffset);
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
                    //this.MinimumSize = new Size(251, showTabs ? 134 : 99);
                    this.DefaultSize = new Size(251, showTabs ? 134 : 99);

                    this.drawWidth -= firstColumnWidth + secondColumnWidth + (spacerWidth * 2);

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblRound.DrawVisible = false;
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
            
            if (!string.IsNullOrEmpty(serializedFontColor)) {
                ColorConverter colorConverter = new ColorConverter();
                this.SetFontColor((Color)colorConverter.ConvertFromString(serializedFontColor));
            } else {
                this.SetDefaultFontColor();
            }
            
            this.Background = this.RecreateBackground();
            if (width.HasValue) {
                this.Width = width.Value;
            }
            if (height.HasValue) {
                this.Height = height.Value;
            }
            
            this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnBlur : this.positionNeOffBlur;
            this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnBlur : this.positionNwOffBlur;
            this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnBlur : this.positionSeOffBlur;
            this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnBlur : this.positionSwOffBlur;
            this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
            this.picPositionNE.Location = new Point((this.Width / 2) - (this.picPositionNE.Size.Width + 2), (this.Height / 2) - (this.picPositionNE.Size.Height + 2) + (showTabs ? 11 : -6));
            this.picPositionNW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) - (this.picPositionNW.Size.Height + 2) + (showTabs ? 11 : -6));
            this.picPositionSE.Location = new Point((this.Width / 2) - (this.picPositionSE.Size.Width + 2), (this.Height / 2) + 2 + (showTabs ? 11 : -6));
            this.picPositionSW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) + 2 + (showTabs ? 11 : -6));
            this.picPositionLock.Location = new Point(flipDisplay ? (this.Width - this.picPositionLock.Width - 14) : 14, (this.Height / 2) - (this.picPositionLock.Size.Height + 6) + (showTabs ? 11 : -6));

            if (this.IsFixed()) {
                if (this.isFixedPositionSe || this.isFixedPositionSw) {
                    if (this.isFixedPositionSe) {
                        Screen screen = this.StatsForm.GetCurrentScreen(this.Location);
                        Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                        Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                        this.Location = new Point(screenLocation.X, screenLocation.Y + screenSize.Height - this.Height);
                    } else if (this.isFixedPositionSw) {
                        Screen screen = this.StatsForm.GetCurrentScreen(this.Location);
                        Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                        Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                        this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, screenLocation.Y + screenSize.Height - this.Height);
                    }
                }
                this.DisplayTabs(showTabs);
                this.DisplayProfile(showTabs);
                this.FlipDisplay(this.StatsForm.CurrentSettings.FixedFlippedDisplay);
                this.SetBackgroundColor(colorOption);
            } else {
                this.DisplayTabs(showTabs);
                this.DisplayProfile(showTabs);
                this.FlipDisplay(flipDisplay);
                this.SetBackgroundColor(colorOption);
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
            this.picPositionLock.Location = new Point(flipped ? (this.Width - this.picPositionLock.Width - 14) : 14, (this.Height / 2) - (this.picPositionLock.Size.Height + 6) + (this.drawHeight > 99 ? 11 : -6));
        }
        private int GetCountNumeric(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (0x30 <= ch && ch <= 0x39) count++;
            }
            return count;
        }
        private int GetCountSpace(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (0x20 == ch) count++;
            }
            return count;
        }
        private int GetCountBigSignCharacter(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
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
            foreach (char ch in charArr) {
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
            foreach (char ch in charArr) {
                if ((0xAC00 <= ch && ch <= 0xD7A3) //Korean
                    || (0x3131 <= ch && ch <= 0x318E) //Korean
                   ) count++;
            }
            return count;
        }
        private int GetCountJpnAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
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
            foreach (char ch in charArr) {
                if ((0x61 <= ch && ch <= 0x7A)) count++;
            }
            return count;
        }
        public void SetCurrentProfileForeColor(Color color) {
            this.lblProfile.ForeColor = color;
        }
        private int GetOverlayProfileOffset(string s) {
            int sizeOfText = TextRenderer.MeasureText(s, this.lblProfile.Font).Width;
            int offset;
            if (this.lblProfile.Font.FontFamily.Name.Equals(DefaultFontCollection.Families[2].Name)) { // eng, fre
                offset = 22 - (int)(this.GetCountEnglishlowercase(s) * (-0.3F)) -
                         (int)(this.GetCountKorAlphabet(s) * (6.7F)) -
                         (int)(this.GetCountJpnAlphabet(s) * (0.8F)) -
                         (int)(this.GetCountBigSignCharacter(s) * (0.1F)) -
                         (int)(this.GetCountSmallSignCharacter(s) * (0.2F));
            } else if (this.lblProfile.Font.FontFamily.Name.Equals(DefaultFontCollection.Families[0].Name)) { // kor, jpn
                offset = 22 - (int)(this.GetCountBigSignCharacter(s) * (0.1F)) -
                         (int)(this.GetCountSmallSignCharacter(s) * (0.2F));
            } else if (this.lblProfile.Font.FontFamily.Name.Equals(DefaultFontCollection.Families[1].Name)) { // sc
                offset = 22 - (this.GetCountKorAlphabet(s) * 2) -
                         (int)(this.GetCountBigSignCharacter(s) * (0.1F)) -
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
                this.Background?.Dispose();

                bool tabsDisplayed = this.StatsForm.CurrentSettings.ShowOverlayTabs;
                //bool profileDisplayed = StatsForm.CurrentSettings.ShowOverlayProfile;
                bool overlayCustomized = this.StatsForm.CurrentSettings.IsOverlayBackgroundCustomized;
                Bitmap newImage = new Bitmap(this.drawWidth, this.drawHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage)) {
                    Bitmap background;
                    if (string.IsNullOrEmpty(this.BackgroundResourceName)) {
                        background = Properties.Resources.background;
                    } else {
                        background = overlayCustomized
                            ? File.Exists($"Overlay/{this.BackgroundResourceName}.png") ? new Bitmap($"Overlay/{this.BackgroundResourceName}.png") : Properties.Resources.background
                            : (Bitmap)Properties.Resources.ResourceManager.GetObject(this.BackgroundResourceName) ?? Properties.Resources.background;
                    }
                    g.DrawImage(background, 0, tabsDisplayed ? 35 : 0);
                    
                    if (tabsDisplayed) {
                        Bitmap tab;
                        if (string.IsNullOrEmpty(this.TabResourceName)) {
                            tab = Properties.Resources.tab_unselected;
                        } else {
                            tab = overlayCustomized
                                ? File.Exists($"Overlay/{this.TabResourceName}.png") ? new Bitmap($"Overlay/{this.TabResourceName}.png") : Properties.Resources.tab_unselected
                                : (Bitmap)Properties.Resources.ResourceManager.GetObject(this.TabResourceName) ?? Properties.Resources.tab_unselected;
                        }
                        g.DrawImage(tab, this.drawWidth - 170 - this.GetOverlayProfileOffset(this.lblProfile.Text), 0);
                        g.DrawImage(tab, this.drawWidth - 110, 0);
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
            this.lblRound.Text = $"{Multilingual.GetWord("overlay_round_prefix")}1{Multilingual.GetWord("overlay_round_suffix")} :";
            this.lblWins.Text = $"{Multilingual.GetWord("overlay_wins")} :";
            this.lblWins.TextRight = $"0{Multilingual.GetWord("overlay_inning")} - 0.0%";
            this.lblFinish.Text = $"{Multilingual.GetWord("overlay_finish")} :";
            this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_players")} :";
        }
    }
}