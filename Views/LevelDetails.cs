using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public partial class LevelDetails : MetroFramework.Forms.MetroForm {
        public string LevelName { get; set; }
        public Image RoundIcon { get; set; }
        public bool IsCreative { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        public Stats StatsForm { get; set; }
        private int _showStats;
        private int currentPage, totalPages;
        private int pageSize = 5000;
        // private int totalHeight;
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        
        public LevelDetails() {
            this.InitializeComponent();
            this.Opacity = 0;
        }
        
        private void LevelDetails_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
            //
            // dataGridViewCellStyle1
            //
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            //
            // dataGridViewCellStyle2
            //
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(14);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            
            this.gridDetails.CurrentCell = null;
            this.gridDetails.ClearSelection();
            this.ClientSize = new Size(this.GetClientWidth(this.LevelName), this.Height + 387);
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(20, 20, 0, 0);
            if (this.LevelName == "Shows") {
                this.gridDetails.Name = "gridShowsStats";
                this.gridDetails.MultiSelect = true;
                this.BackImage = Properties.Resources.fallguys_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_show_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 2;
            } else if (this.LevelName == "Rounds") {
                this.gridDetails.Name = "gridRoundsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_round_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 1;
            } else if (this.LevelName == "Finals") {
                this.gridDetails.Name = "gridFinalsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_final_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 1;
            } else {
                this.gridDetails.Name = "gridRoundStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.RoundIcon;
                this._showStats = 0;
                this.Text = $@"     {Multilingual.GetWord("level_detail_level_stats")} - {(this.IsCreative ? "🛠️ " : "")}{this.LevelName} ({StatsForm.GetCurrentFilterName()})";
            }
            
            this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
            this.currentPage = this.totalPages;
            if (this.totalPages > 1) {
                this.SetPagingUI(true);
                this.EnablePagingUI(false);
                this.gridDetails.Enabled = true;
                // this.lblPagingInfo.Enabled = true;
            }
            this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
            if (this.gridDetails.Rows.Count > 0) this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.Rows.Count - 1;
            
            if (this.gridDetails.RowCount == 0) {
                this.gridDetails.DeallocContextMenu();
            }
            
            if (this._showStats == 2 && this.gridDetails.RowCount > 0) {
                // add separator
                this.gridDetails.MenuSeparator = new ToolStripSeparator();
                this.gridDetails.CMenu.Items.Add(this.gridDetails.MenuSeparator);

                if (this.StatsForm.AllProfiles.Count > 1) {
                    // 
                    // moveShows
                    // 
                    this.gridDetails.MoveShows = new ToolStripMenuItem {
                        Name = "moveShows",
                        Size = new Size(134, 22),
                        Text = Multilingual.GetWord("main_move_shows"),
                        ShowShortcutKeys = true,
                        Image = Properties.Resources.move,
                        ShortcutKeys = Keys.Control | Keys.P
                    };
                    this.gridDetails.MoveShows.Click += this.moveShows_Click;
                    this.gridDetails.CMenu.Items.Add(this.gridDetails.MoveShows);
                }
                
                // 
                // deleteShows
                // 
                this.gridDetails.DeleteShows = new ToolStripMenuItem {
                    Name = "deleteShows",
                    Size = new Size(134, 22),
                    Text = Multilingual.GetWord("main_delete_shows"),
                    ShowShortcutKeys = true,
                    Image = Properties.Resources.delete,
                    ShortcutKeys = Keys.Control | Keys.D
                };
                this.gridDetails.DeleteShows.Click += this.deleteShows_Click;
                this.gridDetails.CMenu.Items.Add(this.gridDetails.DeleteShows);
            }

            if (this._showStats != 2 && this.gridDetails.RowCount > 0) {
                // add separator
                this.gridDetails.MenuSeparator = new ToolStripSeparator();
                this.gridDetails.CMenu.Items.Add(this.gridDetails.MenuSeparator);
                
                // 
                // updateCreativeShows
                // 
                this.gridDetails.UpdateCreativeShows = new ToolStripMenuItem {
                    Name = "updateCreativeShows",
                    Size = new Size(134, 22),
                    Text = Multilingual.GetWord("main_update_shows"),
                    ShowShortcutKeys = true,
                    Image = Properties.Resources.update,
                    ShortcutKeys = Keys.Control | Keys.U
                };
                this.gridDetails.UpdateCreativeShows.Click += this.updateShows_Click;
                this.gridDetails.CMenu.Items.Add(this.gridDetails.UpdateCreativeShows);
            }

            this.gridDetails.SetContextMenuTheme();
        }

        private void SetPagingUI(bool visible) {
            if (visible) {
                this.mlLastPagingButton.Location = new Point(this.gridDetails.Right - this.mlLastPagingButton.Width, this.mlLastPagingButton.Top);
                this.mlRightPagingButton.Location = new Point(this.mlLastPagingButton.Left - this.mlRightPagingButton.Width - 5, this.mlRightPagingButton.Top);
                this.lblPagingInfo.Text = $"{this.currentPage} / {this.totalPages}";
                this.lblPagingInfo.Location = new Point(this.mlRightPagingButton.Left - this.lblPagingInfo.Width - 5, this.lblPagingInfo.Top);
                this.mlLeftPagingButton.Location = new Point(this.lblPagingInfo.Left - this.mlLeftPagingButton.Width - 5, this.mlLeftPagingButton.Top);
                this.mlFirstPagingButton.Location = new Point(this.mlLeftPagingButton.Left - this.mlFirstPagingButton.Width - 5, this.mlFirstPagingButton.Top);
            }
            
            this.mlFirstPagingButton.Visible = visible;
            this.mlLeftPagingButton.Visible = visible;
            this.lblPagingInfo.Visible = visible;
            this.mlRightPagingButton.Visible = visible;
            this.mlLastPagingButton.Visible = visible;
        }

        private void EnablePagingUI(bool enable) {
            this.gridDetails.Enabled = enable;
            this.mlFirstPagingButton.Enabled = enable;
            this.mlLeftPagingButton.Enabled = enable;
            // this.lblPagingInfo.Enabled = enable;
            this.mlRightPagingButton.Enabled = enable;
            this.mlLastPagingButton.Enabled = enable;
        }

        private void pagingButton_Click(object sender, EventArgs e) {
            this.EnablePagingUI(false);
            if (sender.Equals(this.mlFirstPagingButton)) {
                this.currentPage = 1;
                this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                if (this.gridDetails.Rows.Count > 0) {
                    this.SetPagingUI(true);
                    this.gridDetails.FirstDisplayedScrollingRowIndex = 0;
                }
            } else if (sender.Equals(this.mlLeftPagingButton)) {
                this.currentPage -= 1;
                this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                if (this.gridDetails.Rows.Count > 0) {
                    this.SetPagingUI(true);
                    this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.Rows.Count - 1;
                }
            } else if (sender.Equals(this.mlRightPagingButton)) {
                this.currentPage += 1;
                this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                if (this.gridDetails.Rows.Count > 0) {
                    this.SetPagingUI(true);
                    this.gridDetails.FirstDisplayedScrollingRowIndex = 0;
                }
            } else if (sender.Equals(this.mlLastPagingButton)) {
                this.currentPage = this.totalPages;
                this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                if (this.gridDetails.Rows.Count > 0) {
                    this.SetPagingUI(true);
                    this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.Rows.Count - 1;
                }
            }
            this.gridDetails.Enabled = true;
        }

        private void LevelDetails_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();

            this.mlFirstPagingButton.Theme = theme;
            this.mlLeftPagingButton.Theme = theme;
            this.lblPagingInfo.Font = Overlay.GetDefaultFont(23, 0);
            this.lblPagingInfo.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.mlRightPagingButton.Theme = theme;
            this.mlLastPagingButton.Theme = theme;
            
            this.gridDetails.Theme = theme;
            this.gridDetails.BackgroundColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.SelectionForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            // this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkMagenta;
            // this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.SpringGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            this.Theme = theme;
            this.ResumeLayout();
        }
        
        private int GetClientWidth(string level) {
            Language lang = Stats.CurrentLanguage;
            switch (level) {
                case "Shows":
                    return this.Width - (lang == Language.English ? -380 : lang == Language.French ? -400 : lang == Language.Korean ? -370 : lang == Language.Japanese ? -370 : -380);
                case "Rounds":
                    return this.Width + (lang == Language.English ? 1100 : lang == Language.French ? 1200 : lang == Language.Korean ? 1100 : lang == Language.Japanese ? 1100 : 1180);
                case "Finals":
                    return this.Width + (lang == Language.English ? 1100 : lang == Language.French ? 1200 : lang == Language.Korean ? 1100 : lang == Language.Japanese ? 1100 : 1180);
                default:
                    return this.Width + (lang == Language.English ? 1100 : lang == Language.French ? 1200 : lang == Language.Korean ? 1100 : lang == Language.Japanese ? 1100 : 1180);
            }
        }
        
        private int GetDataGridViewColumnWidth(string columnName, string columnText) {
            int sizeOfText;
            switch (columnName) {
                case "RoundIcon":
                    return 37;
                case "Medal":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "IsFinalIcon":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "ShowID":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "ShowNameId":
                    return 0;
                case "Round":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Name":
                    return 0;
                case "Players":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersPs4":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersPs5":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersXb1":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersXsx":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersSw":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersPc":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "PlayersBots":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Start":
                    return 0;
                case "End":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Finish":
                    return 80;
                case "Position":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Score":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Kudos":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                default:
                    return 0;
            }
            
            return sizeOfText + 24;
        }
        
        // private void gridDetails_Scroll(object sender, ScrollEventArgs e) {
        //     if (this.gridDetails.VerticalScrollingOffset == 0) {
        //         if (this.currentPage <= 1) { return; }
        //         this.currentPage -= 1;
        //         this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
        //     } else if (this.totalHeight - this.gridDetails.Height < this.gridDetails.VerticalScrollingOffset) {
        //         if (this.currentPage >= this.totalPages) { return; }
        //         this.currentPage += 1;
        //         this.gridDetails.DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
        //     }
        // }

        // private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            // if (this.gridDetails.Rows.Count > 0) this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.Rows.Count - 1;
        // }
        
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridDetails.Columns.Count == 0) { return; }
            
            if (this.currentPage == 1) {
                this.mlRightPagingButton.Enabled = true;
                this.mlLastPagingButton.Enabled = true;
            } else if (this.currentPage == this.totalPages) {
                this.mlFirstPagingButton.Enabled = true;
                this.mlLeftPagingButton.Enabled = true;
            } else {
                this.mlFirstPagingButton.Enabled = true;
                this.mlLeftPagingButton.Enabled = true;
                this.mlRightPagingButton.Enabled = true;
                this.mlLastPagingButton.Enabled = true;
            }
            
            int pos = 0;
            this.gridDetails.Columns["Tier"].Visible = false;
            this.gridDetails.Columns["ID"].Visible = false;
            this.gridDetails.Columns["Crown"].Visible = false;
            this.gridDetails.Columns["Profile"].Visible = false;
            this.gridDetails.Columns["InParty"].Visible = false;
            this.gridDetails.Columns["PrivateLobby"].Visible = false;
            this.gridDetails.Columns["Qualified"].Visible = false;
            this.gridDetails.Columns["IsFinal"].Visible = false;
            this.gridDetails.Columns["IsTeam"].Visible = false;
            this.gridDetails.Columns["SessionId"].Visible = false;
            this.gridDetails.Columns["IsAbandon"].Visible = false;
            this.gridDetails.Columns["UseShareCode"].Visible = false;
            this.gridDetails.Columns["CreativeShareCode"].Visible = false;
            this.gridDetails.Columns["CreativeStatus"].Visible = false;
            this.gridDetails.Columns["CreativeAuthor"].Visible = false;
            this.gridDetails.Columns["CreativeOnlinePlatformId"].Visible = false;
            this.gridDetails.Columns["CreativeVersion"].Visible = false;
            this.gridDetails.Columns["CreativeTitle"].Visible = false;
            this.gridDetails.Columns["CreativeDescription"].Visible = false;
            this.gridDetails.Columns["CreativeMaxPlayer"].Visible = false;
            this.gridDetails.Columns["CreativePlatformId"].Visible = false;
            this.gridDetails.Columns["CreativeLastModifiedDate"].Visible = false;
            this.gridDetails.Columns["CreativePlayCount"].Visible = false;
            this.gridDetails.Columns["CreativeQualificationPercent"].Visible = false;
            this.gridDetails.Columns["CreativeTimeLimitSeconds"].Visible = false;
            this.gridDetails.Columns["OnlineServiceType"].Visible = false;
            this.gridDetails.Columns["OnlineServiceId"].Visible = false;
            this.gridDetails.Columns["OnlineServiceNickname"].Visible = false;
            if (this._showStats == 0) {
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
            }
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("level_detail_medal") });
            this.gridDetails.Setup("Medal", pos++, this.GetDataGridViewColumnWidth("Medal", $"{Multilingual.GetWord("level_detail_medal")}"), $"{Multilingual.GetWord("level_detail_medal")}", DataGridViewContentAlignment.MiddleCenter);
            if (this._showStats == 2) { // Shows
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "IsFinalIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "IsFinalIcon" });
                this.gridDetails.Setup("IsFinalIcon", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
                //this.gridDetails.Setup("IsFinal", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
            }
            this.gridDetails.Setup("ShowID", pos++, this.GetDataGridViewColumnWidth("ShowID", $"{Multilingual.GetWord("level_detail_show_id")}"), $"{Multilingual.GetWord("level_detail_show_id")}", DataGridViewContentAlignment.MiddleRight);
            this.gridDetails.Setup("ShowNameId", pos++, this.GetDataGridViewColumnWidth("ShowNameId", $"{Multilingual.GetWord("level_detail_show_name_id")}"), $"{Multilingual.GetWord("level_detail_show_name_id")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Columns["ShowNameId"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.gridDetails.Setup("Round", pos++, this.GetDataGridViewColumnWidth("Round", $"{Multilingual.GetWord("level_detail_round")}{(_showStats == 2 ? Multilingual.GetWord("level_detail_round_suffix") : "")}"), $"{Multilingual.GetWord("level_detail_round")}{(_showStats == 2 ? Multilingual.GetWord("level_detail_round_suffix") : "")}", DataGridViewContentAlignment.MiddleRight);
            if (this._showStats == 1) { // Rounds
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
                this.gridDetails.Columns["Name"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            } else if (this._showStats == 0) {
                this.gridDetails.Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
                this.gridDetails.Columns["Name"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            } else {
                this.gridDetails.Columns["Name"].Visible = false;
            }
            if (this._showStats == 2) { // Shows  
                this.gridDetails.Columns["Players"].Visible = false;
                this.gridDetails.Columns["PlayersPs4"].Visible = false;
                this.gridDetails.Columns["PlayersPs5"].Visible = false;
                this.gridDetails.Columns["PlayersXb1"].Visible = false;
                this.gridDetails.Columns["PlayersXsx"].Visible = false;
                this.gridDetails.Columns["PlayersSw"].Visible = false;
                this.gridDetails.Columns["PlayersPc"].Visible = false;
                this.gridDetails.Columns["PlayersBots"].Visible = false;
                this.gridDetails.Columns["PlayersEtc"].Visible = false;
            } else {
                this.gridDetails.Setup("Players", pos++,     this.GetDataGridViewColumnWidth("Players", $"{Multilingual.GetWord("level_detail_players")}"), $"{Multilingual.GetWord("level_detail_players")}", DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("PlayersPs4", pos++,  this.GetDataGridViewColumnWidth("PlayersPs4", $"{Multilingual.GetWord("level_detail_playersPs4")}"), $"{Multilingual.GetWord("level_detail_playersPs4")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersPs5", pos++,  this.GetDataGridViewColumnWidth("PlayersPs5", $"{Multilingual.GetWord("level_detail_playersPs5")}"), $"{Multilingual.GetWord("level_detail_playersPs5")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersXb1", pos++,  this.GetDataGridViewColumnWidth("PlayersXb1", $"{Multilingual.GetWord("level_detail_playersXb1")}"), $"{Multilingual.GetWord("level_detail_playersXb1")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersXsx", pos++,  this.GetDataGridViewColumnWidth("PlayersXsx", $"{Multilingual.GetWord("level_detail_playersXsx")}"), $"{Multilingual.GetWord("level_detail_playersXsx")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersSw", pos++,   this.GetDataGridViewColumnWidth("PlayersSw", $"{Multilingual.GetWord("level_detail_playersSw")}"), $"{Multilingual.GetWord("level_detail_playersSw")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersPc", pos++,   this.GetDataGridViewColumnWidth("PlayersPc", $"{Multilingual.GetWord("level_detail_playersPc")}"), $"{Multilingual.GetWord("level_detail_playersPc")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersBots", pos++, this.GetDataGridViewColumnWidth("PlayersBots", $"{Multilingual.GetWord("level_detail_playersBots")}"), $"{Multilingual.GetWord("level_detail_playersBots")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Columns["PlayersEtc"].Visible = false;
            }
            this.gridDetails.Setup("Start", pos++, this.GetDataGridViewColumnWidth("Start", $"{Multilingual.GetWord("level_detail_start")}"), $"{Multilingual.GetWord("level_detail_start")}", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Setup("End", pos++, this.GetDataGridViewColumnWidth("End", $"{Multilingual.GetWord("level_detail_end")}"), $"{Multilingual.GetWord("level_detail_end")}", DataGridViewContentAlignment.MiddleCenter);
            if (this._showStats == 2) { // Shows
                this.gridDetails.Columns["Finish"].Visible = false;
                this.gridDetails.Columns["Position"].Visible = false;
                this.gridDetails.Columns["Score"].Visible = false;
            } else {
                this.gridDetails.Setup("Finish", pos++, this.GetDataGridViewColumnWidth("Finish", $"{Multilingual.GetWord("level_detail_finish")}"), $"{Multilingual.GetWord("level_detail_finish")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("Position", pos++, this.GetDataGridViewColumnWidth("Position", $"{Multilingual.GetWord("level_detail_position")}"), $"{Multilingual.GetWord("level_detail_position")}", DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Score", pos++, this.GetDataGridViewColumnWidth("Score", $"{Multilingual.GetWord("level_detail_score")}"), $"{Multilingual.GetWord("level_detail_score")}", DataGridViewContentAlignment.MiddleRight);
            }
            this.gridDetails.Setup("Kudos", pos++, this.GetDataGridViewColumnWidth("Kudos", $"{Multilingual.GetWord("level_detail_kudos")}"), $"{Multilingual.GetWord("level_detail_kudos")}", DataGridViewContentAlignment.MiddleRight);

            bool colorSwitch = true;
            int lastShow = -1;
            // this.totalHeight = 0;
            for (int i = 0; i < this.gridDetails.RowCount; i++) {
                int showID = (int)this.gridDetails.Rows[i].Cells["ShowID"].Value;
                if (showID != lastShow) {
                    colorSwitch = !colorSwitch;
                    lastShow = showID;
                }

                if (colorSwitch) {
                    this.gridDetails.Rows[i].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
                    this.gridDetails.Rows[i].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
                }
                // this.totalHeight += this.gridDetails.Rows[i].Height;
            }
            this.gridDetails.ClearSelection();
        }
        
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }

            RoundInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
            if (info.PrivateLobby) { // Custom
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(8, 8, 8);
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
            
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "End") {
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Start") {
                e.Value = info.StartLocal.ToString(Multilingual.GetWord("level_grid_date_format"));
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Finish") {
                if (info.Finish.HasValue) {
                    e.Value = (info.Finish.Value - info.Start).ToString("m\\:ss\\.fff");
                }
            } else if (this._showStats == 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "Qualified") { // Shows
                e.Value = !string.IsNullOrEmpty(info.Name);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                            e.Value = Properties.Resources.medal_pink_grid_icon;
                            break;
                        case 1:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                            e.Value = Properties.Resources.medal_gold_grid_icon;
                            break;
                        case 2:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                            e.Value = Properties.Resources.medal_silver_grid_icon;
                            break;
                        case 3:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                            e.Value = Properties.Resources.medal_bronze_grid_icon;
                            break;
                    }
                } else {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_eliminated");
                    e.Value = Properties.Resources.medal_eliminated_grid_icon;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "IsFinalIcon") {
                if (info.IsFinal || info.Qualified) {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_success_reaching_finals");
                    e.Value = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                } else {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_failure_reaching_finals");
                    e.Value = Properties.Resources.uncheckmark_icon;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowID") {
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "RoundIcon") {
                //if ((this._showStats == 0 || this._showStats == 1) && this.StatsForm.StatLookup.TryGetValue((string)this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value, out LevelStats level)) {
                if ((this._showStats == 0 || this._showStats == 1) && this.StatsForm.StatLookup.TryGetValue(info.Name, out LevelStats level)) {
                    e.Value = level.RoundIcon;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Round") {
                //if (this._showStats == 1 && this.StatsForm.StatLookup.TryGetValue((string)this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value, out LevelStats level)) {
                if ((this._showStats == 0 || this._showStats == 1) && this.StatsForm.StatLookup.TryGetValue(info.Name, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam, this.Theme);
                    //e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : Color.FromArgb(c1.A, (int)(c1.R * 0.5), (int)(c1.G * 0.5), (int)(c1.B * 0.5));
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : info.PrivateLobby ? c1 : ControlPaint.LightLight(c1);
                }
                // else if (this._showStats == 2) { }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                if (this.StatsForm.StatLookup.TryGetValue((string)e.Value, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam, this.Theme);
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : info.PrivateLobby ? c1 : ControlPaint.LightLight(c1);
                    e.Value = $"{(level.IsCreative ? "🔧 " : "")}{level.Name}";
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = (info.UseShareCode && info.CreativeLastModifiedDate != DateTime.MinValue) ? $"☑️ {info.CreativeTitle}" : Multilingual.GetShowName((string)e.Value) ?? e.Value;
                    //if (info.UseShareCode) this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_share_code_copied_tooltip");
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Position") {
                if ((int)e.Value == 0) {
                    e.Value = "";
                }
            // } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "Players") {
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersPs4") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPs4_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersPs5") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPs5_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersXb1") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersXb1_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersXsx") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersXsx_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersSw") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersSw_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersPc") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPc_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersBots") {
                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersBots_desc");
                if ((int)e.Value == 0) {
                    e.Value = "-";
                }
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "Score") {
                e.Value = $"{e.Value:N0}";
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Kudos") {
                e.Value = $"{e.Value:N0}";
            }
        }
        
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.RoundDetails == null) return;
            string columnName = this.gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridDetails.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "ShowID"; }

            this.RoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
                int roundCompare = one.Round.CompareTo(two.Round);
                int showCompare = one.ShowID.CompareTo(two.ShowID);
                if (sortOrder == SortOrder.Descending) {
                    (one, two) = (two, one);
                }

                switch (columnName) {
                    case "ShowID":
                        showCompare = one.ShowID.CompareTo(two.ShowID);
                        return showCompare != 0 ? showCompare : roundCompare;
                    case "ShowNameId":
                        string showNameIdOne = Multilingual.GetShowName(one.ShowNameId) ?? @" ";
                        string showNameIdTwo = Multilingual.GetShowName(two.ShowNameId) ?? @" ";
                        int showNameIdCompare = showNameIdOne.CompareTo(showNameIdTwo);
                        return showNameIdCompare != 0 ? showNameIdCompare : roundCompare;
                    case "Round":
                        roundCompare = one.Round.CompareTo(two.Round);
                        return roundCompare == 0 ? showCompare : roundCompare;
                    case "RoundIcon":
                    case "Name":
                        if (this._showStats == 0) {
                            showCompare = one.ShowID.CompareTo(two.ShowID);
                            return showCompare != 0 ? showCompare : roundCompare;
                        } else {
                            string nameOne = this.StatsForm.StatLookup.TryGetValue(one.Name, out LevelStats level1) ? level1.Name : one.Name;
                            string nameTwo = this.StatsForm.StatLookup.TryGetValue(two.Name, out LevelStats level2) ? level2.Name : two.Name;
                            int nameCompare = nameOne.CompareTo(nameTwo);
                            return nameCompare != 0 ? nameCompare : roundCompare;
                        }
                    case "Players":
                        int playerCompare = one.Players.CompareTo(two.Players);
                        return playerCompare != 0 ? playerCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersPs4":
                        int playerPs4Compare = one.PlayersPs4.CompareTo(two.PlayersPs4);
                        return playerPs4Compare != 0 ? playerPs4Compare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersPs5":
                        int playerPs5Compare = one.PlayersPs5.CompareTo(two.PlayersPs5);
                        return playerPs5Compare != 0 ? playerPs5Compare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersXb1":
                        int playerXb1Compare = one.PlayersXb1.CompareTo(two.PlayersXb1);
                        return playerXb1Compare != 0 ? playerXb1Compare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersXsx":
                        int playerXsxCompare = one.PlayersXsx.CompareTo(two.PlayersXsx);
                        return playerXsxCompare != 0 ? playerXsxCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersSw":
                        int playerSwCompare = one.PlayersSw.CompareTo(two.PlayersSw);
                        return playerSwCompare != 0 ? playerSwCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersPc":
                        int playerPcCompare = one.PlayersPc.CompareTo(two.PlayersPc);
                        return playerPcCompare != 0 ? playerPcCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "PlayersBots":
                        int playersBotsCompare = one.PlayersBots.CompareTo(two.PlayersBots);
                        return playersBotsCompare != 0 ? playersBotsCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Start": return one.Start.CompareTo(two.Start);
                    case "End": return (one.End - one.Start).CompareTo(two.End - two.Start);
                    case "Finish": return one.Finish.HasValue && two.Finish.HasValue ? (one.Finish.Value - one.Start).CompareTo(two.Finish.Value - two.Start) : one.Finish.HasValue ? -1 : 1;
                    case "Qualified":
                        int qualifiedCompare = this._showStats == 2 ? string.IsNullOrEmpty(one.Name).CompareTo(string.IsNullOrEmpty(two.Name)) : one.Qualified.CompareTo(two.Qualified);
                        return qualifiedCompare != 0 ? qualifiedCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Position":
                        int positionCompare = one.Position.CompareTo(two.Position);
                        return positionCompare != 0 ? positionCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Score":
                        int scoreCompare = one.Score.GetValueOrDefault(-1).CompareTo(two.Score.GetValueOrDefault(-1));
                        return scoreCompare != 0 ? scoreCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Medal":
                        int tierOne = one.Qualified ? one.Tier == 0 ? 4 : one.Tier : 5;
                        int tierTwo = two.Qualified ? two.Tier == 0 ? 4 : two.Tier : 5;
                        int tierCompare = tierOne.CompareTo(tierTwo);
                        return tierCompare != 0 ? tierCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "IsFinalIcon":
                        int finalsOne = one.IsFinal || one.Qualified ? 1 : 0;
                        int finalsTwo = two.IsFinal || two.Qualified ? 1 : 0;
                        return finalsOne.CompareTo(finalsTwo);
                    default:
                        int kudosCompare = one.Kudos.CompareTo(two.Kudos);
                        return kudosCompare != 0 ? kudosCompare : showCompare == 0 ? roundCompare : showCompare;
                }
            });

            this.gridDetails.DataSource = null;
            this.gridDetails.DataSource = this.RoundDetails;
            this.gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (this._showStats != 2 && this.gridDetails.SelectedCells.Count > 0) {
                if (((DataGridView)sender).SelectedRows.Count == 1) {
                    RoundInfo info = this.gridDetails.Rows[((DataGridView)sender).SelectedRows[0].Index].DataBoundItem as RoundInfo;
                    if ((info.UseShareCode && info.CreativeLastModifiedDate == DateTime.MinValue) || (info.UseShareCode && info.CreativeQualificationPercent == 0 && info.CreativeTimeLimitSeconds == 0)) {
                        if (this.gridDetails.MenuSeparator != null && !this.gridDetails.CMenu.Items.Contains(this.gridDetails.MenuSeparator)) {
                            this.gridDetails.CMenu.Items.Add(this.gridDetails.MenuSeparator);
                        }
                        if (this.gridDetails.UpdateCreativeShows != null && !this.gridDetails.CMenu.Items.Contains(this.gridDetails.UpdateCreativeShows)) {
                            this.gridDetails.CMenu.Items.Add(this.gridDetails.UpdateCreativeShows);
                        }
                    } else {
                        this.gridDetails.ClearSelection();
                        if (this.gridDetails.MenuSeparator != null && this.gridDetails.CMenu.Items.Contains(this.gridDetails.MenuSeparator)) {
                            this.gridDetails.CMenu.Items.Remove(this.gridDetails.MenuSeparator);
                        }
                        if (this.gridDetails.UpdateCreativeShows != null && this.gridDetails.CMenu.Items.Contains(this.gridDetails.UpdateCreativeShows)) {
                            this.gridDetails.CMenu.Items.Remove(this.gridDetails.UpdateCreativeShows);
                        }
                    }
                } else {
                    this.gridDetails.ClearSelection();
                }
            }
        }
        
        private void LevelDetails_KeyDown(object sender, KeyEventArgs e) {
            try {
                if (e.KeyCode == Keys.Delete) {
                    this.DeleteShow();
                } else if (e.KeyCode == Keys.Escape) {
                    this.Close();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.Message, $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void deleteShows_Click(object sender, EventArgs e) {
            this.DeleteShow();
        }
        
        private void DeleteShow() {
            int selectedCount = this.gridDetails.SelectedRows.Count;
            if (selectedCount > 0) {
                if (MetroMessageBox.Show(this, 
                        $@"{Multilingual.GetWord("message_delete_show_prefix")} ({selectedCount:N0}) {Multilingual.GetWord("message_delete_show_suffix")}", 
                        Multilingual.GetWord("message_delete_show_caption"), 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                    lock (this.StatsForm.StatsDB) {
                        this.StatsForm.StatsDB.BeginTrans();
                        foreach (DataGridViewRow row in this.gridDetails.SelectedRows) {
                            RoundInfo d = row.DataBoundItem as RoundInfo;
                            this.RoundDetails.Remove(d);
                            this.StatsForm.AllStats.RemoveAll(r => r.ShowID == d.ShowID);
                            this.StatsForm.RoundDetails.DeleteMany(r => r.ShowID == d.ShowID);
                        }
                        this.StatsForm.StatsDB.Commit();
                    }
                    
                    this.gridDetails.DataSource = null;
                    this.gridDetails.DataSource = this.RoundDetails;
                    if (minIndex < this.RoundDetails.Count) {
                        this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                    } else if (this.RoundDetails.Count > 0) {
                        this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                    }

                    this.StatsForm.ResetStats();
                    Stats.IsOverlayRoundInfoNeedRefresh = true;
                }
            }
        }
        
        private void moveShows_Click(object sender, EventArgs e) {
            int selectedCount = this.gridDetails.SelectedRows.Count;
            if (selectedCount > 0) {
                using (EditShows moveShows = new EditShows()) {
                    moveShows.StatsForm = this.StatsForm; 
                    moveShows.Profiles = this.StatsForm.AllProfiles; 
                    moveShows.FunctionFlag = "move"; 
                    moveShows.SelectedCount = selectedCount; 
                    moveShows.Icon = Icon;
                    if (moveShows.ShowDialog(this) == DialogResult.OK) {
                        int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                        int fromProfileId = this.StatsForm.GetCurrentProfileId();
                        int toProfileId = moveShows.SelectedProfileId;
                        
                        lock (this.StatsForm.StatsDB) {
                            this.StatsForm.StatsDB.BeginTrans();
                            foreach (DataGridViewRow row in this.gridDetails.SelectedRows) {
                                RoundInfo d = row.DataBoundItem as RoundInfo;
                                this.RoundDetails.Remove(d);
                                List<RoundInfo> rl = this.StatsForm.AllStats.FindAll(r => r.ShowID == d.ShowID && r.Profile == fromProfileId);
                                foreach (RoundInfo r in rl) {
                                    r.Profile = toProfileId;
                                }
                                this.StatsForm.RoundDetails.Update(rl);
                            }
                            this.StatsForm.StatsDB.Commit();
                        }
                        
                        this.gridDetails.DataSource = null;
                        this.gridDetails.DataSource = this.RoundDetails;
                        if (minIndex < this.RoundDetails.Count) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                        } else if (this.RoundDetails.Count > 0) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                        }

                        this.StatsForm.ResetStats();
                        Stats.IsOverlayRoundInfoNeedRefresh = true;
                    }
                }
            }
        }
        
        private void updateShows_Click(object sender, EventArgs e) {
            if (Utils.IsInternetConnected()) {
                if (this._showStats != 2 && this.gridDetails.SelectedCells.Count > 0 && this.gridDetails.SelectedRows.Count == 1) {
                    RoundInfo ri = this.gridDetails.Rows[this.gridDetails.SelectedCells[0].RowIndex].DataBoundItem as RoundInfo;
                    if ((ri.UseShareCode && ri.CreativeLastModifiedDate == DateTime.MinValue) || (ri.UseShareCode && ri.CreativeQualificationPercent == 0 && ri.CreativeTimeLimitSeconds == 0)) {
                        if (MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_prefix")}{ri.ShowNameId}{Multilingual.GetWord("message_update_creative_show_suffix")}", Multilingual.GetWord("message_update_creative_show_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            bool isSucceed = false;
                            int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                            try {
                                JsonElement resData = Utils.GetApiData(Utils.FALLGUYSDB_API_URL, $"creative/{ri.ShowNameId}.json");
                                if (resData.TryGetProperty("data", out JsonElement je)) {
                                    JsonElement snapshot = je.GetProperty("snapshot");
                                    JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                                    List<RoundInfo> filteredInfo = this.RoundDetails.FindAll(r => ri.ShowNameId.Equals(r.ShowNameId) &&
                                        (r.CreativeLastModifiedDate == DateTime.MinValue || (r.CreativeQualificationPercent == 0 && r.CreativeTimeLimitSeconds == 0)));
                                    lock (this.StatsForm.StatsDB) {
                                        this.StatsForm.StatsDB.BeginTrans();
                                        foreach (RoundInfo info in filteredInfo) {
                                            string[] onlinePlatformInfo = this.StatsForm.FindCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                                            info.CreativeShareCode = snapshot.GetProperty("share_code").GetString();
                                            info.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                            info.CreativeAuthor = onlinePlatformInfo[1];
                                            info.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                            info.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                            info.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                            info.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                            info.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                            info.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                            info.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                            info.CreativePlayCount = snapshot.GetProperty("play_count").GetInt32();
                                            info.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                            info.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                            this.StatsForm.RoundDetails.Update(info);
                                        }
                                        this.StatsForm.StatsDB.Commit();
                                    }
                                    isSucceed = true;
                                }
                            } catch {
                                isSucceed = false;
                            }
                            
                            if (!isSucceed) {
                                RoundInfo urInfo = this.StatsForm.GetRoundInfoFromShareCode(ri.ShowNameId);
                                if (urInfo != null && !string.IsNullOrEmpty(urInfo.CreativeTitle)) {
                                    List<RoundInfo> filteredInfo = this.RoundDetails.FindAll(r => urInfo.ShowNameId.Equals(r.ShowNameId) &&
                                        (r.CreativeLastModifiedDate == DateTime.MinValue || (r.CreativeQualificationPercent == 0 && r.CreativeTimeLimitSeconds == 0)));
                                    lock (this.StatsForm.StatsDB) {
                                        this.StatsForm.StatsDB.BeginTrans();
                                        foreach (RoundInfo info in filteredInfo) {
                                            info.CreativeShareCode = urInfo.CreativeShareCode;
                                            info.CreativeOnlinePlatformId = urInfo.CreativeOnlinePlatformId;
                                            info.CreativeAuthor = urInfo.CreativeAuthor;
                                            info.CreativeVersion = urInfo.CreativeVersion;
                                            info.CreativeStatus = urInfo.CreativeStatus;
                                            info.CreativeTitle = urInfo.CreativeTitle;
                                            info.CreativeDescription = urInfo.CreativeDescription;
                                            info.CreativeMaxPlayer = urInfo.CreativeMaxPlayer;
                                            info.CreativePlatformId = urInfo.CreativePlatformId;
                                            info.CreativeLastModifiedDate = urInfo.CreativeLastModifiedDate;
                                            info.CreativePlayCount = urInfo.CreativePlayCount;
                                            info.CreativeQualificationPercent = urInfo.CreativeQualificationPercent;
                                            info.CreativeTimeLimitSeconds = urInfo.CreativeTimeLimitSeconds;
                                            this.StatsForm.RoundDetails.Update(info);
                                        }
                                        this.StatsForm.StatsDB.Commit();
                                    }
                                } else {
                                    MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_error")}", $"{Multilingual.GetWord("message_update_error_caption")}",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            
                            this.gridDetails.DataSource = null;
                            this.gridDetails.DataSource = this.RoundDetails;
                            if (minIndex < this.RoundDetails.Count) {
                                this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                            } else if (this.RoundDetails.Count > 0) {
                                this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                            }
                        }
                    }
                }
            } else {
                MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_check_internet_connection")}", $"{Multilingual.GetWord("message_check_internet_connection_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridDetails_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId" && (bool)this.gridDetails.Rows[e.RowIndex].Cells["UseShareCode"].Value) {
                string shareCode = (string)this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                Clipboard.SetText(shareCode, TextDataFormat.Text);
                this.StatsForm.AllocTooltip();
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 4, cursorPosition.Y - 20);
                this.StatsForm.ShowTooltip(Multilingual.GetWord("level_detail_share_code_copied"), this, position, 2000);
            }
        }

        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }
            this.gridDetails.Cursor = Cursors.Hand;
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId" && (bool)this.gridDetails.Rows[e.RowIndex].Cells["UseShareCode"].Value) {
                this.gridDetails.ShowCellToolTips = false;
                RoundInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.CreativeLastModifiedDate == DateTime.MinValue) return;
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(info.CreativeTitle);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(info.CreativeDescription);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                if (string.IsNullOrEmpty(info.CreativeAuthor) || string.IsNullOrEmpty(info.CreativeOnlinePlatformId)) {
                    strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_author")} : N/A");
                } else {
                    string[] createAuthorArr = info.CreativeAuthor.Split(';');
                    string[] creativeOnlinePlatformIdArr = info.CreativeOnlinePlatformId.Split(';');
                    for (int i = 0; i < creativeOnlinePlatformIdArr.Length; i++) {
                        strBuilder.Append(i == 0 ? $"{Multilingual.GetWord("level_detail_creative_author")} : {createAuthorArr[i]} ({this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])})"
                            : $"{Environment.NewLine}\t{createAuthorArr[i]} ({this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])})");
                    }
                }
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_share_code")} : {info.CreativeShareCode}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_version")} : v{info.CreativeVersion}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_max_players")} : {info.CreativeMaxPlayer}{Multilingual.GetWord("level_detail_creative_player_suffix")}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_time_limit")} : {TimeSpan.FromSeconds(info.CreativeTimeLimitSeconds > 0 ? info.CreativeTimeLimitSeconds : 240):m\\:ss}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_platform")} : {this.GetCreativePlatformName(info.CreativePlatformId)}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_last_modified")} : {info.CreativeLastModifiedDate.ToString(Multilingual.GetWord("level_date_format"))}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{Multilingual.GetWord("level_detail_creative_play_count")} : {info.CreativePlayCount:N0}{Multilingual.GetWord("level_detail_creative_inning")}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"# {Multilingual.GetWord("level_detail_share_code_copied_tooltip")}");

                this.StatsForm.AllocCustomTooltip(this.StatsForm.cmtt_levelDetails_Draw);
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 120, cursorPosition.Y);
                this.StatsForm.ShowCustomTooltip(strBuilder.ToString(), this, position);
            } else {
                this.gridDetails.ShowCellToolTips = true;
            }
        }

        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            this.StatsForm.HideCustomTooltip(this);
            this.gridDetails.Cursor = Cursors.Default;
        }

        private string GetCreativeOnlinePlatformName(string platform) {
            switch (platform) {
                case "eos": return Multilingual.GetWord("level_detail_online_platform_eos");
                case "steam": return Multilingual.GetWord("level_detail_online_platform_steam");
                case "psn": return Multilingual.GetWord("level_detail_online_platform_psn");
                case "xbl": return Multilingual.GetWord("level_detail_online_platform_xbl");
                case "nso": return Multilingual.GetWord("level_detail_online_platform_nso");
            }
            return platform;
        }

        private string GetCreativePlatformName(string platform) {
            switch (platform) {
                case "ps4": return Multilingual.GetWord("level_detail_playersPs4");
                case "ps5": return Multilingual.GetWord("level_detail_playersPs5");
                case "xb1": return Multilingual.GetWord("level_detail_playersXb1");
                case "xsx": return Multilingual.GetWord("level_detail_playersXsx");
                case "switch": return Multilingual.GetWord("level_detail_playersSw");
                case "win": return Multilingual.GetWord("level_detail_playersPc");
            }
            return platform;
        }
    }
}