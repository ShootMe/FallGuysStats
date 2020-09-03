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
        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;
            Draw(g);
        }
        public void Draw(Graphics g) {
            using (SolidBrush brBack = new SolidBrush(BackColor)) {
                using (SolidBrush brFore = new SolidBrush(ForeColor)) {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    StringFormat stringFormat = new StringFormat();
                    switch (TextAlign) {
                        case ContentAlignment.BottomLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.BottomCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.BottomRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.MiddleLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.MiddleCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.MiddleRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.TopLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.TopCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.TopRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                    }

                    g.DrawString(Text, Font, brFore, ClientRectangle, stringFormat);
                }
            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent) {
            base.OnPaintBackground(pevent);
        }
    }
}