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
            this.chkOnlyShowGold = new System.Windows.Forms.CheckBox();
            this.lblOrGold = new System.Windows.Forms.Label();
            this.lblOrLongest = new System.Windows.Forms.Label();
            this.chkOnlyShowLongest = new System.Windows.Forms.CheckBox();
            this.lblOrFinal = new System.Windows.Forms.Label();
            this.chkOnlyShowFinalStreak = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkOnlyShowPing = new System.Windows.Forms.CheckBox();
            this.grpOverlay.SuspendLayout();
            this.grpStats.SuspendLayout();
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
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSave.Location = new System.Drawing.Point(296, 417);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkCycleOverlayQualify
            // 
            this.chkCycleOverlayQualify.AutoSize = true;
            this.chkCycleOverlayQualify.Location = new System.Drawing.Point(16, 188);
            this.chkCycleOverlayQualify.Name = "chkCycleOverlayQualify";
            this.chkCycleOverlayQualify.Size = new System.Drawing.Size(120, 17);
            this.chkCycleOverlayQualify.TabIndex = 8;
            this.chkCycleOverlayQualify.Text = "Cycle Qualify / Gold";
            this.chkCycleOverlayQualify.UseVisualStyleBackColor = true;
            this.chkCycleOverlayQualify.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // chkCycleOverlayLongest
            // 
            this.chkCycleOverlayLongest.AutoSize = true;
            this.chkCycleOverlayLongest.Location = new System.Drawing.Point(16, 211);
            this.chkCycleOverlayLongest.Name = "chkCycleOverlayLongest";
            this.chkCycleOverlayLongest.Size = new System.Drawing.Size(138, 17);
            this.chkCycleOverlayLongest.TabIndex = 11;
            this.chkCycleOverlayLongest.Text = "Cycle Fastest / Longest";
            this.chkCycleOverlayLongest.UseVisualStyleBackColor = true;
            this.chkCycleOverlayLongest.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // grpOverlay
            // 
            this.grpOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOverlay.Controls.Add(this.label1);
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
            this.grpOverlay.Location = new System.Drawing.Point(12, 115);
            this.grpOverlay.Name = "grpOverlay";
            this.grpOverlay.Size = new System.Drawing.Size(645, 290);
            this.grpOverlay.TabIndex = 4;
            this.grpOverlay.TabStop = false;
            this.grpOverlay.Text = "Overlay";
            // 
            // chkCycleOverlayStreak
            // 
            this.chkCycleOverlayStreak.AutoSize = true;
            this.chkCycleOverlayStreak.Location = new System.Drawing.Point(16, 234);
            this.chkCycleOverlayStreak.Name = "chkCycleOverlayStreak";
            this.chkCycleOverlayStreak.Size = new System.Drawing.Size(141, 17);
            this.chkCycleOverlayStreak.TabIndex = 14;
            this.chkCycleOverlayStreak.Text = "Cycle Win / Final Streak";
            this.chkCycleOverlayStreak.UseVisualStyleBackColor = true;
            this.chkCycleOverlayStreak.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // chkCycleOverlayPlayers
            // 
            this.chkCycleOverlayPlayers.AutoSize = true;
            this.chkCycleOverlayPlayers.Location = new System.Drawing.Point(16, 257);
            this.chkCycleOverlayPlayers.Name = "chkCycleOverlayPlayers";
            this.chkCycleOverlayPlayers.Size = new System.Drawing.Size(121, 17);
            this.chkCycleOverlayPlayers.TabIndex = 17;
            this.chkCycleOverlayPlayers.Text = "Cycle Players / Ping";
            this.chkCycleOverlayPlayers.UseVisualStyleBackColor = true;
            this.chkCycleOverlayPlayers.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
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
            this.cboOverlayColor.Location = new System.Drawing.Point(447, 157);
            this.cboOverlayColor.Name = "cboOverlayColor";
            this.cboOverlayColor.Size = new System.Drawing.Size(183, 21);
            this.cboOverlayColor.TabIndex = 27;
            // 
            // lblOverlayColor
            // 
            this.lblOverlayColor.AutoSize = true;
            this.lblOverlayColor.Location = new System.Drawing.Point(376, 160);
            this.lblOverlayColor.Name = "lblOverlayColor";
            this.lblOverlayColor.Size = new System.Drawing.Size(65, 13);
            this.lblOverlayColor.TabIndex = 26;
            this.lblOverlayColor.Text = "Background";
            // 
            // chkFlipped
            // 
            this.chkFlipped.AutoSize = true;
            this.chkFlipped.Location = new System.Drawing.Point(447, 188);
            this.chkFlipped.Name = "chkFlipped";
            this.chkFlipped.Size = new System.Drawing.Size(132, 17);
            this.chkFlipped.TabIndex = 28;
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
            this.cboFastestFilter.TabIndex = 25;
            // 
            // lblFastestFilter
            // 
            this.lblFastestFilter.AutoSize = true;
            this.lblFastestFilter.Location = new System.Drawing.Point(326, 72);
            this.lblFastestFilter.Name = "lblFastestFilter";
            this.lblFastestFilter.Size = new System.Drawing.Size(115, 13);
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
            this.cboQualifyFilter.Location = new System.Drawing.Point(447, 44);
            this.cboQualifyFilter.Name = "cboQualifyFilter";
            this.cboQualifyFilter.Size = new System.Drawing.Size(183, 21);
            this.cboQualifyFilter.TabIndex = 23;
            // 
            // lblQualifyFilter
            // 
            this.lblQualifyFilter.AutoSize = true;
            this.lblQualifyFilter.Location = new System.Drawing.Point(344, 47);
            this.lblQualifyFilter.Name = "lblQualifyFilter";
            this.lblQualifyFilter.Size = new System.Drawing.Size(97, 13);
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
            this.cboWinsFilter.Location = new System.Drawing.Point(447, 19);
            this.cboWinsFilter.Name = "cboWinsFilter";
            this.cboWinsFilter.Size = new System.Drawing.Size(183, 21);
            this.cboWinsFilter.TabIndex = 21;
            // 
            // lblWinsFilter
            // 
            this.lblWinsFilter.AutoSize = true;
            this.lblWinsFilter.Location = new System.Drawing.Point(352, 22);
            this.lblWinsFilter.Name = "lblWinsFilter";
            this.lblWinsFilter.Size = new System.Drawing.Size(89, 13);
            this.lblWinsFilter.TabIndex = 20;
            this.lblWinsFilter.Text = "Wins / Final Filter";
            // 
            // chkOverlayOnTop
            // 
            this.chkOverlayOnTop.AutoSize = true;
            this.chkOverlayOnTop.Location = new System.Drawing.Point(447, 211);
            this.chkOverlayOnTop.Name = "chkOverlayOnTop";
            this.chkOverlayOnTop.Size = new System.Drawing.Size(120, 17);
            this.chkOverlayOnTop.TabIndex = 29;
            this.chkOverlayOnTop.Text = "Always show on top";
            this.chkOverlayOnTop.UseVisualStyleBackColor = true;
            // 
            // chkUseNDI
            // 
            this.chkUseNDI.AutoSize = true;
            this.chkUseNDI.Location = new System.Drawing.Point(447, 234);
            this.chkUseNDI.Name = "chkUseNDI";
            this.chkUseNDI.Size = new System.Drawing.Size(157, 17);
            this.chkUseNDI.TabIndex = 30;
            this.chkUseNDI.Text = "Use NDI to transmit Overlay";
            this.chkUseNDI.UseVisualStyleBackColor = true;
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(125, 160);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(24, 13);
            this.lblCycleTimeSecondsTag.TabIndex = 7;
            this.lblCycleTimeSecondsTag.Text = "sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(19, 160);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(59, 13);
            this.lblCycleTimeSeconds.TabIndex = 5;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(84, 157);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(35, 20);
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
            // chkOnlyShowGold
            // 
            this.chkOnlyShowGold.AutoSize = true;
            this.chkOnlyShowGold.Location = new System.Drawing.Point(193, 188);
            this.chkOnlyShowGold.Name = "chkOnlyShowGold";
            this.chkOnlyShowGold.Size = new System.Drawing.Size(102, 17);
            this.chkOnlyShowGold.TabIndex = 10;
            this.chkOnlyShowGold.Text = "Only Show Gold";
            this.chkOnlyShowGold.UseVisualStyleBackColor = true;
            this.chkOnlyShowGold.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // lblOrGold
            // 
            this.lblOrGold.AutoSize = true;
            this.lblOrGold.ForeColor = System.Drawing.Color.DimGray;
            this.lblOrGold.Location = new System.Drawing.Point(160, 189);
            this.lblOrGold.Name = "lblOrGold";
            this.lblOrGold.Size = new System.Drawing.Size(16, 13);
            this.lblOrGold.TabIndex = 9;
            this.lblOrGold.Text = "or";
            // 
            // lblOrLongest
            // 
            this.lblOrLongest.AutoSize = true;
            this.lblOrLongest.ForeColor = System.Drawing.Color.DimGray;
            this.lblOrLongest.Location = new System.Drawing.Point(160, 212);
            this.lblOrLongest.Name = "lblOrLongest";
            this.lblOrLongest.Size = new System.Drawing.Size(16, 13);
            this.lblOrLongest.TabIndex = 12;
            this.lblOrLongest.Text = "or";
            // 
            // chkOnlyShowLongest
            // 
            this.chkOnlyShowLongest.AutoSize = true;
            this.chkOnlyShowLongest.Location = new System.Drawing.Point(193, 211);
            this.chkOnlyShowLongest.Name = "chkOnlyShowLongest";
            this.chkOnlyShowLongest.Size = new System.Drawing.Size(118, 17);
            this.chkOnlyShowLongest.TabIndex = 13;
            this.chkOnlyShowLongest.Text = "Only Show Longest";
            this.chkOnlyShowLongest.UseVisualStyleBackColor = true;
            this.chkOnlyShowLongest.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // lblOrFinal
            // 
            this.lblOrFinal.AutoSize = true;
            this.lblOrFinal.ForeColor = System.Drawing.Color.DimGray;
            this.lblOrFinal.Location = new System.Drawing.Point(160, 235);
            this.lblOrFinal.Name = "lblOrFinal";
            this.lblOrFinal.Size = new System.Drawing.Size(16, 13);
            this.lblOrFinal.TabIndex = 15;
            this.lblOrFinal.Text = "or";
            // 
            // chkOnlyShowFinalStreak
            // 
            this.chkOnlyShowFinalStreak.AutoSize = true;
            this.chkOnlyShowFinalStreak.Location = new System.Drawing.Point(193, 234);
            this.chkOnlyShowFinalStreak.Name = "chkOnlyShowFinalStreak";
            this.chkOnlyShowFinalStreak.Size = new System.Drawing.Size(136, 17);
            this.chkOnlyShowFinalStreak.TabIndex = 16;
            this.chkOnlyShowFinalStreak.Text = "Only Show Final Streak";
            this.chkOnlyShowFinalStreak.UseVisualStyleBackColor = true;
            this.chkOnlyShowFinalStreak.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(160, 258);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "or";
            // 
            // chkOnlyShowPing
            // 
            this.chkOnlyShowPing.AutoSize = true;
            this.chkOnlyShowPing.Location = new System.Drawing.Point(193, 257);
            this.chkOnlyShowPing.Name = "chkOnlyShowPing";
            this.chkOnlyShowPing.Size = new System.Drawing.Size(101, 17);
            this.chkOnlyShowPing.TabIndex = 19;
            this.chkOnlyShowPing.Text = "Only Show Ping";
            this.chkOnlyShowPing.UseVisualStyleBackColor = true;
            this.chkOnlyShowPing.CheckedChanged += new System.EventHandler(this.chkCycleOnly_CheckedChanged);
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(669, 455);
            this.Controls.Add(this.grpOverlay);
            this.Controls.Add(this.grpStats);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtLogPath);
            this.Controls.Add(this.lblLogPathNote);
            this.Controls.Add(this.lblLogPath);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.grpOverlay.ResumeLayout(false);
            this.grpOverlay.PerformLayout();
            this.grpStats.ResumeLayout(false);
            this.grpStats.PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOnlyShowPing;
        private System.Windows.Forms.Label lblOrFinal;
        private System.Windows.Forms.CheckBox chkOnlyShowFinalStreak;
        private System.Windows.Forms.Label lblOrLongest;
        private System.Windows.Forms.CheckBox chkOnlyShowLongest;
        private System.Windows.Forms.Label lblOrGold;
        private System.Windows.Forms.CheckBox chkOnlyShowGold;
    }
}