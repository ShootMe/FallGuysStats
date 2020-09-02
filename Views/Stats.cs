﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class Stats : Form {
        [STAThread]
        static void Main(string[] args) {
            try {
                foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.old")) {
                    int retries = 0;
                    while (retries < 20) {
                        try {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                            break;
                        } catch {
                            retries++;
                        }
                        Thread.Sleep(50);
                    }
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Stats());
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static DateTime SeasonStart = new DateTime(2020, 8, 2, 0, 0, 0, DateTimeKind.Local);
        private static DateTime WeekStart = DateTime.SpecifyKind(DateTime.Now.AddDays(-7).ToUniversalTime(), DateTimeKind.Local);
        private static DateTime DayStart = DateTime.SpecifyKind(DateTime.Now.Date.ToUniversalTime(), DateTimeKind.Local);
        private static DateTime SessionStart = DateTime.SpecifyKind(DateTime.Now.ToUniversalTime(), DateTimeKind.Local);

        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public bool RoundChanged;
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        public int Shows;
        public int Rounds;
        public TimeSpan Duration;
        public int Wins;
        public int Finals;
        public int Kudos;
        private string logPath;
        private int nextShowID;
        private bool loadingExisting;
        private LiteDatabase statsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        private Overlay overlay;
        private bool setOverlayPosition;
        public Stats() {
            InitializeComponent();

            Text = $"Fall Guys Stats v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";

            logFile.OnParsedLogLines += LogFile_OnParsedLogLines;
            logFile.OnNewLogFileDate += LogFile_OnNewLogFileDate;
            logFile.OnError += LogFile_OnError;
            logFile.OnParsedLogLinesCurrent += LogFile_OnParsedLogLinesCurrent;

            StatDetails.Add(new LevelStats("round_door_dash", LevelType.Race));
            StatDetails.Add(new LevelStats("round_gauntlet_02", LevelType.Race));
            StatDetails.Add(new LevelStats("round_dodge_fall", LevelType.Race));
            StatDetails.Add(new LevelStats("round_chompchomp", LevelType.Race));
            StatDetails.Add(new LevelStats("round_gauntlet_01", LevelType.Race));
            StatDetails.Add(new LevelStats("round_see_saw", LevelType.Race));
            StatDetails.Add(new LevelStats("round_lava", LevelType.Race));
            StatDetails.Add(new LevelStats("round_tip_toe", LevelType.Race));
            StatDetails.Add(new LevelStats("round_gauntlet_03", LevelType.Race));

            StatDetails.Add(new LevelStats("round_block_party", LevelType.Survival));
            StatDetails.Add(new LevelStats("round_jump_club", LevelType.Survival));
            StatDetails.Add(new LevelStats("round_match_fall", LevelType.Survival));
            StatDetails.Add(new LevelStats("round_tunnel", LevelType.Survival));
            StatDetails.Add(new LevelStats("round_tail_tag", LevelType.Survival));

            StatDetails.Add(new LevelStats("round_egg_grab", LevelType.Team));
            StatDetails.Add(new LevelStats("round_fall_ball_60_players", LevelType.Team));
            StatDetails.Add(new LevelStats("round_ballhogs", LevelType.Team));
            StatDetails.Add(new LevelStats("round_hoops", LevelType.Team));
            StatDetails.Add(new LevelStats("round_jinxed", LevelType.Team));
            StatDetails.Add(new LevelStats("round_rocknroll", LevelType.Team));
            StatDetails.Add(new LevelStats("round_conveyor_arena", LevelType.Team));

            StatDetails.Add(new LevelStats("round_fall_mountain_hub_complete", LevelType.Final));
            StatDetails.Add(new LevelStats("round_floor_fall", LevelType.Final));
            StatDetails.Add(new LevelStats("round_jump_showdown", LevelType.Final));
            StatDetails.Add(new LevelStats("round_royal_rumble", LevelType.Final));

            for (int i = 0; i < StatDetails.Count; i++) {
                LevelStats calculator = StatDetails[i];
                StatLookup.Add(calculator.LevelName, calculator);
            }

            gridDetails.DataSource = StatDetails;

            statsDB = new LiteDatabase(@"data.db");
            RoundDetails = statsDB.GetCollection<RoundInfo>("RoundDetails");
            statsDB.BeginTrans();
            RoundDetails.EnsureIndex(x => x.Name);
            RoundDetails.EnsureIndex(x => x.ShowID);
            RoundDetails.EnsureIndex(x => x.Round);
            RoundDetails.EnsureIndex(x => x.Start);
            RoundDetails.EnsureIndex(x => x.InParty);
            statsDB.Commit();

            CurrentRound = new List<RoundInfo>();
            overlay = new Overlay() { StatsForm = this };
            overlay.StartTimer();
        }
        private void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                statsDB.Dispose();
            } catch { }
        }
        private void Stats_Shown(object sender, EventArgs e) {
            try {
                if (RoundDetails.Count() > 0) {
                    nextShowID = RoundDetails.Max(x => x.ShowID);
                    List<RoundInfo> rounds = new List<RoundInfo>();
                    rounds.AddRange(RoundDetails.FindAll());
                    rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                        int showCompare = one.ShowID.CompareTo(two.ShowID);
                        return showCompare != 0 ? showCompare : one.Round.CompareTo(two.Round);
                    });

                    for (int i = rounds.Count - 1; i >= 0; i--) {
                        RoundInfo info = rounds[i];
                        CurrentRound.Insert(0, info);
                        if (info.Round == 1) {
                            break;
                        }
                    }
                    RoundChanged = true;
                    loadingExisting = true;
                    LogFile_OnParsedLogLines(rounds);
                    loadingExisting = false;
                }

                logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                logFile.Start(logPath, "Player.log");
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogFile_OnError(string error) {
            if (!this.Disposing && !this.IsDisposed) {
                try {
                    if (this.InvokeRequired) {
                        this.Invoke((Action<string>)LogFile_OnError, error);
                    } else {
                        MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch { }
            }
        }
        private void LogFile_OnNewLogFileDate(DateTime newDate) {
            if (SessionStart != newDate) {
                SessionStart = newDate;
                if (rdSession.Checked) {
                    rdAll_CheckedChanged(rdSession, null);
                }
            }
        }
        private void LogFile_OnParsedLogLinesCurrent(List<RoundInfo> round) {
            if (RoundChanged) { return; }

            lock (CurrentRound) {
                if (CurrentRound == null || CurrentRound.Count != round.Count) {
                    CurrentRound = round;
                    RoundChanged = round.Count > 0;
                } else {
                    for (int i = 0; i < CurrentRound.Count; i++) {
                        RoundInfo info = CurrentRound[i];
                        if (!info.Equals(round[i])) {
                            CurrentRound = round;
                            RoundChanged = true;
                            break;
                        }
                    }
                }
            }
        }
        private void LogFile_OnParsedLogLines(List<RoundInfo> round) {
            try {
                if (!loadingExisting) { statsDB.BeginTrans(); }

                foreach (RoundInfo stat in round) {
                    if (!loadingExisting) {
                        RoundInfo info = RoundDetails.FindOne(x => x.Start == stat.Start && x.Name == stat.Name);
                        if (info == null) {
                            if (stat.Round == 1) {
                                nextShowID++;
                            }
                            stat.ShowID = nextShowID;

                            RoundDetails.Insert(stat);
                        } else {
                            continue;
                        }
                    }

                    if (stat.Round == 1) {
                        Shows++;
                    }
                    Rounds++;
                    Duration += stat.End - stat.Start;
                    Kudos += stat.Kudos;

                    if (StatLookup.ContainsKey(stat.Name)) {
                        stat.ToLocalTime();
                        LevelStats levelStats = StatLookup[stat.Name];
                        if (levelStats.Type == LevelType.Final) {
                            Finals++;
                            if (stat.Qualified) {
                                Wins++;
                            }
                        }
                        levelStats.Add(stat);
                    }
                }

                lock (CurrentRound) {
                    CurrentRound.Clear();
                    for (int i = round.Count - 1; i >= 0; i--) {
                        RoundInfo info = round[i];
                        CurrentRound.Insert(0, info);
                        if (info.Round == 1) {
                            break;
                        }
                    }
                    RoundChanged = true;
                }

                if (!loadingExisting) { statsDB.Commit(); }

                if (!this.Disposing && !this.IsDisposed) {
                    try {
                        this.Invoke((Action)UpdateTotals);
                    } catch { }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public Tuple<int, int, TimeSpan?, int?, int, int> GetLevelInfo(string name) {
            TimeSpan? best = null;
            LevelStats levelDetails = null;
            int qualifyCount = 0;
            int totalCount = 0;
            int? bestScore = null;
            int streak = 0;
            int maxStreak = 0;
            bool wonFinal = false;
            List<RoundInfo> rounds = new List<RoundInfo>();
            for (int i = 0; i < StatDetails.Count; i++) {
                rounds.AddRange(StatDetails[i].Stats);
            }
            rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                return one.Start.CompareTo(two.Start);
            });

            for (int i = 0; i < rounds.Count; i++) {
                RoundInfo info = rounds[i];
                if (StatLookup.TryGetValue(info.Name, out levelDetails)) {
                    if (info.Qualified && levelDetails.Type == LevelType.Final) {
                        wonFinal = true;
                        streak++;
                        if (streak > maxStreak) {
                            maxStreak = streak;
                        }
                    }

                    if (info.Round == 1) {
                        if (!wonFinal) {
                            streak = 0;
                        } else {
                            wonFinal = false;
                        }
                    }
                }
            }

            if (StatLookup.TryGetValue(name, out levelDetails)) {
                totalCount = levelDetails.Stats.Count;
                for (int i = 0; i < totalCount; i++) {
                    RoundInfo info = levelDetails.Stats[i];
                    TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                    if (info.Qualified) {
                        if (finishTime.TotalSeconds > 1.1 && (!best.HasValue || best.Value > finishTime)) {
                            best = finishTime;
                        }
                        qualifyCount++;
                    }

                    if (levelDetails.Type == LevelType.Team && info.Score.HasValue && (!bestScore.HasValue || info.Score.Value > bestScore.Value)) {
                        bestScore = info.Score;
                    }
                }
            }
            return new Tuple<int, int, TimeSpan?, int?, int, int>(totalCount, qualifyCount, best, bestScore, streak, maxStreak);
        }
        private void ClearTotals() {
            Rounds = 0;
            Duration = TimeSpan.Zero;
            Wins = 0;
            Shows = 0;
            Finals = 0;
            Kudos = 0;
        }
        private void UpdateTotals() {
            try {
                lblTotalRounds.Text = $"Rounds: {Rounds}";
                lblTotalShows.Text = $"Shows: {Shows}";
                lblTotalTime.Text = $"Time Played: {(int)Duration.TotalHours}:{Duration:mm\\:ss}";
                lblTotalWins.Text = $"Wins: {Wins}";
                float finalChance = (float)Finals * 100 / (Shows == 0 ? 1 : Shows);
                lblFinalChance.Text = $"Final %: {finalChance:0.0}";
                float winChance = (float)Wins * 100 / (Shows == 0 ? 1 : Shows);
                lblWinChance.Text = $"Win %: {winChance:0.0}";
                lblKudos.Text = $"Kudos: {Kudos}";
                gridDetails.Refresh();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            try {
                if (gridDetails.Columns.Count == 0) { return; }
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
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                LevelStats info = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;

                if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                    switch (info.Type) {
                        case LevelType.Race: e.CellStyle.BackColor = Color.LightGoldenrodYellow; break;
                        case LevelType.Survival: e.CellStyle.BackColor = Color.LightBlue; break;
                        case LevelType.Team: e.CellStyle.BackColor = Color.LightGreen; break;
                        case LevelType.Final: e.CellStyle.BackColor = Color.Pink; break;
                    }
                } else if (gridDetails.Columns[e.ColumnIndex].Name == "Info" && e.Value == null) {
                    gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Click to view level stats";
                    e.Value = Properties.Resources.info;
                } else if (gridDetails.Columns[e.ColumnIndex].Name == "AveDuration") {
                    e.Value = info.AveDuration.ToString("m\\:ss");
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                if (gridDetails.Columns[e.ColumnIndex].Name == "Info") {
                    gridDetails.Cursor = Cursors.Hand;
                } else {
                    gridDetails.Cursor = Cursors.Default;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                if (gridDetails.Columns[e.ColumnIndex].Name == "Info") {
                    using (LevelDetails levelDetails = new LevelDetails()) {
                        LevelStats stats = gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                        levelDetails.LevelName = stats.Name;
                        List<RoundInfo> rounds = stats.Stats;
                        rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                            return one.Start.CompareTo(two.Start);
                        });
                        levelDetails.RoundDetails = rounds;
                        levelDetails.ShowDialog(this);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = gridDetails.GetSortOrder(columnName);

            StatDetails.Sort(delegate (LevelStats one, LevelStats two) {
                int typeCompare = ((int)one.Type).CompareTo((int)two.Type);
                int nameCompare = one.Name.CompareTo(two.Name);

                if (sortOrder == SortOrder.Descending) {
                    LevelStats temp = one;
                    one = two;
                    two = temp;
                }

                if (typeCompare == 0) {
                    switch (columnName) {
                        case "Gold": typeCompare = one.Gold.CompareTo(two.Gold); break;
                        case "Silver": typeCompare = one.Silver.CompareTo(two.Silver); break;
                        case "Bronze": typeCompare = one.Bronze.CompareTo(two.Bronze); break;
                        case "Played": typeCompare = one.Played.CompareTo(two.Played); break;
                        case "Qualified": typeCompare = one.Qualified.CompareTo(two.Qualified); break;
                        case "Kudos": typeCompare = one.Kudos.CompareTo(two.Kudos); break;
                        case "AveKudos": typeCompare = one.AveKudos.CompareTo(two.AveKudos); break;
                        case "AveDuration": typeCompare = one.AveDuration.CompareTo(two.AveDuration); break;
                        default: typeCompare = one.Name.CompareTo(two.Name); break;
                    }
                }

                if (typeCompare == 0) {
                    typeCompare = nameCompare;
                }

                return typeCompare;
            });

            gridDetails.DataSource = null;
            gridDetails.DataSource = StatDetails;
            gridDetails.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private void lblTotalShows_Click(object sender, EventArgs e) {
            try {
                using (LevelDetails levelDetails = new LevelDetails()) {
                    levelDetails.LevelName = "Shows";
                    List<RoundInfo> rounds = new List<RoundInfo>();
                    for (int i = 0; i < StatDetails.Count; i++) {
                        rounds.AddRange(StatDetails[i].Stats);
                    }
                    rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                        return one.Start.CompareTo(two.Start);
                    });

                    List<RoundInfo> shows = new List<RoundInfo>();
                    int roundCount = 0;
                    int kudosTotal = 0;
                    bool won = false;
                    bool isFinal = false;
                    DateTime endDate = DateTime.MinValue;
                    for (int i = rounds.Count - 1; i >= 0; i--) {
                        RoundInfo info = rounds[i];
                        if (roundCount == 0) {
                            endDate = info.End;
                            won = info.Qualified;
                            LevelStats levelStats = StatLookup[info.Name];
                            isFinal = levelStats.Type == LevelType.Final;
                        }
                        roundCount++;
                        kudosTotal += info.Kudos;
                        if (info.Round == 1) {
                            shows.Insert(0, new RoundInfo() { Name = isFinal ? "Final" : string.Empty, End = endDate, Start = info.Start, Kudos = kudosTotal, Qualified = won, Round = roundCount, ShowID = info.ShowID, Tier = won ? 1 : 0 });
                            roundCount = 0;
                            kudosTotal = 0;
                        }
                    }
                    levelDetails.RoundDetails = shows;
                    levelDetails.ShowDialog(this);
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalRounds_Click(object sender, EventArgs e) {
            try {
                using (LevelDetails levelDetails = new LevelDetails()) {
                    levelDetails.LevelName = "Rounds";
                    List<RoundInfo> rounds = new List<RoundInfo>();
                    for (int i = 0; i < StatDetails.Count; i++) {
                        rounds.AddRange(StatDetails[i].Stats);
                    }
                    rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                        return one.Start.CompareTo(two.Start);
                    });
                    levelDetails.RoundDetails = rounds;
                    levelDetails.ShowDialog(this);
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalWins_Click(object sender, EventArgs e) {
            try {
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }
                rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                    return one.Start.CompareTo(two.Start);
                });

                using (StatsDisplay display = new StatsDisplay() { Text = "Wins Per Day" }) {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Date", typeof(DateTime));
                    dt.Columns.Add("Wins", typeof(int));

                    if (rounds.Count > 0) {
                        DateTime start = rounds[0].Start;
                        int currentWins = 0;
                        for (int i = 0; i < rounds.Count; i++) {
                            RoundInfo info = rounds[i];
                            LevelStats levelStats = null;
                            if (info.Qualified && StatLookup.TryGetValue(info.Name, out levelStats) && levelStats.Type == LevelType.Final) {
                                currentWins++;
                            }

                            if (info.Start.Date != start.Date) {
                                dt.Rows.Add(start.Date, currentWins);
                                currentWins = 0;
                                start = info.Start;
                            }
                        }

                        dt.Rows.Add(start.Date, currentWins);
                    } else {
                        dt.Rows.Add(DateTime.Now.Date, 0);
                    }

                    display.Details = dt;
                    display.ShowDialog(this);
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void rdAll_CheckedChanged(object sender, EventArgs e) {
            try {
                RadioButton button = sender as RadioButton;
                if (!button.Checked) { return; }
                if (button == rdAllParty || button == rdSolo || button == rdParty) {
                    button = rdAll.Checked ? rdAll : rdSeason.Checked ? rdSeason : rdWeek.Checked ? rdWeek : rdDay.Checked ? rdDay : rdSession;
                }

                for (int i = 0; i < StatDetails.Count; i++) {
                    LevelStats calculator = StatDetails[i];
                    calculator.Clear();
                }

                ClearTotals();

                bool soloOnly = rdSolo.Checked;
                List<RoundInfo> rounds = new List<RoundInfo>();
                if (button == rdAll) {
                    if (!rdAllParty.Checked) {
                        rounds.AddRange(RoundDetails.Find(x => x.InParty == !soloOnly));
                    } else {
                        rounds.AddRange(RoundDetails.FindAll());
                    }
                } else if (button == rdSeason) {
                    if (!rdAllParty.Checked) {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > SeasonStart && x.InParty == !soloOnly));
                    } else {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > SeasonStart));
                    }
                } else if (button == rdWeek) {
                    if (!rdAllParty.Checked) {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > WeekStart && x.InParty == !soloOnly));
                    } else {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > WeekStart));
                    }
                } else if (button == rdDay) {
                    if (!rdAllParty.Checked) {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > DayStart && x.InParty == !soloOnly));
                    } else {
                        rounds.AddRange(RoundDetails.Find(x => x.Start > DayStart));
                    }
                } else if (!rdAllParty.Checked) {
                    rounds.AddRange(RoundDetails.Find(x => x.Start > SessionStart && x.InParty == !soloOnly));
                } else {
                    rounds.AddRange(RoundDetails.Find(x => x.Start > SessionStart));
                }

                rounds.Sort(delegate (RoundInfo one, RoundInfo two) {
                    return one.Start.CompareTo(two.Start);
                });

                if (rounds.Count > 0 && (button == rdWeek || button == rdDay || button == rdSession)) {
                    int minShowID = rounds[0].ShowID;
                    if (button == rdWeek) {
                        rounds.AddRange(RoundDetails.Find(x => x.ShowID == minShowID && x.Start < WeekStart));
                    } else if (button == rdDay) {
                        rounds.AddRange(RoundDetails.Find(x => x.ShowID == minShowID && x.Start < DayStart));
                    } else {
                        rounds.AddRange(RoundDetails.Find(x => x.ShowID == minShowID && x.Start < SessionStart));
                    }
                }

                loadingExisting = true;
                LogFile_OnParsedLogLines(rounds);
                loadingExisting = false;
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e) {
            try {
                string assemblyInfo = null;
                using (ZipWebClient web = new ZipWebClient()) {
                    assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                    int index = assemblyInfo.IndexOf("AssemblyVersion(");
                    if (index > 0) {
                        int indexEnd = assemblyInfo.IndexOf("\")", index);
                        Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                        if (newVersion > Assembly.GetEntryAssembly().GetName().Version) {
                            if (MessageBox.Show(this, $"There is a new version of Fall Guy Stats available (v{newVersion.ToString(2)}). Do you wish to update now?", "Update Program", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                                byte[] data = web.DownloadData($"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStats.zip");
                                string exeName = null;
                                using (MemoryStream ms = new MemoryStream(data)) {
                                    using (ZipArchive zipFile = new ZipArchive(ms, ZipArchiveMode.Read)) {
                                        foreach (var entry in zipFile.Entries) {
                                            if (entry.Name.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) > 0) {
                                                exeName = entry.Name;
                                            }
                                            File.Move(entry.Name, $"{entry.Name}.old");
                                            entry.ExtractToFile(entry.Name, true);
                                        }
                                    }
                                }

                                Process.Start(new ProcessStartInfo(exeName));
                                this.Close();
                            }
                        } else {
                            MessageBox.Show(this, "You are at the latest version.", "Updater", MessageBoxButtons.OK, MessageBoxIcon.None);
                        }
                    } else {
                        MessageBox.Show(this, "Could not determine version.", "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOverlay_Click(object sender, EventArgs e) {
            if (overlay.Visible) {
                overlay.Hide();
            } else {
                overlay.Show(this);
                if (!setOverlayPosition) {
                    setOverlayPosition = true;
                    overlay.Location = this.Location;
                }
            }
        }
    }
}