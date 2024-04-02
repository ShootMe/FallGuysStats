using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class LeaderboardDisplay : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        readonly DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        readonly DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        private readonly string AVAILABLE_LEVEL_API_URL = "https://data.fallalytics.com/api/leaderboards";
        private readonly string LEADERBOARD_API_URL = "https://data.fallalytics.com/api/leaderboard";
        private readonly string PLAYER_LIST_API_URL = "https://data.fallalytics.com/api/user-search?q=";
        private readonly string PLAYER_DETAILS_API_URL = "https://data.fallalytics.com/api/user-stats?user=";
        private string levelKey = String.Empty;
        private int totalPages; //, currentPage, totalHeight;
        private int totalLevelPlayers, myLevelRank = -1, myOverallRank = -1, myWeeklyCrownRank = -1;
        private DateTime refreshTime;
        private List<AvailableLevel.LevelInfo> availableLevelList;
        private List<LevelRank.RankInfo> levelRankList;
        private readonly List<LevelRank.RankInfo> levelRankNodata = new List<LevelRank.RankInfo>();
        private List<OverallRank.Player> overallRankList;
        private readonly List<OverallRank.Player> overallRankNodata = new List<OverallRank.Player>();
        private List<OverallSummary> overallSummary;
        private readonly List<OverallSummary> overallSummaryNodata = new List<OverallSummary>();
        private List<SearchResult.Player> searchResult;
        private readonly List<SearchResult.Player> searchResultNodata = new List<SearchResult.Player>();
        private List<PlayerStats.PbInfo> playerDetails;
        private readonly List<PlayerStats.PbInfo> playerDetailsNodata = new List<PlayerStats.PbInfo>();
        private PlayerStats.SpeedrunRank? speedrunRank;
        private PlayerStats.CrownLeagueRank? crownLeagueRank;
        private List<WeeklyCrown.Player> weeklyCrownList;
        private readonly List<WeeklyCrown.Player> weeklyCrownNodata = new List<WeeklyCrown.Player>();
        private bool isSearchCompleted;
        private Timer spinnerTransition = new Timer { Interval = 1 };
        private bool isIncreasing;
        private MetroProgressSpinner targetSpinner;
        private string currentUserId;
        private bool isHeaderClicked;
        
        public LeaderboardDisplay() {
            this.InitializeComponent();
            this.Opacity = 0;
            this.cboLevelList.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
        }
        
        private void spinnerTransition_Tick(object sender, EventArgs e) {
            if (this.targetSpinner == null) return;
            if (this.isIncreasing) {
                this.targetSpinner.Speed = 3.2F;
                if (this.targetSpinner.Value < 90) {
                    this.targetSpinner.Value++;
                } else {
                    this.isIncreasing = false;
                }
            } else {
                this.targetSpinner.Speed = 2.7F;
                if (this.targetSpinner.Value > 10) {
                    this.targetSpinner.Value--;
                } else {
                    this.isIncreasing = true;
                }
            }
        }
        
        private void LeaderboardDisplay_Load(object sender, EventArgs e) {
            this.spinnerTransition.Tick += this.spinnerTransition_Tick;
            this.SetTheme(Stats.CurrentTheme);
            this.SetLevelList();
            
            this.mlVisitFallalytics.Text = Multilingual.GetWord("leaderboard_see_full_rankings_in_fallalytics");
            this.mtpOverallRankPage.Text = Multilingual.GetWord("leaderboard_overall_rank");
            this.mtpLevelRankPage.Text = $@"🕹️ {Multilingual.GetWord("leaderboard_choose_a_round")}";
            this.mtpSearchPlayersPage.Text = Multilingual.GetWord("leaderboard_search_players");
            this.mtpWeeklyCrownPage.Text = Multilingual.GetWord("leaderboard_weekly_crown_league");
            this.mtbSearchPlayersText.WaterMark = Multilingual.GetWord("leaderboard_search_players_WaterMark");
            this.lblSearchDescription.Text = Multilingual.GetWord("leaderboard_choose_a_round");
            
            this.gridLevelRank.DataSource = this.levelRankNodata;
            this.gridOverallRank.DataSource = this.overallRankNodata;
            this.gridOverallSummary.DataSource = this.overallSummaryNodata;
            this.gridPlayerList.DataSource = this.searchResultNodata;
            this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
            this.gridWeeklyCrown.DataSource = this.weeklyCrownNodata;
            this.overallRankList = this.StatsForm.leaderboardOverallRankList;
            if (this.overallRankList == null && (DateTime.UtcNow - this.StatsForm.overallRankLoadTime).TotalHours >= 12) {
                this.mpsSpinner01.Visible = true;
                this.mpsSpinner01.BringToFront();
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner01;
                this.cboLevelList.Enabled = false;
                this.mtcTabControl.Enabled = false;
                Task.Run(() => this.StatsForm.InitializeOverallRankList()).ContinueWith(prevTask => {
                    this.overallRankList = this.StatsForm.leaderboardOverallRankList;
                    this.Invoke((MethodInvoker)delegate {
                        int index = this.overallRankList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                        this.myOverallRank = index + 1;
                        if (this.mtcTabControl.SelectedIndex == 0 && index != -1) {
                            this.mlMyRank.Visible = true;
                            this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myOverallRank)} {Stats.OnlineServiceNickname}";
                            this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3);

                            if (this.myOverallRank == 1) {
                                this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                            } else {
                                double percentage = ((double)(this.myOverallRank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100;
                                if (percentage <= 20) {
                                    if (this.myOverallRank == 2) {
                                        this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                                    } else if (this.myOverallRank == 3) {
                                        this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                                    } else {
                                        this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                                    }
                                } else if (percentage <= 50) {
                                    if (this.myOverallRank == 2) {
                                        this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                                    } else if (this.myOverallRank == 3) {
                                        this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                                    } else {
                                        this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                                    }
                                } else if (percentage <= 100) {
                                    this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                                } else {
                                    this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                                }
                            }
                        }
                        this.mpsSpinner01.Visible = false;
                        this.spinnerTransition.Stop();
                        // this.targetSpinner = null;
                        this.gridOverallRank.DataSource = prevTask.Result ? this.overallRankList : this.overallRankNodata;
                        this.gridOverallRank.ClearSelection();
                        if (!prevTask.Result) this.overallRankList = null;
                        this.overallSummary = this.overallRankList?.Where(o => !o.isAnonymous && !string.IsNullOrEmpty(o.country))
                            .GroupBy(o => o.country)
                            .Select(g => new OverallSummary {
                                country = g.Key, players = g.Count()
                                , gold = g.Count(o => o.rank == 1)
                                , silver = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) < 20)
                                , bronze = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) < 50)
                                , pink = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) >= 50)
                            })
                            .OrderByDescending(s => s.gold)
                            .ThenByDescending(s => s.silver)
                            .ThenByDescending(s => s.bronze)
                            .ThenByDescending(s => s.pink)
                            .ThenByDescending(s => s.players)
                            .ThenBy(s => s.country).ToList();
                        int weight = 0;
                        for (int i = 0; i < this.overallSummary.Count; i++) {
                            OverallSummary current = this.overallSummary[i];
                            if (current.gold == 0 && current.silver == 0 &&current.bronze == 0) {
                                break;
                            }
                            
                            if (i > 0) {
                                OverallSummary previous = this.overallSummary[i - 1];
                                if (previous.gold == current.gold && previous.silver == current.silver && previous.bronze == current.bronze) {
                                    current.rank = previous.rank;
                                    weight++;
                                } else {
                                    current.rank = previous.rank + 1 + weight;
                                    weight = 0;
                                }
                            } else {
                                current.rank = 1;
                            }
                            this.overallSummary[i] = current;
                        }
                        this.gridOverallSummary.DataSource = prevTask.Result ? this.overallSummary : this.overallSummaryNodata;
                        
                        if (index != -1) {
                            int displayedRowCount = this.gridOverallRank.DisplayedRowCount(false);
                            int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                            this.gridOverallRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                        }
                        this.mtcTabControl.Enabled = true;
                        this.cboLevelList.Enabled = true;
                    });
                });
            } else {
                this.mpsSpinner01.Visible = false;
                this.spinnerTransition.Stop();
                // this.targetSpinner = null;
                this.gridOverallRank.DataSource = this.overallRankList;
                this.gridOverallRank.ClearSelection();
                this.overallSummary = this.overallRankList?
                    .Where(o => !o.isAnonymous && !string.IsNullOrEmpty(o.country))
                    .GroupBy(o => o.country)
                    .Select(g => new OverallSummary {
                        country = g.Key, players = g.Count()
                        , gold = g.Count(o => o.rank == 1)
                        , silver = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) < 20)
                        , bronze = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) < 50)
                        , pink = g.Count(o => (((double)(o.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100) >= 50)
                    })
                    .OrderByDescending(s => s.gold)
                    .ThenByDescending(s => s.silver)
                    .ThenByDescending(s => s.bronze)
                    .ThenByDescending(s => s.pink)
                    .ThenByDescending(s => s.players)
                    .ThenBy(s => s.country).ToList();
                int weight = 0;
                for (int i = 0; i < this.overallSummary.Count; i++) {
                    OverallSummary current = this.overallSummary[i];
                    if (current.gold == 0 && current.silver == 0 &&current.bronze == 0) {
                        break;
                    }
                            
                    if (i > 0) {
                        OverallSummary previous = this.overallSummary[i - 1];
                        if (previous.gold == current.gold && previous.silver == current.silver && previous.bronze == current.bronze) {
                            current.rank = previous.rank;
                            weight++;
                        } else {
                            current.rank = previous.rank + 1 + weight;
                            weight = 0;
                        }
                    } else {
                        current.rank = 1;
                    }
                }
                this.gridOverallSummary.DataSource = this.overallSummary;
            }
        }

        private void LeaderboardDisplay_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
            int index = this.overallRankList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
            if (index != -1) {
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(index + 1)} {Stats.OnlineServiceNickname}";
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                this.mlMyRank.Visible = true;
                int displayedRowCount = this.gridOverallRank.DisplayedRowCount(false);
                int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                this.gridOverallRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                this.myOverallRank = index + 1;
                
                if (this.myOverallRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(this.myOverallRank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100;
                    if (percentage <= 20) {
                        if (this.myOverallRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (this.myOverallRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage <= 50) {
                        if (this.myOverallRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (this.myOverallRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else if (percentage <= 100) {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                    }
                }
            }
            
            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
            this.mlVisitFallalytics.Visible = true;
        }
        
        private void LeaderboardDisplay_Resize(object sender, EventArgs e) {
            this.mpsSpinner01.Location = new Point((this.gridOverallRank.Width - this.mpsSpinner01.Width) / 2, (this.gridOverallRank.Height - this.mpsSpinner01.Height) / 2);
            this.mpsSpinner02.Location = new Point((this.gridLevelRank.Width - this.mpsSpinner02.Width) / 2, (this.gridLevelRank.Height - this.mpsSpinner02.Height) / 2);
            this.mpsSpinner03.Location = new Point((this.gridPlayerList.Width - this.mpsSpinner03.Width) / 2, (this.gridPlayerList.Height - this.mpsSpinner03.Height) / 2);
            this.mpsSpinner04.Location = new Point(this.gridPlayerDetails.Left + ((this.gridPlayerDetails.Width - this.mpsSpinner04.Width) / 2), (this.gridPlayerDetails.Height - this.mpsSpinner04.Height) / 2);
            this.mpsSpinner05.Location = new Point((this.gridWeeklyCrown.Width - this.mpsSpinner05.Width) / 2, (this.gridWeeklyCrown.Height - this.mpsSpinner05.Height) / 2);
            this.lblSearchDescription.Location = new Point((this.gridLevelRank.Width - this.lblSearchDescription.Width) / 2, (this.gridLevelRank.Height - this.lblSearchDescription.Height) / 2);
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            foreach (Control c1 in Controls) {
                if (c1 is MetroLink ml1) {
                    if (ml1.Equals(this.mlMyRank)) {
                        ml1.ForeColor = theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(Color.Fuchsia, 0.6f) : Utils.GetColorBrightnessAdjustment(Color.GreenYellow, 0.5f);
                    } else if (ml1.Equals(this.mlVisitFallalytics)) {
                        ml1.ForeColor = Utils.GetColorBrightnessAdjustment(Color.FromArgb(0, 174, 219), 0.6f);
                    } else {
                        ml1.Theme = theme;
                    }
                    ml1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    ml1.BringToFront();
                } else if (c1 is Label lb1) {
                    lb1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                } else if (c1 is ImageComboBox icb1) {
                    icb1.Theme = theme;
                    icb1.BringToFront();
                } else if (c1 is MetroTabControl mtc1) {
                    mtc1.Theme = theme;
                    foreach (Control c2 in mtc1.Controls) {
                        if (c2 is MetroTabPage mtp2) {
                            mtp2.Theme = theme;
                            foreach (Control c3 in mtp2.Controls) {
                                if (c3 is Grid g3) {
                                    g3.Theme = theme;
                                    g3.BackgroundColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                    g3.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
                                    g3.DefaultCellStyle = this.dataGridViewCellStyle2;
                                    g3.RowTemplate.Height = 32;
                                    g3.SetContextMenuTheme();
                                } else if (c3 is MetroTextBox mtb3) {
                                    mtb3.Theme = theme;
                                } else if (c3 is MetroLabel mlb3) {
                                    mlb3.Theme = theme;
                                    mlb3.ForeColor = theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(Color.Fuchsia, 0.8f) : Color.GreenYellow;
                                } else if (c3 is MetroProgressSpinner mps3) {
                                    mps3.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                                }
                            }
                        }
                    }
                }
            }
            
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(14);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.SelectionForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(16);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.SpringGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            
            this.Theme = theme;
            this.ResumeLayout();
            this.Refresh();
        }
        
        private void cboLevelList_SelectedIndexChanged(object sender, EventArgs e) {
            if (((ImageComboBox)sender).SelectedIndex == -1 || string.Equals(((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0], this.levelKey)) { return; }
            this.levelKey = ((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0];
            // this.totalHeight = 0;
            // this.currentPage = 0;
            this.mtcTabControl.SelectedIndex = 1;
            this.SetLevelRankList(this.levelKey);
        }

        private void metroLink_MouseEnter(object sender, EventArgs e) {
            if (sender.Equals(this.mlMyRank)) {
                this.mlMyRank.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            } else if (sender.Equals(this.mlVisitFallalytics)) {
                this.mlVisitFallalytics.ForeColor = Color.FromArgb(0, 174, 219);
            }
        }
        
        private void metroLink_MouseLeave(object sender, EventArgs e) {
            if (sender.Equals(this.mlMyRank)) {
                this.mlMyRank.ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(Color.Fuchsia, 0.6f) : Utils.GetColorBrightnessAdjustment(Color.GreenYellow, 0.5f);
            } else if (sender.Equals(this.mlVisitFallalytics)) {
                this.mlVisitFallalytics.ForeColor = Utils.GetColorBrightnessAdjustment(Color.FromArgb(0, 174, 219), 0.6f);
            }
        }

        private void SetLevelList() {
            this.cboLevelList.Enabled = false;
            this.cboLevelList.SetImageItemData(new List<ImageItem>());
            this.mpsSpinner02.Visible = true;
            Task.Run(this.GetAvailableLevel).ContinueWith(prevTask => {
                List<ImageItem> roundItemList = new List<ImageItem>();
                foreach (AvailableLevel.LevelInfo level in this.availableLevelList) {
                    foreach (string id in level.ids) {
                        if (LevelStats.ALL.TryGetValue(id, out LevelStats levelStats)) {
                            roundItemList.Add(new ImageItem(Utils.ResizeImageHeight(levelStats.RoundBigIcon, 23), levelStats.Name, Overlay.GetMainFont(15f), new[] { level.queryname, string.Join(";", level.ids), levelStats.IsCreative.ToString() }));
                            break;
                        }
                    }
                }
                roundItemList.Sort((x, y) => {
                    int result = string.Compare(x.Data[2], y.Data[2], StringComparison.OrdinalIgnoreCase);
                    if (result == 0) {
                        result = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
                    } else if (string.Equals(x.Data[2], "false")) {
                        result = -1;
                    }
                    return result;
                });
                this.BeginInvoke((MethodInvoker)delegate {
                    if (prevTask.Result) {
                        this.mpsSpinner02.Visible = false;
                        this.lblSearchDescription.Visible = true;
                        this.cboLevelList.SetImageItemData(roundItemList);
                        this.cboLevelList.Enabled = true;
                    } else {
                        this.mpsSpinner02.Visible = false;
                        this.gridLevelRank.DataSource = this.levelRankNodata;
                        this.mlRefreshList.Visible = false;
                        this.mlVisitFallalytics.Visible = false;
                        this.lblSearchDescription.Text = Multilingual.GetWord("level_detail_no_data_caption");
                        this.lblSearchDescription.Visible = true;
                        this.cboLevelList.Enabled = false;
                    }
                });
            });
        }

        private void SetLevelRankData(int index) {
            this.mtcTabControl.Enabled = true;
            this.mtpLevelRankPage.Text = $@"🏅 {this.cboLevelList.SelectedName} ({this.totalLevelPlayers}{Multilingual.GetWord("level_detail_creative_player_suffix")})";
            this.mpsSpinner02.Visible = false;
            this.spinnerTransition.Stop();
            this.gridLevelRank.DataSource = this.levelRankList;
            Application.DoEvents();
            this.myLevelRank = -1;
            if (index != -1) {
                int displayedRowCount = this.gridLevelRank.DisplayedRowCount(false);
                int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                this.gridLevelRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                
                this.myLevelRank = index + 1;

                if (this.mtcTabControl.SelectedIndex == 1) {
                    this.mlMyRank.Visible = true;
                    this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myLevelRank)} {Stats.OnlineServiceNickname}";
                    if (this.myLevelRank == 1) {
                        this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                    } else {
                        double percentage = ((double)(this.myLevelRank - 1) / (Math.Min(1000, this.totalLevelPlayers) - 1)) * 100;
                        if (percentage <= 20) {
                            if (this.myLevelRank == 2) {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                            } else if (this.myLevelRank == 3) {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                            } else {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                            }
                        } else if (percentage <= 50) {
                            if (this.myLevelRank == 2) {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                            } else if (this.myLevelRank == 3) {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                            } else {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                            }
                        } else if (percentage <= 100) {
                            this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                        }
                    }
                    this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                }
            }
            if (this.mtcTabControl.SelectedIndex == 1) {
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                this.mlVisitFallalytics.Visible = true;
            }
            // this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
            this.BackMaxSize = 38;
            this.BackImage = ((ImageItem)this.cboLevelList.SelectedItem).Image;
            foreach (string key in ((ImageItem)this.cboLevelList.SelectedItem).Data[1].Split(';')) {
                if (this.StatsForm.StatLookup.TryGetValue(key, out LevelStats level)) {
                    this.BackImage = level.RoundBigIcon;
                    break;
                }
            }
            this.BackImagePadding = new Padding(17, (int)(15 + (Math.Ceiling(Math.Max(0, 60 - this.BackImage.Height) / 5f) * 2f)), 0, 0);
            this.mlRefreshList.Location = new Point(this.cboLevelList.Right + 15, this.cboLevelList.Location.Y);
            this.mlRefreshList.Visible = true;
            this.cboLevelList.Enabled = true;
        }

        private void SetLevelRankNoData() {
            this.mtcTabControl.Enabled = true;
            // this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
            this.mpsSpinner02.Visible = false;
            this.spinnerTransition.Stop();
            // this.targetSpinner = null;
            this.gridLevelRank.DataSource = this.levelRankNodata;
            this.mlRefreshList.Visible = false;
            this.mlVisitFallalytics.Visible = false;
            this.BackMaxSize = 32;
            this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.leaderboard_icon : Properties.Resources.leaderboard_gray_icon;
            this.BackImagePadding = new Padding(20, 21, 0, 0);
            this.lblSearchDescription.Text = Multilingual.GetWord("level_detail_no_data_caption");
            this.lblSearchDescription.Visible = true;
            this.mlMyRank.Visible = false;
            this.Invalidate();
        }
        
        private void SetLevelRankList(string queryKey) {
            this.mtcTabControl.Enabled = false;
            this.cboLevelList.Enabled = false;
            this.mlRefreshList.Visible = false;
            this.lblSearchDescription.Visible = false;
            this.mlMyRank.Visible = false;
            this.mlVisitFallalytics.Visible = false;
            this.mpsSpinner02.Visible = true;
            this.mpsSpinner02.BringToFront();
            this.spinnerTransition.Start();
            this.targetSpinner = this.mpsSpinner02;
            this.gridLevelRank.DataSource = this.levelRankNodata;
            try {
                Task.Run(() => this.DataLoadBulk(queryKey)).ContinueWith(prevTask => {
                    int index = this.levelRankList.FindIndex(r => string.Equals(r.onlineServiceId, Stats.OnlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
                    this.BeginInvoke((MethodInvoker)delegate {
                        if (prevTask.Result) {
                            this.SetLevelRankData(index);
                        } else {
                            this.SetLevelRankNoData();
                        }
                        this.refreshTime = DateTime.Now;
                    });
                });
            } catch {
                this.SetLevelRankNoData();
            }
        }

        private bool DataLoadBulk(string queryKey) {
            bool isFound;
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    this.levelRankList = null;
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    string json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={queryKey}&p=1");
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new LevelRankInfoConverter());
                    LevelRank levelRank = JsonSerializer.Deserialize<LevelRank>(json, options);
                    isFound = levelRank.found;
                    if (isFound) {
                        this.totalLevelPlayers = levelRank.total;
                        this.totalPages = (int)Math.Ceiling(Math.Min(1000, this.totalLevelPlayers) / 100f);
                        for (int i = 0; i < levelRank.recordholders.Count; i++) {
                            LevelRank.RankInfo temp = levelRank.recordholders[i];
                            temp.rank = i + 1;
                            levelRank.recordholders[i] = temp;
                        }
                        this.levelRankList = levelRank.recordholders;
                        if (this.totalPages > 1) {
                            var tasks = new List<Task>();
                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                            for (int i = 2; i <= this.totalPages; i++) {
                                int page = i;
                                tasks.Add(Task.Run(async () => {
                                    HttpResponseMessage response = await client.GetAsync($"{this.LEADERBOARD_API_URL}?round={queryKey}&p={page}");
                                    if (response.IsSuccessStatusCode) {
                                        json = await response.Content.ReadAsStringAsync();
                                        levelRank = JsonSerializer.Deserialize<LevelRank>(json, options);
                                        for (int j = 0; j < levelRank.recordholders.Count; j++) {
                                            LevelRank.RankInfo temp = levelRank.recordholders[j];
                                            temp.rank = j + 1 + ((page - 1) * 100);
                                            levelRank.recordholders[j] = temp;
                                        }
                                        this.levelRankList.AddRange(levelRank.recordholders);
                                    }
                                }));
                            }
                            Task.WhenAll(tasks).Wait();
                            this.levelRankList.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                        }
                    } else {
                        this.totalPages = 0;
                        this.totalLevelPlayers = 0;
                    }
                } catch {
                    isFound = false;
                    this.totalPages = 0;
                    this.totalLevelPlayers = 0;
                }
            }
            return isFound;
        }

        private bool GetAvailableLevel() {
            bool result;
            using (ApiWebClient web = new ApiWebClient()) {
                web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                try {
                    string json = web.DownloadString($"{this.AVAILABLE_LEVEL_API_URL}");
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new RoundConverter());
                    var availableLevel = JsonSerializer.Deserialize<AvailableLevel>(json, options);
                    result = availableLevel.found;
                    this.availableLevelList = availableLevel.leaderboards;
                } catch {
                    result = false;
                }
            }
            return result;
        }
        
        private int GetDataGridViewColumnWidth(string columnName) {
            switch (columnName) {
                case "rank":
                    return 80;
                case "level":
                case "show":
                    return 0;
                case "flag":
                    return 35;
                case "platform":
                    return 45;
                case "RoundIcon":
                    return 45;
                case "crowns":
                    return 90;
                case "shards":
                    return 200;
                case "crownIcon":
                    return 50;
                case "shardIcon":
                    return 40;
                case "medal":
                    return 40;
                case "onlineServiceNickname":
                    return 0;
                case "score":
                    return 140;
                case "firstPlaces":
                    return 80;
                case "record":
                    return 180;
                case "finish":
                    return 200;
                case "country":
                    return 0;
                default:
                    return 0;
            }
        }
        
        private void gridOverallRank_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            ((Grid)sender).Columns["isAnonymous"].Visible = false;
            ((Grid)sender).Columns["country"].Visible = false;
            ((Grid)sender).Columns["onlineServiceType"].Visible = false;
            ((Grid)sender).Columns["id"].Visible = false;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["medal"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["platform"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["flag"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("score", pos++, this.GetDataGridViewColumnWidth("score"), $"{Multilingual.GetWord("leaderboard_grid_header_score")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["score"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("firstPlaces", pos++, this.GetDataGridViewColumnWidth("firstPlaces"), $"{Multilingual.GetWord("leaderboard_grid_header_first_places")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["firstPlaces"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
        }
        
        private void gridOverallRank_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            OverallRank.Player info = (OverallRank.Player)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if ((int)Stats.OnlineServiceType == int.Parse(info.onlineServiceType) && string.Equals(Stats.OnlineServiceNickname, info.onlineServiceNickname)) {
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (info.rank == 1) {
                ((Grid)sender).Rows[e.RowIndex].Height = 41;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(19.5f);
            } else if (info.rank == 2) {
                ((Grid)sender).Rows[e.RowIndex].Height = 39;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18.6f);
            } else if (info.rank == 3) {
                ((Grid)sender).Rows[e.RowIndex].Height = 36;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18f);
            }
            
            if (columnName == "rank") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            } else if (columnName == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (columnName == "platform") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord(info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam");
                e.Value = info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon;
            } else if (columnName == "medal") {
                if (info.rank == 1) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100;
                    if (percentage < 20) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage < 50) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (columnName == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (columnName == "score") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"{e.Value:N0}";
            } else if (columnName == "firstPlaces") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                if (info.firstPlaces == 0) {
                    e.Value = "-";
                }
            }
        }
        
        private void gridOverallRank_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.overallRankList == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.overallRankList.Sort(delegate (OverallRank.Player one, OverallRank.Player two) {
                int rankCompare = one.rank.CompareTo(two.rank);
                int scoreCompare = one.score.CompareTo(two.score);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                switch (columnName) {
                    case "medal":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : scoreCompare;
                    case "rank":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : scoreCompare;
                    case "platform":
                        int platformCompare = String.Compare(one.onlineServiceType, two.onlineServiceType, StringComparison.OrdinalIgnoreCase);
                        return platformCompare != 0 ? platformCompare : rankCompare;
                    case "flag":
                        int countryCompare = String.Compare(one.country, two.country, StringComparison.OrdinalIgnoreCase);
                        return countryCompare != 0 ? countryCompare : rankCompare;
                    case "onlineServiceNickname":
                        int nicknameCompare = String.Compare(one.onlineServiceNickname, two.onlineServiceNickname, StringComparison.OrdinalIgnoreCase);
                        return nicknameCompare != 0 ? nicknameCompare : rankCompare;
                    case "score":
                        scoreCompare = one.score.CompareTo(two.score);
                        return scoreCompare != 0 ? scoreCompare : rankCompare;
                    case "firstPlaces":
                        int firstPlacesCompare = one.firstPlaces.CompareTo(two.firstPlaces);
                        return firstPlacesCompare != 0 ? firstPlacesCompare : rankCompare;
                    default:
                        return 0;
                }
            });
            
            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.overallRankList;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridOverallSummary_SelectionChanged(object sender, EventArgs e) {
            ((Grid)sender).ClearSelection();
        }
        
        private void gridOverallSummary_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            ((Grid)sender).Columns["pink"].Visible = false;
            ((Grid)sender).Columns["players"].Visible = false;
            ((Grid)sender).Setup("rank", pos++, 40, $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["flag"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("country", pos++, 0, $"{Multilingual.GetWord("leaderboard_grid_header_country")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["country"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "goldIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("goldIcon", pos++, 25, "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["goldIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("gold", pos++, 70, $"{Multilingual.GetWord("main_gold")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["gold"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "silverIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("silverIcon", pos++, 25, "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["silverIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("silver", pos++, 70, $"{Multilingual.GetWord("main_silver")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["silver"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "bronzeIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("bronzeIcon", pos++, 25, "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["bronzeIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("bronze", pos++, 70, $"{Multilingual.GetWord("main_bronze")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["bronze"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
        }
        
        private void gridOverallSummary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            OverallSummary summary = (OverallSummary)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if (columnName == "rank") {
                if (string.Equals(Stats.HostCountryCode, summary.country, StringComparison.OrdinalIgnoreCase)) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
                }
                if (summary.rank == 0) {
                    e.Value = "-";
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            } else if (columnName == "flag") {
                e.Value = (Image)Properties.Resources.ResourceManager.GetObject($"country_{summary.country.ToLower()}_icon");
            } else if (columnName == "country") {
                e.Value = Multilingual.GetCountryName(summary.country);
                if (string.Equals(Stats.HostCountryCode, summary.country, StringComparison.OrdinalIgnoreCase)) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
                }
            } else if (columnName == "goldIcon") {
                if (summary.gold != 0) { e.Value = Properties.Resources.medal_gold_grid_icon; }
            } else if (columnName == "silverIcon") {
                if (summary.silver != 0) { e.Value = Properties.Resources.medal_silver_grid_icon; }
            } else if (columnName == "bronzeIcon") {
                    if (summary.bronze != 0) { e.Value = Properties.Resources.medal_bronze_grid_icon; }
            } else if (columnName == "gold") {
                if (summary.gold == 0) { e.Value = ""; }
                else {
                    e.CellStyle.ForeColor = string.Equals(Stats.HostCountryCode, summary.country, StringComparison.OrdinalIgnoreCase)
                        ? (this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow) : (this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold);
                }
            } else if (columnName == "silver") {
                if (summary.silver == 0) { e.Value = ""; }
                else {
                    e.CellStyle.ForeColor = string.Equals(Stats.HostCountryCode, summary.country, StringComparison.OrdinalIgnoreCase)
                        ? (this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow) : (this.Theme == MetroThemeStyle.Light ? Color.DimGray : Color.LightGray);
                }
            } else if (columnName == "bronze") {
                if (summary.bronze == 0) { e.Value = ""; }
                else {
                    e.CellStyle.ForeColor = string.Equals(Stats.HostCountryCode, summary.country, StringComparison.OrdinalIgnoreCase)
                        ? (this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow) : (this.Theme == MetroThemeStyle.Light ? Color.Sienna : Color.Chocolate);
                }
            }
        }
        
        private void gridOverallRank_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1 && ((Grid)sender).SelectedRows.Count > 0) {
                OverallRank.Player data = (OverallRank.Player)((Grid)sender).SelectedRows[0].DataBoundItem;
                if (string.IsNullOrEmpty(data.id) || data.isAnonymous) return;
                ((Grid)sender).Enabled = false;
                this.gridPlayerList.Enabled = false;
                this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner04;
                this.mpsSpinner04.BringToFront();
                this.mpsSpinner04.Visible = true;
                this.currentUserId = data.id;
                this.SetPlayerInfo(data.id);
                this.mtcTabControl.SelectedIndex = 2;
            }
        }
        
        private void gridLevelRank_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            // ((Grid)sender).Columns["round"].Visible = false;
            ((Grid)sender).Columns["isAnonymous"].Visible = false;
            ((Grid)sender).Columns["country"].Visible = false;
            ((Grid)sender).Columns["onlineServiceType"].Visible = false;
            ((Grid)sender).Columns["onlineServiceId"].Visible = false;
            ((Grid)sender).Columns["id"].Visible = false;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["medal"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("show", pos++, this.GetDataGridViewColumnWidth("show"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["platform"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["flag"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("record", pos++, this.GetDataGridViewColumnWidth("record"), $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("finish", pos++, this.GetDataGridViewColumnWidth("finish"), $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
            
            // foreach (DataGridViewRow row in ((Grid)sender).Rows) {
            //     this.totalHeight += row.Height;
            // }
        }
        
        private void gridLevelRank_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            LevelRank.RankInfo info = (LevelRank.RankInfo)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if (string.Equals(info.onlineServiceId, Stats.OnlineServiceId)) {
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (info.rank == 1) {
                ((Grid)sender).Rows[e.RowIndex].Height = 41;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(19.5f);
            } else if (info.rank == 2) {
                ((Grid)sender).Rows[e.RowIndex].Height = 39;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18.6f);
            } else if (info.rank == 3) {
                ((Grid)sender).Rows[e.RowIndex].Height = 36;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18f);
            }
            
            if (columnName == "rank") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            } else if (columnName == "show") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = Overlay.GetMainFont(16f);
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (columnName == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (columnName == "platform") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord(info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam");
                e.Value = info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon;
            } else if (columnName == "medal") {
                if (info.rank == 1) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (Math.Min(1000, this.totalLevelPlayers) - 1)) * 100;
                    if (percentage < 20) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage < 50) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (columnName == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (columnName == "record") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (columnName == "finish") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = Overlay.GetMainFont(16f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ((DateTime)e.Value).ToString(Multilingual.GetWord("level_grid_date_format"));
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
        }
        
        private void gridLevelRank_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.levelRankList == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.levelRankList.Sort(delegate (LevelRank.RankInfo one, LevelRank.RankInfo two) {
                int rankCompare = one.rank.CompareTo(two.rank);
                int recordCompare = one.record.CompareTo(two.record);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                switch (columnName) {
                    case "medal":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : recordCompare;
                    case "rank":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : recordCompare;
                    case "show":
                        int showCompare = String.Compare(one.show, two.show, StringComparison.OrdinalIgnoreCase);
                        return showCompare != 0 ? showCompare : rankCompare;
                    case "platform":
                        int platformCompare = String.Compare(one.onlineServiceType, two.onlineServiceType, StringComparison.OrdinalIgnoreCase);
                        return platformCompare != 0 ? platformCompare : rankCompare;
                    case "flag":
                        int countryCompare = String.Compare(one.country, two.country, StringComparison.OrdinalIgnoreCase);
                        return countryCompare != 0 ? countryCompare : rankCompare;
                    case "onlineServiceNickname":
                        int nicknameCompare = String.Compare(one.onlineServiceNickname, two.onlineServiceNickname, StringComparison.OrdinalIgnoreCase);
                        return nicknameCompare != 0 ? nicknameCompare : rankCompare;
                    case "record":
                        recordCompare = one.record.CompareTo(two.record);
                        return recordCompare != 0 ? recordCompare : rankCompare;
                    case "finish":
                        int finishCompare = one.finish.CompareTo(two.finish);
                        return finishCompare != 0 ? finishCompare : rankCompare;
                    default:
                        return 0;
                }
            });
            
            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.levelRankList;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridLevelRank_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1 && ((Grid)sender).SelectedRows.Count > 0) {
                LevelRank.RankInfo data = (LevelRank.RankInfo)((Grid)sender).SelectedRows[0].DataBoundItem;
                if (string.IsNullOrEmpty(data.id) || data.isAnonymous) return;
                ((Grid)sender).Enabled = false;
                this.gridPlayerList.Enabled = false;
                this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner04;
                this.mpsSpinner04.BringToFront();
                this.mpsSpinner04.Visible = true;
                this.currentUserId = data.id;
                this.SetPlayerInfo(data.id);
                this.mtcTabControl.SelectedIndex = 2;
            }
        }
        
        // private void gridLevelRank_Scroll(object sender, ScrollEventArgs e) {
        //     if (this.totalHeight - ((Grid)sender).Height < ((Grid)sender).VerticalScrollingOffset) {
        //         // to do
        //     }
        // }

        private void gridPlayerList_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            ((Grid)sender).Columns["isAnonymous"].Visible = false;
            ((Grid)sender).Columns["country"].Visible = false;
            ((Grid)sender).Columns["onlineServiceType"].Visible = false;
            ((Grid)sender).Columns["onlineServiceId"].Visible = false;
            ((Grid)sender).Columns["id"].Visible = false;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["platform"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["flag"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
        }
        
        private void gridPlayerList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            SearchResult.Player info = (SearchResult.Player)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if (string.Equals(info.onlineServiceId, Stats.OnlineServiceId)) {
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (((Grid)sender).Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            }
        }
        
        private void gridPlayerList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.searchResult == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "onlineServiceNickname"; }
            
            this.searchResult.Sort(delegate (SearchResult.Player one, SearchResult.Player two) {
                int nicknameCompare = String.Compare(one.onlineServiceNickname, two.onlineServiceNickname, StringComparison.OrdinalIgnoreCase);
                int countryCompare = String.Compare(one.country, two.country, StringComparison.OrdinalIgnoreCase);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                switch (columnName) {
                    case "platform":
                        int platformCompare = String.Compare(one.onlineServiceType, two.onlineServiceType, StringComparison.OrdinalIgnoreCase);
                        return platformCompare != 0 ? platformCompare : nicknameCompare;
                    case "flag":
                        countryCompare = String.Compare(one.country, two.country, StringComparison.OrdinalIgnoreCase);
                        return countryCompare != 0 ? countryCompare : nicknameCompare;
                    case "onlineServiceNickname":
                        nicknameCompare = String.Compare(one.onlineServiceNickname, two.onlineServiceNickname, StringComparison.OrdinalIgnoreCase);
                        return nicknameCompare != 0 ? nicknameCompare : countryCompare;
                    default:
                        return 0;
                }
            });
            
            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.searchResult;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridPlayerList_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1 && ((Grid)sender).SelectedRows.Count > 0) {
                if (this.isSearchCompleted) {
                    ((Grid)sender).Enabled = false;
                    this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                    this.spinnerTransition.Start();
                    this.targetSpinner = this.mpsSpinner04;
                    this.mpsSpinner04.BringToFront();
                    this.mpsSpinner04.Visible = true;
                    SearchResult.Player data = (SearchResult.Player)((Grid)sender).SelectedRows[0].DataBoundItem;
                    this.currentUserId = data.id;
                    this.SetPlayerInfo(data.id);
                }
            }
        }

        private void SetPlayerInfo(string userId) {
            this.cboLevelList.Enabled = false;
            this.mtcTabControl.Enabled = false;
            Task.Run(() => {
                using (ApiWebClient web = new ApiWebClient()) {
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    string json = web.DownloadString($"{this.PLAYER_DETAILS_API_URL}{userId}");
                    PlayerStats ps = JsonSerializer.Deserialize<PlayerStats>(json);
                    if (ps.found) {
                        ps.pbs.Sort((p1, p2) => {
                            bool isCreative1 = LevelStats.ALL.TryGetValue(p1.round, out LevelStats l1) && l1.IsCreative;
                            bool isCreative2 = LevelStats.ALL.TryGetValue(p2.round, out LevelStats l2) && l2.IsCreative;
                            int result = isCreative1.CompareTo(isCreative2);
                            return result == 0 ? string.Compare(l1.Name, l2.Name, StringComparison.Ordinal) : result;
                        });
                        this.playerDetails = ps.pbs;
                        this.speedrunRank = ps.speedrunrank;
                        this.crownLeagueRank = ps.crownrank;
                    } else {
                        this.playerDetails = null;
                        this.speedrunRank = null;
                        this.crownLeagueRank = null;
                    }
                }
            }).ContinueWith(prevTask => {
                this.spinnerTransition.Stop();
                // this.targetSpinner = null;
                this.BeginInvoke((MethodInvoker)delegate {
                    this.mtcTabControl.Enabled = true;
                    this.cboLevelList.Enabled = true;
                    this.mtbSearchPlayersText.Width = this.playerDetails == null || this.playerDetails.Count == 0 ? (this.gridPlayerList.Width + this.gridPlayerDetails.Width + 2) : this.gridPlayerList.Width + 1;
                    this.mtbSearchPlayersText.Invalidate();
                    this.mpsSpinner04.Visible = false;
                    this.gridPlayerDetails.DataSource = this.playerDetails ?? this.playerDetailsNodata;
                    this.gridPlayerDetails.ClearSelection();
                    this.gridPlayerList.Enabled = true;
                    this.gridOverallRank.Enabled = true;
                    this.gridLevelRank.Enabled = true;
                    this.gridWeeklyCrown.Enabled = true;
                    if (this.speedrunRank != null) {
                        this.picPlayerInfo01.Image = (string.Equals(this.speedrunRank.Value.onlineServiceType, "0") ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon);
                        this.picPlayerInfo02.Image = string.IsNullOrEmpty(this.speedrunRank.Value.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{this.speedrunRank.Value.country.ToLower()}_icon");
                        this.lblPlayerInfo01.Text = this.speedrunRank.Value.onlineServiceNickname;
                        this.lblPlayerInfo02.Left = this.lblPlayerInfo01.Right + 15;
                        this.lblPlayerInfo02.Text = $@"{Multilingual.GetWord("leaderboard_overall_rank")} :";
                        this.picPlayerInfo03.Left = this.lblPlayerInfo02.Right;
                        double percentage = ((double)(this.speedrunRank.Value.index - 1) / (Math.Min(1000, this.speedrunRank.Value.total) - 1)) * 100;
                        if (this.speedrunRank.Value.index == 0) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_eliminated;
                        } else if (this.speedrunRank.Value.index == 1) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_gold;
                        } else if (percentage <= 20) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_silver;
                        } else if (percentage <= 50) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_bronze;
                        } else if (percentage <= 100) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_pink;
                        } else {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_eliminated;
                        }
                        this.lblPlayerInfo03.Left = this.picPlayerInfo03.Right;
                        this.lblPlayerInfo03.Text = $@"{this.speedrunRank.Value.index} ({this.speedrunRank.Value.total})";
                        this.lblPlayerInfo04.Left = this.lblPlayerInfo03.Right + 15;
                        this.lblPlayerInfo04.Text = $@"{Multilingual.GetWord("leaderboard_grid_header_score")} : {this.speedrunRank.Value.score:N0}";
                        this.lblPlayerInfo05.Left = this.lblPlayerInfo04.Right + 15;
                        this.lblPlayerInfo05.Text = $@"{Multilingual.GetWord("leaderboard_grid_header_first_places")} : {this.speedrunRank.Value.firstPlaces}";
                        if (this.crownLeagueRank == null) {
                            this.gridPlayerDetails.Height = this.gridPlayerList.Height;
                            this.gridPlayerDetails.Top = this.gridPlayerList.Top;
                            this.lblPlayerInfo06.Visible = false;
                            this.lblPlayerInfo07.Visible = false;
                            this.lblPlayerInfo08.Visible = false;
                            this.lblPlayerInfo09.Visible = false;
                            this.lblPlayerInfo10.Visible = false;
                            this.picPlayerInfo04.Visible = false;
                            this.picPlayerInfo05.Visible = false;
                            this.picPlayerInfo06.Visible = false;
                        }
                    }

                    if (this.crownLeagueRank != null) {
                        this.lblPlayerInfo06.Text = $@"{Multilingual.GetWord("leaderboard_weekly_crown_league")} :";
                        int widthOfText = TextRenderer.MeasureText(this.lblPlayerInfo01.Text, this.lblPlayerInfo01.Font).Width;
                        this.lblPlayerInfo06.Left = this.lblPlayerInfo02.Left - widthOfText;
                        this.picPlayerInfo04.Left = this.lblPlayerInfo06.Right;
                        double percentage = ((double)(this.crownLeagueRank.Value.index - 1) / (Math.Min(1000, this.crownLeagueRank.Value.total) - 1)) * 100;
                        if (this.crownLeagueRank.Value.index == 0) {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_eliminated;
                        } else if (this.crownLeagueRank.Value.index == 1) {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_gold;
                        } else if (percentage <= 20) {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_silver;
                        } else if (percentage <= 50) {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_bronze;
                        } else if (percentage <= 100) {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_pink;
                        } else {
                            this.picPlayerInfo04.Image = Properties.Resources.medal_eliminated;
                        }
                        this.lblPlayerInfo07.Left = this.picPlayerInfo04.Right;
                        this.lblPlayerInfo07.Text = $@"{this.crownLeagueRank.Value.index} ({this.crownLeagueRank.Value.total})";
                        
                        this.lblPlayerInfo08.Left = this.lblPlayerInfo07.Right + 15;
                        this.lblPlayerInfo08.Text = $@"{Multilingual.GetWord("leaderboard_grid_header_score")} : {this.crownLeagueRank.Value.score:N0}";
                        
                        
                        this.picPlayerInfo05.Left = this.lblPlayerInfo08.Right + 15;
                        this.lblPlayerInfo09.Left = this.picPlayerInfo05.Right;
                        this.lblPlayerInfo09.Text = $@"{this.crownLeagueRank.Value.crowns:N0}";
                        this.picPlayerInfo06.Left = this.lblPlayerInfo09.Right + 15;
                        this.lblPlayerInfo10.Left = this.picPlayerInfo06.Right - 3;
                        this.lblPlayerInfo10.Text = $@"{this.crownLeagueRank.Value.shards:N0}";
                        this.gridPlayerDetails.Height = this.gridPlayerList.Height - 25;
                        this.gridPlayerDetails.Top = this.gridPlayerList.Top + 25;
                        this.lblPlayerInfo06.Visible = true;
                        this.lblPlayerInfo07.Visible = true;
                        this.lblPlayerInfo08.Visible = true;
                        this.lblPlayerInfo09.Visible = true;
                        this.lblPlayerInfo10.Visible = true;
                        this.picPlayerInfo04.Visible = true;
                        this.picPlayerInfo05.Visible = true;
                        this.picPlayerInfo06.Visible = true;
                    }
                });
            });
        }
        
        private void gridPlayerDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1 && ((Grid)sender).SelectedRows.Count > 0) {
                PlayerStats.PbInfo data = (PlayerStats.PbInfo)((Grid)sender).SelectedRows[0].DataBoundItem;
                if (string.IsNullOrEmpty(data.round)) return;
                this.cboLevelList.SelectedImageItem(this.StatsForm.ReplaceLevelIdInShuffleShow(data.show, data.round), 1);
            }
        }
        
        private void gridPlayerDetails_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            ((Grid)sender).Columns["session"].Visible = false;
            ((Grid)sender).Columns["isAnonymous"].Visible = false;
            ((Grid)sender).Columns["ip"].Visible = false;
            ((Grid)sender).Columns["country"].Visible = false;
            ((Grid)sender).Columns["user"].Visible = false;
            ((Grid)sender).Columns["roundDisplayName"].Visible = false;
            ((Grid)sender).Columns["roundName"].Visible = false;
            ((Grid)sender).Setup("show", pos++, this.GetDataGridViewColumnWidth("show"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["RoundIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("round", pos++, this.GetDataGridViewColumnWidth("level"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["round"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["medal"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("index", pos++, 40, $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleRight);
            ((Grid)sender).Columns["index"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("roundTotal", pos++, 100, "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["roundTotal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("record", pos++, 0, $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("finish", pos++, 0, $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
        }
        
        private void gridPlayerDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            PlayerStats.PbInfo info = (PlayerStats.PbInfo)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if (((Grid)sender).Columns[e.ColumnIndex].Name == "show") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon") {
                if (this.StatsForm.StatLookup.TryGetValue(info.round, out LevelStats l1)) {
                    e.Value = l1.RoundBigIcon;
                } else if (this.StatsForm.StatLookup.TryGetValue(this.StatsForm.ReplaceLevelIdInShuffleShow(info.show, info.round), out LevelStats l2)) {
                    e.Value = l2.RoundBigIcon;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "round") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    if (info.index == 1) {
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                    }
                    e.Value = this.StatsForm.StatLookup.TryGetValue((string)e.Value, out LevelStats l2) ? l2.Name : (string)e.Value;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "medal") {
                if (info.index == 0) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_eliminated");
                    e.Value = Properties.Resources.medal_eliminated_grid_icon;
                } else if (info.index == 1) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.index - 1) / (Math.Min(1000, info.roundTotal) - 1)) * 100;
                    if (percentage <= 20) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "index") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.index == 0 ? (object)"-" : info.index;
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "roundTotal") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"({e.Value})";
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "record") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "finish") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ((DateTime)e.Value).ToString(Multilingual.GetWord("level_grid_date_format"));
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
        }
        
        private void gridPlayerDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.playerDetails == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "round"; }
            
            this.playerDetails.Sort(delegate (PlayerStats.PbInfo one, PlayerStats.PbInfo two) {
                int showCompare = String.Compare(Multilingual.GetShowName(one.show), Multilingual.GetShowName(two.show), StringComparison.Ordinal);
                int roundCompare = String.Compare((this.StatsForm.StatLookup.TryGetValue(one.round, out LevelStats l1) ? l1.Name : one.round), (this.StatsForm.StatLookup.TryGetValue(two.round, out LevelStats l2) ? l2.Name : two.round), StringComparison.Ordinal);
                int rankCompare = one.index.CompareTo(two.index);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }
            
                switch (columnName) {
                    case "show":
                        showCompare = String.Compare(Multilingual.GetShowName(one.show), Multilingual.GetShowName(two.show), StringComparison.Ordinal);
                        return showCompare != 0 ? showCompare : roundCompare;
                    case "roundIcon":
                    case "round":
                        roundCompare = String.Compare(l1.Name, l2.Name, StringComparison.Ordinal);
                        return roundCompare != 0 ? roundCompare : showCompare;
                    case "medal":
                        double onePercentage = ((double)(one.index - 1) / (Math.Min(1000, one.roundTotal) - 1)) * 100;
                        double twoPercentage = ((double)(two.index - 1) / (Math.Min(1000, two.roundTotal) - 1)) * 100;
                        int medalCompare = onePercentage.CompareTo(twoPercentage);
                        return medalCompare != 0 ? medalCompare : roundCompare;
                    case "index":
                        rankCompare = one.index.CompareTo(two.index);
                        return rankCompare != 0 ? rankCompare : roundCompare;
                    case "roundTotal":
                        int totalCompare = one.roundTotal.CompareTo(two.roundTotal);
                        return totalCompare != 0 ? totalCompare : roundCompare;
                    case "record":
                        int recordCompare = one.record.CompareTo(two.record);
                        return recordCompare != 0 ? recordCompare : roundCompare;
                    case "finish":
                        int finishCompare = one.finish.CompareTo(two.finish);
                        return finishCompare != 0 ? finishCompare : roundCompare;
                    default:
                        return 0;
                }
            });
            
            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.playerDetails;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                ((Grid)sender).Cursor = Cursors.Hand;
            }
        }
        
        private void grid_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            ((Grid)sender).Cursor = Cursors.Default;
        }
        
        private void grid_SelectionChanged(object sender, EventArgs e) {
            if (this.isHeaderClicked) {
                ((Grid)sender).ClearSelection();
                this.isHeaderClicked = false;
            }
        }
        
        private void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            if (e.RowIndex == -1) {
                this.isHeaderClicked = true;
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Escape) {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        private void mtbSearchPlayersText_KeyDown(object sender, KeyEventArgs e) {
            if (string.IsNullOrEmpty(this.mtbSearchPlayersText.Text)) { return; }
            if (e.KeyCode == Keys.Enter) {
                this.mtcTabControl.Enabled = false;
                this.cboLevelList.Enabled = false;
                this.mtbSearchPlayersText.Enabled = false;
                e.SuppressKeyPress = true;
                this.gridPlayerList.DataSource = this.searchResultNodata;
                this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                this.mpsSpinner03.Visible = true;
                this.mpsSpinner03.BringToFront();
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner03;
                this.isSearchCompleted = false;
                Task.Run(() => {
                    using (ApiWebClient web = new ApiWebClient()) {
                        web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                        string json = web.DownloadString($"{this.PLAYER_LIST_API_URL}{this.mtbSearchPlayersText.Text}");
                        SearchResult result = JsonSerializer.Deserialize<SearchResult>(json);
                        this.searchResult = result.found ? result.users : null;
                        this.searchResult?.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.onlineServiceNickname, y.onlineServiceNickname));
                    }
                }).ContinueWith(prevTask => {
                    this.spinnerTransition.Stop();
                    // this.targetSpinner = null;
                    this.BeginInvoke((MethodInvoker)delegate {
                        this.mtcTabControl.Enabled = true;
                        this.cboLevelList.Enabled = true;
                        this.mtbSearchPlayersText.Width = this.gridPlayerList.Width + this.gridPlayerDetails.Width + 2;
                        this.gridPlayerDetails.BringToFront();
                        this.gridPlayerDetails.Height = this.gridPlayerList.Height;
                        this.gridPlayerDetails.Top = this.gridPlayerList.Top;
                        this.mpsSpinner03.Visible = false;
                        this.gridPlayerList.DataSource = this.searchResult ?? this.searchResultNodata;
                        this.mtpSearchPlayersPage.Text = this.searchResult == null ? Multilingual.GetWord("leaderboard_search_players") : $"{Multilingual.GetWord("leaderboard_search_players")} ({this.searchResult.Count}{Multilingual.GetWord("level_detail_creative_player_suffix")})";
                        this.gridPlayerList.ClearSelection();
                        this.isSearchCompleted = true;
                        this.mtbSearchPlayersText.Enabled = true;
                    });
                });
            }
        }
        
        private void gridWeeklyCrown_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }
            int pos = 0;
            ((Grid)sender).Columns["isAnonymous"].Visible = false;
            ((Grid)sender).Columns["country"].Visible = false;
            ((Grid)sender).Columns["onlineServiceType"].Visible = false;
            ((Grid)sender).Columns["id"].Visible = false;
            ((Grid)sender).Columns["crowns"].Visible = false;
            ((Grid)sender).Columns["shards"].Visible = false;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["medal"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), Multilingual.GetWord("leaderboard_grid_header_rank"), DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["platform"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            ((Grid)sender).Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["flag"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "crownIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("crownIcon", pos++, this.GetDataGridViewColumnWidth("crownIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["crownIcon"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            ((Grid)sender).Columns["crownIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("crowns", pos++, this.GetDataGridViewColumnWidth("crowns"), Multilingual.GetWord("leaderboard_grid_header_crowns"), DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["crowns"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "shardIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            ((Grid)sender).Setup("shardIcon", pos++, this.GetDataGridViewColumnWidth("shardIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Columns["shardIcon"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            ((Grid)sender).Columns["shardIcon"].DefaultCellStyle.NullValue = null;
            ((Grid)sender).Setup("shards", pos++, 225, Multilingual.GetWord("leaderboard_grid_header_shards"), DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["shards"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("score", pos++, 125, $"{Multilingual.GetWord("leaderboard_grid_header_score")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["score"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("period", pos++, this.GetDataGridViewColumnWidth("period"), $"{Multilingual.GetWord("leaderboard_grid_header_period")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["period"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).ClearSelection();
        }
        
        private void gridWeeklyCrown_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            WeeklyCrown.Player info = (WeeklyCrown.Player)((Grid)sender).Rows[e.RowIndex].DataBoundItem;
            
            if (e.RowIndex % 2 == 0) {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
            
            if ((int)Stats.OnlineServiceType == int.Parse(info.onlineServiceType) && string.Equals(Stats.OnlineServiceNickname, info.onlineServiceNickname)) {
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }

            if (info.rank == 1) {
                ((Grid)sender).Rows[e.RowIndex].Height = 41;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(19.5f);
            } else if (info.rank == 2) {
                ((Grid)sender).Rows[e.RowIndex].Height = 39;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18.6f);
            } else if (info.rank == 3) {
                ((Grid)sender).Rows[e.RowIndex].Height = 36;
                ((Grid)sender).Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(18f);
            }
            
            if (columnName == "rank") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            } else if (columnName == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (columnName == "platform") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord(info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam");
                e.Value = info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon;
            } else if (columnName == "medal") {
                if (info.rank == 1) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (Math.Min(1000, this.StatsForm.totalWeeklyCrownPlayers) - 1)) * 100;
                    if (percentage < 20) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage < 50) {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        if (info.rank == 2) {
                            e.Value = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (info.rank == 3) {
                            e.Value = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            e.Value = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else {
                        ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (columnName == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (columnName == "crowns") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.crowns == 0 ? "-" : $"{e.Value:N0}";
                // e.Value = $"{Math.Truncate(info.score / 60):N0}";
            } else if (columnName == "shards") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.shards == 0 ? "-" : info.shards >= 60 ? $"{e.Value:N0} ⟦👑{Math.Truncate(info.shards / 60):N0}⟧" : e.Value;
                // e.Value = $"{info.score % 60:N0}";
            } else if (columnName == "crownIcon") {
                e.Value = Properties.Resources.crown_grid_icon;
            } else if (columnName == "shardIcon") {
                e.Value = Properties.Resources.shards_grid_icon;
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "score") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"{e.Value:N0}";
            } else if (columnName == "period") {
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = Overlay.GetMainFont(16f);
                e.Value = $@"📆 {this.StatsForm.weeklyCrownPeriod}";
            }
        }
        
        private void gridWeeklyCrown_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.weeklyCrownList == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            if (string.Equals(columnName, "period")) return;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.weeklyCrownList.Sort(delegate (WeeklyCrown.Player one, WeeklyCrown.Player two) {
                int rankCompare = one.rank.CompareTo(two.rank);
                int scoreCompare = one.score.CompareTo(two.score);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                switch (columnName) {
                    case "medal":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : scoreCompare;
                    case "rank":
                        rankCompare = one.rank.CompareTo(two.rank);
                        return rankCompare != 0 ? rankCompare : scoreCompare;
                    case "platform":
                        int platformCompare = String.Compare(one.onlineServiceType, two.onlineServiceType, StringComparison.OrdinalIgnoreCase);
                        return platformCompare != 0 ? platformCompare : rankCompare;
                    case "flag":
                        int countryCompare = String.Compare(one.country, two.country, StringComparison.OrdinalIgnoreCase);
                        return countryCompare != 0 ? countryCompare : rankCompare;
                    case "onlineServiceNickname":
                        int nicknameCompare = String.Compare(one.onlineServiceNickname, two.onlineServiceNickname, StringComparison.OrdinalIgnoreCase);
                        return nicknameCompare != 0 ? nicknameCompare : rankCompare;
                    case "crowns":
                    case "crownIcon":
                        int crownsCompare = (one.crowns).CompareTo(two.crowns);
                        return crownsCompare != 0 ? crownsCompare : scoreCompare;
                    case "shards":
                    case "shardIcon":
                        int shardsCompare = (one.shards).CompareTo(two.shards);
                        return shardsCompare != 0 ? shardsCompare : scoreCompare;
                    case "score":
                        scoreCompare = one.score.CompareTo(two.score);
                        return scoreCompare != 0 ? scoreCompare : rankCompare;
                    default:
                        return 0;
                }
            });
            
            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.weeklyCrownList;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridWeeklyCrown_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1 && ((Grid)sender).SelectedRows.Count > 0) {
                WeeklyCrown.Player data = (WeeklyCrown.Player)((Grid)sender).SelectedRows[0].DataBoundItem;
                if (string.IsNullOrEmpty(data.id) || data.isAnonymous) return;
                ((Grid)sender).Enabled = false;
                this.gridPlayerList.Enabled = false;
                this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner04;
                this.mpsSpinner04.BringToFront();
                this.mpsSpinner04.Visible = true;
                this.currentUserId = data.id;
                this.SetPlayerInfo(data.id);
                this.mtcTabControl.SelectedIndex = 2;
            }
        }

        private void pagingButton_Click(object sender, EventArgs e) {
            this.mlLeftPagingButton.Enabled = false;
            this.mlRightPagingButton.Enabled = false;
            if (sender.Equals(this.mlLeftPagingButton)) {
                this.SetWeeklyCrownList(this.StatsForm.weeklyCrownPrevious);
            } else if (sender.Equals(this.mlRightPagingButton)) {
                this.SetWeeklyCrownList(this.StatsForm.weeklyCrownNext);
            }
        }

        private void SetWeeklyCrownList(string date = "") {
            if (!string.IsNullOrEmpty(date) || this.StatsForm.weeklyCrownList == null || (DateTime.UtcNow > this.StatsForm.weeklyCrownLoadTime &&
                                                                                          (DateTime.UtcNow.Year != this.StatsForm.weeklyCrownLoadTime.Year
                                                                                           || DateTime.UtcNow.Month != this.StatsForm.weeklyCrownLoadTime.Month
                                                                                           || DateTime.UtcNow.Day != this.StatsForm.weeklyCrownLoadTime.Day
                                                                                           || DateTime.UtcNow.Hour != this.StatsForm.weeklyCrownLoadTime.Hour))) {
                this.cboLevelList.Enabled = false;
                this.mlMyRank.Enabled = false;
                this.mtcTabControl.Enabled = false;
                this.mlVisitFallalytics.Enabled = false;
                this.gridWeeklyCrown.DataSource = this.weeklyCrownNodata;
                this.mpsSpinner05.Visible = true;
                this.mpsSpinner05.BringToFront();
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner05;
                Task.Run(() => this.StatsForm.InitializeWeeklyCrownList(date)).ContinueWith(prevTask => {
                    this.weeklyCrownList = this.StatsForm.weeklyCrownList;
                    this.Invoke((MethodInvoker)delegate {
                        this.mtcTabControl.Enabled = true;
                        this.mlMyRank.Enabled = true;
                        this.cboLevelList.Enabled = true;
                        this.mpsSpinner05.Visible = false;
                        this.spinnerTransition.Stop();
                        // this.targetSpinner = null;
                        this.gridWeeklyCrown.DataSource = prevTask.Result ? this.weeklyCrownList : this.weeklyCrownNodata;
                        this.mlVisitFallalytics.Enabled = true;
                        if (prevTask.Result) {
                            // this.mtpWeeklyCrownPage.Text = $@"📆 {Utils.GetWeekString(this.StatsForm.leaderboardWeeklyCrownYear, this.StatsForm.leaderboardWeeklyCrownWeek)}";
                            this.lblPagingInfo.Font = Overlay.GetMainFont(23f);
                            this.lblPagingInfo.Text = $@"📆 {Utils.GetWeekString(this.StatsForm.weeklyCrownCurrentYear, this.StatsForm.weeklyCrownCurrentWeek)}";
                            int index = this.weeklyCrownList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                            this.myWeeklyCrownRank = index + 1;
                            this.mlMyRank.Visible = index != -1;
                            if (this.mtcTabControl.SelectedIndex == 3 && index != -1) {
                                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myWeeklyCrownRank)} {Stats.OnlineServiceNickname}";
                                int displayedRowCount = this.gridWeeklyCrown.DisplayedRowCount(false);
                                int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                                this.gridWeeklyCrown.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                                if (this.myWeeklyCrownRank == 1) {
                                    this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                                } else {
                                    double percentage = ((double)(this.myWeeklyCrownRank - 1) / (Math.Min(1000, this.StatsForm.totalWeeklyCrownPlayers) - 1)) * 100;
                                    if (percentage <= 20) {
                                        if (this.myWeeklyCrownRank == 2) {
                                            this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                                        } else if (this.myWeeklyCrownRank == 3) {
                                            this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                                        } else {
                                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                                        }
                                    } else if (percentage <= 50) {
                                        if (this.myWeeklyCrownRank == 2) {
                                            this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                                        } else if (this.myWeeklyCrownRank == 3) {
                                            this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                                        } else {
                                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                                        }
                                    } else if (percentage <= 100) {
                                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                                    } else {
                                        this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                                    }
                                }
                                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                            }
                            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                            this.lblPagingInfo.Left = (this.ClientSize.Width / 2) - (this.lblPagingInfo.Width / 2);
                            this.mlLeftPagingButton.Left = this.lblPagingInfo.Left - this.mlRightPagingButton.Width - 5;
                            this.mlRightPagingButton.Left = this.lblPagingInfo.Right + 5;
                            this.lblPagingInfo.Visible = true;
                            this.mlLeftPagingButton.Enabled = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownPrevious);
                            this.mlRightPagingButton.Enabled = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownNext);
                            this.mlLeftPagingButton.Visible = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownPrevious);
                            this.mlRightPagingButton.Visible = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownNext);
                            this.mtpWeeklyCrownPage.Invalidate();
                        } else {
                            // this.mtpWeeklyCrownPage.Text = Multilingual.GetWord("leaderboard_weekly_crown_league");
                            this.weeklyCrownList = null;
                            this.lblPagingInfo.Visible = false;
                            this.mlLeftPagingButton.Visible = false;
                            this.mlRightPagingButton.Visible = false;
                        }
                    });
                });
            } else {
                this.weeklyCrownList = this.StatsForm.weeklyCrownList;
                this.gridWeeklyCrown.DataSource = this.weeklyCrownList;
                int index = this.weeklyCrownList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                this.myWeeklyCrownRank = index + 1;
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                this.mlMyRank.Visible = index != -1;
                if (this.mtcTabControl.SelectedIndex == 3 && index != -1) {
                    this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myWeeklyCrownRank)} {Stats.OnlineServiceNickname}";
                    if (this.myWeeklyCrownRank == 1) {
                        this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                    } else { 
                        double percentage = ((double)(this.myWeeklyCrownRank - 1) / (Math.Min(1000, this.StatsForm.totalWeeklyCrownPlayers) - 1)) * 100;
                        if (percentage <= 20) {
                            if (this.myWeeklyCrownRank == 2) {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                            } else if (this.myWeeklyCrownRank == 3) {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                            } else {
                                this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                            }
                        } else if (percentage <= 50) {
                            if (this.myWeeklyCrownRank == 2) {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                            } else if (this.myWeeklyCrownRank == 3) {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                            } else {
                                this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                            }
                        } else if (percentage <= 100) {
                            this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                        }
                    }
                    this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                }
                this.lblPagingInfo.Font = Overlay.GetMainFont(23f);
                this.lblPagingInfo.Text = $@"📆 {Utils.GetWeekString(this.StatsForm.weeklyCrownCurrentYear, this.StatsForm.weeklyCrownCurrentWeek)}";
                this.lblPagingInfo.Left = (this.ClientSize.Width / 2) - (this.lblPagingInfo.Width / 2);
                this.mlLeftPagingButton.Left = this.lblPagingInfo.Left - this.mlRightPagingButton.Width - 5;
                this.mlRightPagingButton.Left = this.lblPagingInfo.Right + 5;
                this.lblPagingInfo.Visible = true;
                this.mlLeftPagingButton.Enabled = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownPrevious);
                this.mlRightPagingButton.Enabled = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownNext);
                this.mlLeftPagingButton.Visible = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownPrevious);
                this.mlRightPagingButton.Visible = !string.IsNullOrEmpty(this.StatsForm.weeklyCrownNext);
            }
        }

        private void mtcTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.mtcTabControl.SelectedIndex == 0) {
                this.mlMyRank.Visible = this.myOverallRank != -1;
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myOverallRank)} {Stats.OnlineServiceNickname}";
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.myOverallRank != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                if (this.myOverallRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(this.myOverallRank - 1) / (Math.Min(1000, this.StatsForm.totalOverallRankPlayers) - 1)) * 100;
                    if (percentage <= 20) {
                        if (this.myOverallRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (this.myOverallRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage <= 50) {
                        if (this.myOverallRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (this.myOverallRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else if (percentage <= 100) {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                    }
                }
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                this.lblPagingInfo.Visible = false;
                this.mlLeftPagingButton.Visible = false;
                this.mlRightPagingButton.Visible = false;
            } else if (this.mtcTabControl.SelectedIndex == 1) {
                this.mlMyRank.Visible = this.myLevelRank != -1;
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myLevelRank)} {Stats.OnlineServiceNickname}";
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.myLevelRank != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                if (this.myLevelRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_1st_grid_icon;
                } else {
                    double percentage = ((double)(this.myLevelRank - 1) / (Math.Min(1000, this.totalLevelPlayers) - 1)) * 100;
                    if (percentage <= 20) {
                        if (this.myLevelRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_2nd_grid_icon;
                        } else if (this.myLevelRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                        }
                    } else if (percentage <= 50) {
                        if (this.myLevelRank == 2) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_2nd_grid_icon;
                        } else if (this.myLevelRank == 3) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_3rd_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                        }
                    } else if (percentage <= 100) {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_eliminated_grid_icon;
                    }
                }
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? -20 : 5));
                this.lblPagingInfo.Visible = false;
                this.mlLeftPagingButton.Visible = false;
                this.mlRightPagingButton.Visible = false;
            } else if (this.mtcTabControl.SelectedIndex == 2) {
                this.mlMyRank.Visible = false;
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                this.lblPagingInfo.Visible = false;
                this.mlLeftPagingButton.Visible = false;
                this.mlRightPagingButton.Visible = false;
            } else if (this.mtcTabControl.SelectedIndex == 3) {
                this.SetWeeklyCrownList();
            }
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.mlVisitFallalytics)) {
                if (this.mtcTabControl.SelectedIndex == 0) {
                    Process.Start("https://fallalytics.com/leaderboards/speedrun-total");
                } else if (this.mtcTabControl.SelectedIndex == 1) {
                    Process.Start(string.IsNullOrEmpty(this.levelKey) ? "https://fallalytics.com/leaderboards/speedrun" : $"https://fallalytics.com/leaderboards/speedrun/{this.levelKey}/1");
                } else if (this.mtcTabControl.SelectedIndex == 2) {
                    Process.Start(this.playerDetails == null ? $"https://fallalytics.com/player-search?q={this.mtbSearchPlayersText.Text}" : $"https://fallalytics.com/player/{this.currentUserId}");
                } else if (this.mtcTabControl.SelectedIndex == 3) {
                    if (this.StatsForm.weeklyCrownCurrentWeek != 0 && this.StatsForm.weeklyCrownCurrentYear != 0) {
                        Process.Start($"https://fallalytics.com/leaderboards/crowns/{this.StatsForm.weeklyCrownCurrentWeek}-{this.StatsForm.weeklyCrownCurrentYear}");
                    } else {
                        Process.Start("https://fallalytics.com/leaderboards/crowns");
                    }
                }
            } else if (sender.Equals(this.mlRefreshList)) {
                if (!string.IsNullOrEmpty(this.levelKey)) {
                    TimeSpan difference = DateTime.Now - this.refreshTime;
                    if (difference.TotalSeconds > 8) {
                        this.mtcTabControl.SelectedIndex = 1;
                        this.SetLevelRankList(this.levelKey);
                    }
                }
            } else if (sender.Equals(this.mlMyRank)) {
                if (this.mtcTabControl.SelectedIndex == 0) {
                    int displayedRowCount = this.gridOverallRank.DisplayedRowCount(false);
                    int index = this.overallRankList.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
                    int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                    this.gridOverallRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                } else if (this.mtcTabControl.SelectedIndex == 1) {
                    int displayedRowCount = this.gridLevelRank.DisplayedRowCount(false);
                    int index = this.levelRankList.FindIndex(r => string.Equals(r.onlineServiceId, Stats.OnlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
                    int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                    this.gridLevelRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                } else if (this.mtcTabControl.SelectedIndex == 3) {
                    int displayedRowCount = this.gridWeeklyCrown.DisplayedRowCount(false);
                    int index = this.weeklyCrownList.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
                    int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                    this.gridWeeklyCrown.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                }
            }
        }
    }
}