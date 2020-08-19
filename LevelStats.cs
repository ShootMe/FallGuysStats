using System;
using System.Collections.Generic;
namespace FallGuysStats {
    public class RoundInfo {
        public string Name { get; set; }
        public int ShowID { get; set; }
        public int Round { get; set; }
        public int Position { get; set; }
        public int? Score { get; set; }
        public int Tier { get; set; }
        public bool Qualified { get; set; }
        public int Kudos { get; set; }
        public int Players { get; set; }
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;

        public void ToLocalTime() {
            Start = Start.Add(Start - Start.ToUniversalTime());
            End = End.Add(End - End.ToUniversalTime());
        }
        public override string ToString() {
            return $"{Name}: Round={Round} Position={Position} Duration={End - Start} Kudos={Kudos}";
        }
    }
    public enum LevelType {
        Race,
        Survival,
        Team,
        Final
    }
    public class LevelStats {
        public string Name { get; set; }
        public int Qualified { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
        public int Played { get; set; }
        public int Kudos { get; set; }
        public int AveKudos { get { return Kudos / (Played == 0 ? 1 : Played); } }
        public LevelType Type;
        public TimeSpan AveDuration { get { return TimeSpan.FromSeconds((int)Duration.TotalSeconds / (Played == 0 ? 1 : Played)); } }
        public TimeSpan Duration;
        public string LevelName;
        public List<RoundInfo> Stats;

        public LevelStats(string name, string levelName, LevelType type) {
            Name = name;
            LevelName = levelName;
            Type = type;
            Stats = new List<RoundInfo>();
            Clear();
        }
        public void Clear() {
            Qualified = 0;
            Gold = 0;
            Silver = 0;
            Bronze = 0;
            Played = 0;
            Kudos = 0;
            Duration = TimeSpan.Zero;
            Stats.Clear();
        }
        public void Add(RoundInfo stat) {
            Stats.Add(stat);
            Played++;
            if (stat.Tier == 1) {
                Gold++;
            } else if (stat.Tier == 2) {
                Silver++;
            } else if (stat.Tier == 3) {
                Bronze++;
            }
            Kudos += stat.Kudos;
            Duration += stat.End - stat.Start;
            Qualified += stat.Qualified ? 1 : 0;
        }

        public override string ToString() {
            return $"{Name}: {Qualified} / {Played}";
        }
    }
}