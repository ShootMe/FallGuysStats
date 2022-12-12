using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FallGuysStats {
    public partial class EditProfilesForm : Form {
        public List<Profiles> Profiles { get; set; }
        public List<RoundInfo> AllStats { get; set; }
        public EditProfilesForm() {
            InitializeComponent();
        }

        private void EditProfilesForm_Load(object sender, EventArgs e) {
            reloadProfileList();
        }

        private void reloadProfileList() {
            AddPageTextbox.Text = "";
            RenamePageTextbox.Text = "";
            ProfileList.Items.Clear();
            RenamePageCombobox.Items.Clear();
            MoveFromCombobox.Items.Clear();
            MoveToCombobox.Items.Clear();
            RemoveProfileCombobox.Items.Clear();
            for (int i = 0; i < Profiles.Count; i++) {
                ProfileList.Items.Insert(0, "[" + Profiles[i].ProfileID + "] " + Profiles[i].ProfileName + " (" + AllStats.FindAll(r => r.Profile == Profiles[i].ProfileID).Count + " Rounds)");
                RenamePageCombobox.Items.Insert(0, Profiles[i].ProfileName);
                MoveFromCombobox.Items.Insert(0, Profiles[i].ProfileName);
                MoveToCombobox.Items.Insert(0, Profiles[i].ProfileName);
                if (Profiles[i].ProfileID != 0 && AllStats.FindAll(r => r.Profile == Profiles[i].ProfileID).Count == 0) {
                    RemoveProfileCombobox.Items.Insert(0, Profiles[i].ProfileName);
                }
            }
        }

        private void AddPageButton_Click(object sender, EventArgs e) {
            if (AddPageTextbox.Text.Length == 0) { return; }
            int maxID = 1;
            for (int i = 0; i < Profiles.Count; i++) {
                if (maxID < Profiles[i].ProfileID) { maxID = Profiles[i].ProfileID; }
                if (Profiles[i].ProfileName == AddPageTextbox.Text) { return; }
            }
            Profiles.Insert(0, new Profiles() { ProfileID = maxID + 1, ProfileName = AddPageTextbox.Text });
            reloadProfileList();
        }

        private void RemovePageButton_Click(object sender, EventArgs e) {
            if (RemoveProfileCombobox.SelectedIndex < 0) { return; }
            Profiles.Remove(Profiles.Find(p => p.ProfileName == RemoveProfileCombobox.SelectedItem.ToString()));
            reloadProfileList();
        }

        private void MovePageButton_Click(object sender, EventArgs e) {
            if (MoveToCombobox.SelectedIndex < 0) { return; }
            if (MoveFromCombobox.SelectedIndex < 0) { return; }
            if (MoveFromCombobox.SelectedIndex == MoveToCombobox.SelectedIndex) { return; }
            int fromID = Profiles.Find(p => p.ProfileName == MoveFromCombobox.SelectedItem.ToString()).ProfileID;
            int toID = Profiles.Find(p => p.ProfileName == MoveToCombobox.SelectedItem.ToString()).ProfileID;
            for (int i = 0; i < AllStats.Count; i++) {
                if (AllStats[i].Profile != fromID) { continue; }
                AllStats[i].Profile = toID;
            }
            reloadProfileList();
        }

        private void RenameButton_Click(object sender, EventArgs e) {
            if (RenamePageCombobox.SelectedIndex < 0) { return; }
            if (RenamePageTextbox.Text.Length == 0) { return; }
            for (int i = 0; i < Profiles.Count; i++) {
                if (Profiles[i].ProfileName != RenamePageCombobox.SelectedItem.ToString()) { continue; }
                Profiles[i].ProfileName = RenamePageTextbox.Text;
            }
            reloadProfileList();
        }
    }
}
