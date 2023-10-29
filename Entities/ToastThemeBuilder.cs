using System.Drawing;

namespace FallGuysStats {
	public class ToastThemeBuilder {
		internal static ColorScheme CustomScheme;

		public static void CreateCustomScheme(Color backgroundColor, Color foregroundColor) {
			CustomScheme = new ColorScheme(backgroundColor.R, backgroundColor.B, backgroundColor.G, foregroundColor.R, foregroundColor.B, foregroundColor.G);
		}
		internal static class BuiltinScheme {
			internal static readonly ColorScheme DarkScheme = new ColorScheme(33, 33, 33, 255,255,255);
			internal static readonly ColorScheme LightScheme = new ColorScheme(255, 255, 255, 33, 33, 33);
			internal static readonly ColorScheme PrimaryLightScheme = new ColorScheme(33, 150, 243, 255,255,255);
			internal static readonly ColorScheme SuccessLightScheme = new ColorScheme(76, 175, 80, 255, 255, 255);
			internal static readonly ColorScheme WarningLightScheme = new ColorScheme(255, 152, 0, 255, 255, 255);
			internal static readonly ColorScheme ErrorLightScheme = new ColorScheme(213, 0, 0, 255, 255, 255);
			internal static readonly ColorScheme PrimaryDarkScheme = new ColorScheme(33, 33, 33, 33, 150, 243);
			internal static readonly ColorScheme SuccessDarkScheme = new ColorScheme(33, 33, 33, 76, 175, 80);
			internal static readonly ColorScheme WarningDarkScheme = new ColorScheme(33, 33, 33, 255, 152, 0);
			internal static readonly ColorScheme ErrorDarkScheme = new ColorScheme(33, 33, 33, 213,0,0);
		}
	}

	public class ColorScheme {
		private byte RBg { get; set; }
		private byte BBg { get; set; }
		private byte GBg { get; set; }
		private byte RFg { get; set; }
		private byte BFg { get; set; }
		private byte GFg { get; set; }

		/// <summary>
		/// Create new color scheme
		/// </summary>
		/// <param name="rbg"></param>
		/// <param name="bbg"></param>
		/// <param name="gbg"></param>
		/// <param name="rfg"></param>
		/// <param name="bfg"></param>
		/// <param name="gfg"></param>
		public ColorScheme(byte rbg, byte bbg, byte gbg, byte rfg, byte bfg, byte gfg) {
			RBg = rbg;
			BBg = bbg;
			GBg = gbg;
			RFg = rfg;
			BFg = bfg;
			GFg = gfg;
		}

		public Color GetBackgroundColor() {
			return Color.FromArgb(RBg, BBg, GBg);
		}

		public Color GetForegroundColor() {
			return Color.FromArgb(RFg, BFg, GFg);
		}
	}
}
