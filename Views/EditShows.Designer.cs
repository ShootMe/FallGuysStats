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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditShows));
            this.picEditShowsIcon = new System.Windows.Forms.PictureBox();
            this.lblEditShowsQuestion = new MetroFramework.Controls.MetroLabel();
            this.lblEditShowslabel = new MetroFramework.Controls.MetroLabel();
            this.cboEditShows = new MetroFramework.Controls.MetroComboBox();
            this.lblEditShowsBackColor = new System.Windows.Forms.Label();
            this.btnEditShowsSave = new MetroFramework.Controls.MetroButton();
            this.btnEditShowsCancel = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.picEditShowsIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // picEditShowsIcon
            // 
            this.picEditShowsIcon.Image = global::FallGuysStats.Properties.Resources.fallguys_icon;
            this.picEditShowsIcon.Location = new System.Drawing.Point(42, 80);
            this.picEditShowsIcon.Name = "picEditShowsIcon";
            this.picEditShowsIcon.Size = new System.Drawing.Size(45, 39);
            this.picEditShowsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEditShowsIcon.TabIndex = 0;
            this.picEditShowsIcon.TabStop = false;
            // 
            // lblEditShowsQuestion
            // 
            this.lblEditShowsQuestion.AutoSize = true;
            this.lblEditShowsQuestion.Location = new System.Drawing.Point(100, 80);
            this.lblEditShowsQuestion.Name = "lblEditShowsQuestion";
            this.lblEditShowsQuestion.Size = new System.Drawing.Size(74, 19);
            this.lblEditShowsQuestion.TabIndex = 1;
            this.lblEditShowsQuestion.Text = "Description";
            // 
            // lblEditShowslabel
            // 
            this.lblEditShowslabel.AutoSize = true;
            this.lblEditShowslabel.Location = new System.Drawing.Point(86, 137);
            this.lblEditShowslabel.Name = "lblEditShowslabel";
            this.lblEditShowslabel.Size = new System.Drawing.Size(69, 19);
            this.lblEditShowslabel.TabIndex = 2;
            this.lblEditShowslabel.Text = "Profile List";
            // 
            // cboEditShows
            // 
            this.cboEditShows.FormattingEnabled = true;
            this.cboEditShows.ItemHeight = 23;
            this.cboEditShows.Location = new System.Drawing.Point(185, 135);
            this.cboEditShows.Name = "cboEditShows";
            this.cboEditShows.Size = new System.Drawing.Size(198, 29);
            this.cboEditShows.TabIndex = 0;
            this.cboEditShows.UseSelectable = true;
            this.cboEditShows.SelectedIndexChanged += new System.EventHandler(this.cboEditShows_Changed);
            // 
            // lblEditShowsBackColor
            // 
            this.lblEditShowsBackColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(0)))), ((int)(((byte)(182)))), ((int)(((byte)(254)))));
            this.lblEditShowsBackColor.Location = new System.Drawing.Point(0, 191);
            this.lblEditShowsBackColor.Name = "lblEditShowsBackColor";
            this.lblEditShowsBackColor.Size = new System.Drawing.Size(445, 63);
            this.lblEditShowsBackColor.TabIndex = 3;
            // 
            // btnEditShowsSave
            // 
            this.btnEditShowsSave.Location = new System.Drawing.Point(238, 210);
            this.btnEditShowsSave.Name = "btnEditShowsSave";
            this.btnEditShowsSave.Size = new System.Drawing.Size(87, 25);
            this.btnEditShowsSave.TabIndex = 1;
            this.btnEditShowsSave.Text = "Save";
            this.btnEditShowsSave.UseSelectable = true;
            this.btnEditShowsSave.Click += new System.EventHandler(this.btnEditShowsSave_Click);
            // 
            // btnEditShowsCancel
            // 
            this.btnEditShowsCancel.Location = new System.Drawing.Point(337, 210);
            this.btnEditShowsCancel.Name = "btnEditShowsCancel";
            this.btnEditShowsCancel.Size = new System.Drawing.Size(87, 25);
            this.btnEditShowsCancel.TabIndex = 2;
            this.btnEditShowsCancel.Text = "Cancel";
            this.btnEditShowsCancel.UseSelectable = true;
            this.btnEditShowsCancel.Click += new System.EventHandler(this.btnEditShowsCancel_Click);
            // 
            // EditShows
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(445, 253);
            this.Controls.Add(this.picEditShowsIcon);
            this.Controls.Add(this.lblEditShowsQuestion);
            this.Controls.Add(this.lblEditShowslabel);
            this.Controls.Add(this.cboEditShows);
            this.Controls.Add(this.btnEditShowsSave);
            this.Controls.Add(this.btnEditShowsCancel);
            this.Controls.Add(this.lblEditShowsBackColor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditShows";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Title";
            this.Load += new System.EventHandler(this.EditShows_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditShows_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picEditShowsIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.PictureBox picEditShowsIcon;
        private MetroFramework.Controls.MetroLabel lblEditShowsQuestion;
        private MetroFramework.Controls.MetroLabel lblEditShowslabel;
        private MetroFramework.Controls.MetroComboBox cboEditShows;
        private System.Windows.Forms.Label lblEditShowsBackColor;
        private MetroFramework.Controls.MetroButton btnEditShowsSave;
        private MetroFramework.Controls.MetroButton btnEditShowsCancel;
    }
}