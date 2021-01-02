using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Settings : Form {
        private PrivateFontCollection CustomFonts;
        private string overlayFontSerialized = string.Empty;

        public UserSettings CurrentSettings { get; set; }
        public Settings() {
            InitializeComponent();

            CustomFonts = new PrivateFontCollection();
            CustomFonts.AddFontFile("TitanOne-Regular.ttf");
        }
        private void Settings_Load(object sender, EventArgs e) {
            txtLogPath.Text = CurrentSettings.LogPath;

            if (CurrentSettings.SwitchBetweenLongest) {
                chkCycleFastestLongest.Checked = true;
            } else if (CurrentSettings.OnlyShowLongest) {
                chkOnlyShowLongest.Checked = true;
            } else {
                chkOnlyShowFastest.Checked = true;
            }
            if (CurrentSettings.SwitchBetweenQualify) {
                chkCycleQualifyGold.Checked = true;
            } else if (CurrentSettings.OnlyShowGold) {
                chkOnlyShowGold.Checked = true;
            } else {
                chkOnlyShowQualify.Checked = true;
            }
            if (CurrentSettings.SwitchBetweenPlayers) {
                chkCyclePlayersPing.Checked = true;
            } else if (CurrentSettings.OnlyShowPing) {
                chkOnlyShowPing.Checked = true;
            } else {
                chkOnlyShowPlayers.Checked = true;
            }
            if (CurrentSettings.SwitchBetweenStreaks) {
                chkCycleWinFinalStreak.Checked = true;
            } else if (CurrentSettings.OnlyShowFinalStreak) {
                chkOnlyShowFinalStreak.Checked = true;
            } else {
                chkOnlyShowWinStreak.Checked = true;
            }

            txtCycleTimeSeconds.Text = CurrentSettings.CycleTimeSeconds.ToString();
            txtPreviousWins.Text = CurrentSettings.PreviousWins.ToString();
            chkUseNDI.Checked = CurrentSettings.UseNDI;
            chkOverlayOnTop.Checked = !CurrentSettings.OverlayNotOnTop;
            chkHideWinsInfo.Checked = CurrentSettings.HideWinsInfo;
            chkHideRoundInfo.Checked = CurrentSettings.HideRoundInfo;
            chkHideTimeInfo.Checked = CurrentSettings.HideTimeInfo;
            chkShowTabs.Checked = CurrentSettings.ShowOverlayTabs;
            chkAutoUpdate.Checked = CurrentSettings.AutoUpdate;
            chkFlipped.Checked = CurrentSettings.FlippedDisplay;
            chkHidePercentages.Checked = CurrentSettings.HideOverlayPercentages;
            chkChangeHoopsieLegends.Checked = CurrentSettings.HoopsieHeros;

            switch (CurrentSettings.OverlayColor) {
                case 0: cboOverlayColor.SelectedItem = "Magenta"; break;
                case 1: cboOverlayColor.SelectedItem = "Blue"; break;
                case 2: cboOverlayColor.SelectedItem = "Red"; break;
                case 3: cboOverlayColor.SelectedItem = "Transparent"; break;
                case 4: cboOverlayColor.SelectedItem = "Black"; break;
                case 5: cboOverlayColor.SelectedItem = "Green"; break;
            }
            switch (CurrentSettings.WinsFilter) {
                case 0: cboWinsFilter.SelectedItem = "Stats and Party Filter"; break;
                case 1: cboWinsFilter.SelectedItem = "Season Stats"; break;
                case 2: cboWinsFilter.SelectedItem = "Week Stats"; break;
                case 3: cboWinsFilter.SelectedItem = "All Time Stats"; break;
                case 4: cboWinsFilter.SelectedItem = "Day Stats"; break;
                case 5: cboWinsFilter.SelectedItem = "Session Stats"; break;
            }
            switch (CurrentSettings.QualifyFilter) {
                case 0: cboQualifyFilter.SelectedItem = "All Time Stats"; break;
                case 1: cboQualifyFilter.SelectedItem = "Stats and Party Filter"; break;
                case 2: cboQualifyFilter.SelectedItem = "Season Stats"; break;
                case 3: cboQualifyFilter.SelectedItem = "Week Stats"; break;
                case 4: cboQualifyFilter.SelectedItem = "Day Stats"; break;
                case 5: cboQualifyFilter.SelectedItem = "Session Stats"; break;
            }
            switch (CurrentSettings.FastestFilter) {
                case 0: cboFastestFilter.SelectedItem = "All Time Stats"; break;
                case 1: cboFastestFilter.SelectedItem = "Stats and Party Filter"; break;
                case 2: cboFastestFilter.SelectedItem = "Season Stats"; break;
                case 3: cboFastestFilter.SelectedItem = "Week Stats"; break;
                case 4: cboFastestFilter.SelectedItem = "Day Stats"; break;
                case 5: cboFastestFilter.SelectedItem = "Session Stats"; break;
            }

            txtGameExeLocation.Text = CurrentSettings.GameExeLocation;
            chkAutoLaunchGameOnStart.Checked = CurrentSettings.AutoLaunchGameOnStartup;
            chkIgnoreLevelTypeWhenSorting.Checked = CurrentSettings.IgnoreLevelTypeWhenSorting;
            
            if((CurrentSettings.OverlayScale < (trkOverlayScale.Minimum / 10.0)) || (CurrentSettings.OverlayScale > (trkOverlayScale.Maximum / 10.0))) {
                CurrentSettings.OverlayScale = 1.0;
            }

            this.trkOverlayScale.Value = (int)(CurrentSettings.OverlayScale * 10);
            this.lblOverlayScale.Text = (CurrentSettings.OverlayScale * 100) + "%";

            if (!string.IsNullOrEmpty(CurrentSettings.OverlayFontSerialized)) {
                FontConverter fontConverter = new FontConverter();
                Font exampleFont = fontConverter.ConvertFromString(CurrentSettings.OverlayFontSerialized) as Font;
                lblOverlayFontExample.Font = exampleFont;
            } else if (CustomFonts != null) {
                lblOverlayFontExample.Font = new Font(CustomFonts.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            CurrentSettings.LogPath = txtLogPath.Text;

            if (string.IsNullOrEmpty(txtCycleTimeSeconds.Text)) {
                CurrentSettings.CycleTimeSeconds = 5;
            } else {
                CurrentSettings.CycleTimeSeconds = int.Parse(txtCycleTimeSeconds.Text);
                if (CurrentSettings.CycleTimeSeconds <= 0) {
                    CurrentSettings.CycleTimeSeconds = 5;
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

            if (chkCycleFastestLongest.Checked) {
                CurrentSettings.SwitchBetweenLongest = true;
                CurrentSettings.OnlyShowLongest = false;
            } else if (chkOnlyShowLongest.Checked) {
                CurrentSettings.SwitchBetweenLongest = false;
                CurrentSettings.OnlyShowLongest = true;
            } else {
                CurrentSettings.SwitchBetweenLongest = false;
                CurrentSettings.OnlyShowLongest = false;
            }
            if (chkCycleQualifyGold.Checked) {
                CurrentSettings.SwitchBetweenQualify = true;
                CurrentSettings.OnlyShowGold = false;
            } else if (chkOnlyShowGold.Checked) {
                CurrentSettings.SwitchBetweenQualify = false;
                CurrentSettings.OnlyShowGold = true;
            } else {
                CurrentSettings.SwitchBetweenQualify = false;
                CurrentSettings.OnlyShowGold = false;
            }
            if (chkCyclePlayersPing.Checked) {
                CurrentSettings.SwitchBetweenPlayers = true;
                CurrentSettings.OnlyShowPing = false;
            } else if (chkOnlyShowPing.Checked) {
                CurrentSettings.SwitchBetweenPlayers = false;
                CurrentSettings.OnlyShowPing = true;
            } else {
                CurrentSettings.SwitchBetweenPlayers = false;
                CurrentSettings.OnlyShowPing = false;
            }
            if (chkCycleWinFinalStreak.Checked) {
                CurrentSettings.SwitchBetweenStreaks = true;
                CurrentSettings.OnlyShowFinalStreak = false;
            } else if (chkOnlyShowFinalStreak.Checked) {
                CurrentSettings.SwitchBetweenStreaks = false;
                CurrentSettings.OnlyShowFinalStreak = true;
            } else {
                CurrentSettings.SwitchBetweenStreaks = false;
                CurrentSettings.OnlyShowFinalStreak = false;
            }

            CurrentSettings.UseNDI = chkUseNDI.Checked;
            CurrentSettings.OverlayNotOnTop = !chkOverlayOnTop.Checked;
            if (chkHideRoundInfo.Checked && chkHideTimeInfo.Checked && chkHideWinsInfo.Checked) {
                chkHideWinsInfo.Checked = false;
            }
            bool resizeOverlay = CurrentSettings.HideWinsInfo != chkHideWinsInfo.Checked ||
                CurrentSettings.HideRoundInfo != chkHideRoundInfo.Checked ||
                CurrentSettings.HideTimeInfo != chkHideTimeInfo.Checked ||
                CurrentSettings.ShowOverlayTabs != chkShowTabs.Checked;

            CurrentSettings.HideWinsInfo = chkHideWinsInfo.Checked;
            CurrentSettings.HideRoundInfo = chkHideRoundInfo.Checked;
            CurrentSettings.HideTimeInfo = chkHideTimeInfo.Checked;
            CurrentSettings.ShowOverlayTabs = chkShowTabs.Checked;
            CurrentSettings.AutoUpdate = chkAutoUpdate.Checked;
            CurrentSettings.FlippedDisplay = chkFlipped.Checked;
            CurrentSettings.HideOverlayPercentages = chkHidePercentages.Checked;
            CurrentSettings.HoopsieHeros = chkChangeHoopsieLegends.Checked;

            switch ((string)cboOverlayColor.SelectedItem) {
                case "Magenta": CurrentSettings.OverlayColor = 0; break;
                case "Blue": CurrentSettings.OverlayColor = 1; break;
                case "Red": CurrentSettings.OverlayColor = 2; break;
                case "Transparent": CurrentSettings.OverlayColor = 3; break;
                case "Black": CurrentSettings.OverlayColor = 4; break;
                case "Green": CurrentSettings.OverlayColor = 5; break;
            }
            switch ((string)cboWinsFilter.SelectedItem) {
                case "Stats and Party Filter": CurrentSettings.WinsFilter = 0; break;
                case "Season Stats": CurrentSettings.WinsFilter = 1; break;
                case "Week Stats": CurrentSettings.WinsFilter = 2; break;
                case "All Time Stats": CurrentSettings.WinsFilter = 3; break;
                case "Day Stats": CurrentSettings.WinsFilter = 4; break;
                case "Session Stats": CurrentSettings.WinsFilter = 5; break;
            }
            switch ((string)cboQualifyFilter.SelectedItem) {
                case "All Time Stats": CurrentSettings.QualifyFilter = 0; break;
                case "Stats and Party Filter": CurrentSettings.QualifyFilter = 1; break;
                case "Season Stats": CurrentSettings.QualifyFilter = 2; break;
                case "Week Stats": CurrentSettings.QualifyFilter = 3; break;
                case "Day Stats": CurrentSettings.QualifyFilter = 4; break;
                case "Session Stats": CurrentSettings.QualifyFilter = 5; break;
            }
            switch ((string)cboFastestFilter.SelectedItem) {
                case "All Time Stats": CurrentSettings.FastestFilter = 0; break;
                case "Stats and Party Filter": CurrentSettings.FastestFilter = 1; break;
                case "Season Stats": CurrentSettings.FastestFilter = 2; break;
                case "Week Stats": CurrentSettings.FastestFilter = 3; break;
                case "Day Stats": CurrentSettings.FastestFilter = 4; break;
                case "Session Stats": CurrentSettings.FastestFilter = 5; break;
            }

            if (resizeOverlay) {
                int overlaySetting = (CurrentSettings.HideWinsInfo ? 4 : 0) + (CurrentSettings.HideRoundInfo ? 2 : 0) + (CurrentSettings.HideTimeInfo ? 1 : 0);
                switch (overlaySetting) {
                    case 0: CurrentSettings.OverlayWidth = 786; break;
                    case 1: CurrentSettings.OverlayWidth = 786 - 225 - 6; break;
                    case 2: CurrentSettings.OverlayWidth = 786 - 281 - 6; break;
                    case 3: CurrentSettings.OverlayWidth = 786 - 281 - 225 - 12; break;
                    case 4: CurrentSettings.OverlayWidth = 786 - 242 - 6; break;
                    case 5: CurrentSettings.OverlayWidth = 786 - 242 - 225 - 12; break;
                    case 6: CurrentSettings.OverlayWidth = 786 - 242 - 281 - 12; break;
                }

                if (CurrentSettings.ShowOverlayTabs) {
                    CurrentSettings.OverlayHeight = 134;
                } else {
                    CurrentSettings.OverlayHeight = 99;
                }
            }

            CurrentSettings.IgnoreLevelTypeWhenSorting = chkIgnoreLevelTypeWhenSorting.Checked;
            CurrentSettings.GameExeLocation = txtGameExeLocation.Text;
            CurrentSettings.AutoLaunchGameOnStartup = chkAutoLaunchGameOnStart.Checked;

            CurrentSettings.OverlayScale = trkOverlayScale.Value / 10.0;

            if (!string.IsNullOrEmpty(CurrentSettings.OverlayFontSerialized)) {
                FontConverter fontConverter = new FontConverter();
                CurrentSettings.OverlayFontSerialized = fontConverter.ConvertToString(lblOverlayFontExample.Font);
            } else {
                CurrentSettings.OverlayFontSerialized = string.Empty;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        private void txtCycleTimeSeconds_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(txtCycleTimeSeconds.Text) && !int.TryParse(txtCycleTimeSeconds.Text, out _)) {
                txtCycleTimeSeconds.Text = "5";
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
        private void btnGameExeLocationBrowse_Click(object sender, EventArgs e) {
            try {
                using (OpenFileDialog openFile = new OpenFileDialog()) {
                    FileInfo currentExeLocation = new FileInfo(txtGameExeLocation.Text);
                    if (currentExeLocation.Directory.Exists) {
                        openFile.InitialDirectory = currentExeLocation.Directory.FullName;
                    }

                    openFile.Filter = "Exe files (*.exe)|*.exe";
                    openFile.FileName = "FallGuys_client.exe";
                    openFile.Title = "Locate Fall Guys";

                    DialogResult result = openFile.ShowDialog(this);
                    if (result.Equals(DialogResult.OK)) {
                        if (openFile.FileName.IndexOf("FallGuys_client.exe", StringComparison.OrdinalIgnoreCase) >= 0) {
                            txtGameExeLocation.Text = openFile.FileName;
                        } else {
                            MessageBox.Show("Please select \"FallGuys_client.exe\" in the install folder.", "Wrong File Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOverlayScaleReset_Click(object sender, EventArgs e) {
            trkOverlayScale.Value = 10;
        }

        private void trkOverlayScale_ValueChanged(object sender, EventArgs e) {
            this.lblOverlayScale.Text = (trkOverlayScale.Value * 10) + "%";
        }
        private void btnSelectFont_Click(object sender, EventArgs e) {
            dlgOverlayFont.Font = lblOverlayFont.Font;
            DialogResult result = dlgOverlayFont.ShowDialog(this);

            if (result.Equals(DialogResult.OK)) {
                lblOverlayFontExample.Font = dlgOverlayFont.Font;
                FontConverter fontConverter = new FontConverter();
                overlayFontSerialized = fontConverter.ConvertToString(dlgOverlayFont.Font);
            }
        }
        private void btnResetOverlayFont_Click(object sender, EventArgs e) {
            Font defaultFont = new Font(CustomFonts.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            lblOverlayFontExample.Font = defaultFont;
            overlayFontSerialized = string.Empty;
        }
    }
}