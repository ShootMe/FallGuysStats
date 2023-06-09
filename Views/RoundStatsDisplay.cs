using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public BindingList<object> roundList;
        private string[] titleList = {
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

            public SharedColor[] Colors { get; } = SharedColor.FromHex(HexColors);

            private static readonly string[] HexColors = {
                // "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd",
                //"#8c564b", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf",
                "#1f77b4", "#ffd700", "#c0c0c0", "#cd7f32", "#ff1493",
                "#800080", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf",
            };
        }

        private void RoundStatsDisplay_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            
            this.isStartingUp = true;
            this.cboRoundList.DataSource = roundList;
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
                        lb1.ForeColor = Color.DarkGray;
                    }
                }
            }
        }

        private void SetGraph() {
            //this.formsPlot.Plot.Grid(false);
            //this.formsPlot.Plot.Frameless();
            string roundId = (string)this.cboRoundList.SelectedValue;

            if (this.StatsForm.StatLookup.TryGetValue(roundId, out LevelStats level)) {
                this.picRoundIcon.Size = new Size(level.RoundBigIcon.Width, level.RoundBigIcon.Height);
                this.picRoundIcon.Image = level.RoundBigIcon;
                this.formsPlot.Plot.Title(level.Name);
            }
            
            TimeSpan duration = this.roundDurationData[roundId];
            this.lblRoundTime.Text = $"{(int)duration.TotalHours}{Multilingual.GetWord("main_hour")}{duration:mm}{Multilingual.GetWord("main_min")}{duration:ss}{Multilingual.GetWord("main_sec")}";
            double[] values = this.roundGraphData[roundId];
            
            this.formsPlot.Plot.Palette = new CustomPalette();

            this.lblCountGoldMedal.Text = values[1].ToString();
            this.lblCountSilverMedal.Text = values[2].ToString();
            this.lblCountBronzeMedal.Text = values[3].ToString();
            this.lblCountPinkMedal.Text = values[4].ToString();
            this.lblCountEliminatedMedal.Text = values[5].ToString();
            
            RadialGaugePlot gauges = this.formsPlot.Plot.AddRadialGauge(values);
            gauges.OrderInsideOut = false;
            gauges.Clockwise = false;
            gauges.SpaceFraction = .1;
            //gauges.BackgroundTransparencyFraction = .3;
            gauges.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            gauges.LabelPositionFraction = 0;
            gauges.FontSizeFraction = .5;
            //gauges.Font.Color = Color.Black;
            gauges.Labels = this.titleList;
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
            this.lblCountGoldMedal.Font = Overlay.GetDefaultFont(0, 49);
            this.lblCountSilverMedal.Font = Overlay.GetDefaultFont(0, 49);
            this.lblCountBronzeMedal.Font = Overlay.GetDefaultFont(0, 49);
            this.lblCountPinkMedal.Font = Overlay.GetDefaultFont(0, 49);
            this.lblCountEliminatedMedal.Font = Overlay.GetDefaultFont(0, 49);
        }
    }
}