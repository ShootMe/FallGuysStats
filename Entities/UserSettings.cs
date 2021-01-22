namespace FallGuysStats {
    public class UserSettings {
        public int ID { get; set; }
        public string LogPath { get; set; }
        public int FilterType { get; set; }
        public int SelectedProfile { get; set; }
        public int? OverlayLocationX { get; set; }
        public int? OverlayLocationY { get; set; }
        public int OverlayColor { get; set; }
        public bool FlippedDisplay { get; set; }
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
        public int PreviousWins { get; set; }
        public int WinsFilter { get; set; }
        public int FastestFilter { get; set; }
        public int QualifyFilter { get; set; }
        public bool HideWinsInfo { get; set; }
        public bool HideRoundInfo { get; set; }
        public bool HideTimeInfo { get; set; }
        public bool ShowOverlayTabs { get; set; }
        public bool ShowPercentages { get; set; }
        public bool UpdatedDateFormat { get; set; }
        public bool AutoUpdate { get; set; }
        public int? FormLocationX { get; set; }
        public int? FormLocationY { get; set; }
        public int? FormWidth { get; set; }
        public int? FormHeight { get; set; }
        public int? OverlayWidth { get; set; }
        public int? OverlayHeight { get; set; }
        public bool HideOverlayPercentages { get; set; }
        public bool HoopsieHeros { get; set; }
        public int Version { get; set; }
        public bool IgnoreLevelTypeWhenSorting { get; set; }
        public string GameExeLocation { get; set; }
        public bool AutoLaunchGameOnStartup { get; set; }
        public string OverlayFontSerialized { get; set; }
    }
}