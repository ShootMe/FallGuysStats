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
        private void InitializeComponent()
        {
            this.picLanguageSelection = new System.Windows.Forms.PictureBox();
            this.cboLanguage = new MetroFramework.Controls.MetroComboBox();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.btnLanguageSave = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.picLanguageSelection)).BeginInit();
            this.SuspendLayout();
            // 
            // picLanguageSelection
            // 
            this.picLanguageSelection.Image = global::FallGuysStats.Properties.Resources.language_icon;
            this.picLanguageSelection.Location = new System.Drawing.Point(90, 75);
            this.picLanguageSelection.Name = "picLanguageSelection";
            this.picLanguageSelection.Size = new System.Drawing.Size(32, 32);
            this.picLanguageSelection.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLanguageSelection.TabIndex = 0;
            this.picLanguageSelection.TabStop = false;
            // 
            // cboLanguage
            // 
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.ItemHeight = 23;
            this.cboLanguage.Items.AddRange(new object[] {
            "🇺🇸 English",
            "🇫🇷 Français",
            "🇰🇷 한국어",
            "🇯🇵 日本語",
            "🇨🇳 简体中文"});
            this.cboLanguage.Location = new System.Drawing.Point(135, 76);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(110, 29);
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.UseSelectable = true;
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.CboLanguage_SelectedIndexChanged);
            // 
            // lblBackColor
            // 
            this.lblBackColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(0)))), ((int)(((byte)(182)))), ((int)(((byte)(254)))));
            this.lblBackColor.Location = new System.Drawing.Point(0, 130);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(350, 50);
            this.lblBackColor.TabIndex = 1;
            // 
            // btnLanguageSave
            // 
            this.btnLanguageSave.Location = new System.Drawing.Point(250, 143);
            this.btnLanguageSave.Name = "btnLanguageSave";
            this.btnLanguageSave.Size = new System.Drawing.Size(75, 25);
            this.btnLanguageSave.TabIndex = 2;
            this.btnLanguageSave.Text = "Confirm";
            this.btnLanguageSave.UseSelectable = true;
            this.btnLanguageSave.Click += new System.EventHandler(this.BtnLanguageSave_Click);
            // 
            // SelectLanguage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(350, 180);
            this.Controls.Add(this.picLanguageSelection);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.btnLanguageSave);
            this.Controls.Add(this.lblBackColor);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectLanguage";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Title";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectLanguage_FormClosing);
            this.Load += new System.EventHandler(this.SelectLanguage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLanguageSelection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox picLanguageSelection;
        private MetroFramework.Controls.MetroComboBox cboLanguage;
        private System.Windows.Forms.Label lblBackColor;
        private MetroFramework.Controls.MetroButton btnLanguageSave;
    }
}