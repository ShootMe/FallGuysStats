using System;
using System.Collections.Generic;
using LiteDB;
namespace FallGuysStats {
    public class RoundInfo {
        public ObjectId ID { get; set; }
        public string Name { get; set; }
        public int ShowID { get; set; }
        public int Round { get; set; }
        public int Position { get; set; }
        public int? Score { get; set; }
        public int Tier { get; set; }
        public bool Qualified { get; set; }
        public int Kudos { get; set; }
        public int Players { get; set; }
        public bool InParty { get; set; }
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public DateTime? Finish { get; set; } = null;
        public bool Crown { get; set; }
        public DateTime StartLocal;
        public DateTime EndLocal;
        public DateTime? FinishLocal;
        public DateTime ShowStart = DateTime.MinValue;
        public bool Playing;
        public bool setLocalTime;

        public void ToLocalTime() {
            if (setLocalTime) { return; }
            setLocalTime = true;

            StartLocal = Start.ToLocalTime();
            EndLocal = End.ToLocalTime();
            if (Finish.HasValue) {
                FinishLocal = Finish.Value.ToLocalTime();
            }
        }
        public override string ToString() {
            return $"{Name}: Round={Round} Position={Position} Duration={End - Start} Kudos={Kudos}";
        }
        public override bool Equals(object obj) {
            return obj is RoundInfo info
                && info.End == this.End
                && info.Finish == this.Finish
                && info.InParty == this.InParty
                && info.Kudos == this.Kudos
                && info.Players == this.Players
                && info.Position == this.Position
                && info.Qualified == this.Qualified
                && info.Round == this.Round
                && info.Score == this.Score
                && info.ShowID == this.ShowID
                && info.Start == this.Start
                && info.Tier == this.Tier
                && info.Name == this.Name;
        }
        public override int GetHashCode() {
            return Name.GetHashCode() ^ ShowID ^ Round;
        }
    }
    public enum LevelType {
        Race,
        Survival,
        Team,
        Final,
        Unknown
    }
    public enum QualifyTier {
        None,
        Gold,
        Silver,
        Bronze
    }
    public class LevelStats {
        public static Dictionary<string, string> DisplayNameLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "round_door_dash",                  "Door Dash" },
            { "round_gauntlet_02",                "Dizzy Heights" },
            { "round_dodge_fall",                 "Fruit Chute" },
            { "round_chompchomp",                 "Gate Crash" },
            { "round_gauntlet_01",                "Hit Parade" },
            { "round_see_saw",                    "See Saw" },
            { "round_lava",                       "Slime Climb" },
            { "round_tip_toe",                    "Tip Toe" },
            { "round_gauntlet_03",                "Whirlygig" },

            { "round_block_party",                "Block Party" },
            { "round_jump_club",                  "Jump Club" },
            { "round_match_fall",                 "Perfect Match" },
            { "round_tunnel",                     "Roll Out" },
            { "round_tail_tag",                   "Tail Tag" },

            { "round_egg_grab",                   "Egg Scramble" },
            { "round_fall_ball_60_players",       "Fall Ball" },
            { "round_ballhogs",                   "Hoarders" },
            { "round_hoops",                      "Hoopsie Daisy" },
            { "round_jinxed",                     "Jinxed" },
            { "round_rocknroll",                  "Rock'N'Roll" },
            { "round_conveyor_arena",             "Team Tail Tag" },

            { "round_fall_mountain_hub_complete", "Fall Mountain" },
            { "round_floor_fall",                 "Hex-A-Gone" },
            { "round_jump_showdown",              "Jump Showdown" },
            { "round_royal_rumble",               "Royal Fumble" },
        };

        public string Name { get; set; }
        public int Qualified { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
        public int Played { get; set; }
        public int Kudos { get; set; }
        public TimeSpan Fastest { get; set; }
        public TimeSpan Longest { get; set; }
        public int AveKudos { get { return Kudos / (Played == 0 ? 1 : Played); } }
        public LevelType Type;
        public TimeSpan AveDuration { get { return TimeSpan.FromSeconds((int)Duration.TotalSeconds / (Played == 0 ? 1 : Played)); } }
        public TimeSpan Duration;
        public string LevelName;
        public List<RoundInfo> Stats;

        public LevelStats(string levelName, LevelType type) {
            string name = null;
            if (DisplayNameLookup.TryGetValue(levelName, out name)) {
                Name = name;
            } else {
                Name = levelName;
            }
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
            Fastest = TimeSpan.Zero;
            Longest = TimeSpan.Zero;
            Stats.Clear();
        }
        public void Add(RoundInfo stat) {
            Stats.Add(stat);
            Played++;
            switch (stat.Tier) {
                case (int)QualifyTier.Gold:
                    Gold++;
                    break;
                case (int)QualifyTier.Silver:
                    Silver++;
                    break;
                case (int)QualifyTier.Bronze:
                    Bronze++;
                    break;
            }
            Kudos += stat.Kudos;
            TimeSpan finishTime = stat.Finish.GetValueOrDefault(stat.End) - stat.Start;
            if (stat.Finish.HasValue && finishTime.TotalSeconds > 1.1) {
                if (Fastest == TimeSpan.Zero || Fastest > finishTime) {
                    Fastest = finishTime;
                }
                if (Longest < finishTime) {
                    Longest = finishTime;
                }
            }
            Duration += stat.End - stat.Start;
            Qualified += stat.Qualified ? 1 : 0;
        }

        public override string ToString() {
            return $"{Name}: {Qualified} / {Played}";
        }
    }
}