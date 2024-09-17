using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class Settings : MetroFramework.Forms.MetroForm {
        private string overlayFontSerialized = string.Empty;
        private string overlayFontColorSerialized = string.Empty;
        public UserSettings CurrentSettings { get; set; }
        public Stats StatsForm { get; set; }
        public Overlay Overlay { get; set; }
        private int LaunchPlatform;
        private Language DisplayLang;
        // private bool CboMultilingualIsFocus, CboOverlayBackgroundIsFocus;
        private bool TrkOverlayOpacityIsEnter;
        private string PrevTabName;
        private bool IsSucceededTestProxy;
        
        public Settings() {
            this.InitializeComponent();
            this.Opacity = 0;
            this.cboNotificationSounds.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboNotificationWindowPosition.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboNotificationWindowAnimation.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboMultilingual.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboTheme.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboWinsFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboQualifyFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboFastestFilter.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboOverlayBackground.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
            this.cboOverlayColor.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
        }

        private void Settings_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);

            this.ChangeTab(this.tileProgram, null);
            
            this.LaunchPlatform = this.CurrentSettings.LaunchPlatform;
            this.DisplayLang = Stats.CurrentLanguage;
            this.ChangeLanguage(Stats.CurrentLanguage);
            
            List<ImageItem> flagItemArray = new List<ImageItem> {
                new ImageItem(Properties.Resources.country_us_shiny_icon, "English", Overlay.GetMainFont(14f)),
                new ImageItem(Properties.Resources.country_fr_shiny_icon, "Français", Overlay.GetMainFont(14f)),
                new ImageItem(Properties.Resources.country_kr_shiny_icon, "한국어", Overlay.GetMainFont(14f)),
                new ImageItem(Properties.Resources.country_jp_shiny_icon, "日本語", Overlay.GetMainFont(14f)),
                new ImageItem(Properties.Resources.country_cn_shiny_icon, "简体中文", Overlay.GetMainFont(14f)),
                new ImageItem(Properties.Resources.country_cn_shiny_icon, "繁體中文", Overlay.GetMainFont(14f)),
            };
            this.cboMultilingual.SetImageItemData(flagItemArray);
            this.cboMultilingual.SelectedIndex = (int)Stats.CurrentLanguage;
            
            this.txtLogPath.Text = this.CurrentSettings.LogPath;

            if (this.CurrentSettings.SwitchBetweenLongest) {
                this.rdoCycleFastestLongest.Checked = true;
            } else if (this.CurrentSettings.OnlyShowLongest) {
                this.rdoOnlyShowLongest.Checked = true;
            } else {
                this.rdoOnlyShowFastest.Checked = true;
            }
            
            if (this.CurrentSettings.SwitchBetweenQualify) {
                this.rdoCycleQualifyGold.Checked = true;
            } else if (this.CurrentSettings.OnlyShowGold) {
                this.rdoOnlyShowGold.Checked = true;
            } else {
                this.rdoOnlyShowQualify.Checked = true;
            }
            
            if (this.CurrentSettings.SwitchBetweenPlayers) {
                this.rdoCyclePlayersPing.Checked = true;
            } else if (this.CurrentSettings.OnlyShowPing) {
                this.rdoOnlyShowPing.Checked = true;
            } else {
                this.rdoOnlyShowPlayers.Checked = true;
            }
            
            if (this.CurrentSettings.SwitchBetweenStreaks) {
                this.rdoCycleWinFinalStreak.Checked = true;
            } else if (this.CurrentSettings.OnlyShowFinalStreak) {
                this.rdoOnlyShowFinalStreak.Checked = true;
            } else {
                this.rdoOnlyShowWinStreak.Checked = true;
            }

            this.txtCycleTimeSeconds.Text = this.CurrentSettings.CycleTimeSeconds.ToString();
            this.txtPreviousWins.Text = this.CurrentSettings.PreviousWins.ToString();
            this.chkOverlayOnTop.Checked = !this.CurrentSettings.OverlayNotOnTop;
            this.chkPlayerByConsoleType.Checked = this.CurrentSettings.PlayerByConsoleType;
            this.chkColorByRoundType.Checked = this.CurrentSettings.ColorByRoundType;
            this.chkAutoChangeProfile.Checked = this.CurrentSettings.AutoChangeProfile;
            this.chkShadeTheFlagImage.Checked = this.CurrentSettings.ShadeTheFlagImage;
            this.chkDisplayCurrentTime.Checked = this.CurrentSettings.DisplayCurrentTime;
            this.chkDisplayGamePlayedInfo.Checked = this.CurrentSettings.DisplayGamePlayedInfo;
            this.chkCountPlayersDuringTheLevel.Checked = this.CurrentSettings.CountPlayersDuringTheLevel;
            this.chkHideWinsInfo.Checked = this.CurrentSettings.HideWinsInfo;
            this.chkHideRoundInfo.Checked = this.CurrentSettings.HideRoundInfo;
            this.chkHideTimeInfo.Checked = this.CurrentSettings.HideTimeInfo;
            this.chkShowTabs.Checked = this.CurrentSettings.ShowOverlayTabs;
            //this.chkShowProfile.Checked = this.CurrentSettings.ShowOverlayProfile;
            this.chkAutoUpdate.Checked = this.CurrentSettings.AutoUpdate;
            this.chkSystemTrayIcon.Checked = this.CurrentSettings.SystemTrayIcon;
            
            this.chkNotifyServerConnected.Checked = this.CurrentSettings.NotifyServerConnected;
            this.chkNotifyPersonalBest.Checked = this.CurrentSettings.NotifyPersonalBest;
            this.cboNotificationSounds.SelectedIndex = this.CurrentSettings.NotificationSounds;
            this.cboNotificationSounds.Enabled = this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked;
            this.cboNotificationWindowPosition.SelectedIndex = this.CurrentSettings.NotificationWindowPosition;
            this.cboNotificationWindowPosition.Enabled = this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked;
            this.cboNotificationWindowAnimation.SelectedIndex = this.CurrentSettings.NotificationWindowAnimation;
            this.cboNotificationWindowAnimation.Enabled = this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked;
            this.mlPlayNotificationSounds.Enabled = this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked;
            this.chkMuteNotificationSounds.Enabled = this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked;
            if (this.chkNotifyServerConnected.Checked || this.chkNotifyPersonalBest.Checked) {
                this.chkMuteNotificationSounds.Checked = this.CurrentSettings.MuteNotificationSounds;
            }
            
            this.chkPreventOverlayMouseClicks.Checked = this.CurrentSettings.PreventOverlayMouseClicks;
            this.chkFlipped.Checked = this.CurrentSettings.FlippedDisplay;
            this.chkHidePercentages.Checked = this.CurrentSettings.HideOverlayPercentages;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;
            this.chkFallalyticsReporting.Checked = this.CurrentSettings.EnableFallalyticsReporting;
            this.chkFallalyticsWeeklyCrownLeague.Checked = this.CurrentSettings.EnableFallalyticsWeeklyCrownLeague;
            this.chkFallalyticsAnonymous.Enabled = this.chkFallalyticsReporting.Checked || this.chkFallalyticsWeeklyCrownLeague.Checked;
            if (this.chkFallalyticsReporting.Checked || this.chkFallalyticsWeeklyCrownLeague.Checked) {
                this.chkFallalyticsAnonymous.Checked = this.CurrentSettings.EnableFallalyticsAnonymous;
            }
            this.txtFallalyticsAPIKey.Text = this.CurrentSettings.FallalyticsAPIKey;
            
            this.chkUseProxy.Checked = this.CurrentSettings.UseProxyServer;
            this.txtProxyAddress.Text = this.CurrentSettings.ProxyAddress;
            this.txtProxyPort.Text = this.CurrentSettings.ProxyPort;
            this.chkUseProxyLoginRequired.Checked = this.CurrentSettings.EnableProxyAuthentication;
            this.txtProxyUsername.Text = this.CurrentSettings.ProxyUsername;
            this.txtProxyPassword.Text = this.CurrentSettings.ProxyPassword;
            this.IsSucceededTestProxy = this.CurrentSettings.SucceededTestProxy;

            List<ImageItem> overlayItemArray = new List<ImageItem>();
            if (Directory.Exists("Overlay")) {
                DirectoryInfo di = new DirectoryInfo("Overlay");
                foreach (FileInfo file in di.GetFiles()) {
                    if (string.Equals(file.Name, "background.png") || string.Equals(file.Name, "tab.png")) continue;
                    if (file.Name.StartsWith("background_") && file.Name.EndsWith(".png")) {
                        string backgroundName = file.Name.Substring(11);
                        backgroundName = backgroundName.Remove(backgroundName.Length - 4);
                        Bitmap background = new Bitmap(file.FullName);
                        if (background.Width == 786 && background.Height == 99) {
                            overlayItemArray.Add(new ImageItem(background, backgroundName, this.Font, new[] { $"background_{backgroundName}", $"tab_{backgroundName}" }, true));
                        }
                    }
                }
            }

            int customizedBacgroundCount = overlayItemArray.Count;
            ImageItem[] overlayItems = {
                new ImageItem(Properties.Resources.background, "Default", this.Font, new[] { "background", "tab_unselected" }),
                new ImageItem(Properties.Resources.background_monarch, "Monarch", this.Font, new[] { "background_monarch", "tab_unselected_monarch" }),
                new ImageItem(Properties.Resources.background_candycane, "Candy Cane", this.Font, new[] { "background_candycane", "tab_unselected_candycane" }),
                new ImageItem(Properties.Resources.background_coffee, "Coffee", this.Font, new[] { "background_coffee", "tab_unselected_coffee" }),
                new ImageItem(Properties.Resources.background_dove, "Dove", this.Font, new[] { "background_dove", "tab_unselected_dove" }),
                new ImageItem(Properties.Resources.background_fall_guys_logo, "Fall Guys Logo", this.Font, new[] { "background_fall_guys_logo", "tab_unselected_fall_guys_logo" }),
                new ImageItem(Properties.Resources.background_helter_skelter, "Helter Skelter", this.Font, new[] { "background_helter_skelter", "tab_unselected_helter_skelter" }),
                new ImageItem(Properties.Resources.background_hex_a_thon, "Hex A Thon", this.Font, new[] { "background_hex_a_thon", "tab_unselected_hex_a_thon" }),
                new ImageItem(Properties.Resources.background_ill_be_slime, "I'll Be Slime", this.Font, new[] { "background_ill_be_slime", "tab_unselected_ill_be_slime" }),
                new ImageItem(Properties.Resources.background_mockingbird, "Mocking Bird", this.Font, new[] { "background_mockingbird", "tab_unselected_mockingbird" }),
                new ImageItem(Properties.Resources.background_newlove, "New Love", this.Font, new[] { "background_newlove", "tab_unselected_newlove" }),
                new ImageItem(Properties.Resources.background_parade_guy, "Parade Guy", this.Font, new[] { "background_parade_guy", "tab_unselected_parade_guy" }),
                new ImageItem(Properties.Resources.background_party_pegwin, "Party Pegwin", this.Font, new[] { "background_party_pegwin", "tab_unselected_party_pegwin" }),
                new ImageItem(Properties.Resources.background_penguin, "Penguin", this.Font, new[] { "background_penguin", "tab_unselected_penguin" }),
                new ImageItem(Properties.Resources.background_suits_you, "Suits You", this.Font, new[] { "background_suits_you", "tab_unselected_suits_you" }),
                new ImageItem(Properties.Resources.background_sunny_guys, "Sunny Guys", this.Font, new[] { "background_sunny_guys", "tab_unselected_sunny_guys" }),
                new ImageItem(Properties.Resources.background_ta_da, "Ta Da", this.Font, new[] { "background_ta_da", "tab_unselected_ta_da" }),
                new ImageItem(Properties.Resources.background_timeattack, "Time Attack", this.Font, new[] { "background_timeattack", "tab_unselected_timeattack" }),
                new ImageItem(Properties.Resources.background_watermelon, "Watermelon", this.Font, new[] { "background_watermelon", "tab_unselected_watermelon" }),
                new ImageItem(Properties.Resources.background_super_mario_bros, "Super Mario Bros.", this.Font, new[] { "background_super_mario_bros", "tab_unselected_super_mario_bros" }),
                new ImageItem(Properties.Resources.background_super_mario_bros_3, "Super Mario Bros. 3", this.Font, new[] { "background_super_mario_bros_3", "tab_unselected_super_mario_bros_3" }),
                new ImageItem(Properties.Resources.background_wallpaper_01, "Wallpaper 01", this.Font, new[] { "background_wallpaper_01", "tab_unselected_wallpaper_01" }),
                new ImageItem(Properties.Resources.background_wallpaper_02, "Wallpaper 02", this.Font, new[] { "background_wallpaper_02", "tab_unselected_wallpaper_02" }),
                new ImageItem(Properties.Resources.background_wallpaper_03, "Wallpaper 03", this.Font, new[] { "background_wallpaper_03", "tab_unselected_wallpaper_03" })
            };
            overlayItemArray.AddRange(overlayItems);
            this.cboOverlayBackground.SetImageItemData(overlayItemArray);
            bool isSelected = false;
            if (this.CurrentSettings.IsOverlayBackgroundCustomized) {
                for (int i = 0; i < overlayItemArray.Count; i++) {
                    if (!string.Equals(overlayItemArray[i].Data[0], this.CurrentSettings.OverlayBackgroundResourceName)) { continue; }
                    this.cboOverlayBackground.SelectedIndex = i;
                    isSelected = true;
                    break;
                }
            } else {
                for (int i = overlayItemArray.Count - 1; i >= 0; i--) {
                    if (!string.Equals(overlayItemArray[i].Data[0], this.CurrentSettings.OverlayBackgroundResourceName)) { continue; }
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
            this.chkRecordEscapeDuringAGame.Checked = this.CurrentSettings.RecordEscapeDuringAGame;

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
        
        private void Settings_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            this.BackImage = theme == MetroThemeStyle.Light ? Properties.Resources.setting_icon : Properties.Resources.setting_gray_icon;
            foreach (Control c1 in Controls) {
                if (c1 is MetroLabel ml1) {
                    ml1.Theme = theme;
                } else if (c1 is MetroTextBox mtb1) {
                    mtb1.Theme = theme;
                } else if (c1 is MetroButton mb1) {
                    mb1.Theme = theme;
                } else if (c1 is MetroCheckBox mcb1) {
                    mcb1.Theme = theme;
                    if (mcb1.UseCustomForeColor && !mcb1.Checked) {
                        mcb1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    }
                } else if (c1 is MetroRadioButton mrb1) {
                    mrb1.Theme = theme;
                } else if (c1 is MetroComboBox mcbo1) {
                    mcbo1.Theme = theme;
                } else if (c1 is MetroPanel mp1) {
                    mp1.Theme = theme;
                    foreach (Control c2 in mp1.Controls) {
                        if (c2 is MetroLabel ml2) {
                            ml2.Theme = theme;
                        } else if (c2 is MetroTextBox mtb2) {
                            mtb2.Theme = theme;
                        } else if (c2 is MetroButton mb2) {
                            mb2.Theme = theme;
                        } else if (c2 is MetroCheckBox mcb2) {
                            mcb2.Theme = theme;
                            if (mcb2.UseCustomForeColor && mcb2.Checked) {
                                mcb2.ForeColor = theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow;
                            } else {
                                mcb2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            }
                        } else if (c2 is MetroRadioButton mrb2) {
                            mrb2.Theme = theme;
                        } else if (c2 is MetroComboBox mcbo2) {
                            mcbo2.Theme = theme;
                        } else if (c2 is ImageComboBox icbo2) {
                            icbo2.Theme = theme;
                        } else if (c2 is MetroLink mlnk2) {
                            mlnk2.Theme = theme;
                        } else if (c2 is MetroTrackBar mtrb2) {
                            mtrb2.Theme = theme;
                        } else if (c2 is MetroProgressSpinner mps2) {
                            mps2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(17, 17, 17);
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
            this.ResumeLayout();
            this.Refresh();
        }

        private void mlPlayNotificationSounds_Click(object sender, EventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                Image flagImage = (Image)Properties.Resources.ResourceManager.GetObject($"country_kr{(this.CurrentSettings.ShadeTheFlagImage ? "_shiny" : "")}_icon");
                ToastPosition toastPosition = Enum.TryParse(this.cboNotificationWindowPosition.SelectedIndex.ToString(), out ToastPosition position) ? position : ToastPosition.BottomRight;
                ToastAnimation toastAnimation = this.cboNotificationWindowAnimation.SelectedIndex == 0 ? ToastAnimation.FADE : ToastAnimation.SLIDE;
                ToastTheme toastTheme = this.Theme == MetroThemeStyle.Light ? ToastTheme.Light : ToastTheme.Dark;
                ToastSound toastSound = Enum.TryParse(this.cboNotificationSounds.SelectedIndex.ToString(), out ToastSound sound) ? sound : ToastSound.Generic01;
                
                this.StatsForm.ShowToastNotification(this.StatsForm, Properties.Resources.main_120_icon, Multilingual.GetWord("message_test_notifications_caption", this.DisplayLang), "MADE BY Qubit Guy@eunma A.K.A. 제임스 웹 우주 망원경",
                    Overlay.GetMainFont(16, FontStyle.Bold, this.DisplayLang), flagImage, ToastDuration.VERY_SHORT, toastPosition, toastAnimation, toastTheme, toastSound, this.chkMuteNotificationSounds.Checked, true);
            });
        }
        
        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e) {
            this.SetTheme(((ComboBox)sender).SelectedIndex == 0 ? MetroThemeStyle.Light : ((ComboBox)sender).SelectedIndex == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
        }
        
        // private void chkFallalyticsReporting_CheckedChanged(object sender, EventArgs e) {
        //     this.chkFallalyticsAnonymous.Enabled = ((MetroCheckBox)sender).Checked;
        //     if (!((MetroCheckBox)sender).Checked) {
        //         this.chkFallalyticsAnonymous.Checked = ((MetroCheckBox)sender).Checked;
        //     }
        // }
        
        // private void chkNotify_CheckedChanged(object sender, EventArgs e) {
        //     this.chkMuteNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
        //     this.cboNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
        //     this.cboNotificationWindowPosition.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
        //     this.cboNotificationWindowAnimation.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
        //     this.mlPlayNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
        //     if (!this.chkNotifyPersonalBest.Checked && !this.chkNotifyServerConnected.Checked) {
        //         this.chkMuteNotificationSounds.Checked = false;
        //     }
        // }
        
        private void CheckBox_CheckedChanged(object sender, EventArgs e) {
            ((MetroCheckBox)sender).ForeColor = ((MetroCheckBox)sender).Checked ? (this.Theme == MetroThemeStyle.Light ? Color.FromArgb(0, 174, 219) : Color.GreenYellow) : Color.DimGray;

            if (sender.Equals(this.chkNotifyPersonalBest) || sender.Equals(this.chkNotifyServerConnected)) {
                this.chkMuteNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
                this.cboNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
                this.cboNotificationWindowPosition.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
                this.cboNotificationWindowAnimation.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
                this.mlPlayNotificationSounds.Enabled = this.chkNotifyPersonalBest.Checked || this.chkNotifyServerConnected.Checked;
                if (!this.chkNotifyPersonalBest.Checked && !this.chkNotifyServerConnected.Checked) {
                    this.chkMuteNotificationSounds.Checked = false;
                }
            } else if (sender.Equals(this.chkFallalyticsReporting) || sender.Equals(this.chkFallalyticsWeeklyCrownLeague)) {
                this.chkFallalyticsAnonymous.Enabled = this.chkFallalyticsReporting.Checked || this.chkFallalyticsWeeklyCrownLeague.Checked;
                this.txtFallalyticsAPIKey.Enabled = this.chkFallalyticsReporting.Checked || this.chkFallalyticsWeeklyCrownLeague.Checked;
                if (!this.chkFallalyticsReporting.Checked && !this.chkFallalyticsWeeklyCrownLeague.Checked) {
                    this.chkFallalyticsAnonymous.Checked = false;
                }
            } else if (sender.Equals(this.chkUseProxy)) {
                this.IsSucceededTestProxy = false;
                this.mpsProxySpinner.Visible = false;
                this.picProxyTextResult.Visible = false;
                this.txtProxyAddress.Enabled = this.chkUseProxy.Checked;
                this.txtProxyPort.Enabled = this.chkUseProxy.Checked;
                this.chkUseProxyLoginRequired.Enabled = this.chkUseProxy.Checked;
                this.txtProxyUsername.Enabled = this.chkUseProxyLoginRequired.Checked;
                this.txtProxyPassword.Enabled = this.chkUseProxyLoginRequired.Checked;
                this.btnProxyTestConnection.Enabled = this.chkUseProxy.Checked;
            } else if (sender.Equals(this.chkUseProxyLoginRequired)) {
                this.IsSucceededTestProxy = false;
                this.mpsProxySpinner.Visible = false;
                this.picProxyTextResult.Visible = false;
                this.txtProxyUsername.Enabled = this.chkUseProxyLoginRequired.Checked;
                this.txtProxyPassword.Enabled = this.chkUseProxyLoginRequired.Checked;
            }
        }
        
        private void CheckBox_MouseEnter(object sender, EventArgs e) {
            if (!((MetroCheckBox)sender).Checked) ((MetroCheckBox)sender).ForeColor = Color.DimGray;
        }
        
        private void CheckBox_MouseLeave(object sender, EventArgs e) {
            if (!((MetroCheckBox)sender).Checked) ((MetroCheckBox)sender).ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            this.CurrentSettings.LogPath = this.txtLogPath.Text;
            Stats.CurrentLanguage = (Language)this.cboMultilingual.SelectedIndex;
            this.CurrentSettings.Multilingual = this.cboMultilingual.SelectedIndex;
            this.CurrentSettings.Theme = this.cboTheme.SelectedIndex;
            Stats.CurrentTheme = this.cboTheme.SelectedIndex == 0 ? MetroThemeStyle.Light :
                this.cboTheme.SelectedIndex == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default;

            if (string.IsNullOrEmpty(this.txtCycleTimeSeconds.Text)) {
                this.CurrentSettings.CycleTimeSeconds = 5;
            } else {
                this.CurrentSettings.CycleTimeSeconds = int.TryParse(this.txtCycleTimeSeconds.Text, out int cycleTimeSeconds) ? cycleTimeSeconds : 5;
                if (this.CurrentSettings.CycleTimeSeconds <= 0) {
                    this.CurrentSettings.CycleTimeSeconds = 5;
                }
            }

            if (string.IsNullOrEmpty(this.txtPreviousWins.Text)) {
                this.CurrentSettings.PreviousWins = 0;
            } else {
                this.CurrentSettings.PreviousWins = int.TryParse(this.txtPreviousWins.Text, out int previousWins) ? previousWins : 0;
                if (this.CurrentSettings.PreviousWins < 0) {
                    this.CurrentSettings.PreviousWins = 0;
                }
            }

            if (this.rdoCycleFastestLongest.Checked) {
                this.CurrentSettings.SwitchBetweenLongest = true;
                this.CurrentSettings.OnlyShowLongest = false;
            } else if (this.rdoOnlyShowLongest.Checked) {
                this.CurrentSettings.SwitchBetweenLongest = false;
                this.CurrentSettings.OnlyShowLongest = true;
            } else {
                this.CurrentSettings.SwitchBetweenLongest = false;
                this.CurrentSettings.OnlyShowLongest = false;
            }
            
            if (this.rdoCycleQualifyGold.Checked) {
                this.CurrentSettings.SwitchBetweenQualify = true;
                this.CurrentSettings.OnlyShowGold = false;
            } else if (this.rdoOnlyShowGold.Checked) {
                this.CurrentSettings.SwitchBetweenQualify = false;
                this.CurrentSettings.OnlyShowGold = true;
            } else {
                this.CurrentSettings.SwitchBetweenQualify = false;
                this.CurrentSettings.OnlyShowGold = false;
            }
            
            if (this.rdoCyclePlayersPing.Checked) {
                this.CurrentSettings.SwitchBetweenPlayers = true;
                this.CurrentSettings.OnlyShowPing = false;
            } else if (this.rdoOnlyShowPing.Checked) {
                this.CurrentSettings.SwitchBetweenPlayers = false;
                this.CurrentSettings.OnlyShowPing = true;
            } else {
                this.CurrentSettings.SwitchBetweenPlayers = false;
                this.CurrentSettings.OnlyShowPing = false;
            }
            
            if (this.rdoCycleWinFinalStreak.Checked) {
                this.CurrentSettings.SwitchBetweenStreaks = true;
                this.CurrentSettings.OnlyShowFinalStreak = false;
            } else if (this.rdoOnlyShowFinalStreak.Checked) {
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
            this.CurrentSettings.DisplayCurrentTime = this.chkDisplayCurrentTime.Checked;
            this.CurrentSettings.DisplayGamePlayedInfo = this.chkDisplayGamePlayedInfo.Checked;
            this.CurrentSettings.CountPlayersDuringTheLevel = this.chkCountPlayersDuringTheLevel.Checked;
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
                int overlaySetting = this.StatsForm.GetOverlaySetting();
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
                    this.CurrentSettings.OverlayFixedHeight = 134;
                } else {
                    this.CurrentSettings.OverlayHeight = 99;
                    this.CurrentSettings.OverlayFixedHeight = 99;
                }
            }
            
            this.CurrentSettings.AutoUpdate = this.chkAutoUpdate.Checked;
            this.CurrentSettings.SystemTrayIcon = this.chkSystemTrayIcon.Checked;
            this.CurrentSettings.NotifyServerConnected = this.chkNotifyServerConnected.Checked;
            this.CurrentSettings.NotifyPersonalBest = this.chkNotifyPersonalBest.Checked;
            this.CurrentSettings.MuteNotificationSounds = this.chkMuteNotificationSounds.Checked;
            this.CurrentSettings.NotificationSounds = this.cboNotificationSounds.SelectedIndex;
            this.CurrentSettings.NotificationWindowPosition = this.cboNotificationWindowPosition.SelectedIndex;
            this.CurrentSettings.NotificationWindowAnimation = this.cboNotificationWindowAnimation.SelectedIndex;
            this.CurrentSettings.PreventOverlayMouseClicks = this.chkPreventOverlayMouseClicks.Checked;
            
            this.CurrentSettings.FlippedDisplay = this.chkFlipped.Checked;
            this.CurrentSettings.HideOverlayPercentages = this.chkHidePercentages.Checked;
            this.CurrentSettings.HoopsieHeros = this.chkChangeHoopsieLegends.Checked;

            this.CurrentSettings.OverlayBackgroundResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).Data[0];
            this.CurrentSettings.OverlayTabResourceName = ((ImageItem)this.cboOverlayBackground.SelectedItem).Data[1];
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
            this.CurrentSettings.RecordEscapeDuringAGame = this.chkRecordEscapeDuringAGame.Checked;
            this.CurrentSettings.LaunchPlatform = this.LaunchPlatform;
            this.CurrentSettings.GameExeLocation = this.txtGameExeLocation.Text;
            this.CurrentSettings.GameShortcutLocation = this.txtGameShortcutLocation.Text;
            this.CurrentSettings.AutoLaunchGameOnStartup = this.chkLaunchGameOnStart.Checked;

            if (!string.IsNullOrEmpty(this.overlayFontSerialized)) {
                FontConverter fontConverter = new FontConverter();
                this.CurrentSettings.OverlayFontSerialized = fontConverter.ConvertToString(this.lblOverlayFontExample.Font);
            } else {
                this.CurrentSettings.OverlayFontSerialized = string.Empty;
                Overlay.SetDefaultFont(18, Stats.CurrentLanguage);
            }

            if (!string.IsNullOrEmpty(this.overlayFontColorSerialized)) {
                ColorConverter colorConverter = new ColorConverter();
                this.CurrentSettings.OverlayFontColorSerialized = colorConverter.ConvertToString(this.lblOverlayFontExample.ForeColor);
            } else {
                this.CurrentSettings.OverlayFontColorSerialized = string.Empty;
            }

            this.CurrentSettings.OverlayBackgroundOpacity = this.trkOverlayOpacity.Value;

            this.CurrentSettings.EnableFallalyticsReporting = this.chkFallalyticsReporting.Checked;
            this.CurrentSettings.EnableFallalyticsWeeklyCrownLeague = this.chkFallalyticsWeeklyCrownLeague.Checked;
            this.CurrentSettings.EnableFallalyticsAnonymous = this.chkFallalyticsAnonymous.Checked;
            this.CurrentSettings.FallalyticsAPIKey = this.txtFallalyticsAPIKey.Text;
            
            this.CurrentSettings.UseProxyServer = this.chkUseProxy.Checked;
            this.CurrentSettings.ProxyAddress = this.txtProxyAddress.Text;
            this.CurrentSettings.ProxyPort = this.txtProxyPort.Text;
            this.CurrentSettings.EnableProxyAuthentication = this.chkUseProxyLoginRequired.Checked;
            this.CurrentSettings.ProxyUsername = this.txtProxyUsername.Text;
            this.CurrentSettings.ProxyPassword = this.txtProxyPassword.Text;
            this.CurrentSettings.SucceededTestProxy = this.IsSucceededTestProxy;
            
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
                this.txtPreviousWins.Text = @"0";
            }
        }
        
        private void enterOnlyDigitInTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) {
                e.Handled = true;
            }
        }
        
        private void btnGameExeLocationBrowse_Click(object sender, EventArgs e) {
            this.BeginInvoke((MethodInvoker)(() => {
                try {
                    using (OpenFileDialog openFile = new OpenFileDialog()) {
                        if (this.LaunchPlatform == 0) { // Epic Games
                            if (string.IsNullOrEmpty(this.StatsForm.FindEpicGamesShortcutLocation())) {
                                MetroMessageBox.Show(this, Multilingual.GetWord("message_not_installed_epicGames", this.DisplayLang), Multilingual.GetWord("message_not_installed_epicGames_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            openFile.Filter = Multilingual.GetWord("settings_fall_guys_shortcut_openfile_filter", this.DisplayLang);
                            openFile.FileName = Multilingual.GetWord("settings_fall_guys_shortcut_openfile_name", this.DisplayLang);
                            openFile.Title = Multilingual.GetWord("settings_fall_guys_shortcut_openfile_title", this.DisplayLang);

                            if (openFile.ShowDialog(this) == DialogResult.OK) {
                                string fileContent;
                                string epicGamesFallGuysApp = "50118b7f954e450f8823df1614b24e80%3A38ec4849ea4f4de6aa7b6fb0f2d278e1%3A0a2d9f6403244d12969e11da6713137b";
                                FileStream fileStream = new FileStream(openFile.FileName, FileMode.Open);
                                using (StreamReader reader = new StreamReader(fileStream)) {
                                    fileContent = reader.ReadToEnd();
                                }

                                string[] splitContent = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                string url = string.Empty;

                                foreach (string content in splitContent) {
                                    if (content.ToLower().StartsWith("url=")) {
                                        url = content.Substring(4);
                                        break;
                                    }
                                }

                                if (url.ToLower().StartsWith("com.epicgames.launcher://apps/") && url.IndexOf(epicGamesFallGuysApp, StringComparison.OrdinalIgnoreCase) != -1) {
                                    this.txtGameShortcutLocation.Text = url;
                                } else {
                                    MetroMessageBox.Show(this, Multilingual.GetWord("message_wrong_selected_file_epicgames", this.DisplayLang), Multilingual.GetWord("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        } else { // Steam
                            if (string.IsNullOrEmpty(this.StatsForm.FindSteamExeLocation())) {
                                MetroMessageBox.Show(this, Multilingual.GetWord("message_not_installed_steam", this.DisplayLang), Multilingual.GetWord("message_not_installed_steam_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            FileInfo currentExeLocation = new FileInfo(this.txtGameExeLocation.Text);
                            if (currentExeLocation.Directory.Exists) {
                                openFile.InitialDirectory = currentExeLocation.Directory.FullName;
                            }
                            openFile.Filter = Multilingual.GetWord("settings_fall_guys_exe_openfile_filter", this.DisplayLang);
                            openFile.FileName = Multilingual.GetWord("settings_fall_guys_exe_openfile_name", this.DisplayLang);
                            openFile.Title = Multilingual.GetWord("settings_fall_guys_exe_openfile_title", this.DisplayLang);

                            if (openFile.ShowDialog(this) == DialogResult.OK) {
                                if (openFile.FileName.IndexOf("FallGuys_client", StringComparison.OrdinalIgnoreCase) >= 0) {
                                    txtGameExeLocation.Text = openFile.FileName;
                                } else {
                                    MetroMessageBox.Show(this, Multilingual.GetWord("message_wrong_selected_file_steam", this.DisplayLang), Multilingual.GetWord("message_wrong_selected_file_caption", this.DisplayLang), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    ControlErrors.HandleException(this, ex, false);
                }
            }));
        }
        
        private void launchPlatform_Click(object sender, EventArgs e) {
            this.StatsForm.UpdateGameExeLocation();
            switch (((PictureBox)sender).Name) {
                case "picEpicGames": // Epic Games
                    this.picPlatformCheck.Parent = this.picEpicGames;
                    this.platformToolTip.SetToolTip(this.picPlatformCheck, "Epic Games");
                    this.txtGameShortcutLocation.Visible = true;
                    this.txtGameExeLocation.Visible = false;

                    this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_shortcut_location", this.DisplayLang);
                    this.txtGameShortcutLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                    this.txtGameShortcutLocation.Size = new Size(567 - this.txtGameShortcutLocation.Location.X, 25);

                    this.LaunchPlatform = 0;
                    break;
                case "picSteam": // Steam
                    this.txtGameExeLocation.Text = this.CurrentSettings.GameExeLocation;
                    this.picPlatformCheck.Parent = this.picSteam;
                    this.platformToolTip.SetToolTip(this.picPlatformCheck, "Steam");
                    this.txtGameShortcutLocation.Visible = false;
                    this.txtGameExeLocation.Visible = true;

                    this.lblGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 3, 20);
                    this.lblGameExeLocation.Text = Multilingual.GetWord("settings_fall_guys_exe_location", this.DisplayLang);
                    this.txtGameExeLocation.Location = new Point(this.grpLaunchPlatform.Location.X + this.grpLaunchPlatform.Width + 8, 46);
                    this.txtGameExeLocation.Size = new Size(567 - this.txtGameExeLocation.Location.X, 25);

                    this.LaunchPlatform = 1;
                    break;
            }
            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(20, 16) : new Point(19, 14);
        }
        
        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void btnSelectFont_Click(object sender, EventArgs e) {
            this.BeginInvoke((MethodInvoker)(() => {
                this.dlgOverlayFont.Font = string.IsNullOrEmpty(this.overlayFontSerialized)
                    ? Overlay.GetDefaultFont(24, this.DisplayLang)
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
            }));
        }
        
        private void btnResetOverlayFont_Click(object sender, EventArgs e) {
            this.lblOverlayFontExample.Font = Overlay.GetDefaultFont(18, this.DisplayLang);
            this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.overlayFontColorSerialized = string.Empty;
            this.overlayFontSerialized = string.Empty;
        }
        
        private void cboMultilingual_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.DisplayLang == (Language)((ImageComboBox)sender).SelectedIndex) return;
            this.BeginInvoke((MethodInvoker)delegate {
                this.ChangeLanguage((Language)((ImageComboBox)sender).SelectedIndex);
            });
        }
        
        private void trkOverlayOpacity_ValueChanged(object sender, EventArgs e) {
            if (((MetroTrackBar)sender).Value == (int)(this.Overlay.Opacity * 100)) { return; }
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
        
        private void proxyInfoChanged_TextChanged(object sender, EventArgs e) {
            this.IsSucceededTestProxy = false;
            this.mpsProxySpinner.Visible = false;
            this.picProxyTextResult.Visible = false;
        }

        private void btnTestProxyConnection_Click(object sender, EventArgs e) {
            if (!this.chkUseProxy.Checked || string.IsNullOrEmpty(this.txtProxyAddress.Text)) {
                return;
            }
            ((MetroButton)sender).Enabled = false;
            
            WebProxy webproxy = new WebProxy($"{this.txtProxyAddress.Text}:{(!string.IsNullOrEmpty(this.txtProxyPort.Text) ? this.txtProxyPort.Text : "80")}", false) {
                BypassProxyOnLocal = false
            };

            if (this.chkUseProxyLoginRequired.Checked) {
                if (string.IsNullOrEmpty(this.txtProxyUsername.Text) || string.IsNullOrEmpty(this.txtProxyPassword.Text)) {
                    ((MetroButton)sender).Enabled = true;
                    return;
                }
                webproxy.Credentials = new NetworkCredential(this.txtProxyUsername.Text, this.txtProxyPassword.Text);
            }
            
            this.mpsProxySpinner.Visible = true;
            Task.Run(() => {
                try {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fallalytics.com");
                    request.Proxy = webproxy;
                    request.Timeout = 5000;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                        bool isOk = response.StatusCode == HttpStatusCode.OK;
                        this.IsSucceededTestProxy = isOk;
                        this.BeginInvoke((MethodInvoker)delegate {
                            this.mpsProxySpinner.Visible = false;
                            this.picProxyTextResult.Image = isOk ? Properties.Resources.checkmark_icon : Properties.Resources.uncheckmark_icon;
                            this.picProxyTextResult.Visible = true;
                            ((MetroButton)sender).Enabled = true;
                        });
                    }
                } catch {
                    this.IsSucceededTestProxy = false;
                    this.BeginInvoke((MethodInvoker)delegate {
                        this.mpsProxySpinner.Visible = false;
                        this.picProxyTextResult.Image = Properties.Resources.uncheckmark_icon;
                        this.picProxyTextResult.Visible = true;
                        ((MetroButton)sender).Enabled = true;
                    });
                }
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Tab) { SendKeys.Send("%"); }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeLanguage(Language lang) {
            this.SuspendLayout();
            this.DisplayLang = lang;
            this.Font = Overlay.GetMainFont(12, FontStyle.Regular, this.DisplayLang);
            Language tempLanguage = Stats.CurrentLanguage;
            Stats.CurrentLanguage = lang;

            this.Text = $@"     {Multilingual.GetWord("settings_title")}";
            this.lblOverlayFontExample.Font = Overlay.GetDefaultFont(18, this.DisplayLang);
            this.overlayFontSerialized = string.Empty;
            this.lblOverlayFontExample.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.overlayFontColorSerialized = string.Empty;
            this.chkChangeHoopsieLegends.Visible = true;
            this.chkChangeHoopsieLegends.Checked = this.CurrentSettings.HoopsieHeros;

            this.tileProgram.Text = Multilingual.GetWord("settings_program");
            this.tileDisplay.Text = Multilingual.GetWord("settings_display");
            this.tileOverlay.Text = Multilingual.GetWord("settings_overlay");
            this.tileFallGuys.Text = Multilingual.GetWord("settings_launch_fallguys");
            this.tileFallalytics.Text = Multilingual.GetWord("settings_fallalytics");
            this.tileProxy.Text = Multilingual.GetWord("settings_proxy_settings");
            this.tileAbout.Text = Multilingual.GetWord("settings_about");
            
            this.btnCancel.Text = Multilingual.GetWord("settings_cancel");
            this.btnCancel.Width = TextRenderer.MeasureText(this.btnCancel.Text, this.btnCancel.Font).Width + 45;
            this.btnCancel.Left = this.Width - this.btnCancel.Width - 25;
            this.btnSave.Text = Multilingual.GetWord("settings_save");
            this.btnSave.Width = TextRenderer.MeasureText(this.btnSave.Text, this.btnSave.Font).Width + 45;
            this.btnSave.Left = this.btnCancel.Left - this.btnSave.Width - 15;

            this.picPlatformCheck.Location = this.LaunchPlatform == 0 ? new Point(20, 16) : new Point(19, 14);

            this.lblTheme.Text = Multilingual.GetWord("settings_theme");
            this.cboTheme.Items.Clear();
            this.cboTheme.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_theme_light"),
                Multilingual.GetWord("settings_theme_dark"),
            });
            this.cboTheme.SelectedItem = this.Theme == MetroThemeStyle.Light
                ? Multilingual.GetWord("settings_theme_light")
                : Multilingual.GetWord("settings_theme_dark");

            // this.lblLogPath.Text = Multilingual.GetWord("settings_log_path");
            this.lblLogPathNote.Text = Multilingual.GetWord("settings_log_path_description");
            
            this.rdoOnlyShowGold.Text = Multilingual.GetWord("settings_gold_only");
            this.rdoOnlyShowQualify.Text = Multilingual.GetWord("settings_qualify_only");
            this.rdoCycleQualifyGold.Text = Multilingual.GetWord("settings_cycle_qualify__gold");
            this.rdoOnlyShowLongest.Text = Multilingual.GetWord("settings_personal_lowest_only");
            this.rdoOnlyShowFastest.Text = Multilingual.GetWord("settings_personal_best_only");
            this.rdoCycleFastestLongest.Text = Multilingual.GetWord("settings_cycle_best__lowest");
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
            this.chkDisplayCurrentTime.Text = Multilingual.GetWord("settings_diaplay_current_time");
            this.chkDisplayGamePlayedInfo.Text = Multilingual.GetWord("settings_diaplay_game_played_info");
            this.chkCountPlayersDuringTheLevel.Text = Multilingual.GetWord("settings_count_players_during_the_level");
            this.lblCycleTimeSecondsTag.Text = Multilingual.GetWord("settings_sec");
            this.lblCycleTimeSeconds.Text = Multilingual.GetWord("settings_cycle_time");
            this.rdoOnlyShowFinalStreak.Text = Multilingual.GetWord("settings_final_streak_only");
            this.rdoOnlyShowWinStreak.Text = Multilingual.GetWord("settings_win_streak_only");
            this.rdoCycleWinFinalStreak.Text = Multilingual.GetWord("settings_cycle_win__final_streak");
            this.rdoOnlyShowPing.Text = Multilingual.GetWord("settings_ping_only");
            this.rdoOnlyShowPlayers.Text = Multilingual.GetWord("settings_players_only");
            this.rdoCyclePlayersPing.Text = Multilingual.GetWord("settings_cycle_players__ping");
            this.lblOverlayFont.Text = Multilingual.GetWord("settings_custom_overlay_font");
            this.btnSelectFont.Text = Multilingual.GetWord("settings_select_font");
            this.btnSelectFont.Width = TextRenderer.MeasureText(this.btnSelectFont.Text, this.btnSelectFont.Font).Width + 40;
            this.btnSelectFont.Left = this.lblOverlayFont.Right + 15;
            this.btnResetOverlayFont.Text = Multilingual.GetWord("settings_reset_font");
            this.btnResetOverlayFont.Width = TextRenderer.MeasureText(this.btnResetOverlayFont.Text, this.btnResetOverlayFont.Font).Width + 40;
            this.btnResetOverlayFont.Left = this.btnSelectFont.Right + 10;
            this.grpOverlayFontExample.Text = Multilingual.GetWord("settings_font_example");
            this.lblOverlayFontExample.Text = Multilingual.GetWord("settings_round_example");
            this.chkChangeHoopsieLegends.Text = Multilingual.GetWord("settings_rename_hoopsie_legends_to_hoopsie_heroes");
            this.chkAutoUpdate.Text = Multilingual.GetWord("settings_auto_update_program");
            this.chkSystemTrayIcon.Text = Multilingual.GetWord("settings_system_tray_icon");
            this.chkNotifyServerConnected.Text = Multilingual.GetWord("settings_notify_server_connected");
            this.chkNotifyPersonalBest.Text = Multilingual.GetWord("settings_notify_personal_best");
            this.chkMuteNotificationSounds.Text = Multilingual.GetWord("settings_mute_notification_sounds");
            this.cboNotificationSounds.Items.Clear();
            this.cboNotificationSounds.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_notification_sounds_01"),
                Multilingual.GetWord("settings_notification_sounds_02"),
                Multilingual.GetWord("settings_notification_sounds_03"),
                Multilingual.GetWord("settings_notification_sounds_04"),
            });
            this.cboNotificationSounds.SelectedIndex = this.CurrentSettings.NotificationSounds;
            // this.cboNotificationSounds.Width = (lang == Language.English || lang == Language.French) ? 172 : lang == Language.Korean ? 115 : lang == Language.Japanese ? 95 : 110;
            this.cboNotificationWindowPosition.Items.Clear();
            this.cboNotificationWindowPosition.Items.AddRange(new object[] {
                Multilingual.GetWord("settings_notification_window_top_left"),
                Multilingual.GetWord("settings_notification_window_top_right"),
                Multilingual.GetWord("settings_notification_window_bottom_left"),
                Multilingual.GetWord("settings_notification_window_bottom_right"),
            });
            this.cboNotificationWindowPosition.SelectedIndex = this.CurrentSettings.NotificationWindowPosition;
            this.cboNotificationWindowPosition.Width = lang == Language.English ? 116 : lang == Language.French ? 140 : lang == Language.Korean ? 108 : lang == Language.Japanese ? 62 : lang == Language.SimplifiedChinese ? 77 : lang == Language.TraditionalChinese ? 77 : 120;
            this.cboNotificationWindowPosition.Location = new Point(this.cboNotificationSounds.Location.X + this.cboNotificationSounds.Width + 5, this.cboNotificationWindowPosition.Location.Y);
            this.cboNotificationWindowAnimation.Location = new Point(this.cboNotificationWindowPosition.Location.X + this.cboNotificationWindowPosition.Width + 5, this.cboNotificationWindowAnimation.Location.Y);
            this.mlPlayNotificationSounds.Location = new Point(this.cboNotificationWindowAnimation.Location.X + this.cboNotificationWindowAnimation.Width + 5, this.mlPlayNotificationSounds.Location.Y);
            this.chkPreventOverlayMouseClicks.Text = Multilingual.GetWord("settings_prevent_overlay_mouse_clicks");
            this.lblPreviousWinsNote.Text = Multilingual.GetWord("settings_before_using_tracker");
            this.lblPreviousWins.Text = Multilingual.GetWord("settings_previous_win");
            this.grpLaunchPlatform.Text = Multilingual.GetWord("settings_game_options_platform");
            this.grpLaunchPlatform.Width = lang == Language.Japanese ? 120 : 95;
            this.btnGameExeLocationBrowse.Text = Multilingual.GetWord("settings_browse");
            this.chkLaunchGameOnStart.Text = Multilingual.GetWord("settings_launch_fall_guys_on_tracker_launch");
            this.chkIgnoreLevelTypeWhenSorting.Text = Multilingual.GetWord("settings_ignore_level_type_when_sorting");
            this.chkGroupingCreativeRoundLevels.Text = Multilingual.GetWord("settings_grouping_creative_round_levels");
            this.chkRecordEscapeDuringAGame.Text = Multilingual.GetWord("settings_record_escape_during_a_game");

            this.txtLogPath.WaterMark = Multilingual.GetWord("settings_log_path");
            // this.txtLogPath.Location = new Point(this.lblLogPath.Location.X + this.lblLogPath.Width + 4, 12);
            // this.txtLogPath.Size = new Size(630 - this.lblLogPath.Width - 4, 25);
            this.txtPreviousWins.Location = new Point(this.lblPreviousWins.Location.X + this.lblPreviousWins.Width + 4, 10);
            this.lblPreviousWinsNote.Location = new Point(this.txtPreviousWins.Location.X + this.txtPreviousWins.Width + 4, 12);
            this.cboTheme.Location = new Point(this.lblTheme.Location.X + this.lblTheme.Width + 4, this.cboTheme.Location.Y);
            this.cboTheme.Width = lang == Language.English ? 90 : lang == Language.French ? 105 : lang == Language.Korean ? 100 : lang == Language.Japanese ? 100 : 85;
            this.txtCycleTimeSeconds.Location = new Point(this.lblCycleTimeSeconds.Location.X + this.lblCycleTimeSeconds.Width + 4, 167);
            this.lblCycleTimeSecondsTag.Location = new Point(this.txtCycleTimeSeconds.Location.X + this.txtCycleTimeSeconds.Width + 4, 170);
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
            this.chkFallalyticsWeeklyCrownLeague.Text = Multilingual.GetWord("settings_join_a_weekly_crown_league");
            this.chkFallalyticsAnonymous.Text = Multilingual.GetWord("settings_sends_anonymously_to_fallalytics");
            // this.lblFallalyticsAPIKey.Text = Multilingual.GetWord("settings_enter_fallalytics_api_key");
            this.txtFallalyticsAPIKey.WaterMark = Multilingual.GetWord("settings_enter_fallalytics_api_key");
            this.lblFallalyticsDesc.Text = Multilingual.GetWord("settings_fallalytics_desc");
            this.linkFallalytics.Text = Multilingual.GetWord("settings_visit_fallalytics");
            
            this.fglink1.Text = Multilingual.GetWord("settings_github");
            this.fglink2.Text = $"{Multilingual.GetWord("settings_issue_traker")} && {Multilingual.GetWord("settings_translation")}";
            this.btnCheckUpdates.Text = Multilingual.GetWord("main_update");
            this.btnCheckUpdates.Width = TextRenderer.MeasureText(this.btnCheckUpdates.Text, this.btnCheckUpdates.Font).Width + 30;
            this.lblthirdpartyLicences.Font = Overlay.GetMainFont(18);
#if AllowUpdate
            this.lblVersion.Text = $"{Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
#else
            this.lblVersion.Text = $"{Multilingual.GetWord("main_fall_guys_stats")} v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)} ({Multilingual.GetWord("main_manual_update_version")})";
            this.chkAutoUpdate.Visible = false;
#endif
            Stats.CurrentLanguage = tempLanguage;
            this.ResumeLayout();
        }

        private void ChangeTab(object sender, EventArgs e) {
            if (!(sender is MetroTile tile) || string.Equals(tile.Name, this.PrevTabName)) {
                return;
            }
            this.PrevTabName = tile.Name;
            this.BeginInvoke((MethodInvoker)delegate {
                this.panelProgram.Location = new Point(211, 75);
                this.panelDisplay.Location = new Point(211, 75);
                this.panelOverlay.Location = new Point(211, 75);
                this.panelFallGuys.Location = new Point(211, 75);
                this.panelAbout.Location = new Point(211, 75);
                this.panelFallalytics.Location = new Point(211, 75);
                this.panelProxy.Location = new Point(211, 75);
                this.panelProgram.Visible = false;
                this.panelDisplay.Visible = false;
                this.panelOverlay.Visible = false;
                this.panelFallGuys.Visible = false;
                this.panelAbout.Visible = false;
                this.panelFallalytics.Visible = false;
                this.panelProxy.Visible = false;
                this.tileProgram.Style = MetroColorStyle.Silver;
                this.tileDisplay.Style = MetroColorStyle.Silver;
                this.tileOverlay.Style = MetroColorStyle.Silver;
                this.tileFallGuys.Style = MetroColorStyle.Silver;
                this.tileAbout.Style = MetroColorStyle.Silver;
                this.tileFallalytics.Style = MetroColorStyle.Silver;
                this.tileProxy.Style = MetroColorStyle.Silver;
                if (sender.Equals(this.tileProgram)) {
                    this.tileProgram.Style = MetroColorStyle.Teal;
                    this.panelProgram.Visible = true;
                } else if (sender.Equals(this.tileDisplay)) {
                    this.tileDisplay.Style = MetroColorStyle.Teal;
                    this.panelDisplay.Visible = true;
                } else if (sender.Equals(this.tileOverlay)) {
                    this.tileOverlay.Style = MetroColorStyle.Teal;
                    this.panelOverlay.Visible = true;
                } else if (sender.Equals(this.tileFallGuys)) {
                    this.tileFallGuys.Style = MetroColorStyle.Teal;
                    this.panelFallGuys.Visible = true;
                } else if (sender.Equals(this.tileAbout)) {
                    this.tileAbout.Style = MetroColorStyle.Teal;
                    #if AllowUpdate
                    this.lblupdateNote.Text = Multilingual.GetWord("settings_checking_for_updates");
                    using (ZipWebClient web = new ZipWebClient()) {
                        string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                        int index = assemblyInfo.IndexOf("AssemblyVersion(");
                        if (index > 0) {
                            int indexEnd = assemblyInfo.IndexOf("\")", index);
                            Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                            Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                            if (newVersion > currentVersion) {
                                this.btnCheckUpdates.Visible = true;
                                this.lblupdateNote.Text = $"{Multilingual.GetWord("settings_new_update_prefix", this.DisplayLang)} v{newVersion.ToString(2)} {Multilingual.GetWord("settings_new_update_suffix", this.DisplayLang)}";
                                this.lblupdateNote.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.LimeGreen : Color.LightGreen;
                                this.lblupdateNote.Location = new Point(this.btnCheckUpdates.Location.X + this.btnCheckUpdates.Width + 8, this.btnCheckUpdates.Location.Y + 4);
                                this.StatsForm.ChangeStateForAvailableNewVersion(newVersion.ToString(2));
                            } else {
                                this.lblupdateNote.Text = $"{Multilingual.GetWord("message_update_latest_version", this.DisplayLang)}{Environment.NewLine}{Environment.NewLine}{Multilingual.GetWord("main_update_prefix_tooltip", this.DisplayLang).Trim()}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip", this.DisplayLang).Trim()}";
                                this.lblupdateNote.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
                            }
                        } else {
                            this.lblupdateNote.Text = Multilingual.GetWord("message_update_not_determine_version", this.DisplayLang);
                            this.lblupdateNote.ForeColor = Color.Crimson;
                        }
                    }
#else
                    this.lblupdateNote.Text = $"{Multilingual.GetWord("main_update_prefix_tooltip", this.DisplayLang).Trim()}{Environment.NewLine}{Multilingual.GetWord("main_update_suffix_tooltip", this.DisplayLang).Trim()}";
                    this.lblupdateNote.ForeColor = this.Theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
#endif
                    this.panelAbout.Visible = true;
                } else if (sender.Equals(this.tileFallalytics)) {
                    this.tileFallalytics.Style = MetroColorStyle.Teal;
                    this.panelFallalytics.Visible = true;
                } else if (sender.Equals(this.tileProxy)) {
                    this.tileProxy.Style = MetroColorStyle.Teal;
                    this.panelProxy.Visible = true;
                }
                this.Refresh();
            });
        }

        private void link_Click(object sender, EventArgs e) {
            if (sender.Equals(this.fglink1)) {
                this.OpenLink(@"https://github.com/ShootMe/FallGuysStats");
            }
            if (sender.Equals(this.fglink2)) {
                this.OpenLink(@"https://github.com/ShootMe/FallGuysStats/issues");
            }
            if (sender.Equals(this.lbltpl0)) {
                this.OpenLink(@"https://github.com/mbdavid/LiteDB/blob/master/LICENSE");
            }
            if (sender.Equals(this.lbltpl1)) {
                this.OpenLink(@"https://github.com/Fody/Costura/blob/develop/LICENSE");
            }
            if (sender.Equals(this.lbltpl2)) {
                this.OpenLink(@"https://github.com/Fody/Home/blob/master/license.txt");
            }
            if (sender.Equals(this.lbltpl3)) {
                this.OpenLink(@"https://github.com/dennismagno/metroframework-modern-ui/blob/master/LICENSE.md");
            }
            if (sender.Equals(this.lbltpl4)) {
                this.OpenLink(@"https://github.com/ScottPlot/ScottPlot/blob/main/LICENSE");
            }
            if (sender.Equals(this.linkFallalytics)) {
                this.OpenLink(@"https://fallalytics.com/");
            }
        }
        
        private void OpenLink(string link) {
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
                                 progress.DownloadUrl = Utils.FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL;
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
