using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
    public partial class SplashForm : Form {
        private PictureBox pBox;
        // private ProgressBar progressBar;

        public SplashForm() {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ClientSize = new Size(399, 224);
            this.ShowInTaskbar = false;

            pBox = new PictureBox {
                Image = Properties.Resources.splash_image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, -38),
                Size = new Size(400, 300)
            };
            this.Controls.Add(pBox);
        }
    }
}