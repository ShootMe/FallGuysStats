﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public partial class LevelDetails : MetroFramework.Forms.MetroForm {
        public string LevelName { get; set; }
        public Image RoundIcon { get; set; }
        public bool IsCreative { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        public List<RoundInfo> currentRoundDetails;
        public Stats StatsForm { get; set; }
        private StatType statType;
        private int currentPage, totalPages;
        private readonly int pageSize = 1000;
        private int currentProfileId = -1;
        // private int totalHeight;
        readonly DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        readonly DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        
        private Timer spinnerTransition = new Timer { Interval = 1 };
        private bool isIncreasing;
        
        private Timer scrollTimer = new Timer { Interval = 100 };
        private bool isScrollingStopped = true;

        public LevelDetails() {
            this.InitializeComponent();
            this.Opacity = 0;
            this.scrollTimer.Tick += this.scrollTimer_Tick;
            this.spinnerTransition.Tick += this.spinnerTransition_Tick;
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

            this.currentProfileId = this.StatsForm.GetCurrentProfileId();
            this.gridDetails.CurrentCell = null;
            this.gridDetails.ClearSelection();
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(20, 20, 0, 0);
            if (string.Equals(this.LevelName, "Shows")) {
                this.gridDetails.Name = "gridShowsStats";
                this.gridDetails.MultiSelect = true;
                this.BackImage = Properties.Resources.fallguys_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_show_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this.statType = StatType.Shows;
            } else if (string.Equals(this.LevelName, "Rounds")) {
                this.gridDetails.Name = "gridRoundsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_round_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this.statType = StatType.Rounds;
            } else if (string.Equals(this.LevelName, "Finals")) {
                this.gridDetails.Name = "gridFinalsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_final_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                this.statType = StatType.Rounds;
            } else {
                this.gridDetails.Name = "gridLevelsStats";
                this.gridDetails.MultiSelect = false;
                this.BackImage = this.RoundIcon;
                this.Text = $@"     {Multilingual.GetWord("level_detail_level_stats")} - {(this.IsCreative ? "🛠️ " : "")}{Multilingual.GetRoundName(this.LevelName)} ({StatsForm.GetCurrentFilterName()})";
                this.statType = StatType.Levels;
            }
            this.ClientSize = new Size(this.GetClientWidth(), this.Height + 387);
            
            this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
            this.currentPage = this.totalPages;
            if (this.totalPages > 1) {
                this.SetPagingDisplay(true);
                this.EnablePagingUI(false);
                this.gridDetails.Enabled = true;
            }
            this.UpdatePage(false, true, false, true);
        }
        
        public void LevelDetails_OnUpdatedLevelDetails() {
            switch (this.statType) {
                case StatType.Shows when string.Equals(this.gridDetails.Name, "gridShowsStats"):
                    this.RoundDetails = this.StatsForm.GetShowsForDisplay();
                    this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                    if (this.currentProfileId != this.StatsForm.GetCurrentProfileId()) {
                        this.currentProfileId = this.StatsForm.GetCurrentProfileId();
                        this.currentPage = this.totalPages;
                    }
                    this.UpdatePage(false, true, false, false);
                    this.BackImage = Properties.Resources.fallguys_icon;
                    this.Text = $@"     {Multilingual.GetWord("level_detail_show_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                    this.Invalidate();
                    break;
                case StatType.Rounds when string.Equals(this.gridDetails.Name, "gridRoundsStats"):
                    this.RoundDetails = this.StatsForm.GetRoundsForDisplay();
                    this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                    if (this.currentProfileId != this.StatsForm.GetCurrentProfileId()) {
                        this.currentProfileId = this.StatsForm.GetCurrentProfileId();
                        this.currentPage = this.totalPages;
                    }
                    this.UpdatePage(false, true, false, false);
                    this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.round_icon : Properties.Resources.round_gray_icon;
                    this.Text = $@"     {Multilingual.GetWord("level_detail_round_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                    this.Invalidate();
                    break;
                case StatType.Rounds when string.Equals(this.gridDetails.Name, "gridFinalsStats"):
                    this.RoundDetails = this.StatsForm.GetFinalsForDisplay();
                    this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                    if (this.currentProfileId != this.StatsForm.GetCurrentProfileId()) {
                        this.currentProfileId = this.StatsForm.GetCurrentProfileId();
                        this.currentPage = this.totalPages;
                    }
                    this.UpdatePage(false, true, false, false);
                    this.BackImage = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                    this.Text = $@"     {Multilingual.GetWord("level_detail_final_stats")} - {StatsForm.GetCurrentProfileName().Replace("&", "&&")} ({StatsForm.GetCurrentFilterName()})";
                    this.Invalidate();
                    break;
                case StatType.Levels when string.Equals(this.gridDetails.Name, "gridLevelsStats"):
                    LevelStats levelStats = this.StatsForm.GetFilteredDataSource(this.StatsForm.CurrentSettings.GroupingCreativeRoundLevels).Find(l => string.Equals(l.Id, this.LevelName));
                    this.RoundDetails = levelStats.Stats;
                    this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                    if (this.currentProfileId != this.StatsForm.GetCurrentProfileId()) {
                        this.currentProfileId = this.StatsForm.GetCurrentProfileId();
                        this.currentPage = this.totalPages;
                    }
                    this.UpdatePage(false, true, false, false);
                    this.BackImage = levelStats.RoundIcon;
                    this.Text = $@"     {Multilingual.GetWord("level_detail_level_stats")} - {(this.IsCreative ? "🛠️ " : "")}{Multilingual.GetRoundName(this.LevelName)} ({StatsForm.GetCurrentFilterName()})";
                    this.Invalidate();
                    break;
            }
        }

        private void SetContextMenu() {
            if (this.gridDetails.RowCount == 0) {
                this.gridDetails.DeallocContextMenu();
            } else {
                if (this.statType == StatType.Shows) {
                    this.gridDetails.MenuSeparator = new ToolStripSeparator {
                        BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)
                        , ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray
                    };
                    this.gridDetails.MenuSeparator.Paint += this.gridDetails.CustomToolStripSeparator_Paint;
                    // this.gridDetails.CMenu.Items.Add(this.gridDetails.MenuSeparator);

                    if (this.StatsForm.AllProfiles.Count > 1) {
                        this.gridDetails.MoveShows = new ToolStripMenuItem {
                            Name = "moveShows"
                            , Size = new Size(134, 22)
                            , Text = Multilingual.GetWord("main_move_shows")
                            , ShowShortcutKeys = true
                            , Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.move : Properties.Resources.move_gray
                            , ShortcutKeys = Keys.Control | Keys.P
                            , BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)
                            , ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray
                        };
                        this.gridDetails.MoveShows.Click += this.moveShows_Click;
                        this.gridDetails.MoveShows.MouseEnter += this.gridDetails.CMenu_MouseEnter;
                        this.gridDetails.MoveShows.MouseLeave += this.gridDetails.CMenu_MouseLeave;
                        // this.gridDetails.CMenu.Items.Add(this.gridDetails.MoveShows);
                    }
                    
                    this.gridDetails.DeleteShows = new ToolStripMenuItem {
                        Name = "deleteShows"
                        , Size = new Size(134, 22)
                        , Text = Multilingual.GetWord("main_delete_shows")
                        , ShowShortcutKeys = true
                        , Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.delete : Properties.Resources.delete_gray
                        , ShortcutKeys = Keys.Control | Keys.D
                        , BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)
                        , ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray
                    };
                    this.gridDetails.DeleteShows.Click += this.deleteShows_Click;
                    this.gridDetails.DeleteShows.MouseEnter += this.gridDetails.CMenu_MouseEnter;
                    this.gridDetails.DeleteShows.MouseLeave += this.gridDetails.CMenu_MouseLeave;
                    // this.gridDetails.CMenu.Items.Add(this.gridDetails.DeleteShows);
                } else {
                    this.gridDetails.MenuSeparator = new ToolStripSeparator {
                        BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)
                        , ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray
                    };
                    this.gridDetails.MenuSeparator.Paint += this.gridDetails.CustomToolStripSeparator_Paint;
                    // this.gridDetails.CMenu.Items.Add(this.gridDetails.MenuSeparator);
                    
                    this.gridDetails.UpdateCreativeLevel = new ToolStripMenuItem {
                        Name = "updateCreativeShows"
                        , Size = new Size(134, 22)
                        , Text = Multilingual.GetWord("main_update_shows")
                        , ShowShortcutKeys = true
                        , Image = this.Theme == MetroThemeStyle.Light ? Properties.Resources.update : Properties.Resources.update_gray
                        , ShortcutKeys = Keys.Control | Keys.U
                        , BackColor = this.Theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17)
                        , ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray
                    };
                    this.gridDetails.UpdateCreativeLevel.Click += this.updateLevel_Click;
                    this.gridDetails.UpdateCreativeLevel.MouseEnter += this.gridDetails.CMenu_MouseEnter;
                    this.gridDetails.UpdateCreativeLevel.MouseLeave += this.gridDetails.CMenu_MouseLeave;
                    // this.gridDetails.CMenu.Items.Add(this.gridDetails.UpdateCreativeLevel);
                }
                this.gridDetails.SetContextMenuTheme();
            }
        }
        
        private void spinnerTransition_Tick(object sender, EventArgs e) {
            if (this.isIncreasing) {
                this.mpsSpinner01.Speed = 3.2F;
                if (this.mpsSpinner01.Value < 90) {
                    this.mpsSpinner01.Value++;
                } else {
                    this.isIncreasing = false;
                }
            } else {
                this.mpsSpinner01.Speed = 2.7F;
                if (this.mpsSpinner01.Value > 10) {
                    this.mpsSpinner01.Value--;
                } else {
                    this.isIncreasing = true;
                }
            }
        }

        private void SetPagingDisplay(bool visible) {
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
            this.mlRightPagingButton.Enabled = enable;
            this.mlLastPagingButton.Enabled = enable;
        }
        
        private void UpdatePage(bool isFirstPage, bool isLastPage, bool isFirstDisplayed, bool isInitialize) {
            this.EnablePagingUI(false);
            Task.Run(() => {
                if (this.RoundDetails.Count > 0) {
                    this.currentRoundDetails = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                    if (this.statType != StatType.Shows) {
                        if ((!isFirstPage && !isLastPage) || isLastPage) {
                            int firstShowId = this.currentRoundDetails[0].ShowID;
                            List<RoundInfo> currentShows = this.currentRoundDetails.FindAll(r => r.ShowID == firstShowId);
                            List<RoundInfo> allShows = this.RoundDetails.FindAll(r => r.ShowID == firstShowId);

                            if (currentShows.Count != allShows.Count) {
                                this.currentRoundDetails.RemoveAll(r => r.ShowID == firstShowId);
                                this.currentRoundDetails.InsertRange(0, allShows);
                            }
                        }

                        if ((!isFirstPage && !isLastPage) || isFirstPage) {
                            int lastShowId = this.currentRoundDetails[this.currentRoundDetails.Count - 1].ShowID;
                            List<RoundInfo> currentShows = this.currentRoundDetails.FindAll(r => r.ShowID == lastShowId);
                            List<RoundInfo> allShows = this.RoundDetails.FindAll(r => r.ShowID == lastShowId);

                            if (currentShows.Count != allShows.Count) {
                                this.currentRoundDetails.RemoveAll(r => r.ShowID == lastShowId);
                                this.currentRoundDetails.AddRange(allShows);
                            }
                        }
                    }
                }
            }).ContinueWith(prevTask => {
                this.BeginInvoke((MethodInvoker)delegate {
                    if (this.RoundDetails.Count > 0) {
                        int prevIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                        this.gridDetails.DataSource = this.currentRoundDetails;
                        if (this.gridDetails.RowCount > 0) {
                            this.gridDetails.FirstDisplayedScrollingRowIndex = this.currentPage < this.totalPages && this.gridDetails.RowCount < prevIndex ? prevIndex : (isFirstDisplayed ? 0 : this.gridDetails.RowCount - 1);
                        }
                        this.gridDetails.Enabled = true;
                        this.SetPagingDisplay(true);
                    } else {
                        this.gridDetails.DataSource = this.RoundDetails;
                    }

                    if (isInitialize) {
                        this.SetContextMenu();
                    }
                });
            });
        }

        private void pagingButton_Click(object sender, EventArgs e) {
            if (sender.Equals(this.mlFirstPagingButton)) {
                this.currentPage = 1;
                this.UpdatePage(true, false, true, false);
            } else if (sender.Equals(this.mlLeftPagingButton)) {
                this.currentPage -= 1;
                this.UpdatePage(false, false, false, false);
            } else if (sender.Equals(this.mlRightPagingButton)) {
                this.currentPage += 1;
                this.UpdatePage(false, false, true, false);
            } else if (sender.Equals(this.mlLastPagingButton)) {
                this.currentPage = this.totalPages;
                this.UpdatePage(false, true, false, false);
            }
        }

        private void LevelDetails_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();

            this.mpsSpinner01.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
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
        
        private int GetClientWidth() {
            Language lang = Stats.CurrentLanguage;
            switch (this.statType) {
                case StatType.Shows:
                    return this.Width - (lang == Language.English ? -380 : lang == Language.French ? -400 : lang == Language.Korean ? -370 : lang == Language.Japanese ? -370 : -380);
                case StatType.Rounds:
                case StatType.Levels:
                    return this.Width + (lang == Language.English ? 1150 : lang == Language.French ? 1250 : lang == Language.Korean ? 1150 : lang == Language.Japanese ? 1150 : 1230);
                default:
                    return this.Width + (lang == Language.English ? 1150 : lang == Language.French ? 1250 : lang == Language.Korean ? 1150 : lang == Language.Japanese ? 1150 : 1230);
            }
        }
        
        private int GetDataGridViewColumnWidth(string columnName, string columnText = "") {
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
                case "CreativeLikes":
                case "CreativeDislikes":
                    return 80;
                default:
                    return 0;
            }
            
            return sizeOfText + 24;
        }
        
        private void scrollTimer_Tick(object sender, EventArgs e) {
            this.scrollTimer.Stop();
            this.isScrollingStopped = true;
        }
        
        private void gridDetails_Scroll(object sender, ScrollEventArgs e) {
            this.isScrollingStopped = false;
            this.scrollTimer.Stop();
            this.scrollTimer.Start();
            
            // if (((Grid)sender).VerticalScrollingOffset == 0) {
            //     if (this.currentPage <= 1) { return; }
            //     this.currentPage -= 1;
            //     ((Grid)sender).DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
            // } else if (this.totalHeight - ((Grid)sender).Height < ((Grid)sender).VerticalScrollingOffset) {
            //     if (this.currentPage >= this.totalPages) { return; }
            //     this.currentPage += 1;
            //     ((Grid)sender).DataSource = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
            // }
        }

        // private void gridDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            // if (((Grid)sender).RowCount > 0) ((Grid)sender).FirstDisplayedScrollingRowIndex = ((Grid)sender).RowCount - 1;
        // }
        
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            if (((Grid)sender).Columns.Count == 0) { return; }

            if (this.totalPages > 1) {
                if (this.currentPage == 1) {
                    this.mlFirstPagingButton.Enabled = false;
                    this.mlLeftPagingButton.Enabled = false;
                    this.mlRightPagingButton.Enabled = true;
                    this.mlLastPagingButton.Enabled = true;
                } else if (this.currentPage == this.totalPages) {
                    this.mlFirstPagingButton.Enabled = true;
                    this.mlLeftPagingButton.Enabled = true;
                    this.mlRightPagingButton.Enabled = false;
                    this.mlLastPagingButton.Enabled = false;
                } else {
                    this.mlFirstPagingButton.Enabled = true;
                    this.mlLeftPagingButton.Enabled = true;
                    this.mlRightPagingButton.Enabled = true;
                    this.mlLastPagingButton.Enabled = true;
                }
            } else {
                this.mlFirstPagingButton.Enabled = false;
                this.mlLeftPagingButton.Enabled = false;
                this.mlRightPagingButton.Enabled = false;
                this.mlLastPagingButton.Enabled = false;
                this.lblPagingInfo.Visible = false;
                this.mlFirstPagingButton.Visible = false;
                this.mlLeftPagingButton.Visible = false;
                this.mlRightPagingButton.Visible = false;
                this.mlLastPagingButton.Visible = false;
            }
            
            int pos = 0;
            ((Grid)sender).Columns["Tier"].Visible = false;
            ((Grid)sender).Columns["ID"].Visible = false;
            ((Grid)sender).Columns["Crown"].Visible = false;
            ((Grid)sender).Columns["Profile"].Visible = false;
            ((Grid)sender).Columns["InParty"].Visible = false;
            ((Grid)sender).Columns["PrivateLobby"].Visible = false;
            ((Grid)sender).Columns["Qualified"].Visible = false;
            ((Grid)sender).Columns["IsFinal"].Visible = false;
            ((Grid)sender).Columns["IsTeam"].Visible = false;
            ((Grid)sender).Columns["SessionId"].Visible = false;
            ((Grid)sender).Columns["IsAbandon"].Visible = false;
            ((Grid)sender).Columns["UseShareCode"].Visible = false;
            ((Grid)sender).Columns["CreativeShareCode"].Visible = false;
            ((Grid)sender).Columns["CreativeStatus"].Visible = false;
            ((Grid)sender).Columns["CreativeAuthor"].Visible = false;
            ((Grid)sender).Columns["CreativeOnlinePlatformId"].Visible = false;
            ((Grid)sender).Columns["CreativeVersion"].Visible = false;
            ((Grid)sender).Columns["CreativeTitle"].Visible = false;
            ((Grid)sender).Columns["CreativeDescription"].Visible = false;
            ((Grid)sender).Columns["CreativeCreatorTags"].Visible = false;
            ((Grid)sender).Columns["CreativeMaxPlayer"].Visible = false;
            ((Grid)sender).Columns["CreativeThumbUrl"].Visible = false;
            ((Grid)sender).Columns["CreativePlatformId"].Visible = false;
            ((Grid)sender).Columns["CreativeLastModifiedDate"].Visible = false;
            ((Grid)sender).Columns["CreativePlayCount"].Visible = false;
            ((Grid)sender).Columns["CreativeQualificationPercent"].Visible = false;
            ((Grid)sender).Columns["CreativeTimeLimitSeconds"].Visible = false;
            ((Grid)sender).Columns["CreativeGameModeId"].Visible = false;
            ((Grid)sender).Columns["CreativeLevelThemeId"].Visible = false;
            ((Grid)sender).Columns["OnlineServiceType"].Visible = false;
            ((Grid)sender).Columns["OnlineServiceId"].Visible = false;
            ((Grid)sender).Columns["OnlineServiceNickname"].Visible = false;
            if (this.statType == StatType.Levels) {
                ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                ((Grid)sender).Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon"), "", DataGridViewContentAlignment.MiddleCenter);
            }
            ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = Multilingual.GetWord("level_detail_medal") });
            ((Grid)sender).Setup("Medal", pos++, this.GetDataGridViewColumnWidth("Medal", $"{Multilingual.GetWord("level_detail_medal")}"), $"{Multilingual.GetWord("level_detail_medal")}", DataGridViewContentAlignment.MiddleCenter);
            if (this.statType == StatType.Shows) {
                ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "IsFinalIcon", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "IsFinalIcon" });
                ((Grid)sender).Setup("IsFinalIcon", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
                //((Grid)sender).Setup("IsFinal", pos++, this.GetDataGridViewColumnWidth("IsFinalIcon", $"{Multilingual.GetWord("level_detail_is_final")}"), $"{Multilingual.GetWord("level_detail_is_final")}", DataGridViewContentAlignment.MiddleCenter);
            }
            ((Grid)sender).Setup("ShowID", pos++, this.GetDataGridViewColumnWidth("ShowID", $"{Multilingual.GetWord("level_detail_show_id")}"), $"{Multilingual.GetWord("level_detail_show_id")}", DataGridViewContentAlignment.MiddleRight);
            ((Grid)sender).Setup("ShowNameId", pos++, this.GetDataGridViewColumnWidth("ShowNameId", $"{Multilingual.GetWord("level_detail_show_name_id")}"), $"{Multilingual.GetWord("level_detail_show_name_id")}", DataGridViewContentAlignment.MiddleLeft);
            ((Grid)sender).Columns["ShowNameId"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ((Grid)sender).Setup("Round", pos++, this.GetDataGridViewColumnWidth("Round", $"{Multilingual.GetWord("level_detail_round")}{(this.statType == StatType.Shows ? Multilingual.GetWord("level_detail_round_suffix") : "")}"), $"{Multilingual.GetWord("level_detail_round")}{(this.statType == StatType.Shows ? Multilingual.GetWord("level_detail_round_suffix") : "")}", DataGridViewContentAlignment.MiddleRight);
            if (this.statType == StatType.Rounds) {
                ((Grid)sender).Columns.Add(new DataGridViewImageColumn { Name = "RoundIcon", ImageLayout = DataGridViewImageCellLayout.Zoom });
                ((Grid)sender).Setup("RoundIcon", pos++, this.GetDataGridViewColumnWidth("RoundIcon"), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
                ((Grid)sender).Columns["Name"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                ((Grid)sender).Setup("CreativeLikes", pos++, this.GetDataGridViewColumnWidth("CreativeLikes"), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["CreativeLikes"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                ((Grid)sender).Setup("CreativeDislikes", pos++, this.GetDataGridViewColumnWidth("CreativeDislikes"), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["CreativeDislikes"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            } else if (this.statType == StatType.Levels) {
                ((Grid)sender).Setup("Name", pos++, this.GetDataGridViewColumnWidth("Name", $"{Multilingual.GetWord("level_detail_name")}"), $"{Multilingual.GetWord("level_detail_name")}", DataGridViewContentAlignment.MiddleLeft);
                ((Grid)sender).Columns["Name"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                ((Grid)sender).Setup("CreativeLikes", pos++, this.GetDataGridViewColumnWidth("CreativeLikes"), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["CreativeLikes"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                ((Grid)sender).Setup("CreativeDislikes", pos++, this.GetDataGridViewColumnWidth("CreativeDislikes"), "", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["CreativeDislikes"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            } else {
                ((Grid)sender).Columns["Name"].Visible = false;
                ((Grid)sender).Columns["CreativeLikes"].Visible = false;
                ((Grid)sender).Columns["CreativeDislikes"].Visible = false;
            }
            if (this.statType == StatType.Shows) {
                ((Grid)sender).Columns["Players"].Visible = false;
                ((Grid)sender).Columns["PlayersPs4"].Visible = false;
                ((Grid)sender).Columns["PlayersPs5"].Visible = false;
                ((Grid)sender).Columns["PlayersXb1"].Visible = false;
                ((Grid)sender).Columns["PlayersXsx"].Visible = false;
                ((Grid)sender).Columns["PlayersSw"].Visible = false;
                ((Grid)sender).Columns["PlayersPc"].Visible = false;
                ((Grid)sender).Columns["PlayersBots"].Visible = false;
                ((Grid)sender).Columns["PlayersEtc"].Visible = false;
            } else {
                ((Grid)sender).Setup("Players", pos++,     this.GetDataGridViewColumnWidth("Players", $"{Multilingual.GetWord("level_detail_players")}"), $"{Multilingual.GetWord("level_detail_players")}", DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("PlayersPs4", pos++,  this.GetDataGridViewColumnWidth("PlayersPs4", $"{Multilingual.GetWord("level_detail_playersPs4")}"), $"{Multilingual.GetWord("level_detail_playersPs4")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersPs5", pos++,  this.GetDataGridViewColumnWidth("PlayersPs5", $"{Multilingual.GetWord("level_detail_playersPs5")}"), $"{Multilingual.GetWord("level_detail_playersPs5")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersXb1", pos++,  this.GetDataGridViewColumnWidth("PlayersXb1", $"{Multilingual.GetWord("level_detail_playersXb1")}"), $"{Multilingual.GetWord("level_detail_playersXb1")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersXsx", pos++,  this.GetDataGridViewColumnWidth("PlayersXsx", $"{Multilingual.GetWord("level_detail_playersXsx")}"), $"{Multilingual.GetWord("level_detail_playersXsx")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersSw", pos++,   this.GetDataGridViewColumnWidth("PlayersSw", $"{Multilingual.GetWord("level_detail_playersSw")}"), $"{Multilingual.GetWord("level_detail_playersSw")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersPc", pos++,   this.GetDataGridViewColumnWidth("PlayersPc", $"{Multilingual.GetWord("level_detail_playersPc")}"), $"{Multilingual.GetWord("level_detail_playersPc")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("PlayersBots", pos++, this.GetDataGridViewColumnWidth("PlayersBots", $"{Multilingual.GetWord("level_detail_playersBots")}"), $"{Multilingual.GetWord("level_detail_playersBots")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Columns["PlayersEtc"].Visible = false;
            }
            ((Grid)sender).Setup("Start", pos++, this.GetDataGridViewColumnWidth("Start", $"{Multilingual.GetWord("level_detail_start")}"), $"{Multilingual.GetWord("level_detail_start")}", DataGridViewContentAlignment.MiddleCenter);
            ((Grid)sender).Setup("End", pos++, this.GetDataGridViewColumnWidth("End", $"{Multilingual.GetWord("level_detail_end")}"), $"{Multilingual.GetWord("level_detail_end")}", DataGridViewContentAlignment.MiddleCenter);
            if (this.statType == StatType.Shows) {
                ((Grid)sender).Columns["Finish"].Visible = false;
                ((Grid)sender).Columns["Position"].Visible = false;
                ((Grid)sender).Columns["Score"].Visible = false;
            } else {
                ((Grid)sender).Setup("Finish", pos++, this.GetDataGridViewColumnWidth("Finish", $"{Multilingual.GetWord("level_detail_finish")}"), $"{Multilingual.GetWord("level_detail_finish")}", DataGridViewContentAlignment.MiddleCenter);
                ((Grid)sender).Setup("Position", pos++, this.GetDataGridViewColumnWidth("Position", $"{Multilingual.GetWord("level_detail_position")}"), $"{Multilingual.GetWord("level_detail_position")}", DataGridViewContentAlignment.MiddleRight);
                ((Grid)sender).Setup("Score", pos++, this.GetDataGridViewColumnWidth("Score", $"{Multilingual.GetWord("level_detail_score")}"), $"{Multilingual.GetWord("level_detail_score")}", DataGridViewContentAlignment.MiddleRight);
            }
            ((Grid)sender).Setup("Kudos", pos++, this.GetDataGridViewColumnWidth("Kudos", $"{Multilingual.GetWord("level_detail_kudos")}"), $"{Multilingual.GetWord("level_detail_kudos")}", DataGridViewContentAlignment.MiddleRight);

            bool colorSwitch = true;
            int lastShow = -1;
            Color backColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            Color foreColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            // this.totalHeight = 0;
            for (int i = 0; i < ((Grid)sender).RowCount; i++) {
                int showID = (int)((Grid)sender).Rows[i].Cells["ShowID"].Value;
                if (showID != lastShow) {
                    colorSwitch = !colorSwitch;
                    lastShow = showID;
                }
            
                if (colorSwitch) {
                    ((Grid)sender).Rows[i].DefaultCellStyle.BackColor = backColor;
                    ((Grid)sender).Rows[i].DefaultCellStyle.ForeColor = foreColor;
                }
                // this.totalHeight += ((Grid)sender).Rows[i].Height;
            }
            ((Grid)sender).ClearSelection();
        }

        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (((Grid)sender).RowCount <= 0 || e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            RoundInfo info = ((Grid)sender).Rows[e.RowIndex].DataBoundItem as RoundInfo;
            if (info.PrivateLobby) { // Custom
                e.CellStyle.BackColor = this.Theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(8, 8, 8);
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
            
            if (((Grid)sender).Columns[e.ColumnIndex].Name == "End") {
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Start") {
                e.Value = info.StartLocal.ToString(Multilingual.GetWord("level_grid_date_format"), Utils.GetCultureInfo());
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Finish") {
                if (info.Finish.HasValue) {
                    e.Value = (info.Finish.Value - info.Start).ToString("m\\:ss\\.fff");
                }
            } else if (this.statType == StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "Qualified") {
                e.Value = !string.IsNullOrEmpty(info.Name);
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0:
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_pink");
                            e.Value = Properties.Resources.medal_pink_grid_icon;
                            break;
                        case 1:
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_gold");
                            e.Value = Properties.Resources.medal_gold_grid_icon;
                            break;
                        case 2:
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_silver");
                            e.Value = Properties.Resources.medal_silver_grid_icon;
                            break;
                        case 3:
                            ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_bronze");
                            e.Value = Properties.Resources.medal_bronze_grid_icon;
                            break;
                    }
                } else {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_eliminated");
                    e.Value = Properties.Resources.medal_eliminated_grid_icon;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "IsFinalIcon") {
                if (info.IsFinal || info.Qualified) {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_success_reaching_finals");
                    e.Value = this.Theme == MetroThemeStyle.Light ? Properties.Resources.final_icon : Properties.Resources.final_gray_icon;
                } else {
                    ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_failure_reaching_finals");
                    e.Value = Properties.Resources.uncheckmark_icon;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "ShowID") {
                e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Navy : Color.Snow;
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon") {
                if ((this.statType == StatType.Levels || this.statType == StatType.Rounds) && this.StatsForm.StatLookup.TryGetValue(info.UseShareCode ? (string.IsNullOrEmpty(info.ShowNameId) ? "user_creative_race_round" : info.ShowNameId) : info.Name, out LevelStats level)) {
                    e.Value = level.RoundIcon;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Round") {
                if ((this.statType == StatType.Levels || this.statType == StatType.Rounds) && this.StatsForm.StatLookup.TryGetValue(info.UseShareCode ? info.ShowNameId : info.Name, out LevelStats level)) {
                    Color c1 = level.Type.LevelForeColor(false, info.IsTeam, this.Theme);
                    e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : ControlPaint.LightLight(c1);
                    if (level.IsCreative && (string.IsNullOrEmpty(info.CreativeShareCode) || string.IsNullOrEmpty(info.CreativeLevelThemeId))) {
                        e.Value = $"🔄️ {e.Value}";
                    }
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Name") {
                if (info.UseShareCode) {
                    if (this.StatsForm.StatLookup.TryGetValue((string.IsNullOrEmpty(info.ShowNameId) ? "user_creative_race_round" : info.ShowNameId), out LevelStats level)) {
                        Color c1 = string.IsNullOrEmpty(info.CreativeTitle) ? (this.Theme == MetroThemeStyle.Light ? Color.Navy : Color.Snow) : level.Type.LevelForeColor(info.IsFinal, info.IsTeam, this.Theme);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : ControlPaint.LightLight(c1);
                        e.Value = $"🔧 {info.CreativeTitle}";
                    }
                } else {
                    if (this.StatsForm.StatLookup.TryGetValue((string)e.Value, out LevelStats level)) {
                        Color c1 = level.Type.LevelForeColor(info.IsFinal, info.IsTeam, this.Theme);
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? c1 : ControlPaint.LightLight(c1);
                        e.Value = $"{(level.IsCreative ? "🔧 " : "")}{level.Name}";
                    }
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "CreativeLikes") {
                if ((this.statType == StatType.Levels || this.statType == StatType.Rounds) && this.StatsForm.StatLookup.TryGetValue(info.UseShareCode ? info.ShowNameId : info.Name, out LevelStats level)) {
                    if (level.IsCreative && !string.IsNullOrEmpty(info.CreativeShareCode)) {
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(214, 86, 100) : Color.FromArgb(220, 155, 162);
                        e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.75f);
                        e.Value = $"👍 {e.Value:N0}";
                    } else {
                        e.Value = "";
                    }
                } else {
                    e.Value = "";
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "CreativeDislikes") {
                if ((this.statType == StatType.Levels || this.statType == StatType.Rounds) && this.StatsForm.StatLookup.TryGetValue(info.UseShareCode ? info.ShowNameId : info.Name, out LevelStats level)) {
                    if (level.IsCreative && !string.IsNullOrEmpty(info.CreativeShareCode)) {
                        e.CellStyle.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.FromArgb(101, 82, 183) : Color.FromArgb(147, 136, 195);
                        e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.75f);
                        e.Value = $"👎 {e.Value:N0}";
                    } else {
                        e.Value = "";
                    }
                } else {
                    e.Value = "";
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "ShowNameId") {
                if (!string.IsNullOrEmpty((string)e.Value)) {
                    e.Value = info.UseShareCode ? $"🔧 {Multilingual.GetShowName("fall_guys_creative_mode")}" : Multilingual.GetShowName((string)e.Value) ?? e.Value;
                }
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Position") {
                if ((int)e.Value == 0) { e.Value = ""; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "Players") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 1.1f);
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersPs4") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPs4_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersPs5") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPs5_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersXb1") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersXb1_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersXsx") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersXsx_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersSw") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersSw_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersPc") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersPc_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "PlayersBots") {
                e.CellStyle.Font = Overlay.GetMainFont(e.CellStyle.Font.Size * 0.9f);
                ((Grid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_playersBots_desc");
                if ((int)e.Value == 0) { e.Value = "-"; }
            } else if (this.statType != StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "Score") {
                e.Value = $"{e.Value:N0}";
            } else if (((Grid)sender).Columns[e.ColumnIndex].Name == "Kudos") {
                e.Value = (int)e.Value == 0 ? "" : $"{e.Value:N0}";
            }
        }
        
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (this.currentRoundDetails == null) return;
            string columnName = ((Grid)sender).Columns[e.ColumnIndex].Name;
            if (string.Equals(columnName, "CreativeLikes") || string.Equals(columnName, "CreativeDislikes")) return;
            SortOrder sortOrder = ((Grid)sender).GetSortOrder(columnName);
            if (sortOrder == SortOrder.None) { columnName = "ShowID"; }

            this.currentRoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
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
                        if (this.statType != StatType.Levels) {
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
                        int qualifiedCompare = this.statType == StatType.Shows ? string.IsNullOrEmpty(one.Name).CompareTo(string.IsNullOrEmpty(two.Name)) : one.Qualified.CompareTo(two.Qualified);
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

            ((Grid)sender).DataSource = null;
            ((Grid)sender).DataSource = this.currentRoundDetails;
            ((Grid)sender).Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (this.statType != StatType.Shows && ((Grid)sender).SelectedCells.Count > 0) {
                if (((Grid)sender).SelectedRows.Count == 1) {
                    RoundInfo info = ((Grid)sender).Rows[((DataGridView)sender).SelectedRows[0].Index].DataBoundItem as RoundInfo;
                    if (info.UseShareCode || (LevelStats.ALL.TryGetValue(info.Name, out LevelStats levelStats) && levelStats.IsCreative && !string.IsNullOrEmpty(levelStats.ShareCode))) {
                        if (((Grid)sender).MenuSeparator != null && !((Grid)sender).CMenu.Items.Contains(((Grid)sender).MenuSeparator)) {
                            ((Grid)sender).CMenu.Items.Add(((Grid)sender).MenuSeparator);
                        }
                        if (((Grid)sender).UpdateCreativeLevel != null && !((Grid)sender).CMenu.Items.Contains(((Grid)sender).UpdateCreativeLevel)) {
                            ((Grid)sender).CMenu.Items.Add(((Grid)sender).UpdateCreativeLevel);
                        }
                    } else {
                        ((Grid)sender).ClearSelection();
                        if (((Grid)sender).MenuSeparator != null && ((Grid)sender).CMenu.Items.Contains(((Grid)sender).MenuSeparator)) {
                            ((Grid)sender).CMenu.Items.Remove(((Grid)sender).MenuSeparator);
                        }
                        if (((Grid)sender).UpdateCreativeLevel != null && ((Grid)sender).CMenu.Items.Contains(((Grid)sender).UpdateCreativeLevel)) {
                            ((Grid)sender).CMenu.Items.Remove(((Grid)sender).UpdateCreativeLevel);
                        }
                    }
                } else {
                    ((Grid)sender).ClearSelection();
                }
            } else if (this.statType == StatType.Shows) {
                if (((Grid)sender).SelectedCells.Count > 0) {
                    if (((Grid)sender).MenuSeparator != null && !((Grid)sender).CMenu.Items.Contains(((Grid)sender).MenuSeparator)) {
                        ((Grid)sender).CMenu.Items.Add(((Grid)sender).MenuSeparator);
                    }
                    if (((Grid)sender).MoveShows != null && !((Grid)sender).CMenu.Items.Contains(((Grid)sender).MoveShows)) {
                        ((Grid)sender).CMenu.Items.Add(((Grid)sender).MoveShows);
                    }
                    if (((Grid)sender).DeleteShows != null && !((Grid)sender).CMenu.Items.Contains(((Grid)sender).DeleteShows)) {
                        ((Grid)sender).CMenu.Items.Add(((Grid)sender).DeleteShows);
                    }
                } else {
                    if (((Grid)sender).MenuSeparator != null && ((Grid)sender).CMenu.Items.Contains(((Grid)sender).MenuSeparator)) {
                        ((Grid)sender).CMenu.Items.Remove(((Grid)sender).MenuSeparator);
                    }
                    if (((Grid)sender).MoveShows != null && ((Grid)sender).CMenu.Items.Contains(((Grid)sender).MoveShows)) {
                        ((Grid)sender).CMenu.Items.Remove(((Grid)sender).MoveShows);
                    }
                    if (((Grid)sender).DeleteShows != null && ((Grid)sender).CMenu.Items.Contains(((Grid)sender).DeleteShows)) {
                        ((Grid)sender).CMenu.Items.Remove(((Grid)sender).DeleteShows);
                    }
                }
            }
        }
        
        private void LevelDetails_KeyDown(object sender, KeyEventArgs e) {
            try {
                if (this.statType == StatType.Shows && e.KeyCode == Keys.Delete) {
                    this.DeleteShow();
                }
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.Message, $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Escape) {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
                    this.gridDetails.Enabled = false;
                    this.spinnerTransition.Start();
                    this.mpsSpinner01.Visible = true;
                    int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                    Task.Run(() => {
                        lock (this.StatsForm.StatsDB) {
                            this.StatsForm.StatsDB.BeginTrans();
                            foreach (DataGridViewRow row in this.gridDetails.SelectedRows) {
                                RoundInfo bi = row.DataBoundItem as RoundInfo;
                                this.RoundDetails.Remove(bi);
                                this.StatsForm.AllStats.RemoveAll(r => r.ShowID == bi.ShowID);
                                this.StatsForm.RoundDetails.DeleteMany(r => r.ShowID == bi.ShowID);
                            }
                            this.StatsForm.StatsDB.Commit();
                        }
                    }).ContinueWith(prevTask => {
                        this.BeginInvoke((MethodInvoker)delegate {
                            this.EnablePagingUI(false);
                            this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                            if (this.currentPage > this.totalPages) {
                                this.currentPage = this.totalPages;
                            }
                            this.SetPagingDisplay(true);
                            this.gridDetails.Enabled = true;
                            this.spinnerTransition.Stop();
                            this.mpsSpinner01.Visible = false;
                    
                            this.currentRoundDetails = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                            this.gridDetails.DataSource = null;
                            this.gridDetails.DataSource = this.currentRoundDetails;
                            if (this.gridDetails.RowCount > 0) {
                                this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.RowCount < minIndex ? this.gridDetails.RowCount - 1 : minIndex;
                            }

                            this.StatsForm.ResetStats();
                            Stats.IsOverlayRoundInfoNeedRefresh = true;
                        });
                    });
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
                        this.gridDetails.Enabled = false;
                        this.spinnerTransition.Start();
                        this.mpsSpinner01.Visible = true;
                        int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                        Task.Run(() => {
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
                        }).ContinueWith(prevTask => {
                            this.BeginInvoke((MethodInvoker)delegate {
                                this.EnablePagingUI(false);
                                this.totalPages = (int)Math.Ceiling(this.RoundDetails.Count / (float)this.pageSize);
                                if (this.currentPage > this.totalPages) {
                                    this.currentPage = this.totalPages;
                                }
                                this.SetPagingDisplay(true);
                                
                                this.gridDetails.Enabled = true;
                                this.spinnerTransition.Stop();
                                this.mpsSpinner01.Visible = false;
                        
                                this.currentRoundDetails = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                                this.gridDetails.DataSource = null;
                                this.gridDetails.DataSource = this.currentRoundDetails;
                                if (this.gridDetails.RowCount > 0) {
                                    this.gridDetails.FirstDisplayedScrollingRowIndex = this.gridDetails.RowCount < minIndex ? this.gridDetails.RowCount - 1 : minIndex;
                                }

                                this.StatsForm.ResetStats();
                                Stats.IsOverlayRoundInfoNeedRefresh = true;
                            });
                        });
                    }
                }
            }
        }
        
        private void updateLevel_Click(object sender, EventArgs e) {
            if (Utils.IsInternetConnected()) {
                if (this.statType != StatType.Shows && this.gridDetails.SelectedCells.Count > 0 && this.gridDetails.SelectedRows.Count == 1) {
                    RoundInfo ri = this.gridDetails.Rows[this.gridDetails.SelectedCells[0].RowIndex].DataBoundItem as RoundInfo;
                    if ((LevelStats.ALL.TryGetValue(ri.Name, out LevelStats l1) && l1.IsCreative && !string.IsNullOrEmpty(l1.ShareCode)) || ri.UseShareCode) {
                        string shareCode = ri.UseShareCode ? ri.Name : l1.ShareCode;
                        if (MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_prefix")}{(string.IsNullOrEmpty(ri.CreativeTitle) ? shareCode : ri.CreativeTitle)}{Multilingual.GetWord("message_update_creative_show_suffix")}", Multilingual.GetWord("message_update_creative_show_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            int minIndex = this.gridDetails.FirstDisplayedScrollingRowIndex;
                            this.gridDetails.Enabled = false;
                            this.spinnerTransition.Start();
                            this.mpsSpinner01.Visible = true;
                            Task.Run(() => {
                                try {
                                    JsonElement resData = Utils.GetApiData(Utils.FALLGUYSDB_API_URL, $"creative/{shareCode}.json");
                                    if (resData.TryGetProperty("data", out JsonElement je)) {
                                        JsonElement snapshot = je.GetProperty("snapshot");
                                        JsonElement versionMetadata = snapshot.GetProperty("version_metadata");
                                        JsonElement stats = snapshot.GetProperty("stats");
                                        
                                        List<RoundInfo> filteredInfo = ri.UseShareCode ? this.RoundDetails.FindAll(r => r.UseShareCode && string.Equals(r.Name, shareCode)) 
                                                                                       : this.RoundDetails.FindAll(r => string.Equals(r.Name, ri.Name));
                                        foreach (RoundInfo info in filteredInfo) {
                                            if (ri.UseShareCode) { info.ShowNameId = this.StatsForm.GetUserCreativeLevelTypeId(versionMetadata.GetProperty("game_mode_id").GetString()); }
                                            string[] onlinePlatformInfo = this.StatsForm.FindUserCreativeAuthor(snapshot.GetProperty("author").GetProperty("name_per_platform"));
                                            info.CreativeShareCode = snapshot.GetProperty("share_code").GetString();
                                            info.CreativeOnlinePlatformId = onlinePlatformInfo[0];
                                            info.CreativeAuthor = onlinePlatformInfo[1];
                                            info.CreativeVersion = versionMetadata.GetProperty("version").GetInt32();
                                            info.CreativeStatus = versionMetadata.GetProperty("status").GetString();
                                            info.CreativeTitle = versionMetadata.GetProperty("title").GetString();
                                            info.CreativeDescription = versionMetadata.GetProperty("description").GetString();
                                            if (versionMetadata.TryGetProperty("creator_tags", out JsonElement creatorTags) && creatorTags.ValueKind == JsonValueKind.Array) {
                                                string temps = string.Empty;
                                                foreach (JsonElement t in creatorTags.EnumerateArray()) {
                                                    if (!string.IsNullOrEmpty(temps)) { temps += ";"; }
                                                    temps += t.GetString();
                                                }
                                                info.CreativeCreatorTags = temps;
                                            }
                                            info.CreativeMaxPlayer = versionMetadata.GetProperty("max_player_count").GetInt32();
                                            info.CreativeThumbUrl = versionMetadata.GetProperty("thumb_url").GetString();
                                            info.CreativePlatformId = versionMetadata.GetProperty("platform_id").GetString();
                                            info.CreativeGameModeId = versionMetadata.GetProperty("game_mode_id").GetString();
                                            info.CreativeLevelThemeId = versionMetadata.GetProperty("level_theme_id").GetString();
                                            info.CreativeLastModifiedDate = versionMetadata.GetProperty("last_modified_date").GetDateTime();
                                            info.CreativePlayCount = stats.GetProperty("play_count").GetInt32();
                                            info.CreativeLikes = stats.GetProperty("likes").GetInt32();
                                            info.CreativeDislikes = stats.GetProperty("dislikes").GetInt32();
                                            info.CreativeQualificationPercent = versionMetadata.GetProperty("qualification_percent").GetInt32();
                                            info.CreativeTimeLimitSeconds = versionMetadata.GetProperty("config").TryGetProperty("time_limit_seconds", out JsonElement jeTimeLimitSeconds) ? jeTimeLimitSeconds.GetInt32() : 240;
                                        }
                                        
                                        lock (this.StatsForm.StatsDB) {
                                            this.StatsForm.StatsDB.BeginTrans();
                                            this.StatsForm.RoundDetails.Update(filteredInfo);
                                            this.StatsForm.StatsDB.Commit();
                                        }
                                        
                                        this.StatsForm.UpdateCreativeLevels(ri.Name, shareCode, snapshot);
                                    }
                                } catch {
                                    MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_creative_show_error")}", $"{Multilingual.GetWord("message_update_error_caption")}", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }).ContinueWith(prevTask => {
                                this.BeginInvoke((MethodInvoker)delegate {
                                    this.spinnerTransition.Stop();
                                    this.mpsSpinner01.Visible = false;
                                    this.gridDetails.Enabled = true;
                                    this.currentRoundDetails = this.RoundDetails.Skip((this.currentPage - 1) * this.pageSize).Take(this.pageSize).ToList();
                                    this.gridDetails.DataSource = null;
                                    this.gridDetails.DataSource = this.currentRoundDetails;
                                    this.gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                                });
                            });
                        }
                    }
                }
            } else {
                MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_check_internet_connection")}", $"{Multilingual.GetWord("message_check_internet_connection_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (this.statType == StatType.Shows || e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }
            if ((bool)((Grid)sender).Rows[e.RowIndex].Cells["UseShareCode"].Value) {
                string shareCode = (string)((Grid)sender).Rows[e.RowIndex].Cells["Name"].Value;
                Clipboard.SetText(shareCode, TextDataFormat.Text);
                this.StatsForm.AllocTooltip();
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 4, cursorPosition.Y - 20);
                this.StatsForm.ShowTooltip(Multilingual.GetWord("level_detail_share_code_copied"), this, position, 2000);
            } else if (LevelStats.ALL.TryGetValue((string)((Grid)sender).Rows[e.RowIndex].Cells["Name"].Value, out LevelStats levelStats) && levelStats.IsCreative && !string.IsNullOrEmpty(levelStats.ShareCode)) {
                string shareCode = levelStats.ShareCode;
                Clipboard.SetText(shareCode, TextDataFormat.Text);
                this.StatsForm.AllocTooltip();
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 4, cursorPosition.Y - 20);
                this.StatsForm.ShowTooltip(Multilingual.GetWord("level_detail_share_code_copied"), this, position, 2000);
            }
        }

        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (!this.isScrollingStopped || e.RowIndex < 0 || e.RowIndex >= ((Grid)sender).RowCount) { return; }

            if (this.statType == StatType.Shows
                || (bool)((Grid)sender).Rows[e.RowIndex].Cells["UseShareCode"].Value
                || LevelStats.ALL.TryGetValue((string)((Grid)sender).Rows[e.RowIndex].Cells["Name"].Value, out LevelStats l1) && l1.IsCreative && !string.IsNullOrEmpty(l1.ShareCode)) {
                ((Grid)sender).Cursor = Cursors.Hand;
            }
            
            if (this.statType != StatType.Shows && (((Grid)sender).Columns[e.ColumnIndex].Name == "Round" || ((Grid)sender).Columns[e.ColumnIndex].Name == "RoundIcon" || ((Grid)sender).Columns[e.ColumnIndex].Name == "Name" || ((Grid)sender).Columns[e.ColumnIndex].Name == "CreativeLikes" || ((Grid)sender).Columns[e.ColumnIndex].Name == "CreativeDislikes") &&
                ((bool)((Grid)sender).Rows[e.RowIndex].Cells["UseShareCode"].Value || !string.IsNullOrEmpty((string)((Grid)sender).Rows[e.RowIndex].Cells["CreativeShareCode"].Value))) {
                ((Grid)sender).ShowCellToolTips = false;
                RoundInfo info = ((Grid)sender).Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.CreativeLastModifiedDate == DateTime.MinValue) return;
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"⟦{this.GetLevelTypeName(info.CreativeGameModeId)}⟧ {info.CreativeTitle}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(info.CreativeDescription);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                if (string.IsNullOrEmpty(info.CreativeAuthor) || string.IsNullOrEmpty(info.CreativeOnlinePlatformId)) {
                    strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_author")} : N/A");
                } else {
                    string[] createAuthorArr = info.CreativeAuthor.Split(';');
                    string[] creativeOnlinePlatformIdArr = info.CreativeOnlinePlatformId.Split(';');
                    for (int i = 0; i < creativeOnlinePlatformIdArr.Length; i++) {
                        strBuilder.Append(i == 0 ? $"• {Multilingual.GetWord("level_detail_creative_author")} : ⟦{this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])}⟧ {createAuthorArr[i]}"
                            :$"{Environment.NewLine}\t   ⟦{this.GetCreativeOnlinePlatformName(creativeOnlinePlatformIdArr[i])}⟧ {createAuthorArr[i]}");
                    }
                }
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {(Stats.InstalledEmojiFont ? "👍 " : "")}{Multilingual.GetWord("level_detail_creative_likes")} {info.CreativeLikes:N0}\t      • {(Stats.InstalledEmojiFont ? "👎 " : "")}{Multilingual.GetWord("level_detail_creative_dislikes")} {info.CreativeDislikes:N0}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_share_code")} : {info.CreativeShareCode}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_version")} : v{info.CreativeVersion}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_max_players")} : {info.CreativeMaxPlayer}{Multilingual.GetWord("level_detail_creative_player_suffix")}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_time_limit")} : {TimeSpan.FromSeconds(info.CreativeTimeLimitSeconds > 0 ? info.CreativeTimeLimitSeconds : 240):m\\:ss}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_platform")} : {this.GetCreativePlatformName(info.CreativePlatformId)}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_last_modified")} : {info.CreativeLastModifiedDate.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"• {Multilingual.GetWord("level_detail_creative_play_count")} : {info.CreativePlayCount:N0}{Multilingual.GetWord("level_detail_creative_inning")}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"# {(Stats.InstalledEmojiFont ? "📋 " : "")}{Multilingual.GetWord("level_detail_share_code_copied_tooltip")}");

                this.StatsForm.AllocCustomTooltip(this.StatsForm.cmtt_levelDetails_Draw);
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 50, cursorPosition.Y);
                this.StatsForm.ShowCustomTooltip(strBuilder.ToString(), this, position);
            } else if (this.statType == StatType.Shows && ((Grid)sender).Columns[e.ColumnIndex].Name == "ShowNameId") {
                ((Grid)sender).ShowCellToolTips = false;
                RoundInfo info = ((Grid)sender).Rows[e.RowIndex].DataBoundItem as RoundInfo;
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(info.StartLocal.ToString(Multilingual.GetWord("level_grid_date_format"), Utils.GetCultureInfo()));
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($" ⟦ {info.ShowID}{(!string.IsNullOrEmpty(Multilingual.GetShowName(info.ShowNameId)) ? $" {Multilingual.GetShowName(info.ShowNameId)} ⟧" : " ⟧")}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(Environment.NewLine);
                
                string[] s = info.Name.Split(';');
                for (int i = 0; i < s.Length; i++) {
                    string name = s[i];
                    string type = string.Empty;
                    if (this.StatsForm.StatLookup.TryGetValue(info.UseShareCode ? info.ShowNameId : name, out LevelStats levelStats)) {
                        type = $"⟦{levelStats.Type.LevelTitle(false)}⟧ ";
                    }
                    strBuilder.Append($"• {Multilingual.GetWord("overlay_round_prefix")}{i + 1}{Multilingual.GetWord("overlay_round_suffix")} : {type}{Multilingual.GetRoundName(name)}");
                    if (i != s.Length - 1) strBuilder.Append(Environment.NewLine);
                }

                this.StatsForm.AllocCustomTooltip(this.StatsForm.cmtt_levelDetails_Draw2);
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 40, cursorPosition.Y);
                this.StatsForm.ShowCustomTooltip(strBuilder.ToString(), this, position);
            } else {
                ((Grid)sender).ShowCellToolTips = true;
            }
        }

        private void gridDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            this.StatsForm.HideCustomTooltip(this);
            ((Grid)sender).Cursor = Cursors.Default;
        }

        private string GetLevelTypeName(string gameModeId) {
            switch (gameModeId) {
                case "GAMEMODE_GAUNTLET": return Multilingual.GetWord("level_detail_race");
                case "GAMEMODE_SURVIVAL": return Multilingual.GetWord("level_detail_survival");
                case "GAMEMODE_HUNT": return Multilingual.GetWord("level_detail_hunt");
                case "GAMEMODE_LOGIC": return Multilingual.GetWord("level_detail_logic");
                case "GAMEMODE_TEAM": return Multilingual.GetWord("level_detail_team");
                default: return Multilingual.GetWord("level_detail_race");
            }
        }

        private string GetCreativeOnlinePlatformName(string platform) {
            switch (platform) {
                case "win": return Multilingual.GetWord("level_detail_playersPc");
                case "eos": return Multilingual.GetWord("level_detail_online_platform_eos");
                case "steam": return Multilingual.GetWord("level_detail_online_platform_steam");
                case "psn": return Multilingual.GetWord("level_detail_online_platform_psn");
                case "xbl": return Multilingual.GetWord("level_detail_online_platform_xbl");
                case "nso":
                case "nintendo":
                    return Multilingual.GetWord("level_detail_online_platform_nso");
                default: return platform;
            }
        }

        private string GetCreativePlatformName(string platform) {
            switch (platform) {
                case "ps4": return Multilingual.GetWord("level_detail_playersPs4");
                case "ps5": return Multilingual.GetWord("level_detail_playersPs5");
                case "xb1": return Multilingual.GetWord("level_detail_playersXb1");
                case "xsx": return Multilingual.GetWord("level_detail_playersXsx");
                case "switch": return Multilingual.GetWord("level_detail_playersSw");
                case "win": return Multilingual.GetWord("level_detail_playersPc");
                default: return platform;
            }
        }
    }
}