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
        private void InitializeComponent() {
            this.dtStart = new MetroFramework.Controls.MetroDateTime();
            this.lblTilde = new MetroFramework.Controls.MetroLabel();
            this.dtEnd = new MetroFramework.Controls.MetroDateTime();
            this.templatesListBox = new System.Windows.Forms.ListBox();
            this.lblCustomRange = new MetroFramework.Controls.MetroLabel();
            this.lblTemplates = new MetroFramework.Controls.MetroLabel();
            this.btnFilter = new MetroFramework.Controls.MetroButton();
            this.btnSaveTemplate = new MetroFramework.Controls.MetroButton();
            this.txtTemplateName = new MetroFramework.Controls.MetroTextBox();
            this.lblTemplateName = new MetroFramework.Controls.MetroLabel();
            this.chkEndNotSet = new MetroFramework.Controls.MetroCheckBox();
            this.chkStartNotSet = new MetroFramework.Controls.MetroCheckBox();
            this.SuspendLayout();
            // 
            // dtStart
            // 
            this.dtStart.Location = new System.Drawing.Point(23, 107);
            this.dtStart.MinimumSize = new System.Drawing.Size(0, 29);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(200, 29);
            this.dtStart.TabIndex = 0;
            // 
            // lblTilde
            // 
            this.lblTilde.AutoSize = true;
            this.lblTilde.Location = new System.Drawing.Point(229, 111);
            this.lblTilde.Name = "lblTilde";
            this.lblTilde.Size = new System.Drawing.Size(18, 19);
            this.lblTilde.TabIndex = 1;
            this.lblTilde.Text = "~";
            // 
            // dtEnd
            // 
            this.dtEnd.Location = new System.Drawing.Point(253, 107);
            this.dtEnd.MinimumSize = new System.Drawing.Size(0, 29);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(200, 29);
            this.dtEnd.TabIndex = 2;
            // 
            // templatesListBox
            // 
            this.templatesListBox.FormattingEnabled = true;
            this.templatesListBox.ItemHeight = 12;
            this.templatesListBox.Location = new System.Drawing.Point(459, 85);
            this.templatesListBox.Name = "templatesListBox";
            this.templatesListBox.Size = new System.Drawing.Size(268, 256);
            this.templatesListBox.TabIndex = 3;
            this.templatesListBox.SelectedValueChanged += new System.EventHandler(this.templatesListBox_SelectedValueChanged);
            // 
            // lblCustomRange
            // 
            this.lblCustomRange.AutoSize = true;
            this.lblCustomRange.Location = new System.Drawing.Point(23, 60);
            this.lblCustomRange.Name = "lblCustomRange";
            this.lblCustomRange.Size = new System.Drawing.Size(46, 19);
            this.lblCustomRange.TabIndex = 4;
            this.lblCustomRange.Text = "Range";
            // 
            // lblTemplates
            // 
            this.lblTemplates.AutoSize = true;
            this.lblTemplates.Location = new System.Drawing.Point(459, 60);
            this.lblTemplates.Name = "lblTemplates";
            this.lblTemplates.Size = new System.Drawing.Size(67, 19);
            this.lblTemplates.TabIndex = 5;
            this.lblTemplates.Text = "Templates";
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(378, 318);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseSelectable = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.Location = new System.Drawing.Point(351, 142);
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
            this.txtTemplateName.Location = new System.Drawing.Point(185, 142);
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
            this.lblTemplateName.Location = new System.Drawing.Point(74, 144);
            this.lblTemplateName.Name = "lblTemplateName";
            this.lblTemplateName.Size = new System.Drawing.Size(105, 19);
            this.lblTemplateName.TabIndex = 9;
            this.lblTemplateName.Text = "Template Name:";
            this.lblTemplateName.Visible = false;
            // 
            // chkEndNotSet
            // 
            this.chkEndNotSet.AutoSize = true;
            this.chkEndNotSet.Location = new System.Drawing.Point(253, 86);
            this.chkEndNotSet.Name = "chkEndNotSet";
            this.chkEndNotSet.Size = new System.Drawing.Size(62, 15);
            this.chkEndNotSet.TabIndex = 11;
            this.chkEndNotSet.Text = "Not Set";
            this.chkEndNotSet.UseSelectable = true;
            this.chkEndNotSet.CheckedChanged += new System.EventHandler(this.chkEndNotSet_CheckedChanged);
            // 
            // chkStartNotSet
            // 
            this.chkStartNotSet.AutoSize = true;
            this.chkStartNotSet.Location = new System.Drawing.Point(23, 86);
            this.chkStartNotSet.Name = "chkStartNotSet";
            this.chkStartNotSet.Size = new System.Drawing.Size(62, 15);
            this.chkStartNotSet.TabIndex = 12;
            this.chkStartNotSet.Text = "Not Set";
            this.chkStartNotSet.UseSelectable = true;
            this.chkStartNotSet.CheckedChanged += new System.EventHandler(this.chkStartNotSet_CheckedChanged);
            // 
            // FilterCustomRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 358);
            this.Controls.Add(this.chkStartNotSet);
            this.Controls.Add(this.chkEndNotSet);
            this.Controls.Add(this.lblTemplateName);
            this.Controls.Add(this.txtTemplateName);
            this.Controls.Add(this.btnSaveTemplate);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.lblTemplates);
            this.Controls.Add(this.lblCustomRange);
            this.Controls.Add(this.templatesListBox);
            this.Controls.Add(this.dtEnd);
            this.Controls.Add(this.lblTilde);
            this.Controls.Add(this.dtStart);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterCustomRange";
            this.Text = "Custom Range";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroDateTime dtStart;
        private MetroFramework.Controls.MetroLabel lblTilde;
        private MetroFramework.Controls.MetroDateTime dtEnd;
        private System.Windows.Forms.ListBox templatesListBox;
        private MetroFramework.Controls.MetroLabel lblCustomRange;
        private MetroFramework.Controls.MetroLabel lblTemplates;
        private MetroFramework.Controls.MetroButton btnFilter;
        private MetroFramework.Controls.MetroButton btnSaveTemplate;
        private MetroFramework.Controls.MetroTextBox txtTemplateName;
        private MetroFramework.Controls.MetroLabel lblTemplateName;
        private MetroFramework.Controls.MetroCheckBox chkEndNotSet;
        private MetroFramework.Controls.MetroCheckBox chkStartNotSet;
    }
}