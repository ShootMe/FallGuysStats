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
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.lblPreviousWinsNote = new System.Windows.Forms.Label();
            this.lblPreviousWins = new System.Windows.Forms.Label();
            this.txtPreviousWins = new System.Windows.Forms.TextBox();
            this.chkHidePercentages = new System.Windows.Forms.CheckBox();
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
            this.txtLogPath.Size = new System.Drawing.Size(538, 20);
            this.txtLogPath.TabIndex = 1;
            this.txtLogPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLogPath_Validating);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSave.Location = new System.Drawing.Point(269, 352);
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
            this.chkCycleOverlayQualify.Location = new System.Drawing.Point(16, 151);
            this.chkCycleOverlayQualify.Name = "chkCycleOverlayQualify";
            this.chkCycleOverlayQualify.Size = new System.Drawing.Size(120, 17);
            this.chkCycleOverlayQualify.TabIndex = 5;
            this.chkCycleOverlayQualify.Text = "Cycle Qualify / Gold";
            this.chkCycleOverlayQualify.UseVisualStyleBackColor = true;
            // 
            // chkCycleOverlayLongest
            // 
            this.chkCycleOverlayLongest.AutoSize = true;
            this.chkCycleOverlayLongest.Location = new System.Drawing.Point(16, 174);
            this.chkCycleOverlayLongest.Name = "chkCycleOverlayLongest";
            this.chkCycleOverlayLongest.Size = new System.Drawing.Size(138, 17);
            this.chkCycleOverlayLongest.TabIndex = 6;
            this.chkCycleOverlayLongest.Text = "Cycle Fastest / Longest";
            this.chkCycleOverlayLongest.UseVisualStyleBackColor = true;
            // 
            // grpOverlay
            // 
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
            this.grpOverlay.Location = new System.Drawing.Point(12, 114);
            this.grpOverlay.Name = "grpOverlay";
            this.grpOverlay.Size = new System.Drawing.Size(590, 225);
            this.grpOverlay.TabIndex = 4;
            this.grpOverlay.TabStop = false;
            this.grpOverlay.Text = "Overlay";
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
            this.cboOverlayColor.Location = new System.Drawing.Point(350, 120);
            this.cboOverlayColor.Name = "cboOverlayColor";
            this.cboOverlayColor.Size = new System.Drawing.Size(183, 21);
            this.cboOverlayColor.TabIndex = 17;
            // 
            // lblOverlayColor
            // 
            this.lblOverlayColor.AutoSize = true;
            this.lblOverlayColor.Location = new System.Drawing.Point(252, 123);
            this.lblOverlayColor.Name = "lblOverlayColor";
            this.lblOverlayColor.Size = new System.Drawing.Size(92, 13);
            this.lblOverlayColor.TabIndex = 16;
            this.lblOverlayColor.Text = "Background Color";
            // 
            // chkFlipped
            // 
            this.chkFlipped.AutoSize = true;
            this.chkFlipped.Location = new System.Drawing.Point(350, 151);
            this.chkFlipped.Name = "chkFlipped";
            this.chkFlipped.Size = new System.Drawing.Size(132, 17);
            this.chkFlipped.TabIndex = 18;
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
            this.cboFastestFilter.Location = new System.Drawing.Point(350, 69);
            this.cboFastestFilter.Name = "cboFastestFilter";
            this.cboFastestFilter.Size = new System.Drawing.Size(183, 21);
            this.cboFastestFilter.TabIndex = 15;
            // 
            // lblFastestFilter
            // 
            this.lblFastestFilter.AutoSize = true;
            this.lblFastestFilter.Location = new System.Drawing.Point(229, 72);
            this.lblFastestFilter.Name = "lblFastestFilter";
            this.lblFastestFilter.Size = new System.Drawing.Size(115, 13);
            this.lblFastestFilter.TabIndex = 14;
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
            this.cboQualifyFilter.Location = new System.Drawing.Point(350, 44);
            this.cboQualifyFilter.Name = "cboQualifyFilter";
            this.cboQualifyFilter.Size = new System.Drawing.Size(183, 21);
            this.cboQualifyFilter.TabIndex = 13;
            // 
            // lblQualifyFilter
            // 
            this.lblQualifyFilter.AutoSize = true;
            this.lblQualifyFilter.Location = new System.Drawing.Point(247, 47);
            this.lblQualifyFilter.Name = "lblQualifyFilter";
            this.lblQualifyFilter.Size = new System.Drawing.Size(97, 13);
            this.lblQualifyFilter.TabIndex = 12;
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
            this.cboWinsFilter.Location = new System.Drawing.Point(350, 19);
            this.cboWinsFilter.Name = "cboWinsFilter";
            this.cboWinsFilter.Size = new System.Drawing.Size(183, 21);
            this.cboWinsFilter.TabIndex = 11;
            // 
            // lblWinsFilter
            // 
            this.lblWinsFilter.AutoSize = true;
            this.lblWinsFilter.Location = new System.Drawing.Point(255, 22);
            this.lblWinsFilter.Name = "lblWinsFilter";
            this.lblWinsFilter.Size = new System.Drawing.Size(89, 13);
            this.lblWinsFilter.TabIndex = 10;
            this.lblWinsFilter.Text = "Wins / Final Filter";
            // 
            // chkOverlayOnTop
            // 
            this.chkOverlayOnTop.AutoSize = true;
            this.chkOverlayOnTop.Location = new System.Drawing.Point(350, 174);
            this.chkOverlayOnTop.Name = "chkOverlayOnTop";
            this.chkOverlayOnTop.Size = new System.Drawing.Size(120, 17);
            this.chkOverlayOnTop.TabIndex = 19;
            this.chkOverlayOnTop.Text = "Always show on top";
            this.chkOverlayOnTop.UseVisualStyleBackColor = true;
            // 
            // chkUseNDI
            // 
            this.chkUseNDI.AutoSize = true;
            this.chkUseNDI.Location = new System.Drawing.Point(350, 197);
            this.chkUseNDI.Name = "chkUseNDI";
            this.chkUseNDI.Size = new System.Drawing.Size(234, 17);
            this.chkUseNDI.TabIndex = 20;
            this.chkUseNDI.Text = "Use NDI to send Overlay over local network";
            this.chkUseNDI.UseVisualStyleBackColor = true;
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(140, 200);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(24, 13);
            this.lblCycleTimeSecondsTag.TabIndex = 9;
            this.lblCycleTimeSecondsTag.Text = "sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(34, 200);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(59, 13);
            this.lblCycleTimeSeconds.TabIndex = 7;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(99, 197);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(35, 20);
            this.txtCycleTimeSeconds.TabIndex = 8;
            this.txtCycleTimeSeconds.Text = "5";
            this.txtCycleTimeSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
            // 
            // grpStats
            // 
            this.grpStats.Controls.Add(this.chkAutoUpdate);
            this.grpStats.Controls.Add(this.lblPreviousWinsNote);
            this.grpStats.Controls.Add(this.lblPreviousWins);
            this.grpStats.Controls.Add(this.txtPreviousWins);
            this.grpStats.Location = new System.Drawing.Point(12, 58);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(590, 50);
            this.grpStats.TabIndex = 3;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(350, 22);
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
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(614, 390);
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
    }
}