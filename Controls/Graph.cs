using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    public class Graph : PictureBox {
        private DataTable dataSource;
        [Browsable(false)]
        public DataTable DataSource {
            get { return dataSource; }
            set {
                int ccount = dataSource != null ? dataSource.Columns.Count : 0;
                int ncount = value != null ? value.Columns.Count : 0;
                dataSource = value;
                if (dataSource != null) {
                    if (ccount != ncount) {
                        yColumns = new bool[dataSource.Columns.Count];
                        bool found = false;
                        for (int i = dataSource.Columns.Count - 1; i >= 0; i--) {
                            yColumns[i] = i != XColumn && !found;
                            if (yColumns[i])
                                found = true;
                        }
                        RefreshColors();
                    }
                }
                Invalidate();
            }
        }
        [DefaultValue(0)]
        public int XColumn { get; set; }
        private bool[] yColumns;
        [Browsable(false)]
        public bool[] YColumns { get { return yColumns; } }
        [DefaultValue(true)]
        public bool DrawPoints { get; set; }
        [DefaultValue(0)]
        private int opacity;
        public int Opacity { get { return opacity; } set { opacity = value; Invalidate(); } }
        [DefaultValue(typeof(Color), "Transparent")]
        private Color backColor;
        public Color BackgroundColor { get { return backColor; } set { backColor = value; Invalidate(); } }
        [Browsable(false)]
        public new Image InitialImage { get; set; }
        [Browsable(false)]
        public new Image ErrorImage { get; set; }
        [Browsable(false)]
        public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        private Brush[] brushes;
        private Pen[] pens;
        private int closeRowIndex, closeColumnIndex;
        private Point lastMousePosition;
        private static Color[] Colors = new Color[] { Color.Black, Color.Red, Color.Green, Color.Blue };

        public Graph() {
            closeRowIndex = -1;
            closeColumnIndex = -1;
            opacity = 0;
            backColor = Color.Transparent;
            XColumn = 0;
            DrawPoints = true;
        }

        public void RefreshColors() {
            if (dataSource == null) { return; }
            brushes = new Brush[dataSource.Columns.Count];
            pens = new Pen[dataSource.Columns.Count];
            foreach (DataColumn col in dataSource.Columns) {
                brushes[col.Ordinal] = new SolidBrush(Colors[col.Ordinal]);
                pens[col.Ordinal] = new Pen(brushes[col.Ordinal]);
            }
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            closeRowIndex = -1;
            closeColumnIndex = -1;
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (dataSource == null || dataSource.DefaultView.Count == 0) { return; }

            int w = Width; int h = Height;
            decimal xmax = decimal.MinValue; decimal xmin = decimal.MaxValue; decimal ymax = decimal.MinValue; decimal ymin = decimal.MaxValue;
            Type yType = null;
            Type xType = dataSource.Columns[XColumn].DataType;
            bool visible = false;
            //Determine min / max
            foreach (DataRowView row in dataSource.DefaultView) {
                decimal newx = GetValue(row[XColumn]);
                if (newx > xmax) { xmax = newx; }
                if (newx < xmin) { xmin = newx; }
                foreach (DataColumn col in dataSource.Columns) {
                    if (!yColumns[col.Ordinal]) { continue; }
                    visible = true;
                    yType = col.DataType;
                    decimal newy = GetValue(row[col.Ordinal]);
                    if (newy > ymax) { ymax = newy; }
                    if (newy < ymin) { ymin = newy; }
                }
            }
            if (!visible) { return; }
            ymin = 0;
            //Get bounds
            int wmax = 0; int wmin = 0; int hmin = 0; int hmax = 0;
            CalculateMinMax(xmin, xmax, xType, ymin, ymax, yType, ref wmin, ref wmax, ref hmin, ref hmax);
            int mod = (int)ymax % 8;
            ymax += mod == 0 ? 0 : 8 - mod;
            //Get inital values
            int closeInd = 0;
            int closeTemp = 0;
            int close = int.MaxValue;
            int closeIndY = 0;
            int i = 0;
            foreach (DataRowView row in dataSource.DefaultView) {
                int x = NormalizeX(GetValue(row[XColumn]), xmin, xmax, wmin, wmax) - e.X;
                closeTemp = x * x;
                foreach (DataColumn col in dataSource.Columns) {
                    if (!yColumns[col.Ordinal]) { continue; }
                    int y = NormalizeY(GetValue(row[col.Ordinal]), ymin, ymax, hmin, hmax) - e.Y;
                    y = closeTemp + y * y;
                    if (close > y) {
                        close = y;
                        closeIndY = col.Ordinal;
                        closeInd = i;
                    }
                }
                i++;
            }
            if (closeRowIndex != closeInd || closeColumnIndex != closeIndY) {
                closeRowIndex = closeInd;
                closeColumnIndex = closeIndY;
            }
            lastMousePosition = e.Location;
            Invalidate();
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;

            if (Parent != null) {
                base.BackColor = Color.Transparent;
                int index = Parent.Controls.GetChildIndex(this);

                for (int i = Parent.Controls.Count - 1; i > index; i--) {
                    Control c = Parent.Controls[i];
                    if (c.Bounds.IntersectsWith(Bounds) && c.Visible) {
                        Bitmap bmp = new Bitmap(c.Width, c.Height, g);
                        c.DrawToBitmap(bmp, c.ClientRectangle);

                        g.TranslateTransform(c.Left - Left, c.Top - Top);
                        g.DrawImageUnscaled(bmp, Point.Empty);
                        g.TranslateTransform(Left - c.Left, Top - c.Top);
                        bmp.Dispose();
                    }
                }
                g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackgroundColor)), this.ClientRectangle);
            } else {
                g.Clear(Color.Transparent);
                g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackgroundColor)), this.ClientRectangle);
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (dataSource == null || dataSource.DefaultView.Count == 0) {
                if (DesignMode) {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("X", typeof(int));
                    dt.Columns.Add("Y", typeof(int));
                    dt.Rows.Add(1, 1);
                    dt.Rows.Add(2, 8);
                    dt.Rows.Add(3, 5);
                    dt.Rows.Add(4, 3);
                    dt.Rows.Add(5, 10);
                    dt.Rows.Add(6, 8);
                    dt.Rows.Add(7, 3);
                    DataSource = dt;
                } else {
                    return;
                }
            }

            int w = Width; int h = Height;
            decimal xmax = decimal.MinValue; decimal xmin = decimal.MaxValue; decimal ymax = decimal.MinValue; decimal ymin = decimal.MaxValue;
            Type yType = null;
            Type xType = dataSource.Columns[XColumn].DataType;
            bool visible = false;
            //Determine min / max
            foreach (DataRowView row in dataSource.DefaultView) {
                decimal newx = GetValue(row[XColumn]);
                if (newx > xmax) { xmax = newx; }
                if (newx < xmin) { xmin = newx; }
                foreach (DataColumn col in dataSource.Columns) {
                    if (!yColumns[col.Ordinal]) { continue; }
                    visible = true;
                    yType = col.DataType;
                    decimal newy = GetValue(row[col.Ordinal]);
                    if (newy > ymax) { ymax = newy; }
                    if (newy < ymin) { ymin = newy; }
                }
            }
            if (!visible) {
                ymax = 10;
                ymin = 0;
            }
            ymin = 0;

            //Draw labels
            int wmax = 0; int wmin = 0; int hmin = 0; int hmax = 0;
            CalculateMinMax(xmin, xmax, xType, ymin, ymax, yType, ref wmin, ref wmax, ref hmin, ref hmax);
            int mod = (int)ymax % 8;
            ymax += mod == 0 ? 0 : 8 - mod;
            float sz = DefaultFont.SizeInPoints;
            Graphics g = e.Graphics;
            decimal y8 = (ymax - ymin) / (decimal)8.0; decimal x8 = (xmax - xmin) / (decimal)8.0;
            double h8 = (double)(hmax - hmin) / (double)8.0; double w8 = (double)(wmax - wmin) / (double)8.0;
            Pen bp = new Pen(Color.Black, 1);
            bp.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(bp, wmin, 0, wmin, hmax);
            g.DrawLine(bp, wmin, hmax, w - 1, hmax);
            bp.Color = Color.FromArgb(30, 0, 0, 0);
            for (int i = 0; i <= 8; i++) {
                string xval = GetRepresentation(xType, x8 * i + xmin, xmax - xmin);
                float xsz = TextRenderer.MeasureText(xval, DefaultFont).Width;
                float tx = (float)(w8 * i + wmin);
                g.DrawString(xval, DefaultFont, Brushes.Black, tx + 2 - xsz / (float)2.0, hmax + 2);
                if (i > 0)
                    g.DrawLine(bp, tx, 0, tx, hmax - 1);
                float ty = (float)(h - 3 * sz - h8 * i);
                g.DrawString((y8 * i + ymin).ToString("0"), DefaultFont, Brushes.Black, (float)2, ty);
                if (i > 0)
                    g.DrawLine(bp, wmin + 1, ty + sz, w - 1, ty + sz);
            }

            //Draw data
            DataRowView init = dataSource.DefaultView[0];
            int x = NormalizeX(GetValue(init[XColumn]), xmin, xmax, wmin, wmax);
            int[] y = new int[brushes.Length];
            foreach (DataColumn col in dataSource.Columns) {
                if (!yColumns[col.Ordinal]) { continue; }
                y[col.Ordinal] = NormalizeY(GetValue(init[col.Ordinal]), ymin, ymax, hmin, hmax);
                if (DrawPoints) { g.FillRectangle(brushes[col.Ordinal], x - 2, y[col.Ordinal] - 2, 4, 4); }
            }

            bool start = true;
            foreach (DataRowView row in dataSource.DefaultView) {
                if (start) { start = false; continue; }
                int newx = NormalizeX(GetValue(row[XColumn]), xmin, xmax, wmin, wmax);
                foreach (DataColumn col in dataSource.Columns) {
                    if (!yColumns[col.Ordinal]) { continue; }
                    int newy = NormalizeY(GetValue(row[col.Ordinal]), ymin, ymax, hmin, hmax);
                    if (DrawPoints) { g.FillRectangle(brushes[col.Ordinal], newx - 2, newy - 2, 4, 4); }
                    g.DrawLine(pens[col.Ordinal], x, y[col.Ordinal], newx, newy);
                    y[col.Ordinal] = newy;
                }
                x = newx;
            }

            if (closeRowIndex >= 0) {
                g.DrawLine(Pens.Black, lastMousePosition, new Point(NormalizeX(GetValue(dataSource.DefaultView[closeRowIndex][XColumn]), xmin, xmax, wmin, wmax), NormalizeY(GetValue(dataSource.DefaultView[closeRowIndex][closeColumnIndex]), ymin, ymax, hmin, hmax)));
                string val = dataSource.Columns[closeColumnIndex].ColumnName + " = " + dataSource.DefaultView[closeRowIndex][closeColumnIndex].ToString() + " (" + GetRepresentation(xType, GetValue(dataSource.DefaultView[closeRowIndex][XColumn]), xmax - xmin) + ")";
                Size size = TextRenderer.MeasureText(val, DefaultFont);
                int px = lastMousePosition.X + size.Width > w ? w - size.Width : lastMousePosition.X;
                int py = lastMousePosition.Y - size.Height < 0 ? 0 : lastMousePosition.Y - size.Height;
                g.DrawString(val, DefaultFont, Brushes.Black, px, py);
            }
            e.Dispose();
        }
        private void CalculateMinMax(decimal xmin, decimal xmax, Type xType, decimal ymin, decimal ymax, Type yType, ref int wmin, ref int wmax, ref int hmin, ref int hmax) {
            int ysz = TextRenderer.MeasureText(GetRepresentation(yType, ymin, ymax - ymin), DefaultFont).Width;
            int ysz2 = TextRenderer.MeasureText(GetRepresentation(yType, ymax, ymax - ymin), DefaultFont).Width;
            ysz = ysz > ysz2 ? ysz : ysz2;
            ysz += TextRenderer.MeasureText("00", DefaultFont).Width;
            int xsz = TextRenderer.MeasureText(GetRepresentation(xType, xmax, xmax - xmin), DefaultFont).Width;
            int xsz2 = TextRenderer.MeasureText(GetRepresentation(xType, xmin, xmax - xmin), DefaultFont).Width / 2;
            ysz = ysz > xsz2 ? ysz : xsz2;
            float sz = DefaultFont.SizeInPoints;
            decimal xdiff = xmax - xmin; decimal ydiff = ymax - ymin;
            wmax = (int)(Width - (double)xsz / 2.0);
            wmin = ysz;
            hmin = (int)sz;
            hmax = (int)(Height - 2 * sz);
        }
        private int NormalizeX(decimal x, decimal xmin, decimal xmax, int wmin, int wmax) {
            double point = xmax - xmin == 0 ? 0 : (double)(x - xmin) / (double)(xmax - xmin);
            return (int)((wmax - wmin) * point + wmin);
        }
        private int NormalizeY(decimal y, decimal ymin, decimal ymax, int hmin, int hmax) {
            double point = ymax - ymin == 0 ? 0 : (double)(y - ymin) / (double)(ymax - ymin);
            return (int)(hmax - (double)(hmax - hmin) * point);
        }
        private decimal GetValue(object value) {
            if (value == null) {
                return 0;
            } else if (value is DateTime) {
                return ((DateTime)value).Ticks;
            } else if (value is int) {
                return (int)value;
            } else if (value is long) {
                return (long)value;
            } else if (value is double) {
                return new decimal((double)value);
            } else if (value is float) {
                return new decimal((float)value);
            } else if (value is short) {
                return (short)value;
            } else if (value is byte) {
                return (byte)value;
            } else {
                return 0;
            }
        }
        private string GetRepresentation(Type t, decimal value, decimal range) {
            if (t == typeof(DateTime)) {
                if (TimeSpan.FromTicks((long)range).Days > 0) {
                    return new DateTime((long)value).ToString("yy-MM-dd");
                } else {
                    return new DateTime((long)value).ToString("HH:mm");
                }
            } else if (t == typeof(int)) {
                return ((int)value).ToString();
            } else if (t == typeof(long)) {
                return ((long)value).ToString();
            } else if (t == typeof(byte)) {
                return ((byte)value).ToString();
            } else if (t == typeof(short)) {
                return ((int)value).ToString();
            }
            return value.ToString();
        }
    }
}