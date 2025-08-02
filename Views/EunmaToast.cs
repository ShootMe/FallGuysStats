using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace FallGuysStats {
	public partial class EunmaToast : Form {
        private readonly int _horizontalMargin;

		private readonly int _verticalMargin;

		private bool _shown;

		private bool _isMuted = false;

		private ToastAnimation _toastAnimation;

		private const int AW_SLIDE = 0X40000;

		private const int AW_HOR_POSITIVE = 0X1;

		private const int AW_HOR_NEGATIVE = 0X2;

		private const int AW_HIDE = 0x00010000;

		private const int AW_ACTIVATE = 0x00020000;

		private const int AW_BLEND = 0X80000;

		private const int AW_CENTER = 0x00000010;

		private byte _counter = 2;
        
		private byte _transitionCounter;
        
		private int _maxTransition;
        
        private int maxTransition {
            get => _maxTransition;
            set {
                if (_maxTransition != value) {
                    _maxTransition = value;
                    lblProgress.Width = (int)(_deltaTransition * _maxTransition);
                }
            }
        }
        
		private double _deltaTransition;
		
		private ToastDuration _toastDuration;
		
		private ToastSound _toastSound;

		private Font _toastFont;

		private Toast _toast;

		internal CancellationToken CancellationToken { get; set; }

		public bool IsAsync = false;

		public ToastTheme ToastTheme;
		
		public ToastCloseStyle ToastCloseStyle;
		
		private DWM_WINDOW_CORNER_PREFERENCE windowConerPreference_default = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;
		private DWM_WINDOW_CORNER_PREFERENCE windowConerPreference_roundsmall = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;

		internal EunmaToast() {
			InitializeComponent();
			
			_horizontalMargin = 10;
			_verticalMargin = 10;
			
			var workingArea = Screen.GetWorkingArea(this);

			Location = new Point(workingArea.Right - Size.Width - _horizontalMargin,
				workingArea.Bottom - Size.Height - _verticalMargin);
		}

		internal Toast Toast {
			get => _toast;
			set => _toast = value;
		}

		internal bool IsShown => _shown;
		
		[DefaultValue(ToastDuration.VERY_SHORT)]
		internal ToastDuration ToastDuration {
			get => _toastDuration;
			set => _toastDuration = value;
		}

		[DefaultValue(ToastAnimation.FADE)]
		internal ToastAnimation ToastAnimation {
			get => _toastAnimation;
			set => _toastAnimation = value;
		}

		[DefaultValue("")]
		internal string Caption {
			get => lblCaption.Text;
			set => lblCaption.Text = value?.Trim() ?? string.Empty;
		}

		[DefaultValue("")]
		internal string Description {
			get => lblDescription.Text;
			set => lblDescription.Text = value?.Trim() ?? string.Empty;

		}
		
		[DefaultValue(null)]
		internal Font ToastFont {
			get => _toastFont;
			set {
				_toastFont = value;
				if (value != null) {
					lblCaption.Font = value;
					lblDescription.Font = new Font(value.FontFamily, value.Size * 0.81f, FontStyle.Regular, value.Unit);
				}
			}
		}
		
		[DefaultValue(ToastSound.Generic01)]
		internal ToastSound ToastSound {
			get => _toastSound;
			set => _toastSound = value;
		}

		internal Image Thumbnails {
			get => picImage.Image;
			set {
				picImage.Image = value;
				Invalidate();
				if (value != null) {
					picImage.Visible = true;
					HasImage = true;
				} else {
					picImage.Visible = false;
				}
			} 
		}
		
		internal Image AppOwnerIcon {
			get => picAppOwnerIcon.Image;
			set {
				picAppOwnerIcon.Image = value;
				picAppOwnerIcon.Visible = value != null;
				Invalidate();
			} 
		}

        public bool HasImage { get; private set; }

        [DefaultValue(false)]
		internal bool IsMuted {
			get => _isMuted;
			set => _isMuted = value;
		}

		internal int HorizontalMargin => _horizontalMargin;
		internal int VerticalMargin => _verticalMargin;

		private async void FrmToast_Load(object sender, EventArgs e) {
			if (IsAsync) {
				await Task.Yield();
			}
			
			Utils.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference_roundsmall, sizeof(uint));

			switch (ToastCloseStyle) {
				case ToastCloseStyle.ClickEntire:
					textContainer.Panel1Collapsed = true;
					break;
				case ToastCloseStyle.Button:
					textContainer.Panel1Collapsed = false;
					break;
				case ToastCloseStyle.ButtonAndClickEntire:
					textContainer.Panel1Collapsed = false;
					break;
			}

			// switch (_toastDuration) {
			// 	case ToastDuration.LENGTH_SHORT:
			// 		_counter = 2;
			// 		break;
			// 	case ToastDuration.LENGTH_MEDIUM:
			// 		_counter = 3;
			// 		break;
			// 	case ToastDuration.LENGTH_LONG:
			// 		_counter = 4;
			// 		break;
			// }

            _counter = (byte)_toastDuration;
            _deltaTransition = (double)lblDescription.Width / ((byte)_toastDuration * 100);
			
			if (!_isMuted) {
				PlaySound();
			}
            
			SetTheme();
            
			switch (_toastAnimation) {
				case ToastAnimation.FADE:
					FadeIn();
					break;
				case ToastAnimation.SLIDE:
                    // Utils.AnimateWindow(Handle, 100, AW_SLIDE | AW_HOR_NEGATIVE | AW_ACTIVATE);
                    FadeIn();
					break;
			}
        }

		private async void PlaySound() {
			Stream sound;
			switch (_toastSound) {
				case ToastSound.Generic02:
					sound = Properties.Resources.notify_sound_02;
					break;
				case ToastSound.Generic03:
					sound = Properties.Resources.notify_sound_03;
					break;
				case ToastSound.Generic04:
					sound = Properties.Resources.notify_sound_04;
					break;
				default:
					sound = Properties.Resources.notify_sound_01;
					break;
			}
			sound.Position = 0;
			
			await Task.Run(() => {
				var player = new SoundPlayer(sound);
				player.Play();
			}, CancellationToken);
		}

		private void SetTheme() {
			switch (ToastTheme) {
				case ToastTheme.Light:
                    lblProgress.BackColor = Color.Red;
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.LightScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.Black;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.LightScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.LightScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.LightScheme.GetBackgroundColor();
					break;
				case ToastTheme.PrimaryLight:
                    lblProgress.BackColor = Color.Red;
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.PrimaryLightScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.PrimaryLightScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.PrimaryLightScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.PrimaryLightScheme.GetBackgroundColor();
					break;
				case ToastTheme.SuccessLight:
                    lblProgress.BackColor = Color.Red;
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.SuccessLightScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.SuccessLightScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.SuccessLightScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.SuccessLightScheme.GetBackgroundColor();
					break;
				case ToastTheme.WarningLight:
                    lblProgress.BackColor = Color.Red;
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.WarningLightScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.WarningLightScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.WarningLightScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.WarningLightScheme.GetBackgroundColor();
					break;
				case ToastTheme.ErrorLight:
                    lblProgress.BackColor = Color.Red;
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.ErrorLightScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.ErrorLightScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.ErrorLightScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.ErrorLightScheme.GetBackgroundColor();
					break;
				case ToastTheme.Dark:
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.DarkScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.DarkScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.DarkScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.DarkScheme.GetBackgroundColor();
					break;
				case ToastTheme.PrimaryDark:
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.PrimaryDarkScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.PrimaryDarkScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.PrimaryDarkScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.PrimaryDarkScheme.GetBackgroundColor();
					break;
				case ToastTheme.SuccessDark:
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.SuccessDarkScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.SuccessDarkScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.SuccessDarkScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.SuccessDarkScheme.GetBackgroundColor();
					break;
				case ToastTheme.WarningDark:
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.WarningDarkScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.WarningDarkScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.WarningDarkScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.WarningDarkScheme.GetBackgroundColor();
					break;
				case ToastTheme.ErrorDark:
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
					lblCaption.ForeColor = ToastThemeBuilder.BuiltinScheme.ErrorDarkScheme.GetForegroundColor();
					lblDescription.ForeColor = Color.White;
					btnClose.ForeColor = ToastThemeBuilder.BuiltinScheme.ErrorDarkScheme.GetForegroundColor();
					BackColor = ToastThemeBuilder.BuiltinScheme.ErrorDarkScheme.GetBackgroundColor();
					btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.BuiltinScheme.ErrorDarkScheme.GetBackgroundColor();
					break;
				case ToastTheme.Custom:
					if (ToastThemeBuilder.CustomScheme == null) {
						throw new NullReferenceException($"You must create your scheme before set custom theme. Use ${nameof(ToastThemeBuilder.CreateCustomScheme)}() to create a custom scheme");
					}
                    lblProgress.BackColor = Color.FromArgb(99, 230, 190);
                    lblCaption.ForeColor = ToastThemeBuilder.CustomScheme.GetForegroundColor();
                    btnClose.ForeColor = ToastThemeBuilder.CustomScheme.GetForegroundColor();
                    BackColor = ToastThemeBuilder.CustomScheme.GetBackgroundColor();
                    btnClose.FlatAppearance.BorderColor = ToastThemeBuilder.CustomScheme.GetBackgroundColor();
                    break;
			}
		}

		private void FrmToast_Shown(object sender, EventArgs e) {
			_shown = true;
			tmrClose.Start();
		}

		private void FrmToast_FormClosing(object sender, FormClosingEventArgs e) {
			switch (_toastAnimation) {
				case ToastAnimation.FADE:
                    Utils.AnimateWindow(Handle, 150, AW_BLEND | AW_HIDE);
					break;
				case ToastAnimation.SLIDE:
                    Utils.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref windowConerPreference_default, sizeof(uint));
                    Utils.AnimateWindow(Handle, 150, AW_SLIDE | AW_HOR_POSITIVE | AW_HIDE);
					break;
			}
		}

		private void ToastContentClick(object sender, MouseEventArgs e) {
			switch (ToastCloseStyle) {
				case ToastCloseStyle.ClickEntire:
				case ToastCloseStyle.ButtonAndClickEntire:
                    if (e.Button == MouseButtons.Left) {
                        tmrClose.Stop();
					    Close();
                    }
					break;
				case ToastCloseStyle.Button:
					return;
			}
		}

        private void TmrClose_Tick(object sender, EventArgs e) {
            _transitionCounter++;
            maxTransition++;

            if (_transitionCounter % 5 == 0) {
                this.Thumbnails = (Image)Properties.Resources.ResourceManager.GetObject($"loading_{((_transitionCounter / 5) - 1) % 10 + 1}");
            }

            if (_transitionCounter >= 100) {
                _transitionCounter = 0;
                _counter--;
            }

            if (_counter == 0) {
                tmrClose.Stop();
                Close();
            }
        }

		private async void FadeIn() {
			Opacity = 0;
			while (Opacity < 1.0) {
				await Task.Delay(3, CancellationToken);
				Opacity += 0.1;
			}
			Opacity = 1;
		}

		private SizeF CalculateString() {
			if (string.IsNullOrEmpty(lblCaption.Text)) return SizeF.Empty;
			using (var g = CreateGraphics()) {
				var size = g.MeasureString(lblCaption.Text, lblCaption.Font);
				return size;
			}
		}
        
        private void FrmToast_FormEnter(object sender, EventArgs e) {
            tmrClose.Stop();
        }
        
        private void FrmToast_FormLeave(object sender, EventArgs e) {
            tmrClose.Start();
        }

		private void FrmToast_FormClosed(object sender, FormClosedEventArgs e) {
			ToastManager.ToastCollection.Remove(_toast);
		}

		private void BtnClose_Click(object sender, EventArgs e) {
            tmrClose.Stop();
			Close();
		}
	}
}
