using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class EditShows : MetroFramework.Forms.MetroForm {
        //protected override CreateParams CreateParams {
        //    get {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}
        public Stats StatsForm { get; set; }
        public List<Profiles> Profiles { get; set; }
        public int SelectedProfileId = 0;
        public string FunctionFlag = string.Empty;
        public int SelectedCount = 0;
        public EditShows() => this.InitializeComponent();

        private void EditShows_Load(object sender, EventArgs e) {
            this.SuspendLayout();
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            this.Profiles = this.Profiles.OrderBy(p => p.ProfileOrder).ToList();
            this.cboEditShows.Items.Clear();

            if (this.Profiles.Count == 1) {
                this.cboEditShows.Items.Insert(0, this.Profiles[0].ProfileName);
                this.cboEditShows.SelectedIndex = 0;
                return;
            }

            for (int i = this.Profiles.Count - 1; i >= 0; i--) {
                if (this.FunctionFlag == "move" && this.Profiles[i].ProfileId == StatsForm.CurrentSettings.SelectedProfile) continue;
                this.cboEditShows.Items.Insert(0, this.Profiles[i].ProfileName);
            }
            this.cboEditShows.SelectedIndex = 0;
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            foreach (Control c1 in Controls) {
                if (c1 is MetroLabel ml1) {
                    ml1.Theme = theme;
                } else if (c1 is MetroTextBox mtb1) {
                    mtb1.Theme = theme;
                } else if (c1 is MetroButton mb1) {
                    mb1.Theme = theme;
                } else if (c1 is MetroCheckBox mcb1) {
                    mcb1.Theme = theme;
                } else if (c1 is MetroRadioButton mrb1) {
                    mrb1.Theme = theme;
                } else if (c1 is MetroComboBox mcbo1) {
                    mcbo1.Theme = theme;
                } else if (c1 is MetroTabControl mtc1) {
                    mtc1.Theme = theme;
                    foreach (Control c2 in mtc1.Controls) {
                        if (c2 is MetroTabPage mtp2) {
                            mtp2.Theme = theme;
                            foreach (Control c3 in mtp2.Controls) {
                                if (c3 is MetroLabel ml3) {
                                    ml3.Theme = theme;
                                } else if (c3 is MetroTextBox mtb3) {
                                    mtb3.Theme = theme;
                                } else if (c3 is MetroButton mb3) {
                                    mb3.Theme = theme;
                                } else if (c3 is MetroCheckBox mcb3) {
                                    mcb3.Theme = theme;
                                } else if (c3 is MetroRadioButton mrb3) {
                                    mrb3.Theme = theme;
                                } else if (c3 is MetroComboBox mcbo3) {
                                    mcbo3.Theme = theme;
                                }
                            }
                        }
                    }
                } else if (c1 is GroupBox gb1) {
                    if (this.Theme == MetroThemeStyle.Dark) {
                        gb1.ForeColor = Color.DarkGray;
                    } else if (this.Theme == MetroThemeStyle.Light) {
                        gb1.ForeColor = Color.Black;
                    }
                    foreach (Control c2 in gb1.Controls) {
                        if (c2 is MetroLabel ml2) {
                            ml2.Theme = theme;
                        } else if (c2 is MetroTextBox mtb2) {
                            mtb2.Theme = theme;
                        } else if (c2 is MetroButton mb2) {
                            mb2.Theme = theme;
                        } else if (c2 is MetroCheckBox mcb2) {
                            mcb2.Theme = theme;
                        } else if (c2 is MetroRadioButton mrb2) {
                            mrb2.Theme = theme;
                        } else if (c2 is MetroComboBox mcbo2) {
                            mcbo2.Theme = theme;
                        } else if (c2 is GroupBox gb2) {
                            if (this.Theme == MetroThemeStyle.Dark) {
                                gb2.ForeColor = Color.DarkGray;
                            } else if (this.Theme == MetroThemeStyle.Light) {
                                gb2.ForeColor = Color.Black;
                            }
                            foreach (Control c3 in gb2.Controls) {
                                if (c3 is MetroRadioButton mrb3) {
                                    mrb3.Theme = theme;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CboEditShows_Changed(object sender, EventArgs e) {
            this.SelectedProfileId = this.Profiles.Find(p => p.ProfileName == (string)this.cboEditShows.SelectedItem).ProfileId;
        }

        private void BtnEditShowsSave_Click(object sender, EventArgs e) {
            string questionStr = string.Empty;
            if (FunctionFlag == "add") {
                questionStr = $"{Multilingual.GetWord("message_save_profile_prefix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("message_save_profile_suffix")}";
            } else if (FunctionFlag == "move") {
                questionStr = $"{Multilingual.GetWord("profile_move_select_question_prefix")} ({this.SelectedCount}) {Multilingual.GetWord("profile_move_select_question_infix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("profile_move_select_question_suffix")}";
            }
            if (MessageBox.Show(this,
                    questionStr,
                    Multilingual.GetWord("message_save_profile_caption"), MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) == DialogResult.OK) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnEditShowsCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void EditShows_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ChangeLanguage() {
            this.Font = Overlay.GetMainFont(9);
            if (this.FunctionFlag == "add") {
                this.Text = Multilingual.GetWord("profile_add_select_title");
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_add_select_question_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_add_select_question_suffix")}";
            } else if (this.FunctionFlag == "move") {
                this.Text = Multilingual.GetWord("profile_move_select_title");
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_move_select_description_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_move_select_description_suffix")} : {SelectedCount}{Multilingual.GetWord("numeric_suffix")}";
            }
            this.lblEditShowslabel.Text = Multilingual.GetWord("profile_list");
            this.btnEditShowsSave.Text = Multilingual.GetWord("profile_apply_change_button");
            this.btnEditShowsCancel.Text = Multilingual.GetWord("profile_undo_change_button");
            if (Stats.CurrentLanguage == 0) { // English
                this.ClientSize = new Size(445, 244);
                this.cboEditShows.Location = new Point(185, 135);
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Size = new Size(445, 53);
                this.btnEditShowsSave.Location = new Point(240, 206);
                this.btnEditShowsCancel.Location = new Point(339, 206);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.ClientSize = new Size(465, 244);
                this.cboEditShows.Location = new Point(185, 135);
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Size = new Size(465, 53);
                this.btnEditShowsSave.Location = new Point(260, 206);
                this.btnEditShowsCancel.Location = new Point(359, 206);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.ClientSize = new Size(445, 244);
                this.cboEditShows.Location = new Point(185, 135);
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Size = new Size(445, 53);
                this.btnEditShowsSave.Location = new Point(240, 206);
                this.btnEditShowsCancel.Location = new Point(339, 206);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.ClientSize = new Size(535, 244);
                this.cboEditShows.Location = new Point(230, 135);
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Size = new Size(535, 53);
                this.btnEditShowsSave.Location = new Point(328, 206);
                this.btnEditShowsCancel.Location = new Point(427, 206);
            } else if (Stats.CurrentLanguage == 4) {  // Simplified Chinese
                this.ClientSize = new Size(445, 244);
                this.cboEditShows.Location = new Point(185, 135);
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Size = new Size(445, 53);
                this.btnEditShowsSave.Location = new Point(240, 206);
                this.btnEditShowsCancel.Location = new Point(339, 206);
            }
        }
    }
}