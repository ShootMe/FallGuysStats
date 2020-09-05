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
        [DefaultValue(null)]
        public string TextRight { get; set; }
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
                    stringFormat.Alignment = StringAlignment.Near;
                    switch (TextAlign) {
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

                    g.DrawString(Text, Font, brFore, ClientRectangle, stringFormat);

                    if (!string.IsNullOrEmpty(TextRight)) {
                        stringFormat.Alignment = StringAlignment.Far;
                        g.DrawString(TextRight, Font, brFore, ClientRectangle, stringFormat);
                    }
                }
            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent) {
            base.OnPaintBackground(pevent);
        }
    }
}