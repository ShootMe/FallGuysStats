using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class EditShows : Form {
        public Stats StatsForm { get; set; }
        public List<Profiles> Profiles { get; set; }
        public int SelectedProfileId = 0;
        public string Title = string.Empty;
        public string FunctionFlag = string.Empty;
        public string SaveBtnName = string.Empty;
        public int SelectedCount = 0;
        public EditShows() {
            this.InitializeComponent();
        }

        private void EditShows_Load(object sender, EventArgs e) {
            this.Text = this.Title;
            ChangeLanguage();
            if (this.FunctionFlag == "add") {
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_add_select_question_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_add_select_question_suffix")}";
            } else if (this.FunctionFlag == "move") {
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_move_select_description_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_move_select_description_suffix")} : {SelectedCount}{Multilingual.GetWord("numeric_suffix")}";
            }
            this.btnEditShowsSave.Text = this.SaveBtnName;
            this.Profiles = this.Profiles.OrderBy(p => p.ProfileOrder).ToList();
            this.cboEditShows.Items.Clear();
            
            for (int i = this.Profiles.Count - 1; i >= 0; i--) {
                if (this.FunctionFlag == "move" && this.Profiles[i].ProfileID == StatsForm.CurrentSettings.SelectedProfile) continue;
                this.cboEditShows.Items.Insert(0, this.Profiles[i].ProfileName);
            }
            this.cboEditShows.SelectedIndex = 0;
        }
        
        private void cboEditShows_Changed(object sender, EventArgs e) {
            this.SelectedProfileId = this.Profiles.Find(p => p.ProfileName == (string)this.cboEditShows.SelectedItem).ProfileID;
        }

        private void btnEditShowsSave_Click(object sender, EventArgs e) {
            string questionStr = string.Empty;
            if (FunctionFlag == "add") {
                questionStr = $"{Multilingual.GetWord("message_save_profile_prefix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("message_save_profile_suffix")}";
            } else if (FunctionFlag == "move") {
                questionStr = $"{Multilingual.GetWord("profile_move_select_question_prefix")} ({this.SelectedCount.ToString()}) {Multilingual.GetWord("profile_move_select_question_infix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("profile_move_select_question_suffix")}";
            }
            if (MessageBox.Show(this,
                    questionStr,
                    Multilingual.GetWord("message_save_profile_caption"), MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) == DialogResult.OK)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void btnEditShowsCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangeLanguage() {
            this.Font = new Font(Overlay.DefaultFontCollection.Families[0], 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.lblEditShowslabel.Text = Multilingual.GetWord("profile_list");
            if (Stats.CurrentLanguage == 0) { // English
                this.ClientSize = new Size(405, 194);
                this.cboEditShows.Location = new Point(167, 85);
                this.cboEditShows.Size = new Size(175, 17);
                this.lblEditShowsBackColor.Size = new Size(405, 60);
                this.btnEditShowsSave.Location = new Point(203, 154);
                this.btnEditShowsCancel.Location = new Point(298, 154);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.ClientSize = new Size(405, 194);
                this.cboEditShows.Location = new Point(167, 85);
                this.cboEditShows.Size = new Size(175, 17);
                this.lblEditShowsBackColor.Size = new Size(405, 60);
                this.btnEditShowsSave.Location = new Point(203, 154);
                this.btnEditShowsCancel.Location = new Point(298, 154);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.ClientSize = new Size(370, 194);
                this.cboEditShows.Location = new Point(167, 85);
                this.cboEditShows.Size = new Size(170, 17);
                this.lblEditShowsBackColor.Size = new Size(370, 60);
                this.btnEditShowsSave.Location = new Point(168, 154);
                this.btnEditShowsCancel.Location = new Point(263, 154);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.ClientSize = new Size(430, 194);
                this.cboEditShows.Location = new Point(202, 85);
                this.cboEditShows.Size = new Size(170, 17);
                this.lblEditShowsBackColor.Size = new Size(430, 60);
                this.btnEditShowsSave.Location = new Point(228, 154);
                this.btnEditShowsCancel.Location = new Point(323, 154);
            }
        }
    }
}