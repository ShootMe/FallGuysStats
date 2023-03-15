using System;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    partial class SelectLanguage {
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
            this.picLanguageSelection = new System.Windows.Forms.PictureBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.btnLanguageSave = new System.Windows.Forms.Button();
            this.picLanguageSelection.SuspendLayout();
            this.cboLanguage.SuspendLayout();
            this.lblBackColor.SuspendLayout();
            this.btnLanguageSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // picLanguageSelection
            // 
            this.picLanguageSelection.Location = new System.Drawing.Point(22, 12);
            this.picLanguageSelection.Name = "picLanguageSelection";
            this.picLanguageSelection.Width = 23;
            this.picLanguageSelection.Height = 23;
            this.picLanguageSelection.Image = Properties.Resources.language_icon;
            this.picLanguageSelection.SizeMode = PictureBoxSizeMode.Zoom;
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.Items.AddRange(new object[] {
                "🇺🇸 English",
                "🇫🇷 Français",
                "🇰🇷 한국어",
                "🇯🇵 日本語",
                "🇨🇳 简体中文"});
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(62, 14);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(80, 26);
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += new EventHandler(this.cboLanguage_SelectedIndexChanged);
            // 
            // lblBackColor
            // 
            this.lblBackColor.Location = new System.Drawing.Point(0, 48);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(180, 32);
            this.lblBackColor.BackColor = Color.FromArgb(40,0,182,254);
            // 
            // btnLanguageSave
            // 
            this.btnLanguageSave.Location = new System.Drawing.Point(92, 55);
            this.btnLanguageSave.Name = "btnLanguageSave";
            this.btnLanguageSave.Size = new System.Drawing.Size(75, 18);
            this.btnLanguageSave.TabIndex = 2;
            this.btnLanguageSave.Text = "Confirm";
            this.btnLanguageSave.UseVisualStyleBackColor = true;
            this.btnLanguageSave.Click += new System.EventHandler(this.btnLanguageSave_Click);
            // 
            // SelectLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 80);
            this.Controls.Add(this.picLanguageSelection);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.lblBackColor);
            this.Controls.Add(this.btnLanguageSave);
            this.Name = "EditShows";
            this.Text = "Title";
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new EventHandler(this.SelectLanguage_Load);
            this.FormClosing += new FormClosingEventHandler(this.SelectLanguage_FormClosing);
            
            this.picLanguageSelection.ResumeLayout(false);
            this.cboLanguage.ResumeLayout(false);
            this.lblBackColor.ResumeLayout(false);
            this.btnLanguageSave.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.btnLanguageSave.BringToFront();
        }

        #endregion
        private System.Windows.Forms.PictureBox picLanguageSelection;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Label lblBackColor;
        private System.Windows.Forms.Button btnLanguageSave;
    }
}