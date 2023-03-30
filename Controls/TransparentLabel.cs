using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
namespace FallGuysStats {
    public class TransparentLabel : Label {
        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
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
        public void Draw(Graphics g) {
            if (!this.DrawVisible) { return; }
            if (this.PlatformIcon != null) {
                using (SolidBrush brFore = new SolidBrush(this.ForeColor)) {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Far;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.DrawImage(this.PlatformIcon, this.ImageX, this.ImageY, this.ImageWidth, this.ImageHeight);
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

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;
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
                            if (this.Name.Equals("lblRound")) {
                                if (!this.LevelColor.IsEmpty) {
                                    this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size, this.Text, stringFormat);
                                    //g.DrawString(this.Text, this.Font, brFore, this.ClientRectangle.X, this.ClientRectangle.Y + ((Stats.CurrentLanguage == 2 || Stats.CurrentLanguage == 3) ? 10 : 0), stringFormat);
                                    
                                } else {
                                    this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size, this.Text, stringFormat);
                                    //g.DrawString(this.Text, this.Font, brFore, this.ClientRectangle, stringFormat);
                                }
                            } else {
                                this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size, this.Text, stringFormat);
                                //g.DrawString(this.Text, this.Font, brFore, this.ClientRectangle, stringFormat);
                            }
                        }

                        if (this.Image != null) {
                            g.DrawImage(this.Image, this.ImageX, this.ImageY, this.ImageWidth, this.ImageHeight);
                        }

                        if (!string.IsNullOrEmpty(this.TextRight)) {
                            stringFormat.Alignment = StringAlignment.Far;
                            if (this.Name.Equals("lblRound")) {
                                Font fontForLongText = this.GetFontForLongText(this.TextRight);
                                if (!this.LevelColor.IsEmpty) {
                                    //float sizeOfText = g.MeasureString(this.TextRight, fontForLongText).Width;
                                    float widthOfText = TextRenderer.MeasureText(this.TextRight, fontForLongText).Width;
                                    this.FillRoundedRectangle(g, null, new SolidBrush(this.LevelColor), (int)(this.ClientRectangle.Width - widthOfText), this.ClientRectangle.Y, (int)widthOfText, 22, 10);
                                    if (this.RoundIcon != null) {
                                        g.DrawImage(this.RoundIcon, (this.ClientRectangle.Width - widthOfText - this.ImageWidth) - 5, this.ClientRectangle.Y, this.ImageWidth, this.ImageHeight);
                                    }
                                }

                                brFore.Color = this.LevelColor.IsEmpty ? this.ForeColor : Color.White;
                                this.DrawOutlineText(g, this.ClientRectangle, null, brFore, fontForLongText.FontFamily, fontForLongText.Style, fontForLongText.Size, this.TextRight, stringFormat);
                                //g.DrawString(this.TextRight, this.GetFontForLongText(this.TextRight), brFore, this.ClientRectangle, stringFormat);
                            } else {
                                this.DrawOutlineText(g, this.ClientRectangle, null, brFore, this.Font.FontFamily, this.Font.Style, this.Font.Size, this.TextRight, stringFormat);
                                //g.DrawString(this.TextRight, this.Font, brFore, this.ClientRectangle, stringFormat);
                            }
                        }
                    }
                }
            }
        }
        private Color GetComplementaryColor(Color source, int alpha) {
            return Color.FromArgb(alpha,255 - source.R, 255 - source.G, 255 - source.B);
        }
        private Font GetFontForLongText(string text) {
            return (Stats.CurrentLanguage == 2 && text.Length > 12 || Stats.CurrentLanguage == 3 && text.Length > 9)
                ? new Font(this.Font.FontFamily, this.GetRoundNameFontSize(text.Length, 21), this.Font.Style, GraphicsUnit.Pixel)
                : this.Font;
        }
        private float GetRoundNameFontSize(int textLength, int offset) {
            float weight = 1.0F;
            if (Stats.CurrentLanguage == 3 && textLength > 9) { // Japanese
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
            } else if (Stats.CurrentLanguage == 2 && textLength > 12) { // Korean
                offset += 3;
                if (textLength == 13) {
                    weight = 1.15F;
                } else if (textLength == 14) {
                    weight = 1.2F;
                } else if (textLength == 15) {
                    weight = 1.225F;
                }
            }
            return (offset - textLength) * weight;
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
                if (pen != null) {
                    g.DrawPath(pen, path);
                }
            }
        }
        private void DrawOutlineText(Graphics g, Rectangle layoutRect, Pen outlinePen, Brush fillBrush, FontFamily fontFamily, FontStyle fontStyle, float fontSize, string text, StringFormat stringFormat) {
            using(GraphicsPath path = new GraphicsPath()) {
                path.AddString(text, fontFamily, (int)fontStyle, fontSize, layoutRect, stringFormat);
                path.CloseFigure();
                g.FillPath(fillBrush, path);
                if(outlinePen != null) g.DrawPath(outlinePen, path);
            }
        }
    }
}