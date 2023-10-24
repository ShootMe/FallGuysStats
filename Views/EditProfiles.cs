using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
namespace FallGuysStats {
    public partial class EditProfiles : MetroFramework.Forms.MetroForm {
        public List<Profiles> Profiles { get; set; }
        public List<RoundInfo> AllStats { get; set; }
        public Stats StatsForm { get; set; }
        public bool IsUpdate, IsDelete;
        public List<int> DeleteList = new List<int>();
        private DataTable ProfilesData;
        private DataGridViewComboBoxColumn cboShowsList;
        private int selectedRowIndex;
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();

        public EditProfiles() => this.InitializeComponent();

        private void EditProfiles_Load(object sender, EventArgs e) {
            this.dgvProfiles.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            this.dgvProfiles.DefaultCellStyle = this.dataGridViewCellStyle2;
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            this.InitProfileList();
            this.ReloadProfileList();
            this.dgvProfiles.ClearSelection();
        }
        
        private void SetTheme(MetroThemeStyle theme) {
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(20, 19, 0, 0);
            this.BackImage = theme == MetroThemeStyle.Light ? Properties.Resources.profile_icon : Properties.Resources.profile_gray_icon;
            
            this.dataGridViewCellStyle1.BackColor = theme == MetroThemeStyle.Light ? Color.LightGray : Color.FromArgb(2, 2, 2);
            this.dataGridViewCellStyle1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.dataGridViewCellStyle1.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.Cyan : Color.DarkSlateBlue;
            //this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle2.BackColor = theme == MetroThemeStyle.Light ? Color.White : Color.FromArgb(49, 51, 56);
            this.dataGridViewCellStyle2.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            this.dataGridViewCellStyle2.SelectionBackColor = theme == MetroThemeStyle.Light ? Color.DeepSkyBlue : Color.PaleGreen;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            this.dgvProfiles.AlternatingRowsDefaultCellStyle.BackColor = theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            this.dgvProfiles.AlternatingRowsDefaultCellStyle.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            
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
                    gb1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    foreach (Control c2 in gb1.Controls) {
                        if (c2 is MetroButton ml2) {
                            ml2.Theme = theme;
                        }
                        //else if (c2 is DataGridView dgv2) {
                        //    dgv2.Theme = theme;
                        //}
                    }
                }
            }
            this.Theme = theme;
        }

        private void InitProfileList() {
            this.cboShowsList = new DataGridViewComboBoxColumn {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            DataTable showsData = new DataTable();
            showsData.Columns.Add("showName");
            showsData.Columns.Add("showId");
            showsData.Rows.Add("", "");
            foreach (string showId in this.StatsForm.PublicShowIdList) {
                showsData.Rows.Add(Multilingual.GetShowName(showId), showId);
            }

            showsData.DefaultView.Sort = "showName ASC";

            this.cboShowsList.DataSource = showsData;
            this.cboShowsList.DisplayMember = "showName";
            this.cboShowsList.ValueMember = "showId";
            
            this.dgvProfiles.Columns.Add("profile", "profile");
            this.dgvProfiles.Columns.Add(this.cboShowsList);
            this.dgvProfiles.Columns[0].ReadOnly = true;
            this.dgvProfiles.Columns[0].DataPropertyName = "profile";
            this.dgvProfiles.Columns[1].Name = "show";
            this.dgvProfiles.Columns[1].DataPropertyName = "show";
            
            this.ProfilesData = new DataTable();
            this.ProfilesData.Columns.Add("profile");
            this.ProfilesData.Columns.Add("show");
            
            this.dgvProfiles.DataSource = this.ProfilesData;
        }
        
        private void ReloadProfileList() {
            this.Profiles = this.Profiles.OrderBy(p => p.ProfileOrder).ToList();
            this.ProfilesData.Clear();
            foreach (Profiles profile in this.Profiles) {
                this.ProfilesData.Rows.Add($"{profile.ProfileName} [{this.AllStats.FindAll(r => r.Profile == profile.ProfileId).Count:N0} {Multilingual.GetWord("profile_rounds_suffix")}]", profile.LinkedShowId);
            }
            this.Profiles = this.Profiles.OrderByDescending(p => p.ProfileOrder).ToList();
            this.txtAddProfile.Text = "";
            this.txtRenameProfile.Text = "";
            this.cboProfileRename.Items.Clear();
            this.cboProfileMoveFrom.Items.Clear();
            this.cboProfileMoveTo.Items.Clear();
            this.cboProfileRemove.Items.Clear();
            for (int i = 0; i < this.Profiles.Count; i++) {
                if (this.Profiles[i].ProfileOrder == 0) { this.Profiles[i].ProfileOrder = this.Profiles.Count - i; }
                this.cboProfileRename.Items.Insert(0, this.Profiles[i].ProfileName);
                this.cboProfileMoveTo.Items.Insert(0, this.Profiles[i].ProfileName);
                if (this.AllStats.FindAll(r => r.Profile == this.Profiles[i].ProfileId).Count != 0) {
                    this.cboProfileMoveFrom.Items.Insert(0, this.Profiles[i].ProfileName);
                }
                if (this.Profiles[i].ProfileId != 0) {
                    this.cboProfileRemove.Items.Insert(0, this.Profiles[i].ProfileName);
                }
            }
            this.RefreshComponent();
        }

        private void RefreshComponent() {
            this.txtAddProfile.Invalidate();
            this.txtRenameProfile.Invalidate();
            this.cboProfileRename.Invalidate();
            this.cboProfileMoveFrom.Invalidate();
            this.cboProfileMoveTo.Invalidate();
            this.cboProfileRemove.Invalidate();
        }

        private void ProfileList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            DataGridView datagridview = sender as DataGridView;
            if (datagridview.CurrentCell.ColumnIndex == 1 & e.Control is ComboBox) {
                ComboBox cb = e.Control as ComboBox;
                this.selectedRowIndex = cb.SelectedIndex;
                cb.SelectionChangeCommitted -= this.SubCombo_SelectionChangeCommitted;
                cb.SelectionChangeCommitted += this.SubCombo_SelectionChangeCommitted;
            }
        }

        private void SubCombo_SelectionChangeCommitted(object sender, EventArgs e) {
            DataRowView dataRow = (DataRowView)((ComboBox)sender).SelectedItem;
            string linkedShowId = (string)dataRow.Row[1];

            if (this.Profiles.FindIndex(item => item.LinkedShowId == linkedShowId) != -1) {
                ((ComboBox)sender).SelectedIndex = this.selectedRowIndex;
            } else {
                int profileListIndex = this.dgvProfiles.CurrentCell.RowIndex;
                int profileIndex = this.Profiles.Count - profileListIndex - 1;
                this.Profiles[profileIndex].LinkedShowId = linkedShowId;
                this.IsUpdate = true;
                this.selectedRowIndex = ((ComboBox)sender).SelectedIndex;
            }
            this.dgvProfiles.ClearSelection();
            this.dgvProfiles[1, this.dgvProfiles.CurrentCell.RowIndex].Selected = true;
        }

        private void ProfileList_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0 && e.ColumnIndex == 1) {
                DataGridView datagridview = sender as DataGridView;
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void ProfileList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            DataGridView datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex].Name == "profile") {
                datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = (string)e.Value;
            } else if (datagridview.Columns[e.ColumnIndex].Name == "profile") {
                datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = (string)e.Value;
            }
        }

        private void DeleteAmpersand_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 0x26) e.KeyChar = Convert.ToChar(0);
        }

        private void AddPageButton_Click(object sender, EventArgs e) {
            if (this.txtAddProfile.Text.Length == 0) { return; }
            if (this.Profiles.Find(p => p.ProfileName.Equals(this.txtAddProfile.Text)) != null) {
                MetroMessageBox.Show(this, Multilingual.GetWord("message_same_profile_name_exists"), $"{Multilingual.GetWord("message_create_profile_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_create_profile_prefix")} ({this.txtAddProfile.Text}) {Multilingual.GetWord("message_create_profile_suffix")}",
                    Multilingual.GetWord("message_create_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                int maxId = this.Profiles.Max(p => p.ProfileId);
                int maxOrder = this.Profiles.Max(p => p.ProfileOrder);
                this.Profiles.Insert(0, new Profiles { ProfileId = maxId + 1, ProfileName = this.txtAddProfile.Text, ProfileOrder = maxOrder + 1, LinkedShowId = string.Empty });
                this.IsUpdate = true;
                this.ReloadProfileList();
                this.dgvProfiles[0, this.dgvProfiles.Rows.Count - 1].Selected = true;
            }
        }

        private void RemovePageButton_Click(object sender, EventArgs e) {
            if (this.cboProfileRemove.SelectedIndex < 0) { return; }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_delete_profile_prefix")} ({this.cboProfileRemove.SelectedItem}) {Multilingual.GetWord("message_delete_profile_infix")} {Multilingual.GetWord("message_delete_profile_suffix")}",
                    Multilingual.GetWord("message_delete_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                string prevProfileName = string.Empty;
                for (int i = 0; i < this.cboProfileRemove.Items.Count; i++) {
                    if (this.cboProfileRemove.Items[i].ToString() == this.cboProfileRemove.SelectedItem.ToString()) {
                        if (i > 0) {
                            prevProfileName = this.cboProfileRemove.Items[i - 1].ToString();
                        }
                    }
                }

                int prevProfileId = string.IsNullOrEmpty(prevProfileName) ? 0 : this.Profiles.Find(p => p.ProfileName == prevProfileName).ProfileId;
                int profileId = this.Profiles.Find(p => p.ProfileName == this.cboProfileRemove.SelectedItem.ToString()).ProfileId;
                this.Profiles.Remove(this.Profiles.Find(p => p.ProfileName == this.cboProfileRemove.SelectedItem.ToString()));
                this.AllStats.RemoveAll(r => r.Profile == profileId);
                this.DeleteList.Add(profileId);
                this.IsDelete = true;
                if (this.StatsForm.CurrentSettings.SelectedProfile == profileId) {
                    this.StatsForm.CurrentSettings.SelectedProfile = prevProfileId;
                }
                this.StatsForm.ReloadProfileMenuItems();
                this.ReloadProfileList();
            }
        }

        private void MovePageButton_Click(object sender, EventArgs e) {
            if (this.cboProfileMoveTo.SelectedIndex < 0) { return; }
            if (this.cboProfileMoveFrom.SelectedIndex < 0) { return; }
            if (this.cboProfileMoveFrom.SelectedItem.ToString() == this.cboProfileMoveTo.SelectedItem.ToString()) { return; }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_move_profile_prefix")} ({this.cboProfileMoveFrom.SelectedItem}) {Multilingual.GetWord("message_move_profile_infix")} ({this.cboProfileMoveTo.SelectedItem}) {Multilingual.GetWord("message_move_profile_suffix")}",
                    Multilingual.GetWord("message_move_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                int fromId = this.Profiles.Find(p => p.ProfileName == this.cboProfileMoveFrom.SelectedItem.ToString()).ProfileId;
                int toId = this.Profiles.Find(p => p.ProfileName == this.cboProfileMoveTo.SelectedItem.ToString()).ProfileId;
                List<RoundInfo> targetList = this.AllStats.FindAll(r => r.Profile == fromId);
                if (targetList.Count > 0) {
                    foreach (RoundInfo target in targetList) {
                        target.Profile = toId;
                    }
                    this.IsUpdate = true;
                    this.ReloadProfileList();
                }
            }
        }

        private void RenameButton_Click(object sender, EventArgs e) {
            if (this.cboProfileRename.SelectedIndex < 0) { return; }
            if (this.txtRenameProfile.Text.Length == 0) { return; }
            if (this.cboProfileRename.SelectedItem.ToString() == this.txtRenameProfile.Text) { return; }
            if (this.Profiles.Find(p => p.ProfileName.Equals(this.txtRenameProfile.Text)) != null) {
                MetroMessageBox.Show(this, Multilingual.GetWord("message_same_profile_name_exists"), $"{Multilingual.GetWord("message_create_profile_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_rename_profile_prefix")} ({this.cboProfileRename.SelectedItem}) {Multilingual.GetWord("message_rename_profile_infix")} ({this.txtRenameProfile.Text}) {Multilingual.GetWord("message_rename_profile_suffix")}",
                    Multilingual.GetWord("message_rename_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                Profiles profileToUpdate = this.Profiles.Find(p => p.ProfileName.Equals(this.cboProfileRename.SelectedItem.ToString()));
                if (profileToUpdate != null) {
                    profileToUpdate.ProfileName = this.txtRenameProfile.Text;
                    this.IsUpdate = true;
                    this.ReloadProfileList();
                }
            }
        }

        private void ProfileListUp_Click(object sender, EventArgs e) {
            if (this.dgvProfiles.SelectedRows.Count <= 0) { return; }
            if (this.dgvProfiles.CurrentCell.RowIndex <= 0) { return; }
            int profileListIndex = this.dgvProfiles.CurrentCell.RowIndex;
            int profileIndex = this.Profiles.Count - profileListIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex + 1].ProfileOrder) = (this.Profiles[profileIndex + 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.IsUpdate = true;
            this.ReloadProfileList();
            this.dgvProfiles[0, profileListIndex - 1].Selected = true;
            this.dgvProfiles.FirstDisplayedScrollingRowIndex = profileListIndex - 7 < 0 ? 0 : profileListIndex - 7;
        }
        
        private void ProfileListDown_Click(object sender, EventArgs e) {
            if (this.dgvProfiles.SelectedRows.Count <= 0) { return; }
            if (this.dgvProfiles.CurrentCell.RowIndex >= this.dgvProfiles.Rows.Count - 1) { return; }
            int profileListIndex = this.dgvProfiles.CurrentCell.RowIndex;
            int profileIndex = this.Profiles.Count - profileListIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex - 1].ProfileOrder) = (this.Profiles[profileIndex - 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.IsUpdate = true;
            this.ReloadProfileList();
            this.dgvProfiles[0, profileListIndex + 1].Selected = true;
            this.dgvProfiles.FirstDisplayedScrollingRowIndex = profileListIndex - 5 < 0 ? 0 : profileListIndex - 5;
        }

        private void RenameComboboxChanged(object sender, EventArgs e) {
            this.txtRenameProfile.Text = this.cboProfileRename.SelectedItem.ToString();
        }
        
        private void EditProfile_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        
        private void ProfileList_SelectionChanged(object sender, EventArgs e) {
            if (((DataGridView)sender).SelectedRows.Count <= 0) {
                this.btnProfileUp.Enabled = false;
                this.btnProfileDown.Enabled = false;
            } else {
                this.btnProfileUp.Enabled = true;
                this.btnProfileDown.Enabled = true;
            }
        }

        private void ChangeLanguage() {
            this.Font = Overlay.GetMainFont(12);
            this.Text = $"     {Multilingual.GetWord("profile_title")}";
            this.grpProfiles.Text = Multilingual.GetWord("profile_list");
            this.mtpAddTabPage.Text = Multilingual.GetWord("profile_add_tab");
            this.btnAddProfile.Text = Multilingual.GetWord("profile_add_tab_button");
            this.lblAddProfile1.Text = Multilingual.GetWord("profile_add_tab_label1");
            this.mtpMoveTabPage.Text = Multilingual.GetWord("profile_move_tab");
            this.btnMoveProfile.Text = Multilingual.GetWord("profile_move_tab_button");
            this.lblMoveProfile2.Text = Multilingual.GetWord("profile_move_tab_label2");
            this.lblMoveProfile1.Text = Multilingual.GetWord("profile_move_tab_label1");
            this.mtpRemoveTabPage.Text = Multilingual.GetWord("profile_remove_tab");
            //this.lblRemoveProfile2.Text = Multilingual.GetWord("profile_remove_tab_label2");
            this.btnRemoveProfile.Text = Multilingual.GetWord("profile_remove_tab_button");
            this.lblRemoveProfile1.Text = Multilingual.GetWord("profile_remove_tab_label1");
            this.mtpRenameTabPage.Text = Multilingual.GetWord("profile_rename_tab");
            this.lblRenameProfile1.Text = Multilingual.GetWord("profile_rename_tab_label1");
            this.lblRenameProfile2.Text = Multilingual.GetWord("profile_rename_tab_label2");
            this.btnRenameProfile.Text = Multilingual.GetWord("profile_rename_tab_button");
            //this.ApplyChangeButton.Text = Multilingual.GetWord("profile_apply_change_button");
            //this.UndoChangeButton.Text = Multilingual.GetWord("profile_undo_change_button");
            
            if (Stats.CurrentLanguage == 0) { // English
                this.txtAddProfile.Location = new Point(107, 7);
                this.cboProfileRename.Location = new Point(107, 7);
                this.txtRenameProfile.Location = new Point(107, 45);
                this.cboProfileMoveFrom.Location = new Point(107, 7);
                this.cboProfileMoveTo.Location = new Point(107, 45);
                this.cboProfileRemove.Location = new Point(107, 7);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.txtAddProfile.Location = new Point(99, 7);
                this.cboProfileRename.Location = new Point(99, 7);
                this.txtRenameProfile.Location = new Point(99, 45);
                this.cboProfileMoveFrom.Location = new Point(99, 7);
                this.cboProfileMoveTo.Location = new Point(99, 45);
                this.cboProfileRemove.Location = new Point(99, 7);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.txtAddProfile.Location = new Point(96, 7);
                this.cboProfileRename.Location = new Point(96, 7);
                this.txtRenameProfile.Location = new Point(96, 45);
                this.cboProfileMoveFrom.Location = new Point(96, 7);
                this.cboProfileMoveTo.Location = new Point(96, 45);
                this.cboProfileRemove.Location = new Point(96, 7);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.txtAddProfile.Location = new Point(130, 7);
                this.cboProfileRename.Location = new Point(130, 7);
                this.txtRenameProfile.Location = new Point(130, 45);
                this.cboProfileMoveFrom.Location = new Point(96, 7);
                this.cboProfileMoveTo.Location = new Point(96, 45);
                this.cboProfileRemove.Location = new Point(130, 7);
            } else if (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) { // Simplified Chinese & Traditional Chinese
                this.txtAddProfile.Location = new Point(120, 7);
                this.cboProfileRename.Location = new Point(120, 7);
                this.txtRenameProfile.Location = new Point(120, 45);
                this.cboProfileMoveFrom.Location = new Point(96, 7);
                this.cboProfileMoveTo.Location = new Point(96, 45);
                this.cboProfileRemove.Location = new Point(96, 7);
            }
        }
    }
}