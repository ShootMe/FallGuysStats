using System.Drawing;

namespace FallGuysStats {
    public enum LevelType {
        Race,
        Survival,
        Team,
        Hunt,
        Unknown,
    }
    static class LevelTypeBehavior {
        public static int FastestLabel(this LevelType type) {
            switch (type) {
                case LevelType.Race:
                case LevelType.Hunt:
                    return 1; // FASTEST
                case LevelType.Survival:
                    return 0; // LONGEST
                case LevelType.Team:
                    return 2; // HIGH_SCORE
            }
            return 1;
        }
        public static Color LevelBackColor(this LevelType type, bool isFinal, int alpha) {
            if (isFinal) {
                return Color.FromArgb(alpha, 251, 198, 0);
            }
            switch (type) {
                case LevelType.Race:
                    return Color.FromArgb(alpha, 0, 236, 106);
                case LevelType.Hunt:
                    return Color.FromArgb(alpha, 45, 101, 186);
                case LevelType.Survival:
                    return Color.FromArgb(alpha, 184, 21, 213);
                case LevelType.Team:
                    return Color.FromArgb(alpha, 248, 82, 0);
            }
            return Color.White;
        }
        public static Color LevelForeColor(this LevelType type, bool isFinal) {
            if (isFinal) {
                return Color.FromArgb(161, 126, 0);
            }
            switch (type) {
                case LevelType.Race:
                    return Color.FromArgb(0, 151, 68);
                case LevelType.Hunt:
                    return Color.FromArgb(29, 65, 119);
                case LevelType.Survival:
                    return Color.FromArgb(118, 14, 136);
                case LevelType.Team:
                    return Color.FromArgb(158, 53, 0);
            }
            return Color.Black;
        }
    }
}