using System;

namespace FallGuysStats {
    public class StatSummary {
        public int CurrentStreak { get; set; }
        public int CurrentFinalStreak { get; set; }
        public int BestStreak { get; set; }
        public int BestFinalStreak { get; set; }
        public int AllWins { get; set; }
        public int TotalWins { get; set; }
        public int TotalShows { get; set; }
        public int TotalFinals { get; set; }
        public int TotalPlays { get; set; }
        public int TotalQualify { get; set; }
        public int TotalGolds { get; set; }
        public TimeSpan? BestFinish { get; set; }
        public TimeSpan? BestFinishOverall { get; set; }
        public TimeSpan? LongestFinish { get; set; }
        public TimeSpan? LongestFinishOverall { get; set; }
        public int? BestScore { get; set; }
    }
}