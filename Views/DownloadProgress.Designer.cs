namespace FallGuysStats {
    partial class DownloadProgress {
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
            this.lblDownloadDescription = new System.Windows.Forms.Label();
            this.mpbProgressBar = new MetroFramework.Controls.MetroProgressBar();
            this.SuspendLayout();
            // 
            // lblDownloadDescription
            // 
            this.lblDownloadDescription.AutoSize = true;
            this.lblDownloadDescription.Location = new System.Drawing.Point(23, 30);
            this.lblDownloadDescription.Name = "lblDownloadDescription";
            this.lblDownloadDescription.Size = new System.Drawing.Size(21, 12);
            this.lblDownloadDescription.TabIndex = 1;
            this.lblDownloadDescription.Text = "0%";
            this.lblDownloadDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mpbProgressBar
            // 
            this.mpbProgressBar.FontSize = MetroFramework.MetroProgressBarSize.Tall;
            this.mpbProgressBar.FontWeight = MetroFramework.MetroProgressBarWeight.Bold;
            this.mpbProgressBar.HideProgressText = false;
            this.mpbProgressBar.Location = new System.Drawing.Point(22, 65);
            this.mpbProgressBar.Name = "mpbProgressBar";
            this.mpbProgressBar.Size = new System.Drawing.Size(315, 33);
            this.mpbProgressBar.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpbProgressBar.TabIndex = 2;
            this.mpbProgressBar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mpbProgressBar.UseCustomBackColor = true;
            // 
            // Progress
            // 
            this.ClientSize = new System.Drawing.Size(360, 125);
            this.ControlBox = false;
            this.Controls.Add(this.lblDownloadDescription);
            this.Controls.Add(this.mpbProgressBar);
            this.DisplayHeader = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Movable = false;
            this.Name = "DownloadProgress";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Progress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblDownloadDescription;
        private MetroFramework.Controls.MetroProgressBar mpbProgressBar;

        #endregion
        
    }
}