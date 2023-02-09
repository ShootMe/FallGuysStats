using System;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    partial class EditShows {
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
            this.picEditShowsIcon = new PictureBox();
            this.lblEditShowsQuestion = new System.Windows.Forms.Label();
            this.lblEditShowslabel = new System.Windows.Forms.Label();
            this.cboEditShows = new System.Windows.Forms.ComboBox();
            this.lblEditShowsBackColor = new System.Windows.Forms.Label();
            this.btnEditShowsSave = new System.Windows.Forms.Button();
            this.btnEditShowsCancel = new System.Windows.Forms.Button();
            this.picEditShowsIcon.SuspendLayout();
            this.lblEditShowsQuestion.SuspendLayout();
            this.lblEditShowslabel.SuspendLayout();
            this.cboEditShows.SuspendLayout();
            this.lblEditShowsBackColor.SuspendLayout();
            this.btnEditShowsSave.SuspendLayout();
            this.btnEditShowsCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // picEditShowsIcon
            // 
            this.picEditShowsIcon.Location = new System.Drawing.Point(24, 17);
            this.picEditShowsIcon.Name = "picEditShowsIcon";
            this.picEditShowsIcon.Width = 36;
            this.picEditShowsIcon.Height = 36;
            this.picEditShowsIcon.Image = Properties.Resources.fallguys_icon;
            this.picEditShowsIcon.SizeMode = PictureBoxSizeMode.Zoom;
            // 
            // lblEditShowsQuestion
            // 
            this.lblEditShowsQuestion.AutoSize = true;
            this.lblEditShowsQuestion.Location = new System.Drawing.Point(74, 23);
            this.lblEditShowsQuestion.Name = "lblEditShowsQuestion";
            this.lblEditShowsQuestion.Size = new System.Drawing.Size(62, 12);
            this.lblEditShowsQuestion.Text = "Description";
            // 
            // lblEditShowslabel
            // 
            this.lblEditShowslabel.AutoSize = true;
            this.lblEditShowslabel.Location = new System.Drawing.Point(74, 58);
            this.lblEditShowslabel.Name = "lblEditShowslabel";
            this.lblEditShowslabel.Size = new System.Drawing.Size(62, 12);
            this.lblEditShowslabel.Text = "Profile List";
            // 
            // cboEditShows
            // 
            this.cboEditShows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEditShows.FormattingEnabled = true;
            this.cboEditShows.Location = new System.Drawing.Point(142, 58);
            this.cboEditShows.Name = "cboEditShows";
            this.cboEditShows.Size = new System.Drawing.Size(170, 20);
            this.cboEditShows.TabIndex = 0;
            this.cboEditShows.SelectedIndexChanged += new EventHandler(this.cboEditShows_Changed);
            // 
            // lblEditShowsBackColor
            // 
            this.lblEditShowsBackColor.Location = new System.Drawing.Point(0, 95);
            this.lblEditShowsBackColor.Name = "lblEditShowsBackColor";
            this.lblEditShowsBackColor.Size = new System.Drawing.Size(350, 40);
            this.lblEditShowsBackColor.BackColor = Color.FromArgb(40,0,182,254);
            // 
            // btnEditShowsSave
            // 
            this.btnEditShowsSave.Location = new System.Drawing.Point(175, 105);
            this.btnEditShowsSave.Name = "btnEditShowsSave";
            this.btnEditShowsSave.Size = new System.Drawing.Size(75, 18);
            this.btnEditShowsSave.TabIndex = 1;
            this.btnEditShowsSave.Text = "Save";
            this.btnEditShowsSave.UseVisualStyleBackColor = true;
            this.btnEditShowsSave.Click += new System.EventHandler(this.btnEditShowsSave_Click);
            // 
            // btnEditShowsCancel
            // 
            this.btnEditShowsCancel.Location = new System.Drawing.Point(260, 105);
            this.btnEditShowsCancel.Name = "btnEditShowsCancel";
            this.btnEditShowsCancel.Size = new System.Drawing.Size(75, 18);
            this.btnEditShowsCancel.TabIndex = 2;
            this.btnEditShowsCancel.Text = Multilingual.GetWord("profile_undo_change_button");
            this.btnEditShowsCancel.UseVisualStyleBackColor = true;
            this.btnEditShowsCancel.Click += new System.EventHandler(this.btnEditShowsCancel_Click);
            // 
            // EditShows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 135);
            this.Controls.Add(this.picEditShowsIcon);
            this.Controls.Add(this.lblEditShowsQuestion);
            this.Controls.Add(this.lblEditShowslabel);
            this.Controls.Add(this.cboEditShows);
            this.Controls.Add(this.lblEditShowsBackColor);
            this.Controls.Add(this.btnEditShowsSave);
            this.Controls.Add(this.btnEditShowsCancel);
            this.Name = "EditShows";
            this.Text = "Title";
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new EventHandler(this.EditShows_Load);
            
            this.picEditShowsIcon.ResumeLayout(false);
            this.lblEditShowsQuestion.ResumeLayout(false);
            this.lblEditShowslabel.ResumeLayout(false);
            this.cboEditShows.ResumeLayout(false);
            this.lblEditShowsBackColor.ResumeLayout(false);
            this.btnEditShowsSave.ResumeLayout(false);
            this.btnEditShowsCancel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.btnEditShowsSave.BringToFront();
            this.btnEditShowsCancel.BringToFront();
        }

        #endregion
        private PictureBox picEditShowsIcon;
        private System.Windows.Forms.Label lblEditShowsQuestion;
        private System.Windows.Forms.Label lblEditShowslabel;
        private System.Windows.Forms.ComboBox cboEditShows;
        private System.Windows.Forms.Label lblEditShowsBackColor;
        private System.Windows.Forms.Button btnEditShowsSave;
        private System.Windows.Forms.Button btnEditShowsCancel;
    }
}