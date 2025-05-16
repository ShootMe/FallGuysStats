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
    public partial class LevelStatsDisplay : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        public Dictionary<string, double[]> levelMedalInfo;
        public Dictionary<string, TimeSpan> levelTotalPlayTime;
        public Dictionary<string, int[]> levelScoreInfo;
        public IOrderedEnumerable<KeyValuePair<string, string>> levelList;
        private RadialGaugePlot radialGauges;
        private readonly string[] labelList = {
            Multilingual.GetWord("main_played"), Multilingual.GetWord("level_detail_gold"),
            Multilingual.GetWord("level_detail_silver"), Multilingual.GetWord("level_detail_bronze"),
            Multilingual.GetWord("level_detail_pink"), Multilingual.GetWord("level_detail_eliminated")
        };
        private bool isInitComplete;
        private string goldMedalCount, silverMedalCount, bronzeMedalCount, pinkMedalCount, eliminatedMedalCount;
        private string goldMedalPercent, silverMedalPercent, bronzeMedalPercent, pinkMedalPercent, eliminatedMedalPercent;
        private bool switching;
        public LevelStatsDisplay() {
            this.InitializeComponent();
            this.Opacity = 0;
        }
        
        private class CustomPalette : IPalette {
            public string Name => "Custom Palette";
            public string Description => "Custom Palette";

            public Color[] Colors { get; } = {
                Color.FromArgb(31, 119, 180), Color.FromArgb(255, 215, 0), Color.FromArgb(192, 192, 192), Color.FromArgb(205, 127, 50), Color.FromArgb(255, 20, 147),
                Color.FromArgb(128, 0, 128), Color.FromArgb(227, 119, 194), Color.FromArgb(127, 127, 127), Color.FromArgb(188, 189, 34), Color.FromArgb(23, 190, 207)
            };
        }

        private void RoundStatsDisplay_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
            this.ChangeLanguage();
            
            this.cboRoundList.DataSource = new BindingSource(this.levelList, null);
            this.cboRoundList.DisplayMember = "Value";
            this.cboRoundList.ValueMember = "Key";

            this.formsPlot.Plot.Legend(location: Alignment.UpperRight);
            this.SetGraph(this.cboRoundList.SelectedItem);
            this.isInitComplete = true;
        }

        private void RoundStatsDisplay_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            if (theme == MetroThemeStyle.Dark) {
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
                        if (lb1.Equals(this.lblRoundType) || lb1.Equals(this.lblRoundTime) || lb1.Equals(this.lblBestRecord) || lb1.Equals(this.lblWorstRecord)) continue;
                        lb1.ForeColor = Color.DarkGray;
                    }
                }
            }
            this.Theme = theme;
            this.ResumeLayout();
        }

        private void SetGraph(object selectedItem) {
            //this.formsPlot.Plot.Grid(false);
            //this.formsPlot.Plot.Frameless();
            KeyValuePair<string, string> selectedRoundPair = (KeyValuePair<string, string>)selectedItem;
            string levelId = selectedRoundPair.Key;
            
            MatchCollection matches = Regex.Matches(levelId, @"^\d{4}-\d{4}-\d{4}$");
            if (matches.Count > 0) {
                List<RoundInfo> info = this.StatsForm.AllStats.FindAll(r => r.UseShareCode && string.Equals(r.Name, levelId));
                this.picRoundIcon.Size = Properties.Resources.round_creative_big_icon.Size;
                this.picRoundIcon.Image = Properties.Resources.round_creative_big_icon;
                this.formsPlot.Plot.Title(selectedRoundPair.Value);
                
                switch (info.Last().ShowNameId) {
                    case "user_creative_race_round":
                        this.lblRoundType.Text = Multilingual.GetWord("level_detail_race");
                        this.lblRoundType.borderColor = Color.FromArgb(0, 236, 106);
                        this.lblRoundType.backColor = Color.FromArgb(0, 236, 106);
                        this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {info.Min(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {info.Max(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        break;
                    case "user_creative_hunt_round":
                        this.lblRoundType.Text = Multilingual.GetWord("level_detail_hunt");
                        this.lblRoundType.borderColor = Color.FromArgb(45, 101, 186);
                        this.lblRoundType.backColor = Color.FromArgb(45, 101, 186);
                        this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {info.Min(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {info.Max(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        break;
                    case "user_creative_survival_round":
                        this.lblRoundType.Text = Multilingual.GetWord("level_detail_survival");
                        this.lblRoundType.borderColor = Color.FromArgb(184, 21, 213);
                        this.lblRoundType.backColor = Color.FromArgb(184, 21, 213);
                        this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {info.Max(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {info.Min(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        break;
                    case "user_creative_logic_round":
                        this.lblRoundType.Text = Multilingual.GetWord("level_detail_logic");
                        this.lblRoundType.borderColor = Color.FromArgb(91, 181, 189);
                        this.lblRoundType.backColor = Color.FromArgb(91, 181, 189);
                        this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {info.Max(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {info.Min(r => r.Finish.GetValueOrDefault(r.Start) - r.Start):m\\:ss\\.fff}";
                        break;
                    case "user_creative_team_round":
                        this.lblRoundType.Text = Multilingual.GetWord("level_detail_team");
                        this.lblRoundType.borderColor = Color.FromArgb(248, 82, 0);
                        this.lblRoundType.backColor = Color.FromArgb(248, 82, 0);
                        this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_high_score")} : {this.levelScoreInfo[levelId][0]}";
                        this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_low_score")} : {this.levelScoreInfo[levelId][1]}";
                        break;
                    default:
                        this.lblRoundType.Text = "UNKNOWN";
                        this.lblRoundType.borderColor = Color.DarkGray;
                        this.lblRoundType.backColor = Color.DarkGray;
                        this.lblBestRecord.Text = @"-";
                        this.lblWorstRecord.Text = @"-";
                        break;
                }
                
                this.lblRoundType.Width = TextRenderer.MeasureText(this.lblRoundType.Text, this.lblRoundType.Font).Width + 12;
                this.lblBestRecord.Left = this.lblRoundType.Right + 12;
                this.lblWorstRecord.Left = this.lblRoundType.Right + 12;
            } else {
                if (this.StatsForm.StatLookup.TryGetValue(levelId, out LevelStats level)) {
                    this.picRoundIcon.Size = level.RoundBigIcon.Size;
                    this.picRoundIcon.Image = level.RoundBigIcon;
                    this.formsPlot.Plot.Title(level.Name);
                    
                    this.lblRoundType.Text = level.Type.LevelTitle(level.IsFinal);
                    this.lblRoundType.borderColor = level.Type.LevelDefaultColor(level.IsFinal);
                    this.lblRoundType.backColor = level.Type.LevelDefaultColor(level.IsFinal);
                    this.lblRoundType.Width = TextRenderer.MeasureText(this.lblRoundType.Text, this.lblRoundType.Font).Width + 12;
                    this.lblBestRecord.Left = this.lblRoundType.Right + 12;
                    this.lblWorstRecord.Left = this.lblRoundType.Right + 12;
                    
                    switch (level.BestRecordType) {
                        case BestRecordType.Fastest:
                            this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {level.Fastest:m\\:ss\\.fff}";
                            this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {level.Longest:m\\:ss\\.fff}";
                            break;
                        case BestRecordType.Longest:
                            this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_longest")} : {level.Longest:m\\:ss\\.fff}";
                            this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_fastest")} : {level.Fastest:m\\:ss\\.fff}";
                            break;
                        case BestRecordType.HighScore:
                            this.lblBestRecord.Text = $"{Multilingual.GetWord("overlay_high_score")} : {this.levelScoreInfo[levelId][0]}";
                            this.lblWorstRecord.Text = $"{Multilingual.GetWord("overlay_low_score")} : {this.levelScoreInfo[levelId][1]}";
                            break;
                        default:
                            this.lblBestRecord.Text = @"-";
                            this.lblWorstRecord.Text = @"-";
                            break;
                    }
                }
            }

            TimeSpan playTime = this.levelTotalPlayTime[levelId];
            this.lblRoundTime.Text = $@"{Multilingual.GetWord("level_round_played_prefix")} {(int)playTime.TotalHours}{Multilingual.GetWord("main_hour")}{playTime:mm}{Multilingual.GetWord("main_min")}{playTime:ss}{Multilingual.GetWord("main_sec")} {Multilingual.GetWord("level_round_played_suffix")}";
            double[] values = this.levelMedalInfo[levelId];
            
            this.formsPlot.Plot.Palette = new CustomPalette();

            this.goldMedalPercent = $@"{Math.Round((values[1] / values[0]) * 100, 2)}%";
            this.silverMedalPercent = $@"{Math.Round((values[2] / values[0]) * 100, 2)}%";
            this.bronzeMedalPercent = $@"{Math.Round((values[3] / values[0]) * 100, 2)}%";
            this.pinkMedalPercent = $@"{Math.Round((values[4] / values[0]) * 100, 2)}%";
            this.eliminatedMedalPercent = $@"{Math.Round((values[5] / values[0]) * 100, 2)}%";
            
            this.goldMedalCount = $@"{values[1]:N0}";
            this.silverMedalCount = $@"{values[2]:N0}";
            this.bronzeMedalCount = $@"{values[3]:N0}";
            this.pinkMedalCount = $@"{values[4]:N0}";
            this.eliminatedMedalCount = $@"{values[5]:N0}";
            
            if (this.switching) {
                this.lblCountGoldMedal.Text = this.goldMedalPercent;
                this.lblCountSilverMedal.Text = this.silverMedalPercent;
                this.lblCountBronzeMedal.Text = this.bronzeMedalPercent;
                this.lblCountPinkMedal.Text = this.pinkMedalPercent;
                this.lblCountEliminatedMedal.Text = this.eliminatedMedalPercent;
            } else {
                this.lblCountGoldMedal.Text = this.goldMedalCount;
                this.lblCountSilverMedal.Text = this.silverMedalCount;
                this.lblCountBronzeMedal.Text = this.bronzeMedalCount;
                this.lblCountPinkMedal.Text = this.pinkMedalCount;
                this.lblCountEliminatedMedal.Text = this.eliminatedMedalCount;
            }
            
            
            
            this.radialGauges = this.formsPlot.Plot.AddRadialGauge(values);
            this.radialGauges.OrderInsideOut = false;
            //this.radialGauges.Clockwise = false;
            this.radialGauges.SpaceFraction = .1;
            //this.radialGauges.BackgroundTransparencyFraction = .3;
            //this.radialGauges.StartingAngle = 270;
            this.radialGauges.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            this.radialGauges.LabelPositionFraction = 0;
            this.radialGauges.FontSizeFraction = .7;
            //this.radialGauges.Font.Color = Color.Black;
            this.radialGauges.Labels = this.labelList;
            
            this.formsPlot.Plot.AxisZoom(.9, .9);
            this.formsPlot.Plot.YAxis.SetBoundary(0, 0);
            this.formsPlot.Plot.XAxis.SetBoundary(0, 0);
            this.formsPlot.Refresh();
            
            // this.formsPlot.Configuration.Pan = false;
            // this.formsPlot.Configuration.Zoom = false;
            // this.formsPlot.Configuration.ScrollWheelZoom = false;
            // this.formsPlot.Configuration.RightClickDragZoom = false;
            // this.formsPlot.Configuration.MiddleClickAutoAxis = false;
            // this.formsPlot.Configuration.DoubleClickBenchmark = false;
            // this.formsPlot.Configuration.LockHorizontalAxis = true;
            // this.formsPlot.Configuration.LockVerticalAxis = true;
            
            var pb = this.formsPlot.Controls.OfType<PictureBox>().FirstOrDefault();
            if (pb != null) {
                pb.Enabled = false;
            }
        }

        private void Medal_MouseEnter(object sender, EventArgs e) {
            this.Cursor = this.Theme == MetroThemeStyle.Light
                ? new System.Windows.Forms.Cursor(Properties.Resources.transform_icon.GetHicon())
                : new System.Windows.Forms.Cursor(Properties.Resources.transform_gray_icon.GetHicon());
        }

        private void Medal_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
        }
        
        private void Medal_MouseClick(object sender, EventArgs e) {
            this.switching = !this.switching;
            if (this.switching) {
                this.lblCountGoldMedal.Text = this.goldMedalPercent;
                this.lblCountSilverMedal.Text = this.silverMedalPercent;
                this.lblCountBronzeMedal.Text = this.bronzeMedalPercent;
                this.lblCountPinkMedal.Text = this.pinkMedalPercent;
                this.lblCountEliminatedMedal.Text = this.eliminatedMedalPercent;
            } else {
                this.lblCountGoldMedal.Text = this.goldMedalCount;
                this.lblCountSilverMedal.Text = this.silverMedalCount;
                this.lblCountBronzeMedal.Text = this.bronzeMedalCount;
                this.lblCountPinkMedal.Text = this.pinkMedalCount;
                this.lblCountEliminatedMedal.Text = this.eliminatedMedalCount;
            }
        }

        private void cboRoundList_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.isInitComplete) {
                this.formsPlot.Plot.Clear();
                this.SetGraph(((MetroComboBox)sender).SelectedItem);
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Escape) {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeLanguage() {
            this.lblRoundTime.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
            this.lblBestRecord.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
            this.lblWorstRecord.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
            this.lblRoundType.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
            this.lblCountGoldMedal.Font = Overlay.GetDefaultFont(45, 0);
            this.lblCountSilverMedal.Font = Overlay.GetDefaultFont(45, 0);
            this.lblCountBronzeMedal.Font = Overlay.GetDefaultFont(45, 0);
            this.lblCountPinkMedal.Font = Overlay.GetDefaultFont(45, 0);
            this.lblCountEliminatedMedal.Font = Overlay.GetDefaultFont(45, 0);
        }
    }
}