using System;
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
        public Color LevelTrueColor { get; set; }
        public Image RoundIcon { get; set; }
        public bool UseShareCode { get; set; }
        public int SecondProgress { get; set; }
        public int OverlaySetting { get; set; }
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
                        g.DrawString(this.TextRight, new Font(this.Font.FontFamily, 12f, this.Font.Style, GraphicsUnit.Pixel), brFore, this.ClientRectangle, stringFormat);
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
                            if ((this.Name.Equals("lblPlayers") && (this.OverlaySetting == 0 || this.OverlaySetting == 1 || this.OverlaySetting == 4 || this.OverlaySetting == 5)) ||
                                (this.Name.Equals("lblFastest") && this.OverlaySetting == 2) ||
                                (this.Name.Equals("lblFinals") && this.OverlaySetting == 3) ||
                                (this.Name.Equals("lblDuration") && this.OverlaySetting == 6)) {
                                if (this.SecondProgress > 0) this.FillRoundedRectangleF(g, new Pen(this.GetComplementaryColor(brFore.Color, 95)), new SolidBrush(this.GetComplementaryColor(brFore.Color, 95)), this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width * this.SecondProgress / 60f, this.ClientRectangle.Height * 2, 4f);
                            }
                            this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size * this.GetFontSizeFactor(), this.Text, stringFormat);
                        }

                        if (this.Image != null) {
                            g.DrawImage(this.Image, this.ImageX, this.ImageY, this.ImageWidth == 0 ? this.Image.Width : this.ImageWidth, this.ImageHeight == 0 ? this.Image.Height : this.ImageHeight);
                        }

                        if (!string.IsNullOrEmpty(this.TextRight)) {
                            stringFormat.Alignment = StringAlignment.Far;
                            if (this.Name.Equals("lblRound")) {
                                Font fontForLongText = this.GetFontForRoundName(this.TextRight);
                                if (!this.LevelColor.IsEmpty) {
                                    float widthOfText = g.MeasureString(this.TextRight, fontForLongText).Width;
                                    //float widthOfText = TextRenderer.MeasureText(this.TextRight, fontForLongText).Width;
                                    RectangleF roundRect = this.FillRoundedRectangleF(g, new Pen(this.LevelColor), new SolidBrush(this.LevelColor), (this.ClientRectangle.Width - widthOfText), this.ClientRectangle.Y, widthOfText, 22f, 16f);
                                    if (this.RoundIcon != null) {
                                        this.FillRoundedRectangleF(g, new Pen(this.LevelTrueColor), new SolidBrush(this.LevelTrueColor), (roundRect.X - 7f) - this.ImageWidth, this.ClientRectangle.Y, this.ImageWidth + 2, 22f, 8f);
                                        g.DrawImage(this.RoundIcon, (roundRect.X - 6f) - this.ImageWidth, this.ClientRectangle.Y, this.ImageWidth, this.ImageHeight);
                                    }
                                    brFore.Color = this.LevelColor.IsEmpty ? this.ForeColor : Color.White;
                                    stringFormat.Alignment = StringAlignment.Near;
                                    this.DrawOutlineText(g, new RectangleF(roundRect.X + 0.2f, ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height), null, brFore, fontForLongText.FontFamily, fontForLongText.Style, fontForLongText.Size, this.TextRight, stringFormat);
                                } else {
                                    brFore.Color = this.LevelColor.IsEmpty ? this.ForeColor : Color.White;
                                    this.DrawOutlineText(g, this.ClientRectangle, null, brFore, fontForLongText.FontFamily, fontForLongText.Style, fontForLongText.Size, this.TextRight, stringFormat);
                                }
                                //g.DrawString(this.TextRight, this.GetFontForRoundName(this.TextRight), brFore, this.ClientRectangle, stringFormat);
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
            float factor;
            switch (this.Name) {
                case "lblWins":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 15 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 15 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 15 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblFinals":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (1f - (((this.TextRight.Length * 3.3f) - 68 + (this.Text.Length * 3.3f)) / 100f)) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 14 ? (1f - (((this.TextRight.Length * 3.3f) - 64 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 14 ? (1f - (((this.TextRight.Length * 3.3f) - 64 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblStreak":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.Text.Length > 0 && this.TextRight.Length > 8 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.Text.Length > 0 && this.TextRight.Length > 12 ? (1f - (((this.TextRight.Length * 3.3f) - 53 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.Text.Length > 0 && this.TextRight.Length > 10 ? (1f - (((this.TextRight.Length * 3.3f) - 53 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblQualifyChance":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 17 ? (1f - (((this.TextRight.Length * 3.3f) - 78 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 17 ? (1f - (((this.TextRight.Length * 3.3f) - 68 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 17 ? (1f - (((this.TextRight.Length * 3.3f) - 68 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblFastest":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 63 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 49 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 46 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblPlayers":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 63 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 49 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 7 ? (1f - (((this.TextRight.Length * 3.3f) - 46 + (this.Text.Length * 3.3f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblDuration":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.Text.Length > 16 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : (this.Text.Length > 13 && this.TextRight.Length == 8 ? 1f - (((this.TextRight.Length * 3.3f) - 63 + (this.Text.Length * 3.3f)) / 100f) : 1f)) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.Text.Length > 14 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.Text.Length > 14 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                    break;
                case "lblFinish":
                    factor = (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.Text.Length <= 8 ? (this.TextRight.Length > 14 ? 1f - (((this.TextRight.Length * 2.5f) - 26) / 100f) : 1f) : (1f - (((this.Text.Length * 2.5f) - 41) / 100f))) :
                             (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.Text.Length <= 4 ? (this.TextRight.Length > 15 ? 1f - ((this.TextRight.Length - 13) / 100f) : 1f) : 1f) :
                             (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.Text.Length <= 4 ? (this.TextRight.Length > 15 ? 1f - ((this.TextRight.Length - 13) / 100f) : 1f) : 1f) : 1f;
                    break;
                default:
                    factor = 1f;
                    break;
            }
            return factor > 1f ? 1f : factor;
        }
        private Font GetFontForRoundName(string text) {
            return new Font(this.Font.FontFamily, this.Font.Size * this.GetFontSizeFactorForRoundName(text), this.Font.Style, GraphicsUnit.Pixel);
        }
        private float GetFontSizeFactorForRoundName(string text) {
            float factor = 1f,
                  factorOffsetForSpace = 0f,
                  factorOffsetForEngAlphabet = 0f,
                  factorOffsetForKorAlphabet = 0f,
                  factorOffsetForJpnAlphabet = 0f,
                  factorOffsetForChineseCharacter = 0f,
                  factorOffsetForNumeric = 0f,
                  factorOffsetForSignCharacter = 0f;
            
            if (text.Length >= 9 && 30 >= text.Length) {
                if ((Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) { // English & French
                    factor = 0.33f; factorOffsetForSpace = 0.03f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0365f : 0.0435f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.013f : 0.019f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.013f : 0.019f;
                    factorOffsetForChineseCharacter = this.LevelColor.IsEmpty ? 0.013f : 0.019f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.03f : 0.038f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.03f : 0.038f;
                } else if ((Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(4).Name)) { // Simplified Chinese & Traditional Chinese
                    factor = 0.33f; factorOffsetForSpace = 0.03f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.045f : 0.033f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0205f : 0.013f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.024f : 0.017f;
                    factorOffsetForChineseCharacter = this.LevelColor.IsEmpty ? 0.024f : 0.017f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.038f : 0.031f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.045f : 0.04f;
                } else { // Korean & Japanese & Custom font
                    factor = 0.33f; factorOffsetForSpace = 0.03f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0375f : 0.0435f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0205f : 0.0265f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.017f : 0.0225f;
                    factorOffsetForChineseCharacter = this.LevelColor.IsEmpty ? 0.0165f : 0.0225f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.033f : 0.04f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.043f : 0.05f;
                }
                
                factor += (this.GetCountSpace(text) * factorOffsetForSpace)
                          + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet)
                          + (this.GetCountKorAlphabet(text) * factorOffsetForKorAlphabet) + (this.GetCountJpnAlphabet(text) * factorOffsetForJpnAlphabet)
                          + (this.GetCountChineseCharacter(text) * factorOffsetForChineseCharacter)
                          + (this.GetCountSignCharacter(text) * factorOffsetForSignCharacter) + (this.GetCountNumeric(text) * factorOffsetForNumeric);
            }
                
            if (text.Length == 9) {
                factor *= 2f;
            } else if (text.Length == 10) {
                factor *= 1.72f;
            } else if (text.Length == 11) {
                factor *= 1.52f;
            } else if (text.Length == 12) {
                factor *= 1.36f;
            } else if (text.Length == 13) {
                factor *= 1.23f;
            } else if (text.Length == 14) {
                factor *= 1.11f;
            } else if (text.Length == 15) {
                factor *= 1f;
            } else if (text.Length == 16) {
                factor *= 0.92f;
            } else if (text.Length == 17) {
                factor *= 0.86f;
            } else if (text.Length == 18) {
                factor *= 0.79f;
            } else if (text.Length == 19) {
                factor *= 0.73f;
            } else if (text.Length == 20) {
                factor *= 0.67f;
            } else if (text.Length == 21) {
                factor *= 0.62f;
            } else if (text.Length == 22) {
                factor *= 0.58f;
            } else if (text.Length == 23) {
                factor *= 0.54f;
            } else if (text.Length == 24) {
                factor *= 0.51f;
            } else if (text.Length == 25) {
                factor *= 0.48f;
            } else if (text.Length == 26) {
                factor *= 0.45f;
            } else if (text.Length == 27) {
                factor *= 0.42f;
            } else if (text.Length == 28) {
                factor *= 0.395f;
            } else if (text.Length == 29) {
                factor *= 0.375f;
            } else if (text.Length == 30) {
                factor *= 0.355f;
            }
            
            return factor > 1f ? 1f : factor;
        }
        private RectangleF FillRoundedRectangleF(Graphics g, Pen pen, Brush brush, float x, float y, float width, float height, float radius) {
            using (GraphicsPath path = new GraphicsPath()) {
                RectangleF rect = new RectangleF(x, y, radius, radius);
                path.AddArc(rect, 180, 90);
                rect.X = x + width - radius;
                path.AddArc(rect, 270, 90);
                rect.Y = y + height - radius;
                path.AddArc(rect, 0, 90);
                rect.X = x;
                path.AddArc(rect, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
                if (pen != null) { g.DrawPath(pen, path); }
                return rect;
            }
        }
        private void DrawOutlineText(Graphics g, RectangleF layoutRect, Pen outlinePen, Brush fillBrush, FontFamily fontFamily, FontStyle fontStyle, float fontSize, string text, StringFormat stringFormat) {
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
                if ((0x3040 <= ch && ch <= 0x309F) //Hiragana
                    || (0x30A0 <= ch && ch <= 0x30FF) //Katakana
                    //|| (0x3400 <= ch && ch <= 0x4DBF) //CJK Unified Ideographs Extension A (6592EA)
                    //|| (0x4E00 <= ch && ch <= 0x9FBF) //CJK Unified Ideographs (20928EA)
                    //|| (0xF900 <= ch && ch <= 0xFAFF) //CJK Compatibility Ideographs (512EA)
                   ) count++;
            }
            return count;
        }
        // private int GetCountChineseSimplified(string s) {
        //     int count = 0;
        //     char[] charArr = s.ToCharArray();
        //     foreach (char ch in charArr) {
        //         if (ch >= 0x4e00 && ch <= 0x9fff &&
        //              (
        //                  ch <= 0x9fa5 ||
        //                  (ch >= 0x3400 && ch <= 0x4dbf) ||
        //                  (ch >= 0x20000 && ch <= 0x2a6df) ||
        //                  (ch >= 0x2a700 && ch <= 0x2b73f) ||
        //                  (ch >= 0x2b740 && ch <= 0x2b81f) ||
        //                  (ch >= 0x2b820 && ch <= 0x2ceaf) ||
        //                  (ch >= 0xff00 && ch <= 0xffef)
        //              )
        //             ) count++;
        //     }
        //     return count;
        // }
        // private int GetCountChineseTraditional(string s) {
        //     int count = 0;
        //     char[] charArr = s.ToCharArray();
        //     foreach (char ch in charArr) {
        //         if (ch >= 0x4e00 && ch <= 0x9fff && 
        //              (
        //                 ch >= 0x9fa6 ||
        //                 (ch >= 0x2f00 && ch <= 0x2fdf) ||
        //                 (ch >= 0x2e80 && ch <= 0x2eff) ||
        //                 (ch >= 0x2f00 && ch <= 0x2fdf) ||
        //                 (ch >= 0x31c0 && ch <= 0x31ef) ||
        //                 (ch >= 0x2f800 && ch <= 0x2fa1f)
        //              )
        //             ) count++;
        //     }
        //     return count;
        // }
        private int GetCountChineseCharacter(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (ch >= 0x4e00 && ch <= 0x9fff) count++;
            }
            return count;
        }
        private int GetCountEngAlphabet(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if ((0x61 <= ch && ch <= 0x7A) //Lowercase
                     || (0x41 <= ch && ch <= 0x5A) //Uppercase
                   ) count++;
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
        private int GetCountSignCharacter(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if ((0x21 <= ch && ch <= 0x2F)
                    || (0x3A <= ch && ch <= 0x40)
                    || (0x5B <= ch && ch <= 0x60)
                    || (0x7B <= ch && ch <= 0x7E)
                   ) count++;
            }
            return count;
        }
        private int GetCountNumeric(string s) {
            int count = 0;
            char[] charArr = s.ToCharArray();
            foreach (char ch in charArr) {
                if (0x30 <= ch && ch <= 0x39) count++;
            }
            return count;
        }
    }
}