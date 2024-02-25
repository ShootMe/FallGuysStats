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
    public sealed partial class Overlay : Form {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        
        public Stats StatsForm { get; set; }
        public string BackgroundResourceName;
        public string TabResourceName;
        private Thread timer;
        public bool flippedImage;
        private int frameCount;
        private bool isTimeToSwitch;
        private int switchCount;
        private Bitmap Background, DrawImage;
        private Graphics DrawGraphics;
        private RoundInfo lastRound;
        private string savedSessionId;
        private int savedRoundNum;
        private string roundId;
        private string roundName;
        private LevelStats levelStats;
        private LevelType levelType;
        private BestRecordType recordType;
        private StatSummary levelSummary;
        private int drawWidth, drawHeight;
        private bool startedPlaying;
        private DateTime startTime;
        private bool shiftKeyToggle, ctrlKeyToggle;
        private new Size DefaultSize;
        private Bitmap customizedBackground, customizedTab;
        private string backgroundResourceNameCache, tabResourceNameCache;

        private bool isPositionButtonMouseEnter;
        private readonly Image positionNeOffBlur = Utils.ImageOpacity(Properties.Resources.position_ne_off_icon, 0.4F);
        private readonly Image positionNwOffBlur = Utils.ImageOpacity(Properties.Resources.position_nw_off_icon, 0.4F);
        private readonly Image positionSeOffBlur = Utils.ImageOpacity(Properties.Resources.position_se_off_icon, 0.4F);
        private readonly Image positionSwOffBlur = Utils.ImageOpacity(Properties.Resources.position_sw_off_icon, 0.4F);
        private readonly Image positionNeOffFocus = Utils.ImageOpacity(Properties.Resources.position_ne_off_icon, 0.8F);
        private readonly Image positionNwOffFocus = Utils.ImageOpacity(Properties.Resources.position_nw_off_icon, 0.8F);
        private readonly Image positionSeOffFocus = Utils.ImageOpacity(Properties.Resources.position_se_off_icon, 0.8F);
        private readonly Image positionSwOffFocus = Utils.ImageOpacity(Properties.Resources.position_sw_off_icon, 0.8F);
        private readonly Image positionNeOnBlur = Utils.ImageOpacity(Properties.Resources.position_ne_on_icon, 0.4F);
        private readonly Image positionNwOnBlur = Utils.ImageOpacity(Properties.Resources.position_nw_on_icon, 0.4F);
        private readonly Image positionSeOnBlur = Utils.ImageOpacity(Properties.Resources.position_se_on_icon, 0.4F);
        private readonly Image positionSwOnBlur = Utils.ImageOpacity(Properties.Resources.position_sw_on_icon, 0.4F);
        private readonly Image positionNeOnFocus = Utils.ImageOpacity(Properties.Resources.position_ne_on_icon, 0.8F);
        private readonly Image positionNwOnFocus = Utils.ImageOpacity(Properties.Resources.position_nw_on_icon, 0.8F);
        private readonly Image positionSeOnFocus = Utils.ImageOpacity(Properties.Resources.position_se_on_icon, 0.8F);
        private readonly Image positionSwOnFocus = Utils.ImageOpacity(Properties.Resources.position_sw_on_icon, 0.8F);
        private readonly Image positionLockBlur = Utils.ImageOpacity(Properties.Resources.switch_lock_icon, 0.4F);
        private readonly Image positionLockFocus = Utils.ImageOpacity(Properties.Resources.switch_lock_icon, 0.8F);
        private readonly Image positionUnlockBlur = Utils.ImageOpacity(Properties.Resources.switch_unlock_icon, 0.4F);
        private readonly Image positionUnlockFocus = Utils.ImageOpacity(Properties.Resources.switch_unlock_icon, 0.8F);
        public bool isFixedPositionNe, isFixedPositionNw, isFixedPositionSe, isFixedPositionSw, isPositionLock;
        private bool isFocused, isMouseOver;

        private static readonly PrivateFontCollection DefaultFontCollection;
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
            SetDefaultFont(18, Stats.CurrentLanguage);
            
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
        }
        
        private void Overlay_Load(object sender, EventArgs e) {
            this.ChangeLanguage();
            this.SetBackground();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        
        public void SetFixedPosition(bool positionNe, bool positionNw, bool positionSe, bool positionSw, bool positionFree) {
            this.isFixedPositionNe = positionNe;
            this.isFixedPositionNw = positionNw;
            this.isFixedPositionSe = positionSe;
            this.isFixedPositionSw = positionSw;
            this.isPositionLock = positionFree;
        }
        public void SetBackgroundResourcesName(string backgroundResourceName, string tabResourceName) {
            this.BackgroundResourceName = backgroundResourceName;
            this.TabResourceName = tabResourceName;
        }
        private void SetBackground(string backgroundResourceName = null) {
            Bitmap background = string.IsNullOrEmpty(backgroundResourceName)
                ? Properties.Resources.background
                : (Bitmap)Properties.Resources.ResourceManager.GetObject(backgroundResourceName) ?? Properties.Resources.background;
            
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

        private static void SetFonts(Control control, float customSize = -1, Font font = null) {
            if (font == null) {
                font = customSize <= 0 ? DefaultFont : new Font(GetDefaultFontFamilies(), customSize, FontStyle.Regular, GraphicsUnit.Pixel);
            }

            control.Font = font;
            foreach (Control ctr in control.Controls) {
                if (ctr.Name.Equals("lblProfile")) { ctr.Font = GetMainFont(font.Size, FontStyle.Bold); continue; }
                ctr.Font = font;
                if (ctr.HasChildren) {
                    SetFonts(ctr, customSize, font);
                }
            }
        }

        private void SetDefaultFontColor() {
            this.ForeColor = Color.White;
        }
        private void SetFontColor(Color color) {
            this.ForeColor = color;
        }
        public static void SetDefaultFont(float emSize, Language lang) {
            DefaultFont = new Font(GetDefaultFontFamilies(lang), emSize, (lang == Language.English || lang == Language.French) ? FontStyle.Regular : FontStyle.Bold, GraphicsUnit.Pixel);
        }
        public static Font GetDefaultFont(float emSize, Language lang) {
            return new Font(GetDefaultFontFamilies(lang), emSize, (lang == Language.English || lang == Language.French) ? FontStyle.Regular : FontStyle.Bold, GraphicsUnit.Pixel);
        }
        public static FontFamily GetDefaultFontFamilies(Language lang = Language.English) {
            return (lang == Language.English || lang == Language.French) ? DefaultFontCollection.Families[2] : (lang == Language.SimplifiedChinese || lang == Language.TraditionalChinese) ? DefaultFontCollection.Families[1] : DefaultFontCollection.Families[0];
        }
        public static Font GetMainFont(float emSize, FontStyle fontStyle = FontStyle.Regular, Language lang = Language.English) {
            return new Font(GetMainFontFamilies(lang), emSize, fontStyle, GraphicsUnit.Pixel);
        }
        public static FontFamily GetMainFontFamilies(Language lang) {
            return (lang == Language.SimplifiedChinese || lang == Language.TraditionalChinese) ? DefaultFontCollection.Families[1] : DefaultFontCollection.Families[0];
        }
        public bool IsFocused() {
            return this.isFocused;
        }
        public bool IsMouseOver() {
            return this.isMouseOver;
        }
        public bool IsFixed() {
            return this.isFixedPositionNe || this.isFixedPositionNw || this.isFixedPositionSe || this.isFixedPositionSw || this.isPositionLock;
        }
        // public void ShowCountryName(string message, int duration) {
        //     if (this.lblPlayers.DrawVisible) {
        //         this.StatsForm.AllocOverlayTooltip();
        //         Rectangle rectangle = this.lblPlayers.Bounds;
        //         Point position = new Point(rectangle.X, rectangle.Y);
        //         this.StatsForm.ShowOverlayTooltip(message, this, position, duration);
        //     }
        // }
        private void Position_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                string iconName = ((PictureBox)sender).Name;
                Screen screen = Utils.GetCurrentScreen(this.Location);
                Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                if (iconName.Equals("picPositionNE")) {
                    if (this.isFixedPositionNe) {
                        if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && Utils.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width, this.Height)) {
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
                        this.SetVisiblePositionLockButton(true);
                    } else {
                        if (!this.IsFixed()) {
                            this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                        }
                        this.FlipDisplay(true);
                        //this.Background = this.RecreateBackground();
                        this.Location = new Point(screenLocation.X, 0);
                        this.SetFocusPositionMenu("ne");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        //this.StatsForm.CurrentSettings.FlippedDisplay = true;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "ne";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                        this.SetVisiblePositionLockButton(false);
                    }
                } else if (iconName.Equals("picPositionNW")) {
                    if (this.isFixedPositionNw) {
                        if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && Utils.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width, this.Height)) {
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
                        this.SetVisiblePositionLockButton(true);
                    } else {
                        if (!this.IsFixed()) {
                            this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                        }
                        this.FlipDisplay(false);
                        //this.Background = this.RecreateBackground();
                        this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, 0);
                        this.SetFocusPositionMenu("nw");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        //this.StatsForm.CurrentSettings.FlippedDisplay = false;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "nw";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                        this.SetVisiblePositionLockButton(false);
                    }
                } else if (iconName.Equals("picPositionSE")) {
                    if (this.isFixedPositionSe) {
                        if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && Utils.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width, this.Height)) {
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
                        this.SetVisiblePositionLockButton(true);
                    } else {
                        if (!this.IsFixed()) {
                            this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                        }
                        this.FlipDisplay(true);
                        //this.Background = this.RecreateBackground();
                        this.Location = new Point(screenLocation.X, screenLocation.Y + screenSize.Height - this.Height);
                        this.SetFocusPositionMenu("se");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        //this.StatsForm.CurrentSettings.FlippedDisplay = true;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "se";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                        this.SetVisiblePositionLockButton(false);
                    }
                } else if (iconName.Equals("picPositionSW")) {
                    if (this.isFixedPositionSw) {
                        if (this.StatsForm.CurrentSettings.OverlayLocationX.HasValue && Utils.IsOnScreen(this.StatsForm.CurrentSettings.OverlayLocationX.Value, this.StatsForm.CurrentSettings.OverlayLocationY.Value, this.Width, this.Height)) {
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
                        this.SetVisiblePositionLockButton(true);
                    } else {
                        if (!this.IsFixed()) {
                            this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
                            this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
                        }
                        this.FlipDisplay(false);
                        //this.Background = this.RecreateBackground();
                        this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, screenLocation.Y + screenSize.Height - this.Height);
                        this.SetFocusPositionMenu("sw");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        //this.StatsForm.CurrentSettings.FlippedDisplay = false;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "sw";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                        this.SetVisiblePositionLockButton(false);
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
                        this.SetVisiblePositionMenu(true);
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
                        this.SetVisiblePositionMenu(false);
                    }
                }
                this.StatsForm.SaveUserSettings();
            }
        }
        private void Position_MouseEnter(object sender, EventArgs e) {
            this.StatsForm.HideTooltip(this);
            if (!this.IsFixed()) {
                Rectangle rectangle = ((PictureBox)sender).Bounds;
                Point position = new Point(rectangle.Right + 5, rectangle.Top);
                this.StatsForm.AllocTooltip();
                this.StatsForm.ShowTooltip(Multilingual.GetWord($"overlay_{((PictureBox)sender).Name}_tooltip"), this, position);
            }
            
            if (this.isPositionButtonMouseEnter) return;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.picPositionLock.Image = this.isPositionLock ? this.positionLockFocus : this.positionUnlockFocus;
                } else {
                    this.SetFocusPositionMenu();
                }
            } else {
                this.SetFocusPositionMenu();
                this.picPositionLock.Image = this.isPositionLock ? this.positionLockFocus : this.positionUnlockFocus;
            }
            this.isPositionButtonMouseEnter = true;
        }
        private void Position_MouseLeave(object sender, EventArgs e) {
            this.StatsForm.HideTooltip(this);

            if (!this.isPositionButtonMouseEnter) return;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
                } else {
                    this.SetBlurPositionMenu();
                }
            } else {
                this.SetBlurPositionMenu();
                this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
            }
            this.isPositionButtonMouseEnter = false;
        }
        private void SetLocationPositionMenu(bool visibleTab, bool flipped) {
            this.picPositionNE.Location = new Point((this.Width / 2) - (this.picPositionNE.Size.Width + 2), (this.Height / 2) - (this.picPositionNE.Size.Height + 2) + (visibleTab ? 11 : -6));
            this.picPositionNW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) - (this.picPositionNE.Size.Height + 2) + (visibleTab ? 11 : -6));
            this.picPositionSE.Location = new Point((this.Width / 2) - (this.picPositionSE.Size.Width + 2), (this.Height / 2) + 2 + (visibleTab ? 11 : -6));
            this.picPositionSW.Location = new Point((this.Width / 2) + 2, (this.Height / 2) + 2 + (visibleTab ? 11 : -6));
            this.picPositionLock.Location = new Point(flipped ? (this.Width - this.picPositionLock.Width - 14) : 14, (this.Height / 2) - (this.picPositionLock.Size.Height + 6) + (visibleTab ? 11 : -6));
        }
        private void SetBlurPositionMenu() {
            this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnBlur : this.positionNeOffBlur;
            this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnBlur : this.positionNwOffBlur;
            this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnBlur : this.positionSeOffBlur;
            this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnBlur : this.positionSwOffBlur;
        }
        private void SetFocusPositionMenu() {
            this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnFocus : this.positionNeOffFocus;
            this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnFocus : this.positionNwOffFocus;
            this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnFocus : this.positionSeOffFocus;
            this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnFocus : this.positionSwOffFocus;
        }
        private void SetFocusPositionMenu(string flag) {
            this.isFixedPositionNe = flag.Equals("ne");
            this.isFixedPositionNw = flag.Equals("nw");
            this.isFixedPositionSe = flag.Equals("se");
            this.isFixedPositionSw = flag.Equals("sw");
            this.picPositionNE.Image = flag.Equals("ne") ? this.positionNeOnFocus : this.positionNeOffFocus;
            this.picPositionNW.Image = flag.Equals("nw") ? this.positionNwOnFocus : this.positionNwOffFocus;
            this.picPositionSE.Image = flag.Equals("se") ? this.positionSeOnFocus : this.positionSeOffFocus;
            this.picPositionSW.Image = flag.Equals("sw") ? this.positionSwOnFocus : this.positionSwOffFocus;
        }
        private void SetVisiblePositionMenu(bool visible) {
            if (visible) {
                this.picPositionNE.Show();
                this.picPositionNW.Show();
                this.picPositionSE.Show();
                this.picPositionSW.Show();
            } else {
                this.picPositionNE.Hide();
                this.picPositionNW.Hide();
                this.picPositionSE.Hide();
                this.picPositionSW.Hide();
            }
        }
        
        private void SetVisiblePositionLockButton(bool visible) {
            if (visible) { this.picPositionLock.Show(); }
            else { this.picPositionLock.Hide(); }
        }
        
        private void Overlay_MouseEnter(object sender, EventArgs e) {
            this.isMouseOver = true;
        }
        
        private void Overlay_MouseLeave(object sender, EventArgs e) {
            this.isMouseOver = false;
        }
        
        private void Overlay_GotFocus(object sender, EventArgs e) {
            this.isFocused = true;
            if (this.IsFixed()) {
                if (this.isPositionLock) {
                    this.SetVisiblePositionLockButton(true);
                    this.SetVisiblePositionMenu(false);
                } else {
                    this.SetVisiblePositionLockButton(false);
                    this.SetVisiblePositionMenu(true);
                    
                }
            } else {
                if (this.picPositionNE.Visible && this.picPositionNW.Visible &&
                    this.picPositionSE.Visible && this.picPositionSW.Visible && this.picPositionLock.Visible) return;
                this.SetVisiblePositionMenu(true);
                this.SetVisiblePositionLockButton(true);
            }
        }
        
        private void Overlay_LostFocus(object sender, EventArgs e) {
            this.isFocused = false;
            if (!this.picPositionNE.Visible && !this.picPositionNW.Visible &&
                    !this.picPositionSE.Visible && !this.picPositionSW.Visible && !this.picPositionLock.Visible) return;
            this.SetVisiblePositionMenu(false);
            this.SetVisiblePositionLockButton(false);
        }
        
        private void Overlay_Resize(object sender, EventArgs e) {
            this.SetLocationPositionMenu(this.drawHeight > 99, this.StatsForm.CurrentSettings.FlippedDisplay);
        }
        
        private void Overlay_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            if ((sender.GetType() == this.picPositionNE.GetType()) ||
                (sender.GetType() == this.picPositionNW.GetType()) ||
                (sender.GetType() == this.picPositionSE.GetType()) ||
                (sender.GetType() == this.picPositionSW.GetType()) ||
                (sender.GetType() == this.picPositionLock.GetType())) return;
            if (this.IsFixed()) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        
        private void UpdateTimer() {
            while (this.StatsForm != null && !this.StatsForm.IsDisposed && !this.StatsForm.Disposing) {
                try {
                    if (this.IsHandleCreated && !this.StatsForm.Disposing && !this.StatsForm.IsDisposed) {
                        this.frameCount++;
                        this.isTimeToSwitch = this.frameCount % (this.StatsForm.CurrentSettings.CycleTimeSeconds * 20) == 0;
                        this.Invoke((Action)this.UpdateInfo);
                    }

                    this.StatsForm.UpdateDates();
                    Thread.Sleep(50);
                } catch {
                    // ignored
                }
            }
        }
        
        private void SetRoundLabel(LevelStats level, LevelType levelType, string roundName, int overlaySetting) {
            if (Stats.IsQueued && (overlaySetting == 1 || overlaySetting == 5)) {
                this.lblRound.LevelColor = Color.Empty;
                this.lblRound.LevelTrueColor = Color.Empty;
                this.lblRound.RoundIcon = null;
                this.lblRound.ImageWidth = 0;
                this.lblRound.ImageHeight = 0;
                this.lblRound.Text = $"{Multilingual.GetWord("overlay_queued_players")} :";
                this.lblRound.TextRight = Stats.QueuedPlayers.ToString();
                this.lblRound.ForeColor = this.ForeColor;
            } else {
                if (this.StatsForm.CurrentSettings.ColorByRoundType) {
                    this.lblRound.Text = $"{Multilingual.GetWord("overlay_round_abbreviation_prefix")}{this.lastRound.Round}{Multilingual.GetWord("overlay_round_abbreviation_suffix")} :";
                    this.lblRound.LevelColor = levelType == LevelType.Unknown ? levelType.LevelBackColor(false, false, 127) : levelType.LevelBackColor(this.lastRound.IsFinal, this.lastRound.IsTeam, 223);
                    this.lblRound.LevelTrueColor = levelType == LevelType.Unknown ? levelType.LevelBackColor(false, false, 127) : levelType.LevelBackColor(false, this.lastRound.IsTeam, 127);
                    this.lblRound.RoundIcon = levelType == LevelType.Unknown ? Properties.Resources.round_unknown_icon : level.RoundBigIcon;
                    if (this.lblRound.RoundIcon.Height != 23) {
                        float ratio = 23f / this.lblRound.RoundIcon.Height;
                        this.lblRound.ImageHeight = 23;
                        this.lblRound.ImageWidth = (int)(this.lblRound.RoundIcon.Width * ratio);
                    } else {
                        this.lblRound.ImageHeight = this.lblRound.RoundIcon.Height;
                        this.lblRound.ImageWidth = this.lblRound.RoundIcon.Width;
                    }
                } else {
                    this.lblRound.Text = $"{Multilingual.GetWord("overlay_round_prefix")}{this.lastRound.Round}{Multilingual.GetWord("overlay_round_suffix")} :";
                    this.lblRound.LevelColor = Color.Empty;
                    this.lblRound.LevelTrueColor = Color.Empty;
                    this.lblRound.RoundIcon = null;
                    this.lblRound.ImageWidth = 0;
                    this.lblRound.ImageHeight = 0;
                }
                if (roundName.Length > 30) { roundName = roundName.Substring(0, 30); }
                this.lblRound.TextRight = roundName;
            }
        }
        
        private void SetWinsLabel(StatSummary levelSummary, int overlaySetting) {
            if (Stats.IsQueued && overlaySetting == 3) {
                this.lblWins.Text = $"{Multilingual.GetWord("overlay_queued_players")} :";
                this.lblWins.TextRight = $"{Stats.QueuedPlayers:N0}";
                this.lblWins.ForeColor = this.ForeColor;
            } else {
                this.lblWins.Text = $"{Multilingual.GetWord("overlay_wins")} :";
                float winChance = levelSummary.TotalWins * 100f / (levelSummary.TotalShows == 0 ? 1 : levelSummary.TotalShows);
                string winChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(winChance * 10) / 10}%";
                if (this.StatsForm.CurrentSettings.PreviousWins > 0) {
                    this.lblWins.TextRight = $"{levelSummary.TotalWins:N0} ({levelSummary.AllWins + this.StatsForm.CurrentSettings.PreviousWins:N0}){winChanceDisplay}";
                } else {
                    this.lblWins.TextRight = this.StatsForm.CurrentSettings.FilterType != 1
                        ? $"{levelSummary.TotalWins:N0}{Multilingual.GetWord("overlay_win")} ({levelSummary.AllWins:N0}{Multilingual.GetWord("overlay_win")}){winChanceDisplay}"
                        : $"{levelSummary.TotalWins:N0}{Multilingual.GetWord("overlay_win")}{winChanceDisplay}";
                }
            }
        }
        
        private void SetFinalsLabel(StatSummary levelSummary, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && overlaySetting == 3) {
                this.lblFinals.OverlaySetting = overlaySetting;
                this.lblFinals.TickProgress = DateTime.Now.Second;
                this.lblFinals.Text = $"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblFinals.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblFinals.TickProgress = 0;
                this.lblFinals.Text = $"{Multilingual.GetWord("overlay_finals")} :";
                string finalText = $"{levelSummary.TotalFinals:N0}{(levelSummary.TotalShows < 100000 ? $" / {levelSummary.TotalShows:N0}" : Multilingual.GetWord("overlay_inning"))}";
                float finalChance = levelSummary.TotalFinals * 100f / (levelSummary.TotalShows == 0 ? 1 : levelSummary.TotalShows);
                string finalChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(finalChance * 10) / 10}%";
                this.lblFinals.TextRight = $"{finalText}{finalChanceDisplay}";
            }
        }
        
        private void SetStreakLabel(StatSummary levelSummary, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (overlaySetting == 3)) {
                this.lblStreak.OverlaySetting = overlaySetting;
                this.lblStreak.Text = "";
                this.lblStreak.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
            } else {
                int streakSwitchCount = this.switchCount;
                if (!this.StatsForm.CurrentSettings.SwitchBetweenStreaks) {
                    streakSwitchCount = this.StatsForm.CurrentSettings.OnlyShowFinalStreak ? 1 : 0;
                }
                switch (streakSwitchCount % 2) {
                    case 0:
                        this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak")} :";
                        string currentStreakSuffix = Stats.CurrentLanguage == Language.Korean ? (levelSummary.CurrentStreak > 1 ? Multilingual.GetWord("overlay_streak_suffix") : Multilingual.GetWord("overlay_win")) : Multilingual.GetWord("overlay_streak_suffix");
                        string bestStreakSuffix = Stats.CurrentLanguage == Language.Korean ? (levelSummary.BestStreak > 1 ? Multilingual.GetWord("overlay_streak_suffix") : Multilingual.GetWord("overlay_win")) : Multilingual.GetWord("overlay_streak_suffix");
                        this.lblStreak.TextRight = $"{levelSummary.CurrentStreak:N0}{currentStreakSuffix} ({Multilingual.GetWord("overlay_best")}{levelSummary.BestStreak:N0}{bestStreakSuffix})";
                        break;
                    case 1:
                        this.lblStreak.Text = $"{Multilingual.GetWord("overlay_streak_finals")} :";
                        this.lblStreak.TextRight = $"{levelSummary.CurrentFinalStreak:N0}{Multilingual.GetWord("overlay_inning")} ({Multilingual.GetWord("overlay_best")}{levelSummary.BestFinalStreak:N0}{Multilingual.GetWord("overlay_inning")})";
                        break;
                }
            }
        }
        
        private void SetQualifyChanceLabel(StatSummary levelSummary, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (overlaySetting == 1 || overlaySetting == 5)) {
                this.lblQualifyChance.OverlaySetting = overlaySetting;
                this.lblQualifyChance.Text = "";
                this.lblQualifyChance.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
            } else {
                int qualifySwitchCount = this.switchCount;
                if (!this.StatsForm.CurrentSettings.SwitchBetweenQualify) {
                    qualifySwitchCount = this.StatsForm.CurrentSettings.OnlyShowGold ? 1 : 0;
                }
                float qualifyChance;
                string qualifyChanceDisplay;
                string qualifyDisplay;
                switch (qualifySwitchCount % 2) {
                    case 0:
                        this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_chance")} :";
                        qualifyChance = levelSummary.TotalQualify * 100f / (levelSummary.TotalPlays == 0 ? 1 : levelSummary.TotalPlays);
                        qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(qualifyChance * 10) / 10}%";
                        qualifyDisplay = $"{levelSummary.TotalQualify}{(levelSummary.TotalPlays < 100000 ? $" / {levelSummary.TotalPlays:N0}" : Multilingual.GetWord("overlay_inning"))}";
                        this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                        break;
                    case 1:
                        this.lblQualifyChance.Text = $"{Multilingual.GetWord("overlay_qualify_gold")} :";
                        qualifyChance = levelSummary.TotalGolds * 100f / (levelSummary.TotalPlays == 0 ? 1 : levelSummary.TotalPlays);
                        qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(qualifyChance * 10) / 10}%";
                        qualifyDisplay = $"{levelSummary.TotalGolds}{(levelSummary.TotalPlays < 100000 ? $" / {levelSummary.TotalPlays:N0}" : Multilingual.GetWord("overlay_inning"))}";
                        this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                        break;
                }
            }
        }
        
        private void SetFastestLabel(StatSummary levelSummary, BestRecordType recordType, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (overlaySetting == 2)) {
                this.lblFastest.OverlaySetting = overlaySetting;
                this.lblFastest.TickProgress = DateTime.Now.Second;
                this.lblFastest.Text = $"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblFastest.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                if (Stats.IsQueued && overlaySetting == 6) {
                    this.lblFastest.Text = $"{Multilingual.GetWord("overlay_queued_players")} :";
                    this.lblFastest.TextRight = Stats.QueuedPlayers.ToString();
                    this.lblFastest.ForeColor = this.ForeColor;
                } else {
                    this.lblFastest.TickProgress = 0;
                    int fastestSwitchCount = this.switchCount;
                    
                    if (!this.StatsForm.CurrentSettings.SwitchBetweenLongest) {
                        fastestSwitchCount = this.StatsForm.CurrentSettings.OnlyShowLongest ? 0 : recordType == BestRecordType.HighScore ? 2 : 1;
                    }
                    switch (fastestSwitchCount % ((levelSummary.BestScore.HasValue && recordType != BestRecordType.Fastest) ? 3 : 2)) {
                        case 0:
                            this.lblFastest.Text = $"{Multilingual.GetWord("overlay_longest")} :";
                            this.lblFastest.TextRight = levelSummary.LongestFinish.HasValue ? $"{levelSummary.LongestFinish:m\\:ss\\.fff}" : "-";
                            break;
                        case 1:
                            if (this.StatsForm.CurrentSettings.SwitchBetweenLongest) {
                                this.lblFastest.Text = $"{Multilingual.GetWord("overlay_fastest")} :";
                                this.lblFastest.TextRight = levelSummary.BestFinish.HasValue ? $"{levelSummary.BestFinish:m\\:ss\\.fff}" : "-";
                            } else {
                                this.lblFastest.Text = $"{Multilingual.GetWord("overlay_personal_best")} :";
                                if (recordType == BestRecordType.Longest) {
                                    this.lblFastest.TextRight = levelSummary.LongestFinish.HasValue ? $"{levelSummary.LongestFinish:m\\:ss\\.fff}" : "-";
                                } else {
                                    this.lblFastest.TextRight = levelSummary.BestFinish.HasValue ? $"{levelSummary.BestFinish:m\\:ss\\.fff}" : "-";
                                }
                            }
                            break;
                        case 2:
                            this.lblFastest.Text = $"{Multilingual.GetWord("overlay_best_score")} :";
                            this.lblFastest.TextRight = $"{levelSummary.BestScore}";
                            break;
                    }
                }
            }
        }
        
        private void SetPlayersLabel(int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (overlaySetting == 0 || overlaySetting == 1 || overlaySetting == 4 || overlaySetting == 5)) {
                this.lblPlayers.Image = null;
                this.lblPlayersPs.DrawVisible = false;
                this.lblPlayersXbox.DrawVisible = false;
                this.lblPlayersSwitch.DrawVisible = false;
                this.lblPlayersPc.DrawVisible = false;
                this.lblCountryIcon.DrawVisible = false;
                this.lblPingIcon.DrawVisible = false;
                this.lblPlayers.OverlaySetting = overlaySetting;
                this.lblPlayers.TickProgress = DateTime.Now.Second;
                this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblPlayers.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblPlayers.TickProgress = 0;
                int playersSwitchCount = this.switchCount;
                if (!this.StatsForm.CurrentSettings.SwitchBetweenPlayers) {
                    playersSwitchCount = this.StatsForm.CurrentSettings.OnlyShowPing ? 1 : 0;
                }
                switch (playersSwitchCount % 2) {
                    case 0:
                        this.lblPlayers.TextRight = $"{this.lastRound?.Players}";
                        if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                            this.lblPlayers.Image = Properties.Resources.player_icon;
                            this.lblPlayers.Text = @"ㅤ   :";
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
                            this.lblCountryIcon.DrawVisible = false;
                            this.lblPingIcon.DrawVisible = false;
                        } else {
                            this.lblPlayers.Image = null;
                            this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_players")} :";
                            this.lblPlayersPs.DrawVisible = false;
                            this.lblPlayersXbox.DrawVisible = false;
                            this.lblPlayersSwitch.DrawVisible = false;
                            this.lblPlayersPc.DrawVisible = false;
                            this.lblCountryIcon.DrawVisible = false;
                            this.lblPingIcon.DrawVisible = false;
                        }
                        break;
                    case 1:
                        this.lblPlayers.Image = null;
                        this.lblPlayersPs.DrawVisible = false;
                        this.lblPlayersXbox.DrawVisible = false;
                        this.lblPlayersSwitch.DrawVisible = false;
                        this.lblPlayersPc.DrawVisible = false;
                        this.lblCountryIcon.DrawVisible = true;
                        this.lblPingIcon.DrawVisible = true;
                        if (Stats.IsBadServerPing) {
                            this.lblCountryIcon.ImageX = 49;
                            this.lblCountryIcon.Image = string.IsNullOrEmpty(Stats.LastCountryAlpha2Code) ? null : (Image)Properties.Resources.ResourceManager.GetObject($"country_{Stats.LastCountryAlpha2Code}{(this.StatsForm.CurrentSettings.ShadeTheFlagImage ? "_shiny" : "")}_icon");
                            this.lblPingIcon.ImageX = 40;
                            this.lblPingIcon.Image = Properties.Resources.ping_200_icon;
                        } else {
                            if (Stats.LastServerPing >= 100 && 199 >= Stats.LastServerPing) {
                                this.lblPlayers.PingColor = Color.Orange;
                            } else if (Stats.LastServerPing >= 200) {
                                this.lblPlayers.PingColor = Color.Red;
                            } else {
                                this.lblPlayers.PingColor = Color.Green;
                            }
                            this.lblCountryIcon.Image = (Image)(!string.IsNullOrEmpty(Stats.LastCountryAlpha2Code) && Stats.IsConnectedToServer && Stats.LastServerPing > 0 ? Properties.Resources.ResourceManager.GetObject($"country_{Stats.LastCountryAlpha2Code}{(this.StatsForm.CurrentSettings.ShadeTheFlagImage ? "_shiny" : "")}_icon") : null);
                            this.lblCountryIcon.ImageX = (Stats.IsConnectedToServer && Stats.LastServerPing < 1000)
                                                         ? (Stats.LastServerPing > 0 && 9 >= Stats.LastServerPing ? 39
                                                           : Stats.LastServerPing >= 10 && 99 >= Stats.LastServerPing ? 30
                                                           : Stats.LastServerPing >= 100 && 199 >= Stats.LastServerPing ? -3
                                                           : Stats.LastServerPing >= 200 && 999 >= Stats.LastServerPing ? -6 : 0) : -12;
                            
                            this.lblPingIcon.Image = (Stats.IsConnectedToServer && Stats.LastServerPing > 99 && 200 > Stats.LastServerPing) ? Properties.Resources.ping_100_icon
                                                     : (Stats.IsConnectedToServer && Stats.LastServerPing >= 200 ? Properties.Resources.ping_200_icon : null);
                            this.lblPingIcon.ImageX = (Stats.IsConnectedToServer && Stats.LastServerPing >= 100 && 199 >= Stats.LastServerPing) ? -15
                                                      : (Stats.IsConnectedToServer && Stats.LastServerPing >= 200 && 999 >= Stats.LastServerPing) ? -17
                                                      : (Stats.IsConnectedToServer && Stats.LastServerPing > 999) ? -24 : 0;
                            
                            if (!this.Font.FontFamily.Name.Equals(GetDefaultFontFamilies(0).Name)) {
                                this.lblCountryIcon.ImageX += 7;
                                this.lblPingIcon.ImageX += 7;
                            }
                        }
                        
                        this.lblPlayers.Text = $"{Multilingual.GetWord("overlay_ping")} :";
                        this.lblPlayers.TextRight = (Stats.IsConnectedToServer && Stats.LastServerPing > 0) ? $"{Stats.LastServerPing} ms" : "-";
                        break;
                }
            }
        }
        
        private void SetDurationLabel(LevelStats level, DateTime currentUTC, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (overlaySetting == 0 || overlaySetting == 2 || overlaySetting == 4)) {
                this.lblDuration.OverlaySetting = overlaySetting;
                this.lblDuration.TickProgress = 0;
                this.lblDuration.Text = "";
                this.lblDuration.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
            } else if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && overlaySetting == 6) {
                this.lblDuration.OverlaySetting = overlaySetting;
                this.lblDuration.TickProgress = DateTime.Now.Second;
                this.lblDuration.Text = $"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblDuration.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblDuration.TickProgress = 0;
                string showId = this.StatsForm.GetAlternateShowId(this.lastRound.ShowNameId);
                int showType = level == null ? 0
                               : ((string.Equals(showId, "main_show") || string.Equals(showId, "invisibeans_mode") || level.IsCreative) || (!level.IsCreative && level.IsFinal)) && level.TimeLimitSeconds > 0 ? 1
                               : ((string.Equals(showId, "squads_2player_template") || string.Equals(showId, "squads_4player")) && level.TimeLimitSecondsForSquad > 0 ? 2 : 0);
                int timeLimit = showType == 1 ? level.TimeLimitSeconds : (showType == 2 ? level.TimeLimitSecondsForSquad : 0);
                
                if (this.lastRound.UseShareCode) {
                    this.lblDuration.Text = this.lastRound.CreativeTimeLimitSeconds > 0 ? $"{Multilingual.GetWord("overlay_duration")} ({TimeSpan.FromSeconds(this.lastRound.CreativeTimeLimitSeconds):m\\:ss}) :"
                                                                                        : $"{Multilingual.GetWord("overlay_duration")} :";
                } else {
                    this.lblDuration.Text = timeLimit > 0 ? $"{Multilingual.GetWord("overlay_duration")} ({TimeSpan.FromSeconds(timeLimit):m\\:ss}) :" : $"{Multilingual.GetWord("overlay_duration")} :";
                }
                
                DateTime start = this.lastRound.Start;
                DateTime end = this.lastRound.End;
                TimeSpan runningTime = start > currentUTC ? currentUTC - this.startTime : currentUTC - start;
                int maxRunningTime = 12; // in minutes
                
                if (!Stats.IsDisplayOverlayTime) {
                    this.lblDuration.TextRight = "-";
                } else if (Stats.LastPlayedRoundEnd.HasValue && Stats.LastPlayedRoundEnd > Stats.LastPlayedRoundStart) {
                    this.lblDuration.TextRight = $"{Stats.LastPlayedRoundEnd - Stats.LastPlayedRoundStart:m\\:ss\\.fff}";
                } else if (Stats.IsLastPlayedRoundStillPlaying) {
                    bool isOverRunningTime = runningTime.TotalMinutes >= maxRunningTime || !Stats.IsGameRunning;
                    if (this.lastRound.UseShareCode) {
                        runningTime = this.lastRound.CreativeTimeLimitSeconds > 0 ? TimeSpan.FromSeconds(this.lastRound.CreativeTimeLimitSeconds) - (currentUTC - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUTC)) : currentUTC - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUTC);
                    } else {
                        runningTime = timeLimit > 0 ? TimeSpan.FromSeconds(timeLimit) - (currentUTC - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUTC)) : currentUTC - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUTC);
                    }
                    this.lblDuration.TextRight = isOverRunningTime ? "-" : $"{runningTime:m\\:ss}";
                } else if (end != DateTime.MinValue) {
                    TimeSpan time = end - start;
                    if (this.lastRound.UseShareCode) {
                        this.lblDuration.TextRight = this.lastRound.CreativeTimeLimitSeconds > 0 ? $"{TimeSpan.FromSeconds(this.lastRound.CreativeTimeLimitSeconds) - time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                    } else {
                        this.lblDuration.TextRight = timeLimit > 0? $"{TimeSpan.FromSeconds(timeLimit) - time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                    }
                } else if (this.lastRound.Playing) {
                    if (this.lastRound.UseShareCode) {
                        this.lblDuration.TextRight = this.lastRound.CreativeTimeLimitSeconds > 0 ? $"{TimeSpan.FromSeconds(this.lastRound.CreativeTimeLimitSeconds) - runningTime:m\\:ss}" : $"{runningTime:m\\:ss}";
                    } else {
                        this.lblDuration.TextRight = timeLimit > 0 ? $"{TimeSpan.FromSeconds(timeLimit) - runningTime:m\\:ss}" : $"{runningTime:m\\:ss}";
                    }
                } else {
                    this.lblDuration.TextRight = "-";
                }
            }
        }
        
        private void SetFinishLabel(StatSummary levelSummary, LevelType levelType, string roundId, BestRecordType recordType, DateTime currentUTC, int overlaySetting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && overlaySetting == 6) {
                this.lblFinish.OverlaySetting = overlaySetting;
                this.lblFinish.Text = "";
                this.lblFinish.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
                this.lblFinish.ForeColor = this.ForeColor;
            } else {
                if (Stats.IsQueued && (overlaySetting == 0 || overlaySetting == 2 || overlaySetting == 4)) {
                    this.lblFinish.Text = $"{Multilingual.GetWord("overlay_queued_players")} :";
                    this.lblFinish.TextRight = Stats.QueuedPlayers.ToString();
                    this.lblFinish.ForeColor = this.ForeColor;
                } else {
                    this.lblFinish.Text = $"{Multilingual.GetWord("overlay_finish")} :";
                    DateTime start = this.lastRound.Start;
                    DateTime end = this.lastRound.End;
                    DateTime? finish = this.lastRound.Finish;
                    TimeSpan runningTime = start > currentUTC ? currentUTC - this.startTime : currentUTC - start;
                    int maxRunningTime = 12; // in minutes
                    float fBrightness = 0.7f;
                    
                    if (!Stats.IsDisplayOverlayTime) {
                        this.lblFinish.TextRight = "-";
                        this.lblFinish.ForeColor = this.ForeColor;
                    } else if (finish.HasValue) {
                        TimeSpan time = finish.GetValueOrDefault(start) - start;
                        if (this.lastRound.Crown) {
                            this.lblFinish.TextRight = this.StatsForm.CurrentSettings.DisplayGamePlayedInfo ? $"{Multilingual.GetWord("overlay_position_win")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                        } else {
                            if (roundId.Equals("round_skeefall")) { // "Ski Fall" Hunt-like Level Type
                                this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{Multilingual.GetWord("overlay_position_qualified")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                            } else {
                                switch (levelType) {
                                    case LevelType.Survival:
                                        this.lblFinish.TextRight = recordType == BestRecordType.Fastest ? (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"# {Multilingual.GetWord("overlay_position_prefix")}{this.lastRound.Position}{Multilingual.GetWord("overlay_position_suffix")} - {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}"
                                                                                                        : (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{this.lastRound.Position} {Multilingual.GetWord("overlay_position_survived")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                    case LevelType.Logic:
                                    case LevelType.Hunt:
                                    case LevelType.Team:
                                    case LevelType.Invisibeans:
                                        this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{Multilingual.GetWord("overlay_position_qualified")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                    default:
                                        this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"# {Multilingual.GetWord("overlay_position_prefix")}{this.lastRound.Position}{Multilingual.GetWord("overlay_position_suffix")} - {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                }
                            }
                        }

                        if (recordType == BestRecordType.Fastest) {
                            if (time < levelSummary.BestFinish.GetValueOrDefault(TimeSpan.MaxValue) && time > levelSummary.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.LightGreen;
                            } else if (time < levelSummary.BestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.Gold;
                            } else {
                                this.lblFinish.ForeColor = this.ForeColor;
                            }
                        } else if (recordType == BestRecordType.Longest) {
                            if (time > levelSummary.LongestFinish && time < levelSummary.LongestFinishOverall) {
                                this.lblFinish.ForeColor = Color.LightGreen;
                            } else if (time > levelSummary.LongestFinishOverall) {
                                this.lblFinish.ForeColor = Color.Gold;
                            } else {
                                this.lblFinish.ForeColor = this.ForeColor;
                            }
                        } else {
                            this.lblFinish.ForeColor = this.ForeColor;
                        }
                    } else if (end != DateTime.MinValue) {
                        TimeSpan time = end - start;
                        this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Crown) ? $"{Multilingual.GetWord("overlay_position_win")}! {time:m\\:ss\\.fff}" : 
                                                   (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && !(Stats.InShow && !Stats.EndedShow)) ? $"{Multilingual.GetWord("overlay_position_eliminated")}! {time:m\\:ss\\.fff}" :
                                                   $"{time:m\\:ss\\.fff}";
                        this.lblFinish.ForeColor = (Stats.InShow && !Stats.EndedShow) || this.lastRound.Crown ? this.ForeColor : Utils.GetColorBrightnessAdjustment(this.ForeColor, fBrightness);
                    } else if (this.lastRound.Playing) {
                        bool isOverRunningTime = runningTime.TotalMinutes >= maxRunningTime || !Stats.IsGameRunning;
                        this.lblFinish.TextRight = isOverRunningTime ? "-" : $"{runningTime:m\\:ss}";
                        this.lblFinish.ForeColor = isOverRunningTime ? Utils.GetColorBrightnessAdjustment(this.ForeColor, fBrightness) : (!Stats.EndedShow ? this.ForeColor : Utils.GetColorBrightnessAdjustment(this.ForeColor, fBrightness));
                    } else {
                        this.lblFinish.TextRight = "-";
                        this.lblFinish.ForeColor = Stats.InShow && !Stats.EndedShow ? this.ForeColor : Utils.GetColorBrightnessAdjustment(this.ForeColor, fBrightness);
                    }
                }
            }
        }
        
        private void UpdateInfo() {
            if (this.StatsForm == null) { return; }
            
            lock (this.StatsForm.CurrentRound) {
                bool hasCurrentRound = this.StatsForm.CurrentRound != null && this.StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound) {
                    this.lastRound = this.StatsForm.CurrentRound[this.StatsForm.CurrentRound.Count - 1];
                }
                
                this.lblFilter.Text = this.StatsForm.GetCurrentFilterName();
                this.lblProfile.Text = this.StatsForm.GetCurrentProfileName();
                
                this.Background = this.RecreateBackground();
                this.lblProfile.Location = new Point(this.flippedImage ? 125 : this.drawWidth - (145 + this.GetOverlayProfileOffset(this.lblProfile.Text)), 9);
                
                if (this.lastRound != null && !string.IsNullOrEmpty(this.lastRound.Name)) {
                    int overlaySetting = (this.StatsForm.CurrentSettings.HideWinsInfo ? 4 : 0) + (this.StatsForm.CurrentSettings.HideRoundInfo ? 2 : 0) + (this.StatsForm.CurrentSettings.HideTimeInfo ? 1 : 0);
                    bool isRoundInfoNeedRefresh = this.savedRoundNum != this.lastRound.Round ||
                                                  !string.Equals(this.savedSessionId, this.lastRound.SessionId) ||
                                                  Stats.IsOverlayRoundInfoNeedRefresh;
                    if (isRoundInfoNeedRefresh) {
                        if (Stats.IsOverlayRoundInfoNeedRefresh) {
                            Stats.IsOverlayRoundInfoNeedRefresh = false;
                        }
                        this.savedSessionId = this.lastRound.SessionId;
                        this.savedRoundNum = this.lastRound.Round;
                        
                        // this.roundName = this.roundId = this.lastRound.VerifiedName();
                        this.roundId = this.lastRound.VerifiedName();
                        
                        if (this.StatsForm.StatLookup.TryGetValue(this.roundId, out this.levelStats)) {
                            this.roundName = this.lastRound.UseShareCode ? (string.IsNullOrEmpty(this.lastRound.CreativeTitle) ? this.StatsForm.FindCreativeLevelInfo(this.lastRound.ShowNameId) : this.lastRound.CreativeTitle) : this.levelStats.Name.ToUpper();
                        } else if (this.roundId.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                            this.roundName = this.roundId.Substring(6).Replace('_', ' ').ToUpper();
                        } else {
                            this.roundName = this.roundId.Replace('_', ' ').ToUpper();
                        }
                        
                        this.levelType = (this.levelStats?.Type).GetValueOrDefault(LevelType.Unknown);
                        this.recordType = (this.levelStats?.BestRecordType).GetValueOrDefault(BestRecordType.Fastest);
                        this.levelSummary = this.StatsForm.GetLevelInfo(this.lastRound.UseShareCode ? this.lastRound.ShowNameId : this.roundName, this.levelType, this.recordType, this.lastRound.UseShareCode);
                    }
                    
                    this.SetRoundLabel(this.levelStats, this.levelType, this.roundName, overlaySetting);
                    this.SetWinsLabel(this.levelSummary, overlaySetting);
                    this.SetFinalsLabel(this.levelSummary, overlaySetting);
                    this.SetQualifyChanceLabel(this.levelSummary, overlaySetting);
                    this.SetPlayersLabel(overlaySetting);
                    this.SetFastestLabel(this.levelSummary, this.recordType, overlaySetting);
                    this.SetStreakLabel(this.levelSummary, overlaySetting);
                    
                    if (this.isTimeToSwitch) {
                        this.frameCount = 0;
                        this.switchCount = this.switchCount == 0 ? 1 : 0;
                    }
                    
                    DateTime currentUTC = DateTime.UtcNow;
                    if (this.lastRound.Playing != this.startedPlaying) {
                        if (this.lastRound.Playing) {
                            this.startTime = currentUTC;
                        }
                        this.startedPlaying = this.lastRound.Playing;
                    }
                    
                    this.SetFinishLabel(this.levelSummary, this.levelType, this.roundId, this.recordType, currentUTC, overlaySetting);
                    this.SetDurationLabel(this.levelStats, currentUTC, overlaySetting);
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
                        this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[i - 1], EventArgs.Empty);
                        break;
                    } else if (menuItem.Checked && i == 0) {
                        this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count - 1], EventArgs.Empty);
                        break;
                    }
                }
            } else {
                for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                    ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                    if (!(item is ToolStripMenuItem menuItem)) { continue; }
                    if (menuItem.Checked && i + 1 < this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[i + 1], EventArgs.Empty);
                        break;
                    } else if (menuItem.Checked && i + 1 >= this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[0], EventArgs.Empty);
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
            }
            
            switch (e.Control) {
                case true when e.KeyCode == Keys.M:
                    this.StatsForm.CurrentSettings.OverlayNotOnTop = !this.StatsForm.CurrentSettings.OverlayNotOnTop;
                    this.StatsForm.SetOverlayTopMost(!this.StatsForm.CurrentSettings.OverlayNotOnTop);
                    this.StatsForm.SaveUserSettings();
                    break;
                case true when e.Shift && e.KeyCode == Keys.Z:
                    this.StatsForm.SetAutoChangeProfile(!this.StatsForm.CurrentSettings.AutoChangeProfile);
                    break;
                case true when e.Shift && e.KeyCode == Keys.X:
                    this.ResetOverlaySize();
                    break;
                case true when e.Shift && e.KeyCode == Keys.C:
                    this.ResetOverlayLocation(true);
                    break;
                case true when e.Shift && e.KeyCode == Keys.Up:
                    this.StatsForm.SetOverlayBackgroundOpacity(this.StatsForm.CurrentSettings.OverlayBackgroundOpacity + 5);
                    break;
                case true when e.Shift && e.KeyCode == Keys.Down:
                    this.StatsForm.SetOverlayBackgroundOpacity(this.StatsForm.CurrentSettings.OverlayBackgroundOpacity - 5);
                    break;
                case true when e.KeyCode == Keys.O:
                    this.StatsForm.ToggleOverlay(this);
                    break;
                case true when e.KeyCode == Keys.T:
                    int colorOption = 0;
                    if (BackColor.ToArgb() == Color.FromArgb(224, 224, 224).ToArgb()) {
                        colorOption = 1;
                    } else if (BackColor.ToArgb() == Color.White.ToArgb()) {
                        colorOption = 2;
                    } else if (BackColor.ToArgb() == Color.Black.ToArgb()) {
                        colorOption = 3;
                    } else if (BackColor.ToArgb() == Color.Magenta.ToArgb()) {
                        colorOption = 4;
                    } else if (BackColor.ToArgb() == Color.Red.ToArgb()) {
                        colorOption = 5;
                    } else if (BackColor.ToArgb() == Color.Green.ToArgb()) {
                        colorOption = 6;
                    } else if (BackColor.ToArgb() == Color.Blue.ToArgb()) {
                        colorOption = 0;
                    }
                    this.SetBackgroundColor(colorOption);
                    this.StatsForm.CurrentSettings.OverlayColor = colorOption;
                    this.StatsForm.SaveUserSettings();
                    break;
                case true when e.KeyCode == Keys.F:
                    if (!this.IsFixed()) {
                        this.FlipDisplay(!this.flippedImage);
                        this.StatsForm.CurrentSettings.FlippedDisplay = this.flippedImage;
                        this.StatsForm.SaveUserSettings();
                    }
                    break;
                case true when e.KeyCode == Keys.R:
                    this.StatsForm.CurrentSettings.ColorByRoundType = !this.StatsForm.CurrentSettings.ColorByRoundType;
                    this.StatsForm.SaveUserSettings();
                    break;
                case true when e.KeyCode == Keys.C:
                    this.StatsForm.CurrentSettings.PlayerByConsoleType = !this.StatsForm.CurrentSettings.PlayerByConsoleType;
                    this.StatsForm.SaveUserSettings();
                    this.ArrangeDisplay(this.StatsForm.CurrentSettings.FlippedDisplay, this.StatsForm.CurrentSettings.ShowOverlayTabs,
                        this.StatsForm.CurrentSettings.HideWinsInfo, this.StatsForm.CurrentSettings.HideRoundInfo, this.StatsForm.CurrentSettings.HideTimeInfo,
                        this.StatsForm.CurrentSettings.OverlayColor, this.StatsForm.CurrentSettings.OverlayWidth, this.StatsForm.CurrentSettings.OverlayHeight,
                        this.StatsForm.CurrentSettings.OverlayFontSerialized, this.StatsForm.CurrentSettings.OverlayFontColorSerialized);
                    break;
                case false when e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.P):
                    if (this.StatsForm.ProfileMenuItems.Count > 1) {
                        for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                            ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                            if (!(item is ToolStripMenuItem menuItem)) { continue; }
                            if (menuItem.Checked && i > 0) {
                                this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[i - 1], EventArgs.Empty);
                                break;
                            } else if (menuItem.Checked && i == 0) {
                                this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[this.StatsForm.ProfileMenuItems.Count - 1], EventArgs.Empty);
                                break;
                            }
                        }
                    }
                    break;
                case false when (e.Shift && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)) || e.KeyCode == Keys.P:
                    if (this.StatsForm.ProfileMenuItems.Count > 1) {
                        for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                            ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                            if (!(item is ToolStripMenuItem menuItem)) { continue; }
                            if (menuItem.Checked && i + 1 < this.StatsForm.ProfileMenuItems.Count) {
                                this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[i + 1], EventArgs.Empty);
                                break;
                            } else if (menuItem.Checked && i + 1 >= this.StatsForm.ProfileMenuItems.Count) {
                                this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[0], EventArgs.Empty);
                                break;
                            }
                        }
                    }
                    break;
                case false when e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 || e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9:
                    int index = Convert.ToInt32(((char)e.KeyValue).ToString());
                    if (index <= this.StatsForm.ProfileMenuItems.Count) {
                        this.StatsForm.menuStats_Click(this.StatsForm.ProfileMenuItems[index - 1], EventArgs.Empty);
                    }
                    break;
            }
            e.SuppressKeyPress = true;
        }
        
        public void ResetOverlaySize() {
            this.Size = this.DefaultSize;
        }
        
        public void ResetOverlayLocation(bool visible) {
            this.Location = this.StatsForm.screenCenter;
            this.StatsForm.CurrentSettings.OverlayLocationX = this.Location.X;
            this.StatsForm.CurrentSettings.OverlayLocationY = this.Location.Y;
            this.StatsForm.CurrentSettings.OverlayFixedPosition = string.Empty;
            this.StatsForm.CurrentSettings.OverlayFixedPositionX = null;
            this.StatsForm.CurrentSettings.OverlayFixedPositionY = null;
            this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
            this.StatsForm.SaveUserSettings();
            this.Cursor = Cursors.SizeAll;
            this.SetFixedPosition(false, false, false, false, false);
            this.SetBlurPositionMenu();
            this.picPositionLock.Image = this.positionUnlockBlur;
            this.SetVisiblePositionMenu(visible);
            this.SetVisiblePositionLockButton(visible);
        }
        
        public void SetBackgroundColor(int colorOption) {
            switch (colorOption) {
                case 0: this.BackColor = Color.FromArgb(224, 224, 224); break;
                case 1: this.BackColor = Color.White; break;
                case 2: this.BackColor = Color.Black; break;
                case 3: this.BackColor = Color.Magenta; break;
                case 4: this.BackColor = Color.Red; break;
                case 5: this.BackColor = Color.Green; break;
                case 6: this.BackColor = Color.Blue; break;
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
                    this.lblCountryIcon.Location = new Point(thirdColumnX + 101, 9 + heightOffset);
                    this.lblCountryIcon.DrawVisible = true;
                    this.lblPingIcon.Location = new Point(thirdColumnX + 143, 14 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(thirdColumnX + 46, 12 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 20;
                        this.lblPlayersPs.ImageHeight = 18;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(thirdColumnX + 84, 12 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 18;
                        this.lblPlayersXbox.ImageHeight = 18;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(thirdColumnX + 122, 12 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(thirdColumnX + 160, 12 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 18;
                        this.lblPlayersPc.ImageHeight = 19;
                        this.lblPlayersPc.DrawVisible = true;
                    }
                    
                    this.lblDuration.Location = new Point(thirdColumnX, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(thirdColumnX, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 1:
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
                    this.lblCountryIcon.Location = new Point(secondColumnX + 159, 31 + heightOffset);
                    this.lblCountryIcon.DrawVisible = true;
                    this.lblPingIcon.Location = new Point(secondColumnX + 200, 36 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(secondColumnX + 60, 34 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 20;
                        this.lblPlayersPs.ImageHeight = 18;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(secondColumnX + 107, 34 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 18;
                        this.lblPlayersXbox.ImageHeight = 18;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(secondColumnX + 157, 34 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(secondColumnX + 205, 34 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 18;
                        this.lblPlayersPc.ImageHeight = 19;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblQualifyChance.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;
                    break;
                case 2:
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
                    this.lblCountryIcon.DrawVisible = false;
                    this.lblCountryIcon.Location = new Point(999, 999);
                    this.lblPingIcon.DrawVisible = false;
                    this.lblPingIcon.Location = new Point(999, 999);
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersPs.Location = new Point(999, 999);
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersXbox.Location = new Point(999, 999);
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersSwitch.Location = new Point(999, 999);
                    this.lblPlayersPc.DrawVisible = false;
                    this.lblPlayersPc.Location = new Point(999, 999);

                    this.lblFastest.Location = new Point(secondColumnX, 9 + heightOffset);
                    this.lblFastest.Size = new Size(thirdColumnWidth, 22);
                    this.lblFastest.DrawVisible = true;
                    this.lblDuration.Location = new Point(secondColumnX, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(secondColumnX, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 3:
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
                    this.lblCountryIcon.DrawVisible = false;
                    this.lblCountryIcon.Location = new Point(999, 999);
                    this.lblPingIcon.DrawVisible = false;
                    this.lblPingIcon.Location = new Point(999, 999);
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersPs.Location = new Point(999, 999);
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersXbox.Location = new Point(999, 999);
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersSwitch.Location = new Point(999, 999);
                    this.lblPlayersPc.DrawVisible = false;
                    this.lblPlayersPc.Location = new Point(999, 999);

                    this.lblFastest.DrawVisible = false;
                    this.lblDuration.DrawVisible = false;
                    this.lblFinish.DrawVisible = false;
                    break;
                case 4:
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
                    this.lblCountryIcon.Location = new Point(firstColumnX + secondColumnWidth + 108, 8 + heightOffset);
                    this.lblCountryIcon.DrawVisible = true;
                    this.lblPingIcon.Location = new Point(firstColumnX + secondColumnWidth + 149, 13 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + secondColumnWidth + 53, 11 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 20;
                        this.lblPlayersPs.ImageHeight = 18;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + secondColumnWidth + 90, 11 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 18;
                        this.lblPlayersXbox.ImageHeight = 18;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + secondColumnWidth + 127, 11 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + secondColumnWidth + 164, 11 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 18;
                        this.lblPlayersPc.ImageHeight = 19;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblDuration.Location = new Point(firstColumnX + secondColumnWidth + 6, 32 + heightOffset);
                    this.lblDuration.DrawVisible = true;
                    this.lblFinish.Location = new Point(firstColumnX + secondColumnWidth + 6, 55 + heightOffset);
                    this.lblFinish.DrawVisible = true;
                    break;
                case 5:
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
                    this.lblCountryIcon.Location = new Point(firstColumnX + 159, 31 + heightOffset);
                    this.lblCountryIcon.DrawVisible = true;
                    this.lblPingIcon.Location = new Point(firstColumnX + 200, 36 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + 60, 34 + heightOffset);
                        this.lblPlayersPs.Size = new Size(26, 16);
                        this.lblPlayersPs.ImageWidth = 20;
                        this.lblPlayersPs.ImageHeight = 18;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + 107, 34 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(26, 16);
                        this.lblPlayersXbox.ImageWidth = 18;
                        this.lblPlayersXbox.ImageHeight = 18;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + 157, 34 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(26, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + 205, 34 + heightOffset);
                        this.lblPlayersPc.Size = new Size(26, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 18;
                        this.lblPlayersPc.ImageHeight = 19;
                        this.lblPlayersPc.DrawVisible = true;
                    }

                    this.lblQualifyChance.Location = new Point(firstColumnX, 55 + heightOffset);
                    this.lblQualifyChance.DrawVisible = true;

                    this.lblFastest.DrawVisible = false;
                    this.lblDuration.DrawVisible = false;
                    this.lblFinish.DrawVisible = false;
                    break;
                case 6:
                    this.DefaultSize = new Size(251, showTabs ? 134 : 99);

                    this.drawWidth -= firstColumnWidth + secondColumnWidth + (spacerWidth * 2);

                    this.lblWins.DrawVisible = false;
                    this.lblFinals.DrawVisible = false;
                    this.lblStreak.DrawVisible = false;

                    this.lblRound.DrawVisible = false;
                    this.lblQualifyChance.DrawVisible = false;
                    this.lblPlayers.DrawVisible = false;
                    this.lblCountryIcon.DrawVisible = false;
                    this.lblCountryIcon.Location = new Point(999, 999);
                    this.lblPingIcon.DrawVisible = false;
                    this.lblPingIcon.Location = new Point(999, 999);
                    this.lblPlayersPs.DrawVisible = false;
                    this.lblPlayersPs.Location = new Point(999, 999);
                    this.lblPlayersXbox.DrawVisible = false;
                    this.lblPlayersXbox.Location = new Point(999, 999);
                    this.lblPlayersSwitch.DrawVisible = false;
                    this.lblPlayersSwitch.Location = new Point(999, 999);
                    this.lblPlayersPc.DrawVisible = false;
                    this.lblPlayersPc.Location = new Point(999, 999);

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
            
            this.picPositionLock.Image = this.isPositionLock ? this.positionLockBlur : this.positionUnlockBlur;
            this.SetBlurPositionMenu();
            this.SetLocationPositionMenu(showTabs, flipDisplay);

            if (this.IsFixed()) {
                if (this.isFixedPositionSe || this.isFixedPositionSw) {
                    if (this.isFixedPositionSe) {
                        Screen screen = Utils.GetCurrentScreen(this.Location);
                        Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                        Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                        this.Location = new Point(screenLocation.X, screenLocation.Y + screenSize.Height - this.Height);
                    } else if (this.isFixedPositionSw) {
                        Screen screen = Utils.GetCurrentScreen(this.Location);
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
        
        private int GetCountEngLowercase(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (0x61 <= ch && ch <= 0x7A) count++;
            }
            return count;
        }
        
        private int GetCountAmpersand(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (0x26 == ch) count++;
            }
            return count;
        }
        
        public void SetCurrentProfileForeColor(Color color) {
            this.lblProfile.ForeColor = color;
        }
        
        private int GetOverlayProfileOffset(string s) {
            int sizeOfText = TextRenderer.MeasureText(s, new Font(this.lblProfile.Font.FontFamily, this.lblProfile.Font.Size, FontStyle.Regular, this.lblProfile.Font.Unit)).Width;
            return sizeOfText + (this.GetCountAmpersand(s) * 14) - 22;
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
                        if (overlayCustomized) {
                            if (!this.BackgroundResourceName.Equals(this.backgroundResourceNameCache) && File.Exists($"Overlay/{this.BackgroundResourceName}.png")) {
                                this.customizedBackground = new Bitmap($"Overlay/{this.BackgroundResourceName}.png");
                                this.backgroundResourceNameCache = this.BackgroundResourceName;
                            }
                            background = File.Exists($"Overlay/{this.BackgroundResourceName}.png") ? this.customizedBackground : Properties.Resources.background;
                        } else {
                            background = (Bitmap)Properties.Resources.ResourceManager.GetObject(this.BackgroundResourceName) ?? Properties.Resources.background;
                        }
                    }
                    g.DrawImage(background, 0, tabsDisplayed ? 35 : 0);
                    
                    if (tabsDisplayed) {
                        Bitmap tab;
                        if (string.IsNullOrEmpty(this.TabResourceName)) {
                            tab = Properties.Resources.tab_unselected;
                        } else {
                            if (overlayCustomized) {
                                if (!this.TabResourceName.Equals(this.tabResourceNameCache) && File.Exists($"Overlay/{this.TabResourceName}.png")) {
                                    this.customizedTab = new Bitmap($"Overlay/{this.TabResourceName}.png");
                                    this.tabResourceNameCache = this.TabResourceName;
                                }
                                tab = File.Exists($"Overlay/{this.TabResourceName}.png") ? this.customizedTab : Properties.Resources.tab_unselected;
                            } else {
                                tab = (Bitmap)Properties.Resources.ResourceManager.GetObject(this.TabResourceName) ?? Properties.Resources.tab_unselected;
                            }
                        }
                        g.DrawImage(tab, this.drawWidth - 170 - this.GetOverlayProfileOffset(this.lblProfile.Text), 0);
                        g.DrawImage(tab, this.drawWidth - 110, 0);
                    }
                }

                if (this.flippedImage) {
                    newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }

                this.DrawGraphics?.Dispose();
                this.DrawImage?.Dispose();
                this.DrawImage = new Bitmap(newImage.Width, newImage.Height, PixelFormat.Format32bppArgb);
                this.DrawGraphics = Graphics.FromImage(this.DrawImage);

                return newImage;
            }
        }
        
        private void DisplayTabs(bool showTabs) {
            if (showTabs) {
                this.drawHeight = 134;
                this.lblFilter.Location = new Point(this.flippedImage ? -5 : this.drawWidth - 105, 9);
                this.lblFilter.DrawVisible = true;
            } else {
                this.drawHeight = 99;
                this.lblFilter.DrawVisible = false;
            }
        }
        
        private void DisplayProfile(bool showProfile) {
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
            this.SuspendLayout();
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
            this.ResumeLayout();
        }
    }
}