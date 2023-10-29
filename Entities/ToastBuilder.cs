using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
	/// <summary>
	/// Make your own custom Toast Notification by the Builder
	/// </summary>
	public class ToastBuilder {
		private readonly Toast _toast;

		public ToastBuilder(IWin32Window window) {
			_toast = new Toast(window);
		}

		/// <summary>
		/// Set caption for toast
		/// </summary>
		/// <param name="toast">toast</param>
		/// <param name="text">Text data to display</param>
		/// <returns></returns>
		public ToastBuilder SetCaption(string caption) {
			_toast.Caption = caption;
			return this;
		}

		/// <summary>
		/// Set description for Toast
		/// </summary>
		/// <param name="description"></param>
		/// <returns></returns>
		public ToastBuilder SetDescription(string description) {
			_toast.Description = description?.Trim() ?? string.Empty;
			return this;
		}
		
		/// <summary>
		/// Set font for Toast
		/// </summary>
		/// <param name="description"></param>
		/// <returns></returns>
		public ToastBuilder SetFont(Font font) {
			_toast.ToastFont = font;
			return this;
		}

		/// <summary>
		/// Set duration time of Toast
		/// </summary>
		/// <param name="toast"></param>
		/// <param name="toastDuration"></param>
		/// <returns></returns>
		public ToastBuilder SetDuration(ToastDuration toastDuration) {
			_toast.ToastDuration = toastDuration;
			return this;
		}

		/// <summary>
		/// Set muting mode for Toast
		/// </summary>
		/// <param name="muting"></param>
		/// <returns></returns>
		public ToastBuilder SetMuting(bool muting = false) {
			_toast.IsMuted = muting;
			return this;
		}

		public ToastBuilder SetThumbnail(Image image) {
			_toast.Thumbnail = image;
			return this;
		}
		
		public ToastBuilder SetAppOwnerIcon(Image image) {
			_toast.AppOwnerIcon = image;
			return this;
		}
		
		public ToastBuilder SetPosition(ToastPosition toastPosition) {
			_toast.ToastPosition = toastPosition;
			return this;
		}
		
		public ToastBuilder SetAnimation(ToastAnimation toastAnimation) {
			_toast.ToastAnimation = toastAnimation;
			return this;
		}
		
		public ToastBuilder SetCloseStyle(ToastCloseStyle toastCloseStyle) {
			_toast.ToastCloseStyle = toastCloseStyle;
			return this;
		}
		
		public ToastBuilder SetTheme(ToastTheme toastTheme) {
			_toast.ToastThemeStyle = toastTheme;
			return this;
		}
		
		public ToastBuilder SetSound(ToastSound toastSound) {
			_toast.ToastSound = toastSound;
			return this;
		}

		/// <summary>
		/// Build the final toast
		/// </summary>
		/// <returns></returns>
		public Toast Build() {
			return _toast;
		}
	}
}
