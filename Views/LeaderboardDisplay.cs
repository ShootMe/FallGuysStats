using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class LeaderboardDisplay : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        private readonly string AVAILABLE_ROUND_API_URL = "https://data.fallalytics.com/api/leaderboards";
        private readonly string LEADERBOARD_API_URL = "https://data.fallalytics.com/api/leaderboard";
        private readonly string PLAYER_LIST_API_URL = "https://data.fallalytics.com/api/user-search?q=";
        private readonly string PLAYER_DETAILS_API_URL = "https://data.fallalytics.com/api/user-stats?user=";
        private string key = String.Empty;
        private int totalPlayers, totalPages, currentPage, totalHeight;
        private int myLevelRank = -1, myOverallRank = -1, myWeeklyCrownRank = -1;
        private DateTime refreshTime;
        private List<RankRound> availableRoundlist;
        private List<LevelRankInfo> levelRankList;
        private List<LevelRankInfo> levelRankNodata = new List<LevelRankInfo>();
        private List<OverallRankInfo> overallRankList;
        private List<OverallRankInfo> overallRankNodata = new List<OverallRankInfo>();
        private List<SearchPlayer> searchResult;
        private List<SearchPlayer> searchResultNodata = new List<SearchPlayer>();
        private List<PbInfo> playerDetails;
        private List<PbInfo> playerDetailsNodata = new List<PbInfo>();
        private OverallInfo overallInfo;
        private List<WeeklyCrownUser> weeklyCrownList;
        private List<WeeklyCrownUser> weeklyCrownNodata = new List<WeeklyCrownUser>();
        private bool isSearchCompleted;
        private Timer spinnerTransition;
        private bool isIncreasing;
        private MetroProgressSpinner targetSpinner;
        private string currentUserId;
        
        public LeaderboardDisplay() {
            this.InitializeComponent();
            this.Opacity = 0;
            this.cboRoundList.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
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
            this.spinnerTransition = new Timer();
            this.spinnerTransition.Interval = 1;
            this.spinnerTransition.Tick += this.spinnerTransition_Tick;
            
            this.SetTheme(Stats.CurrentTheme);
            this.SetRoundList();
            
            this.mlVisitFallalytics.Text = Multilingual.GetWord("leaderboard_see_full_rankings_in_fallalytics");
            this.mtpOverallRankPage.Text = Multilingual.GetWord("leaderboard_overall_rank");
            this.mtpLevelRankPage.Text = $"🕹️ {Multilingual.GetWord("leaderboard_choose_a_round")}";
            this.mtpSearchPlayersPage.Text = Multilingual.GetWord("leaderboard_search_players");
            this.mtpWeeklyCrownPage.Text = Multilingual.GetWord("leaderboard_weekly_crown_league");
            this.mtbSearchPlayersText.WaterMark = Multilingual.GetWord("leaderboard_search_players_WaterMark");
            this.lblSearchDescription.Text = Multilingual.GetWord("leaderboard_choose_a_round");
            this.lblSearchDescription.Location = new Point((this.gridLevelRank.Width - this.lblSearchDescription.Width) / 2, (this.gridLevelRank.Height - this.lblSearchDescription.Height) / 2);
            this.mpsSpinner01.Location = new Point((this.gridOverallRank.Width - this.mpsSpinner01.Width) / 2, (this.gridOverallRank.Height - this.mpsSpinner01.Height) / 2);
            this.mpsSpinner02.Location = new Point((this.gridLevelRank.Width - this.mpsSpinner02.Width) / 2, (this.gridLevelRank.Height - this.mpsSpinner02.Height) / 2);
            this.mpsSpinner03.Location = new Point((this.gridPlayerList.Width - this.mpsSpinner03.Width) / 2, (this.gridPlayerList.Height - this.mpsSpinner03.Height) / 2);
            this.mpsSpinner04.Location = new Point(this.gridPlayerDetails.Left + ((this.gridPlayerDetails.Width - this.mpsSpinner04.Width) / 2), (this.gridPlayerDetails.Height - this.mpsSpinner04.Height) / 2);
            this.mpsSpinner05.Location = new Point((this.gridWeeklyCrown.Width - this.mpsSpinner05.Width) / 2, (this.gridWeeklyCrown.Height - this.mpsSpinner05.Height) / 2);
            
            this.gridLevelRank.DataSource = this.levelRankNodata;
            this.gridOverallRank.DataSource = this.overallRankNodata;
            this.gridPlayerList.DataSource = this.searchResultNodata;
            this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
            this.gridWeeklyCrown.DataSource = this.weeklyCrownNodata;
            this.overallRankList = this.StatsForm.leaderboardOverallRankList;
            if (this.overallRankList == null && (DateTime.UtcNow - this.StatsForm.overallRankLoadTime).TotalHours >= 12) {
                this.mpsSpinner01.Visible = true;
                this.mpsSpinner01.BringToFront();
                this.spinnerTransition.Start();
                this.targetSpinner = this.mpsSpinner01;
                Task.Run(() => this.StatsForm.InitializeOverallRankList()).ContinueWith(prevTask => {
                    this.overallRankList = this.StatsForm.leaderboardOverallRankList;
                    this.BeginInvoke((MethodInvoker)delegate {
                        int index = this.overallRankList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                        if (this.mtcTabControl.SelectedIndex == 0 && index != -1) {
                            this.myOverallRank = index + 1;
                            this.mlMyRank.Visible = true;
                            this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myOverallRank)} {Stats.OnlineServiceNickname}";
                            this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
                            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3);
                        }
                        this.mpsSpinner01.Visible = false;
                        this.spinnerTransition.Stop();
                        this.targetSpinner = null;
                        this.gridOverallRank.DataSource = prevTask.Result ? this.overallRankList : this.overallRankNodata;
                        this.gridOverallRank.ClearSelection();
                        if (!prevTask.Result) this.overallRankList = null;
                        if (index != -1) {
                            int displayedRowCount = this.gridOverallRank.DisplayedRowCount(false);
                            int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                            this.gridOverallRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                        }
                    });
                });
            } else {
                this.mpsSpinner01.Visible = false;
                this.spinnerTransition.Stop();
                this.targetSpinner = null;
                this.gridOverallRank.DataSource = this.overallRankList;
                this.gridOverallRank.ClearSelection();
            }

            if (this.StatsForm.leaderboardWeeklyCrownList != null) {
                this.mtpWeeklyCrownPage.Text = $@"👑 {Utils.GetWeekString(this.StatsForm.leaderboardWeeklyCrownYear, this.StatsForm.leaderboardWeeklyCrownWeek)}";
            }
        }

        private void LeaderboardDisplay_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
            int index = this.overallRankList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
            if (index != -1) {
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(index + 1)} {Stats.OnlineServiceNickname}";
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
                this.mlMyRank.Visible = true;
                int displayedRowCount = this.gridOverallRank.DisplayedRowCount(false);
                int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                this.gridOverallRank.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                this.myOverallRank = index + 1;
            }
            
            if (this.myOverallRank == 1) {
                this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
            } else {
                double percentage = ((double)(this.myOverallRank - 1) / (1000 - 1)) * 100;
                if (percentage <= 20) {
                    this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                } else if (percentage <= 50) {
                    this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                } else {
                    this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                }
            }
            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.mtcTabControl.Top + 5);
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
                    } else if (ml1.Equals(this.mlRefreshList)) {
                        ml1.Theme = theme;
                    }
                    ml1.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    ml1.BringToFront();
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
                                    mlb3.ForeColor = theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
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
        
        private void cboRoundList_SelectedIndexChanged(object sender, EventArgs e) {
            if (((ImageComboBox)sender).SelectedIndex == -1 || ((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0].Equals(this.key)) { return; }
            this.key = ((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0];
            // this.totalHeight = 0;
            this.currentPage = 0;
            this.mtcTabControl.SelectedIndex = 1;
            this.SetGridList(this.key);
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

        private void SetRoundList() {
            this.cboRoundList.Enabled = false;
            this.cboRoundList.SetImageItemData(new List<ImageItem>());
            this.mpsSpinner02.Visible = true;
            Task.Run(() => this.DataLoad()).ContinueWith(prevTask => {
                List<ImageItem> roundItemList = new List<ImageItem>();
                foreach (RankRound round in this.availableRoundlist) {
                    foreach (string id in round.ids) {
                        if (LevelStats.ALL.TryGetValue(id, out LevelStats levelStats)) {
                            roundItemList.Add(new ImageItem(Utils.ResizeImageHeight(levelStats.RoundBigIcon, 23), levelStats.Name, Overlay.GetMainFont(15f), new[] { round.queryname, levelStats.Id }));
                            break;
                        }
                    }
                }
                roundItemList.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text));
                this.BeginInvoke((MethodInvoker)delegate {
                    if (prevTask.Result) {
                        this.mpsSpinner02.Visible = false;
                        this.lblSearchDescription.Visible = true;
                        this.cboRoundList.SetImageItemData(roundItemList);
                        this.cboRoundList.Enabled = true;
                    } else {
                        this.mpsSpinner02.Visible = false;
                        this.gridLevelRank.DataSource = this.levelRankNodata;
                        this.mlRefreshList.Visible = false;
                        this.mlVisitFallalytics.Visible = false;
                        this.lblSearchDescription.Text = Multilingual.GetWord("level_detail_no_data_caption");
                        this.lblSearchDescription.Visible = true;
                        this.cboRoundList.Enabled = false;
                    }
                });
            });
        }

        private void SetLeaderboardUI(int index) {
            // this.mtpLevelRankPage.Text = $@"{this.cboRoundList.SelectedName} ({Multilingual.GetWord("leaderboard_total_players_prefix")}{this.totalPlayers}{Multilingual.GetWord("leaderboard_total_players_suffix")})";
            this.mtpLevelRankPage.Text = $@"🏅 {this.cboRoundList.SelectedName} ({this.totalPlayers}{Multilingual.GetWord("level_detail_creative_player_suffix")})";
            this.mpsSpinner02.Visible = false;
            this.spinnerTransition.Stop();
            this.targetSpinner = null;
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
                        this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                    } else {
                        double percentage = ((double)(this.myLevelRank - 1) / (this.totalPlayers - 1)) * 100;
                        if (percentage <= 20) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                        } else if (percentage <= 50) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                        }
                    }
                    this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
                }
            }
            if (this.mtcTabControl.SelectedIndex == 1) {
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                this.mlVisitFallalytics.Visible = true;
            }
            // this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
            this.BackMaxSize = 38;
            this.BackImage = LevelStats.ALL.TryGetValue(((ImageItem)this.cboRoundList.SelectedItem).Data[1], out LevelStats levelStats) ? levelStats.RoundBigIcon : ((ImageItem)this.cboRoundList.SelectedItem).Image;
            this.BackImagePadding = new Padding(17, (int)(15 + (Math.Ceiling(Math.Max(0, 60 - this.BackImage.Height) / 5f) * 2f)), 0, 0);
            this.mlRefreshList.Location = new Point(this.cboRoundList.Right + 15, this.cboRoundList.Location.Y);
            this.mlRefreshList.Visible = true;
            this.cboRoundList.Enabled = true;
        }

        private void SetGridNoData() {
            // this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
            this.mpsSpinner02.Visible = false;
            this.spinnerTransition.Stop();
            this.targetSpinner = null;
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
        
        private void SetGridList(string queryKey) {
            this.cboRoundList.Enabled = false;
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
                    int index = this.levelRankList.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
                    this.BeginInvoke((MethodInvoker)delegate {
                        if (prevTask.Result) {
                            this.SetLeaderboardUI(index);
                        } else {
                            this.SetGridNoData();
                        }
                        this.refreshTime = DateTime.Now;
                    });
                });
            } catch {
                this.SetGridNoData();
            }
        }

        private bool DataLoadBulk(string queryKey) {
            bool isFound;
            string json;
            JsonSerializerOptions options;
            LevelRank levelRank;
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    this.levelRankList = null;
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={queryKey}&p=1");
                    options = new JsonSerializerOptions();
                    options.Converters.Add(new LevelRankInfoConverter());
                    levelRank = JsonSerializer.Deserialize<LevelRank>(json, options);
                    isFound = levelRank.found;
                    if (isFound) {
                        this.totalPlayers = levelRank.total;
                        this.totalPages = (int)Math.Ceiling((this.totalPlayers > 1000 ? 1000 : this.totalPlayers) / 100f);
                        for (int i = 0; i < levelRank.recordholders.Count; i++) {
                            levelRank.recordholders[i].rank = i + 1;
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
                                        options = new JsonSerializerOptions();
                                        options.Converters.Add(new LevelRankInfoConverter());
                                        levelRank = JsonSerializer.Deserialize<LevelRank>(json, options);
                                        for (int j = 0; j < levelRank.recordholders.Count; j++) {
                                            levelRank.recordholders[j].rank = j + 1 + ((page - 1) * 100);
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
                        this.totalPlayers = 0;
                    }
                } catch {
                    isFound = false;
                    this.totalPages = 0;
                    this.totalPlayers = 0;
                }
            }
            return isFound;
        }

        private bool DataLoad(string queryKey = null) {
            bool result;
            using (ApiWebClient web = new ApiWebClient()) {
                web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                if (string.IsNullOrEmpty(queryKey)) {
                    try {
                        string json = web.DownloadString($"{this.AVAILABLE_ROUND_API_URL}");
                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new RoundConverter());
                        var availableRound = JsonSerializer.Deserialize<AvailableRound>(json, options);
                        result = availableRound.found;
                        this.availableRoundlist = availableRound.leaderboards;
                    } catch {
                        result = false;
                    }
                } else {
                    try {
                        string json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={queryKey}&p={this.currentPage + 1}");
                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new LevelRankInfoConverter());
                        LevelRank levelRank = JsonSerializer.Deserialize<LevelRank>(json, options);
                        result = levelRank.found;
                        if (result) {
                            this.totalPlayers = levelRank.total;
                            this.totalPages = (int)Math.Ceiling((this.totalPlayers > 1000 ? 1000 : this.totalPlayers) / 100f);
                            for (int i = 0; i < levelRank.recordholders.Count; i++) {
                                levelRank.recordholders[i].rank = i + 1 + (this.currentPage * 100);
                            }
                            this.levelRankList = levelRank.recordholders;
                        } else {
                            this.totalPages = 0;
                            this.totalPlayers = 0;
                        }
                    } catch {
                        result = false;
                        this.totalPages = 0;
                        this.totalPlayers = 0;
                    }
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
                    return 120;
                case "shards":
                    return 120;
                case "crownIcon":
                    return 40;
                case "shardIcon":
                    return 40;
                case "medal":
                    return 30;
                case "onlineServiceNickname":
                    return 0;
                case "record":
                case "score":
                case "firstPlaces":
                    return 170;
                case "finish":
                    return 200;
                default:
                    return 0;
            }
        }
        
        private void gridOverallRank_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridOverallRank.Columns.Count == 0) { return; }
            int pos = 0;
            this.gridOverallRank.Columns["isAnonymous"].Visible = false;
            this.gridOverallRank.Columns["country"].Visible = false;
            this.gridOverallRank.Columns["onlineServiceType"].Visible = false;
            this.gridOverallRank.Columns["id"].Visible = false;
            this.gridOverallRank.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridOverallRank.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridOverallRank.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridOverallRank.Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridOverallRank.Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridOverallRank.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridOverallRank.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridOverallRank.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridOverallRank.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            this.gridOverallRank.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            this.gridOverallRank.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridOverallRank.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridOverallRank.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridOverallRank.Setup("score", pos++, this.GetDataGridViewColumnWidth("score"), $"{Multilingual.GetWord("leaderboard_grid_header_score")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridOverallRank.Columns["score"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridOverallRank.Setup("firstPlaces", pos++, this.GetDataGridViewColumnWidth("firstPlaces"), $"{Multilingual.GetWord("leaderboard_grid_header_first_places")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridOverallRank.Columns["firstPlaces"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        private void gridOverallRank_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridOverallRank.Rows.Count) { return; }

            OverallRankInfo info = this.gridOverallRank.Rows[e.RowIndex].DataBoundItem as OverallRankInfo;
            if ((int)Stats.OnlineServiceType == int.Parse(info.onlineServiceType) && string.Equals(Stats.OnlineServiceNickname, info.onlineServiceNickname)) {
                this.gridOverallRank.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "rank") {
                e.CellStyle.Font = Overlay.GetMainFont(16f, FontStyle.Bold);
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "medal") {
                if (info.rank == 1) {
                    this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (1000 - 1)) * 100;
                    if (percentage <= 20) {
                        this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.gridOverallRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "score") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"{e.Value:N0}";
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "firstPlaces") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridOverallRank.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                this.gridOverallRank.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
        }
        
        private void gridOverallRank_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.overallRankList == null) return;
            string columnName = this.gridOverallRank.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridOverallRank.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.overallRankList.Sort(delegate (OverallRankInfo one, OverallRankInfo two) {
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
            
            this.gridOverallRank.DataSource = null;
            this.gridOverallRank.DataSource = this.overallRankList;
            this.gridOverallRank.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridOverallRank_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (this.gridOverallRank.SelectedRows.Count > 0) {
                OverallRankInfo data = this.gridOverallRank.SelectedRows[0].DataBoundItem as OverallRankInfo;
                if (string.IsNullOrEmpty(data.id) || data.isAnonymous) return;
                this.gridOverallRank.Enabled = false;
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
            if (this.gridLevelRank.Columns.Count == 0) { return; }
            int pos = 0;
            // this.gridLevelRank.Columns["round"].Visible = false;
            this.gridLevelRank.Columns["isAnonymous"].Visible = false;
            this.gridLevelRank.Columns["country"].Visible = false;
            this.gridLevelRank.Columns["onlineServiceType"].Visible = false;
            this.gridLevelRank.Columns["onlineServiceId"].Visible = false;
            this.gridLevelRank.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridLevelRank.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridLevelRank.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridLevelRank.Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridLevelRank.Setup("show", pos++, this.GetDataGridViewColumnWidth("show"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridLevelRank.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridLevelRank.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridLevelRank.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridLevelRank.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            this.gridLevelRank.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridLevelRank.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridLevelRank.Setup("record", pos++, this.GetDataGridViewColumnWidth("record"), $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridLevelRank.Setup("finish", pos++, this.GetDataGridViewColumnWidth("finish"), $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridLevelRank.Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            // foreach (DataGridViewRow row in this.gridLevelRank.Rows) {
            //     this.totalHeight += row.Height;
            // }
        }
        
        private void gridLevelRank_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridLevelRank.Rows.Count) { return; }

            LevelRankInfo info = this.gridLevelRank.Rows[e.RowIndex].DataBoundItem as LevelRankInfo;
            if (Stats.OnlineServiceId.Equals(info.onlineServiceId)) {
                this.gridLevelRank.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "rank") {
                e.CellStyle.Font = Overlay.GetMainFont(16f, FontStyle.Bold);
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "show") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "medal") {
                if (info.rank == 1) {
                    this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "record") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (this.gridLevelRank.Columns[e.ColumnIndex].Name == "finish") {
                this.gridLevelRank.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ((DateTime)e.Value).ToString(Multilingual.GetWord("level_grid_date_format"));
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridLevelRank.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
                // this.gridLevelRank.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            } else {
                this.gridLevelRank.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
        }
        
        private void gridLevelRank_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.levelRankList == null) return;
            string columnName = this.gridLevelRank.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridLevelRank.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.levelRankList.Sort(delegate (LevelRankInfo one, LevelRankInfo two) {
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
            
            this.gridLevelRank.DataSource = null;
            this.gridLevelRank.DataSource = this.levelRankList;
            this.gridLevelRank.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        // private void gridLevelRank_Scroll(object sender, ScrollEventArgs e) {
        //     if (this.totalHeight - this.gridLevelRank.Height < this.gridLevelRank.VerticalScrollingOffset) {
        //         // to do
        //     }
        // }

        private void gridPlayerList_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridPlayerList.Columns.Count == 0) { return; }
            int pos = 0;
            this.gridPlayerList.Columns["isAnonymous"].Visible = false;
            this.gridPlayerList.Columns["country"].Visible = false;
            this.gridPlayerList.Columns["onlineServiceType"].Visible = false;
            this.gridPlayerList.Columns["onlineServiceId"].Visible = false;
            this.gridPlayerList.Columns["id"].Visible = false;
            this.gridPlayerList.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridPlayerList.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridPlayerList.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridPlayerList.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            this.gridPlayerList.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerList.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridPlayerList.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerList.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        private void gridPlayerList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridPlayerList.Rows.Count) { return; }

            SearchPlayer info = this.gridPlayerList.Rows[e.RowIndex].DataBoundItem as SearchPlayer;
            if (Stats.OnlineServiceId.Equals(info.onlineServiceId)) {
                this.gridPlayerList.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (this.gridPlayerList.Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) this.gridPlayerList.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (this.gridPlayerList.Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    this.gridPlayerList.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridPlayerList.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridPlayerList.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                this.gridPlayerList.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
        }
        
        private void gridPlayerList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.searchResult == null) return;
            string columnName = this.gridPlayerList.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridPlayerList.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "onlineServiceNickname"; }
            
            this.searchResult.Sort(delegate (SearchPlayer one, SearchPlayer two) {
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
            
            this.gridPlayerList.DataSource = null;
            this.gridPlayerList.DataSource = this.searchResult;
            this.gridPlayerList.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridPlayerList_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (this.gridPlayerList.SelectedRows.Count > 0) {
                if (this.isSearchCompleted) {
                    this.gridPlayerList.Enabled = false;
                    this.gridPlayerDetails.DataSource = this.playerDetailsNodata;
                    this.spinnerTransition.Start();
                    this.targetSpinner = this.mpsSpinner04;
                    this.mpsSpinner04.BringToFront();
                    this.mpsSpinner04.Visible = true;
                    SearchPlayer data = this.gridPlayerList.SelectedRows[0].DataBoundItem as SearchPlayer;
                    this.currentUserId = data.id;
                    this.SetPlayerInfo(data.id);
                }
            }
        }

        private void SetPlayerInfo(string userId) {
            Task.Run(() => {
                using (ApiWebClient web = new ApiWebClient()) {
                    web.Headers.Add("X-Authorization-Key", Environment.GetEnvironmentVariable("FALLALYTICS_KEY"));
                    string json = web.DownloadString($"{this.PLAYER_DETAILS_API_URL}{userId}");
                    PlayerStats ps = JsonSerializer.Deserialize<PlayerStats>(json);
                    if (ps.found) {
                        ps.pbs.Sort((g1, g2) => String.Compare(Multilingual.GetRoundName(g1.round), Multilingual.GetRoundName(g2.round), StringComparison.Ordinal));
                        this.playerDetails = ps.pbs;
                        this.overallInfo = ps.speedrunrank;
                    } else {
                        this.playerDetails = null;
                        this.overallInfo = null;
                    }
                }
            }).ContinueWith(prevTask => {
                this.spinnerTransition.Stop();
                this.targetSpinner = null;
                this.BeginInvoke((MethodInvoker)delegate {
                    this.mtbSearchPlayersText.Width = this.playerDetails == null || this.playerDetails.Count == 0 ? 1332 : 351;
                    this.mtbSearchPlayersText.Invalidate();
                    this.mpsSpinner04.Visible = false;
                    this.gridPlayerDetails.DataSource = this.playerDetails ?? this.playerDetailsNodata;
                    this.gridPlayerDetails.ClearSelection();
                    this.gridPlayerList.Enabled = true;
                    this.gridOverallRank.Enabled = true;
                    if (this.overallInfo != null) {
                        this.picPlayerInfo01.Image = (string.Equals(this.overallInfo.onlineServiceType, "0") ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon);
                        this.picPlayerInfo02.Image = string.IsNullOrEmpty(this.overallInfo.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{this.overallInfo.country.ToLower()}_icon");
                        this.lblPlayerInfo01.Text = this.overallInfo.onlineServiceNickname;
                        this.lblPlayerInfo02.Left = this.lblPlayerInfo01.Right + 30;
                        this.lblPlayerInfo02.Text = $@"{Multilingual.GetWord("leaderboard_overall_rank")} :";
                        this.picPlayerInfo03.Left = this.lblPlayerInfo02.Right;
                        double percentage = ((double)(this.overallInfo.index - 1) / (this.overallInfo.total - 1)) * 100;
                        if (this.overallInfo.index == 0) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_eliminated;
                        } else if (this.overallInfo.index == 1) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_gold;
                        } else if (percentage <= 20) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_silver;
                        } else if (percentage <= 50) {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_bronze;
                        } else {
                            this.picPlayerInfo03.Image = Properties.Resources.medal_pink;
                        }
                        this.lblPlayerInfo03.Left = this.picPlayerInfo03.Right;
                        this.lblPlayerInfo03.Text = $@"{this.overallInfo.index} ({this.overallInfo.total})";
                        this.lblPlayerInfo04.Left = this.lblPlayerInfo03.Right + 15;
                        this.lblPlayerInfo04.Text = $@"{Multilingual.GetWord("leaderboard_grid_header_score")} : {this.overallInfo.score:N0}";
                        this.lblPlayerInfo05.Left = this.lblPlayerInfo04.Right + 15;
                        this.lblPlayerInfo05.Text = $@"{Multilingual.GetWord("leaderboard_grid_header_first_places")} : {this.overallInfo.firstPlaces}";
                    }
                });
            });
        }
        
        private void gridPlayerDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (this.gridPlayerDetails.SelectedRows.Count > 0) {
                PbInfo data = this.gridPlayerDetails.SelectedRows[0].DataBoundItem as PbInfo;
                if (string.IsNullOrEmpty(data.round)) return;
                string roundId = string.Equals(Multilingual.GetRoundName(data.round), data.round) ? this.StatsForm.ReplaceLevelIdInShuffleShow(data.show, data.round) : data.round;
                this.cboRoundList.SelectedImageItem(roundId, 1);
            }
        }
        
        private void gridPlayerDetails_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridPlayerDetails.Columns.Count == 0) { return; }
            int pos = 0;
            this.gridPlayerDetails.Columns["session"].Visible = false;
            this.gridPlayerDetails.Columns["isAnonymous"].Visible = false;
            this.gridPlayerDetails.Columns["ip"].Visible = false;
            this.gridPlayerDetails.Columns["country"].Visible = false;
            this.gridPlayerDetails.Columns["user"].Visible = false;
            this.gridPlayerDetails.Columns["roundDisplayName"].Visible = false;
            this.gridPlayerDetails.Columns["roundName"].Visible = false;
            this.gridPlayerDetails.Setup("show", pos++, this.GetDataGridViewColumnWidth("show"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerDetails.Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridPlayerDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridPlayerDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridPlayerDetails.Columns["RoundIcon"].DefaultCellStyle.NullValue = null;
            this.gridPlayerDetails.Setup("round", pos++, this.GetDataGridViewColumnWidth("level"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerDetails.Columns["round"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridPlayerDetails.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridPlayerDetails.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridPlayerDetails.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridPlayerDetails.Setup("index", pos++, 40, $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleRight);
            this.gridPlayerDetails.Columns["index"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridPlayerDetails.Setup("roundTotal", pos++, 100, "", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerDetails.Columns["roundTotal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridPlayerDetails.Setup("record", pos++, 0, $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerDetails.Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridPlayerDetails.Setup("finish", pos++, 0, $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridPlayerDetails.Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        private void gridPlayerDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridPlayerDetails.Rows.Count) { return; }

            PbInfo info = this.gridPlayerDetails.Rows[e.RowIndex].DataBoundItem as PbInfo;
            
            if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "show") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "RoundIcon") {
                if (this.StatsForm.StatLookup.TryGetValue(info.round, out LevelStats l1)) {
                    e.Value = l1.RoundBigIcon;
                } else if (this.StatsForm.StatLookup.TryGetValue(this.StatsForm.ReplaceLevelIdInShuffleShow(info.show, info.round), out LevelStats l2)) {
                    e.Value = l2.RoundBigIcon;
                }
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "round") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    if (info.index == 1) {
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                    }

                    e.Value = string.Equals(Multilingual.GetRoundName((string)e.Value), (string)e.Value)
                        ? Multilingual.GetRoundName(this.StatsForm.ReplaceLevelIdInShuffleShow(info.show, (string)e.Value))
                        : Multilingual.GetRoundName((string)e.Value);
                }
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "medal") {
                if (info.index == 0) {
                    this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_eliminated");
                    e.Value = Properties.Resources.medal_eliminated_grid_icon;
                } else if (info.index == 1) {
                    this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.index - 1) / (info.roundTotal - 1)) * 100;
                    if (percentage <= 20) {
                        this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "index") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "roundTotal") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"({e.Value})";
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "record") {
                if (info.index == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (this.gridPlayerDetails.Columns[e.ColumnIndex].Name == "finish") {
                this.gridPlayerDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ((DateTime)e.Value).ToString(Multilingual.GetWord("level_grid_date_format"));
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridPlayerDetails.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                this.gridPlayerDetails.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
        }
        
        private void gridPlayerDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.playerDetails == null) return;
            string columnName = this.gridPlayerDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridPlayerDetails.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "round"; }
            
            this.playerDetails.Sort(delegate (PbInfo one, PbInfo two) {
                int showCompare = String.Compare(Multilingual.GetShowName(one.show), Multilingual.GetShowName(two.show), StringComparison.Ordinal);
                int roundCompare = String.Compare(Multilingual.GetRoundName(one.round), Multilingual.GetRoundName(two.round), StringComparison.Ordinal);
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
                        roundCompare = String.Compare(Multilingual.GetRoundName(one.round), Multilingual.GetRoundName(two.round), StringComparison.Ordinal);
                        return roundCompare != 0 ? roundCompare : showCompare;
                    case "medal":
                        double onePercentage = ((double)(one.index - 1) / (one.roundTotal - 1)) * 100;
                        double twoPercentage = ((double)(two.index - 1) / (two.roundTotal - 1)) * 100;
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
            
            this.gridPlayerDetails.DataSource = null;
            this.gridPlayerDetails.DataSource = this.playerDetails;
            this.gridPlayerDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
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
            ((Grid)sender).ClearSelection();
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
                        if (this.searchResult != null) {
                            this.searchResult.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.onlineServiceNickname, y.onlineServiceNickname));
                        }
                    }
                }).ContinueWith(prevTask => {
                    this.spinnerTransition.Stop();
                    this.targetSpinner = null;
                    this.BeginInvoke((MethodInvoker)delegate {
                        this.mtbSearchPlayersText.Width = 1332;
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
            if (this.gridWeeklyCrown.Columns.Count == 0) { return; }
            int pos = 0;
            this.gridWeeklyCrown.Columns["isAnonymous"].Visible = false;
            this.gridWeeklyCrown.Columns["country"].Visible = false;
            this.gridWeeklyCrown.Columns["onlineServiceType"].Visible = false;
            this.gridWeeklyCrown.Columns["id"].Visible = false;
            this.gridWeeklyCrown.Columns["crowns"].Visible = false;
            this.gridWeeklyCrown.Columns["shards"].Visible = false;
            this.gridWeeklyCrown.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridWeeklyCrown.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridWeeklyCrown.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridWeeklyCrown.Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), Multilingual.GetWord("leaderboard_grid_header_rank"), DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridWeeklyCrown.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridWeeklyCrown.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridWeeklyCrown.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridWeeklyCrown.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = "" });
            this.gridWeeklyCrown.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridWeeklyCrown.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridWeeklyCrown.Columns.Add(new DataGridViewImageColumn { Name = "crownIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridWeeklyCrown.Setup("crownIcon", pos++, this.GetDataGridViewColumnWidth("crownIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridWeeklyCrown.Columns["crownIcon"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.gridWeeklyCrown.Columns["crownIcon"].DefaultCellStyle.NullValue = null;
            this.gridWeeklyCrown.Setup("crowns", pos++, this.GetDataGridViewColumnWidth("crowns"), Multilingual.GetWord("leaderboard_grid_header_crowns"), DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["crowns"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridWeeklyCrown.Columns.Add(new DataGridViewImageColumn { Name = "shardIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "" });
            this.gridWeeklyCrown.Setup("shardIcon", pos++, this.GetDataGridViewColumnWidth("shardIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridWeeklyCrown.Columns["shardIcon"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.gridWeeklyCrown.Columns["shardIcon"].DefaultCellStyle.NullValue = null;
            this.gridWeeklyCrown.Setup("shards", pos++, this.GetDataGridViewColumnWidth("shards"), Multilingual.GetWord("leaderboard_grid_header_shards"), DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["shards"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridWeeklyCrown.Setup("score", pos++, this.GetDataGridViewColumnWidth("score"), $"{Multilingual.GetWord("leaderboard_grid_header_score")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["score"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridWeeklyCrown.Setup("period", pos++, this.GetDataGridViewColumnWidth("period"), $"{Multilingual.GetWord("leaderboard_grid_header_period")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridWeeklyCrown.Columns["period"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        private void gridWeeklyCrown_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridWeeklyCrown.Rows.Count) { return; }

            WeeklyCrownUser info = this.gridWeeklyCrown.Rows[e.RowIndex].DataBoundItem as WeeklyCrownUser;
            if ((int)Stats.OnlineServiceType == int.Parse(info.onlineServiceType) && string.Equals(Stats.OnlineServiceNickname, info.onlineServiceNickname)) {
                this.gridWeeklyCrown.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            }
            
            if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "rank") {
                e.CellStyle.Font = Overlay.GetMainFont(16f, FontStyle.Bold);
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "medal") {
                if (info.rank == 1) {
                    this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.gridWeeklyCrown.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = info.isAnonymous ? $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}" : e.Value;
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "crowns") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"   {Math.Truncate(info.score / 60):N0}";
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "shards") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"  {info.score % 60:N0}";
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "crownIcon") {
                e.Value = Properties.Resources.crown_grid_icon;
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "shardIcon") {
                e.Value = Properties.Resources.shards_grid_icon;
            } else if (this.gridOverallRank.Columns[e.ColumnIndex].Name == "score") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = $"  {e.Value:N0}";
            } else if (this.gridWeeklyCrown.Columns[e.ColumnIndex].Name == "period") {
                e.Value = $@"📆 {this.StatsForm.leaderboardWeeklyCrownPeriod}";
            }

            if (e.RowIndex % 2 == 0) {
                this.gridWeeklyCrown.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            } else {
                this.gridWeeklyCrown.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.WhiteSmoke : Color.FromArgb(49, 51, 56);
            }
        }
        
        private void gridWeeklyCrown_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.weeklyCrownList == null) return;
            string columnName = this.gridWeeklyCrown.Columns[e.ColumnIndex].Name;
            if (string.Equals(columnName, "period")) return;
            SortOrder sortOrder = this.gridWeeklyCrown.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "rank"; }
            
            this.weeklyCrownList.Sort(delegate (WeeklyCrownUser one, WeeklyCrownUser two) {
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
                        int crownsCompare = (Math.Truncate(one.score / 60)).CompareTo(Math.Truncate(two.score / 60));
                        return crownsCompare != 0 ? crownsCompare : scoreCompare;
                    case "shards":
                    case "shardIcon":
                        int shardsCompare = (one.score % 60).CompareTo(two.score % 60);
                        return shardsCompare != 0 ? shardsCompare : scoreCompare;
                    case "score":
                        scoreCompare = one.score.CompareTo(two.score);
                        return scoreCompare != 0 ? scoreCompare : rankCompare;
                    default:
                        return 0;
                }
            });
            
            this.gridWeeklyCrown.DataSource = null;
            this.gridWeeklyCrown.DataSource = this.weeklyCrownList;
            this.gridWeeklyCrown.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void mtcTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.mtcTabControl.SelectedIndex == 0) {
                this.mlMyRank.Visible = this.myOverallRank != -1;
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myOverallRank)} {Stats.OnlineServiceNickname}";
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.myOverallRank != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                if (this.myOverallRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(this.myOverallRank - 1) / (1000 - 1)) * 100;
                    if (percentage <= 20) {
                        this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    }
                }
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
            } else if (this.mtcTabControl.SelectedIndex == 1) {
                this.mlMyRank.Visible = this.myLevelRank != -1;
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myLevelRank)} {Stats.OnlineServiceNickname}";
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.myLevelRank != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
                if (this.myLevelRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(this.myLevelRank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    }
                }
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
            } else if (this.mtcTabControl.SelectedIndex == 2) {
                this.mlMyRank.Visible = false;
                this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, (Stats.CurrentLanguage == Language.French || Stats.CurrentLanguage == Language.Japanese ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5));
            } else if (this.mtcTabControl.SelectedIndex == 3) {
                if (this.StatsForm.leaderboardWeeklyCrownList == null || (DateTime.UtcNow > this.StatsForm.weeklyCrownLoadTime &&
                                                                          (DateTime.UtcNow.Year != this.StatsForm.weeklyCrownLoadTime.Year
                                                                           || DateTime.UtcNow.Month != this.StatsForm.weeklyCrownLoadTime.Month
                                                                           || DateTime.UtcNow.Day != this.StatsForm.weeklyCrownLoadTime.Day
                                                                           || DateTime.UtcNow.Hour != this.StatsForm.weeklyCrownLoadTime.Hour))) {
                    this.mpsSpinner05.Visible = true;
                    this.mpsSpinner05.BringToFront();
                    this.spinnerTransition.Start();
                    this.targetSpinner = this.mpsSpinner05;
                    Task.Run(() => this.StatsForm.InitializeWeeklyCrownList()).ContinueWith(prevTask => {
                        this.weeklyCrownList = this.StatsForm.leaderboardWeeklyCrownList;
                        this.BeginInvoke((MethodInvoker)delegate {
                            this.mpsSpinner05.Visible = false;
                            this.spinnerTransition.Stop();
                            this.targetSpinner = null;
                            this.gridWeeklyCrown.DataSource = prevTask.Result ? this.weeklyCrownList : this.weeklyCrownNodata;
                            if (prevTask.Result) {
                                this.mtpWeeklyCrownPage.Text = $@"📆 {Utils.GetWeekString(this.StatsForm.leaderboardWeeklyCrownYear, this.StatsForm.leaderboardWeeklyCrownWeek)}";
                                int index = this.weeklyCrownList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                                if (index != -1) {
                                    this.myWeeklyCrownRank = index + 1;
                                    this.mlMyRank.Visible = true;
                                    this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myWeeklyCrownRank)} {Stats.OnlineServiceNickname}";
                                    this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3);
                                    int displayedRowCount = this.gridWeeklyCrown.DisplayedRowCount(false);
                                    int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                                    this.gridWeeklyCrown.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                                    if (this.myWeeklyCrownRank == 1) {
                                        this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                                    } else {
                                        double percentage = ((double)(this.myWeeklyCrownRank - 1) / (1000 - 1)) * 100;
                                        if (percentage <= 20) {
                                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                                        } else if (percentage <= 50) {
                                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                                        } else {
                                            this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                                        }
                                    }
                                    this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
                                }
                            } else {
                                this.mtpWeeklyCrownPage.Text = Multilingual.GetWord("leaderboard_weekly_crown_league");
                                this.weeklyCrownList = null;
                            }
                        });
                    });
                } else {
                    this.weeklyCrownList = this.StatsForm.leaderboardWeeklyCrownList;
                    this.gridWeeklyCrown.DataSource = this.weeklyCrownList;
                    int index = this.weeklyCrownList?.FindIndex(r => string.Equals(Stats.OnlineServiceNickname, r.onlineServiceNickname) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType)) ?? -1;
                    this.myWeeklyCrownRank = index + 1;
                    this.mlMyRank.Visible = index != -1;
                    this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myWeeklyCrownRank)} {Stats.OnlineServiceNickname}";
                    this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? this.mlMyRank.Top - this.mlVisitFallalytics.Height - 3 : this.mtcTabControl.Top + 5);
                    if (this.myWeeklyCrownRank == 1) {
                        this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                    } else {
                        double percentage = ((double)(this.myWeeklyCrownRank - 1) / (1000 - 1)) * 100;
                        if (percentage <= 20) {
                            this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                        } else if (percentage <= 50) {
                            this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                        } else {
                            this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                        }
                    }
                    this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mtcTabControl.Top + 5);
                }
            }
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.mlVisitFallalytics)) {
                if (this.mtcTabControl.SelectedIndex == 0) {
                    Process.Start("https://fallalytics.com/leaderboards/speedrun-total");
                } else if (this.mtcTabControl.SelectedIndex == 1) {
                    Process.Start(string.IsNullOrEmpty(this.key) ? "https://fallalytics.com/leaderboards/speedrun" : $"https://fallalytics.com/leaderboards/speedrun/{this.key}/1");
                } else if (this.mtcTabControl.SelectedIndex == 2) {
                    Process.Start(this.playerDetails == null ? $"https://fallalytics.com/player-search?q={this.mtbSearchPlayersText.Text}" : $"https://fallalytics.com/player/{this.currentUserId}");
                } else if (this.mtcTabControl.SelectedIndex == 3) {
                    Process.Start("https://fallalytics.com/leaderboards");
                }
            } else if (sender.Equals(this.mlRefreshList)) {
                if (!string.IsNullOrEmpty(this.key)) {
                    TimeSpan difference = DateTime.Now - this.refreshTime;
                    if (difference.TotalSeconds > 8) {
                        this.mtcTabControl.SelectedIndex = 1;
                        this.SetGridList(this.key);
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
                    int index = this.levelRankList.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
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