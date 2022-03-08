namespace FallGuysStats {
    public enum LevelType {
        Race,
        Survival,
        Team,
        Hunt,
        Invisibeans,
        Unknown
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
                case LevelType.Invisibeans:
                    return 2; // HIGH_SCORE
            }
            return 1;
        }
    }
}