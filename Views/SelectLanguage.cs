using System;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class SelectLanguage : MetroFramework.Forms.MetroForm {
        private Language defaultLanguage;
        public Language selectedLanguage;
        public bool autoGenerateProfiles;

        public SelectLanguage(string sysLang) {
            this.defaultLanguage = string.Equals(sysLang, "fr", StringComparison.OrdinalIgnoreCase) ? Language.French :
                                   string.Equals(sysLang, "ko", StringComparison.OrdinalIgnoreCase) ? Language.Korean :
                                   string.Equals(sysLang, "ja", StringComparison.OrdinalIgnoreCase) ? Language.Japanese :
                                   string.Equals(sysLang, "zh-chs", StringComparison.OrdinalIgnoreCase) ? Language.SimplifiedChinese :
                                   string.Equals(sysLang, "zh-cht", StringComparison.OrdinalIgnoreCase) ? Language.TraditionalChinese : Language.English;
            this.InitializeComponent();
        }

        private void SelectLanguage_Load(object sender, EventArgs e) {
            this.cboLanguage.SelectedIndex = (int)this.defaultLanguage;
            this.ChangeLanguage(this.defaultLanguage);
        }
        
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedLanguage = (Language)((ComboBox)sender).SelectedIndex;
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

        private void ChangeLanguage(Language lang) {
            this.Font = Overlay.GetMainFont(9, FontStyle.Regular, lang);
            this.Text = Multilingual.GetWord("settings_select_language_title", lang);
            this.chkAutoGenerateProfile.Text = Multilingual.GetWord("settings_auto_generate_profiles", lang);
            this.btnLanguageSave.Text = Multilingual.GetWord("settings_select_language_button", lang);
            this.Refresh();
        }
    }
}