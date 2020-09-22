namespace FallGuysStats {
    partial class Overlay {
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
            this.lblFilter = new FallGuysStats.TransparentLabel();
            this.lblStreak = new FallGuysStats.TransparentLabel();
            this.lblFinals = new FallGuysStats.TransparentLabel();
            this.lblQualifyChance = new FallGuysStats.TransparentLabel();
            this.lblFastest = new FallGuysStats.TransparentLabel();
            this.lblDuration = new FallGuysStats.TransparentLabel();
            this.lblPlayers = new FallGuysStats.TransparentLabel();
            this.lblName = new FallGuysStats.TransparentLabel();
            this.lblWins = new FallGuysStats.TransparentLabel();
            this.lblFinish = new FallGuysStats.TransparentLabel();
            this.SuspendLayout();
            // 
            // lblFilter
            // 
            this.lblFilter.Location = new System.Drawing.Point(22, 77);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(110, 22);
            this.lblFilter.TabIndex = 22;
            this.lblFilter.Text = "SEASON";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblFilter.TextRight = "";
            this.lblFilter.Visible = false;
            // 
            // lblStreak
            // 
            this.lblStreak.Location = new System.Drawing.Point(22, 55);
            this.lblStreak.Name = "lblStreak";
            this.lblStreak.Size = new System.Drawing.Size(238, 22);
            this.lblStreak.TabIndex = 21;
            this.lblStreak.Text = "STREAK:";
            this.lblStreak.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblStreak.TextRight = "0 (BEST 0)";
            this.lblStreak.Visible = false;
            // 
            // lblFinals
            // 
            this.lblFinals.Location = new System.Drawing.Point(22, 32);
            this.lblFinals.Name = "lblFinals";
            this.lblFinals.Size = new System.Drawing.Size(238, 22);
            this.lblFinals.TabIndex = 5;
            this.lblFinals.Text = "FINAL:";
            this.lblFinals.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblFinals.TextRight = "0 - 0.0%";
            this.lblFinals.Visible = false;
            // 
            // lblQualifyChance
            // 
            this.lblQualifyChance.Location = new System.Drawing.Point(268, 32);
            this.lblQualifyChance.Name = "lblQualifyChance";
            this.lblQualifyChance.Size = new System.Drawing.Size(281, 22);
            this.lblQualifyChance.TabIndex = 14;
            this.lblQualifyChance.Text = "QUALIFY:";
            this.lblQualifyChance.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblQualifyChance.TextRight = "0 / 0 - 0.0%";
            this.lblQualifyChance.Visible = false;
            // 
            // lblFastest
            // 
            this.lblFastest.Location = new System.Drawing.Point(268, 55);
            this.lblFastest.Name = "lblFastest";
            this.lblFastest.Size = new System.Drawing.Size(281, 22);
            this.lblFastest.TabIndex = 16;
            this.lblFastest.Text = "FASTEST:";
            this.lblFastest.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblFastest.TextRight = "-";
            this.lblFastest.Visible = false;
            // 
            // lblDuration
            // 
            this.lblDuration.Location = new System.Drawing.Point(557, 32);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(225, 22);
            this.lblDuration.TabIndex = 18;
            this.lblDuration.Text = "TIME:";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblDuration.TextRight = "-";
            this.lblDuration.Visible = false;
            // 
            // lblPlayers
            // 
            this.lblPlayers.Location = new System.Drawing.Point(557, 9);
            this.lblPlayers.Name = "lblPlayers";
            this.lblPlayers.Size = new System.Drawing.Size(225, 22);
            this.lblPlayers.TabIndex = 12;
            this.lblPlayers.Text = "PLAYERS:";
            this.lblPlayers.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblPlayers.TextRight = "0";
            this.lblPlayers.Visible = false;
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.Location = new System.Drawing.Point(268, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(281, 22);
            this.lblName.TabIndex = 10;
            this.lblName.Text = "ROUND 1:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblName.TextRight = "N/A";
            this.lblName.Visible = false;
            // 
            // lblWins
            // 
            this.lblWins.Location = new System.Drawing.Point(22, 9);
            this.lblWins.Name = "lblWins";
            this.lblWins.Size = new System.Drawing.Size(238, 22);
            this.lblWins.TabIndex = 1;
            this.lblWins.Text = "WINS:";
            this.lblWins.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblWins.TextRight = "0 - 0.0%";
            this.lblWins.Visible = false;
            // 
            // lblFinish
            // 
            this.lblFinish.Location = new System.Drawing.Point(557, 55);
            this.lblFinish.Name = "lblFinish";
            this.lblFinish.Size = new System.Drawing.Size(225, 22);
            this.lblFinish.TabIndex = 20;
            this.lblFinish.Text = "FINISH:";
            this.lblFinish.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblFinish.TextRight = "-";
            this.lblFinish.Visible = false;
            // 
            // Overlay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Magenta;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(786, 99);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lblStreak);
            this.Controls.Add(this.lblFinals);
            this.Controls.Add(this.lblQualifyChance);
            this.Controls.Add(this.lblFastest);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblPlayers);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblWins);
            this.Controls.Add(this.lblFinish);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Overlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Overlay_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
        private TransparentLabel lblName;
        private TransparentLabel lblDuration;
        private TransparentLabel lblFinish;
        private TransparentLabel lblFastest;
        private TransparentLabel lblQualifyChance;
        private TransparentLabel lblWins;
        private TransparentLabel lblFinals;
        private TransparentLabel lblPlayers;
        private TransparentLabel lblStreak;
        private TransparentLabel lblFilter;
    }
}