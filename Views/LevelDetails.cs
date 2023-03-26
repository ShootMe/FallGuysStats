using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public partial class LevelDetails : MetroFramework.Forms.MetroForm {
        public string LevelName { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        public Stats StatsForm { get; set; }
        private int _showStats;
        private readonly DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public LevelDetails() {
            this.InitializeComponent();
        }
        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            if (this.Theme == MetroThemeStyle.Light) {
                this.dataGridViewCellStyle1.BackColor = Color.LightGray;
                this.dataGridViewCellStyle1.ForeColor = Color.Black;
                this.dataGridViewCellStyle1.SelectionBackColor = Color.Cyan;
                //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;

                this.dataGridViewCellStyle2.BackColor = Color.White;
                this.dataGridViewCellStyle2.ForeColor = Color.Black;
                this.dataGridViewCellStyle2.SelectionBackColor = Color.DeepSkyBlue;
                this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            } else if (this.Theme == MetroThemeStyle.Dark) {
                this.dataGridViewCellStyle1.BackColor = Color.FromArgb(2, 2, 2);
                this.dataGridViewCellStyle1.ForeColor = Color.DarkGray;
                this.dataGridViewCellStyle1.SelectionBackColor = Color.DarkSlateBlue;
                //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;

                this.dataGridViewCellStyle2.BackColor = Color.FromArgb(49, 51, 56);
                this.dataGridViewCellStyle2.ForeColor = Color.WhiteSmoke;
                this.dataGridViewCellStyle2.SelectionBackColor = Color.PaleGreen;
                this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            }
        }
        private int GetClientWidth(string level) {
            int lang = Stats.CurrentLanguage;
            switch (level) {
                case "Shows":
                    return this.Width + (lang == 0 ? 99 : lang == 1 ? 130 : lang == 2 ? 73 : lang == 3 ? 80 : 50);
                case "Rounds":
                    return this.Width + (lang == 0 ? 909 : lang == 1 ? 948 : lang == 2 ? 848 : lang == 3 ? 858 : 836);
                case "Finals":
                    return this.Width + (lang == 0 ? 909 : lang == 1 ? 948 : lang == 2 ? 848 : lang == 3 ? 858 : 836);
                default:
                    return this.Width + (lang == 0 ? 759 : lang == 1 ? 798 : lang == 2 ? 698 : lang == 3 ? 708 : 685);
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
                    return 150;
                case "Round":
                    sizeOfText = TextRenderer.MeasureText(columnText, this.dataGridViewCellStyle1.Font).Width;
                    break;
                case "Name":
                    return 150;
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
                    return 120;
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
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
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
            this.ClientSize = new Size(this.GetClientWidth(this.LevelName), this.Height + 86);
            if (this.LevelName == "Shows") {
                this.gridDetails.Name = "gridShowsStats";
                this.Text = $@"{Multilingual.GetWord("level_detail_show_stats")} - {StatsForm.GetCurrentProfile()}";
                this._showStats = 2;
            } else if (this.LevelName == "Rounds") {
                this.gridDetails.Name = "gridRoundsStats";
                this.Text = $@"{Multilingual.GetWord("level_detail_round_stats")} - {StatsForm.GetCurrentProfile()}";
                this._showStats = 1;
            } else if (this.LevelName == "Finals") {
                this.gridDetails.Name = "gridFinalsStats";
                this.Text = $@"{Multilingual.GetWord("level_detail_final_stats")} - {StatsForm.GetCurrentProfile()}";
                this._showStats = 1;
            } else {
                this.gridDetails.Name = "gridRoundStats";
                this._showStats = 0;
                this.Text = $@"{Multilingual.GetWord("level_detail_level_stats")} - {this.LevelName}";
            }

            this.gridDetails.DataSource = RoundDetails;
            if (this.gridDetails.RowCount == 0) {
                this.gridDetails.DeallocContextMenu();
            }

            if (this._showStats == 2 && this.gridDetails.RowCount > 0) {
                // add separator
                this.gridDetails.CMenu.Items.Add("-");
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
                this.gridDetails.MoveShows.Click += this.MoveShows_Click;
                this.gridDetails.CMenu.Items.Add(this.gridDetails.MoveShows);
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
                this.gridDetails.DeleteShows.Click += this.DeleteShows_Click;
                this.gridDetails.CMenu.Items.Add(this.gridDetails.DeleteShows);
            }

            foreach (object item in this.gridDetails.CMenu.Items) {
                if (item is ToolStripMenuItem tsi) {
                    tsi.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tsi.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    tsi.MouseEnter += CMenu_MouseEnter;
                    tsi.MouseLeave += CMenu_MouseLeave;
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
                } else if (item is ToolStripSeparator tss) {
                    tss.Paint += MnuToolStripSeparator_Custom_Paint;
                    tss.BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
                    tss.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                }
            }
        }
        private void MnuToolStripSeparator_Custom_Paint(object sender, PaintEventArgs e) {
            ToolStripSeparator sep = (ToolStripSeparator)sender;
            e.Graphics.FillRectangle(new SolidBrush(this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)), 0, 0, sep.Width, sep.Height); // CUSTOM_COLOR_BACKGROUND
            e.Graphics.DrawLine(new Pen(this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2); // CUSTOM_COLOR_FOREGROUND

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
        private void GridDetails_DataSourceChanged(object sender, EventArgs e) {
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
            this.gridDetails.Setup("Round", pos++, this.GetDataGridViewColumnWidth("Round", $"{Multilingual.GetWord("level_detail_round")}"), $"{Multilingual.GetWord("level_detail_round")}{(_showStats == 2 ? Multilingual.GetWord("level_detail_round_suffix") : "")}", DataGridViewContentAlignment.MiddleRight);
            if (this._showStats == 1) { // Rounds
                this.gridDetails.Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                this.gridDetails.Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon", ""), "", DataGridViewContentAlignment.MiddleCenter);
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
                this.gridDetails.Setup("Players", pos++, this.GetDataGridViewColumnWidth("Players", $"{Multilingual.GetWord("level_detail_players")}"), $"{Multilingual.GetWord("level_detail_players")}", DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("PlayersPs4", pos++, this.GetDataGridViewColumnWidth("PlayersPs4", $"{Multilingual.GetWord("level_detail_playersPs4")}"), $"{Multilingual.GetWord("level_detail_playersPs4")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersPs5", pos++, this.GetDataGridViewColumnWidth("PlayersPs5", $"{Multilingual.GetWord("level_detail_playersPs5")}"), $"{Multilingual.GetWord("level_detail_playersPs5")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersXb1", pos++, this.GetDataGridViewColumnWidth("PlayersXb1", $"{Multilingual.GetWord("level_detail_playersXb1")}"), $"{Multilingual.GetWord("level_detail_playersXb1")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersXsx", pos++, this.GetDataGridViewColumnWidth("PlayersXsx", $"{Multilingual.GetWord("level_detail_playersXsx")}"), $"{Multilingual.GetWord("level_detail_playersXsx")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersSw", pos++, this.GetDataGridViewColumnWidth("PlayersSw", $"{Multilingual.GetWord("level_detail_playersSw")}"), $"{Multilingual.GetWord("level_detail_playersSw")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersPc", pos++, this.GetDataGridViewColumnWidth("PlayersPc", $"{Multilingual.GetWord("level_detail_playersPc")}"), $"{Multilingual.GetWord("level_detail_playersPc")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Setup("PlayersBots", pos++, this.GetDataGridViewColumnWidth("PlayersBots", $"{Multilingual.GetWord("level_detail_playersBots")}"), $"{Multilingual.GetWord("level_detail_playersBots")}", DataGridViewContentAlignment.MiddleCenter);
                this.gridDetails.Columns["PlayersEtc"].Visible = false;
            }
            this.gridDetails.Setup("Start", pos++, this.GetDataGridViewColumnWidth("Start", $"{Multilingual.GetWord("level_detail_start")}"), $"{Multilingual.GetWord("level_detail_start")}", DataGridViewContentAlignment.MiddleCenter);
            this.gridDetails.Setup("End", pos++, this.GetDataGridViewColumnWidth("End", $"{Multilingual.GetWord("level_detail_end")}"), $"{Multilingual.GetWord("level_detail_end")}", DataGridViewContentAlignment.MiddleCenter);
            if (this._showStats == 2) { // Shows
                this.gridDetails.Columns["Finish"].Visible = false;
            } else {
                this.gridDetails.Setup("Finish", pos++, this.GetDataGridViewColumnWidth("Finish", $"{Multilingual.GetWord("level_detail_finish")}"), $"{Multilingual.GetWord("level_detail_finish")}", DataGridViewContentAlignment.MiddleCenter);
            }
            if (this._showStats == 2) { // Shows
                this.gridDetails.Columns["Position"].Visible = false;
                this.gridDetails.Columns["Score"].Visible = false;
            } else {
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
        private void GridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= this.gridDetails.Rows.Count) { return; }

            RoundInfo info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;

            //RoundInfo nextInfo = null;
            //if (e.RowIndex > this.gridDetails.Rows.Count) {
            //    nextInfo = this.gridDetails.Rows[e.RowIndex+1].DataBoundItem as RoundInfo;
            //}
            if (info.PrivateLobby) { // Custom
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(8, 8, 8);
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
            if (this.gridDetails.Columns[e.ColumnIndex].Name == "End") {
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Start") {
                e.Value = info.StartLocal.ToString("yyyy-MM-dd HH:mm");
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
                if ((this._showStats == 0 || this._showStats == 1) && this.StatsForm.StatLookup.TryGetValue((string)this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value, out LevelStats level)) {
                    e.Value = level.RoundIcon;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Round") {
                if (this._showStats == 1 && this.StatsForm.StatLookup.TryGetValue((string)this.gridDetails.Rows[e.RowIndex].Cells["Name"].Value, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam);
                    //e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : Color.FromArgb(c1.A, (int)(c1.R * 0.5), (int)(c1.G * 0.5), (int)(c1.B * 0.5));
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : info.PrivateLobby ? c1 : ControlPaint.LightLight(c1);
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                if (this.StatsForm.StatLookup.TryGetValue((string)e.Value, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam);
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : info.PrivateLobby ? c1 : ControlPaint.LightLight(c1);
                    e.Value = level.Name;
                    //gridDetails.Columns[e.ColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            } else if (this.gridDetails.Columns[e.ColumnIndex].Name == "ShowNameId") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = Multilingual.GetShowName((string)e.Value);
                }
                //gridDetails.Columns[e.ColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            } else if (this._showStats != 2 && this.gridDetails.Columns[e.ColumnIndex].Name == "PlayersPs4") {
                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPs4_desc");
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
        private void GridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = this.gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridDetails.GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "ShowID"; }

            this.RoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
                int roundCompare = one.Round.CompareTo(two.Round);
                int showCompare = one.ShowID.CompareTo(two.ShowID);
                if (sortOrder == SortOrder.Descending) {
                    (two, one) = (one, two);
                }

                switch (columnName) {
                    case "ShowID":
                        showCompare = one.ShowID.CompareTo(two.ShowID);
                        return showCompare != 0 ? showCompare : roundCompare;
                    case "ShowNameId":
                        int showNameCompare = (string.IsNullOrEmpty(one.ShowNameId) ? @" " : one.ShowNameId).CompareTo(string.IsNullOrEmpty(two.ShowNameId) ? @" " : two.ShowNameId);
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
        private void GridDetails_SelectionChanged(object sender, EventArgs e) {
            if (this._showStats != 2 && this.gridDetails.SelectedCells.Count > 0) {
                this.gridDetails.ClearSelection();
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

                    if (MessageBox.Show(this,
                            $@"{Multilingual.GetWord("message_delete_show_prefix")} ({rows.Count}) {Multilingual.GetWord("message_delete_show_suffix")}",
                            Multilingual.GetWord("message_delete_show_caption"),
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
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
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MoveShows_Click(object sender, EventArgs e) {
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
                        if (moveShows.Profiles.Count == 1) {
                            return;
                        }
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
        private void DeleteShows_Click(object sender, EventArgs e) {
            int selectedCount = this.gridDetails.SelectedCells.Count;
            if (selectedCount > 0) {
                HashSet<RoundInfo> rows = new HashSet<RoundInfo>();
                int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                for (int i = 0; i < selectedCount; i++) {
                    DataGridViewCell cell = this.gridDetails.SelectedCells[i];
                    rows.Add((RoundInfo)this.gridDetails.Rows[cell.RowIndex].DataBoundItem);
                }

                if (MessageBox.Show(this,
                        $@"{Multilingual.GetWord("message_delete_show_prefix")} ({rows.Count}) {Multilingual.GetWord("message_delete_show_suffix")}",
                        Multilingual.GetWord("message_delete_show_caption"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
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
    }
}