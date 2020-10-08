namespace FallGuysStats {
    public enum LevelType {
        Race,
        Survival,
        Team,
        Hunt,
        Unknown
    }
    static class LevelTypeBehavior {
        public static int FastestLabel(this LevelType type) {
            switch (type) {
                case LevelType.Race:
                    return 1; // FASTEST
                case LevelType.Survival:
                    return 0; // LONGEST
                case LevelType.Team:
                    return 2; // HIGH_SCORE
                case LevelType.Hunt:
                    return 1; // FASTEST
            }
            return 1;
        }
    }
}