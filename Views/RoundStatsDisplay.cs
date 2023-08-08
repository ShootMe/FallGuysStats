using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using ScottPlot;
using ScottPlot.Plottable;

namespace FallGuysStats {
    public partial class RoundStatsDisplay : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        public Dictionary<string, double[]> roundGraphData;
        public Dictionary<string, TimeSpan> roundDurationData;
        //public Dictionary<string, double[]> roundRecordData;
        public Dictionary<string, int[]> roundScoreData;
        public IOrderedEnumerable<KeyValuePair<string, string>> roundList;
        private string[] labelList = {
            Multilingual.GetWord("main_played"), Multilingual.GetWord("level_detail_gold"),
            Multilingual.GetWord("level_detail_silver"), Multilingual.GetWord("level_detail_bronze"),
            Multilingual.GetWord("level_detail_pink"), Multilingual.GetWord("level_detail_eliminated")
        };
        private bool isStartingUp;
        public RoundStatsDisplay() {
            this.InitializeComponent();
        }
        
        private class CustomPalette : IPalette {
            public string Name { get; } = "Custom Palette";

            public string Description { get; } = "Custom Palette";

            public Color[] Colors { get; } = {
                Color.FromArgb(31, 119, 180), Color.FromArgb(255, 215, 0), Color.FromArgb(192, 192, 192), Color.FromArgb(205, 127, 50), Color.FromArgb(255, 20, 147),
                Color.FromArgb(128, 0, 128), Color.FromArgb(227, 119, 194), Color.FromArgb(127, 127, 127), Color.FromArgb(188, 189, 34), Color.FromArgb(23, 190, 207)
            };
            
            //public SharedColor[] Colors { get; } = SharedColor.FromHex(HexColors);

            // private static readonly string[] HexColors = {
            //     // "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd",
            //     //"#8c564b", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf",
            //     "#1f77b4", "#ffd700", "#c0c0c0", "#cd7f32", "#ff1493",
            //     "#800080", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf",
            // };
        }

        private void RoundStatsDisplay_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            
            this.isStartingUp = true;
            this.cboRoundList.DataSource = new BindingSource(this.roundList, null);
            this.cboRoundList.DisplayMember = "Value";
            this.cboRoundList.ValueMember = "Key";

            this.formsPlot.Plot.Legend(location: Alignment.UpperRight);
            this.SetGraph();
            this.isStartingUp = false;
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            if (theme == MetroThemeStyle.Light) {
                
            } else if (theme == MetroThemeStyle.Dark) {
                this.formsPlot.Plot.Style(ScottPlot.Style.Black);
                this.formsPlot.Plot.Style(figureBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(dataBackground: Color.FromArgb(17, 17, 17));
                this.formsPlot.Plot.Style(tick: Color.WhiteSmoke);
                foreach (Control c1 in Controls) {
                    if (c1 is MetroComboBox mcbo1) {
                        mcbo1.Theme = theme;
                    } else if (c1 is MetroLabel mlb1) {
                        mlb1.Theme = theme;
                    } else if (c1 is Label lb1) {
                        if (lb1.Name.Equals("lblRoundType")) continue;
                        lb1.ForeColor = Color.DarkGray;
                    }
                }
            }
        }

        private void SetGraph() {
            //this.formsPlot.Plot.Grid(false);
            //this.formsPlot.Plot.Frameless();
            KeyValuePair<string, string> selectedRoundPair = (KeyValuePair<string, string>)this.cboRoundList.SelectedItem;
            //string roundId = (string)this.cboRoundList.SelectedValue;
            string roundId = selectedRoundPair.Key;
            if (this.StatsForm.StatLookup.TryGetValue(roundId, out LevelStats level)) {
                this.picRoundIcon.Size = level.RoundBigIcon.Size;
                this.picRoundIcon.Image = level.RoundBigIcon;
                this.formsPlot.Plot.Title(level.Name);
                
                LevelType levelType = level.Type;
                this.lblRoundType.Text = levelType.LevelTitle(level.IsFinal);
                this.lblRoundType.borderColor = levelType.LevelDefaultColor(level.IsFinal);
                this.lblRoundType.backColor = levelType.LevelDefaultColor(level.IsFinal);
                this.lblRoundType.Width = TextRenderer.MeasureText(this.lblRoundType.Text, this.lblRoundType.Font).Width + 12;
                int recordType = ("round_pixelperfect_almond".Equals(roundId) ||
                                  "round_hoverboardsurvival_s4_show".Equals(roundId) ||
                                  "round_hoverboardsurvival2_almond".Equals(roundId) ||
                                  "round_snowy_scrap".Equals(roundId) ||
                                  "round_jinxed".Equals(roundId) ||
                                  "round_rocknroll".Equals(roundId) ||
                                  "round_conveyor_arena".Equals(roundId)) ? 1
                                : ("round_1v1_button_basher".Equals(roundId) || "round_1v1_volleyfall_symphony_launch_show".Equals(roundId)) ? 2
                                : levelType.FastestLabel();
                this.lblBestRecord.Left = this.lblRoundType.Right + 12;
                this.lblWorstRecord.Left = this.lblRoundType.Right + 12;
                this.lblBestRecord.Text = recordType == 0 ? $"{Multilingual.GetWord("overlay_longest")} : {level.Longest:m\\:ss\\.ff}" :
                                            recordType == 1 ? $"{Multilingual.GetWord("overlay_fastest")} : {level.Fastest:m\\:ss\\.ff}" :
                                            recordType == 2 ? $"{Multilingual.GetWord("overlay_best_score")} : {this.roundScoreData[roundId][0]}" : "-";
                this.lblWorstRecord.Text = recordType == 0 ? $"{Multilingual.GetWord("overlay_fastest")} : {level.Fastest:m\\:ss\\.ff}" :
                                            recordType == 1 ? $"{Multilingual.GetWord("overlay_longest")} : {level.Longest:m\\:ss\\.ff}" :
                                            recordType == 2 ? $"{Multilingual.GetWord("overlay_worst_score")} : {this.roundScoreData[roundId][1]}" : "-";
            } else {
                MatchCollection matches = Regex.Matches(roundId, @"^\d{4}-\d{4}-\d{4}$");
                if (matches.Count > 0) { // user creative round
                    if (this.StatsForm.StatLookup.TryGetValue(this.StatsForm.AllStats.Find(ri => roundId.Equals(ri.ShowNameId)).Name, out LevelStats creativeLevel)) {
                        this.picRoundIcon.Size = creativeLevel.RoundBigIcon.Size;
                        this.picRoundIcon.Image = creativeLevel.RoundBigIcon;
                        this.formsPlot.Plot.Title(selectedRoundPair.Value);
                        
                        LevelType levelType = creativeLevel.Type;
                        this.lblRoundType.Text = levelType.LevelTitle(creativeLevel.IsFinal);
                        this.lblRoundType.borderColor = levelType.LevelDefaultColor(creativeLevel.IsFinal);
                        this.lblRoundType.backColor = levelType.LevelDefaultColor(creativeLevel.IsFinal);
                        this.lblRoundType.Width = TextRenderer.MeasureText(this.lblRoundType.Text, this.lblRoundType.Font).Width + 12;
                        int recordType = levelType.FastestLabel();
                        this.lblBestRecord.Left = this.lblRoundType.Right + 12;
                        this.lblWorstRecord.Left = this.lblRoundType.Right + 12;
                        this.lblBestRecord.Text = recordType == 0 ? $"{Multilingual.GetWord("overlay_longest")} : {creativeLevel.Longest:m\\:ss\\.ff}" :
                            recordType == 1 ? $"{Multilingual.GetWord("overlay_fastest")} : {creativeLevel.Fastest:m\\:ss\\.ff}" :
                            recordType == 2 ? $"{Multilingual.GetWord("overlay_best_score")} : {this.roundScoreData[roundId][0]}" : "-";
                        this.lblWorstRecord.Text = recordType == 0 ? $"{Multilingual.GetWord("overlay_fastest")} : {creativeLevel.Fastest:m\\:ss\\.ff}" :
                            recordType == 1 ? $"{Multilingual.GetWord("overlay_longest")} : {creativeLevel.Longest:m\\:ss\\.ff}" :
                            recordType == 2 ? $"{Multilingual.GetWord("overlay_worst_score")} : {this.roundScoreData[roundId][1]}" : "-";
                    }
                }
            }

            TimeSpan duration = this.roundDurationData[roundId];
            this.lblRoundTime.Text = $"{Multilingual.GetWord("level_played_prefix")} {(int)duration.TotalHours}{Multilingual.GetWord("main_hour")}{duration:mm}{Multilingual.GetWord("main_min")}{duration:ss}{Multilingual.GetWord("main_sec")} {Multilingual.GetWord("level_played_suffix")}";
            double[] values = this.roundGraphData[roundId];
            
            this.formsPlot.Plot.Palette = new CustomPalette();

            this.lblCountGoldMedal.Text = values[1].ToString();
            this.lblCountSilverMedal.Text = values[2].ToString();
            this.lblCountBronzeMedal.Text = values[3].ToString();
            this.lblCountPinkMedal.Text = values[4].ToString();
            this.lblCountEliminatedMedal.Text = values[5].ToString();
            
            RadialGaugePlot gauges = this.formsPlot.Plot.AddRadialGauge(values);
            gauges.OrderInsideOut = false;
            //gauges.Clockwise = false;
            gauges.SpaceFraction = .1;
            //gauges.BackgroundTransparencyFraction = .3;
            gauges.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            gauges.LabelPositionFraction = 0;
            gauges.FontSizeFraction = .5;
            //gauges.Font.Color = Color.Black;
            gauges.Labels = this.labelList;
            this.formsPlot.Plot.AxisZoom(.9, .9);
            this.formsPlot.Refresh();
        }
        
        private void cboRoundList_SelectedIndexChanged(object sender, EventArgs e) {
            if (!this.isStartingUp) {
                this.formsPlot.Plot.Clear();
                this.SetGraph();
            }
        }
        
        private void RoundStatsDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ChangeLanguage() {
            this.lblRoundType.Font = new Font(Overlay.GetDefaultFontFamilies(Stats.CurrentLanguage), 18, Stats.CurrentLanguage > 1 ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            this.lblCountGoldMedal.Font = Overlay.GetDefaultFont(0, 45);
            this.lblCountSilverMedal.Font = Overlay.GetDefaultFont(0, 45);
            this.lblCountBronzeMedal.Font = Overlay.GetDefaultFont(0, 45);
            this.lblCountPinkMedal.Font = Overlay.GetDefaultFont(0, 45);
            this.lblCountEliminatedMedal.Font = Overlay.GetDefaultFont(0, 45);
        }
    }
}