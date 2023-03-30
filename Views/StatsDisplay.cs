using System;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using ScottPlot;

namespace FallGuysStats {
    public partial class StatsDisplay : MetroFramework.Forms.MetroForm {
        public double[] dates, shows, finals, wins;
        public int manualSpacing = 1;
        public Stats StatsForm { get; set; }
        public StatsDisplay() {
            this.InitializeComponent();
        }
        private ScottPlot.Plottable.ScatterPlot MyScatterPlot1;
        private ScottPlot.Plottable.ScatterPlot MyScatterPlot2;
        private ScottPlot.Plottable.ScatterPlot MyScatterPlot3;
        private ScottPlot.Plottable.MarkerPlot HighlightedPoint;
        private ScottPlot.Plottable.Tooltip tooltip;

        private void StatsDisplay_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            //this.formsPlot.Plot.Title("Title");
            //this.formsPlot.Plot.XLabel("Horizontal Axis");
            //this.formsPlot.Plot.YLabel("Vertical Axis");

            if (this.dates != null) {
                this.MyScatterPlot1 = this.formsPlot.Plot.AddScatter(this.dates, this.shows, color: this.chkShows.ForeColor, label: Multilingual.GetWord("level_detail_shows"));
                this.MyScatterPlot2 = this.formsPlot.Plot.AddScatter(this.dates, this.finals, color: this.chkFinals.ForeColor, label: Multilingual.GetWord("level_detail_finals"));
                this.MyScatterPlot3 = this.formsPlot.Plot.AddScatter(this.dates, this.wins, color: this.chkWins.ForeColor, label: Multilingual.GetWord("level_detail_wins"));
                this.formsPlot.Plot.Legend();
                this.formsPlot.Plot.XAxis.DateTimeFormat(true);
                
                this.formsPlot.Plot.XAxis.ManualTickSpacing((this.manualSpacing <= 0 ? 1 : this.manualSpacing), ScottPlot.Ticks.DateTimeUnit.Day);
                this.formsPlot.Plot.XAxis.TickLabelStyle(rotation: 45);
                this.formsPlot.Plot.XAxis.SetSizeLimit(min: 50);
                
                this.HighlightedPoint = this.formsPlot.Plot.AddPoint(0, 0);
                this.HighlightedPoint.Color = this.Theme == MetroThemeStyle.Light ? Color.Magenta : Color.Chartreuse;
                this.HighlightedPoint.MarkerSize = 10;
                this.HighlightedPoint.MarkerShape = MarkerShape.openCircle;
                this.HighlightedPoint.IsVisible = false;
                
                this.formsPlot.Refresh();
                this.MyScatterPlot1.IsVisible = false;
                this.MyScatterPlot2.IsVisible = false;
                this.MyScatterPlot3.IsVisible = false;
                
                this.chkWins.Checked = true;
            } else {
                this.formsPlot.Refresh();
            }
        }

        public double DistanceToPoint(double x1, double y1, double x2, double y2) {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private void formsPlot_MouseMove(object sender, MouseEventArgs e) {
            if (this.dates == null) { return; }
            if (!(this.MyScatterPlot1.IsVisible || this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible)) { return; }
            this.formsPlot.Plot.Remove(this.tooltip);
            
            (double mouseCoordX, double mouseCoordY) = this.formsPlot.GetMouseCoordinates();
            double xyRatio = this.formsPlot.Plot.XAxis.Dims.PxPerUnit / this.formsPlot.Plot.YAxis.Dims.PxPerUnit;
            
            (double pointX1, double pointY1, int pointIndex1) = MyScatterPlot1.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
            (double pointX2, double pointY2, int pointIndex2) = MyScatterPlot2.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
            (double pointX3, double pointY3, int pointIndex3) = MyScatterPlot3.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

            int currentIndex = -1;
            double ans = -1;
            int p = 0;
            if (this.MyScatterPlot1.IsVisible) {
                double d = this.DistanceToPoint(mouseCoordX,mouseCoordY,pointX1,pointY1);
                if (ans == -1 || ans > d) {
                    ans = d;
                    p += 1;
                }
            }
            
            if (this.MyScatterPlot2.IsVisible) {
                double d = this.DistanceToPoint(mouseCoordX,mouseCoordY,pointX2,pointY2);
                if (ans == -1 || ans > d) {
                    ans = d;
                    p += 2;
                }
            }
            
            if (this.MyScatterPlot3.IsVisible) {
                double d = this.DistanceToPoint(mouseCoordX,mouseCoordY,pointX3,pointY3);
                if (ans == -1 || ans > d) {
                    p += 4;
                }
            }

            switch (p) {
                case 1:
                    this.HighlightedPoint.X = pointX1;
                    this.HighlightedPoint.Y = pointY1;
                    currentIndex = pointIndex1;
                    break;
                case 2:
                    this.HighlightedPoint.X = pointX2;
                    this.HighlightedPoint.Y = pointY2;
                    currentIndex = pointIndex2;
                    break;
                case 3:
                    this.HighlightedPoint.X = pointX2;
                    this.HighlightedPoint.Y = pointY2;
                    currentIndex = pointIndex2;
                    break;
                case 4:
                    this.HighlightedPoint.X = pointX3;
                    this.HighlightedPoint.Y = pointY3;
                    currentIndex = pointIndex3;
                    break;
                case 5:
                    this.HighlightedPoint.X = pointX3;
                    this.HighlightedPoint.Y = pointY3;
                    currentIndex = pointIndex3;
                    break;
                case 6:
                    this.HighlightedPoint.X = pointX3;
                    this.HighlightedPoint.Y = pointY3;
                    currentIndex = pointIndex3;
                    break;
                case 7:
                    this.HighlightedPoint.X = pointX3;
                    this.HighlightedPoint.Y = pointY3;
                    currentIndex = pointIndex3;
                    break;
            }
            
            this.tooltip = this.formsPlot.Plot.AddTooltip(label: ($"{DateTime.FromOADate(this.MyScatterPlot1.Xs[currentIndex]).ToString(Multilingual.GetWord("level_date_format"))}{Environment.NewLine}") +
                                                                 (this.MyScatterPlot1.IsVisible ? $"{Multilingual.GetWord("level_detail_shows")} : {this.MyScatterPlot1.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}{Environment.NewLine}" : "") +
                                                                 (this.MyScatterPlot2.IsVisible ? $"{Multilingual.GetWord("level_detail_finals")} : {this.MyScatterPlot2.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}{Environment.NewLine}" : "") +
                                                                 (this.MyScatterPlot3.IsVisible ? $"{Multilingual.GetWord("level_detail_wins")} : {this.MyScatterPlot3.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}" : ""),
                x: this.HighlightedPoint.X, y: this.HighlightedPoint.Y);
            
            this.HighlightedPoint.IsVisible = true;
            this.formsPlot.Render();
        }

        private void formsPlot_MouseLeave(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (!(this.MyScatterPlot1.IsVisible || this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible)) { return; }
            this.HighlightedPoint.IsVisible = false;
            this.HighlightedPoint.X = 0;
            this.HighlightedPoint.Y = 0;
            this.formsPlot.Plot.Remove(this.tooltip);
            this.formsPlot.Refresh();
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.chkWins.Theme = theme;
            this.chkFinals.Theme = theme;
            this.chkShows.Theme = theme;
            if (theme == MetroThemeStyle.Light) {
                this.chkWins.ForeColor =  Color.Red;
                this.chkFinals.ForeColor =  Color.Green;
                this.chkShows.ForeColor =  Color.Blue;
            } else if (theme == MetroThemeStyle.Dark) {
                this.chkWins.ForeColor = Color.Gold;
                this.chkFinals.ForeColor = Color.DeepPink;
                this.chkShows.ForeColor = Color.DodgerBlue;
                
                this.formsPlot.Plot.Style(ScottPlot.Style.Black);
                this.formsPlot.Plot.Style(figureBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(dataBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(tick: Color.WhiteSmoke);
            }
        }
        private void StatsDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            this.MyScatterPlot3.IsVisible = chkWins.Checked;
            this.formsPlot.Refresh();
        }
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            this.MyScatterPlot2.IsVisible = chkFinals.Checked;
            this.formsPlot.Refresh();
        }
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            this.MyScatterPlot1.IsVisible = chkShows.Checked;
            this.formsPlot.Refresh();
        }
        private void ChangeLanguage() {
            this.chkWins.Text = Multilingual.GetWord("level_detail_wins");
            this.chkFinals.Text = Multilingual.GetWord("level_detail_finals");
            this.chkShows.Text = Multilingual.GetWord("level_detail_shows");
            if (Stats.CurrentLanguage == 0) { // English
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1053, 38);
                this.chkWins.Location =   new Point(961, 38);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1060, 38);
                this.chkWins.Location =   new Point(961, 38);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1048, 38);
                this.chkWins.Location =   new Point(961, 38);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1048, 38);
                this.chkWins.Location =   new Point(961, 38);
            } else if (Stats.CurrentLanguage == 4) { // Simplified Chinese
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1053, 38);
                this.chkWins.Location =   new Point(961, 38);
            }
        }
    }
}