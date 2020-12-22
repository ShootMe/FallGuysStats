namespace FallGuysStats {
    partial class Settings {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
      this.lblLogPath = new System.Windows.Forms.Label();
      this.lblLogPathNote = new System.Windows.Forms.Label();
      this.txtLogPath = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkCycleOverlayQualify = new System.Windows.Forms.CheckBox();
      this.chkCycleOverlayLongest = new System.Windows.Forms.CheckBox();
      this.grpOverlay = new System.Windows.Forms.GroupBox();
      this.lblOrPing = new System.Windows.Forms.Label();
      this.chkOnlyShowPing = new System.Windows.Forms.CheckBox();
      this.lblOrFinal = new System.Windows.Forms.Label();
      this.chkOnlyShowFinalStreak = new System.Windows.Forms.CheckBox();
      this.lblOrLongest = new System.Windows.Forms.Label();
      this.chkOnlyShowLongest = new System.Windows.Forms.CheckBox();
      this.lblOrGold = new System.Windows.Forms.Label();
      this.chkOnlyShowGold = new System.Windows.Forms.CheckBox();
      this.chkCycleOverlayStreak = new System.Windows.Forms.CheckBox();
      this.chkCycleOverlayPlayers = new System.Windows.Forms.CheckBox();
      this.chkHidePercentages = new System.Windows.Forms.CheckBox();
      this.chkHideWinsInfo = new System.Windows.Forms.CheckBox();
      this.cboOverlayColor = new System.Windows.Forms.ComboBox();
      this.lblOverlayColor = new System.Windows.Forms.Label();
      this.chkFlipped = new System.Windows.Forms.CheckBox();
      this.chkShowTabs = new System.Windows.Forms.CheckBox();
      this.chkHideTimeInfo = new System.Windows.Forms.CheckBox();
      this.chkHideRoundInfo = new System.Windows.Forms.CheckBox();
      this.cboFastestFilter = new System.Windows.Forms.ComboBox();
      this.lblFastestFilter = new System.Windows.Forms.Label();
      this.cboQualifyFilter = new System.Windows.Forms.ComboBox();
      this.lblQualifyFilter = new System.Windows.Forms.Label();
      this.cboWinsFilter = new System.Windows.Forms.ComboBox();
      this.lblWinsFilter = new System.Windows.Forms.Label();
      this.chkOverlayOnTop = new System.Windows.Forms.CheckBox();
      this.chkUseNDI = new System.Windows.Forms.CheckBox();
      this.lblCycleTimeSecondsTag = new System.Windows.Forms.Label();
      this.lblCycleTimeSeconds = new System.Windows.Forms.Label();
      this.txtCycleTimeSeconds = new System.Windows.Forms.TextBox();
      this.grpStats = new System.Windows.Forms.GroupBox();
      this.chkChangeHoopsieLegends = new System.Windows.Forms.CheckBox();
      this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
      this.lblPreviousWinsNote = new System.Windows.Forms.Label();
      this.lblPreviousWins = new System.Windows.Forms.Label();
      this.txtPreviousWins = new System.Windows.Forms.TextBox();
      this.grpSortingOptions = new System.Windows.Forms.GroupBox();
      this.chkIgnoreLevelTypeWhenSorting = new System.Windows.Forms.CheckBox();
      this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
      this.txtGameExeLocation = new System.Windows.Forms.TextBox();
      this.btnGameExeLocationBrowse = new System.Windows.Forms.Button();
      this.dlgGameExeBrowser = new System.Windows.Forms.OpenFileDialog();
      this.chkAutoLaunchGameOnStart = new System.Windows.Forms.CheckBox();
      this.grpGameOptions = new System.Windows.Forms.GroupBox();
      this.lblGameExeLocation = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.grpOverlay.SuspendLayout();
      this.grpStats.SuspendLayout();
      this.grpSortingOptions.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
      this.grpGameOptions.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblLogPath
      // 
      this.lblLogPath.AutoSize = true;
      this.lblLogPath.Location = new System.Drawing.Point(12, 23);
      this.lblLogPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLogPath.Name = "lblLogPath";
      this.lblLogPath.Size = new System.Drawing.Size(73, 20);
      this.lblLogPath.TabIndex = 0;
      this.lblLogPath.Text = "Log Path";
      // 
      // lblLogPathNote
      // 
      this.lblLogPathNote.AutoSize = true;
      this.lblLogPathNote.ForeColor = System.Drawing.Color.DimGray;
      this.lblLogPathNote.Location = new System.Drawing.Point(92, 54);
      this.lblLogPathNote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLogPathNote.Name = "lblLogPathNote";
      this.lblLogPathNote.Size = new System.Drawing.Size(682, 20);
      this.lblLogPathNote.TabIndex = 2;
      this.lblLogPathNote.Text = "* You should not need to set this. Only use when the program is not reading the c" +
    "orrect location.";
      // 
      // txtLogPath
      // 
      this.txtLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtLogPath.Location = new System.Drawing.Point(96, 18);
      this.txtLogPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtLogPath.Name = "txtLogPath";
      this.txtLogPath.Size = new System.Drawing.Size(888, 26);
      this.txtLogPath.TabIndex = 1;
      this.txtLogPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLogPath_Validating);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btnSave.Location = new System.Drawing.Point(750, 791);
      this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(112, 35);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // chkCycleOverlayQualify
      // 
      this.chkCycleOverlayQualify.AutoSize = true;
      this.chkCycleOverlayQualify.Location = new System.Drawing.Point(24, 289);
      this.chkCycleOverlayQualify.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkCycleOverlayQualify.Name = "chkCycleOverlayQualify";
      this.chkCycleOverlayQualify.Size = new System.Drawing.Size(171, 24);
      this.chkCycleOverlayQualify.TabIndex = 8;
      this.chkCycleOverlayQualify.Text = "Cycle Qualify / Gold";
      this.chkCycleOverlayQualify.UseVisualStyleBackColor = true;
      this.chkCycleOverlayQualify.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // chkCycleOverlayLongest
      // 
      this.chkCycleOverlayLongest.AutoSize = true;
      this.chkCycleOverlayLongest.Location = new System.Drawing.Point(24, 325);
      this.chkCycleOverlayLongest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkCycleOverlayLongest.Name = "chkCycleOverlayLongest";
      this.chkCycleOverlayLongest.Size = new System.Drawing.Size(201, 24);
      this.chkCycleOverlayLongest.TabIndex = 11;
      this.chkCycleOverlayLongest.Text = "Cycle Fastest / Longest";
      this.chkCycleOverlayLongest.UseVisualStyleBackColor = true;
      this.chkCycleOverlayLongest.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // grpOverlay
      // 
      this.grpOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grpOverlay.Controls.Add(this.lblOrPing);
      this.grpOverlay.Controls.Add(this.chkOnlyShowPing);
      this.grpOverlay.Controls.Add(this.lblOrFinal);
      this.grpOverlay.Controls.Add(this.chkOnlyShowFinalStreak);
      this.grpOverlay.Controls.Add(this.lblOrLongest);
      this.grpOverlay.Controls.Add(this.chkOnlyShowLongest);
      this.grpOverlay.Controls.Add(this.lblOrGold);
      this.grpOverlay.Controls.Add(this.chkOnlyShowGold);
      this.grpOverlay.Controls.Add(this.chkCycleOverlayStreak);
      this.grpOverlay.Controls.Add(this.chkCycleOverlayPlayers);
      this.grpOverlay.Controls.Add(this.chkHidePercentages);
      this.grpOverlay.Controls.Add(this.chkHideWinsInfo);
      this.grpOverlay.Controls.Add(this.cboOverlayColor);
      this.grpOverlay.Controls.Add(this.lblOverlayColor);
      this.grpOverlay.Controls.Add(this.chkFlipped);
      this.grpOverlay.Controls.Add(this.chkShowTabs);
      this.grpOverlay.Controls.Add(this.chkHideTimeInfo);
      this.grpOverlay.Controls.Add(this.chkHideRoundInfo);
      this.grpOverlay.Controls.Add(this.cboFastestFilter);
      this.grpOverlay.Controls.Add(this.lblFastestFilter);
      this.grpOverlay.Controls.Add(this.cboQualifyFilter);
      this.grpOverlay.Controls.Add(this.lblQualifyFilter);
      this.grpOverlay.Controls.Add(this.cboWinsFilter);
      this.grpOverlay.Controls.Add(this.lblWinsFilter);
      this.grpOverlay.Controls.Add(this.chkOverlayOnTop);
      this.grpOverlay.Controls.Add(this.chkUseNDI);
      this.grpOverlay.Controls.Add(this.lblCycleTimeSecondsTag);
      this.grpOverlay.Controls.Add(this.lblCycleTimeSeconds);
      this.grpOverlay.Controls.Add(this.txtCycleTimeSeconds);
      this.grpOverlay.Controls.Add(this.chkCycleOverlayQualify);
      this.grpOverlay.Controls.Add(this.chkCycleOverlayLongest);
      this.grpOverlay.Location = new System.Drawing.Point(18, 177);
      this.grpOverlay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.grpOverlay.Name = "grpOverlay";
      this.grpOverlay.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.grpOverlay.Size = new System.Drawing.Size(968, 436);
      this.grpOverlay.TabIndex = 4;
      this.grpOverlay.TabStop = false;
      this.grpOverlay.Text = "Overlay";
      // 
      // lblOrPing
      // 
      this.lblOrPing.AutoSize = true;
      this.lblOrPing.ForeColor = System.Drawing.Color.DimGray;
      this.lblOrPing.Location = new System.Drawing.Point(240, 397);
      this.lblOrPing.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblOrPing.Name = "lblOrPing";
      this.lblOrPing.Size = new System.Drawing.Size(23, 20);
      this.lblOrPing.TabIndex = 18;
      this.lblOrPing.Text = "or";
      // 
      // chkOnlyShowPing
      // 
      this.chkOnlyShowPing.AutoSize = true;
      this.chkOnlyShowPing.Location = new System.Drawing.Point(290, 395);
      this.chkOnlyShowPing.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkOnlyShowPing.Name = "chkOnlyShowPing";
      this.chkOnlyShowPing.Size = new System.Drawing.Size(145, 24);
      this.chkOnlyShowPing.TabIndex = 19;
      this.chkOnlyShowPing.Text = "Only Show Ping";
      this.chkOnlyShowPing.UseVisualStyleBackColor = true;
      this.chkOnlyShowPing.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // lblOrFinal
      // 
      this.lblOrFinal.AutoSize = true;
      this.lblOrFinal.ForeColor = System.Drawing.Color.DimGray;
      this.lblOrFinal.Location = new System.Drawing.Point(240, 362);
      this.lblOrFinal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblOrFinal.Name = "lblOrFinal";
      this.lblOrFinal.Size = new System.Drawing.Size(23, 20);
      this.lblOrFinal.TabIndex = 15;
      this.lblOrFinal.Text = "or";
      // 
      // chkOnlyShowFinalStreak
      // 
      this.chkOnlyShowFinalStreak.AutoSize = true;
      this.chkOnlyShowFinalStreak.Location = new System.Drawing.Point(290, 360);
      this.chkOnlyShowFinalStreak.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkOnlyShowFinalStreak.Name = "chkOnlyShowFinalStreak";
      this.chkOnlyShowFinalStreak.Size = new System.Drawing.Size(199, 24);
      this.chkOnlyShowFinalStreak.TabIndex = 16;
      this.chkOnlyShowFinalStreak.Text = "Only Show Final Streak";
      this.chkOnlyShowFinalStreak.UseVisualStyleBackColor = true;
      this.chkOnlyShowFinalStreak.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // lblOrLongest
      // 
      this.lblOrLongest.AutoSize = true;
      this.lblOrLongest.ForeColor = System.Drawing.Color.DimGray;
      this.lblOrLongest.Location = new System.Drawing.Point(240, 326);
      this.lblOrLongest.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblOrLongest.Name = "lblOrLongest";
      this.lblOrLongest.Size = new System.Drawing.Size(23, 20);
      this.lblOrLongest.TabIndex = 12;
      this.lblOrLongest.Text = "or";
      // 
      // chkOnlyShowLongest
      // 
      this.chkOnlyShowLongest.AutoSize = true;
      this.chkOnlyShowLongest.Location = new System.Drawing.Point(290, 325);
      this.chkOnlyShowLongest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkOnlyShowLongest.Name = "chkOnlyShowLongest";
      this.chkOnlyShowLongest.Size = new System.Drawing.Size(172, 24);
      this.chkOnlyShowLongest.TabIndex = 13;
      this.chkOnlyShowLongest.Text = "Only Show Longest";
      this.chkOnlyShowLongest.UseVisualStyleBackColor = true;
      this.chkOnlyShowLongest.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // lblOrGold
      // 
      this.lblOrGold.AutoSize = true;
      this.lblOrGold.ForeColor = System.Drawing.Color.DimGray;
      this.lblOrGold.Location = new System.Drawing.Point(240, 291);
      this.lblOrGold.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblOrGold.Name = "lblOrGold";
      this.lblOrGold.Size = new System.Drawing.Size(23, 20);
      this.lblOrGold.TabIndex = 9;
      this.lblOrGold.Text = "or";
      // 
      // chkOnlyShowGold
      // 
      this.chkOnlyShowGold.AutoSize = true;
      this.chkOnlyShowGold.Location = new System.Drawing.Point(290, 289);
      this.chkOnlyShowGold.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkOnlyShowGold.Name = "chkOnlyShowGold";
      this.chkOnlyShowGold.Size = new System.Drawing.Size(148, 24);
      this.chkOnlyShowGold.TabIndex = 10;
      this.chkOnlyShowGold.Text = "Only Show Gold";
      this.chkOnlyShowGold.UseVisualStyleBackColor = true;
      this.chkOnlyShowGold.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // chkCycleOverlayStreak
      // 
      this.chkCycleOverlayStreak.AutoSize = true;
      this.chkCycleOverlayStreak.Location = new System.Drawing.Point(24, 360);
      this.chkCycleOverlayStreak.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkCycleOverlayStreak.Name = "chkCycleOverlayStreak";
      this.chkCycleOverlayStreak.Size = new System.Drawing.Size(201, 24);
      this.chkCycleOverlayStreak.TabIndex = 14;
      this.chkCycleOverlayStreak.Text = "Cycle Win / Final Streak";
      this.chkCycleOverlayStreak.UseVisualStyleBackColor = true;
      this.chkCycleOverlayStreak.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // chkCycleOverlayPlayers
      // 
      this.chkCycleOverlayPlayers.AutoSize = true;
      this.chkCycleOverlayPlayers.Location = new System.Drawing.Point(24, 395);
      this.chkCycleOverlayPlayers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkCycleOverlayPlayers.Name = "chkCycleOverlayPlayers";
      this.chkCycleOverlayPlayers.Size = new System.Drawing.Size(171, 24);
      this.chkCycleOverlayPlayers.TabIndex = 17;
      this.chkCycleOverlayPlayers.Text = "Cycle Players / Ping";
      this.chkCycleOverlayPlayers.UseVisualStyleBackColor = true;
      this.chkCycleOverlayPlayers.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
      // 
      // chkHidePercentages
      // 
      this.chkHidePercentages.AutoSize = true;
      this.chkHidePercentages.Location = new System.Drawing.Point(24, 138);
      this.chkHidePercentages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkHidePercentages.Name = "chkHidePercentages";
      this.chkHidePercentages.Size = new System.Drawing.Size(162, 24);
      this.chkHidePercentages.TabIndex = 3;
      this.chkHidePercentages.Text = "Hide Percentages";
      this.chkHidePercentages.UseVisualStyleBackColor = true;
      // 
      // chkHideWinsInfo
      // 
      this.chkHideWinsInfo.AutoSize = true;
      this.chkHideWinsInfo.Location = new System.Drawing.Point(24, 32);
      this.chkHideWinsInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkHideWinsInfo.Name = "chkHideWinsInfo";
      this.chkHideWinsInfo.Size = new System.Drawing.Size(137, 24);
      this.chkHideWinsInfo.TabIndex = 0;
      this.chkHideWinsInfo.Text = "Hide Wins info";
      this.chkHideWinsInfo.UseVisualStyleBackColor = true;
      // 
      // cboOverlayColor
      // 
      this.cboOverlayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboOverlayColor.FormattingEnabled = true;
      this.cboOverlayColor.Items.AddRange(new object[] {
            "Transparent",
            "Magenta",
            "Red",
            "Green",
            "Blue",
            "Black"});
      this.cboOverlayColor.Location = new System.Drawing.Point(670, 242);
      this.cboOverlayColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cboOverlayColor.Name = "cboOverlayColor";
      this.cboOverlayColor.Size = new System.Drawing.Size(272, 28);
      this.cboOverlayColor.TabIndex = 27;
      // 
      // lblOverlayColor
      // 
      this.lblOverlayColor.AutoSize = true;
      this.lblOverlayColor.Location = new System.Drawing.Point(564, 246);
      this.lblOverlayColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblOverlayColor.Name = "lblOverlayColor";
      this.lblOverlayColor.Size = new System.Drawing.Size(95, 20);
      this.lblOverlayColor.TabIndex = 26;
      this.lblOverlayColor.Text = "Background";
      // 
      // chkFlipped
      // 
      this.chkFlipped.AutoSize = true;
      this.chkFlipped.Location = new System.Drawing.Point(670, 289);
      this.chkFlipped.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkFlipped.Name = "chkFlipped";
      this.chkFlipped.Size = new System.Drawing.Size(195, 24);
      this.chkFlipped.TabIndex = 28;
      this.chkFlipped.Text = "Flip display horizontally";
      this.chkFlipped.UseVisualStyleBackColor = true;
      // 
      // chkShowTabs
      // 
      this.chkShowTabs.AutoSize = true;
      this.chkShowTabs.Location = new System.Drawing.Point(24, 174);
      this.chkShowTabs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkShowTabs.Name = "chkShowTabs";
      this.chkShowTabs.Size = new System.Drawing.Size(217, 24);
      this.chkShowTabs.TabIndex = 4;
      this.chkShowTabs.Text = "Show Tab for current filter";
      this.chkShowTabs.UseVisualStyleBackColor = true;
      // 
      // chkHideTimeInfo
      // 
      this.chkHideTimeInfo.AutoSize = true;
      this.chkHideTimeInfo.Location = new System.Drawing.Point(24, 103);
      this.chkHideTimeInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkHideTimeInfo.Name = "chkHideTimeInfo";
      this.chkHideTimeInfo.Size = new System.Drawing.Size(136, 24);
      this.chkHideTimeInfo.TabIndex = 2;
      this.chkHideTimeInfo.Text = "Hide Time info";
      this.chkHideTimeInfo.UseVisualStyleBackColor = true;
      // 
      // chkHideRoundInfo
      // 
      this.chkHideRoundInfo.AutoSize = true;
      this.chkHideRoundInfo.Location = new System.Drawing.Point(24, 68);
      this.chkHideRoundInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkHideRoundInfo.Name = "chkHideRoundInfo";
      this.chkHideRoundInfo.Size = new System.Drawing.Size(150, 24);
      this.chkHideRoundInfo.TabIndex = 1;
      this.chkHideRoundInfo.Text = "Hide Round info";
      this.chkHideRoundInfo.UseVisualStyleBackColor = true;
      // 
      // cboFastestFilter
      // 
      this.cboFastestFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboFastestFilter.FormattingEnabled = true;
      this.cboFastestFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
      this.cboFastestFilter.Location = new System.Drawing.Point(670, 106);
      this.cboFastestFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cboFastestFilter.Name = "cboFastestFilter";
      this.cboFastestFilter.Size = new System.Drawing.Size(272, 28);
      this.cboFastestFilter.TabIndex = 25;
      // 
      // lblFastestFilter
      // 
      this.lblFastestFilter.AutoSize = true;
      this.lblFastestFilter.Location = new System.Drawing.Point(489, 111);
      this.lblFastestFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblFastestFilter.Name = "lblFastestFilter";
      this.lblFastestFilter.Size = new System.Drawing.Size(172, 20);
      this.lblFastestFilter.TabIndex = 24;
      this.lblFastestFilter.Text = "Fastest / Longest Filter";
      // 
      // cboQualifyFilter
      // 
      this.cboQualifyFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboQualifyFilter.FormattingEnabled = true;
      this.cboQualifyFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
      this.cboQualifyFilter.Location = new System.Drawing.Point(670, 68);
      this.cboQualifyFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cboQualifyFilter.Name = "cboQualifyFilter";
      this.cboQualifyFilter.Size = new System.Drawing.Size(272, 28);
      this.cboQualifyFilter.TabIndex = 23;
      // 
      // lblQualifyFilter
      // 
      this.lblQualifyFilter.AutoSize = true;
      this.lblQualifyFilter.Location = new System.Drawing.Point(516, 72);
      this.lblQualifyFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblQualifyFilter.Name = "lblQualifyFilter";
      this.lblQualifyFilter.Size = new System.Drawing.Size(142, 20);
      this.lblQualifyFilter.TabIndex = 22;
      this.lblQualifyFilter.Text = "Qualify / Gold Filter";
      // 
      // cboWinsFilter
      // 
      this.cboWinsFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboWinsFilter.FormattingEnabled = true;
      this.cboWinsFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
      this.cboWinsFilter.Location = new System.Drawing.Point(670, 29);
      this.cboWinsFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cboWinsFilter.Name = "cboWinsFilter";
      this.cboWinsFilter.Size = new System.Drawing.Size(272, 28);
      this.cboWinsFilter.TabIndex = 21;
      // 
      // lblWinsFilter
      // 
      this.lblWinsFilter.AutoSize = true;
      this.lblWinsFilter.Location = new System.Drawing.Point(528, 34);
      this.lblWinsFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblWinsFilter.Name = "lblWinsFilter";
      this.lblWinsFilter.Size = new System.Drawing.Size(129, 20);
      this.lblWinsFilter.TabIndex = 20;
      this.lblWinsFilter.Text = "Wins / Final Filter";
      // 
      // chkOverlayOnTop
      // 
      this.chkOverlayOnTop.AutoSize = true;
      this.chkOverlayOnTop.Location = new System.Drawing.Point(670, 325);
      this.chkOverlayOnTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkOverlayOnTop.Name = "chkOverlayOnTop";
      this.chkOverlayOnTop.Size = new System.Drawing.Size(174, 24);
      this.chkOverlayOnTop.TabIndex = 29;
      this.chkOverlayOnTop.Text = "Always show on top";
      this.chkOverlayOnTop.UseVisualStyleBackColor = true;
      // 
      // chkUseNDI
      // 
      this.chkUseNDI.AutoSize = true;
      this.chkUseNDI.Location = new System.Drawing.Point(670, 360);
      this.chkUseNDI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkUseNDI.Name = "chkUseNDI";
      this.chkUseNDI.Size = new System.Drawing.Size(231, 24);
      this.chkUseNDI.TabIndex = 30;
      this.chkUseNDI.Text = "Use NDI to transmit Overlay";
      this.chkUseNDI.UseVisualStyleBackColor = true;
      // 
      // lblCycleTimeSecondsTag
      // 
      this.lblCycleTimeSecondsTag.AutoSize = true;
      this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(188, 246);
      this.lblCycleTimeSecondsTag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
      this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(34, 20);
      this.lblCycleTimeSecondsTag.TabIndex = 7;
      this.lblCycleTimeSecondsTag.Text = "sec";
      // 
      // lblCycleTimeSeconds
      // 
      this.lblCycleTimeSeconds.AutoSize = true;
      this.lblCycleTimeSeconds.Location = new System.Drawing.Point(28, 246);
      this.lblCycleTimeSeconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
      this.lblCycleTimeSeconds.Size = new System.Drawing.Size(85, 20);
      this.lblCycleTimeSeconds.TabIndex = 5;
      this.lblCycleTimeSeconds.Text = "Cycle Time";
      // 
      // txtCycleTimeSeconds
      // 
      this.txtCycleTimeSeconds.Location = new System.Drawing.Point(126, 242);
      this.txtCycleTimeSeconds.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtCycleTimeSeconds.MaxLength = 2;
      this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
      this.txtCycleTimeSeconds.Size = new System.Drawing.Size(50, 26);
      this.txtCycleTimeSeconds.TabIndex = 6;
      this.txtCycleTimeSeconds.Text = "5";
      this.txtCycleTimeSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
      // 
      // grpStats
      // 
      this.grpStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grpStats.Controls.Add(this.chkChangeHoopsieLegends);
      this.grpStats.Controls.Add(this.chkAutoUpdate);
      this.grpStats.Controls.Add(this.lblPreviousWinsNote);
      this.grpStats.Controls.Add(this.lblPreviousWins);
      this.grpStats.Controls.Add(this.txtPreviousWins);
      this.grpStats.Location = new System.Drawing.Point(18, 89);
      this.grpStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.grpStats.Name = "grpStats";
      this.grpStats.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.grpStats.Size = new System.Drawing.Size(968, 78);
      this.grpStats.TabIndex = 3;
      this.grpStats.TabStop = false;
      this.grpStats.Text = "Stats";
      // 
      // chkChangeHoopsieLegends
      // 
      this.chkChangeHoopsieLegends.AutoSize = true;
      this.chkChangeHoopsieLegends.Location = new System.Drawing.Point(585, 32);
      this.chkChangeHoopsieLegends.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkChangeHoopsieLegends.Name = "chkChangeHoopsieLegends";
      this.chkChangeHoopsieLegends.Size = new System.Drawing.Size(362, 24);
      this.chkChangeHoopsieLegends.TabIndex = 4;
      this.chkChangeHoopsieLegends.Text = "Rename Hoopsie Legends to Hoopsie Heroes";
      this.chkChangeHoopsieLegends.UseVisualStyleBackColor = true;
      // 
      // chkAutoUpdate
      // 
      this.chkAutoUpdate.AutoSize = true;
      this.chkAutoUpdate.Location = new System.Drawing.Point(381, 32);
      this.chkAutoUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkAutoUpdate.Name = "chkAutoUpdate";
      this.chkAutoUpdate.Size = new System.Drawing.Size(190, 24);
      this.chkAutoUpdate.TabIndex = 3;
      this.chkAutoUpdate.Text = "Auto Update Program";
      this.chkAutoUpdate.UseVisualStyleBackColor = true;
      // 
      // lblPreviousWinsNote
      // 
      this.lblPreviousWinsNote.AutoSize = true;
      this.lblPreviousWinsNote.ForeColor = System.Drawing.Color.DimGray;
      this.lblPreviousWinsNote.Location = new System.Drawing.Point(200, 34);
      this.lblPreviousWinsNote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblPreviousWinsNote.Name = "lblPreviousWinsNote";
      this.lblPreviousWinsNote.Size = new System.Drawing.Size(162, 20);
      this.lblPreviousWinsNote.TabIndex = 2;
      this.lblPreviousWinsNote.Text = "(Before using tracker)";
      // 
      // lblPreviousWins
      // 
      this.lblPreviousWins.AutoSize = true;
      this.lblPreviousWins.Location = new System.Drawing.Point(16, 34);
      this.lblPreviousWins.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblPreviousWins.Name = "lblPreviousWins";
      this.lblPreviousWins.Size = new System.Drawing.Size(108, 20);
      this.lblPreviousWins.TabIndex = 0;
      this.lblPreviousWins.Text = "Previous Wins";
      // 
      // txtPreviousWins
      // 
      this.txtPreviousWins.Location = new System.Drawing.Point(138, 29);
      this.txtPreviousWins.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtPreviousWins.MaxLength = 4;
      this.txtPreviousWins.Name = "txtPreviousWins";
      this.txtPreviousWins.Size = new System.Drawing.Size(50, 26);
      this.txtPreviousWins.TabIndex = 1;
      this.txtPreviousWins.Text = "0";
      this.txtPreviousWins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.txtPreviousWins.Validating += new System.ComponentModel.CancelEventHandler(this.txtPreviousWins_Validating);
      // 
      // grpSortingOptions
      // 
      this.grpSortingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grpSortingOptions.Controls.Add(this.chkIgnoreLevelTypeWhenSorting);
      this.grpSortingOptions.Location = new System.Drawing.Point(18, 621);
      this.grpSortingOptions.Name = "grpSortingOptions";
      this.grpSortingOptions.Size = new System.Drawing.Size(968, 62);
      this.grpSortingOptions.TabIndex = 6;
      this.grpSortingOptions.TabStop = false;
      this.grpSortingOptions.Text = "Sorting Options";
      // 
      // chkIgnoreLevelTypeWhenSorting
      // 
      this.chkIgnoreLevelTypeWhenSorting.AutoSize = true;
      this.chkIgnoreLevelTypeWhenSorting.Location = new System.Drawing.Point(24, 26);
      this.chkIgnoreLevelTypeWhenSorting.Name = "chkIgnoreLevelTypeWhenSorting";
      this.chkIgnoreLevelTypeWhenSorting.Size = new System.Drawing.Size(254, 24);
      this.chkIgnoreLevelTypeWhenSorting.TabIndex = 0;
      this.chkIgnoreLevelTypeWhenSorting.Text = "Ignore Level Type when sorting";
      this.chkIgnoreLevelTypeWhenSorting.UseVisualStyleBackColor = true;
      // 
      // fileSystemWatcher1
      // 
      this.fileSystemWatcher1.EnableRaisingEvents = true;
      this.fileSystemWatcher1.SynchronizingObject = this;
      // 
      // txtGameExeLocation
      // 
      this.txtGameExeLocation.Enabled = false;
      this.txtGameExeLocation.Location = new System.Drawing.Point(210, 22);
      this.txtGameExeLocation.Name = "txtGameExeLocation";
      this.txtGameExeLocation.Size = new System.Drawing.Size(669, 26);
      this.txtGameExeLocation.TabIndex = 0;
      // 
      // btnGameExeLocationBrowse
      // 
      this.btnGameExeLocationBrowse.Location = new System.Drawing.Point(885, 20);
      this.btnGameExeLocationBrowse.Name = "btnGameExeLocationBrowse";
      this.btnGameExeLocationBrowse.Size = new System.Drawing.Size(75, 30);
      this.btnGameExeLocationBrowse.TabIndex = 0;
      this.btnGameExeLocationBrowse.Text = "Browse";
      // 
      // chkAutoLaunchGameOnStart
      // 
      this.chkAutoLaunchGameOnStart.Location = new System.Drawing.Point(20, 60);
      this.chkAutoLaunchGameOnStart.Name = "chkAutoLaunchGameOnStart";
      this.chkAutoLaunchGameOnStart.Size = new System.Drawing.Size(306, 24);
      this.chkAutoLaunchGameOnStart.TabIndex = 0;
      this.chkAutoLaunchGameOnStart.Text = "Auto-launch Fall Guys on tracker start";
      // 
      // grpGameOptions
      // 
      this.grpGameOptions.Controls.Add(this.lblGameExeLocation);
      this.grpGameOptions.Controls.Add(this.txtGameExeLocation);
      this.grpGameOptions.Controls.Add(this.btnGameExeLocationBrowse);
      this.grpGameOptions.Controls.Add(this.chkAutoLaunchGameOnStart);
      this.grpGameOptions.Location = new System.Drawing.Point(18, 689);
      this.grpGameOptions.Name = "grpGameOptions";
      this.grpGameOptions.Size = new System.Drawing.Size(966, 93);
      this.grpGameOptions.TabIndex = 0;
      this.grpGameOptions.TabStop = false;
      this.grpGameOptions.Text = "Game Options";
      // 
      // lblGameExeLocation
      // 
      this.lblGameExeLocation.AutoSize = true;
      this.lblGameExeLocation.Location = new System.Drawing.Point(16, 25);
      this.lblGameExeLocation.Name = "lblGameExeLocation";
      this.lblGameExeLocation.Size = new System.Drawing.Size(188, 20);
      this.lblGameExeLocation.TabIndex = 1;
      this.lblGameExeLocation.Text = "Fall Guys Game Location";
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btnCancel.Location = new System.Drawing.Point(872, 791);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(112, 35);
      this.btnCancel.TabIndex = 7;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // Settings
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
      this.ClientSize = new System.Drawing.Size(1004, 840);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.grpGameOptions);
      this.Controls.Add(this.grpSortingOptions);
      this.Controls.Add(this.grpOverlay);
      this.Controls.Add(this.grpStats);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.txtLogPath);
      this.Controls.Add(this.lblLogPathNote);
      this.Controls.Add(this.lblLogPath);
      this.ForeColor = System.Drawing.Color.Black;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Settings";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Settings";
      this.Load += new System.EventHandler(this.Settings_Load);
      this.grpOverlay.ResumeLayout(false);
      this.grpOverlay.PerformLayout();
      this.grpStats.ResumeLayout(false);
      this.grpStats.PerformLayout();
      this.grpSortingOptions.ResumeLayout(false);
      this.grpSortingOptions.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
      this.grpGameOptions.ResumeLayout(false);
      this.grpGameOptions.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLogPath;
        private System.Windows.Forms.Label lblLogPathNote;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkCycleOverlayQualify;
        private System.Windows.Forms.CheckBox chkCycleOverlayLongest;
        private System.Windows.Forms.GroupBox grpOverlay;
        private System.Windows.Forms.Label lblCycleTimeSecondsTag;
        private System.Windows.Forms.Label lblCycleTimeSeconds;
        private System.Windows.Forms.TextBox txtCycleTimeSeconds;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.Label lblPreviousWinsNote;
        private System.Windows.Forms.Label lblPreviousWins;
        private System.Windows.Forms.TextBox txtPreviousWins;
        private System.Windows.Forms.CheckBox chkUseNDI;
        private System.Windows.Forms.CheckBox chkOverlayOnTop;
        private System.Windows.Forms.ComboBox cboFastestFilter;
        private System.Windows.Forms.Label lblFastestFilter;
        private System.Windows.Forms.ComboBox cboQualifyFilter;
        private System.Windows.Forms.Label lblQualifyFilter;
        private System.Windows.Forms.ComboBox cboWinsFilter;
        private System.Windows.Forms.Label lblWinsFilter;
        private System.Windows.Forms.CheckBox chkHideTimeInfo;
        private System.Windows.Forms.CheckBox chkHideRoundInfo;
        private System.Windows.Forms.CheckBox chkShowTabs;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.ComboBox cboOverlayColor;
        private System.Windows.Forms.Label lblOverlayColor;
        private System.Windows.Forms.CheckBox chkFlipped;
        private System.Windows.Forms.CheckBox chkHideWinsInfo;
        private System.Windows.Forms.CheckBox chkHidePercentages;
        private System.Windows.Forms.CheckBox chkCycleOverlayPlayers;
        private System.Windows.Forms.CheckBox chkCycleOverlayStreak;
        private System.Windows.Forms.CheckBox chkChangeHoopsieLegends;
        private System.Windows.Forms.Label lblOrPing;
        private System.Windows.Forms.CheckBox chkOnlyShowPing;
        private System.Windows.Forms.Label lblOrFinal;
        private System.Windows.Forms.CheckBox chkOnlyShowFinalStreak;
        private System.Windows.Forms.Label lblOrLongest;
        private System.Windows.Forms.CheckBox chkOnlyShowLongest;
        private System.Windows.Forms.Label lblOrGold;
        private System.Windows.Forms.CheckBox chkOnlyShowGold;
        private System.Windows.Forms.Button btnGameExeLocationBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtGameExeLocation;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.OpenFileDialog dlgGameExeBrowser;
        private System.Windows.Forms.CheckBox chkAutoLaunchGameOnStart;
        private System.Windows.Forms.GroupBox grpSortingOptions;
        private System.Windows.Forms.CheckBox chkIgnoreLevelTypeWhenSorting;
        private System.Windows.Forms.GroupBox grpGameOptions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblGameExeLocation;
    }
}