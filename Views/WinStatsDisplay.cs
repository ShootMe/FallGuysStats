using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using ScottPlot;

namespace FallGuysStats {
    public partial class WinStatsDisplay : MetroFramework.Forms.MetroForm {
        public double[] dates, shows, finals, wins;
        public int manualSpacing = 1;
        public Stats StatsForm { get; set; }
        public WinStatsDisplay() {
            this.InitializeComponent();
        }

        private int switchGraphStyle;
        private double yMax;
        private ScottPlot.Plottable.ScatterPlot MyScatterPlot1, MyScatterPlot2, MyScatterPlot3;
        private ScottPlot.Plottable.BarPlot MyBarPlot1, MyBarPlot2, MyBarPlot3;
        private ScottPlot.Plottable.MarkerPlot HighlightedPoint;
        private ScottPlot.Plottable.Tooltip tooltip;

        private void WinStatsDisplay_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            //this.formsPlot.Plot.Title("Title");
            //this.formsPlot.Plot.XLabel("Horizontal Axis");
            //this.formsPlot.Plot.YLabel("Vertical Axis");

            if (this.dates != null) {
                this.yMax = this.shows.Max() < this.finals.Max() ? (this.finals.Max() < this.wins.Max() ? this.wins.Max() : this.finals.Max()) : this.shows.Max();
                this.MyBarPlot1 = this.formsPlot.Plot.AddBar(this.shows, this.dates, color: this.GetColorWithAlpha(this.chkShows.ForeColor, 255));
                this.MyBarPlot1.Label = Multilingual.GetWord("level_detail_shows");
                this.MyBarPlot2 = this.formsPlot.Plot.AddBar(this.finals, this.dates, color: this.GetColorWithAlpha(this.chkFinals.ForeColor, 255));
                this.MyBarPlot2.Label = Multilingual.GetWord("level_detail_finals");
                this.MyBarPlot3 = this.formsPlot.Plot.AddBar(this.wins, this.dates, color: this.GetColorWithAlpha(this.chkWins.ForeColor, 255));
                this.MyBarPlot3.Label = Multilingual.GetWord("level_detail_wins");
                
                this.MyScatterPlot1 = this.formsPlot.Plot.AddScatter(this.dates, this.shows, markerSize: 4, color: this.GetColorWithAlpha(this.chkShows.ForeColor, 255), label: Multilingual.GetWord("level_detail_shows"));
                this.MyScatterPlot2 = this.formsPlot.Plot.AddScatter(this.dates, this.finals, markerSize: 4, color: this.GetColorWithAlpha(this.chkFinals.ForeColor, 255), label: Multilingual.GetWord("level_detail_finals"));
                this.MyScatterPlot3 = this.formsPlot.Plot.AddScatter(this.dates, this.wins, markerSize: 4, color: this.GetColorWithAlpha(this.chkWins.ForeColor, 255), label: Multilingual.GetWord("level_detail_wins"));

                this.formsPlot.Plot.Legend(location: Alignment.UpperRight);
                this.formsPlot.Plot.XAxis.DateTimeFormat(true);
                
                this.formsPlot.Plot.XAxis.ManualTickSpacing((this.manualSpacing <= 0 ? 1 : this.manualSpacing), ScottPlot.Ticks.DateTimeUnit.Day);
                this.formsPlot.Plot.XAxis.TickLabelStyle(rotation: 45);
                //this.formsPlot.Plot.XAxis.SetSizeLimit(min: 50);
                this.formsPlot.Plot.SetAxisLimits(
                    yMin: (this.dates.Length / 14D) * -1,
                    yMax: this.yMax + (this.dates.Length / 14D),
                    xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                    xMax: DateTime.FromOADate(this.dates[this.dates.Length-1]).AddDays(4).ToOADate()
                );
                
                this.HighlightedPoint = this.formsPlot.Plot.AddPoint(0, 0);
                this.HighlightedPoint.Color = this.Theme == MetroThemeStyle.Light ? Color.SlateGray : Color.LightGray;
                this.HighlightedPoint.MarkerSize = 7;
                this.HighlightedPoint.MarkerShape = MarkerShape.openCircle;
                this.HighlightedPoint.IsVisible = false;

                this.formsPlot.Refresh();
                this.MyScatterPlot1.IsVisible = false;
                this.MyScatterPlot2.IsVisible = false;
                this.MyScatterPlot3.IsVisible = false;
                this.MyBarPlot1.IsVisible = false;
                this.MyBarPlot2.IsVisible = false;
                this.MyBarPlot3.IsVisible = false;
                
                this.chkWins.Checked = true;
                this.switchGraphStyle = this.StatsForm.CurrentSettings.WinPerDayGraphStyle;
                this.picSwitchGraphStyle.Image = this.switchGraphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : Properties.Resources.bar_plot_teal_icon;
                this.ChangeFormsPlotStyle(this.switchGraphStyle);
            } else {
                this.formsPlot.Refresh();
            }
        }

        private void ChangeFormsPlotStyle(int style) {
            if (style == 1) { // BarPlot
                this.StatsForm.CurrentSettings.WinPerDayGraphStyle = 1;
                this.MyBarPlot1.IsVisible = this.chkShows.Checked;
                this.MyBarPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot1.Label = Multilingual.GetWord("level_detail_shows");
                this.MyBarPlot2.Label = Multilingual.GetWord("level_detail_finals");
                this.MyBarPlot3.Label = Multilingual.GetWord("level_detail_wins");
                
                this.MyScatterPlot1.Color = this.GetColorWithAlpha(this.chkShows.ForeColor, 0);
                this.MyScatterPlot2.Color = this.GetColorWithAlpha(this.chkFinals.ForeColor, 0);
                this.MyScatterPlot3.Color = this.GetColorWithAlpha(this.chkWins.ForeColor, 0);
                this.MyScatterPlot1.Label = null;
                this.MyScatterPlot2.Label = null;
                this.MyScatterPlot3.Label = null;
                
                this.HighlightedPoint.MarkerShape = MarkerShape.none;
                
                this.formsPlot.Plot.SetAxisLimits(
                    yMin: (this.dates.Length / 14D) * -1,
                    yMax: this.yMax + (this.dates.Length / 14D),
                    xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                    xMax: DateTime.FromOADate(this.dates[this.dates.Length-1]).AddDays(4).ToOADate()
                );
            } else { // ScatterPlot
                this.StatsForm.CurrentSettings.WinPerDayGraphStyle = 0;
                this.MyBarPlot1.IsVisible = false;
                this.MyBarPlot2.IsVisible = false;
                this.MyBarPlot3.IsVisible = false;
                this.MyBarPlot1.Label = null;
                this.MyBarPlot2.Label = null;
                this.MyBarPlot3.Label = null;
                
                this.MyScatterPlot1.Color = this.GetColorWithAlpha(this.chkShows.ForeColor, 255);
                this.MyScatterPlot2.Color = this.GetColorWithAlpha(this.chkFinals.ForeColor, 255);
                this.MyScatterPlot3.Color = this.GetColorWithAlpha(this.chkWins.ForeColor, 255);
                this.MyScatterPlot1.IsVisible = this.chkShows.Checked;
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
                this.MyScatterPlot1.Label = Multilingual.GetWord("level_detail_shows");
                this.MyScatterPlot2.Label = Multilingual.GetWord("level_detail_finals");
                this.MyScatterPlot3.Label = Multilingual.GetWord("level_detail_wins");
                
                this.HighlightedPoint.MarkerShape = MarkerShape.openCircle;
                
                this.formsPlot.Plot.SetAxisLimits(
                    yMin: (this.dates.Length / 14D) * -1,
                    yMax: this.yMax + (this.dates.Length / 14D),
                    xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                    xMax: DateTime.FromOADate(this.dates[this.dates.Length-1]).AddDays(4).ToOADate()
                );
            }
            this.formsPlot.Plot.AxisZoom(.9, .9);
            this.formsPlot.Refresh();
        }

        private Color GetColorWithAlpha(Color color, int alpha) {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private double DistanceToPoint(double x1, double y1, double x2, double y2) {
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
                                                                 Environment.NewLine +
                                                                 (this.MyScatterPlot1.IsVisible ? $"{Multilingual.GetWord("level_detail_shows")} : {this.MyScatterPlot1.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}{Environment.NewLine}" : "") +
                                                                 (this.MyScatterPlot2.IsVisible ? $"{Multilingual.GetWord("level_detail_finals")} : {this.MyScatterPlot2.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}{Environment.NewLine}" : "") +
                                                                 (this.MyScatterPlot3.IsVisible ? $"{Multilingual.GetWord("level_detail_wins")} : {this.MyScatterPlot3.Ys[currentIndex]}{Multilingual.GetWord("main_inning")}" : ""),
                                                          x: this.HighlightedPoint.X, y: this.HighlightedPoint.Y);
            
            this.tooltip.BorderWidth = 1;
            this.tooltip.BorderColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(239, 49,51,56) : Color.FromArgb(239, 211,211,211);
            this.tooltip.FillColor = Color.FromArgb(239, 49,51,56);
            this.tooltip.Font.Size = 13;
            this.tooltip.Font.Color = Color.White;
            this.tooltip.ArrowSize = 5;
            
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
            this.picSwitchGraphStyle.Image = this.switchGraphStyle == 1 ? Properties.Resources.bar_plot_teal_icon : Properties.Resources.scatter_plot_teal_icon;
            this.chkWins.Theme = theme;
            this.chkFinals.Theme = theme;
            this.chkShows.Theme = theme;
            this.chkWins.ForeColor = theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
            this.chkFinals.ForeColor = theme == MetroThemeStyle.Light ? Color.DeepPink : Color.DeepPink;
            this.chkShows.ForeColor = theme == MetroThemeStyle.Light ? Color.RoyalBlue : Color.DodgerBlue;
            if (theme == MetroThemeStyle.Dark) {
                this.formsPlot.Plot.Style(ScottPlot.Style.Black);
                this.formsPlot.Plot.Style(figureBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(dataBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(tick: Color.WhiteSmoke);
            }
        }
        private void WinStatsDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
        private void picSwitchGraphStyle_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                this.switchGraphStyle = this.switchGraphStyle == 0 ? 1 : 0;
                this.picSwitchGraphStyle.Image = this.switchGraphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : Properties.Resources.bar_plot_teal_icon;
                if (this.dates == null) { return; }
                this.ChangeFormsPlotStyle(this.switchGraphStyle);
            }
        }
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot3.IsVisible = chkWins.Checked;
                this.MyBarPlot3.IsVisible = chkWins.Checked;
            } else {
                this.MyScatterPlot3.IsVisible = chkWins.Checked;
            }
            this.formsPlot.Refresh();
        }
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot2.IsVisible = chkFinals.Checked;
                this.MyBarPlot2.IsVisible = chkFinals.Checked;
            } else {
                this.MyScatterPlot2.IsVisible = chkFinals.Checked;
            }
            this.formsPlot.Refresh();
        }
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot1.IsVisible = chkShows.Checked;
                this.MyBarPlot1.IsVisible = chkShows.Checked;
            } else {
                this.MyScatterPlot1.IsVisible = chkShows.Checked;
            }
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
            } else if (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) { // Simplified Chinese & Traditional Chinese
                this.chkShows.Location =  new Point(1147, 38);
                this.chkFinals.Location = new Point(1053, 38);
                this.chkWins.Location =   new Point(961, 38);
            }
        }
    }
}