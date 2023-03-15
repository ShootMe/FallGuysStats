using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class SelectLanguage : Form {
        public int selectedLanguage = 0;
        public SelectLanguage() => this.InitializeComponent();

        private void SelectLanguage_Load(object sender, EventArgs e) {
            this.ChangeLanguage(0);
            this.cboLanguage.SelectedIndex = 0;
        }
        
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedLanguage = ((ComboBox)sender).SelectedIndex;
            this.ChangeLanguage(((ComboBox)sender).SelectedIndex);
        }
        
        private void btnLanguageSave_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectLanguage_FormClosing(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
        }

        private void ChangeLanguage(int lang) {
            this.Font = new Font(Overlay.GetMainFontFamilies(lang), 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Text = Multilingual.GetWordWithLang("settings_select_language_title", lang);
            this.btnLanguageSave.Text = Multilingual.GetWordWithLang("settings_select_language_button", lang);
        }
    }
}