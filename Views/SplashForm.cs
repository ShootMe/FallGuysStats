using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
    public partial class SplashForm : Form {
        private PictureBox pBox;

        public SplashForm() {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            
            Image splashImage = Properties.Resources.splash_image;
            int factor = 5;
            Size splashSize = new Size(splashImage.Width / factor, splashImage.Height / factor);

            pBox = new PictureBox {
                Image = splashImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, 0),
                Size = splashSize
            };
            
            this.ClientSize = splashSize;
            this.Controls.Add(pBox);
        }
    }
}