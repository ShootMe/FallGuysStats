using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;

namespace FallGuysStats {
    public partial class FilterCustomRange : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        public DateTime startDate, endDate;
        public bool isStartNotSet, isEndNotSet;
        public int selectedCustomTemplateSeason;
        private readonly List<DateTime[]> periodDateTemplates = new List<DateTime[]>();
        public FilterCustomRange() {
            InitializeComponent();
            this.Opacity = 0;
        }

        private void FilterCustomRange_Load(object sender, EventArgs e) {
            this.lbTemplatesList.Items.Clear();
            for (int i = 0; i < Stats.Seasons.Count; i++) {
                if (Stats.Seasons.Count - 1 == i) {
                    this.lbTemplatesList.Items.Add($"{Multilingual.GetWord("custom_range_ffa_season")} 4 [{Stats.Seasons.ElementAt(i).Key}] ({Stats.Seasons.ElementAt(i).Value.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())} - ???)");
                    this.periodDateTemplates.Add(new[] { Stats.Seasons.ElementAt(i).Value, DateTime.MaxValue });
                } else {
                    this.lbTemplatesList.Items.Add($"{(i < 6 ? Multilingual.GetWord("custom_range_legacy_season") : Multilingual.GetWord("custom_range_ffa_season"))} {(i < 6 ? (i + 1) : (i < 10 ? (i - 5) : 4))} [{Stats.Seasons.ElementAt(i).Key}] ({Stats.Seasons.ElementAt(i).Value.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())} - {Stats.Seasons.ElementAt(i + 1).Value.ToString(Multilingual.GetWord("level_date_format"), Utils.GetCultureInfo())})");
                    this.periodDateTemplates.Add(new[] { Stats.Seasons.ElementAt(i).Value, Stats.Seasons.ElementAt(i + 1).Value });
                }
            }
            
            if (this.startDate == DateTime.MinValue && this.endDate == DateTime.MaxValue) {
                this.mdtpStart.Value = DateTime.Now;
                this.mdtpEnd.Value = DateTime.Now;
            } else if (this.selectedCustomTemplateSeason > -1) {
                this.lbTemplatesList.SetSelected(this.selectedCustomTemplateSeason, true);
            } else {
                try {
                    if (this.startDate != DateTime.MinValue) {
                        this.mdtpStart.Value = this.startDate.ToLocalTime();
                    } else {
                        this.picStartDate_MouseClick(this.picStartDate, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                    }
                    if (this.endDate != DateTime.MaxValue) {
                        this.mdtpEnd.Value = this.endDate.ToLocalTime();
                    } else {
                        this.picEndDate_MouseClick(this.picEndDate, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                    }
                } catch {
                    this.mdtpStart.Value = DateTime.Now;
                    this.mdtpEnd.Value = DateTime.Now;
                }
            }

            this.SetTheme(Stats.CurrentTheme);
            this.ChangeLanguage();
        }

        private void FilterCustomRange_Shown(object sender, EventArgs e) {
            this.Opacity = 1;
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.SuspendLayout();
            this.Theme = theme;
            this.BackMaxSize = 32;
            this.BackImagePadding = new Padding(20, 17, 0, 0);
            this.BackImage = theme == MetroThemeStyle.Light ? Properties.Resources.calendar_icon : Properties.Resources.calendar_gray_icon;
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
                } else if (c1 is MetroDateTime mdt1) {
                    mdt1.Theme = theme;
                } else if (c1 is GroupBox gb1) {
                    gb1.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
                    foreach (Control c2 in gb1.Controls) {
                        if (c2 is ListBox lb1) {
                            if (theme == MetroThemeStyle.Dark) {
                                lb1.BackColor = Color.FromArgb(21, 21, 21);
                                lb1.ForeColor = Color.WhiteSmoke;
                            }
                        }
                    }
                }
            }
            this.ResumeLayout();
        }
        
        private void FixDates() {
            if (this.startDate != DateTime.MinValue) {
                this.startDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day, 0, 0, 0, this.selectedCustomTemplateSeason > -1 ? DateTimeKind.Utc : DateTimeKind.Local);
            }
            if (this.endDate != DateTime.MaxValue) {
                if (this.selectedCustomTemplateSeason > -1) {
                    this.endDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 59, 59, DateTimeKind.Utc).AddDays(-1);
                } else {
                    this.endDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 59, 59, DateTimeKind.Local);
                }
            }
        }
        
        private void picStartDate_MouseClick(object sender, MouseEventArgs e) {
            this.isStartNotSet = !this.isStartNotSet;
            ((PictureBox)sender).Image = this.isStartNotSet ? Properties.Resources.calendar_off_icon : Properties.Resources.calendar_on_icon;
            this.mdtpStart.Visible = !this.isStartNotSet;
            this.lbTemplatesList.ClearSelected();
            if (this.isStartNotSet && this.isEndNotSet) {
                this.isEndNotSet = false;
                this.mdtpEnd.Visible = true;
                this.picEndDate.Image = Properties.Resources.calendar_on_icon;
            }

            if (this.mdtpStart.Visible) {
                if (this.mdtpStart.Value > this.mdtpEnd.Value) {
                    this.startDate = this.mdtpEnd.Value;
                    this.mdtpStart.Value = this.mdtpEnd.Value;
                }
            }
        }

        private void picEndDate_MouseClick(object sender, MouseEventArgs e) {
            this.isEndNotSet = !this.isEndNotSet;
            ((PictureBox)sender).Image = this.isEndNotSet ? Properties.Resources.calendar_off_icon : Properties.Resources.calendar_on_icon;
            this.mdtpEnd.Visible = !this.isEndNotSet;
            this.lbTemplatesList.ClearSelected();
            if (this.isStartNotSet && this.isEndNotSet) {
                this.isStartNotSet = false;
                this.mdtpStart.Visible = true;
                this.picStartDate.Image = Properties.Resources.calendar_on_icon;
            }

            if (this.mdtpEnd.Visible) {
                if (this.mdtpEnd.Value < this.mdtpStart.Value) {
                    this.endDate = this.mdtpStart.Value;
                    this.mdtpEnd.Value = this.mdtpStart.Value;
                }
            }
        }
        
        private void dtStart_CloseUp(object sender, EventArgs e) {
            if (((MetroDateTime)sender).Value != this.startDate) {
                this.lbTemplatesList.ClearSelected();
                if (((MetroDateTime)sender).Value > this.mdtpEnd.Value) {
                    this.endDate = ((MetroDateTime)sender).Value;
                    this.mdtpEnd.Value = ((MetroDateTime)sender).Value;
                }
            }
        }
        
        private void dtEnd_CloseUp(object sender, EventArgs e) {
            if (((MetroDateTime)sender).Value != this.endDate) {
                this.lbTemplatesList.ClearSelected();
                if (((MetroDateTime)sender).Value < this.mdtpStart.Value) {
                    this.startDate = ((MetroDateTime)sender).Value;
                    this.mdtpStart.Value = ((MetroDateTime)sender).Value;
                }
            }
        }

        private void lbTemplatesList_SelectedValueChanged(object sender, EventArgs e) {
            this.selectedCustomTemplateSeason = ((ListBox)sender).SelectedIndex;
            if (((ListBox)sender).SelectedIndex < 0) { return; }
            if (this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][0] == DateTime.MinValue) {
                this.isStartNotSet = true;
                this.picStartDate.Image = Properties.Resources.calendar_off_icon;
                this.startDate = DateTime.MinValue;
                this.mdtpStart.Visible = false;
            } else {
                this.isStartNotSet = false;
                this.picStartDate.Image = Properties.Resources.calendar_on_icon;
                this.mdtpStart.Value = this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][0];
                this.startDate = this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][0];
                this.mdtpStart.Visible = true;
            }
            if (this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][1] == DateTime.MaxValue) {
                this.isEndNotSet = true;
                this.picEndDate.Image = Properties.Resources.calendar_off_icon;
                this.endDate = DateTime.MaxValue;
                this.mdtpEnd.Visible = false;
            } else {
                this.isEndNotSet = false;
                this.picEndDate.Image = Properties.Resources.calendar_on_icon;
                this.mdtpEnd.Value = this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][1];
                this.endDate = this.periodDateTemplates[this.lbTemplatesList.SelectedIndex][1];
                this.mdtpEnd.Visible = true;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e) {
            this.startDate = this.isStartNotSet ? DateTime.MinValue : this.mdtpStart.Value;
            this.endDate = this.isEndNotSet ? DateTime.MaxValue : this.mdtpEnd.Value;
            this.FixDates();
            this.DialogResult = DialogResult.OK;
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
            this.Text = $"     {Multilingual.GetWord("custom_range_range")}";
            this.mdtpStart.CalendarFont = Overlay.GetMainFont(14);
            this.mdtpEnd.CalendarFont = Overlay.GetMainFont(14);
            this.lbTemplatesList.Font = Overlay.GetMainFont(14);
            this.btnFilter.Text = Multilingual.GetWord("custom_range_filter");
            this.btnFilter.Width = TextRenderer.MeasureText(this.btnFilter.Text, this.btnFilter.Font).Width + 45;
            this.btnFilter.Left = this.grpTemplates.Right - this.btnFilter.Width;
            this.grpTemplates.Text = Multilingual.GetWord("custom_range_templates");
        }
    }
}
