using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class Settings : MetroFramework.Forms.MetroForm {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private string overlayFontSerialized = string.Empty;
        private string overlayFontColorSerialized = string.Empty;
        public UserSettings CurrentSettings { get; set; }
        public Stats StatsForm { get; set; }
        private int LaunchPlatform;
        private int DisplayLang;
        private bool CboOverlayBackgroundIsFocus;
        public Settings() {
            this.InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(this.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);

            this.LaunchPlatform = this.CurrentSettings.LaunchPlatform;
            this.DisplayLang = Stats.CurrentLanguage;
            this.ChangeLanguage(Stats.CurrentLanguage);
            this.cboMultilingual.SelectedIndex = Stats.CurrentLanguage;
            this.txtLogPath.Text = this.CurrentSettings.LogPath;

#if !AllowUpdate
            this.chkAutoUpdate.Visible = false;
#endif

            if (this.CurrentSettings.SwitchBetweenLongest) {
                this.chkCycleFastestLongest.Checked = true;
            } else if (this.CurrentSettings.OnlyShowLongest) {
                this.chkOnlyShowLongest.Checked = true;
            } else {
                this.chkOnlyShowFastest.Checked = true;
            }
            if (this.CurrentSettings.SwitchBetweenQualify) {
                this.chkCycleQualifyGold.Checked = true;
            } else if (this.CurrentSettings.OnlyShowGold) {
                this.chkOnlyShowGold.Checked = true;
            } else {
                this.chkOnlyShowQualify.Checked = true;
            }
            if (this.CurrentSettings.SwitchBetweenPlayers) {
                this.chkCyclePlayersPing.Checked = true;
            } else if (this.CurrentSettings.OnlyShowPing) {
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
            this.chkAutoChangeProfile.Checked = this.CurrentSettings.AutoChangeProfile;
            this.chkHideWinsInfo.Checked = this.CurrentSettings.HideWinsInfo;
            this.chkHideRoundInfo.Checked = this.CurrentSettings.HideRoundInfo;
            this.chkHideTimeInfo.Checked = this.CurrentSettings.HideTimeInfo;
            this.chkShowTabs.Checked = this.CurrentSettings.ShowOverlayTabs;
            //this.chkShowProfile.Checked = this.CurrentSettings.ShowOverlayProfile;
            this.chkAutoUpdate.Checked = this.CurrentSettings.AutoUpdate;
            this.chkFlipped.Checked = this.CurrentSettings.FlippedDisplay;
            this.chkHidePercentages.Checked = this.CurrentSettings.HideOverlayPercentages;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;

            ArrayList imageItemArray = new ArrayList();
            if (Directory.Exists("Overlay")) {
                DirectoryInfo di = new DirectoryInfo("Overlay");
                foreach (FileInfo file in di.GetFiles()) {
                    if (file.Name.Equals("background.png") || file.Name.Equals("tab.png")) continue;
                    if (file.Name.StartsWith("background_") && file.Name.EndsWith(".png")) {
                        string backgroundName = file.Name.Substring(11);
                        backgroundName = backgroundName.Remove(backgroundName.Length - 4);
                        Bitmap background = new Bitmap(file.FullName);
                        //if (background.Width == 396 && background.Height == 43) continue;
                        if (background.Width == 786 && background.Height == 99) {
                            imageItemArray.Add(new ImageItem(background, new[] { $"background_{backgroundName}", $"tab_{backgroundName}" }, backgroundName, this.Font, true));
                        }
                    }
                }
            }

            int customizedBacgroundCount = imageItemArray.Count;

            ImageItem[] imageItems = {
                new ImageItem(Properties.Resources.background, new[] { "background", "tab_unselected" }, "Default", this.Font, false),
                new ImageItem(Properties.Resources.background_candycane, new[] { "background_candycane", "tab_unselected_candycane" }, "Candy Cane", this.Font, false),
                new ImageItem(Properties.Resources.background_coffee, new[] { "background_coffee", "tab_unselected_coffee" }, "Coffee", this.Font, false),
                new ImageItem(Properties.Resources.background_dove, new[] { "background_dove", "tab_unselected_dove" }, "Dove", this.Font, false),
                new ImageItem(Properties.Resources.background_fall_guys_logo, new[] { "background_fall_guys_logo", "tab_unselected_fall_guys_logo" }, "Fall Guys Logo", this.Font, false),
                new ImageItem(Properties.Resources.background_helter_skelter, new[] { "background_helter_skelter", "tab_unselected_helter_skelter" }, "Helter Skelter", this.Font, false),
                new ImageItem(Properties.Resources.background_hex_a_thon, new[] { "background_hex_a_thon", "tab_unselected_hex_a_thon" }, "Hex A Thon", this.Font, false),
                new ImageItem(Properties.Resources.background_ill_be_slime, new[] { "background_ill_be_slime", "tab_unselected_ill_be_slime" }, "I'll Be Slime", this.Font, false),
                new ImageItem(Properties.Resources.background_mockingbird, new[] { "background_mockingbird", "tab_unselected_mockingbird" }, "Mocking Bird", this.Font, false),
                new ImageItem(Properties.Resources.background_newlove, new[] { "background_newlove", "tab_unselected_newlove" }, "New Love", this.Font, false),
                new ImageItem(Properties.Resources.background_parade_guy, new[] { "background_parade_guy", "tab_unselected_parade_guy" }, "Parade Guy", this.Font, false),
                new ImageItem(Properties.Resources.background_party_pegwin, new[] { "background_party_pegwin", "tab_unselected_party_pegwin" }, "Party Pegwin", this.Font, false),
                new ImageItem(Properties.Resources.background_penguin, new[] { "background_penguin", "tab_unselected_penguin" }, "Penguin", this.Font, false),
                new ImageItem(Properties.Resources.background_suits_you, new[] { "background_suits_you", "tab_unselected_suits_you" }, "Suits You", this.Font, false),
                new ImageItem(Properties.Resources.background_sunny_guys, new[] { "background_sunny_guys", "tab_unselected_sunny_guys" }, "Sunny Guys", this.Font, false),
                new ImageItem(Properties.Resources.background_ta_da, new[] { "background_ta_da", "tab_unselected_ta_da" }, "Ta Da", this.Font, false),
                new ImageItem(Properties.Resources.background_timeattack, new[] { "background_timeattack", "tab_unselected_timeattack" }, "Time Attack", this.Font, false),
                new ImageItem(Properties.Resources.background_watermelon, new[] { "background_watermelon", "tab_unselected_watermelon" }, "Watermelon", this.Font, false),
                new ImageItem(Properties.Resources.background_super_mario_bros, new[] { "background_super_mario_bros", "tab_unselected_super_mario_bros" }, "Super Mario Bros.", this.Font, false),
                new ImageItem(Properties.Resources.background_wallpaper_01, new[] { "background_wallpaper_01", "tab_unselected_wallpaper_01" }, "Wallpaper 01", this.Font, false),
                new ImageItem(Properties.Resources.background_wallpaper_02, new[] { "background_wallpaper_02", "tab_unselected_wallpaper_02" }, "Wallpaper 02", this.Font, false),
                new ImageItem(Properties.Resources.background_wallpaper_03, new[] { "background_wallpaper_03", "tab_unselected_wallpaper_03" }, "Wallpaper 03", this.Font, false)
            };
            imageItemArray.AddRange(imageItems);
            this.cboOverlayBackground.SetImageItemData(imageItemArray);
            bool isSelected = false;
            if (this.CurrentSettings.IsOverlayBackgroundCustomized) {
                for (int i = 0; i < imageItemArray.Count; i++) {
                    if (!((ImageItem)imageItemArray[i]).ResourceName[0].Equals(this.CurrentSettings.OverlayBackgroundResourceName)) { continue; }
                    this.cboOverlayBackground.SelectedIndex = i;
                    isSelected = true;
                    break;
                }
            } else {
                for (int i = imageItemArray.Count - 1; i >= 0; i--) {
                    if (!((ImageItem)imageItemArray[i]).ResourceName[0].Equals(this.CurrentSettings.OverlayBackgroundResourceName)) { continue; }
                    this.cboOverlayBackground.SelectedIndex = i;
                    isSelected = true;
                    break;
                }
            }
            if (!isSelected) { this.cboOverlayBackground.SelectedIndex = customizedBacgroundCount; }

            switch (this.CurrentSettings.OverlayColor) {
                case 0: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_transparent"); break;
                case 1: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 2: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 3: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 4: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
                case 5: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
            }
            switch (this.CurrentSettings.WinsFilter) {
                case 0: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
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
                case 0: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }

            this.txtGameExeLocation.Text = this.CurrentSettings.GameExeLocation;
            this.txtGameShortcutLocation.Text = this.CurrentSettings.GameShortcutLocation;
            this.chkLaunchGameOnStart.Checked = this.CurrentSettings.AutoLaunchGameOnStartup;
            this.chkIgnoreLevelTypeWhenSorting.Checked = this.CurrentSettings.IgnoreLevelTypeWhenSorting;

            this.picPlatformCheck.Image = Stats.ImageOpacity(this.picPlatformCheck.Image, 0.8F);
            if (this.LaunchPlatform == 0) { // Epic Games
                this.picPlatformCheck.Parent = this.picEpicGames;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Epic Games");
                this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                this.txtGameExeLocation.Visible = false;
            } else { // Steam
                this.picPlatformCheck.Parent = this.picSteam;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Steam");
                this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                this.txtGameShortcutLocation.Visible = false;
            }

            if (!string.IsNullOrEmpty(this.CurrentSettings.OverlayFontSerialized)) {
                this.overlayFontSerialized = this.CurrentSettings.OverlayFontSerialized;
                FontConverter fontConverter = new FontConverter();
                this.lblOverlayFontExample.Font = fontConverter.ConvertFromString(this.CurrentSettings.OverlayFontSerialized) as Font;
            } else {
                this.lblOverlayFontExample.Font = Overlay.DefaultFont;
            }

            if (!string.IsNullOrEmpty(this.CurrentSettings.OverlayFontColorSerialized)) {
                this.overlayFontColorSerialized = this.CurrentSettings.OverlayFontColorSerialized;
                ColorConverter colorConverter = new ColorConverter();
                this.lblOverlayFontExample.ForeColor = (Color)colorConverter.ConvertFromString(this.CurrentSettings.OverlayFontColorSerialized);
            } else {
                this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            }
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.CboOverlayBackground_blur();
            foreach (Control c1 in Controls) {
                if (c1 is MetroLabel ml1) {
                    ml1.Theme = theme;
                } else if (c1 is MetroTextBox mtb1) {
                    mtb1.Theme = theme;
                } else if (c1 is MetroButton mb1) {
                    mb1.Theme = theme;
                } else if (c1 is MetroCheckBox mcb1) {
                    mcb1.Theme = theme;
                } else if (c1 is MetroRadioButton mrb1) {
                    mrb1.Theme = theme;
                } else if (c1 is MetroComboBox mcbo1) {
                    mcbo1.Theme = theme;
                } else if (c1 is GroupBox gb1) {
                    gb1.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    foreach (Control c2 in gb1.Controls) {
                        if (c2 is MetroLabel ml2) {
                            ml2.Theme = theme;
                        } else if (c2 is MetroTextBox mtb2) {
                            mtb2.Theme = theme;
                        } else if (c2 is MetroButton mb2) {
                            mb2.Theme = theme;
                        } else if (c2 is MetroCheckBox mcb2) {
                            mcb2.Theme = theme;
                        } else if (c2 is MetroRadioButton mrb2) {
                            mrb2.Theme = theme;
                        } else if (c2 is MetroComboBox mcbo2) {
                            mcbo2.Theme = theme;
                        } else if (c2 is GroupBox gb2) {
                            gb2.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            foreach (Control c3 in gb2.Controls) {
                                if (c3 is MetroRadioButton mrb3) {
                                    mrb3.Theme = theme;
                                } else if (c3 is Label lb3) { //lblOverlayFontExample
                                    if (!string.IsNullOrEmpty(this.overlayFontColorSerialized)) {
                                        ColorConverter colorConverter = new ColorConverter();
                                        lb3.ForeColor = (Color)colorConverter.ConvertFromString(this.overlayFontColorSerialized);
                                    } else {
                                        lb3.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CboTheme_SelectedIndexChanged(object sender, EventArgs e) {
            this.SetTheme(((ComboBox)sender).SelectedIndex == 0 ? MetroThemeStyle.Light : ((ComboBox)sender).SelectedIndex == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.Invalidate(true);
        }
        private void CboOverlayBackground_blur() {
            if (this.Theme == MetroThemeStyle.Dark) {
                this.cboOverlayBackground.ForeColor = Color.DarkGray;
                this.cboOverlayBackground.BackColor = Color.FromArgb(17, 17, 17);

                this.cboOverlayBackground.BorderColor = Color.FromArgb(153, 153, 153);
                this.cboOverlayBackground.ButtonColor = Color.FromArgb(153, 153, 153);
            } else if (this.Theme == MetroThemeStyle.Light) {
                this.cboOverlayBackground.ForeColor = Color.FromArgb(153, 153, 153);
                this.cboOverlayBackground.BackColor = Color.White;

                this.cboOverlayBackground.BorderColor = Color.FromArgb(153, 153, 153);
                this.cboOverlayBackground.ButtonColor = Color.FromArgb(153, 153, 153);
            }
        }
        private void CboOverlayBackground_focus() {
            if (this.Theme == MetroThemeStyle.Dark) {
                this.cboOverlayBackground.ForeColor = Color.FromArgb(204, 204, 204);
                this.cboOverlayBackground.BackColor = Color.FromArgb(17, 17, 17);

                this.cboOverlayBackground.BorderColor = Color.FromArgb(204, 204, 204);
                this.cboOverlayBackground.ButtonColor = Color.FromArgb(170, 170, 170);
            } else if (this.Theme == MetroThemeStyle.Light) {
                this.cboOverlayBackground.ForeColor = Color.Black;
                this.cboOverlayBackground.BackColor = Color.White;

                this.cboOverlayBackground.BorderColor = Color.FromArgb(17, 17, 17);
                this.cboOverlayBackground.ButtonColor = Color.FromArgb(17, 17, 17);
            }
        }
        private void CboOverlayBackground_MouseEnter(object sender, EventArgs e) {
            if (!this.CboOverlayBackgroundIsFocus) {
                this.CboOverlayBackground_focus();
            }
        }
        private void CboOverlayBackground_GotFocus(object sender, EventArgs e) {
            this.CboOverlayBackgroundIsFocus = true;
            this.CboOverlayBackground_focus();
        }
        private void CboOverlayBackground_MouseLeave(object sender, EventArgs e) {
            if (!this.CboOverlayBackgroundIsFocus) {
                this.CboOverlayBackground_blur();
            }
        }
        private void CboOverlayBackground_LostFocus(object sender, EventArgs e) {
            this.CboOverlayBackgroundIsFocus = false;
            this.CboOverlayBackground_blur();
        }

        private void BtnSave_Click(object sender, EventArgs e) {
            Stats.CurrentLanguage = this.cboMultilingual.SelectedIndex;
            this.CurrentSettings.Multilingual = this.cboMultilingual.SelectedIndex;

            this.CurrentSettings.LogPath = this.txtLogPath.Text;

            this.CurrentSettings.Theme = this.cboTheme.SelectedIndex;

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
            this.CurrentSettings.AutoChangeProfile = this.chkAutoChangeProfile.Checked;
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

            this.CurrentSettings.OverlayBackgroundResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).ResourceName[0];
            this.CurrentSettings.OverlayTabResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).ResourceName[1];
            this.CurrentSettings.OverlayBackground = this.cboOverlayBackground.SelectedIndex;
            this.CurrentSettings.IsOverlayBackgroundCustomized = ((ImageItem)this.cboOverlayBackground.SelectedItem).IsCustomized;

            if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_transparent")}") {
                this.CurrentSettings.OverlayColor = 0;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_black")}") {
                this.CurrentSettings.OverlayColor = 1;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_magenta")}") {
                this.CurrentSettings.OverlayColor = 2;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_red")}") {
                this.CurrentSettings.OverlayColor = 3;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_green")}") {
                this.CurrentSettings.OverlayColor = 4;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_blue")}") {
                this.CurrentSettings.OverlayColor = 5;
            }

            if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_all_time_stats")}") {
                this.CurrentSettings.WinsFilter = 0;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_stats_and_party_filter")}") {
                this.CurrentSettings.WinsFilter = 1;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_season_stats")}") {
                this.CurrentSettings.WinsFilter = 2;
            } else if ((string)this.cboWinsFilter.SelectedItem == $"{Multilingual.GetWord("settings_week_stats")}") {
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

                this.CurrentSettings.OverlayHeight = this.CurrentSettings.ShowOverlayTabs ? 134 : 99;
            }

            this.CurrentSettings.IgnoreLevelTypeWhenSorting = this.chkIgnoreLevelTypeWhenSorting.Checked;
            this.CurrentSettings.LaunchPlatform = this.LaunchPlatform;
            this.CurrentSettings.GameExeLocation = this.txtGameExeLocation.Text;
            this.CurrentSettings.GameShortcutLocation = this.txtGameShortcutLocation.Text;
            this.CurrentSettings.AutoLaunchGameOnStartup = this.chkLaunchGameOnStart.Checked;

            if (!string.IsNullOrEmpty(this.overlayFontSerialized)) {
                FontConverter fontConverter = new FontConverter();
                this.CurrentSettings.OverlayFontSerialized = fontConverter.ConvertToString(this.lblOverlayFontExample.Font);
            } else {
                this.CurrentSettings.OverlayFontSerialized = string.Empty;
                Overlay.SetDefaultFont(Stats.CurrentLanguage, 18);
            }

            if (!string.IsNullOrEmpty(this.overlayFontColorSerialized)) {
                ColorConverter colorConverter = new ColorConverter();
                this.CurrentSettings.OverlayFontColorSerialized = colorConverter.ConvertToString(this.lblOverlayFontExample.ForeColor);
            } else {
                this.CurrentSettings.OverlayFontColorSerialized = string.Empty;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void TxtCycleTimeSeconds_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(this.txtCycleTimeSeconds.Text) && !int.TryParse(this.txtCycleTimeSeconds.Text, out _)) {
                this.txtCycleTimeSeconds.Text = "5";
            }
        }
        private void TxtLogPath_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            try {
                if (this.txtLogPath.Text.IndexOf(".log", StringComparison.OrdinalIgnoreCase) > 0) {
                    this.txtLogPath.Text = Path.GetDirectoryName(this.txtLogPath.Text);
                }
            } catch { }
        }
        private void TxtPreviousWins_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!string.IsNullOrEmpty(this.txtPreviousWins.Text) && !int.TryParse(this.txtPreviousWins.Text, out _)) {
                this.txtPreviousWins.Text = "0";
            }
        }
        private void BtnGameExeLocationBrowse_Click(object sender, EventArgs e) {
            try {
                using (OpenFileDialog openFile = new OpenFileDialog()) {
                    if (this.LaunchPlatform == 0) { // Epic Games
                        openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        openFile.Filter = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_openfile_filter", this.DisplayLang);
                        openFile.FileName = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_openfile_name", this.DisplayLang);
                        openFile.Title = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_openfile_title", this.DisplayLang);

                        if (openFile.ShowDialog(this) == DialogResult.OK) {
                            string fileContent;
                            string epicGamesFallGuysApp = "50118b7f954e450f8823df1614b24e80%3A38ec4849ea4f4de6aa7b6fb0f2d278e1%3A0a2d9f6403244d12969e11da6713137b";
                            FileStream fileStream = new FileStream(openFile.FileName, FileMode.Open);
                            using (StreamReader reader = new StreamReader(fileStream)) {
                                fileContent = reader.ReadToEnd();
                            }

                            string[] splitContent = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            string url = string.Empty;

                            for (int i = 0; i < splitContent.Length; i++) {
                                if (splitContent[i].ToLower().StartsWith("url=")) {
                                    url = splitContent[i].Substring(4);
                                    break;
                                }
                            }

                            if (url.ToLower().StartsWith("com.epicgames.launcher://apps") && url.IndexOf(epicGamesFallGuysApp) > 0) {
                                this.txtGameShortcutLocation.Text = url;
                            } else {
                                MessageBox.Show(Multilingual.GetWordWithLang("message_wrong_selected_file_epicgames", this.DisplayLang), Multilingual.GetWordWithLang("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    } else { // Steam
                        if (string.IsNullOrEmpty(this.txtGameExeLocation.Text)) {
                            MessageBox.Show(Multilingual.GetWordWithLang("message_not_installed_steam", this.DisplayLang), Multilingual.GetWordWithLang("message_not_installed_steam_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        FileInfo currentExeLocation = new FileInfo(this.txtGameExeLocation.Text);
                        if (currentExeLocation.Directory.Exists) {
                            openFile.InitialDirectory = currentExeLocation.Directory.FullName;
                        }
                        openFile.Filter = Multilingual.GetWordWithLang("settings_fall_guys_exe_openfile_filter", this.DisplayLang);
                        openFile.FileName = Multilingual.GetWordWithLang("settings_fall_guys_exe_openfile_name", this.DisplayLang);
                        openFile.Title = Multilingual.GetWordWithLang("settings_fall_guys_exe_openfile_title", this.DisplayLang);

                        if (openFile.ShowDialog(this) == DialogResult.OK) {
                            if (openFile.FileName.IndexOf("FallGuys_client", StringComparison.OrdinalIgnoreCase) >= 0) {
                                txtGameExeLocation.Text = openFile.FileName;
                            } else {
                                MessageBox.Show(Multilingual.GetWordWithLang("message_wrong_selected_file_steam", this.DisplayLang), Multilingual.GetWordWithLang("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }
        private void LaunchPlatform_Click(object sender, EventArgs e) {
            if ((bool)((PictureBox)sender)?.Name.Equals("picEpicGames")) { // Epic Games
                this.picPlatformCheck.Parent = this.picEpicGames;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Epic Games");
                this.txtGameShortcutLocation.Visible = true;
                this.txtGameExeLocation.Visible = false;

                if (this.DisplayLang == 0) { // English
                    this.txtGameShortcutLocation.Location = new Point(267, 23);
                    this.txtGameShortcutLocation.Size = new Size(508, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", "eng");
                } else if (this.DisplayLang == 1) { // French
                    this.txtGameShortcutLocation.Location = new Point(215, 23);
                    this.txtGameShortcutLocation.Size = new Size(560, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", "fre");
                } else if (this.DisplayLang == 2) { // Korean
                    this.txtGameShortcutLocation.Location = new Point(278, 23);
                    this.txtGameShortcutLocation.Size = new Size(497, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", "kor");
                } else if (this.DisplayLang == 3) { // Japanese
                    this.txtGameShortcutLocation.Location = new Point(293, 23);
                    this.txtGameShortcutLocation.Size = new Size(482, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", "jpn");
                } else if (this.DisplayLang == 4) { // Simplified Chinese
                    this.txtGameShortcutLocation.Location = new Point(252, 23);
                    this.txtGameShortcutLocation.Size = new Size(523, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", "chs");
                }

                this.LaunchPlatform = 0;
            } else if (((PictureBox)sender).Name.Equals("picSteam")) { // Steam
                this.StatsForm.UpdateGameExeLocation();
                this.txtGameExeLocation.Text = this.CurrentSettings.GameExeLocation;
                this.picPlatformCheck.Parent = this.picSteam;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Steam");
                this.txtGameShortcutLocation.Visible = false;
                this.txtGameExeLocation.Visible = true;

                if (this.DisplayLang == 0) { // English
                    this.txtGameExeLocation.Location = new Point(240, 23);
                    this.txtGameExeLocation.Size = new Size(535, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", "eng");
                } else if (this.DisplayLang == 1) { // French
                    this.txtGameExeLocation.Location = new Point(195, 23);
                    this.txtGameExeLocation.Size = new Size(580, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", "fre");
                } else if (this.DisplayLang == 2) { // Korean
                    this.txtGameExeLocation.Location = new Point(250, 23);
                    this.txtGameExeLocation.Size = new Size(525, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", "kor");
                } else if (this.DisplayLang == 3) { // Japanese
                    this.txtGameExeLocation.Location = new Point(280, 23);
                    this.txtGameExeLocation.Size = new Size(495, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", "jpn");
                } else if (this.DisplayLang == 4) { // Simplified Chinese
                    this.txtGameExeLocation.Location = new Point(224, 23);
                    this.txtGameExeLocation.Size = new Size(551, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", "chs");
                }

                this.LaunchPlatform = 1;
            }
            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(11, 1) : new Point(9, 1);
        }
        private void BtnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void BtnSelectFont_Click(object sender, EventArgs e) {
            this.dlgOverlayFont.Font = string.IsNullOrEmpty(this.overlayFontSerialized)
                ? Overlay.GetDefaultFont(this.DisplayLang, 24)
                : new Font(this.lblOverlayFontExample.Font.FontFamily, lblOverlayFontExample.Font.Size, lblOverlayFontExample.Font.Style, GraphicsUnit.Point, (byte)1);

            this.dlgOverlayFont.ShowColor = true;
            this.dlgOverlayFont.Color = string.IsNullOrEmpty(this.overlayFontColorSerialized) ? Color.White : this.lblOverlayFontExample.ForeColor;

            if (this.dlgOverlayFont.ShowDialog(this) == DialogResult.OK) {
                if (this.dlgOverlayFont.Color == Color.White) {
                    this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    this.overlayFontColorSerialized = string.Empty;
                } else {
                    this.lblOverlayFontExample.ForeColor = this.dlgOverlayFont.Color;
                    this.overlayFontColorSerialized = new ColorConverter().ConvertToString(this.lblOverlayFontExample.ForeColor);
                }

                this.lblOverlayFontExample.Font = new Font(this.dlgOverlayFont.Font.FontFamily, this.dlgOverlayFont.Font.Size, this.dlgOverlayFont.Font.Style, GraphicsUnit.Pixel, (byte)1);
                this.overlayFontSerialized = new FontConverter().ConvertToString(this.lblOverlayFontExample.Font);
            }
        }
        private void BtnResetOverlayFont_Click(object sender, EventArgs e) {
            this.lblOverlayFontExample.Font = Overlay.GetDefaultFont(this.DisplayLang, 18);

            this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.overlayFontColorSerialized = string.Empty;

            this.overlayFontSerialized = string.Empty;
        }
        private void CboMultilingual_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.DisplayLang == ((ComboBox)sender).SelectedIndex) return;
            this.ChangeLanguage(((ComboBox)sender).SelectedIndex);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Tab) {
                SendKeys.Send("%");
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeLanguage(int lang) {
            this.DisplayLang = lang;
            this.Font = Overlay.GetMainFont(12, this.DisplayLang);
            int tempLanguage = Stats.CurrentLanguage;
            Stats.CurrentLanguage = lang;

            this.lblOverlayFontExample.Font = Overlay.GetDefaultFont(this.DisplayLang, 18);
            this.overlayFontSerialized = string.Empty;
            this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.overlayFontColorSerialized = string.Empty;
            //if (string.IsNullOrEmpty(this.overlayFontColorSerialized)) {
            //    this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            //    this.overlayFontColorSerialized = string.Empty;
            //}

            this.chkChangeHoopsieLegends.Visible = true;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;

            if (this.DisplayLang == 0) {
                // English
                this.txtLogPath.Location = new Point(103, 73);
                this.txtLogPath.Size = new Size(760, 22);
                this.lblLogPathNote.Location = new Point(103, 98);

                this.txtPreviousWins.Location = new Point(120, 55);
                this.lblPreviousWinsNote.Location = new Point(170, 55);
                this.chkChangeHoopsieLegends.Location = new Point(399, 55);

                this.lblWinsFilter.Location = new Point(501, 23);
                this.lblQualifyFilter.Location = new Point(487, 58);
                this.lblFastestFilter.Location = new Point(473, 93);
                this.lblOverlayBackground.Location = new Point(491, 129);
                this.lblOverlayColor.Location = new Point(532, 168);

                this.txtCycleTimeSeconds.Location = new Point(90, 192);
                this.lblCycleTimeSecondsTag.Location = new Point(118, 192);

                this.grpLaunchPlatform.Size = new Size(97, 60);
                this.lblGameExeLocation.Location = new Point(113, 23);
                this.chkLaunchGameOnStart.Location = new Point(118, 55);
                if (this.LaunchPlatform == 0) {
                    this.txtGameShortcutLocation.Location = new Point(267, 23);
                    this.txtGameShortcutLocation.Size = new Size(508, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                } else {
                    this.txtGameExeLocation.Location = new Point(267, 23);
                    this.txtGameExeLocation.Size = new Size(508, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                }
            } else if (this.DisplayLang == 1) {
                // French
                this.txtLogPath.Location = new Point(188, 73);
                this.txtLogPath.Size = new Size(675, 22);
                this.lblLogPathNote.Location = new Point(188, 98);

                this.txtPreviousWins.Location = new Point(154, 55);
                this.lblPreviousWinsNote.Location = new Point(200, 55);
                this.chkChangeHoopsieLegends.Location = new Point(429, 55);

                this.lblWinsFilter.Location = new Point(477, 23);
                this.lblQualifyFilter.Location = new Point(491, 58);
                this.lblFastestFilter.Location = new Point(444, 93);
                this.lblOverlayBackground.Location = new Point(480, 129);
                this.lblOverlayColor.Location = new Point(516, 168);

                this.txtCycleTimeSeconds.Location = new Point(190, 192);
                this.lblCycleTimeSecondsTag.Location = new Point(218, 192);

                this.grpLaunchPlatform.Size = new Size(97, 60);
                this.lblGameExeLocation.Location = new Point(113, 23);
                this.chkLaunchGameOnStart.Location = new Point(118, 55);
                if (this.LaunchPlatform == 0) {
                    this.txtGameShortcutLocation.Location = new Point(215, 23);
                    this.txtGameShortcutLocation.Size = new Size(560, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                } else {
                    this.txtGameExeLocation.Location = new Point(195, 23);
                    this.txtGameExeLocation.Size = new Size(580, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                }
            } else if (this.DisplayLang == 2) {
                // Korean
                this.txtLogPath.Location = new Point(114, 73);
                this.txtLogPath.Size = new Size(749, 22);
                this.lblLogPathNote.Location = new Point(114, 98);

                this.txtPreviousWins.Location = new Point(116, 55);
                this.lblPreviousWinsNote.Location = new Point(166, 55);
                this.chkChangeHoopsieLegends.Location = new Point(399, 55);

                this.lblWinsFilter.Location = new Point(465, 23);
                this.lblQualifyFilter.Location = new Point(483, 58);
                this.lblFastestFilter.Location = new Point(433, 93);
                this.lblOverlayBackground.Location = new Point(478, 129);
                this.lblOverlayColor.Location = new Point(492, 168);

                this.txtCycleTimeSeconds.Location = new Point(90, 192);
                this.lblCycleTimeSecondsTag.Location = new Point(118, 192);

                this.grpLaunchPlatform.Size = new Size(97, 60);
                this.lblGameExeLocation.Location = new Point(113, 23);
                this.chkLaunchGameOnStart.Location = new Point(118, 55);
                if (this.LaunchPlatform == 0) {
                    this.txtGameShortcutLocation.Location = new Point(278, 23);
                    this.txtGameShortcutLocation.Size = new Size(497, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                } else {
                    this.txtGameExeLocation.Location = new Point(250, 23);
                    this.txtGameExeLocation.Size = new Size(525, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                }
            } else if (this.DisplayLang == 3) {
                // Japanese
                this.txtLogPath.Location = new Point(136, 73);
                this.txtLogPath.Size = new Size(727, 22);
                this.lblLogPathNote.Location = new Point(67, 98);

                this.txtPreviousWins.Location = new Point(121, 55);
                this.lblPreviousWinsNote.Location = new Point(166, 55);
                this.chkChangeHoopsieLegends.Location = new Point(390, 55);

                this.lblWinsFilter.Location = new Point(430, 23);
                this.lblQualifyFilter.Location = new Point(430, 58);
                this.lblFastestFilter.Location = new Point(401, 93);
                this.lblOverlayBackground.Location = new Point(469, 129);
                this.lblOverlayColor.Location = new Point(455, 168);

                this.txtCycleTimeSeconds.Location = new Point(110, 192);
                this.lblCycleTimeSecondsTag.Location = new Point(138, 192);

                this.grpLaunchPlatform.Size = new Size(97, 60);
                this.lblGameExeLocation.Location = new Point(113, 23);
                this.chkLaunchGameOnStart.Location = new Point(118, 55);
                if (this.LaunchPlatform == 0) {
                    this.txtGameShortcutLocation.Location = new Point(293, 23);
                    this.txtGameShortcutLocation.Size = new Size(482, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                } else {
                    this.txtGameExeLocation.Location = new Point(280, 23);
                    this.txtGameExeLocation.Size = new Size(495, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                }
            } else if (this.DisplayLang == 4) {
                // Simplified Chinese
                this.txtLogPath.Location = new Point(109, 73);
                this.txtLogPath.Size = new Size(754, 22);
                this.lblLogPathNote.Location = new Point(109, 98);

                this.txtPreviousWins.Location = new Point(110, 55);
                this.lblPreviousWinsNote.Location = new Point(155, 55);
                this.chkChangeHoopsieLegends.Location = new Point(399, 55);

                this.lblWinsFilter.Location = new Point(500, 23);
                this.lblQualifyFilter.Location = new Point(500, 58);
                this.lblFastestFilter.Location = new Point(500, 93);
                this.lblOverlayBackground.Location = new Point(555, 129);
                this.lblOverlayColor.Location = new Point(569, 168);

                this.txtCycleTimeSeconds.Location = new Point(87, 192);
                this.lblCycleTimeSecondsTag.Location = new Point(116, 192);

                this.grpLaunchPlatform.Size = new Size(97, 60);
                this.lblGameExeLocation.Location = new Point(113, 23);
                this.chkLaunchGameOnStart.Location = new Point(118, 55);
                if (this.LaunchPlatform == 0) {
                    this.txtGameShortcutLocation.Location = new Point(252, 23);
                    this.txtGameShortcutLocation.Size = new Size(523, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                } else {
                    this.txtGameExeLocation.Location = new Point(224, 23);
                    this.txtGameExeLocation.Size = new Size(551, 25);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                }
            }

            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(11, 1) : new Point(9, 1);

            this.lblTheme.Text = Multilingual.GetWord("settings_theme");
            this.cboTheme.Items.Clear();
            this.cboTheme.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_theme_light"),
                Multilingual.GetWord("settings_theme_dark"),
            });
            switch (this.CurrentSettings.Theme) {
                case 0: this.cboTheme.SelectedItem = Multilingual.GetWord("settings_theme_light"); break;
                case 1: this.cboTheme.SelectedItem = Multilingual.GetWord("settings_theme_dark"); break;
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

            //ImageItem[] imageItemArray = {
            //    new ImageItem(Properties.Resources.background, "", "Default", this.Font),
            //    new ImageItem(Properties.Resources.background_candycane, "candycane", "Candy Cane", this.Font),
            //    new ImageItem(Properties.Resources.background_coffee, "coffee", "Coffee", this.Font),
            //    new ImageItem(Properties.Resources.background_dove, "dove", "Dove", this.Font),
            //    new ImageItem(Properties.Resources.background_fall_guys_logo, "fall_guys_logo", "Fall Guys Logo", this.Font),
            //    new ImageItem(Properties.Resources.background_helter_skelter, "helter_skelter", "Helter Skelter", this.Font),
            //    new ImageItem(Properties.Resources.background_hex_a_thon, "hex_a_thon", "Hex A Thon", this.Font),
            //    new ImageItem(Properties.Resources.background_ill_be_slime, "ill_be_slime", "I'll Be Slime", this.Font),
            //    new ImageItem(Properties.Resources.background_mockingbird, "mockingbird", "Mocking Bird", this.Font),
            //    new ImageItem(Properties.Resources.background_newlove, "newlove", "New Love", this.Font),
            //    new ImageItem(Properties.Resources.background_parade_guy, "parade_guy", "Parade Guy", this.Font),
            //    new ImageItem(Properties.Resources.background_party_pegwin, "party_pegwin", "Party Pegwin", this.Font),
            //    new ImageItem(Properties.Resources.background_penguin, "penguin", "Penguin", this.Font),
            //    new ImageItem(Properties.Resources.background_suits_you, "suits_you", "Suits You", this.Font),
            //    new ImageItem(Properties.Resources.background_sunny_guys, "sunny_guys", "Sunny Guys", this.Font),
            //    new ImageItem(Properties.Resources.background_ta_da, "ta_da", "Ta Da", this.Font),
            //    new ImageItem(Properties.Resources.background_timeattack, "timeattack", "Time Attack", this.Font),
            //    new ImageItem(Properties.Resources.background_watermelon, "watermelon", "Watermelon", this.Font),
            //    new ImageItem(Properties.Resources.background_wallpaper_01, "wallpaper_01", "Wallpaper 01", this.Font),
            //    new ImageItem(Properties.Resources.background_wallpaper_02, "wallpaper_02", "Wallpaper 02", this.Font),
            //    new ImageItem(Properties.Resources.background_wallpaper_03, "wallpaper_03", "Wallpaper 03", this.Font),
            //};
            //this.cboOverlayBackground.SetImageItemData(imageItemArray);
            //this.cboOverlayBackground.SelectedIndex = this.CurrentSettings.OverlayBackground;

            this.cboOverlayColor.Items.Clear();
            this.cboOverlayColor.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_transparent"),
                Multilingual.GetWord("settings_black"),
                Multilingual.GetWord("settings_magenta"),
                Multilingual.GetWord("settings_red"),
                Multilingual.GetWord("settings_green"),
                Multilingual.GetWord("settings_blue")
            });
            switch (this.CurrentSettings.OverlayColor) {
                case 0: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_transparent"); break;
                case 1: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 2: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 3: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 4: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
                case 5: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
            }

            this.lblOverlayBackground.Text = Multilingual.GetWord("settings_background_image");
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
                Multilingual.GetWord("settings_session_stats")
            });
            switch (this.CurrentSettings.FastestFilter) {
                case 0: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboFastestFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }

            this.lblFastestFilter.Text = Multilingual.GetWord("settings_fastest__longest_filter");

            this.cboQualifyFilter.Items.Clear();
            this.cboQualifyFilter.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_all_time_stats"),
                Multilingual.GetWord("settings_stats_and_party_filter"),
                Multilingual.GetWord("settings_season_stats"),
                Multilingual.GetWord("settings_week_stats"),
                Multilingual.GetWord("settings_day_stats"),
                Multilingual.GetWord("settings_session_stats")
            });
            switch (this.CurrentSettings.QualifyFilter) {
                case 0: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboQualifyFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }

            this.lblQualifyFilter.Text = Multilingual.GetWord("settings_qualify__gold_filter");

            this.cboWinsFilter.Items.Clear();
            this.cboWinsFilter.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_all_time_stats"),
                Multilingual.GetWord("settings_stats_and_party_filter"),
                Multilingual.GetWord("settings_season_stats"),
                Multilingual.GetWord("settings_week_stats"),
                Multilingual.GetWord("settings_day_stats"),
                Multilingual.GetWord("settings_session_stats")
            });
            switch (this.CurrentSettings.WinsFilter) {
                case 0: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_all_time_stats"); break;
                case 1: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_stats_and_party_filter"); break;
                case 2: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_season_stats"); break;
                case 3: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_week_stats"); break;
                case 4: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_day_stats"); break;
                case 5: this.cboWinsFilter.SelectedItem = Multilingual.GetWord("settings_session_stats"); break;
            }

            this.lblWinsFilter.Text = Multilingual.GetWord("settings_wins__final_filter");
            this.chkOverlayOnTop.Text = Multilingual.GetWord("settings_always_show_on_top");
            this.chkPlayerByConsoleType.Text = Multilingual.GetWord("settings_display_players_based_on_platform");
            this.chkColorByRoundType.Text = Multilingual.GetWord("settings_color_round_name_based_on_round_type");
            this.chkAutoChangeProfile.Text = Multilingual.GetWord("settings_auto_change_profile");
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
            this.grpGameOptions.Text = Multilingual.GetWord("settings_game_options");
            this.grpLaunchPlatform.Text = Multilingual.GetWord("settings_game_options_platform");
            //this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
            this.btnGameExeLocationBrowse.Text = Multilingual.GetWord("settings_browse");
            this.chkLaunchGameOnStart.Text = Multilingual.GetWord("settings_launch_fall_guys_on_tracker_launch");
            this.grpSortingOptions.Text = Multilingual.GetWord("settings_sorting_options");
            this.chkIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            //this.lblLanguageSelection.Text = Multilingual.GetWord("settings_language");
            this.btnCancel.Text = Multilingual.GetWord("settings_cancel");
            this.Text = Multilingual.GetWord("settings_title");

            Stats.CurrentLanguage = tempLanguage;
            this.Invalidate(true);
        }
    }
}