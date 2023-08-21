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
        private DataTable ProfilesData;
        private DataGridViewComboBoxColumn cboShowsList;
        private int selectedRowIndex;
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();

        public EditProfiles() => this.InitializeComponent();

        private void EditProfiles_Load(object sender, EventArgs e) {
            this.ProfileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ProfileList.DefaultCellStyle = dataGridViewCellStyle2;
            this.SuspendLayout();
            this.SetTheme(Stats.CurrentTheme);
            this.ResumeLayout(false);
            this.ChangeLanguage();
            this.InitProfileList();
            this.ReloadProfileList();
            this.ProfileList.ClearSelection();
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
            this.ProfileList.AlternatingRowsDefaultCellStyle.BackColor = theme == MetroThemeStyle.Light ? Color.FromArgb(225, 235, 255) : Color.FromArgb(40, 66, 66);
            this.ProfileList.AlternatingRowsDefaultCellStyle.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.WhiteSmoke;
            
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
            foreach (string showId in this.StatsForm.publicShowIdList) {
                showsData.Rows.Add(Multilingual.GetShowName(showId), showId);
            }

            showsData.DefaultView.Sort = "showName ASC";

            this.cboShowsList.DataSource = showsData;
            this.cboShowsList.DisplayMember = "showName";
            this.cboShowsList.ValueMember = "showId";
            
            this.ProfileList.Columns.Add("profile", "profile");
            this.ProfileList.Columns.Add(this.cboShowsList);
            this.ProfileList.Columns[0].ReadOnly = true;
            this.ProfileList.Columns[0].DataPropertyName = "profile";
            this.ProfileList.Columns[1].Name = "show";
            this.ProfileList.Columns[1].DataPropertyName = "show";
            
            this.ProfilesData = new DataTable();
            this.ProfilesData.Columns.Add("profile");
            this.ProfilesData.Columns.Add("show");
            
            this.ProfileList.DataSource = this.ProfilesData;
        }
        
        private void ReloadProfileList() {
            this.Profiles = this.Profiles.OrderBy(p => p.ProfileOrder).ToList();
            this.ProfilesData.Clear();
            foreach (Profiles profile in this.Profiles) {
                this.ProfilesData.Rows.Add($"{profile.ProfileName} [{this.AllStats.FindAll(r => r.Profile == profile.ProfileId).Count} {Multilingual.GetWord("profile_rounds_suffix")}]", profile.LinkedShowId);
            }
            this.Profiles = this.Profiles.OrderByDescending(p => p.ProfileOrder).ToList();
            this.AddPageTextbox.Text = "";
            this.RenamePageTextbox.Text = "";
            this.RenamePageCombobox.Items.Clear();
            this.MoveFromCombobox.Items.Clear();
            this.MoveToCombobox.Items.Clear();
            this.RemoveProfileCombobox.Items.Clear();
            for (int i = 0; i < this.Profiles.Count; i++) {
                if (this.Profiles[i].ProfileOrder == 0) { this.Profiles[i].ProfileOrder = this.Profiles.Count - i; }
                this.RenamePageCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                this.MoveToCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                if (this.AllStats.FindAll(r => r.Profile == this.Profiles[i].ProfileId).Count != 0) {
                    this.MoveFromCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
                if (this.Profiles[i].ProfileId != 0) {
                    this.RemoveProfileCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
            }
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
                int profileListIndex = this.ProfileList.CurrentCell.RowIndex;
                int profileIndex = this.Profiles.Count - profileListIndex - 1;
                this.Profiles[profileIndex].LinkedShowId = linkedShowId;
                this.selectedRowIndex = ((ComboBox)sender).SelectedIndex;
            }
            this.ProfileList.ClearSelection();
            this.ProfileList[1, this.ProfileList.CurrentCell.RowIndex].Selected = true;
        }

        private void ProfileList_CellClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView datagridview = sender as DataGridView;
            if (e.ColumnIndex == 1) {
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

        //private void ProfileList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e) {
        //    e.NewWidth = ((ListView)sender).Columns[e.ColumnIndex].Width;
        //    e.Cancel = true;
        //}

        private void DeleteAmpersand_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 0x26) e.KeyChar = Convert.ToChar(0);
        }

        private void AddPageButton_Click(object sender, EventArgs e) {
            if (this.AddPageTextbox.Text.Length == 0) { return; }

            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_create_profile_prefix")} ({this.AddPageTextbox.Text}) {Multilingual.GetWord("message_create_profile_suffix")}",
                    Multilingual.GetWord("message_create_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                int maxID = 0;
                int order = 1;
                for (int i = 0; i < this.Profiles.Count; i++) {
                    if (maxID <= this.Profiles[i].ProfileId) { maxID = this.Profiles[i].ProfileId; }
                    if (order <= this.Profiles[i].ProfileOrder) { order = this.Profiles[i].ProfileOrder; }
                    if (this.Profiles[i].ProfileName == this.AddPageTextbox.Text) { return; }
                }
                this.Profiles.Insert(0, new Profiles { ProfileId = maxID + 1, ProfileName = this.AddPageTextbox.Text, ProfileOrder = order + 1 });
                this.ReloadProfileList();
                this.ProfileList[0, this.ProfileList.Rows.Count - 1].Selected = true;
            }
        }

        private void RemovePageButton_Click(object sender, EventArgs e) {
            if (this.RemoveProfileCombobox.SelectedIndex < 0) { return; }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_delete_profile_prefix")} ({this.RemoveProfileCombobox.SelectedItem}) {Multilingual.GetWord("message_delete_profile_infix")} {Multilingual.GetWord("message_delete_profile_suffix")}",
                    Multilingual.GetWord("message_delete_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                string prevProfileName = string.Empty;
                for (int i = 0; i < this.RemoveProfileCombobox.Items.Count; i++) {
                    if (this.RemoveProfileCombobox.Items[i].ToString() == this.RemoveProfileCombobox.SelectedItem.ToString()) {
                        if (i > 0) {
                            prevProfileName = this.RemoveProfileCombobox.Items[i - 1].ToString();
                        }
                    }
                }

                int prevProfileId = string.IsNullOrEmpty(prevProfileName) ? 0 : this.Profiles.Find(p => p.ProfileName == prevProfileName).ProfileId;
                int profileId = this.Profiles.Find(p => p.ProfileName == this.RemoveProfileCombobox.SelectedItem.ToString()).ProfileId;
                this.Profiles.Remove(this.Profiles.Find(p => p.ProfileName == this.RemoveProfileCombobox.SelectedItem.ToString()));
                this.AllStats.RemoveAll(r => r.Profile == profileId);
                if (this.StatsForm.CurrentSettings.SelectedProfile == profileId) {
                    this.StatsForm.CurrentSettings.SelectedProfile = prevProfileId;
                }
                this.StatsForm.ReloadProfileMenuItems();
                this.ReloadProfileList();
            }
        }

        private void MovePageButton_Click(object sender, EventArgs e) {
            if (this.MoveToCombobox.SelectedIndex < 0) { return; }
            if (this.MoveFromCombobox.SelectedIndex < 0) { return; }
            if (this.MoveFromCombobox.SelectedItem.ToString() == this.MoveToCombobox.SelectedItem.ToString()) { return; }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_move_profile_prefix")} ({this.MoveFromCombobox.SelectedItem}) {Multilingual.GetWord("message_move_profile_infix")} ({this.MoveToCombobox.SelectedItem}) {Multilingual.GetWord("message_move_profile_suffix")}",
                    Multilingual.GetWord("message_move_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                int fromId = this.Profiles.Find(p => p.ProfileName == this.MoveFromCombobox.SelectedItem.ToString()).ProfileId;
                int toId = this.Profiles.Find(p => p.ProfileName == this.MoveToCombobox.SelectedItem.ToString()).ProfileId;
                for (int i = 0; i < this.AllStats.Count; i++) {
                    if (this.AllStats[i].Profile != fromId) { continue; }
                    this.AllStats[i].Profile = toId;
                }
                this.ReloadProfileList();
            }
        }

        private void RenameButton_Click(object sender, EventArgs e) {
            if (this.RenamePageCombobox.SelectedIndex < 0) { return; }
            if (this.RenamePageTextbox.Text.Length == 0) { return; }
            if (this.RenamePageCombobox.SelectedItem.ToString() == this.RenamePageTextbox.Text) { return; }
            if (MetroMessageBox.Show(this,
                    $"{Multilingual.GetWord("message_rename_profile_prefix")} ({this.RenamePageCombobox.SelectedItem}) {Multilingual.GetWord("message_rename_profile_infix")} ({this.RenamePageTextbox.Text}) {Multilingual.GetWord("message_rename_profile_suffix")}",
                    Multilingual.GetWord("message_rename_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                for (int i = 0; i < this.Profiles.Count; i++) {
                    if (this.Profiles[i].ProfileName != this.RenamePageCombobox.SelectedItem.ToString()) { continue; }
                    this.Profiles[i].ProfileName = this.RenamePageTextbox.Text;
                }
                this.ReloadProfileList();
            }
        }

        private void ProfileListUp_Click(object sender, EventArgs e) {
            if (this.ProfileList.SelectedRows.Count <= 0) { return; }
            if (this.ProfileList.CurrentCell.RowIndex <= 0) { return; }
            int profileListIndex = this.ProfileList.CurrentCell.RowIndex;
            int profileIndex = this.Profiles.Count - profileListIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex + 1].ProfileOrder) = (this.Profiles[profileIndex + 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.ReloadProfileList();
            this.ProfileList[0, profileListIndex - 1].Selected = true;
        }
        
        private void ProfileListDown_Click(object sender, EventArgs e) {
            if (this.ProfileList.SelectedRows.Count <= 0) { return; }
            if (this.ProfileList.CurrentCell.RowIndex >= this.ProfileList.Rows.Count - 1) { return; }
            int profileListIndex = this.ProfileList.CurrentCell.RowIndex;
            int profileIndex = this.Profiles.Count - profileListIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex - 1].ProfileOrder) = (this.Profiles[profileIndex - 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.ReloadProfileList();
            this.ProfileList[0, profileListIndex + 1].Selected = true;
        }

        private void RenameComboboxChanged(object sender, EventArgs e) {
            this.RenamePageTextbox.Text = this.RenamePageCombobox.SelectedItem.ToString();
        }
        
        private void EditProfile_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        
        private void ProfileList_SelectionChanged(object sender, EventArgs e) {
            if (((DataGridView)sender).SelectedRows.Count <= 0) {
                this.ProfileListUp.Enabled = false;
                this.ProfileListDown.Enabled = false;
            } else {
                this.ProfileListUp.Enabled = true;
                this.ProfileListDown.Enabled = true;
            }
        }

        private void ChangeLanguage() {
            this.Font = Overlay.GetMainFont(12);
            this.Text = $"     {Multilingual.GetWord("profile_title")}";
            this.groupBox1.Text = Multilingual.GetWord("profile_list");
            this.AddTabPage.Text = Multilingual.GetWord("profile_add_tab");
            this.AddPageButton.Text = Multilingual.GetWord("profile_add_tab_button");
            this.AddPageLabel1.Text = Multilingual.GetWord("profile_add_tab_label1");
            this.MoveTabPage.Text = Multilingual.GetWord("profile_move_tab");
            this.MovePageButton.Text = Multilingual.GetWord("profile_move_tab_button");
            this.MovePageLabel2.Text = Multilingual.GetWord("profile_move_tab_label2");
            this.MovePageLabel1.Text = Multilingual.GetWord("profile_move_tab_label1");
            this.RemoveTabPage.Text = Multilingual.GetWord("profile_remove_tab");
            //this.RemovePageLabel2.Text = Multilingual.GetWord("profile_remove_tab_label2");
            this.RemovePageButton.Text = Multilingual.GetWord("profile_remove_tab_button");
            this.RemovePageLabel1.Text = Multilingual.GetWord("profile_remove_tab_label1");
            this.RenameTabPage.Text = Multilingual.GetWord("profile_rename_tab");
            this.RenamePageLabel1.Text = Multilingual.GetWord("profile_rename_tab_label1");
            this.RenamePageLabel2.Text = Multilingual.GetWord("profile_rename_tab_label2");
            this.RenameButton.Text = Multilingual.GetWord("profile_rename_tab_button");
            //this.ApplyChangeButton.Text = Multilingual.GetWord("profile_apply_change_button");
            //this.UndoChangeButton.Text = Multilingual.GetWord("profile_undo_change_button");
            
            if (Stats.CurrentLanguage == 0) { // English
                this.AddPageTextbox.Location =        new Point(96, 10);
                this.RenamePageCombobox.Location =    new Point(96, 7);
                this.RenamePageTextbox.Location =     new Point(96, 45);
                this.MoveFromCombobox.Location =      new Point(96, 7);
                this.MoveToCombobox.Location =        new Point(96, 45);
                this.RemoveProfileCombobox.Location = new Point(96, 7);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.AddPageTextbox.Location =        new Point(96, 10);
                this.RenamePageCombobox.Location =    new Point(96, 7);
                this.RenamePageTextbox.Location =     new Point(96, 45);
                this.MoveFromCombobox.Location =      new Point(96, 7);
                this.MoveToCombobox.Location =        new Point(96, 45);
                this.RemoveProfileCombobox.Location = new Point(96, 7);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.AddPageTextbox.Location =        new Point(96, 10);
                this.RenamePageCombobox.Location =    new Point(96, 7);
                this.RenamePageTextbox.Location =     new Point(96, 45);
                this.MoveFromCombobox.Location =      new Point(96, 7);
                this.MoveToCombobox.Location =        new Point(96, 45);
                this.RemoveProfileCombobox.Location = new Point(96, 7);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.AddPageTextbox.Location =        new Point(130, 10);
                this.RenamePageCombobox.Location =    new Point(130, 7);
                this.RenamePageTextbox.Location =     new Point(130, 45);
                this.MoveFromCombobox.Location =      new Point(96, 7);
                this.MoveToCombobox.Location =        new Point(96, 45);
                this.RemoveProfileCombobox.Location = new Point(130, 7);
            } else if (Stats.CurrentLanguage == 4 || Stats.CurrentLanguage == 5) { // Simplified Chinese & Traditional Chinese
                this.AddPageTextbox.Location =        new Point(120, 10);
                this.RenamePageCombobox.Location =    new Point(120, 7);
                this.RenamePageTextbox.Location =     new Point(120, 45);
                this.MoveFromCombobox.Location =      new Point(96, 7);
                this.MoveToCombobox.Location =        new Point(96, 45);
                this.RemoveProfileCombobox.Location = new Point(96, 7);
            }
        }
    }
}