using System;
using System.Drawing;
using System.Windows.Forms;
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
            this.ProfileList = new System.Windows.Forms.DataGridView();
            this.ProfileListUp = new MetroFramework.Controls.MetroButton();
            this.ProfileListDown = new MetroFramework.Controls.MetroButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.AddTabPage = new MetroFramework.Controls.MetroTabPage();
            this.AddPageButton = new MetroFramework.Controls.MetroButton();
            this.AddPageTextbox = new MetroFramework.Controls.MetroTextBox();
            this.AddPageLabel1 = new MetroFramework.Controls.MetroLabel();
            this.RenameTabPage = new MetroFramework.Controls.MetroTabPage();
            this.RenameButton = new MetroFramework.Controls.MetroButton();
            this.RenamePageTextbox = new MetroFramework.Controls.MetroTextBox();
            this.RenamePageLabel2 = new MetroFramework.Controls.MetroLabel();
            this.RenamePageCombobox = new MetroFramework.Controls.MetroComboBox();
            this.RenamePageLabel1 = new MetroFramework.Controls.MetroLabel();
            this.MoveTabPage = new MetroFramework.Controls.MetroTabPage();
            this.MovePageButton = new MetroFramework.Controls.MetroButton();
            this.MovePageLabel2 = new MetroFramework.Controls.MetroLabel();
            this.MoveToCombobox = new MetroFramework.Controls.MetroComboBox();
            this.MovePageLabel1 = new MetroFramework.Controls.MetroLabel();
            this.MoveFromCombobox = new MetroFramework.Controls.MetroComboBox();
            this.RemoveTabPage = new MetroFramework.Controls.MetroTabPage();
            this.RemovePageButton = new MetroFramework.Controls.MetroButton();
            this.RemoveProfileCombobox = new MetroFramework.Controls.MetroComboBox();
            this.RemovePageLabel1 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ProfileList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.AddTabPage.SuspendLayout();
            this.RenameTabPage.SuspendLayout();
            this.MoveTabPage.SuspendLayout();
            this.RemoveTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProfileList
            // 
            this.ProfileList.AllowUserToAddRows = false;
            this.ProfileList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.ProfileList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.ProfileList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ProfileList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ProfileList.ColumnHeadersVisible = false;
            this.ProfileList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProfileList.Location = new System.Drawing.Point(7, 20);
            this.ProfileList.MultiSelect = false;
            this.ProfileList.Name = "ProfileList";
            this.ProfileList.RowHeadersVisible = false;
            this.ProfileList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ProfileList.Size = new System.Drawing.Size(365, 248);
            this.ProfileList.TabIndex = 0;
            this.ProfileList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProfileList_CellClick);
            this.ProfileList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.ProfileList_CellFormatting);
            this.ProfileList.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ProfileList_EditingControlShowing);
            // 
            // ProfileListUp
            // 
            this.ProfileListUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProfileListUp.Location = new System.Drawing.Point(375, 19);
            this.ProfileListUp.Name = "ProfileListUp";
            this.ProfileListUp.Size = new System.Drawing.Size(20, 50);
            this.ProfileListUp.TabIndex = 3;
            this.ProfileListUp.Text = "∧";
            this.ProfileListUp.UseSelectable = true;
            this.ProfileListUp.Click += new System.EventHandler(this.ProfileListUp_Click);
            // 
            // ProfileListDown
            // 
            this.ProfileListDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProfileListDown.Location = new System.Drawing.Point(375, 218);
            this.ProfileListDown.Name = "ProfileListDown";
            this.ProfileListDown.Size = new System.Drawing.Size(20, 50);
            this.ProfileListDown.TabIndex = 4;
            this.ProfileListDown.Text = "∨";
            this.ProfileListDown.UseSelectable = true;
            this.ProfileListDown.Click += new System.EventHandler(this.ProfileListDown_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ProfileList);
            this.groupBox1.Controls.Add(this.ProfileListUp);
            this.groupBox1.Controls.Add(this.ProfileListDown);
            this.groupBox1.Location = new System.Drawing.Point(9, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 275);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile List";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.AddTabPage);
            this.tabControl1.Controls.Add(this.RenameTabPage);
            this.tabControl1.Controls.Add(this.MoveTabPage);
            this.tabControl1.Controls.Add(this.RemoveTabPage);
            this.tabControl1.Location = new System.Drawing.Point(9, 65);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(382, 165);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.UseSelectable = true;
            // 
            // AddTabPage
            // 
            this.AddTabPage.Controls.Add(this.AddPageButton);
            this.AddTabPage.Controls.Add(this.AddPageTextbox);
            this.AddTabPage.Controls.Add(this.AddPageLabel1);
            this.AddTabPage.HorizontalScrollbarBarColor = true;
            this.AddTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.AddTabPage.HorizontalScrollbarSize = 10;
            this.AddTabPage.Location = new System.Drawing.Point(4, 38);
            this.AddTabPage.Name = "AddTabPage";
            this.AddTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.AddTabPage.Size = new System.Drawing.Size(374, 123);
            this.AddTabPage.TabIndex = 0;
            this.AddTabPage.Text = "Add";
            this.AddTabPage.VerticalScrollbarBarColor = true;
            this.AddTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.AddTabPage.VerticalScrollbarSize = 10;
            // 
            // AddPageButton
            // 
            this.AddPageButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddPageButton.Location = new System.Drawing.Point(8, 88);
            this.AddPageButton.Name = "AddPageButton";
            this.AddPageButton.Size = new System.Drawing.Size(76, 26);
            this.AddPageButton.TabIndex = 2;
            this.AddPageButton.Text = "Add";
            this.AddPageButton.UseSelectable = true;
            this.AddPageButton.Click += new System.EventHandler(this.AddPageButton_Click);
            // 
            // AddPageTextbox
            // 
            // 
            // 
            // 
            this.AddPageTextbox.CustomButton.Image = null;
            this.AddPageTextbox.CustomButton.Location = new System.Drawing.Point(154, 1);
            this.AddPageTextbox.CustomButton.Name = "";
            this.AddPageTextbox.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.AddPageTextbox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.AddPageTextbox.CustomButton.TabIndex = 1;
            this.AddPageTextbox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.AddPageTextbox.CustomButton.UseSelectable = true;
            this.AddPageTextbox.CustomButton.Visible = false;
            this.AddPageTextbox.Lines = new string[0];
            this.AddPageTextbox.Location = new System.Drawing.Point(96, 10);
            this.AddPageTextbox.MaxLength = 20;
            this.AddPageTextbox.Name = "AddPageTextbox";
            this.AddPageTextbox.PasswordChar = '\0';
            this.AddPageTextbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.AddPageTextbox.SelectedText = "";
            this.AddPageTextbox.SelectionLength = 0;
            this.AddPageTextbox.SelectionStart = 0;
            this.AddPageTextbox.ShortcutsEnabled = true;
            this.AddPageTextbox.Size = new System.Drawing.Size(178, 25);
            this.AddPageTextbox.TabIndex = 1;
            this.AddPageTextbox.UseSelectable = true;
            this.AddPageTextbox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.AddPageTextbox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.AddPageTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DeleteAmpersend_KeyPress);
            // 
            // AddPageLabel1
            // 
            this.AddPageLabel1.AutoSize = true;
            this.AddPageLabel1.Location = new System.Drawing.Point(7, 9);
            this.AddPageLabel1.Name = "AddPageLabel1";
            this.AddPageLabel1.Size = new System.Drawing.Size(87, 19);
            this.AddPageLabel1.TabIndex = 0;
            this.AddPageLabel1.Text = "Profile Name";
            // 
            // RenameTabPage
            // 
            this.RenameTabPage.Controls.Add(this.RenameButton);
            this.RenameTabPage.Controls.Add(this.RenamePageTextbox);
            this.RenameTabPage.Controls.Add(this.RenamePageLabel2);
            this.RenameTabPage.Controls.Add(this.RenamePageCombobox);
            this.RenameTabPage.Controls.Add(this.RenamePageLabel1);
            this.RenameTabPage.HorizontalScrollbarBarColor = true;
            this.RenameTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.RenameTabPage.HorizontalScrollbarSize = 10;
            this.RenameTabPage.Location = new System.Drawing.Point(4, 38);
            this.RenameTabPage.Name = "RenameTabPage";
            this.RenameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.RenameTabPage.Size = new System.Drawing.Size(374, 123);
            this.RenameTabPage.TabIndex = 3;
            this.RenameTabPage.Text = "Rename";
            this.RenameTabPage.VerticalScrollbarBarColor = true;
            this.RenameTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.RenameTabPage.VerticalScrollbarSize = 10;
            // 
            // RenameButton
            // 
            this.RenameButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RenameButton.Location = new System.Drawing.Point(8, 88);
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.Size = new System.Drawing.Size(76, 26);
            this.RenameButton.TabIndex = 4;
            this.RenameButton.Text = "Rename";
            this.RenameButton.UseSelectable = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // RenamePageTextbox
            // 
            // 
            // 
            // 
            this.RenamePageTextbox.CustomButton.Image = null;
            this.RenamePageTextbox.CustomButton.Location = new System.Drawing.Point(154, 1);
            this.RenamePageTextbox.CustomButton.Name = "";
            this.RenamePageTextbox.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.RenamePageTextbox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.RenamePageTextbox.CustomButton.TabIndex = 1;
            this.RenamePageTextbox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.RenamePageTextbox.CustomButton.UseSelectable = true;
            this.RenamePageTextbox.CustomButton.Visible = false;
            this.RenamePageTextbox.Lines = new string[0];
            this.RenamePageTextbox.Location = new System.Drawing.Point(96, 45);
            this.RenamePageTextbox.MaxLength = 20;
            this.RenamePageTextbox.Name = "RenamePageTextbox";
            this.RenamePageTextbox.PasswordChar = '\0';
            this.RenamePageTextbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.RenamePageTextbox.SelectedText = "";
            this.RenamePageTextbox.SelectionLength = 0;
            this.RenamePageTextbox.SelectionStart = 0;
            this.RenamePageTextbox.ShortcutsEnabled = true;
            this.RenamePageTextbox.Size = new System.Drawing.Size(178, 25);
            this.RenamePageTextbox.TabIndex = 3;
            this.RenamePageTextbox.UseSelectable = true;
            this.RenamePageTextbox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.RenamePageTextbox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.RenamePageTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DeleteAmpersend_KeyPress);
            // 
            // RenamePageLabel2
            // 
            this.RenamePageLabel2.AutoSize = true;
            this.RenamePageLabel2.Location = new System.Drawing.Point(7, 47);
            this.RenamePageLabel2.Name = "RenamePageLabel2";
            this.RenamePageLabel2.Size = new System.Drawing.Size(87, 19);
            this.RenamePageLabel2.TabIndex = 2;
            this.RenamePageLabel2.Text = "Profile Name";
            // 
            // RenamePageCombobox
            // 
            this.RenamePageCombobox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RenamePageCombobox.FormattingEnabled = true;
            this.RenamePageCombobox.Location = new System.Drawing.Point(96, 7);
            this.RenamePageCombobox.Name = "RenamePageCombobox";
            this.RenamePageCombobox.Size = new System.Drawing.Size(192, 22);
            this.RenamePageCombobox.TabIndex = 1;
            this.RenamePageCombobox.UseSelectable = true;
            this.RenamePageCombobox.SelectedIndexChanged += new System.EventHandler(this.RenameComboxChanged);
            // 
            // RenamePageLabel1
            // 
            this.RenamePageLabel1.AutoSize = true;
            this.RenamePageLabel1.Location = new System.Drawing.Point(7, 9);
            this.RenamePageLabel1.Name = "RenamePageLabel1";
            this.RenamePageLabel1.Size = new System.Drawing.Size(47, 19);
            this.RenamePageLabel1.TabIndex = 0;
            this.RenamePageLabel1.Text = "Profile";
            // 
            // MoveTabPage
            // 
            this.MoveTabPage.Controls.Add(this.MovePageButton);
            this.MoveTabPage.Controls.Add(this.MovePageLabel2);
            this.MoveTabPage.Controls.Add(this.MoveToCombobox);
            this.MoveTabPage.Controls.Add(this.MovePageLabel1);
            this.MoveTabPage.Controls.Add(this.MoveFromCombobox);
            this.MoveTabPage.HorizontalScrollbarBarColor = true;
            this.MoveTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.MoveTabPage.HorizontalScrollbarSize = 10;
            this.MoveTabPage.Location = new System.Drawing.Point(4, 38);
            this.MoveTabPage.Name = "MoveTabPage";
            this.MoveTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MoveTabPage.Size = new System.Drawing.Size(374, 123);
            this.MoveTabPage.TabIndex = 1;
            this.MoveTabPage.Text = "Move";
            this.MoveTabPage.VerticalScrollbarBarColor = true;
            this.MoveTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.MoveTabPage.VerticalScrollbarSize = 10;
            // 
            // MovePageButton
            // 
            this.MovePageButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MovePageButton.Location = new System.Drawing.Point(8, 88);
            this.MovePageButton.Name = "MovePageButton";
            this.MovePageButton.Size = new System.Drawing.Size(76, 26);
            this.MovePageButton.TabIndex = 4;
            this.MovePageButton.Text = "Move";
            this.MovePageButton.UseSelectable = true;
            this.MovePageButton.Click += new System.EventHandler(this.MovePageButton_Click);
            // 
            // MovePageLabel2
            // 
            this.MovePageLabel2.AutoSize = true;
            this.MovePageLabel2.Location = new System.Drawing.Point(7, 47);
            this.MovePageLabel2.Name = "MovePageLabel2";
            this.MovePageLabel2.Size = new System.Drawing.Size(22, 19);
            this.MovePageLabel2.TabIndex = 3;
            this.MovePageLabel2.Text = "To";
            // 
            // MoveToCombobox
            // 
            this.MoveToCombobox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MoveToCombobox.FormattingEnabled = true;
            this.MoveToCombobox.Location = new System.Drawing.Point(96, 45);
            this.MoveToCombobox.Name = "MoveToCombobox";
            this.MoveToCombobox.Size = new System.Drawing.Size(192, 22);
            this.MoveToCombobox.TabIndex = 2;
            this.MoveToCombobox.UseSelectable = true;
            // 
            // MovePageLabel1
            // 
            this.MovePageLabel1.AutoSize = true;
            this.MovePageLabel1.Location = new System.Drawing.Point(7, 9);
            this.MovePageLabel1.Name = "MovePageLabel1";
            this.MovePageLabel1.Size = new System.Drawing.Size(41, 19);
            this.MovePageLabel1.TabIndex = 1;
            this.MovePageLabel1.Text = "From";
            // 
            // MoveFromCombobox
            // 
            this.MoveFromCombobox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MoveFromCombobox.FormattingEnabled = true;
            this.MoveFromCombobox.Location = new System.Drawing.Point(96, 7);
            this.MoveFromCombobox.Name = "MoveFromCombobox";
            this.MoveFromCombobox.Size = new System.Drawing.Size(192, 22);
            this.MoveFromCombobox.TabIndex = 0;
            this.MoveFromCombobox.UseSelectable = true;
            // 
            // RemoveTabPage
            // 
            this.RemoveTabPage.Controls.Add(this.RemovePageButton);
            this.RemoveTabPage.Controls.Add(this.RemoveProfileCombobox);
            this.RemoveTabPage.Controls.Add(this.RemovePageLabel1);
            this.RemoveTabPage.HorizontalScrollbarBarColor = true;
            this.RemoveTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.RemoveTabPage.HorizontalScrollbarSize = 10;
            this.RemoveTabPage.Location = new System.Drawing.Point(4, 38);
            this.RemoveTabPage.Name = "RemoveTabPage";
            this.RemoveTabPage.Size = new System.Drawing.Size(374, 123);
            this.RemoveTabPage.TabIndex = 2;
            this.RemoveTabPage.Text = "Remove";
            this.RemoveTabPage.VerticalScrollbarBarColor = true;
            this.RemoveTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.RemoveTabPage.VerticalScrollbarSize = 10;
            // 
            // RemovePageButton
            // 
            this.RemovePageButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RemovePageButton.Location = new System.Drawing.Point(8, 88);
            this.RemovePageButton.Name = "RemovePageButton";
            this.RemovePageButton.Size = new System.Drawing.Size(76, 26);
            this.RemovePageButton.TabIndex = 2;
            this.RemovePageButton.Text = "Remove";
            this.RemovePageButton.UseSelectable = true;
            this.RemovePageButton.Click += new System.EventHandler(this.RemovePageButton_Click);
            // 
            // RemoveProfileCombobox
            // 
            this.RemoveProfileCombobox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RemoveProfileCombobox.FormattingEnabled = true;
            this.RemoveProfileCombobox.Location = new System.Drawing.Point(96, 7);
            this.RemoveProfileCombobox.Name = "RemoveProfileCombobox";
            this.RemoveProfileCombobox.Size = new System.Drawing.Size(192, 22);
            this.RemoveProfileCombobox.TabIndex = 1;
            this.RemoveProfileCombobox.UseSelectable = true;
            // 
            // RemovePageLabel1
            // 
            this.RemovePageLabel1.AutoSize = true;
            this.RemovePageLabel1.Location = new System.Drawing.Point(7, 9);
            this.RemovePageLabel1.Name = "RemovePageLabel1";
            this.RemovePageLabel1.Size = new System.Drawing.Size(47, 19);
            this.RemovePageLabel1.TabIndex = 0;
            this.RemovePageLabel1.Text = "Profile";
            // 
            // EditProfiles
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(419, 520);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditProfiles";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Profile Settings";
            this.Load += new System.EventHandler(this.EditProfiles_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditProfile_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.ProfileList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.AddTabPage.ResumeLayout(false);
            this.AddTabPage.PerformLayout();
            this.RenameTabPage.ResumeLayout(false);
            this.RenameTabPage.PerformLayout();
            this.MoveTabPage.ResumeLayout(false);
            this.MoveTabPage.PerformLayout();
            this.RemoveTabPage.ResumeLayout(false);
            this.RemoveTabPage.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
        //private System.Windows.Forms.ListBox ProfileList;
        private System.Windows.Forms.DataGridView ProfileList;
        private MetroFramework.Controls.MetroButton ProfileListUp;
        private MetroFramework.Controls.MetroButton ProfileListDown;
        private System.Windows.Forms.GroupBox groupBox1;
        private MetroFramework.Controls.MetroTabControl tabControl1;
        private MetroFramework.Controls.MetroTabPage AddTabPage;
        private MetroFramework.Controls.MetroButton AddPageButton;
        private MetroFramework.Controls.MetroTextBox AddPageTextbox;
        private MetroFramework.Controls.MetroLabel AddPageLabel1;
        private MetroFramework.Controls.MetroTabPage MoveTabPage;
        private MetroFramework.Controls.MetroButton MovePageButton;
        private MetroFramework.Controls.MetroLabel MovePageLabel2;
        private MetroFramework.Controls.MetroComboBox MoveToCombobox;
        private MetroFramework.Controls.MetroLabel MovePageLabel1;
        private MetroFramework.Controls.MetroComboBox MoveFromCombobox;
        private MetroFramework.Controls.MetroTabPage RemoveTabPage;
        private MetroFramework.Controls.MetroButton RemovePageButton;
        private MetroFramework.Controls.MetroComboBox RemoveProfileCombobox;
        private MetroFramework.Controls.MetroLabel RemovePageLabel1;
        //private MetroFramework.Controls.MetroLabel RemovePageLabel2;
        private MetroFramework.Controls.MetroTabPage RenameTabPage;
        private MetroFramework.Controls.MetroButton RenameButton;
        //private MetroFramework.Controls.MetroButton ApplyChangeButton;
        //private MetroFramework.Controls.MetroButton UndoChangeButton;
        private MetroFramework.Controls.MetroTextBox RenamePageTextbox;
        private MetroFramework.Controls.MetroLabel RenamePageLabel2;
        private MetroFramework.Controls.MetroComboBox RenamePageCombobox;
        private MetroFramework.Controls.MetroLabel RenamePageLabel1;
    }
}