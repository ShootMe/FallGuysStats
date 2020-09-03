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
            this.chkCycleOverlayLongest = new System.Windows.Forms.CheckBox();
            this.grpOverlay = new System.Windows.Forms.GroupBox();
            this.chkUseNDI = new System.Windows.Forms.CheckBox();
            this.lblCycleTimeSecondsTag = new System.Windows.Forms.Label();
            this.lblCycleTimeSeconds = new System.Windows.Forms.Label();
            this.txtCycleTimeSeconds = new System.Windows.Forms.TextBox();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.lblPreviousWinsNote = new System.Windows.Forms.Label();
            this.lblPreviousWins = new System.Windows.Forms.Label();
            this.txtPreviousWins = new System.Windows.Forms.TextBox();
            this.chkOverlayOnTop = new System.Windows.Forms.CheckBox();
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
            this.btnSave.Location = new System.Drawing.Point(269, 204);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkCycleOverlayLongest
            // 
            this.chkCycleOverlayLongest.AutoSize = true;
            this.chkCycleOverlayLongest.Location = new System.Drawing.Point(17, 44);
            this.chkCycleOverlayLongest.Name = "chkCycleOverlayLongest";
            this.chkCycleOverlayLongest.Size = new System.Drawing.Size(221, 17);
            this.chkCycleOverlayLongest.TabIndex = 1;
            this.chkCycleOverlayLongest.Text = "Cycle Between Fastest and Longest stats";
            this.chkCycleOverlayLongest.UseVisualStyleBackColor = true;
            // 
            // grpOverlay
            // 
            this.grpOverlay.Controls.Add(this.chkOverlayOnTop);
            this.grpOverlay.Controls.Add(this.chkUseNDI);
            this.grpOverlay.Controls.Add(this.lblCycleTimeSecondsTag);
            this.grpOverlay.Controls.Add(this.lblCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.txtCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.chkCycleOverlayLongest);
            this.grpOverlay.Location = new System.Drawing.Point(314, 70);
            this.grpOverlay.Name = "grpOverlay";
            this.grpOverlay.Size = new System.Drawing.Size(288, 119);
            this.grpOverlay.TabIndex = 4;
            this.grpOverlay.TabStop = false;
            this.grpOverlay.Text = "Overlay";
            // 
            // chkUseNDI
            // 
            this.chkUseNDI.AutoSize = true;
            this.chkUseNDI.Location = new System.Drawing.Point(17, 93);
            this.chkUseNDI.Name = "chkUseNDI";
            this.chkUseNDI.Size = new System.Drawing.Size(234, 17);
            this.chkUseNDI.TabIndex = 5;
            this.chkUseNDI.Text = "Use NDI to send Overlay over local network";
            this.chkUseNDI.UseVisualStyleBackColor = true;
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(140, 70);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(24, 13);
            this.lblCycleTimeSecondsTag.TabIndex = 4;
            this.lblCycleTimeSecondsTag.Text = "sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(36, 70);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(59, 13);
            this.lblCycleTimeSeconds.TabIndex = 2;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(101, 67);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(35, 20);
            this.txtCycleTimeSeconds.TabIndex = 3;
            this.txtCycleTimeSeconds.Text = "3";
            this.txtCycleTimeSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
            // 
            // grpStats
            // 
            this.grpStats.Controls.Add(this.lblPreviousWinsNote);
            this.grpStats.Controls.Add(this.lblPreviousWins);
            this.grpStats.Controls.Add(this.txtPreviousWins);
            this.grpStats.Location = new System.Drawing.Point(50, 70);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(258, 50);
            this.grpStats.TabIndex = 3;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
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
            // chkOverlayOnTop
            // 
            this.chkOverlayOnTop.AutoSize = true;
            this.chkOverlayOnTop.Location = new System.Drawing.Point(17, 21);
            this.chkOverlayOnTop.Name = "chkOverlayOnTop";
            this.chkOverlayOnTop.Size = new System.Drawing.Size(205, 17);
            this.chkOverlayOnTop.TabIndex = 0;
            this.chkOverlayOnTop.Text = "Always show on top of other programs";
            this.chkOverlayOnTop.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(614, 239);
            this.Controls.Add(this.grpStats);
            this.Controls.Add(this.grpOverlay);
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
    }
}