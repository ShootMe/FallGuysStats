using System;
using System.Collections.Generic;
using LiteDB;
namespace FallGuysStats {
    public class RoundInfo {
        public ObjectId ID { get; set; }
        public int Profile { get; set; }
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
        public bool PrivateLobby { get; set; }
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public DateTime? Finish { get; set; } = null;
        public bool Crown { get; set; }
        public DateTime StartLocal;
        public DateTime EndLocal;
        public DateTime? FinishLocal;
        public DateTime ShowStart = DateTime.MinValue;
        public DateTime ShowEnd = DateTime.MinValue;
        public int GameDuration;
        public string SceneName;
        public bool Playing;
        private bool setLocalTime;

        public void ToLocalTime() {
            if (setLocalTime) { return; }
            setLocalTime = true;

            StartLocal = Start.ToLocalTime();
            EndLocal = End.ToLocalTime();
            if (Finish.HasValue) {
                FinishLocal = Finish.Value.ToLocalTime();
            }
        }
        public void VerifyName() {
            if (string.IsNullOrEmpty(SceneName)) { return; }

            string roundName;
            if (LevelStats.SceneToRound.TryGetValue(SceneName, out roundName)) {
                Name = roundName;
            }
        }
        public string VerifiedName() {
            if (string.IsNullOrEmpty(SceneName)) { return Name; }

            string roundName;
            if (LevelStats.SceneToRound.TryGetValue(SceneName, out roundName)) {
                return roundName;
            }
            return Name;
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
    public enum QualifyTier {
        None,
        Gold,
        Silver,
        Bronze
    }
    public class LevelStats {
        public static Dictionary<string, LevelStats> ALL = new Dictionary<string, LevelStats>(StringComparer.OrdinalIgnoreCase) {
            { "round_biggestfan",                 new LevelStats("Big Fans", LevelType.Race, false, 2) },
            { "round_door_dash",                  new LevelStats("Door Dash", LevelType.Race, false, 1) },
            { "round_gauntlet_02",                new LevelStats("Dizzy Heights", LevelType.Race, false, 1) },
            { "round_iceclimb",                   new LevelStats("Freezy Peak", LevelType.Race, false, 3) },
            { "round_dodge_fall",                 new LevelStats("Fruit Chute", LevelType.Race, false, 1) },
            { "round_chompchomp",                 new LevelStats("Gate Crash", LevelType.Race, false, 1) },
            { "round_gauntlet_01",                new LevelStats("Hit Parade", LevelType.Race, false, 1) },
            { "round_hoops_blockade_solo",        new LevelStats("Hoopsie Legends", LevelType.Hunt, false, 2) },
            { "round_gauntlet_04",                new LevelStats("Knight Fever", LevelType.Race, false, 2) },
            { "round_see_saw",                    new LevelStats("See Saw", LevelType.Race, false, 1) },
            { "round_skeefall",                   new LevelStats("Ski Fall", LevelType.Hunt, false, 3) },
            { "round_lava",                       new LevelStats("Slime Climb", LevelType.Race, false, 1) },
            { "round_tip_toe",                    new LevelStats("Tip Toe", LevelType.Race, false, 1) },
            { "round_gauntlet_05",                new LevelStats("Tundra Run", LevelType.Race, false, 3) },
            { "round_gauntlet_03",                new LevelStats("Whirlygig", LevelType.Race, false, 1) },
            { "round_wall_guys",                  new LevelStats("Wall Guys", LevelType.Race, false, 2) },

            { "round_block_party",                new LevelStats("Block Party", LevelType.Survival, false, 1) },
            { "round_jump_club",                  new LevelStats("Jump Club", LevelType.Survival, false, 1) },
            { "round_match_fall",                 new LevelStats("Perfect Match", LevelType.Survival, false, 1) },
            { "round_tunnel",                     new LevelStats("Roll Out", LevelType.Survival, false, 1) },
            { "round_tail_tag",                   new LevelStats("Tail Tag", LevelType.Survival, false, 1) },

            { "round_egg_grab",                   new LevelStats("Egg Scramble", LevelType.Team, false, 1) },
            { "round_egg_grab_02",                new LevelStats("Egg Siege", LevelType.Team, false, 2) },
            { "round_fall_ball_60_players",       new LevelStats("Fall Ball", LevelType.Team, false, 1) },
            { "round_ballhogs",                   new LevelStats("Hoarders", LevelType.Team, false, 1) },
            { "round_hoops",                      new LevelStats("Hoopsie Daisy", LevelType.Team, false, 1) },
            { "round_jinxed",                     new LevelStats("Jinxed", LevelType.Team, false, 1) },
            { "round_chicken_chase",              new LevelStats("Pegwin Pursuit", LevelType.Team, false, 3) },
            { "round_rocknroll",                  new LevelStats("Rock'N'Roll", LevelType.Team, false, 1) },
            { "round_snowy_scrap",                new LevelStats("Snowy Scrap", LevelType.Team, false, 3) },
            { "round_conveyor_arena",             new LevelStats("Team Tail Tag", LevelType.Team, false, 1) },

            { "round_fall_mountain_hub_complete", new LevelStats("Fall Mountain", LevelType.Race, true, 1) },
            { "round_floor_fall",                 new LevelStats("Hex-A-Gone", LevelType.Survival, true, 1) },
            { "round_jump_showdown",              new LevelStats("Jump Showdown", LevelType.Survival, true, 1) },
            { "round_tunnel_final",               new LevelStats("Roll Off", LevelType.Survival, true, 3) },
            { "round_royal_rumble",               new LevelStats("Royal Fumble", LevelType.Hunt, true, 1) },
            { "round_thin_ice",                   new LevelStats("Thin Ice", LevelType.Survival, true, 3) }
        };
        public static Dictionary<string, string> SceneToRound = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "FallGuy_BiggestFan",                "round_biggestfan" },
            { "FallGuy_DoorDash",                  "round_door_dash" },
            { "FallGuy_Gauntlet_02_01",            "round_gauntlet_02" },
            { "FallGuy_IceClimb_01",               "round_iceclimb" },
            { "FallGuy_DodgeFall",                 "round_dodge_fall" },
            { "FallGuy_ChompChomp_01",             "round_chompchomp" },
            { "FallGuy_Gauntlet_01",               "round_gauntlet_01" },
            { "FallGuy_Hoops_Blockade",            "round_hoops_blockade_solo" },
            { "FallGuy_Gauntlet_04",               "round_gauntlet_04" },
            { "FallGuy_SeeSaw_variant2",           "round_see_saw" },
            { "FallGuy_SkeeFall",                  "round_skeefall" },
            { "FallGuy_Lava_02",                   "round_lava" },
            { "FallGuy_TipToe",                    "round_tip_toe" },
            { "FallGuy_Gauntlet_05",               "round_gauntlet_05" },
            { "FallGuy_Gauntlet_03",               "round_gauntlet_03" },
            { "FallGuy_WallGuys",                  "round_wall_guys" },

            { "FallGuy_Block_Party",               "round_block_party" },
            { "FallGuy_JumpClub_01",               "round_jump_club" },
            { "FallGuy_MatchFall",                 "round_match_fall" },
            { "FallGuy_Tunnel_01",                 "round_tunnel" },
            { "FallGuy_TailTag_2",                 "round_tail_tag" },

            { "FallGuy_EggGrab",                   "round_egg_grab" },
            { "FallGuy_EggGrab_02",                "round_egg_grab_02" },
            { "FallGuy_FallBall_5",                "round_fall_ball_60_players" },
            { "FallGuy_BallHogs_01",               "round_ballhogs" },
            { "FallGuy_Hoops_01",                  "round_hoops" },
            { "FallGuy_TeamInfected",              "round_jinxed" },
            { "FallGuy_ChickenChase_01",           "round_chicken_chase" },
            { "FallGuy_RocknRoll",                 "round_rocknroll" },
            { "FallGuy_Snowy_Scrap",               "round_snowy_scrap" },
            { "FallGuy_ConveyorArena_01",          "round_conveyor_arena" },

            { "FallGuy_FallMountain_Hub_Complete", "round_fall_mountain_hub_complete" },
            { "FallGuy_FloorFall",                 "round_floor_fall" },
            { "FallGuy_JumpShowdown_01",           "round_jump_showdown" },
            { "FallGuy_Tunnel_Final",              "round_tunnel_final" },
            { "FallGuy_Arena_01",                  "round_royal_rumble" },
            { "FallGuy_ThinIce",                   "round_thin_ice" }
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
        public bool IsFinal;
        public TimeSpan AveDuration { get { return TimeSpan.FromSeconds((int)Duration.TotalSeconds / (Played == 0 ? 1 : Played)); } }
        public TimeSpan Duration;
        public List<RoundInfo> Stats;
        public int Season;

        public LevelStats(string levelName, LevelType type, bool isFinal, int season) {
            Name = levelName;
            Type = type;
            Season = season;
            IsFinal = isFinal;
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