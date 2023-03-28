using System;
using System.Collections.Generic;
using System.Drawing;
using LiteDB;

namespace FallGuysStats {
    public class RoundInfo : IComparable<RoundInfo> {
        public ObjectId ID { get; set; }
        public int Profile { get; set; }
        public string Name { get; set; }
        public int ShowID { get; set; }
        public string ShowNameId { get; set; }
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
                && info.Name == this.Name;
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
        None,
        Gold,
        Silver,
        Bronze
    }
    public class LevelStats {
        public static Dictionary<string, LevelStats> ALL = new Dictionary<string, LevelStats>(StringComparer.OrdinalIgnoreCase) {
            { "round_biggestfan",                 new LevelStats("Big Fans", LevelType.Race, false, 2, Properties.Resources.round_big_fans_icon) },
            { "round_satellitehoppers_almond",    new LevelStats("Cosmic Highway", LevelType.Race, false, 8, Properties.Resources.round_cosmic_highway_icon) },
            { "round_gauntlet_02",                new LevelStats("Dizzy Heights", LevelType.Race, false, 1, Properties.Resources.round_dizzy_heights_icon) },
            { "round_door_dash",                  new LevelStats("Door Dash", LevelType.Race, false, 1, Properties.Resources.round_door_dash_icon) },
            { "round_iceclimb",                   new LevelStats("Freezy Peak", LevelType.Race, false, 3, Properties.Resources.round_freezy_peak_icon) },
            { "round_dodge_fall",                 new LevelStats("Fruit Chute", LevelType.Race, false, 1, Properties.Resources.round_fruit_chute_icon) },
            { "round_see_saw_360",                new LevelStats("Full Tilt", LevelType.Race, false, 6, Properties.Resources.round_full_tilt_icon) },
            { "round_chompchomp",                 new LevelStats("Gate Crash", LevelType.Race, false, 1, Properties.Resources.round_gate_crash_icon) },
            { "round_gauntlet_01",                new LevelStats("Hit Parade", LevelType.Race, false, 1, Properties.Resources.round_hit_parade_icon) },
            { "round_gauntlet_04",                new LevelStats("Knight Fever", LevelType.Race, false, 2, Properties.Resources.round_knight_fever_icon) },
            { "round_drumtop",                    new LevelStats("Lily Leapers", LevelType.Race, false, 5, Properties.Resources.round_lily_leapers_icon) },
            { "round_gauntlet_08",                new LevelStats("Party Promenade", LevelType.Race, false, 6, Properties.Resources.round_party_promenade_icon) },
            { "round_pipedup_s6_launch",          new LevelStats("Pipe Dream", LevelType.Race, false, 6, Properties.Resources.round_pipe_dream_icon) },
            { "round_pixelperfect_almond",        new LevelStats("Pixel Painters", LevelType.Race, false, 8, Properties.Resources.round_pixel_painters_icon) },
            { "round_follow_the_line",            new LevelStats("Puzzle Path", LevelType.Race, false, 9, Properties.Resources.round_puzzle_path_icon) },
            { "round_tunnel_race",                new LevelStats("Roll On", LevelType.Race, false, 4, Properties.Resources.round_roll_on_icon) },
            { "round_see_saw",                    new LevelStats("See Saw", LevelType.Race, false, 1, Properties.Resources.round_see_saw_icon) },
            { "round_shortcircuit",               new LevelStats("Short Circuit", LevelType.Race, false, 4, Properties.Resources.round_short_circuit_icon) },
            { "round_skeefall",                   new LevelStats("Ski Fall", LevelType.Race, false, 3, Properties.Resources.round_ski_fall_icon) },
            { "round_gauntlet_06",                new LevelStats("Skyline Stumble", LevelType.Race, false, 4, Properties.Resources.round_skyline_stumble_icon) },
            { "round_lava",                       new LevelStats("Slime Climb", LevelType.Race, false, 1, Properties.Resources.round_slime_climb_icon) },
            { "round_gauntlet_10_almond",         new LevelStats("Space Race", LevelType.Race, false, 8, Properties.Resources.round_space_race_icon) },
            { "round_short_circuit_2_symphony_launch_show", new LevelStats("Speed Circuit", LevelType.Race, false, 7, Properties.Resources.round_speed_circuit_icon) },
            { "round_slide_chute",                new LevelStats("Speed Slider", LevelType.Race, false, 9, Properties.Resources.round_speed_slider_icon) },
            { "round_starlink_almond",            new LevelStats("Starchart", LevelType.Race, false, 8, Properties.Resources.round_starchart_icon) },
            { "round_slimeclimb_2",               new LevelStats("The Slimescraper", LevelType.Race, false, 4, Properties.Resources.round_the_slimescraper_icon) },
            { "round_gauntlet_03",                new LevelStats("The Whirlygig", LevelType.Race, false, 1, Properties.Resources.round_the_whirlygig_icon) },
            { "round_tip_toe",                    new LevelStats("Tip Toe", LevelType.Race, false, 1, Properties.Resources.round_tip_toe_icon) },
            { "round_gauntlet_09_symphony_launch_show", new LevelStats("Track Attack", LevelType.Race, false, 7, Properties.Resources.round_track_attack_icon) },
            { "round_gauntlet_07",                new LevelStats("Treetop Tumble", LevelType.Race, false, 5, Properties.Resources.round_treetop_tumble_icon) },
            { "round_gauntlet_05",                new LevelStats("Tundra Run", LevelType.Race, false, 3, Properties.Resources.round_tundra_run_icon) },
            { "round_wall_guys",                  new LevelStats("Wall Guys", LevelType.Race, false, 2, Properties.Resources.round_wall_guys_icon) },

            { "round_fruitpunch_s4_show",         new LevelStats("Big Shots", LevelType.Survival, false, 4, Properties.Resources.round_big_shots_icon) },
            { "round_blastballruins",             new LevelStats("Blastlantis", LevelType.Survival, false, 9, Properties.Resources.round_blastlantis_icon) },
            { "round_block_party",                new LevelStats("Block Party", LevelType.Survival, false, 1, Properties.Resources.round_block_party_icon) },
            { "round_hoverboardsurvival_s4_show", new LevelStats("Hoverboard Heroes", LevelType.Survival, false, 4, Properties.Resources.round_hoverboard_heroes_icon) },
            { "round_hoverboardsurvival2_almond", new LevelStats("Hyperdrive Heroes", LevelType.Survival, false, 8, Properties.Resources.round_hyperdrive_heroes_icon) },
            { "round_jump_club",                  new LevelStats("Jump Club", LevelType.Survival, false, 1, Properties.Resources.round_jump_club_icon) },
            { "round_tunnel",                     new LevelStats("Roll Out", LevelType.Survival, false, 1, Properties.Resources.round_roll_out_icon) },
            { "round_snowballsurvival",           new LevelStats("Snowball Survival", LevelType.Survival, false, 3, Properties.Resources.round_snowball_survival_icon) },
            { "round_robotrampage_arena_2",       new LevelStats("Stompin' Ground", LevelType.Survival, false, 5, Properties.Resources.round_stompin_ground_icon) },
            { "round_spin_ring_symphony_launch_show", new LevelStats("The Swiveller", LevelType.Survival, false, 7, Properties.Resources.round_the_swiveller_icon) },

            { "round_airtime",                    new LevelStats("Airtime", LevelType.Hunt, false, 6, Properties.Resources.round_airtime_icon) },
            { "round_bluejay",                    new LevelStats("Bean Hill Zone", LevelType.Hunt, false, 7, Properties.Resources.round_bean_hill_zone_icon) },
            { "round_hoops_revenge_symphony_launch_show", new LevelStats("Bounce Party", LevelType.Hunt, false, 7, Properties.Resources.round_bounce_party_icon) },
            { "round_king_of_the_hill",           new LevelStats("Bubble Trouble", LevelType.Hunt, false, 5, Properties.Resources.round_bubble_trouble_icon) },
            { "round_1v1_button_basher",          new LevelStats("Button Bashers", LevelType.Hunt, false, 4, Properties.Resources.round_button_bashers_icon) },
            { "round_ffa_button_bashers_squads_almond", new LevelStats("Frantic Factory", LevelType.Hunt, false, 8, Properties.Resources.round_frantic_factory_icon) },
            { "round_slippy_slide",               new LevelStats("Hoop Chute", LevelType.Hunt, false, 9, Properties.Resources.round_hoop_chute_icon) },
            { "round_hoops_blockade_solo",        new LevelStats("Hoopsie Legends", LevelType.Hunt, false, 2, Properties.Resources.round_hoopsie_legends_icon) },
            { "round_follow-the-leader_s6_launch",new LevelStats("Leading Light", LevelType.Hunt, false, 6, Properties.Resources.round_leading_light_icon) },
            { "round_penguin_solos",              new LevelStats("Pegwin Pool Party", LevelType.Hunt, false, 5, Properties.Resources.round_pegwin_pool_party_icon) },
            { "round_tail_tag",                   new LevelStats("Tail Tag", LevelType.Hunt, false, 1, Properties.Resources.round_tail_tag_icon) },
            { "round_1v1_volleyfall_symphony_launch_show", new LevelStats("Volleyfall", LevelType.Hunt, false, 7, Properties.Resources.round_volleyfall_icon) },

            { "round_match_fall",                 new LevelStats("Perfect Match", LevelType.Logic, false, 1, Properties.Resources.round_perfect_match_icon) },
            { "round_fruit_bowl",                 new LevelStats("Sum Fruit", LevelType.Logic, false, 5, Properties.Resources.round_sum_fruit_icon) },

            { "round_basketfall_s4_show",         new LevelStats("Basketfall", LevelType.Team, false, 4, Properties.Resources.round_basketfall_icon) },
            { "round_egg_grab",                   new LevelStats("Egg Scramble", LevelType.Team, false, 1, Properties.Resources.round_egg_scramble_icon) },
            { "round_egg_grab_02",                new LevelStats("Egg Siege", LevelType.Team, false, 2, Properties.Resources.round_egg_siege_icon) },
            { "round_fall_ball_60_players",       new LevelStats("Fall Ball", LevelType.Team, false, 1, Properties.Resources.round_fall_ball_icon) },
            { "round_ballhogs",                   new LevelStats("Hoarders", LevelType.Team, false, 1, Properties.Resources.round_hoarders_icon) },
            { "round_hoops",                      new LevelStats("Hoopsie Daisy", LevelType.Team, false, 1, Properties.Resources.round_hoopsie_daisy_icon) },
            { "round_jinxed",                     new LevelStats("Jinxed", LevelType.Team, false, 1, Properties.Resources.round_jinxed_icon) },
            { "round_chicken_chase",              new LevelStats("Pegwin Pursuit", LevelType.Team, false, 3, Properties.Resources.round_pegwin_pursuit_icon) },
            { "round_territory_control_s4_show",  new LevelStats("Power Trip", LevelType.Team, false, 4, Properties.Resources.round_power_trip_icon) },
            { "round_rocknroll",                  new LevelStats("Rock 'n' Roll", LevelType.Team, false, 1, Properties.Resources.round_rock_n_roll_icon) },
            { "round_snowy_scrap",                new LevelStats("Snowy Scrap", LevelType.Team, false, 3, Properties.Resources.round_snowy_scrap_icon) },
            { "round_conveyor_arena",             new LevelStats("Team Tail Tag", LevelType.Team, false, 1, Properties.Resources.round_team_tail_tag_icon) },

            { "round_blastball_arenasurvival_symphony_launch_show", new LevelStats("Blast Ball", LevelType.Survival, true, 7, Properties.Resources.round_blast_ball_icon) },
            { "round_fall_mountain_hub_complete", new LevelStats("Fall Mountain", LevelType.Race, true, 1, Properties.Resources.round_fall_mountain_icon) },
            { "round_floor_fall",                 new LevelStats("Hex-A-Gone", LevelType.Survival, true, 1, Properties.Resources.round_hex_a_gone_icon) },
            { "round_hexaring_symphony_launch_show", new LevelStats("Hex-A-Ring", LevelType.Survival, true, 7, Properties.Resources.round_hex_a_ring_icon) },
            { "round_hexsnake_almond",            new LevelStats("Hex-A-Terrestrial", LevelType.Survival, true, 8, Properties.Resources.round_hex_a_terrestrial_icon) },
            { "round_jump_showdown",              new LevelStats("Jump Showdown", LevelType.Survival, true, 1, Properties.Resources.round_jump_showdown_icon) },
            { "round_kraken_attack",              new LevelStats("Kraken Slam", LevelType.Survival, true, 9, Properties.Resources.round_kraken_slam_icon) },
            { "round_crown_maze",                 new LevelStats("Lost Temple", LevelType.Race, true, 5, Properties.Resources.round_lost_temple_icon) },
            { "round_tunnel_final",               new LevelStats("Roll Off", LevelType.Survival, true, 3, Properties.Resources.round_roll_off_icon) },
            { "round_royal_rumble",               new LevelStats("Royal Fumble", LevelType.Hunt, true, 1, Properties.Resources.round_royal_fumble_icon) },
            { "round_thin_ice",                   new LevelStats("Thin Ice", LevelType.Survival, true, 3, Properties.Resources.round_thin_ice_icon) },
            { "round_tiptoefinale_almond",        new LevelStats("Tip Toe Finale", LevelType.Race, true, 8, Properties.Resources.round_tip_toe_finale_icon) },

            { "round_invisibeans",                new LevelStats("Sweet Thieves", LevelType.Invisibeans, false, 6, Properties.Resources.round_sweet_thieves_icon) },
            { "round_pumpkin_pie",                new LevelStats("Treat Thieves", LevelType.Invisibeans, false, 8, Properties.Resources.round_treat_thieves_icon) }
        };
        public static Dictionary<string, LevelStats> ALL_FRE = new Dictionary<string, LevelStats>(StringComparer.OrdinalIgnoreCase) {
            { "round_lava",                       new LevelStats("Slime Climb", LevelType.Race, false, 1, Properties.Resources.round_slime_climb_icon) },
            { "round_gauntlet_09_symphony_launch_show", new LevelStats("Track Attack", LevelType.Race, false, 7, Properties.Resources.round_track_attack_icon) },
            { "round_pixelperfect_almond",        new LevelStats("Pixel Painters", LevelType.Race, false, 8, Properties.Resources.round_pixel_painters_icon) },
            { "round_satellitehoppers_almond",    new LevelStats("Cosmic Highway", LevelType.Race, false, 8, Properties.Resources.round_cosmic_highway_icon) },
            { "round_starlink_almond",            new LevelStats("Starchart", LevelType.Race, false, 8, Properties.Resources.round_starchart_icon) },
            { "round_short_circuit_2_symphony_launch_show", new LevelStats("Speed Circuit", LevelType.Race, false, 7, Properties.Resources.round_speed_circuit_icon) },
            { "round_gauntlet_10_almond",         new LevelStats("Space Race", LevelType.Race, false, 8, Properties.Resources.round_space_race_icon) },
            { "round_gauntlet_07",                new LevelStats("Treetop Tumble", LevelType.Race, false, 5, Properties.Resources.round_treetop_tumble_icon) },
            { "round_gauntlet_05",                new LevelStats("Tundra Run", LevelType.Race, false, 3, Properties.Resources.round_tundra_run_icon) },
            { "round_see_saw_360",                new LevelStats("Full Tilt", LevelType.Race, false, 6, Properties.Resources.round_full_tilt_icon) },
            { "round_gauntlet_04",                new LevelStats("Knight Fever", LevelType.Race, false, 2, Properties.Resources.round_knight_fever_icon) },
            { "round_dodge_fall",                 new LevelStats("Fruit Chute", LevelType.Race, false, 1, Properties.Resources.round_fruit_chute_icon) },
            { "round_slide_chute",                new LevelStats("Speed Slider", LevelType.Race, false, 9, Properties.Resources.round_speed_slider_icon) },
            { "round_gauntlet_02",                new LevelStats("Dizzy Heights", LevelType.Race, false, 1, Properties.Resources.round_dizzy_heights_icon) },
            { "round_gauntlet_01",                new LevelStats("Hit Parade", LevelType.Race, false, 1, Properties.Resources.round_hit_parade_icon) },
            { "round_slimeclimb_2",               new LevelStats("The Slimescraper", LevelType.Race, false, 4, Properties.Resources.round_the_slimescraper_icon) },
            { "round_gauntlet_06",                new LevelStats("Skyline Stumble", LevelType.Race, false, 4, Properties.Resources.round_skyline_stumble_icon) },
            { "round_gauntlet_03",                new LevelStats("The Whirlygig", LevelType.Race, false, 1, Properties.Resources.round_the_whirlygig_icon) },
            { "round_see_saw",                    new LevelStats("See Saw", LevelType.Race, false, 1, Properties.Resources.round_see_saw_icon) },
            { "round_pipedup_s6_launch",          new LevelStats("Pipe Dream", LevelType.Race, false, 6, Properties.Resources.round_pipe_dream_icon) },
            { "round_shortcircuit",               new LevelStats("Short Circuit", LevelType.Race, false, 4, Properties.Resources.round_short_circuit_icon) },
            { "round_wall_guys",                  new LevelStats("Wall Guys", LevelType.Race, false, 2, Properties.Resources.round_wall_guys_icon) },
            { "round_chompchomp",                 new LevelStats("Gate Crash", LevelType.Race, false, 1, Properties.Resources.round_gate_crash_icon) },
            { "round_iceclimb",                   new LevelStats("Freezy Peak", LevelType.Race, false, 3, Properties.Resources.round_freezy_peak_icon) },
            { "round_skeefall",                   new LevelStats("Ski Fall", LevelType.Race, false, 3, Properties.Resources.round_ski_fall_icon) },
            { "round_gauntlet_08",                new LevelStats("Party Promenade", LevelType.Race, false, 6, Properties.Resources.round_party_promenade_icon) },
            { "round_tunnel_race",                new LevelStats("Roll On", LevelType.Race, false, 4, Properties.Resources.round_roll_on_icon) },
            { "round_door_dash",                  new LevelStats("Door Dash", LevelType.Race, false, 1, Properties.Resources.round_door_dash_icon) },
            { "round_follow_the_line",            new LevelStats("Puzzle Path", LevelType.Race, false, 9, Properties.Resources.round_puzzle_path_icon) },
            { "round_tip_toe",                    new LevelStats("Tip Toe", LevelType.Race, false, 1, Properties.Resources.round_tip_toe_icon) },
            { "round_biggestfan",                 new LevelStats("Big Fans", LevelType.Race, false, 2, Properties.Resources.round_big_fans_icon) },
            { "round_drumtop",                    new LevelStats("Lily Leapers", LevelType.Race, false, 5, Properties.Resources.round_lily_leapers_icon) },

            { "round_blastballruins",             new LevelStats("Blastlantis", LevelType.Survival, false, 9, Properties.Resources.round_blastlantis_icon) },
            { "round_snowballsurvival",           new LevelStats("Snowball Survival", LevelType.Survival, false, 3, Properties.Resources.round_snowball_survival_icon) },
            { "round_tunnel",                     new LevelStats("Roll Out", LevelType.Survival, false, 1, Properties.Resources.round_roll_out_icon) },
            { "round_robotrampage_arena_2",       new LevelStats("Stompin' Ground", LevelType.Survival, false, 5, Properties.Resources.round_stompin_ground_icon) },
            { "round_fruitpunch_s4_show",         new LevelStats("Big Shots", LevelType.Survival, false, 4, Properties.Resources.round_big_shots_icon) },
            { "round_block_party",                new LevelStats("Block Party", LevelType.Survival, false, 1, Properties.Resources.round_block_party_icon) },
            { "round_hoverboardsurvival_s4_show", new LevelStats("Hoverboard Heroes", LevelType.Survival, false, 4, Properties.Resources.round_hoverboard_heroes_icon) },
            { "round_hoverboardsurvival2_almond", new LevelStats("Hyperdrive Heroes", LevelType.Survival, false, 8, Properties.Resources.round_hyperdrive_heroes_icon) },
            { "round_spin_ring_symphony_launch_show", new LevelStats("The Swiveller", LevelType.Survival, false, 7, Properties.Resources.round_the_swiveller_icon) },
            { "round_jump_club",                  new LevelStats("Jump Club", LevelType.Survival, false, 1, Properties.Resources.round_jump_club_icon) },

            { "round_tail_tag",                   new LevelStats("Tail Tag", LevelType.Hunt, false, 1, Properties.Resources.round_tail_tag_icon) },
            { "round_king_of_the_hill",           new LevelStats("Bubble Trouble", LevelType.Hunt, false, 5, Properties.Resources.round_bubble_trouble_icon) },
            { "round_airtime",                    new LevelStats("Airtime", LevelType.Hunt, false, 6, Properties.Resources.round_airtime_icon) },
            { "round_hoops_blockade_solo",        new LevelStats("Hoopsie Legends", LevelType.Hunt, false, 2, Properties.Resources.round_hoopsie_legends_icon) },
            { "round_follow-the-leader_s6_launch",new LevelStats("Leading Light", LevelType.Hunt, false, 6, Properties.Resources.round_leading_light_icon) },
            { "round_1v1_button_basher",          new LevelStats("Button Bashers", LevelType.Hunt, false, 4, Properties.Resources.round_button_bashers_icon) },
            { "round_penguin_solos",              new LevelStats("Pegwin Pool Party", LevelType.Hunt, false, 5, Properties.Resources.round_pegwin_pool_party_icon) },
            { "round_hoops_revenge_symphony_launch_show", new LevelStats("Bounce Party", LevelType.Hunt, false, 7, Properties.Resources.round_bounce_party_icon) },
            { "round_slippy_slide",               new LevelStats("Hoop Chute", LevelType.Hunt, false, 9, Properties.Resources.round_hoop_chute_icon) },
            { "round_ffa_button_bashers_squads_almond", new LevelStats("Frantic Factory", LevelType.Hunt, false, 8, Properties.Resources.round_frantic_factory_icon) },
            { "round_1v1_volleyfall_symphony_launch_show", new LevelStats("Volleyfall", LevelType.Hunt, false, 7, Properties.Resources.round_volleyfall_icon) },
            { "round_bluejay",                    new LevelStats("Bean Hill Zone", LevelType.Hunt, false, 7, Properties.Resources.round_bean_hill_zone_icon) },

            { "round_fruit_bowl",                 new LevelStats("Sum Fruit", LevelType.Logic, false, 5, Properties.Resources.round_sum_fruit_icon) },
            { "round_match_fall",                 new LevelStats("Perfect Match", LevelType.Logic, false, 1, Properties.Resources.round_perfect_match_icon) },

            { "round_egg_grab_02",                new LevelStats("Egg Siege", LevelType.Team, false, 2, Properties.Resources.round_egg_siege_icon) },
            { "round_conveyor_arena",             new LevelStats("Team Tail Tag", LevelType.Team, false, 1, Properties.Resources.round_team_tail_tag_icon) },
            { "round_basketfall_s4_show",         new LevelStats("Basketfall", LevelType.Team, false, 4, Properties.Resources.round_basketfall_icon) },
            { "round_snowy_scrap",                new LevelStats("Snowy Scrap", LevelType.Team, false, 3, Properties.Resources.round_snowy_scrap_icon) },
            { "round_chicken_chase",              new LevelStats("Pegwin Pursuit", LevelType.Team, false, 3, Properties.Resources.round_pegwin_pursuit_icon) },
            { "round_fall_ball_60_players",       new LevelStats("Fall Ball", LevelType.Team, false, 1, Properties.Resources.round_fall_ball_icon) },
            { "round_hoops",                      new LevelStats("Hoopsie Daisy", LevelType.Team, false, 1, Properties.Resources.round_hoopsie_daisy_icon) },
            { "round_ballhogs",                   new LevelStats("Hoarders", LevelType.Team, false, 1, Properties.Resources.round_hoarders_icon) },
            { "round_jinxed",                     new LevelStats("Jinxed", LevelType.Team, false, 1, Properties.Resources.round_jinxed_icon) },
            { "round_egg_grab",                   new LevelStats("Egg Scramble", LevelType.Team, false, 1, Properties.Resources.round_egg_scramble_icon) },
            { "round_territory_control_s4_show",  new LevelStats("Power Trip", LevelType.Team, false, 4, Properties.Resources.round_power_trip_icon) },
            { "round_rocknroll",                  new LevelStats("Rock 'n' Roll", LevelType.Team, false, 1, Properties.Resources.round_rock_n_roll_icon) },

            { "round_blastball_arenasurvival_symphony_launch_show", new LevelStats("Blast Ball", LevelType.Survival, true, 7, Properties.Resources.round_blast_ball_icon) },
            { "round_royal_rumble",               new LevelStats("Royal Fumble", LevelType.Hunt, true, 1, Properties.Resources.round_royal_fumble_icon) },
            { "round_tunnel_final",               new LevelStats("Roll Off", LevelType.Survival, true, 3, Properties.Resources.round_roll_off_icon) },
            { "round_tiptoefinale_almond",        new LevelStats("Tip Toe Finale", LevelType.Race, true, 8, Properties.Resources.round_tip_toe_finale_icon) },
            { "round_thin_ice",                   new LevelStats("Thin Ice", LevelType.Survival, true, 3, Properties.Resources.round_thin_ice_icon) },
            { "round_floor_fall",                 new LevelStats("Hex-A-Gone", LevelType.Survival, true, 1, Properties.Resources.round_hex_a_gone_icon) },
            { "round_hexaring_symphony_launch_show", new LevelStats("Hex-A-Ring", LevelType.Survival, true, 7, Properties.Resources.round_hex_a_ring_icon) },
            { "round_hexsnake_almond",            new LevelStats("Hex-A-Terrestrial", LevelType.Survival, true, 8, Properties.Resources.round_hex_a_terrestrial_icon) },
            { "round_fall_mountain_hub_complete", new LevelStats("Fall Mountain", LevelType.Race, true, 1, Properties.Resources.round_fall_mountain_icon) },
            { "round_jump_showdown",              new LevelStats("Jump Showdown", LevelType.Survival, true, 1, Properties.Resources.round_jump_showdown_icon) },
            { "round_crown_maze",                 new LevelStats("Lost Temple", LevelType.Race, true, 5, Properties.Resources.round_lost_temple_icon) },
            { "round_kraken_attack",              new LevelStats("Kraken Slam", LevelType.Survival, true, 9, Properties.Resources.round_kraken_slam_icon) },

            { "round_pumpkin_pie",                new LevelStats("Treat Thieves", LevelType.Invisibeans, false, 8, Properties.Resources.round_treat_thieves_icon) },
            { "round_invisibeans",                new LevelStats("Sweet Thieves", LevelType.Invisibeans, false, 6, Properties.Resources.round_sweet_thieves_icon) }
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

            { "FallGuy_BlueJay_UNPACKED",          "round_bluejay" },
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
            { "FallGuy_SlideChute",                "round_slide_chute" }
        };
        public Image RoundIcon { get; set; }
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
        public LevelType Type;
        public bool IsFinal;
        public TimeSpan AveDuration { get { return TimeSpan.FromSeconds((int)this.Duration.TotalSeconds / (this.Played == 0 ? 1 : this.Played)); } }
        public TimeSpan AveFinish { get { return TimeSpan.FromSeconds((double)this.FinishTime.TotalSeconds / (this.FinishedCount == 0 ? 1 : this.FinishedCount)); } }
        public TimeSpan Duration;
        public TimeSpan FinishTime;
        public List<RoundInfo> Stats;
        public int Season;
        public int FinishedCount;

        public LevelStats(string levelName, LevelType type, bool isFinal, int season, Image roundIcon) {
            this.RoundIcon = roundIcon;
            this.Name = levelName;
            this.Type = type;
            this.Season = season;
            this.IsFinal = isFinal;
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