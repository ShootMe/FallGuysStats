using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class EditProfiles : Form {
        public List<Profiles> Profiles { get; set; }
        public List<RoundInfo> AllStats { get; set; }
        public Stats StatsForm { get; set; }
        public EditProfiles() => this.InitializeComponent();

        private void EditProfiles_Load(object sender, EventArgs e) {
            this.ChangeLanguage();
            this.ReloadProfileList();
        }

        private void ReloadProfileList() {
            
            
            this.Profiles = this.Profiles.OrderByDescending(p => p.ProfileOrder).ToList();
            this.AddPageTextbox.Text = "";
            this.RenamePageTextbox.Text = "";
            this.ProfileList.Items.Clear();
            this.RenamePageCombobox.Items.Clear();
            this.MoveFromCombobox.Items.Clear();
            this.MoveToCombobox.Items.Clear();
            this.RemoveProfileCombobox.Items.Clear();
            for (int i = 0; i < this.Profiles.Count; i++) {
                if (this.Profiles[i].ProfileOrder == 0) { this.Profiles[i].ProfileOrder = this.Profiles.Count - i; }
                this.ProfileList.Items.Insert(0, $"{this.Profiles[i].ProfileName} [{AllStats.FindAll(r => r.Profile == this.Profiles[i].ProfileID).Count} {Multilingual.GetWord("profile_rounds_suffix")}]");
                this.RenamePageCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                this.MoveToCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                if (AllStats.FindAll(r => r.Profile == this.Profiles[i].ProfileID).Count != 0) {
                    this.MoveFromCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
                /*if (Profiles[i].ProfileID != 0 && AllStats.FindAll(r => r.Profile == Profiles[i].ProfileID).Count == 0) {
                    RemoveProfileCombobox.Items.Insert(0, Profiles[i].ProfileName);
                }*/
                if (this.Profiles[i].ProfileID != 0) {
                    this.RemoveProfileCombobox.Items.Insert(0, this.Profiles[i].ProfileName);
                }
            }
        }

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
                    if (maxID <= this.Profiles[i].ProfileID) { maxID = this.Profiles[i].ProfileID; }
                    if (order <= this.Profiles[i].ProfileOrder) { order = this.Profiles[i].ProfileOrder; }
                    if (this.Profiles[i].ProfileName == this.AddPageTextbox.Text) { return; }
                }
                this.Profiles.Insert(0, new Profiles { ProfileID = maxID + 1, ProfileName = this.AddPageTextbox.Text, ProfileOrder = order + 1 });
                this.ReloadProfileList();
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
                    prevProfileId = this.Profiles.Find(p => p.ProfileName == prevProfileName).ProfileID;
                }
                int profileId = this.Profiles.Find(p => p.ProfileName == this.RemoveProfileCombobox.SelectedItem.ToString()).ProfileID;
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
                int fromId = this.Profiles.Find(p => p.ProfileName == this.MoveFromCombobox.SelectedItem.ToString()).ProfileID;
                int toId = this.Profiles.Find(p => p.ProfileName == this.MoveToCombobox.SelectedItem.ToString()).ProfileID;
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
            if (this.ProfileList.SelectedItems.Count <= 0) { return; }
            if (this.ProfileList.SelectedIndex <= 0) { return; }
            
            int profileListIndex = this.ProfileList.SelectedIndex;
            int profileIndex = this.Profiles.Count - this.ProfileList.SelectedIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex + 1].ProfileOrder) = (this.Profiles[profileIndex + 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.ReloadProfileList();
            this.ProfileList.SelectedIndex = profileListIndex - 1;
        }
        
        private void ProfileListDown_Click(object sender, EventArgs e) {
            if (this.ProfileList.SelectedItems.Count <= 0) { return; }
            if (this.ProfileList.SelectedIndex >= this.ProfileList.Items.Count - 1) { return; }
            
            int profileListIndex = this.ProfileList.SelectedIndex;
            int profileIndex = this.Profiles.Count - this.ProfileList.SelectedIndex - 1;
            (this.Profiles[profileIndex].ProfileOrder, this.Profiles[profileIndex - 1].ProfileOrder) = (this.Profiles[profileIndex - 1].ProfileOrder, this.Profiles[profileIndex].ProfileOrder);
            this.ReloadProfileList();
            this.ProfileList.SelectedIndex = profileListIndex + 1;
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
            this.Font = new Font(Overlay.DefaultFontCollection.Families[0], 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Text = Multilingual.GetWord("profile_title");
            this.ProfileList.Items.AddRange(new object[] {
                Multilingual.GetWord("main_profile_solo"),
                Multilingual.GetWord("main_profile_duo"),
                Multilingual.GetWord("main_profile_squad")});
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
                this.RemoveProfileCombobox.Location = new Point(95, 10);
            }
        }
    }
}