using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Settings : Form {
        private string overlayFontSerialized = string.Empty;

        public UserSettings CurrentSettings { get; set; }
        public Settings() {
            this.InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e) {
            this.ChangeLanguage(Stats.CurrentLanguage);
            switch (Stats.CurrentLanguage) {
                case 0: this.cboMultilingual.SelectedItem = "English"; break;
                case 1: this.cboMultilingual.SelectedItem = "한국어"; break;
                case 2: this.cboMultilingual.SelectedItem = "日本語"; break;
                case 3: this.cboMultilingual.SelectedItem = "简体中文"; break;
            }
            this.txtLogPath.Text = CurrentSettings.LogPath;

            if (CurrentSettings.SwitchBetweenLongest) {
                this.chkCycleFastestLongest.Checked = true;
            } else if (CurrentSettings.OnlyShowLongest) {
                this.chkOnlyShowLongest.Checked = true;
            } else {
                this.chkOnlyShowFastest.Checked = true;
            }
            if (this.CurrentSettings.SwitchBetweenQualify) {
                this.chkCycleQualifyGold.Checked = true;
            } else if (CurrentSettings.OnlyShowGold) {
                this.chkOnlyShowGold.Checked = true;
            } else {
                this.chkOnlyShowQualify.Checked = true;
            }
            if (this.CurrentSettings.SwitchBetweenPlayers) {
                this.chkCyclePlayersPing.Checked = true;
            } else if (CurrentSettings.OnlyShowPing) {
                this.chkOnlyShowPing.Checked = true;
            } else {
                this.chkOnlyShowPlayers.Checked = true;
            }
            if (this.CurrentSettings.SwitchBetweenStreaks) {
                this.chkCycleWinFinalStreak.Checked = true;
            } else if (this.CurrentSettings.OnlyShowFinalStreak) {
                this.chkOnlyShowFinalStreak.Checked = true;
            } else {
                this.chkOnlyShowWinStreak.Checked = true;
            }

            this.txtCycleTimeSeconds.Text = this.CurrentSettings.CycleTimeSeconds.ToString();
            this.txtPreviousWins.Text = this.CurrentSettings.PreviousWins.ToString();
            this.chkOverlayOnTop.Checked = !this.CurrentSettings.OverlayNotOnTop;
            this.chkPlayerByConsoleType.Checked = this.CurrentSettings.PlayerByConsoleType;
            this.chkColorByRoundType.Checked = this.CurrentSettings.ColorByRoundType;
            this.chkHideWinsInfo.Checked = this.CurrentSettings.HideWinsInfo;
            this.chkHideRoundInfo.Checked = this.CurrentSettings.HideRoundInfo;
            this.chkHideTimeInfo.Checked = this.CurrentSettings.HideTimeInfo;
            this.chkShowTabs.Checked = this.CurrentSettings.ShowOverlayTabs;
            //this.chkShowProfile.Checked = this.CurrentSettings.ShowOverlayProfile;
            this.chkAutoUpdate.Checked = this.CurrentSettings.AutoUpdate;
            this.chkFlipped.Checked = this.CurrentSettings.FlippedDisplay;
            this.chkHidePercentages.Checked = this.CurrentSettings.HideOverlayPercentages;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;

            switch (this.CurrentSettings.OverlayColor) {
                case 0: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 1: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
                case 2: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 3: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_transparent"); break;
                case 4: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 5: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
            }
            switch (this.CurrentSettings.WinsFilter) {
                case 0: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 1: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 2: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 3: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 4: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }
            switch (this.CurrentSettings.QualifyFilter) {
                case 0: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }
            switch (this.CurrentSettings.FastestFilter) {
                case 0: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }

            this.txtGameExeLocation.Text = this.CurrentSettings.GameExeLocation;
            this.txtGameFileLocation.Text = this.CurrentSettings.GameFileLocation;
            this.chkAutoLaunchGameOnStart.Checked = this.CurrentSettings.AutoLaunchGameOnStartup;
            this.chkIgnoreLevelTypeWhenSorting.Checked = this.CurrentSettings.IgnoreLevelTypeWhenSorting;

            if (!string.IsNullOrEmpty(this.CurrentSettings.OverlayFontSerialized)) {
                this.overlayFontSerialized = this.CurrentSettings.OverlayFontSerialized;
                FontConverter fontConverter = new FontConverter();
                Font exampleFont = fontConverter.ConvertFromString(this.CurrentSettings.OverlayFontSerialized) as Font;
                this.lblOverlayFontExample.Font = exampleFont;
            } else {
                this.lblOverlayFontExample.Font = Overlay.DefaultFont;
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            switch ((string)this.cboMultilingual.SelectedItem) {
                case "English":
                    Stats.CurrentLanguage = 0;
                    this.CurrentSettings.Multilingual = 0;
                    break;
                case "한국어":
                    Stats.CurrentLanguage = 1;
                    this.CurrentSettings.Multilingual = 1;
                    break;
                case "日本語":
                    Stats.CurrentLanguage = 2;
                    this.CurrentSettings.Multilingual = 2;
                    break;
                case "简体中文":
                    Stats.CurrentLanguage = 3;
                    this.CurrentSettings.Multilingual = 3;
                    break;
                default:
                    Stats.CurrentLanguage = 0;
                    this.CurrentSettings.Multilingual = 0;
                    break;
            }

            this.CurrentSettings.LogPath = this.txtLogPath.Text;

            if (string.IsNullOrEmpty(this.txtCycleTimeSeconds.Text)) {
                this.CurrentSettings.CycleTimeSeconds = 5;
            } else {
                this.CurrentSettings.CycleTimeSeconds = int.Parse(this.txtCycleTimeSeconds.Text);
                if (this.CurrentSettings.CycleTimeSeconds <= 0) {
                    this.CurrentSettings.CycleTimeSeconds = 5;
                }
            }

            if (string.IsNullOrEmpty(this.txtPreviousWins.Text)) {
                this.CurrentSettings.PreviousWins = 0;
            } else {
                this.CurrentSettings.PreviousWins = int.Parse(this.txtPreviousWins.Text);
                if (this.CurrentSettings.PreviousWins < 0) {
                    this.CurrentSettings.PreviousWins = 0;
                }
            }

            if (this.chkCycleFastestLongest.Checked) {
                this.CurrentSettings.SwitchBetweenLongest = true;
                this.CurrentSettings.OnlyShowLongest = false;
            } else if (this.chkOnlyShowLongest.Checked) {
                this.CurrentSettings.SwitchBetweenLongest = false;
                this.CurrentSettings.OnlyShowLongest = true;
            } else {
                this.CurrentSettings.SwitchBetweenLongest = false;
                this.CurrentSettings.OnlyShowLongest = false;
            }
            if (this.chkCycleQualifyGold.Checked) {
                this.CurrentSettings.SwitchBetweenQualify = true;
                this.CurrentSettings.OnlyShowGold = false;
            } else if (this.chkOnlyShowGold.Checked) {
                this.CurrentSettings.SwitchBetweenQualify = false;
                this.CurrentSettings.OnlyShowGold = true;
            } else {
                this.CurrentSettings.SwitchBetweenQualify = false;
                this.CurrentSettings.OnlyShowGold = false;
            }
            if (this.chkCyclePlayersPing.Checked) {
                this.CurrentSettings.SwitchBetweenPlayers = true;
                this.CurrentSettings.OnlyShowPing = false;
            } else if (this.chkOnlyShowPing.Checked) {
                this.CurrentSettings.SwitchBetweenPlayers = false;
                this.CurrentSettings.OnlyShowPing = true;
            } else {
                this.CurrentSettings.SwitchBetweenPlayers = false;
                this.CurrentSettings.OnlyShowPing = false;
            }
            if (this.chkCycleWinFinalStreak.Checked) {
                this.CurrentSettings.SwitchBetweenStreaks = true;
                this.CurrentSettings.OnlyShowFinalStreak = false;
            } else if (this.chkOnlyShowFinalStreak.Checked) {
                this.CurrentSettings.SwitchBetweenStreaks = false;
                this.CurrentSettings.OnlyShowFinalStreak = true;
            } else {
                this.CurrentSettings.SwitchBetweenStreaks = false;
                this.CurrentSettings.OnlyShowFinalStreak = false;
            }

            this.CurrentSettings.OverlayNotOnTop = !this.chkOverlayOnTop.Checked;
            this.CurrentSettings.PlayerByConsoleType = this.chkPlayerByConsoleType.Checked;
            this.CurrentSettings.ColorByRoundType = this.chkColorByRoundType.Checked;
            if (this.chkHideRoundInfo.Checked && this.chkHideTimeInfo.Checked && this.chkHideWinsInfo.Checked) {
                this.chkHideWinsInfo.Checked = false;
            }

            bool resizeOverlay = this.CurrentSettings.HideWinsInfo != this.chkHideWinsInfo.Checked ||
                                 this.CurrentSettings.HideRoundInfo != this.chkHideRoundInfo.Checked ||
                                 this.CurrentSettings.HideTimeInfo != this.chkHideTimeInfo.Checked ||
                                 this.CurrentSettings.ShowOverlayTabs != this.chkShowTabs.Checked;

            this.CurrentSettings.HideWinsInfo = this.chkHideWinsInfo.Checked;
            this.CurrentSettings.HideRoundInfo = this.chkHideRoundInfo.Checked;
            this.CurrentSettings.HideTimeInfo = this.chkHideTimeInfo.Checked;
            this.CurrentSettings.ShowOverlayTabs = this.chkShowTabs.Checked;
            this.CurrentSettings.AutoUpdate = this.chkAutoUpdate.Checked;
            this.CurrentSettings.FlippedDisplay = this.chkFlipped.Checked;
            this.CurrentSettings.HideOverlayPercentages = this.chkHidePercentages.Checked;
            this.CurrentSettings.HoopsieHeros = this.chkChangeHoopsieLegends.Checked;

            if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_magenta")}") {
                this.CurrentSettings.OverlayColor = 0;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_blue")}") {
                this.CurrentSettings.OverlayColor = 1;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_red")}") {
                this.CurrentSettings.OverlayColor = 2;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_transparent")}") {
                this.CurrentSettings.OverlayColor = 3;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_black")}") {
                this.CurrentSettings.OverlayColor = 4;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_green")}") {
                this.CurrentSettings.OverlayColor = 5;
            }
            
            if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_stats_and_party_filter")}") {
                this.CurrentSettings.WinsFilter = 0;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_season_stats")}") {
                this.CurrentSettings.WinsFilter = 1;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_week_stats")}") {
                this.CurrentSettings.WinsFilter = 2;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_all_time_stats")}") {
                this.CurrentSettings.WinsFilter = 3;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_day_stats")}") {
                this.CurrentSettings.WinsFilter = 4;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_session_stats")}") {
                this.CurrentSettings.WinsFilter = 5;
            }
            
            if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_all_time_stats")}") {
                this.CurrentSettings.QualifyFilter = 0;
            } else if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_stats_and_party_filter")}") {
                this.CurrentSettings.QualifyFilter = 1;
            } else if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_season_stats")}") {
                this.CurrentSettings.QualifyFilter = 2;
            } else if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_week_stats")}") {
                this.CurrentSettings.QualifyFilter = 3;
            } else if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_day_stats")}") {
                this.CurrentSettings.QualifyFilter = 4;
            } else if ((string)this.cboQualifyFilter.SelectedItem == $"{Multilingual.GetWord("settings_session_stats")}") {
                this.CurrentSettings.QualifyFilter = 5;
            }
            
            if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_all_time_stats")}") {
                this.CurrentSettings.FastestFilter = 0;
            } else if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_stats_and_party_filter")}") {
                this.CurrentSettings.FastestFilter = 1;
            } else if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_season_stats")}") {
                this.CurrentSettings.FastestFilter = 2;
            } else if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_week_stats")}") {
                this.CurrentSettings.FastestFilter = 3;
            } else if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_day_stats")}") {
                this.CurrentSettings.FastestFilter = 4;
            } else if ((string)this.cboFastestFilter.SelectedItem == $"{Multilingual.GetWord("settings_session_stats")}") {
                this.CurrentSettings.FastestFilter = 5;
            }

            if (resizeOverlay) {
                int overlaySetting = (this.CurrentSettings.HideWinsInfo ? 4 : 0) + (this.CurrentSettings.HideRoundInfo ? 2 : 0) + (this.CurrentSettings.HideTimeInfo ? 1 : 0);
                switch (overlaySetting) {
                    case 0: this.CurrentSettings.OverlayWidth = 786; break;
                    case 1: this.CurrentSettings.OverlayWidth = 786 - 225 - 6; break;
                    case 2: this.CurrentSettings.OverlayWidth = 786 - 281 - 6; break;
                    case 3: this.CurrentSettings.OverlayWidth = 786 - 281 - 225 - 12; break;
                    case 4: this.CurrentSettings.OverlayWidth = 786 - 242 - 6; break;
                    case 5: this.CurrentSettings.OverlayWidth = 786 - 242 - 225 - 12; break;
                    case 6: this.CurrentSettings.OverlayWidth = 786 - 242 - 281 - 12; break;
                }

                if (this.CurrentSettings.ShowOverlayTabs) {
                    this.CurrentSettings.OverlayHeight = 134;
                } else {
                    this.CurrentSettings.OverlayHeight = 99;
                }
            }

            this.CurrentSettings.IgnoreLevelTypeWhenSorting = this.chkIgnoreLevelTypeWhenSorting.Checked;
            this.CurrentSettings.GameExeLocation = this.txtGameExeLocation.Text;
            this.CurrentSettings.GameFileLocation = this.txtGameFileLocation.Text;
            this.CurrentSettings.AutoLaunchGameOnStartup = this.chkAutoLaunchGameOnStart.Checked;

            if (!string.IsNullOrEmpty(this.overlayFontSerialized)) {
                FontConverter fontConverter = new FontConverter();
                this.CurrentSettings.OverlayFontSerialized = fontConverter.ConvertToString(this.lblOverlayFontExample.Font);
            } else {
                this.CurrentSettings.OverlayFontSerialized = string.Empty;
                Overlay.DefaultFont = Stats.CurrentLanguage == 0 
                    ? new Font(Overlay.DefaultFontCollection.Families[1], 18, FontStyle.Regular, GraphicsUnit.Pixel)
                    : new Font(Overlay.DefaultFontCollection.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void txtCycleTimeSeconds_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(this.txtCycleTimeSeconds.Text) && !int.TryParse(this.txtCycleTimeSeconds.Text, out _)) {
                this.txtCycleTimeSeconds.Text = "5";
            }
        }
        private void txtLogPath_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            try {
                if (this.txtLogPath.Text.IndexOf(".log", StringComparison.OrdinalIgnoreCase) > 0) {
                    this.txtLogPath.Text = Path.GetDirectoryName(this.txtLogPath.Text);
                }
            } catch { }
        }
        private void txtPreviousWins_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(this.txtPreviousWins.Text) && !int.TryParse(this.txtPreviousWins.Text, out _)) {
                this.txtPreviousWins.Text = "0";
            }
        }
        private void btnGameExeLocationBrowse_Click(object sender, EventArgs e) {
            try {
                using (OpenFileDialog openFile = new OpenFileDialog()) {
                    /*FileInfo currentExeLocation = new FileInfo(txtGameExeLocation.Text);
                    if (currentExeLocation.Directory.Exists) {
                        openFile.InitialDirectory = currentExeLocation.Directory.FullName;
                    }*/
                    openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    openFile.Filter = "URL files (*.url)|*.url";
                    openFile.FileName = "Fall Guys Clinet 바로가기";
                    openFile.Title = "Fall Guys Client 바로가기 찾기";

                    if (openFile.ShowDialog(this).Equals(DialogResult.OK)) {
                        string fileContent = string.Empty;
                        string epicGamesFallGuysApp = "50118b7f954e450f8823df1614b24e80%3A38ec4849ea4f4de6aa7b6fb0f2d278e1%3A0a2d9f6403244d12969e11da6713137b";
                        FileStream fileStream = new FileStream(openFile.FileName ,FileMode.Open);
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                        
                        string[] splitContent = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string workingDir = string.Empty;
                        string url = string.Empty;
                        string iconFile = string.Empty;
                        
                        for (int i = 0; i < splitContent.Length; i++) {
                            if (splitContent[i].ToLower().StartsWith("workingdirectory=")) {
                                workingDir = splitContent[i].Substring(17);
                            } else if (splitContent[i].ToLower().StartsWith("url=")) {
                                url = splitContent[i].Substring(4);
                            } else if (splitContent[i].ToLower().StartsWith("iconfile=")) {
                                iconFile = splitContent[i].Substring(9);
                            }
                        }

                        if (url.ToLower().StartsWith("com.epicgames.launcher://apps") && url.IndexOf(epicGamesFallGuysApp) != -1) {
                            this.txtGameExeLocation.Text = url;
                            this.txtGameFileLocation.Text = iconFile;
                        } else {
                            MessageBox.Show(Multilingual.GetWord("message_wrong_file_selected"), Multilingual.GetWord("message_wrong_file_selected_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void btnSelectFont_Click(object sender, EventArgs e) {
            this.dlgOverlayFont.Font = this.lblOverlayFontExample.Font;
            if (this.dlgOverlayFont.ShowDialog(this).Equals(DialogResult.OK)) {
                this.lblOverlayFontExample.Font = this.dlgOverlayFont.Font;
                this.overlayFontSerialized = new FontConverter().ConvertToString(this.dlgOverlayFont.Font);
            }
        }
        private void btnResetOverlayFont_Click(object sender, EventArgs e) {
            this.lblOverlayFontExample.Font = this.cboMultilingual.SelectedIndex == 0
                ? new Font(Overlay.DefaultFontCollection.Families[1], 18, FontStyle.Regular, GraphicsUnit.Pixel)
                : new Font(Overlay.DefaultFontCollection.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            this.overlayFontSerialized = string.Empty;
        }
        private void cboMultilingual_SelectedIndexChanged(object sender, EventArgs e) {
            this.ChangeLanguage(this.cboMultilingual.SelectedIndex);
        }
        private void ChangeLanguage(int lang) {
            this.Font = new Font(Overlay.DefaultFontCollection.Families[0], 12, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
            int tempLanguage = Stats.CurrentLanguage;
            Stats.CurrentLanguage = lang;
            this.lblOverlayFontExample.Font = this.cboMultilingual.SelectedIndex == 0
                ? new Font(Overlay.DefaultFontCollection.Families[1], 18, FontStyle.Regular, GraphicsUnit.Pixel)
                : new Font(Overlay.DefaultFontCollection.Families[0], 18, FontStyle.Regular, GraphicsUnit.Pixel);
            if (lang == 0) { // English
                this.txtLogPath.Location = new Point(95, 15);
                this.txtLogPath.Size = new Size(670, 17);
                this.lblLogPathNote.Location = new Point(95, 40);
                
                this.grpStats.Font = new Font(Font.FontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
                this.txtPreviousWins.Location = new Point(94, 23);
                this.lblPreviousWinsNote.Location = new Point(140, 23);
                this.chkAutoUpdate.Location = new Point(285, 23);
                this.chkChangeHoopsieLegends.Location = new Point(446, 23);
                
                this.lblWinsFilter.Location = new Point(400, 28);
                this.lblQualifyFilter.Location = new Point(390, 64);
                this.lblFastestFilter.Location = new Point(365, 100);
                
                this.lblOverlayColor.Location = new Point(429, 173);
                
                this.txtCycleTimeSeconds.Location = new Point(96, 171);
                this.lblCycleTimeSecondsTag.Location = new Point(127, 170);

                this.txtGameExeLocation.Location = new Point(177, 22);
                this.txtGameExeLocation.Size = new Size(502, 20);
            } else if (lang == 1) { // Korean
                this.txtLogPath.Location = new Point(95, 15);
                this.txtLogPath.Size = new Size(670, 17);
                this.lblLogPathNote.Location = new Point(95, 40);
                
                this.grpStats.Font = new Font(Font.FontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
                this.txtPreviousWins.Location = new Point(94, 23);
                this.lblPreviousWinsNote.Location = new Point(140, 23);
                this.chkAutoUpdate.Location = new Point(285, 23);
                this.chkChangeHoopsieLegends.Location = new Point(446, 23);
                
                this.lblWinsFilter.Location = new Point(391, 28);
                this.lblQualifyFilter.Location = new Point(405, 64);
                this.lblFastestFilter.Location = new Point(367, 100);
                
                this.lblOverlayColor.Location = new Point(429, 173);
                
                this.txtCycleTimeSeconds.Location = new Point(83, 171);
                this.lblCycleTimeSecondsTag.Location = new Point(114, 170);

                this.txtGameExeLocation.Location = new Point(119, 22);
                this.txtGameExeLocation.Size = new Size(560, 20);
            } else if (lang == 2) { // Japanese
                this.txtLogPath.Location = new Point(123, 15);
                this.txtLogPath.Size = new Size(644, 17);
                this.lblLogPathNote.Location = new Point(73, 41);
                
                this.grpStats.Font = new Font(Font.FontFamily, 10.25F, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
                this.txtPreviousWins.Location = new Point(98, 23);
                this.lblPreviousWinsNote.Location = new Point(140, 23);
                this.chkAutoUpdate.Location = new Point(275, 23);
                this.chkChangeHoopsieLegends.Location = new Point(387, 23);

                this.lblWinsFilter.Location = new Point(355, 28);
                this.lblQualifyFilter.Location = new Point(355, 64);
                this.lblFastestFilter.Location = new Point(331, 100);
                
                this.lblOverlayColor.Location = new Point(359, 173);
                
                this.txtCycleTimeSeconds.Location = new Point(105, 171);
                this.lblCycleTimeSecondsTag.Location = new Point(135, 170);

                this.txtGameExeLocation.Location = new Point(163, 22);
                this.txtGameExeLocation.Size = new Size(518, 20);
            } else if (lang == 3) { // Simplified Chinese
                this.txtLogPath.Location = new Point(123, 15);
                this.txtLogPath.Size = new Size(644, 17);
                this.lblLogPathNote.Location = new Point(73, 41);

                this.grpStats.Font = new Font(Font.FontFamily, 10.25F, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
                this.txtPreviousWins.Location = new Point(98, 23);
                this.lblPreviousWinsNote.Location = new Point(140, 23);
                this.chkAutoUpdate.Location = new Point(275, 23);
                this.chkChangeHoopsieLegends.Location = new Point(387, 23);

                // Disabled following settings due to not applicable.
                this.chkChangeHoopsieLegends.Visible = false;
                this.chkChangeHoopsieLegends.Checked = false;


                this.lblWinsFilter.Location = new Point(420, 32);
                this.lblQualifyFilter.Location = new Point(420, 67);
                this.lblFastestFilter.Location = new Point(420, 102);

                this.lblOverlayColor.Location = new Point(480, 173);

                this.txtCycleTimeSeconds.Location = new Point(85, 170);
                this.lblCycleTimeSecondsTag.Location = new Point(115, 172);

                this.txtGameExeLocation.Location = new Point(110, 22);
                this.txtGameExeLocation.Size = new Size(571, 20);
            }

            this.lblLogPath.Text = Multilingual.GetWord("settings_log_path");
            this.lblLogPathNote.Text = Multilingual.GetWord("settings_log_path_description");
            this.btnSave.Text = Multilingual.GetWord("settings_save");
            this.grpOverlay.Text = Multilingual.GetWord("settings_overlay");
            this.chkOnlyShowGold.Text = Multilingual.GetWord("settings_gold_only");
            this.chkOnlyShowQualify.Text = Multilingual.GetWord("settings_qualify_only");
            this.chkCycleQualifyGold.Text = Multilingual.GetWord("settings_cycle_qualify__gold");
            this.chkOnlyShowLongest.Text = Multilingual.GetWord("settings_longest_only");
            this.chkOnlyShowFastest.Text = Multilingual.GetWord("settings_fastest_only");
            this.chkCycleFastestLongest.Text = Multilingual.GetWord("settings_cycle_fastest__longest");
            this.chkHidePercentages.Text = Multilingual.GetWord("settings_hide_percentages");
            this.chkHideWinsInfo.Text = Multilingual.GetWord("settings_hide_wins_info");
            this.cboOverlayColor.Items.Clear();
            this.cboOverlayColor.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_transparent"),
                Multilingual.GetWord("settings_magenta"),
                Multilingual.GetWord("settings_red"),
                Multilingual.GetWord("settings_green"),
                Multilingual.GetWord("settings_blue"),
                Multilingual.GetWord("settings_black")});
            switch (CurrentSettings.OverlayColor) {
                case 0: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 1: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
                case 2: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 3: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_transparent"); break;
                case 4: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 5: cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
            }
            this.lblOverlayColor.Text = Multilingual.GetWord("settings_background");
            this.chkFlipped.Text = Multilingual.GetWord("settings_flip_display_horizontally");
            this.chkShowTabs.Text = Multilingual.GetWord("settings_show_tab_for_currnet_filter__profile");
            this.chkHideTimeInfo.Text = Multilingual.GetWord("settings_hide_time_info");
            this.chkHideRoundInfo.Text = Multilingual.GetWord("settings_hide_round_info");
            this.cboFastestFilter.Items.Clear();
            this.cboFastestFilter.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_all_time_stats"),
                Multilingual.GetWord("settings_stats_and_party_filter"),
                Multilingual.GetWord("settings_season_stats"),
                Multilingual.GetWord("settings_week_stats"),
                Multilingual.GetWord("settings_day_stats"),
                Multilingual.GetWord("settings_session_stats")});
            switch (CurrentSettings.WinsFilter) {
                case 0: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 1: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 2: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 3: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 4: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }
            this.lblFastestFilter.Text = Multilingual.GetWord("settings_fastest__longest_filter");
            this.cboQualifyFilter.Items.Clear();
            this.cboQualifyFilter.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_all_time_stats"),
                Multilingual.GetWord("settings_stats_and_party_filter"),
                Multilingual.GetWord("settings_season_stats"),
                Multilingual.GetWord("settings_week_stats"),
                Multilingual.GetWord("settings_day_stats"),
                Multilingual.GetWord("settings_session_stats")});
            switch (CurrentSettings.QualifyFilter) {
                case 0: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }
            this.lblQualifyFilter.Text = Multilingual.GetWord("settings_qualify__gold_filter");
            this.cboWinsFilter.Items.Clear();
            this.cboWinsFilter.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_all_time_stats"),
                Multilingual.GetWord("settings_stats_and_party_filter"),
                Multilingual.GetWord("settings_season_stats"),
                Multilingual.GetWord("settings_week_stats"),
                Multilingual.GetWord("settings_day_stats"),
                Multilingual.GetWord("settings_session_stats")});
            switch (CurrentSettings.FastestFilter) {
                case 0: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }
            this.lblWinsFilter.Text = Multilingual.GetWord("settings_wins__final_filter");
            this.chkOverlayOnTop.Text = Multilingual.GetWord("settings_always_show_on_top");
            this.chkPlayerByConsoleType.Text = Multilingual.GetWord("settings_display_the_player_by_console_type");
            this.chkColorByRoundType.Text = Multilingual.GetWord("settings_display_the_color_by_round_type");
            this.lblCycleTimeSecondsTag.Text = Multilingual.GetWord("settings_sec");
            this.lblCycleTimeSeconds.Text = Multilingual.GetWord("settings_cycle_time");
            this.chkOnlyShowFinalStreak.Text = Multilingual.GetWord("settings_final_streak_only");
            this.chkOnlyShowWinStreak.Text = Multilingual.GetWord("settings_win_streak_only");
            this.chkCycleWinFinalStreak.Text = Multilingual.GetWord("settings_cycle_win__final_streak");
            this.chkOnlyShowPing.Text = Multilingual.GetWord("settings_ping_only");
            this.chkOnlyShowPlayers.Text = Multilingual.GetWord("settings_players_only");
            this.chkCyclePlayersPing.Text = Multilingual.GetWord("settings_cycle_players__ping");
            this.lblOverlayFont.Text = Multilingual.GetWord("settings_custom_overlay_font");
            this.btnSelectFont.Text = Multilingual.GetWord("settings_select_font");
            this.btnResetOverlayFont.Text = Multilingual.GetWord("settings_reset_font");
            this.grpOverlayFontExample.Text = Multilingual.GetWord("settings_font_example");
            this.lblOverlayFontExample.Text = Multilingual.GetWord("settings_round_example");
            this.grpStats.Text = Multilingual.GetWord("settings_stats");
            this.chkChangeHoopsieLegends.Text = Multilingual.GetWord("settings_rename_hoopsie_legends_to_hoopsie_heroes");
            this.chkAutoUpdate.Text = Multilingual.GetWord("settings_auto_update_program");
            this.lblPreviousWinsNote.Text = Multilingual.GetWord("settings_before_using_tracker");
            this.lblPreviousWins.Text = Multilingual.GetWord("settings_previous_win");
            this.grpGameOptions.Text = Multilingual.GetWord("settings_game_oprions");
            this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
            this.btnGameExeLocationBrowse.Text = Multilingual.GetWord("settings_browse");
            this.chkAutoLaunchGameOnStart.Text = Multilingual.GetWord("settings_auto_launch_fall_guys_on_tracker");
            this.grpSortingOptions.Text = Multilingual.GetWord("settings_sorting_options");
            this.chkIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            //this.lblLanguageSelection.Text = Multilingual.GetWord("settings_language");
            this.btnCancel.Text = Multilingual.GetWord("settings_cancel");
            this.Text = Multilingual.GetWord("settings_title");

            Stats.CurrentLanguage = tempLanguage;
        }
    }
}
