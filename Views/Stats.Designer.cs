﻿namespace FallGuysStats {
    partial class Stats {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stats));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStatsFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeasonStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWeekStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDayStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSessionStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPartyFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllPartyStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSoloStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPartyStats = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProfileMain = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProfilePractice = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOverlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLaunchFallGuys = new System.Windows.Forms.ToolStripMenuItem();
            this.gridDetails = new FallGuysStats.Grid();
            this.infoStrip = new System.Windows.Forms.ToolStrip();
            this.lblTotalTime = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalShows = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalRounds = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalWins = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalFinals = new System.Windows.Forms.ToolStripLabel();
            this.lblKudos = new System.Windows.Forms.ToolStripLabel();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
            this.infoStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSettings,
            this.menuFilters,
            this.menuProfile,
            this.menuOverlay,
            this.menuUpdate,
            this.menuHelp,
            this.menuLaunchFallGuys});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(634, 24);
            this.menu.TabIndex = 12;
            this.menu.Text = "menuStrip1";
            // 
            // menuSettings
            // 
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSettings.Size = new System.Drawing.Size(61, 20);
            this.menuSettings.Text = "Settings";
            this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
            // 
            // menuFilters
            // 
            this.menuFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStatsFilter,
            this.menuPartyFilter});
            this.menuFilters.Name = "menuFilters";
            this.menuFilters.Size = new System.Drawing.Size(50, 20);
            this.menuFilters.Text = "Filters";
            // 
            // menuStatsFilter
            // 
            this.menuStatsFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAllStats,
            this.menuSeasonStats,
            this.menuWeekStats,
            this.menuDayStats,
            this.menuSessionStats});
            this.menuStatsFilter.Name = "menuStatsFilter";
            this.menuStatsFilter.Size = new System.Drawing.Size(101, 22);
            this.menuStatsFilter.Text = "Stats";
            // 
            // menuAllStats
            // 
            this.menuAllStats.Checked = true;
            this.menuAllStats.CheckOnClick = true;
            this.menuAllStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuAllStats.Name = "menuAllStats";
            this.menuAllStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.menuAllStats.Size = new System.Drawing.Size(187, 22);
            this.menuAllStats.Text = "All";
            this.menuAllStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSeasonStats
            // 
            this.menuSeasonStats.CheckOnClick = true;
            this.menuSeasonStats.Name = "menuSeasonStats";
            this.menuSeasonStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.menuSeasonStats.Size = new System.Drawing.Size(187, 22);
            this.menuSeasonStats.Text = "Season";
            this.menuSeasonStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuWeekStats
            // 
            this.menuWeekStats.CheckOnClick = true;
            this.menuWeekStats.Name = "menuWeekStats";
            this.menuWeekStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.menuWeekStats.Size = new System.Drawing.Size(187, 22);
            this.menuWeekStats.Text = "Week";
            this.menuWeekStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuDayStats
            // 
            this.menuDayStats.CheckOnClick = true;
            this.menuDayStats.Name = "menuDayStats";
            this.menuDayStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.menuDayStats.Size = new System.Drawing.Size(187, 22);
            this.menuDayStats.Text = "Day";
            this.menuDayStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSessionStats
            // 
            this.menuSessionStats.CheckOnClick = true;
            this.menuSessionStats.Name = "menuSessionStats";
            this.menuSessionStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.menuSessionStats.Size = new System.Drawing.Size(187, 22);
            this.menuSessionStats.Text = "Session";
            this.menuSessionStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuPartyFilter
            // 
            this.menuPartyFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAllPartyStats,
            this.menuSoloStats,
            this.menuPartyStats});
            this.menuPartyFilter.Name = "menuPartyFilter";
            this.menuPartyFilter.Size = new System.Drawing.Size(101, 22);
            this.menuPartyFilter.Text = "Party";
            // 
            // menuAllPartyStats
            // 
            this.menuAllPartyStats.Checked = true;
            this.menuAllPartyStats.CheckOnClick = true;
            this.menuAllPartyStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuAllPartyStats.Name = "menuAllPartyStats";
            this.menuAllPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.menuAllPartyStats.Size = new System.Drawing.Size(174, 22);
            this.menuAllPartyStats.Text = "All";
            this.menuAllPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSoloStats
            // 
            this.menuSoloStats.CheckOnClick = true;
            this.menuSoloStats.Name = "menuSoloStats";
            this.menuSoloStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.menuSoloStats.Size = new System.Drawing.Size(174, 22);
            this.menuSoloStats.Text = "Solo";
            this.menuSoloStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuPartyStats
            // 
            this.menuPartyStats.CheckOnClick = true;
            this.menuPartyStats.Name = "menuPartyStats";
            this.menuPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.menuPartyStats.Size = new System.Drawing.Size(174, 22);
            this.menuPartyStats.Text = "Party";
            this.menuPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuProfile
            // 
            this.menuProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuProfileMain,
            this.menuProfilePractice});
            this.menuProfile.Name = "menuProfile";
            this.menuProfile.Size = new System.Drawing.Size(53, 20);
            this.menuProfile.Text = "Profile";
            // 
            // menuProfileMain
            // 
            this.menuProfileMain.Checked = true;
            this.menuProfileMain.CheckOnClick = true;
            this.menuProfileMain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuProfileMain.Name = "menuProfileMain";
            this.menuProfileMain.Size = new System.Drawing.Size(116, 22);
            this.menuProfileMain.Text = "Main";
            this.menuProfileMain.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuProfilePractice
            // 
            this.menuProfilePractice.CheckOnClick = true;
            this.menuProfilePractice.Name = "menuProfilePractice";
            this.menuProfilePractice.Size = new System.Drawing.Size(116, 22);
            this.menuProfilePractice.Text = "Practice";
            this.menuProfilePractice.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuOverlay
            // 
            this.menuOverlay.Name = "menuOverlay";
            this.menuOverlay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOverlay.Size = new System.Drawing.Size(59, 20);
            this.menuOverlay.Text = "Overlay";
            this.menuOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuUpdate
            // 
            this.menuUpdate.Name = "menuUpdate";
            this.menuUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuUpdate.Size = new System.Drawing.Size(57, 20);
            this.menuUpdate.Text = "Update";
            this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuHelp.Size = new System.Drawing.Size(44, 20);
            this.menuHelp.Text = "Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // menuLaunchFallGuys
            // 
            this.menuLaunchFallGuys.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuLaunchFallGuys.Name = "menuLaunchFallGuys";
            this.menuLaunchFallGuys.Size = new System.Drawing.Size(109, 20);
            this.menuLaunchFallGuys.Text = "Launch Fall Guys";
            this.menuLaunchFallGuys.Click += new System.EventHandler(this.menuLaunchFallGuys_Click);
            // 
            // gridDetails
            // 
            this.gridDetails.AllowUserToDeleteRows = false;
            this.gridDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.gridDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeight = 20;
            this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridDetails.EnableHeadersVisualStyles = false;
            this.gridDetails.GridColor = System.Drawing.Color.Gray;
            this.gridDetails.Location = new System.Drawing.Point(0, 50);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.ReadOnly = true;
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.Size = new System.Drawing.Size(634, 570);
            this.gridDetails.TabIndex = 11;
            this.gridDetails.TabStop = false;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            this.gridDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDetails_ColumnHeaderMouseClick);
            this.gridDetails.SelectionChanged += new System.EventHandler(this.gridDetails_SelectionChanged);
            // 
            // infoStrip
            // 
            this.infoStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.infoStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTotalTime,
            this.lblTotalShows,
            this.lblTotalRounds,
            this.lblTotalWins,
            this.lblTotalFinals,
            this.lblKudos});
            this.infoStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.infoStrip.Location = new System.Drawing.Point(0, 24);
            this.infoStrip.Name = "infoStrip";
            this.infoStrip.Padding = new System.Windows.Forms.Padding(4, 6, 1, 1);
            this.infoStrip.Size = new System.Drawing.Size(634, 26);
            this.infoStrip.TabIndex = 13;
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(107, 13);
            this.lblTotalTime.Text = "Time Played: 0:00:00";
            // 
            // lblTotalShows
            // 
            this.lblTotalShows.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalShows.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalShows.Name = "lblTotalShows";
            this.lblTotalShows.Size = new System.Drawing.Size(51, 13);
            this.lblTotalShows.Text = "Shows: 0";
            this.lblTotalShows.Click += new System.EventHandler(this.lblTotalShows_Click);
            this.lblTotalShows.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalShows.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalRounds
            // 
            this.lblTotalRounds.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalRounds.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalRounds.Name = "lblTotalRounds";
            this.lblTotalRounds.Size = new System.Drawing.Size(56, 13);
            this.lblTotalRounds.Text = "Rounds: 0";
            this.lblTotalRounds.Click += new System.EventHandler(this.lblTotalRounds_Click);
            this.lblTotalRounds.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalRounds.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalWins
            // 
            this.lblTotalWins.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalWins.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalWins.Name = "lblTotalWins";
            this.lblTotalWins.Size = new System.Drawing.Size(78, 13);
            this.lblTotalWins.Text = "Wins: 0 (0.0 %)";
            this.lblTotalWins.Click += new System.EventHandler(this.lblTotalWins_Click);
            this.lblTotalWins.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalWins.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalFinals
            // 
            this.lblTotalFinals.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalFinals.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalFinals.Name = "lblTotalFinals";
            this.lblTotalFinals.Size = new System.Drawing.Size(81, 13);
            this.lblTotalFinals.Text = "Finals: 0 (0.0 %)";
            this.lblTotalFinals.Click += new System.EventHandler(this.lblTotalFinals_Click);
            this.lblTotalFinals.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalFinals.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblKudos
            // 
            this.lblKudos.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblKudos.Name = "lblKudos";
            this.lblKudos.Size = new System.Drawing.Size(49, 13);
            this.lblKudos.Text = "Kudos: 0";
            // 
            // Stats
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(634, 620);
            this.Controls.Add(this.infoStrip);
            this.Controls.Add(this.gridDetails);
            this.Controls.Add(this.menu);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.MinimumSize = new System.Drawing.Size(650, 300);
            this.Name = "Stats";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fall Guys Stats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
            this.Load += new System.EventHandler(this.Stats_Load);
            this.Shown += new System.EventHandler(this.Stats_Shown);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
            this.infoStrip.ResumeLayout(false);
            this.infoStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Grid gridDetails;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuFilters;
        private System.Windows.Forms.ToolStripMenuItem menuStatsFilter;
        private System.Windows.Forms.ToolStripMenuItem menuAllStats;
        private System.Windows.Forms.ToolStripMenuItem menuSeasonStats;
        private System.Windows.Forms.ToolStripMenuItem menuWeekStats;
        private System.Windows.Forms.ToolStripMenuItem menuDayStats;
        private System.Windows.Forms.ToolStripMenuItem menuSessionStats;
        private System.Windows.Forms.ToolStripMenuItem menuPartyFilter;
        private System.Windows.Forms.ToolStripMenuItem menuAllPartyStats;
        private System.Windows.Forms.ToolStripMenuItem menuSoloStats;
        private System.Windows.Forms.ToolStripMenuItem menuPartyStats;
        private System.Windows.Forms.ToolStripMenuItem menuOverlay;
        private System.Windows.Forms.ToolStripMenuItem menuUpdate;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuProfile;
        private System.Windows.Forms.ToolStripMenuItem menuProfileMain;
        private System.Windows.Forms.ToolStripMenuItem menuProfilePractice;
        private System.Windows.Forms.ToolStrip infoStrip;
        private System.Windows.Forms.ToolStripLabel lblTotalTime;
        private System.Windows.Forms.ToolStripLabel lblTotalShows;
        private System.Windows.Forms.ToolStripLabel lblTotalRounds;
        private System.Windows.Forms.ToolStripLabel lblTotalWins;
        private System.Windows.Forms.ToolStripLabel lblTotalFinals;
        private System.Windows.Forms.ToolStripLabel lblKudos;
        private System.Windows.Forms.ToolStripMenuItem menuLaunchFallGuys;
    }
}

