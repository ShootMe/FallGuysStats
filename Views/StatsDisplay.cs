using System;
using System.Data;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class StatsDisplay : Form {
        public DataTable Details { get; set; }
        public StatsDisplay() {
            InitializeComponent();
        }

        private void StatsDisplay_Load(object sender, EventArgs e) {
            graph.DataSource = Details;
            graph.YColumns[1] = true;
            for (int i = 2; i < graph.YColumns.Length; i++) {
                graph.YColumns[i] = false;
            }
            chkWins.Checked = true;
        }
        private void StatsDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            graph.YColumns[1] = chkWins.Checked;
            graph.Invalidate();
        }
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            graph.YColumns[2] = chkFinals.Checked;
            graph.Invalidate();
        }
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            graph.YColumns[3] = chkShows.Checked;
            graph.Invalidate();
        }
    }
}