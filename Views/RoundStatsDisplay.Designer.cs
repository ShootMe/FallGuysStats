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
            this.lblRoundType = new FallGuysStats.RoundLabel();
            this.lblBestRecord = new MetroFramework.Controls.MetroLabel();
            this.lblWorstRecord = new MetroFramework.Controls.MetroLabel();
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
            this.picRoundIcon.Location = new System.Drawing.Point(616, 64);
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
            // lblRoundType
            // 
            this.lblRoundType.ForeColor = System.Drawing.Color.White;
            this.lblRoundType.Location = new System.Drawing.Point(616, 138);
            this.lblRoundType.Name = "lblRoundType";
            this.lblRoundType.Size = new System.Drawing.Size(100, 35);
            this.lblRoundType.TabIndex = 15;
            this.lblRoundType.Text = "RACE";
            // 
            // lblBestRecord
            // 
            this.lblBestRecord.AutoSize = true;
            this.lblBestRecord.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblBestRecord.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblBestRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(165)))), ((int)(((byte)(32)))));
            this.lblBestRecord.Location = new System.Drawing.Point(722, 138);
            this.lblBestRecord.Name = "lblBestRecord";
            this.lblBestRecord.Size = new System.Drawing.Size(22, 25);
            this.lblBestRecord.TabIndex = 16;
            this.lblBestRecord.Text = "0";
            this.lblBestRecord.UseCustomForeColor = true;
            // 
            // lblWorstRecord
            // 
            this.lblWorstRecord.AutoSize = true;
            this.lblWorstRecord.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblWorstRecord.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblWorstRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.lblWorstRecord.Location = new System.Drawing.Point(722, 163);
            this.lblWorstRecord.Name = "lblWorstRecord";
            this.lblWorstRecord.Size = new System.Drawing.Size(22, 25);
            this.lblWorstRecord.TabIndex = 17;
            this.lblWorstRecord.Text = "0";
            this.lblWorstRecord.UseCustomForeColor = true;
            // 
            // picGoldMedalIcon
            // 
            this.picGoldMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_gold_original;
            this.picGoldMedalIcon.Location = new System.Drawing.Point(616, 211);
            this.picGoldMedalIcon.Name = "picGoldMedalIcon";
            this.picGoldMedalIcon.Size = new System.Drawing.Size(55, 55);
            this.picGoldMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGoldMedalIcon.TabIndex = 5;
            this.picGoldMedalIcon.TabStop = false;
            this.picGoldMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picGoldMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountGoldMedal
            // 
            this.lblCountGoldMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountGoldMedal.Location = new System.Drawing.Point(690, 211);
            this.lblCountGoldMedal.Name = "lblCountGoldMedal";
            this.lblCountGoldMedal.Size = new System.Drawing.Size(250, 54);
            this.lblCountGoldMedal.TabIndex = 6;
            this.lblCountGoldMedal.Text = "0";
            // 
            // picSilverMedalIcon
            // 
            this.picSilverMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_silver_original;
            this.picSilverMedalIcon.Location = new System.Drawing.Point(616, 280);
            this.picSilverMedalIcon.Name = "picSilverMedalIcon";
            this.picSilverMedalIcon.Size = new System.Drawing.Size(55, 55);
            this.picSilverMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSilverMedalIcon.TabIndex = 7;
            this.picSilverMedalIcon.TabStop = false;
            this.picSilverMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picSilverMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountSilverMedal
            // 
            this.lblCountSilverMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountSilverMedal.Location = new System.Drawing.Point(690, 280);
            this.lblCountSilverMedal.Name = "lblCountSilverMedal";
            this.lblCountSilverMedal.Size = new System.Drawing.Size(250, 54);
            this.lblCountSilverMedal.TabIndex = 8;
            this.lblCountSilverMedal.Text = "0";
            // 
            // picBronzeMedalIcon
            // 
            this.picBronzeMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_bronze_original;
            this.picBronzeMedalIcon.Location = new System.Drawing.Point(616, 349);
            this.picBronzeMedalIcon.Name = "picBronzeMedalIcon";
            this.picBronzeMedalIcon.Size = new System.Drawing.Size(55, 55);
            this.picBronzeMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBronzeMedalIcon.TabIndex = 9;
            this.picBronzeMedalIcon.TabStop = false;
            this.picBronzeMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picBronzeMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountBronzeMedal
            // 
            this.lblCountBronzeMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountBronzeMedal.Location = new System.Drawing.Point(690, 349);
            this.lblCountBronzeMedal.Name = "lblCountBronzeMedal";
            this.lblCountBronzeMedal.Size = new System.Drawing.Size(250, 54);
            this.lblCountBronzeMedal.TabIndex = 10;
            this.lblCountBronzeMedal.Text = "0";
            // 
            // picPinkMedalIcon
            // 
            this.picPinkMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_pink_original;
            this.picPinkMedalIcon.Location = new System.Drawing.Point(616, 418);
            this.picPinkMedalIcon.Name = "picPinkMedalIcon";
            this.picPinkMedalIcon.Size = new System.Drawing.Size(55, 55);
            this.picPinkMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPinkMedalIcon.TabIndex = 11;
            this.picPinkMedalIcon.TabStop = false;
            this.picPinkMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picPinkMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountPinkMedal
            // 
            this.lblCountPinkMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountPinkMedal.Location = new System.Drawing.Point(690, 418);
            this.lblCountPinkMedal.Name = "lblCountPinkMedal";
            this.lblCountPinkMedal.Size = new System.Drawing.Size(250, 54);
            this.lblCountPinkMedal.TabIndex = 12;
            this.lblCountPinkMedal.Text = "0";
            // 
            // picEliminatedMedalIcon
            // 
            this.picEliminatedMedalIcon.Image = global::FallGuysStats.Properties.Resources.medal_eliminated_original;
            this.picEliminatedMedalIcon.Location = new System.Drawing.Point(616, 487);
            this.picEliminatedMedalIcon.Name = "picEliminatedMedalIcon";
            this.picEliminatedMedalIcon.Size = new System.Drawing.Size(55, 55);
            this.picEliminatedMedalIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEliminatedMedalIcon.TabIndex = 13;
            this.picEliminatedMedalIcon.TabStop = false;
            this.picEliminatedMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picEliminatedMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountEliminatedMedal
            // 
            this.lblCountEliminatedMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountEliminatedMedal.Location = new System.Drawing.Point(690, 487);
            this.lblCountEliminatedMedal.Name = "lblCountEliminatedMedal";
            this.lblCountEliminatedMedal.Size = new System.Drawing.Size(250, 54);
            this.lblCountEliminatedMedal.TabIndex = 14;
            this.lblCountEliminatedMedal.Text = "0";
            // 
            // RoundStatsDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(990, 580);
            this.Controls.Add(this.formsPlot);
            this.Controls.Add(this.picRoundIcon);
            this.Controls.Add(this.cboRoundList);
            this.Controls.Add(this.lblRoundTime);
            this.Controls.Add(this.lblRoundType);
            this.Controls.Add(this.lblBestRecord);
            this.Controls.Add(this.lblWorstRecord);
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
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RoundStatsDisplay_KeyDown);
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
        private RoundLabel lblRoundType;
        private MetroFramework.Controls.MetroLabel lblBestRecord;
        private MetroFramework.Controls.MetroLabel lblWorstRecord;
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