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
        }
    }
}