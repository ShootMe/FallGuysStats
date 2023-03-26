using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FallGuysStats {
    public class ImageItem {
        #region Public Field
        public Image Image;
        public string[] ResourceName;
        public string Text;
        public Font Font;
        public bool IsCustomized;
        #endregion

        #region Private Field
        private const int MARGIN_WIDTH = 2;
        private const int MARGIN_HEIGHT = 2;
        private int width;
        private int height;
        private bool sizeCalculated = false;
        #endregion

        #region Public Constructor - ImageItem(image, text, font)
        public ImageItem(Image image, string[] resourceName, string text, Font font, bool isCustomized) {
            this.Image = image;
            this.ResourceName = resourceName;
            this.Text = text;
            this.Font = font;
            this.IsCustomized = isCustomized;
        }
        #endregion

        #region Public Method - MeasureItem(e)
        public void MeasureItem(MeasureItemEventArgs e) {
            if (!this.sizeCalculated) {
                this.sizeCalculated = true;
                SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font);
                this.height = (2 * MARGIN_HEIGHT) + (int)Math.Max(this.Image.Height, textSize.Height);
                this.width = (int)((4 * MARGIN_WIDTH) + this.Image.Width + textSize.Width);
            }
            e.ItemWidth = this.width;
            e.ItemHeight = this.height;
        }
        #endregion

        #region Public Method - DrawItem(e)
        public void DrawItem(DrawItemEventArgs e, Color foreColor) {
            e.DrawBackground();

            float height = e.Bounds.Height - (2 * MARGIN_HEIGHT);
            float scale = height / this.Image.Height;
            float width = this.Image.Width * scale;

            RectangleF rectangle = new RectangleF
            (
                e.Bounds.X + MARGIN_WIDTH,
                e.Bounds.Y + MARGIN_HEIGHT,
                width,
                height
            );
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.DrawImage(this.Image, rectangle);
            //if (e.Bounds.Height <= 25) {
            //    e.Graphics.DrawImage(e.Index > 0 ? this.BrightnessImage(this.Image, 60) : this.Image, rectangle);
            //} else {
            //    e.Graphics.DrawImage(this.Image, rectangle);
            //}

            if (e.Bounds.Height <= 25) {
                //string visibleText = this.Text;

                //if(e.Bounds.Height < this.Image.Height) {
                //    visibleText = Text.Substring(0, this.Text.IndexOf('\n'));
                //}

                width = e.Bounds.Width - rectangle.Right - (3 * MARGIN_WIDTH);

                rectangle = new RectangleF
                (
                    rectangle.Right + (2 * MARGIN_WIDTH),
                    rectangle.Y,
                    width,
                    height
                );

                using (StringFormat stringFormat = new StringFormat()) {
                    stringFormat.Alignment = StringAlignment.Far;
                    stringFormat.LineAlignment = StringAlignment.Far;

                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(foreColor), rectangle, stringFormat);
                }

                //e.Graphics.DrawRectangle(Pens.Blue, Rectangle.Round(rectangle));
            }

            e.DrawFocusRectangle();
        }
        #endregion

        #region private Method - BrightnessImage(imgSource, iBrightness)
        private Bitmap BrightnessImage(Image imgSource, int iBrightness) {
            Bitmap btTmp = new Bitmap(imgSource);
            Color cTmp;
            int iR, iG, iB;
            for (int iY = 0; iY < btTmp.Height; iY++) {
                for (int iX = 0; iX < btTmp.Width; iX++) {
                    cTmp = btTmp.GetPixel(iX, iY);

                    if (cTmp.A != 255) continue;

                    iR = Math.Max(0, Math.Min(255, cTmp.R + iBrightness));
                    iG = Math.Max(0, Math.Min(255, cTmp.G + iBrightness));
                    iB = Math.Max(0, Math.Min(255, cTmp.B + iBrightness));

                    cTmp = Color.FromArgb(iR, iG, iB);
                    btTmp.SetPixel(iX, iY, cTmp);
                }
            }
            return btTmp;
        }
        #endregion
    }
}