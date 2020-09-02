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
            this.lblCycleTimeSecondsTag = new System.Windows.Forms.Label();
            this.lblCycleTimeSeconds = new System.Windows.Forms.Label();
            this.txtCycleTimeSeconds = new System.Windows.Forms.TextBox();
            this.grpOverlay.SuspendLayout();
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
            this.btnSave.Location = new System.Drawing.Point(269, 164);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkCycleOverlayLongest
            // 
            this.chkCycleOverlayLongest.AutoSize = true;
            this.chkCycleOverlayLongest.Location = new System.Drawing.Point(17, 21);
            this.chkCycleOverlayLongest.Name = "chkCycleOverlayLongest";
            this.chkCycleOverlayLongest.Size = new System.Drawing.Size(196, 17);
            this.chkCycleOverlayLongest.TabIndex = 0;
            this.chkCycleOverlayLongest.Text = "Cycle Between Fastest and Longest";
            this.chkCycleOverlayLongest.UseVisualStyleBackColor = true;
            // 
            // grpOverlay
            // 
            this.grpOverlay.Controls.Add(this.lblCycleTimeSecondsTag);
            this.grpOverlay.Controls.Add(this.lblCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.txtCycleTimeSeconds);
            this.grpOverlay.Controls.Add(this.chkCycleOverlayLongest);
            this.grpOverlay.Location = new System.Drawing.Point(64, 74);
            this.grpOverlay.Name = "grpOverlay";
            this.grpOverlay.Size = new System.Drawing.Size(241, 75);
            this.grpOverlay.TabIndex = 3;
            this.grpOverlay.TabStop = false;
            this.grpOverlay.Text = "Overlay";
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(140, 47);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(24, 13);
            this.lblCycleTimeSecondsTag.TabIndex = 3;
            this.lblCycleTimeSecondsTag.Text = "sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(36, 47);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(59, 13);
            this.lblCycleTimeSeconds.TabIndex = 1;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(101, 44);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(35, 20);
            this.txtCycleTimeSeconds.TabIndex = 2;
            this.txtCycleTimeSeconds.Text = "3";
            this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(614, 199);
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
    }
}