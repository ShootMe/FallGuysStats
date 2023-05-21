using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;

namespace FallGuysStats {
    public partial class FilterCustomRange : MetroFramework.Forms.MetroForm {
        public DateTime startTime = DateTime.MinValue;
        public DateTime endTime = DateTime.MaxValue;
        private List<DateTime[]> templates = new List<DateTime[]>();
        public FilterCustomRange() {
            InitializeComponent();

            //this.Font = Overlay.GetMainFont(12);
            this.Text = Multilingual.GetWord("main_custom_range");
            this.lblCustomRange.Text = Multilingual.GetWord("custom_range_range");
            this.lblTemplates.Text = Multilingual.GetWord("custom_range_templates");
            this.chkStartNotSet.Text = Multilingual.GetWord("custom_range_not_set");
            this.chkEndNotSet.Text = Multilingual.GetWord("custom_range_not_set");
            this.btnFilter.Text = Multilingual.GetWord("main_filters");

            this.templatesListBox.Items.Clear();
            for (int i = 0; i < Stats.Seasons.Count; i++) {
                if (Stats.Seasons.Count - 1 == i) {
                    this.templatesListBox.Items.Add(Multilingual.GetWord("custom_range_season") + " " + (i < 6 ? (i + 1) + " [" + Multilingual.GetWord("custom_range_legacy") + "]" : (i - 5) + " [" + Multilingual.GetWord("custom_range_ffa") + "]") + " (" + Stats.Seasons[i].ToString("d") + "-)");
                    templates.Add(new DateTime[2] { Stats.Seasons[i], DateTime.MaxValue });
                } else {
                    this.templatesListBox.Items.Add(Multilingual.GetWord("custom_range_season") + " " + (i < 6 ? (i + 1) + " [" + Multilingual.GetWord("custom_range_legacy") + "]" : (i - 5) + " [" + Multilingual.GetWord("custom_range_ffa") + "]") + " (" + Stats.Seasons[i].ToString("d") + "-" + Stats.Seasons[i + 1].ToString("d") + ")");
                    templates.Add(new DateTime[2] { Stats.Seasons[i], Stats.Seasons[i + 1] });
                }
            }
        }

        private void templatesListBox_SelectedValueChanged(object sender, EventArgs e) {
            if (templates[this.templatesListBox.SelectedIndex][0] == DateTime.MinValue) {
                this.dtStart.Visible = false;
                this.chkStartNotSet.Checked = true;
            } else {
                this.dtStart.Visible = true;
                this.chkStartNotSet.Checked = false;
                this.dtStart.Value = templates[this.templatesListBox.SelectedIndex][0];
            }
            if (templates[this.templatesListBox.SelectedIndex][1] == DateTime.MaxValue) {
                this.dtEnd.Visible = false;
                this.chkEndNotSet.Checked = true;
            } else {
                this.dtEnd.Visible = true;
                this.chkEndNotSet.Checked = false;
                this.dtEnd.Value = templates[this.templatesListBox.SelectedIndex][1];
            }
        }

        private void chkStartNotSet_CheckedChanged(object sender, EventArgs e) {
            this.dtStart.Visible = !this.chkStartNotSet.Checked;
            if (this.chkStartNotSet.Checked && this.chkEndNotSet.Checked) {
                this.chkEndNotSet.Checked = false;
            }
        }

        private void chkEndNotSet_CheckedChanged(object sender, EventArgs e) {
            this.dtEnd.Visible = !this.chkEndNotSet.Checked;
            if (this.chkEndNotSet.Checked && this.chkStartNotSet.Checked) {
                this.chkStartNotSet.Checked = false;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e) {
            this.startTime = this.chkStartNotSet.Checked ? DateTime.MinValue : this.dtStart.Value;
            this.endTime = this.chkEndNotSet.Checked ? DateTime.MaxValue : this.dtEnd.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
