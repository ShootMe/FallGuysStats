﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public sealed class ImageComboBox : ComboBox {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        private MetroThemeStyle _theme = MetroThemeStyle.Light;
        public MetroThemeStyle Theme {
            get { return _theme; }
            set {
                if (_theme != value) {
                    _theme = value;
                    SetBlur();
                    Invalidate();
                    Update();
                }
            }
        }
        
        private string _name = String.Empty;
        public string SelectedName {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                }
            }
        }
        
        private Image _image;
        public Image SelectedImage {
            get { return _image; }
            set {
                if (_image != value) {
                    _image = value;
                }
            }
        }
        
        private Color _borderColor = Color.Gray;
        private Color BorderColor {
            get { return _borderColor; }
            set {
                if (_borderColor != value) {
                    _borderColor = value;
                    Invalidate();
                    Update();
                }
            }
        }
        
        private Color _buttonColor = Color.DarkGray;
        private Color ButtonColor
        {
            get { return _buttonColor; }
            set {
                if (_buttonColor != value) {
                    _buttonColor = value;
                    Invalidate();
                    Update();
                }
            }
        }
        
        protected override void WndProc(ref Message m) {
            // if (m.Msg == WM_PAINT && DropDownStyle != ComboBoxStyle.Simple) {
            if (m.Msg == WM_PAINT) {
                Rectangle clientRect = ClientRectangle;
                int dropDownButtonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
                Rectangle outerBorder = new Rectangle(clientRect.Location, new Size(clientRect.Width - 1, clientRect.Height - 1));
                Rectangle innerBorder = new Rectangle(outerBorder.X + 1, outerBorder.Y + 1, outerBorder.Width - dropDownButtonWidth - 2, outerBorder.Height - 2);
                Rectangle innerInnerBorder = new Rectangle(innerBorder.X + 1, innerBorder.Y + 1, innerBorder.Width - 2, innerBorder.Height - 2);
                Rectangle dropDownRect = new Rectangle(innerBorder.Right + 1, innerBorder.Y, dropDownButtonWidth, innerBorder.Height + 1);
                if (RightToLeft == RightToLeft.Yes) {
                    innerBorder.X = clientRect.Width - innerBorder.Right;
                    innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
                    dropDownRect.X = clientRect.Width - dropDownRect.Right;
                    dropDownRect.Width += 1;
                }
                Color innerBorderColor = Enabled ? BackColor : SystemColors.Control;
                Color outerBorderColor = Enabled ? BorderColor : SystemColors.ControlDark;
                Color buttonColor = Enabled ? ButtonColor : SystemColors.Control;
                Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
                Point[] arrow = {
                    //new Point(middle.X - 3, middle.Y - 2),
                    //new Point(middle.X + 4, middle.Y - 2),
                    //new Point(middle.X, middle.Y + 2)
                    new Point(middle.X - 10, middle.Y - 2),
                    new Point(middle.X + 1, middle.Y - 2),
                    new Point(middle.X - 5, middle.Y + 4)
                };
                PAINTSTRUCT ps = new PAINTSTRUCT();
                bool shoulEndPaint = false;
                IntPtr dc;
                if (m.WParam == IntPtr.Zero) {
                    dc = BeginPaint(Handle, ref ps);
                    m.WParam = dc;
                    shoulEndPaint = true;
                } else {
                    dc = m.WParam;
                }
                IntPtr rgn = CreateRectRgn(innerInnerBorder.Left, innerInnerBorder.Top, innerInnerBorder.Right, innerInnerBorder.Bottom);
                SelectClipRgn(dc, rgn);
                DefWndProc(ref m);
                DeleteObject(rgn);
                rgn = CreateRectRgn(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
                SelectClipRgn(dc, rgn);
                using (Graphics g = Graphics.FromHdc(dc)) {
                    //using (var b = new SolidBrush(buttonColor)) {
                    using (SolidBrush b = new SolidBrush(Color.Transparent)) {
                        g.FillRectangle(b, dropDownRect);
                    }
                    using (SolidBrush b = new SolidBrush(buttonColor)) {
                        g.FillPolygon(b, arrow);
                    }
                    //using (var p = new Pen(innerBorderColor)) {
                    using (Pen p = new Pen(Color.Transparent)) {
                        g.DrawRectangle(p, innerBorder);
                        g.DrawRectangle(p, innerInnerBorder);
                    }
                    using (Pen p = new Pen(outerBorderColor)) {
                        g.DrawRectangle(p, outerBorder);
                    }
                }
                if (shoulEndPaint) { EndPaint(Handle, ref ps); }
                DeleteObject(rgn);
            } else {
                base.WndProc(ref m);
            }
        }

        private const int WM_PAINT = 0xF;
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int L, T, R, B;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT {
            public IntPtr hdc;
            public bool fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd, [In, Out] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("user32.dll")]
        public static extern int GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool fErase);
        public enum RegionFlags {
            ERROR = 0,
            NULLREGION = 1,
            SIMPLEREGION = 2,
            COMPLEXREGION = 3,
        }
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
        
        #region Private Field
        private const int COLOR_ITEM_MARGIN_WIDTH = 4;
        private const int COLOR_ITEM_MARGIN_HEIGHT = 4;
        private const int IMAGE_ITEM_MARGIN_WIDTH = 2;
        private const int IMAGE_ITEM_MARGIN_HEIGHT = 2;
        #endregion
        
        #region SetColorData(colorArray)
        public void SetColorData(Color[] colorArray) {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.Items.Clear();

            foreach (Color color in colorArray) {
                this.Items.Add(color);
            }

            this.DrawItem += this.ColorComboBox_DrawItem;
        }
        #endregion
        
        #region SetImageData(imageArray)
        public void SetImageData(Image[] imageArray) {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.Items.Clear();

            foreach (Image image in imageArray) {
                this.Items.Add(image);
            }

            this.MeasureItem += this.ImageComboBox_MeasureItem;
            this.DrawItem += this.ImageComboBox_DrawItem;
        }
        #endregion
        
        #region SetImageItemData(imageItemArray)
        public void SetImageItemData(List<ImageItem> imageItemArray) {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.Items.Clear();
            this.Items.AddRange(imageItemArray.ToArray());

            this.MeasureItem += this.ImageItemComboBox_MeasureItem;
            this.DrawItem += this.ImageItemComboBox_DrawItem;

            this.MouseEnter += ImageItemComboBox_MouseEnter;
            this.MouseLeave += ImageItemComboBox_MouseLeave;
            this.GotFocus += ImageItemComboBox_GotFocus;
            this.LostFocus += ImageItemComboBox_LostFocus;
            
            this.SelectedIndexChanged += ImageItemComboBox_SelectedIndexChanged;
            
            this.SetBlur();
        }
        #endregion
        
        #region SelectedImageItem(data, dataIndex)
        public void SelectedImageItem(string data, int dataIndex = 0) {
            int index = this.Items.Cast<ImageItem>()
                .Select((item, idx) => new { Item = item, Index = idx })
                .FirstOrDefault(x => x.Item.Data[dataIndex].Split(';').Any(d => d == data))?.Index ?? -1;

            if (index != -1) {
                this.SelectedIndex = index;
            }
        }
        #endregion

        #region ColorComboBox_DrawItem(sender, e)
        private void ColorComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if(e.Index < 0) return;

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
        
        #region ImageComboBox_MeasureItem(sender, e)
        private void ImageComboBox_MeasureItem(object sender, MeasureItemEventArgs e) {
            if (e.Index < 0) return;

            ComboBox comboBox = sender as ComboBox;
            Image image = (Image)comboBox.Items[e.Index];
            
            e.ItemHeight = image.Height + 2 * IMAGE_ITEM_MARGIN_HEIGHT;
            e.ItemWidth = image.Width  + 2 * IMAGE_ITEM_MARGIN_WIDTH;
        }
        #endregion
        
        #region ImageComboBox_DrawItem(sender, e)
        private void ImageComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0) return;

            e.DrawBackground();
            ComboBox comboBox = sender as ComboBox;
            Image image = (Image)comboBox.Items[e.Index];

            float height = e.Bounds.Height - 2 * IMAGE_ITEM_MARGIN_HEIGHT;
            float scale = height / image.Height;
            float width = image.Width * scale;

            RectangleF rectangle = new RectangleF (
                e.Bounds.X + IMAGE_ITEM_MARGIN_WIDTH,
                e.Bounds.Y + IMAGE_ITEM_MARGIN_HEIGHT,
                width,
                height
            );

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawImage(image, rectangle);
            e.DrawFocusRectangle();
        }
        #endregion
        
        #region ImageItemComboBox_MeasureItem(sender, e)
        private void ImageItemComboBox_MeasureItem(object sender, MeasureItemEventArgs e) {
            if (e.Index < 0) return;

            ComboBox comboBox = sender as ComboBox;
            ImageItem item = (ImageItem)comboBox.Items[e.Index];
            item.MeasureItem(e);
        }
        #endregion
        
        #region ImageItemComboBox_DrawItem(sender, e)
        private void ImageItemComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0) return;
            
            ComboBox comboBox = sender as ComboBox;
            ImageItem item = (ImageItem)comboBox.Items[e.Index];
            item.DrawItem(e, this.ForeColor, this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17));
        }
        
        private void ImageItemComboBox_MouseEnter(object sender, EventArgs e) {
            if (!this.Focused) this.SetFocus();
        }
        
        private void ImageItemComboBox_MouseLeave(object sender, EventArgs e) {
            if (!this.Focused) this.SetBlur();
        }
        
        private void ImageItemComboBox_GotFocus(object sender, EventArgs e) {
            this.SetFocus();
        }
        
        private void ImageItemComboBox_LostFocus(object sender, EventArgs e) {
            this.SetBlur();
        }
        
        private void ImageItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.SelectedIndex == -1) return;

            this.SelectedName = ((ImageItem)((ImageComboBox)sender).SelectedItem).Text;
            this.SelectedImage = ((ImageItem)((ImageComboBox)sender).SelectedItem).Image;
        }

        private void SetFocus() {
            this.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.FromArgb(204, 204, 204);
            this.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.BorderColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(17, 17, 17) : Color.FromArgb(204, 204, 204);
            this.ButtonColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(17, 17, 17) : Color.FromArgb(170, 170, 170);
        }
        
        private void SetBlur() {
            this.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(153, 153, 153) : Color.DarkGray;
            this.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.BorderColor = Color.FromArgb(153, 153, 153);
            this.ButtonColor = Color.FromArgb(153, 153, 153);
        }
        #endregion
    }
}