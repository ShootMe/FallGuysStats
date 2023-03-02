using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class StatsDisplay : Form {
        public DataTable Details { get; set; }
        public StatsDisplay() {
            this.InitializeComponent();
        }

        private void StatsDisplay_Load(object sender, EventArgs e) {
            this.ChangeLanguage();
            this.graph.DataSource = this.Details;
            this.graph.YColumns[1] = true;
            for (int i = 2; i < this.graph.YColumns.Length; i++) {
                this.graph.YColumns[i] = false;
            }
            this.chkWins.Checked = true;
        }
        private void StatsDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
        private void chkWins_CheckedChanged(object sender, EventArgs e) {
            this.graph.YColumns[1] = chkWins.Checked;
            this.graph.Invalidate();
        }
        private void chkFinals_CheckedChanged(object sender, EventArgs e) {
            this.graph.YColumns[2] = chkFinals.Checked;
            this.graph.Invalidate();
        }
        private void chkShows_CheckedChanged(object sender, EventArgs e) {
            this.graph.YColumns[3] = chkShows.Checked;
            this.graph.Invalidate();
        }
        private void ChangeLanguage() {
            this.ClientSize = new Size(1280, 540);
            //this.Font = new Font(Stats.CurrentLanguage == 4 ? Overlay.DefaultFontCollection.Families[1] : Overlay.DefaultFontCollection.Families[0], 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.chkWins.Text = Multilingual.GetWord("level_detail_wins");
            this.chkFinals.Text = Multilingual.GetWord("level_detail_finals");
            this.chkShows.Text = Multilingual.GetWord("level_detail_shows");
            
            if (Stats.CurrentLanguage == 0) { // English
                this.chkWins.Location =   new Point(75, 10);
                this.chkFinals.Location = new Point(180, 10);
                this.chkShows.Location =  new Point(295, 10);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.chkWins.Location =   new Point(75, 10);
                this.chkFinals.Location = new Point(200, 10);
                this.chkShows.Location =  new Point(310, 10);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.chkWins.Location =   new Point(75, 10);
                this.chkFinals.Location = new Point(170, 10);
                this.chkShows.Location =  new Point(280, 10);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.chkWins.Location =   new Point(75, 10);
                this.chkFinals.Location = new Point(170, 10);
                this.chkShows.Location =  new Point(280, 10);
            } else if (Stats.CurrentLanguage == 4) { // Simplified Chinese
                this.chkWins.Location =   new Point(75, 10);
                this.chkFinals.Location = new Point(170, 10);
                this.chkShows.Location =  new Point(265, 10);
            }
        }
    }
}