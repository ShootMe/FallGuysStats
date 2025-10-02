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
        public double manualSpacing = 1.0;
        public int graphStyle;
        public Stats StatsForm { get; set; }
        public WinStatsDisplay() {
            this.InitializeComponent();
            this.Opacity = 0;
        }

        private double yMax;
        private ScatterPlot MyScatterPlot1, MyScatterPlot2, MyScatterPlot3;
        private BarPlot MyBarPlot1, MyBarPlot2, MyBarPlot3;
        private LollipopPlot MyLollipopPlot1, MyLollipopPlot2, MyLollipopPlot3;
        private MarkerPlot HighlightedDate;
        private Tooltip tooltip;

        private void WinStatsDisplay_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
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
                
                this.formsPlot.Plot.YAxis.SetBoundary(-0.9);
                this.formsPlot.Plot.YAxis.MinimumTickSpacing(1.0);
                
                this.formsPlot.Plot.XAxis.DateTimeFormat(true);
                this.formsPlot.Plot.XAxis.ManualTickSpacing(Math.Max(1.0, this.manualSpacing), ScottPlot.Ticks.DateTimeUnit.Day);
                this.formsPlot.Plot.XAxis.TickLabelStyle(rotation: 45);
                //this.formsPlot.Plot.XAxis.SetSizeLimit(min: 50);
                this.formsPlot.Plot.SetAxisLimits(
                    yMin: (this.dates.Length / 14D) * -1,
                    yMax: this.yMax + (this.dates.Length / 14D),
                    xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                    xMax: DateTime.FromOADate(this.dates[this.dates.Length-1]).AddDays(4).ToOADate()
                );
                
                this.HighlightedDate = this.formsPlot.Plot.AddPoint(0, 0);
                this.HighlightedDate.Color = this.Theme == MetroThemeStyle.Light ? Color.Green : Color.Red;
                this.HighlightedDate.MarkerSize = 9000;
                this.HighlightedDate.MarkerShape = MarkerShape.verticalBar;
                this.HighlightedDate.IsVisible = false;

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
                this.picSwitchGraphStyle.Image = this.graphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.graphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
                this.ChangeFormsPlotStyle(this.graphStyle);
            } else {
                this.formsPlot.Refresh();
            }
        }

        private void WinStatsDisplay_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }

        private void ChangeFormsPlotStyle(int style) {
            this.BeginInvoke((MethodInvoker)delegate {
                if (style == 1) {
                    // LollipopPlot
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

                    //this.HighlightedDate.MarkerShape = MarkerShape.none;
                } else if (style == 2) {
                    // BarPlot
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

                    //this.HighlightedDate.MarkerShape = MarkerShape.none;
                } else {
                    // ScatterPlot
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

                    //this.HighlightedDate.MarkerShape = MarkerShape.openCircle;
                }

                this.formsPlot.Plot.SetAxisLimits(
                    yMin: (this.dates.Length / 14D) * -1,
                    yMax: this.yMax + (this.dates.Length / 14D),
                    xMin: DateTime.FromOADate(this.dates[0]).AddDays(-4).ToOADate(),
                    xMax: DateTime.FromOADate(this.dates[this.dates.Length - 1]).AddDays(4).ToOADate()
                );
                //this.formsPlot.Plot.AxisZoom(0.9, 0.9);
                this.formsPlot.Refresh();
            });
        }

        private Color GetColorWithAlpha(Color color, int alpha) {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private void formsPlot_MouseMove(object sender, MouseEventArgs e) {
            if (this.dates == null) return;
            if (!(this.MyScatterPlot1.IsVisible || this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible)) return;

            this.BeginInvoke((MethodInvoker)delegate {
                this.formsPlot.Plot.Remove(this.tooltip);

                (int nearestDateIndex, double nearestDateX) = this.GetNearestDateColumn(this.formsPlot.GetMouseCoordinates().x);

                if (nearestDateIndex == -1) return;

                this.HighlightedDate.X = nearestDateX;
                this.HighlightedDate.Y = Math.Round(this.formsPlot.Plot.GetAxisLimits().YMax / 1.2);
                this.HighlightedDate.IsVisible = true;

                if (this.HasStatisticalData(nearestDateIndex)) {
                    string tooltipText = this.BuildStatTooltip(nearestDateIndex);
                    this.tooltip = this.formsPlot.Plot.AddTooltip(tooltipText, this.HighlightedDate.X, this.HighlightedDate.Y);
                } else {
                    string tooltipText = $" {DateTime.FromOADate(this.HighlightedDate.X).ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}{Environment.NewLine}{Environment.NewLine}{Multilingual.GetWord("level_no_statistical_data")}";
                    this.tooltip = this.formsPlot.Plot.AddTooltip(tooltipText, this.HighlightedDate.X, this.HighlightedDate.Y);
                }

                this.SetTooltipStyle();

                //this.HighlightedDate.MarkerShape = (this.graphStyle == 0) ? MarkerShape.openCircle : MarkerShape.none;
                this.formsPlot.Render();
            });
        }

        private (int index, double x) GetNearestDateColumn(double mouseX) {
            double minDist = double.MaxValue;
            (int index, double x) nearest = (-1, 0);
            for (int i = 0; i < this.MyScatterPlot1.Xs.Length; i++) {
                double dateX = this.MyScatterPlot1.Xs[i];
                double dist = Math.Sqrt(Math.Pow(mouseX - dateX, 2));
                if (dist < minDist) {
                    minDist = dist;
                    nearest = (i, dateX);
                }
            }
            return nearest;
        }
        
        private bool HasStatisticalData(int idx) {
            return this.MyScatterPlot1.Ys[idx] > 0 || this.MyScatterPlot2.Ys[idx] > 0 || this.MyScatterPlot3.Ys[idx] > 0;
        }
        
        private string BuildStatTooltip(int idx) {
            var sb = new StringBuilder();
            sb.Append($" {DateTime.FromOADate(this.MyScatterPlot1.Xs[idx]).ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}{Environment.NewLine}{Environment.NewLine}");
            if (this.MyScatterPlot1.IsVisible)
                sb.Append($" - {Multilingual.GetWord("level_detail_shows")} :  ⟪ {this.MyScatterPlot1.Ys[idx]:N0}{Multilingual.GetWord("main_inning")} ⟫{(this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible ? Environment.NewLine : "")}");
            if (this.MyScatterPlot2.IsVisible)
                sb.Append($" - {Multilingual.GetWord("level_detail_finals")} :  {this.MyScatterPlot2.Ys[idx]:N0}{Multilingual.GetWord("main_inning")}{(this.MyScatterPlot1.Ys[idx] > 0 ? $" - {Math.Truncate(this.MyScatterPlot2.Ys[idx] * 100d / this.MyScatterPlot1.Ys[idx] * 10) / 10}% " : "")}{(this.MyScatterPlot3.IsVisible ? Environment.NewLine : "")}");
            if (this.MyScatterPlot3.IsVisible)
                sb.Append($" - {Multilingual.GetWord("level_detail_wins")} :  {this.MyScatterPlot3.Ys[idx]:N0}{Multilingual.GetWord(this.MyScatterPlot3.Ys[idx] > 1 ? "level_wins_suffix" : "level_win_suffix")}{(this.MyScatterPlot1.Ys[idx] > 0 ? $" - {Math.Truncate(this.MyScatterPlot3.Ys[idx] * 100d / this.MyScatterPlot1.Ys[idx] * 10) / 10}% " : "")}");

            if (this.winsInfo.ContainsKey(this.MyScatterPlot1.Xs[idx])) {
                sb.Append(BuildWinsInfoTooltip(this.MyScatterPlot1.Xs[idx]));
            }
            return sb.ToString();
        }
        
        private string BuildWinsInfoTooltip(double dateKey) {
            var infos = this.winsInfo[dateKey];
            int winsCount = infos.Where(kv => kv.Key.EndsWith(";crown")).Sum(kv => kv.Value);
            int lossesCount = infos.Where(kv => kv.Key.EndsWith(";eliminated")).Sum(kv => kv.Value);
            int winLevelCount = infos.Keys.Count(s => s.EndsWith(";crown"));
            int lossLevelCount = infos.Keys.Count(s => s.EndsWith(";eliminated"));
            int levelCount = Math.Max(winLevelCount, lossLevelCount);

            var sb = new StringBuilder();
            if (this.MyScatterPlot3.IsVisible)
                sb.Append($"{Environment.NewLine} - {Multilingual.GetWord("level_detail_losses")} :  {lossesCount:N0}{Multilingual.GetWord(lossesCount > 1 ? "level_losses_suffix" : "level_loss_suffix")}{(this.MyScatterPlot1.Ys[(int)Array.IndexOf(this.MyScatterPlot1.Xs, dateKey)] > 0 ? $" - {Math.Truncate(lossesCount * 100d / this.MyScatterPlot1.Ys[(int)Array.IndexOf(this.MyScatterPlot1.Xs, dateKey)] * 10) / 10}% " : "")}");

            sb.Append($"{Environment.NewLine}{Environment.NewLine}⁘ {Multilingual.GetWord("level_detail_finals_stats")} ⟪ {winsCount}{Multilingual.GetWord(winsCount > 1 ? "level_wins_suffix" : "level_win_suffix")} / {lossesCount}{Multilingual.GetWord(lossesCount > 1 ? "level_losses_suffix" : "level_loss_suffix")} ⟫ - {Math.Truncate(winsCount * 100d / (winsCount + lossesCount) * 10) / 10}%{Environment.NewLine}");

            string prevLevel = string.Empty;
            int prevLength = 0;
            string temp = string.Empty;
            int index = 0;
            int longestLength = 0;
            if (levelCount > 5) {
                int i = 0;
                StringBuilder c = new StringBuilder();
                foreach (var kv in infos) {
                    if (!string.IsNullOrEmpty(prevLevel) && string.Equals(kv.Key.Split(';')[0], prevLevel)) {
                        c.Append($" / {kv.Value}{(kv.Key.EndsWith(";crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}");
                        continue;
                    }
                    if (!string.IsNullOrEmpty(prevLevel) && !string.Equals(kv.Key.Split(';')[0], prevLevel)) {
                        if (i % 2 != 0 && longestLength < c.ToString().Length)
                            longestLength = c.ToString().Length;
                        c.Clear();
                    }
                    c.Append($"{kv.Key.Split(';')[0]} :  {kv.Value}{(kv.Key.EndsWith(";crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}");
                    prevLevel = kv.Key.Split(';')[0];
                    i++;
                }
            }
            foreach (var kv in infos) {
                if (!string.IsNullOrEmpty(prevLevel) && string.Equals(kv.Key.Split(';')[0], prevLevel)) {
                    temp = $" / {kv.Value}{(kv.Key.EndsWith(";crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}";
                    sb.Append(temp);
                    prevLength += temp.Length;
                    continue;
                }
                if (index > 0) {
                    if (levelCount > 5) {
                        if (index % 2 == 0) {
                            sb.Append($"{Environment.NewLine}    •  ");
                        } else {
                            temp = $"{new string('\t', (int)Math.Ceiling(-1f * (prevLength / (float)longestLength) + 1.75f))}\t    •  ";
                            sb.Append(temp);
                        }
                    } else {
                        sb.Append($"{Environment.NewLine}    •  ");
                    }
                } else {
                    sb.Append("    •  ");
                }
                temp = $"{kv.Key.Split(';')[0]} :  {kv.Value}{(kv.Key.EndsWith(";crown") ? Multilingual.GetWord(kv.Value > 1 ? "level_wins_suffix" : "level_win_suffix") : Multilingual.GetWord(kv.Value > 1 ? "level_losses_suffix" : "level_loss_suffix"))}";
                prevLength = temp.Length;
                sb.Append(temp);
                prevLevel = kv.Key.Split(';')[0];
                index++;
            }
            return sb.ToString();
        }
        
        private void SetTooltipStyle() {
            this.tooltip.BorderWidth = 1.7f;
            this.tooltip.BorderColor = Color.FromArgb(239, this.Theme == MetroThemeStyle.Light ? Color.GreenYellow : Color.Crimson);
            this.tooltip.FillColor = Color.Black;
            this.tooltip.Font.Color = Color.White;
            this.tooltip.Font.Family = Overlay.GetMainFontFamilies(Stats.CurrentLanguage);
            this.tooltip.Font.Size = 15f;
            this.tooltip.ArrowSize = 5;
        }

        private void formsPlot_MouseLeave(object sender, EventArgs e) {
            if (this.dates == null) return;
            if (!(this.MyScatterPlot1.IsVisible || this.MyScatterPlot2.IsVisible || this.MyScatterPlot3.IsVisible)) return;

            this.HighlightedDate.IsVisible = false;
            this.HighlightedDate.X = 0;
            this.HighlightedDate.Y = 0;
            this.formsPlot.Plot.Remove(this.tooltip);
            this.formsPlot.Refresh();
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            this.picSwitchGraphStyle.Image = this.graphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.graphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
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
            this.Theme = theme;
            this.ResumeLayout();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Escape) {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        private void picSwitchGraphStyle_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                this.graphStyle += 1;
                if (this.graphStyle > 2) { this.graphStyle = 0; }
            } else if (e.Button == MouseButtons.Right) {
                this.graphStyle -= 1;
                if (this.graphStyle < 0) { this.graphStyle = 2; }
            }
            this.picSwitchGraphStyle.Image = this.graphStyle == 0 ? Properties.Resources.scatter_plot_teal_icon : (this.graphStyle == 1 ? Properties.Resources.lollipop_plot_teal_icon : Properties.Resources.bar_plot_teal_icon);
            if (this.dates == null) return;

            this.ChangeFormsPlotStyle(this.graphStyle);
        }
        
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) return;
            if (this.graphStyle == 1) {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot3.IsVisible = false;
                this.MyLollipopPlot3.IsVisible = this.chkWins.Checked;
            } else if (this.graphStyle == 2) {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
                this.MyBarPlot3.IsVisible = this.chkWins.Checked;
                this.MyLollipopPlot3.IsVisible = false;
            } else {
                this.MyScatterPlot3.IsVisible = this.chkWins.Checked;
            }
            this.formsPlot.Refresh();
        }
        
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) return;
            if (this.graphStyle == 1) {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot2.IsVisible = false;
                this.MyLollipopPlot2.IsVisible = this.chkFinals.Checked;
            } else if (this.graphStyle == 2) {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
                this.MyBarPlot2.IsVisible = this.chkFinals.Checked;
                this.MyLollipopPlot2.IsVisible = false;
            } else {
                this.MyScatterPlot2.IsVisible = this.chkFinals.Checked;
            }
            this.formsPlot.Refresh();
        }
        
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            if (this.dates == null) return;
            if (this.graphStyle == 1) {
                this.MyScatterPlot1.IsVisible = this.chkShows.Checked;
                this.MyBarPlot1.IsVisible = false;
                this.MyLollipopPlot1.IsVisible = this.chkShows.Checked;
            } else if (this.graphStyle == 2) {
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
            if (Stats.CurrentLanguage == Language.English) {
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1053, 35);
                this.chkWins.Location = new Point(961, 35);
            } else if (Stats.CurrentLanguage == Language.French) {
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1042, 35);
                this.chkWins.Location = new Point(925, 35);
            } else if (Stats.CurrentLanguage == Language.Korean) {
                this.chkShows.Location = new Point(1187, 35);
                this.chkFinals.Location = new Point(1088, 35);
                this.chkWins.Location = new Point(1001, 35);
            } else if (Stats.CurrentLanguage == Language.Japanese) {
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1048, 35);
                this.chkWins.Location = new Point(961, 35);
            } else if (Stats.CurrentLanguage == Language.SimplifiedChinese || Stats.CurrentLanguage == Language.TraditionalChinese) {
                this.chkShows.Location = new Point(1147, 35);
                this.chkFinals.Location = new Point(1053, 35);
                this.chkWins.Location = new Point(961, 35);
            }
            this.picSwitchGraphStyle.Location = new Point(this.chkWins.Location.X - this.picSwitchGraphStyle.Size.Width - 50 , 33);
        }
    }
}