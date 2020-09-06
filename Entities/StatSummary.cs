using System;
namespace FallGuysStats {
    public class StatSummary {
        public string CurrentFilter { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
        public int TotalPlays { get; set; }
        public int TotalQualify { get; set; }
        public int TotalGolds { get; set; }
        public TimeSpan? BestFinish { get; set; }
        public TimeSpan? LongestFinish { get; set; }
        public int? BestScore { get; set; }
    }
}