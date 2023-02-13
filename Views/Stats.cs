using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LiteDB;
using Microsoft.Win32;
namespace FallGuysStats {
    public partial class Stats : Form {
        [STAThread]
        static void Main(string[] args) {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Stats());
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static string LOGNAME = "Player.log";
        private static List<DateTime> Seasons = new List<DateTime> {
            new DateTime(2020, 8, 4, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 10, 8, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 3, 22, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 6, 21, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 11, 22, 0, 0, 0, DateTimeKind.Utc)
        };
        private static DateTime SeasonStart, WeekStart, DayStart;
        private static DateTime SessionStart = DateTime.UtcNow;
        public static bool InShow = false;
        public static bool EndedShow = false;
        public static int LastServerPing = 0;
        public static int CurrentLanguage = 0;

        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        public List<LevelStats> StatDetails = new List<LevelStats>();
        public List<RoundInfo> CurrentRound = null;
        public List<RoundInfo> AllStats = new List<RoundInfo>();
        public Dictionary<string, LevelStats> StatLookup = new Dictionary<string, LevelStats>();
        private LogFileWatcher logFile = new LogFileWatcher();
        public int Shows;
        public int Rounds;
        public TimeSpan Duration;
        public int Wins;
        public int Finals;
        public int Kudos;
        private int nextShowID;
        private bool loadingExisting;
        public LiteDatabase StatsDB;
        public ILiteCollection<RoundInfo> RoundDetails;
        public ILiteCollection<UserSettings> UserSettings;
        public ILiteCollection<Profiles> Profiles;
        public List<Profiles> AllProfiles = new List<Profiles>();
        public List<ToolStripMenuItem> ProfileMenuItems = new List<ToolStripMenuItem>();
        public UserSettings CurrentSettings;
        private Overlay overlay;
        private DateTime lastAddedShow = DateTime.MinValue;
        private DateTime startupTime = DateTime.UtcNow;
        private int askedPreviousShows = 0;
        private TextInfo textInfo;
        private int currentProfile;
        private Color infoStripForeColor;

        public Stats() {
            this.StatsDB = new LiteDatabase(@"data.db");
            this.StatsDB.Pragma("UTC_DATE", true);
            this.UserSettings = this.StatsDB.GetCollection<UserSettings>("UserSettings");
            this.StatsDB.BeginTrans();
            if (this.UserSettings.Count() == 0) {
                this.CurrentSettings = this.GetDefaultSettings();
                this.UserSettings.Insert(this.CurrentSettings);
            } else {
                try {
                    this.CurrentSettings = this.UserSettings.FindAll().First();
                    CurrentLanguage = this.CurrentSettings.Multilingual;
                } catch {
                    this.UserSettings.DeleteAll();
                    this.CurrentSettings = GetDefaultSettings();
                    this.UserSettings.Insert(this.CurrentSettings);
                }
            }
            this.StatsDB.Commit();
            
            this.InitializeComponent();

            this.ChangeMainLanguage();
            this.Text = $"Fall Guys Stats v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
            this.textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            this.logFile.OnParsedLogLines += this.LogFile_OnParsedLogLines;
            this.logFile.OnNewLogFileDate += this.LogFile_OnNewLogFileDate;
            this.logFile.OnError += this.LogFile_OnError;
            this.logFile.OnParsedLogLinesCurrent += this.LogFile_OnParsedLogLinesCurrent;

            foreach (var entry in LevelStats.ALL) {
                this.StatDetails.Add(entry.Value);
                this.StatLookup.Add(entry.Key, entry.Value);
            }

            this.gridDetails.DataSource = this.StatDetails;

            this.RoundDetails = this.StatsDB.GetCollection<RoundInfo>("RoundDetails");
            this.Profiles = this.StatsDB.GetCollection<Profiles>("Profiles");

            this.StatsDB.BeginTrans();

            if (this.Profiles.Count() == 0) {
                this.Profiles.Insert(new Profiles() { ProfileID = 2, ProfileName = Multilingual.GetWord("main_profile_squad"), ProfileOrder = 3 });
                this.Profiles.Insert(new Profiles() { ProfileID = 1, ProfileName = Multilingual.GetWord("main_profile_duo"), ProfileOrder = 2 });
                this.Profiles.Insert(new Profiles() { ProfileID = 0, ProfileName = Multilingual.GetWord("main_profile_solo"), ProfileOrder = 1 });
            }
            this.UpdateGridRoundName();
            this.UpdateHoopsieLegends();

            this.RoundDetails.EnsureIndex(x => x.Name);
            this.RoundDetails.EnsureIndex(x => x.ShowID);
            this.RoundDetails.EnsureIndex(x => x.Round);
            this.RoundDetails.EnsureIndex(x => x.Start);
            this.RoundDetails.EnsureIndex(x => x.InParty);
            this.StatsDB.Commit();

            this.UpdateDatabaseVersion();
            
            this.CurrentRound = new List<RoundInfo>();

            this.overlay = new Overlay() { StatsForm = this, Icon = this.Icon, ShowIcon = true };
            this.overlay.Show();
            this.overlay.Visible = false;
            this.overlay.StartTimer();

            this.UpdateGameExeLocation();
            if (this.CurrentSettings.AutoLaunchGameOnStartup) {
                this.LaunchGame(true);
            }

            this.RemoveUpdateFiles();
            
            this.ReloadProfileMenuItems();
        }
        public void ReloadProfileMenuItems() {
            this.ProfileMenuItems.Clear();
            this.menuProfile.DropDownItems.Clear();
            this.menuProfile.DropDownItems.Add(this.menuEditProfiles);
            this.AllProfiles.Clear();
            this.AllProfiles = this.Profiles.FindAll().ToList();
            int profileNumber = 0; 
            for (int i = this.AllProfiles.Count - 1; i >= 0; i--) {
                Profiles profile = this.AllProfiles[i];
                var menuItem = new ToolStripMenuItem();
                menuItem.Checked = this.CurrentSettings.SelectedProfile == profile.ProfileID;
                menuItem.CheckOnClick = true;
                menuItem.CheckState = this.CurrentSettings.SelectedProfile == profile.ProfileID ? CheckState.Checked : CheckState.Unchecked;
                menuItem.Name = "menuProfile" + profile.ProfileID;
                switch (profileNumber++) {
                    case 0:
                        menuItem.Image = Properties.Resources.number_1;
                        break;
                    case 1:
                        menuItem.Image = Properties.Resources.number_2;
                        break;
                    case 2:
                        menuItem.Image = Properties.Resources.number_3;
                        break;
                    case 3:
                        menuItem.Image = Properties.Resources.number_4;
                        break;
                    case 4:
                        menuItem.Image = Properties.Resources.number_5;
                        break;
                    case 5:
                        menuItem.Image = Properties.Resources.number_6;
                        break;
                    case 6:
                        menuItem.Image = Properties.Resources.number_7;
                        break;
                    case 7:
                        menuItem.Image = Properties.Resources.number_8;
                        break;
                    case 8:
                        menuItem.Image = Properties.Resources.number_9;
                        break;
                }
                menuItem.Size = new Size(180, 22);
                menuItem.Text = profile.ProfileName;
                menuItem.Click += this.menuStats_Click;
                menuProfile.DropDownItems.Add(menuItem);
                this.ProfileMenuItems.Add(menuItem);
                if (this.CurrentSettings.SelectedProfile == profile.ProfileID) {
                    menuItem.PerformClick();
                }
            }
        }
        private void RemoveUpdateFiles() {
#if AllowUpdate
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string file in Directory.EnumerateFiles(filePath, "*.bak")) {
                try {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                } catch { }
            }
#endif
        }
        private void UpdateDatabaseVersion() {
            if (!this.CurrentSettings.UpdatedDateFormat) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    info.Start = DateTime.SpecifyKind(info.Start.ToLocalTime(), DateTimeKind.Utc);
                    info.End = DateTime.SpecifyKind(info.End.ToLocalTime(), DateTimeKind.Utc);
                    info.Finish = info.Finish.HasValue ? DateTime.SpecifyKind(info.Finish.Value.ToLocalTime(), DateTimeKind.Utc) : (DateTime?)null;
                    this.RoundDetails.Update(info);
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.UpdatedDateFormat = true;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 0) {
                this.CurrentSettings.SwitchBetweenQualify = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 1;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 1) {
                this.CurrentSettings.SwitchBetweenPlayers = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 2;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 2) {
                this.CurrentSettings.SwitchBetweenStreaks = this.CurrentSettings.SwitchBetweenLongest;
                this.CurrentSettings.Version = 3;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 3 || this.CurrentSettings.Version == 4) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_variation", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 5;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 5 || this.CurrentSettings.Version == 6) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_northernlion", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 7;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 7) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_hard_mode", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 8;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 8) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 9;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 9) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    if (info.Name.Equals("round_fall_mountain", StringComparison.OrdinalIgnoreCase)) {
                        info.Name = "round_fall_mountain_hub_complete";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 10;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 10) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];
                    int index = 0;
                    if ((index = info.Name.IndexOf("_event_", StringComparison.OrdinalIgnoreCase)) > 0
                        || (index = info.Name.IndexOf(". D", StringComparison.OrdinalIgnoreCase)) > 0) {
                        info.Name = info.Name.Substring(0, index);
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 11;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 11) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.AllStats.Sort();
                this.StatsDB.BeginTrans();
                int lastShow = -1;
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (lastShow != info.ShowID) {
                        lastShow = info.ShowID;

                        if (this.StatLookup.TryGetValue(info.Name, out LevelStats stats)) {
                            info.IsFinal = stats.IsFinal && (info.Name != "round_floor_fall" || info.Round >= 3 || (i > 0 && this.AllStats[i - 1].Name != "round_floor_fall"));
                        } else {
                            info.IsFinal = false;
                        }
                    } else {
                        info.IsFinal = false;
                    }

                    this.RoundDetails.Update(info);
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 12;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 12 || this.CurrentSettings.Version == 13) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_fruitpunch", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_fruitpunch_s4_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoverboardsurvival", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoverboardsurvival_s4_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_basketfall", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_basketfall_s4_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_territory", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_territory_control_s4_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_shortcircuit", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_shortcircuit";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_06", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_06";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_tunnel_race", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_tunnel_race";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_1v1_button", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_1v1_button_basher";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_slimeclimb", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slimeclimb_2";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 14;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 14) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_king_of", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_king_of_the_hill";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_drumtop", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_drumtop";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_penguin_solos", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_penguin_solos";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_07", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_07";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_robotrampage", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_robotrampage_arena_2";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_crown_maze", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_crown_maze";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 15;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 15) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_gauntlet_08", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_08";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_airtime", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_airtime";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_follow-", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow-the-leader_s6_launch";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_pipedup", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_pipedup_s6_launch";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_see_saw_360", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_see_saw_360";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 16;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 16) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_fruit_bowl", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_fruit_bowl";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 17;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 17) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];

                    if (info.Name.IndexOf("round_invisibeans", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_invisibeans";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 18;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 18) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_1v1_volleyfall", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_1v1_volleyfall_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_09", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_09_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_short_circuit_2", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_short_circuit_2_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoops_revenge", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoops_revenge_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hexaring", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hexaring_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_spin_ring", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_spin_ring_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_blastball", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_blastball_arenasurvival_symphony_launch_show";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 19;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 19) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_satellitehoppers", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_satellitehoppers_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_ffa_button_bashers", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_ffa_button_bashers_squads_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hoverboardsurvival2", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hoverboardsurvival2_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_gauntlet_10", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_gauntlet_10_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_starlink", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_starlink_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_tiptoefinale", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_tiptoefinale_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_pixelperfect", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_pixelperfect_almond";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_hexsnake", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_hexsnake_almond";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 20;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 20) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_follow-the-leader", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow-the-leader_s6_launch";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 21;
                this.SaveUserSettings();
            }
            
            if (this.CurrentSettings.Version == 21) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_slippy_slide", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slippy_slide";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_follow_the_line", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow_the_line";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_slide_chute", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slide_chute";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_blastballruins", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_blastballruins";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_kraken_attack", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_kraken_attack";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_bluejay", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_bluejay";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 22;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 22) {
                this.AllStats.AddRange(this.RoundDetails.FindAll());
                this.StatsDB.BeginTrans();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = this.AllStats[i];

                    if (info.Name.IndexOf("round_slippy_slide", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slippy_slide";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_follow_the_line", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_follow_the_line";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_slide_chute", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_slide_chute";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_blastballruins", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_blastballruins";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_kraken_attack", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_kraken_attack";
                        this.RoundDetails.Update(info);
                    } else if (info.Name.IndexOf("round_bluejay", StringComparison.OrdinalIgnoreCase) == 0) {
                        info.Name = "round_bluejay";
                        this.RoundDetails.Update(info);
                    }
                }
                this.StatsDB.Commit();
                this.AllStats.Clear();
                this.CurrentSettings.Version = 23;
                this.SaveUserSettings();
            }

            if (this.CurrentSettings.Version == 23) {
                this.CurrentSettings.GameExeLocation = string.Empty;
                this.CurrentSettings.Version = 24;
                this.SaveUserSettings();
            }
        }
        private UserSettings GetDefaultSettings() {
            return new UserSettings() {
                ID = 1,
                CycleTimeSeconds = 5,
                FilterType = 0,
                SelectedProfile = 0,
                FlippedDisplay = false,
                LogPath = null,
                OverlayColor = 0,
                OverlayLocationX = null,
                OverlayLocationY = null,
                SwitchBetweenLongest = true,
                SwitchBetweenQualify = true,
                SwitchBetweenPlayers = true,
                SwitchBetweenStreaks = true,
                OnlyShowLongest = false,
                OnlyShowGold = false,
                OnlyShowPing = false,
                OnlyShowFinalStreak = false,
                OverlayVisible = false,
                OverlayNotOnTop = false,
                PlayerByConsoleType = false,
                ColorByRoundType = false,
                PreviousWins = 0,
                WinsFilter = 0,
                QualifyFilter = 0,
                FastestFilter = 0,
                HideWinsInfo = false,
                HideRoundInfo = false,
                HideTimeInfo = false,
                ShowOverlayTabs = false,
                ShowPercentages = false,
                AutoUpdate = false,
                FormLocationX = null,
                FormLocationY = null,
                FormWidth = null,
                FormHeight = null,
                OverlayWidth = 786,
                OverlayHeight = 99,
                HideOverlayPercentages = false,
                HoopsieHeros = false,
                Version = 23,
                AutoLaunchGameOnStartup = false,
                GameExeLocation = string.Empty,
                GameShortcutLocation = string.Empty,
                IgnoreLevelTypeWhenSorting = false,
                UpdatedDateFormat = true
            };
        }
        private void UpdateHoopsieLegends() {
            LevelStats level = this.StatLookup["round_hoops_blockade_solo"];
            string newName = this.CurrentSettings.HoopsieHeros ? Multilingual.GetWord("main_hoopsie_heroes") : Multilingual.GetWord("main_hoopsie_legends");
            if (level.Name != newName) {
                level.Name = newName;
                this.gridDetails.Invalidate();
            }
        }
        private void UpdateGridRoundName() {
            Dictionary<string, string> rounds = Multilingual.GetRoundsDictionary();
            foreach(KeyValuePair<string, string> item in rounds) {
                LevelStats level = StatLookup[item.Key];
                level.Name = item.Value;
            }
            this.gridDetails.Invalidate();
        }
        public void UpdateDates() {
            if (DateTime.Now.Date.ToUniversalTime() == DayStart) { return; }

            DateTime currentUTC = DateTime.UtcNow;
            for (int i = Seasons.Count - 1; i >= 0; i--) {
                if (currentUTC > Seasons[i]) {
                    SeasonStart = Seasons[i];
                    break;
                }
            }
            WeekStart = DateTime.Now.Date.AddDays(-7).ToUniversalTime();
            DayStart = DateTime.Now.Date.ToUniversalTime();

            this.ResetStats();
        }
        public void SaveUserSettings() {
            lock (this.StatsDB) {
                this.StatsDB.BeginTrans();
                this.UserSettings.Update(this.CurrentSettings);
                this.StatsDB.Commit();
            }
        }
        public void ResetStats() {
            for (int i = 0; i < this.StatDetails.Count; i++) {
                LevelStats calculator = StatDetails[i];
                calculator.Clear();
            }

            this.ClearTotals();

            List<RoundInfo> rounds = new List<RoundInfo>();
            int profile = this.currentProfile;

            lock (this.StatsDB) {
                this.AllStats.Clear();
                this.nextShowID = 0;
                this.lastAddedShow = DateTime.MinValue;
                if (this.RoundDetails.Count() > 0) {
                    this.AllStats.AddRange(this.RoundDetails.FindAll());
                    this.AllStats.Sort();

                    if (this.AllStats.Count > 0) {
                        this.nextShowID = this.AllStats[this.AllStats.Count - 1].ShowID;

                        int lastAddedShowID = -1;
                        for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                            RoundInfo info = this.AllStats[i];
                            info.ToLocalTime();
                            if (info.Profile != profile) { continue; }

                            if (info.ShowID == lastAddedShowID || (IsInStatsFilter(info.Start) && IsInPartyFilter(info))) {
                                lastAddedShowID = info.ShowID;
                                rounds.Add(info);
                            }

                            if (info.Start > lastAddedShow && info.Round == 1) {
                                this.lastAddedShow = info.Start;
                            }
                        }
                    }
                }
            }

            lock (this.CurrentRound) {
                this.CurrentRound.Clear();
                for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                    RoundInfo info = AllStats[i];
                    if (info.Profile != profile) { continue; }

                    this.CurrentRound.Insert(0, info);
                    if (info.Round == 1) {
                        break;
                    }
                }
            }

            rounds.Sort();
            this.loadingExisting = true;
            this.LogFile_OnParsedLogLines(rounds);
            this.loadingExisting = false;
        }
        private void Stats_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                if (!this.overlay.Disposing && !this.overlay.IsDisposed && !this.IsDisposed && !this.Disposing) {
                    if (this.overlay.Visible) {
                        this.CurrentSettings.OverlayLocationX = this.overlay.Location.X;
                        this.CurrentSettings.OverlayLocationY = this.overlay.Location.Y;

                        this.CurrentSettings.OverlayWidth = this.overlay.Width;
                        this.CurrentSettings.OverlayHeight = this.overlay.Height;
                    }
                    this.CurrentSettings.FilterType = this.menuAllStats.Checked ? 0 : this.menuSeasonStats.Checked ? 1 : this.menuWeekStats.Checked ? 2 : this.menuDayStats.Checked ? 3 : 4;
                    this.CurrentSettings.SelectedProfile = this.currentProfile;

                    this.CurrentSettings.FormLocationX = this.Location.X;
                    this.CurrentSettings.FormLocationY = this.Location.Y;
                    this.CurrentSettings.FormWidth = this.ClientSize.Width;
                    this.CurrentSettings.FormHeight = this.ClientSize.Height;
                    this.SaveUserSettings();
                }
                this.StatsDB.Dispose();
            } catch { }
        }
        private void Stats_Load(object sender, EventArgs e) {
            try {
                if (this.CurrentSettings.FormWidth.HasValue) {
                    this.ClientSize = new Size(this.CurrentSettings.FormWidth.Value, this.CurrentSettings.FormHeight.Value);
                }
                if (this.CurrentSettings.FormLocationX.HasValue && IsOnScreen(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value, this.Width)) {
                    this.Location = new Point(this.CurrentSettings.FormLocationX.Value, this.CurrentSettings.FormLocationY.Value);
                }

#if AllowUpdate
                if (this.CurrentSettings.AutoUpdate && this.CheckForUpdate(true)) {
                    return;
                }
#endif

                this.menuProfile.DropDownItems["menuProfile" + this.CurrentSettings.SelectedProfile].PerformClick();

                this.UpdateDates();
            } catch { }
        }
        private void Stats_Shown(object sender, EventArgs e) {
            try {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                    logPath = this.CurrentSettings.LogPath;
                }
                this.logFile.Start(logPath, LOGNAME);

                this.overlay.ArrangeDisplay(this.CurrentSettings.FlippedDisplay, this.CurrentSettings.ShowOverlayTabs,this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo, this.CurrentSettings.OverlayColor, this.CurrentSettings.OverlayWidth, this.CurrentSettings.OverlayHeight, this.CurrentSettings.OverlayFontSerialized);
                if (this.CurrentSettings.OverlayVisible) {
                    ToggleOverlay(this.overlay);
                }

                this.menuAllStats.Checked = false;
                switch (this.CurrentSettings.FilterType) {
                    case 0:
                        this.menuAllStats.Checked = true;
                        this.menuStats_Click(this.menuAllStats, null);
                        break;
                    case 1:
                        this.menuSeasonStats.Checked = true;
                        this.menuStats_Click(this.menuSeasonStats, null);
                        break;
                    case 2: this.menuWeekStats.Checked = true;
                        this.menuStats_Click(this.menuWeekStats, null);
                        break;
                    case 3: this.menuDayStats.Checked = true;
                        this.menuStats_Click(this.menuDayStats, null);
                        break;
                    case 4: this.menuSessionStats.Checked = true;
                        this.menuStats_Click(this.menuSessionStats, null);
                        break;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogFile_OnError(string error) {
            if (!this.Disposing && !this.IsDisposed) {
                try {
                    if (this.InvokeRequired) {
                        Invoke((Action<string>)LogFile_OnError, error);
                    } else {
                        MessageBox.Show(this, error, $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch { }
            }
        }
        private void LogFile_OnNewLogFileDate(DateTime newDate) {
            if (SessionStart != newDate) {
                SessionStart = newDate;
                if (this.menuSessionStats.Checked) {
                    menuStats_Click(this.menuSessionStats, null);
                }
            }
        }
        private void LogFile_OnParsedLogLinesCurrent(List<RoundInfo> round) {
            lock (this.CurrentRound) {
                if (this.CurrentRound == null || this.CurrentRound.Count != round.Count) {
                    this.CurrentRound = round;
                } else {
                    for (int i = 0; i < this.CurrentRound.Count; i++) {
                        RoundInfo info = this.CurrentRound[i];
                        if (!info.Equals(round[i])) {
                            this.CurrentRound = round;
                            break;
                        }
                    }
                }
            }
        }
        private void LogFile_OnParsedLogLines(List<RoundInfo> round) {
            try {
                if (InvokeRequired) {
                    this.Invoke((Action<List<RoundInfo>>)this.LogFile_OnParsedLogLines, round);
                    return;
                }

                lock (this.StatsDB) {
                    if (!this.loadingExisting) { this.StatsDB.BeginTrans(); }

                    int profile = this.currentProfile;
                    for (int k = 0; k < round.Count; k++) {
                        RoundInfo stat = round[k];

                        if (!this.loadingExisting) {
                            RoundInfo info = null;
                            for (int i = this.AllStats.Count - 1; i >= 0; i--) {
                                RoundInfo temp = this.AllStats[i];
                                if (temp.Start == stat.Start && temp.Name == stat.Name) {
                                    info = temp;
                                    break;
                                }
                            }

                            if (info == null && stat.Start > this.lastAddedShow) {
                                if (stat.ShowEnd < this.startupTime && this.askedPreviousShows == 0) {
                                    using (EditShows editShows = new EditShows()) {
                                        editShows.Title = Multilingual.GetWord("profile_add_select_title");
                                        editShows.FunctionFlag = "add";
                                        editShows.SaveBtnName = Multilingual.GetWord("profile_apply_change_button");
                                        editShows.Icon = this.Icon;
                                        editShows.Profiles = this.AllProfiles;
                                        editShows.StatsForm = this;
                                        if (editShows.ShowDialog(this) == DialogResult.OK) {
                                            this.askedPreviousShows = 1;
                                            profile = editShows.SelectedProfileId;
                                        } else {
                                            this.askedPreviousShows = 2;
                                        }
                                    }
                                }

                                if (stat.ShowEnd < this.startupTime && this.askedPreviousShows == 2) {
                                    continue;
                                }

                                if (stat.Round == 1) {
                                    this.nextShowID++;
                                    this.lastAddedShow = stat.Start;
                                }
                                stat.ShowID = nextShowID;
                                stat.Profile = profile;
                                this.RoundDetails.Insert(stat);
                                this.AllStats.Add(stat);
                            } else {
                                continue;
                            }
                        }

                        if (!stat.PrivateLobby) {
                            if (stat.Round == 1) {
                                this.Shows++;
                            }
                            this.Rounds++;
                        }
                        this.Duration += stat.End - stat.Start;
                        this.Kudos += stat.Kudos;

                        // add new type of round to the rounds lookup
                        if (!this.StatLookup.ContainsKey(stat.Name)) {
                            string roundName = stat.Name;
                            if (roundName.StartsWith("round_", StringComparison.OrdinalIgnoreCase)) {
                                roundName = roundName.Substring(6).Replace('_', ' ');
                            }

                            LevelStats newLevel = new LevelStats(this.textInfo.ToTitleCase(roundName), LevelType.Unknown, false, 0);
                            this.StatLookup.Add(stat.Name, newLevel);
                            this.StatDetails.Add(newLevel);
                            this.gridDetails.DataSource = null;
                            this.gridDetails.DataSource = this.StatDetails;
                        }

                        stat.ToLocalTime();
                        LevelStats levelStats = this.StatLookup[stat.Name];

                        if (!stat.PrivateLobby) {
                            if (stat.IsFinal || stat.Crown) {
                                this.Finals++;
                                if (stat.Qualified) {
                                    this.Wins++;
                                }
                            }
                        }
                        levelStats.Add(stat);
                    }

                    if (!this.loadingExisting) { this.StatsDB.Commit(); }
                }

                lock (this.CurrentRound) {
                    this.CurrentRound.Clear();
                    for (int i = round.Count - 1; i >= 0; i--) {
                        RoundInfo info = round[i];
                        this.CurrentRound.Insert(0, info);
                        if (info.Round == 1) {
                            break;
                        }
                    }
                }

                if (!Disposing && !IsDisposed) {
                    try {
                        UpdateTotals();
                    } catch { }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsInStatsFilter(DateTime showEnd) {
            return this.menuAllStats.Checked ||
                   (this.menuSeasonStats.Checked && showEnd > SeasonStart) ||
                   (this.menuWeekStats.Checked && showEnd > WeekStart) ||
                   (this.menuDayStats.Checked && showEnd > DayStart) ||
                   (this.menuSessionStats.Checked && showEnd > SessionStart);
        }
        private bool IsInPartyFilter(RoundInfo info) {
            return this.menuAllPartyStats.Checked ||
                   (this.menuSoloStats.Checked && !info.InParty) ||
                   (this.menuPartyStats.Checked && info.InParty);
        }
        public string GetCurrentFilter() {
            return this.menuAllStats.Checked ? Multilingual.GetWord("main_all") : this.menuSeasonStats.Checked ? Multilingual.GetWord("main_season") : this.menuWeekStats.Checked ? Multilingual.GetWord("main_week") : this.menuDayStats.Checked ? Multilingual.GetWord("main_day") : Multilingual.GetWord("main_session");
        }
        public string GetCurrentProfile() {
            return this.menuProfile.DropDownItems["menuProfile" + this.CurrentSettings.SelectedProfile].Text;
        }
        public int GetCurrentProfileId() {
            return this.currentProfile;
        }
        public StatSummary GetLevelInfo(string name) {
            StatSummary summary = new StatSummary();
            LevelStats levelDetails;

            summary.AllWins = 0;
            summary.TotalShows = 0;
            summary.TotalPlays = 0;
            summary.TotalWins = 0;
            summary.TotalFinals = 0;
            int lastShow = -1;
            LevelStats currentLevel;
            if (!this.StatLookup.TryGetValue(name ?? string.Empty, out currentLevel)) {
                currentLevel = new LevelStats(name, LevelType.Unknown, false, 0);
            }
            int profile = currentProfile;

            for (int i = 0; i < this.AllStats.Count; i++) {
                RoundInfo info = this.AllStats[i];
                if (info.Profile != profile) { continue; }

                TimeSpan finishTime = info.Finish.GetValueOrDefault(info.End) - info.Start;
                bool hasLevelDetails = StatLookup.TryGetValue(info.Name, out levelDetails);
                bool isCurrentLevel = currentLevel.Name.Equals(hasLevelDetails ? levelDetails.Name : info.Name, StringComparison.OrdinalIgnoreCase);

                int currentShow = info.ShowID;
                RoundInfo endShow = info;
                for (int j = i + 1; j < this.AllStats.Count; j++) {
                    if (this.AllStats[j].ShowID != currentShow) {
                        break;
                    }
                    endShow = this.AllStats[j];
                }

                bool isInWinsFilter = !endShow.PrivateLobby && (this.CurrentSettings.WinsFilter == 0 ||
                    (this.CurrentSettings.WinsFilter == 1 && IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.WinsFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info)));
                bool isInQualifyFilter = !endShow.PrivateLobby && (this.CurrentSettings.QualifyFilter == 0 ||
                    (this.CurrentSettings.QualifyFilter == 1 && IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.QualifyFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info)));
                bool isInFastestFilter = this.CurrentSettings.FastestFilter == 0 ||
                    (this.CurrentSettings.FastestFilter == 1 && IsInStatsFilter(endShow.Start) && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 2 && endShow.Start > SeasonStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 3 && endShow.Start > WeekStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 4 && endShow.Start > DayStart && this.IsInPartyFilter(info)) ||
                    (this.CurrentSettings.FastestFilter == 5 && endShow.Start > SessionStart && this.IsInPartyFilter(info));

                if (info.ShowID != lastShow) {
                    lastShow = info.ShowID;
                    if (isInWinsFilter) {
                        summary.TotalShows++;
                    }
                }

                if (isCurrentLevel) {
                    if (isInQualifyFilter) {
                        summary.TotalPlays++;
                    }

                    if (isInFastestFilter) {
                        if ((!hasLevelDetails || levelDetails.Type == LevelType.Team) && info.Score.HasValue && (!summary.BestScore.HasValue || info.Score.Value > summary.BestScore.Value)) {
                            summary.BestScore = info.Score;
                        }
                    }
                }

                if (info == endShow && (levelDetails.IsFinal || info.Crown) && !endShow.PrivateLobby) {
                    if (info.IsFinal) {
                        summary.CurrentFinalStreak++;
                        if (summary.BestFinalStreak < summary.CurrentFinalStreak) {
                            summary.BestFinalStreak = summary.CurrentFinalStreak;
                        }
                    }
                }

                if (info.Qualified) {
                    if (hasLevelDetails && (info.IsFinal || info.Crown)) {
                        if (!info.PrivateLobby) {
                            summary.AllWins++;
                        }

                        if (isInWinsFilter) {
                            summary.TotalWins++;
                            summary.TotalFinals++;
                        }

                        if (!info.PrivateLobby) {
                            summary.CurrentStreak++;
                            if (summary.CurrentStreak > summary.BestStreak) {
                                summary.BestStreak = summary.CurrentStreak;
                            }
                        }
                    }

                    if (isCurrentLevel) {
                        if (isInQualifyFilter) {
                            if (info.Tier == (int)QualifyTier.Gold) {
                                summary.TotalGolds++;
                            }
                            summary.TotalQualify++;
                        }

                        if (isInFastestFilter) {
                            if (finishTime.TotalSeconds > 1.1 && (!summary.BestFinish.HasValue || summary.BestFinish.Value > finishTime)) {
                                summary.BestFinish = finishTime;
                            }
                            if (finishTime.TotalSeconds > 1.1 && info.Finish.HasValue && (!summary.LongestFinish.HasValue || summary.LongestFinish.Value < finishTime)) {
                                summary.LongestFinish = finishTime;
                            }
                        }

                        if (finishTime.TotalSeconds > 1.1 && (!summary.BestFinishOverall.HasValue || summary.BestFinishOverall.Value > finishTime)) {
                            summary.BestFinishOverall = finishTime;
                        }
                        if (finishTime.TotalSeconds > 1.1 && info.Finish.HasValue && (!summary.LongestFinishOverall.HasValue || summary.LongestFinishOverall.Value < finishTime)) {
                            summary.LongestFinishOverall = finishTime;
                        }
                    }
                } else if (!info.PrivateLobby) {
                    if (!info.IsFinal && !info.Crown) {
                        summary.CurrentFinalStreak = 0;
                    }
                    summary.CurrentStreak = 0;
                    if (isInWinsFilter && hasLevelDetails && (info.IsFinal || info.Crown)) {
                        summary.TotalFinals++;
                    }
                }
            }

            return summary;
        }
        private void ClearTotals() {
            this.Rounds = 0;
            this.Duration = TimeSpan.Zero;
            this.Wins = 0;
            this.Shows = 0;
            this.Finals = 0;
            this.Kudos = 0;
        }
        private void UpdateTotals() {
            try {
                this.lblCurrentProfile.Text = $"{GetCurrentProfile()}";
                this.lblCurrentProfile.ToolTipText = $"{Multilingual.GetWord("profile_change_tooltiptext")}";
                this.lblTotalRounds.Text = $"{Multilingual.GetWord("main_rounds")} : {this.Rounds}{Multilingual.GetWord("main_round")}";
                this.lblTotalRounds.ToolTipText = $"{Multilingual.GetWord("rounds_detail_tooltiptext")}";
                this.lblTotalShows.Text = $"{Multilingual.GetWord("main_shows")} : {this.Shows}{Multilingual.GetWord("main_inning")}";
                this.lblTotalShows.ToolTipText = $"{Multilingual.GetWord("shows_detail_tooltiptext")}";
                this.lblTotalTime.Text = $"{(int)this.Duration.TotalHours}{Multilingual.GetWord("main_hour")}{this.Duration:mm}{Multilingual.GetWord("main_min")}{this.Duration:ss}{Multilingual.GetWord("main_sec")}";
                float winChance = (float)this.Wins * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalWins.Text = $"{this.Wins}{Multilingual.GetWord("main_win")} ({winChance:0.0} %)";
                this.lblTotalWins.ToolTipText = $"{Multilingual.GetWord("wins_detail_tooltiptext")}";
                float finalChance = (float)this.Finals * 100 / (this.Shows == 0 ? 1 : this.Shows);
                this.lblTotalFinals.Text = $"{this.Finals}{Multilingual.GetWord("main_inning")} ({finalChance:0.0} %)";
                this.lblTotalFinals.ToolTipText = $"{Multilingual.GetWord("finals_detail_tooltiptext")}";
                this.lblKudos.Text = $"{this.Kudos}";
                this.gridDetails.Refresh();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_DataSourceChanged(object sender, EventArgs e) {
            try {
                if (this.gridDetails.Columns.Count == 0) { return; }
                int pos = 0;
                this.gridDetails.Columns["AveKudos"].Visible = false;
                this.gridDetails.Columns["AveDuration"].Visible = false;
                this.gridDetails.Setup("Name",      pos++, 0, Multilingual.GetWord("main_round_name"), DataGridViewContentAlignment.MiddleLeft);
                this.gridDetails.Setup("Played",    pos++, CurrentLanguage <= 1 ? 63 : CurrentLanguage == 2 ? 58 : 81, Multilingual.GetWord("main_played"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Qualified", pos++, CurrentLanguage <= 1 ? 66 : CurrentLanguage == 2 ? 55 : 51, Multilingual.GetWord("main_qualified"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Gold",      pos++, CurrentLanguage <= 1 ? 53 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_gold"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Silver",    pos++, CurrentLanguage <= 1 ? 58 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_silver"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Bronze",    pos++, CurrentLanguage <= 1 ? 65 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_bronze"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Kudos",     pos++, CurrentLanguage <= 1 ? 60 : CurrentLanguage == 2 ? 58 : 60, Multilingual.GetWord("main_kudos"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Fastest",   pos++, CurrentLanguage <= 1 ? 67 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_fastest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Longest",   pos++, CurrentLanguage <= 1 ? 69 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_longest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("AveFinish", pos,   CurrentLanguage <= 1 ? 70 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_ave_finish"), DataGridViewContentAlignment.MiddleRight);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }

                LevelStats info = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;

                switch (this.gridDetails.Columns[e.ColumnIndex].Name) {
                    case "Name":
                        this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = Multilingual.GetWord("level_detail_tooltiptext");
                        if (info.IsFinal) {
                            e.CellStyle.BackColor = Color.FromArgb(255,230,138);
                            break;
                        }
                        switch (info.Type) {
                            case LevelType.Race: e.CellStyle.BackColor = Color.FromArgb(206,255,228); break;
                            case LevelType.Survival: e.CellStyle.BackColor = Color.FromArgb(244,206,250); break;
                            case LevelType.Team: e.CellStyle.BackColor = Color.FromArgb(255,238,230); break;
                            case LevelType.Hunt: e.CellStyle.BackColor = Color.FromArgb(208,222,244); break;
                            case LevelType.Unknown: e.CellStyle.BackColor = Color.LightGray; break;
                        }
                        break;
                    case "Qualified": {
                            float qualifyChance = info.Qualified * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Qualified}";
                            } else {
                                e.Value = info.Qualified;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Gold": {
                            float qualifyChance = info.Gold * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Gold}";
                            } else {
                                e.Value = info.Gold;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Silver": {
                            float qualifyChance = info.Silver * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Silver}";
                            } else {
                                e.Value = info.Silver;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "Bronze": {
                            float qualifyChance = info.Bronze * 100f / (info.Played == 0 ? 1 : info.Played);
                            if (this.CurrentSettings.ShowPercentages) {
                                e.Value = $"{qualifyChance:0.0}%";
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{info.Bronze}";
                            } else {
                                e.Value = info.Bronze;
                                this.gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = $"{qualifyChance:0.0}%";
                            }
                            break;
                        }
                    case "AveFinish":
                        e.Value = info.AveFinish.ToString("m\\:ss\\.ff");
                        break;
                    case "Fastest":
                        e.Value = info.Fastest.ToString("m\\:ss\\.ff");
                        break;
                    case "Longest":
                        e.Value = info.Longest.ToString("m\\:ss\\.ff");
                        break;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex >= 0 && this.gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                    this.gridDetails.Cursor = Cursors.Hand;
                } else {
                    this.gridDetails.Cursor = Cursors.Default;
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_CellClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if (e.RowIndex < 0) { return; }
                if (this.gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                    LevelStats stats = this.gridDetails.Rows[e.RowIndex].DataBoundItem as LevelStats;
                    using (LevelDetails levelDetails = new LevelDetails()) {
                        levelDetails.LevelName = stats.Name;
                        List<RoundInfo> rounds = stats.Stats;
                        rounds.Sort();
                        levelDetails.RoundDetails = rounds;
                        levelDetails.StatsForm = this;
                        levelDetails.ShowDialog(this);
                    }
                } else {
                    ToggleWinPercentageDisplay();
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = this.gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = this.gridDetails.GetSortOrder(columnName);

            this.StatDetails.Sort(delegate (LevelStats one, LevelStats two) {
                LevelType oneType = one.IsFinal ? LevelType.Final : one.Type;
                LevelType twoType = two.IsFinal ? LevelType.Final : two.Type;

                int typeCompare = this.CurrentSettings.IgnoreLevelTypeWhenSorting && sortOrder != SortOrder.None ? 0 : ((int)oneType).CompareTo((int)twoType);

                if (sortOrder == SortOrder.Descending) {
                    LevelStats temp = one;
                    one = two;
                    two = temp;
                }

                int nameCompare = one.Name.CompareTo(two.Name);
                bool percents = this.CurrentSettings.ShowPercentages;
                if (typeCompare == 0 && sortOrder != SortOrder.None) {
                    switch (columnName) {
                        case "Gold": typeCompare = ((double)one.Gold / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Gold / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Silver": typeCompare = ((double)one.Silver / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Silver / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Bronze": typeCompare = ((double)one.Bronze / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Bronze / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Played": typeCompare = one.Played.CompareTo(two.Played); break;
                        case "Qualified": typeCompare = ((double)one.Qualified / (one.Played > 0 && percents ? one.Played : 1)).CompareTo((double)two.Qualified / (two.Played > 0 && percents ? two.Played : 1)); break;
                        case "Kudos": typeCompare = one.Kudos.CompareTo(two.Kudos); break;
                        case "AveKudos": typeCompare = one.AveKudos.CompareTo(two.AveKudos); break;
                        case "AveFinish": typeCompare = one.AveFinish.CompareTo(two.AveFinish); break;
                        case "Fastest": typeCompare = one.Fastest.CompareTo(two.Fastest); break;
                        case "Longest": typeCompare = one.Longest.CompareTo(two.Longest); break;
                        default: typeCompare = nameCompare; break;
                    }
                }

                if (typeCompare == 0) {
                    typeCompare = nameCompare;
                }

                return typeCompare;
            });

            this.gridDetails.DataSource = null;
            this.gridDetails.DataSource = this.StatDetails;
            this.gridDetails.Columns[columnName].HeaderCell.SortGlyphDirection = sortOrder;
        }
        private void gridDetails_SelectionChanged(object sender, EventArgs e) {
            if (this.gridDetails.SelectedCells.Count > 0) {
                this.gridDetails.ClearSelection();
            }
        }
        private void ToggleWinPercentageDisplay() {
            this.CurrentSettings.ShowPercentages = !this.CurrentSettings.ShowPercentages;
            SaveUserSettings();
            this.gridDetails.Invalidate();
        }
        private void ShowShows() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Shows";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }

                rounds.Sort();

                List<RoundInfo> shows = new List<RoundInfo>();
                int roundCount = 0;
                int kudosTotal = 0;
                bool won = false;
                bool isFinal = false;
                DateTime endDate = DateTime.MinValue;

                for (int i = rounds.Count - 1; i >= 0; i--) {
                    RoundInfo info = rounds[i];
                    if (roundCount == 0) {
                        endDate = info.End;
                        won = info.Qualified;
                        isFinal = info.IsFinal || info.Crown;
                    }

                    roundCount++;
                    kudosTotal += info.Kudos;
                    if (info.Round == 1) {
                        shows.Insert(0,
                            new RoundInfo {
                                Name = isFinal ? "Final" : string.Empty, ShowNameId = info.ShowNameId, IsFinal = isFinal, End = endDate,
                                Start = info.Start, StartLocal = info.StartLocal, Kudos = kudosTotal,
                                Qualified = won, Round = roundCount, ShowID = info.ShowID, Tier = won ? 1 : 0
                            });
                        roundCount = 0;
                        kudosTotal = 0;
                    }
                }

                levelDetails.RoundDetails = shows;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowRounds() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Rounds";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }
                rounds.Sort();
                levelDetails.RoundDetails = rounds;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowFinals() {
            using (LevelDetails levelDetails = new LevelDetails()) {
                levelDetails.LevelName = "Finals";
                List<RoundInfo> rounds = new List<RoundInfo>();
                for (int i = 0; i < StatDetails.Count; i++) {
                    rounds.AddRange(StatDetails[i].Stats);
                }

                rounds.Sort();

                int keepShow = -1;
                for (int i = rounds.Count - 1; i >= 0; i--) {
                    RoundInfo info = rounds[i];
                    if (info.ShowID != keepShow && (info.Crown || info.IsFinal)) {
                        keepShow = info.ShowID;
                    } else if (info.ShowID != keepShow) {
                        rounds.RemoveAt(i);
                    }
                }

                levelDetails.RoundDetails = rounds;
                levelDetails.StatsForm = this;
                levelDetails.ShowDialog(this);
            }
        }
        private void ShowWinGraph() {
            List<RoundInfo> rounds = new List<RoundInfo>();
            for (int i = 0; i < StatDetails.Count; i++) {
                rounds.AddRange(StatDetails[i].Stats);
            }

            rounds.Sort();

            using (StatsDisplay display = new StatsDisplay()
                       { Text = Multilingual.GetWord("level_detail_wins_per_day") }) {
                DataTable dt = new DataTable();
                dt.Columns.Add(Multilingual.GetWord("level_detail_date"), typeof(DateTime));
                dt.Columns.Add(Multilingual.GetWord("level_detail_wins"), typeof(int));
                dt.Columns.Add(Multilingual.GetWord("level_detail_finals"), typeof(int));
                dt.Columns.Add(Multilingual.GetWord("level_detail_shows"), typeof(int));

                if (rounds.Count > 0) {
                    DateTime start = rounds[0].StartLocal;
                    int currentWins = 0;
                    int currentFinals = 0;
                    int currentShows = 0;
                    for (int i = 0; i < rounds.Count; i++) {
                        RoundInfo info = rounds[i];
                        if (info.PrivateLobby) { continue; }

                        if (info.Round == 1) {
                            currentShows++;
                        }

                        if (info.Crown || info.IsFinal) {
                            currentFinals++;
                            if (info.Qualified) {
                                currentWins++;
                            }
                        }

                        if (info.StartLocal.Date != start.Date) {
                            dt.Rows.Add(start.Date, currentWins, currentFinals, currentShows);

                            int missingCount = (int)(info.StartLocal.Date - start.Date).TotalDays;
                            while (missingCount > 1) {
                                missingCount--;
                                start = start.Date.AddDays(1);
                                dt.Rows.Add(start, 0, 0, 0);
                            }

                            currentWins = 0;
                            currentFinals = 0;
                            currentShows = 0;
                            start = info.StartLocal;
                        }
                    }

                    dt.Rows.Add(start.Date, currentWins, currentFinals, currentShows);
                } else {
                    dt.Rows.Add(DateTime.Now.Date, 0, 0, 0);
                }

                display.Details = dt;
                display.ShowDialog(this);
            }
        }
        private void LaunchHelpInBrowser() {
            try {
                Process.Start(@"https://github.com/ShootMe/FallGuysStats");
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LaunchGame(bool ignoreExisting) {
            try {
                if (CurrentSettings.LaunchPlatform == 0) {
                    if (!string.IsNullOrEmpty(this.CurrentSettings.GameShortcutLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = "FallGuys_client_game";
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MessageBox.Show(this, Multilingual.GetWord("message_already_running"), Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
                        }

                        if (MessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}", Multilingual.GetWord("message_execution_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameShortcutLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MessageBox.Show(this, Multilingual.GetWord("message_register_shortcut"), Multilingual.GetWord("message_register_shortcut_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    this.UpdateGameExeLocation();
                    if (!string.IsNullOrEmpty(CurrentSettings.GameExeLocation) && File.Exists(CurrentSettings.GameExeLocation)) {
                        Process[] processes = Process.GetProcesses();
                        string fallGuysProcessName = Path.GetFileNameWithoutExtension(CurrentSettings.GameExeLocation);
                        for (int i = 0; i < processes.Length; i++) {
                            string name = processes[i].ProcessName;
                            if (name.IndexOf(fallGuysProcessName, StringComparison.OrdinalIgnoreCase) >= 0) {
                                if (!ignoreExisting) {
                                    MessageBox.Show(this, Multilingual.GetWord("message_already_running"), Multilingual.GetWord("message_already_running_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                return;
                            }
                        }

                        if (MessageBox.Show(this, $"{Multilingual.GetWord("message_execution_question")}", Multilingual.GetWord("message_execution_caption"),
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Process.Start(this.CurrentSettings.GameExeLocation);
                            this.WindowState = FormWindowState.Minimized;
                        }
                    } else {
                        MessageBox.Show(this, Multilingual.GetWord("message_register_exe"), Multilingual.GetWord("message_register_exe_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void UpdateGameExeLocation() {
            if (string.IsNullOrEmpty(this.CurrentSettings.GameExeLocation)) {
                string fallGuysExeLocation = this.FindGameExeLocation();
                if (!string.IsNullOrEmpty(fallGuysExeLocation)) {
                    this.menuLaunchFallGuys.Image = Properties.Resources.steam_main_icon;
                    this.CurrentSettings.LaunchPlatform = 1;
                    this.CurrentSettings.GameExeLocation = fallGuysExeLocation;
                    this.SaveUserSettings();
                }
            }
        }
        private string FindGameExeLocation() {
            try {
                // get steam install folder
                object regValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
                string steamPath = (string)regValue;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    string userName = Environment.UserName;
                    steamPath = Path.Combine("/", "home", userName, ".local", "share", "Steam");
                }

                string fallGuys = Path.Combine(steamPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                if (File.Exists(fallGuys)) {
                    return fallGuys;
                }
                // read libraryfolders.vdf from install folder to get games installation folder
                // note: this parsing is terrible, but does technically work fine. There's a better way by specifying a schema and
                // fully parsing the file or something like that. This is quick and dirty, for sure.
                FileInfo libraryFoldersFile = new FileInfo(Path.Combine(steamPath, "steamapps", "libraryfolders.vdf"));
                if (libraryFoldersFile.Exists) {
                    JsonClass json = Json.Read(File.ReadAllText(libraryFoldersFile.FullName)) as JsonClass;
                    foreach (JsonObject obj in json) {
                        if (obj is JsonClass library) {
                            string libraryPath = library["path"].AsString();
                            if (!string.IsNullOrEmpty(libraryPath)) {
                                // look for exe in standard location under library
                                fallGuys = Path.Combine(libraryPath, "steamapps", "common", "Fall Guys", "FallGuys_client_game.exe");
                                if (File.Exists(fallGuys)) {
                                    return fallGuys;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return string.Empty;
        }
        private void lblCurrentProfile_Click(object sender, EventArgs e) {
            for (var i = 0; i < this.ProfileMenuItems.Count; i++) {
                ToolStripItem item = this.ProfileMenuItems[i];
                if (!(item is ToolStripMenuItem menuItem)) { continue; }

                if (menuItem.Checked && i + 1 < this.ProfileMenuItems.Count) {
                    this.ProfileMenuItems[i + 1].PerformClick();
                    break;
                } else if (menuItem.Checked && i + 1 >= this.ProfileMenuItems.Count) {
                    this.ProfileMenuItems[0].PerformClick();
                    break;
                }
            }
        }
        private void lblTotalFinals_Click(object sender, EventArgs e) {
            try {
                this.ShowFinals();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalShows_Click(object sender, EventArgs e) {
            try {
                this.ShowShows();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalRounds_Click(object sender, EventArgs e) {
            try {
                this.ShowRounds();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lblTotalWins_Click(object sender, EventArgs e) {
            try {
                this.ShowWinGraph();
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuStats_Click(object sender, EventArgs e) {
            try {
                ToolStripMenuItem button = sender as ToolStripMenuItem;
                if (button == this.menuAllStats || button == this.menuSeasonStats || button == this.menuWeekStats || button == this.menuDayStats || button == this.menuSessionStats) {
                    if (!this.menuAllStats.Checked && !this.menuSeasonStats.Checked && !this.menuWeekStats.Checked && !this.menuDayStats.Checked && !this.menuSessionStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in this.menuStatsFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }
                }

                if (button == this.menuAllPartyStats || button == this.menuSoloStats || button == this.menuPartyStats) {
                    if (!this.menuAllPartyStats.Checked && !this.menuSoloStats.Checked && !this.menuPartyStats.Checked) {
                        button.Checked = true;
                        return;
                    }

                    foreach (ToolStripItem item in menuPartyFilter.DropDownItems) {
                        if (item is ToolStripMenuItem menuItem && menuItem.Checked && menuItem != button) {
                            menuItem.Checked = false;
                        }
                    }

                    button = this.menuAllStats.Checked ? this.menuAllStats : this.menuSeasonStats.Checked ? this.menuSeasonStats : this.menuWeekStats.Checked ? this.menuWeekStats : this.menuDayStats.Checked ? this.menuDayStats : this.menuSessionStats;
                }

                if(ProfileMenuItems.Contains(button)) {
                    for (int i = ProfileMenuItems.Count - 1; i >= 0; i--) {
                        ProfileMenuItems[i].Checked = ProfileMenuItems[i].Name == button.Name;
                    }
                    currentProfile = Int32.Parse(button.Name.Substring(11));
                    button = this.menuAllStats.Checked ? this.menuAllStats : this.menuSeasonStats.Checked ? this.menuSeasonStats : this.menuWeekStats.Checked ? this.menuWeekStats : this.menuDayStats.Checked ? this.menuDayStats : this.menuSessionStats;
                }

                for (int i = 0; i < this.StatDetails.Count; i++) {
                    LevelStats calculator = this.StatDetails[i];
                    calculator.Clear();
                }

                ClearTotals();

                int profile = this.currentProfile;
                bool soloOnly = this.menuSoloStats.Checked;
                List<RoundInfo> rounds = new List<RoundInfo>();

                DateTime compareDate = this.menuAllStats.Checked ? DateTime.MinValue : this.menuSeasonStats.Checked ? SeasonStart : this.menuWeekStats.Checked ? WeekStart : this.menuDayStats.Checked ? DayStart : SessionStart;
                for (int i = 0; i < this.AllStats.Count; i++) {
                    RoundInfo round = this.AllStats[i];
                    if (round.Start > compareDate && round.Profile == profile && (this.menuAllPartyStats.Checked || round.InParty == soloOnly)) {
                        rounds.Add(round);
                    }
                }

                rounds.Sort();

                if (rounds.Count > 0 && (button == this.menuWeekStats || button == this.menuDayStats || button == this.menuSessionStats)) {
                    int minShowID = rounds[0].ShowID;

                    for (int i = 0; i < this.AllStats.Count; i++) {
                        RoundInfo round = this.AllStats[i];
                        if (round.ShowID == minShowID && round.Start <= compareDate) {
                            rounds.Add(round);
                        }
                    }
                }

                rounds.Sort();

                this.CurrentSettings.SelectedProfile = profile;
                this.CurrentSettings.FilterType = menuAllStats.Checked ? 0 : menuSeasonStats.Checked ? 1 : menuWeekStats.Checked ? 2 : menuDayStats.Checked ? 3 : 4;
                this.SaveUserSettings();

                this.loadingExisting = true;
                this.LogFile_OnParsedLogLines(rounds);
                this.loadingExisting = false;
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuUpdate_Click(object sender, EventArgs e) {
            try {
                this.CheckForUpdate(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_update_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool CheckForUpdate(bool silent) {
#if AllowUpdate
            using (ZipWebClient web = new ZipWebClient()) {
                string assemblyInfo = web.DownloadString(@"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/AssemblyInfo.cs");

                int index = assemblyInfo.IndexOf("AssemblyVersion(");
                if (index > 0) {
                    int indexEnd = assemblyInfo.IndexOf("\")", index);
                    Version newVersion = new Version(assemblyInfo.Substring(index + 17, indexEnd - index - 17));
                    if (newVersion > Assembly.GetEntryAssembly().GetName().Version) {
                        if (silent || MessageBox.Show(this, $"{Multilingual.GetWord("message_update_question_prefix")} (v{newVersion.ToString(2)}) {Multilingual.GetWord("message_update_question_suffix")}", $"{Multilingual.GetWord("message_update_question_caption")}", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                            byte[] data = web.DownloadData($"https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStats.zip");
                            string exeName = null;
                            using (MemoryStream ms = new MemoryStream(data)) {
                                using (ZipArchive zipFile = new ZipArchive(ms, ZipArchiveMode.Read)) {
                                    foreach (var entry in zipFile.Entries) {
                                        if (entry.Name.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) > 0) {
                                            exeName = entry.Name;
                                        }
                                        File.Move(entry.Name, $"{entry.Name}.bak");
                                        entry.ExtractToFile(entry.Name, true);
                                    }
                                }
                            }

                            Process.Start(new ProcessStartInfo(exeName));
                            Visible = false;
                            Close();
                            return true;
                        }
                    } else if (!silent) {
                        MessageBox.Show(this, $"{Multilingual.GetWord("message_update_latest_version")}", $"{Multilingual.GetWord("message_update_question_caption")}", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                } else if (!silent) {
                    MessageBox.Show(this, $"{Multilingual.GetWord("message_update_not_determine_version")}", $"{Multilingual.GetWord("message_update_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
#else
            this.LaunchHelpInBrowser();
#endif
            return false;
        }
        private async void menuSettings_Click(object sender, EventArgs e) {
            try {
                using (Settings settings = new Settings()) {
                    settings.Icon = this.Icon;
                    settings.CurrentSettings = this.CurrentSettings;
                    settings.StatsForm = this;
                    string lastLogPath = this.CurrentSettings.LogPath;
                    if (settings.ShowDialog(this) == DialogResult.OK) {
                        this.CurrentSettings = settings.CurrentSettings;
                        this.SaveUserSettings();
                        this.ChangeMainLanguage();
                        this.gridDetails.ChangeContextMenuLanguage();
                        this.overlay.ChangeLanguage();
                        this.UpdateTotals();
                        this.UpdateGridRoundName();
                        this.UpdateHoopsieLegends();

                        if (string.IsNullOrEmpty(lastLogPath) != string.IsNullOrEmpty(this.CurrentSettings.LogPath) || (!string.IsNullOrEmpty(lastLogPath) && lastLogPath.Equals(this.CurrentSettings.LogPath, StringComparison.OrdinalIgnoreCase))) {
                            await this.logFile.Stop();

                            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Mediatonic", "FallGuys_client");
                            if (!string.IsNullOrEmpty(this.CurrentSettings.LogPath)) {
                                logPath = this.CurrentSettings.LogPath;
                            }
                            this.logFile.Start(logPath, LOGNAME);
                        }
                        
                        overlay.ArrangeDisplay(this.CurrentSettings.FlippedDisplay, this.CurrentSettings.ShowOverlayTabs, this.CurrentSettings.HideWinsInfo, this.CurrentSettings.HideRoundInfo, this.CurrentSettings.HideTimeInfo, this.CurrentSettings.OverlayColor, this.CurrentSettings.OverlayWidth, this.CurrentSettings.OverlayHeight, this.CurrentSettings.OverlayFontSerialized);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuOverlay_Click(object sender, EventArgs e) {
            ToggleOverlay(overlay);
        }
        private void ToggleOverlay(Overlay overlay) {
            if (overlay.Visible) {
                overlay.Hide();
                this.menuOverlay.Image = Properties.Resources.stat_gray_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
                this.CurrentSettings.OverlayLocationX = overlay.Location.X;
                this.CurrentSettings.OverlayLocationY = overlay.Location.Y;
                this.CurrentSettings.OverlayWidth = overlay.Width;
                this.CurrentSettings.OverlayHeight = overlay.Height;
                this.CurrentSettings.OverlayVisible = false;
                this.SaveUserSettings();
            } else {
                overlay.TopMost = !this.CurrentSettings.OverlayNotOnTop;
                overlay.Show();
                this.menuOverlay.Image = Properties.Resources.stat_icon;
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
                this.CurrentSettings.OverlayVisible = true;
                this.SaveUserSettings();

                if (this.CurrentSettings.OverlayLocationX.HasValue && this.IsOnScreen(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value, overlay.Width)) {
                    overlay.Location = new Point(this.CurrentSettings.OverlayLocationX.Value, this.CurrentSettings.OverlayLocationY.Value);
                } else {
                    overlay.Location = this.Location;
                }
            }
        }
        private void menuHelp_Click(object sender, EventArgs e) {
            this.LaunchHelpInBrowser();
        }
        private void infoStrip_MouseEnter(object sender, EventArgs e) {
            this.Cursor = Cursors.Hand;
            if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripLabel") {
                var lblInfo = sender as ToolStripLabel;
                if (lblInfo != null) {
                    this.infoStripForeColor = lblInfo.ForeColor;
                    if (lblInfo.Name == "lblCurrentProfile") {
                        lblInfo.ForeColor = Color.FromArgb(245, 154, 168);
                    } else {
                        lblInfo.ForeColor = Color.FromArgb(147, 174, 248);
                    }
                }
            }
        }
        private void infoStrip_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
            if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripLabel") {
                var lblInfo = sender as ToolStripLabel;
                if (lblInfo != null) {
                    lblInfo.ForeColor = this.infoStripForeColor;
                }
            }
        }
        private void menuEditProfiles_Click(object sender, EventArgs e) {
            try {
                using (EditProfiles editProfiles = new EditProfiles()) {
                    editProfiles.Icon = this.Icon;
                    editProfiles.StatsForm = this;
                    editProfiles.Profiles = this.AllProfiles;
                    editProfiles.AllStats = this.RoundDetails.FindAll().ToList();
                    editProfiles.ShowDialog(this);
                    lock (this.StatsDB) {
                        this.StatsDB.BeginTrans();
                        this.AllProfiles = editProfiles.Profiles;
                        this.Profiles.DeleteAll();
                        this.Profiles.InsertBulk(this.AllProfiles);
                        this.AllStats = editProfiles.AllStats;
                        this.RoundDetails.DeleteAll();
                        this.RoundDetails.InsertBulk(this.AllStats);
                        this.StatsDB.Commit();
                    }
                    this.ReloadProfileMenuItems();
                }
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void menuLaunchFallGuys_Click(object sender, EventArgs e) {
            try {
                this.LaunchGame(false);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.ToString(), $"{Multilingual.GetWord("message_program_error_caption")}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool IsOnScreen(int x, int y, int w) {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens) {
                if (screen.WorkingArea.Contains(new Point(x, y)) || screen.WorkingArea.Contains(new Point(x + w, y))) {
                    return true;
                }
            }

            return false;
        }
        private void ChangeMainLanguage() {
            this.menu.Font = new Font(Overlay.DefaultFontCollection.Families[0], 9, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.menuLaunchFallGuys.Font = new Font(Overlay.DefaultFontCollection.Families[0], 12, FontStyle.Bold, GraphicsUnit.Pixel, ((byte)(0)));
            this.infoStrip.Font = new Font(Overlay.DefaultFontCollection.Families[0], 12, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
            
            this.dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.BackColor = Color.LightGray;
            this.dataGridViewCellStyle1.Font = new Font(Overlay.DefaultFontCollection.Families[0], 10, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
            this.dataGridViewCellStyle1.ForeColor = Color.Black;
            this.dataGridViewCellStyle1.SelectionBackColor = Color.Cyan;
            this.dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.gridDetails.ColumnHeadersDefaultCellStyle = this.dataGridViewCellStyle1;
            
            this.dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.BackColor = Color.White;
            this.dataGridViewCellStyle2.Font = new Font(Overlay.DefaultFontCollection.Families[0], 12, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
            this.dataGridViewCellStyle2.ForeColor = Color.Black;
            this.dataGridViewCellStyle2.SelectionBackColor = Color.DeepSkyBlue;
            this.dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            this.dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.gridDetails.DefaultCellStyle = this.dataGridViewCellStyle2;

            if (this.gridDetails.Columns.Count != 0) {
                int pos = 0;
                this.gridDetails.Columns["AveKudos"].Visible = false;
                this.gridDetails.Columns["AveDuration"].Visible = false;
                this.gridDetails.Setup("Name",      pos++, 0, Multilingual.GetWord("main_round_name"), DataGridViewContentAlignment.MiddleLeft);
                this.gridDetails.Setup("Played",    pos++, CurrentLanguage <= 1 ? 63 : CurrentLanguage == 2 ? 58 : 81, Multilingual.GetWord("main_played"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Qualified", pos++, CurrentLanguage <= 1 ? 66 : CurrentLanguage == 2 ? 55 : 51, Multilingual.GetWord("main_qualified"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Gold",      pos++, CurrentLanguage <= 1 ? 53 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_gold"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Silver",    pos++, CurrentLanguage <= 1 ? 58 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_silver"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Bronze",    pos++, CurrentLanguage <= 1 ? 65 : CurrentLanguage == 2 ? 58 : 71, Multilingual.GetWord("main_bronze"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Kudos",     pos++, CurrentLanguage <= 1 ? 60 : CurrentLanguage == 2 ? 58 : 60, Multilingual.GetWord("main_kudos"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Fastest",   pos++, CurrentLanguage <= 1 ? 67 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_fastest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("Longest",   pos++, CurrentLanguage <= 1 ? 69 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_longest"), DataGridViewContentAlignment.MiddleRight);
                this.gridDetails.Setup("AveFinish", pos,   CurrentLanguage <= 1 ? 70 : CurrentLanguage == 2 ? 74 : 71, Multilingual.GetWord("main_ave_finish"), DataGridViewContentAlignment.MiddleRight);
            }
            this.menuSettings.Text = $"{Multilingual.GetWord("main_settings")}";
            this.menuFilters.Text = $"{Multilingual.GetWord("main_filters")}";
            this.menuStatsFilter.Text = Multilingual.GetWord("main_stats");
            this.menuAllStats.Text = Multilingual.GetWord("main_all");
            this.menuSeasonStats.Text = Multilingual.GetWord("main_season");
            this.menuWeekStats.Text = Multilingual.GetWord("main_week");
            this.menuDayStats.Text = Multilingual.GetWord("main_day");
            this.menuSessionStats.Text = Multilingual.GetWord("main_session");
            this.menuPartyFilter.Text = Multilingual.GetWord("main_party_type");
            this.menuAllPartyStats.Text = Multilingual.GetWord("main_all");
            this.menuSoloStats.Text = Multilingual.GetWord("main_solo");
            this.menuPartyStats.Text = Multilingual.GetWord("main_party");
            this.menuProfile.Text = $"{Multilingual.GetWord("main_profile")}";
            this.menuEditProfiles.Text = $"{Multilingual.GetWord("main_profile_setting")}";
            if (!CurrentSettings.OverlayVisible) {
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_show_overlay")}";
            } else {
                this.menuOverlay.Text = $"{Multilingual.GetWord("main_hide_overlay")}";
            }
            this.menuUpdate.Text = $"{Multilingual.GetWord("main_update")}";
            this.menuHelp.Text = $"{Multilingual.GetWord("main_help")}";
            this.menuLaunchFallGuys.Text = $"{Multilingual.GetWord("main_launch_fall_guys")}";
            //this.menuLaunchFallGuys.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            this.menuLaunchFallGuys.Image = this.CurrentSettings.LaunchPlatform == 0 ? Properties.Resources.epic_main_icon : Properties.Resources.steam_main_icon;
        }
    }
}