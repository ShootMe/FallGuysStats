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
            this.mtpSearchPlayersPage = new MetroFramework.Controls.MetroTabPage();
            this.mtbSearchPlayersText = new MetroFramework.Controls.MetroTextBox();
            this.mpsSpinner01 = new MetroFramework.Controls.MetroProgressSpinner();
            this.mpsSpinner02 = new MetroFramework.Controls.MetroProgressSpinner();
            this.mpsSpinner03 = new MetroFramework.Controls.MetroProgressSpinner();
            this.mpsSpinner04 = new MetroFramework.Controls.MetroProgressSpinner();
            this.lblSearchDescription = new MetroFramework.Controls.MetroLabel();
            this.gridOverallRank = new FallGuysStats.Grid();
            this.gridLevelRank = new FallGuysStats.Grid();
            this.gridPlayerList = new FallGuysStats.Grid();
            this.gridPlayerDetails = new FallGuysStats.Grid();
            this.mlMyRank = new MetroFramework.Controls.MetroLink();
            this.mlVisitFallalytics = new MetroFramework.Controls.MetroLink();
            this.mlRefreshList = new MetroFramework.Controls.MetroLink();
            this.cboRoundList = new FallGuysStats.ImageComboBox();
            this.picPlayerInfo01 = new System.Windows.Forms.PictureBox();
            this.picPlayerInfo02 = new System.Windows.Forms.PictureBox();
            this.picPlayerInfo03 = new System.Windows.Forms.PictureBox();
            this.picPlayerInfo04 = new System.Windows.Forms.PictureBox();
            this.lblPlayerInfo01 = new MetroFramework.Controls.MetroLabel();
            this.lblPlayerInfo02 = new MetroFramework.Controls.MetroLabel();
            this.lblPlayerInfo03 = new MetroFramework.Controls.MetroLabel();
            this.lblPlayerInfo04 = new MetroFramework.Controls.MetroLabel();
            this.lblPlayerInfo05 = new MetroFramework.Controls.MetroLabel();
            this.mtcTabControl.SuspendLayout();
            this.mtpLevelRankPage.SuspendLayout();
            this.mtpOverallRankPage.SuspendLayout();
            this.mtpSearchPlayersPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo01)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo03)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo04)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOverallRank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelRank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlayerList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlayerDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // mtcTabControl
            // 
            this.mtcTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.mtcTabControl.Controls.Add(this.mtpOverallRankPage);
            this.mtcTabControl.Controls.Add(this.mtpLevelRankPage);
            this.mtcTabControl.Controls.Add(this.mtpSearchPlayersPage);
            this.mtcTabControl.FontSize = MetroFramework.MetroTabControlSize.Tall;
            this.mtcTabControl.FontWeight = MetroFramework.MetroTabControlWeight.Regular;
            this.mtcTabControl.Location = new System.Drawing.Point(6, 96);
            this.mtcTabControl.Name = "mtcTabControl";
            this.mtcTabControl.SelectedIndex = 0;
            this.mtcTabControl.Size = new System.Drawing.Size(1339, 809);
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
            this.mtpOverallRankPage.Size = new System.Drawing.Size(1331, 761);
            this.mtpOverallRankPage.TabIndex = 0;
            this.mtpOverallRankPage.Text = "Overall Rank";
            this.mtpOverallRankPage.VerticalScrollbarBarColor = true;
            this.mtpOverallRankPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpOverallRankPage.VerticalScrollbarSize = 12;
            // 
            // mtpSearchPlayers
            // 
            this.mtpSearchPlayersPage.Controls.Add(this.mtbSearchPlayersText);
            this.mtpSearchPlayersPage.Controls.Add(this.picPlayerInfo01);
            this.mtpSearchPlayersPage.Controls.Add(this.picPlayerInfo02);
            this.mtpSearchPlayersPage.Controls.Add(this.picPlayerInfo03);
            this.mtpSearchPlayersPage.Controls.Add(this.lblPlayerInfo01);
            this.mtpSearchPlayersPage.Controls.Add(this.lblPlayerInfo02);
            this.mtpSearchPlayersPage.Controls.Add(this.lblPlayerInfo03);
            this.mtpSearchPlayersPage.Controls.Add(this.lblPlayerInfo04);
            this.mtpSearchPlayersPage.Controls.Add(this.lblPlayerInfo05);
            this.mtpSearchPlayersPage.Controls.Add(this.mpsSpinner03);
            this.mtpSearchPlayersPage.Controls.Add(this.mpsSpinner04);
            this.mtpSearchPlayersPage.Controls.Add(this.gridPlayerList);
            this.mtpSearchPlayersPage.Controls.Add(this.gridPlayerDetails);
            this.mtpSearchPlayersPage.HorizontalScrollbarBarColor = true;
            this.mtpSearchPlayersPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpSearchPlayersPage.HorizontalScrollbarSize = 9;
            this.mtpSearchPlayersPage.Location = new System.Drawing.Point(4, 44);
            this.mtpSearchPlayersPage.Name = "mtpSearchPlayersPage";
            this.mtpSearchPlayersPage.Size = new System.Drawing.Size(1331, 761);
            this.mtpSearchPlayersPage.TabIndex = 0;
            this.mtpSearchPlayersPage.Text = "Search for players";
            this.mtpSearchPlayersPage.VerticalScrollbarBarColor = true;
            this.mtpSearchPlayersPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpSearchPlayersPage.VerticalScrollbarSize = 12;
            //
            // mtbSearchPlayersText
            //
            this.mtbSearchPlayersText.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.mtbSearchPlayersText.Lines = new string[0];
            this.mtbSearchPlayersText.Location = new System.Drawing.Point(0, 0);
            this.mtbSearchPlayersText.MaxLength = 30;
            this.mtbSearchPlayersText.Name = "mtbSearchPlayersText";
            this.mtbSearchPlayersText.PasswordChar = '\0';
            this.mtbSearchPlayersText.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.mtbSearchPlayersText.SelectedText = "";
            this.mtbSearchPlayersText.SelectionLength = 0;
            this.mtbSearchPlayersText.SelectionStart = 0;
            this.mtbSearchPlayersText.ShortcutsEnabled = true;
            this.mtbSearchPlayersText.Size = new System.Drawing.Size(1332, 32);
            this.mtbSearchPlayersText.TabIndex = 1;
            this.mtbSearchPlayersText.UseSelectable = true;
            this.mtbSearchPlayersText.WaterMark = "🔎 My awesome nickname";
            this.mtbSearchPlayersText.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.mtbSearchPlayersText.WaterMarkFont = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.mtbSearchPlayersText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbSearchPlayersText_KeyDown);
            // 
            // picPlayerInfo01
            // 
            this.picPlayerInfo01.BackColor = System.Drawing.Color.Transparent;
            this.picPlayerInfo01.Image = Properties.Resources.steam_main_icon;
            this.picPlayerInfo01.Location = new System.Drawing.Point(361, 5);
            this.picPlayerInfo01.Name = "picPlayerInfo01";
            this.picPlayerInfo01.Size = new System.Drawing.Size(22, 22);
            this.picPlayerInfo01.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPlayerInfo01.TabStop = false;
            // 
            // picPlayerInfo02
            // 
            this.picPlayerInfo02.BackColor = System.Drawing.Color.Transparent;
            this.picPlayerInfo02.Image = Properties.Resources.country_unknown_icon;
            this.picPlayerInfo02.Location = new System.Drawing.Point(392, 2);
            this.picPlayerInfo02.Name = "picPlayerInfo02";
            this.picPlayerInfo02.Size = new System.Drawing.Size(28, 28);
            this.picPlayerInfo02.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPlayerInfo02.TabStop = false;
            // 
            // picPlayerInfo03
            // 
            this.picPlayerInfo03.BackColor = System.Drawing.Color.Transparent;
            this.picPlayerInfo03.Image = Properties.Resources.country_unknown_icon;
            this.picPlayerInfo03.Location = new System.Drawing.Point(392, 5);
            this.picPlayerInfo03.Name = "picPlayerInfo03";
            this.picPlayerInfo03.Size = new System.Drawing.Size(26, 22);
            this.picPlayerInfo03.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPlayerInfo03.TabStop = false;
            // 
            // lblPlayerInfo01
            // 
            this.lblPlayerInfo01.AutoSize = true;
            this.lblPlayerInfo01.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblPlayerInfo01.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayerInfo01.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblPlayerInfo01.Location = new System.Drawing.Point(425, 3);
            this.lblPlayerInfo01.Name = "lblPlayerInfo01";
            this.lblPlayerInfo01.Size = new System.Drawing.Size(10, 32);
            this.lblPlayerInfo01.TabIndex = 3;
            this.lblPlayerInfo01.Text = "Nickname";
            this.lblPlayerInfo01.UseCustomForeColor = true;
            // 
            // lblPlayerInfo02
            // 
            this.lblPlayerInfo02.AutoSize = true;
            this.lblPlayerInfo02.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblPlayerInfo02.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayerInfo02.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblPlayerInfo02.Location = new System.Drawing.Point(425, 3);
            this.lblPlayerInfo02.Name = "lblPlayerInfo02";
            this.lblPlayerInfo02.Size = new System.Drawing.Size(10, 32);
            this.lblPlayerInfo02.TabIndex = 3;
            this.lblPlayerInfo02.Text = "Overall Rank";
            this.lblPlayerInfo02.UseCustomForeColor = true;
            // 
            // lblPlayerInfo03
            // 
            this.lblPlayerInfo03.AutoSize = true;
            this.lblPlayerInfo03.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblPlayerInfo03.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayerInfo03.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblPlayerInfo03.Location = new System.Drawing.Point(425, 3);
            this.lblPlayerInfo03.Name = "lblPlayerInfo03";
            this.lblPlayerInfo03.Size = new System.Drawing.Size(10, 32);
            this.lblPlayerInfo03.TabIndex = 3;
            this.lblPlayerInfo03.Text = "1 (1000)";
            this.lblPlayerInfo03.UseCustomForeColor = true;
            // 
            // lblPlayerInfo04
            // 
            this.lblPlayerInfo04.AutoSize = true;
            this.lblPlayerInfo04.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblPlayerInfo04.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayerInfo04.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblPlayerInfo04.Location = new System.Drawing.Point(425, 3);
            this.lblPlayerInfo04.Name = "lblPlayerInfo04";
            this.lblPlayerInfo04.Size = new System.Drawing.Size(10, 32);
            this.lblPlayerInfo04.TabIndex = 3;
            this.lblPlayerInfo04.Text = "Score : 1000";
            this.lblPlayerInfo04.UseCustomForeColor = true;
            // 
            // lblPlayerInfo05
            // 
            this.lblPlayerInfo05.AutoSize = true;
            this.lblPlayerInfo05.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblPlayerInfo05.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayerInfo05.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.lblPlayerInfo05.Location = new System.Drawing.Point(425, 3);
            this.lblPlayerInfo05.Name = "lblPlayerInfo05";
            this.lblPlayerInfo05.Size = new System.Drawing.Size(10, 32);
            this.lblPlayerInfo05.TabIndex = 3;
            this.lblPlayerInfo05.Text = "WRs(*) : 1";
            this.lblPlayerInfo05.UseCustomForeColor = true;
            // 
            // gridPlayerList
            // 
            this.gridPlayerList.AllowUserToDeleteRows = false;
            this.gridPlayerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPlayerList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridPlayerList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridPlayerList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridPlayerList.ColumnHeadersHeight = 24;
            this.gridPlayerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridPlayerList.EnableHeadersVisualStyles = false;
            this.gridPlayerList.Location = new System.Drawing.Point(0, 32);
            this.gridPlayerList.MultiSelect = false;
            this.gridPlayerList.Name = "gridPlayerList";
            this.gridPlayerList.ReadOnly = true;
            this.gridPlayerList.RowHeadersVisible = false;
            this.gridPlayerList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridPlayerList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPlayerList.Size = new System.Drawing.Size(350, 746);
            this.gridPlayerList.TabIndex = 2;
            this.gridPlayerList.DataSourceChanged += new System.EventHandler(this.gridPlayerList_DataSourceChanged);
            this.gridPlayerList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridPlayerList_CellFormatting);
            this.gridPlayerList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            this.gridPlayerList.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseLeave);
            this.gridPlayerList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridPlayerList_ColumnHeaderMouseClick);
            this.gridPlayerList.SelectionChanged += new System.EventHandler(this.gridPlayerList_SelectionChanged);
            // 
            // gridPlayerDetails
            // 
            this.gridPlayerDetails.AllowUserToDeleteRows = false;
            this.gridPlayerDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPlayerDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridPlayerDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridPlayerDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridPlayerDetails.ColumnHeadersHeight = 24;
            this.gridPlayerDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridPlayerDetails.EnableHeadersVisualStyles = false;
            this.gridPlayerDetails.Location = new System.Drawing.Point(351, 32);
            this.gridPlayerDetails.MultiSelect = false;
            this.gridPlayerDetails.Name = "gridPlayerDetails";
            this.gridPlayerDetails.ReadOnly = true;
            this.gridPlayerDetails.RowHeadersVisible = false;
            this.gridPlayerDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridPlayerDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPlayerDetails.Size = new System.Drawing.Size(980, 746);
            this.gridPlayerDetails.TabIndex = 2;
            this.gridPlayerDetails.DataSourceChanged += new System.EventHandler(this.gridPlayerDetails_DataSourceChanged);
            this.gridPlayerDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridPlayerDetails_CellFormatting);
            this.gridPlayerDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridPlayerDetails_ColumnHeaderMouseClick);
            this.gridPlayerDetails.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
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
            this.gridOverallRank.Size = new System.Drawing.Size(1332, 768);
            this.gridOverallRank.TabIndex = 2;
            this.gridOverallRank.DataSourceChanged += new System.EventHandler(this.gridOverallRank_DataSourceChanged);
            this.gridOverallRank.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridOverallRank_CellDoubleClick);
            this.gridOverallRank.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridOverallRank_CellFormatting);
            this.gridOverallRank.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            this.gridOverallRank.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseLeave);
            this.gridOverallRank.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridOverallRank_ColumnHeaderMouseClick);
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
            this.mtpLevelRankPage.Size = new System.Drawing.Size(1331, 761);
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
            this.mpsSpinner01.Speed = 3F;
            this.mpsSpinner01.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner01.TabIndex = 4;
            this.mpsSpinner01.UseCustomBackColor = true;
            this.mpsSpinner01.UseSelectable = true;
            this.mpsSpinner01.Value = 10;
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
            this.mpsSpinner02.Speed = 3F;
            this.mpsSpinner02.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner02.TabIndex = 4;
            this.mpsSpinner02.UseCustomBackColor = true;
            this.mpsSpinner02.UseSelectable = true;
            this.mpsSpinner02.Value = 10;
            this.mpsSpinner02.Visible = false;
            // 
            // mpsSpinner03
            // 
            this.mpsSpinner03.BackColor = System.Drawing.Color.White;
            this.mpsSpinner03.Location = new System.Drawing.Point(509, 344);
            this.mpsSpinner03.Margin = new System.Windows.Forms.Padding(0);
            this.mpsSpinner03.Maximum = 100;
            this.mpsSpinner03.MaximumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner03.MinimumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner03.Name = "mpsSpinner03";
            this.mpsSpinner03.Size = new System.Drawing.Size(28, 28);
            this.mpsSpinner03.Speed = 3F;
            this.mpsSpinner03.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner03.TabIndex = 4;
            this.mpsSpinner03.UseCustomBackColor = true;
            this.mpsSpinner03.UseSelectable = true;
            this.mpsSpinner03.Value = 10;
            this.mpsSpinner03.Visible = false;
            // 
            // mpsSpinner04
            // 
            this.mpsSpinner04.BackColor = System.Drawing.Color.White;
            this.mpsSpinner04.Location = new System.Drawing.Point(509, 344);
            this.mpsSpinner04.Margin = new System.Windows.Forms.Padding(0);
            this.mpsSpinner04.Maximum = 100;
            this.mpsSpinner04.MaximumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner04.MinimumSize = new System.Drawing.Size(28, 28);
            this.mpsSpinner04.Name = "mpsSpinner04";
            this.mpsSpinner04.Size = new System.Drawing.Size(28, 28);
            this.mpsSpinner04.Speed = 3F;
            this.mpsSpinner04.Style = MetroFramework.MetroColorStyle.Teal;
            this.mpsSpinner04.TabIndex = 4;
            this.mpsSpinner04.UseCustomBackColor = true;
            this.mpsSpinner04.UseSelectable = true;
            this.mpsSpinner04.Value = 10;
            this.mpsSpinner04.Visible = false;
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
            this.gridLevelRank.Size = new System.Drawing.Size(1332, 768);
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
            this.ClientSize = new System.Drawing.Size(1350, 920);
            this.Controls.Add(this.mtcTabControl);
            this.Controls.Add(this.mlMyRank);
            this.Controls.Add(this.mlVisitFallalytics);
            this.Controls.Add(this.mlRefreshList);
            this.Controls.Add(this.cboRoundList);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1350, 1176);
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(1350, 440);
            this.Name = "LeaderboardDisplay";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 18);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
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
            this.mtpSearchPlayersPage.ResumeLayout(false);
            this.mtpSearchPlayersPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo01)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo03)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerInfo04)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOverallRank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevelRank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlayerList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlayerDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MetroFramework.Controls.MetroTabControl mtcTabControl;
        private MetroFramework.Controls.MetroTabPage mtpLevelRankPage;
        private MetroFramework.Controls.MetroTabPage mtpOverallRankPage;
        private MetroFramework.Controls.MetroTabPage mtpSearchPlayersPage;
        private MetroFramework.Controls.MetroTextBox mtbSearchPlayersText;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner01;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner02;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner03;
        private MetroFramework.Controls.MetroProgressSpinner mpsSpinner04;
        private MetroFramework.Controls.MetroLabel lblSearchDescription;
        private MetroFramework.Controls.MetroLink mlMyRank;
        private MetroFramework.Controls.MetroLink mlVisitFallalytics;
        private MetroFramework.Controls.MetroLink mlRefreshList;
        private FallGuysStats.ImageComboBox cboRoundList;
        private System.Windows.Forms.PictureBox picPlayerInfo01;
        private System.Windows.Forms.PictureBox picPlayerInfo02;
        private System.Windows.Forms.PictureBox picPlayerInfo03;
        private System.Windows.Forms.PictureBox picPlayerInfo04;
        private MetroFramework.Controls.MetroLabel lblPlayerInfo01;
        private MetroFramework.Controls.MetroLabel lblPlayerInfo02;
        private MetroFramework.Controls.MetroLabel lblPlayerInfo03;
        private MetroFramework.Controls.MetroLabel lblPlayerInfo04;
        private MetroFramework.Controls.MetroLabel lblPlayerInfo05;
        private FallGuysStats.Grid gridOverallRank;
        private FallGuysStats.Grid gridLevelRank;
        private FallGuysStats.Grid gridPlayerList;
        private FallGuysStats.Grid gridPlayerDetails;
    }
}