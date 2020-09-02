using System.Drawing;
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
    }
}