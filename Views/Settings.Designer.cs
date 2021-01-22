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
            this.lblLogPath = new System.Windows.Forms.Label();
            this.lblLogPathNote = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpOverlay = new System.Windows.Forms.GroupBox();
            this.grpCycleQualifyGold = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowGold = new System.Windows.Forms.RadioButton();
            this.chkOnlyShowQualify = new System.Windows.Forms.RadioButton();
            this.chkCycleQualifyGold = new System.Windows.Forms.RadioButton();
            this.grpCycleFastestLongest = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowLongest = new System.Windows.Forms.RadioButton();
            this.chkOnlyShowFastest = new System.Windows.Forms.RadioButton();
            this.chkCycleFastestLongest = new System.Windows.Forms.RadioButton();
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
            this.lblCycleTimeSecondsTag = new System.Windows.Forms.Label();
            this.lblCycleTimeSeconds = new System.Windows.Forms.Label();
            this.txtCycleTimeSeconds = new System.Windows.Forms.TextBox();
            this.grpCycleWinFinalStreak = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowFinalStreak = new System.Windows.Forms.RadioButton();
            this.chkOnlyShowWinStreak = new System.Windows.Forms.RadioButton();
            this.chkCycleWinFinalStreak = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowPing = new System.Windows.Forms.RadioButton();
            this.chkOnlyShowPlayers = new System.Windows.Forms.RadioButton();
            this.chkCyclePlayersPing = new System.Windows.Forms.RadioButton();
            this.lblOverlayFont = new System.Windows.Forms.Label();
            this.btnSelectFont = new System.Windows.Forms.Button();
            this.btnResetOverlayFont = new System.Windows.Forms.Button();
            this.grpOverlayFontExample = new System.Windows.Forms.GroupBox();
            this.lblOverlayFontExample = new System.Windows.Forms.Label();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.chkChangeHoopsieLegends = new System.Windows.Forms.CheckBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.lblPreviousWinsNote = new System.Windows.Forms.Label();
            this.lblPreviousWins = new System.Windows.Forms.Label();
            this.txtPreviousWins = new System.Windows.Forms.TextBox();
            this.grpGameOptions = new System.Windows.Forms.GroupBox();
            this.lblGameExeLocation = new System.Windows.Forms.Label();
            this.txtGameExeLocation = new System.Windows.Forms.TextBox();
            this.btnGameExeLocationBrowse = new System.Windows.Forms.Button();
            this.chkAutoLaunchGameOnStart = new System.Windows.Forms.CheckBox();
            this.grpSortingOptions = new System.Windows.Forms.GroupBox();
            this.chkIgnoreLevelTypeWhenSorting = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dlgOverlayFont = new System.Windows.Forms.FontDialog();
            this.grpOverlay.SuspendLayout();
            this.grpCycleQualifyGold.SuspendLayout();
            this.grpCycleFastestLongest.SuspendLayout();
            this.grpCycleWinFinalStreak.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpOverlayFontExample.SuspendLayout();
            this.grpStats.SuspendLayout();
            this.grpGameOptions.SuspendLayout();
            this.grpSortingOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLogPath
            // 
            this.lblLogPath.AutoSize = true;
            this.lblLogPath.Location = new System.Drawing.Point(8, 15);
            this.lblLogPath.Name = "lblLogPath";
            this.lblLogPath.Size = new System.Drawing.Size(50, 13);
            this.lblLogPath.TabIndex = 0;
            this.lblLogPath.Text = "Log Path";
            // 
            // lblLogPathNote
            // 
            this.lblLogPathNote.AutoSize = true;
            this.lblLogPathNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblLogPathNote.Location = new System.Drawing.Point(61, 35);
            this.lblLogPathNote.Name = "lblLogPathNote";
            this.lblLogPathNote.Size = new System.Drawing.Size(458, 13);
            this.lblLogPathNote.TabIndex = 2;
            this.lblLogPathNote.Text = "* You should not need to set this. Only use when the program is not reading the c" +
    "orrect location.";
            // 
            // txtLogPath
            // 
            this.txtLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogPath.Location = new System.Drawing.Point(64, 12);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.Size = new System.Drawing.Size(593, 20);
            this.txtLogPath.TabIndex = 1;
            this.txtLogPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLogPath_Validating);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(500, 596);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpOverlay
            // 
            this.grpOverlay.Controls.Add(this.grpCycleQualifyGold);
            this.grpOverlay.Controls.Add(this.grpCycleFastestLongest);
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
            this.grpOverlay.Controls.Add(this.lblCycleTimeSecondsTag);
            this.grpOverlay.Controls.Add(this.lblCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.txtCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.grpCycleWinFinalStreak);
            this.grpOverlay.Controls.Add(this.groupBox1);
            this.grpOverlay.Controls.Add(this.lblOverlayFont);
            this.grpOverlay.Controls.Add(this.btnSelectFont);
            this.grpOverlay.Controls.Add(this.btnResetOverlayFont);
            this.grpOverlay.Controls.Add(this.grpOverlayFontExample);
            this.grpOverlay.Location = new System.Drawing.Point(12, 115);
            this.grpOverlay.Name = "grpOverlay";
            this.grpOverlay.Size = new System.Drawing.Size(645, 352);
            this.grpOverlay.TabIndex = 4;
            this.grpOverlay.TabStop = false;
            this.grpOverlay.Text = "Overlay";
            // 
            // grpCycleQualifyGold
            // 
            this.grpCycleQualifyGold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleQualifyGold.Controls.Add(this.chkOnlyShowGold);
            this.grpCycleQualifyGold.Controls.Add(this.chkOnlyShowQualify);
            this.grpCycleQualifyGold.Controls.Add(this.chkCycleQualifyGold);
            this.grpCycleQualifyGold.Location = new System.Drawing.Point(12, 171);
            this.grpCycleQualifyGold.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleQualifyGold.Name = "grpCycleQualifyGold";
            this.grpCycleQualifyGold.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleQualifyGold.Size = new System.Drawing.Size(369, 32);
            this.grpCycleQualifyGold.TabIndex = 8;
            this.grpCycleQualifyGold.TabStop = false;
            // 
            // chkOnlyShowGold
            // 
            this.chkOnlyShowGold.AutoSize = true;
            this.chkOnlyShowGold.Location = new System.Drawing.Point(259, 10);
            this.chkOnlyShowGold.Name = "chkOnlyShowGold";
            this.chkOnlyShowGold.Size = new System.Drawing.Size(71, 17);
            this.chkOnlyShowGold.TabIndex = 2;
            this.chkOnlyShowGold.Text = "Gold Only";
            this.chkOnlyShowGold.UseVisualStyleBackColor = true;
            // 
            // chkOnlyShowQualify
            // 
            this.chkOnlyShowQualify.AutoSize = true;
            this.chkOnlyShowQualify.Location = new System.Drawing.Point(151, 10);
            this.chkOnlyShowQualify.Name = "chkOnlyShowQualify";
            this.chkOnlyShowQualify.Size = new System.Drawing.Size(81, 17);
            this.chkOnlyShowQualify.TabIndex = 1;
            this.chkOnlyShowQualify.Text = "Qualify Only";
            this.chkOnlyShowQualify.UseVisualStyleBackColor = true;
            // 
            // chkCycleQualifyGold
            // 
            this.chkCycleQualifyGold.AutoSize = true;
            this.chkCycleQualifyGold.Checked = true;
            this.chkCycleQualifyGold.Location = new System.Drawing.Point(5, 10);
            this.chkCycleQualifyGold.Name = "chkCycleQualifyGold";
            this.chkCycleQualifyGold.Size = new System.Drawing.Size(119, 17);
            this.chkCycleQualifyGold.TabIndex = 0;
            this.chkCycleQualifyGold.TabStop = true;
            this.chkCycleQualifyGold.Text = "Cycle Qualify / Gold";
            this.chkCycleQualifyGold.UseVisualStyleBackColor = true;
            // 
            // grpCycleFastestLongest
            // 
            this.grpCycleFastestLongest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleFastestLongest.Controls.Add(this.chkOnlyShowLongest);
            this.grpCycleFastestLongest.Controls.Add(this.chkOnlyShowFastest);
            this.grpCycleFastestLongest.Controls.Add(this.chkCycleFastestLongest);
            this.grpCycleFastestLongest.Location = new System.Drawing.Point(12, 196);
            this.grpCycleFastestLongest.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleFastestLongest.Name = "grpCycleFastestLongest";
            this.grpCycleFastestLongest.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleFastestLongest.Size = new System.Drawing.Size(369, 32);
            this.grpCycleFastestLongest.TabIndex = 9;
            this.grpCycleFastestLongest.TabStop = false;
            // 
            // chkOnlyShowLongest
            // 
            this.chkOnlyShowLongest.AutoSize = true;
            this.chkOnlyShowLongest.Location = new System.Drawing.Point(259, 10);
            this.chkOnlyShowLongest.Name = "chkOnlyShowLongest";
            this.chkOnlyShowLongest.Size = new System.Drawing.Size(87, 17);
            this.chkOnlyShowLongest.TabIndex = 2;
            this.chkOnlyShowLongest.Text = "Longest Only";
            this.chkOnlyShowLongest.UseVisualStyleBackColor = true;
            // 
            // chkOnlyShowFastest
            // 
            this.chkOnlyShowFastest.AutoSize = true;
            this.chkOnlyShowFastest.Location = new System.Drawing.Point(151, 10);
            this.chkOnlyShowFastest.Name = "chkOnlyShowFastest";
            this.chkOnlyShowFastest.Size = new System.Drawing.Size(83, 17);
            this.chkOnlyShowFastest.TabIndex = 1;
            this.chkOnlyShowFastest.Text = "Fastest Only";
            this.chkOnlyShowFastest.UseVisualStyleBackColor = true;
            // 
            // chkCycleFastestLongest
            // 
            this.chkCycleFastestLongest.AutoSize = true;
            this.chkCycleFastestLongest.Checked = true;
            this.chkCycleFastestLongest.Location = new System.Drawing.Point(5, 10);
            this.chkCycleFastestLongest.Name = "chkCycleFastestLongest";
            this.chkCycleFastestLongest.Size = new System.Drawing.Size(137, 17);
            this.chkCycleFastestLongest.TabIndex = 0;
            this.chkCycleFastestLongest.TabStop = true;
            this.chkCycleFastestLongest.Text = "Cycle Fastest / Longest";
            this.chkCycleFastestLongest.UseVisualStyleBackColor = true;
            // 
            // chkHidePercentages
            // 
            this.chkHidePercentages.AutoSize = true;
            this.chkHidePercentages.Location = new System.Drawing.Point(16, 90);
            this.chkHidePercentages.Name = "chkHidePercentages";
            this.chkHidePercentages.Size = new System.Drawing.Size(111, 17);
            this.chkHidePercentages.TabIndex = 3;
            this.chkHidePercentages.Text = "Hide Percentages";
            this.chkHidePercentages.UseVisualStyleBackColor = true;
            // 
            // chkHideWinsInfo
            // 
            this.chkHideWinsInfo.AutoSize = true;
            this.chkHideWinsInfo.Location = new System.Drawing.Point(16, 21);
            this.chkHideWinsInfo.Name = "chkHideWinsInfo";
            this.chkHideWinsInfo.Size = new System.Drawing.Size(95, 17);
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
            this.cboOverlayColor.Location = new System.Drawing.Point(447, 150);
            this.cboOverlayColor.Name = "cboOverlayColor";
            this.cboOverlayColor.Size = new System.Drawing.Size(183, 21);
            this.cboOverlayColor.TabIndex = 19;
            // 
            // lblOverlayColor
            // 
            this.lblOverlayColor.AutoSize = true;
            this.lblOverlayColor.Location = new System.Drawing.Point(376, 153);
            this.lblOverlayColor.Name = "lblOverlayColor";
            this.lblOverlayColor.Size = new System.Drawing.Size(65, 13);
            this.lblOverlayColor.TabIndex = 18;
            this.lblOverlayColor.Text = "Background";
            // 
            // chkFlipped
            // 
            this.chkFlipped.AutoSize = true;
            this.chkFlipped.Location = new System.Drawing.Point(447, 181);
            this.chkFlipped.Name = "chkFlipped";
            this.chkFlipped.Size = new System.Drawing.Size(132, 17);
            this.chkFlipped.TabIndex = 20;
            this.chkFlipped.Text = "Flip display horizontally";
            this.chkFlipped.UseVisualStyleBackColor = true;
            // 
            // chkShowTabs
            // 
            this.chkShowTabs.AutoSize = true;
            this.chkShowTabs.Location = new System.Drawing.Point(16, 113);
            this.chkShowTabs.Name = "chkShowTabs";
            this.chkShowTabs.Size = new System.Drawing.Size(148, 17);
            this.chkShowTabs.TabIndex = 4;
            this.chkShowTabs.Text = "Show Tab for current filter";
            this.chkShowTabs.UseVisualStyleBackColor = true;
            // 
            // chkHideTimeInfo
            // 
            this.chkHideTimeInfo.AutoSize = true;
            this.chkHideTimeInfo.Location = new System.Drawing.Point(16, 67);
            this.chkHideTimeInfo.Name = "chkHideTimeInfo";
            this.chkHideTimeInfo.Size = new System.Drawing.Size(94, 17);
            this.chkHideTimeInfo.TabIndex = 2;
            this.chkHideTimeInfo.Text = "Hide Time info";
            this.chkHideTimeInfo.UseVisualStyleBackColor = true;
            // 
            // chkHideRoundInfo
            // 
            this.chkHideRoundInfo.AutoSize = true;
            this.chkHideRoundInfo.Location = new System.Drawing.Point(16, 44);
            this.chkHideRoundInfo.Name = "chkHideRoundInfo";
            this.chkHideRoundInfo.Size = new System.Drawing.Size(103, 17);
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
            this.cboFastestFilter.Location = new System.Drawing.Point(447, 69);
            this.cboFastestFilter.Name = "cboFastestFilter";
            this.cboFastestFilter.Size = new System.Drawing.Size(183, 21);
            this.cboFastestFilter.TabIndex = 17;
            // 
            // lblFastestFilter
            // 
            this.lblFastestFilter.AutoSize = true;
            this.lblFastestFilter.Location = new System.Drawing.Point(326, 72);
            this.lblFastestFilter.Name = "lblFastestFilter";
            this.lblFastestFilter.Size = new System.Drawing.Size(115, 13);
            this.lblFastestFilter.TabIndex = 16;
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
            this.cboQualifyFilter.Location = new System.Drawing.Point(447, 44);
            this.cboQualifyFilter.Name = "cboQualifyFilter";
            this.cboQualifyFilter.Size = new System.Drawing.Size(183, 21);
            this.cboQualifyFilter.TabIndex = 15;
            // 
            // lblQualifyFilter
            // 
            this.lblQualifyFilter.AutoSize = true;
            this.lblQualifyFilter.Location = new System.Drawing.Point(344, 47);
            this.lblQualifyFilter.Name = "lblQualifyFilter";
            this.lblQualifyFilter.Size = new System.Drawing.Size(97, 13);
            this.lblQualifyFilter.TabIndex = 14;
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
            this.cboWinsFilter.Location = new System.Drawing.Point(447, 19);
            this.cboWinsFilter.Name = "cboWinsFilter";
            this.cboWinsFilter.Size = new System.Drawing.Size(183, 21);
            this.cboWinsFilter.TabIndex = 13;
            // 
            // lblWinsFilter
            // 
            this.lblWinsFilter.AutoSize = true;
            this.lblWinsFilter.Location = new System.Drawing.Point(352, 22);
            this.lblWinsFilter.Name = "lblWinsFilter";
            this.lblWinsFilter.Size = new System.Drawing.Size(89, 13);
            this.lblWinsFilter.TabIndex = 12;
            this.lblWinsFilter.Text = "Wins / Final Filter";
            // 
            // chkOverlayOnTop
            // 
            this.chkOverlayOnTop.AutoSize = true;
            this.chkOverlayOnTop.Location = new System.Drawing.Point(447, 204);
            this.chkOverlayOnTop.Name = "chkOverlayOnTop";
            this.chkOverlayOnTop.Size = new System.Drawing.Size(120, 17);
            this.chkOverlayOnTop.TabIndex = 21;
            this.chkOverlayOnTop.Text = "Always show on top";
            this.chkOverlayOnTop.UseVisualStyleBackColor = true;
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(125, 151);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(24, 13);
            this.lblCycleTimeSecondsTag.TabIndex = 7;
            this.lblCycleTimeSecondsTag.Text = "sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(19, 151);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(59, 13);
            this.lblCycleTimeSeconds.TabIndex = 5;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(84, 148);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(35, 20);
            this.txtCycleTimeSeconds.TabIndex = 6;
            this.txtCycleTimeSeconds.Text = "5";
            this.txtCycleTimeSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
            // 
            // grpCycleWinFinalStreak
            // 
            this.grpCycleWinFinalStreak.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleWinFinalStreak.Controls.Add(this.chkOnlyShowFinalStreak);
            this.grpCycleWinFinalStreak.Controls.Add(this.chkOnlyShowWinStreak);
            this.grpCycleWinFinalStreak.Controls.Add(this.chkCycleWinFinalStreak);
            this.grpCycleWinFinalStreak.Location = new System.Drawing.Point(12, 221);
            this.grpCycleWinFinalStreak.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleWinFinalStreak.Name = "grpCycleWinFinalStreak";
            this.grpCycleWinFinalStreak.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleWinFinalStreak.Size = new System.Drawing.Size(369, 32);
            this.grpCycleWinFinalStreak.TabIndex = 10;
            this.grpCycleWinFinalStreak.TabStop = false;
            // 
            // chkOnlyShowFinalStreak
            // 
            this.chkOnlyShowFinalStreak.AutoSize = true;
            this.chkOnlyShowFinalStreak.Location = new System.Drawing.Point(259, 10);
            this.chkOnlyShowFinalStreak.Name = "chkOnlyShowFinalStreak";
            this.chkOnlyShowFinalStreak.Size = new System.Drawing.Size(105, 17);
            this.chkOnlyShowFinalStreak.TabIndex = 2;
            this.chkOnlyShowFinalStreak.Text = "Final Streak Only";
            this.chkOnlyShowFinalStreak.UseVisualStyleBackColor = true;
            // 
            // chkOnlyShowWinStreak
            // 
            this.chkOnlyShowWinStreak.AutoSize = true;
            this.chkOnlyShowWinStreak.Location = new System.Drawing.Point(151, 10);
            this.chkOnlyShowWinStreak.Name = "chkOnlyShowWinStreak";
            this.chkOnlyShowWinStreak.Size = new System.Drawing.Size(102, 17);
            this.chkOnlyShowWinStreak.TabIndex = 1;
            this.chkOnlyShowWinStreak.Text = "Win Streak Only";
            this.chkOnlyShowWinStreak.UseVisualStyleBackColor = true;
            // 
            // chkCycleWinFinalStreak
            // 
            this.chkCycleWinFinalStreak.AutoSize = true;
            this.chkCycleWinFinalStreak.Checked = true;
            this.chkCycleWinFinalStreak.Location = new System.Drawing.Point(5, 10);
            this.chkCycleWinFinalStreak.Name = "chkCycleWinFinalStreak";
            this.chkCycleWinFinalStreak.Size = new System.Drawing.Size(140, 17);
            this.chkCycleWinFinalStreak.TabIndex = 0;
            this.chkCycleWinFinalStreak.TabStop = true;
            this.chkCycleWinFinalStreak.Text = "Cycle Win / Final Streak";
            this.chkCycleWinFinalStreak.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkOnlyShowPing);
            this.groupBox1.Controls.Add(this.chkOnlyShowPlayers);
            this.groupBox1.Controls.Add(this.chkCyclePlayersPing);
            this.groupBox1.Location = new System.Drawing.Point(12, 246);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(369, 32);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // chkOnlyShowPing
            // 
            this.chkOnlyShowPing.AutoSize = true;
            this.chkOnlyShowPing.Location = new System.Drawing.Point(259, 10);
            this.chkOnlyShowPing.Name = "chkOnlyShowPing";
            this.chkOnlyShowPing.Size = new System.Drawing.Size(70, 17);
            this.chkOnlyShowPing.TabIndex = 2;
            this.chkOnlyShowPing.Text = "Ping Only";
            this.chkOnlyShowPing.UseVisualStyleBackColor = true;
            // 
            // chkOnlyShowPlayers
            // 
            this.chkOnlyShowPlayers.AutoSize = true;
            this.chkOnlyShowPlayers.Location = new System.Drawing.Point(151, 10);
            this.chkOnlyShowPlayers.Name = "chkOnlyShowPlayers";
            this.chkOnlyShowPlayers.Size = new System.Drawing.Size(83, 17);
            this.chkOnlyShowPlayers.TabIndex = 1;
            this.chkOnlyShowPlayers.Text = "Players Only";
            this.chkOnlyShowPlayers.UseVisualStyleBackColor = true;
            // 
            // chkCyclePlayersPing
            // 
            this.chkCyclePlayersPing.AutoSize = true;
            this.chkCyclePlayersPing.Checked = true;
            this.chkCyclePlayersPing.Location = new System.Drawing.Point(5, 10);
            this.chkCyclePlayersPing.Name = "chkCyclePlayersPing";
            this.chkCyclePlayersPing.Size = new System.Drawing.Size(120, 17);
            this.chkCyclePlayersPing.TabIndex = 0;
            this.chkCyclePlayersPing.TabStop = true;
            this.chkCyclePlayersPing.Text = "Cycle Players / Ping";
            this.chkCyclePlayersPing.UseVisualStyleBackColor = true;
            // 
            // lblOverlayFont
            // 
            this.lblOverlayFont.AutoSize = true;
            this.lblOverlayFont.Location = new System.Drawing.Point(9, 284);
            this.lblOverlayFont.Name = "lblOverlayFont";
            this.lblOverlayFont.Size = new System.Drawing.Size(105, 13);
            this.lblOverlayFont.TabIndex = 23;
            this.lblOverlayFont.Text = "Custom Overlay Font";
            // 
            // btnSelectFont
            // 
            this.btnSelectFont.Location = new System.Drawing.Point(27, 299);
            this.btnSelectFont.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelectFont.Name = "btnSelectFont";
            this.btnSelectFont.Size = new System.Drawing.Size(70, 20);
            this.btnSelectFont.TabIndex = 24;
            this.btnSelectFont.Text = "Select Font";
            this.btnSelectFont.UseVisualStyleBackColor = true;
            this.btnSelectFont.Click += new System.EventHandler(this.btnSelectFont_Click);
            // 
            // btnResetOverlayFont
            // 
            this.btnResetOverlayFont.Location = new System.Drawing.Point(27, 323);
            this.btnResetOverlayFont.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetOverlayFont.Name = "btnResetOverlayFont";
            this.btnResetOverlayFont.Size = new System.Drawing.Size(70, 20);
            this.btnResetOverlayFont.TabIndex = 25;
            this.btnResetOverlayFont.Text = "Reset Font";
            this.btnResetOverlayFont.UseVisualStyleBackColor = true;
            this.btnResetOverlayFont.Click += new System.EventHandler(this.btnResetOverlayFont_Click);
            // 
            // grpOverlayFontExample
            // 
            this.grpOverlayFontExample.Controls.Add(this.lblOverlayFontExample);
            this.grpOverlayFontExample.Location = new System.Drawing.Point(119, 284);
            this.grpOverlayFontExample.Margin = new System.Windows.Forms.Padding(2);
            this.grpOverlayFontExample.Name = "grpOverlayFontExample";
            this.grpOverlayFontExample.Padding = new System.Windows.Forms.Padding(2);
            this.grpOverlayFontExample.Size = new System.Drawing.Size(517, 59);
            this.grpOverlayFontExample.TabIndex = 35;
            this.grpOverlayFontExample.TabStop = false;
            this.grpOverlayFontExample.Text = "Example";
            // 
            // lblOverlayFontExample
            // 
            this.lblOverlayFontExample.Location = new System.Drawing.Point(4, 15);
            this.lblOverlayFontExample.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOverlayFontExample.Name = "lblOverlayFontExample";
            this.lblOverlayFontExample.Size = new System.Drawing.Size(509, 34);
            this.lblOverlayFontExample.TabIndex = 0;
            this.lblOverlayFontExample.Text = "Round 3: Freezy Peak";
            this.lblOverlayFontExample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.grpStats.Location = new System.Drawing.Point(12, 58);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(645, 51);
            this.grpStats.TabIndex = 3;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
            // 
            // chkChangeHoopsieLegends
            // 
            this.chkChangeHoopsieLegends.AutoSize = true;
            this.chkChangeHoopsieLegends.Location = new System.Drawing.Point(390, 21);
            this.chkChangeHoopsieLegends.Name = "chkChangeHoopsieLegends";
            this.chkChangeHoopsieLegends.Size = new System.Drawing.Size(243, 17);
            this.chkChangeHoopsieLegends.TabIndex = 4;
            this.chkChangeHoopsieLegends.Text = "Rename Hoopsie Legends to Hoopsie Heroes";
            this.chkChangeHoopsieLegends.UseVisualStyleBackColor = true;
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(254, 21);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(128, 17);
            this.chkAutoUpdate.TabIndex = 3;
            this.chkAutoUpdate.Text = "Auto Update Program";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // lblPreviousWinsNote
            // 
            this.lblPreviousWinsNote.AutoSize = true;
            this.lblPreviousWinsNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblPreviousWinsNote.Location = new System.Drawing.Point(133, 22);
            this.lblPreviousWinsNote.Name = "lblPreviousWinsNote";
            this.lblPreviousWinsNote.Size = new System.Drawing.Size(108, 13);
            this.lblPreviousWinsNote.TabIndex = 2;
            this.lblPreviousWinsNote.Text = "(Before using tracker)";
            // 
            // lblPreviousWins
            // 
            this.lblPreviousWins.AutoSize = true;
            this.lblPreviousWins.Location = new System.Drawing.Point(11, 22);
            this.lblPreviousWins.Name = "lblPreviousWins";
            this.lblPreviousWins.Size = new System.Drawing.Size(75, 13);
            this.lblPreviousWins.TabIndex = 0;
            this.lblPreviousWins.Text = "Previous Wins";
            // 
            // txtPreviousWins
            // 
            this.txtPreviousWins.Location = new System.Drawing.Point(92, 19);
            this.txtPreviousWins.MaxLength = 4;
            this.txtPreviousWins.Name = "txtPreviousWins";
            this.txtPreviousWins.Size = new System.Drawing.Size(35, 20);
            this.txtPreviousWins.TabIndex = 1;
            this.txtPreviousWins.Text = "0";
            this.txtPreviousWins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPreviousWins.Validating += new System.ComponentModel.CancelEventHandler(this.txtPreviousWins_Validating);
            // 
            // grpGameOptions
            // 
            this.grpGameOptions.Controls.Add(this.lblGameExeLocation);
            this.grpGameOptions.Controls.Add(this.txtGameExeLocation);
            this.grpGameOptions.Controls.Add(this.btnGameExeLocationBrowse);
            this.grpGameOptions.Controls.Add(this.chkAutoLaunchGameOnStart);
            this.grpGameOptions.Location = new System.Drawing.Point(12, 521);
            this.grpGameOptions.Margin = new System.Windows.Forms.Padding(2);
            this.grpGameOptions.Name = "grpGameOptions";
            this.grpGameOptions.Padding = new System.Windows.Forms.Padding(2);
            this.grpGameOptions.Size = new System.Drawing.Size(644, 69);
            this.grpGameOptions.TabIndex = 6;
            this.grpGameOptions.TabStop = false;
            this.grpGameOptions.Text = "Game Options";
            // 
            // lblGameExeLocation
            // 
            this.lblGameExeLocation.AutoSize = true;
            this.lblGameExeLocation.Location = new System.Drawing.Point(11, 21);
            this.lblGameExeLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGameExeLocation.Name = "lblGameExeLocation";
            this.lblGameExeLocation.Size = new System.Drawing.Size(125, 13);
            this.lblGameExeLocation.TabIndex = 0;
            this.lblGameExeLocation.Text = "Fall Guys Game Location";
            // 
            // txtGameExeLocation
            // 
            this.txtGameExeLocation.Enabled = false;
            this.txtGameExeLocation.Location = new System.Drawing.Point(140, 19);
            this.txtGameExeLocation.Margin = new System.Windows.Forms.Padding(2);
            this.txtGameExeLocation.Name = "txtGameExeLocation";
            this.txtGameExeLocation.Size = new System.Drawing.Size(447, 20);
            this.txtGameExeLocation.TabIndex = 1;
            // 
            // btnGameExeLocationBrowse
            // 
            this.btnGameExeLocationBrowse.Location = new System.Drawing.Point(589, 18);
            this.btnGameExeLocationBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.btnGameExeLocationBrowse.Name = "btnGameExeLocationBrowse";
            this.btnGameExeLocationBrowse.Size = new System.Drawing.Size(50, 22);
            this.btnGameExeLocationBrowse.TabIndex = 2;
            this.btnGameExeLocationBrowse.Text = "Browse";
            this.btnGameExeLocationBrowse.UseVisualStyleBackColor = true;
            this.btnGameExeLocationBrowse.Click += new System.EventHandler(this.btnGameExeLocationBrowse_Click);
            // 
            // chkAutoLaunchGameOnStart
            // 
            this.chkAutoLaunchGameOnStart.Location = new System.Drawing.Point(13, 43);
            this.chkAutoLaunchGameOnStart.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoLaunchGameOnStart.Name = "chkAutoLaunchGameOnStart";
            this.chkAutoLaunchGameOnStart.Size = new System.Drawing.Size(204, 16);
            this.chkAutoLaunchGameOnStart.TabIndex = 3;
            this.chkAutoLaunchGameOnStart.Text = "Auto-launch Fall Guys on tracker start";
            // 
            // grpSortingOptions
            // 
            this.grpSortingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSortingOptions.Controls.Add(this.chkIgnoreLevelTypeWhenSorting);
            this.grpSortingOptions.Location = new System.Drawing.Point(12, 473);
            this.grpSortingOptions.Margin = new System.Windows.Forms.Padding(2);
            this.grpSortingOptions.Name = "grpSortingOptions";
            this.grpSortingOptions.Padding = new System.Windows.Forms.Padding(2);
            this.grpSortingOptions.Size = new System.Drawing.Size(645, 44);
            this.grpSortingOptions.TabIndex = 5;
            this.grpSortingOptions.TabStop = false;
            this.grpSortingOptions.Text = "Sorting Options";
            // 
            // chkIgnoreLevelTypeWhenSorting
            // 
            this.chkIgnoreLevelTypeWhenSorting.AutoSize = true;
            this.chkIgnoreLevelTypeWhenSorting.Location = new System.Drawing.Point(16, 19);
            this.chkIgnoreLevelTypeWhenSorting.Margin = new System.Windows.Forms.Padding(2);
            this.chkIgnoreLevelTypeWhenSorting.Name = "chkIgnoreLevelTypeWhenSorting";
            this.chkIgnoreLevelTypeWhenSorting.Size = new System.Drawing.Size(175, 17);
            this.chkIgnoreLevelTypeWhenSorting.TabIndex = 0;
            this.chkIgnoreLevelTypeWhenSorting.Text = "Ignore Level Type when sorting";
            this.chkIgnoreLevelTypeWhenSorting.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(581, 596);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(669, 627);
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
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.grpOverlay.ResumeLayout(false);
            this.grpOverlay.PerformLayout();
            this.grpCycleQualifyGold.ResumeLayout(false);
            this.grpCycleQualifyGold.PerformLayout();
            this.grpCycleFastestLongest.ResumeLayout(false);
            this.grpCycleFastestLongest.PerformLayout();
            this.grpCycleWinFinalStreak.ResumeLayout(false);
            this.grpCycleWinFinalStreak.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpOverlayFontExample.ResumeLayout(false);
            this.grpStats.ResumeLayout(false);
            this.grpStats.PerformLayout();
            this.grpGameOptions.ResumeLayout(false);
            this.grpGameOptions.PerformLayout();
            this.grpSortingOptions.ResumeLayout(false);
            this.grpSortingOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLogPath;
        private System.Windows.Forms.Label lblLogPathNote;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpOverlay;
        private System.Windows.Forms.Label lblCycleTimeSecondsTag;
        private System.Windows.Forms.Label lblCycleTimeSeconds;
        private System.Windows.Forms.TextBox txtCycleTimeSeconds;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.Label lblPreviousWinsNote;
        private System.Windows.Forms.Label lblPreviousWins;
        private System.Windows.Forms.TextBox txtPreviousWins;
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
        private System.Windows.Forms.CheckBox chkChangeHoopsieLegends;
        private System.Windows.Forms.GroupBox grpGameOptions;
        private System.Windows.Forms.Label lblGameExeLocation;
        private System.Windows.Forms.TextBox txtGameExeLocation;
        private System.Windows.Forms.Button btnGameExeLocationBrowse;
        private System.Windows.Forms.CheckBox chkAutoLaunchGameOnStart;
        private System.Windows.Forms.GroupBox grpSortingOptions;
        private System.Windows.Forms.CheckBox chkIgnoreLevelTypeWhenSorting;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpCycleQualifyGold;
        private System.Windows.Forms.RadioButton chkOnlyShowGold;
        private System.Windows.Forms.RadioButton chkOnlyShowQualify;
        private System.Windows.Forms.RadioButton chkCycleQualifyGold;
        private System.Windows.Forms.GroupBox grpCycleFastestLongest;
        private System.Windows.Forms.RadioButton chkOnlyShowLongest;
        private System.Windows.Forms.RadioButton chkOnlyShowFastest;
        private System.Windows.Forms.RadioButton chkCycleFastestLongest;
        private System.Windows.Forms.GroupBox grpCycleWinFinalStreak;
        private System.Windows.Forms.RadioButton chkOnlyShowFinalStreak;
        private System.Windows.Forms.RadioButton chkOnlyShowWinStreak;
        private System.Windows.Forms.RadioButton chkCycleWinFinalStreak;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton chkOnlyShowPing;
        private System.Windows.Forms.RadioButton chkOnlyShowPlayers;
        private System.Windows.Forms.RadioButton chkCyclePlayersPing;
        private System.Windows.Forms.Label lblOverlayFont;
        private System.Windows.Forms.Button btnSelectFont;
        private System.Windows.Forms.FontDialog dlgOverlayFont;
        private System.Windows.Forms.Label lblOverlayFontExample;
        private System.Windows.Forms.GroupBox grpOverlayFontExample;
        private System.Windows.Forms.Button btnResetOverlayFont;
    }
}