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
                                    this.DrawOutlineText(g, new RectangleF(roundRect.X + 0.33f, ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height), null, brFore, fontForLongText.FontFamily, fontForLongText.Style, fontForLongText.Size, this.TextRight, stringFormat);
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
        // private Color GetComplementaryColor(Color source, int alpha) {
        //     return Color.FromArgb(alpha, 255 - source.R, 255 - source.G, 255 - source.B);
        // }
        private float GetFontSizeFactor() {
            switch (this.Name) {
                case "lblWins":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 15 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 15 ? (1f - ((this.TextRight.Length - 18 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 15 ? (1f - ((this.TextRight.Length - 18 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                case "lblFinals":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 14 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 14 ? (1f - ((this.TextRight.Length - 20 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 14 ? (1f - ((this.TextRight.Length - 20 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                case "lblStreak":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 8 ? (1f - (((this.TextRight.Length * 3.3f) - 65 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 12 ? (1f - ((this.TextRight.Length - 15 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 10 ? (1f - ((this.TextRight.Length - 20 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                case "lblQualifyChance":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.TextRight.Length > 17 ? (1f - (((this.TextRight.Length * 3.3f) - 78 + (this.Text.Length * 3.3f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.TextRight.Length > 17 ? (1f - ((this.TextRight.Length - 30 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.TextRight.Length > 17 ? (1f - ((this.TextRight.Length - 30 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                case "lblDuration":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.Text.Length > 14 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.Text.Length > 14 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.Text.Length > 14 ? (1f - ((this.TextRight.Length - 42 + (this.Text.Length * 2.5f)) / 100f)) : 1f) : 1f;
                case "lblFinish":
                    return (Stats.CurrentLanguage == 0 || Stats.CurrentLanguage == 1) ? (this.Text.Length <= 8 ? (this.TextRight.Length > 14 ? 1f - (((this.TextRight.Length * 2.5f) - 28) / 100f) : 1f) : (1f - (((this.Text.Length * 2.5f) - 39) / 100f))) :
                           (Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? (this.Text.Length <= 4 ? (this.TextRight.Length > 15 ? 1f - ((this.TextRight.Length - 13) / 100f) : 1f) : 1f) :
                           (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) ? (this.Text.Length <= 4 ? (this.TextRight.Length > 15 ? 1f - ((this.TextRight.Length - 13) / 100f) : 1f) : 1f) : 1f;
                default:
                    return 1f;
            }
        }
        private Font GetFontForRoundName(string text) {
            return new Font(this.Font.FontFamily, this.Font.Size * this.GetFontSizeFactorForRoundName(text), this.Font.Style, GraphicsUnit.Pixel);
        }
        private float GetFontSizeFactorForRoundName(string text) {
            float factor = 1.0f,
                  factorOffsetForSpace = 0f,
                  factorOffsetForEngAlphabet = 0f,
                  factorOffsetForKorAlphabet = 0f,
                  factorOffsetForJpnAlphabet = 0f,
                  factorOffsetForChineseTraditional = 0f,
                  factorOffsetForChineseSimplified = 0f,
                  factorOffsetForNumeric = 0f,
                  factorOffsetForSignCharacter = 0f;
            if (this.UseShareCode) {
                if (text.Length == 10) {
                    factor = 0.5f; factorOffsetForSpace = 0.0315f;
                    factorOffsetForEngAlphabet = 0.0498f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.036f : 0.042f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.036f : 0.042f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.036f : 0.042f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.036f : 0.042f;
                    factorOffsetForSignCharacter = 0.0498f; factorOffsetForNumeric = 0.0498f;
                } else if (text.Length == 11) {
                    factor = 0.475f; factorOffsetForSpace = 0.031f;
                    factorOffsetForEngAlphabet = 0.0477f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.032f : 0.037f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.032f : 0.037f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.032f : 0.037f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.032f : 0.037f;
                    factorOffsetForSignCharacter = 0.0477f; factorOffsetForNumeric = 0.0477f;
                } else if (text.Length == 12) {
                    factor = 0.45f; factorOffsetForSpace = 0.0305f;
                    factorOffsetForEngAlphabet = 0.0448f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.03f : 0.034f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.03f : 0.034f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.03f : 0.034f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.03f : 0.034f;
                    factorOffsetForSignCharacter = 0.0448f; factorOffsetForNumeric = 0.0448f;
                } else if (text.Length == 13) {
                    factor = 0.43f; factorOffsetForSpace = 0.03f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0425f : 0.0443f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.029f : 0.035f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.029f : 0.035f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.029f : 0.035f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.029f : 0.035f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0425f : 0.0443f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0425f : 0.0443f;
                } else if (text.Length == 14) {
                    factor = 0.38f; factorOffsetForSpace = 0.0295f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.042f : 0.0443f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.027f : 0.036f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.027f : 0.036f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.027f : 0.036f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.027f : 0.036f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.042f : 0.0443f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.042f : 0.0443f;
                } else if (text.Length == 15) {
                    factor = 0.33f; factorOffsetForSpace = 0.029f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0405f : 0.044f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0255f : 0.0345f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0255f : 0.0345f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0255f : 0.0345f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0255f : 0.0345f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0405f : 0.044f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0405f : 0.044f;
                } else if (text.Length == 16) {
                    factor = 0.27f; factorOffsetForSpace = 0.0285f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.037f : 0.044f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.026f : 0.0325f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.026f : 0.0325f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.026f : 0.0325f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.026f : 0.0325f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.037f : 0.044f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.037f : 0.044f;
                } else if (text.Length == 17) {
                    factor = 0.23f; factorOffsetForSpace = 0.028f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0343f : 0.041f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0235f : 0.0295f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0235f : 0.0295f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0235f : 0.0295f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0235f : 0.0295f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0343f : 0.041f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0343f : 0.041f;
                } else if (text.Length == 18) {
                    factor = 0.19f; factorOffsetForSpace = 0.0275f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0325f : 0.039f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0215f : 0.0275f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0215f : 0.0275f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0215f : 0.0275f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0215f : 0.0275f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0325f : 0.039f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0325f : 0.039f;
                } else if (text.Length == 19) {
                    factor = 0.17f; factorOffsetForSpace = 0.027f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.03f : 0.0365f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0197f : 0.025f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0197f : 0.025f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0197f : 0.025f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0197f : 0.025f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.03f : 0.0365f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.03f : 0.0365f;
                } else if (text.Length == 20) {
                    factor = 0.14f; factorOffsetForSpace = 0.0265f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.028f : 0.034f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0192f : 0.0238f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0192f : 0.0238f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0192f : 0.0238f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0192f : 0.0238f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.028f : 0.034f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.028f : 0.034f;
                } else if (text.Length == 21) {
                    factor = 0.11f; factorOffsetForSpace = 0.026f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0265f : 0.0325f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0177f : 0.022f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0177f : 0.022f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0177f : 0.022f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0177f : 0.022f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0265f : 0.0325f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0265f : 0.0325f;
                } else if (text.Length == 22) {
                    factor = 0.09f; factorOffsetForSpace = 0.0255f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0245f : 0.03f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.016f : 0.02f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.016f : 0.02f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.016f : 0.02f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.016f : 0.02f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0245f : 0.03f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0245f : 0.03f;
                } else if (text.Length == 23) {
                    factor = 0.07f; factorOffsetForSpace = 0.025f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0235f : 0.028f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0153f : 0.0193f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0153f : 0.0193f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0153f : 0.0193f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0153f : 0.0193f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0235f : 0.028f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0235f : 0.028f;
                } else if (text.Length == 24) {
                    factor = 0.05f; factorOffsetForSpace = 0.0245f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0225f : 0.0265f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.015f : 0.019f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.015f : 0.019f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.015f : 0.019f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.015f : 0.019f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0225f : 0.0265f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0225f : 0.0265f;
                } else if (text.Length == 25) {
                    factor = 0.02f; factorOffsetForSpace = 0.024f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.022f : 0.026f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0145f : 0.0175f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0145f : 0.0175f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0145f : 0.0175f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0145f : 0.0175f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.022f : 0.026f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.022f : 0.026f;
                } else if (text.Length == 26) {
                    factor = 0.01f; factorOffsetForSpace = 0.0235f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0205f : 0.0245f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0135f : 0.016f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0135f : 0.016f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0135f : 0.016f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0135f : 0.016f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0205f : 0.0245f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0205f : 0.0245f;
                } else if (text.Length == 27) {
                    factor = -0.005f; factorOffsetForSpace = 0.023f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0198f : 0.0235f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.013f : 0.0155f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.013f : 0.0155f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.013f : 0.0155f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.013f : 0.0155f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0198f : 0.0235f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0198f : 0.0235f;
                } else if (text.Length == 28) {
                    factor = -0.025f; factorOffsetForSpace = 0.0225f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0191f : 0.0227f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0125f : 0.015f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0125f : 0.015f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0125f : 0.015f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0125f : 0.015f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0191f : 0.0227f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0191f : 0.0227f;
                } else if (text.Length == 29) {
                    factor = -0.04f; factorOffsetForSpace = 0.022f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0185f : 0.0219f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.012f : 0.0145f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.012f : 0.0145f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.012f : 0.0145f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.012f : 0.0145f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0185f : 0.0219f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0185f : 0.0219f;
                } else if (text.Length == 30) {
                    factor = -0.05f; factorOffsetForSpace = 0.0215f;
                    factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0178f : 0.021f;
                    factorOffsetForKorAlphabet = this.LevelColor.IsEmpty ? 0.0114f : 0.014f; factorOffsetForJpnAlphabet = this.LevelColor.IsEmpty ? 0.0114f : 0.014f;
                    factorOffsetForChineseTraditional = this.LevelColor.IsEmpty ? 0.0114f : 0.014f; factorOffsetForChineseSimplified = this.LevelColor.IsEmpty ? 0.0114f : 0.014f;
                    factorOffsetForSignCharacter = this.LevelColor.IsEmpty ? 0.0178f : 0.021f; factorOffsetForNumeric = this.LevelColor.IsEmpty ? 0.0178f : 0.021f;
                }
                factor += (this.GetCountSpace(text) * factorOffsetForSpace)
                          + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet)
                          + (this.GetCountKorAlphabet(text) * factorOffsetForKorAlphabet) + (this.GetCountJpnAlphabet(text) * factorOffsetForJpnAlphabet)
                          + (this.GetCountChineseTraditional(text) * factorOffsetForChineseTraditional) + (this.GetCountChineseSimplified(text) * factorOffsetForChineseSimplified)
                          + (this.GetCountSignCharacter(text) * factorOffsetForSignCharacter) + (this.GetCountNumeric(text) * factorOffsetForNumeric);
            } else {
                if (Stats.CurrentLanguage == 0 && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(0).Name)) { // English
                    if (text.Length == 13) {
                        factor = 0.93f; factorOffsetForSpace = 0.008f;
                    } else if (text.Length == 14) {
                        factor = 0.88f; factorOffsetForSpace = 0.02f;
                    } else if (text.Length == 15) {
                        factor = 0.83f; factorOffsetForSpace = 0.025f;
                    } else if (text.Length == 16) {
                        factor = 0.77f; factorOffsetForSpace = 0.042f;
                    } else if (text.Length == 17) {
                        factor = 0.73f; factorOffsetForSpace = 0.031f;
                    } else if (text.Length == 18) {
                        factor = 0.69f; factorOffsetForSpace = 0.025f;
                    } else if (text.Length == 19) {
                        factor = 0.67f; factorOffsetForSpace = 0.019f;
                    } else if (text.Length == 20) {
                        factor = 0.64f; factorOffsetForSpace = 0.024f;
                    } else if (text.Length == 21) {
                        factor = 0.61f; factorOffsetForSpace = 0.018f;
                    } else if (text.Length == 22) {
                        factor = 0.59f; factorOffsetForSpace = 0.027f;
                    } else if (text.Length == 23) {
                        factor = 0.57f; factorOffsetForSpace = 0.028f;
                    } else if (text.Length == 24) {
                        factor = 0.55f; factorOffsetForSpace = 0.017f;
                    } else if (text.Length == 25) {
                        factor = 0.52f; factorOffsetForSpace = 0.018f;
                    } else if (text.Length == 26) {
                        factor = 0.51f; factorOffsetForSpace = 0.015f;
                    } else if (text.Length == 27) {
                        factor = 0.495f; factorOffsetForSpace = 0.012f;
                    } else if (text.Length == 28) {
                        factor = 0.475f; factorOffsetForSpace = 0.0105f;
                    } else if (text.Length == 29) {
                        factor = 0.46f; factorOffsetForSpace = 0.011f;
                    } else if (text.Length == 30) {
                        factor = 0.45f; factorOffsetForSpace = 0.012f;
                    }
                    factor += (this.GetCountSpace(text) * factorOffsetForSpace);
                } else if (Stats.CurrentLanguage == 1 && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(1).Name)) { // French
                    if (text.Length == 13) {
                        factor = 0.93f; factorOffsetForSpace = 0.008f;
                    } else if (text.Length == 14) {
                        factor = 0.88f; factorOffsetForSpace = 0.02f;
                    } else if (text.Length == 15) {
                        factor = 0.83f; factorOffsetForSpace = 0.025f;
                    } else if (text.Length == 16) {
                        factor = 0.77f; factorOffsetForSpace = 0.042f;
                    } else if (text.Length == 17) {
                        factor = 0.73f; factorOffsetForSpace = 0.031f;
                    } else if (text.Length == 18) {
                        factor = 0.69f; factorOffsetForSpace = 0.025f;
                    } else if (text.Length == 19) {
                        factor = 0.67f; factorOffsetForSpace = 0.019f;
                    } else if (text.Length == 20) {
                        factor = 0.64f; factorOffsetForSpace = 0.024f;
                    } else if (text.Length == 21) {
                        factor = 0.61f; factorOffsetForSpace = 0.018f;
                    } else if (text.Length == 22) {
                        factor = this.LevelColor.IsEmpty ? 0.535f : 0.59f; factorOffsetForSpace = 0.027f;
                    } else if (text.Length == 23) {
                        factor = this.LevelColor.IsEmpty ? 0.515f : 0.57f; factorOffsetForSpace = 0.028f;
                    } else if (text.Length == 24) {
                        factor = this.LevelColor.IsEmpty ? 0.525f : 0.55f; factorOffsetForSpace = 0.017f;
                    } else if (text.Length == 25) {
                        factor = this.LevelColor.IsEmpty ? 0.495f : 0.52f; factorOffsetForSpace = 0.018f;
                    } else if (text.Length == 26) {
                        factor = this.LevelColor.IsEmpty ? 0.485f : 0.51f; factorOffsetForSpace = 0.015f;
                    } else if (text.Length == 27) {
                        factor = this.LevelColor.IsEmpty ? 0.47f : 0.495f; factorOffsetForSpace = 0.012f;
                    } else if (text.Length == 28) {
                        factor = this.LevelColor.IsEmpty ? 0.45f : 0.475f; factorOffsetForSpace = 0.0105f;
                    } else if (text.Length == 29) {
                        factor = this.LevelColor.IsEmpty ? 0.43f : 0.46f; factorOffsetForSpace = 0.011f;
                    } else if (text.Length == 30) {
                        factor = this.LevelColor.IsEmpty ? 0.42f : 0.45f; factorOffsetForSpace = 0.012f;
                    }
                    factor += (this.GetCountSpace(text) * factorOffsetForSpace);
                } else if (Stats.CurrentLanguage == 2 && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(2).Name)) { // Korean
                    if (text.Length == 12) {
                        factor = 0.93f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.00005f;
                    } else if (text.Length == 13) {
                        factor = 0.87f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.0005f;
                    } else if (text.Length == 14) {
                        factor = 0.81f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.005f;
                    } else if (text.Length == 15) {
                        factor = 0.76f; factorOffsetForSpace = 0.028f; factorOffsetForEngAlphabet = 0.0055f;
                    } else if (text.Length == 16) {
                        factor = 0.71f; factorOffsetForSpace = 0.029f; factorOffsetForEngAlphabet = 0.0085f;
                    } else if (text.Length == 17) {
                        factor = 0.67f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.011f;
                    } else if (text.Length == 18) {
                        factor = 0.63f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.0115f;
                    } else if (text.Length == 19) {
                        factor = 0.6f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.012f;
                    } else if (text.Length == 20) {
                        factor = 0.57f; factorOffsetForSpace = 0.022f; factorOffsetForEngAlphabet = 0.0115f;
                    } else if (text.Length == 21) {
                        factor = 0.545f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.011f;
                    } else if (text.Length == 22) {
                        factor = 0.52f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.0105f;
                    } else if (text.Length == 23) {
                        factor = 0.495f; factorOffsetForSpace = 0.018f; factorOffsetForEngAlphabet = 0.01f;
                    } else if (text.Length == 24) {
                        factor = 0.475f; factorOffsetForSpace = 0.016f; factorOffsetForEngAlphabet = 0.009f;
                    } else if (text.Length == 25) {
                        factor = 0.455f; factorOffsetForSpace = 0.0145f; factorOffsetForEngAlphabet = 0.0085f;
                    } else if (text.Length == 26) {
                        factor = 0.44f; factorOffsetForSpace = 0.013f; factorOffsetForEngAlphabet = 0.008f;
                    } else if (text.Length == 27) {
                        factor = 0.425f; factorOffsetForSpace = 0.012f; factorOffsetForEngAlphabet = 0.0075f;
                    } else if (text.Length == 28) {
                        factor = 0.41f; factorOffsetForSpace = 0.0115f; factorOffsetForEngAlphabet = 0.007f;
                    } else if (text.Length == 29) {
                        factor = 0.395f; factorOffsetForSpace = 0.0115f; factorOffsetForEngAlphabet = 0.00655f;
                    } else if (text.Length == 30) {
                        factor = 0.385f; factorOffsetForSpace = 0.0105f; factorOffsetForEngAlphabet = 0.006f;
                    }
                    factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                } else if (Stats.CurrentLanguage == 3 && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(3).Name)) { // Japanese
                    if (text.Length == 10) {
                        factor = 0.9f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = 0.0005f;
                    } else if (text.Length == 11) {
                        factor = 0.82f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.004f;
                    } else if (text.Length == 12) {
                        factor = 0.76f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.008f;
                    } else if (text.Length == 13) {
                        factor = 0.71f; factorOffsetForSpace = 0.046f; factorOffsetForEngAlphabet = 0.008f;
                    } else if (text.Length == 14) {
                        factor = 0.67f; factorOffsetForSpace = 0.041f; factorOffsetForEngAlphabet = 0.014f;
                    } else if (text.Length == 15) {
                        factor = 0.62f; factorOffsetForSpace = 0.036f; factorOffsetForEngAlphabet = 0.017f;
                    } else if (text.Length == 16) {
                        factor = 0.58f; factorOffsetForSpace = 0.033f; factorOffsetForEngAlphabet = 0.016f;
                    } else if (text.Length == 17) {
                        factor = 0.55f; factorOffsetForSpace = 0.027f; factorOffsetForEngAlphabet = 0.015f;
                    } else if (text.Length == 18) {
                        factor = 0.52f; factorOffsetForSpace = 0.026f; factorOffsetForEngAlphabet = 0.014f;
                    } else if (text.Length == 19) {
                        factor = 0.5f; factorOffsetForSpace = 0.0225f; factorOffsetForEngAlphabet = 0.013f;
                    } else if (text.Length == 20) {
                        factor = 0.47f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.013f;
                    } else if (text.Length == 21) {
                        factor = 0.45f; factorOffsetForSpace = 0.018f; factorOffsetForEngAlphabet = 0.012f;
                    } else if (text.Length == 22) {
                        factor = 0.43f; factorOffsetForSpace = 0.017f; factorOffsetForEngAlphabet = 0.011f;
                    } else if (text.Length == 23) {
                        factor = 0.41f; factorOffsetForSpace = 0.016f; factorOffsetForEngAlphabet = 0.01f;
                    } else if (text.Length == 24) {
                        factor = 0.395f; factorOffsetForSpace = 0.014f; factorOffsetForEngAlphabet = 0.01f;
                    } else if (text.Length == 25) {
                        factor = 0.38f; factorOffsetForSpace = 0.013f; factorOffsetForEngAlphabet = 0.009f;
                    } else if (text.Length == 26) {
                        factor = 0.365f; factorOffsetForSpace = 0.0115f; factorOffsetForEngAlphabet = 0.0085f;
                    } else if (text.Length == 27) {
                        factor = 0.355f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = 0.008f;
                    } else if (text.Length == 28) {
                        factor = 0.34f; factorOffsetForSpace = 0.0095f; factorOffsetForEngAlphabet = 0.0075f;
                    } else if (text.Length == 29) {
                        factor = 0.33f; factorOffsetForSpace = 0.0095f; factorOffsetForEngAlphabet = 0.0067f;
                    } else if (text.Length == 30) {
                        factor = 0.32f; factorOffsetForSpace = 0.0085f; factorOffsetForEngAlphabet = 0.0062f;
                    }
                    factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                } else if ((Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) && this.Font.FontFamily.Name.Equals(Overlay.GetDefaultFontFamilies(4).Name)) { // Simplified Chinese & Traditional Chinese
                    if (text.Length == 10) {
                        factor = 0.9f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = 0.002f;
                    } else if (text.Length == 11) {
                        factor = 0.82f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.007f;
                    } else if (text.Length == 12) {
                        factor = 0.76f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.009f;
                    } else if (text.Length == 13) {
                        factor = 0.71f; factorOffsetForSpace = 0.046f; factorOffsetForEngAlphabet = 0.011f;
                    } else if (text.Length == 14) {
                        factor = 0.67f; factorOffsetForSpace = 0.041f; factorOffsetForEngAlphabet = 0.013f;
                    } else if (text.Length == 15) {
                        factor = 0.62f; factorOffsetForSpace = 0.036f; factorOffsetForEngAlphabet = 0.015f;
                    } else if (text.Length == 16) {
                        factor = 0.58f; factorOffsetForSpace = 0.033f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.017f : 0.015f;
                    } else if (text.Length == 17) {
                        factor = 0.55f; factorOffsetForSpace = 0.027f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.014f;
                    } else if (text.Length == 18) {
                        factor = 0.52f; factorOffsetForSpace = 0.026f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.013f;
                    } else if (text.Length == 19) {
                        factor = 0.5f; factorOffsetForSpace = 0.0225f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.02f : 0.0125f;
                    } else if (text.Length == 20) {
                        factor = 0.47f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0195f : 0.012f;
                    } else if (text.Length == 21) {
                        factor = 0.45f; factorOffsetForSpace = 0.018f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.011f;
                    } else if (text.Length == 22) {
                        factor = 0.43f; factorOffsetForSpace = 0.017f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0165f : 0.01f;
                    } else if (text.Length == 23) {
                        factor = 0.41f; factorOffsetForSpace = 0.016f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.015f : 0.009f;
                    } else if (text.Length == 24) {
                        factor = 0.395f; factorOffsetForSpace = 0.014f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.014f : 0.008f;
                    } else if (text.Length == 25) {
                        factor = 0.38f; factorOffsetForSpace = 0.013f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.013f : 0.007f;
                    } else if (text.Length == 26) {
                        factor = 0.365f; factorOffsetForSpace = 0.0115f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.012f : 0.0065f;
                    } else if (text.Length == 27) {
                        factor = 0.355f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0115f : 0.0065f;
                    } else if (text.Length == 28) {
                        factor = 0.34f; factorOffsetForSpace = 0.0095f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.011f : 0.006f;
                    } else if (text.Length == 29) {
                        factor = 0.33f; factorOffsetForSpace = 0.0095f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.01f : 0.0055f;
                    } else if (text.Length == 30) {
                        factor = 0.32f; factorOffsetForSpace = 0.0085f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.009f : 0.005f;
                    }
                    factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                } else {
                    if (Stats.CurrentLanguage == 0) {
                        if (text.Length == 13) {
                            factor = 0.93f; factorOffsetForSpace = 0.008f;
                        } else if (text.Length == 14) {
                            factor = 0.88f; factorOffsetForSpace = 0.02f;
                        } else if (text.Length == 15) {
                            factor = 0.83f; factorOffsetForSpace = 0.025f;
                        } else if (text.Length == 16) {
                            factor = 0.77f; factorOffsetForSpace = 0.042f;
                        } else if (text.Length == 17) {
                            factor = 0.73f; factorOffsetForSpace = 0.031f;
                        } else if (text.Length == 18) {
                            factor = 0.69f; factorOffsetForSpace = 0.025f;
                        } else if (text.Length == 19) {
                            factor = 0.67f; factorOffsetForSpace = 0.019f;
                        } else if (text.Length == 20) {
                            factor = 0.64f; factorOffsetForSpace = 0.024f;
                        } else if (text.Length == 21) {
                            factor = 0.61f; factorOffsetForSpace = 0.018f;
                        }
                        factor += (this.GetCountSpace(text) * factorOffsetForSpace);
                    } else if (Stats.CurrentLanguage == 1) {
                        if (text.Length == 13) {
                            factor = 0.93f; factorOffsetForSpace = 0.008f;
                        } else if (text.Length == 14) {
                            factor = 0.88f; factorOffsetForSpace = 0.02f;
                        } else if (text.Length == 15) {
                            factor = 0.83f; factorOffsetForSpace = 0.025f;
                        } else if (text.Length == 16) {
                            factor = 0.77f; factorOffsetForSpace = 0.042f;
                        } else if (text.Length == 17) {
                            factor = 0.73f; factorOffsetForSpace = 0.031f;
                        } else if (text.Length == 18) {
                            factor = 0.69f; factorOffsetForSpace = 0.025f;
                        } else if (text.Length == 19) {
                            factor = 0.67f; factorOffsetForSpace = 0.019f;
                        } else if (text.Length == 20) {
                            factor = 0.64f; factorOffsetForSpace = 0.024f;
                        } else if (text.Length == 21) {
                            factor = 0.61f; factorOffsetForSpace = 0.018f;
                        }
                        factor += (this.GetCountSpace(text) * factorOffsetForSpace);
                    } else if (Stats.CurrentLanguage == 2) {
                        if (text.Length == 12) {
                            factor = 0.93f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.00005f;
                        } else if (text.Length == 13) {
                            factor = 0.87f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.0005f;
                        } else if (text.Length == 14) {
                            factor = 0.81f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.005f;
                        } else if (text.Length == 15) {
                            factor = 0.76f; factorOffsetForSpace = 0.028f; factorOffsetForEngAlphabet = 0.0055f;
                        } else if (text.Length == 16) {
                            factor = 0.71f; factorOffsetForSpace = 0.029f; factorOffsetForEngAlphabet = 0.0085f;
                        } else if (text.Length == 17) {
                            factor = 0.67f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.011f;
                        } else if (text.Length == 18) {
                            factor = 0.63f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.0115f;
                        } else if (text.Length == 19) {
                            factor = 0.6f; factorOffsetForSpace = 0.025f; factorOffsetForEngAlphabet = 0.012f;
                        } else if (text.Length == 20) {
                            factor = 0.57f; factorOffsetForSpace = 0.022f; factorOffsetForEngAlphabet = 0.0115f;
                        } else if (text.Length == 21) {
                            factor = 0.545f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.011f;
                        }
                        factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                    } else if (Stats.CurrentLanguage == 3) {
                        if (text.Length == 10) {
                            factor = 0.9f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = 0.0005f;
                        } else if (text.Length == 11) {
                            factor = 0.82f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.004f;
                        } else if (text.Length == 12) {
                            factor = 0.76f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.008f;
                        } else if (text.Length == 13) {
                            factor = 0.71f; factorOffsetForSpace = 0.046f; factorOffsetForEngAlphabet = 0.008f;
                        } else if (text.Length == 14) {
                            factor = 0.67f; factorOffsetForSpace = 0.041f; factorOffsetForEngAlphabet = 0.014f;
                        } else if (text.Length == 15) {
                            factor = 0.62f; factorOffsetForSpace = 0.036f; factorOffsetForEngAlphabet = 0.017f;
                        } else if (text.Length == 16) {
                            factor = 0.58f; factorOffsetForSpace = 0.033f; factorOffsetForEngAlphabet = 0.016f;
                        } else if (text.Length == 17) {
                            factor = 0.55f; factorOffsetForSpace = 0.027f; factorOffsetForEngAlphabet = 0.015f;
                        } else if (text.Length == 18) {
                            factor = 0.52f; factorOffsetForSpace = 0.026f; factorOffsetForEngAlphabet = 0.014f;
                        } else if (text.Length == 19) {
                            factor = 0.5f; factorOffsetForSpace = 0.0225f; factorOffsetForEngAlphabet = 0.013f;
                        } else if (text.Length == 20) {
                            factor = 0.47f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.013f;
                        } else if (text.Length == 21) {
                            factor = 0.45f; factorOffsetForSpace = 0.018f; factorOffsetForEngAlphabet = 0.012f;
                        }
                        factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                    } else if (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) {
                        if (text.Length == 10) {
                            factor = 0.9f; factorOffsetForSpace = 0.01f; factorOffsetForEngAlphabet = 0.002f;
                        } else if (text.Length == 11) {
                            factor = 0.82f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = 0.007f;
                        } else if (text.Length == 12) {
                            factor = 0.76f; factorOffsetForSpace = 0.03f; factorOffsetForEngAlphabet = 0.009f;
                        } else if (text.Length == 13) {
                            factor = 0.71f; factorOffsetForSpace = 0.046f; factorOffsetForEngAlphabet = 0.011f;
                        } else if (text.Length == 14) {
                            factor = 0.67f; factorOffsetForSpace = 0.041f; factorOffsetForEngAlphabet = 0.013f;
                        } else if (text.Length == 15) {
                            factor = 0.62f; factorOffsetForSpace = 0.036f; factorOffsetForEngAlphabet = 0.015f;
                        } else if (text.Length == 16) {
                            factor = 0.58f; factorOffsetForSpace = 0.033f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.017f : 0.015f;
                        } else if (text.Length == 17) {
                            factor = 0.55f; factorOffsetForSpace = 0.027f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.014f;
                        } else if (text.Length == 18) {
                            factor = 0.52f; factorOffsetForSpace = 0.026f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.013f;
                        } else if (text.Length == 19) {
                            factor = 0.5f; factorOffsetForSpace = 0.0225f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.02f : 0.0125f;
                        } else if (text.Length == 20) {
                            factor = 0.47f; factorOffsetForSpace = 0.02f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.0195f : 0.012f;
                        } else if (text.Length == 21) {
                            factor = 0.45f; factorOffsetForSpace = 0.018f; factorOffsetForEngAlphabet = this.LevelColor.IsEmpty ? 0.018f : 0.011f;
                        }
                        factor += (this.GetCountSpace(text) * factorOffsetForSpace) + (this.GetCountEngAlphabet(text) * factorOffsetForEngAlphabet);
                    }
                }
            }
            return factor;
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