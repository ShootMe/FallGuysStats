using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
	public class Toast {
		#region Private fields

		private static IWin32Window _window;
        internal readonly EunmaToast _eunmaToast;

		#endregion
		
		// #region Public fields
		//
		// #endregion

		#region properties

		internal static IWin32Window Window {
			get => _window;
			set => _window = value;
		}

		/// <summary>
		/// Gets unique ID of Toast
		/// </summary>
		public string Guid { get; }
		
		/// <summary>
		/// SequentialId
		/// </summary>
		[DefaultValue(0)]
		public int SequentialId { get; internal set; }

		/// <summary>
		/// Gets or sets caption of Toast
		/// </summary>
		[DefaultValue("")]
		public string Caption { get; internal set; } = string.Empty;

		/// <summary>
		/// Gets or sets description of Toast
		/// </summary>
		[DefaultValue("")]
		internal string Description { get; set; } = string.Empty;

		[DefaultValue(null)] internal Font ToastFont { get; set; }

		/// <summary>
		/// Gets or sets timeout duration of Toast
		/// </summary>
		[DefaultValue(ToastDuration.VERY_SHORT)]
		internal ToastDuration ToastDuration { get; set; } = ToastDuration.VERY_SHORT;
		
		[DefaultValue(ToastSound.Generic01)]
		internal ToastSound ToastSound { get; set; } = ToastSound.Generic01;

		[DefaultValue(false)]
		internal bool IsMuted { get; set; }

		[DefaultValue(ToastAnimation.FADE)]
		internal ToastAnimation ToastAnimation { get; set; } = ToastAnimation.FADE;

		internal Image Thumbnail { get; set; }
		internal Image AppOwnerIcon { get; set; }

		[DefaultValue(ToastPosition.BottomRight)]
		internal ToastPosition ToastPosition { get; set; } = ToastPosition.BottomRight;

		[DefaultValue(ToastTheme.Dark)]
		internal ToastTheme ToastThemeStyle { get; set; } = ToastTheme.Dark;

		[DefaultValue(ToastCloseStyle.ButtonAndClickEntire)]
		internal ToastCloseStyle ToastCloseStyle { get; set; } = ToastCloseStyle.ButtonAndClickEntire;

		internal ColorScheme CustomTheme { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Construct an empty Toast object. You must sets View before you can call Show().
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
        internal Toast(IWin32Window window) {
			Guid = Utils.GetGuid();
			_window = window;
			_eunmaToast = new EunmaToast();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Display the Toast for the specified configuration.
		/// <exception cref="ArgumentException">Thrown when Text property is null or empty</exception>
		/// </summary>
		public void Show() {
			InternalDisplayToast();
		}

		/// <summary>
		/// Display the Toast asynchronously for the specified configuration.
		/// <exception cref="ArgumentException">Thrown when Text property is null or empty</exception>
		/// </summary>
		public void ShowAsync() {
			InternalDisplayToast(true);
		}

		/// <summary>
		/// Close the Toast if it's showing, or don't show it if it isn't showing yet. You do not normally have to call this. Normally Toast will disappear on its own after the appropriate duration.
		/// </summary>
		public void Cancel() {
			if(_eunmaToast.IsShown)
				_eunmaToast.Close();
			else
				throw new InvalidOperationException("You cannot cancel toast displaying when it doesn't display");
		}

		/// <summary>
		/// Get current horizontal margin of toast
		/// </summary>
		/// <returns></returns>
		public int GetHorizontalMargin() {
			return _eunmaToast.HorizontalMargin;
		}

		/// <summary>
		/// Get current vertical margin of toast
		/// </summary>
		/// <returns></returns>
		public int GetVerticalMargin() {
			return _eunmaToast.VerticalMargin;
		}

		#endregion

		#region Private methods

		private void InternalDisplayToast(bool async = false) {
			_eunmaToast.IsAsync = async;
			ToastManager.CurrentToast = this;
			_eunmaToast.Click += EunmaToastClick;
			_eunmaToast.MouseHover += EunmaToastMouseHover;
			_eunmaToast.FormClosed += EunmaToastFormClosed;
			ToastManager.AddToCollection();
		}

		private void EunmaToastFormClosed(object sender, FormClosedEventArgs e) {
			OnClosed?.Invoke(this, EventArgs.Empty);
		}

		private void EunmaToastMouseHover(object sender, EventArgs e) {
			OnHover?.Invoke(this, e);
		}

		private void EunmaToastClick(object sender, EventArgs e) {
			OnClick?.Invoke(this, e);
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Build a simplest Toast with Text only.
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="text">Text to display. Required not null or empty.</param>
		/// <returns>Toast has been create but not yet display. Use Show() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption) {
			var toast = new Toast(window)
			{
				Caption = caption,
				Description = string.Empty
			};
			return toast;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="window"></param>
		/// <param name="caption"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		public static Toast Build(IWin32Window window, string caption, string description) {
			var toast = new Toast(window) {
				Caption = caption,
				Description = description
			};
			return toast;
		}

		/// <summary>
		/// Build a Toast with custom duration and animation.
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <returns>Toast has been create but not yet display. Use Show() or ShowAsync() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption, ToastDuration toastDuration, ToastAnimation toastAnimation) {
			var toast = new Toast(window) {
				Caption = caption,
				ToastDuration = toastDuration,
				ToastAnimation = toastAnimation
			};

			return toast;
		}

		/// <summary>
		/// Build Toast with custom duration length.
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="description">Description</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <returns>Toast has been create but not yet display. Use Show() or ShowAsync() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption, string description, ToastDuration toastDuration) {
			var toast = new Toast(window) {
				Caption = caption,
				ToastDuration = toastDuration,
				Description = description
			};

			return toast;
		}

		/// <summary>
		/// Build a Toast with custom animation, duration, sound
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <param name="muting">Set sound state. Muting or not. Sound using Windows 10 default notification sound</param>
		/// <returns>Toast has been create but not yet display. Use Show() or ShowAsync() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption, ToastAnimation toastAnimation, ToastDuration toastDuration, bool muting) {
			var toast = new Toast(window) {
				Caption = caption,
				ToastAnimation = toastAnimation,
				ToastDuration = toastDuration,
				IsMuted = muting
			};

			return toast;
		}

		/// <summary>
		/// Build a Toast with custom animation
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <returns>Toast has been create but not yet display. Use Show() or ShowAsync() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption, ToastAnimation toastAnimation) {
			var toast = new Toast(window) {
				Caption = caption,
				ToastAnimation = toastAnimation
			};

			return toast;
		}

		/// <summary>
		/// Build a simple Toast with specific sound status
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="muting">Set sound state. Muting or not. Sound using Windows 10 default notification sound</param>
		/// <returns>Toast has been create but not yet display. Use Show() or ShowAsync() to display it.</returns>
		public static Toast Build(IWin32Window window, string caption, bool muting) {
			var toast = new Toast(window) {
				Caption = caption,
				IsMuted = muting
			};

			return toast;
		}

		/// <summary>
		/// Build a Toast with text and thumbnails and custom duration, animation
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="thumbnail">Thumbnail image to display in Toast. Required image have MINIMUM SIZE 64x64 pixels for best display</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <returns>Toast has been create but not yet display. Use Show() to display it</returns>
		public static Toast Build(IWin32Window window, string caption, Image thumbnail, ToastDuration toastDuration, ToastAnimation toastAnimation) {
			var toast = new Toast(window) {
				Caption = caption,
				Thumbnail = thumbnail,
				ToastDuration = toastDuration,
				ToastAnimation = toastAnimation
			};
			return toast;
		}

		/// <summary>
		/// Build a simple Toast with thumbnail and custom duration, animation and sound
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="thumbnail">Thumbnail image to display in Toast. Required image have MINIMUM SIZE 64x64 pixels for best display</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <param name="muting"></param>
		/// <returns>Toast has been create but not yet display. Use Show() to display it</returns>
		public static Toast Build(IWin32Window window, string caption, Image thumbnail, ToastDuration toastDuration, ToastAnimation toastAnimation, bool muting) {
			var toast = new Toast(window) {
				Caption = caption,
				Thumbnail = thumbnail,
				ToastDuration = toastDuration,
				ToastAnimation = toastAnimation,
				IsMuted = muting
			};
			return toast;
		}
		
		/// <summary>
		/// Build a simple Toast with thumbnail and custom duration, animation and sound
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="description">Description</param>
		/// <param name="toastFont">Font</param>
		/// <param name="thumbnail">Thumbnail image to display in Toast. Required image have MINIMUM SIZE 64x64 pixels for best display</param>
		/// <param name="appOwnerIcon">appOwnerIcon image to display in Toast.</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <param name="toastPosition"></param>
		/// <param name="toastAnimation">Toast transition animation style. Use both fading and sliding animation style.</param>
		/// <param name="toastCloseStyle"></param>
		/// <param name="toastTheme"></param>
		/// <param name="muting"></param>
		/// <returns>Toast has been create but not yet display. Use Show() to display it</returns>
		public static Toast Build(IWin32Window window, string caption, string description, Font toastFont, Image thumbnail, Image appOwnerIcon, ToastDuration toastDuration, ToastPosition toastPosition, ToastAnimation toastAnimation, ToastCloseStyle toastCloseStyle, ToastTheme toastTheme, ToastSound toastSound, bool muting) {
			var toast = new Toast(window) {
				Caption = caption,
				Description = description,
				ToastFont = toastFont,
				Thumbnail = thumbnail,
				AppOwnerIcon = appOwnerIcon,
				ToastDuration = toastDuration,
				ToastPosition = toastPosition,
				ToastAnimation = toastAnimation,
				ToastCloseStyle = toastCloseStyle,
				ToastThemeStyle = toastTheme,
				ToastSound = toastSound,
				IsMuted = muting
			};
			return toast;
		}

		/// <summary>
		/// Build a simple Toast with thumbnail
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="text">Text to display. Required not null or empty.</param>
		/// <param name="thumbnail">Thumbnail image to display in Toast. Required image have MINIMUM SIZE 64x64 pixels for best display</param>
		/// <returns></returns>
		public static Toast Build(IWin32Window window, string caption, Image thumbnail) {
			var toast = new Toast(window) {
				Caption = caption,
				Thumbnail = thumbnail
			};
			return toast;
		}

		/// <summary>
		/// Build a simple Toast with thumbnail and custom duration
		/// </summary>
		/// <param name="window">Containter form. Usually MainForm.</param>
		/// <param name="caption">Text to display. Required not null or empty.</param>
		/// <param name="thumbnail">Thumbnail image to display in Toast. Required image have MINIMUM SIZE 64x64 pixels for best display</param>
		/// <param name="toastDuration">How long to display. SHORT is 2 seconds and LONG is 3 seconds.</param>
		/// <returns></returns>
		public static Toast Build(IWin32Window window, string caption, Image thumbnail, ToastDuration toastDuration) {
			var toast = new Toast(window) {
				Caption = caption,
				Thumbnail = thumbnail,
				ToastDuration = toastDuration
			};
			return toast;
		}

		#endregion

		#region Events

		public delegate void ClickEventHandler(object sender, EventArgs e);

		public event ClickEventHandler OnClick;

		public delegate void HoverEventHandler(object sender, EventArgs e);

		public event HoverEventHandler OnHover;

		public delegate void CloseEventHandler(object sender, EventArgs e);

		public event CloseEventHandler OnClosed;

		#endregion
	}
}
