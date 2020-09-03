using System;
using System.IO;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Settings : Form {
        public UserSettings CurrentSettings { get; set; }
        public Settings() {
            InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e) {
            txtLogPath.Text = CurrentSettings.LogPath;
            chkCycleOverlayLongest.Checked = CurrentSettings.SwitchBetweenLongest;
            txtCycleTimeSeconds.Text = CurrentSettings.CycleTimeSeconds.ToString();
            txtPreviousWins.Text = CurrentSettings.PreviousWins.ToString();
        }
        private void btnSave_Click(object sender, EventArgs e) {
            CurrentSettings.LogPath = txtLogPath.Text;

            if (string.IsNullOrEmpty(txtCycleTimeSeconds.Text)) {
                CurrentSettings.CycleTimeSeconds = 3;
            } else {
                CurrentSettings.CycleTimeSeconds = int.Parse(txtCycleTimeSeconds.Text);
                if (CurrentSettings.CycleTimeSeconds <= 0) {
                    CurrentSettings.CycleTimeSeconds = 3;
                }
            }

            if (string.IsNullOrEmpty(txtPreviousWins.Text)) {
                CurrentSettings.PreviousWins = 0;
            } else {
                CurrentSettings.PreviousWins = int.Parse(txtPreviousWins.Text);
                if (CurrentSettings.PreviousWins < 0) {
                    CurrentSettings.PreviousWins = 0;
                }
            }

            CurrentSettings.SwitchBetweenLongest = chkCycleOverlayLongest.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void txtCycleTimeSeconds_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(txtCycleTimeSeconds.Text) && !int.TryParse(txtCycleTimeSeconds.Text, out _)) {
                txtCycleTimeSeconds.Text = "3";
            }
        }
        private void txtLogPath_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            try {
                if (txtLogPath.Text.IndexOf(".log", StringComparison.OrdinalIgnoreCase) > 0) {
                    txtLogPath.Text = Path.GetDirectoryName(txtLogPath.Text);
                }
            } catch { }
        }

        private void txtPreviousWins_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(txtPreviousWins.Text) && !int.TryParse(txtPreviousWins.Text, out _)) {
                txtPreviousWins.Text = "0";
            }
        }
    }
}