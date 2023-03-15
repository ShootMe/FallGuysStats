using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FallGuysStats {
    public class ImageComboBox : ComboBox  {
        // Private Field
        #region Field
        private const int COLOR_ITEM_MARGIN_WIDTH  = 4;
        private const int COLOR_ITEM_MARGIN_HEIGHT = 4;
        private const int IMAGE_ITEM_MARGIN_WIDTH  = 2;
        private const int IMAGE_ITEM_MARGIN_HEIGHT = 2;
        private ToolTip toolTip = new ToolTip();
        #endregion

        #region SetColorData(colorArray)
        public void SetColorData(Color[] colorArray) {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.Items.Clear();

            foreach (Color color in colorArray) {
                this.Items.Add(color);
            }

            this.DrawItem += this.colorComboBox_DrawItem;
        }
        #endregion
        
        #region SetImageData(imageArray)
        public void SetImageData(Image[] imageArray) {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.Items.Clear();

            foreach(Image image in imageArray) {
                this.Items.Add(image);
            }

            this.MeasureItem += this.imageComboBox_MeasureItem;
            this.DrawItem    += this.imageComboBox_DrawItem;
        }
        #endregion
        
        #region SetImageItemData(imageItemArray)
        public void SetImageItemData(ImageItem[] imageItemArray) {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.Items.Clear();
            this.Items.AddRange(imageItemArray);

            this.MeasureItem += this.imageItemComboBox_MeasureItem;
            this.DrawItem    += this.imageItemComboBox_DrawItem;
        }
        #endregion

        #region colorComboBox_DrawItem(sender, e)
        private void colorComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if(e.Index < 0) { return; }

            e.DrawBackground();

            int height = e.Bounds.Height - 2 * COLOR_ITEM_MARGIN_HEIGHT;

            Rectangle rectangle = new Rectangle
            (
                e.Bounds.X + COLOR_ITEM_MARGIN_WIDTH,
                e.Bounds.Y + COLOR_ITEM_MARGIN_HEIGHT,
                height,
                height
            );

            ComboBox comboBox = sender as ComboBox;
            Color color = (Color)comboBox.Items[e.Index];

            using(SolidBrush brush = new SolidBrush(color)) {
                e.Graphics.FillRectangle(brush, rectangle);
            }

            e.Graphics.DrawRectangle(Pens.Black, rectangle);

            using(Font font = new Font(comboBox.Font.FontFamily, comboBox.Font.Size * 0.75f, FontStyle.Bold)) {
                using(StringFormat stringFormat = new StringFormat()) {
                    stringFormat.Alignment     = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    int x = height + 2 * COLOR_ITEM_MARGIN_WIDTH;
                    int y = e.Bounds.Y + e.Bounds.Height / 2;

                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    e.Graphics.DrawString(color.Name, font, Brushes.Black, x, y, stringFormat);
                }
            }

            e.DrawFocusRectangle();
        }
        #endregion
        
        #region imageComboBox_MeasureItem(sender, e)
        private void imageComboBox_MeasureItem(object sender, MeasureItemEventArgs e) {
            if(e.Index < 0) { return; }

            ComboBox comboBox = sender as ComboBox;
            Image image = (Image)comboBox.Items[e.Index];
            this.toolTip.SetToolTip(this, comboBox.SelectedText);
            
            e.ItemHeight = image.Height + 2 * IMAGE_ITEM_MARGIN_HEIGHT;
            e.ItemWidth  = image.Width  + 2 * IMAGE_ITEM_MARGIN_WIDTH;
        }
        #endregion
        
        #region imageComboBox_DrawItem(sender, e)
        private void imageComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if(e.Index < 0) { return; }

            e.DrawBackground();
            ComboBox comboBox = sender as ComboBox;
            this.toolTip.SetToolTip(this, comboBox.SelectedText);
            Image image = (Image)comboBox.Items[e.Index];

            float height = e.Bounds.Height - 2 * IMAGE_ITEM_MARGIN_HEIGHT;
            float scale  = height / image.Height;
            float width  = image.Width * scale;

            RectangleF rectangle = new RectangleF
            (
                e.Bounds.X + IMAGE_ITEM_MARGIN_WIDTH,
                e.Bounds.Y + IMAGE_ITEM_MARGIN_HEIGHT,
                width,
                height
            );

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.DrawImage(image, rectangle);
            e.DrawFocusRectangle();
        }
        #endregion
        
        #region imageItemComboBox_MeasureItem(sender, e)
        private void imageItemComboBox_MeasureItem(object sender, MeasureItemEventArgs e) {
            if(e.Index < 0) { return; }

            ComboBox comboBox = sender as ComboBox;
            ImageItem item = (ImageItem)comboBox.Items[e.Index];
            this.toolTip.SetToolTip(this, item.Text);
            item.MeasureItem(e);
        }
        #endregion
        
        #region imageItemComboBox_DrawItem(sender, e)
        private void imageItemComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if(e.Index < 0) { return; }

            ComboBox comboBox = sender as ComboBox;
            ImageItem item = (ImageItem)comboBox.Items[e.Index];
            this.toolTip.SetToolTip(this, item.Text);
            item.DrawItem(e);
        }
        #endregion
    }
}