using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
    public partial class SplashForm : Form {
        private PictureBox pBox;
        // private ProgressBar progressBar;

        public SplashForm() {
            Image splashImage = Properties.Resources.splash_image;

            pBox = new PictureBox {
                Image = splashImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, 0),
                Size = new Size((int)(splashImage.Width / 2.5), (int)(splashImage.Height / 2.5))
            };

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ClientSize = new Size((int)(splashImage.Width / 2.5), (int)(splashImage.Height / 2.5));
            this.ShowInTaskbar = false;

            this.Controls.Add(pBox);
        }
    }
}