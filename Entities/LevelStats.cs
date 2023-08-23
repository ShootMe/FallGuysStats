using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media;
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
            { "user_creative_race_round",          new LevelStats("user_creative_race_round", "User Creative Race Round", LevelType.CreativeRace, true, false, 10, 0, 0, Properties.Resources.round_creative_icon, Properties.Resources.round_creative_big_icon) },
            { "creative_race_round",               new LevelStats("creative_race_round", "Creative Race Round", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "creative_race_final_round",         new LevelStats("creative_race_final_round", "Creative Race Final Round", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_orig_round_001",            new LevelStats("wle_s10_orig_round_001", "Beans Ahoy!", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_002",            new LevelStats("wle_s10_orig_round_002", "Airborne Antics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_003",            new LevelStats("wle_s10_orig_round_003", "Scythes & Roundabouts", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_004",            new LevelStats("wle_s10_orig_round_004", "Cardio Runners", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_005",            new LevelStats("wle_s10_orig_round_005", "Fan Flingers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_006",            new LevelStats("wle_s10_orig_round_006", "Uphill Struggle", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_007",            new LevelStats("wle_s10_orig_round_007", "Spinner Sprint", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_008",            new LevelStats("wle_s10_orig_round_008", "Lane Changers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_009",            new LevelStats("wle_s10_orig_round_009", "Gentle Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_012",            new LevelStats("wle_s10_orig_round_012", "Up & Down", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_013",            new LevelStats("wle_s10_orig_round_013", "Choo Choo Challenge", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_014",            new LevelStats("wle_s10_orig_round_014", "Runner Beans", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_015",            new LevelStats("wle_s10_orig_round_015", "Disc Dashers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_016",            new LevelStats("wle_s10_orig_round_016", "Two Faced", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_019",            new LevelStats("wle_s10_orig_round_019", "Blueberry Bombardment", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_020",            new LevelStats("wle_s10_orig_round_020", "Chuting Stars", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_021",            new LevelStats("wle_s10_orig_round_021", "Slimy Slopes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_022",            new LevelStats("wle_s10_orig_round_022", "Circuit Breakers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_023",            new LevelStats("wle_s10_orig_round_023", "Winding Walkways", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_026",            new LevelStats("wle_s10_orig_round_026", "Hyperlink Hijinks", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_027",            new LevelStats("wle_s10_orig_round_027", "Fan Frolics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_028",            new LevelStats("wle_s10_orig_round_028", "Windmill Road", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_029",            new LevelStats("wle_s10_orig_round_029", "Conveyor Clash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_032",            new LevelStats("wle_s10_orig_round_032", "Fortress Frolics", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_033",            new LevelStats("wle_s10_orig_round_033", "Super Door Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_034",            new LevelStats("wle_s10_orig_round_034", "Spiral Of Woo", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_035",            new LevelStats("wle_s10_orig_round_035", "Tornado Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_036",            new LevelStats("wle_s10_orig_round_036", "Hopscotch Havoc", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_037",            new LevelStats("wle_s10_orig_round_037", "Beat Bouncers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_038",            new LevelStats("wle_s10_orig_round_038", "Blunder Bridges", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_039",            new LevelStats("wle_s10_orig_round_039", "Incline Rewind", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_040",            new LevelStats("wle_s10_orig_round_040", "Prismatic Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_041",            new LevelStats("wle_s10_orig_round_041", "Swept Away", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_042",            new LevelStats("wle_s10_orig_round_042", "Balancing Act", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_043",            new LevelStats("wle_s10_orig_round_043", "Trouble Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_044",            new LevelStats("wle_s10_orig_round_044", "Serpent Slalom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_045",            new LevelStats("wle_s10_orig_round_045", "Floorless", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_046",            new LevelStats("wle_s10_orig_round_046", "In The Cloud", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_047",            new LevelStats("wle_s10_orig_round_047", "Downstream Duel", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "wle_s10_orig_round_048",            new LevelStats("wle_s10_orig_round_048", "Lost Palace", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_045_long",       new LevelStats("wle_s10_orig_round_045_long", "Floorless", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_003",            new LevelStats("wle_s10_long_round_003", "Fall Speedway", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_004",            new LevelStats("wle_s10_long_round_004", "Zig Zag Zoomies", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_long_round_005",            new LevelStats("wle_s10_long_round_005", "Terrabyte Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_round_001",                 new LevelStats("wle_s10_round_001", "Digi Trek", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_002",                 new LevelStats("wle_s10_round_002", "Shortcut Links", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_003",                 new LevelStats("wle_s10_round_003", "Upload Heights", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_005",                 new LevelStats("wle_s10_round_005", "Data Streams", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_006",                 new LevelStats("wle_s10_round_006", "Gigabyte Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_007",                 new LevelStats("wle_s10_round_007", "Cube Corruption", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_008",                 new LevelStats("wle_s10_round_008", "Wham Bam Boom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_010",                 new LevelStats("wle_s10_round_010", "Pixel Hearts", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_011",                 new LevelStats("wle_s10_round_011", "Cyber Circuit", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_012",                 new LevelStats("wle_s10_round_012", "Boom Blaster Trial", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk3_01",       new LevelStats("wle_s10_player_round_wk3_01", "Cloudy Chaos", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_02",       new LevelStats("wle_s10_player_round_wk3_02", "Door Game", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_03",       new LevelStats("wle_s10_player_round_wk3_03", "Full Speed Sliding (FSS) - Jelly Road", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_04",       new LevelStats("wle_s10_player_round_wk3_04", "Sky High Run", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_06",       new LevelStats("wle_s10_player_round_wk3_06", "Spiral Upheaval", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_07",       new LevelStats("wle_s10_player_round_wk3_07", "Creative Descent", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_08",       new LevelStats("wle_s10_player_round_wk3_08", "Rainbow Slide", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_09",       new LevelStats("wle_s10_player_round_wk3_09", "Fragrant Trumpet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_10",       new LevelStats("wle_s10_player_round_wk3_10", "Bridges That Don't Like You", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_11",       new LevelStats("wle_s10_player_round_wk3_11", "Rainbow Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_12",       new LevelStats("wle_s10_player_round_wk3_12", "Variable Valley", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_fp2_wk6_01",                    new LevelStats("wle_fp2_wk6_01", "Broken Course", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_14",       new LevelStats("wle_s10_player_round_wk3_14", "Tower of Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_15",       new LevelStats("wle_s10_player_round_wk3_15", "Parkour Party", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_16",       new LevelStats("wle_s10_player_round_wk3_16", "Catastrophe Climb", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_17",       new LevelStats("wle_s10_player_round_wk3_17", "Yeet Golf", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_18",       new LevelStats("wle_s10_player_round_wk3_18", "Hill of Fear", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_19",       new LevelStats("wle_s10_player_round_wk3_19", "Sky Time", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk3_20",       new LevelStats("wle_s10_player_round_wk3_20", "Ezz Map", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk4_01",       new LevelStats("wle_s10_player_round_wk4_01", "Slippery Stretch", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_02",       new LevelStats("wle_s10_player_round_wk4_02", "Ball 'N Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_03",       new LevelStats("wle_s10_player_round_wk4_03", "Rowdy Cloudy", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_05",       new LevelStats("wle_s10_player_round_wk4_05", "Vertiginous Steps", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_06",       new LevelStats("wle_s10_player_round_wk4_06", "Topsie Tursie", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_07",       new LevelStats("wle_s10_player_round_wk4_07", "Arcade Assault", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_08",       new LevelStats("wle_s10_player_round_wk4_08", "The Eight Pit Trials", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_09",       new LevelStats("wle_s10_player_round_wk4_09", "Green Beans", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_10",       new LevelStats("wle_s10_player_round_wk4_10", "Hop Hill", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_11",       new LevelStats("wle_s10_player_round_wk4_11", "Quick Sliders", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_12",       new LevelStats("wle_s10_player_round_wk4_12", "Split Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_13",       new LevelStats("wle_s10_player_round_wk4_13", "Piso Resbaloso", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_15",       new LevelStats("wle_s10_player_round_wk4_15", "Snowboard Street", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_18",       new LevelStats("wle_s10_player_round_wk4_18", "House Invasion", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_19",       new LevelStats("wle_s10_player_round_wk4_19", "SOLO FULL-TILT RAGE", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_20",       new LevelStats("wle_s10_player_round_wk4_20", "Terminal Slime-ocity", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_21",       new LevelStats("wle_s10_player_round_wk4_21", "Spin", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk4_22",       new LevelStats("wle_s10_player_round_wk4_22", "Lane Changers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "wle_s10_player_round_wk5_01",       new LevelStats("wle_s10_player_round_wk5_01", "Block Park", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_02",       new LevelStats("wle_s10_player_round_wk5_02", "The Drummatical Story", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_03",       new LevelStats("wle_s10_player_round_wk5_03", "Digital Temple", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_04",       new LevelStats("wle_s10_player_round_wk5_04", "Tower Escape", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_05",       new LevelStats("wle_s10_player_round_wk5_05", "Tower Dash", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_06",       new LevelStats("wle_s10_player_round_wk5_06", "Gpu Gauntlet", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_07",       new LevelStats("wle_s10_player_round_wk5_07", "Looooping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_08",       new LevelStats("wle_s10_player_round_wk5_08", "Rad Bean Skatepark", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_10",       new LevelStats("wle_s10_player_round_wk5_10", "Siank Arena", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_11",       new LevelStats("wle_s10_player_round_wk5_11", "Pro Players Only", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_12",       new LevelStats("wle_s10_player_round_wk5_12", "Extreme Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_13",       new LevelStats("wle_s10_player_round_wk5_13", "Dessert Village", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_14",       new LevelStats("wle_s10_player_round_wk5_14", "Extreme Trampoline Jumping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_15",       new LevelStats("wle_s10_player_round_wk5_15", "Beast Route", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_16",       new LevelStats("wle_s10_player_round_wk5_16", "METROPOLIS", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_17",       new LevelStats("wle_s10_player_round_wk5_17", "Big Bookcase", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk5_18",       new LevelStats("wle_s10_player_round_wk5_18", "Digital Doom", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_player_round_wk6_01",       new LevelStats("wle_s10_player_round_wk6_01", "Hammer Heaven", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_02",       new LevelStats("wle_s10_player_round_wk6_02", "RISKY ROUTES", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_03",       new LevelStats("wle_s10_player_round_wk6_03", "Castle Rush", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_04",       new LevelStats("wle_s10_player_round_wk6_04", "Chaotic Race", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_05",       new LevelStats("wle_s10_player_round_wk6_05", "FREEFALL TOWER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_06",       new LevelStats("wle_s10_player_round_wk6_06", "西西的天空之城 Castle in the Sky", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_08",       new LevelStats("wle_s10_player_round_wk6_08", "Flower Power", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_09",       new LevelStats("wle_s10_player_round_wk6_09", "Dimension Explorer", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_10",       new LevelStats("wle_s10_player_round_wk6_10", "Forked Passage", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_12",       new LevelStats("wle_s10_player_round_wk6_12", "The Bee Hive", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_13",       new LevelStats("wle_s10_player_round_wk6_13", "Yeets & Ladders", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_14",       new LevelStats("wle_s10_player_round_wk6_14", "Snek", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_15",       new LevelStats("wle_s10_player_round_wk6_15", "SCHOOL OF FISH", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_17",       new LevelStats("wle_s10_player_round_wk6_17", "Slippery Helixes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_18",       new LevelStats("wle_s10_player_round_wk6_18", "Recess", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_player_round_wk6_19",       new LevelStats("wle_s10_player_round_wk6_19", "Parrot river", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp3_07_01",             new LevelStats("current_wle_fp3_07_01", "Block Sledding", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_02",             new LevelStats("current_wle_fp3_07_02", "Layup Wallop", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_03",             new LevelStats("current_wle_fp3_07_03", "Minecart Mayhem", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_04",             new LevelStats("current_wle_fp3_07_04", "Bouncing Pass", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_05",             new LevelStats("current_wle_fp3_07_05", "Ball Factory", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_0_01",           new LevelStats("current_wle_fp3_07_0_01", "Funky Sanctuaries", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_0_02",           new LevelStats("current_wle_fp3_07_0_02", "Woo-F-O", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_07_0_03",           new LevelStats("current_wle_fp3_07_0_03", "Travel Diary - Great Wall of China", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp3_10_01",             new LevelStats("current_wle_fp3_10_01", "When Nature Falls", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_02",             new LevelStats("current_wle_fp3_10_02", "The Slippery Conveyor", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_03",             new LevelStats("current_wle_fp3_10_03", "The Slime Trials", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_04",             new LevelStats("current_wle_fp3_10_04", "Friendly Obstacles", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_05",             new LevelStats("current_wle_fp3_10_05", "Climb and Fall", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_06",             new LevelStats("current_wle_fp3_10_06", "Stairs and some other things", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_07",             new LevelStats("current_wle_fp3_10_07", "Meowgical World", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_08",             new LevelStats("current_wle_fp3_10_08", "Polluelo Speed", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_09",             new LevelStats("current_wle_fp3_10_09", "Pixel Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_10",             new LevelStats("current_wle_fp3_10_10", "Total Madness", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_11",             new LevelStats("current_wle_fp3_10_11", "The Abstract Maze", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_12",             new LevelStats("current_wle_fp3_10_12", "Fan Off", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_13",             new LevelStats("current_wle_fp3_10_13", "cloud highway", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_14",             new LevelStats("current_wle_fp3_10_14", "はねるの！？トビラ（Door Bounce）", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_15",             new LevelStats("current_wle_fp3_10_15", "Speedrunners be like", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_16",             new LevelStats("current_wle_fp3_10_16", "Tumble Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_17",             new LevelStats("current_wle_fp3_10_17", "Silver's Snake Run", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_18",             new LevelStats("current_wle_fp3_10_18", "Now Boarding", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_19",             new LevelStats("current_wle_fp3_10_19", "Slime Scale", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_20",             new LevelStats("current_wle_fp3_10_20", "TUMBLEDOWN MINESHAFT", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_21",             new LevelStats("current_wle_fp3_10_21", "Circuito CHILL 1", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_22",             new LevelStats("current_wle_fp3_10_22", "STUMBLE SLIDER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_23",             new LevelStats("current_wle_fp3_10_23", "Controlled Chaos", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_24",             new LevelStats("current_wle_fp3_10_24", "Xtreme Jumping", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_25",             new LevelStats("current_wle_fp3_10_25", "Odin", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_26",             new LevelStats("current_wle_fp3_10_26", "Ciudad nube", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_27",             new LevelStats("current_wle_fp3_10_27", "Bean Voyage", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_28",             new LevelStats("current_wle_fp3_10_28", "SLIP-SAW", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_10_29",             new LevelStats("current_wle_fp3_10_29", "Bbq bacon burger", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
                
            { "current_wle_fp3_08_01",             new LevelStats("current_wle_fp3_08_01", "Grabbers Territory", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_02",             new LevelStats("current_wle_fp3_08_02", "A Way Out", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_03",             new LevelStats("current_wle_fp3_08_03", "Wall Block", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_04",             new LevelStats("current_wle_fp3_08_04", "The dream island", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_05",             new LevelStats("current_wle_fp3_08_05", "Rainbow pulsion", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_06",             new LevelStats("current_wle_fp3_08_06", "WHIPPITY WOPPITY", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_09",             new LevelStats("current_wle_fp3_08_09", "Big Fans Box Challenge", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_10",             new LevelStats("current_wle_fp3_08_10", "Crazy boxes", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_13",             new LevelStats("current_wle_fp3_08_13", "Season 1 Race Mashup", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_14",             new LevelStats("current_wle_fp3_08_14", "Flippy Hoopshots", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_15",             new LevelStats("current_wle_fp3_08_15", "Stumble Teams", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_16",             new LevelStats("current_wle_fp3_08_16", "Twisting Tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_17",             new LevelStats("current_wle_fp3_08_17", "PUSH 'N' PULL", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_18",             new LevelStats("current_wle_fp3_08_18", "The Rising Blocks", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_08_19",             new LevelStats("current_wle_fp3_08_19", "Puzzle Blokies Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_01",             new LevelStats("current_wle_fp3_09_01", "The up tower", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_02",             new LevelStats("current_wle_fp3_09_02", "Short shuriken", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_03",             new LevelStats("current_wle_fp3_09_03", "Les mêmes mécaniques de + en + dure", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_04",             new LevelStats("current_wle_fp3_09_04", "Digi-Lily Sliding", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_05",             new LevelStats("current_wle_fp3_09_05", "STUMBLE MEDIEVAL TOWER", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_06",             new LevelStats("current_wle_fp3_09_06", "Random Heights", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_07",             new LevelStats("current_wle_fp3_09_07", "Climb scramble", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_08",             new LevelStats("current_wle_fp3_09_08", "Collide Gaming", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_09",             new LevelStats("current_wle_fp3_09_09", "Very Compressed Level", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_01",           new LevelStats("current_wle_fp3_09_0_01", "Slippery Slope", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_02",           new LevelStats("current_wle_fp3_09_0_02", "The Most Hardest Fall Guys LEVEL", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_03",           new LevelStats("current_wle_fp3_09_0_03", "Free Falling", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_04",           new LevelStats("current_wle_fp3_09_0_04", "Conveyor Problems", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_05",           new LevelStats("current_wle_fp3_09_0_05", "Clocktower Climb", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_06",           new LevelStats("current_wle_fp3_09_0_06", "Savour Your Happiness", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp3_09_0_0_01",         new LevelStats("current_wle_fp3_09_0_0_01", "Pastel Paradise", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "current_wle_fp4_09_01",             new LevelStats("current_wle_fp4_09_01", "Crate Collector", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_09_02",             new LevelStats("current_wle_fp4_09_02", "Dribble Drills", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_08",             new LevelStats("current_wle_fp4_10_08", "Wall Breaker", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_11",             new LevelStats("current_wle_fp4_10_11", "HOARDER BLOCKS", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_12",             new LevelStats("current_wle_fp4_10_12", "Chickens run away", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_20",             new LevelStats("current_wle_fp4_10_20", "Co-op and CO", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
                
            { "current_wle_fp4_10_01",             new LevelStats("current_wle_fp4_10_01", "Bouncy Box Boulevard 3 Extreme Delivery", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_02",             new LevelStats("current_wle_fp4_10_02", "Hot Blast", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_03",             new LevelStats("current_wle_fp4_10_03", "Box Fan Blitz", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_04",             new LevelStats("current_wle_fp4_10_04", "Woo-terfall Way", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_05",             new LevelStats("current_wle_fp4_10_05", "Slime race", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_06",             new LevelStats("current_wle_fp4_10_06", "Moving Day", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_07",             new LevelStats("current_wle_fp4_10_07", "Birthday Dash", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            // { "current_wle_fp4_10_08",             new LevelStats("current_wle_fp4_10_08", "Chess History", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_0_01",           new LevelStats("current_wle_fp4_10_0_01", "Cheese Canyon", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "current_wle_fp4_10_0_02",           new LevelStats("current_wle_fp4_10_0_02", "Molehills", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_bt_round_001",              new LevelStats("wle_s10_bt_round_001", "Push Ups", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_002",              new LevelStats("wle_s10_bt_round_002", "Heave & Haul", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_003",              new LevelStats("wle_s10_bt_round_003", "Stepping Stones", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_bt_round_004",              new LevelStats("wle_s10_bt_round_004", "Double Trouble", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_s10_cf_round_001",              new LevelStats("wle_s10_cf_round_001", "Blocky Bridges", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_cf_round_002",              new LevelStats("wle_s10_cf_round_002", "Gappy-go-Lucky", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_cf_round_003",              new LevelStats("wle_s10_cf_round_003", "Drop n' Drag", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_cf_round_004",              new LevelStats("wle_s10_cf_round_004", "Fun with Fans", LevelType.Race, true, false, 10, 360, 360, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            
            { "wle_mrs_bagel_opener_1",            new LevelStats("wle_mrs_bagel_opener_1", "Tunnel of Love", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_2",            new LevelStats("wle_mrs_bagel_opener_2", "Pink Parade", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_3",            new LevelStats("wle_mrs_bagel_opener_3", "Prideful Path", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_opener_4",            new LevelStats("wle_mrs_bagel_opener_4", "Coming Together", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_1",            new LevelStats("wle_mrs_bagel_filler_1", "Clifftop Capers", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_2",            new LevelStats("wle_mrs_bagel_filler_2", "Waveway Splits", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_3",            new LevelStats("wle_mrs_bagel_filler_3", "In the Groove", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_filler_4",            new LevelStats("wle_mrs_bagel_filler_4", "Heartfall Heat", LevelType.Race, true, false, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_final_1",             new LevelStats("wle_mrs_bagel_final_1", "Rainbow Rise", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_mrs_bagel_final_2",             new LevelStats("wle_mrs_bagel_final_2", "Out and About", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "wle_s10_orig_round_010",            new LevelStats("wle_s10_orig_round_010", "Square Up", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_011",            new LevelStats("wle_s10_orig_round_011", "Slide Showdown", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_017",            new LevelStats("wle_s10_orig_round_017", "Bellyflop Battlers", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_018",            new LevelStats("wle_s10_orig_round_018", "Apples & Oranges", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_024",            new LevelStats("wle_s10_orig_round_024", "Wooseleum", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_025",            new LevelStats("wle_s10_orig_round_025", "Mount Boom", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_030",            new LevelStats("wle_s10_orig_round_030", "Mega Monument", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_orig_round_031",            new LevelStats("wle_s10_orig_round_031", "Transfer Turnpike", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_004",                 new LevelStats("wle_s10_round_004", "Parkour Panic", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },
            { "wle_s10_round_009",                 new LevelStats("wle_s10_round_009", "Firewall Finale", LevelType.Race, true, true, 10, 0, 0, Properties.Resources.round_gauntlet_icon, Properties.Resources.round_gauntlet_big_icon) },

            { "round_biggestfan",                  new LevelStats("round_biggestfan", "Big Fans", LevelType.Race, false, false, 2, 210, 120, Properties.Resources.round_big_fans_icon, Properties.Resources.round_big_fans_big_icon) },
            { "round_satellitehoppers_almond",     new LevelStats("round_satellitehoppers_almond", "Cosmic Highway", LevelType.Race, false, false, 8, 180, 180, Properties.Resources.round_cosmic_highway_icon, Properties.Resources.round_cosmic_highway_big_icon) },
            { "round_gauntlet_02",                 new LevelStats("round_gauntlet_02", "Dizzy Heights", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_dizzy_heights_icon, Properties.Resources.round_dizzy_heights_big_icon) },
            { "round_door_dash",                   new LevelStats("round_door_dash", "Door Dash", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_door_dash_icon, Properties.Resources.round_door_dash_big_icon) },
            { "round_iceclimb",                    new LevelStats("round_iceclimb", "Freezy Peak", LevelType.Race, false, false, 3, 180, 120, Properties.Resources.round_freezy_peak_icon, Properties.Resources.round_freezy_peak_big_icon) },
            { "round_dodge_fall",                  new LevelStats("round_dodge_fall", "Fruit Chute", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_fruit_chute_icon, Properties.Resources.round_fruit_chute_big_icon) },
            { "round_see_saw_360",                 new LevelStats("round_see_saw_360", "Full Tilt", LevelType.Race, false, false, 6, 180, 180, Properties.Resources.round_full_tilt_icon, Properties.Resources.round_full_tilt_big_icon) },
            { "round_chompchomp",                  new LevelStats("round_chompchomp", "Gate Crash", LevelType.Race, false, false, 1, 300, 120, Properties.Resources.round_gate_crash_icon, Properties.Resources.round_gate_crash_big_icon) },
            { "round_gauntlet_01",                 new LevelStats("round_gauntlet_01", "Hit Parade", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_hit_parade_icon, Properties.Resources.round_hit_parade_big_icon) },
            { "round_gauntlet_04",                 new LevelStats("round_gauntlet_04", "Knight Fever", LevelType.Race, false, false, 2, 180, 120, Properties.Resources.round_knight_fever_icon, Properties.Resources.round_knight_fever_big_icon) },
            { "round_drumtop",                     new LevelStats("round_drumtop", "Lily Leapers", LevelType.Race, false, false, 5, 300, 140, Properties.Resources.round_lily_leapers_icon, Properties.Resources.round_lily_leapers_big_icon) },
            { "round_gauntlet_08",                 new LevelStats("round_gauntlet_08", "Party Promenade", LevelType.Race, false, false, 6, 300, 120, Properties.Resources.round_party_promenade_icon, Properties.Resources.round_party_promenade_big_icon) },
            { "round_pipedup_s6_launch",           new LevelStats("round_pipedup_s6_launch", "Pipe Dream", LevelType.Race, false, false, 6, 300, 150, Properties.Resources.round_pipe_dream_icon, Properties.Resources.round_pipe_dream_big_icon) },
            { "round_follow_the_line",             new LevelStats("round_follow_the_line", "Puzzle Path", LevelType.Race, false, false, 9, 150, 150, Properties.Resources.round_puzzle_path_icon, Properties.Resources.round_puzzle_path_big_icon) },
            { "round_tunnel_race",                 new LevelStats("round_tunnel_race", "Roll On", LevelType.Race, false, false, 4, 120, 120, Properties.Resources.round_roll_on_icon, Properties.Resources.round_roll_on_big_icon) },
            { "round_see_saw",                     new LevelStats("round_see_saw", "See Saw", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_see_saw_icon, Properties.Resources.round_see_saw_big_icon) },
            { "round_shortcircuit",                new LevelStats("round_shortcircuit", "Short Circuit", LevelType.Race, false, false, 4, 300, 300, Properties.Resources.round_short_circuit_icon, Properties.Resources.round_short_circuit_big_icon) },
            { "round_skeefall",                    new LevelStats("round_skeefall", "Ski Fall", LevelType.Race, false, false, 3, 300, 300, Properties.Resources.round_ski_fall_icon, Properties.Resources.round_ski_fall_big_icon) },
            { "round_gauntlet_06",                 new LevelStats("round_gauntlet_06", "Skyline Stumble", LevelType.Race, false, false, 4, 180, 120, Properties.Resources.round_skyline_stumble_icon, Properties.Resources.round_skyline_stumble_big_icon) },
            { "round_lava",                        new LevelStats("round_lava", "Slime Climb", LevelType.Race, false, false, 1, 140, 140, Properties.Resources.round_slime_climb_icon, Properties.Resources.round_slime_climb_big_icon) },
            { "round_gauntlet_10_almond",          new LevelStats("round_gauntlet_10_almond", "Space Race", LevelType.Race, false, false, 8, 150, 150, Properties.Resources.round_space_race_icon, Properties.Resources.round_space_race_big_icon) },
            { "round_short_circuit_2_symphony_launch_show", new LevelStats("round_short_circuit_2_symphony_launch_show", "Speed Circuit", LevelType.Race, false, false, 7, 180, 180, Properties.Resources.round_speed_circuit_icon, Properties.Resources.round_speed_circuit_big_icon) },
            { "round_slide_chute",                 new LevelStats("round_slide_chute", "Speed Slider", LevelType.Race, false, false, 9, 165, 120, Properties.Resources.round_speed_slider_icon, Properties.Resources.round_speed_slider_big_icon) },
            { "round_starlink_almond",             new LevelStats("round_starlink_almond", "Starchart", LevelType.Race, false, false, 8, 150, 150, Properties.Resources.round_starchart_icon, Properties.Resources.round_starchart_big_icon) },
            { "round_slimeclimb_2",                new LevelStats("round_slimeclimb_2", "The Slimescraper", LevelType.Race, false, false, 4, 190, 190, Properties.Resources.round_the_slimescraper_icon, Properties.Resources.round_the_slimescraper_big_icon) },
            { "round_gauntlet_03",                 new LevelStats("round_gauntlet_03", "The Whirlygig", LevelType.Race, false, false, 1, 180, 120, Properties.Resources.round_the_whirlygig_icon, Properties.Resources.round_the_whirlygig_big_icon) },
            { "round_tip_toe",                     new LevelStats("round_tip_toe", "Tip Toe", LevelType.Race, false, false, 1, 300, 120, Properties.Resources.round_tip_toe_icon, Properties.Resources.round_tip_toe_big_icon) },
            { "round_gauntlet_09_symphony_launch_show", new LevelStats("round_gauntlet_09_symphony_launch_show", "Track Attack", LevelType.Race, false, false, 7, 90, 90, Properties.Resources.round_track_attack_icon, Properties.Resources.round_track_attack_big_icon) },
            { "round_gauntlet_07",                 new LevelStats("round_gauntlet_07", "Treetop Tumble", LevelType.Race, false, false, 5, 180, 120, Properties.Resources.round_treetop_tumble_icon, Properties.Resources.round_treetop_tumble_big_icon) },
            { "round_gauntlet_05",                 new LevelStats("round_gauntlet_05", "Tundra Run", LevelType.Race, false, false, 3, 180, 120, Properties.Resources.round_tundra_run_icon, Properties.Resources.round_tundra_run_big_icon) },
            { "round_wall_guys",                   new LevelStats("round_wall_guys", "Wall Guys", LevelType.Race, false, false, 2, 300, 120, Properties.Resources.round_wall_guys_icon, Properties.Resources.round_wall_guys_big_icon) },
            { "round_airtime",                     new LevelStats("round_airtime", "Airtime", LevelType.Hunt, false, false, 6, 300, 300, Properties.Resources.round_airtime_icon, Properties.Resources.round_airtime_big_icon) },
            { "round_bluejay",                     new LevelStats("round_bluejay", "Bean Hill Zone", LevelType.Hunt, false, false, 7, 300, 300, Properties.Resources.round_bean_hill_zone_icon, Properties.Resources.round_bean_hill_zone_big_icon) },
            { "round_hoops_revenge_symphony_launch_show", new LevelStats("round_hoops_revenge_symphony_launch_show", "Bounce Party", LevelType.Hunt, false, false, 7, 300, 300, Properties.Resources.round_bounce_party_icon, Properties.Resources.round_bounce_party_big_icon) },
            { "round_king_of_the_hill",            new LevelStats("round_king_of_the_hill", "Bubble Trouble", LevelType.Hunt, false, false, 5, 300, 300, Properties.Resources.round_bubble_trouble_icon, Properties.Resources.round_bubble_trouble_big_icon) },
            { "round_1v1_button_basher",           new LevelStats("round_1v1_button_basher", "Button Bashers", LevelType.Hunt, false, false, 4, 90, 90, Properties.Resources.round_button_bashers_icon, Properties.Resources.round_button_bashers_big_icon) },
            { "round_ffa_button_bashers_squads_almond", new LevelStats("round_ffa_button_bashers_squads_almond", "Frantic Factory", LevelType.Hunt, false, false, 8, 300, 300, Properties.Resources.round_frantic_factory_icon, Properties.Resources.round_frantic_factory_big_icon) },
            { "round_slippy_slide",                new LevelStats("round_slippy_slide", "Hoop Chute", LevelType.Hunt, false, false, 9, 180, 180, Properties.Resources.round_hoop_chute_icon, Properties.Resources.round_hoop_chute_big_icon) },
            { "round_hoops_blockade_solo",         new LevelStats("round_hoops_blockade_solo", "Hoopsie Legends", LevelType.Hunt, false, false, 2, 300, 300, Properties.Resources.round_hoopsie_legends_icon, Properties.Resources.round_hoopsie_legends_big_icon) },
            { "round_follow-the-leader_s6_launch", new LevelStats("round_follow-the-leader_s6_launch", "Leading Light", LevelType.Hunt, false, false, 6, 300, 300, Properties.Resources.round_leading_light_icon, Properties.Resources.round_leading_light_big_icon) },
            { "round_penguin_solos",               new LevelStats("round_penguin_solos", "Pegwin Pool Party", LevelType.Hunt, false, false, 5, 300, 300, Properties.Resources.round_pegwin_pool_party_icon, Properties.Resources.round_pegwin_pool_party_big_icon) },
            { "round_tail_tag",                    new LevelStats("round_tail_tag", "Tail Tag", LevelType.Hunt, false, false, 1, 90, 90, Properties.Resources.round_tail_tag_icon, Properties.Resources.round_tail_tag_big_icon) },
            { "round_1v1_volleyfall_symphony_launch_show", new LevelStats("round_1v1_volleyfall_symphony_launch_show", "Volleyfall", LevelType.Hunt, false, false, 7, 100, 100, Properties.Resources.round_volleyfall_icon, Properties.Resources.round_volleyfall_big_icon) },
            { "round_fruitpunch_s4_show",          new LevelStats("round_fruitpunch_s4_show", "Big Shots", LevelType.Survival, false, false, 4, 90, 90, Properties.Resources.round_big_shots_icon, Properties.Resources.round_big_shots_big_icon) },
            { "round_blastballruins",              new LevelStats("round_blastballruins", "Blastlantis", LevelType.Survival, false, false, 9, 270, 150, Properties.Resources.round_blastlantis_icon, Properties.Resources.round_blastlantis_big_icon) },
            { "round_block_party",                 new LevelStats("round_block_party", "Block Party", LevelType.Survival, false, false, 1, 105, 105, Properties.Resources.round_block_party_icon, Properties.Resources.round_block_party_big_icon) },
            { "round_hoverboardsurvival_s4_show",  new LevelStats("round_hoverboardsurvival_s4_show", "Hoverboard Heroes", LevelType.Survival, false, false, 4, 140, 140, Properties.Resources.round_hoverboard_heroes_icon, Properties.Resources.round_hoverboard_heroes_big_icon) },
            { "round_hoverboardsurvival2_almond",  new LevelStats("round_hoverboardsurvival2_almond", "Hyperdrive Heroes", LevelType.Survival, false, false, 8, 170, 170, Properties.Resources.round_hyperdrive_heroes_icon, Properties.Resources.round_hyperdrive_heroes_big_icon) },
            { "round_jump_club",                   new LevelStats("round_jump_club", "Jump Club", LevelType.Survival, false, false, 1, 90, 90, Properties.Resources.round_jump_club_icon, Properties.Resources.round_jump_club_big_icon) },
            { "round_tunnel",                      new LevelStats("round_tunnel", "Roll Out", LevelType.Survival, false, false, 1, 150, 90, Properties.Resources.round_roll_out_icon, Properties.Resources.round_roll_out_big_icon) },
            { "round_snowballsurvival",            new LevelStats("round_snowballsurvival", "Snowball Survival", LevelType.Survival, false, false, 3, 60, 60, Properties.Resources.round_snowball_survival_icon, Properties.Resources.round_snowball_survival_big_icon) },
            { "round_robotrampage_arena_2",        new LevelStats("round_robotrampage_arena_2", "Stompin' Ground", LevelType.Survival, false, false, 5, 70, 70, Properties.Resources.round_stompin_ground_icon, Properties.Resources.round_stompin_ground_big_icon) },
            { "round_spin_ring_symphony_launch_show", new LevelStats("round_spin_ring_symphony_launch_show", "The Swiveller", LevelType.Survival, false, false, 7, 180, 180, Properties.Resources.round_the_swiveller_icon, Properties.Resources.round_the_swiveller_big_icon) },

            { "round_match_fall",                 new LevelStats("round_match_fall", "Perfect Match", LevelType.Logic, false, false, 1, 80, 80, Properties.Resources.round_perfect_match_icon, Properties.Resources.round_perfect_match_big_icon) },
            { "round_pixelperfect_almond",        new LevelStats("round_pixelperfect_almond", "Pixel Painters", LevelType.Logic, false, false, 8, 180, 180, Properties.Resources.round_pixel_painters_icon, Properties.Resources.round_pixel_painters_big_icon) },
            { "round_fruit_bowl",                 new LevelStats("round_fruit_bowl", "Sum Fruit", LevelType.Logic, false, false, 5, 100, 100, Properties.Resources.round_sum_fruit_icon, Properties.Resources.round_sum_fruit_big_icon) },
            { "round_basketfall_s4_show",         new LevelStats("round_basketfall_s4_show", "Basketfall", LevelType.Team, false, false, 4, 90, 90, Properties.Resources.round_basketfall_icon, Properties.Resources.round_basketfall_big_icon) },
            { "round_egg_grab",                   new LevelStats("round_egg_grab", "Egg Scramble", LevelType.Team, false, false, 1, 120, 120, Properties.Resources.round_egg_scramble_icon, Properties.Resources.round_egg_scramble_big_icon) },
            { "round_egg_grab_02",                new LevelStats("round_egg_grab_02", "Egg Siege", LevelType.Team, false, false, 2, 120, 120, Properties.Resources.round_egg_siege_icon, Properties.Resources.round_egg_siege_big_icon) },
            { "round_fall_ball_60_players",       new LevelStats("round_fall_ball_60_players", "Fall Ball", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_fall_ball_icon, Properties.Resources.round_fall_ball_big_icon) },
            { "round_ballhogs",                   new LevelStats("round_ballhogs", "Hoarders", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_hoarders_icon, Properties.Resources.round_hoarders_big_icon) },
            { "round_hoops",                      new LevelStats("round_hoops", "Hoopsie Daisy", LevelType.Team, false, false, 1, 120, 120, Properties.Resources.round_hoopsie_daisy_icon, Properties.Resources.round_hoopsie_daisy_big_icon) },
            { "round_jinxed",                     new LevelStats("round_jinxed", "Jinxed", LevelType.Team, false, false, 1, 300, 300, Properties.Resources.round_jinxed_icon, Properties.Resources.round_jinxed_big_icon) },
            { "round_chicken_chase",              new LevelStats("round_chicken_chase", "Pegwin Pursuit", LevelType.Team, false, false, 3, 120, 120, Properties.Resources.round_pegwin_pursuit_icon, Properties.Resources.round_pegwin_pursuit_big_icon) },
            { "round_territory_control_s4_show",  new LevelStats("round_territory_control_s4_show", "Power Trip", LevelType.Team, false, false, 4, 100, 100, Properties.Resources.round_power_trip_icon, Properties.Resources.round_power_trip_big_icon) },
            { "round_rocknroll",                  new LevelStats("round_rocknroll", "Rock 'n' Roll", LevelType.Team, false, false, 1, 180, 180, Properties.Resources.round_rock_n_roll_icon, Properties.Resources.round_rock_n_roll_big_icon) },
            { "round_snowy_scrap",                new LevelStats("round_snowy_scrap", "Snowy Scrap", LevelType.Team, false, false, 3, 180, 180, Properties.Resources.round_snowy_scrap_icon, Properties.Resources.round_snowy_scrap_big_icon) },
            { "round_conveyor_arena",             new LevelStats("round_conveyor_arena", "Team Tail Tag", LevelType.Team, false, false, 1, 90, 90, Properties.Resources.round_team_tail_tag_icon, Properties.Resources.round_team_tail_tag_big_icon) },

            { "round_invisibeans",                new LevelStats("round_invisibeans", "Sweet Thieves", LevelType.Invisibeans, false, false, 6, 180, 180, Properties.Resources.round_sweet_thieves_icon, Properties.Resources.round_sweet_thieves_big_icon) },
            { "round_pumpkin_pie",                new LevelStats("round_pumpkin_pie", "Treat Thieves", LevelType.Invisibeans, false, false, 8, 180, 180, Properties.Resources.round_treat_thieves_icon, Properties.Resources.round_treat_thieves_big_icon) },
            { "round_blastball_arenasurvival_symphony_launch_show", new LevelStats("round_blastball_arenasurvival_symphony_launch_show", "Blast Ball", LevelType.Survival, false, true, 7, 270, 270, Properties.Resources.round_blast_ball_icon, Properties.Resources.round_blast_ball_big_icon) },
            { "round_fall_mountain_hub_complete", new LevelStats("round_fall_mountain_hub_complete", "Fall Mountain", LevelType.Race, false, true, 1, 300, 300, Properties.Resources.round_fall_mountain_icon, Properties.Resources.round_fall_mountain_big_icon) },
            { "round_floor_fall",                 new LevelStats("round_floor_fall", "Hex-A-Gone", LevelType.Survival, false, true, 1, 300, 300, Properties.Resources.round_hex_a_gone_icon, Properties.Resources.round_hex_a_gone_big_icon) },
            { "round_hexaring_symphony_launch_show", new LevelStats("round_hexaring_symphony_launch_show", "Hex-A-Ring", LevelType.Survival, false, true, 7, 300, 300, Properties.Resources.round_hex_a_ring_icon, Properties.Resources.round_hex_a_ring_big_icon) },
            { "round_hexsnake_almond",            new LevelStats("round_hexsnake_almond", "Hex-A-Terrestrial", LevelType.Survival, false, true, 8, 300, 300, Properties.Resources.round_hex_a_terrestrial_icon, Properties.Resources.round_hex_a_terrestrial_big_icon) },
            { "round_jump_showdown",              new LevelStats("round_jump_showdown", "Jump Showdown", LevelType.Survival, false, true, 1, 300, 300, Properties.Resources.round_jump_showdown_icon, Properties.Resources.round_jump_showdown_big_icon) },
            { "round_kraken_attack",              new LevelStats("round_kraken_attack", "Kraken Slam", LevelType.Survival, false, true, 9, 300, 300, Properties.Resources.round_kraken_slam_icon, Properties.Resources.round_kraken_slam_big_icon) },
            { "round_crown_maze",                 new LevelStats("round_crown_maze", "Lost Temple", LevelType.Race, false, true, 5, 300, 300, Properties.Resources.round_lost_temple_icon, Properties.Resources.round_lost_temple_big_icon) },
            { "round_tunnel_final",               new LevelStats("round_tunnel_final", "Roll Off", LevelType.Survival, false, true, 3, 300, 300, Properties.Resources.round_roll_off_icon, Properties.Resources.round_roll_off_big_icon) },
            { "round_royal_rumble",               new LevelStats("round_royal_rumble", "Royal Fumble", LevelType.Hunt, false, true, 1, 90, 90, Properties.Resources.round_royal_fumble_icon, Properties.Resources.round_royal_fumble_big_icon) },
            { "round_thin_ice",                   new LevelStats("round_thin_ice", "Thin Ice", LevelType.Survival, false, true, 3, 300, 300, Properties.Resources.round_thin_ice_icon, Properties.Resources.round_thin_ice_big_icon) },
            { "round_tiptoefinale_almond",        new LevelStats("round_tiptoefinale_almond", "Tip Toe Finale", LevelType.Survival, false, true, 8, 300, 300, Properties.Resources.round_tip_toe_finale_icon, Properties.Resources.round_tip_toe_finale_big_icon) },
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
        public string Id { get; set; }
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

        public LevelStats(string levelId, string levelName, LevelType type, bool isCreative, bool isFinal, int season, int timeLimitSeconds, int timeLimitSecondsForSquad, Image roundIcon, Image roundBigIcon) {
            this.Id = levelId;
            this.Name = levelName;
            this.Type = type;
            this.Season = season;
            this.IsCreative = isCreative;
            this.IsFinal = isFinal;
            this.TimeLimitSeconds = timeLimitSeconds;
            this.TimeLimitSecondsForSquad = timeLimitSecondsForSquad;
            this.RoundIcon = roundIcon;
            this.RoundBigIcon = roundBigIcon;
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
        public void Increase(RoundInfo stat, bool isLinkedCustomShow) {
            if (!stat.PrivateLobby || "user_creative_race_round".Equals(stat.Name) || isLinkedCustomShow) {
                this.Played++;
                this.Duration += stat.End - stat.Start;
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
                this.Qualified += stat.Qualified ? 1 : 0;
            }

            TimeSpan finishTime = stat.Finish.GetValueOrDefault(stat.Start) - stat.Start;
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
        public void Add(RoundInfo stat) {
            this.Stats.Add(stat);
        }

        public override string ToString() {
            return $"{this.Name}: {this.Qualified} / {this.Played}";
        }
    }
}