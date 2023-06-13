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
                            this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size, this.Text, stringFormat);
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
                    return (this.TextRight.Length > 14 ? (Stats.CurrentLanguage == 0 ? 0.92f : Stats.CurrentLanguage == 1 ? 0.86f : 1) : 1);
                default:
                    return 1f;
            }
        }
        private Font GetFontForLongText(string text) {
            return (((Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) && text.Length > 9) || Stats.CurrentLanguage == 2 && text.Length > 9 || Stats.CurrentLanguage == 3 && text.Length > 9)
                    ? new Font(this.Font.FontFamily, this.GetRoundNameFontSize(text.Length, 21), this.Font.Style, GraphicsUnit.Pixel)
                    : this.Font;
        }
        private float GetRoundNameFontSize(int textLength, int offset) {
            float weight = 1.0F;
            if (this.IsCreativeRound) {
                offset += 9;
                if (textLength == 10) {
                    weight = 0.87F;
                } else if (textLength == 11) {
                    weight = 0.86F;
                } else if (textLength == 12) {
                    weight = 0.85F;
                } else if (textLength == 13) {
                    weight = 0.83F;
                } else if (textLength == 14) {
                    weight = 0.81F;
                } else if (textLength == 15) {
                    weight = 0.8F;
                } else if (textLength == 16) {
                    weight = 0.8F;
                } else if (textLength == 17) {
                    weight = 0.8F;
                } else if (textLength == 18) {
                    weight = 0.84F;
                } else if (textLength == 19) {
                    weight = 0.87F;
                } else if (textLength == 20) {
                    weight = 0.9F;
                } else if (textLength == 21) {
                    weight = 0.96F;
                } else if (textLength == 22) {
                    weight = 1.02F;
                } else if (textLength == 23) {
                    weight = 1.12F;
                } else if (textLength == 24) {
                    weight = 1.25F;
                } else if (textLength == 25) {
                    weight = 1.44F;
                } else if (textLength == 26) {
                    weight = 1.72F;
                } else if (textLength == 27) {
                    weight = 2.25F;
                } else if (textLength == 28) {
                    weight = 3.25F;
                } else if (textLength == 29) {
                    weight = 6.25F;
                }
            } else {
                if (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) { // English, French
                    offset += 9;
                    if (textLength == 13) {
                        weight = 0.95F;
                    } else if (textLength == 14) {
                        weight = 1.05F;
                    } else if (textLength == 15) {
                        weight = 1.05F;
                    } else if (textLength == 16) {
                        weight = 1.05F;
                    } else if (textLength == 17) {
                        weight = 1.05F;
                    } else if (textLength == 18) {
                        weight = 1.05F;
                    } else if (textLength == 19) {
                        weight = 1.05F;
                    } else if (textLength == 20) {
                        weight = 1.1F;
                    } else if (textLength == 21) {
                        weight = 1.2F;
                    } else if (textLength == 22) {
                        weight = 1.4F;
                    } else if (textLength == 23) {
                        weight = 1.6F;
                    } else if (textLength == 24) {
                        weight = 1.8F;
                    } else if (textLength == 25) {
                        weight = 2.0F;
                    } else if (textLength == 26) {
                        weight = 2.2F;
                    } else if (textLength == 27) {
                        weight = 3.4F;
                    } else if (textLength == 28) {
                        weight = 4.7F;
                    } else if (textLength == 29) {
                        weight = 9.7F;
                    }
                } else if (Stats.CurrentLanguage == 2) { // Korean
                    offset += 3;
                    if (textLength == 13) {
                        weight = 1.15F;
                    } else if (textLength == 14) {
                        weight = 1.2F;
                    } else if (textLength == 15) {
                        weight = 1.225F;
                    }
                } else if (Stats.CurrentLanguage == 3) { // Japanese
                    if (textLength == 10) {
                        weight = 1.075F;
                    } else if (textLength == 11) {
                        weight = 1.1F;
                    } else if (textLength == 12) {
                        weight = 1.15F;
                    } else if (textLength == 13) {
                        weight = 1.2F;
                    } else if (textLength == 14) {
                        weight = 1.25F;
                    } else if (textLength == 15) {
                        weight = 1.35F;
                    }
                }
            }
            
            return (offset - textLength) * weight;
        }
        private int GetFontOffsetForCreativeRound(string s) {
            int offset = 0;
            if (this.IsCreativeRound && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) { // English, French
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