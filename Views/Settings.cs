using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
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
        public Overlay Overlay { get; set; }
        private int LaunchPlatform;
        private int DisplayLang;
        private bool CboOverlayBackgroundIsFocus;
        private bool TrkOverlayOpacityIsEnter;

        private Bitmap ResizeImage(Bitmap source, int scale) {
            return new Bitmap(source, new Size(source.Width / scale, source.Height / scale));
        }
        public Settings() {
            this.InitializeComponent();
            this.cboMultilingual.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboTheme.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboWinsFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboQualifyFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboFastestFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboOverlayBackground.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboOverlayColor.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
        }
        private void Settings_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);

            this.ChangeTab(this.tileProgram, null);
            
            this.LaunchPlatform = this.CurrentSettings.LaunchPlatform;
            this.DisplayLang = Stats.CurrentLanguage;
            this.ChangeLanguage(Stats.CurrentLanguage);
            this.cboMultilingual.SelectedIndex = Stats.CurrentLanguage;
            this.txtLogPath.Text = this.CurrentSettings.LogPath;

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
            this.chkShadeTheFlagImage.Checked = this.CurrentSettings.ShadeTheFlagImage;
            this.chkHideWinsInfo.Checked = this.CurrentSettings.HideWinsInfo;
            this.chkHideRoundInfo.Checked = this.CurrentSettings.HideRoundInfo;
            this.chkHideTimeInfo.Checked = this.CurrentSettings.HideTimeInfo;
            this.chkShowTabs.Checked = this.CurrentSettings.ShowOverlayTabs;
            //this.chkShowProfile.Checked = this.CurrentSettings.ShowOverlayProfile;
            this.chkAutoUpdate.Checked = this.CurrentSettings.AutoUpdate;
            this.chkSystemTrayIcon.Checked = this.CurrentSettings.SystemTrayIcon;
            this.chkNotifyServerConnected.Checked = this.CurrentSettings.NotifyServerConnected;
            this.chkPreventOverlayMouseClicks.Checked = this.CurrentSettings.PreventOverlayMouseClicks;
            this.chkFlipped.Checked = this.CurrentSettings.FlippedDisplay;
            this.chkHidePercentages.Checked = this.CurrentSettings.HideOverlayPercentages;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;

            this.chkFallalyticsReporting.Checked = this.CurrentSettings.EnableFallalyticsReporting;
            this.txtFallalyticsAPIKey.Text = this.CurrentSettings.FallalyticsAPIKey;

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
                new ImageItem(Properties.Resources.background_monarch, new[] { "background_monarch", "tab_unselected_monarch" }, "Monarch", this.Font, false),
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
                new ImageItem(Properties.Resources.background_super_mario_bros_3, new[] { "background_super_mario_bros_3", "tab_unselected_super_mario_bros_3" }, "Super Mario Bros. 3", this.Font, false),
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
                case 1: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_white"); break;
                case 2: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 3: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 4: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 5: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
                case 6: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
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
            this.chkGroupingCreativeRoundLevels.Checked = this.CurrentSettings.GroupingCreativeRoundLevels;

            //this.picPlatformCheck.Image = Stats.ImageOpacity(this.picPlatformCheck.Image, 0.8F);
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
            
            this.trkOverlayOpacity.Value = this.CurrentSettings.OverlayBackgroundOpacity;
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.BackImage = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
            this.cboOverlayBackground_blur(theme);
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
                } else if (c1 is MetroPanel gb1) {
                    gb1.Theme = theme;
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
                        } else if (c2 is MetroLink mlnk2) {
                            mlnk2.Theme = theme;
                        } else if (c2 is MetroTrackBar mtrb2) {
                            mtrb2.Theme = theme;
                        } else if (c2 is GroupBox gb2) {
                            gb2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            foreach (Control c3 in gb2.Controls) {
                                if (c3 is MetroRadioButton mrb3) {
                                    mrb3.Theme = theme;
                                } else if (c3 is Label lb3) { //lblOverlayFontExample
                                    if (!string.IsNullOrEmpty(this.overlayFontColorSerialized)) {
                                        ColorConverter colorConverter = new ColorConverter();
                                        lb3.ForeColor = (Color)colorConverter.ConvertFromString(this.overlayFontColorSerialized);
                                    } else {
                                        lb3.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.Theme = theme;
        }
        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e) {
            this.SetTheme(((ComboBox)sender).SelectedIndex == 0 ? MetroThemeStyle.Light : ((ComboBox)sender).SelectedIndex == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.Invalidate(true);
        }
        private void cboOverlayBackground_blur(MetroThemeStyle theme) {
            this.cboOverlayBackground.ForeColor = theme == MetroThemeStyle.Light ? Color.FromArgb(153, 153, 153) : Color.DarkGray;
            this.cboOverlayBackground.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.cboOverlayBackground.BorderColor = Color.FromArgb(153, 153, 153);
            this.cboOverlayBackground.ButtonColor = Color.FromArgb(153, 153, 153);
        }
        private void cboOverlayBackground_focus(MetroThemeStyle theme) {
            this.cboOverlayBackground.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.FromArgb(204, 204, 204);
            this.cboOverlayBackground.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
            this.cboOverlayBackground.BorderColor = theme == MetroThemeStyle.Light ? Color.FromArgb(17, 17, 17) : Color.FromArgb(204, 204, 204);
            this.cboOverlayBackground.ButtonColor = theme == MetroThemeStyle.Light ? Color.FromArgb(17, 17, 17) : Color.FromArgb(170, 170, 170);
        }
        private void cboOverlayBackground_MouseEnter(object sender, EventArgs e) {
            if (!this.CboOverlayBackgroundIsFocus) {
                this.cboOverlayBackground_focus(this.Theme);
            }
        }
        private void cboOverlayBackground_GotFocus(object sender, EventArgs e) {
            this.CboOverlayBackgroundIsFocus = true;
            this.cboOverlayBackground_focus(this.Theme);
        }
        private void cboOverlayBackground_MouseLeave(object sender, EventArgs e) {
            if (!this.CboOverlayBackgroundIsFocus) {
                this.cboOverlayBackground_blur(this.Theme);
            }
        }
        private void cboOverlayBackground_LostFocus(object sender, EventArgs e) {
            this.CboOverlayBackgroundIsFocus = false;
            this.cboOverlayBackground_blur(this.Theme);
        }

        private void btnSave_Click(object sender, EventArgs e) {
            this.CurrentSettings.LogPath = this.txtLogPath.Text;
            Stats.CurrentLanguage = this.cboMultilingual.SelectedIndex;
            this.CurrentSettings.Multilingual = this.cboMultilingual.SelectedIndex;
            this.CurrentSettings.Theme = this.cboTheme.SelectedIndex;
            Stats.CurrentTheme = this.cboTheme.SelectedIndex == 0 ? MetroThemeStyle.Light :
                this.cboTheme.SelectedIndex == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default;

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
            this.CurrentSettings.ShadeTheFlagImage = this.chkShadeTheFlagImage.Checked;
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
            
            if (resizeOverlay) {
                int overlaySetting = (this.CurrentSettings.HideWinsInfo ? 4 : 0) + (this.CurrentSettings.HideRoundInfo ? 2 : 0) + (this.CurrentSettings.HideTimeInfo ? 1 : 0);
                switch (overlaySetting) {
                    case 0: this.CurrentSettings.OverlayWidth = 786; this.CurrentSettings.OverlayFixedWidth = 786; break;
                    case 1: this.CurrentSettings.OverlayWidth = 786 - 225 - 6; this.CurrentSettings.OverlayFixedWidth = 786 - 225 - 6; break;
                    case 2: this.CurrentSettings.OverlayWidth = 786 - 281 - 6; this.CurrentSettings.OverlayFixedWidth = 786 - 281 - 6; break;
                    case 3: this.CurrentSettings.OverlayWidth = 786 - 281 - 225 - 12; this.CurrentSettings.OverlayFixedWidth = 786 - 281 - 225 - 12; break;
                    case 4: this.CurrentSettings.OverlayWidth = 786 - 242 - 6; this.CurrentSettings.OverlayFixedWidth = 786 - 242 - 6; break;
                    case 5: this.CurrentSettings.OverlayWidth = 786 - 242 - 225 - 12; this.CurrentSettings.OverlayFixedWidth = 786 - 242 - 225 - 12; break;
                    case 6: this.CurrentSettings.OverlayWidth = 786 - 242 - 281 - 12; this.CurrentSettings.OverlayFixedWidth = 786 - 242 - 281 - 12; break;
                }

                if (this.CurrentSettings.ShowOverlayTabs) {
                    this.CurrentSettings.OverlayHeight = 134;
                } else {
                    this.CurrentSettings.OverlayHeight = 99;
                }
            }
            
            this.CurrentSettings.AutoUpdate = this.chkAutoUpdate.Checked;
            this.CurrentSettings.SystemTrayIcon = this.chkSystemTrayIcon.Checked;
            this.CurrentSettings.NotifyServerConnected = this.chkNotifyServerConnected.Checked;
            this.CurrentSettings.PreventOverlayMouseClicks = this.chkPreventOverlayMouseClicks.Checked;
            this.CurrentSettings.FlippedDisplay = this.chkFlipped.Checked;
            this.CurrentSettings.HideOverlayPercentages = this.chkHidePercentages.Checked;
            this.CurrentSettings.HoopsieHeros = this.chkChangeHoopsieLegends.Checked;

            this.CurrentSettings.OverlayBackgroundResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).ResourceName[0];
            this.CurrentSettings.OverlayTabResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).ResourceName[1];
            this.CurrentSettings.OverlayBackground = this.cboOverlayBackground.SelectedIndex;
            this.CurrentSettings.IsOverlayBackgroundCustomized = ((ImageItem)this.cboOverlayBackground.SelectedItem).IsCustomized;

            if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_transparent")}") {
                this.CurrentSettings.OverlayColor = 0;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_white")}") {
                this.CurrentSettings.OverlayColor = 1;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_black")}") {
                this.CurrentSettings.OverlayColor = 2;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_magenta")}") {
                this.CurrentSettings.OverlayColor = 3;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_red")}") {
                this.CurrentSettings.OverlayColor = 4;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_green")}") {
                this.CurrentSettings.OverlayColor = 5;
            } else if ((string)this.cboOverlayColor.SelectedItem == $"{Multilingual.GetWord("settings_blue")}") {
                this.CurrentSettings.OverlayColor = 6;
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

            this.CurrentSettings.IgnoreLevelTypeWhenSorting = this.chkIgnoreLevelTypeWhenSorting.Checked;
            this.CurrentSettings.GroupingCreativeRoundLevels = this.chkGroupingCreativeRoundLevels.Checked;
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

            this.CurrentSettings.OverlayBackgroundOpacity = this.trkOverlayOpacity.Value;

            this.CurrentSettings.EnableFallalyticsReporting = this.chkFallalyticsReporting.Checked;
            this.CurrentSettings.FallalyticsAPIKey = this.txtFallalyticsAPIKey.Text;
            
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

                            if (url.ToLower().StartsWith("com.epicgames.launcher://apps/") && url.IndexOf(epicGamesFallGuysApp) > 0) {
                                this.txtGameShortcutLocation.Text = url;
                            } else {
                                MetroMessageBox.Show(this, Multilingual.GetWordWithLang("message_wrong_selected_file_epicgames", this.DisplayLang), Multilingual.GetWordWithLang("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    } else { // Steam
                        if (string.IsNullOrEmpty(this.txtGameExeLocation.Text)) {
                            MetroMessageBox.Show(this, Multilingual.GetWordWithLang("message_not_installed_steam", this.DisplayLang), Multilingual.GetWordWithLang("message_not_installed_steam_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                MetroMessageBox.Show(this, Multilingual.GetWordWithLang("message_wrong_selected_file_steam", this.DisplayLang), Multilingual.GetWordWithLang("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }
        private void launchPlatform_Click(object sender, EventArgs e) {
            this.StatsForm.UpdateGameExeLocation();
            if ((bool)((PictureBox)sender)?.Name.Equals("picEpicGames")) { // Epic Games
                this.picPlatformCheck.Parent = this.picEpicGames;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Epic Games");
                this.txtGameShortcutLocation.Visible = true;
                this.txtGameExeLocation.Visible = false;

                this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_shortcut_location", this.DisplayLang);
                this.txtGameShortcutLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                this.txtGameShortcutLocation.Size = new Size(567 - this.txtGameShortcutLocation.Location.X, 25);

                this.LaunchPlatform = 0;
            } else if (((PictureBox)sender).Name.Equals("picSteam")) { // Steam
                this.txtGameExeLocation.Text = this.CurrentSettings.GameExeLocation;
                this.picPlatformCheck.Parent = this.picSteam;
                this.platformToolTip.SetToolTip(this.picPlatformCheck, "Steam");
                this.txtGameShortcutLocation.Visible = false;
                this.txtGameExeLocation.Visible = true;

                this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                this.lblGameExeLocation.Text = Multilingual.GetWordWithLang("settings_fall_guys_exe_location", this.DisplayLang);
                this.txtGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                this.txtGameExeLocation.Size = new Size(567 - this.txtGameExeLocation.Location.X, 25);

                this.LaunchPlatform = 1;
            }
            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(20, 16) : new Point(19, 14);
        }
        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void btnSelectFont_Click(object sender, EventArgs e) {
            this.dlgOverlayFont.Font = string.IsNullOrEmpty(this.overlayFontSerialized)
                ? Overlay.GetDefaultFont(this.DisplayLang, 24)
                : new Font(this.lblOverlayFontExample.Font.FontFamily, lblOverlayFontExample.Font.Size, lblOverlayFontExample.Font.Style, GraphicsUnit.Point, (byte)1);

            this.dlgOverlayFont.ShowColor = true;
            this.dlgOverlayFont.Color = string.IsNullOrEmpty(this.overlayFontColorSerialized) ? Color.White : this.lblOverlayFontExample.ForeColor;

            try {
                if (this.dlgOverlayFont.ShowDialog(this) == DialogResult.OK) {
                    if (this.dlgOverlayFont.Color == Color.White) {
                        this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                        this.overlayFontColorSerialized = string.Empty;
                    } else {
                        this.lblOverlayFontExample.ForeColor = this.dlgOverlayFont.Color;
                        this.overlayFontColorSerialized = new ColorConverter().ConvertToString(this.lblOverlayFontExample.ForeColor);
                    }

                    this.lblOverlayFontExample.Font = new Font(this.dlgOverlayFont.Font.FontFamily, this.dlgOverlayFont.Font.Size, this.dlgOverlayFont.Font.Style, GraphicsUnit.Pixel, ((byte)(1)));
                    this.overlayFontSerialized = new FontConverter().ConvertToString(this.lblOverlayFontExample.Font);
                }
            } catch {
                MetroMessageBox.Show(this, $"{Multilingual.GetWord("settings_font_need_to_be_installed_message")}", $"{Multilingual.GetWord("settings_font_need_to_be_installed_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnResetOverlayFont_Click(object sender, EventArgs e) {
            this.lblOverlayFontExample.Font = Overlay.GetDefaultFont(this.DisplayLang, 18);
            this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.overlayFontColorSerialized = string.Empty;
            this.overlayFontSerialized = string.Empty;
        }
        private void cboMultilingual_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.DisplayLang == ((ComboBox)sender).SelectedIndex) return;
            this.ChangeLanguage(((ComboBox)sender).SelectedIndex);
        }
        private void trkOverlayOpacity_ValueChanged(object sender, EventArgs e) {
            if (((MetroTrackBar)sender).Value == (this.Overlay.Opacity * 100)) { return; }
            this.Overlay.Opacity = ((MetroTrackBar)sender).Value / 100d;
            if (this.TrkOverlayOpacityIsEnter) {
                Point cursorPosition = this.PointToClient(Cursor.Position);
                Point position = new Point(cursorPosition.X + 4, cursorPosition.Y - 20);
                this.StatsForm.ShowTooltip(((MetroTrackBar)sender).Value.ToString(), this, position);
            } else {
                Point position = new Point(this.trkOverlayOpacity.Location.X + 220 + (this.trkOverlayOpacity.Width * ((MetroTrackBar)sender).Value / 102), this.trkOverlayOpacity.Location.Y + 74);
                this.StatsForm.ShowTooltip(((MetroTrackBar)sender).Value.ToString(), this, position, 1500);
            }
            
        }
        private void trkOverlayOpacity_MouseEnter(object sender, EventArgs e) {
            this.TrkOverlayOpacityIsEnter = true;
        }
        private void trkOverlayOpacity_MouseLeave(object sender, EventArgs e) {
            this.TrkOverlayOpacityIsEnter = false;
            this.StatsForm.HideTooltip(this);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Tab) { SendKeys.Send("%"); }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeLanguage(int lang) {
            this.DisplayLang = lang;
            this.Font = Overlay.GetMainFont(12, FontStyle.Regular, this.DisplayLang);
            int tempLanguage = Stats.CurrentLanguage;
            Stats.CurrentLanguage = lang;

            this.Text = $"     {Multilingual.GetWord("settings_title")}";
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

            //this.lblGameExeLocation.Text = Multilingual.GetWord(this.LaunchPlatform == 0 ? "settings_fall_guys_shortcut_location" : "settings_fall_guys_exe_location");

            this.tileProgram.Text = Multilingual.GetWord("settings_program");
            this.tileDisplay.Text = Multilingual.GetWord("settings_display");
            this.tileOverlay.Text = Multilingual.GetWord("settings_overlay");
            this.tileFallGuys.Text = Multilingual.GetWord("settings_launch_fallguys");
            this.tileFallalytics.Text = Multilingual.GetWord("settings_fallalytics");
            this.tileAbout.Text = Multilingual.GetWord("settings_about");

            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(20, 16) : new Point(19, 14);

            this.lblTheme.Text = Multilingual.GetWord("settings_theme");
            this.cboTheme.Items.Clear();
            this.cboTheme.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_theme_light"),
                Multilingual.GetWord("settings_theme_dark"),
            });
            //switch (this.CurrentSettings.Theme) {
            //    case 0: this.cboTheme.SelectedItem = Multilingual.GetWord("settings_theme_light"); break;
            //    case 1: this.cboTheme.SelectedItem = Multilingual.GetWord("settings_theme_dark"); break;
            //}
            this.cboTheme.SelectedItem = this.Theme == MetroThemeStyle.Light
                ? Multilingual.GetWord("settings_theme_light")
                : Multilingual.GetWord("settings_theme_dark");

            this.lblLogPath.Text = Multilingual.GetWord("settings_log_path");
            this.lblLogPathNote.Text = Multilingual.GetWord("settings_log_path_description");
            this.btnSave.Text = Multilingual.GetWord("settings_save");
            //this.grpOverlay.Text = Multilingual.GetWord("settings_overlay");
            this.chkOnlyShowGold.Text = Multilingual.GetWord("settings_gold_only");
            this.chkOnlyShowQualify.Text = Multilingual.GetWord("settings_qualify_only");
            this.chkCycleQualifyGold.Text = Multilingual.GetWord("settings_cycle_qualify__gold");
            this.chkOnlyShowLongest.Text = Multilingual.GetWord("settings_personal_lowest_only");
            this.chkOnlyShowFastest.Text = Multilingual.GetWord("settings_personal_best_only");
            this.chkCycleFastestLongest.Text = Multilingual.GetWord("settings_cycle_best__lowest");
            this.chkHidePercentages.Text = Multilingual.GetWord("settings_hide_percentages");
            this.chkHideWinsInfo.Text = Multilingual.GetWord("settings_hide_wins_info");


            this.cboOverlayColor.Items.Clear();
            this.cboOverlayColor.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_transparent"),
                Multilingual.GetWord("settings_white"),
                Multilingual.GetWord("settings_black"),
                Multilingual.GetWord("settings_magenta"),
                Multilingual.GetWord("settings_red"),
                Multilingual.GetWord("settings_green"),
                Multilingual.GetWord("settings_blue")
            });
            switch (this.CurrentSettings.OverlayColor) {
                case 0: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_transparent"); break;
                case 1: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_white"); break;
                case 2: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_black"); break;
                case 3: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_magenta"); break;
                case 4: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_red"); break;
                case 5: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_green"); break;
                case 6: this.cboOverlayColor.SelectedItem = Multilingual.GetWord("settings_blue"); break;
            }

            this.lblOverlayBackground.Text = Multilingual.GetWord("settings_background_image");
            this.lblOverlayColor.Text = Multilingual.GetWord("settings_background");
            this.lblOverlayOpacity.Text = Multilingual.GetWord("settings_background_opacity");
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
            this.chkShadeTheFlagImage.Text = Multilingual.GetWord("settings_shade_the_flag_image");
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
            //this.grpStats.Text = Multilingual.GetWord("settings_stats");
            this.chkChangeHoopsieLegends.Text = Multilingual.GetWord("settings_rename_hoopsie_legends_to_hoopsie_heroes");
            this.chkAutoUpdate.Text = Multilingual.GetWord("settings_auto_update_program");
            this.chkSystemTrayIcon.Text = Multilingual.GetWord("settings_system_tray_icon");
            this.chkNotifyServerConnected.Text = Multilingual.GetWord("settings_notify_server_connected");
            this.chkPreventOverlayMouseClicks.Text = Multilingual.GetWord("settings_prevent_overlay_mouse_clicks");
            this.lblPreviousWinsNote.Text = Multilingual.GetWord("settings_before_using_tracker");
            this.lblPreviousWins.Text = Multilingual.GetWord("settings_previous_win");
            this.grpLaunchPlatform.Text = Multilingual.GetWord("settings_game_options_platform");
            this.grpLaunchPlatform.Width = lang == 3 ? 120 : 95;
            //this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
            this.btnGameExeLocationBrowse.Text = Multilingual.GetWord("settings_browse");
            this.chkLaunchGameOnStart.Text = Multilingual.GetWord("settings_launch_fall_guys_on_tracker_launch");
            //this.grpSortingOptions.Text = Multilingual.GetWord("settings_sorting_options");
            this.chkIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            this.chkGroupingCreativeRoundLevels.Text = Multilingual.GetWord("settings_grouping_creative_round_levels");
            //this.lblLanguageSelection.Text = Multilingual.GetWord("settings_language");
            this.btnCancel.Text = Multilingual.GetWord("settings_cancel");

            this.txtLogPath.Location = new Point(this.lblLogPath.Location.X + this.lblLogPath.Width + 4, 12);
            this.txtLogPath.Size = new Size(630 - this.lblLogPath.Width - 4, 22);
            this.txtPreviousWins.Location = new Point(this.lblPreviousWins.Location.X + this.lblPreviousWins.Width + 4, 12);
            this.lblPreviousWinsNote.Location = new Point(this.txtPreviousWins.Location.X + this.txtPreviousWins.Width + 4, 12);
            this.cboTheme.Location = new Point(this.lblTheme.Location.X + this.lblTheme.Width + 4, 200);
            //this.cboMultilingual.Location = new Point(this.lblLanguage.Location.X + this.lblLanguage.Width + 4, 162);
            //this.lblWinsFilter.Location = new Point(389 - this.lblWinsFilter.Width, 19);
            //this.lblQualifyFilter.Location = new Point(389 - this.lblQualifyFilter.Width, 52);
            //this.lblFastestFilter.Location = new Point(389 - this.lblFastestFilter.Width, 85);
            this.txtCycleTimeSeconds.Location = new Point(this.lblCycleTimeSeconds.Location.X + this.lblCycleTimeSeconds.Width + 4, 170);
            this.lblCycleTimeSecondsTag.Location = new Point(this.txtCycleTimeSeconds.Location.X + this.txtCycleTimeSeconds.Width + 4, 170);
            //this.lblOverlayBackground.Location = new Point(15, 353);
            //this.lblOverlayColor.Location = new Point(15, 370);
            if (this.LaunchPlatform == 0) {
                this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location");
                this.txtGameShortcutLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                this.txtGameShortcutLocation.Size = new Size(567 - this.txtGameShortcutLocation.Location.X, 25);
            } else {
                this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location");
                this.txtGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                this.txtGameExeLocation.Size = new Size(567 - this.txtGameExeLocation.Location.X, 25);
            }
            this.chkFallalyticsReporting.Text = Multilingual.GetWord("settings_sends_info_about_rounds_played_to_fallalytics");
            this.lblFallalyticsAPIKey.Text = Multilingual.GetWord("settings_enter_fallalytics_api_key");
            this.lblFallalyticsDesc.Text = Multilingual.GetWord("settings_fallalytics_desc");
            this.linkFallalytics.Text = $@"    {Multilingual.GetWord("settings_visit_fallalytics")}";
            
            this.fglink1.Text = Multilingual.GetWord("settings_github");
            this.fglink2.Text = $"{Multilingual.GetWord("settings_issue_traker")} && {Multilingual.GetWord("settings_translation")}";
            this.btnCheckUpdates.Text = Multilingual.GetWord("main_update");
#if AllowUpdate
            this.lblVersion.Text = $"{Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
#else
            this.lblVersion.Text = $"{Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)} ({Multilingual.GetWord("main_manual_update_version")})";
            this.chkAutoUpdate.Visible = false;
#endif
            Stats.CurrentLanguage = tempLanguage;
            this.Invalidate(true);
        }

        private void ChangeTab(object sender, EventArgs e) {
            this.panelProgram.Location = new Point(211, 75);
            this.panelDisplay.Location = new Point(211, 75);
            this.panelOverlay.Location = new Point(211, 75);
            this.panelFallGuys.Location = new Point(211, 75);
            this.panelAbout.Location = new Point(211, 75);
            this.panelFallalytics.Location = new Point(211, 75);
            this.panelProgram.Visible = false;
            this.panelDisplay.Visible = false;
            this.panelOverlay.Visible = false;
            this.panelFallGuys.Visible = false;
            this.panelAbout.Visible = false;
            this.panelFallalytics.Visible = false;
            this.tileProgram.Style = MetroColorStyle.Silver;
            this.tileDisplay.Style = MetroColorStyle.Silver;
            this.tileOverlay.Style = MetroColorStyle.Silver;
            this.tileFallGuys.Style = MetroColorStyle.Silver;
            this.tileAbout.Style = MetroColorStyle.Silver;
            this.tileFallalytics.Style = MetroColorStyle.Silver;
            if (sender.Equals(this.tileProgram)) {
                this.tileProgram.Style = MetroColorStyle.Teal;
                this.panelProgram.Visible = true;
            }
            if (sender.Equals(this.tileDisplay)) {
                this.tileDisplay.Style = MetroColorStyle.Teal;
                this.panelDisplay.Visible = true;
            }
            if (sender.Equals(this.tileOverlay)) {
                this.tileOverlay.Style = MetroColorStyle.Teal;
                this.panelOverlay.Visible = true;
            }
            if (sender.Equals(this.tileFallGuys)) {
                this.tileFallGuys.Style = MetroColorStyle.Teal;
                this.panelFallGuys.Visible = true;
            }
            if (sender.Equals(this.tileAbout)) {
                this.tileAbout.Style = MetroColorStyle.Teal;
                this.panelAbout.Visible = true;
            }
            if (sender.Equals(this.tileFallalytics)) {
                this.tileFallalytics.Style = MetroColorStyle.Teal;
                this.panelFallalytics.Visible = true;
            }
            this.Refresh();

            if (sender.Equals(this.tileAbout)) {
#if AllowUpdate
                this.lblupdateNote.UseCustomForeColor = false;
                this.lblupdateNote.Text = $"Checking for updates...";
                using (ZipWebClient web = new ZipWebClient()) {
                    string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                    int index = assemblyInfo.IndexOf("AssemblyVersion(");
                    if (index > 0) {
                        int indexEnd = assemblyInfo.IndexOf("\")", index);
                        Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                        Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                        if (newVersion > currentVersion) {
                            this.lblupdateNote.Text = $"{Multilingual.GetWordWithLang("settings_new_update_prefix", this.DisplayLang)} v{newVersion} {Multilingual.GetWordWithLang("settings_new_update_suffix", this.DisplayLang)}";
                            this.lblupdateNote.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.LimeGreen : Color.LightGreen;
                            this.lblupdateNote.UseCustomForeColor = true;
                            this.lblupdateNote.Location = new Point(this.btnCheckUpdates.Location.X + this.btnCheckUpdates.Width + 5, 92);
                            this.btnCheckUpdates.Visible = true;
                            this.StatsForm.ChangeStateForAvailableNewVersion(newVersion.ToString(2));
                        } else {
                            this.lblupdateNote.Text = $"{Multilingual.GetWordWithLang("message_update_latest_version", this.DisplayLang)}{Environment.NewLine}{Environment.NewLine}{Multilingual.GetWordWithLang("main_update_prefix_tooltip", this.DisplayLang).Trim()}{Environment.NewLine}{Multilingual.GetWordWithLang("main_update_suffix_tooltip", this.DisplayLang).Trim()}";
                        }
                    } else {
                        this.lblupdateNote.Text = Multilingual.GetWordWithLang("message_update_not_determine_version", this.DisplayLang);
                        this.lblupdateNote.ForeColor = Color.Red;
                        this.lblupdateNote.UseCustomForeColor = true;
                    }
                }
#else
                 this.lblupdateNote.Text = $"{Multilingual.GetWordWithLang("main_update_prefix_tooltip", this.DisplayLang).Trim()}{Environment.NewLine}{Multilingual.GetWordWithLang("main_update_suffix_tooltip", this.DisplayLang).Trim()}";
#endif
            }
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.fglink1)) {
                this.openLink(@"https://github.com/ShootMe/FallGuysStats");
            }
            if (sender.Equals(this.fglink2)) {
                this.openLink(@"https://github.com/ShootMe/FallGuysStats/issues");
            }
            if (sender.Equals(this.lbltpl1)) {
                this.openLink(@"https://github.com/Fody/Costura/blob/develop/LICENSE");
            }
            if (sender.Equals(this.lbltpl2)) {
                this.openLink(@"https://github.com/Fody/Home/blob/master/license.txt");
            }
            if (sender.Equals(this.lbltpl3)) {
                this.openLink(@"https://github.com/dennismagno/metroframework-modern-ui/blob/master/LICENSE.md");
            }
            if (sender.Equals(this.lbltpl4)) {
                this.openLink(@"https://github.com/ScottPlot/ScottPlot/blob/main/LICENSE");
            }
            if (sender.Equals(this.linkFallalytics)) {
                this.openLink(@"https://fallalytics.com/");
            }
        }
        private void openLink(string link) {
            try {
                Process.Start(link);
            } catch (Exception ex) {
                MetroMessageBox.Show(this, ex.Message, $"{Multilingual.GetWord("message_program_error_caption")}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e) {
#if AllowUpdate
             using (ZipWebClient web = new ZipWebClient()) {
                 string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");
                 int index = assemblyInfo.IndexOf("AssemblyVersion(");
                 if (index > 0) {
                     int indexEnd = assemblyInfo.IndexOf("\")", index);
                     Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                     Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                     if (newVersion > currentVersion) {
                         if (MetroMessageBox.Show(this,
                                 $"{Multilingual.GetWord("message_update_question_prefix")} [ v{newVersion.ToString(2)} ] {Multilingual.GetWord("message_update_question_suffix")}",
                                 $"{Multilingual.GetWord("message_update_question_caption")}",
                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                         {
                             this.Hide();
                             this.StatsForm.CurrentSettings.ShowChangelog = true;
                             this.StatsForm.SaveWindowState();
                             this.StatsForm.SaveUserSettings();
                             this.StatsForm.Hide();
                             this.StatsForm.overlay?.Hide();
                             using (DownloadProgress progress = new DownloadProgress()) {
                                 this.StatsForm.StatsDB?.Dispose();
                                 progress.ZipWebClient = web;
                                 progress.DownloadUrl = this.StatsForm.FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL;
                                 progress.FileName = "FallGuysStats.zip";
                                 progress.ShowDialog(this);
                             }
                             this.StatsForm.isUpdate = true;
                             this.StatsForm.Stats_ExitProgram(this, null);
                         }
                     } else {
                         MetroMessageBox.Show(this,
                             $"{Multilingual.GetWord("message_update_latest_version")}" +
                             $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}" +
                             $"{Multilingual.GetWord("main_update_prefix_tooltip").Trim()}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip").Trim()}",
                             $"{Multilingual.GetWord("message_update_question_caption")}",
                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                 } else {
                     MetroMessageBox.Show(this, $"{Multilingual.GetWord("message_update_not_determine_version")}", $"{Multilingual.GetWord("message_update_error_caption")}",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
             }
#endif
        }
    }
}
