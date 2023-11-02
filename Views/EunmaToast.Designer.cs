namespace FallGuysStats {
	partial class EunmaToast {
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.textContainer = new System.Windows.Forms.SplitContainer();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.picAppOwnerIcon = new System.Windows.Forms.PictureBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textContainer)).BeginInit();
            this.textContainer.Panel1.SuspendLayout();
            this.textContainer.Panel2.SuspendLayout();
            this.textContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAppOwnerIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.BackColor = System.Drawing.Color.Transparent;
            this.mainContainer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainContainer.ForeColor = System.Drawing.Color.White;
            this.mainContainer.IsSplitterFixed = true;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Panel1MinSize = 110;
            this.mainContainer.Size = new System.Drawing.Size(474, 102);
            this.mainContainer.SplitterDistance = 110;
            this.mainContainer.SplitterWidth = 1;
            this.mainContainer.TabIndex = 0;
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.picImage);
            this.mainContainer.Panel1.Click += new System.EventHandler(this.ToastContentClick);
            this.mainContainer.Panel1.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.mainContainer.Panel1.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.mainContainer.Panel2.Controls.Add(this.textContainer);
            this.mainContainer.Panel2.Click += new System.EventHandler(this.ToastContentClick);
            this.mainContainer.Panel2.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.mainContainer.Panel2.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.Color.DimGray;
            this.picImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picImage.Location = new System.Drawing.Point(0, 0);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(110, 102);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            this.picImage.Click += new System.EventHandler(this.ToastContentClick);
            this.picImage.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.picImage.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // textContainer
            // 
            this.textContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.textContainer.IsSplitterFixed = true;
            this.textContainer.Location = new System.Drawing.Point(0, 0);
            this.textContainer.Name = "textContainer";
            this.textContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.textContainer.Size = new System.Drawing.Size(363, 102);
            this.textContainer.SplitterDistance = 30;
            this.textContainer.SplitterWidth = 1;
            this.textContainer.TabIndex = 1;
            this.textContainer.Click += new System.EventHandler(this.ToastContentClick);
            this.textContainer.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.textContainer.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // textContainer.Panel1
            // 
            this.textContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.textContainer.Panel1.Controls.Add(this.lblProgress);
            this.textContainer.Panel1.Controls.Add(this.btnClose);
            this.textContainer.Panel1.Controls.Add(this.lblCaption);
            this.textContainer.Panel1.Click += new System.EventHandler(this.ToastContentClick);
            this.textContainer.Panel1.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.textContainer.Panel1.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // textContainer.Panel2
            // 
            this.textContainer.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.textContainer.Panel2.Controls.Add(this.picAppOwnerIcon);
            this.textContainer.Panel2.Controls.Add(this.lblDescription);
            this.textContainer.Panel2.Click += new System.EventHandler(this.ToastContentClick);
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.BackColor = System.Drawing.Color.Teal;
            this.lblProgress.Location = new System.Drawing.Point(0, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 2);
            this.lblProgress.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Location = new System.Drawing.Point(329, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(33, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "❌";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // lblCaption
            // 
            this.lblCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaption.AutoEllipsis = true;
            this.lblCaption.BackColor = System.Drawing.Color.Transparent;
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblCaption.Location = new System.Drawing.Point(3, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(323, 25);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCaption.Click += new System.EventHandler(this.ToastContentClick);
            this.lblCaption.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.lblCaption.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // picAppOwnerIcon
            // 
            this.picAppOwnerIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picAppOwnerIcon.BackColor = System.Drawing.Color.Transparent;
            this.picAppOwnerIcon.Image = global::FallGuysStats.Properties.Resources.country_unknown_icon;
            this.picAppOwnerIcon.Location = new System.Drawing.Point(331, 46);
            this.picAppOwnerIcon.Name = "picAppOwnerIcon";
            this.picAppOwnerIcon.Size = new System.Drawing.Size(24, 24);
            this.picAppOwnerIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAppOwnerIcon.TabIndex = 2;
            this.picAppOwnerIcon.TabStop = false;
            this.picAppOwnerIcon.Click += new System.EventHandler(this.ToastContentClick);
            this.picAppOwnerIcon.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.picAppOwnerIcon.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoEllipsis = true;
            this.lblDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDescription.ForeColor = System.Drawing.Color.DarkGray;
            this.lblDescription.Location = new System.Drawing.Point(0, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(2);
            this.lblDescription.Size = new System.Drawing.Size(360, 76);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Click += new System.EventHandler(this.ToastContentClick);
            this.lblDescription.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.lblDescription.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            // 
            // tmrClose
            // 
            this.tmrClose.Interval = 10;
            this.tmrClose.Tick += new System.EventHandler(this.TmrClose_Tick);
            // 
            // EunmaToast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(474, 102);
            this.ControlBox = false;
            this.Controls.Add(this.mainContainer);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "EunmaToast";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmToast_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmToast_FormClosed);
            // this.MouseHover += new System.EventHandler(this.FrmToast_FormHover);
            this.MouseEnter += new System.EventHandler(this.FrmToast_FormEnter);
            this.MouseLeave += new System.EventHandler(this.FrmToast_FormLeave);
            this.Load += new System.EventHandler(this.FrmToast_Load);
            this.Shown += new System.EventHandler(this.FrmToast_Shown);
            this.Click += new System.EventHandler(this.ToastContentClick);
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.textContainer.Panel1.ResumeLayout(false);
            this.textContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textContainer)).EndInit();
            this.textContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAppOwnerIcon)).EndInit();
            this.ResumeLayout(false);
        }

		#endregion

		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.SplitContainer mainContainer;
		private System.Windows.Forms.PictureBox picImage;
		private System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Timer tmrClose;
		private System.Windows.Forms.SplitContainer textContainer;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.PictureBox picAppOwnerIcon;
	}
}