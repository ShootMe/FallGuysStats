namespace FallGuysStats {
    partial class LeaderboardDisplay {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeaderboardDisplay));
            this.mtcTabControl = new MetroFramework.Controls.MetroTabControl();
            this.mtpLevelRankPage = new MetroFramework.Controls.MetroTabPage();
            this.mtpOverallRankPage = new MetroFramework.Controls.MetroTabPage();
            this.mpsSpinner01 = new MetroFramework.Controls.MetroProgressSpinner();
            this.mpsSpinner02 = new MetroFramework.Controls.MetroProgressSpinner();
            this.lblSearchDescription = new MetroFramework.Controls.MetroLabel();
            this.gridOverallRank = new FallGuysStats.Grid();
            this.gridLevelRank = new FallGuysStats.Grid();
            this.mlMyRank = new MetroFramework.Controls.MetroLink();
            this.mlVisitFallalytics = new MetroFramework.Controls.MetroLink();
            this.mlRefreshList = new MetroFramework.Controls.MetroLink();
            this.cboRoundList = new FallGuysStats.ImageComboBox();
            this.mtcTabControl.SuspendLayout();
            this.mtpLevelRankPage.SuspendLayout();
            this.mtpOverallRankPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOverallRank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelRank)).BeginInit();
            this.SuspendLayout();
            // 
            // mtcTabControl
            // 
            this.mtcTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.mtcTabControl.Controls.Add(this.mtpOverallRankPage);
            this.mtcTabControl.Controls.Add(this.mtpLevelRankPage);
            this.mtcTabControl.FontSize = MetroFramework.MetroTabControlSize.Tall;
            this.mtcTabControl.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.mtcTabControl.Location = new System.Drawing.Point(6, 96);
            this.mtcTabControl.Name = "mtcTabControl";
            this.mtcTabControl.SelectedIndex = 0;
            this.mtcTabControl.Size = new System.Drawing.Size(1186, 809);
            this.mtcTabControl.TabIndex = 6;
            this.mtcTabControl.UseSelectable = true;
            this.mtcTabControl.SelectedIndexChanged += new System.EventHandler(this.mtcTabControl_SelectedIndexChanged);
            // 
            // mtpOverallRankPage
            // 
            this.mtpOverallRankPage.Controls.Add(this.gridOverallRank);
            this.mtpOverallRankPage.Controls.Add(this.mpsSpinner01);
            this.mtpOverallRankPage.HorizontalScrollbarBarColor = true;
            this.mtpOverallRankPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpOverallRankPage.HorizontalScrollbarSize = 9;
            this.mtpOverallRankPage.Location = new System.Drawing.Point(4, 44);
            this.mtpOverallRankPage.Name = "mtpOverallRankPage";
            this.mtpOverallRankPage.Size = new System.Drawing.Size(1178, 761);
            this.mtpOverallRankPage.TabIndex = 0;
            this.mtpOverallRankPage.Text = "Overall Rank";
            this.mtpOverallRankPage.VerticalScrollbarBarColor = true;
            this.mtpOverallRankPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpOverallRankPage.VerticalScrollbarSize = 12;
            // 
            // gridOverallRank
            // 
            this.gridOverallRank.AllowUserToDeleteRows = false;
            this.gridOverallRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridOverallRank.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridOverallRank.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOverallRank.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridOverallRank.ColumnHeadersHeight = 24;
            this.gridOverallRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridOverallRank.EnableHeadersVisualStyles = false;
            this.gridOverallRank.Location = new System.Drawing.Point(0, 0);
            this.gridOverallRank.MultiSelect = false;
            this.gridOverallRank.Name = "gridOverallRank";
            this.gridOverallRank.ReadOnly = true;
            this.gridOverallRank.RowHeadersVisible = false;
            this.gridOverallRank.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridOverallRank.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridOverallRank.Size = new System.Drawing.Size(1179, 768);
            this.gridOverallRank.TabIndex = 2;
            this.gridOverallRank.DataSourceChanged += new System.EventHandler(this.gridOverallRank_DataSourceChanged);
            this.gridOverallRank.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridOverallRank_CellFormatting);
            this.gridOverallRank.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridOverallRank_ColumnHeaderMouseClick);
            this.gridOverallRank.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // mtpLevelRankPage
            // 
            this.mtpLevelRankPage.Controls.Add(this.mpsSpinner02);
            this.mtpLevelRankPage.Controls.Add(this.lblSearchDescription);
            this.mtpLevelRankPage.Controls.Add(this.gridLevelRank);
            this.mtpLevelRankPage.HorizontalScrollbarBarColor = true;
            this.mtpLevelRankPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpLevelRankPage.HorizontalScrollbarSize = 9;
            this.mtpLevelRankPage.Location = new System.Drawing.Point(4, 44);
            this.mtpLevelRankPage.Name = "mtpLevelRankPage";
            this.mtpLevelRankPage.Size = new System.Drawing.Size(1178, 761);
            this.mtpLevelRankPage.TabIndex = 0;
            this.mtpLevelRankPage.Text = "Level Rank";
            this.mtpLevelRankPage.VerticalScrollbarBarColor = true;
            this.mtpLevelRankPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpLevelRankPage.VerticalScrollbarSize = 12;
            // 
            // mpsSpinner01
            // 
            this.mpsSpinner01.BackColor = System.Drawing.Color.White;
            this.mpsSpinner01.Location = new System.Drawing.Point(509, 344);
            this.mpsSpinner01.Margin = new System.Windows.Forms.Padding(0);
            this.mpsSpinner01.Maximum = 100;
            this.mpsSpinner01.MaximumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner01.MinimumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner01.Name = "mpsSpinner01";
            this.mpsSpinner01.Size = new System.Drawing.Size(28, 28);
            this.mpsSpinner01.Speed = 2F;
            this.mpsSpinner01.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner01.TabIndex = 4;
            this.mpsSpinner01.UseCustomBackColor = true;
            this.mpsSpinner01.UseSelectable = true;
            this.mpsSpinner01.Value = 30;
            this.mpsSpinner01.Visible = false;
            // 
            // mpsSpinner02
            // 
            this.mpsSpinner02.BackColor = System.Drawing.Color.White;
            this.mpsSpinner02.Location = new System.Drawing.Point(509, 344);
            this.mpsSpinner02.Margin = new System.Windows.Forms.Padding(0);
            this.mpsSpinner02.Maximum = 100;
            this.mpsSpinner02.MaximumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner02.MinimumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner02.Name = "mpsSpinner02";
            this.mpsSpinner02.Size = new System.Drawing.Size(28, 28);
            this.mpsSpinner02.Speed = 2F;
            this.mpsSpinner02.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner02.TabIndex = 4;
            this.mpsSpinner02.UseCustomBackColor = true;
            this.mpsSpinner02.UseSelectable = true;
            this.mpsSpinner02.Value = 30;
            this.mpsSpinner02.Visible = false;
            // 
            // lblSearchDescription
            // 
            this.lblSearchDescription.AutoSize = true;
            this.lblSearchDescription.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblSearchDescription.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblSearchDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblSearchDescription.Location = new System.Drawing.Point(540, 344);
            this.lblSearchDescription.Name = "lblSearchDescription";
            this.lblSearchDescription.Size = new System.Drawing.Size(139, 25);
            this.lblSearchDescription.TabIndex = 3;
            this.lblSearchDescription.Text = "Choose a round";
            this.lblSearchDescription.UseCustomForeColor = true;
            this.lblSearchDescription.Visible = false;
            // 
            // gridLevelRank
            // 
            this.gridLevelRank.AllowUserToDeleteRows = false;
            this.gridLevelRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLevelRank.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridLevelRank.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridLevelRank.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridLevelRank.ColumnHeadersHeight = 24;
            this.gridLevelRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridLevelRank.EnableHeadersVisualStyles = false;
            this.gridLevelRank.Location = new System.Drawing.Point(0, 0);
            this.gridLevelRank.MultiSelect = false;
            this.gridLevelRank.Name = "gridLevelRank";
            this.gridLevelRank.ReadOnly = true;
            this.gridLevelRank.RowHeadersVisible = false;
            this.gridLevelRank.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridLevelRank.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLevelRank.Size = new System.Drawing.Size(1179, 768);
            this.gridLevelRank.TabIndex = 2;
            this.gridLevelRank.DataSourceChanged += new System.EventHandler(this.gridLevelRank_DataSourceChanged);
            this.gridLevelRank.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridLevelRank_CellFormatting);
            this.gridLevelRank.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridLevelRank_ColumnHeaderMouseClick);
            // this.gridLevelRank.Scroll += new System.Windows.Forms.ScrollEventHandler(this.gridLevelRank_Scroll);
            this.gridLevelRank.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // mlMyRank
            // 
            this.mlMyRank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlMyRank.AutoSize = true;
            this.mlMyRank.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlMyRank.FontSize = MetroFramework.MetroLinkSize.Tall;
            this.mlMyRank.Image = global::FallGuysStats.Properties.Resources.medal_gold_grid_icon;
            this.mlMyRank.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mlMyRank.ImageSize = 29;
            this.mlMyRank.Location = new System.Drawing.Point(902, 63);
            this.mlMyRank.Name = "mlMyRank";
            this.mlMyRank.Size = new System.Drawing.Size(286, 28);
            this.mlMyRank.TabIndex = 7;
            this.mlMyRank.Text = "My Online Service Nickname";
            this.mlMyRank.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mlMyRank.UseCustomBackColor = true;
            this.mlMyRank.UseCustomForeColor = true;
            this.mlMyRank.UseSelectable = true;
            this.mlMyRank.UseStyleColors = true;
            this.mlMyRank.Visible = false;
            this.mlMyRank.Click += new System.EventHandler(this.link_Click);
            this.mlMyRank.MouseEnter += new System.EventHandler(this.metroLink_MouseEnter);
            this.mlMyRank.MouseLeave += new System.EventHandler(this.metroLink_MouseLeave);
            // 
            // mlVisitFallalytics
            // 
            this.mlVisitFallalytics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlVisitFallalytics.AutoSize = true;
            this.mlVisitFallalytics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlVisitFallalytics.FontSize = MetroFramework.MetroLinkSize.Tall;
            this.mlVisitFallalytics.Image = global::FallGuysStats.Properties.Resources.fallalytics_icon;
            this.mlVisitFallalytics.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mlVisitFallalytics.ImageSize = 20;
            this.mlVisitFallalytics.Location = new System.Drawing.Point(878, 34);
            this.mlVisitFallalytics.Name = "mlVisitFallalytics";
            this.mlVisitFallalytics.Size = new System.Drawing.Size(310, 29);
            this.mlVisitFallalytics.TabIndex = 7;
            this.mlVisitFallalytics.Text = "See full rankings in FALLALYTICS";
            this.mlVisitFallalytics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mlVisitFallalytics.UseCustomBackColor = true;
            this.mlVisitFallalytics.UseCustomForeColor = true;
            this.mlVisitFallalytics.UseSelectable = true;
            this.mlVisitFallalytics.UseStyleColors = true;
            this.mlVisitFallalytics.Visible = false;
            this.mlVisitFallalytics.Click += new System.EventHandler(this.link_Click);
            this.mlVisitFallalytics.MouseEnter += new System.EventHandler(this.metroLink_MouseEnter);
            this.mlVisitFallalytics.MouseLeave += new System.EventHandler(this.metroLink_MouseLeave);
            // 
            // mlRefreshList
            // 
            this.mlRefreshList.AutoSize = true;
            this.mlRefreshList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlRefreshList.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.mlRefreshList.Image = global::FallGuysStats.Properties.Resources.refresh_icon;
            this.mlRefreshList.ImageSize = 27;
            this.mlRefreshList.Location = new System.Drawing.Point(386, 63);
            this.mlRefreshList.Name = "mlRefreshList";
            this.mlRefreshList.Size = new System.Drawing.Size(29, 28);
            this.mlRefreshList.TabIndex = 7;
            this.mlRefreshList.UseSelectable = true;
            this.mlRefreshList.UseStyleColors = true;
            this.mlRefreshList.Visible = false;
            this.mlRefreshList.Click += new System.EventHandler(this.link_Click);
            // 
            // cboRoundList
            // 
            this.cboRoundList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboRoundList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboRoundList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoundList.FormattingEnabled = true;
            this.cboRoundList.ItemHeight = 23;
            this.cboRoundList.Location = new System.Drawing.Point(10, 67);
            this.cboRoundList.MaxDropDownItems = 20;
            this.cboRoundList.Name = "cboRoundList";
            this.cboRoundList.SelectedImage = null;
            this.cboRoundList.SelectedName = "";
            this.cboRoundList.Size = new System.Drawing.Size(370, 29);
            this.cboRoundList.TabIndex = 1;
            this.cboRoundList.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cboRoundList.SelectedIndexChanged += new System.EventHandler(this.cboRoundList_SelectedIndexChanged);
            // 
            // LeaderboardDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 920);
            this.Controls.Add(this.mtcTabControl);
            this.Controls.Add(this.mlMyRank);
            this.Controls.Add(this.mlVisitFallalytics);
            this.Controls.Add(this.mlRefreshList);
            this.Controls.Add(this.cboRoundList);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1200, 1176);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1200, 440);
            this.Name = "LeaderboardDisplay";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 18);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Level Stats";
            this.Load += new System.EventHandler(this.LeaderboardDisplay_Load);
            this.Shown += new System.EventHandler(this.LeaderboardDisplay_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LeaderboardDisplay_KeyDown);
            this.Resize += new System.EventHandler(this.LeaderboardDisplay_Resize);
            this.mtcTabControl.ResumeLayout(false);
            this.mtpLevelRankPage.ResumeLayout(false);
            this.mtpLevelRankPage.PerformLayout();
            this.mtpOverallRankPage.ResumeLayout(false);
            this.mtpOverallRankPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOverallRank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelRank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MetroFramework.Controls.MetroTabControl mtcTabControl;
        private MetroFramework.Controls.MetroTabPage mtpLevelRankPage;
        private MetroFramework.Controls.MetroTabPage mtpOverallRankPage;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner01;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner02;
        private MetroFramework.Controls.MetroLabel lblSearchDescription;
        private MetroFramework.Controls.MetroLink mlMyRank;
        private MetroFramework.Controls.MetroLink mlVisitFallalytics;
        private MetroFramework.Controls.MetroLink mlRefreshList;
        private FallGuysStats.ImageComboBox cboRoundList;
        private FallGuysStats.Grid gridOverallRank;
        private FallGuysStats.Grid gridLevelRank;
    }
}