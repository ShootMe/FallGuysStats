using LiteDB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Stats : Form {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Stats());
        }

        private static DateTime SeasonStart = new DateTime(2020, 8, 2, 0, 0, 0, DateTimeKind.Local);
        private static DateTime WeekStart = DateTime.SpecifyKind(DateTime.Now.AddDays(-7).ToUniversalTime(), DateTimeKind.Local);
        private static DateTime SessionStart = DateTime.SpecifyKind(DateTime.Now.ToUniversalTime(), DateTimeKind.Local);

        private List<LevelStats> details = new List<LevelStats>();
        private Dictionary<string, LevelStats> lookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        private int Shows;
        private int Rounds;
        private TimeSpan Duration;
        private int Wins;
        private int Finals;
        private string logPath;
        private int nextShowID;
        private bool loadingExisting;
        private LiteDatabase statsDB;
        private ILiteCollection<RoundInfo> roundDetails;
        public Stats() {
            InitializeComponent();

            logFile.OnParsedLogLines += LogFile_OnParsedLogLines;

            details.Add(new LevelStats("Door Dash", "round_door_dash"));
            details.Add(new LevelStats("Dizzy Heights", "round_gauntlet_02"));
            details.Add(new LevelStats("Fruit Chute", "round_dodge_fall"));
            details.Add(new LevelStats("Gate Crash", "round_chompchomp"));
            details.Add(new LevelStats("Hit Parade", "round_gauntlet_01"));
            details.Add(new LevelStats("Sea Saw", "round_see_saw"));
            details.Add(new LevelStats("Slime Climb", "round_lava"));
            details.Add(new LevelStats("Tip Toe", "round_tip_toe"));
            details.Add(new LevelStats("Whirlygig", "round_gauntlet_03"));

            details.Add(new LevelStats("Block Party", "round_block_party"));
            details.Add(new LevelStats("Jump Club", "round_jump_club"));
            details.Add(new LevelStats("Perfect Match", "round_match_fall"));
            details.Add(new LevelStats("Roll Out", "round_tunnel"));
            details.Add(new LevelStats("Tail Tag", "round_tail_tag"));

            details.Add(new LevelStats("Egg Scramble", "round_egg_grab"));
            details.Add(new LevelStats("Fall Ball", "round_fall_ball_60_players"));
            details.Add(new LevelStats("Hoarders", "round_ballhogs"));
            details.Add(new LevelStats("Hoopsie Daisy", "round_hoops"));
            details.Add(new LevelStats("Jinxed", "round_jinxed"));
            details.Add(new LevelStats("Rock'N'Roll", "round_rocknroll"));
            details.Add(new LevelStats("Team Tail Tag", "round_conveyor_arena"));

            details.Add(new LevelStats("Fall Mountain", "round_fall_mountain_hub_complete"));
            details.Add(new LevelStats("Hex-A-Gone", "round_floor_fall"));
            details.Add(new LevelStats("Jump Showdown", "round_jump_showdown"));
            details.Add(new LevelStats("Royal Fumble", "round_royal_rumble"));

            for (int i = 0; i < details.Count; i++) {
                LevelStats calculator = details[i];
                lookup.Add(calculator.LevelName, calculator);
            }

            gridDetails.DataSource = details;

            statsDB = new LiteDatabase(@"data.db");
            roundDetails = statsDB.GetCollection<RoundInfo>("RoundDetails");
        }
        private void Stats_Shown(object sender, EventArgs e) {
            if (roundDetails.Count() > 0) {
                nextShowID = roundDetails.Max(x => x.ShowID);
                List<RoundInfo> rounds = new List<RoundInfo>();
                rounds.AddRange(roundDetails.FindAll());
                loadingExisting = true;
                LogFile_OnParsedLogLines(rounds);
                loadingExisting = false;
            }

            logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
            logFile.Start(logPath, "Player.log");
        }
        private void LogFile_OnParsedLogLines(List<RoundInfo> round) {
            if (!loadingExisting) { statsDB.BeginTrans(); }

            foreach (RoundInfo stat in round) {
                if (!loadingExisting) {
                    RoundInfo info = roundDetails.FindOne(x => x.Start == stat.Start && x.Name == stat.Name);
                    if (info == null) {
                        if (stat.Round == 1) {
                            nextShowID++;
                        }
                        stat.ShowID = nextShowID;

                        roundDetails.Insert(stat);
                    } else {
                        continue;
                    }
                }

                if (stat.Round == 1) {
                    Shows++;
                }
                Rounds++;
                Duration += stat.End - stat.Start;
                switch (stat.Name) {
                    case "round_fall_mountain_hub_complete":
                    case "round_floor_fall":
                    case "round_jump_showdown":
                    case "round_royal_rumble":
                        Finals++;
                        if (stat.Position == 1) {
                            Wins++;
                        }
                        break;
                }

                if (lookup.ContainsKey(stat.Name)) {
                    lookup[stat.Name].Add(stat);
                }
            }

            if (!loadingExisting) { statsDB.Commit(); }
            this.Invoke((Action)UpdateTotals);
        }
        private void ClearTotals() {
            Rounds = 0;
            Duration = TimeSpan.Zero;
            Wins = 0;
            Shows = 0;
            Finals = 0;
        }
        private void UpdateTotals() {
            lblTotalRounds.Text = $"Rounds: {Rounds}";
            lblTotalShows.Text = $"Shows: {Shows}";
            lblTotalTime.Text = $"Time Played: {Duration:h\\:mm\\:ss}";
            lblTotalWins.Text = $"Wins: {Wins}";
            float finalChance = (float)Finals * 100 / (Shows == 0 ? 1 : Shows);
            lblFinalChance.Text = $"Final %: {finalChance:0.0}";
            float winChance = (float)Wins * 100 / (Shows == 0 ? 1 : Shows);
            lblWinChance.Text = $"Win %: {winChance:0.0}";
            gridDetails.Refresh();
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            int pos = 0;
            gridDetails.Columns.Add(new DataGridViewImageColumn() { Name = "Info", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "Level Info" });
            gridDetails.Setup("Name", pos++, 0, "Level Name", DataGridViewContentAlignment.MiddleLeft);
            gridDetails.Setup("Info", pos++, 20, "", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("Played", pos++, 50, "Played", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Qualified", pos++, 60, "Qualified", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Gold", pos++, 50, "Gold", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Silver", pos++, 50, "Silver", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Bronze", pos++, 50, "Bronze", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Kudos", pos++, 60, "Kudos", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("AveKudos", pos++, 70, "Avg Kudos", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("AveDuration", pos++, 80, "Avg Duration", DataGridViewContentAlignment.MiddleRight);
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                switch ((string)e.Value) {
                    case "Door Dash":
                    case "Dizzy Heights":
                    case "Fruit Chute":
                    case "Gate Crash":
                    case "Hit Parade":
                    case "Sea Saw":
                    case "Slime Climb":
                    case "Tip Toe":
                    case "Whirlygig":
                        e.CellStyle.BackColor = Color.LightGoldenrodYellow;
                        break;
                    case "Block Party":
                    case "Jump Club":
                    case "Perfect Match":
                    case "Roll Out":
                    case "Tail Tag":
                        e.CellStyle.BackColor = Color.LightBlue;
                        break;
                    case "Egg Scramble":
                    case "Fall Ball":
                    case "Hoarders":
                    case "Hoopsie Daisy":
                    case "Jinxed":
                    case "Rock'N'Roll":
                    case "Team Tail Tag":
                        e.CellStyle.BackColor = Color.LightGreen;
                        break;
                    case "Fall Mountain":
                    case "Hex-A-Gone":
                    case "Jump Showdown":
                    case "Royal Fumble":
                        e.CellStyle.BackColor = Color.Pink;
                        break;
                }
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Info" && e.Value == null) {
                gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Click to view level stats";
                e.Value = Properties.Resources.info;
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "AveDuration") {
                LevelStats info = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                e.Value = info.AveDuration.ToString("m\\:ss");
            }
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (gridDetails.Columns[e.ColumnIndex].Name == "Info") {
                gridDetails.Cursor = Cursors.Hand;
            } else {
                gridDetails.Cursor = Cursors.Default;
            }
        }
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (gridDetails.Columns[e.ColumnIndex].Name == "Info") {
                using (LevelDetails details = new LevelDetails()) {
                    LevelStats stats = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                    details.LevelName = stats.Name;
                    List<RoundInfo> rounds = new List<RoundInfo>();
                    rounds.AddRange(roundDetails.Find(x => x.Name == stats.LevelName));
                    rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                        return one.Start.CompareTo(two.Start);
                    });
                    for (int i = 0; i < rounds.Count; i++) {
                        rounds[i].ToLocalTime();
                    }
                    details.RoundDetails = rounds;
                    details.ShowDialog(this);
                }
            }
        }
        private void rdAll_CheckedChanged(object sender, EventArgs e) {
            RadioButton button = sender as RadioButton;
            if (!button.Checked) { return; }

            for (int i = 0; i < details.Count; i++) {
                LevelStats calculator = details[i];
                calculator.Clear();
            }

            ClearTotals();

            List<RoundInfo> rounds = new List<RoundInfo>();
            if (button == rdAll) {
                rounds.AddRange(roundDetails.FindAll());
            } else if (button == rdSeason) {
                rounds.AddRange(roundDetails.Find(x => x.Start > SeasonStart));
            } else if (button == rdWeek) {
                rounds.AddRange(roundDetails.Find(x => x.Start > WeekStart));
            } else {
                rounds.AddRange(roundDetails.Find(x => x.Start > SessionStart));
            }

            rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                return one.Start.CompareTo(two.Start);
            });

            loadingExisting = true;
            LogFile_OnParsedLogLines(rounds);
            loadingExisting = false;
        }
    }
}