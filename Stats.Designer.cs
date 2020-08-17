namespace FallGuysStats {
    partial class Stats {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stats));
            this.lblTotalShows = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblTotalRounds = new System.Windows.Forms.Label();
            this.lblTotalWins = new System.Windows.Forms.Label();
            this.lblFinalChance = new System.Windows.Forms.Label();
            this.lblWinChance = new System.Windows.Forms.Label();
            this.gridDetails = new FallGuysStats.Grid();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotalShows
            // 
            this.lblTotalShows.AutoSize = true;
            this.lblTotalShows.Location = new System.Drawing.Point(152, 9);
            this.lblTotalShows.Name = "lblTotalShows";
            this.lblTotalShows.Size = new System.Drawing.Size(51, 13);
            this.lblTotalShows.TabIndex = 4;
            this.lblTotalShows.Text = "Shows: 0";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(12, 9);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(134, 13);
            this.lblTotalTime.TabIndex = 3;
            this.lblTotalTime.Text = "Time Played: 00:00:00.000";
            // 
            // lblTotalRounds
            // 
            this.lblTotalRounds.AutoSize = true;
            this.lblTotalRounds.Location = new System.Drawing.Point(226, 9);
            this.lblTotalRounds.Name = "lblTotalRounds";
            this.lblTotalRounds.Size = new System.Drawing.Size(56, 13);
            this.lblTotalRounds.TabIndex = 5;
            this.lblTotalRounds.Text = "Rounds: 0";
            // 
            // lblTotalWins
            // 
            this.lblTotalWins.AutoSize = true;
            this.lblTotalWins.Location = new System.Drawing.Point(306, 9);
            this.lblTotalWins.Name = "lblTotalWins";
            this.lblTotalWins.Size = new System.Drawing.Size(43, 13);
            this.lblTotalWins.TabIndex = 6;
            this.lblTotalWins.Text = "Wins: 0";
            // 
            // lblFinalChance
            // 
            this.lblFinalChance.AutoSize = true;
            this.lblFinalChance.Location = new System.Drawing.Point(370, 9);
            this.lblFinalChance.Name = "lblFinalChance";
            this.lblFinalChance.Size = new System.Drawing.Size(52, 13);
            this.lblFinalChance.TabIndex = 7;
            this.lblFinalChance.Text = "Final %: 0";
            // 
            // lblWinChance
            // 
            this.lblWinChance.AutoSize = true;
            this.lblWinChance.Location = new System.Drawing.Point(447, 9);
            this.lblWinChance.Name = "lblWinChance";
            this.lblWinChance.Size = new System.Drawing.Size(49, 13);
            this.lblWinChance.TabIndex = 8;
            this.lblWinChance.Text = "Win %: 0";
            // 
            // gridDetails
            // 
            this.gridDetails.AllowUserToDeleteRows = false;
            this.gridDetails.AllowUserToOrderColumns = false;
            this.gridDetails.AllowUserToResizeColumns = false;
            this.gridDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.gridDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeight = 20;
            this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridDetails.EnableHeadersVisualStyles = false;
            this.gridDetails.Location = new System.Drawing.Point(11, 33);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.Size = new System.Drawing.Size(611, 571);
            this.gridDetails.TabIndex = 9;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(634, 616);
            this.Controls.Add(this.lblWinChance);
            this.Controls.Add(this.lblFinalChance);
            this.Controls.Add(this.lblTotalWins);
            this.Controls.Add(this.lblTotalRounds);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.lblTotalShows);
            this.Controls.Add(this.gridDetails);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 655);
            this.MinimumSize = new System.Drawing.Size(650, 655);
            this.Name = "Stats";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fall Guys Stats v1.2";
            this.Shown += new System.EventHandler(this.Stats_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Grid gridDetails;
        private System.Windows.Forms.Label lblTotalShows;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblTotalRounds;
        private System.Windows.Forms.Label lblTotalWins;
        private System.Windows.Forms.Label lblFinalChance;
        private System.Windows.Forms.Label lblWinChance;
    }
}

