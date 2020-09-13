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
            chkUseNDI.Checked = CurrentSettings.UseNDI;
            chkOverlayOnTop.Checked = !CurrentSettings.OverlayNotOnTop;
            chkHideRoundInfo.Checked = CurrentSettings.HideRoundInfo;
            chkHideTimeInfo.Checked = CurrentSettings.HideTimeInfo;
            chkShowTabs.Checked = CurrentSettings.ShowOverlayTabs;

            switch (CurrentSettings.WinsFilter) {
                case 0: cboWinsFilter.SelectedItem = "Stats and Party Filter"; break;
                case 1: cboWinsFilter.SelectedItem = "Stats Filter Only"; break;
                case 2: cboWinsFilter.SelectedItem = "Party Filter Only"; break;
                case 3: cboWinsFilter.SelectedItem = "No Filter"; break;
            }
            switch (CurrentSettings.QualifyFilter) {
                case 0: cboQualifyFilter.SelectedItem = "No Filter"; break;
                case 1: cboQualifyFilter.SelectedItem = "Stats and Party Filter"; break;
                case 2: cboQualifyFilter.SelectedItem = "Stats Filter Only"; break;
                case 3: cboQualifyFilter.SelectedItem = "Party Filter Only"; break;
            }
            switch (CurrentSettings.FastestFilter) {
                case 0: cboFastestFilter.SelectedItem = "No Filter"; break;
                case 1: cboFastestFilter.SelectedItem = "Stats and Party Filter"; break;
                case 2: cboFastestFilter.SelectedItem = "Stats Filter Only"; break;
                case 3: cboFastestFilter.SelectedItem = "Party Filter Only"; break;
            }
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
            CurrentSettings.UseNDI = chkUseNDI.Checked;
            CurrentSettings.OverlayNotOnTop = !chkOverlayOnTop.Checked;
            CurrentSettings.HideRoundInfo = chkHideRoundInfo.Checked;
            CurrentSettings.HideTimeInfo = chkHideTimeInfo.Checked;
            CurrentSettings.ShowOverlayTabs = chkShowTabs.Checked;

            switch ((string)cboWinsFilter.SelectedItem) {
                case "Stats and Party Filter": CurrentSettings.WinsFilter = 0; break;
                case "Stats Filter Only": CurrentSettings.WinsFilter = 1; break;
                case "Party Filter Only": CurrentSettings.WinsFilter = 2; break;
                case "No Filter": CurrentSettings.WinsFilter = 3; break;
            }
            switch ((string)cboQualifyFilter.SelectedItem) {
                case "No Filter": CurrentSettings.QualifyFilter = 0; break;
                case "Stats and Party Filter": CurrentSettings.QualifyFilter = 1; break;
                case "Stats Filter Only": CurrentSettings.QualifyFilter = 2; break;
                case "Party Filter Only": CurrentSettings.QualifyFilter = 3; break;
            }
            switch ((string)cboFastestFilter.SelectedItem) {
                case "No Filter": CurrentSettings.FastestFilter = 0; break;
                case "Stats and Party Filter": CurrentSettings.FastestFilter = 1; break;
                case "Stats Filter Only": CurrentSettings.FastestFilter = 2; break;
                case "Party Filter Only": CurrentSettings.FastestFilter = 3; break;
            }

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