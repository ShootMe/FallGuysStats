using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public partial class LeaderboardDisplay : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        private readonly string LEADERBOARD_API_URL = "https://data.fallalytics.com/api/leaderboard";
        private string key = String.Empty;
        private int totalPlayers;
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
            this.lblTotalPlayers.Theme = theme;
            this.lblTotalPlayers.Location = new Point(this.cboRoundList.Right + 15, this.cboRoundList.Location.Y);
            this.lblSearchDescription.Theme = theme;
            this.lblSearchDescription.Text = $"{Multilingual.GetWord("leaderboard_choose_a_round")}";
            this.lblSearchDescription.Location = new Point((this.ClientSize.Width - this.lblSearchDescription.Width) / 2, (this.ClientSize.Height - this.lblSearchDescription.Height) / 2 + 20);
            this.mpsSpinner.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            // this.mpsSpinner.Location = new Point(this.cboRoundList.Right + 15, this.cboRoundList.Location.Y);
            this.mpsSpinner.Location = new Point((this.ClientSize.Width - this.mpsSpinner.Width) / 2, (this.ClientSize.Height - this.mpsSpinner.Height) / 2 + 20);
            this.cboRoundList.Theme = theme;
            
            this.gridDetails.Theme = theme;
            this.gridDetails.BackgroundColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkMagenta;
            this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(14);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.SpringGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            
            this.gridDetails.SetContextMenuTheme();
            
            this.mlVisitFallalytics.Theme = theme;
            this.mlVisitFallalytics.Text = $@"     {Multilingual.GetWord("leaderboard_see_full_rankings_in_fallalytics")}";
            switch (Stats.CurrentLanguage) {
                case Language.English:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 320, this.cboRoundList.Location.Y);
                    break;
                case Language.French:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 430, this.cboRoundList.Location.Y);
                    break;
                case Language.Korean:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 295, this.cboRoundList.Location.Y);
                    break;
                case Language.Japanese:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 385, this.cboRoundList.Location.Y);
                    break;
                case Language.SimplifiedChinese:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 335, this.cboRoundList.Location.Y);
                    break;
                case Language.TraditionalChinese:
                    this.mlVisitFallalytics.Location = new Point(this.ClientSize.Width - 335, this.cboRoundList.Location.Y);
                    break;
            }
            
            this.Theme = theme;
            this.ResumeLayout();
        }
        
        private void cboRoundList_SelectedIndexChanged(object sender, EventArgs e) {
            if ((ImageItem)((ImageComboBox)sender).SelectedItem == null || ((ImageItem)((ImageComboBox)sender).SelectedItem).DataArray[0].Equals(this.key)) { return; }
            this.key = ((ImageItem)((ImageComboBox)sender).SelectedItem).DataArray[0];
            this.lblTotalPlayers.Visible = false;
            this.lblTotalPlayers.Text = "";
            this.lblSearchDescription.Visible = false;
            this.mpsSpinner.Visible = true;
            this.gridDetails.DataSource = null;
            Task.Run(() => this.DataLoad(this.key)).ContinueWith(prevTask => {
                this.BeginInvoke((MethodInvoker)delegate {
                    if (prevTask.Result) {
                        this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")} - {((ImageItem)((ImageComboBox)sender).SelectedItem).Text}";
                        this.mpsSpinner.Visible = false;
                        this.gridDetails.DataSource = this.recordholders;
                        // this.lblTotalPlayers.Location = new Point(this.cboRoundList.Right + 15, this.cboRoundList.Location.Y);
                        this.lblTotalPlayers.Text = $"{Multilingual.GetWord("leaderboard_total_players_prefix")}{this.totalPlayers}{Multilingual.GetWord("leaderboard_total_players_suffix")}";
                        this.lblTotalPlayers.Visible = true;
                        this.mlVisitFallalytics.Visible = true;
                        this.Refresh();
                    } else {
                        this.Text = $@"     {Multilingual.GetWord("leaderboard_menu_title")}";
                        this.mpsSpinner.Visible = false;
                        this.gridDetails.DataSource = this.nodata;
                        this.lblTotalPlayers.Visible = false;
                        this.mlVisitFallalytics.Visible = false;
                        this.lblSearchDescription.Visible = true;
                        this.Refresh();
                    }
                });
            });
        }

        private void SetRoundList() {
            this.cboRoundList.Enabled = false;
            this.cboRoundList.SetImageItemData(new List<ImageItem>());
            this.mpsSpinner.Visible = true;
            Task.Run(() => this.DataLoad()).ContinueWith(prevTask => {
                this.BeginInvoke((MethodInvoker)delegate {
                    if (prevTask.Result) {
                        this.mpsSpinner.Visible = false;
                        this.lblSearchDescription.Visible = true;
                        List<ImageItem> roundItemList = new List<ImageItem>();
                        foreach (RankRound round in this.roundlist) {
                            foreach (var id in round.ids) {
                                if (LevelStats.ALL.TryGetValue(id, out LevelStats levelStats)) {
                                    roundItemList.Add(new ImageItem(levelStats.RoundIcon, levelStats.Name, Overlay.GetMainFont(14), new[] { round.queryname }));
                                    break;
                                }
                            }
                        }
                        roundItemList.Sort((x, y) => String.CompareOrdinal(x.Text, y.Text));
                        this.cboRoundList.SetImageItemData(roundItemList);
                        this.cboRoundList.Enabled = true;
                        this.cboRoundList.Invalidate();
                    } else {
                        this.mpsSpinner.Visible = false;
                        this.cboRoundList.Invalidate();
                    }
                });
            });
        }

        private bool DataLoad(string round = null) {
            bool result;
            using (ApiWebClient web = new ApiWebClient()) {
                if (string.IsNullOrEmpty(round)) {
                    string json = web.DownloadString($"{this.LEADERBOARD_API_URL}s");
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new RoundConverter());
                    var availableRound = JsonSerializer.Deserialize<AvailableRound>(json, options);
                    result = availableRound.found;
                    this.roundlist = availableRound.leaderboards;
                } else {
                    string json = web.DownloadString($"{this.LEADERBOARD_API_URL}?round={round}");
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new RecordHolderConverter());
                    Leaderboard leaderboard = JsonSerializer.Deserialize<Leaderboard>(json, options);
                    result = leaderboard.found;
                    if (result) {
                        this.totalPlayers = leaderboard.total;
                        for (var i = 0; i < leaderboard.recordholders.Count; i++) {
                            leaderboard.recordholders[i].rank = i + 1;
                        }
                        this.recordholders = leaderboard.recordholders;
                    } else {
                        this.totalPlayers = 0;
                    }
                }
            }
            return result;
        }
        
        private int GetDataGridViewColumnWidth(string columnName, string columnText) {
            int sizeOfText;
            switch (columnName) {
                case "rank":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "show":
                    return 0;
                case "flag":
                case "platform":
                case "medal":
                    return 25;
                case "onlineServiceNickname":
                case "record":
                case "finish":
                    return 0;
                default:
                    return 0;
            }
            
            return sizeOfText + 24;
        }
        
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridDetails.Columns.Count == 0) { return; }
            int pos = 0;
            // this.gridDetails.Columns["round"].Visible = false;
            this.gridDetails.Columns["isAnonymous"].Visible = false;
            this.gridDetails.Columns["country"].Visible = false;
            this.gridDetails.Columns["onlineServiceType"].Visible = false;
            this.gridDetails.Columns["onlineServiceId"].Visible = false;
            this.gridDetails.Setup("rank", pos++, this.GetDataGridViewColumnWidth("rank", $"{Multilingual.GetWord("leaderboard_grid_header_rank")}"), $"{Multilingual.GetWord("leaderboard_grid_header_rank")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["rank"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("show", pos++, this.GetDataGridViewColumnWidth("show", $"{Multilingual.GetWord("leaderboard_grid_header_show")}"), $"{Multilingual.GetWord("leaderboard_grid_header_show")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["show"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "flag", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("flag", pos++, this.GetDataGridViewColumnWidth("flag", ""), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Columns["flag"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "platform", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("platform", pos++, this.GetDataGridViewColumnWidth("platform", ""), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Columns["platform"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("") });
            this.gridDetails.Setup("medal", pos++, this.GetDataGridViewColumnWidth("medal", ""), "", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Columns["medal"].DefaultCellStyle.NullValue = null;
            this.gridDetails.Setup("onlineServiceNickname", pos++, this.GetDataGridViewColumnWidth("onlineServiceNickname", ""), $"{Multilingual.GetWord("leaderboard_grid_header_player")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["onlineServiceNickname"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("record", pos++, this.GetDataGridViewColumnWidth("record", $"{Multilingual.GetWord("leaderboard_grid_header_record")}"), $"{Multilingual.GetWord("leaderboard_grid_header_record")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["record"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("finish", pos++, this.GetDataGridViewColumnWidth("finish", $"{Multilingual.GetWord("leaderboard_grid_header_finish")}"), $"{Multilingual.GetWord("leaderboard_grid_header_finish")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["finish"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }

            RankInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RankInfo;
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "show") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "flag") {
                e.Value = info.isAnonymous ? Properties.Resources.country_unknown_shiny_icon : (string.IsNullOrEmpty(info.country) ? Properties.Resources.country_unknown_shiny_icon : (Image)Properties.Resources.ResourceManager.GetObject($"country_{info.country.ToLower()}_shiny_icon"));
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "platform") {
                e.Value = info.isAnonymous ? null : (info.onlineServiceType == "0" ? Properties.Resources.epic_grid_icon : Properties.Resources.steam_grid_icon);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "medal") {
                if (info.rank == 1) {
                    e.Value = Properties.Resources.medal_gold_grid_icon;
                } else {
                    double percentage = ((double)(info.rank - 1) / (this.totalPlayers - 1)) * 100;
                    if (percentage <= 20) {
                        e.Value = Properties.Resources.medal_silver_grid_icon;
                    } else if (percentage <= 50) {
                        e.Value = Properties.Resources.medal_bronze_grid_icon;
                    } else {
                        e.Value = Properties.Resources.medal_pink_grid_icon;
                    }
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "onlineServiceNickname") {
                if (info.rank == 1) {
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(165, 124, 0) : Color.Gold;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "record") {
                e.Value = Utils.FormatTime((double)e.Value);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "finish") {
                e.Value = Utils.GetRelativeTime((DateTime)e.Value);
            }
            
            if (e.RowIndex % 2 == 0) {
                this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
                this.gridDetails.Rows[e.RowIndex].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            }
        }
        
        // private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
        //     
        // }

        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            this.gridDetails.ClearSelection();
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.mlVisitFallalytics)) {
                Process.Start($"https://fallalytics.com/leaderboards/speedrun/{this.key}");
            }
        }
    }
}