using System;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class SelectLanguage : MetroFramework.Forms.MetroForm {
        private int defaultLanguage;
        public int selectedLanguage;
        public bool autoGenerateProfiles;

        public SelectLanguage(string sysLang) {
            this.defaultLanguage = string.Equals(sysLang, "fr", StringComparison.Ordinal) ? 1 :
                                   string.Equals(sysLang, "ko", StringComparison.Ordinal) ? 2 :
                                   string.Equals(sysLang, "ja", StringComparison.Ordinal) ? 3 :
                                   string.Equals(sysLang, "zh-chs", StringComparison.Ordinal) ? 4 :
                                   string.Equals(sysLang, "zh-cht", StringComparison.Ordinal) ? 5 : 0;
            this.InitializeComponent();
        }

        private void SelectLanguage_Load(object sender, EventArgs e) {
            this.cboLanguage.SelectedIndex = this.defaultLanguage;
            this.ChangeLanguage(this.defaultLanguage);
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
            this.Font = Overlay.GetMainFont(9, FontStyle.Regular, lang);
            this.Text = Multilingual.GetWordWithLang("settings_select_language_title", lang);
            this.chkAutoGenerateProfile.Text = Multilingual.GetWordWithLang("settings_auto_generate_profiles", lang);
            this.btnLanguageSave.Text = Multilingual.GetWordWithLang("settings_select_language_button", lang);
            this.Refresh();
        }
    }
}