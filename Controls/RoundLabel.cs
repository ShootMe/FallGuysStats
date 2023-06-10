using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace FallGuysStats {
    public class RoundLabel : Label {
        public int cornerRadius = 15;
        public Color borderColor = Color.DarkGray;
        public int borderWidth = 1;
        public Color backColor = Color.LightGray;
 
        public bool isFillLeftTop = false;
        public bool isFillRightTop = false;
        public bool isFillLeftBottom = false;
        public bool isFillRightBottom = false;
 
        public RoundLabel() {
            this.DoubleBuffered = true;
        }
 
        protected override void OnPaint(PaintEventArgs e) {
            //base.OnPaint(e);
            using(var graphicsPath = this.GetRoundRectangle(this.ClientRectangle)) {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
 
                var brush = new SolidBrush(this.backColor);
                var pen = new Pen(this.borderColor, this.borderWidth);
                e.Graphics.FillPath(brush, graphicsPath);
                e.Graphics.DrawPath(pen, graphicsPath);
 
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor);
            }
        }
 
        private GraphicsPath GetRoundRectangle(Rectangle rectangle) {
            GraphicsPath path = new GraphicsPath();
            if(this.isFillLeftTop) {
                path.AddLine(rectangle.X, rectangle.Y + this.cornerRadius, rectangle.Left, rectangle.Top);
                path.AddLine(rectangle.X, rectangle.Y, rectangle.Left + this.cornerRadius, rectangle.Top);
            } else {
                path.AddArc(rectangle.Left, rectangle.Top, this.cornerRadius, this.cornerRadius, 180, 90);
            }
            if(this.isFillRightTop) {
                path.AddLine(rectangle.Right - this.cornerRadius, rectangle.Top, rectangle.Right, rectangle.Top);
                path.AddLine(rectangle.Right, rectangle.Top, rectangle.Right, rectangle.Top + this.cornerRadius);
            } else {
                path.AddArc(rectangle.X + rectangle.Width - this.cornerRadius - this.borderWidth, rectangle.Y, this.cornerRadius, this.cornerRadius, 270, 90);
            }
            if(this.isFillRightBottom) {
                path.AddLine(rectangle.Right, rectangle.Bottom - cornerRadius, rectangle.Right, rectangle.Bottom);
                path.AddLine(rectangle.Right, rectangle.Bottom, rectangle.Right - this.cornerRadius, rectangle.Bottom);
            } else {
                path.AddArc(rectangle.X + rectangle.Width - this.cornerRadius - this.borderWidth, rectangle.Y + rectangle.Height - this.cornerRadius - this.borderWidth, this.cornerRadius, this.cornerRadius, 0, 90);
            }
            if(this.isFillLeftBottom) {
                path.AddLine(rectangle.Left + this.cornerRadius, rectangle.Bottom, rectangle.Left, rectangle.Bottom);
                path.AddLine(rectangle.Left, rectangle.Bottom, rectangle.Left, rectangle.Bottom - this.cornerRadius);
            } else {
                path.AddArc(rectangle.X, rectangle.Y + rectangle.Height - this.cornerRadius - this.borderWidth, this.cornerRadius, this.cornerRadius, 90, 90);
            }
 
            path.CloseAllFigures();
            return path;
        }
    }
}