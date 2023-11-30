namespace FallGuysStats {
    partial class LevelDetails {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelDetails));
            this.lblPagingInfo = new System.Windows.Forms.Label();
            this.mlFirstPagingButton = new MetroFramework.Controls.MetroLink();
            this.mlLastPagingButton = new MetroFramework.Controls.MetroLink();
            this.mlLeftPagingButton = new MetroFramework.Controls.MetroLink();
            this.mlRightPagingButton = new MetroFramework.Controls.MetroLink();
            this.gridDetails = new FallGuysStats.Grid();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // mlFirstPagingButton
            // 
            this.mlFirstPagingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlFirstPagingButton.AutoSize = true;
            this.mlFirstPagingButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlFirstPagingButton.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.mlFirstPagingButton.Image = global::FallGuysStats.Properties.Resources.first_button_icon;
            this.mlFirstPagingButton.ImageSize = 21;
            this.mlFirstPagingButton.Location = new System.Drawing.Point(0, 34);
            this.mlFirstPagingButton.Name = "mlFirstPagingButton";
            this.mlFirstPagingButton.Size = new System.Drawing.Size(23, 22);
            this.mlFirstPagingButton.TabIndex = 7;
            this.mlFirstPagingButton.UseSelectable = true;
            this.mlFirstPagingButton.UseStyleColors = true;
            this.mlFirstPagingButton.Visible = false;
            this.mlFirstPagingButton.Click += new System.EventHandler(this.pagingButton_Click);
            // 
            // mlLeftPagingButton
            // 
            this.mlLeftPagingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlLeftPagingButton.AutoSize = true;
            this.mlLeftPagingButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlLeftPagingButton.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.mlLeftPagingButton.Image = global::FallGuysStats.Properties.Resources.left_button_icon;
            this.mlLeftPagingButton.ImageSize = 21;
            this.mlLeftPagingButton.Location = new System.Drawing.Point(31, 34);
            this.mlLeftPagingButton.Name = "mlLeftPagingButton";
            this.mlLeftPagingButton.Size = new System.Drawing.Size(23, 22);
            this.mlLeftPagingButton.TabIndex = 7;
            this.mlLeftPagingButton.UseSelectable = true;
            this.mlLeftPagingButton.UseStyleColors = true;
            this.mlLeftPagingButton.Visible = false;
            this.mlLeftPagingButton.Click += new System.EventHandler(this.pagingButton_Click);
            // 
            // lblPagingInfo
            // 
            this.lblPagingInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPagingInfo.AutoSize = true;
            this.lblPagingInfo.Location = new System.Drawing.Point(65, 32);
            this.lblPagingInfo.Name = "lblPagingInfo";
            this.lblPagingInfo.Size = new System.Drawing.Size(31, 22);
            this.lblPagingInfo.TabIndex = 3;
            this.lblPagingInfo.Text = "1 / 2";
            this.lblPagingInfo.Visible = false;
            // 
            // mlRightPagingButton
            // 
            this.mlRightPagingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlRightPagingButton.AutoSize = true;
            this.mlRightPagingButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlRightPagingButton.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.mlRightPagingButton.Image = global::FallGuysStats.Properties.Resources.right_button_icon;
            this.mlRightPagingButton.ImageSize = 21;
            this.mlRightPagingButton.Location = new System.Drawing.Point(127, 34);
            this.mlRightPagingButton.Name = "mlRightPagingButton";
            this.mlRightPagingButton.Size = new System.Drawing.Size(23, 22);
            this.mlRightPagingButton.TabIndex = 7;
            this.mlRightPagingButton.UseSelectable = true;
            this.mlRightPagingButton.UseStyleColors = true;
            this.mlRightPagingButton.Visible = false;
            this.mlRightPagingButton.Click += new System.EventHandler(this.pagingButton_Click);
            // 
            // mlLastPagingButton
            // 
            this.mlLastPagingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlLastPagingButton.AutoSize = true;
            this.mlLastPagingButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mlLastPagingButton.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.mlLastPagingButton.Image = global::FallGuysStats.Properties.Resources.last_button_icon;
            this.mlLastPagingButton.ImageSize = 21;
            this.mlLastPagingButton.Location = new System.Drawing.Point(158, 34);
            this.mlLastPagingButton.Name = "mlLastPagingButton";
            this.mlLastPagingButton.Size = new System.Drawing.Size(23, 22);
            this.mlLastPagingButton.TabIndex = 7;
            this.mlLastPagingButton.UseSelectable = true;
            this.mlLastPagingButton.UseStyleColors = true;
            this.mlLastPagingButton.Visible = false;
            this.mlLastPagingButton.Click += new System.EventHandler(this.pagingButton_Click);
            // 
            // gridDetails
            // 
            this.gridDetails.AllowUserToDeleteRows = false;
            this.gridDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDetails.ColumnHeadersHeight = 20;
            this.gridDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDetails.EnableHeadersVisualStyles = false;
            this.gridDetails.Location = new System.Drawing.Point(23, 60);
            this.gridDetails.Name = "gridDetails";
            this.gridDetails.ReadOnly = true;
            this.gridDetails.RowHeadersVisible = false;
            this.gridDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDetails.Size = new System.Drawing.Size(670, 392);
            this.gridDetails.TabIndex = 10;
            this.gridDetails.DataSourceChanged += new System.EventHandler(this.gridDetails_DataSourceChanged);
            this.gridDetails.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellContentDoubleClick);
            this.gridDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDetails_CellFormatting);
            this.gridDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseEnter);
            this.gridDetails.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDetails_CellMouseLeave);
            this.gridDetails.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDetails_ColumnHeaderMouseClick);
            // this.gridDetails.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridDetails_DataBindingComplete);
            // this.gridDetails.Scroll += new System.Windows.Forms.ScrollEventHandler(this.gridDetails_Scroll);
            this.gridDetails.SelectionChanged += new System.EventHandler(this.gridDetails_SelectionChanged);
            // 
            // LevelDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(716, 470);
            this.Controls.Add(this.mlFirstPagingButton);
            this.Controls.Add(this.mlLeftPagingButton);
            this.Controls.Add(this.lblPagingInfo);
            this.Controls.Add(this.mlRightPagingButton);
            this.Controls.Add(this.mlLastPagingButton);
            this.Controls.Add(this.gridDetails);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "LevelDetails";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 18);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Level Stats";
            this.Load += new System.EventHandler(this.LevelDetails_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LevelDetails_KeyDown);
            this.Shown += new System.EventHandler(this.LevelDetails_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblPagingInfo;
        private MetroFramework.Controls.MetroLink mlFirstPagingButton;
        private MetroFramework.Controls.MetroLink mlLastPagingButton;
        private MetroFramework.Controls.MetroLink mlLeftPagingButton;
        private MetroFramework.Controls.MetroLink mlRightPagingButton;
        private FallGuysStats.Grid gridDetails;
    }
}