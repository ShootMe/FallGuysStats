using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class LevelDetails : Form {
        public string LevelName { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        public LevelDetails() {
            InitializeComponent();
        }

        private void LevelDetails_Load(object sender, System.EventArgs e) {
            gridDetails.DataSource = RoundDetails;
            Text = $"Level Stats - {LevelName}";
        }
        private void gridDetails_DataSourceChanged(object sender, System.EventArgs e) {
            if (gridDetails.Columns.Count == 0) { return; }
            int pos = 0;
            gridDetails.Columns["Name"].Visible = false;
            gridDetails.Setup("ShowID", pos++, 0, "Show", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Round", pos++, 50, "Round", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Start", pos++, 115, "Start", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("End", pos++, 60, "Duration", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("Qualified", pos++, 70, "Qualified", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("Position", pos++, 60, "Position", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Tier", pos++, 60, "Tier", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Kudos", pos++, 60, "Kudos", DataGridViewContentAlignment.MiddleRight);
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (gridDetails.Columns[e.ColumnIndex].Name == "End") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Tier") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = "";
                switch (info.Tier) {
                    case 0: e.CellStyle.BackColor = Color.FromArgb(255, 0, 128); break;
                    case 1: e.CellStyle.BackColor = Color.FromArgb(196, 196, 0); break;
                    case 2: e.CellStyle.BackColor = Color.Silver; break;
                    case 3: e.CellStyle.BackColor = Color.FromArgb(120, 95, 7); break;
                }

            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            SortOrder sortOrder = GetSortOrder(e.ColumnIndex);
            RoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
                if (sortOrder == SortOrder.Descending) {
                    RoundInfo temp = one;
                    one = two;
                    two = temp;
                }
                switch (gridDetails.Columns[e.ColumnIndex].Name) {
                    case "ShowID": return one.ShowID.CompareTo(two.ShowID);
                    case "Round": return one.Round.CompareTo(two.Round);
                    case "Start": return one.Start.CompareTo(two.Start);
                    case "End": return (one.End - one.Start).CompareTo(two.End - two.Start);
                    case "Qualified": return one.Qualified.CompareTo(two.Qualified);
                    case "Position": return one.Position.CompareTo(two.Position);
                    case "Tier":
                        int tierOne = one.Tier == 0 ? 4 : one.Tier;
                        int tierTwo = two.Tier == 0 ? 4 : two.Tier;
                        return tierOne.CompareTo(tierTwo);
                    default: return one.Kudos.CompareTo(two.Kudos);
                }
            });
            gridDetails.DataSource = null;
            gridDetails.DataSource = RoundDetails;
            gridDetails.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private SortOrder GetSortOrder(int columnIndex) {
            if (gridDetails.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None ||
                gridDetails.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending) {
                gridDetails.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                return SortOrder.Ascending;
            } else {
                gridDetails.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                return SortOrder.Descending;
            }
        }
    }
}