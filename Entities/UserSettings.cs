﻿using System;

namespace FallGuysStats {
    public class UserSettings {
        public int ID { get; set; }
        public int Multilingual { get; set; }
        public string LogPath { get; set; }
        public int Theme { get; set; }
        public bool Visible { get; set; }
        public int FilterType { get; set; }
        public DateTime CustomFilterRangeStart { get; set; }
        public DateTime CustomFilterRangeEnd { get; set; }
        public int SelectedCustomTemplateSeason { get; set; }
        public int SelectedProfile { get; set; }
        public int? OverlayLocationX { get; set; }
        public int? OverlayLocationY { get; set; }
        public string OverlayFixedPosition { get; set; }
        public int? OverlayFixedPositionX { get; set; }
        public int? OverlayFixedPositionY { get; set; }
        public int? OverlayFixedWidth { get; set; }
        public int? OverlayFixedHeight { get; set; }
        public int OverlayBackground { get; set; }
        public string OverlayBackgroundResourceName { get; set; }
        public string OverlayTabResourceName { get; set; }
        public int OverlayBackgroundOpacity { get; set; }
        public bool IsOverlayBackgroundCustomized { get; set; }
        public int OverlayColor { get; set; }
        public bool FlippedDisplay { get; set; }
        public bool FixedFlippedDisplay { get; set; }
        public bool SwitchBetweenLongest { get; set; }
        public bool SwitchBetweenQualify { get; set; }
        public bool SwitchBetweenPlayers { get; set; }
        public bool SwitchBetweenStreaks { get; set; }
        public bool OnlyShowLongest { get; set; }
        public bool OnlyShowGold { get; set; }
        public bool OnlyShowPing { get; set; }
        public bool OnlyShowFinalStreak { get; set; }
        public int CycleTimeSeconds { get; set; }
        public bool OverlayVisible { get; set; }
        public bool OverlayNotOnTop { get; set; }
        public bool PlayerByConsoleType { get; set; }
        public bool ColorByRoundType { get; set; }
        public bool AutoChangeProfile { get; set; }
        public bool ShadeTheFlagImage { get; set; }
        public bool DisplayCurrentTime { get; set; }
        public bool DisplayGamePlayedInfo { get; set; }
        public bool CountPlayersDuringTheLevel { get; set; }
        public int PreviousWins { get; set; }
        public int WinsFilter { get; set; }
        public int FastestFilter { get; set; }
        public int QualifyFilter { get; set; }
        public bool HideWinsInfo { get; set; }
        public bool HideRoundInfo { get; set; }
        public bool HideTimeInfo { get; set; }
        public bool ShowOverlayTabs { get; set; }
        //public bool ShowOverlayProfile { get; set; }
        public bool ShowPercentages { get; set; }
        public bool UpdatedDateFormat { get; set; }
        public bool AutoUpdate { get; set; }
        public bool SystemTrayIcon { get; set; }
        public bool PreventOverlayMouseClicks { get; set; }
        public bool NotifyServerConnected { get; set; }
        public bool NotifyPersonalBest { get; set; }
        public bool MuteNotificationSounds { get; set; }
        public int NotificationSounds { get; set; }
        public int NotificationWindowPosition { get; set; }
        public int NotificationWindowAnimation { get; set; }
        public bool MaximizedWindowState { get; set; }
        public int? FormLocationX { get; set; }
        public int? FormLocationY { get; set; }
        public int? FormWidth { get; set; }
        public int? FormHeight { get; set; }
        public int? OverlayWidth { get; set; }
        public int? OverlayHeight { get; set; }
        public bool HideOverlayPercentages { get; set; }
        public bool HoopsieHeros { get; set; }
        public int LevelTimeLimitVersion { get; set; }
        public int Version { get; set; }
        public bool IgnoreLevelTypeWhenSorting { get; set; }
        public bool GroupingCreativeRoundLevels { get; set; }
        public bool RecordEscapeDuringAGame { get; set; }
        public int LaunchPlatform { get; set; }
        public string GameExeLocation { get; set; }
        public string GameShortcutLocation { get; set; }
        public bool AutoLaunchGameOnStartup { get; set; }
        public string OverlayFontSerialized { get; set; }
        public string OverlayFontColorSerialized { get; set; }
        public int WinPerDayGraphStyle { get; set; }
        public bool ShowChangelog { get; set; }
        public bool EnableFallalyticsReporting { get; set; }
        public bool EnableFallalyticsWeeklyCrownLeague { get; set; }
        public bool EnableFallalyticsAnonymous { get; set; }
        public string FallalyticsAPIKey { get; set; }
        public bool UseProxyServer { get; set; }
        public string ProxyAddress { get; set; }
        public string ProxyPort { get; set; }
        public bool EnableProxyAuthentication { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public bool SucceededTestProxy { get; set; }
        public int IpGeolocationService {  get; set; }
    }
}