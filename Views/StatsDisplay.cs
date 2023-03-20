using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
namespace FallGuysStats {
    public partial class StatsDisplay : MetroFramework.Forms.MetroForm {
        public DataTable Details { get; set; }
        public Stats StatsForm { get; set; }
        public StatsDisplay() {
            this.InitializeComponent();
        }

        private void StatsDisplay_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            this.graph.DataSource = this.Details;
            this.graph.YColumns[1] = true;
            for (int i = 2; i < this.graph.YColumns.Length; i++) {
                this.graph.YColumns[i] = false;
            }
            this.chkWins.Checked = true;
        }
        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.chkWins.Theme = theme;
            this.chkFinals.Theme = theme;
            this.chkShows.Theme = theme;
            if (theme == MetroThemeStyle.Light) {
                this.chkWins.ForeColor =  Color.Red;
                this.chkFinals.ForeColor =  Color.Green;
                this.chkShows.ForeColor =  Color.Blue;
            
                this.graph.GraphXColumnColor = Color.Black;
                this.graph.GraphYColumnColor = Color.Black;
            
                this.graph.GraphXBackLineColor = Color.FromArgb(30, 0, 0, 0);
                this.graph.GraphYBackLineColor = Color.FromArgb(30, 0, 0, 0);

                this.graph.GraphSummaryBackColor = Color.FromArgb(223, 255, 255, 255);
                this.graph.GraphSummaryTitleColor = Color.Black;
                this.graph.GraphWinsColor = this.chkWins.ForeColor;
                this.graph.GraphFinalsColor = this.chkFinals.ForeColor;
                this.graph.GraphShowsColor = this.chkShows.ForeColor;
                this.graph.GraphGuideLineColor = Color.Magenta;
                this.graph.SetDataColors(new[] { Color.Black, this.graph.GraphWinsColor, this.graph.GraphFinalsColor, this.graph.GraphShowsColor });
            } else if (theme == MetroThemeStyle.Dark) {
                this.chkWins.ForeColor = Color.Gold;
                //this.chkFinals.ForeColor =  Color.FromArgb(102, 0, 153);
                this.chkFinals.ForeColor = Color.DeepPink;
                this.chkShows.ForeColor = Color.DeepSkyBlue;
            
                this.graph.GraphXColumnColor = Color.DarkGray;
                this.graph.GraphYColumnColor = Color.DarkGray;
            
                this.graph.GraphXBackLineColor = Color.FromArgb(30, 169, 169, 169);
                this.graph.GraphYBackLineColor = Color.FromArgb(30, 169, 169, 169);

                this.graph.GraphSummaryBackColor = Color.WhiteSmoke;
                this.graph.GraphSummaryTitleColor = Color.Black;
                this.graph.GraphWinsColor = this.chkWins.ForeColor;
                this.graph.GraphFinalsColor = this.chkFinals.ForeColor;
                this.graph.GraphShowsColor = this.chkShows.ForeColor;
                this.graph.GraphGuideLineColor = Color.Magenta;
                this.graph.SetDataColors(new[] { Color.Black, this.graph.GraphWinsColor, this.graph.GraphFinalsColor, this.graph.GraphShowsColor });
            }
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
            this.graph.Font = Overlay.GetMainFont(14);
            this.chkWins.Text = Multilingual.GetWord("level_detail_wins");
            this.chkFinals.Text = Multilingual.GetWord("level_detail_finals");
            this.chkShows.Text = Multilingual.GetWord("level_detail_shows");
            
            //if (Stats.CurrentLanguage == 0) { // English
            //    this.chkWins.Location =   new Point(75, 10);
            //    this.chkFinals.Location = new Point(180, 10);
            //    this.chkShows.Location =  new Point(295, 10);
            //} else if (Stats.CurrentLanguage == 1) { // French
            //    this.chkWins.Location =   new Point(75, 10);
            //    this.chkFinals.Location = new Point(200, 10);
            //    this.chkShows.Location =  new Point(310, 10);
            //} else if (Stats.CurrentLanguage == 2) { // Korean
            //    this.chkWins.Location =   new Point(75, 10);
            //    this.chkFinals.Location = new Point(170, 10);
            //    this.chkShows.Location =  new Point(280, 10);
            //} else if (Stats.CurrentLanguage == 3) { // Japanese
            //    this.chkWins.Location =   new Point(75, 10);
            //    this.chkFinals.Location = new Point(170, 10);
            //    this.chkShows.Location =  new Point(280, 10);
            //} else if (Stats.CurrentLanguage == 4) { // Simplified Chinese
            //    this.chkWins.Location =   new Point(75, 10);
            //    this.chkFinals.Location = new Point(170, 10);
            //    this.chkShows.Location =  new Point(265, 10);
            //}
        }
    }
}