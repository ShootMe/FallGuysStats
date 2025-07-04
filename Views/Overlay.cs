﻿using System;
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
        private string levelId;
        private string levelName;
        private int levelTimeLimit;
        private LevelStats levelStats;
        private LevelType levelType;
        private BestRecordType recordType;
        private Image roundIcon;
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
        private bool isFixedPositionNe, isFixedPositionNw, isFixedPositionSe, isFixedPositionSw, isPositionLock;
        private bool isFocused, isMouseOver;

        private static readonly PrivateFontCollection DefaultFontCollection;
        public static new Font DefaultFont;

        static Overlay() {
            if (!File.Exists($"{Stats.CURRENTDIR}TitanOne-Regular.ttf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.TitanOne-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes($"{Stats.CURRENTDIR}TitanOne-Regular.ttf", fontdata);
                }
            }
            
            if (!File.Exists($"{Stats.CURRENTDIR}NotoSans-Regular.ttf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.NotoSans-Regular.ttf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes($"{Stats.CURRENTDIR}NotoSans-Regular.ttf", fontdata);
                }
            }
            
            if (!File.Exists($"{Stats.CURRENTDIR}NotoSansSC-Regular.otf")) {
                using (Stream fontStream = typeof(Overlay).Assembly.GetManifestResourceStream("FallGuysStats.Resources.font.NotoSansSC-Regular.otf")) {
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int)fontStream.Length);
                    File.WriteAllBytes($"{Stats.CURRENTDIR}NotoSansSC-Regular.otf", fontdata);
                }
            }
            
            DefaultFontCollection = new PrivateFontCollection();
            DefaultFontCollection.AddFontFile($"{Stats.CURRENTDIR}TitanOne-Regular.ttf");
            DefaultFontCollection.AddFontFile($"{Stats.CURRENTDIR}NotoSans-Regular.ttf");
            DefaultFontCollection.AddFontFile($"{Stats.CURRENTDIR}NotoSansSC-Regular.otf");
            SetDefaultFont(18, Stats.CurrentLanguage);
            
            if (!Directory.Exists($"{Stats.CURRENTDIR}Overlay")) {
                Directory.CreateDirectory($"{Stats.CURRENTDIR}Overlay");
                using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.background.png")) {
                    byte[] overlaydata = new byte[overlayStream.Length];
                    overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                    File.WriteAllBytes(Path.Combine($"{Stats.CURRENTDIR}Overlay", "background.png"), overlaydata);
                }
                using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.tab_unselected.png")) {
                    byte[] overlaydata = new byte[overlayStream.Length];
                    overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                    File.WriteAllBytes(Path.Combine($"{Stats.CURRENTDIR}Overlay", "tab.png"), overlaydata);
                }
            } else {
                if (!File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", "background.png"))) {
                    using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.background.png")) {
                        byte[] overlaydata = new byte[overlayStream.Length];
                        overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                        File.WriteAllBytes(Path.Combine($"{Stats.CURRENTDIR}Overlay", "background.png"), overlaydata);
                    }
                }
                if (!File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", "tab.png"))) {
                    using (Stream overlayStream = typeof(Stats).Assembly.GetManifestResourceStream("FallGuysStats.Resources.overlay.tab_unselected.png")) {
                        byte[] overlaydata = new byte[overlayStream.Length];
                        overlayStream.Read(overlaydata, 0, (int)overlayStream.Length);
                        File.WriteAllBytes(Path.Combine($"{Stats.CURRENTDIR}Overlay", "tab.png"), overlaydata);
                    }
                }
            }
        }
        
        public Overlay() {
            this.InitializeComponent();
        }
        
        private void Overlay_Load(object sender, EventArgs e) {
            this.ChangeLanguage();
            // this.SetBackground();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public void ResetBackgroundImage(int width = 0, int height = 0) {
            this.ArrangeDisplay(string.IsNullOrEmpty(this.StatsForm.CurrentSettings.OverlayFixedPosition) ? this.StatsForm.CurrentSettings.FlippedDisplay : this.StatsForm.CurrentSettings.FixedFlippedDisplay, this.StatsForm.CurrentSettings.ShowOverlayTabs,
                this.StatsForm.CurrentSettings.HideWinsInfo, this.StatsForm.CurrentSettings.HideRoundInfo, this.StatsForm.CurrentSettings.HideTimeInfo,
                this.StatsForm.CurrentSettings.OverlayColor, width > 0 ? width : string.IsNullOrEmpty(this.StatsForm.CurrentSettings.OverlayFixedPosition) ? this.StatsForm.CurrentSettings.OverlayWidth : this.StatsForm.CurrentSettings.OverlayFixedWidth, height > 0 ? height : string.IsNullOrEmpty(this.StatsForm.CurrentSettings.OverlayFixedPosition) ? this.StatsForm.CurrentSettings.OverlayHeight : this.StatsForm.CurrentSettings.OverlayFixedHeight,
                this.StatsForm.CurrentSettings.OverlayFontSerialized, this.StatsForm.CurrentSettings.OverlayFontColorSerialized);
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
        
        // private void SetBackground(string backgroundResourceName = null) {
        //     Bitmap background = string.IsNullOrEmpty(backgroundResourceName)
        //         ? Properties.Resources.background
        //         : (Bitmap)Properties.Resources.ResourceManager.GetObject(backgroundResourceName) ?? Properties.Resources.background;
        //     
        //     Bitmap newImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
        //     using (Graphics g = Graphics.FromImage(newImage)) {
        //         g.DrawImage(background, 0, 0);
        //     }
        //     this.Background = newImage;
        //     this.DrawImage = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
        //     this.DrawGraphics = Graphics.FromImage(this.DrawImage);
        //     this.drawWidth = background.Width;
        //     this.drawHeight = background.Height;
        // }
        
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
                if (string.Equals(ctr.Name, "lblProfile")) { ctr.Font = GetMainFont(font.Size, FontStyle.Bold); continue; }
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
                Screen screen = Utils.GetCurrentScreen(this.Location);
                Point screenLocation = screen != null ? screen.Bounds.Location : Screen.PrimaryScreen.Bounds.Location;
                Size screenSize = screen != null ? screen.Bounds.Size : Screen.PrimaryScreen.Bounds.Size;
                if (sender.Equals(this.picPositionNE)) {
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
                        this.Location = new Point(screenLocation.X, 0);
                        this.SetFocusPositionMenu("ne");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "ne";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                        this.SetVisiblePositionLockButton(false);
                    }
                    this.ResetBackgroundImage();
                } else if (sender.Equals(this.picPositionNW)) {
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
                        this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, 0);
                        this.SetFocusPositionMenu("nw");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "nw";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                        this.SetVisiblePositionLockButton(false);
                    }
                    this.ResetBackgroundImage();
                } else if (sender.Equals(this.picPositionSE)) {
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
                        this.Location = new Point(screenLocation.X, screenLocation.Y + screenSize.Height - this.Height);
                        this.SetFocusPositionMenu("se");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "se";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = true;
                        this.SetVisiblePositionLockButton(false);
                    }
                    this.ResetBackgroundImage();
                } else if (sender.Equals(this.picPositionSW)) {
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
                        this.Location = new Point(screenLocation.X + screenSize.Width - this.Width, screenLocation.Y + screenSize.Height - this.Height);
                        this.SetFocusPositionMenu("sw");
                        this.Cursor = Cursors.Default;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionX = this.Location.X;
                        this.StatsForm.CurrentSettings.OverlayFixedPositionY = this.Location.Y;
                        this.StatsForm.CurrentSettings.OverlayFixedWidth = this.Width;
                        this.StatsForm.CurrentSettings.OverlayFixedHeight = this.Height;
                        this.StatsForm.CurrentSettings.OverlayFixedPosition = "sw";
                        this.StatsForm.CurrentSettings.FixedFlippedDisplay = false;
                        this.SetVisiblePositionLockButton(false);
                    }
                    this.ResetBackgroundImage();
                } else if (sender.Equals(this.picPositionLock)) {
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
                    this.ResetBackgroundImage();
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
            this.isFixedPositionNe = string.Equals(flag, "ne");
            this.isFixedPositionNw = string.Equals(flag, "nw");
            this.isFixedPositionSe = string.Equals(flag, "se");
            this.isFixedPositionSw = string.Equals(flag, "sw");
            this.picPositionNE.Image = this.isFixedPositionNe ? this.positionNeOnFocus : this.positionNeOffFocus;
            this.picPositionNW.Image = this.isFixedPositionNw ? this.positionNwOnFocus : this.positionNwOffFocus;
            this.picPositionSE.Image = this.isFixedPositionSe ? this.positionSeOnFocus : this.positionSeOffFocus;
            this.picPositionSW.Image = this.isFixedPositionSw ? this.positionSwOnFocus : this.positionSwOffFocus;
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
        
        private void SetRoundLabel(Image roundIcon, LevelType type, string roundName, int setting) {
            if (!Stats.InShow && Stats.IsQueued && (setting == 1 || setting == 5)) {
                this.lblRound.LevelColor = Color.Empty;
                this.lblRound.LevelTrueColor = Color.Empty;
                this.lblRound.RoundIcon = null;
                this.lblRound.ImageWidth = 0;
                this.lblRound.ImageHeight = 0;
                this.lblRound.Text = $@"{Multilingual.GetWord("overlay_queued_players")} :";
                this.lblRound.TextRight = Stats.QueuedPlayers.ToString();
                this.lblRound.ForeColor = this.ForeColor;
            } else {
                int roundNum = this.lastRound.Round > 0 ? (this.lastRound.IsCasualShow ? (Stats.CasualRoundNum != 0 ? Stats.CasualRoundNum : 1) : this.lastRound.Round) : 0;
                if (this.StatsForm.CurrentSettings.ColorByRoundType) {
                    this.lblRound.Text = $@"{Multilingual.GetWord("overlay_round_abbreviation_prefix")}{roundNum}{Multilingual.GetWord("overlay_round_abbreviation_suffix")} :";
                    this.lblRound.LevelColor = type == LevelType.Unknown ? type.LevelBackColor(false, false, 159) : (this.lastRound.UseShareCode ? type.LevelBackColor(false, this.lastRound.IsTeam, 159) : type.LevelBackColor(this.lastRound.IsFinal, this.lastRound.IsTeam, 223));
                    this.lblRound.LevelTrueColor = type == LevelType.Unknown ? type.LevelBackColor(false, false, 159) : type.LevelBackColor(false, this.lastRound.IsTeam, 159);
                    this.lblRound.RoundIcon = roundIcon;
                    if (this.lblRound.RoundIcon.Height != 23) {
                        float ratio = 23f / this.lblRound.RoundIcon.Height;
                        this.lblRound.ImageHeight = 23;
                        this.lblRound.ImageWidth = (int)(this.lblRound.RoundIcon.Width * ratio);
                    } else {
                        this.lblRound.ImageHeight = this.lblRound.RoundIcon.Height;
                        this.lblRound.ImageWidth = this.lblRound.RoundIcon.Width;
                    }
                } else {
                    this.lblRound.Text = $@"{Multilingual.GetWord("overlay_round_prefix")}{roundNum}{Multilingual.GetWord("overlay_round_suffix")} :";
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
        
        private void SetWinsLabel(StatSummary summary, int setting) {
            if (!Stats.InShow && Stats.IsQueued && setting == 3) {
                this.lblWins.Text = $@"{Multilingual.GetWord("overlay_queued_players")} :";
                this.lblWins.TextRight = $"{Stats.QueuedPlayers:N0}";
                this.lblWins.ForeColor = this.ForeColor;
            } else {
                this.lblWins.Text = $@"{Multilingual.GetWord("overlay_wins")} :";
                float winChance = summary.TotalWins * 100f / Math.Max(1, summary.TotalShows);
                string winChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(winChance * 10) / 10}%";
                this.lblWins.TextRight = this.StatsForm.CurrentSettings.FilterType != 1
                    ? $"{summary.TotalWins:N0}{Multilingual.GetWord("overlay_win")} ({summary.AllWins + this.StatsForm.CurrentSettings.PreviousWins:N0}{Multilingual.GetWord("overlay_win")}){winChanceDisplay}"
                    : $"{summary.TotalWins:N0}{Multilingual.GetWord("overlay_win")}{winChanceDisplay}";
            }
        }
        
        private void SetFinalsLabel(StatSummary summary, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && setting == 3) {
                this.lblFinals.OverlaySetting = setting;
                this.lblFinals.TickProgress = DateTime.Now.Second;
                this.lblFinals.Text = $@"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblFinals.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblFinals.TickProgress = 0;
                this.lblFinals.Text = $@"{Multilingual.GetWord("overlay_finals")} :";
                string finalText = $"{summary.TotalFinals:N0}{(summary.TotalShows < 100000 ? $" / {summary.TotalShows:N0}" : Multilingual.GetWord("overlay_inning"))}";
                float finalChance = summary.TotalFinals * 100f / Math.Max(1, summary.TotalShows);
                string finalChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(finalChance * 10) / 10}%";
                this.lblFinals.TextRight = $"{finalText}{finalChanceDisplay}";
            }
        }
        
        private void SetStreakLabel(StatSummary summary, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (setting == 3)) {
                this.lblStreak.OverlaySetting = setting;
                this.lblStreak.Text = "";
                this.lblStreak.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
            } else {
                int streakSwitchCount = this.switchCount;
                if (!this.StatsForm.CurrentSettings.SwitchBetweenStreaks) {
                    streakSwitchCount = this.StatsForm.CurrentSettings.OnlyShowFinalStreak ? 1 : 0;
                }
                switch (streakSwitchCount % 2) {
                    case 0:
                        this.lblStreak.Text = $@"{Multilingual.GetWord("overlay_streak")} :";
                        string currentStreakSuffix = Stats.CurrentLanguage == Language.Korean ? (summary.CurrentStreak > 1 ? Multilingual.GetWord("overlay_streak_suffix") : Multilingual.GetWord("overlay_win")) : Multilingual.GetWord("overlay_streak_suffix");
                        string bestStreakSuffix = Stats.CurrentLanguage == Language.Korean ? (summary.BestStreak > 1 ? Multilingual.GetWord("overlay_streak_suffix") : Multilingual.GetWord("overlay_win")) : Multilingual.GetWord("overlay_streak_suffix");
                        this.lblStreak.TextRight = $"{summary.CurrentStreak:N0}{currentStreakSuffix} ({Multilingual.GetWord("overlay_best")}{summary.BestStreak:N0}{bestStreakSuffix})";
                        break;
                    case 1:
                        this.lblStreak.Text = $@"{Multilingual.GetWord("overlay_streak_finals")} :";
                        this.lblStreak.TextRight = $"{summary.CurrentFinalStreak:N0}{Multilingual.GetWord("overlay_inning")} ({Multilingual.GetWord("overlay_best")}{summary.BestFinalStreak:N0}{Multilingual.GetWord("overlay_inning")})";
                        break;
                }
            }
        }
        
        private void SetQualifyChanceLabel(StatSummary summary, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (setting == 1 || setting == 5)) {
                this.lblQualifyChance.OverlaySetting = setting;
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
                        this.lblQualifyChance.Text = $@"{Multilingual.GetWord("overlay_qualify_chance")} :";
                        qualifyChance = summary.TotalQualify * 100f / Math.Max(1, summary.TotalPlays);
                        qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(qualifyChance * 10) / 10}%";
                        qualifyDisplay = $"{summary.TotalQualify}{(summary.TotalPlays < 100000 ? $" / {summary.TotalPlays:N0}" : Multilingual.GetWord("overlay_inning"))}";
                        this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                        break;
                    case 1:
                        this.lblQualifyChance.Text = $@"{Multilingual.GetWord("overlay_qualify_gold")} :";
                        qualifyChance = summary.TotalGolds * 100f / Math.Max(1, summary.TotalPlays);
                        qualifyChanceDisplay = this.StatsForm.CurrentSettings.HideOverlayPercentages ? string.Empty : $" - {Math.Truncate(qualifyChance * 10) / 10}%";
                        qualifyDisplay = $"{summary.TotalGolds}{(summary.TotalPlays < 100000 ? $" / {summary.TotalPlays:N0}" : Multilingual.GetWord("overlay_inning"))}";
                        this.lblQualifyChance.TextRight = $"{qualifyDisplay}{qualifyChanceDisplay}";
                        break;
                }
            }
        }
        
        private void SetFastestLabel(StatSummary summary, BestRecordType bestRecordType, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && setting == 2) {
                this.lblFastest.OverlaySetting = setting;
                this.lblFastest.TickProgress = DateTime.Now.Second;
                this.lblFastest.Text = $@"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblFastest.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                if (!Stats.InShow && Stats.IsQueued && setting == 6) {
                    this.lblFastest.Text = $@"{Multilingual.GetWord("overlay_queued_players")} :";
                    this.lblFastest.TextRight = Stats.QueuedPlayers.ToString();
                    this.lblFastest.ForeColor = this.ForeColor;
                } else {
                    this.lblFastest.TickProgress = 0;
                    bool useSwitching = this.StatsForm.CurrentSettings.SwitchBetweenLongest;
                    bool onlyShowLongest = this.StatsForm.CurrentSettings.OnlyShowLongest;
                    switch (bestRecordType) {
                        case BestRecordType.HighScore:
                            this.lblFastest.Text = useSwitching ? $@"{Multilingual.GetWord(this.switchCount == 0 ? "overlay_high_score" : "overlay_low_score")} :"
                                                               : $@"{Multilingual.GetWord(onlyShowLongest ? "overlay_low_score" : "overlay_high_score")} :";
                            this.lblFastest.TextRight = useSwitching ? (this.switchCount == 0 ? (summary.HighScore.HasValue ? $"{summary.HighScore:N0}" : "-") : (summary.LowScore.HasValue ? $"{summary.LowScore:N0}" : "-"))
                                                                    : (onlyShowLongest ? (summary.LowScore.HasValue ? $"{summary.LowScore:N0}" : "-") : (summary.HighScore.HasValue ? $"{summary.HighScore:N0}" : "-"));
                            break;
                        case BestRecordType.Longest:
                            this.lblFastest.Text = useSwitching ? $@"{Multilingual.GetWord(this.switchCount == 0 ? "overlay_longest" : "overlay_fastest")} :"
                                                               : $@"{Multilingual.GetWord(onlyShowLongest ? "overlay_fastest" : "overlay_longest")} :";
                            this.lblFastest.TextRight = useSwitching ? (this.switchCount == 0 ? (summary.LongestFinish.HasValue ? $"{summary.LongestFinish:m\\:ss\\.fff}" : "-") : (summary.FastestFinish.HasValue ? $"{summary.FastestFinish:m\\:ss\\.fff}" : "-"))
                                                                    : (onlyShowLongest ? (summary.FastestFinish.HasValue ? $"{summary.FastestFinish:m\\:ss\\.fff}" : "-") : (summary.LongestFinish.HasValue ? $"{summary.LongestFinish:m\\:ss\\.fff}" : "-"));
                            break;
                        case BestRecordType.Fastest:
                        default:
                            this.lblFastest.Text = useSwitching ? $@"{Multilingual.GetWord(this.switchCount == 0 ? "overlay_fastest" : "overlay_longest")} :"
                                                               : $@"{Multilingual.GetWord(onlyShowLongest ? "overlay_longest" : "overlay_fastest")} :";
                            this.lblFastest.TextRight = useSwitching ? (this.switchCount == 0 ? (summary.FastestFinish.HasValue ? $"{summary.FastestFinish:m\\:ss\\.fff}" : "-") : (summary.LongestFinish.HasValue ? $"{summary.LongestFinish:m\\:ss\\.fff}" : "-"))
                                                                    : (onlyShowLongest ? (summary.LongestFinish.HasValue ? $"{summary.LongestFinish:m\\:ss\\.fff}" : "-") : (summary.FastestFinish.HasValue ? $"{summary.FastestFinish:m\\:ss\\.fff}" : "-"));
                            break;
                            
                    }
                }
            }
        }
        
        private void SetPlayersLabel(LevelType type, BestRecordType bestRecordType, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (setting == 0 || setting == 1 || setting == 4 || setting == 5)) {
                this.lblPlayers.Image = null;
                this.lblPlayersPs.DrawVisible = false;
                this.lblPlayersXbox.DrawVisible = false;
                this.lblPlayersSwitch.DrawVisible = false;
                this.lblPlayersPc.DrawVisible = false;
                this.lblPlayersMobile.DrawVisible = false;
                this.lblCountryIcon.DrawVisible = false;
                this.lblPingIcon.DrawVisible = false;
                this.lblPlayers.OverlaySetting = setting;
                this.lblPlayers.TickProgress = DateTime.Now.Second;
                this.lblPlayers.Text = $@"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblPlayers.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblPlayers.TickProgress = 0;
                int playersSwitchCount = this.switchCount;
                if (!this.StatsForm.CurrentSettings.SwitchBetweenPlayers) {
                    playersSwitchCount = this.StatsForm.CurrentSettings.OnlyShowPing ? 1 : 0;
                }
                switch (playersSwitchCount % 2) {
                    case 0:
                        if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                            this.lblPlayers.Image = Properties.Resources.player_icon;
                            this.lblPlayers.Text = @"ㅤ   :";
                            // this.lblPlayers.TextRight = $"{this.lastRound?.Players}";
                            this.lblPlayers.TextRight = string.Empty;
                            int psCount = this.lastRound.PlayersPs4 + this.lastRound.PlayersPs5;
                            int xbCount = this.lastRound.PlayersXb1 + this.lastRound.PlayersXsx;
                            int swCount = this.lastRound.PlayersSw;
                            int pcCount = this.lastRound.PlayersPc;
                            int mobileCount = this.lastRound.PlayersAndroid + this.lastRound.PlayersIos;
                            if (this.StatsForm.CurrentSettings.CountPlayersDuringTheLevel) {
                                psCount -= (Stats.NumPlayersPsEliminated + Stats.NumPlayersPsSucceeded);
                                xbCount -= (Stats.NumPlayersXbEliminated + Stats.NumPlayersXbSucceeded);
                                swCount -= (Stats.NumPlayersSwEliminated + Stats.NumPlayersSwSucceeded);
                                pcCount -= (Stats.NumPlayersPcEliminated + Stats.NumPlayersPcSucceeded);
                                mobileCount -= (Stats.NumPlayersMbEliminated + Stats.NumPlayersMbSucceeded);
                            }
                            this.lblPlayersPs.TextRight = (psCount == 0 ? "-" : $"{psCount}");
                            this.lblPlayersPs.Size = new Size((psCount > 9 ? 31 : 25), 16);
                            this.lblPlayersPs.DrawVisible = true;
                            this.lblPlayersXbox.TextRight = (xbCount == 0 ? "-" : $"{xbCount}");
                            this.lblPlayersXbox.Size = new Size((xbCount > 9 ? 31 : 25), 16);
                            this.lblPlayersXbox.DrawVisible = true;
                            this.lblPlayersSwitch.TextRight = (swCount == 0 ? "-" : $"{swCount}");
                            this.lblPlayersSwitch.Size = new Size((swCount > 9 ? 31 : 25), 16);
                            this.lblPlayersSwitch.DrawVisible = true;
                            this.lblPlayersPc.TextRight = (pcCount == 0 ? "-" : $"{pcCount}");
                            this.lblPlayersPc.Size = new Size((pcCount > 9 ? 31 : 25), 16);
                            this.lblPlayersPc.DrawVisible = true;
                            this.lblPlayersMobile.TextRight = (mobileCount == 0 ? "-" : $"{mobileCount}");
                            this.lblPlayersMobile.Size = new Size((mobileCount > 9 ? 27 : 21), 16);
                            this.lblPlayersMobile.DrawVisible = true;
                            this.lblCountryIcon.DrawVisible = false;
                            this.lblPingIcon.DrawVisible = false;
                        } else {
                            this.lblPlayers.Image = null;
                            this.lblPlayers.Text = $@"{Multilingual.GetWord("overlay_players")} :";
                            if (this.StatsForm.CurrentSettings.CountPlayersDuringTheLevel) {
                                if (type == LevelType.Survival || type == LevelType.CreativeSurvival) {
                                    this.lblPlayers.TextRight = $"{this.lastRound?.Players - Stats.NumPlayersEliminated}";
                                } else if (type == LevelType.Logic || type == LevelType.CreativeLogic) {
                                    if (bestRecordType == BestRecordType.Fastest) {
                                        this.lblPlayers.TextRight = $"{(Stats.NumPlayersSucceeded > 0 ? Stats.NumPlayersSucceeded + " / " : "")}{this.lastRound?.Players - Stats.NumPlayersEliminated}";
                                    } else if (bestRecordType == BestRecordType.Longest) {
                                        this.lblPlayers.TextRight = $"{this.lastRound?.Players - Stats.NumPlayersEliminated}";
                                    }
                                } else if (type == LevelType.Race || type == LevelType.CreativeRace || type == LevelType.Hunt || type == LevelType.CreativeHunt || type == LevelType.Team || type == LevelType.CreativeTeam) {
                                    this.lblPlayers.TextRight = $"{(Stats.NumPlayersSucceeded > 0 ? Stats.NumPlayersSucceeded + " / " : "")}{this.lastRound?.Players - Stats.NumPlayersEliminated}";
                                } else {
                                    this.lblPlayers.TextRight = $"{(Stats.NumPlayersSucceeded > 0 ? Stats.NumPlayersSucceeded + " / " : "")}{this.lastRound?.Players - Stats.NumPlayersEliminated}";
                                }
                            } else {
                                this.lblPlayers.TextRight = $"{this.lastRound?.Players}";
                            }
                            this.lblPlayersPs.DrawVisible = false;
                            this.lblPlayersXbox.DrawVisible = false;
                            this.lblPlayersSwitch.DrawVisible = false;
                            this.lblPlayersPc.DrawVisible = false;
                            this.lblPlayersMobile.DrawVisible = false;
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
                        this.lblPlayersMobile.DrawVisible = false;
                        this.lblCountryIcon.DrawVisible = true;
                        this.lblPingIcon.DrawVisible = true;
                        if (Stats.IsBadServerPing) {
                            this.lblCountryIcon.ImageX = 50;
                            this.lblCountryIcon.Image = string.IsNullOrEmpty(Stats.LastCountryAlpha2Code) ? null : (Image)Properties.Resources.ResourceManager.GetObject($"country_{Stats.LastCountryAlpha2Code}{(this.StatsForm.CurrentSettings.ShadeTheFlagImage ? "_shiny" : "")}_icon");
                            this.lblPingIcon.ImageX = 50;
                            this.lblPingIcon.Image = Properties.Resources.ping_100_icon;
                            this.lblPingIcon.PingColor = Color.Red;
                        } else {
                            if (Stats.LastServerPing >= 100 && 199 >= Stats.LastServerPing) {
                                this.lblPlayers.PingColor = Color.Orange;
                                this.lblPingIcon.PingColor = Color.Orange;
                            } else if (Stats.LastServerPing >= 200) {
                                this.lblPlayers.PingColor = Color.Red;
                                this.lblPingIcon.PingColor = Color.Red;
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
                                                     : (Stats.IsConnectedToServer && Stats.LastServerPing >= 200 ? Properties.Resources.ping_100_icon : null);
                            this.lblPingIcon.ImageX = (Stats.IsConnectedToServer && Stats.LastServerPing >= 100 && 199 >= Stats.LastServerPing) ? -2
                                                      : (Stats.IsConnectedToServer && Stats.LastServerPing >= 200 && 999 >= Stats.LastServerPing) ? -6
                                                      : (Stats.IsConnectedToServer && Stats.LastServerPing > 999) ? -9 : 0;
                            
                            if (!string.Equals(this.Font.FontFamily.Name, GetDefaultFontFamilies().Name)) {
                                this.lblCountryIcon.ImageX += 7;
                                this.lblPingIcon.ImageX += 6;
                            }
                        }
                        
                        this.lblPlayers.Text = $@"{Multilingual.GetWord("overlay_ping")} :";
                        this.lblPlayers.TextRight = (Stats.IsConnectedToServer && Stats.LastServerPing > 0) ? $"{Stats.LastServerPing} ms" : "-";
                        break;
                }
            }
        }
        
        private void SetDurationLabel(int timeLimit, DateTime currentUtc, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && (setting == 0 || setting == 2 || setting == 4)) {
                this.lblDuration.OverlaySetting = setting;
                this.lblDuration.TickProgress = 0;
                this.lblDuration.Text = "";
                this.lblDuration.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
            } else if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && setting == 6) {
                this.lblDuration.OverlaySetting = setting;
                this.lblDuration.TickProgress = DateTime.Now.Second;
                this.lblDuration.Text = $@"{Multilingual.GetWord("overlay_current_time")} :";
                this.lblDuration.TextRight = $"{DateTime.Now:HH\\:mm\\:ss}";
            } else {
                this.lblDuration.TickProgress = 0;
                this.lblDuration.Text = timeLimit > 0 ? $"{Multilingual.GetWord("overlay_duration")} ({TimeSpan.FromSeconds(timeLimit):m\\:ss}) :" : $"{Multilingual.GetWord("overlay_duration")} :";
                
                DateTime start = this.lastRound.Start;
                DateTime end = this.lastRound.End;
                TimeSpan runningTime = start > currentUtc ? currentUtc - this.startTime : currentUtc - start;
                int maxRunningTime = 30; // in minutes
                
                if (!Stats.IsDisplayOverlayTime) {
                    this.lblDuration.TextRight = "-";
                } else if (Stats.LastPlayedRoundEnd.HasValue && Stats.LastPlayedRoundEnd > Stats.LastPlayedRoundStart) {
                    this.lblDuration.TextRight = $"{Stats.LastPlayedRoundEnd - Stats.LastPlayedRoundStart:m\\:ss\\.fff}";
                } else if (Stats.IsLastPlayedRoundStillPlaying) {
                    bool isOverRunningTime = runningTime.TotalMinutes >= maxRunningTime || !Stats.IsGameRunning;
                    runningTime = timeLimit > 0 ? TimeSpan.FromSeconds(timeLimit) - (currentUtc - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUtc)) : currentUtc - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUtc);
                    bool isExtraTime = timeLimit > 0 && TimeSpan.FromSeconds(timeLimit) < (currentUtc - Stats.LastPlayedRoundStart.GetValueOrDefault(currentUtc));
                    this.lblDuration.TextRight = isOverRunningTime ? "-" : $"{(isExtraTime ? "+ " : "")}{runningTime:m\\:ss}";
                } else if (end != DateTime.MinValue) {
                    TimeSpan time = end - start;
                    bool isExtraTime = timeLimit > 0 && TimeSpan.FromSeconds(timeLimit) < time;
                    this.lblDuration.TextRight = timeLimit > 0 ? $"{(isExtraTime ? "+ " : "")}{TimeSpan.FromSeconds(timeLimit) - time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                } else if (this.lastRound.Playing) {
                    bool isExtraTime = timeLimit > 0 && TimeSpan.FromSeconds(timeLimit) < runningTime;
                    this.lblDuration.TextRight = timeLimit > 0 ? $"{(isExtraTime ? "+ " : "")}{TimeSpan.FromSeconds(timeLimit) - runningTime:m\\:ss}" : $"{runningTime:m\\:ss}";
                } else {
                    this.lblDuration.TextRight = "-";
                }
            }
        }
        
        private void SetFinishLabel(StatSummary summary, LevelType type, string roundId, BestRecordType record, DateTime currentUtc, int setting) {
            if (this.StatsForm.CurrentSettings.DisplayCurrentTime && !Stats.IsConnectedToServer && setting == 6) {
                this.lblFinish.OverlaySetting = setting;
                this.lblFinish.Text = "";
                this.lblFinish.TextRight = $@"{DateTime.Now.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}";
                this.lblFinish.ForeColor = this.ForeColor;
            } else {
                if (!Stats.InShow && Stats.IsQueued && (setting == 0 || setting == 2 || setting == 4)) {
                    this.lblFinish.Text = $@"{Multilingual.GetWord("overlay_queued_players")} :";
                    this.lblFinish.TextRight = Stats.QueuedPlayers.ToString();
                    this.lblFinish.ForeColor = this.ForeColor;
                } else {
                    this.lblFinish.Text = $@"{Multilingual.GetWord("overlay_finish")} :";
                    DateTime start = this.lastRound.Start;
                    DateTime end = this.lastRound.End;
                    DateTime? finish = this.lastRound.Finish;
                    TimeSpan runningTime = start > currentUtc ? currentUtc - this.startTime : currentUtc - start;
                    int maxRunningTime = 30; // in minutes
                    float fBrightness = 0.7f;
                    
                    if (!Stats.IsDisplayOverlayTime) {
                        this.lblFinish.TextRight = "-";
                        this.lblFinish.ForeColor = this.ForeColor;
                    } else if (finish.HasValue) {
                        TimeSpan time = finish.GetValueOrDefault(start) - start;
                        if (this.lastRound.Crown) {
                            this.lblFinish.TextRight = this.StatsForm.CurrentSettings.DisplayGamePlayedInfo ? $"{Multilingual.GetWord("overlay_position_win")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                        } else {
                            if (string.Equals(roundId, "round_skeefall")) { // "Ski Fall" Hunt-like Level Type
                                this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{Multilingual.GetWord("overlay_position_qualified")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                            } else {
                                switch (type) {
                                    case LevelType.Survival:
                                        this.lblFinish.TextRight = record == BestRecordType.Fastest ? (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"# {Multilingual.GetWord("overlay_position_prefix")}{this.lastRound.Position}{Multilingual.GetWord("overlay_position_suffix")} - {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}"
                                                                                                        : (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{this.lastRound.Position} {Multilingual.GetWord("overlay_position_survived")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                    case LevelType.Logic:
                                    case LevelType.Hunt:
                                    case LevelType.Team:
                                    case LevelType.Invisibeans:
                                        this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"{Multilingual.GetWord("overlay_position_qualified")}! {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                    case LevelType.Race:
                                    default:
                                        this.lblFinish.TextRight = (this.StatsForm.CurrentSettings.DisplayGamePlayedInfo && this.lastRound.Position > 0) ? $"# {Multilingual.GetWord("overlay_position_prefix")}{this.lastRound.Position}{Multilingual.GetWord("overlay_position_suffix")} - {time:m\\:ss\\.fff}" : $"{time:m\\:ss\\.fff}";
                                        break;
                                }
                            }
                        }

                        if (record == BestRecordType.Fastest) {
                            if (time < summary.FastestFinish.GetValueOrDefault(TimeSpan.MaxValue) && time > summary.FastestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.LightGreen;
                            } else if (time < summary.FastestFinishOverall.GetValueOrDefault(TimeSpan.MaxValue)) {
                                this.lblFinish.ForeColor = Color.Gold;
                            } else {
                                this.lblFinish.ForeColor = this.ForeColor;
                            }
                        } else if (record == BestRecordType.Longest) {
                            if (time > summary.LongestFinish && time < summary.LongestFinishOverall) {
                                this.lblFinish.ForeColor = Color.LightGreen;
                            } else if (time > summary.LongestFinishOverall) {
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
            if (this.StatsForm == null) return;
            
            lock (this.StatsForm.CurrentRound) {
                bool hasCurrentRound = this.StatsForm.CurrentRound != null && this.StatsForm.CurrentRound.Count > 0;
                if (hasCurrentRound) {
                    this.lastRound = this.StatsForm.CurrentRound[this.StatsForm.CurrentRound.Count - 1];
                }
                
                this.lblFilter.Text = this.StatsForm.GetCurrentFilterName();
                this.lblProfile.Text = this.StatsForm.GetCurrentProfileName();
                
                if (this.lastRound != null && !string.IsNullOrEmpty(this.lastRound.Name)) {
                    bool isRoundInfoNeedRefresh = (string.Equals(this.savedSessionId, this.lastRound.SessionId) && this.savedRoundNum != this.lastRound.Round)
                                                  || !string.Equals(this.savedSessionId, this.lastRound.SessionId)
                                                  || this.lastRound.Round == -1
                                                  || Stats.IsOverlayRoundInfoNeedRefresh;
                    if (isRoundInfoNeedRefresh) {
                        if (Stats.IsOverlayRoundInfoNeedRefresh) {
                            Stats.IsOverlayRoundInfoNeedRefresh = false;
                        }
                        this.savedSessionId = this.lastRound.SessionId;
                        this.savedRoundNum = this.lastRound.Round;
                        
                        this.levelId = this.lastRound.VerifiedName();
                        
                        if (this.StatsForm.StatLookup.TryGetValue(this.levelId, out this.levelStats)) {
                            this.levelName = this.levelStats.Name.ToUpper();
                        } else if (this.levelId.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                            this.levelName = this.levelId.Substring(6).Replace('_', ' ').ToUpper();
                        } else if (this.lastRound.UseShareCode && this.StatsForm.StatLookup.TryGetValue(this.lastRound.ShowNameId, out this.levelStats)) {
                            this.levelName = string.IsNullOrEmpty(this.lastRound.CreativeTitle) ? this.StatsForm.GetUserCreativeLevelTitle(this.levelId) : this.lastRound.CreativeTitle;
                        } else {
                            this.levelName = this.levelId.Replace('_', ' ').ToUpper();
                        }
                        
                        this.levelType = (this.levelStats?.Type).GetValueOrDefault(LevelType.Unknown);
                        this.recordType = (this.levelStats?.BestRecordType).GetValueOrDefault(BestRecordType.Fastest);
                        this.roundIcon = this.levelType != LevelType.Unknown ? this.levelStats.RoundBigIcon : Properties.Resources.round_unknown_big_icon;
                        this.levelSummary = this.StatsForm.GetLevelInfo(this.levelId, this.levelType, this.recordType, this.lastRound.UseShareCode);
                    }
                    
                    int overlaySetting = this.StatsForm.GetOverlaySetting();
                    
                    this.SetWinsLabel(this.levelSummary, overlaySetting);
                    this.SetFinalsLabel(this.levelSummary, overlaySetting);
                    this.SetStreakLabel(this.levelSummary, overlaySetting);
                    this.SetRoundLabel(this.roundIcon, this.levelType, this.levelName, overlaySetting);
                    this.SetQualifyChanceLabel(this.levelSummary, overlaySetting);
                    this.SetFastestLabel(this.levelSummary, this.recordType, overlaySetting);
                    this.SetPlayersLabel(this.levelType, this.recordType, overlaySetting);
                    
                    if (this.isTimeToSwitch) {
                        this.frameCount = 0;
                        this.switchCount ^= 1;
                    }
                    
                    DateTime currentUtc = DateTime.UtcNow;
                    if (this.lastRound.Playing != this.startedPlaying) {
                        if (this.lastRound.Playing) {
                            this.startTime = currentUtc;
                        }
                        this.startedPlaying = this.lastRound.Playing;
                    }
                    
                    this.levelTimeLimit = this.lastRound.IsCasualShow || this.lastRound.UseShareCode ? this.lastRound.CreativeTimeLimitSeconds :
                                          this.StatsForm.LevelTimeLimitCache.Find(l => string.Equals(l.LevelId, this.lastRound.RoundId ?? this.lastRound.Name))?.Duration ?? 0;
                    
                    this.SetDurationLabel(this.levelTimeLimit, currentUtc, overlaySetting);
                    this.SetFinishLabel(this.levelSummary, this.levelType, this.levelId, this.recordType, currentUtc, overlaySetting);
                }
                this.Invalidate();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e) {
            lock (DefaultFont) {
                this.DrawGraphics.Clear(Color.Transparent);
                this.DrawGraphics.DrawImage(this.Background, 0, 0);

                foreach (Control control in this.Controls) {
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
            if (this.shiftKeyToggle == false) return;
            if (this.StatsForm.ProfileMenuItems.Count <= 1) return;
            if ((e.Delta / 120) > 0) {
                for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                    ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                    if (!(item is ToolStripMenuItem menuItem)) continue;
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
                    if (!(item is ToolStripMenuItem menuItem)) continue;
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
                    this.ResetOverlaySize();
                    break;
                case true when e.KeyCode == Keys.F:
                    if (!this.IsFixed()) {
                        this.FlipDisplay(!this.flippedImage);
                        this.StatsForm.CurrentSettings.FlippedDisplay = this.flippedImage;
                        this.StatsForm.SaveUserSettings();
                        this.ResetOverlaySize();
                    }
                    break;
                case true when e.KeyCode == Keys.R:
                    this.StatsForm.CurrentSettings.ColorByRoundType = !this.StatsForm.CurrentSettings.ColorByRoundType;
                    this.StatsForm.SaveUserSettings();
                    this.ResetBackgroundImage();
                    break;
                case true when e.KeyCode == Keys.C:
                    this.StatsForm.CurrentSettings.PlayerByConsoleType = !this.StatsForm.CurrentSettings.PlayerByConsoleType;
                    this.StatsForm.SaveUserSettings();
                    this.ResetBackgroundImage();
                    break;
                case false when e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.P):
                    if (this.StatsForm.ProfileMenuItems.Count > 1) {
                        for (int i = 0; i < this.StatsForm.ProfileMenuItems.Count; i++) {
                            ToolStripItem item = this.StatsForm.ProfileMenuItems[i];
                            if (!(item is ToolStripMenuItem menuItem)) continue;
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
                            if (!(item is ToolStripMenuItem menuItem)) continue;
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
            this.ResetBackgroundImage(this.DefaultSize.Width, this.DefaultSize.Height);
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
            this.ResetBackgroundImage();
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
                    this.lblPingIcon.Location = new Point(thirdColumnX + 134, 14 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(thirdColumnX + 49, 12 + heightOffset);
                        this.lblPlayersPs.Size = new Size(25, 16);
                        this.lblPlayersPs.ImageWidth = 19;
                        this.lblPlayersPs.ImageHeight = 17;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(thirdColumnX + 86, 12 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(25, 16);
                        this.lblPlayersXbox.ImageWidth = 17;
                        this.lblPlayersXbox.ImageHeight = 17;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(thirdColumnX + 123, 12 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(25, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(thirdColumnX + 160, 12 + heightOffset);
                        this.lblPlayersPc.Size = new Size(25, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 17;
                        this.lblPlayersPc.ImageHeight = 18;
                        this.lblPlayersPc.DrawVisible = true;
                        
                        this.lblPlayersMobile.Location = new Point(thirdColumnX + 198, 12 + heightOffset);
                        this.lblPlayersMobile.Size = new Size(25, 16);
                        this.lblPlayersMobile.ImageY = -1;
                        this.lblPlayersMobile.ImageWidth = 11;
                        this.lblPlayersMobile.ImageHeight = 18;
                        this.lblPlayersMobile.DrawVisible = true;
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
                    this.lblPingIcon.Location = new Point(secondColumnX + 191, 36 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(secondColumnX + 56, 34 + heightOffset);
                        this.lblPlayersPs.Size = new Size(25, 16);
                        this.lblPlayersPs.ImageWidth = 19;
                        this.lblPlayersPs.ImageHeight = 17;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(secondColumnX + 100, 34 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(25, 16);
                        this.lblPlayersXbox.ImageWidth = 17;
                        this.lblPlayersXbox.ImageHeight = 17;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(secondColumnX + 147, 34 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(25, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(secondColumnX + 194, 34 + heightOffset);
                        this.lblPlayersPc.Size = new Size(25, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 17;
                        this.lblPlayersPc.ImageHeight = 18;
                        this.lblPlayersPc.DrawVisible = true;
                        
                        this.lblPlayersMobile.Location = new Point(secondColumnX + 241, 34 + heightOffset);
                        this.lblPlayersMobile.Size = new Size(25, 16);
                        this.lblPlayersMobile.ImageY = -1;
                        this.lblPlayersMobile.ImageWidth = 11;
                        this.lblPlayersMobile.ImageHeight = 18;
                        this.lblPlayersMobile.DrawVisible = true;
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
                    this.lblPlayersMobile.DrawVisible = false;
                    this.lblPlayersMobile.Location = new Point(999, 999);

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
                    this.lblPlayersMobile.DrawVisible = false;
                    this.lblPlayersMobile.Location = new Point(999, 999);

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
                    this.lblPingIcon.Location = new Point(firstColumnX + secondColumnWidth + 140, 13 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + secondColumnWidth + 55, 11 + heightOffset);
                        this.lblPlayersPs.Size = new Size(25, 16);
                        this.lblPlayersPs.ImageWidth = 19;
                        this.lblPlayersPs.ImageHeight = 17;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + secondColumnWidth + 92, 11 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(25, 16);
                        this.lblPlayersXbox.ImageWidth = 17;
                        this.lblPlayersXbox.ImageHeight = 17;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + secondColumnWidth + 129, 11 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(25, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + secondColumnWidth + 166, 11 + heightOffset);
                        this.lblPlayersPc.Size = new Size(25, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 17;
                        this.lblPlayersPc.ImageHeight = 18;
                        this.lblPlayersPc.DrawVisible = true;
                        
                        this.lblPlayersMobile.Location = new Point(firstColumnX + secondColumnWidth + 204, 11 + heightOffset);
                        this.lblPlayersMobile.Size = new Size(25, 16);
                        this.lblPlayersMobile.ImageY = -1;
                        this.lblPlayersMobile.ImageWidth = 11;
                        this.lblPlayersMobile.ImageHeight = 18;
                        this.lblPlayersMobile.DrawVisible = true;
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
                    this.lblPingIcon.Location = new Point(firstColumnX + 191, 36 + heightOffset);
                    this.lblPingIcon.DrawVisible = true;

                    if (this.StatsForm.CurrentSettings.PlayerByConsoleType) {
                        this.lblPlayersPs.Location = new Point(firstColumnX + 56, 34 + heightOffset);
                        this.lblPlayersPs.Size = new Size(25, 16);
                        this.lblPlayersPs.ImageWidth = 19;
                        this.lblPlayersPs.ImageHeight = 17;
                        this.lblPlayersPs.DrawVisible = true;
                    
                        this.lblPlayersXbox.Location = new Point(firstColumnX + 100, 34 + heightOffset);
                        this.lblPlayersXbox.Size = new Size(25, 16);
                        this.lblPlayersXbox.ImageWidth = 17;
                        this.lblPlayersXbox.ImageHeight = 17;
                        this.lblPlayersXbox.DrawVisible = true;
                    
                        this.lblPlayersSwitch.Location = new Point(firstColumnX + 147, 34 + heightOffset);
                        this.lblPlayersSwitch.Size = new Size(25, 16);
                        this.lblPlayersSwitch.ImageWidth = 17;
                        this.lblPlayersSwitch.ImageHeight = 17;
                        this.lblPlayersSwitch.DrawVisible = true;
                    
                        this.lblPlayersPc.Location = new Point(firstColumnX + 194, 34 + heightOffset);
                        this.lblPlayersPc.Size = new Size(25, 16);
                        this.lblPlayersPc.ImageY = -1;
                        this.lblPlayersPc.ImageWidth = 17;
                        this.lblPlayersPc.ImageHeight = 18;
                        this.lblPlayersPc.DrawVisible = true;
                        
                        this.lblPlayersMobile.Location = new Point(firstColumnX + 241, 34 + heightOffset);
                        this.lblPlayersMobile.Size = new Size(25, 16);
                        this.lblPlayersMobile.ImageY = -1;
                        this.lblPlayersMobile.ImageWidth = 11;
                        this.lblPlayersMobile.ImageHeight = 18;
                        this.lblPlayersMobile.DrawVisible = true;
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
                    this.lblPlayersMobile.DrawVisible = false;
                    this.lblPlayersMobile.Location = new Point(999, 999);

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
            
            this.Background = this.RecreateBackground();
        }
        
        public void FlipDisplay(bool flipped) {
            if (flipped == this.flippedImage) return;

            this.flippedImage = flipped;

            foreach (Control ctr in Controls) {
                if (ctr is TransparentLabel label && !label.Equals(this.lblFilter) && !label.Equals(this.lblProfile)) {
                    label.Location = new Point(label.Location.X + (this.flippedImage ? -18 : 18), label.Location.Y);
                }
            }

            // this.DisplayTabs(this.drawHeight > 99);
            // this.DisplayProfile(this.drawHeight > 99);
            this.DisplayTabs(this.StatsForm.CurrentSettings.ShowOverlayTabs);
            this.DisplayProfile(this.StatsForm.CurrentSettings.ShowOverlayTabs);
            this.picPositionLock.Location = new Point(flipped ? (this.Width - this.picPositionLock.Width - 14) : 14, (this.Height / 2) - (this.picPositionLock.Size.Height + 6) + (this.StatsForm.CurrentSettings.ShowOverlayTabs ? 11 : -6));
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
                bool overlayCustomized = this.StatsForm.CurrentSettings.IsOverlayBackgroundCustomized;
                Bitmap newImage = new Bitmap(this.drawWidth, this.drawHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage)) {
                    Bitmap background;
                    if (string.IsNullOrEmpty(this.BackgroundResourceName)) {
                        background = Properties.Resources.background;
                    } else {
                        if (overlayCustomized) {
                            if (!string.Equals(this.BackgroundResourceName, this.backgroundResourceNameCache) && File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.BackgroundResourceName}.png"))) {
                                this.customizedBackground = new Bitmap(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.BackgroundResourceName}.png"));
                                this.backgroundResourceNameCache = this.BackgroundResourceName;
                            }
                            background = File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.BackgroundResourceName}.png")) ? this.customizedBackground : Properties.Resources.background;
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
                                if (!string.Equals(this.TabResourceName, this.tabResourceNameCache) && File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.TabResourceName}.png"))) {
                                    this.customizedTab = new Bitmap(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.TabResourceName}.png"));
                                    this.tabResourceNameCache = this.TabResourceName;
                                }
                                tab = File.Exists(Path.Combine($"{Stats.CURRENTDIR}Overlay", $"{this.TabResourceName}.png")) ? this.customizedTab : Properties.Resources.tab_unselected;
                            } else {
                                tab = (Bitmap)Properties.Resources.ResourceManager.GetObject(this.TabResourceName) ?? Properties.Resources.tab_unselected;
                            }
                        }
                        g.DrawImage(tab, this.drawWidth - 170 - this.GetOverlayProfileOffset(this.StatsForm.GetCurrentProfileName()), 0);
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
                this.lblProfile.Location = new Point(this.flippedImage ? 125 : this.drawWidth - (145 + this.GetOverlayProfileOffset(this.StatsForm.GetCurrentProfileName())), 9);
                this.lblProfile.DrawVisible = true;
            } else {
                this.drawHeight = 99;
                this.lblProfile.DrawVisible = false;
            }
        }
        
        public void ChangeLanguage() {
            this.SuspendLayout();
            this.lblStreak.Text = $@"{Multilingual.GetWord("overlay_streak")} :";
            this.lblStreak.TextRight = $"0{Multilingual.GetWord("overlay_streak_suffix")} ({Multilingual.GetWord("overlay_best")} 0{Multilingual.GetWord("overlay_streak_suffix")})";
            this.lblFinals.Text = $@"{Multilingual.GetWord("overlay_finals")} :";
            this.lblFinals.TextRight = $"0{Multilingual.GetWord("overlay_inning")} - 0.0%";
            this.lblQualifyChance.Text = $@"{Multilingual.GetWord("overlay_qualify_chance")} :";
            this.lblFastest.Text = $@"{Multilingual.GetWord("overlay_fastest")} :";
            this.lblDuration.Text = $@"{Multilingual.GetWord("overlay_duration")} :";
            this.lblRound.Text = $@"{Multilingual.GetWord("overlay_round_prefix")}1{Multilingual.GetWord("overlay_round_suffix")} :";
            this.lblWins.Text = $@"{Multilingual.GetWord("overlay_wins")} :";
            this.lblWins.TextRight = $"0{Multilingual.GetWord("overlay_inning")} - 0.0%";
            this.lblFinish.Text = $@"{Multilingual.GetWord("overlay_finish")} :";
            this.lblPlayers.Text = $@"{Multilingual.GetWord("overlay_players")} :";
            this.ResumeLayout();
        }
    }
}