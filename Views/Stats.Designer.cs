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
            this.rdAll = new System.Windows.Forms.RadioButton();
            this.rdSeason = new System.Windows.Forms.RadioButton();
            this.rdWeek = new System.Windows.Forms.RadioButton();
            this.rdSession = new System.Windows.Forms.RadioButton();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblKudos = new System.Windows.Forms.Label();
            this.rdDay = new System.Windows.Forms.RadioButton();
            this.gridDetails = new FallGuysStats.Grid();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotalShows
            // 
            this.lblTotalShows.AutoSize = true;
            this.lblTotalShows.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTotalShows.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalShows.Location = new System.Drawing.Point(128, 34);
            this.lblTotalShows.Name = "lblTotalShows";
            this.lblTotalShows.Size = new System.Drawing.Size(51, 13);
            this.lblTotalShows.TabIndex = 7;
            this.lblTotalShows.Text = "Shows: 0";
            this.lblTotalShows.Click += new System.EventHandler(this.lblTotalShows_Click);
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(8, 34);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(107, 13);
            this.lblTotalTime.TabIndex = 6;
            this.lblTotalTime.Text = "Time Played: 0:00:00";
            // 
            // lblTotalRounds
            // 
            this.lblTotalRounds.AutoSize = true;
            this.lblTotalRounds.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTotalRounds.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalRounds.Location = new System.Drawing.Point(206, 34);
            this.lblTotalRounds.Name = "lblTotalRounds";
            this.lblTotalRounds.Size = new System.Drawing.Size(56, 13);
            this.lblTotalRounds.TabIndex = 8;
            this.lblTotalRounds.Text = "Rounds: 0";
            this.lblTotalRounds.Click += new System.EventHandler(this.lblTotalRounds_Click);
            // 
            // lblTotalWins
            // 
            this.lblTotalWins.AutoSize = true;
            this.lblTotalWins.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTotalWins.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalWins.Location = new System.Drawing.Point(296, 34);
            this.lblTotalWins.Name = "lblTotalWins";
            this.lblTotalWins.Size = new System.Drawing.Size(43, 13);
            this.lblTotalWins.TabIndex = 9;
            this.lblTotalWins.Text = "Wins: 0";
            this.lblTotalWins.Click += new System.EventHandler(this.lblTotalWins_Click);
            // 
            // lblFinalChance
            // 
            this.lblFinalChance.AutoSize = true;
            this.lblFinalChance.Location = new System.Drawing.Point(365, 34);
            this.lblFinalChance.Name = "lblFinalChance";
            this.lblFinalChance.Size = new System.Drawing.Size(61, 13);
            this.lblFinalChance.TabIndex = 10;
            this.lblFinalChance.Text = "Final %: 0.0";
            // 
            // lblWinChance
            // 
            this.lblWinChance.AutoSize = true;
            this.lblWinChance.Location = new System.Drawing.Point(443, 34);
            this.lblWinChance.Name = "lblWinChance";
            this.lblWinChance.Size = new System.Drawing.Size(58, 13);
            this.lblWinChance.TabIndex = 11;
            this.lblWinChance.Text = "Win %: 0.0";
            // 
            // rdAll
            // 
            this.rdAll.AutoSize = true;
            this.rdAll.Checked = true;
            this.rdAll.Location = new System.Drawing.Point(12, 9);
            this.rdAll.Name = "rdAll";
            this.rdAll.Size = new System.Drawing.Size(63, 17);
            this.rdAll.TabIndex = 0;
            this.rdAll.TabStop = true;
            this.rdAll.Text = "All Stats";
            this.rdAll.UseVisualStyleBackColor = true;
            this.rdAll.CheckedChanged += new System.EventHandler(this.rdAll_CheckedChanged);
            // 
            // rdSeason
            // 
            this.rdSeason.AutoSize = true;
            this.rdSeason.Location = new System.Drawing.Point(81, 9);
            this.rdSeason.Name = "rdSeason";
            this.rdSeason.Size = new System.Drawing.Size(61, 17);
            this.rdSeason.TabIndex = 1;
            this.rdSeason.Text = "Season";
            this.rdSeason.UseVisualStyleBackColor = true;
            this.rdSeason.CheckedChanged += new System.EventHandler(this.rdAll_CheckedChanged);
            // 
            // rdWeek
            // 
            this.rdWeek.AutoSize = true;
            this.rdWeek.Location = new System.Drawing.Point(147, 9);
            this.rdWeek.Name = "rdWeek";
            this.rdWeek.Size = new System.Drawing.Size(54, 17);
            this.rdWeek.TabIndex = 2;
            this.rdWeek.Text = "Week";
            this.rdWeek.UseVisualStyleBackColor = true;
            this.rdWeek.CheckedChanged += new System.EventHandler(this.rdAll_CheckedChanged);
            // 
            // rdSession
            // 
            this.rdSession.AutoSize = true;
            this.rdSession.Location = new System.Drawing.Point(257, 9);
            this.rdSession.Name = "rdSession";
            this.rdSession.Size = new System.Drawing.Size(62, 17);
            this.rdSession.TabIndex = 4;
            this.rdSession.Text = "Session";
            this.rdSession.UseVisualStyleBackColor = true;
            this.rdSession.CheckedChanged += new System.EventHandler(this.rdAll_CheckedChanged);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(500, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(102, 23);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "Check for Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblKudos
            // 
            this.lblKudos.AutoSize = true;
            this.lblKudos.Location = new System.Drawing.Point(520, 34);
            this.lblKudos.Name = "lblKudos";
            this.lblKudos.Size = new System.Drawing.Size(49, 13);
            this.lblKudos.TabIndex = 12;
            this.lblKudos.Text = "Kudos: 0";
            // 
            // rdDay
            // 
            this.rdDay.AutoSize = true;
            this.rdDay.Location = new System.Drawing.Point(207, 9);
            this.rdDay.Name = "rdDay";
            this.rdDay.Size = new System.Drawing.Size(44, 17);
            this.rdDay.TabIndex = 3;
            this.rdDay.Text = "Day";
            this.rdDay.UseVisualStyleBackColor = true;
            this.rdDay.CheckedChanged += new System.EventHandler(this.rdAll_CheckedChanged);
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
            this.gridDetails.Location = new System.Drawing.Point(0, 53);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.ReadOnly = true;
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.Size = new System.Drawing.Size(614, 570);
            this.gridDetails.TabIndex = 13;
            this.gridDetails.TabStop = false;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            this.gridDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDetails_ColumnHeaderMouseClick);
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(614, 623);
            this.Controls.Add(this.rdDay);
            this.Controls.Add(this.lblKudos);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.rdSession);
            this.Controls.Add(this.rdWeek);
            this.Controls.Add(this.rdSeason);
            this.Controls.Add(this.rdAll);
            this.Controls.Add(this.lblWinChance);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.lblFinalChance);
            this.Controls.Add(this.lblTotalShows);
            this.Controls.Add(this.lblTotalWins);
            this.Controls.Add(this.lblTotalRounds);
            this.Controls.Add(this.gridDetails);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Stats";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fall Guys Stats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
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
        private System.Windows.Forms.RadioButton rdAll;
        private System.Windows.Forms.RadioButton rdSeason;
        private System.Windows.Forms.RadioButton rdWeek;
        private System.Windows.Forms.RadioButton rdSession;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblKudos;
        private System.Windows.Forms.RadioButton rdDay;
    }
}

