using System.Drawing;
namespace FallGuysStats {
    public class UserSettings {
        public int ID { get; set; }
        public string LogPath { get; set; }
        public int FilterType { get; set; }
        public Vector2? OverlayLocation { get; set; }
        public int OverlayColor { get; set; }
        public bool FlippedDisplay { get; set; }
        public bool SwitchBetweenLongest { get; set; }
        public int CycleTimeSeconds { get; set; }
        public bool OverlayVisible { get; set; }
    }
    public struct Vector2 {
        public int X { get; set; }
        public int Y { get; set; }
        public static implicit operator Point(Vector2 vector) => new Point(vector.X, vector.Y);
        public static implicit operator Vector2(Point point) => new Vector2() { X = point.X, Y = point.Y };
    }
}