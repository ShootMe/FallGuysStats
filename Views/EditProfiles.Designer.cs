using System;
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
        private void InitializeComponent() {
            this.ProfileList = new System.Windows.Forms.ListBox();
            this.ProfileListUp = new System.Windows.Forms.Button();
            this.ProfileListDown = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.AddTabPage = new System.Windows.Forms.TabPage();
            this.AddPageButton = new System.Windows.Forms.Button();
            this.AddPageTextbox = new System.Windows.Forms.TextBox();
            this.AddPageLabel1 = new System.Windows.Forms.Label();
            this.MoveTabPage = new System.Windows.Forms.TabPage();
            this.MovePageButton = new System.Windows.Forms.Button();
            this.MovePageLabel2 = new System.Windows.Forms.Label();
            this.MoveToCombobox = new System.Windows.Forms.ComboBox();
            this.MovePageLabel1 = new System.Windows.Forms.Label();
            this.MoveFromCombobox = new System.Windows.Forms.ComboBox();
            this.RemoveTabPage = new System.Windows.Forms.TabPage();
            //this.RemovePageLabel2 = new System.Windows.Forms.Label();
            this.RemovePageButton = new System.Windows.Forms.Button();
            this.RemoveProfileCombobox = new System.Windows.Forms.ComboBox();
            this.RemovePageLabel1 = new System.Windows.Forms.Label();
            this.RenameTabPage = new System.Windows.Forms.TabPage();
            this.RenamePageLabel1 = new System.Windows.Forms.Label();
            this.RenamePageCombobox = new System.Windows.Forms.ComboBox();
            this.RenamePageLabel2 = new System.Windows.Forms.Label();
            this.RenamePageTextbox = new System.Windows.Forms.TextBox();
            this.RenameButton = new System.Windows.Forms.Button();
            //this.ApplyChangeButton = new System.Windows.Forms.Button();
            //this.UndoChangeButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.AddTabPage.SuspendLayout();
            this.MoveTabPage.SuspendLayout();
            this.RemoveTabPage.SuspendLayout();
            this.RenameTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ProfileList);
            this.groupBox1.Controls.Add(this.ProfileListUp);
            this.groupBox1.Controls.Add(this.ProfileListDown);
            this.groupBox1.Location = new System.Drawing.Point(8, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 170);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile List";
            // 
            // ProfileList
            // 
            this.ProfileList.FormattingEnabled = true;
            this.ProfileList.ItemHeight = 12;
            this.ProfileList.Cursor = Cursors.Hand;
            /*this.ProfileList.Items.AddRange(new object[] {
                "Solo",
                "Duo",
                "Squad"});*/
            this.ProfileList.Location = new System.Drawing.Point(6, 15);
            this.ProfileList.Name = "ProfileList";
            this.ProfileList.Size = new System.Drawing.Size(273, 150);
            this.ProfileList.TabIndex = 2;
            // 
            // ProfileListUp
            // 
            this.ProfileListUp.Location = new System.Drawing.Point(282, 14);
            this.ProfileListUp.Name = "ProfileListUp";
            this.ProfileListUp.Size = new System.Drawing.Size(19, 35);
            this.ProfileListUp.TabIndex = 3;
            this.ProfileListUp.Text = "∧";
            this.ProfileListUp.UseVisualStyleBackColor = true;
            this.ProfileListUp.Click += new System.EventHandler(this.ProfileListUp_Click);
            // 
            // ProfileListDown
            // 
            this.ProfileListDown.Location = new System.Drawing.Point(282, 128);
            this.ProfileListDown.Name = "ProfileListDown";
            this.ProfileListDown.Size = new System.Drawing.Size(19, 35);
            this.ProfileListDown.TabIndex = 4;
            this.ProfileListDown.Text = "∨";
            this.ProfileListDown.UseVisualStyleBackColor = true;
            this.ProfileListDown.Click += new System.EventHandler(this.ProfileListDown_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.AddTabPage);
            this.tabControl1.Controls.Add(this.RenameTabPage);
            this.tabControl1.Controls.Add(this.MoveTabPage);
            this.tabControl1.Controls.Add(this.RemoveTabPage);
            this.tabControl1.Location = new System.Drawing.Point(8, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(308, 100);
            this.tabControl1.TabIndex = 6;
            // 
            // AddTabPage
            // 
            this.AddTabPage.Controls.Add(this.AddPageButton);
            this.AddTabPage.Controls.Add(this.AddPageTextbox);
            this.AddTabPage.Controls.Add(this.AddPageLabel1);
            this.AddTabPage.Location = new System.Drawing.Point(4, 22);
            this.AddTabPage.Name = "AddTabPage";
            this.AddTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.AddTabPage.Size = new System.Drawing.Size(291, 84);
            this.AddTabPage.TabIndex = 0;
            this.AddTabPage.Text = "Add";
            this.AddTabPage.UseVisualStyleBackColor = true;
            // 
            // AddPageLabel1
            // 
            this.AddPageLabel1.AutoSize = true;
            this.AddPageLabel1.Location = new System.Drawing.Point(6, 7);
            this.AddPageLabel1.Name = "AddPageLabel1";
            this.AddPageLabel1.Size = new System.Drawing.Size(34, 12);
            this.AddPageLabel1.TabIndex = 0;
            this.AddPageLabel1.Text = "Profile Name";
            // 
            // AddPageTextbox
            // 
            this.AddPageTextbox.Location = new System.Drawing.Point(82, 7);
            this.AddPageTextbox.Name = "AddPageTextbox";
            this.AddPageTextbox.Size = new System.Drawing.Size(153, 17);
            this.AddPageTextbox.MaxLength = 20;
            this.AddPageTextbox.TabIndex = 1;
            this.AddPageTextbox.KeyPress += new KeyPressEventHandler(this.DeleteAmpersend_KeyPress);
            // 
            // AddPageButton
            // 
            this.AddPageButton.Location = new System.Drawing.Point(7, 30);
            this.AddPageButton.Name = "AddPageButton";
            this.AddPageButton.Size = new System.Drawing.Size(65, 18);
            this.AddPageButton.TabIndex = 2;
            this.AddPageButton.Text = "Add";
            this.AddPageButton.UseVisualStyleBackColor = true;
            this.AddPageButton.Click += new System.EventHandler(this.AddPageButton_Click);
            // 
            // RenameTabPage
            // 
            this.RenameTabPage.Controls.Add(this.RenameButton);
            this.RenameTabPage.Controls.Add(this.RenamePageTextbox);
            this.RenameTabPage.Controls.Add(this.RenamePageLabel2);
            this.RenameTabPage.Controls.Add(this.RenamePageCombobox);
            this.RenameTabPage.Controls.Add(this.RenamePageLabel1);
            this.RenameTabPage.Location = new System.Drawing.Point(4, 22);
            this.RenameTabPage.Name = "RenameTabPage";
            this.RenameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.RenameTabPage.Size = new System.Drawing.Size(291, 84);
            this.RenameTabPage.TabIndex = 3;
            this.RenameTabPage.Text = "Rename";
            this.RenameTabPage.UseVisualStyleBackColor = true;
            // 
            // RenamePageLabel1
            // 
            this.RenamePageLabel1.AutoSize = true;
            this.RenamePageLabel1.Location = new System.Drawing.Point(6, 7);
            this.RenamePageLabel1.Name = "RenamePageLabel1";
            this.RenamePageLabel1.Size = new System.Drawing.Size(38, 12);
            this.RenamePageLabel1.TabIndex = 0;
            this.RenamePageLabel1.Text = "Profile";
            // 
            // RenamePageCombobox
            // 
            this.RenamePageCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RenamePageCombobox.FormattingEnabled = true;
            this.RenamePageCombobox.Location = new System.Drawing.Point(82, 7);
            this.RenamePageCombobox.Name = "RenamePageCombobox";
            this.RenamePageCombobox.Size = new System.Drawing.Size(165, 17);
            this.RenamePageCombobox.TabIndex = 1;
            this.RenamePageCombobox.SelectedIndexChanged += new EventHandler(this.RenameComboxChanged);
            // 
            // RenamePageLabel2
            // 
            this.RenamePageLabel2.AutoSize = true;
            this.RenamePageLabel2.Location = new System.Drawing.Point(6, 29);
            this.RenamePageLabel2.Name = "RenamePageLabel2";
            this.RenamePageLabel2.Size = new System.Drawing.Size(34, 12);
            this.RenamePageLabel2.TabIndex = 2;
            this.RenamePageLabel2.Text = "Profile Name";
            // 
            // RenamePageTextbox
            // 
            this.RenamePageTextbox.Location = new System.Drawing.Point(82, 29);
            this.RenamePageTextbox.Name = "RenamePageTextbox";
            this.RenamePageTextbox.Size = new System.Drawing.Size(153, 17);
            this.RenamePageTextbox.MaxLength = 20;
            this.RenamePageTextbox.TabIndex = 3;
            this.RenamePageTextbox.KeyPress += new KeyPressEventHandler(this.DeleteAmpersend_KeyPress);
            // 
            // RenameButton
            // 
            this.RenameButton.Location = new System.Drawing.Point(7, 54);
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.Size = new System.Drawing.Size(65, 18);
            this.RenameButton.TabIndex = 4;
            this.RenameButton.Text = "Rename";
            this.RenameButton.UseVisualStyleBackColor = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // MoveTabPage
            // 
            this.MoveTabPage.Controls.Add(this.MovePageButton);
            this.MoveTabPage.Controls.Add(this.MovePageLabel2);
            this.MoveTabPage.Controls.Add(this.MoveToCombobox);
            this.MoveTabPage.Controls.Add(this.MovePageLabel1); 
            this.MoveTabPage.Controls.Add(this.MoveFromCombobox);
            this.MoveTabPage.Location = new System.Drawing.Point(4, 22);
            this.MoveTabPage.Name = "MoveTabPage";
            this.MoveTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MoveTabPage.Size = new System.Drawing.Size(291, 84);
            this.MoveTabPage.TabIndex = 1;
            this.MoveTabPage.Text = "Move";
            this.MoveTabPage.UseVisualStyleBackColor = true;
            // 
            // MovePageLabel1
            // 
            this.MovePageLabel1.AutoSize = true;
            this.MovePageLabel1.Location = new System.Drawing.Point(6, 7);
            this.MovePageLabel1.Name = "MovePageLabel1";
            this.MovePageLabel1.Size = new System.Drawing.Size(31, 12);
            this.MovePageLabel1.TabIndex = 1;
            this.MovePageLabel1.Text = "From";
            // 
            // MovePageLabel2
            // 
            this.MovePageLabel2.AutoSize = true;
            this.MovePageLabel2.Location = new System.Drawing.Point(6, 29);
            this.MovePageLabel2.Name = "MovePageLabel2";
            this.MovePageLabel2.Size = new System.Drawing.Size(18, 12);
            this.MovePageLabel2.TabIndex = 3;
            this.MovePageLabel2.Text = "To";
            // 
            // MoveFromCombobox
            // 
            this.MoveFromCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MoveFromCombobox.FormattingEnabled = true;
            this.MoveFromCombobox.Location = new System.Drawing.Point(58, 7);
            this.MoveFromCombobox.Name = "MoveFromCombobox";
            this.MoveFromCombobox.Size = new System.Drawing.Size(165, 17);
            this.MoveFromCombobox.TabIndex = 0;
            // 
            // MoveToCombobox
            // 
            this.MoveToCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MoveToCombobox.FormattingEnabled = true;
            this.MoveToCombobox.Location = new System.Drawing.Point(58, 29);
            this.MoveToCombobox.Name = "MoveToCombobox";
            this.MoveToCombobox.Size = new System.Drawing.Size(165, 17);
            this.MoveToCombobox.TabIndex = 2;
            // 
            // MovePageButton
            // 
            this.MovePageButton.Location = new System.Drawing.Point(7, 54);
            this.MovePageButton.Name = "MovePageButton";
            this.MovePageButton.Size = new System.Drawing.Size(65, 18);
            this.MovePageButton.TabIndex = 4;
            this.MovePageButton.Text = "Move";
            this.MovePageButton.UseVisualStyleBackColor = true;
            this.MovePageButton.Click += new System.EventHandler(this.MovePageButton_Click);
            // 
            // RemoveTabPage
            // 
            //this.RemoveTabPage.Controls.Add(this.RemovePageLabel2);
            this.RemoveTabPage.Controls.Add(this.RemovePageButton);
            this.RemoveTabPage.Controls.Add(this.RemoveProfileCombobox);
            this.RemoveTabPage.Controls.Add(this.RemovePageLabel1);
            this.RemoveTabPage.Location = new System.Drawing.Point(4, 22);
            this.RemoveTabPage.Name = "RemoveTabPage";
            this.RemoveTabPage.Size = new System.Drawing.Size(291, 84);
            this.RemoveTabPage.TabIndex = 2;
            this.RemoveTabPage.Text = "Remove";
            this.RemoveTabPage.UseVisualStyleBackColor = true;
            // 
            // RemovePageLabel2
            // 
            /*this.RemovePageLabel2.AutoSize = true;
            this.RemovePageLabel2.Location = new System.Drawing.Point(8, 62);
            this.RemovePageLabel2.Name = "RemovePageLabel2";
            this.RemovePageLabel2.Size = new System.Drawing.Size(221, 12);
            this.RemovePageLabel2.TabIndex = 3;
            this.RemovePageLabel2.ForeColor = System.Drawing.Color.DimGray;
            this.RemovePageLabel2.Text = "* You can only delete profile with no data";*/
            // 
            // RemovePageLabel1
            // 
            this.RemovePageLabel1.AutoSize = true;
            this.RemovePageLabel1.Location = new System.Drawing.Point(6, 7);
            this.RemovePageLabel1.Name = "RemovePageLabel1";
            this.RemovePageLabel1.Size = new System.Drawing.Size(38, 12);
            this.RemovePageLabel1.TabIndex = 0;
            this.RemovePageLabel1.Text = "Profile";
            // 
            // RemoveProfileCombobox
            // 
            this.RemoveProfileCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RemoveProfileCombobox.FormattingEnabled = true;
            this.RemoveProfileCombobox.Location = new System.Drawing.Point(52, 7);
            this.RemoveProfileCombobox.Name = "RemoveProfileCombobox";
            this.RemoveProfileCombobox.Size = new System.Drawing.Size(165, 17);
            this.RemoveProfileCombobox.TabIndex = 1;
            // 
            // 
            // RemovePageButton
            // 
            this.RemovePageButton.Location = new System.Drawing.Point(7, 30);
            this.RemovePageButton.Name = "RemovePageButton";
            this.RemovePageButton.Size = new System.Drawing.Size(65, 18);
            this.RemovePageButton.TabIndex = 2;
            this.RemovePageButton.Text = "Remove";
            this.RemovePageButton.UseVisualStyleBackColor = true;
            this.RemovePageButton.Click += new System.EventHandler(this.RemovePageButton_Click);
            // ApplyChangeButton
            // 
            /*this.ApplyChangeButton.Location = new System.Drawing.Point(158, 347);
            this.ApplyChangeButton.Name = "ApplyChangeButton";
            this.ApplyChangeButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyChangeButton.TabIndex = 98;
            this.ApplyChangeButton.Text = "Save";
            this.ApplyChangeButton.UseVisualStyleBackColor = true;
            this.ApplyChangeButton.Click += new System.EventHandler(this.ApplyChangeButton_Click);*/
            // 
            // UndoChangeButton
            // 
            /*this.UndoChangeButton.Location = new System.Drawing.Point(238, 347);
            this.UndoChangeButton.Name = "UndoChangeButton";
            this.UndoChangeButton.Size = new System.Drawing.Size(75, 23);
            this.UndoChangeButton.TabIndex = 99;
            this.UndoChangeButton.Text = "Cancel";
            this.UndoChangeButton.UseVisualStyleBackColor = true;
            this.UndoChangeButton.Click += new System.EventHandler(this.UndoChangeButton_Click);*/
            // 
            // EditProfiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 286);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            //this.Controls.Add(this.ApplyChangeButton);
            //this.Controls.Add(this.UndoChangeButton);
            this.Name = "EditProfiles";
            this.Text = "Profile Settings";
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new EventHandler(this.EditProfiles_Load);
            this.KeyDown += new KeyEventHandler(this.EditProfile_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.AddTabPage.ResumeLayout(false);
            this.AddTabPage.PerformLayout();
            this.MoveTabPage.ResumeLayout(false);
            this.MoveTabPage.PerformLayout();
            this.RemoveTabPage.ResumeLayout(false);
            this.RemoveTabPage.PerformLayout();
            this.RenameTabPage.ResumeLayout(false);
            this.RenameTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ListBox ProfileList;
        private System.Windows.Forms.Button ProfileListUp;
        private System.Windows.Forms.Button ProfileListDown;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage AddTabPage;
        private System.Windows.Forms.Button AddPageButton;
        private System.Windows.Forms.TextBox AddPageTextbox;
        private System.Windows.Forms.Label AddPageLabel1;
        private System.Windows.Forms.TabPage MoveTabPage;
        private System.Windows.Forms.Button MovePageButton;
        private System.Windows.Forms.Label MovePageLabel2;
        private System.Windows.Forms.ComboBox MoveToCombobox;
        private System.Windows.Forms.Label MovePageLabel1;
        private System.Windows.Forms.ComboBox MoveFromCombobox;
        private System.Windows.Forms.TabPage RemoveTabPage;
        private System.Windows.Forms.Button RemovePageButton;
        private System.Windows.Forms.ComboBox RemoveProfileCombobox;
        private System.Windows.Forms.Label RemovePageLabel1;
        //private System.Windows.Forms.Label RemovePageLabel2;
        private System.Windows.Forms.TabPage RenameTabPage;
        private System.Windows.Forms.Button RenameButton;
        //private System.Windows.Forms.Button ApplyChangeButton;
        //private System.Windows.Forms.Button UndoChangeButton;
        private System.Windows.Forms.TextBox RenamePageTextbox;
        private System.Windows.Forms.Label RenamePageLabel2;
        private System.Windows.Forms.ComboBox RenamePageCombobox;
        private System.Windows.Forms.Label RenamePageLabel1;
    }
}