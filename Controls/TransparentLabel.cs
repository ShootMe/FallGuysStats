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
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
        public TransparentLabel() {
            this.DrawVisible = true;
            this.TextRight = null;
            this.Visible = false;
            this.IsIcon = false;
        }
        [DefaultValue(null)]
        public string TextRight { get; set; }
        [DefaultValue(true)]
        public bool DrawVisible { get; set; }
        public bool IsIcon { get; set; }
        public int ImageX { get; set; }
        public int ImageY { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public Color LevelColor { get; set; }
        public void Draw(Graphics g) {
            if (!this.DrawVisible) { return; }
            if (this.IsIcon) {
                using (SolidBrush brFore = new SolidBrush(this.ForeColor)) {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Far;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.DrawImage(this.Image, this.ImageX, this.ImageY, this.ImageWidth, this.ImageHeight);
                    if (this.TextRight != null) {
                        g.DrawString(this.TextRight, new Font(this.Font.FontFamily, 8.75F, FontStyle.Regular, GraphicsUnit.Point), brFore, this.ClientRectangle, stringFormat);
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

                        g.DrawString(this.Text, this.Font, brFore, this.ClientRectangle, stringFormat);
                        
                        if (this.Image != null) {
                            g.DrawImage(this.Image, this.ImageX, this.ImageY, this.ImageWidth, this.ImageHeight);
                        }

                        if (!string.IsNullOrEmpty(this.TextRight)) {
                            stringFormat.Alignment = StringAlignment.Far;
                            if (this.Name.Equals("lblName")) {
                                if (!this.LevelColor.IsEmpty) {
                                    int sizeOfText = TextRenderer.MeasureText(this.TextRight, this.GetFontForLongText()).Width;
                                    Pen pen = new Pen(this.LevelColor, 0);
                                    pen.Alignment = PenAlignment.Right;
                                    this.FillRoundedRectangle(g, pen, new SolidBrush(this.LevelColor), (ClientRectangle.Width - sizeOfText), ClientRectangle.Y-1, sizeOfText, 22, 10);
                                }
                                g.DrawString(this.TextRight, this.GetFontForLongText(), brFore, this.ClientRectangle, stringFormat);
                            } else {
                                g.DrawString(this.TextRight, this.Font, brFore, this.ClientRectangle, stringFormat);
                            }
                        }
                    }
                }
            }
        }
        private Font GetFontForLongText() {
            return (Stats.CurrentLanguage == 3 && this.TextRight.Length > 10 ||
                    Stats.CurrentLanguage == 2 && this.TextRight.Length > 13)
                ? new Font(this.Font.FontFamily, this.GetRoundNameFontSize(this.TextRight.Length), FontStyle.Regular, GraphicsUnit.Point)
                : this.Font;
        }
        private float GetRoundNameFontSize(int textLength) {
            int offset = 21;
            float weight = 1.1F;
            if (Stats.CurrentLanguage == 3 && textLength > 10) { // Japanese
                offset = 21;
                if (textLength == 11) {
                    weight = 1.1F;
                } else if (textLength == 12) {
                    weight = 1.2F;
                } else if (textLength == 13) {
                    weight = 1.25F;
                } else if (textLength == 14) {
                    weight = 1.325F;
                } else if (textLength == 15) {
                    weight = 1.425F;
                }
            } else if (Stats.CurrentLanguage == 2 && textLength > 13) { // Korean
                offset = 24;
                if (textLength == 14) {
                    weight = 1.275F;
                } else if (textLength == 15) {
                    weight = 1.275F;
                }
            }
            return (offset - textLength) * weight;
        }
        private void FillRoundedRectangle(Graphics g, Pen pen, Brush brush, int x, int y, int width, int height, int radius) {
            Rectangle corner = new Rectangle(x, y, radius, radius);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(corner, 180, 90);
            corner.X = x + width - radius;
            path.AddArc(corner, 270, 90);
            corner.Y = y + height - radius;
            path.AddArc(corner, 0, 90);
            corner.X = x;
            path.AddArc(corner, 90, 90);
            path.CloseFigure();
            g.FillPath(brush, path);
            if(pen != null) {
                g.DrawPath(pen, path);
            }
        }
    }
}