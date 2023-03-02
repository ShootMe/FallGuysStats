using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
namespace FallGuysStats {
    public class Graph : PictureBox {
        private DataTable dataSource;
        [Browsable(false)]
        public DataTable DataSource {
            get { return this.dataSource; }
            set {
                int ccount = this.dataSource != null ? this.dataSource.Columns.Count : 0;
                int ncount = value != null ? value.Columns.Count : 0;
                this.dataSource = value;
                if (this.dataSource != null) {
                    if (ccount != ncount) {
                        yColumns = new bool[this.dataSource.Columns.Count];
                        bool found = false;
                        for (int i = this.dataSource.Columns.Count - 1; i >= 0; i--) {
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
        private static Color[] Colors = { Color.Black, Color.Red, Color.Green, Color.Blue };
        private Font GraphFont = new Font(Overlay.DefaultFontCollection.Families[Stats.CurrentLanguage == 4 ? 1 : 0], 10, FontStyle.Regular, GraphicsUnit.Point);

        public Graph() {
            this.closeRowIndex = -1;
            this.closeColumnIndex = -1;
            this.opacity = 0;
            this.backColor = Color.Transparent;
            this.XColumn = 0;
            this.DrawPoints = true;
        }

        public void RefreshColors() {
            if (this.dataSource == null) { return; }
            brushes = new Brush[this.dataSource.Columns.Count];
            pens = new Pen[this.dataSource.Columns.Count];
            foreach (DataColumn col in this.dataSource.Columns) {
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

            if (this.dataSource == null || this.dataSource.DefaultView.Count == 0) { return; }

            int w = Width; int h = Height;
            decimal xmax = decimal.MinValue; decimal xmin = decimal.MaxValue; decimal ymax = decimal.MinValue; decimal ymin = decimal.MaxValue;
            Type yType = null;
            Type xType = this.dataSource.Columns[XColumn].DataType;
            bool visible = false;
            //Determine min / max
            foreach (DataRowView row in this.dataSource.DefaultView) {
                decimal newx = GetValue(row[XColumn]);
                if (newx > xmax) { xmax = newx; }
                if (newx < xmin) { xmin = newx; }
                foreach (DataColumn col in this.dataSource.Columns) {
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
            foreach (DataRowView row in this.dataSource.DefaultView) {
                int x = NormalizeX(GetValue(row[XColumn]), xmin, xmax, wmin, wmax) - e.X;
                closeTemp = x * x;
                foreach (DataColumn col in this.dataSource.Columns) {
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
            this.lastMousePosition = e.Location;
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
            if (this.dataSource == null || this.dataSource.DefaultView.Count == 0) {
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
            Type xType = this.dataSource.Columns[XColumn].DataType;
            bool visible = false;
            //Determine min / max
            foreach (DataRowView row in this.dataSource.DefaultView) {
                decimal newx = GetValue(row[XColumn]);
                if (newx > xmax) { xmax = newx; }
                if (newx < xmin) { xmin = newx; }
                foreach (DataColumn col in this.dataSource.Columns) {
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
            float sz = GraphFont.SizeInPoints;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            decimal y8 = (ymax - ymin) / (decimal)8.0; decimal x8 = (xmax - xmin) / (decimal)8.0;
            double h8 = (hmax - hmin) / 8.0; double w8 = (wmax - wmin) / 8.0;
            Pen bp = new Pen(Color.Black, 1);
            bp.DashStyle = DashStyle.Dash;
            g.DrawLine(bp, wmin, 0, wmin, hmax);
            g.DrawLine(bp, wmin, hmax, w - 1, hmax);
            bp.Color = Color.FromArgb(30, 0, 0, 0);
            for (int i = 0; i <= 8; i++) {
                string xval = GetRepresentation(xType, x8 * i + xmin, xmax - xmin);
                float xsz = TextRenderer.MeasureText(xval, GraphFont).Width;
                float tx = (float)(w8 * i + wmin);
                g.DrawString(xval, GraphFont, Brushes.Black, tx + 2 - xsz / (float)2.0, hmax + 2);
                if (i > 0)
                    g.DrawLine(bp, tx, 0, tx, hmax - 1);
                float ty = (float)(h - 3 * sz - h8 * i);
                g.DrawString($"{(y8 * i + ymin):0}{Multilingual.GetWord("main_inning")}", GraphFont, Brushes.Black, 2, ty);
                if (i > 0)
                    g.DrawLine(bp, wmin + 1, ty + sz, w - 1, ty + sz);
            }

            //Draw data
            DataRowView init = this.dataSource.DefaultView[0];
            int x = NormalizeX(GetValue(init[XColumn]), xmin, xmax, wmin, wmax);
            int[] y = new int[brushes.Length];
            foreach (DataColumn col in this.dataSource.Columns) {
                if (!yColumns[col.Ordinal]) { continue; }
                y[col.Ordinal] = NormalizeY(GetValue(init[col.Ordinal]), ymin, ymax, hmin, hmax);
                if (DrawPoints) { this.FillRoundedRectangle(g, pens[col.Ordinal], brushes[col.Ordinal], x - 2, y[col.Ordinal] - 2, 4, 4, 4); }
            }

            bool start = true;
            foreach (DataRowView row in this.dataSource.DefaultView) {
                if (start) { start = false; continue; }
                int newx = NormalizeX(GetValue(row[XColumn]), xmin, xmax, wmin, wmax);
                foreach (DataColumn col in this.dataSource.Columns) {
                    if (!yColumns[col.Ordinal]) { continue; }
                    int newy = NormalizeY(GetValue(row[col.Ordinal]), ymin, ymax, hmin, hmax);
                    if (DrawPoints) { this.FillRoundedRectangle(g, pens[col.Ordinal], brushes[col.Ordinal], newx - 2, newy - 2, 4, 4, 4); }
                    g.DrawLine(pens[col.Ordinal], x, y[col.Ordinal], newx, newy);
                    y[col.Ordinal] = newy;
                }
                x = newx;
            }

            if (closeRowIndex >= 0) {
                g.DrawLine(new Pen(Color.FromArgb(95, 255, 0, 255), 0), this.lastMousePosition, new Point(NormalizeX(GetValue(this.dataSource.DefaultView[closeRowIndex][XColumn]), xmin, xmax, wmin, wmax), NormalizeY(GetValue(this.dataSource.DefaultView[closeRowIndex][closeColumnIndex]), ymin, ymax, hmin, hmax)));
                string summaryTitle = GetRepresentation(xType, GetValue(this.dataSource.DefaultView[closeRowIndex][XColumn]), xmax - xmin);
                string summaryWins = string.Empty;
                string summaryFinals = string.Empty;
                string summaryShows = string.Empty;
                int sizeWidth = TextRenderer.MeasureText(summaryTitle, this.GraphFont).Width;
                int sizeHeight = TextRenderer.MeasureText(summaryTitle, this.GraphFont).Height;
                
                // Shows
                if (yColumns[3]) {
                    summaryShows += $"{Environment.NewLine}{this.dataSource.Columns[3].ColumnName} : {this.dataSource.DefaultView[closeRowIndex][3]}{Multilingual.GetWord("main_inning")}";
                    if (sizeWidth < TextRenderer.MeasureText(summaryShows, this.GraphFont).Width) {
                        sizeWidth = TextRenderer.MeasureText(summaryShows, this.GraphFont).Width;
                    }
                    sizeHeight = TextRenderer.MeasureText(summaryShows, this.GraphFont).Height;
                }
                // Finals
                if (yColumns[2]) {
                    if (yColumns[3]) {
                        summaryFinals += Environment.NewLine;
                    }
                    summaryFinals += $"{Environment.NewLine}{this.dataSource.Columns[2].ColumnName} : {this.dataSource.DefaultView[closeRowIndex][2]}{Multilingual.GetWord("main_inning")}";
                    if (sizeWidth < TextRenderer.MeasureText(summaryFinals, this.GraphFont).Width) {
                        sizeWidth = TextRenderer.MeasureText(summaryFinals, this.GraphFont).Width;
                    }
                    sizeHeight = TextRenderer.MeasureText(summaryFinals, this.GraphFont).Height;
                }
                // Wins
                if (yColumns[1]) {
                    if (yColumns[3]) {
                        summaryWins += Environment.NewLine;
                    }
                    if (yColumns[2]) {
                        summaryWins += Environment.NewLine;
                    }
                    summaryWins += $"{Environment.NewLine}{this.dataSource.Columns[1].ColumnName} : {this.dataSource.DefaultView[closeRowIndex][1]}{Multilingual.GetWord("main_inning")}";
                    if (sizeWidth < TextRenderer.MeasureText(summaryWins, this.GraphFont).Width) {
                        sizeWidth = TextRenderer.MeasureText(summaryWins, this.GraphFont).Width;
                    }
                    sizeHeight = TextRenderer.MeasureText(summaryWins, this.GraphFont).Height;
                }

                int px = this.lastMousePosition.X + sizeWidth > w ? w - sizeWidth : this.lastMousePosition.X;
                int py = this.lastMousePosition.Y - sizeHeight < 0 ? 0 : this.lastMousePosition.Y - sizeHeight;

                this.FillRoundedRectangle(g, new Pen(Color.FromArgb(95, 255, 0, 255), 0), new SolidBrush(Color.FromArgb(223, 255, 255, 255)), px - 6, py - 6, sizeWidth + 12, sizeHeight + 12, 10);
                g.DrawString(summaryTitle, this.GraphFont, Brushes.Black, px, py);
                if (yColumns[1]) g.DrawString(summaryWins, this.GraphFont, Brushes.Red, px, py);
                if (yColumns[2]) g.DrawString(summaryFinals, this.GraphFont, Brushes.Green, px, py);
                if (yColumns[3]) g.DrawString(summaryShows, this.GraphFont, Brushes.Blue, px, py);
            }
            e.Dispose();
        }
        private void FillRoundedRectangle(Graphics g, Pen pen, Brush brush, int x, int y, int width, int height, int radius) {
            Rectangle corner = new Rectangle(x, y, radius, radius);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(corner, 180, 90);
            corner.X = x + width - radius;
            path.AddArc(corner, 270, 90);
            corner.Y = y + height - radius;
            path.AddArc(corner, 0, 90);
            corner.X = x;
            path.AddArc(corner, 90, 90);
            path.CloseFigure();
            g.FillPath(brush, path);
            if(pen != null) {
                g.DrawPath(pen, path);
            }
        }
        private void CalculateMinMax(decimal xmin, decimal xmax, Type xType, decimal ymin, decimal ymax, Type yType, ref int wmin, ref int wmax, ref int hmin, ref int hmax) {
            int ysz = TextRenderer.MeasureText(GetRepresentation(yType, ymin, ymax - ymin), this.GraphFont).Width;
            int ysz2 = TextRenderer.MeasureText(GetRepresentation(yType, ymax, ymax - ymin), this.GraphFont).Width;
            ysz = ysz > ysz2 ? ysz : ysz2;
            ysz += TextRenderer.MeasureText("00", this.GraphFont).Width;
            int xsz = TextRenderer.MeasureText(GetRepresentation(xType, xmax, xmax - xmin), this.GraphFont).Width;
            int xsz2 = TextRenderer.MeasureText(GetRepresentation(xType, xmin, xmax - xmin), this.GraphFont).Width / 2;
            ysz = ysz > xsz2 ? ysz : xsz2;
            float sz = this.GraphFont.SizeInPoints;
            //decimal xdiff = xmax - xmin; decimal ydiff = ymax - ymin;
            wmax = Width - xsz / 2;
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
            return (int)(hmax - (hmax - hmin) * point);
        }
        private decimal GetValue(object value) {
            switch (value) {
                case null:
                    return 0;
                case DateTime time:
                    return time.Ticks;
                case int i:
                    return i;
                case long l:
                    return l;
                case double d:
                    return new decimal(d);
                case float f:
                    return new decimal(f);
                case short s:
                    return s;
                case byte b:
                    return b;
                default:
                    return 0;
            }
        }
        private string GetRepresentation(Type t, decimal value, decimal range) {
            if (t == typeof(DateTime)) {
                if (TimeSpan.FromTicks((long)range).Days > 0) {
                    return new DateTime((long)value).ToString(Multilingual.GetWord("level_date_format"));
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