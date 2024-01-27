using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
namespace FallGuysStats {
    public partial class EditShows : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        public List<Profiles> Profiles { get; set; }
        public int SelectedProfileId = 0;
        public string FunctionFlag = string.Empty;
        public int SelectedCount = 0;
        public bool UseLinkedProfiles;
        public EditShows() {
            this.InitializeComponent();
            this.Opacity = 0;
        }

        private void EditShows_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
            this.ChangeLanguage();
            this.Profiles = this.Profiles.OrderBy(p => p.ProfileOrder).ToList();
            this.cboEditShows.Items.Clear();
            
            if (this.Profiles.Count == 1) {
                this.cboEditShows.Items.Insert(0, this.Profiles[0].ProfileName);
                this.cboEditShows.SelectedIndex = 0;
                this.chkUseLinkedProfiles.Visible = false;
            } else {
                for (int i = this.Profiles.Count - 1; i >= 0; i--) {
                    if (this.FunctionFlag == "move" && this.Profiles[i].ProfileId == StatsForm.GetCurrentProfileId()) continue;
                    this.cboEditShows.Items.Insert(0, this.Profiles[i].ProfileName);
                }
                this.cboEditShows.SelectedIndex = 0;
            
                if (this.FunctionFlag == "move") {
                    this.chkUseLinkedProfiles.Visible = false;
                } else if (this.FunctionFlag == "add" && this.StatsForm.CurrentSettings.AutoChangeProfile) {
                    this.chkUseLinkedProfiles.Visible = false;
                    this.chkUseLinkedProfiles.Checked = true;
                }
            }
        }

        private void EditShows_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
            if (this.FunctionFlag == "add") {
                if (Stats.OnlineServiceType != OnlineServiceTypes.None && !string.IsNullOrEmpty(Stats.OnlineServiceId) && !string.IsNullOrEmpty(Stats.OnlineServiceNickname)) {
                    this.mlOnlineServiceInfo.Visible = true;
                    this.mlOnlineServiceInfo.Image = Stats.OnlineServiceType == OnlineServiceTypes.EpicGames ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
                    this.mlOnlineServiceInfo.Text = Stats.OnlineServiceNickname;
                    this.mlOnlineServiceInfo.Location = new Point(this.Width - this.mlOnlineServiceInfo.Width - 15, this.mlOnlineServiceInfo.Location.Y);
                }
            }
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            foreach (Control c1 in Controls) {
                if (c1 is MetroLabel mlb1) {
                    mlb1.Theme = theme;
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
                } else if (c1 is MetroLink ml1) {
                    ml1.Theme = theme;
                } else if (c1 is MetroTabControl mtc1) {
                    mtc1.Theme = theme;
                    foreach (Control c2 in mtc1.Controls) {
                        if (c2 is MetroTabPage mtp2) {
                            mtp2.Theme = theme;
                            foreach (Control c3 in mtp2.Controls) {
                                if (c3 is MetroLabel mlb3) {
                                    mlb3.Theme = theme;
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
                    gb1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    foreach (Control c2 in gb1.Controls) {
                        if (c2 is MetroLabel mlb2) {
                            mlb2.Theme = theme;
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
                            gb2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                            foreach (Control c3 in gb2.Controls) {
                                if (c3 is MetroRadioButton mrb3) {
                                    mrb3.Theme = theme;
                                }
                            }
                        }
                    }
                }
            }
            this.Theme = theme;
            this.ResumeLayout();
        }
        
        private void cboEditShows_Changed(object sender, EventArgs e) {
            this.SelectedProfileId = this.Profiles.Find(p => p.ProfileName == (string)this.cboEditShows.SelectedItem).ProfileId;
        }
        
        private void chkUseLinkedProfiles_CheckedChanged(object sender, EventArgs e) {
            this.UseLinkedProfiles = ((CheckBox)sender).Checked;
            this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_add_select_question_prefix")}{Environment.NewLine}{(this.UseLinkedProfiles ? Multilingual.GetWord("profile_add_select_question_suffix_linked_profiles") : Multilingual.GetWord("profile_add_select_question_suffix"))}";
            if (this.UseLinkedProfiles) {
                this.lblEditShowslabel.Visible = false;
                this.cboEditShows.Visible = false;
                this.cboEditShows.SelectedIndex = 0;
            } else {
                this.lblEditShowslabel.Visible = true;
                this.cboEditShows.SelectedIndex = 0;
                this.cboEditShows.Visible = true;
            }
        }

        private void btnEditShowsSave_Click(object sender, EventArgs e) {
            string questionStr = string.Empty;
            if (FunctionFlag == "add") {
                questionStr = this.UseLinkedProfiles ?
                                $"{Multilingual.GetWord("message_save_data_linked_profiles")}{Environment.NewLine}{Multilingual.GetWord("message_save_data_linked_profiles_info_prefix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("message_save_data_linked_profiles_info_suffix")}" :
                                $"{Multilingual.GetWord("message_save_profile_prefix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("message_save_profile_suffix")}";
            } else if (FunctionFlag == "move") {
                questionStr = $"{Multilingual.GetWord("profile_move_select_question_prefix")} ({this.SelectedCount:N0}) {Multilingual.GetWord("profile_move_select_question_infix")} ({this.cboEditShows.SelectedItem}) {Multilingual.GetWord("profile_move_select_question_suffix")}";
            }
            if (MetroMessageBox.Show(this,
                    questionStr,
                    this.UseLinkedProfiles ? $"{Multilingual.GetWord("message_save_data_caption")}" : Multilingual.GetWord("message_save_profile_caption"),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void btnEditShowsCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Escape) {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeLanguage() {
            this.Font = Overlay.GetMainFont(9);
            if (this.FunctionFlag == "add") {
                this.Text = Multilingual.GetWord("profile_add_select_title");
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_add_select_question_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_add_select_question_suffix")}";
                this.chkUseLinkedProfiles.Text = Multilingual.GetWord("profile_add_select_use_linked_profiles");
            } else if (this.FunctionFlag == "move") {
                this.Text = Multilingual.GetWord("profile_move_select_title");
                this.lblEditShowsQuestion.Text = $"{Multilingual.GetWord("profile_move_select_description_prefix")}{Environment.NewLine}{Multilingual.GetWord("profile_move_select_description_suffix")} : {this.SelectedCount:N0}{Multilingual.GetWord("numeric_suffix")}";
            }
            this.lblEditShowslabel.Text = Multilingual.GetWord("profile_list");
            if (Stats.CurrentLanguage == Language.English) {
                this.Width = 445;
                this.cboEditShows.Left = 185;
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Width = 445;
            } else if (Stats.CurrentLanguage == Language.French) {
                this.Width = 525;
                this.cboEditShows.Left = 185;
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Width = 525;
            } else if (Stats.CurrentLanguage == Language.Korean) {
                this.Width = 445;
                this.cboEditShows.Left = 185;
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Width = 445;
            } else if (Stats.CurrentLanguage == Language.Japanese) {
                this.Width = 540;
                this.cboEditShows.Left = 230;
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Width = 540;
            } else if (Stats.CurrentLanguage == Language.SimplifiedChinese || Stats.CurrentLanguage == Language.TraditionalChinese) {
                this.Width = 445;
                this.cboEditShows.Left = 185;
                //this.cboEditShows.Size = new Size(198, 29);
                this.lblEditShowsBackColor.Width = 445;
            }
            this.btnEditShowsCancel.Text = Multilingual.GetWord("profile_undo_change_button");
            this.btnEditShowsCancel.Width = TextRenderer.MeasureText(this.btnEditShowsCancel.Text, this.btnEditShowsCancel.Font).Width + 45;
            this.btnEditShowsCancel.Left = this.Width - this.btnEditShowsCancel.Width - 20;
            this.btnEditShowsSave.Text = Multilingual.GetWord("profile_apply_change_button");
            this.btnEditShowsSave.Width = TextRenderer.MeasureText(this.btnEditShowsSave.Text, this.btnEditShowsSave.Font).Width + 45;
            this.btnEditShowsSave.Left = this.btnEditShowsCancel.Left - this.btnEditShowsSave.Width - 15;
        }
    }
}