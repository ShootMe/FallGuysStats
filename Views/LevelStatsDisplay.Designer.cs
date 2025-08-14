namespace FallGuysStats {
    partial class LevelStatsDisplay {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelStatsDisplay));
            this.formsPlot = new ScottPlot.FormsPlot();
            this.picRoundIcon = new System.Windows.Forms.PictureBox();
            this.cboRoundList = new MetroFramework.Controls.MetroComboBox();
            this.lblRoundTime = new System.Windows.Forms.Label();
            this.lblRoundType = new FallGuysStats.RoundLabel();
            this.lblBestRecord = new System.Windows.Forms.Label();
            this.lblWorstRecord = new System.Windows.Forms.Label();
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
            this.btnCopyShareCode = new MetroFramework.Controls.MetroButton();
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
            this.picRoundIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picRoundIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picRoundIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // cboRoundList
            // 
            this.cboRoundList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboRoundList.IntegralHeight = false;
            this.cboRoundList.ItemHeight = 23;
            this.cboRoundList.Location = new System.Drawing.Point(706, 95);
            this.cboRoundList.MaxDropDownItems = 18;
            this.cboRoundList.Name = "cboRoundList";
            this.cboRoundList.Size = new System.Drawing.Size(290, 29);
            this.cboRoundList.TabIndex = 4;
            this.cboRoundList.UseSelectable = true;
            this.cboRoundList.SelectedIndexChanged += new System.EventHandler(this.cboRoundList_SelectedIndexChanged);
            // 
            // lblRoundTime
            // 
            this.lblRoundTime.AutoSize = true;
            this.lblRoundTime.ForeColor = System.Drawing.Color.Teal;
            this.lblRoundTime.Location = new System.Drawing.Point(700, 60);
            this.lblRoundTime.Name = "lblRoundTime";
            this.lblRoundTime.Size = new System.Drawing.Size(49, 13);
            this.lblRoundTime.TabIndex = 3;
            this.lblRoundTime.Text = "00:00:00";
            this.lblRoundTime.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblRoundTime.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblRoundTime.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblRoundType
            // 
            this.lblRoundType.ForeColor = System.Drawing.Color.White;
            this.lblRoundType.Location = new System.Drawing.Point(616, 138);
            this.lblRoundType.Name = "lblRoundType";
            this.lblRoundType.Size = new System.Drawing.Size(100, 35);
            this.lblRoundType.TabIndex = 15;
            this.lblRoundType.Text = "RACE";
            this.lblRoundType.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblRoundType.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblRoundType.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblBestRecord
            // 
            this.lblBestRecord.AutoSize = true;
            this.lblBestRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(165)))), ((int)(((byte)(32)))));
            this.lblBestRecord.Location = new System.Drawing.Point(722, 138);
            this.lblBestRecord.Name = "lblBestRecord";
            this.lblBestRecord.Size = new System.Drawing.Size(13, 13);
            this.lblBestRecord.TabIndex = 16;
            this.lblBestRecord.Text = "0";
            this.lblBestRecord.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblBestRecord.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblBestRecord.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblWorstRecord
            // 
            this.lblWorstRecord.AutoSize = true;
            this.lblWorstRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.lblWorstRecord.Location = new System.Drawing.Point(722, 163);
            this.lblWorstRecord.Name = "lblWorstRecord";
            this.lblWorstRecord.Size = new System.Drawing.Size(13, 13);
            this.lblWorstRecord.TabIndex = 17;
            this.lblWorstRecord.Text = "0";
            this.lblWorstRecord.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblWorstRecord.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblWorstRecord.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
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
            this.picGoldMedalIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picGoldMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picGoldMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountGoldMedal
            // 
            this.lblCountGoldMedal.AutoSize = true;
            this.lblCountGoldMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountGoldMedal.Location = new System.Drawing.Point(690, 211);
            this.lblCountGoldMedal.Name = "lblCountGoldMedal";
            this.lblCountGoldMedal.Size = new System.Drawing.Size(13, 13);
            this.lblCountGoldMedal.TabIndex = 6;
            this.lblCountGoldMedal.Text = "0";
            this.lblCountGoldMedal.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblCountGoldMedal.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblCountGoldMedal.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
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
            this.picSilverMedalIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picSilverMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picSilverMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountSilverMedal
            // 
            this.lblCountSilverMedal.AutoSize = true;
            this.lblCountSilverMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountSilverMedal.Location = new System.Drawing.Point(690, 280);
            this.lblCountSilverMedal.Name = "lblCountSilverMedal";
            this.lblCountSilverMedal.Size = new System.Drawing.Size(13, 13);
            this.lblCountSilverMedal.TabIndex = 8;
            this.lblCountSilverMedal.Text = "0";
            this.lblCountSilverMedal.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblCountSilverMedal.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblCountSilverMedal.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
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
            this.picBronzeMedalIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picBronzeMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picBronzeMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountBronzeMedal
            // 
            this.lblCountBronzeMedal.AutoSize = true;
            this.lblCountBronzeMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountBronzeMedal.Location = new System.Drawing.Point(690, 349);
            this.lblCountBronzeMedal.Name = "lblCountBronzeMedal";
            this.lblCountBronzeMedal.Size = new System.Drawing.Size(13, 13);
            this.lblCountBronzeMedal.TabIndex = 10;
            this.lblCountBronzeMedal.Text = "0";
            this.lblCountBronzeMedal.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblCountBronzeMedal.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblCountBronzeMedal.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
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
            this.picPinkMedalIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picPinkMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picPinkMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountPinkMedal
            // 
            this.lblCountPinkMedal.AutoSize = true;
            this.lblCountPinkMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountPinkMedal.Location = new System.Drawing.Point(690, 418);
            this.lblCountPinkMedal.Name = "lblCountPinkMedal";
            this.lblCountPinkMedal.Size = new System.Drawing.Size(13, 13);
            this.lblCountPinkMedal.TabIndex = 12;
            this.lblCountPinkMedal.Text = "0";
            this.lblCountPinkMedal.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblCountPinkMedal.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblCountPinkMedal.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
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
            this.picEliminatedMedalIcon.Click += new System.EventHandler(this.Medal_MouseClick);
            this.picEliminatedMedalIcon.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.picEliminatedMedalIcon.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // lblCountEliminatedMedal
            // 
            this.lblCountEliminatedMedal.AutoSize = true;
            this.lblCountEliminatedMedal.ForeColor = System.Drawing.Color.Teal;
            this.lblCountEliminatedMedal.Location = new System.Drawing.Point(690, 487);
            this.lblCountEliminatedMedal.Name = "lblCountEliminatedMedal";
            this.lblCountEliminatedMedal.Size = new System.Drawing.Size(13, 13);
            this.lblCountEliminatedMedal.TabIndex = 14;
            this.lblCountEliminatedMedal.Text = "0";
            this.lblCountEliminatedMedal.Click += new System.EventHandler(this.Medal_MouseClick);
            this.lblCountEliminatedMedal.MouseEnter += new System.EventHandler(this.Medal_MouseEnter);
            this.lblCountEliminatedMedal.MouseLeave += new System.EventHandler(this.Medal_MouseLeave);
            // 
            // btnCopyShareCode
            // 
            this.btnCopyShareCode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopyShareCode.Location = new System.Drawing.Point(366, 87);
            this.btnCopyShareCode.Name = "btnCopyShareCode";
            this.btnCopyShareCode.Size = new System.Drawing.Size(19, 15);
            this.btnCopyShareCode.TabIndex = 18;
            this.btnCopyShareCode.Text = "⧉";
            this.btnCopyShareCode.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCopyShareCode.UseSelectable = true;
            this.btnCopyShareCode.Visible = false;
            this.btnCopyShareCode.Click += new System.EventHandler(this.btnCopyShareCode_Click);
            // 
            // LevelStatsDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1010, 580);
            this.Controls.Add(this.btnCopyShareCode);
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
            this.Name = "LevelStatsDisplay";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 0);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Round Stats Display";
            this.Load += new System.EventHandler(this.RoundStatsDisplay_Load);
            this.Shown += new System.EventHandler(this.RoundStatsDisplay_Shown);
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
        private System.Windows.Forms.Label lblRoundTime;
        private RoundLabel lblRoundType;
        private System.Windows.Forms.Label lblBestRecord;
        private System.Windows.Forms.Label lblWorstRecord;
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
        private MetroFramework.Controls.MetroButton btnCopyShareCode;
    }
}