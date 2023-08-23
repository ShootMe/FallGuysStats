using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
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
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public LevelDetails() {
            this.InitializeComponent();
        }
        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkMagenta;
            this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.SpringGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
        }
        private int GetClientWidth(string level) {
            int lang = Stats.CurrentLanguage;
            switch (level) {
                case "Shows":
                    return this.Width - (lang == 0 ? -80 : lang == 1 ? -113 : lang == 2 ? -39 : lang == -64 ? -90 : -21);
                case "Rounds":
                    return this.Width + (lang == 0 ? 925 : lang == 1 ? 1058 : lang == 2 ? 820 : lang == 3 ? 885 : 846);
                case "Finals":
                    return this.Width + (lang == 0 ? 925 : lang == 1 ? 1058 : lang == 2 ? 820 : lang == 3 ? 885 : 846);
                default:
                    return this.Width + (lang == 0 ? 925 : lang == 1 ? 1058 : lang == 2 ? 820 : lang == 3 ? 885 : 846);
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
                    sizeOfText = 25;
                    break;
                case "PlayersPs5":
                    sizeOfText = 25;
                    break;
                case "PlayersXb1":
                    sizeOfText = 51;
                    break;
                case "PlayersXsx":
                    sizeOfText = 70;
                    break;
                case "PlayersSw":
                    sizeOfText = 41;
                    break;
                case "PlayersPc":
                    sizeOfText = 19;
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
                    return 60;
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
        private void LevelDetails_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);
            //
            // dataGridViewCellStyle1
            //
            this.dataGridViewCellStyle1.Font = Overlay.GetMainFont(10);
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            //
            // dataGridViewCellStyle2
            //
            this.dataGridViewCellStyle2.Font = Overlay.GetMainFont(12);
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            
            this.gridDetails.CurrentCell = null;
            this.gridDetails.ClearSelection();
            this.ClientSize = new Size(this.GetClientWidth(this.LevelName), this.Height + 226);
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(20, 20, 0, 0);
            if (this.LevelName == "Shows") {
                this.gridDetails.Name = "gridShowsStats";
                this.gridDetails.MultiSelect = true;
                this.BackImage = Properties.Resources.fallguys_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_show_stats")} - {StatsForm.GetCurrentProfileName()} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 2;
            } else if (this.LevelName == "Rounds") {
                this.gridDetails.Name = "gridRoundsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_round_stats")} - {StatsForm.GetCurrentProfileName()} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 1;
            } else if (this.LevelName == "Finals") {
                this.gridDetails.Name = "gridFinalsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_final_stats")} - {StatsForm.GetCurrentProfileName()} ({StatsForm.GetCurrentFilterName()})";
                this._showStats = 1;
            } else {
                this.gridDetails.Name = "gridRoundStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.RoundIcon;
                this._showStats = 0;
                this.Text = $@"     {Multilingual.GetWord("level_detail_level_stats")} - {(this.IsCreative ? "🛠️ " : "")}{this.LevelName} ({StatsForm.GetCurrentFilterName()})";
            }

            this.gridDetails.DataSource = RoundDetails;
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

            foreach (object item in this.gridDetails.CMenu.Items) {
                if (item is ToolStripMenuItem tsi) {
                    tsi.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    tsi.MouseEnter += this.CMenu_MouseEnter;
                    tsi.MouseLeave += this.CMenu_MouseLeave;
                    if (tsi.Name.Equals("exportItemCSV")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemHTML")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemBBCODE")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("exportItemMD")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                    } else if (tsi.Name.Equals("moveShows")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.move : Properties.Resources.move_gray;
                    } else if (tsi.Name.Equals("deleteShows")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.delete : Properties.Resources.delete_gray;
                    } else if (tsi.Name.Equals("updateCreativeShows")) {
                        tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.update : Properties.Resources.update_gray;
                    }
                } else if (item is ToolStripSeparator tss) {
                    tss.Paint += this.CustomToolStripSeparator_Paint;
                    tss.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tss.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                }
            }
        }
        private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            if (this.gridDetails.Rows.Count > 0) this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.Rows.Count - 1;
        }
        private void CustomToolStripSeparator_Paint(Object sender, PaintEventArgs e) {
            ToolStripSeparator separator = (ToolStripSeparator)sender;
            e.Graphics.FillRectangle(new SolidBrush(this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)), 0, 0, separator.Width, separator.Height); // CUSTOM_COLOR_BACKGROUND
            e.Graphics.DrawLine(new Pen(this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray), 30, separator.Height / 2, separator.Width - 4, separator.Height / 2); // CUSTOM_COLOR_FOREGROUND
        }
        private void CMenu_MouseEnter(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = Color.Black;
                if (tsi.Name.Equals("exportItemCSV")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemHTML")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemBBCODE")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("exportItemMD")) {
                    tsi.Image = Properties.Resources.export;
                } else if (tsi.Name.Equals("moveShows")) {
                    tsi.Image = Properties.Resources.move;
                } else if (tsi.Name.Equals("deleteShows")) {
                    tsi.Image = Properties.Resources.delete;
                }
            }
        }
        private void CMenu_MouseLeave(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem tsi) {
                tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                if (tsi.Name.Equals("exportItemCSV")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemHTML")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemBBCODE")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("exportItemMD")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.export : Properties.Resources.export_gray;
                } else if (tsi.Name.Equals("moveShows")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.move : Properties.Resources.move_gray;
                } else if (tsi.Name.Equals("deleteShows")) {
                    tsi.Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.delete : Properties.Resources.delete_gray;
                }
            }
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            if (this.gridDetails.Columns.Count == 0) { return; }

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
            if (this._showStats == 0) {
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
            }
            this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "Medal" });
            this.gridDetails.Setup("Medal", pos++, this.GetDataGridViewColumnWidth("Medal", $"{Multilingual.GetWord("level_detail_medal")}"), $"{Multilingual.GetWord("level_detail_medal")}", DataGridViewContentAlignment.MiddleCenter);
            if (this._showStats == 2) { // Shows
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "IsFinalIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "IsFinalIcon" });
                this.gridDetails.Setup("IsFinalIcon", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
                //this.gridDetails.Setup("IsFinal", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
            }
            this.gridDetails.Setup("ShowID", pos++, this.GetDataGridViewColumnWidth("ShowID", $"{Multilingual.GetWord("level_detail_show_id")}"), $"{Multilingual.GetWord("level_detail_show_id")}", DataGridViewContentAlignment.MiddleRight);
            this.gridDetails.Setup("ShowNameId", pos++, this.GetDataGridViewColumnWidth("ShowNameId", $"{Multilingual.GetWord("level_detail_show_name_id")}"), $"{Multilingual.GetWord("level_detail_show_name_id")}", DataGridViewContentAlignment.MiddleLeft);
            this.gridDetails.Setup("Round", pos++, this.GetDataGridViewColumnWidth("Round", $"{Multilingual.GetWord("level_detail_round")}{(_showStats == 2 ? Multilingual.GetWord("level_detail_round_suffix") : "")}"), $"{Multilingual.GetWord("level_detail_round")}{(_showStats == 2 ? Multilingual.GetWord("level_detail_round_suffix") : "")}", DataGridViewContentAlignment.MiddleRight);
            if (this._showStats == 1) { // Rounds
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
            } else if (this._showStats == 0) {
                this.gridDetails.Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
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
            for (int i = 0; i < this.gridDetails.RowCount; i++) {
                int showID = (int)this.gridDetails.Rows[i].Cells["ShowID"].Value;
                if (showID != lastShow) {
                    colorSwitch = !colorSwitch;
                    lastShow = showID;
                }

                if (colorSwitch) {
                    //this.gridDetails.Rows[i].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 58, 66);
                    this.gridDetails.Rows[i].DefaultCellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
                    this.gridDetails.Rows[i].DefaultCellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
                }
            }
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }

            RoundInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
            if (info.PrivateLobby) { // Custom
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(8, 8, 8);
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
            //if ((bool)this.gridDetails.Rows[e.RowIndex].Cells["PrivateLobby"].Value) {
            //    e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(8, 8, 8);
            //    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            //}
            
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "End") {
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Start") {
                //e.Value = info.StartLocal.ToString("yyyy-MM-dd HH:mm");
                e.Value = info.StartLocal.ToString(Multilingual.GetWord("level_grid_date_format"));
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Finish") {
                if (info.Finish.HasValue) {
                    e.Value = (info.Finish.Value - info.Start).ToString("m\\:ss\\.ff");
                }
            } else if (this._showStats == 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "Qualified") { // Shows
                e.Value = !string.IsNullOrEmpty(info.Name);
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                            e.Value = Properties.Resources.medal_pink;
                            break;
                        case 1:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                            e.Value = Properties.Resources.medal_gold;
                            break;
                        case 2:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                            e.Value = Properties.Resources.medal_silver;
                            break;
                        case 3:
                            this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                            e.Value = Properties.Resources.medal_bronze;
                            break;
                    }
                } else {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_eliminated");
                    e.Value = Properties.Resources.medal_eliminated;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "IsFinalIcon") {
                if (info.IsFinal) {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_success_reaching_finals");
                    e.Value = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                } else {
                    this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_failure_reaching_finals");
                    e.Value = Properties.Resources.uncheckmark_icon;
                }
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
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                if (this.StatsForm.StatLookup.TryGetValue((string)e.Value, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam, this.Theme);
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : info.PrivateLobby ? c1 : ControlPaint.LightLight(c1);
                    e.Value = $"{(level.IsCreative ? "🔧 " : "")}{level.Name}";
                    //gridDetails.Columns[e.ColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = (info.UseShareCode && info.CreativeLastModifiedDate != DateTime.MinValue) ? info.CreativeTitle : Multilingual.GetShowName((string)e.Value) ?? e.Value;
                    //if (info.UseShareCode) this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_share_code_copied_tooltip");
                }
                //gridDetails.Columns[e.ColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Position") {
                if ((int)e.Value == 0) {
                    e.Value = "";
                }
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
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
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
                        int showNameCompare = (string.IsNullOrEmpty(one.ShowNameId) ? @" " : Multilingual.GetShowName(one.ShowNameId)).CompareTo(string.IsNullOrEmpty(two.ShowNameId) ? @" " : Multilingual.GetShowName(two.ShowNameId));
                        return showNameCompare != 0 ? showNameCompare : roundCompare;
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
                        int finalsOne = one.IsFinal ? 1 : 0;
                        int finalsTwo = two.IsFinal ? 1 : 0;
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
                int selectedCount = this.gridDetails.SelectedCells.Count;
                if (e.KeyCode == Keys.Delete && selectedCount > 0) {
                    HashSet<RoundInfo> rows = new HashSet<RoundInfo>();
                    int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                    for (int i = 0; i < selectedCount; i++) {
                        DataGridViewCell cell = this.gridDetails.SelectedCells[i];
                        rows.Add((RoundInfo)this.gridDetails.Rows[cell.RowIndex].DataBoundItem);
                    }

                    if (MetroMessageBox.Show(this, 
                            $@"{Multilingual.GetWord("message_delete_show_prefix")}({rows.Count}){Multilingual.GetWord("message_delete_show_suffix")}", 
                            Multilingual.GetWord("message_delete_show_caption"), 
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        this.gridDetails.DataSource = null;

                        lock (this.StatsForm.StatsDB) {
                            this.StatsForm.StatsDB.BeginTrans();
                            foreach (RoundInfo info in rows) {
                                this.RoundDetails.Remove(info);
                                this.StatsForm.RoundDetails.DeleteMany(x => x.ShowID == info.ShowID);
                            }
                            this.StatsForm.StatsDB.Commit();
                        }

                        this.gridDetails.DataSource = this.RoundDetails;
                        if (minIndex < this.RoundDetails.Count) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                        } else if (this.RoundDetails.Count > 0) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                        }

                        this.StatsForm.ResetStats();
                    }
                } else if (e.KeyCode == Keys.Escape) {
                    Close();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.Message, $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void moveShows_Click(object sender, EventArgs e) {
            int selectedCount = this.gridDetails.SelectedCells.Count;
            if (selectedCount > 0) {
                HashSet<RoundInfo> rows = new HashSet<RoundInfo>();
                int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                for (int i = 0; i < selectedCount; i++) {
                    DataGridViewCell cell = this.gridDetails.SelectedCells[i];
                    rows.Add((RoundInfo)this.gridDetails.Rows[cell.RowIndex].DataBoundItem);
                }
                using (EditShows moveShows = new EditShows()) {
                    moveShows.StatsForm = this.StatsForm; 
                    moveShows.Profiles = this.StatsForm.AllProfiles; 
                    moveShows.FunctionFlag = "move"; 
                    moveShows.SelectedCount = rows.Count; 
                    moveShows.Icon = Icon;
                    if (moveShows.ShowDialog(this) == DialogResult.OK) {
                        lock (this.StatsForm.StatsDB) {
                            this.StatsForm.StatsDB.BeginTrans();
                            
                            this.gridDetails.DataSource = null;
                            int fromProfileId = this.StatsForm.GetCurrentProfileId();
                            int toProfileId = moveShows.SelectedProfileId;
                            List<RoundInfo> target;
                            foreach (RoundInfo info in rows) {
                                this.RoundDetails.Remove(info);
                                target = this.StatsForm.RoundDetails.Find(r => r.ShowID == info.ShowID && r.Profile == fromProfileId).ToList();
                                for (int i = 0; i < target.Count; i++) {
                                    target[i].Profile = toProfileId;
                                }
                                this.StatsForm.RoundDetails.DeleteMany(r => r.ShowID == info.ShowID && r.Profile == fromProfileId);
                                this.StatsForm.RoundDetails.InsertBulk(target);
                            }
                            
                            this.StatsForm.StatsDB.Commit();
                        }
                        
                        this.gridDetails.DataSource = this.RoundDetails;
                        if (minIndex < this.RoundDetails.Count) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                        } else if (this.RoundDetails.Count > 0) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                        }

                        this.StatsForm.ResetStats();
                    }
                }
            }
        }
        private void updateShows_Click(object sender, EventArgs e) {
            if (this.StatsForm.IsInternetConnected()) {
                if (this._showStats != 2 && this.gridDetails.SelectedCells.Count > 0 && this.gridDetails.SelectedRows.Count == 1) {
                    RoundInfo ri = this.gridDetails.Rows[this.gridDetails.SelectedCells[0].RowIndex].DataBoundItem as RoundInfo;
                    if ((ri.UseShareCode && ri.CreativeLastModifiedDate == DateTime.MinValue) || (ri.UseShareCode && ri.CreativeQualificationPercent == 0 && ri.CreativeTimeLimitSeconds == 0)) {
                        if (MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_prefix")}{ri.ShowNameId}{Multilingual.GetWord("message_update_creative_show_suffix")}", Multilingual.GetWord("message_update_creative_show_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            try {
                                JsonElement resData = this.StatsForm.GetApiData(this.StatsForm.FALLGUYSDB_API_URL, $"creative/{ri.ShowNameId}.json").GetProperty("data").GetProperty("snapshot");
                                JsonElement versionMetadata = resData.GetProperty("version_metadata");
                                List<RoundInfo> rows = this.RoundDetails.FindAll(r => ri.ShowNameId.Equals(r.ShowNameId) &&
                                                                                    (r.CreativeLastModifiedDate == DateTime.MinValue || (r.CreativeQualificationPercent == 0 && r.CreativeTimeLimitSeconds == 0)));
                                int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                                this.gridDetails.DataSource = null;
                                lock (this.StatsForm.StatsDB) {
                                    this.StatsForm.StatsDB.BeginTrans();
                                    for (int i = rows.Count - 1; i >= 0; i--) {
                                        RoundInfo temp = rows[i];
                                        string[] onlinePlatformInfo = this.StatsForm.FindCreativeAuthor(resData.GetProperty("author").GetProperty("name_per_platform"));
                                        temp.CreativeShareCode = resData.GetProperty("share_code").GetString();
                                        temp.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                        temp.CreativeAuthor = onlinePlatformInfo[1];
                                        temp.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                        temp.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                        temp.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                        temp.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                        temp.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                        temp.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                        temp.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                        temp.CreativePlayCount = resData.GetProperty("play_count").GetInt32();
                                        temp.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                        //temp.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").GetProperty("time_limit_seconds").GetInt32();
                                        temp.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                        this.StatsForm.RoundDetails.Update(temp);
                                    }
                                    this.StatsForm.StatsDB.Commit();
                                }

                                this.gridDetails.DataSource = this.RoundDetails;
                                if (minIndex < this.RoundDetails.Count) {
                                    this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                                } else if (this.RoundDetails.Count > 0) {
                                    this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                                }
                            } catch (WebException wex) {
                                if (wex.Status == WebExceptionStatus.ProtocolError) {
                                    int statusCode = (int)((HttpWebResponse)wex.Response).StatusCode;
                                    switch (statusCode) {
                                        case 500:
                                            MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_error")}", $"{Multilingual.GetWord("message_update_error_caption")}",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            break;
                                        default:
                                            MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_error")}", $"{Multilingual.GetWord("message_update_error_caption")}",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            break;
                                    }
                                }
                            } catch (Exception ex) {
                                MetroMessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            } else {
                MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_check_internet_connection")}", $"{Multilingual.GetWord("message_check_internet_connection_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void deleteShows_Click(object sender, EventArgs e) {
            int selectedCount = this.gridDetails.SelectedCells.Count;
            if (selectedCount > 0) {
                HashSet<RoundInfo> rows = new HashSet<RoundInfo>();
                int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                for (int i = 0; i < selectedCount; i++) {
                    DataGridViewCell cell = this.gridDetails.SelectedCells[i];
                    rows.Add((RoundInfo)this.gridDetails.Rows[cell.RowIndex].DataBoundItem);
                }

                if (MetroMessageBox.Show(this, 
                        $@"{Multilingual.GetWord("message_delete_show_prefix")} ({rows.Count}) {Multilingual.GetWord("message_delete_show_suffix")}", 
                        Multilingual.GetWord("message_delete_show_caption"), 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.gridDetails.DataSource = null;

                    lock (this.StatsForm.StatsDB) {
                        this.StatsForm.StatsDB.BeginTrans();
                        foreach (RoundInfo info in rows) {
                            this.RoundDetails.Remove(info);
                            this.StatsForm.RoundDetails.DeleteMany(x => x.ShowID == info.ShowID);
                        }
                        this.StatsForm.StatsDB.Commit();
                    }

                    this.gridDetails.DataSource = this.RoundDetails;
                    if (minIndex < this.RoundDetails.Count) {
                        this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                    } else if (this.RoundDetails.Count > 0) {
                        this.gridDetails.FirstDisplayedScrollingRowIndex = this.RoundDetails.Count - 1;
                    }

                    this.StatsForm.ResetStats();
                }
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
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId" && (bool)this.gridDetails.Rows[e.RowIndex].Cells["UseShareCode"].Value) {
                RoundInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.CreativeLastModifiedDate == DateTime.MinValue) return;
                StringBuilder strbuilder = new StringBuilder();
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(info.CreativeTitle);
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(info.CreativeDescription);
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(Environment.NewLine);
                string[] createAuthorArr = info.CreativeAuthor.Split(';');
                string[] creativeOnlinePlatformIdArr = info.CreativeOnlinePlatformId.Split(';');
                string indent = Stats.CurrentLanguage == 0 ? "            " :
                                Stats.CurrentLanguage == 1 ? "                " :
                                Stats.CurrentLanguage == 2 ? "            " :
                                Stats.CurrentLanguage == 3 ? "            " :
                                Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5 ? "         " : "            ";
                for (int i = 0; i < creativeOnlinePlatformIdArr.Length; i++) {
                    strbuilder.Append(i == 0 ? $"{Multilingual.GetWord("level_detail_creative_author")} : {createAuthorArr[i]} ({this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])})"
                                             : $"{Environment.NewLine}{indent}{createAuthorArr[i]} ({this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])})");
                }
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_share_code")} : {info.CreativeShareCode}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_version")} : v{info.CreativeVersion}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_max_players")} : {info.CreativeMaxPlayer}{Multilingual.GetWord("level_detail_creative_player_suffix")}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_time_limit")} : {TimeSpan.FromSeconds(info.CreativeTimeLimitSeconds > 0 ? info.CreativeTimeLimitSeconds : 240):m\\:ss}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_platform")} : {this.GetCreativePlatformName(info.CreativePlatformId)}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_last_modified")} : {info.CreativeLastModifiedDate.ToString(Multilingual.GetWord("level_date_format"))}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"{Multilingual.GetWord("level_detail_creative_play_count")} : {info.CreativePlayCount}{Multilingual.GetWord("level_detail_creative_inning")}");
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append(Environment.NewLine);
                strbuilder.Append($"# {Multilingual.GetWord("level_detail_share_code_copied_tooltip")}");

                this.StatsForm.AllocCustomTooltip(this.StatsForm.cmtt_levelDetails_Draw);
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X, cursorPosition.Y);
                this.StatsForm.ShowCustomTooltip(strbuilder.ToString(), this, position);
            }
        }

        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            this.StatsForm.HideCustomTooltip(this);
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