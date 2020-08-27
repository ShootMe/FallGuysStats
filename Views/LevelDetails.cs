using System.Collections.Generic;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class LevelDetails : Form {
        public string LevelName { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        private int ShowStats = 0;
        public LevelDetails() {
            InitializeComponent();
        }

        private void LevelDetails_Load(object sender, System.EventArgs e) {
            if (LevelName == "Shows") {
                Text = $"Show Stats";
                ShowStats = 2;
                ClientSize = new System.Drawing.Size(Width - 240, Height);
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
            gridDetails.Columns["InParty"].Visible = false;
            gridDetails.Columns["Qualified"].Visible = false;
            gridDetails.Columns.Add(new DataGridViewImageColumn() { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "Medal" });
            gridDetails.Setup("Medal", pos++, 24, "", DataGridViewContentAlignment.MiddleCenter);
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
            if (e.RowIndex < 0) { return; }

            if (gridDetails.Columns[e.ColumnIndex].Name == "End") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Finish") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.Finish.HasValue) {
                    e.Value = (info.Finish.Value - info.Start).ToString("m\\:ss");
                }
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0: e.Value = Properties.Resources.medal_pink; break;
                        case 1: e.Value = Properties.Resources.medal_gold; break;
                        case 2: e.Value = Properties.Resources.medal_silver; break;
                        case 3: e.Value = Properties.Resources.medal_bronze; break;
                    }
                } else {
                    e.Value = Properties.Resources.medal_eliminated;
                }
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                string name = null;
                if (LevelStats.DisplayNameLookup.TryGetValue((string)e.Value, out name)) {
                    e.Value = name;
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
                        string nameOne = one.Name;
                        LevelStats.DisplayNameLookup.TryGetValue(one.Name, out nameOne);
                        string nameTwo = two.Name;
                        LevelStats.DisplayNameLookup.TryGetValue(two.Name, out nameTwo);
                        int nameCompare = nameOne.CompareTo(nameTwo);
                        return nameCompare != 0 ? nameCompare : roundCompare;
                    case "Players":
                        int playerCompare = one.Players.CompareTo(two.Players);
                        return playerCompare != 0 ? playerCompare : showCompare == 0 ? roundCompare : showCompare;
                    case "Start": return one.Start.CompareTo(two.Start);
                    case "End": return (one.End - one.Start).CompareTo(two.End - two.Start);
                    case "Finish": return one.Finish.HasValue && two.Finish.HasValue ? (one.Finish.Value - one.Start).CompareTo(two.Finish.Value - two.Start) : one.Finish.HasValue ? -1 : 1;
                    case "Qualified":
                        int qualifiedCompare = one.Qualified.CompareTo(two.Qualified);
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
            gridDetails.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
        }
    }
}