namespace FallGuysStats {
    partial class EditProfiles {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditProfiles));
            this.dgvProfiles = new System.Windows.Forms.DataGridView();
            this.btnProfileUp = new MetroFramework.Controls.MetroButton();
            this.btnProfileDown = new MetroFramework.Controls.MetroButton();
            this.grpProfiles = new System.Windows.Forms.GroupBox();
            this.mtcTabControl = new MetroFramework.Controls.MetroTabControl();
            this.mtpAddTabPage = new MetroFramework.Controls.MetroTabPage();
            this.btnAddProfile = new MetroFramework.Controls.MetroButton();
            this.txtAddProfile = new MetroFramework.Controls.MetroTextBox();
            this.lblAddProfile1 = new MetroFramework.Controls.MetroLabel();
            this.mtpRenameTabPage = new MetroFramework.Controls.MetroTabPage();
            this.btnRenameProfile = new MetroFramework.Controls.MetroButton();
            this.txtRenameProfile = new MetroFramework.Controls.MetroTextBox();
            this.lblRenameProfile2 = new MetroFramework.Controls.MetroLabel();
            this.cboProfileRename = new MetroFramework.Controls.MetroComboBox();
            this.lblRenameProfile1 = new MetroFramework.Controls.MetroLabel();
            this.mtpMoveTabPage = new MetroFramework.Controls.MetroTabPage();
            this.btnMoveProfile = new MetroFramework.Controls.MetroButton();
            this.lblMoveProfile2 = new MetroFramework.Controls.MetroLabel();
            this.cboProfileMoveTo = new MetroFramework.Controls.MetroComboBox();
            this.lblMoveProfile1 = new MetroFramework.Controls.MetroLabel();
            this.cboProfileMoveFrom = new MetroFramework.Controls.MetroComboBox();
            this.mtpRemoveTabPage = new MetroFramework.Controls.MetroTabPage();
            this.btnRemoveProfile = new MetroFramework.Controls.MetroButton();
            this.cboProfileRemove = new MetroFramework.Controls.MetroComboBox();
            this.lblRemoveProfile1 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
            this.grpProfiles.SuspendLayout();
            this.mtcTabControl.SuspendLayout();
            this.mtpAddTabPage.SuspendLayout();
            this.mtpRenameTabPage.SuspendLayout();
            this.mtpMoveTabPage.SuspendLayout();
            this.mtpRemoveTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvProfiles
            // 
            this.dgvProfiles.AllowUserToAddRows = false;
            this.dgvProfiles.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.dgvProfiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProfiles.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvProfiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProfiles.ColumnHeadersVisible = false;
            this.dgvProfiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvProfiles.Location = new System.Drawing.Point(4, 19);
            this.dgvProfiles.MultiSelect = false;
            this.dgvProfiles.Name = "dgvProfiles";
            this.dgvProfiles.RowHeadersVisible = false;
            this.dgvProfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProfiles.Size = new System.Drawing.Size(498, 352);
            this.dgvProfiles.TabIndex = 0;
            this.dgvProfiles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProfileList_CellClick);
            this.dgvProfiles.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.ProfileList_CellFormatting);
            this.dgvProfiles.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ProfileList_EditingControlShowing);
            this.dgvProfiles.SelectionChanged += new System.EventHandler(this.ProfileList_SelectionChanged);
            // 
            // btnProfileUp
            // 
            this.btnProfileUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfileUp.Location = new System.Drawing.Point(506, 19);
            this.btnProfileUp.Name = "btnProfileUp";
            this.btnProfileUp.Size = new System.Drawing.Size(20, 174);
            this.btnProfileUp.TabIndex = 3;
            this.btnProfileUp.Text = "∧";
            this.btnProfileUp.UseSelectable = true;
            this.btnProfileUp.Click += new System.EventHandler(this.ProfileListUp_Click);
            // 
            // btnProfileDown
            // 
            this.btnProfileDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfileDown.Location = new System.Drawing.Point(506, 197);
            this.btnProfileDown.Name = "btnProfileDown";
            this.btnProfileDown.Size = new System.Drawing.Size(20, 174);
            this.btnProfileDown.TabIndex = 4;
            this.btnProfileDown.Text = "∨";
            this.btnProfileDown.UseSelectable = true;
            this.btnProfileDown.Click += new System.EventHandler(this.ProfileListDown_Click);
            // 
            // grpProfiles
            // 
            this.grpProfiles.Controls.Add(this.dgvProfiles);
            this.grpProfiles.Controls.Add(this.btnProfileUp);
            this.grpProfiles.Controls.Add(this.btnProfileDown);
            this.grpProfiles.Location = new System.Drawing.Point(9, 238);
            this.grpProfiles.Name = "grpProfiles";
            this.grpProfiles.Size = new System.Drawing.Size(531, 376);
            this.grpProfiles.TabIndex = 5;
            this.grpProfiles.TabStop = false;
            this.grpProfiles.Text = "Profile List";
            // 
            // mtcTabControl
            // 
            this.mtcTabControl.Controls.Add(this.mtpAddTabPage);
            this.mtcTabControl.Controls.Add(this.mtpRenameTabPage);
            this.mtcTabControl.Controls.Add(this.mtpMoveTabPage);
            this.mtcTabControl.Controls.Add(this.mtpRemoveTabPage);
            this.mtcTabControl.FontWeight = MetroFramework.MetroTabControlWeight.Regular;
            this.mtcTabControl.Location = new System.Drawing.Point(9, 65);
            this.mtcTabControl.Name = "mtcTabControl";
            this.mtcTabControl.SelectedIndex = 0;
            this.mtcTabControl.Size = new System.Drawing.Size(531, 165);
            this.mtcTabControl.TabIndex = 6;
            this.mtcTabControl.UseSelectable = true;
            // 
            // mtpAddTabPage
            // 
            this.mtpAddTabPage.Controls.Add(this.btnAddProfile);
            this.mtpAddTabPage.Controls.Add(this.txtAddProfile);
            this.mtpAddTabPage.Controls.Add(this.lblAddProfile1);
            this.mtpAddTabPage.HorizontalScrollbarBarColor = true;
            this.mtpAddTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpAddTabPage.HorizontalScrollbarSize = 10;
            this.mtpAddTabPage.Location = new System.Drawing.Point(4, 38);
            this.mtpAddTabPage.Name = "mtpAddTabPage";
            this.mtpAddTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mtpAddTabPage.Size = new System.Drawing.Size(523, 123);
            this.mtpAddTabPage.TabIndex = 0;
            this.mtpAddTabPage.Text = "Add";
            this.mtpAddTabPage.VerticalScrollbarBarColor = true;
            this.mtpAddTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpAddTabPage.VerticalScrollbarSize = 10;
            // 
            // btnAddProfile
            // 
            this.btnAddProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddProfile.Location = new System.Drawing.Point(8, 88);
            this.btnAddProfile.Name = "btnAddProfile";
            this.btnAddProfile.Size = new System.Drawing.Size(76, 26);
            this.btnAddProfile.TabIndex = 2;
            this.btnAddProfile.Text = "Add";
            this.btnAddProfile.UseSelectable = true;
            this.btnAddProfile.Click += new System.EventHandler(this.AddPageButton_Click);
            // 
            // txtAddProfile
            // 
            this.txtAddProfile.CustomButton.Image = null;
            this.txtAddProfile.CustomButton.Location = new System.Drawing.Point(150, 1);
            this.txtAddProfile.CustomButton.Name = "";
            this.txtAddProfile.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.txtAddProfile.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtAddProfile.CustomButton.TabIndex = 1;
            this.txtAddProfile.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtAddProfile.CustomButton.UseSelectable = true;
            this.txtAddProfile.CustomButton.Visible = false;
            this.txtAddProfile.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtAddProfile.Lines = new string[0];
            this.txtAddProfile.Location = new System.Drawing.Point(96, 10);
            this.txtAddProfile.MaxLength = 20;
            this.txtAddProfile.Name = "txtAddProfile";
            this.txtAddProfile.PasswordChar = '\0';
            this.txtAddProfile.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtAddProfile.SelectedText = "";
            this.txtAddProfile.SelectionLength = 0;
            this.txtAddProfile.SelectionStart = 0;
            this.txtAddProfile.ShortcutsEnabled = true;
            this.txtAddProfile.Size = new System.Drawing.Size(178, 29);
            this.txtAddProfile.TabIndex = 1;
            this.txtAddProfile.UseSelectable = true;
            this.txtAddProfile.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtAddProfile.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtAddProfile.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DeleteAmpersand_KeyPress);
            // 
            // lblAddProfile1
            // 
            this.lblAddProfile1.AutoSize = true;
            this.lblAddProfile1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblAddProfile1.Location = new System.Drawing.Point(7, 9);
            this.lblAddProfile1.Name = "lblAddProfile1";
            this.lblAddProfile1.Size = new System.Drawing.Size(87, 19);
            this.lblAddProfile1.TabIndex = 0;
            this.lblAddProfile1.Text = "Profile Name";
            // 
            // mtpRenameTabPage
            // 
            this.mtpRenameTabPage.Controls.Add(this.btnRenameProfile);
            this.mtpRenameTabPage.Controls.Add(this.txtRenameProfile);
            this.mtpRenameTabPage.Controls.Add(this.lblRenameProfile2);
            this.mtpRenameTabPage.Controls.Add(this.cboProfileRename);
            this.mtpRenameTabPage.Controls.Add(this.lblRenameProfile1);
            this.mtpRenameTabPage.HorizontalScrollbarBarColor = true;
            this.mtpRenameTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpRenameTabPage.HorizontalScrollbarSize = 10;
            this.mtpRenameTabPage.Location = new System.Drawing.Point(4, 38);
            this.mtpRenameTabPage.Name = "mtpRenameTabPage";
            this.mtpRenameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mtpRenameTabPage.Size = new System.Drawing.Size(523, 123);
            this.mtpRenameTabPage.TabIndex = 3;
            this.mtpRenameTabPage.Text = "Rename";
            this.mtpRenameTabPage.VerticalScrollbarBarColor = true;
            this.mtpRenameTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpRenameTabPage.VerticalScrollbarSize = 10;
            // 
            // btnRenameProfile
            // 
            this.btnRenameProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRenameProfile.Location = new System.Drawing.Point(8, 88);
            this.btnRenameProfile.Name = "btnRenameProfile";
            this.btnRenameProfile.Size = new System.Drawing.Size(76, 26);
            this.btnRenameProfile.TabIndex = 4;
            this.btnRenameProfile.Text = "Rename";
            this.btnRenameProfile.UseSelectable = true;
            this.btnRenameProfile.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // txtRenameProfile
            // 
            this.txtRenameProfile.CustomButton.Image = null;
            this.txtRenameProfile.CustomButton.Location = new System.Drawing.Point(150, 1);
            this.txtRenameProfile.CustomButton.Name = "";
            this.txtRenameProfile.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.txtRenameProfile.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtRenameProfile.CustomButton.TabIndex = 1;
            this.txtRenameProfile.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtRenameProfile.CustomButton.UseSelectable = true;
            this.txtRenameProfile.CustomButton.Visible = false;
            this.txtRenameProfile.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtRenameProfile.Lines = new string[0];
            this.txtRenameProfile.Location = new System.Drawing.Point(96, 45);
            this.txtRenameProfile.MaxLength = 20;
            this.txtRenameProfile.Name = "txtRenameProfile";
            this.txtRenameProfile.PasswordChar = '\0';
            this.txtRenameProfile.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtRenameProfile.SelectedText = "";
            this.txtRenameProfile.SelectionLength = 0;
            this.txtRenameProfile.SelectionStart = 0;
            this.txtRenameProfile.ShortcutsEnabled = true;
            this.txtRenameProfile.Size = new System.Drawing.Size(178, 29);
            this.txtRenameProfile.TabIndex = 3;
            this.txtRenameProfile.UseSelectable = true;
            this.txtRenameProfile.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtRenameProfile.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtRenameProfile.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DeleteAmpersand_KeyPress);
            // 
            // lblRenameProfile2
            // 
            this.lblRenameProfile2.AutoSize = true;
            this.lblRenameProfile2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblRenameProfile2.Location = new System.Drawing.Point(7, 47);
            this.lblRenameProfile2.Name = "lblRenameProfile2";
            this.lblRenameProfile2.Size = new System.Drawing.Size(87, 19);
            this.lblRenameProfile2.TabIndex = 2;
            this.lblRenameProfile2.Text = "Profile Name";
            // 
            // cboProfileRename
            // 
            this.cboProfileRename.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboProfileRename.FormattingEnabled = true;
            this.cboProfileRename.ItemHeight = 23;
            this.cboProfileRename.Location = new System.Drawing.Point(96, 7);
            this.cboProfileRename.Name = "cboProfileRename";
            this.cboProfileRename.Size = new System.Drawing.Size(192, 29);
            this.cboProfileRename.TabIndex = 1;
            this.cboProfileRename.UseSelectable = true;
            this.cboProfileRename.SelectedIndexChanged += new System.EventHandler(this.RenameComboboxChanged);
            // 
            // lblRenameProfile1
            // 
            this.lblRenameProfile1.AutoSize = true;
            this.lblRenameProfile1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblRenameProfile1.Location = new System.Drawing.Point(7, 9);
            this.lblRenameProfile1.Name = "lblRenameProfile1";
            this.lblRenameProfile1.Size = new System.Drawing.Size(47, 19);
            this.lblRenameProfile1.TabIndex = 0;
            this.lblRenameProfile1.Text = "Profile";
            // 
            // mtpMoveTabPage
            // 
            this.mtpMoveTabPage.Controls.Add(this.btnMoveProfile);
            this.mtpMoveTabPage.Controls.Add(this.lblMoveProfile2);
            this.mtpMoveTabPage.Controls.Add(this.cboProfileMoveTo);
            this.mtpMoveTabPage.Controls.Add(this.lblMoveProfile1);
            this.mtpMoveTabPage.Controls.Add(this.cboProfileMoveFrom);
            this.mtpMoveTabPage.HorizontalScrollbarBarColor = true;
            this.mtpMoveTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpMoveTabPage.HorizontalScrollbarSize = 10;
            this.mtpMoveTabPage.Location = new System.Drawing.Point(4, 38);
            this.mtpMoveTabPage.Name = "mtpMoveTabPage";
            this.mtpMoveTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mtpMoveTabPage.Size = new System.Drawing.Size(523, 123);
            this.mtpMoveTabPage.TabIndex = 1;
            this.mtpMoveTabPage.Text = "Move";
            this.mtpMoveTabPage.VerticalScrollbarBarColor = true;
            this.mtpMoveTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpMoveTabPage.VerticalScrollbarSize = 10;
            // 
            // btnMoveProfile
            // 
            this.btnMoveProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMoveProfile.Location = new System.Drawing.Point(8, 88);
            this.btnMoveProfile.Name = "btnMoveProfile";
            this.btnMoveProfile.Size = new System.Drawing.Size(76, 26);
            this.btnMoveProfile.TabIndex = 4;
            this.btnMoveProfile.Text = "Move";
            this.btnMoveProfile.UseSelectable = true;
            this.btnMoveProfile.Click += new System.EventHandler(this.MovePageButton_Click);
            // 
            // lblMoveProfile2
            // 
            this.lblMoveProfile2.AutoSize = true;
            this.lblMoveProfile2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblMoveProfile2.Location = new System.Drawing.Point(7, 47);
            this.lblMoveProfile2.Name = "lblMoveProfile2";
            this.lblMoveProfile2.Size = new System.Drawing.Size(23, 19);
            this.lblMoveProfile2.TabIndex = 3;
            this.lblMoveProfile2.Text = "To";
            // 
            // cboProfileMoveTo
            // 
            this.cboProfileMoveTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboProfileMoveTo.FormattingEnabled = true;
            this.cboProfileMoveTo.ItemHeight = 23;
            this.cboProfileMoveTo.Location = new System.Drawing.Point(96, 45);
            this.cboProfileMoveTo.Name = "cboProfileMoveTo";
            this.cboProfileMoveTo.Size = new System.Drawing.Size(192, 29);
            this.cboProfileMoveTo.TabIndex = 2;
            this.cboProfileMoveTo.UseSelectable = true;
            // 
            // lblMoveProfile1
            // 
            this.lblMoveProfile1.AutoSize = true;
            this.lblMoveProfile1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblMoveProfile1.Location = new System.Drawing.Point(7, 9);
            this.lblMoveProfile1.Name = "lblMoveProfile1";
            this.lblMoveProfile1.Size = new System.Drawing.Size(41, 19);
            this.lblMoveProfile1.TabIndex = 1;
            this.lblMoveProfile1.Text = "From";
            // 
            // cboProfileMoveFrom
            // 
            this.cboProfileMoveFrom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboProfileMoveFrom.FormattingEnabled = true;
            this.cboProfileMoveFrom.ItemHeight = 23;
            this.cboProfileMoveFrom.Location = new System.Drawing.Point(96, 7);
            this.cboProfileMoveFrom.Name = "cboProfileMoveFrom";
            this.cboProfileMoveFrom.Size = new System.Drawing.Size(192, 29);
            this.cboProfileMoveFrom.TabIndex = 0;
            this.cboProfileMoveFrom.UseSelectable = true;
            // 
            // mtpRemoveTabPage
            // 
            this.mtpRemoveTabPage.Controls.Add(this.btnRemoveProfile);
            this.mtpRemoveTabPage.Controls.Add(this.cboProfileRemove);
            this.mtpRemoveTabPage.Controls.Add(this.lblRemoveProfile1);
            this.mtpRemoveTabPage.HorizontalScrollbarBarColor = true;
            this.mtpRemoveTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mtpRemoveTabPage.HorizontalScrollbarSize = 10;
            this.mtpRemoveTabPage.Location = new System.Drawing.Point(4, 38);
            this.mtpRemoveTabPage.Name = "mtpRemoveTabPage";
            this.mtpRemoveTabPage.Size = new System.Drawing.Size(523, 123);
            this.mtpRemoveTabPage.TabIndex = 2;
            this.mtpRemoveTabPage.Text = "Remove";
            this.mtpRemoveTabPage.VerticalScrollbarBarColor = true;
            this.mtpRemoveTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mtpRemoveTabPage.VerticalScrollbarSize = 10;
            // 
            // btnRemoveProfile
            // 
            this.btnRemoveProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveProfile.Location = new System.Drawing.Point(8, 88);
            this.btnRemoveProfile.Name = "btnRemoveProfile";
            this.btnRemoveProfile.Size = new System.Drawing.Size(76, 26);
            this.btnRemoveProfile.TabIndex = 2;
            this.btnRemoveProfile.Text = "Remove";
            this.btnRemoveProfile.UseSelectable = true;
            this.btnRemoveProfile.Click += new System.EventHandler(this.RemovePageButton_Click);
            // 
            // cboProfileRemove
            // 
            this.cboProfileRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboProfileRemove.FormattingEnabled = true;
            this.cboProfileRemove.ItemHeight = 23;
            this.cboProfileRemove.Location = new System.Drawing.Point(96, 7);
            this.cboProfileRemove.Name = "cboProfileRemove";
            this.cboProfileRemove.Size = new System.Drawing.Size(192, 29);
            this.cboProfileRemove.TabIndex = 1;
            this.cboProfileRemove.UseSelectable = true;
            // 
            // lblRemoveProfile1
            // 
            this.lblRemoveProfile1.AutoSize = true;
            this.lblRemoveProfile1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblRemoveProfile1.Location = new System.Drawing.Point(7, 9);
            this.lblRemoveProfile1.Name = "lblRemoveProfile1";
            this.lblRemoveProfile1.Size = new System.Drawing.Size(47, 19);
            this.lblRemoveProfile1.TabIndex = 0;
            this.lblRemoveProfile1.Text = "Profile";
            // 
            // EditProfiles
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(548, 623);
            this.Controls.Add(this.mtcTabControl);
            this.Controls.Add(this.grpProfiles);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditProfiles";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Profile Settings";
            this.Load += new System.EventHandler(this.EditProfiles_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditProfile_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
            this.grpProfiles.ResumeLayout(false);
            this.mtcTabControl.ResumeLayout(false);
            this.mtpAddTabPage.ResumeLayout(false);
            this.mtpAddTabPage.PerformLayout();
            this.mtpRenameTabPage.ResumeLayout(false);
            this.mtpRenameTabPage.PerformLayout();
            this.mtpMoveTabPage.ResumeLayout(false);
            this.mtpMoveTabPage.PerformLayout();
            this.mtpRemoveTabPage.ResumeLayout(false);
            this.mtpRemoveTabPage.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.DataGridView dgvProfiles;
        private MetroFramework.Controls.MetroButton btnProfileUp;
        private MetroFramework.Controls.MetroButton btnProfileDown;
        private System.Windows.Forms.GroupBox grpProfiles;
        private MetroFramework.Controls.MetroTabControl mtcTabControl;
        private MetroFramework.Controls.MetroTabPage mtpAddTabPage;
        private MetroFramework.Controls.MetroButton btnAddProfile;
        private MetroFramework.Controls.MetroTextBox txtAddProfile;
        private MetroFramework.Controls.MetroLabel lblAddProfile1;
        private MetroFramework.Controls.MetroTabPage mtpMoveTabPage;
        private MetroFramework.Controls.MetroButton btnMoveProfile;
        private MetroFramework.Controls.MetroLabel lblMoveProfile2;
        private MetroFramework.Controls.MetroComboBox cboProfileMoveTo;
        private MetroFramework.Controls.MetroLabel lblMoveProfile1;
        private MetroFramework.Controls.MetroComboBox cboProfileMoveFrom;
        private MetroFramework.Controls.MetroTabPage mtpRemoveTabPage;
        private MetroFramework.Controls.MetroButton btnRemoveProfile;
        private MetroFramework.Controls.MetroComboBox cboProfileRemove;
        private MetroFramework.Controls.MetroLabel lblRemoveProfile1;
        private MetroFramework.Controls.MetroTabPage mtpRenameTabPage;
        private MetroFramework.Controls.MetroButton btnRenameProfile;
        private MetroFramework.Controls.MetroTextBox txtRenameProfile;
        private MetroFramework.Controls.MetroLabel lblRenameProfile2;
        private MetroFramework.Controls.MetroComboBox cboProfileRename;
        private MetroFramework.Controls.MetroLabel lblRenameProfile1;
    }
}