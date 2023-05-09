using System;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class SelectLanguage : MetroFramework.Forms.MetroForm {
        private int defaultLanguage;
        public int selectedLanguage;
        public bool autoGenerateProfiles;

        public SelectLanguage(string sysLang) {
            this.defaultLanguage = sysLang == "fr" ? 1 :
                                    sysLang == "ko" ? 2 :
                                    sysLang == "ja" ? 3 :
                                    sysLang == "zh" ? 4 : 0;
            this.InitializeComponent();
        }

        private void SelectLanguage_Load(object sender, EventArgs e) {
            this.ChangeLanguage(this.defaultLanguage);
            this.cboLanguage.SelectedIndex = this.defaultLanguage;
        }
        
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedLanguage = ((ComboBox)sender).SelectedIndex;
            this.ChangeLanguage(this.selectedLanguage);
        }
        
        private void chkAutoGenerateProfile_CheckedChanged(object sender, EventArgs e) {
            this.autoGenerateProfiles = ((CheckBox)sender).Checked;
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