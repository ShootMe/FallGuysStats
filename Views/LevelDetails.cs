using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class LevelDetails : Form {
        public string LevelName { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        public Stats StatsForm { get; set; }
        private int ShowStats = 0;
        public LevelDetails() {
            InitializeComponent();
        }

        private void LevelDetails_Load(object sender, System.EventArgs e) {
            if (LevelName == "Shows") {
                Text = $"Show Stats";
                ShowStats = 2;
                ClientSize = new System.Drawing.Size(Width - 200, Height);
            } else if (LevelName == "Rounds") {
                Text = $"Round Stats";
                ShowStats = 1;
                ClientSize = new System.Drawing.Size(Width + 85, Height);
            } else {
                Text = $"Level Stats - {LevelName}";
            }

            gridDetails.DataSource = RoundDetails;
        }
        private void gridDetails_DataSourceChanged(object sender, System.EventArgs e) {
            if (gridDetails.Columns.Count == 0) { return; }

            int pos = 0;
            gridDetails.Columns["Tier"].Visible = false;
            gridDetails.Columns["ID"].Visible = false;
            gridDetails.Columns["Crown"].Visible = false;
            gridDetails.Columns["InParty"].Visible = false;
            gridDetails.Columns.Add(new DataGridViewImageColumn() { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "Medal" });
            gridDetails.Setup("Medal", pos++, 24, "", DataGridViewContentAlignment.MiddleCenter);
            if (ShowStats == 2) {
                gridDetails.Setup("Qualified", pos++, 40, "Final", DataGridViewContentAlignment.MiddleCenter);
            } else {
                gridDetails.Columns["Qualified"].Visible = false;
            }
            gridDetails.Setup("ShowID", pos++, 0, "Show", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Round", pos++, 50, ShowStats == 2 ? "Rounds" : "Round", DataGridViewContentAlignment.MiddleRight);
            if (ShowStats == 1) {
                gridDetails.Setup("Name", pos++, 95, "Level", DataGridViewContentAlignment.MiddleLeft);
            } else {
                gridDetails.Columns["Name"].Visible = false;
            }
            if (ShowStats != 2) {
                gridDetails.Setup("Players", pos++, 60, "Players", DataGridViewContentAlignment.MiddleRight);
            } else {
                gridDetails.Columns["Players"].Visible = false;
            }
            gridDetails.Setup("Start", pos++, 115, "Start", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("End", pos++, 60, "Duration", DataGridViewContentAlignment.MiddleCenter);
            if (ShowStats != 2) {
                gridDetails.Setup("Finish", pos++, 60, "Finish", DataGridViewContentAlignment.MiddleCenter);
            } else {
                gridDetails.Columns["Finish"].Visible = false;
            }
            if (ShowStats != 2) {
                gridDetails.Setup("Position", pos++, 60, "Position", DataGridViewContentAlignment.MiddleRight);
                gridDetails.Setup("Score", pos++, 60, "Score", DataGridViewContentAlignment.MiddleRight);
            } else {
                gridDetails.Columns["Position"].Visible = false;
                gridDetails.Columns["Score"].Visible = false;
            }
            gridDetails.Setup("Kudos", pos++, 60, "Kudos", DataGridViewContentAlignment.MiddleRight);
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= gridDetails.Rows.Count) { return; }

            if (gridDetails.Columns[e.ColumnIndex].Name == "End") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Start") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = info.StartLocal.ToString("yyyy-MM-dd HH:mm");
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Finish") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.Finish.HasValue) {
                    e.Value = (info.Finish.Value - info.Start).ToString("m\\:ss\\.ff");
                }
            } else if (ShowStats == 2 && gridDetails.Columns[e.ColumnIndex].Name == "Qualified") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = !string.IsNullOrEmpty(info.Name);
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0:
                            gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Pink";
                            e.Value = Properties.Resources.medal_pink;
                            break;
                        case 1:
                            gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Gold";
                            e.Value = Properties.Resources.medal_gold;
                            break;
                        case 2:
                            gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Silver";
                            e.Value = Properties.Resources.medal_silver;
                            break;
                        case 3:
                            gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Bronze";
                            e.Value = Properties.Resources.medal_bronze;
                            break;
                    }
                } else {
                    gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Eliminated";
                    e.Value = Properties.Resources.medal_eliminated;
                }
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                if (LevelStats.ALL.TryGetValue((string)e.Value, out var level)) {
                    e.Value = level.Name;
                }
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = gridDetails.GetSortOrder(columnName);

            RoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
                int roundCompare = one.Round.CompareTo(two.Round);
                int showCompare = one.ShowID.CompareTo(two.ShowID);
                if (sortOrder == SortOrder.Descending) {
                    RoundInfo temp = one;
                    one = two;
                    two = temp;
                }
                switch (columnName) {
                    case "ShowID":
                        showCompare = one.ShowID.CompareTo(two.ShowID);
                        return showCompare == 0 ? roundCompare : showCompare;
                    case "Round":
                        roundCompare = one.Round.CompareTo(two.Round);
                        return roundCompare == 0 ? showCompare : roundCompare;
                    case "Name":
                        string nameOne = LevelStats.ALL.TryGetValue(one.Name, out var level1) ? level1.Name : one.Name;
                        string nameTwo = LevelStats.ALL.TryGetValue(two.Name, out var level2) ? level2.Name : two.Name;
                        int nameCompare = nameOne.CompareTo(nameTwo);
                        return nameCompare != 0 ? nameCompare : roundCompare;
                    case "Players":
                        int playerCompare = one.Players.CompareTo(two.Players);
                        return playerCompare != 0 ? playerCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Start": return one.Start.CompareTo(two.Start);
                    case "End": return (one.End - one.Start).CompareTo(two.End - two.Start);
                    case "Finish": return one.Finish.HasValue && two.Finish.HasValue ? (one.Finish.Value - one.Start).CompareTo(two.Finish.Value - two.Start) : one.Finish.HasValue ? -1 : 1;
                    case "Qualified":
                        int qualifiedCompare = ShowStats == 2 ? string.IsNullOrEmpty(one.Name).CompareTo(string.IsNullOrEmpty(two.Name)) : one.Qualified.CompareTo(two.Qualified);
                        return qualifiedCompare != 0 ? qualifiedCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Position":
                        int positionCompare = one.Position.CompareTo(two.Position);
                        return positionCompare != 0 ? positionCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Score":
                        int scoreCompare = one.Score.GetValueOrDefault(-1).CompareTo(two.Score.GetValueOrDefault(-1));
                        return scoreCompare != 0 ? scoreCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Medal":
                        int tierOne = one.Qualified ? one.Tier == 0 ? 4 : one.Tier : 5;
                        int tierTwo = two.Qualified ? two.Tier == 0 ? 4 : two.Tier : 5;
                        int tierCompare = tierOne.CompareTo(tierTwo);
                        return tierCompare != 0 ? tierCompare : showCompare == 0 ? roundCompare : showCompare;
                    default:
                        int kudosCompare = one.Kudos.CompareTo(two.Kudos);
                        return kudosCompare != 0 ? kudosCompare : showCompare == 0 ? roundCompare : showCompare;
                }
            });

            gridDetails.DataSource = null;
            gridDetails.DataSource = RoundDetails;
            gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (ShowStats != 2 && gridDetails.SelectedCells.Count > 0) {
                gridDetails.ClearSelection();
            }
        }
        private void LevelDetails_KeyDown(object sender, KeyEventArgs e) {
            try {
                int selectedCount = gridDetails.SelectedCells.Count;
                if (e.KeyCode == Keys.Delete && selectedCount > 0) {
                    HashSet<RoundInfo> rows = new HashSet<RoundInfo>();
                    int minIndex = gridDetails.FirstDisplayedScrollingRowIndex;
                    for (int i = 0; i < selectedCount; i++) {
                        DataGridViewCell cell = gridDetails.SelectedCells[i];
                        rows.Add((RoundInfo)gridDetails.Rows[cell.RowIndex].DataBoundItem);
                    }

                    if (MessageBox.Show(this, $"Are you sure you want to remove the selected ({rows.Count}) Shows?", "Remove Shows", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                        gridDetails.DataSource = null;

                        lock (StatsForm.StatsDB) {
                            StatsForm.StatsDB.BeginTrans();
                            foreach (RoundInfo info in rows) {
                                RoundDetails.Remove(info);
                                StatsForm.RoundDetails.DeleteMany(x => x.ShowID == info.ShowID);
                            }
                            StatsForm.StatsDB.Commit();
                        }

                        gridDetails.DataSource = RoundDetails;
                        if (minIndex < RoundDetails.Count) {
                            gridDetails.FirstDisplayedScrollingRowIndex = minIndex;
                        } else if (RoundDetails.Count > 0) {
                            gridDetails.FirstDisplayedScrollingRowIndex = RoundDetails.Count - 1;
                        }

                        StatsForm.ResetStats();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}