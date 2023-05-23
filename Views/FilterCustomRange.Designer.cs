namespace FallGuysStats {
    partial class FilterCustomRange {
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
            this.mdtpStart = new MetroFramework.Controls.MetroDateTime();
            this.lblTilde = new MetroFramework.Controls.MetroLabel();
            this.mdtpEnd = new MetroFramework.Controls.MetroDateTime();
            this.grpTemplates = new System.Windows.Forms.GroupBox();
            this.lbTemplatesList = new System.Windows.Forms.ListBox();
            this.btnFilter = new MetroFramework.Controls.MetroButton();
            this.btnSaveTemplate = new MetroFramework.Controls.MetroButton();
            this.txtTemplateName = new MetroFramework.Controls.MetroTextBox();
            this.lblTemplateName = new MetroFramework.Controls.MetroLabel();
            this.picStartDate = new System.Windows.Forms.PictureBox();
            this.picEndDate = new System.Windows.Forms.PictureBox();
            this.grpTemplates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEndDate)).BeginInit();
            this.SuspendLayout();
            // 
            // mdtpStart
            // 
            this.mdtpStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mdtpStart.Location = new System.Drawing.Point(64, 78);
            this.mdtpStart.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtpStart.Name = "mdtpStart";
            this.mdtpStart.Size = new System.Drawing.Size(200, 29);
            this.mdtpStart.TabIndex = 0;
            this.mdtpStart.CloseUp += new System.EventHandler(this.dtStart_CloseUp);
            // 
            // lblTilde
            // 
            this.lblTilde.AutoSize = true;
            this.lblTilde.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblTilde.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblTilde.Location = new System.Drawing.Point(268, 80);
            this.lblTilde.Name = "lblTilde";
            this.lblTilde.Size = new System.Drawing.Size(25, 25);
            this.lblTilde.Style = MetroFramework.MetroColorStyle.Teal;
            this.lblTilde.TabIndex = 1;
            this.lblTilde.Text = "~";
            // 
            // mdtpEnd
            // 
            this.mdtpEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mdtpEnd.Location = new System.Drawing.Point(339, 78);
            this.mdtpEnd.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtpEnd.Name = "mdtpEnd";
            this.mdtpEnd.Size = new System.Drawing.Size(200, 29);
            this.mdtpEnd.TabIndex = 2;
            this.mdtpEnd.CloseUp += new System.EventHandler(this.dtEnd_CloseUp);
            // 
            // grpTemplates
            // 
            this.grpTemplates.Controls.Add(this.lbTemplatesList);
            this.grpTemplates.Location = new System.Drawing.Point(25, 130);
            this.grpTemplates.Name = "grpTemplates";
            this.grpTemplates.Size = new System.Drawing.Size(514, 227);
            this.grpTemplates.TabIndex = 5;
            this.grpTemplates.TabStop = false;
            this.grpTemplates.Text = "Templates List";
            // 
            // lbTemplatesList
            // 
            this.lbTemplatesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTemplatesList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbTemplatesList.ItemHeight = 12;
            this.lbTemplatesList.Location = new System.Drawing.Point(7, 17);
            this.lbTemplatesList.Name = "lbTemplatesList";
            this.lbTemplatesList.Size = new System.Drawing.Size(501, 218);
            this.lbTemplatesList.TabIndex = 3;
            this.lbTemplatesList.SelectedValueChanged += new System.EventHandler(this.lbTemplatesList_SelectedValueChanged);
            // 
            // btnFilter
            // 
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.Location = new System.Drawing.Point(464, 367);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseSelectable = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.Location = new System.Drawing.Point(321, 61);
            this.btnSaveTemplate.Name = "btnSaveTemplate";
            this.btnSaveTemplate.Size = new System.Drawing.Size(102, 23);
            this.btnSaveTemplate.TabIndex = 7;
            this.btnSaveTemplate.Text = "Save as template";
            this.btnSaveTemplate.UseSelectable = true;
            this.btnSaveTemplate.Visible = false;
            // 
            // txtTemplateName
            // 
            // 
            // 
            // 
            this.txtTemplateName.CustomButton.Image = null;
            this.txtTemplateName.CustomButton.Location = new System.Drawing.Point(138, 1);
            this.txtTemplateName.CustomButton.Name = "";
            this.txtTemplateName.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtTemplateName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtTemplateName.CustomButton.TabIndex = 1;
            this.txtTemplateName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtTemplateName.CustomButton.UseSelectable = true;
            this.txtTemplateName.CustomButton.Visible = false;
            this.txtTemplateName.Lines = new string[0];
            this.txtTemplateName.Location = new System.Drawing.Point(321, 29);
            this.txtTemplateName.MaxLength = 32767;
            this.txtTemplateName.Name = "txtTemplateName";
            this.txtTemplateName.PasswordChar = '\0';
            this.txtTemplateName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtTemplateName.SelectedText = "";
            this.txtTemplateName.SelectionLength = 0;
            this.txtTemplateName.SelectionStart = 0;
            this.txtTemplateName.ShortcutsEnabled = true;
            this.txtTemplateName.Size = new System.Drawing.Size(160, 23);
            this.txtTemplateName.TabIndex = 8;
            this.txtTemplateName.UseSelectable = true;
            this.txtTemplateName.Visible = false;
            this.txtTemplateName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtTemplateName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // lblTemplateName
            // 
            this.lblTemplateName.AutoSize = true;
            this.lblTemplateName.Location = new System.Drawing.Point(210, 31);
            this.lblTemplateName.Name = "lblTemplateName";
            this.lblTemplateName.Size = new System.Drawing.Size(105, 19);
            this.lblTemplateName.TabIndex = 9;
            this.lblTemplateName.Text = "Template Name:";
            this.lblTemplateName.Visible = false;
            // 
            // picStartDate
            // 
            this.picStartDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStartDate.Image = global::FallGuysStats.Properties.Resources.calendar_on_icon;
            this.picStartDate.Location = new System.Drawing.Point(25, 78);
            this.picStartDate.Name = "picStartDate";
            this.picStartDate.Size = new System.Drawing.Size(29, 29);
            this.picStartDate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picStartDate.TabIndex = 13;
            this.picStartDate.TabStop = false;
            this.picStartDate.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picStartDate_MouseClick);
            // 
            // picEndDate
            // 
            this.picEndDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picEndDate.Image = global::FallGuysStats.Properties.Resources.calendar_on_icon;
            this.picEndDate.Location = new System.Drawing.Point(299, 78);
            this.picEndDate.Name = "picEndDate";
            this.picEndDate.Size = new System.Drawing.Size(29, 29);
            this.picEndDate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEndDate.TabIndex = 14;
            this.picEndDate.TabStop = false;
            this.picEndDate.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picEndDate_MouseClick);
            // 
            // FilterCustomRange
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(565, 402);
            this.Controls.Add(this.picStartDate);
            this.Controls.Add(this.picEndDate);
            this.Controls.Add(this.lblTemplateName);
            this.Controls.Add(this.txtTemplateName);
            this.Controls.Add(this.btnSaveTemplate);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.grpTemplates);
            this.Controls.Add(this.mdtpEnd);
            this.Controls.Add(this.lblTilde);
            this.Controls.Add(this.mdtpStart);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterCustomRange";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Custom Range";
            this.Load += new System.EventHandler(this.FilterCustomRange_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterCustomRange_KeyDown);
            this.grpTemplates.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEndDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MetroFramework.Controls.MetroDateTime mdtpStart;
        private MetroFramework.Controls.MetroLabel lblTilde;
        private MetroFramework.Controls.MetroDateTime mdtpEnd;
        private System.Windows.Forms.GroupBox grpTemplates;
        private System.Windows.Forms.ListBox lbTemplatesList;
        private MetroFramework.Controls.MetroButton btnFilter;
        private MetroFramework.Controls.MetroButton btnSaveTemplate;
        private MetroFramework.Controls.MetroTextBox txtTemplateName;
        private MetroFramework.Controls.MetroLabel lblTemplateName;
        private System.Windows.Forms.PictureBox picStartDate;
        private System.Windows.Forms.PictureBox picEndDate;
    }
}