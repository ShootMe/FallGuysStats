using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
namespace FallGuysStats {
    public class TransparentLabel : Label {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        public TransparentLabel() {
            this.DrawVisible = true;
            this.TextRight = null;
            this.Visible = false;
        }
        [DefaultValue(null)]
        public string TextRight { get; set; }
        [DefaultValue(true)]
        public bool DrawVisible { get; set; }
        public Image PlatformIcon { get; set; }
        public int ImageX { get; set; }
        public int ImageY { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public Color LevelColor { get; set; }
        public Image RoundIcon { get; set; }
        public bool IsCreativeRound { get; set; }
        public void Draw(Graphics g) {
            if (!this.DrawVisible) { return; }
            if (this.PlatformIcon != null) {
                using (SolidBrush brFore = new SolidBrush(this.ForeColor)) {
                    StringFormat stringFormat = new StringFormat {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Far
                    };
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.DrawImage(this.PlatformIcon, this.ImageX, this.ImageY, this.ImageWidth == 0 ? this.PlatformIcon.Width : this.ImageWidth, this.ImageHeight == 0 ? this.PlatformIcon.Height : this.ImageHeight);
                    if (this.TextRight != null) {
                        g.DrawString(this.TextRight, new Font(this.Font.FontFamily, 12, this.Font.Style, GraphicsUnit.Pixel), brFore, this.ClientRectangle, stringFormat);
                    }
                }
            } else {
                using (SolidBrush brBack = new SolidBrush(this.BackColor)) {
                    using (SolidBrush brFore = new SolidBrush(this.ForeColor)) {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        StringFormat stringFormat = new StringFormat {
                            Alignment = StringAlignment.Near
                        };
                        switch (this.TextAlign) {
                            case ContentAlignment.BottomLeft:
                            case ContentAlignment.BottomCenter:
                            case ContentAlignment.BottomRight:
                                stringFormat.LineAlignment = StringAlignment.Far;
                                break;
                            case ContentAlignment.MiddleLeft:
                            case ContentAlignment.MiddleCenter:
                            case ContentAlignment.MiddleRight:
                                stringFormat.LineAlignment = StringAlignment.Center;
                                break;
                            case ContentAlignment.TopLeft:
                            case ContentAlignment.TopCenter:
                            case ContentAlignment.TopRight:
                                stringFormat.LineAlignment = StringAlignment.Near;
                                break;
                        }
                        switch (this.TextAlign) {
                            case ContentAlignment.TopCenter:
                            case ContentAlignment.MiddleCenter:
                            case ContentAlignment.BottomCenter:
                                if (string.IsNullOrEmpty(this.TextRight)) {
                                    stringFormat.Alignment = StringAlignment.Center;
                                }
                                break;
                        }

                        if (!string.IsNullOrEmpty(this.Text)) {
                            this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size * this.GetFontSizeFactor(), this.Text, stringFormat);
                        }

                        if (this.Image != null) {
                            g.DrawImage(this.Image, this.ImageX, this.ImageY, this.ImageWidth == 0 ? this.Image.Width : this.ImageWidth, this.ImageHeight == 0 ? this.Image.Height : this.ImageHeight);
                        }

                        if (!string.IsNullOrEmpty(this.TextRight)) {
                            stringFormat.Alignment = StringAlignment.Far;
                            if (this.Name.Equals("lblRound")) {
                                Font fontForLongText = this.GetFontForLongText(this.TextRight);
                                if (!this.LevelColor.IsEmpty) {
                                    //float sizeOfText = g.MeasureString(this.TextRight, fontForLongText).Width;
                                    float widthOfText = TextRenderer.MeasureText(this.TextRight, fontForLongText).Width;
                                    this.FillRoundedRectangle(g, null, new SolidBrush(this.LevelColor), (int)(this.ClientRectangle.Width - (widthOfText + this.GetFontOffsetForCreativeRound(this.TextRight))), this.ClientRectangle.Y, (int)(widthOfText + this.GetFontOffsetForCreativeRound(this.TextRight)), 22, 10);
                                    if (this.RoundIcon != null) {
                                        g.DrawImage(this.RoundIcon, (this.ClientRectangle.Width - (widthOfText + this.GetFontOffsetForCreativeRound(this.TextRight)) - this.ImageWidth) - 5, this.ClientRectangle.Y, this.ImageWidth, this.ImageHeight);
                                    }
                                }
                                brFore.Color = this.LevelColor.IsEmpty ? this.ForeColor : Color.White;
                                this.DrawOutlineText(g, this.ClientRectangle, null, brFore, fontForLongText.FontFamily, fontForLongText.Style, fontForLongText.Size, this.TextRight, stringFormat);
                                //g.DrawString(this.TextRight, this.GetFontForLongText(this.TextRight), brFore, this.ClientRectangle, stringFormat);
                            } else {
                                this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size * this.GetFontSizeFactor(), this.TextRight, stringFormat);
                                //g.DrawString(this.TextRight, this.Font, brFore, this.ClientRectangle, stringFormat);
                            }
                        }
                    }
                }
            }
        }
        private Color GetComplementaryColor(Color source, int alpha) {
            return Color.FromArgb(alpha, 255 - source.R, 255 - source.G, 255 - source.B);
        }
        private float GetFontSizeFactor() {
            switch (this.Name) {
                case "lblFinals":
                    return (this.TextRight.Length > 15 ? (Stats.CurrentLanguage == 0 ? 0.92f : Stats.CurrentLanguage == 1 ? 0.87f : 1) : 1);
                case "lblStreak":
                    return (this.TextRight.Length > 9 ? (Stats.CurrentLanguage == 0 ? 0.92f : Stats.CurrentLanguage == 1 ? 0.87f : 1) : 1);
                case "lblQualifyChance":
                    return (this.TextRight.Length > 18 ? (Stats.CurrentLanguage == 0 ? 0.92f : Stats.CurrentLanguage == 1 ? 0.87f : 1) : 1);
                case "lblFinish":
                    return (this.TextRight.Length > 14 ? (Stats.CurrentLanguage == 0 ? 0.78f : Stats.CurrentLanguage == 1 ? 0.74f : Stats.CurrentLanguage == 4 ? 0.96f : 1) : 1);
                default:
                    return 1f;
            }
        }
        private Font GetFontForLongText(string text) {
            return (((Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) && (text.Length >= 17) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) ||
                    (Stats.CurrentLanguage == 2 && (text.Length >= 12) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(2).Name)) ||
                    (Stats.CurrentLanguage == 3 && (text.Length >= 10) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(3).Name)) ||
                    (Stats.CurrentLanguage == 4 && (text.Length >= 10) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(4).Name)))
                    ? new Font(this.Font.FontFamily, this.GetRoundNameFontSize(text.Length), this.Font.Style, GraphicsUnit.Pixel) : this.Font;
        }
        private float GetRoundNameFontSize(int textLength) {
            float weight = 1.0F;
            if ((Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) &&
                this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) { // English, French
                if (textLength == 17) {
                    weight = this.LevelColor.IsEmpty ? 1.33F : 1.36F;
                } else if (textLength == 18) {
                    weight = this.LevelColor.IsEmpty ? 1.35F : 1.38F;
                } else if (textLength == 19) {
                    weight = this.LevelColor.IsEmpty ? 1.4F : 1.45F;
                } else if (textLength == 20) {
                    weight = this.LevelColor.IsEmpty ? 1.46F : 1.6F;
                } else if (textLength == 21) {
                    weight = this.LevelColor.IsEmpty ? 1.49F : 1.62F;
                } else if (textLength == 22) {
                    weight = this.LevelColor.IsEmpty ? 1.62F : 1.83F;
                } else if (textLength == 23) {
                    weight = this.LevelColor.IsEmpty ? 1.78F : 1.94F;
                } else if (textLength == 24) {
                    weight = this.LevelColor.IsEmpty ? 1.98F : 2.25F;
                } else if (textLength == 25) {
                    weight = this.LevelColor.IsEmpty ? 2.26F : 2.6F;
                } else if (textLength == 26) {
                    weight = this.LevelColor.IsEmpty ? 2.72F : 2.99F;
                } else if (textLength == 27) {
                    weight = this.LevelColor.IsEmpty ? 3.48F : 3.98F;
                } else if (textLength == 28) {
                    weight = this.LevelColor.IsEmpty ? 5.1F : 5.95F; 
                } else if (textLength == 29) {
                    weight = this.LevelColor.IsEmpty ? 9.8F : 10.65F; 
                }
            } else if (Stats.CurrentLanguage == 2 &&
                       this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(2).Name)) { // Korean
                if (textLength == 12) {
                    weight = 0.94F;
                } else if (textLength == 13) {
                    weight = this.LevelColor.IsEmpty ? 0.92F : 0.94F;
                } else if (textLength == 14) {
                    weight = this.LevelColor.IsEmpty ? 0.91F : 0.93F;
                } else if (textLength == 15) {
                    weight = this.LevelColor.IsEmpty ? 0.91F : 0.93F;
                } else if (textLength == 16) {
                    weight = this.LevelColor.IsEmpty ? 0.91F : 0.92F;
                } else if (textLength == 17) {
                    weight = this.LevelColor.IsEmpty ? 0.93F : 0.98F;
                } else if (textLength == 18) {
                    weight = this.LevelColor.IsEmpty ? 0.94F : 0.98F;
                } else if (textLength == 19) {
                    weight = 0.98F;
                } else if (textLength == 20) {
                    weight = this.LevelColor.IsEmpty ? 1.03F : 1.08F;
                } else if (textLength == 21) {
                    weight = this.LevelColor.IsEmpty ? 1.09F : 1.07F;
                } else if (textLength == 22) {
                    weight = this.LevelColor.IsEmpty ? 1.17F : 1.2F;
                } else if (textLength == 23) {
                    weight = this.LevelColor.IsEmpty ? 1.27F : 1.38F;
                } else if (textLength == 24) {
                    weight = 1.44F;
                } else if (textLength == 25) {
                    weight = this.LevelColor.IsEmpty ? 1.66F : 1.72F;
                } else if (textLength == 26) {
                    weight = this.LevelColor.IsEmpty ? 2F : 1.88F;
                } else if (textLength == 27) {
                    weight = this.LevelColor.IsEmpty ? 2.58F : 2.5F;
                } else if (textLength == 28) {
                    weight = this.LevelColor.IsEmpty ? 3.7F : 3.76F;
                } else if (textLength == 29) {
                    weight = this.LevelColor.IsEmpty ? 7.18F :7.5F;
                }
            } else if (Stats.CurrentLanguage == 3 &&
                       this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(3).Name)) { // Japanese
                if (textLength == 10) {
                    weight = this.LevelColor.IsEmpty ? 0.8F : 0.94F;
                } else if (textLength == 11) {
                    weight = this.LevelColor.IsEmpty ? 0.78F : 0.88F;
                } else if (textLength == 12) {
                    weight = this.LevelColor.IsEmpty ? 0.75F : 0.88F;
                } else if (textLength == 13) {
                    weight = this.LevelColor.IsEmpty ? 0.73F : 0.88F;
                } else if (textLength == 14) {
                    weight = this.LevelColor.IsEmpty ? 0.72F : 0.86F;
                } else if (textLength == 15) {
                    weight = this.LevelColor.IsEmpty ? 0.72F : 0.86F;
                } else if (textLength == 16) {
                    weight = this.LevelColor.IsEmpty ? 0.73F : 0.84F;
                } else if (textLength == 17) {
                    weight = this.LevelColor.IsEmpty ? 0.75F : 0.83F;
                } else if (textLength == 18) {
                    weight = this.LevelColor.IsEmpty ? 0.76F : 0.9F;
                } else if (textLength == 19) {
                    weight = this.LevelColor.IsEmpty ? 0.8F : 0.9F;
                } else if (textLength == 20) {
                    weight = this.LevelColor.IsEmpty ? 0.84F : 0.98F;
                } else if (textLength == 21) {
                    weight = this.LevelColor.IsEmpty ? 0.89F : 0.98F;
                } else if (textLength == 22) {
                    weight = this.LevelColor.IsEmpty ? 0.96F : 1.11F;
                } else if (textLength == 23) {
                    weight = this.LevelColor.IsEmpty ? 1.05F : 1.27F;
                } else if (textLength == 24) {
                    weight = this.LevelColor.IsEmpty ? 1.17F : 1.31F;
                } else if (textLength == 25) {
                    weight = this.LevelColor.IsEmpty ? 1.35F : 1.57F;
                } else if (textLength == 26) {
                    weight = this.LevelColor.IsEmpty ? 1.63F : 1.96F;
                } else if (textLength == 27) {
                    weight = this.LevelColor.IsEmpty ? 2.11F : 2.3F;
                } else if (textLength == 28) {
                    weight = this.LevelColor.IsEmpty ? 3F : 3.45F;
                } else if (textLength == 29) {
                    weight = this.LevelColor.IsEmpty ? 5.9F : 6.9F;
                }
            } else if (Stats.CurrentLanguage == 4 &&
                       this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(4).Name)) { // Simplified Chinese
                if (textLength == 10) {
                    weight = this.LevelColor.IsEmpty ? 0.92F : 0.85F;
                } else if (textLength == 11) {
                    weight = this.LevelColor.IsEmpty ? 0.92F : 0.83F;
                } else if (textLength == 12) {
                    weight = this.LevelColor.IsEmpty ? 0.89F : 0.77F;
                } else if (textLength == 13) {
                    weight = this.LevelColor.IsEmpty ? 0.88F : 0.76F;
                } else if (textLength == 14) {
                    weight = this.LevelColor.IsEmpty ? 0.87F : 0.74F;
                } else if (textLength == 15) {
                    weight = this.LevelColor.IsEmpty ? 0.86F : 0.73F;
                } else if (textLength == 16) {
                    weight = this.LevelColor.IsEmpty ? 0.88F : 0.78F;
                } else if (textLength == 17) {
                    weight = this.LevelColor.IsEmpty ? 0.9F : 0.76F;
                } else if (textLength == 18) {
                    weight = this.LevelColor.IsEmpty ? 0.93F : 0.82F;
                } else if (textLength == 19) {
                    weight = this.LevelColor.IsEmpty ? 0.96F : 0.81F;
                } else if (textLength == 20) {
                    weight = this.LevelColor.IsEmpty ? 1.01F : 0.89F;
                } else if (textLength == 21) {
                    weight = this.LevelColor.IsEmpty ? 1.07F : 0.87F;
                } else if (textLength == 22) {
                    weight = this.LevelColor.IsEmpty ? 1.14F : 0.99F;
                } else if (textLength == 23) {
                    weight = this.LevelColor.IsEmpty ? 1.26F : 1.12F;
                } else if (textLength == 24) {
                    weight = this.LevelColor.IsEmpty ? 1.41F : 1.15F;
                } else if (textLength == 25) {
                    weight = this.LevelColor.IsEmpty ? 1.61F : 1.38F;
                } else if (textLength == 26) {
                    weight = this.LevelColor.IsEmpty ? 1.94F : 1.73F;
                } else if (textLength == 27) {
                    weight = this.LevelColor.IsEmpty ? 2.5F : 1.98F;
                } else if (textLength == 28) {
                    weight = this.LevelColor.IsEmpty ? 3.64F : 2.97F;
                } else if (textLength == 29) {
                    weight = this.LevelColor.IsEmpty ? 6.96F : 5.96F;
                }
            }
            return (30 - textLength) * weight;
        }
        private int GetFontOffsetForCreativeRound(string s) {
            int offset = 0;
            if (this.IsCreativeRound &&
                this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) { // English, French
                if (s.Length < 10) {
                    offset += (this.GetCountKorAlphabet(s) * 7) +
                              (int)(this.GetCountJpnAlphabet(s) * 3.3F) +
                              (int)(this.GetCountChineseSimplified(s) * 7.7F) +
                              (int)(this.GetCountChineseTraditional(s) * 7.7F);
                } else if (s.Length == 10) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 6.4F) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.5F) +
                              (int)(this.GetCountChineseSimplified(s) * 6.8F) +
                              (int)(this.GetCountChineseTraditional(s) * 6.8F);
                } else if (s.Length == 11) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 5.1F) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.2F) +
                              (int)(this.GetCountChineseSimplified(s) * 6.8F) +
                              (int)(this.GetCountChineseTraditional(s) * 6.8F);
                } else if (s.Length == 12) {
                    offset += (this.GetCountKorAlphabet(s) * 3) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.5F) +
                              (int)(this.GetCountChineseSimplified(s) * 5.9F) +
                              (int)(this.GetCountChineseTraditional(s) * 5.9F);
                } else if (s.Length == 13) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.8F) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.3F) +
                              (int)(this.GetCountChineseSimplified(s) * 5.3F) +
                              (int)(this.GetCountChineseTraditional(s) * 5.3F);
                } else if (s.Length == 14) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 5.5F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 5.5F) +
                              (int)(this.GetCountChineseTraditional(s) * 5.5F);
                } else if (s.Length == 15) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 3.4F) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.6F) +
                              (int)(this.GetCountChineseSimplified(s) * 4.3F) +
                              (int)(this.GetCountChineseTraditional(s) * 4.3F);
                } else if (s.Length == 16) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.5F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.7F) +
                              (int)(this.GetCountChineseSimplified(s) * 4.4F) +
                              (int)(this.GetCountChineseTraditional(s) * 4.4F);
                } else if (s.Length == 17) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 0.69F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 4.9F) +
                              (int)(this.GetCountChineseTraditional(s) * 4.9F);
                } else if (s.Length == 18) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 0.25F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 4.7F) +
                              (int)(this.GetCountChineseTraditional(s) * 4.7F);
                } else if (s.Length == 19) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 0.78F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.9F) +
                              (int)(this.GetCountChineseSimplified(s) * 4.1F) +
                              (int)(this.GetCountChineseTraditional(s) * 4.1F);
                } else if (s.Length == 20) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 3.15F) +
                              (int)(this.GetCountJpnAlphabet(s) * 2.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.3F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.3F);
                } else if (s.Length == 21) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.7F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.6F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.5F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.5F);
                } else if (s.Length == 22) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.2F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.5F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.5F);
                } else if (s.Length == 23) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.8F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.9F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.4F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.4F);
                } else if (s.Length == 24) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.45F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.5F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.5F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.5F);
                } else if (s.Length == 25) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.1F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.1F) +
                              (int)(this.GetCountChineseSimplified(s) * 3.6F) +
                              (int)(this.GetCountChineseTraditional(s) * 3.6F);
                } else if (s.Length == 26) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.75F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.6F) +
                              (int)(this.GetCountChineseSimplified(s) * 2.88F) +
                              (int)(this.GetCountChineseTraditional(s) * 2.88F);
                } else if (s.Length == 27) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.58F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.45F) +
                              (int)(this.GetCountChineseSimplified(s) * 2.68F) +
                              (int)(this.GetCountChineseTraditional(s) * 2.68F);
                } else if (s.Length == 28) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.3F) +
                              (int)(this.GetCountJpnAlphabet(s) * 1.2F) +
                              (int)(this.GetCountChineseSimplified(s) * 2.68F) +
                              (int)(this.GetCountChineseTraditional(s) * 2.68F);
                } else if (s.Length == 29) {
                    offset += (int)(this.GetCountKorAlphabet(s) * 2.03F) +
                              (this.GetCountJpnAlphabet(s) * 1) +
                              (int)(this.GetCountChineseSimplified(s) * 2.65F) +
                              (int)(this.GetCountChineseTraditional(s) * 2.65F);
                }
            }
            return offset;
        }
        private void FillRoundedRectangle(Graphics g, Pen pen, Brush brush, int x, int y, int width, int height, int radius) {
            using (GraphicsPath path = new GraphicsPath()) {
                Rectangle corner = new Rectangle(x, y, radius, radius);
                path.AddArc(corner, 180, 90);
                corner.X = x + width - radius;
                path.AddArc(corner, 270, 90);
                corner.Y = y + height - radius;
                path.AddArc(corner, 0, 90);
                corner.X = x;
                path.AddArc(corner, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
                if (pen != null) { g.DrawPath(pen, path); }
            }
        }
        private void DrawOutlineText(Graphics g, Rectangle layoutRect, Pen outlinePen, Brush fillBrush, FontFamily fontFamily, FontStyle fontStyle, float fontSize, string text, StringFormat stringFormat) {
            using (GraphicsPath path = new GraphicsPath()) {
                path.AddString(text, fontFamily, (int)fontStyle, fontSize, layoutRect, stringFormat);
                path.CloseFigure();
                g.FillPath(fillBrush, path);
                if (outlinePen != null) g.DrawPath(outlinePen, path);
            }
        }
        private int GetCountKorAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if ((0xAC00 <= ch && ch <= 0xD7A3)
                    || (0x3131 <= ch && ch <= 0x318E)
                   ) count++;
            }
            return count;
        }
        private int GetCountJpnAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if ((0x3040 <= ch && ch <= 0x309F)
                    || (0x30A0 <= ch && ch <= 0x30FF)
                    || (0x3400 <= ch && ch <= 0x4DBF)
                    || (0x4E00 <= ch && ch <= 0x9FBF)
                    || (0xF900 <= ch && ch <= 0xFAFF)
                   ) count++;
            }
            return count;
        }
        private int GetCountChineseSimplified(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (ch >= 0x4e00 && ch <= 0x9fff &&
                     (
                         ch <= 0x9fa5 ||
                         (ch >= 0x3400 && ch <= 0x4dbf) ||
                         (ch >= 0x20000 && ch <= 0x2a6df) ||
                         (ch >= 0x2a700 && ch <= 0x2b73f) ||
                         (ch >= 0x2b740 && ch <= 0x2b81f) ||
                         (ch >= 0x2b820 && ch <= 0x2ceaf) ||
                         (ch >= 0xff00 && ch <= 0xffef)
                     )
                    ) count++;
            }
            return count;
        }
        private int GetCountChineseTraditional(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (ch >= 0x4e00 && ch <= 0x9fff && 
                     (
                        ch >= 0x9fa6 ||
                        (ch >= 0x2f00 && ch <= 0x2fdf) ||
                        (ch >= 0x2e80 && ch <= 0x2eff) ||
                        (ch >= 0x2f00 && ch <= 0x2fdf) ||
                        (ch >= 0x31c0 && ch <= 0x31ef) ||
                        (ch >= 0x2f800 && ch <= 0x2fa1f)
                     )
                    ) count++;
            }
            return count;
        }
    }
}