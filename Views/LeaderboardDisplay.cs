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
        private readonly string LEADERBOARD_API_URL = "https://data.fallalytics.com/api/leaderboard";
        private string key = String.Empty;
        private int totalPlayers, totalPages, currentPage, totalHeight, myRank;
        private DateTime refreshTime;
        private List<RankRound> roundlist;
        private List<RankInfo> recordholders;
        private List<RankInfo> nodata = new List<RankInfo>();
        
        public LeaderboardDisplay() {
            this.InitializeComponent();
            this.Opacity = 0;
            this.cboRoundList.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
        }
        
        private void LeaderboardDisplay_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
            this.SetRoundList();
            
            this.gridDetails.CurrentCell = null;
            this.gridDetails.ClearSelection();
            this.gridDetails.MultiSelect = false;
            this.gridDetails.DataSource = this.nodata;
        }

        private void LeaderboardDisplay_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }
        
        private void LeaderboardDisplay_Resize(object sender, EventArgs e) {
            this.mpsSpinner.Location = new Point((this.ClientSize.Width - this.mpsSpinner.Width) / 2, (this.ClientSize.Height - this.mpsSpinner.Height) / 2 + 20);
            this.lblSearchDescription.Location = new Point((this.ClientSize.Width - this.lblSearchDescription.Width) / 2, (this.ClientSize.Height - this.lblSearchDescription.Height) / 2 + 20);
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            this.cboRoundList.Theme = theme;
            this.mlRefreshList.Theme = theme;
            // this.mlMyRank.Theme = theme;
            this.mlMyRank.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.mlMyRank.ForeColor = theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(Color.Fuchsia, 0.6f) : Utils.GetColorBrightnessAdjustment(Color.GreenYellow, 0.5f);
            
            // this.lblPagingInfo.Theme = theme;
            // this.mlFirstPagingButton.Theme = theme;
            // this.mlLastPagingButton.Theme = theme;
            // this.mlLeftPagingButton.Theme = theme;
            // this.mlRightPagingButton.Theme = theme;
            
            this.lblSearchDescription.Theme = theme;
            this.lblSearchDescription.ForeColor = theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow;
            this.lblSearchDescription.Text = $"{Multilingual.GetWord("leaderboard_choose_a_round")}";
            this.lblSearchDescription.Location = new Point((this.ClientSize.Width - this.lblSearchDescription.Width) / 2, (this.ClientSize.Height - this.lblSearchDescription.Height) / 2 + 20);
            this.mpsSpinner.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            // this.mpsSpinner.Location = new Point(this.cboRoundList.Right + 15, this.cboRoundList.Location.Y);
            this.mpsSpinner.Location = new Point((this.ClientSize.Width - this.mpsSpinner.Width) / 2, (this.ClientSize.Height - this.mpsSpinner.Height) / 2 + 20);
            
            this.gridDetails.Theme = theme;
            this.gridDetails.BackgroundColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            this.gridDetails.RowTemplate.Height = 32;
            
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(14);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkMagenta;
            this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(16);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.SpringGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            
            this.gridDetails.SetContextMenuTheme();
            
            // this.mlVisitFallalytics.Theme = theme;
            this.mlVisitFallalytics.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.mlVisitFallalytics.ForeColor = Utils.GetColorBrightnessAdjustment(Color.FromArgb(0, 174, 219), 0.6f);
            this.mlVisitFallalytics.Text = Multilingual.GetWord("leaderboard_see_full_rankings_in_fallalytics");
            // this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, this.mlVisitFallalytics.Location.Y);
            
            this.Theme = theme;
            this.ResumeLayout();
            this.Refresh();
        }
        
        private void cboRoundList_SelectedIndexChanged(object sender, EventArgs e) {
            if (((ImageComboBox)sender).SelectedIndex == -1 || ((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0].Equals(this.key)) { return; }
            this.key = ((ImageItem)((ImageComboBox)sender).SelectedItem).Data[0];
            this.totalHeight = 0;
            this.currentPage = 0;
            this.SetGridList(this.key);
        }

        private void metroLink_MouseEnter(object sender, EventArgs e) {
            if (sender.Equals(this.mlMyRank)) {
                ((MetroLink)sender).ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
            } else if (sender.Equals(this.mlVisitFallalytics)) {
                ((MetroLink)sender).ForeColor = Color.FromArgb(0, 174, 219);
            }
        }
        
        private void metroLink_MouseLeave(object sender, EventArgs e) {
            if (sender.Equals(this.mlMyRank)) {
                ((MetroLink)sender).ForeColor = this.Theme == MetroThemeStyle.Light ? Utils.GetColorBrightnessAdjustment(Color.Fuchsia, 0.6f) : Utils.GetColorBrightnessAdjustment(Color.GreenYellow, 0.5f);
            } else if (sender.Equals(this.mlVisitFallalytics)) {
                ((MetroLink)sender).ForeColor = Utils.GetColorBrightnessAdjustment(Color.FromArgb(0, 174, 219), 0.6f);
            }
        }

        private void SetRoundList() {
            this.cboRoundList.Enabled = false;
            this.cboRoundList.SetImageItemData(new List<ImageItem>());
            this.mpsSpinner.Visible = true;
            Task.Run(() => this.DataLoad()).ContinueWith(prevTask => {
                List<ImageItem> roundItemList = new List<ImageItem>();
                foreach (RankRound round in this.roundlist) {
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
                        this.mpsSpinner.Visible = false;
                        this.lblSearchDescription.Visible = true;
                        this.cboRoundList.SetImageItemData(roundItemList);
                        this.cboRoundList.Enabled = true;
                    } else {
                        this.mpsSpinner.Visible = false;
                        this.gridDetails.DataSource = this.nodata;
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
            this.Text = $@"     {this.cboRoundList.SelectedName} ({Multilingual.GetWord("leaderboard_total_players_prefix")}{this.totalPlayers}{Multilingual.GetWord("leaderboard_total_players_suffix")})";
            this.mpsSpinner.Visible = false;
            this.gridDetails.DataSource = this.recordholders;
            Application.DoEvents();
            if (index != -1) {
                int displayedRowCount = this.gridDetails.DisplayedRowCount(false);
                int firstDisplayedScrollingRowIndex = index - (displayedRowCount / 2);
                this.gridDetails.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
                
                this.myRank = index + 1;
                this.mlMyRank.Visible = true;
                this.mlMyRank.Text = $@"{Utils.AppendOrdinal(this.myRank)} {Stats.OnlineServiceNickname}";
                if (this.myRank == 1) {
                    this.mlMyRank.Image = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(this.myRank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        this.mlMyRank.Image = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.mlMyRank.Image = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.mlMyRank.Image = Properties.Resources.medal_pink_grid_icon;
                    }
                }
                // this.mlMyRank.Location = new Point((this.ClientSize.Width - this.mlMyRank.Width) / 2, this.mlMyRank.Location.Y);
                this.mlMyRank.Location = new Point(this.Width - this.mlMyRank.Width - 5, this.mlMyRank.Location.Y);
            }
            this.mlVisitFallalytics.Location = new Point(this.Width - this.mlVisitFallalytics.Width - 5, index != -1 ? 50 : 78);
            
            this.BackImage = LevelStats.ALL.TryGetValue(((ImageItem)this.cboRoundList.SelectedItem).Data[1], out LevelStats levelStats) ? levelStats.RoundBigIcon : ((ImageItem)this.cboRoundList.SelectedItem).Image;
            this.mlRefreshList.Location = new Point(this.cboRoundList.Right + 15, this.mlRefreshList.Location.Y);
            this.mlRefreshList.Visible = true;
            
            this.mlVisitFallalytics.Visible = true;
            this.cboRoundList.Enabled = true;
            // this.lblPagingInfo.Text = $@"{(this.currentPage * 50) + 1} - {(this.totalPages == this.currentPage + 1 ? this.totalPlayers : (this.currentPage + 1) * 50)}";
            // this.lblPagingInfo.Location = new Point((this.ClientSize.Width - this.lblPagingInfo.Width) / 2, this.lblPagingInfo.Location.Y);
            // this.lblPagingInfo.Visible = this.totalPages > 1;
            // this.mlLeftPagingButton.Enabled = this.currentPage + 1 != 1;
            // this.mlLeftPagingButton.Location = new Point(this.lblPagingInfo.Location.X - this.mlLeftPagingButton.Width - 5, this.mlLeftPagingButton.Location.Y);
            // this.mlLeftPagingButton.Visible = this.totalPages > 1;
            // this.mlFirstPagingButton.Enabled = this.currentPage + 1 != 1;
            // this.mlFirstPagingButton.Location = new Point(this.mlLeftPagingButton.Location.X - this.mlFirstPagingButton.Width - 3, this.mlFirstPagingButton.Location.Y);
            // this.mlFirstPagingButton.Visible = this.totalPages > 1;
            // this.mlRightPagingButton.Enabled = this.currentPage + 1 != this.totalPages;
            // this.mlRightPagingButton.Location = new Point(this.lblPagingInfo.Right + 5, this.mlRightPagingButton.Location.Y);
            // this.mlRightPagingButton.Visible = this.totalPages > 1;
            // this.mlLastPagingButton.Enabled = this.currentPage + 1 != this.totalPages;
            // this.mlLastPagingButton.Location = new Point(this.mlRightPagingButton.Right + 3, this.mlLastPagingButton.Location.Y);
            // this.mlLastPagingButton.Visible = this.totalPages > 1;
            this.Invalidate();
        }

        private void SetGridNoData() {
            this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
            this.mpsSpinner.Visible = false;
            this.gridDetails.DataSource = this.nodata;
            this.mlRefreshList.Visible = false;
            this.mlVisitFallalytics.Visible = false;
            this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.leaderboard_icon : Properties.Resources.leaderboard_gray_icon;
            this.lblSearchDescription.Text = Multilingual.GetWord("level_detail_no_data_caption");
            this.lblSearchDescription.Visible = true;
            this.mlMyRank.Visible = false;
            // this.lblPagingInfo.Visible = false;
            // this.mlLeftPagingButton.Visible = false;
            // this.mlRightPagingButton.Visible = false;
            this.Invalidate();
        }
        
        private void SetGridList(string queryKey) {
            this.cboRoundList.Enabled = false;
            this.mlRefreshList.Visible = false;
            this.lblSearchDescription.Visible = false;
            this.mlMyRank.Visible = false;
            this.mlVisitFallalytics.Visible = false;
            // this.lblPagingInfo.Visible = false;
            // this.mlFirstPagingButton.Visible = false;
            // this.mlLastPagingButton.Visible = false;
            // this.mlLeftPagingButton.Visible = false;
            // this.mlRightPagingButton.Visible = false;
            this.mpsSpinner.Visible = true;
            this.gridDetails.DataSource = this.nodata;

            try {
                Task.Run(() => this.DataLoadBulk(queryKey)).ContinueWith(prevTask => {
                    int index = this.recordholders.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
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
                // ignored
            }

            // if (isFirst) {
            //     Task.Run(() => this.DataLoad(queryKey)).ContinueWith(prevTask => {
            //         if (prevTask.Result) {
            //             int index = this.recordholders.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
            //             if (index == -1) {
            //                 var cancellationTokenSource = new CancellationTokenSource();
            //                 var tasks = new List<Task>();
            //                 HttpClient client = new HttpClient();
            //                 for (int i = 1; i < this.totalPages; i++) {
            //                     int page = i;
            //                     tasks.Add(Task.Run(async () => {
            //                         try {
            //                             HttpResponseMessage response = await client.GetAsync($"{this.LEADERBOARD_API_URL}?round={queryKey}&p={page + 1}", cancellationTokenSource.Token);
            //                             if (response.IsSuccessStatusCode) {
            //                                 string json = await response.Content.ReadAsStringAsync();
            //                                 var options = new JsonSerializerOptions();
            //                                 options.Converters.Add(new RecordHolderConverter());
            //                                 Leaderboard leaderboard = JsonSerializer.Deserialize<Leaderboard>(json, options);
            //                                 int findIndex = leaderboard.recordholders.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
            //                                 if (findIndex != -1) {
            //                                     this.currentPage = page;
            //                                     index = findIndex;
            //                                     for (int j = 0; j < leaderboard.recordholders.Count; j++) {
            //                                         leaderboard.recordholders[j].rank = j + 1 + (this.currentPage * 50);
            //                                     }
            //                                     this.recordholders = leaderboard.recordholders;
            //                                     cancellationTokenSource.Cancel();
            //                                 }
            //                             }
            //                         } catch (OperationCanceledException) {
            //                             // Console.WriteLine("Task was cancelled.");
            //                         }
            //                     }));
            //                 }
            //                 
            //                 try {
            //                     Task.WhenAll(tasks).Wait();
            //                     this.BeginInvoke((MethodInvoker)delegate {
            //                         this.SetLeaderboardUI(index);
            //                     });
            //                 } catch (AggregateException e) {
            //                     // foreach (var innerException in e.InnerExceptions) {
            //                     //     if (innerException is OperationCanceledException) {
            //                     //         Console.WriteLine("Task was cancelled.");
            //                     //     }
            //                     // }
            //                 }
            //             } else {
            //                 this.BeginInvoke((MethodInvoker)delegate {
            //                     this.SetLeaderboardUI(index);
            //                 });
            //             }
            //         } else {
            //             this.SetGridNoData();
            //         }
            //         this.refreshTime = DateTime.Now;
            //     });    
            // } else {
            //     Task.Run(() => this.DataLoad(queryKey)).ContinueWith(prevTask => {
            //         this.BeginInvoke((MethodInvoker)delegate {
            //             int index = this.recordholders.FindIndex(r => Stats.OnlineServiceId.Equals(r.onlineServiceId) && (int)Stats.OnlineServiceType == int.Parse(r.onlineServiceType));
            //             if (prevTask.Result) {
            //                 this.SetLeaderboardUI(index);
            //             } else {
            //                 this.SetGridNoData();
            //             }
            //             this.refreshTime = DateTime.Now;
            //         });
            //     });
            // }
        }

        private bool DataLoadBulk(string queryKey) {
            bool result;
            string json;
            JsonSerializerOptions options;
            Leaderboard leaderboard;
            using (ApiWebClient web = new ApiWebClient()) {
                try {
                    this.recordholders = null;
                    json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={queryKey}&p=1");
                    options = new JsonSerializerOptions();
                    options.Converters.Add(new RecordHolderConverter());
                    leaderboard = JsonSerializer.Deserialize<Leaderboard>(json, options);
                    result = leaderboard.found;
                    if (result) {
                        this.totalPlayers = leaderboard.total;
                        this.totalPages = (int)Math.Ceiling(this.totalPlayers / 50f);
                        for (int i = 0; i < leaderboard.recordholders.Count; i++) {
                            leaderboard.recordholders[i].rank = i + 1;
                        }
                        this.recordholders = leaderboard.recordholders;
                        if (this.totalPages > 1) {
                            var tasks = new List<Task>();
                            HttpClient client = new HttpClient();
                            for (int i = 2; i <= this.totalPages; i++) {
                                int page = i;
                                tasks.Add(Task.Run(async () => {
                                    HttpResponseMessage response = await client.GetAsync($"{this.LEADERBOARD_API_URL}?round={queryKey}&p={page}");
                                    if (response.IsSuccessStatusCode) {
                                        json = await response.Content.ReadAsStringAsync();
                                        options = new JsonSerializerOptions();
                                        options.Converters.Add(new RecordHolderConverter());
                                        leaderboard = JsonSerializer.Deserialize<Leaderboard>(json, options);
                                        for (int j = 0; j < leaderboard.recordholders.Count; j++) {
                                            leaderboard.recordholders[j].rank = j + 1 + ((page - 1) * 50);
                                        }
                                        this.recordholders.AddRange(leaderboard.recordholders);
                                    }
                                }));
                            }
                            Task.WhenAll(tasks).Wait();
                            this.recordholders.Sort((r1, r2) => r1.rank.CompareTo(r2.rank));
                        }
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
            return result;
        }

        private bool DataLoad(string queryKey = null) {
            bool result;
            using (ApiWebClient web = new ApiWebClient()) {
                if (string.IsNullOrEmpty(queryKey)) { // round list
                    try {
                        string json = web.DownloadString($"{this.LEADERBOARD_API_URL}s");
                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new RoundConverter());
                        var availableRound = JsonSerializer.Deserialize<AvailableRound>(json, options);
                        result = availableRound.found;
                        this.roundlist = availableRound.leaderboards;
                    } catch {
                        result = false;
                    }
                } else {
                    try {
                        string json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={queryKey}&p={this.currentPage + 1}");
                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new RecordHolderConverter());
                        Leaderboard leaderboard = JsonSerializer.Deserialize<Leaderboard>(json, options);
                        result = leaderboard.found;
                        if (result) {
                            this.totalPlayers = leaderboard.total;
                            this.totalPages = (int)Math.Ceiling(this.totalPlayers / 50f);
                            for (int i = 0; i < leaderboard.recordholders.Count; i++) {
                                leaderboard.recordholders[i].rank = i + 1 + (this.currentPage * 50);
                            }
                            this.recordholders = leaderboard.recordholders;
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
                    return 50;
                case "show":
                    return 0;
                case "flag":
                    return 35;
                case "platform":
                    return 45;
                case "medal":
                    return 30;
                case "onlineServiceNickname":
                    return 0;
                case "record":
                    return 150;
                case "finish":
                    return 200;
                default:
                    return 0;
            }
        }
        
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridDetails.Columns.Count == 0) { return; }
            int pos = 0;
            // this.gridDetails.Columns["round"].Visible = false;
            this.gridDetails.Columns["isAnonymous"].Visible = false;
            this.gridDetails.Columns["country"].Visible = false;
            this.gridDetails.Columns["onlineServiceType"].Visible = false;
            this.gridDetails.Columns["onlineServiceId"].Visible = false;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("show", pos++, this.GetDataGridViewColumnWidth("show"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform"), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Normal, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag"), "", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname"), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("record", pos++, this.GetDataGridViewColumnWidth("record"), $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("finish", pos++, this.GetDataGridViewColumnWidth("finish"), $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            // foreach (DataGridViewRow row in this.gridDetails.Rows) {
            //     this.totalHeight += row.Height;
            // }
        }
        
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }

            RankInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RankInfo;
            if (Stats.OnlineServiceId.Equals(info.onlineServiceId)) {
                this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Fuchsia : Color.GreenYellow;
                // this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.Font = Overlay.GetMainFont(14f, FontStyle.Bold);
            }
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "rank") {
                e.CellStyle.Font = Overlay.GetMainFont(16f, FontStyle.Bold);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "show") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "flag") {
                if (!info.isAnonymous && !string.IsNullOrEmpty(info.country)) this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetCountryName(info.country);
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_icon"));
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "platform") {
                if (!info.isAnonymous) {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord((info.onlineServiceType == "0" ? "level_detail_online_platform_eos" : "level_detail_online_platform_steam"));
                }
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "medal") {
                if (info.rank == 1) {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "record") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Goldenrod : Color.Gold;
                }
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "finish") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ((DateTime)e.Value).ToString(Multilingual.GetWord("level_grid_date_format"));
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
                // this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            }
        }
        
        // private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
        //     
        // }
        
        // private void gridDetails_Scroll(object sender, ScrollEventArgs e) {
        //     if (this.totalHeight - this.gridDetails.Height < this.gridDetails.VerticalScrollingOffset) {
        //         // to do
        //     }
        // }

        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            this.gridDetails.ClearSelection();
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.mlVisitFallalytics)) {
                // Process.Start($"https://fallalytics.com/leaderboards/speedrun/{this.key}/{this.currentPage + 1}");
                Process.Start($"https://fallalytics.com/leaderboards/speedrun/{this.key}/1");
            } else if (sender.Equals(this.mlRefreshList)) {
                if (!string.IsNullOrEmpty(this.key)) {
                    TimeSpan difference = DateTime.Now - this.refreshTime;
                    if (difference.TotalSeconds > 8) {
                        this.SetGridList(this.key);
                    }
                }
            } else if (sender.Equals(this.mlMyRank)) {
                int displayedRowCount = this.gridDetails.DisplayedRowCount(false);
                int firstDisplayedScrollingRowIndex = (this.myRank - 1) - (displayedRowCount / 2);
                this.gridDetails.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex < 0 ? 0 : firstDisplayedScrollingRowIndex;
            }
            // else if (sender.Equals(this.mlFirstPagingButton)) {
            //     if (!string.IsNullOrEmpty(this.key)) {
            //         TimeSpan difference = DateTime.Now - this.refreshTime;
            //         if (difference.TotalSeconds > 1.6) {
            //             this.currentPage = 0;
            //             this.SetGridList(this.key);
            //         }
            //     }
            // } else if (sender.Equals(this.mlLeftPagingButton)) {
            //     if (!string.IsNullOrEmpty(this.key)) {
            //         TimeSpan difference = DateTime.Now - this.refreshTime;
            //         if (difference.TotalSeconds > 1.6) {
            //             this.currentPage -= 1;
            //             this.SetGridList(this.key);
            //         }
            //     }
            // } else if (sender.Equals(this.mlRightPagingButton)) {
            //     if (!string.IsNullOrEmpty(this.key)) {
            //         TimeSpan difference = DateTime.Now - this.refreshTime;
            //         if (difference.TotalSeconds > 1.6) {
            //             this.currentPage += 1;
            //             this.SetGridList(this.key);
            //         }
            //     }
            // } else if (sender.Equals(this.mlLastPagingButton)) {
            //     if (!string.IsNullOrEmpty(this.key)) {
            //         TimeSpan difference = DateTime.Now - this.refreshTime;
            //         if (difference.TotalSeconds > 1.6) {
            //             this.currentPage = this.totalPages - 1;
            //             this.SetGridList(this.key);
            //         }
            //     }
            // }
        }
    }
}