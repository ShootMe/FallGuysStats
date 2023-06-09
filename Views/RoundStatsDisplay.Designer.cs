namespace FallGuysStats {
    partial class RoundStatsDisplay {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoundStatsDisplay));
            this.formsPlot = new ScottPlot.FormsPlot();
            this.picRoundIcon = new System.Windows.Forms.PictureBox();
            this.cboRoundList = new MetroFramework.Controls.MetroComboBox();
            this.lblRoundTime = new MetroFramework.Controls.MetroLabel();
            this.picGoldMedalIcon = new System.Windows.Forms.PictureBox();
            this.lblCountGoldMedal = new System.Windows.Forms.Label();
            this.picSilverMedalIcon = new System.Windows.Forms.PictureBox();
            this.lblCountSilverMedal = new System.Windows.Forms.Label();
            this.picBronzeMedalIcon = new System.Windows.Forms.PictureBox();
            this.lblCountBronzeMedal = new System.Windows.Forms.Label();
            this.picPinkMedalIcon = new System.Windows.Forms.PictureBox();
            this.lblCountPinkMedal = new System.Windows.Forms.Label();
            this.picEliminatedMedalIcon = new System.Windows.Forms.PictureBox();
            this.lblCountEliminatedMedal = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picRoundIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGoldMedalIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSilverMedalIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBronzeMedalIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPinkMedalIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEliminatedMedalIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // formsPlot
            // 
            this.formsPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.formsPlot.Location = new System.Drawing.Point(0, 58);
            this.formsPlot.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.formsPlot.Name = "formsPlot";
            this.formsPlot.Size = new System.Drawing.Size(596, 523);
            this.formsPlot.TabIndex = 1;
            // 
            // picRoundIcon
            // 
            this.picRoundIcon.Image = global::FallGuysStats.Properties.Resources.fallguys_icon;
            this.picRoundIcon.Location = new System.Drawing.Point(616, 63);
            this.picRoundIcon.Name = "picRoundIcon";
            this.picRoundIcon.Size = new System.Drawing.Size(60, 60);
            this.picRoundIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRoundIcon.TabIndex = 2;
            this.picRoundIcon.TabStop = false;
            // 
            // cboRoundList
            // 
            this.cboRoundList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboRoundList.IntegralHeight = false;
            this.cboRoundList.ItemHeight = 23;
            this.cboRoundList.Location = new System.Drawing.Point(706, 95);
            this.cboRoundList.MaxDropDownItems = 18;
            this.cboRoundList.Name = "cboRoundList";
            this.cboRoundList.Size = new System.Drawing.Size(220, 29);
            this.cboRoundList.TabIndex = 4;
            this.cboRoundList.UseSelectable = true;
            this.cboRoundList.SelectedIndexChanged += new System.EventHandler(this.cboRoundList_SelectedIndexChanged);
            // 
            // lblRoundTime
            // 
            this.lblRoundTime.AutoSize = true;
            this.lblRoundTime.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblRoundTime.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblRoundTime.ForeColor = System.Drawing.Color.Teal;
            this.lblRoundTime.Location = new System.Drawing.Point(700, 60);
            this.lblRoundTime.Name = "lblRoundTime";
            this.lblRoundTime.Size = new System.Drawing.Size(82, 25);
            this.lblRoundTime.TabIndex = 3;
            this.lblRoundTime.Text = "00:00:00";
            this.lblRoundTime.UseCustomForeColor = true;
            // 
            // picGoldMedalIcon
            // 
            this.picGoldMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_gold_original;
            this.picGoldMedalIcon.Location = new System.Drawing.Point(616, 163);
            this.picGoldMedalIcon.Name = "picGoldMedalIcon";
            this.picGoldMedalIcon.Size = new System.Drawing.Size(60, 60);
            this.picGoldMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGoldMedalIcon.TabIndex = 5;
            this.picGoldMedalIcon.TabStop = false;
            // 
            // lblCountGoldMedal
            // 
            this.lblCountGoldMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountGoldMedal.Location = new System.Drawing.Point(700, 162);
            this.lblCountGoldMedal.Name = "lblCountGoldMedal";
            this.lblCountGoldMedal.Size = new System.Drawing.Size(200, 58);
            this.lblCountGoldMedal.TabIndex = 6;
            this.lblCountGoldMedal.Text = "0";
            // 
            // picSilverMedalIcon
            // 
            this.picSilverMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_silver_original;
            this.picSilverMedalIcon.Location = new System.Drawing.Point(616, 243);
            this.picSilverMedalIcon.Name = "picSilverMedalIcon";
            this.picSilverMedalIcon.Size = new System.Drawing.Size(60, 60);
            this.picSilverMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSilverMedalIcon.TabIndex = 7;
            this.picSilverMedalIcon.TabStop = false;
            // 
            // lblCountSilverMedal
            // 
            this.lblCountSilverMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountSilverMedal.Location = new System.Drawing.Point(700, 242);
            this.lblCountSilverMedal.Name = "lblCountSilverMedal";
            this.lblCountSilverMedal.Size = new System.Drawing.Size(200, 58);
            this.lblCountSilverMedal.TabIndex = 8;
            this.lblCountSilverMedal.Text = "0";
            // 
            // picBronzeMedalIcon
            // 
            this.picBronzeMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_bronze_original;
            this.picBronzeMedalIcon.Location = new System.Drawing.Point(616, 323);
            this.picBronzeMedalIcon.Name = "picBronzeMedalIcon";
            this.picBronzeMedalIcon.Size = new System.Drawing.Size(60, 60);
            this.picBronzeMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBronzeMedalIcon.TabIndex = 9;
            this.picBronzeMedalIcon.TabStop = false;
            // 
            // lblCountBronzeMedal
            // 
            this.lblCountBronzeMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountBronzeMedal.Location = new System.Drawing.Point(700, 322);
            this.lblCountBronzeMedal.Name = "lblCountBronzeMedal";
            this.lblCountBronzeMedal.Size = new System.Drawing.Size(200, 58);
            this.lblCountBronzeMedal.TabIndex = 10;
            this.lblCountBronzeMedal.Text = "0";
            // 
            // picPinkMedalIcon
            // 
            this.picPinkMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_pink_original;
            this.picPinkMedalIcon.Location = new System.Drawing.Point(616, 403);
            this.picPinkMedalIcon.Name = "picPinkMedalIcon";
            this.picPinkMedalIcon.Size = new System.Drawing.Size(60, 60);
            this.picPinkMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPinkMedalIcon.TabIndex = 11;
            this.picPinkMedalIcon.TabStop = false;
            // 
            // lblCountPinkMedal
            // 
            this.lblCountPinkMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountPinkMedal.Location = new System.Drawing.Point(700, 402);
            this.lblCountPinkMedal.Name = "lblCountPinkMedal";
            this.lblCountPinkMedal.Size = new System.Drawing.Size(200, 58);
            this.lblCountPinkMedal.TabIndex = 12;
            this.lblCountPinkMedal.Text = "0";
            // 
            // picEliminatedMedalIcon
            // 
            this.picEliminatedMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_eliminated_original;
            this.picEliminatedMedalIcon.Location = new System.Drawing.Point(616, 483);
            this.picEliminatedMedalIcon.Name = "picEliminatedMedalIcon";
            this.picEliminatedMedalIcon.Size = new System.Drawing.Size(60, 60);
            this.picEliminatedMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEliminatedMedalIcon.TabIndex = 13;
            this.picEliminatedMedalIcon.TabStop = false;
            // 
            // lblCountEliminatedMedal
            // 
            this.lblCountEliminatedMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountEliminatedMedal.Location = new System.Drawing.Point(700, 482);
            this.lblCountEliminatedMedal.Name = "lblCountEliminatedMedal";
            this.lblCountEliminatedMedal.Size = new System.Drawing.Size(200, 58);
            this.lblCountEliminatedMedal.TabIndex = 14;
            this.lblCountEliminatedMedal.Text = "0";
            // 
            // RoundStatsDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(976, 583);
            this.Controls.Add(this.formsPlot);
            this.Controls.Add(this.picRoundIcon);
            this.Controls.Add(this.cboRoundList);
            this.Controls.Add(this.lblRoundTime);
            this.Controls.Add(this.picGoldMedalIcon);
            this.Controls.Add(this.lblCountGoldMedal);
            this.Controls.Add(this.picSilverMedalIcon);
            this.Controls.Add(this.lblCountSilverMedal);
            this.Controls.Add(this.picBronzeMedalIcon);
            this.Controls.Add(this.lblCountBronzeMedal);
            this.Controls.Add(this.picPinkMedalIcon);
            this.Controls.Add(this.lblCountPinkMedal);
            this.Controls.Add(this.picEliminatedMedalIcon);
            this.Controls.Add(this.lblCountEliminatedMedal);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoundStatsDisplay";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 0);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Round Stats Display";
            this.Load += new System.EventHandler(this.RoundStatsDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picRoundIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGoldMedalIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSilverMedalIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBronzeMedalIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPinkMedalIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEliminatedMedalIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        #endregion
        private ScottPlot.FormsPlot formsPlot;
        private System.Windows.Forms.PictureBox picRoundIcon;
        private MetroFramework.Controls.MetroComboBox cboRoundList;
        private MetroFramework.Controls.MetroLabel lblRoundTime;
        private System.Windows.Forms.PictureBox picGoldMedalIcon;
        private System.Windows.Forms.Label lblCountGoldMedal;
        private System.Windows.Forms.PictureBox picSilverMedalIcon;
        private System.Windows.Forms.Label lblCountSilverMedal;
        private System.Windows.Forms.PictureBox picBronzeMedalIcon;
        private System.Windows.Forms.Label lblCountBronzeMedal;
        private System.Windows.Forms.PictureBox picPinkMedalIcon;
        private System.Windows.Forms.Label lblCountPinkMedal;
        private System.Windows.Forms.PictureBox picEliminatedMedalIcon;
        private System.Windows.Forms.Label lblCountEliminatedMedal;
    }
}