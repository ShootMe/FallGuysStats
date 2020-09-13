namespace FallGuysStats {
    public class UserSettings {
        public int ID { get; set; }
        public string LogPath { get; set; }
        public int FilterType { get; set; }
        public int? OverlayLocationX { get; set; }
        public int? OverlayLocationY { get; set; }
        public int OverlayColor { get; set; }
        public bool FlippedDisplay { get; set; }
        public bool SwitchBetweenLongest { get; set; }
        public int CycleTimeSeconds { get; set; }
        public bool OverlayVisible { get; set; }
        public bool OverlayNotOnTop { get; set; }
        public int PreviousWins { get; set; }
        public bool UseNDI { get; set; }
        public int WinsFilter { get; set; }
        public int FastestFilter { get; set; }
        public int QualifyFilter { get; set; }
        public bool HideRoundInfo { get; set; }
        public bool HideTimeInfo { get; set; }
        public bool ShowOverlayTabs { get; set; }
    }
}