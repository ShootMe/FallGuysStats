using System;

namespace FallGuysStats {
    public class UpcomingShow {
        public string ShowId { get; set; }
        public string LevelId { get; set; }
        public string ShareCode { get; set; }
        public string DisplayName { get; set; }
        public LevelType LevelType { get; set; }
        public BestRecordType BestRecordType { get; set; }
        public bool IsCreative { get; set; }
        public bool IsFinal { get; set; }
        public DateTime AddDate { get; set; }
    }
}