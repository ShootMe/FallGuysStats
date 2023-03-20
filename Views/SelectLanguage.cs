using System;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class SelectLanguage : MetroFramework.Forms.MetroForm {
        public int selectedLanguage = 0;
        public SelectLanguage() => this.InitializeComponent();

        private void SelectLanguage_Load(object sender, EventArgs e) {
            this.ChangeLanguage(0);
            this.cboLanguage.SelectedIndex = 0;
        }
        
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedLanguage = ((ComboBox)sender).SelectedIndex;
            this.ChangeLanguage(this.selectedLanguage);
        }
        
        private void btnLanguageSave_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectLanguage_FormClosing(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
        }

        private void ChangeLanguage(int lang) {
            this.Font = Overlay.GetMainFont(9, lang);
            this.Text = Multilingual.GetWordWithLang("settings_select_language_title", lang);
            this.btnLanguageSave.Text = Multilingual.GetWordWithLang("settings_select_language_button", lang);
            this.Refresh();
        }
    }
}