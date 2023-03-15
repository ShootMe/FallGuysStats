using System.Windows.Forms;

namespace FallGuysStats {
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.resources = new System.ComponentModel.ComponentResourceManager(typeof(Stats));
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
            this.menuEditProfiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOverlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLaunchFallGuys = new System.Windows.Forms.ToolStripMenuItem();
            this.infoStrip = new System.Windows.Forms.ToolStrip();
            this.lblCurrentProfile = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalTime = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalShows = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalRounds = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalWins = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalFinals = new System.Windows.Forms.ToolStripLabel();
            this.lblKudos = new System.Windows.Forms.ToolStripLabel();
            this.gridDetails = new FallGuysStats.Grid();
            this.menu.SuspendLayout();
            this.infoStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
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
            this.menuSettings.Image = global::FallGuysStats.Properties.Resources.setting_icon;
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSettings.Size = new System.Drawing.Size(78, 15);
            this.menuSettings.Text = "Settings";
            this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
            // 
            // menuFilters
            // 
            this.menuFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuStatsFilter, this.menuPartyFilter });
            this.menuFilters.Image = global::FallGuysStats.Properties.Resources.filter_icon;
            this.menuFilters.Name = "menuFilters";
            this.menuFilters.Size = new System.Drawing.Size(66, 15);
            this.menuFilters.Text = "Filters";
            // 
            // menuStatsFilter
            // 
            this.menuStatsFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuAllStats, this.menuSeasonStats, this.menuWeekStats, this.menuDayStats, this.menuSessionStats });
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
            this.menuAllStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.A)));
            this.menuAllStats.Size = new System.Drawing.Size(189, 22);
            this.menuAllStats.Text = "All";
            this.menuAllStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSeasonStats
            // 
            this.menuSeasonStats.CheckOnClick = true;
            this.menuSeasonStats.Name = "menuSeasonStats";
            this.menuSeasonStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.S)));
            this.menuSeasonStats.Size = new System.Drawing.Size(189, 22);
            this.menuSeasonStats.Text = "Season";
            this.menuSeasonStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuWeekStats
            // 
            this.menuWeekStats.CheckOnClick = true;
            this.menuWeekStats.Name = "menuWeekStats";
            this.menuWeekStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.W)));
            this.menuWeekStats.Size = new System.Drawing.Size(189, 22);
            this.menuWeekStats.Text = "Week";
            this.menuWeekStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuDayStats
            // 
            this.menuDayStats.CheckOnClick = true;
            this.menuDayStats.Name = "menuDayStats";
            this.menuDayStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.D)));
            this.menuDayStats.Size = new System.Drawing.Size(189, 22);
            this.menuDayStats.Text = "Day";
            this.menuDayStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSessionStats
            // 
            this.menuSessionStats.CheckOnClick = true;
            this.menuSessionStats.Name = "menuSessionStats";
            this.menuSessionStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.G)));
            this.menuSessionStats.Size = new System.Drawing.Size(189, 22);
            this.menuSessionStats.Text = "Session";
            this.menuSessionStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuPartyFilter
            // 
            this.menuPartyFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuAllPartyStats, this.menuSoloStats, this.menuPartyStats });
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
            this.menuAllPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.F)));
            this.menuAllPartyStats.Size = new System.Drawing.Size(175, 22);
            this.menuAllPartyStats.Text = "All";
            this.menuAllPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuSoloStats
            // 
            this.menuSoloStats.CheckOnClick = true;
            this.menuSoloStats.Name = "menuSoloStats";
            this.menuSoloStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.O)));
            this.menuSoloStats.Size = new System.Drawing.Size(175, 22);
            this.menuSoloStats.Text = "Solo";
            this.menuSoloStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuPartyStats
            // 
            this.menuPartyStats.CheckOnClick = true;
            this.menuPartyStats.Name = "menuPartyStats";
            this.menuPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.P)));
            this.menuPartyStats.Size = new System.Drawing.Size(175, 22);
            this.menuPartyStats.Text = "Party";
            this.menuPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // menuProfile
            // 
            this.menuProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuEditProfiles });
            this.menuProfile.Image = global::FallGuysStats.Properties.Resources.profile_icon;
            this.menuProfile.Name = "menuProfile";
            this.menuProfile.Size = new System.Drawing.Size(69, 15);
            this.menuProfile.Text = "Profile";
            // 
            // menuEditProfiles
            // 
            this.menuEditProfiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.menuEditProfiles.Image = global::FallGuysStats.Properties.Resources.setting_icon;
            this.menuEditProfiles.Name = "menuEditProfiles";
            this.menuEditProfiles.Size = new System.Drawing.Size(155, 22);
            this.menuEditProfiles.Text = "Profile Settings";
            this.menuEditProfiles.Click += new System.EventHandler(this.menuEditProfiles_Click);
            // 
            // menuOverlay
            // 
            this.menuOverlay.Image = global::FallGuysStats.Properties.Resources.stat_gray_icon;
            this.menuOverlay.Name = "menuOverlay";
            this.menuOverlay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOverlay.Size = new System.Drawing.Size(109, 15);
            this.menuOverlay.Text = "Show Overlay";
            this.menuOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuUpdate
            // 
            this.menuUpdate.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.menuUpdate.Name = "menuUpdate";
            this.menuUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuUpdate.Size = new System.Drawing.Size(73, 15);
            this.menuUpdate.Text = "Update";
            this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuHelp.Size = new System.Drawing.Size(60, 15);
            this.menuHelp.Text = "Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // menuLaunchFallGuys
            // 
            this.menuLaunchFallGuys.Image = global::FallGuysStats.Properties.Resources.fallguys_icon;
            this.menuLaunchFallGuys.Name = "menuLaunchFallGuys";
            this.menuLaunchFallGuys.Size = new System.Drawing.Size(126, 15);
            this.menuLaunchFallGuys.Text = "Launch Fall Guys";
            this.menuLaunchFallGuys.Click += new System.EventHandler(this.menuLaunchFallGuys_Click);
            // 
            // infoStrip
            // 
            this.infoStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.infoStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblCurrentProfile,
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
            this.infoStrip.Size = new System.Drawing.Size(770, 23);
            this.infoStrip.TabIndex = 13;
            // 
            // lblCurrentProfile
            // 
            this.lblCurrentProfile.ForeColor = System.Drawing.Color.Crimson;
            this.lblCurrentProfile.Image = global::FallGuysStats.Properties.Resources.profile2_icon;
            this.lblCurrentProfile.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.lblCurrentProfile.Name = "lblCurrentProfile";
            this.lblCurrentProfile.Size = new System.Drawing.Size(54, 16);
            this.lblCurrentProfile.Text = ": Solo";
            this.lblCurrentProfile.ToolTipText = "Click to change your current profile.";
            this.lblCurrentProfile.Click += new System.EventHandler(this.lblCurrentProfile_Click);
            this.lblCurrentProfile.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblCurrentProfile.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.Image = global::FallGuysStats.Properties.Resources.clock_icon;
            this.lblTotalTime.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(71, 16);
            this.lblTotalTime.Text = ": 0:00:00";
            // 
            // lblTotalShows
            // 
            this.lblTotalShows.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalShows.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalShows.Name = "lblTotalShows";
            this.lblTotalShows.Size = new System.Drawing.Size(60, 16);
            this.lblTotalShows.Text = "Shows : 0";
            this.lblTotalShows.ToolTipText = "Click to view shows stats.";
            this.lblTotalShows.Click += new System.EventHandler(this.lblTotalShows_Click);
            this.lblTotalShows.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalShows.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalRounds
            // 
            this.lblTotalRounds.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalRounds.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalRounds.Name = "lblTotalRounds";
            this.lblTotalRounds.Size = new System.Drawing.Size(65, 16);
            this.lblTotalRounds.Text = "Rounds : 0";
            this.lblTotalRounds.ToolTipText = "Click to view rounds stats.";
            this.lblTotalRounds.Click += new System.EventHandler(this.lblTotalRounds_Click);
            this.lblTotalRounds.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalRounds.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalWins
            // 
            this.lblTotalWins.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalWins.Image = global::FallGuysStats.Properties.Resources.crown_icon;
            this.lblTotalWins.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalWins.Name = "lblTotalWins";
            this.lblTotalWins.Size = new System.Drawing.Size(76, 16);
            this.lblTotalWins.Text = ": 0 (0.0%)";
            this.lblTotalWins.ToolTipText = "Click to view wins stats.";
            this.lblTotalWins.Click += new System.EventHandler(this.lblTotalWins_Click);
            this.lblTotalWins.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalWins.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblTotalFinals
            // 
            this.lblTotalFinals.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalFinals.Image = global::FallGuysStats.Properties.Resources.final_icon;
            this.lblTotalFinals.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblTotalFinals.Name = "lblTotalFinals";
            this.lblTotalFinals.Size = new System.Drawing.Size(76, 16);
            this.lblTotalFinals.Text = ": 0 (0.0%)";
            this.lblTotalFinals.ToolTipText = "Click to view finals stats.";
            this.lblTotalFinals.Click += new System.EventHandler(this.lblTotalFinals_Click);
            this.lblTotalFinals.MouseEnter += new System.EventHandler(this.infoStrip_MouseEnter);
            this.lblTotalFinals.MouseLeave += new System.EventHandler(this.infoStrip_MouseLeave);
            // 
            // lblKudos
            // 
            this.lblKudos.Image = global::FallGuysStats.Properties.Resources.kudos_icon;
            this.lblKudos.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.lblKudos.Name = "lblKudos";
            this.lblKudos.Size = new System.Drawing.Size(37, 16);
            this.lblKudos.Text = ": 0";
            // 
            // gridDetails
            // 
            this.gridDetails.AllowUserToDeleteRows = false;
            this.gridDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            this.dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            this.dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeight = 20;
            this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
            this.dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;
            this.gridDetails.EnableHeadersVisualStyles = false;
            this.gridDetails.GridColor = System.Drawing.Color.Gray;
            this.gridDetails.Location = new System.Drawing.Point(0, 72);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.ReadOnly = true;
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.Size = new System.Drawing.Size(828, 548);
            this.gridDetails.TabIndex = 11;
            this.gridDetails.TabStop = false;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            this.gridDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDetails_ColumnHeaderMouseClick);
            this.gridDetails.SelectionChanged += new System.EventHandler(this.gridDetails_SelectionChanged);
            // 
            // Stats
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(830, 620);
            this.Controls.Add(this.infoStrip);
            this.Controls.Add(this.gridDetails);
            this.Controls.Add(this.menu);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.MinimumSize = new System.Drawing.Size(830, 400);
            this.Name = "Stats";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
            this.Load += new System.EventHandler(this.Stats_Load);
            this.Shown += new System.EventHandler(this.Stats_Shown);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.infoStrip.ResumeLayout(false);
            this.infoStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.ComponentResourceManager resources;
        private FallGuysStats.Grid gridDetails;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2;
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
        private System.Windows.Forms.ToolStrip infoStrip;
        private System.Windows.Forms.ToolStripLabel lblCurrentProfile;
        private System.Windows.Forms.ToolStripLabel lblTotalTime;
        private System.Windows.Forms.ToolStripLabel lblTotalShows;
        private System.Windows.Forms.ToolStripLabel lblTotalRounds;
        private System.Windows.Forms.ToolStripLabel lblTotalWins;
        private System.Windows.Forms.ToolStripLabel lblTotalFinals;
        private System.Windows.Forms.ToolStripLabel lblKudos;
        private System.Windows.Forms.ToolStripMenuItem menuLaunchFallGuys;
        private System.Windows.Forms.ToolStripMenuItem menuEditProfiles;
    }
}

