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
        private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stats));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
      this.mainToolStrip = new System.Windows.Forms.ToolStrip();
      this.tsbtnLaunchGame = new System.Windows.Forms.ToolStripButton();
      this.tslblTimePlayed = new System.Windows.Forms.ToolStripLabel();
      this.tsbtnShowCount = new System.Windows.Forms.ToolStripButton();
      this.tsbtnRoundCount = new System.Windows.Forms.ToolStripButton();
      this.tsbtnWinCount = new System.Windows.Forms.ToolStripButton();
      this.tslblFinalPct = new System.Windows.Forms.ToolStripLabel();
      this.tsbtnWinPct = new System.Windows.Forms.ToolStripButton();
      this.tslblKudosCount = new System.Windows.Forms.ToolStripLabel();
      this.tsbtnGridDisplayType = new System.Windows.Forms.ToolStripSplitButton();
      this.tsbtnCounts = new System.Windows.Forms.ToolStripMenuItem();
      this.tsbtnPercentages = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.tsbtnHelp = new System.Windows.Forms.ToolStripButton();
      this.gridDetails = new FallGuysStats.Grid();
      this.menu.SuspendLayout();
      this.mainToolStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
      this.SuspendLayout();
      // 
      // menu
      // 
      this.menu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSettings,
            this.menuFilters,
            this.menuProfile,
            this.menuOverlay,
            this.menuUpdate,
            this.menuHelp});
      this.menu.Location = new System.Drawing.Point(0, 0);
      this.menu.Name = "menu";
      this.menu.Size = new System.Drawing.Size(975, 33);
      this.menu.TabIndex = 12;
      this.menu.Text = "menuStrip1";
      // 
      // menuSettings
      // 
      this.menuSettings.Name = "menuSettings";
      this.menuSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.menuSettings.Size = new System.Drawing.Size(88, 29);
      this.menuSettings.Text = "Settings";
      this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
      // 
      // menuFilters
      // 
      this.menuFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStatsFilter,
            this.menuPartyFilter});
      this.menuFilters.Name = "menuFilters";
      this.menuFilters.Size = new System.Drawing.Size(70, 29);
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
      this.menuStatsFilter.Size = new System.Drawing.Size(135, 30);
      this.menuStatsFilter.Text = "Stats";
      // 
      // menuAllStats
      // 
      this.menuAllStats.Checked = true;
      this.menuAllStats.CheckOnClick = true;
      this.menuAllStats.CheckState = System.Windows.Forms.CheckState.Checked;
      this.menuAllStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuAllStats.Name = "menuAllStats";
      this.menuAllStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
      this.menuAllStats.Size = new System.Drawing.Size(267, 30);
      this.menuAllStats.Text = "All";
      this.menuAllStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuSeasonStats
      // 
      this.menuSeasonStats.CheckOnClick = true;
      this.menuSeasonStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuSeasonStats.Name = "menuSeasonStats";
      this.menuSeasonStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
      this.menuSeasonStats.Size = new System.Drawing.Size(267, 30);
      this.menuSeasonStats.Text = "Season";
      this.menuSeasonStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuWeekStats
      // 
      this.menuWeekStats.CheckOnClick = true;
      this.menuWeekStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuWeekStats.Name = "menuWeekStats";
      this.menuWeekStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
      this.menuWeekStats.Size = new System.Drawing.Size(267, 30);
      this.menuWeekStats.Text = "Week";
      this.menuWeekStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuDayStats
      // 
      this.menuDayStats.CheckOnClick = true;
      this.menuDayStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuDayStats.Name = "menuDayStats";
      this.menuDayStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
      this.menuDayStats.Size = new System.Drawing.Size(267, 30);
      this.menuDayStats.Text = "Day";
      this.menuDayStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuSessionStats
      // 
      this.menuSessionStats.CheckOnClick = true;
      this.menuSessionStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuSessionStats.Name = "menuSessionStats";
      this.menuSessionStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
      this.menuSessionStats.Size = new System.Drawing.Size(267, 30);
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
      this.menuPartyFilter.Size = new System.Drawing.Size(135, 30);
      this.menuPartyFilter.Text = "Party";
      // 
      // menuAllPartyStats
      // 
      this.menuAllPartyStats.Checked = true;
      this.menuAllPartyStats.CheckOnClick = true;
      this.menuAllPartyStats.CheckState = System.Windows.Forms.CheckState.Checked;
      this.menuAllPartyStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuAllPartyStats.Name = "menuAllPartyStats";
      this.menuAllPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
      this.menuAllPartyStats.Size = new System.Drawing.Size(245, 30);
      this.menuAllPartyStats.Text = "All";
      this.menuAllPartyStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuSoloStats
      // 
      this.menuSoloStats.CheckOnClick = true;
      this.menuSoloStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuSoloStats.Name = "menuSoloStats";
      this.menuSoloStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
      this.menuSoloStats.Size = new System.Drawing.Size(245, 30);
      this.menuSoloStats.Text = "Solo";
      this.menuSoloStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuPartyStats
      // 
      this.menuPartyStats.CheckOnClick = true;
      this.menuPartyStats.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuPartyStats.Name = "menuPartyStats";
      this.menuPartyStats.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
      this.menuPartyStats.Size = new System.Drawing.Size(245, 30);
      this.menuPartyStats.Text = "Party";
      this.menuPartyStats.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuProfile
      // 
      this.menuProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuProfileMain,
            this.menuProfilePractice});
      this.menuProfile.Name = "menuProfile";
      this.menuProfile.Size = new System.Drawing.Size(74, 29);
      this.menuProfile.Text = "Profile";
      // 
      // menuProfileMain
      // 
      this.menuProfileMain.Checked = true;
      this.menuProfileMain.CheckOnClick = true;
      this.menuProfileMain.CheckState = System.Windows.Forms.CheckState.Checked;
      this.menuProfileMain.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuProfileMain.Name = "menuProfileMain";
      this.menuProfileMain.Size = new System.Drawing.Size(156, 30);
      this.menuProfileMain.Text = "Main";
      this.menuProfileMain.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuProfilePractice
      // 
      this.menuProfilePractice.CheckOnClick = true;
      this.menuProfilePractice.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.menuProfilePractice.Name = "menuProfilePractice";
      this.menuProfilePractice.Size = new System.Drawing.Size(156, 30);
      this.menuProfilePractice.Text = "Practice";
      this.menuProfilePractice.Click += new System.EventHandler(this.menuStats_Click);
      // 
      // menuOverlay
      // 
      this.menuOverlay.Name = "menuOverlay";
      this.menuOverlay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this.menuOverlay.Size = new System.Drawing.Size(84, 29);
      this.menuOverlay.Text = "Overlay";
      this.menuOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
      // 
      // menuUpdate
      // 
      this.menuUpdate.Name = "menuUpdate";
      this.menuUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
      this.menuUpdate.Size = new System.Drawing.Size(82, 29);
      this.menuUpdate.Text = "Update";
      this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
      // 
      // menuHelp
      // 
      this.menuHelp.Name = "menuHelp";
      this.menuHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
      this.menuHelp.Size = new System.Drawing.Size(61, 29);
      this.menuHelp.Text = "Help";
      this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
      // 
      // mainToolStrip
      // 
      this.mainToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
      this.mainToolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnLaunchGame,
            this.tslblTimePlayed,
            this.tsbtnShowCount,
            this.tsbtnRoundCount,
            this.tsbtnWinCount,
            this.tslblFinalPct,
            this.tsbtnWinPct,
            this.tslblKudosCount,
            this.tsbtnGridDisplayType,
            this.toolStripSeparator,
            this.tsbtnHelp});
      this.mainToolStrip.Location = new System.Drawing.Point(0, 33);
      this.mainToolStrip.Name = "mainToolStrip";
      this.mainToolStrip.Size = new System.Drawing.Size(975, 32);
      this.mainToolStrip.TabIndex = 13;
      this.mainToolStrip.Text = "toolStrip1";
      // 
      // tsbtnLaunchGame
      // 
      this.tsbtnLaunchGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.tsbtnLaunchGame.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnLaunchGame.Image")));
      this.tsbtnLaunchGame.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnLaunchGame.Name = "tsbtnLaunchGame";
      this.tsbtnLaunchGame.Size = new System.Drawing.Size(145, 29);
      this.tsbtnLaunchGame.Text = "Launch Fall Guys";
      this.tsbtnLaunchGame.ToolTipText = "Launch Fall Guys";
      this.tsbtnLaunchGame.Click += new System.EventHandler(this.tsbtnLaunchGame_Click);
      // 
      // tslblTimePlayed
      // 
      this.tslblTimePlayed.Name = "tslblTimePlayed";
      this.tslblTimePlayed.Size = new System.Drawing.Size(174, 29);
      this.tslblTimePlayed.Text = "Time Played: 0:00:00";
      // 
      // tsbtnShowCount
      // 
      this.tsbtnShowCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.tsbtnShowCount.ForeColor = System.Drawing.Color.MediumBlue;
      this.tsbtnShowCount.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnShowCount.Image")));
      this.tsbtnShowCount.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnShowCount.Name = "tsbtnShowCount";
      this.tsbtnShowCount.Size = new System.Drawing.Size(87, 29);
      this.tsbtnShowCount.Text = "Shows: 0";
      this.tsbtnShowCount.Click += new System.EventHandler(this.tsbtnShowCount_Click);
      // 
      // tsbtnRoundCount
      // 
      this.tsbtnRoundCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.tsbtnRoundCount.ForeColor = System.Drawing.Color.MediumBlue;
      this.tsbtnRoundCount.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRoundCount.Image")));
      this.tsbtnRoundCount.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnRoundCount.Name = "tsbtnRoundCount";
      this.tsbtnRoundCount.Size = new System.Drawing.Size(95, 29);
      this.tsbtnRoundCount.Text = "Rounds: 0";
      this.tsbtnRoundCount.Click += new System.EventHandler(this.tsBtnRoundCount_Click);
      // 
      // tsbtnWinCount
      // 
      this.tsbtnWinCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.tsbtnWinCount.ForeColor = System.Drawing.Color.MediumBlue;
      this.tsbtnWinCount.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnWinCount.Image")));
      this.tsbtnWinCount.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnWinCount.Name = "tsbtnWinCount";
      this.tsbtnWinCount.Size = new System.Drawing.Size(74, 29);
      this.tsbtnWinCount.Text = "Wins: 0";
      this.tsbtnWinCount.Click += new System.EventHandler(this.tsbtnWinCount_Click);
      // 
      // tslblFinalPct
      // 
      this.tslblFinalPct.Name = "tslblFinalPct";
      this.tslblFinalPct.Size = new System.Drawing.Size(68, 29);
      this.tslblFinalPct.Text = "Final %";
      // 
      // tsbtnWinPct
      // 
      this.tsbtnWinPct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.tsbtnWinPct.ForeColor = System.Drawing.Color.MediumBlue;
      this.tsbtnWinPct.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnWinPct.Image")));
      this.tsbtnWinPct.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnWinPct.Name = "tsbtnWinPct";
      this.tsbtnWinPct.Size = new System.Drawing.Size(71, 29);
      this.tsbtnWinPct.Text = "Win %:";
      this.tsbtnWinPct.Click += new System.EventHandler(this.tsbtnWinPct_Click);
      // 
      // tslblKudosCount
      // 
      this.tslblKudosCount.Name = "tslblKudosCount";
      this.tslblKudosCount.Size = new System.Drawing.Size(62, 29);
      this.tslblKudosCount.Text = "Kudos";
      // 
      // tsbtnGridDisplayType
      // 
      this.tsbtnGridDisplayType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnCounts,
            this.tsbtnPercentages});
      this.tsbtnGridDisplayType.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnGridDisplayType.Image")));
      this.tsbtnGridDisplayType.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnGridDisplayType.Name = "tsbtnGridDisplayType";
      this.tsbtnGridDisplayType.Size = new System.Drawing.Size(153, 29);
      this.tsbtnGridDisplayType.Text = "Grid Display";
      this.tsbtnGridDisplayType.ToolTipText = "Switch between counts and percentages in the stats grid";
      this.tsbtnGridDisplayType.ButtonClick += new System.EventHandler(this.tsbtnGridDisplayType_ButtonClick);
      // 
      // tsbtnCounts
      // 
      this.tsbtnCounts.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnCounts.Image")));
      this.tsbtnCounts.Name = "tsbtnCounts";
      this.tsbtnCounts.Size = new System.Drawing.Size(252, 30);
      this.tsbtnCounts.Text = "Counts";
      this.tsbtnCounts.Click += new System.EventHandler(this.tsbtnCounts_Click);
      // 
      // tsbtnPercentages
      // 
      this.tsbtnPercentages.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPercentages.Image")));
      this.tsbtnPercentages.Name = "tsbtnPercentages";
      this.tsbtnPercentages.Size = new System.Drawing.Size(252, 30);
      this.tsbtnPercentages.Text = "Percentages";
      this.tsbtnPercentages.Click += new System.EventHandler(this.tsbtnPercentages_Click);
      // 
      // toolStripSeparator
      // 
      this.toolStripSeparator.Name = "toolStripSeparator";
      this.toolStripSeparator.Size = new System.Drawing.Size(6, 32);
      // 
      // tsbtnHelp
      // 
      this.tsbtnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsbtnHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnHelp.Image")));
      this.tsbtnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbtnHelp.Name = "tsbtnHelp";
      this.tsbtnHelp.Size = new System.Drawing.Size(28, 28);
      this.tsbtnHelp.Text = "He&lp";
      this.tsbtnHelp.Click += new System.EventHandler(this.tsbtlHelp_Click);
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
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Cyan;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.gridDetails.ColumnHeadersHeight = 20;
      this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.gridDetails.DefaultCellStyle = dataGridViewCellStyle4;
      this.gridDetails.EnableHeadersVisualStyles = false;
      this.gridDetails.GridColor = System.Drawing.Color.Gray;
      this.gridDetails.Location = new System.Drawing.Point(0, 55);
      this.gridDetails.Name = "gridDetails";
      this.gridDetails.ReadOnly = true;
      this.gridDetails.RowHeadersVisible = false;
      this.gridDetails.Size = new System.Drawing.Size(975, 785);
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
      this.ClientSize = new System.Drawing.Size(975, 853);
      this.Controls.Add(this.mainToolStrip);
      this.Controls.Add(this.gridDetails);
      this.Controls.Add(this.menu);
      this.DoubleBuffered = true;
      this.ForeColor = System.Drawing.Color.Black;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menu;
      this.MinimumSize = new System.Drawing.Size(630, 300);
      this.Name = "Stats";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Fall Guys Stats";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
      this.Load += new System.EventHandler(this.Stats_Load);
      this.Shown += new System.EventHandler(this.Stats_Shown);
      this.menu.ResumeLayout(false);
      this.menu.PerformLayout();
      this.mainToolStrip.ResumeLayout(false);
      this.mainToolStrip.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
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
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripLabel tslblTimePlayed;
        private System.Windows.Forms.ToolStripButton tsbtnShowCount;
        private System.Windows.Forms.ToolStripButton tsbtnRoundCount;
        private System.Windows.Forms.ToolStripButton tsbtnWinCount;
        private System.Windows.Forms.ToolStripButton tsbtnWinPct;
        private System.Windows.Forms.ToolStripLabel tslblFinalPct;
        private System.Windows.Forms.ToolStripLabel tslblKudosCount;
        private System.Windows.Forms.ToolStripSplitButton tsbtnGridDisplayType;
        private System.Windows.Forms.ToolStripMenuItem tsbtnCounts;
        private System.Windows.Forms.ToolStripMenuItem tsbtnPercentages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton tsbtnHelp;
        private System.Windows.Forms.ToolStripButton tsbtnLaunchGame;
    }
}

