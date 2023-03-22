using System;
using System.Windows.Forms;

namespace FallGuysStats {
    public partial class SelectLanguage : MetroFramework.Forms.MetroForm {
        private readonly string sysLang;
        public int selectedLanguage;

        public SelectLanguage(string sysLang) {
            this.sysLang = sysLang;
            this.InitializeComponent();
        }

        private void SelectLanguage_Load(object sender, EventArgs e) {
            var defaultLang = sysLang.Substring(0, 2) == "fr" ? 1 : sysLang.Substring(0, 2) == "ko" ? 2 : sysLang.Substring(0, 2) == "ja" ? 3 : sysLang.Substring(0, 2) == "zh" ? 4 : 0;
            this.ChangeLanguage(defaultLang);
            this.cboLanguage.SelectedIndex = defaultLang;
        }

        private void CboLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedLanguage = ((ComboBox)sender).SelectedIndex;
            this.ChangeLanguage(this.selectedLanguage);
        }

        private void BtnLanguageSave_Click(object sender, EventArgs e) {
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