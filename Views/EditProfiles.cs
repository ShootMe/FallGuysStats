using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class EditProfiles : Form {
        public List<Profiles> Profiles { get; set; }
        public List<RoundInfo> AllStats { get; set; }
        public Stats StatsForm { get; set; }
        private DataTable ProfilesData;
        private DataGridViewComboBoxColumn cboShowsList;
        private int selectedRowIndex;

        private string[] _shows = {
            "",
            "main_show",
            "squads_2player_template",
            "squads_4player",
            "event_xtreme_fall_guys_template",
            "event_xtreme_fall_guys_squads_template",
            "event_only_finals_v2_template",
            "event_only_races_any_final_template",
            "event_only_fall_ball_template",
            "event_only_hexaring_template",
            "event_only_floor_fall_template",
            "event_only_blast_ball_trials_template",
            "event_only_slime_climb",
            "event_only_jump_club_template",
            "event_walnut_template",
            "event_only_survival_ss2_3009_0210_2022",
            "show_robotrampage_ss2_show1_template",
            "event_le_anchovy_template",
            "event_pixel_palooza_template",
            "private_lobbies"
        };

        public EditProfiles() => this.InitializeComponent();

        private void EditProfiles_Load(object sender, EventArgs e) {
            this.ChangeLanguage();
            this.InitProfileList();
            this.ReloadProfileList();
            this.ProfileList.ClearSelection();
        }

        private void InitProfileList() {
            this.cboShowsList = new DataGridViewComboBoxColumn();
            this.cboShowsList.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            DataTable showsData = new DataTable();
            showsData.Columns.Add("showName");
            showsData.Columns.Add("showId");
            foreach (string showId in _shows) {
                //if (this.Profiles.FindIndex(item => item.LinkedShowId == showId) != -1) continue;
                showsData.Rows.Add(Multilingual.GetShowName(showId), showId);
            }

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
            foreach (var profile in this.Profiles) {
                this.ProfilesData.Rows.Add($"{profile.ProfileName} [{AllStats.FindAll(r => r.Profile == profile.ProfileId).Count} {Multilingual.GetWord("profile_rounds_suffix")}]", profile.LinkedShowId);
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
                if (AllStats.FindAll(r => r.Profile == this.Profiles[i].ProfileId).Count != 0) {
                    this.MoveFromCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
                /*if (Profiles[i].ProfileID != 0 && AllStats.FindAll(r => r.Profile == Profiles[i].ProfileID).Count == 0) {
                    RemoveProfileCombobox.Items.Insert(0, Profiles[i].ProfileName);
                }*/
                if (this.Profiles[i].ProfileId != 0) {
                    this.RemoveProfileCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
            }
        }

        private void ProfileList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            var datagridview = sender as DataGridView;
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
            var datagridview = sender as DataGridView;
            if (e.ColumnIndex == 1) {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void ProfileList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            var datagridview = sender as DataGridView;
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

        private void DeleteAmpersend_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 0x26) e.KeyChar = Convert.ToChar(0);
        }

        private void AddPageButton_Click(object sender, EventArgs e) {
            if (this.AddPageTextbox.Text.Length == 0) { return; }

            if (MessageBox.Show(this,
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
            if (MessageBox.Show(this,
                    $"{Multilingual.GetWord("message_delete_profile_prefix")} ({this.RemoveProfileCombobox.SelectedItem}) {Multilingual.GetWord("message_delete_profile_infix")} {Multilingual.GetWord("message_delete_profile_suffix")}",
                    Multilingual.GetWord("message_delete_profile_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                string prevProfileName = string.Empty;
                for (int i = 0; i < this.RemoveProfileCombobox.Items.Count; i++) {
                    if (this.RemoveProfileCombobox.Items[i].ToString() == this.RemoveProfileCombobox.SelectedItem.ToString()) {
                        if (i > 0) {
                            prevProfileName = this.RemoveProfileCombobox.Items[i-1].ToString();
                        }
                    }
                }

                int prevProfileId;
                if (string.IsNullOrEmpty(prevProfileName)) {
                    prevProfileId = 0;
                } else {
                    prevProfileId = this.Profiles.Find(p => p.ProfileName == prevProfileName).ProfileId;
                }
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
            if (MessageBox.Show(this,
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
            if (MessageBox.Show(this, 
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

        private void RenameComboxChanged(object sender, EventArgs e) {
            this.RenamePageTextbox.Text = this.RenamePageCombobox.SelectedItem.ToString();
        }

        /*private void ApplyChangeButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }*/
        
        /*private void UndoChangeButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }*/
        
        private void EditProfile_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ChangeLanguage() {
            this.Font = new Font(Overlay.GetMainFontFamilies(), 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Text = Multilingual.GetWord("profile_title");
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
                this.AddPageTextbox.Location =        new Point(100, 10);
                this.RenamePageCombobox.Location =    new Point(100, 10);
                this.RenamePageTextbox.Location =     new Point(100, 44);
                this.MoveFromCombobox.Location =      new Point(71, 10);
                this.MoveToCombobox.Location =        new Point(71, 44);
                this.RemoveProfileCombobox.Location = new Point(65, 10);
            } else if (Stats.CurrentLanguage == 1) { // French
                this.AddPageTextbox.Location =        new Point(100, 10);
                this.RenamePageCombobox.Location =    new Point(100, 10);
                this.RenamePageTextbox.Location =     new Point(100, 44);
                this.MoveFromCombobox.Location =      new Point(71, 10);
                this.MoveToCombobox.Location =        new Point(71, 44);
                this.RemoveProfileCombobox.Location = new Point(65, 10);
            } else if (Stats.CurrentLanguage == 2) { // Korean
                this.AddPageTextbox.Location =        new Point(76, 7);
                this.RenamePageCombobox.Location =    new Point(76, 7);
                this.RenamePageTextbox.Location =     new Point(76, 41);
                this.MoveFromCombobox.Location =      new Point(76, 7);
                this.MoveToCombobox.Location =        new Point(76, 41);
                this.RemoveProfileCombobox.Location = new Point(76, 7);
            } else if (Stats.CurrentLanguage == 3) { // Japanese
                this.AddPageTextbox.Location =        new Point(110, 8);
                this.RenamePageCombobox.Location =    new Point(110, 8);
                this.RenamePageTextbox.Location =     new Point(110, 44);
                this.MoveFromCombobox.Location =      new Point(81, 10);
                this.MoveToCombobox.Location =        new Point(81, 44);
                this.RemoveProfileCombobox.Location = new Point(95, 10);
            } else if (Stats.CurrentLanguage == 4) { // Simplified Chinese
                this.AddPageTextbox.Location =        new Point(110, 8);
                this.RenamePageCombobox.Location =    new Point(110, 8);
                this.RenamePageTextbox.Location =     new Point(110, 44);
                this.MoveFromCombobox.Location =      new Point(81, 10);
                this.MoveToCombobox.Location =        new Point(81, 44);
                this.RemoveProfileCombobox.Location = new Point(95, 10);
            }
        }
    }
}