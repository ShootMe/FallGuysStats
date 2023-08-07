using System;
using System.Drawing;
using System.Collections.Generic;
using LiteDB;
namespace FallGuysStats {
    public class RoundInfo : IComparable<RoundInfo> {
        public ObjectId ID { get; set; }
        public int Profile { get; set; }
        public string Name { get; set; }
        public int ShowID { get; set; }
        public string ShowNameId { get; set; }
        public bool UseShareCode { get; set; }
        public string CreativeShareCode { get; set; }
        public string CreativeStatus { get; set; }
        public string CreativeAuthor { get; set; }
        public string CreativeOnlinePlatformId { get; set; }
        //public string CreativeNicknameContentId { get; set; }
        //public string CreativeNameplateContentId { get; set; }
        public int CreativeVersion { get; set; }
        public string CreativeTitle { get; set; }
        public string CreativeDescription { get; set; }
        public int CreativeMaxPlayer { get; set; }
        public string CreativePlatformId { get; set; }
        public DateTime CreativeLastModifiedDate { get; set; } = DateTime.MinValue;
        public int CreativePlayCount { get; set; }
        public int CreativeQualificationPercent { get; set; }
        public int CreativeTimeLimitSeconds { get; set; }
        public string SessionId { get; set; }
        public int Round { get; set; }
        public int Position { get; set; }
        public int? Score { get; set; }
        public int Tier { get; set; }
        public bool Qualified { get; set; }
        public int Kudos { get; set; }
        public int Players { get; set; }
        public int PlayersPs4 { get; set; }
        public int PlayersPs5 { get; set; }
        public int PlayersXb1 { get; set; }
        public int PlayersXsx { get; set; }
        public int PlayersSw { get; set; }
        public int PlayersPc { get; set; }
        public int PlayersBots { get; set; }
        public int PlayersEtc { get; set; }
        public bool InParty { get; set; }
        public bool IsFinal { get; set; }
        public bool IsTeam { get; set; }
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
            if (this.setLocalTime) { return; }
            this.setLocalTime = true;

            this.StartLocal = this.Start.ToLocalTime();
            this.EndLocal = this.End.ToLocalTime();
            if (this.Finish.HasValue) {
                this.FinishLocal = this.Finish.Value.ToLocalTime();
            }
        }
        public void VerifyName() {
            if (string.IsNullOrEmpty(this.SceneName)) { return; }

            if (LevelStats.SceneToRound.TryGetValue(this.SceneName, out string roundName)) {
                this.Name = roundName;
            }
        }
        public string VerifiedName() {
            return string.IsNullOrEmpty(this.SceneName)
                ? this.Name
                : LevelStats.SceneToRound.TryGetValue(this.SceneName, out string roundName) ? roundName : this.Name;
        }
        public override string ToString() {
            return $"{this.Name}: Round={this.Round} Position={this.Position} Duration={this.End - this.Start} Kudos={this.Kudos}";
        }
        public override bool Equals(object obj) {
            return obj is RoundInfo info
                   && info.End == this.End
                   && info.Finish == this.Finish
                   && info.InParty == this.InParty
                   && info.Kudos == this.Kudos
                   && info.Players == this.Players
                   && info.PlayersPs4 == this.PlayersPs4
                   && info.PlayersPs5 == this.PlayersPs5
                   && info.PlayersXb1 == this.PlayersXb1
                   && info.PlayersXsx == this.PlayersXsx
                   && info.PlayersSw == this.PlayersSw
                   && info.PlayersPc == this.PlayersPc
                   && info.PlayersBots == this.PlayersBots
                   && info.PlayersEtc == this.PlayersEtc
                   && info.Position == this.Position
                   && info.Qualified == this.Qualified
                   && info.Round == this.Round
                   && info.Score == this.Score
                   && info.ShowID == this.ShowID
                   && info.Start == this.Start
                   && info.Tier == this.Tier
                   && info.Name == this.Name
                   && info.SessionId == this.SessionId
                   && info.UseShareCode == this.UseShareCode;
        }
        public override int GetHashCode() {
            return Name.GetHashCode() ^ ShowID ^ Round;
        }
        public int CompareTo(RoundInfo other) {
            int showCompare = ShowID.CompareTo(other.ShowID);
            return showCompare != 0 ? showCompare : Round.CompareTo(other.Round);
        }
    }
    public enum QualifyTier {
        Pink,
        Gold,
        Silver,
        Bronze
    }
    public class LevelStats {
        public static Dictionary<string, LevelStats> ALL = new Dictionary<string, LevelStats>(StringComparer.OrdinalIgnoreCase) {
            { "user_creative_race_round", new LevelStats("User Creative Race Round", LevelType.CreativeRace, true, false, 10, 0, 0, Properties.Resources.round_creative_icon, Properties.Resources.round_creative_big_icon) },
            
            { "wle_s10_orig_round_001",           new LevelStats("Beans Ahoy!", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_002",           new LevelStats("Airborne Antics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_003",           new LevelStats("Scythes & Roundabouts", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_004",           new LevelStats("Cardio Runners", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_005",           new LevelStats("Fan Flingers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_006",           new LevelStats("Uphill Struggle", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_007",           new LevelStats("Spinner Sprint", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_008",           new LevelStats("Lane Changers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_009",           new LevelStats("Gentle Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_012",           new LevelStats("Up & Down", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_013",           new LevelStats("Choo Choo Challenge", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_014",           new LevelStats("Runner Beans", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_015",           new LevelStats("Disc Dashers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_016",           new LevelStats("Two Faced", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_019",           new LevelStats("Blueberry Bombardment", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_020",           new LevelStats("Chuting Stars", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_021",           new LevelStats("Slimy Slopes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_022",           new LevelStats("Circuit Breakers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_023",           new LevelStats("Winding Walkways", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_026",           new LevelStats("Hyperlink Hijinks", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_027",           new LevelStats("Fan Frolics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_028",           new LevelStats("Windmill Road", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_029",           new LevelStats("Conveyor Clash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_032",           new LevelStats("Fortress Frolics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_033",           new LevelStats("Super Door Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_034",           new LevelStats("Spiral Of Woo", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_035",           new LevelStats("Tornado Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_036",           new LevelStats("Hopscotch Havoc", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_037",           new LevelStats("Beat Bouncers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_038",           new LevelStats("Blunder Bridges", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_039",           new LevelStats("Incline Rewind", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_040",           new LevelStats("Prismatic Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_041",           new LevelStats("Swept Away", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_042",           new LevelStats("Balancing Act", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_043",           new LevelStats("Trouble Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_044",           new LevelStats("Serpent Slalom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_045",           new LevelStats("Floorless", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_046",           new LevelStats("In The Cloud", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_047",           new LevelStats("Downstream Duel", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_048",           new LevelStats("Lost Palace", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_045_long",      new LevelStats("Floorless", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_003",           new LevelStats("Fall Speedway", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_004",           new LevelStats("Zig Zag Zoomies", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_005",           new LevelStats("Terrabyte Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_round_001",                new LevelStats("Digi Trek", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_002",                new LevelStats("Shortcut Links", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_003",                new LevelStats("Upload Heights", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_005",                new LevelStats("Data Streams", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_006",                new LevelStats("Gigabyte Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_007",                new LevelStats("Cube Corruption", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_008",                new LevelStats("Wham Bam Boom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_010",                new LevelStats("Pixel Hearts", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_011",                new LevelStats("Cyber Circuit", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_012",                new LevelStats("Boom Blaster Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk3_01",      new LevelStats("Cloudy Chaos", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_02",      new LevelStats("Door Game", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_03",      new LevelStats("Full Speed Sliding (FSS) - Jelly Road", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_04",      new LevelStats("Sky High Run", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_06",      new LevelStats("Spiral Upheaval", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_07",      new LevelStats("Creative Descent", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_08",      new LevelStats("Rainbow Slide", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_09",      new LevelStats("Fragrant Trumpet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_10",      new LevelStats("Bridges That Don't Like You", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_11",      new LevelStats("Rainbow Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_12",      new LevelStats("Variable Valley", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_fp2_wk6_01",                   new LevelStats("Broken Course", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_14",      new LevelStats("Tower of Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_15",      new LevelStats("Parkour Party", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_16",      new LevelStats("Catastrophe Climb", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_17",      new LevelStats("Yeet Golf", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_18",      new LevelStats("Hill of Fear", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_19",      new LevelStats("Sky Time", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_20",      new LevelStats("Ezz Map", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk4_01",      new LevelStats("Slippery Stretch", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_02",      new LevelStats("Ball 'N Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_03",      new LevelStats("Rowdy Cloudy", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_05",      new LevelStats("Vertiginous Steps", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_06",      new LevelStats("Topsie Tursie", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_07",      new LevelStats("Arcade Assault", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_08",      new LevelStats("The Eight Pit Trials", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_09",      new LevelStats("Green Beans", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_10",      new LevelStats("Hop Hill", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_11",      new LevelStats("Quick Sliders", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_12",      new LevelStats("Split Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_13",      new LevelStats("Piso Resbaloso", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_15",      new LevelStats("Snowboard Street", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_18",      new LevelStats("House Invasion", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_19",      new LevelStats("SOLO FULL-TILT RAGE", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_20",      new LevelStats("Terminal Slime-ocity", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_21",      new LevelStats("Spin", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_22",      new LevelStats("Lane Changers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "wle_s10_player_round_wk5_01",      new LevelStats("Block Park", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_02",      new LevelStats("The Drummatical Story", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_03",      new LevelStats("Digital Temple", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_04",      new LevelStats("Tower Escape", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_05",      new LevelStats("Tower Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_06",      new LevelStats("Gpu Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_07",      new LevelStats("Looooping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_08",      new LevelStats("Rad Bean Skatepark", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_10",      new LevelStats("Siank Arena", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_11",      new LevelStats("Pro Players Only", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_12",      new LevelStats("Extreme Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_13",      new LevelStats("Dessert Village", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_14",      new LevelStats("Extreme Trampoline Jumping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_15",      new LevelStats("Beast Route", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_16",      new LevelStats("METROPOLIS", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_17",      new LevelStats("Big Bookcase", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_18",      new LevelStats("Digital Doom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk6_01",      new LevelStats("Hammer Heaven", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_02",      new LevelStats("RISKY ROUTES", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_03",      new LevelStats("Castle Rush", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_04",      new LevelStats("Chaotic Race", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_05",      new LevelStats("FREEFALL TOWER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_06",      new LevelStats("西西的天空之城 Castle in the Sky", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_08",      new LevelStats("Flower Power", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_09",      new LevelStats("Dimension Explorer", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_10",      new LevelStats("Forked Passage", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_12",      new LevelStats("The Bee Hive", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_13",      new LevelStats("Yeets & Ladders", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_14",      new LevelStats("Snek", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_15",      new LevelStats("SCHOOL OF FISH", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_17",      new LevelStats("Slippery Helixes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_18",      new LevelStats("Recess", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_19",      new LevelStats("Parrot river", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp3_10_01",            new LevelStats("When Nature Falls", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_02",            new LevelStats("The Slippery Conveyor", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_03",            new LevelStats("The Slime Trials", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_04",            new LevelStats("Friendly Obstacles", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_05",            new LevelStats("Climb and Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_06",            new LevelStats("Stairs and some other things", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_07",            new LevelStats("Meowgical World", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_08",            new LevelStats("Polluelo Speed", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_09",            new LevelStats("Pixel Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_10",            new LevelStats("Total Madness", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_11",            new LevelStats("The Abstract Maze", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_12",            new LevelStats("Fan Off", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_13",            new LevelStats("cloud highway", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_14",            new LevelStats("はねるの！？トビラ（Door Bounce）", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_15",            new LevelStats("Speedrunners be like", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_16",            new LevelStats("Tumble Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_17",            new LevelStats("Silver's Snake Run", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_18",            new LevelStats("Now Boarding", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_19",            new LevelStats("Slime Scale", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_20",            new LevelStats("TUMBLEDOWN MINESHAFT", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_21",            new LevelStats("Circuito CHILL 1", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_22",            new LevelStats("STUMBLE SLIDER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_23",            new LevelStats("Controlled Chaos", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_24",            new LevelStats("Xtreme Jumping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_25",            new LevelStats("Odin", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_26",            new LevelStats("Ciudad nube", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_27",            new LevelStats("Bean Voyage", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_28",            new LevelStats("SLIP-SAW", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_29",            new LevelStats("Bbq bacon burger", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp4_10_08",            new LevelStats("Wall Breaker", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_11",            new LevelStats("HOARDER BLOCKS", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_12",            new LevelStats("Chickens run away", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_20",            new LevelStats("Co-op and CO", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp3_08_01",            new LevelStats("Grabbers Territory", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_02",            new LevelStats("A Way Out", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_03",            new LevelStats("Wall Block", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_04",            new LevelStats("The dream island", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_05",            new LevelStats("Rainbow pulsion", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_06",            new LevelStats("WHIPPITY WOPPITY", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_09",            new LevelStats("Big Fans Box Challenge", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_10",            new LevelStats("Crazy boxes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_13",            new LevelStats("Season 1 Race Mashup", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_14",            new LevelStats("Flippy Hoopshots", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_15",            new LevelStats("Stumble Teams", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_16",            new LevelStats("Twisting Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_17",            new LevelStats("PUSH 'N' PULL", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_18",            new LevelStats("The Rising Blocks", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_19",            new LevelStats("Puzzle Blokies Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_01",            new LevelStats("The up tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_02",            new LevelStats("Short shuriken", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_03",            new LevelStats("Les mêmes mécaniques de + en + dure", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_04",            new LevelStats("Digi-Lily Sliding", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_05",            new LevelStats("STUMBLE MEDIEVAL TOWER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_06",            new LevelStats("Random Heights", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_07",            new LevelStats("Climb scramble", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_08",            new LevelStats("Collide Gaming", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_09",            new LevelStats("Very Compressed Level", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_01",          new LevelStats("Slippery Slope", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_02",          new LevelStats("The Most Hardest Fall Guys LEVEL", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_03",          new LevelStats("Free Falling", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_04",          new LevelStats("Conveyor Problems", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_05",          new LevelStats("Clocktower Climb", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_06",          new LevelStats("Savour Your Happiness", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_0_01",        new LevelStats("Pastel Paradise", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_09_01",            new LevelStats("Crate Collector", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_09_02",            new LevelStats("Dribble Drills", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_bt_round_001",             new LevelStats("Push Ups", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_002",             new LevelStats("Heave & Haul", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_003",             new LevelStats("Stepping Stones", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_004",             new LevelStats("Double Trouble", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_mrs_bagel_opener_1",           new LevelStats("Tunnel of Love", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_2",           new LevelStats("Pink Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_3",           new LevelStats("Prideful Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_4",           new LevelStats("Coming Together", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_1",           new LevelStats("Clifftop Capers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_2",           new LevelStats("Waveway Splits", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_3",           new LevelStats("In the Groove", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_4",           new LevelStats("Heartfall Heat", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_final_1",            new LevelStats("Rainbow Rise", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_final_2",            new LevelStats("Out and About", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "wle_s10_orig_round_010",           new LevelStats("Square Up", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_011",           new LevelStats("Slide Showdown", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_017",           new LevelStats("Bellyflop Battlers", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_018",           new LevelStats("Apples & Oranges", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_024",           new LevelStats("Wooseleum", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_025",           new LevelStats("Mount Boom", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_030",           new LevelStats("Mega Monument", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_031",           new LevelStats("Transfer Turnpike", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_004",                new LevelStats("Parkour Panic", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_009",                new LevelStats("Firewall Finale", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "round_biggestfan",                 new LevelStats("Big Fans", LevelType.Race, false, false, 2, 210, 120, Properties.Resources.round_big_fans_icon, Properties.Resources.round_big_fans_big_icon) },
            { "round_satellitehoppers_almond",    new LevelStats("Cosmic Highway", LevelType.Race, false, false, 8, 180, 180, Properties.Resources.round_cosmic_highway_icon, Properties.Resources.round_cosmic_highway_big_icon) },
            { "round_gauntlet_02",                new LevelStats("Dizzy Heights", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_dizzy_heights_icon, Properties.Resources.round_dizzy_heights_big_icon) },
            { "round_door_dash",                  new LevelStats("Door Dash", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_door_dash_icon, Properties.Resources.round_door_dash_big_icon) },
            { "round_iceclimb",                   new LevelStats("Freezy Peak", LevelType.Race, false, false, 3, 180, 120, Properties.Resources.round_freezy_peak_icon, Properties.Resources.round_freezy_peak_big_icon) },
            { "round_dodge_fall",                 new LevelStats("Fruit Chute", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_fruit_chute_icon, Properties.Resources.round_fruit_chute_big_icon) },
            { "round_see_saw_360",                new LevelStats("Full Tilt", LevelType.Race, false, false, 6, 180, 180, Properties.Resources.round_full_tilt_icon, Properties.Resources.round_full_tilt_big_icon) },
            { "round_chompchomp",                 new LevelStats("Gate Crash", LevelType.Race, false, false, 1, 300, 120, Properties.Resources.round_gate_crash_icon, Properties.Resources.round_gate_crash_big_icon) },
            { "round_gauntlet_01",                new LevelStats("Hit Parade", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_hit_parade_icon, Properties.Resources.round_hit_parade_big_icon) },
            { "round_gauntlet_04",                new LevelStats("Knight Fever", LevelType.Race, false, false, 2, 180, 120, Properties.Resources.round_knight_fever_icon, Properties.Resources.round_knight_fever_big_icon) },
            { "round_drumtop",                    new LevelStats("Lily Leapers", LevelType.Race, false, false, 5, 300, 140, Properties.Resources.round_lily_leapers_icon, Properties.Resources.round_lily_leapers_big_icon) },
            { "round_gauntlet_08",                new LevelStats("Party Promenade", LevelType.Race, false, false, 6, 300, 120, Properties.Resources.round_party_promenade_icon, Properties.Resources.round_party_promenade_big_icon) },
            { "round_pipedup_s6_launch",          new LevelStats("Pipe Dream", LevelType.Race, false, false, 6, 300, 150, Properties.Resources.round_pipe_dream_icon, Properties.Resources.round_pipe_dream_big_icon) },
            { "round_follow_the_line",            new LevelStats("Puzzle Path", LevelType.Race, false, false, 9, 150, 150, Properties.Resources.round_puzzle_path_icon, Properties.Resources.round_puzzle_path_big_icon) },
            { "round_tunnel_race",                new LevelStats("Roll On", LevelType.Race, false, false, 4, 120, 120, Properties.Resources.round_roll_on_icon, Properties.Resources.round_roll_on_big_icon) },
            { "round_see_saw",                    new LevelStats("See Saw", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_see_saw_icon, Properties.Resources.round_see_saw_big_icon) },
            { "round_shortcircuit",               new LevelStats("Short Circuit", LevelType.Race, false, false, 4, 300, 300, Properties.Resources.round_short_circuit_icon, Properties.Resources.round_short_circuit_big_icon) },
            { "round_skeefall",                   new LevelStats("Ski Fall", LevelType.Race, false, false, 3, 300, 300, Properties.Resources.round_ski_fall_icon, Properties.Resources.round_ski_fall_big_icon) },
            { "round_gauntlet_06",                new LevelStats("Skyline Stumble", LevelType.Race, false, false, 4, 180, 120, Properties.Resources.round_skyline_stumble_icon, Properties.Resources.round_skyline_stumble_big_icon) },
            { "round_lava",                       new LevelStats("Slime Climb", LevelType.Race, false, false, 1, 140, 140, Properties.Resources.round_slime_climb_icon, Properties.Resources.round_slime_climb_big_icon) },
            { "round_gauntlet_10_almond",         new LevelStats("Space Race", LevelType.Race, false, false, 8, 150, 150, Properties.Resources.round_space_race_icon, Properties.Resources.round_space_race_big_icon) },
            { "round_short_circuit_2_symphony_launch_show", new LevelStats("Speed Circuit", LevelType.Race, false, false, 7, 180, 180, Properties.Resources.round_speed_circuit_icon, Properties.Resources.round_speed_circuit_big_icon) },
            { "round_slide_chute",                new LevelStats("Speed Slider", LevelType.Race, false, false, 9, 165, 120, Properties.Resources.round_speed_slider_icon, Properties.Resources.round_speed_slider_big_icon) },
            { "round_starlink_almond",            new LevelStats("Starchart", LevelType.Race, false, false, 8, 150, 150, Properties.Resources.round_starchart_icon, Properties.Resources.round_starchart_big_icon) },
            { "round_slimeclimb_2",               new LevelStats("The Slimescraper", LevelType.Race, false, false, 4, 190, 190, Properties.Resources.round_the_slimescraper_icon, Properties.Resources.round_the_slimescraper_big_icon) },
            { "round_gauntlet_03",                new LevelStats("The Whirlygig", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_the_whirlygig_icon, Properties.Resources.round_the_whirlygig_big_icon) },
            { "round_tip_toe",                    new LevelStats("Tip Toe", LevelType.Race, false, false, 1, 300, 120, Properties.Resources.round_tip_toe_icon, Properties.Resources.round_tip_toe_big_icon) },
            { "round_gauntlet_09_symphony_launch_show", new LevelStats("Track Attack", LevelType.Race, false, false, 7, 90, 90, Properties.Resources.round_track_attack_icon, Properties.Resources.round_track_attack_big_icon) },
            { "round_gauntlet_07",                new LevelStats("Treetop Tumble", LevelType.Race, false, false, 5, 180, 120, Properties.Resources.round_treetop_tumble_icon, Properties.Resources.round_treetop_tumble_big_icon) },
            { "round_gauntlet_05",                new LevelStats("Tundra Run", LevelType.Race, false, false, 3, 180, 120, Properties.Resources.round_tundra_run_icon, Properties.Resources.round_tundra_run_big_icon) },
            { "round_wall_guys",                  new LevelStats("Wall Guys", LevelType.Race, false, false, 2, 300, 120, Properties.Resources.round_wall_guys_icon, Properties.Resources.round_wall_guys_big_icon) },

            { "round_airtime",                    new LevelStats("Airtime", LevelType.Hunt, false, false, 6, 300, 300, Properties.Resources.round_airtime_icon, Properties.Resources.round_airtime_big_icon) },
            { "round_bluejay",                    new LevelStats("Bean Hill Zone", LevelType.Hunt, false, false, 7, 300, 300, Properties.Resources.round_bean_hill_zone_icon, Properties.Resources.round_bean_hill_zone_big_icon) },
            { "round_hoops_revenge_symphony_launch_show", new LevelStats("Bounce Party", LevelType.Hunt, false, false, 7, 300, 300, Properties.Resources.round_bounce_party_icon, Properties.Resources.round_bounce_party_big_icon) },
            { "round_king_of_the_hill",           new LevelStats("Bubble Trouble", LevelType.Hunt, false, false, 5, 300, 300, Properties.Resources.round_bubble_trouble_icon, Properties.Resources.round_bubble_trouble_big_icon) },
            { "round_1v1_button_basher",          new LevelStats("Button Bashers", LevelType.Hunt, false, false, 4, 90, 90, Properties.Resources.round_button_bashers_icon, Properties.Resources.round_button_bashers_big_icon) },
            { "round_ffa_button_bashers_squads_almond", new LevelStats("Frantic Factory", LevelType.Hunt, false, false, 8, 300, 300, Properties.Resources.round_frantic_factory_icon, Properties.Resources.round_frantic_factory_big_icon) },
            { "round_slippy_slide",               new LevelStats("Hoop Chute", LevelType.Hunt, false, false, 9, 180, 180, Properties.Resources.round_hoop_chute_icon, Properties.Resources.round_hoop_chute_big_icon) },
            { "round_hoops_blockade_solo",        new LevelStats("Hoopsie Legends", LevelType.Hunt, false, false, 2, 300, 300, Properties.Resources.round_hoopsie_legends_icon, Properties.Resources.round_hoopsie_legends_big_icon) },
            { "round_follow-the-leader_s6_launch",new LevelStats("Leading Light", LevelType.Hunt, false, false, 6, 300, 300, Properties.Resources.round_leading_light_icon, Properties.Resources.round_leading_light_big_icon) },
            { "round_penguin_solos",              new LevelStats("Pegwin Pool Party", LevelType.Hunt, false, false, 5, 300, 300, Properties.Resources.round_pegwin_pool_party_icon, Properties.Resources.round_pegwin_pool_party_big_icon) },
            { "round_tail_tag",                   new LevelStats("Tail Tag", LevelType.Hunt, false, false, 1, 90, 90, Properties.Resources.round_tail_tag_icon, Properties.Resources.round_tail_tag_big_icon) },
            { "round_1v1_volleyfall_symphony_launch_show", new LevelStats("Volleyfall", LevelType.Hunt, false, false, 7, 100, 100, Properties.Resources.round_volleyfall_icon, Properties.Resources.round_volleyfall_big_icon) },

            { "round_fruitpunch_s4_show",         new LevelStats("Big Shots", LevelType.Survival, false, false, 4, 90, 90, Properties.Resources.round_big_shots_icon, Properties.Resources.round_big_shots_big_icon) },
            { "round_blastballruins",             new LevelStats("Blastlantis", LevelType.Survival, false, false, 9, 270, 150, Properties.Resources.round_blastlantis_icon, Properties.Resources.round_blastlantis_big_icon) },
            { "round_block_party",                new LevelStats("Block Party", LevelType.Survival, false, false, 1, 105, 105, Properties.Resources.round_block_party_icon, Properties.Resources.round_block_party_big_icon) },
            { "round_hoverboardsurvival_s4_show", new LevelStats("Hoverboard Heroes", LevelType.Survival, false, false, 4, 140, 140, Properties.Resources.round_hoverboard_heroes_icon, Properties.Resources.round_hoverboard_heroes_big_icon) },
            { "round_hoverboardsurvival2_almond", new LevelStats("Hyperdrive Heroes", LevelType.Survival, false, false, 8, 170, 170, Properties.Resources.round_hyperdrive_heroes_icon, Properties.Resources.round_hyperdrive_heroes_big_icon) },
            { "round_jump_club",                  new LevelStats("Jump Club", LevelType.Survival, false, false, 1, 90, 90, Properties.Resources.round_jump_club_icon, Properties.Resources.round_jump_club_big_icon) },
            { "round_tunnel",                     new LevelStats("Roll Out", LevelType.Survival, false, false, 1, 150, 90, Properties.Resources.round_roll_out_icon, Properties.Resources.round_roll_out_big_icon) },
            { "round_snowballsurvival",           new LevelStats("Snowball Survival", LevelType.Survival, false, false, 3, 60, 60, Properties.Resources.round_snowball_survival_icon, Properties.Resources.round_snowball_survival_big_icon) },
            { "round_robotrampage_arena_2",       new LevelStats("Stompin' Ground", LevelType.Survival, false, false, 5, 70, 70, Properties.Resources.round_stompin_ground_icon, Properties.Resources.round_stompin_ground_big_icon) },
            { "round_spin_ring_symphony_launch_show", new LevelStats("The Swiveller", LevelType.Survival, false, false, 7, 180, 180, Properties.Resources.round_the_swiveller_icon, Properties.Resources.round_the_swiveller_big_icon) },
            
            { "round_match_fall",                 new LevelStats("Perfect Match", LevelType.Logic, false, false, 1, 80, 80, Properties.Resources.round_perfect_match_icon, Properties.Resources.round_perfect_match_big_icon) },
            { "round_pixelperfect_almond",        new LevelStats("Pixel Painters", LevelType.Logic, false, false, 8, 180, 180, Properties.Resources.round_pixel_painters_icon, Properties.Resources.round_pixel_painters_big_icon) },
            { "round_fruit_bowl",                 new LevelStats("Sum Fruit", LevelType.Logic, false, false, 5, 100, 100, Properties.Resources.round_sum_fruit_icon, Properties.Resources.round_sum_fruit_big_icon) },

            { "round_basketfall_s4_show",         new LevelStats("Basketfall", LevelType.Team, false, false, 4, 90, 90, Properties.Resources.round_basketfall_icon, Properties.Resources.round_basketfall_big_icon) },
            { "round_egg_grab",                   new LevelStats("Egg Scramble", LevelType.Team, false, false, 1, 120, 120, Properties.Resources.round_egg_scramble_icon, Properties.Resources.round_egg_scramble_big_icon) },
            { "round_egg_grab_02",                new LevelStats("Egg Siege", LevelType.Team, false, false, 2, 120, 120, Properties.Resources.round_egg_siege_icon, Properties.Resources.round_egg_siege_big_icon) },
            { "round_fall_ball_60_players",       new LevelStats("Fall Ball", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_fall_ball_icon, Properties.Resources.round_fall_ball_big_icon) },
            { "round_ballhogs",                   new LevelStats("Hoarders", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_hoarders_icon, Properties.Resources.round_hoarders_big_icon) },
            { "round_hoops",                      new LevelStats("Hoopsie Daisy", LevelType.Team, false, false, 1, 120, 120, Properties.Resources.round_hoopsie_daisy_icon, Properties.Resources.round_hoopsie_daisy_big_icon) },
            { "round_jinxed",                     new LevelStats("Jinxed", LevelType.Team, false, false, 1, 300, 300, Properties.Resources.round_jinxed_icon, Properties.Resources.round_jinxed_big_icon) },
            { "round_chicken_chase",              new LevelStats("Pegwin Pursuit", LevelType.Team, false, false, 3, 120, 120, Properties.Resources.round_pegwin_pursuit_icon, Properties.Resources.round_pegwin_pursuit_big_icon) },
            { "round_territory_control_s4_show",  new LevelStats("Power Trip", LevelType.Team, false, false, 4, 100, 100, Properties.Resources.round_power_trip_icon, Properties.Resources.round_power_trip_big_icon) },
            { "round_rocknroll",                  new LevelStats("Rock 'n' Roll", LevelType.Team, false, false, 1, 180, 180, Properties.Resources.round_rock_n_roll_icon, Properties.Resources.round_rock_n_roll_big_icon) },
            { "round_snowy_scrap",                new LevelStats("Snowy Scrap", LevelType.Team, false, false, 3, 180, 180, Properties.Resources.round_snowy_scrap_icon, Properties.Resources.round_snowy_scrap_big_icon) },
            { "round_conveyor_arena",             new LevelStats("Team Tail Tag", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_team_tail_tag_icon, Properties.Resources.round_team_tail_tag_big_icon) },
            
            { "round_invisibeans",                new LevelStats("Sweet Thieves", LevelType.Invisibeans, false, false, 6, 180, 180, Properties.Resources.round_sweet_thieves_icon, Properties.Resources.round_sweet_thieves_big_icon) },
            { "round_pumpkin_pie",                new LevelStats("Treat Thieves", LevelType.Invisibeans, false, false, 8, 180, 180, Properties.Resources.round_treat_thieves_icon, Properties.Resources.round_treat_thieves_big_icon) },

            { "round_blastball_arenasurvival_symphony_launch_show", new LevelStats("Blast Ball", LevelType.Survival, false, true, 7, 270, 270, Properties.Resources.round_blast_ball_icon, Properties.Resources.round_blast_ball_big_icon) },
            { "round_fall_mountain_hub_complete", new LevelStats("Fall Mountain", LevelType.Race, false, true, 1, 300, 300, Properties.Resources.round_fall_mountain_icon, Properties.Resources.round_fall_mountain_big_icon) },
            { "round_floor_fall",                 new LevelStats("Hex-A-Gone", LevelType.Survival, false, true, 1, 300, 300, Properties.Resources.round_hex_a_gone_icon, Properties.Resources.round_hex_a_gone_big_icon) },
            { "round_hexaring_symphony_launch_show", new LevelStats("Hex-A-Ring", LevelType.Survival, false, true, 7, 300, 300, Properties.Resources.round_hex_a_ring_icon, Properties.Resources.round_hex_a_ring_big_icon) },
            { "round_hexsnake_almond",            new LevelStats("Hex-A-Terrestrial", LevelType.Survival, false, true, 8, 300, 300, Properties.Resources.round_hex_a_terrestrial_icon, Properties.Resources.round_hex_a_terrestrial_big_icon) },
            { "round_jump_showdown",              new LevelStats("Jump Showdown", LevelType.Survival, false, true, 1, 300, 300, Properties.Resources.round_jump_showdown_icon, Properties.Resources.round_jump_showdown_big_icon) },
            { "round_kraken_attack",              new LevelStats("Kraken Slam", LevelType.Survival, false, true, 9, 300, 300, Properties.Resources.round_kraken_slam_icon, Properties.Resources.round_kraken_slam_big_icon) },
            { "round_crown_maze",                 new LevelStats("Lost Temple", LevelType.Race, false, true, 5, 300, 300, Properties.Resources.round_lost_temple_icon, Properties.Resources.round_lost_temple_big_icon) },
            { "round_tunnel_final",               new LevelStats("Roll Off", LevelType.Survival, false, true, 3, 300, 300, Properties.Resources.round_roll_off_icon, Properties.Resources.round_roll_off_big_icon) },
            { "round_royal_rumble",               new LevelStats("Royal Fumble", LevelType.Hunt, false, true, 1, 90, 90, Properties.Resources.round_royal_fumble_icon, Properties.Resources.round_royal_fumble_big_icon) },
            { "round_thin_ice",                   new LevelStats("Thin Ice", LevelType.Survival, false, true, 3, 300, 300, Properties.Resources.round_thin_ice_icon, Properties.Resources.round_thin_ice_big_icon) },
            { "round_tiptoefinale_almond",        new LevelStats("Tip Toe Finale", LevelType.Survival, false, true, 8, 300, 300, Properties.Resources.round_tip_toe_finale_icon, Properties.Resources.round_tip_toe_finale_big_icon) },
        };
        public static Dictionary<string, string> SceneToRound = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "FallGuy_DoorDash",                  "round_door_dash" },
            { "FallGuy_Gauntlet_02_01",            "round_gauntlet_02" },
            { "FallGuy_DodgeFall",                 "round_dodge_fall" },
            { "FallGuy_ChompChomp_01",             "round_chompchomp" },
            { "FallGuy_Gauntlet_01",               "round_gauntlet_01" },
            { "FallGuy_SeeSaw_variant2",           "round_see_saw" },
            { "FallGuy_Lava_02",                   "round_lava" },
            { "FallGuy_TipToe",                    "round_tip_toe" },
            { "FallGuy_Gauntlet_03",               "round_gauntlet_03" },
            { "FallGuy_Block_Party",               "round_block_party" },
            { "FallGuy_JumpClub_01",               "round_jump_club" },
            { "FallGuy_MatchFall",                 "round_match_fall" },
            { "FallGuy_Tunnel_01",                 "round_tunnel" },
            { "FallGuy_TailTag_2",                 "round_tail_tag" },
            { "FallGuy_EggGrab",                   "round_egg_grab" },
            { "FallGuy_FallBall_5",                "round_fall_ball_60_players" },
            { "FallGuy_BallHogs_01",               "round_ballhogs" },
            { "FallGuy_Hoops_01",                  "round_hoops" },
            { "FallGuy_TeamInfected",              "round_jinxed" },
            { "FallGuy_RocknRoll",                 "round_rocknroll" },
            { "FallGuy_ConveyorArena_01",          "round_conveyor_arena" },
            { "FallGuy_FallMountain_Hub_Complete", "round_fall_mountain_hub_complete" },
            { "FallGuy_FloorFall",                 "round_floor_fall" },
            { "FallGuy_JumpShowdown_01",           "round_jump_showdown" },
            { "FallGuy_Arena_01",                  "round_royal_rumble" },

            { "FallGuy_BiggestFan",                "round_biggestfan" },
            { "FallGuy_Hoops_Blockade",            "round_hoops_blockade_solo" },
            { "FallGuy_Gauntlet_04",               "round_gauntlet_04" },
            { "FallGuy_WallGuys",                  "round_wall_guys" },
            { "FallGuy_EggGrab_02",                "round_egg_grab_02" },

            { "FallGuy_IceClimb_01",               "round_iceclimb" },
            { "FallGuy_SkeeFall",                  "round_skeefall" },
            { "FallGuy_Gauntlet_05",               "round_gauntlet_05" },
            { "FallGuy_SnowballSurvival",          "round_snowballsurvival" },
            { "FallGuy_ChickenChase_01",           "round_chicken_chase" },
            { "FallGuy_Snowy_Scrap",               "round_snowy_scrap" },
            { "FallGuy_Tunnel_Final",              "round_tunnel_final" },
            { "FallGuy_ThinIce",                   "round_thin_ice" },

            { "FallGuy_1v1_ButtonBasher",          "round_1v1_button_basher" },
            { "FallGuy_Tunnel_Race_01",            "round_tunnel_race" },
            { "FallGuy_ShortCircuit",              "round_shortcircuit" },
            { "FallGuy_Gauntlet_06",               "round_gauntlet_06" },
            { "FallGuy_SlimeClimb_2",              "round_slimeclimb_2" },
            { "FallGuy_FruitPunch",                "round_fruitpunch_s4_show" },
            { "FallGuy_HoverboardSurvival",        "round_hoverboardsurvival_s4_show" },
            { "FallGuy_Basketfall_01",             "round_basketfall_s4_show" },
            { "FallGuy_TerritoryControl_v2",       "round_territory_control_s4_show" },

            { "FallGuy_KingOfTheHill2",            "round_king_of_the_hill" },
            { "FallGuy_DrumTop",                   "round_drumtop" },
            { "FallGuy_Penguin_Solos",             "round_penguin_solos" },
            { "FallGuy_Gauntlet_07",               "round_gauntlet_07" },
            { "FallGuy_RobotRampage_Arena2",       "round_robotrampage_arena_2" },
            { "FallGuy_FruitBowl",                 "round_fruit_bowl" },
            { "FallGuy_Crown_Maze_Topdown",        "round_crown_maze" },

            { "FallGuy_Airtime",                   "round_airtime" },
            { "FallGuy_SeeSaw360",                 "round_see_saw_360" },
            { "FallGuy_FollowTheLeader",           "round_follow-the-leader_s6_launch" },
            { "FallGuy_Gauntlet_08",               "round_gauntlet_08" },
            { "FallGuy_PipedUp",                   "round_pipedup_s6_launch" },
            { "FallGuy_Invisibeans",               "round_invisibeans" },

            { "FallGuy_BlueJay",                   "round_bluejay" },
            { "FallGuy_HoopsRevenge",              "round_hoops_revenge_symphony_launch_show" },
            { "FallGuy_ShortCircuit2",             "round_short_circuit_2_symphony_launch_show" },
            { "FallGuy_Gauntlet_09",               "round_gauntlet_09_symphony_launch_show" },
            { "FallGuy_SpinRing",                  "round_spin_ring_symphony_launch_show" },
            { "FallGuy_1v1_Volleyfall",            "round_1v1_volleyfall_symphony_launch_show" },
            { "FallGuy_BlastBall_ArenaSurvival",   "round_blastball_arenasurvival_symphony_launch_show" },
            { "FallGuy_HexARing",                  "round_hexaring_symphony_launch_show" },

            { "FallGuy_SatelliteHoppers",          "round_satellitehoppers_almond" },
            { "FallGuy_FFA_Button_Bashers",        "round_ffa_button_bashers_squads_almond" },
            { "FallGuy_Hoverboard_Survival_2",     "round_hoverboardsurvival2_almond" },
            { "FallGuy_PixelPerfect",              "round_pixelperfect_almond" },
            { "FallGuy_Gauntlet_10",               "round_gauntlet_10_almond" },
            { "FallGuy_Starlink",                  "round_starlink_almond" },
            { "FallGuy_HexSnake",                  "round_hexsnake_almond" },
            { "FallGuy_Tip_Toe_Finale",            "round_tiptoefinale_almond" },

            { "FallGuy_BlastBallRuins",            "round_blastballruins" },
            { "FallGuy_FollowTheLine",             "round_follow_the_line" },
            { "FallGuy_Kraken_Attack",             "round_kraken_attack" },
            { "FallGuy_SlippySlide",               "round_slippy_slide" },
            { "FallGuy_SlideChute",                "round_slide_chute" },
            
            { "FallGuy_UseShareCode",              "user_creative_race_round" },
        };
        public Image RoundIcon { get; set; }
        public Image RoundBigIcon { get; set; }
        public string Name { get; set; }
        public int Qualified { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
        public int Played { get; set; }
        public int Kudos { get; set; }
        public TimeSpan Fastest { get; set; }
        public TimeSpan Longest { get; set; }
        public int AveKudos { get { return this.Kudos / (this.Played == 0 ? 1 : this.Played); } }
        public TimeSpan AveDuration { get { return TimeSpan.FromSeconds((int)this.Duration.TotalSeconds / (this.Played == 0 ? 1 : this.Played)); } }
        public TimeSpan AveFinish { get { return TimeSpan.FromSeconds((double)this.FinishTime.TotalSeconds / (this.FinishedCount == 0 ? 1 : this.FinishedCount)); } }
        public LevelType Type;
        public bool IsCreative;
        public bool IsFinal;
        public int TimeLimitSeconds;
        public int TimeLimitSecondsForSquad;
        
        public TimeSpan Duration;
        public TimeSpan FinishTime;
        public List<RoundInfo> Stats;
        public int Season;
        public int FinishedCount;

        public LevelStats(string levelName, LevelType type, bool isCreative, bool isFinal, int season, int timeLimitSeconds, int timeLimitSecondsForSquad, Image roundIcon, Image roundBigIcon) {
            this.RoundIcon = roundIcon;
            this.RoundBigIcon = roundBigIcon;
            this.Name = levelName;
            this.Type = type;
            this.Season = season;
            this.IsCreative = isCreative;
            this.IsFinal = isFinal;
            this.TimeLimitSeconds = timeLimitSeconds;
            this.TimeLimitSecondsForSquad = timeLimitSecondsForSquad;
            this.Stats = new List<RoundInfo>();
            this.Clear();
        }
        public void Clear() {
            this.Qualified = 0;
            this.Gold = 0;
            this.Silver = 0;
            this.Bronze = 0;
            this.Played = 0;
            this.Kudos = 0;
            this.FinishedCount = 0;
            this.Duration = TimeSpan.Zero;
            this.FinishTime = TimeSpan.Zero;
            this.Fastest = TimeSpan.Zero;
            this.Longest = TimeSpan.Zero;
            this.Stats.Clear();
        }
        public void Add(RoundInfo stat) {
            this.Stats.Add(stat);
            if (!stat.PrivateLobby) {
                this.Played++;

                switch (stat.Tier) {
                    case (int)QualifyTier.Gold:
                        this.Gold++;
                        break;
                    case (int)QualifyTier.Silver:
                        this.Silver++;
                        break;
                    case (int)QualifyTier.Bronze:
                        this.Bronze++;
                        break;
                }

                this.Kudos += stat.Kudos;
                this.Duration += stat.End - stat.Start;
                this.Qualified += stat.Qualified ? 1 : 0;
            }

            TimeSpan finishTime = stat.Finish.GetValueOrDefault(stat.End) - stat.Start;
            if (stat.Finish.HasValue && finishTime.TotalSeconds > 1.1) {
                if (!stat.PrivateLobby) {
                    this.FinishedCount++;
                    this.FinishTime += finishTime;
                }
                if (this.Fastest == TimeSpan.Zero || this.Fastest > finishTime) {
                    this.Fastest = finishTime;
                }
                if (this.Longest < finishTime) {
                    this.Longest = finishTime;
                }
            }
        }

        public override string ToString() {
            return $"{this.Name}: {this.Qualified} / {this.Played}";
        }
    }
}