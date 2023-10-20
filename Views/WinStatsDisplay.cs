using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework;
using ScottPlot;
using ScottPlot.Plottable;

namespace FallGuysStats {
    public partial class WinStatsDisplay : MetroFramework.Forms.MetroForm {
        public double[] dates, shows, finals, wins;
        public Dictionary<double, SortedList<string, int>> winsInfo;
        public int manualSpacing = 1;
        public Stats StatsForm { get; set; }
        public WinStatsDisplay() {
            this.InitializeComponent();
        }

        private int switchGraphStyle;
        private double yMax;
        private ScatterPlot MyScatterPlot1, MyScatterPlot2, MyScatterPlot3;
        private BarPlot MyBarPlot1, MyBarPlot2, MyBarPlot3;
        private LollipopPlot MyLollipopPlot1, MyLollipopPlot2, MyLollipopPlot3;
        private MarkerPlot HighlightedPoint;
        private Tooltip tooltip;

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
                this.MyBarPlot2 = this.formsPlot.Plot.AddBar(this.finals, this.dates, color: this.GetColorWithAlpha(this.chkFinals.ForeColor, 255));
                this.MyBarPlot3 = this.formsPlot.Plot.AddBar(this.wins, this.dates, color: this.GetColorWithAlpha(this.chkWins.ForeColor, 255));
                
                this.MyScatterPlot1 = this.formsPlot.Plot.AddScatter(this.dates, this.shows, markerSize: 4, color: this.GetColorWithAlpha(this.chkShows.ForeColor, 255), label: Multilingual.GetWord("level_detail_shows"));
                this.MyScatterPlot2 = this.formsPlot.Plot.AddScatter(this.dates, this.finals, markerSize: 4, color: this.GetColorWithAlpha(this.chkFinals.ForeColor, 255), label: Multilingual.GetWord("level_detail_finals"));
                this.MyScatterPlot3 = this.formsPlot.Plot.AddScatter(this.dates, this.wins, markerSize: 4, color: this.GetColorWithAlpha(this.chkWins.ForeColor, 255), label: Multilingual.GetWord("level_detail_wins"));
                
                this.MyLollipopPlot1 = this.formsPlot.Plot.AddLollipop(this.shows, this.dates, color: this.GetColorWithAlpha(this.chkShows.ForeColor, 255));
                this.MyLollipopPlot2 = this.formsPlot.Plot.AddLollipop(this.finals, this.dates, color: this.GetColorWithAlpha(this.chkFinals.ForeColor, 255));
                this.MyLollipopPlot3 = this.formsPlot.Plot.AddLollipop(this.wins, this.dates, color: this.GetColorWithAlpha(this.chkWins.ForeColor, 255));

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
                // this.HighlightedPoint.Color = this.Theme == MetroThemeStyle.Light ? Color.SlateGray : Color.LightGray;
                this.HighlightedPoint.Color = Color.Red;
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
                this.MyLollipopPlot1.IsVisible = false;
                this.MyLollipopPlot2.IsVisible = false;
                this.MyLollipopPlot3.IsVisible = false;
                
                this.chkShows.Checked = true;
                this.chkFinals.Checked = true;
                this.chkWins.Checked = true;
                this.switchGraphStyle = this.StatsForm.CurrentSettings.WinPerDayGraphStyle;
                this.picSwitchGraphStyle.Image = this.switchGraphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.switchGraphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
                this.ChangeFormsPlotStyle(this.switchGraphStyle);
            } else {
                this.formsPlot.Refresh();
            }
        }

        private void ChangeFormsPlotStyle(int style) {
            this.StatsForm.CurrentSettings.WinPerDayGraphStyle = style;
            if (style == 1) { // LollipopPlot
                this.MyBarPlot1.IsVisible = false;
                this.MyBarPlot2.IsVisible = false;
                this.MyBarPlot3.IsVisible = false;
                this.MyBarPlot1.Label = null;
                this.MyBarPlot2.Label = null;
                this.MyBarPlot3.Label = null;
                
                this.MyLollipopPlot1.IsVisible = this.chkShows.Checked;
                this.MyLollipopPlot2.IsVisible = this.chkFinals.Checked;
                this.MyLollipopPlot3.IsVisible = this.chkWins.Checked;
                this.MyLollipopPlot1.Label = Multilingual.GetWord("level_detail_shows");
                this.MyLollipopPlot2.Label = Multilingual.GetWord("level_detail_finals");
                this.MyLollipopPlot3.Label = Multilingual.GetWord("level_detail_wins");
                
                this.MyScatterPlot1.Color = this.GetColorWithAlpha(this.chkShows.ForeColor, 0);
                this.MyScatterPlot2.Color = this.GetColorWithAlpha(this.chkFinals.ForeColor, 0);
                this.MyScatterPlot3.Color = this.GetColorWithAlpha(this.chkWins.ForeColor, 0);
                this.MyScatterPlot1.Label = null;
                this.MyScatterPlot2.Label = null;
                this.MyScatterPlot3.Label = null;
                
                this.HighlightedPoint.MarkerShape = MarkerShape.none;
            } else if (style == 2) { // BarPlot
                this.MyBarPlot1.IsVisible = this.chkShows.Checked;
                this.MyBarPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot1.Label = Multilingual.GetWord("level_detail_shows");
                this.MyBarPlot2.Label = Multilingual.GetWord("level_detail_finals");
                this.MyBarPlot3.Label = Multilingual.GetWord("level_detail_wins");
                
                this.MyLollipopPlot1.IsVisible = false;
                this.MyLollipopPlot2.IsVisible = false;
                this.MyLollipopPlot3.IsVisible = false;
                this.MyLollipopPlot1.Label = null;
                this.MyLollipopPlot2.Label = null;
                this.MyLollipopPlot3.Label = null;
                
                this.MyScatterPlot1.Color = this.GetColorWithAlpha(this.chkShows.ForeColor, 0);
                this.MyScatterPlot2.Color = this.GetColorWithAlpha(this.chkFinals.ForeColor, 0);
                this.MyScatterPlot3.Color = this.GetColorWithAlpha(this.chkWins.ForeColor, 0);
                this.MyScatterPlot1.Label = null;
                this.MyScatterPlot2.Label = null;
                this.MyScatterPlot3.Label = null;
                
                this.HighlightedPoint.MarkerShape = MarkerShape.none;
            } else { // ScatterPlot
                this.MyBarPlot1.IsVisible = false;
                this.MyBarPlot2.IsVisible = false;
                this.MyBarPlot3.IsVisible = false;
                this.MyBarPlot1.Label = null;
                this.MyBarPlot2.Label = null;
                this.MyBarPlot3.Label = null;
                
                this.MyLollipopPlot1.IsVisible = false;
                this.MyLollipopPlot2.IsVisible = false;
                this.MyLollipopPlot3.IsVisible = false;
                this.MyLollipopPlot1.Label = null;
                this.MyLollipopPlot2.Label = null;
                this.MyLollipopPlot3.Label = null;
                
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
            }
            this.formsPlot.Plot.SetAxisLimits(
                yMin: (this.dates.Length / 14D) * -1,
                yMax: this.yMax + (this.dates.Length / 14D),
                xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                xMax: DateTime.FromOADate(this.dates[this.dates.Length-1]).AddDays(4).ToOADate()
            );
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
                case 3:
                    this.HighlightedPoint.X = pointX2;
                    this.HighlightedPoint.Y = pointY2;
                    currentIndex = pointIndex2;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    this.HighlightedPoint.X = pointX3;
                    this.HighlightedPoint.Y = pointY3;
                    currentIndex = pointIndex3;
                    break;
            }
            
            this.HighlightedPoint.IsVisible = true;
            if (this.MyScatterPlot1.Ys[currentIndex] > 0 || this.MyScatterPlot2.Ys[currentIndex] > 0 || this.MyScatterPlot3.Ys[currentIndex] > 0) {
                StringBuilder builder = new StringBuilder();
                builder.Append($" {DateTime.FromOADate(this.MyScatterPlot1.Xs[currentIndex]).ToString(Multilingual.GetWord("level_date_format"))}{Environment.NewLine}{Environment.NewLine}");
                builder.Append((this.MyScatterPlot1.IsVisible ? $" - {Multilingual.GetWord("level_detail_shows")} :  ⟪ {this.MyScatterPlot1.Ys[currentIndex]:N0}{Multilingual.GetWord("main_inning")} ⟫{(this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible ? Environment.NewLine : "")}" : ""));
                builder.Append((this.MyScatterPlot2.IsVisible ? $" - {Multilingual.GetWord("level_detail_finals")} :  ⟪ {this.MyScatterPlot2.Ys[currentIndex]:N0}{Multilingual.GetWord("main_inning")} {(this.MyScatterPlot1.Ys[currentIndex] > 0 ? $"/ {this.MyScatterPlot1.Ys[currentIndex]:N0}{Multilingual.GetWord("main_inning")} ⟫ - {Math.Truncate(this.MyScatterPlot2.Ys[currentIndex] * 100d / this.MyScatterPlot1.Ys[currentIndex] * 10) / 10}% " : "")}{(this.MyScatterPlot3.IsVisible ? Environment.NewLine : "")}" : ""));
                builder.Append((this.MyScatterPlot3.IsVisible ? $" - {Multilingual.GetWord("level_detail_wins")} :  ⟪ {this.MyScatterPlot3.Ys[currentIndex]:N0}{Multilingual.GetWord("main_inning")} {(this.MyScatterPlot1.Ys[currentIndex] > 0 ? $"/ {this.MyScatterPlot1.Ys[currentIndex]:N0}{Multilingual.GetWord("main_inning")} ⟫ - {Math.Truncate(this.MyScatterPlot3.Ys[currentIndex] * 100d / this.MyScatterPlot1.Ys[currentIndex] * 10) / 10}% " : "")}" : ""));
                if (this.winsInfo.ContainsKey(this.MyScatterPlot1.Xs[currentIndex])) {
                    SortedList<string, int> infos = this.winsInfo[this.MyScatterPlot1.Xs[currentIndex]];
                    int winsCount = infos.Where(kv => kv.Key.EndsWith(";crown")).Sum(kv => kv.Value);
                    int lossesCount = infos.Where(kv => kv.Key.EndsWith(";eliminated")).Sum(kv => kv.Value);
                    int winLevelCount = infos.Keys.Count(s => s.EndsWith(";crown"));
                    int lossLevelCount = infos.Keys.Count(s => s.EndsWith(";eliminated"));
                    int levelCount = winLevelCount > lossLevelCount ? winLevelCount : lossLevelCount;
                    
                    builder.Append($"{Environment.NewLine}{Environment.NewLine}⁘ {Multilingual.GetWord("level_detail_finals_stats")} ⟪ {winsCount}{Multilingual.GetWord(winsCount > 1 ? "level_wins_suffix" : "level_win_suffix")} / {lossesCount}{Multilingual.GetWord(lossesCount > 1 ? "level_losses_suffix" : "level_loss_suffix")} ⟫ - {Math.Truncate(winsCount * 100d / (winsCount + lossesCount) * 10) / 10}%{Environment.NewLine}");
                    string prevLevel = string.Empty;
                    int index = 0;
                    foreach (KeyValuePair<string, int> kv in infos) {
                        if (!string.IsNullOrEmpty(prevLevel) && kv.Key.Split(';')[0].Equals(prevLevel)) {
                            builder.Append($" / {kv.Value}{(kv.Key.Split(';')[1].Equals("crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(prevLevel) && !kv.Key.Split(';')[0].Equals(prevLevel)) {
                            builder.Append(" ⟩    ");
                        }

                        if (index > 0) {
                            if (levelCount > 5) {
                                if (index % 2 == 0) {
                                    builder.Append(Environment.NewLine);
                                    builder.Append("   •  ");
                                } else {
                                    builder.Append("      •  ");
                                }
                            } else {
                                builder.Append($"{Environment.NewLine}");
                                builder.Append("   •  ");
                            }
                        } else {
                            builder.Append("   •  ");
                        }
                        builder.Append($"{kv.Key.Split(';')[0]} :  ⟨ {kv.Value}{(kv.Key.Split(';')[1].Equals("crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}");
                        prevLevel = kv.Key.Split(';')[0];
                        index++;
                    }
                    builder.Append(" ⟩");
                }
                
                this.tooltip = this.formsPlot.Plot.AddTooltip(label: builder.ToString(), x: this.HighlightedPoint.X, y: this.HighlightedPoint.Y);
                
                if (this.switchGraphStyle == 1) { // LollipopPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.none;
                } else if (this.switchGraphStyle == 2) { // BarPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.none;
                } else { // ScatterPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.openCircle;
                }
            } else {
                this.tooltip = this.formsPlot.Plot.AddTooltip(label: $" {DateTime.FromOADate(this.MyScatterPlot1.Xs[currentIndex]).ToString(Multilingual.GetWord("level_date_format"))}{Environment.NewLine}{Environment.NewLine}{Multilingual.GetWord("level_no_statistical_data")}", x: this.HighlightedPoint.X, y: this.HighlightedPoint.Y);
                
                if (this.switchGraphStyle == 1) { // LollipopPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.none;
                } else if (this.switchGraphStyle == 2) { // BarPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.none;
                } else { // ScatterPlot
                    this.HighlightedPoint.MarkerShape = MarkerShape.openCircle;
                }
            }
            this.tooltip.BorderWidth = 1.7f;
            // this.tooltip.BorderColor = Color.FromArgb(239, this.Theme == MetroThemeStyle.Light ? Color.Black : Color.Snow);
            this.tooltip.BorderColor = Color.FromArgb(239, this.Theme == MetroThemeStyle.Light ? Color.Black : Color.Crimson);
            this.tooltip.FillColor = Color.FromArgb(239, 49,51,56);
            this.tooltip.Font.Color = Color.White;
            this.tooltip.Font.Family = Overlay.GetMainFontFamilies(Stats.CurrentLanguage);
            this.tooltip.Font.Size = 15f;
            this.tooltip.ArrowSize = 5;
            
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
            this.picSwitchGraphStyle.Image = this.switchGraphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.switchGraphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
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
                this.switchGraphStyle += 1;
                if (this.switchGraphStyle > 2) { this.switchGraphStyle = 0; }
            } else if (e.Button == MouseButtons.Right) {
                this.switchGraphStyle -= 1;
                if (this.switchGraphStyle < 0) { this.switchGraphStyle = 2; }
            }
            this.picSwitchGraphStyle.Image = this.switchGraphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.switchGraphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
            if (this.dates == null) { return; }
            this.ChangeFormsPlotStyle(this.switchGraphStyle);
        }
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot3.IsVisible = false;
                this.MyLollipopPlot3.IsVisible = this.chkWins.Checked;
            } else if (this.switchGraphStyle == 2) {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot3.IsVisible = this.chkWins.Checked;
                this.MyLollipopPlot3.IsVisible = false;
            } else {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
            }
            this.formsPlot.Refresh();
        }
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot2.IsVisible = false;
                this.MyLollipopPlot2.IsVisible = this.chkFinals.Checked;
            } else if (this.switchGraphStyle == 2) {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot2.IsVisible = this.chkFinals.Checked;
                this.MyLollipopPlot2.IsVisible = false;
            } else {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
            }
            this.formsPlot.Refresh();
        }
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) { return; }
            if (this.switchGraphStyle == 1) {
                this.MyScatterPlot1.IsVisible = this.chkShows.Checked;
                this.MyBarPlot1.IsVisible = false;
                this.MyLollipopPlot1.IsVisible = this.chkShows.Checked;
            } else if (this.switchGraphStyle == 2) {
                this.MyScatterPlot1.IsVisible = this.chkShows.Checked;
                this.MyBarPlot1.IsVisible = this.chkShows.Checked;
                this.MyLollipopPlot1.IsVisible = false;
            } else {
                this.MyScatterPlot1.IsVisible = this.chkShows.Checked;
            }
            this.formsPlot.Refresh();
        }
        private void ChangeLanguage() {
            this.chkWins.Text = Multilingual.GetWord("level_detail_wins");
            this.chkFinals.Text = Multilingual.GetWord("level_detail_finals");
            this.chkShows.Text = Multilingual.GetWord("level_detail_shows");
            if (Stats.CurrentLanguage == 0) { // English
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1053, 35);
                this.chkWins.Location = new Point(961, 35);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1042, 35);
                this.chkWins.Location = new Point(925, 35);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.chkShows.Location = new Point(1187, 35);
                this.chkFinals.Location = new Point(1088, 35);
                this.chkWins.Location = new Point(1001, 35);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1048, 35);
                this.chkWins.Location = new Point(961, 35);
            } else if (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) { // Simplified Chinese & Traditional Chinese
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1053, 35);
                this.chkWins.Location = new Point(961, 35);
            }
            this.picSwitchGraphStyle.Location = new Point(this.chkWins.Location.X - this.picSwitchGraphStyle.Size.Width - 50 , 34);
        }
    }
}