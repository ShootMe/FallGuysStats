using System;
using System.Windows.Controls;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayCMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayOverlay = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.traySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.trayFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.trayStatsFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.trayAllStats = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeasonStats = new System.Windows.Forms.ToolStripMenuItem();
            this.trayWeekStats = new System.Windows.Forms.ToolStripMenuItem();
            this.trayDayStats = new System.Windows.Forms.ToolStripMenuItem();
            this.traySessionStats = new System.Windows.Forms.ToolStripMenuItem();
            this.trayPartyFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.trayAllPartyStats = new System.Windows.Forms.ToolStripMenuItem();
            this.traySoloStats = new System.Windows.Forms.ToolStripMenuItem();
            this.trayPartyStats = new System.Windows.Forms.ToolStripMenuItem();
            this.trayProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.trayEditProfiles = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.trayUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.trayHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.trayLaunchFallGuys = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.trayExitProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.infoStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
            this.trayCMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.menu.AutoSize = false;
            this.menu.BackColor = System.Drawing.Color.Transparent;
            this.menu.Dock = System.Windows.Forms.DockStyle.None;
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuSettings, this.menuFilters, this.menuProfile, this.menuOverlay, this.menuUpdate, this.menuHelp, this.menuLaunchFallGuys });
            this.menu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.menu.Location = new System.Drawing.Point(0, 65);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(828, 24);
            this.menu.TabIndex = 12;
            this.menu.Text = "menuStrip1";
            // 
            // menuSettings
            // 
            this.menuSettings.Image = global::FallGuysStats.Properties.Resources.setting_icon;
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSettings.Size = new System.Drawing.Size(78, 20);
            this.menuSettings.Text = "Settings";
            this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
            // 
            // menuFilters
            // 
            this.menuFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuStatsFilter, this.menuPartyFilter });
            this.menuFilters.Image = global::FallGuysStats.Properties.Resources.filter_icon;
            this.menuFilters.Name = "menuFilters";
            this.menuFilters.Size = new System.Drawing.Size(66, 20);
            this.menuFilters.Text = "Filters";
            // 
            // menuStatsFilter
            // 
            this.menuStatsFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuAllStats, this.menuSeasonStats, this.menuWeekStats, this.menuDayStats, this.menuSessionStats });
            this.menuStatsFilter.Image = global::FallGuysStats.Properties.Resources.stat_icon;
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
            this.menuPartyFilter.Image = global::FallGuysStats.Properties.Resources.player_icon;
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
            this.menuProfile.Size = new System.Drawing.Size(76, 20);
            this.menuProfile.Text = "Profiled";
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
            this.menuOverlay.Size = new System.Drawing.Size(109, 20);
            this.menuOverlay.Text = "Show Overlay";
            this.menuOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuUpdate
            // 
            this.menuUpdate.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.menuUpdate.Name = "menuUpdate";
            this.menuUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuUpdate.Size = new System.Drawing.Size(73, 20);
            this.menuUpdate.Text = "Update";
            this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuHelp.Size = new System.Drawing.Size(60, 20);
            this.menuHelp.Text = "Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // menuLaunchFallGuys
            // 
            this.menuLaunchFallGuys.Image = global::FallGuysStats.Properties.Resources.fallguys_icon;
            this.menuLaunchFallGuys.Name = "menuLaunchFallGuys";
            this.menuLaunchFallGuys.Size = new System.Drawing.Size(126, 20);
            this.menuLaunchFallGuys.Text = "Launch Fall Guys";
            this.menuLaunchFallGuys.Click += new System.EventHandler(this.menuLaunchFallGuys_Click);
            // 
            // infoStrip
            // 
            this.infoStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.infoStrip.AutoSize = false;
            this.infoStrip.BackColor = System.Drawing.Color.Transparent;
            this.infoStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.infoStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.infoStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblCurrentProfile, this.lblTotalTime, this.lblTotalShows, this.lblTotalRounds, this.lblTotalWins, this.lblTotalFinals, this.lblKudos });
            this.infoStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.infoStrip.Location = new System.Drawing.Point(20, 97);
            this.infoStrip.Name = "infoStrip";
            this.infoStrip.Padding = new System.Windows.Forms.Padding(0, 6, 0, 1);
            this.infoStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.infoStrip.Size = new System.Drawing.Size(790, 26);
            this.infoStrip.Stretch = true;
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
            this.lblTotalShows.Size = new System.Drawing.Size(60, 15);
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
            this.lblTotalRounds.Size = new System.Drawing.Size(65, 15);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridDetails.EnableHeadersVisualStyles = false;
            this.gridDetails.GridColor = System.Drawing.Color.Gray;
            this.gridDetails.Location = new System.Drawing.Point(20, 145);
            this.gridDetails.Margin = new System.Windows.Forms.Padding(0);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.ReadOnly = true;
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.Size = new System.Drawing.Size(790, 685);
            this.gridDetails.TabIndex = 11;
            this.gridDetails.TabStop = false;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            this.gridDetails.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseLeave);
            this.gridDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDetails_ColumnHeaderMouseClick);
            this.gridDetails.SelectionChanged += new System.EventHandler(this.gridDetails_SelectionChanged);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayCMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "trayIcon";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            this.trayIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseMove);
            // 
            // trayCMenu
            // 
            this.trayCMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.trayOverlay, this.traySeparator1, this.traySettings, this.traySeparator2, this.trayFilters, this.trayProfile, this.traySeparator3, this.trayUpdate, this.trayHelp, this.trayLaunchFallGuys, this.traySeparator4, this.trayExitProgram });
            this.trayCMenu.Name = "trayCMenu";
            this.trayCMenu.Size = new System.Drawing.Size(166, 204);
            // 
            // trayOverlay
            // 
            this.trayOverlay.Image = global::FallGuysStats.Properties.Resources.stat_gray_icon;
            this.trayOverlay.Name = "trayOverlay";
            this.trayOverlay.Size = new System.Drawing.Size(165, 22);
            this.trayOverlay.Text = "Show Overlay";
            this.trayOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // traySeparator1
            // 
            this.traySeparator1.Name = "traySeparator1";
            this.traySeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // traySettings
            // 
            this.traySettings.Image = global::FallGuysStats.Properties.Resources.setting_icon;
            this.traySettings.Name = "traySettings";
            this.traySettings.Size = new System.Drawing.Size(165, 22);
            this.traySettings.Text = "Settings";
            this.traySettings.Click += new System.EventHandler(this.menuSettings_Click);
            // 
            // traySeparator2
            // 
            this.traySeparator2.Name = "traySeparator2";
            this.traySeparator2.Size = new System.Drawing.Size(162, 6);
            // 
            // trayFilters
            // 
            this.trayFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.trayStatsFilter, this.trayPartyFilter });
            this.trayFilters.Image = global::FallGuysStats.Properties.Resources.filter_icon;
            this.trayFilters.Name = "trayFilters";
            this.trayFilters.Size = new System.Drawing.Size(165, 22);
            this.trayFilters.Text = "Filters";
            // 
            // trayStatsFilter
            // 
            this.trayStatsFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.trayAllStats, this.traySeasonStats, this.trayWeekStats, this.trayDayStats, this.traySessionStats });
            this.trayStatsFilter.Image = global::FallGuysStats.Properties.Resources.stat_icon;
            this.trayStatsFilter.Name = "trayStatsFilter";
            this.trayStatsFilter.Size = new System.Drawing.Size(101, 22);
            this.trayStatsFilter.Text = "Stats";
            // 
            // trayAllStats
            // 
            this.trayAllStats.Checked = true;
            this.trayAllStats.CheckOnClick = true;
            this.trayAllStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trayAllStats.Name = "trayAllStats";
            this.trayAllStats.Size = new System.Drawing.Size(114, 22);
            this.trayAllStats.Text = "All";
            this.trayAllStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // traySeasonStats
            // 
            this.traySeasonStats.CheckOnClick = true;
            this.traySeasonStats.Name = "traySeasonStats";
            this.traySeasonStats.Size = new System.Drawing.Size(114, 22);
            this.traySeasonStats.Text = "Season";
            this.traySeasonStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // trayWeekStats
            // 
            this.trayWeekStats.CheckOnClick = true;
            this.trayWeekStats.Name = "trayWeekStats";
            this.trayWeekStats.Size = new System.Drawing.Size(114, 22);
            this.trayWeekStats.Text = "Week";
            this.trayWeekStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // trayDayStats
            // 
            this.trayDayStats.CheckOnClick = true;
            this.trayDayStats.Name = "trayDayStats";
            this.trayDayStats.Size = new System.Drawing.Size(114, 22);
            this.trayDayStats.Text = "Day";
            this.trayDayStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // traySessionStats
            // 
            this.traySessionStats.CheckOnClick = true;
            this.traySessionStats.Name = "traySessionStats";
            this.traySessionStats.Size = new System.Drawing.Size(114, 22);
            this.traySessionStats.Text = "Session";
            this.traySessionStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // trayPartyFilter
            // 
            this.trayPartyFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.trayAllPartyStats, this.traySoloStats, this.trayPartyStats });
            this.trayPartyFilter.Image = global::FallGuysStats.Properties.Resources.player_icon;
            this.trayPartyFilter.Name = "trayPartyFilter";
            this.trayPartyFilter.Size = new System.Drawing.Size(101, 22);
            this.trayPartyFilter.Text = "Party";
            // 
            // trayAllPartyStats
            // 
            this.trayAllPartyStats.Checked = true;
            this.trayAllPartyStats.CheckOnClick = true;
            this.trayAllPartyStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trayAllPartyStats.Name = "trayAllPartyStats";
            this.trayAllPartyStats.Size = new System.Drawing.Size(101, 22);
            this.trayAllPartyStats.Text = "All";
            this.trayAllPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // traySoloStats
            // 
            this.traySoloStats.CheckOnClick = true;
            this.traySoloStats.Name = "traySoloStats";
            this.traySoloStats.Size = new System.Drawing.Size(101, 22);
            this.traySoloStats.Text = "Solo";
            this.traySoloStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // trayPartyStats
            // 
            this.trayPartyStats.CheckOnClick = true;
            this.trayPartyStats.Name = "trayPartyStats";
            this.trayPartyStats.Size = new System.Drawing.Size(101, 22);
            this.trayPartyStats.Text = "Party";
            this.trayPartyStats.Click += new System.EventHandler(this.menuStats_Click);
            // 
            // trayProfile
            // 
            this.trayProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.trayEditProfiles });
            this.trayProfile.Image = global::FallGuysStats.Properties.Resources.profile_icon;
            this.trayProfile.Name = "trayProfile";
            this.trayProfile.Size = new System.Drawing.Size(165, 22);
            this.trayProfile.Text = "Profile";
            // 
            // trayEditProfiles
            // 
            this.trayEditProfiles.Image = global::FallGuysStats.Properties.Resources.setting_icon;
            this.trayEditProfiles.Name = "trayEditProfiles";
            this.trayEditProfiles.Size = new System.Drawing.Size(155, 22);
            this.trayEditProfiles.Text = "Profile Settings";
            this.trayEditProfiles.Click += new System.EventHandler(this.menuEditProfiles_Click);
            // 
            // traySeparator3
            // 
            this.traySeparator3.Name = "traySeparator3";
            this.traySeparator3.Size = new System.Drawing.Size(162, 6);
            // 
            // trayUpdate
            // 
            this.trayUpdate.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.trayUpdate.Name = "trayUpdate";
            this.trayUpdate.Size = new System.Drawing.Size(165, 22);
            this.trayUpdate.Text = "Update";
            this.trayUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // trayHelp
            // 
            this.trayHelp.Image = global::FallGuysStats.Properties.Resources.github_icon;
            this.trayHelp.Name = "trayHelp";
            this.trayHelp.Size = new System.Drawing.Size(165, 22);
            this.trayHelp.Text = "Help";
            this.trayHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // trayLaunchFallGuys
            // 
            this.trayLaunchFallGuys.Image = global::FallGuysStats.Properties.Resources.fallguys_icon;
            this.trayLaunchFallGuys.Name = "trayLaunchFallGuys";
            this.trayLaunchFallGuys.Size = new System.Drawing.Size(165, 22);
            this.trayLaunchFallGuys.Text = "Launch Fall Guys";
            this.trayLaunchFallGuys.Click += new System.EventHandler(this.menuLaunchFallGuys_Click);
            // 
            // traySeparator4
            // 
            this.traySeparator4.Name = "traySeparator4";
            this.traySeparator4.Size = new System.Drawing.Size(162, 6);
            // 
            // trayExitProgram
            // 
            this.trayExitProgram.Image = global::FallGuysStats.Properties.Resources.shutdown_icon;
            this.trayExitProgram.Name = "trayExitProgram";
            this.trayExitProgram.Size = new System.Drawing.Size(165, 22);
            this.trayExitProgram.Text = "Exit";
            this.trayExitProgram.Click += new System.EventHandler(this.Stats_ExitProgram);
            // 
            // Stats
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(830, 830);
            this.Controls.Add(this.infoStrip);
            this.Controls.Add(this.gridDetails);
            this.Controls.Add(this.menu);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.MinimumSize = new System.Drawing.Size(830, 350);
            this.Name = "Stats";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
            this.Load += new System.EventHandler(this.Stats_Load);
            this.Shown += new System.EventHandler(this.Stats_Shown);
            this.VisibleChanged += new System.EventHandler(this.Stats_VisibleChanged);
            this.Resize += new System.EventHandler(this.Stats_Resize);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.infoStrip.ResumeLayout(false);
            this.infoStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
            this.trayCMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.NotifyIcon trayIcon;
        private FallGuysStats.Grid gridDetails;
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
        
        private System.Windows.Forms.ContextMenuStrip trayCMenu;
        private System.Windows.Forms.ToolStripMenuItem trayAllPartyStats;
        private System.Windows.Forms.ToolStripMenuItem traySoloStats;
        private System.Windows.Forms.ToolStripMenuItem trayPartyStats;
        private System.Windows.Forms.ToolStripMenuItem trayDayStats;
        private System.Windows.Forms.ToolStripMenuItem traySessionStats;
        private System.Windows.Forms.ToolStripMenuItem trayAllStats;
        private System.Windows.Forms.ToolStripMenuItem traySeasonStats;
        private System.Windows.Forms.ToolStripMenuItem trayWeekStats;
        private System.Windows.Forms.ToolStripMenuItem trayPartyFilter;
        private System.Windows.Forms.ToolStripMenuItem trayStatsFilter;
        private System.Windows.Forms.ToolStripSeparator traySeparator3;
        private System.Windows.Forms.ToolStripMenuItem trayUpdate;
        private System.Windows.Forms.ToolStripMenuItem trayLaunchFallGuys;
        private System.Windows.Forms.ToolStripMenuItem trayHelp;
        private System.Windows.Forms.ToolStripSeparator traySeparator4;
        private System.Windows.Forms.ToolStripMenuItem trayExitProgram;
        private System.Windows.Forms.ToolStripMenuItem traySettings;
        private System.Windows.Forms.ToolStripSeparator traySeparator2;
        private System.Windows.Forms.ToolStripMenuItem trayFilters;
        private System.Windows.Forms.ToolStripSeparator traySeparator1;
        private System.Windows.Forms.ToolStripMenuItem trayProfile;
        private System.Windows.Forms.ToolStripMenuItem trayEditProfiles;
        private System.Windows.Forms.ToolStripMenuItem trayOverlay;
    }
}

