namespace FallGuysStats {
    sealed partial class Overlay {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picPositionNE = new System.Windows.Forms.PictureBox();
            this.picPositionNW = new System.Windows.Forms.PictureBox();
            this.picPositionSE = new System.Windows.Forms.PictureBox();
            this.picPositionSW = new System.Windows.Forms.PictureBox();
            this.picPositionLock = new System.Windows.Forms.PictureBox();
            this.lblFilter = new FallGuysStats.TransparentLabel();
            this.lblProfile = new FallGuysStats.TransparentLabel();
            this.lblStreak = new FallGuysStats.TransparentLabel();
            this.lblFinals = new FallGuysStats.TransparentLabel();
            this.lblQualifyChance = new FallGuysStats.TransparentLabel();
            this.lblFastest = new FallGuysStats.TransparentLabel();
            this.lblDuration = new FallGuysStats.TransparentLabel();
            this.lblPingIcon = new FallGuysStats.TransparentLabel();
            this.lblCountryIcon = new FallGuysStats.TransparentLabel();
            this.lblPlayers = new FallGuysStats.TransparentLabel();
            this.lblPlayersPc = new FallGuysStats.TransparentLabel();
            this.lblPlayersPs = new FallGuysStats.TransparentLabel();
            this.lblPlayersXbox = new FallGuysStats.TransparentLabel();
            this.lblPlayersSwitch = new FallGuysStats.TransparentLabel();
            this.lblRound = new FallGuysStats.TransparentLabel();
            this.lblWins = new FallGuysStats.TransparentLabel();
            this.lblFinish = new FallGuysStats.TransparentLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionNE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionNW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionSE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionSW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionLock)).BeginInit();
            this.SuspendLayout();
            // 
            // picPositionNE
            // 
            this.picPositionNE.BackColor = System.Drawing.Color.Transparent;
            this.picPositionNE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPositionNE.Image = Properties.Resources.position_ne_off_icon;
            this.picPositionNE.Location = new System.Drawing.Point(201, 46);
            this.picPositionNE.Name = "picPositionNE";
            this.picPositionNE.Size = new System.Drawing.Size(47, 30);
            this.picPositionNE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPositionNE.TabStop = false;
            this.picPositionNE.Visible = false;
            this.picPositionNE.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Position_MouseClick);
            this.picPositionNE.MouseEnter += new System.EventHandler(this.Position_MouseEnter);
            this.picPositionNE.MouseLeave += new System.EventHandler(this.Position_MouseLeave);
            // 
            // picPositionNW
            // 
            this.picPositionNW.BackColor = System.Drawing.Color.Transparent;
            this.picPositionNW.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPositionNW.Image = Properties.Resources.position_nw_off_icon;
            this.picPositionNW.Location = new System.Drawing.Point(251, 46);
            this.picPositionNW.Name = "picPositionNW";
            this.picPositionNW.Size = new System.Drawing.Size(47, 30);
            this.picPositionNW.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPositionNW.TabStop = false;
            this.picPositionNW.Visible = false;
            this.picPositionNW.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Position_MouseClick);
            this.picPositionNW.MouseEnter += new System.EventHandler(this.Position_MouseEnter);
            this.picPositionNW.MouseLeave += new System.EventHandler(this.Position_MouseLeave);
            // 
            // picPositionSE
            // 
            this.picPositionSE.BackColor = System.Drawing.Color.Transparent;
            this.picPositionSE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPositionSE.Image = Properties.Resources.position_se_off_icon;
            this.picPositionSE.Location = new System.Drawing.Point(201, 80);
            this.picPositionSE.Name = "picPositionSE";
            this.picPositionSE.Size = new System.Drawing.Size(47, 30);
            this.picPositionSE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPositionSE.TabStop = false;
            this.picPositionSE.Visible = false;
            this.picPositionSE.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Position_MouseClick);
            this.picPositionSE.MouseEnter += new System.EventHandler(this.Position_MouseEnter);
            this.picPositionSE.MouseLeave += new System.EventHandler(this.Position_MouseLeave);
            // 
            // picPositionSW
            // 
            this.picPositionSW.BackColor = System.Drawing.Color.Transparent;
            this.picPositionSW.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPositionSW.Image = Properties.Resources.position_sw_off_icon;
            this.picPositionSW.Location = new System.Drawing.Point(251, 80);
            this.picPositionSW.Name = "picPositionSW";
            this.picPositionSW.Size = new System.Drawing.Size(47, 30);
            this.picPositionSW.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPositionSW.TabStop = false;
            this.picPositionSW.Visible = false;
            this.picPositionSW.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Position_MouseClick);
            this.picPositionSW.MouseEnter += new System.EventHandler(this.Position_MouseEnter);
            this.picPositionSW.MouseLeave += new System.EventHandler(this.Position_MouseLeave);
            // 
            // picSwitchPositionLock
            // 
            this.picPositionLock.BackColor = System.Drawing.Color.Transparent;
            this.picPositionLock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPositionLock.Image = Properties.Resources.switch_unlock_icon;
            this.picPositionLock.Location = new System.Drawing.Point(12, 7);
            this.picPositionLock.Name = "picPositionLock";
            this.picPositionLock.Size = new System.Drawing.Size(47, 30);
            this.picPositionLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPositionLock.TabStop = false;
            this.picPositionLock.Visible = false;
            this.picPositionLock.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Position_MouseClick);
            this.picPositionLock.MouseEnter += new System.EventHandler(this.Position_MouseEnter);
            this.picPositionLock.MouseLeave += new System.EventHandler(this.Position_MouseLeave);
            // 
            // lblFilter
            // 
            this.lblFilter.ImageHeight = 0;
            this.lblFilter.ImageWidth = 0;
            this.lblFilter.ImageX = 0;
            this.lblFilter.ImageY = 0;
            this.lblFilter.LevelColor = System.Drawing.Color.Empty;
            this.lblFilter.Location = new System.Drawing.Point(22, 77);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(110, 22);
            this.lblFilter.TabIndex = 22;
            this.lblFilter.Text = "Season";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFilter.TextRight = "";
            this.lblFilter.Visible = false;
            // 
            // lblProfile
            // 
            this.lblProfile.ImageHeight = 0;
            this.lblProfile.ImageWidth = 0;
            this.lblProfile.ImageX = 0;
            this.lblProfile.ImageY = 0;
            this.lblProfile.LevelColor = System.Drawing.Color.Empty;
            this.lblProfile.Location = new System.Drawing.Point(22, 27);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(600, 22);
            this.lblProfile.TabIndex = 23;
            this.lblProfile.Text = "Solo";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProfile.TextRight = "";
            this.lblProfile.Visible = false;
            // 
            // lblStreak
            // 
            this.lblStreak.ImageHeight = 0;
            this.lblStreak.ImageWidth = 0;
            this.lblStreak.ImageX = 0;
            this.lblStreak.ImageY = 0;
            this.lblStreak.LevelColor = System.Drawing.Color.Empty;
            this.lblStreak.Location = new System.Drawing.Point(22, 55);
            this.lblStreak.Name = "lblStreak";
            this.lblStreak.Size = new System.Drawing.Size(242, 22);
            this.lblStreak.TabIndex = 21;
            this.lblStreak.Text = "WIN STREAK :";
            this.lblStreak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStreak.TextRight = "0 (Best 0)";
            this.lblStreak.Visible = false;
            // 
            // lblFinals
            // 
            this.lblFinals.ImageHeight = 0;
            this.lblFinals.ImageWidth = 0;
            this.lblFinals.ImageX = 0;
            this.lblFinals.ImageY = 0;
            this.lblFinals.LevelColor = System.Drawing.Color.Empty;
            this.lblFinals.Location = new System.Drawing.Point(22, 32);
            this.lblFinals.Name = "lblFinals";
            this.lblFinals.Size = new System.Drawing.Size(242, 22);
            this.lblFinals.TabIndex = 5;
            this.lblFinals.Text = "FINALS :";
            this.lblFinals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFinals.TextRight = "0 - 0.0%";
            this.lblFinals.Visible = false;
            // 
            // lblQualifyChance
            // 
            this.lblQualifyChance.ImageHeight = 0;
            this.lblQualifyChance.ImageWidth = 0;
            this.lblQualifyChance.ImageX = 0;
            this.lblQualifyChance.ImageY = 0;
            this.lblQualifyChance.LevelColor = System.Drawing.Color.Empty;
            this.lblQualifyChance.Location = new System.Drawing.Point(270, 32);
            this.lblQualifyChance.Name = "lblQualifyChance";
            this.lblQualifyChance.Size = new System.Drawing.Size(281, 22);
            this.lblQualifyChance.TabIndex = 14;
            this.lblQualifyChance.Text = "QUALIFY :";
            this.lblQualifyChance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblQualifyChance.TextRight = "0 / 0 - 0.0%";
            this.lblQualifyChance.Visible = false;
            // 
            // lblFastest
            // 
            this.lblFastest.ImageHeight = 0;
            this.lblFastest.ImageWidth = 0;
            this.lblFastest.ImageX = 0;
            this.lblFastest.ImageY = 0;
            this.lblFastest.LevelColor = System.Drawing.Color.Empty;
            this.lblFastest.Location = new System.Drawing.Point(270, 55);
            this.lblFastest.Name = "lblFastest";
            this.lblFastest.Size = new System.Drawing.Size(281, 22);
            this.lblFastest.TabIndex = 16;
            this.lblFastest.Text = "FASTEST :";
            this.lblFastest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFastest.TextRight = "-";
            this.lblFastest.Visible = false;
            // 
            // lblDuration
            // 
            this.lblDuration.ImageHeight = 0;
            this.lblDuration.ImageWidth = 0;
            this.lblDuration.ImageX = 0;
            this.lblDuration.ImageY = 0;
            this.lblDuration.LevelColor = System.Drawing.Color.Empty;
            this.lblDuration.Location = new System.Drawing.Point(557, 32);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(225, 22);
            this.lblDuration.TabIndex = 18;
            this.lblDuration.Text = "TIME :";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDuration.TextRight = "-";
            this.lblDuration.Visible = false;
            // 
            // lblPingIcon
            // 
            this.lblPingIcon.ImageHeight = 14;
            this.lblPingIcon.ImageWidth = 20;
            this.lblPingIcon.ImageX = 0;
            this.lblPingIcon.ImageY = 0;
            this.lblPingIcon.LevelColor = System.Drawing.Color.Empty;
            this.lblPingIcon.Location = new System.Drawing.Point(685, 12);
            this.lblPingIcon.Name = "lblPingIcon";
            this.lblPingIcon.Size = new System.Drawing.Size(225, 22);
            this.lblPingIcon.TabIndex = 12;
            this.lblPingIcon.Text = "";
            this.lblPingIcon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPingIcon.TextRight = "";
            this.lblPingIcon.Visible = false;
            // 
            // lblCountryIcon
            // 
            this.lblCountryIcon.ImageHeight = 0;
            this.lblCountryIcon.ImageWidth = 0;
            this.lblCountryIcon.ImageX = 0;
            this.lblCountryIcon.ImageY = 0;
            this.lblCountryIcon.LevelColor = System.Drawing.Color.Empty;
            this.lblCountryIcon.Location = new System.Drawing.Point(640, 12);
            this.lblCountryIcon.Name = "lblCountryIcon";
            this.lblCountryIcon.Size = new System.Drawing.Size(225, 23);
            this.lblCountryIcon.TabIndex = 12;
            this.lblCountryIcon.Text = "";
            this.lblCountryIcon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCountryIcon.TextRight = "";
            this.lblCountryIcon.Visible = false;
            // 
            // lblPlayers
            // 
            this.lblPlayers.ImageHeight = 22;
            this.lblPlayers.ImageWidth = 28;
            this.lblPlayers.ImageX = 4;
            this.lblPlayers.ImageY = -1;
            this.lblPlayers.LevelColor = System.Drawing.Color.Empty;
            this.lblPlayers.Location = new System.Drawing.Point(557, 10);
            this.lblPlayers.Name = "lblPlayers";
            this.lblPlayers.Size = new System.Drawing.Size(225, 22);
            this.lblPlayers.TabIndex = 12;
            this.lblPlayers.Text = "PLAYERS :";
            this.lblPlayers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPlayers.TextRight = "0";
            this.lblPlayers.Visible = false;
            // 
            // lblPlayersPc
            // 
            this.lblPlayersPc.DrawVisible = false;
            this.lblPlayersPc.PlatformIcon = Properties.Resources.pc_icon;
            this.lblPlayersPc.ImageHeight = 13;
            this.lblPlayersPc.ImageWidth = 13;
            this.lblPlayersPc.ImageX = 0;
            this.lblPlayersPc.ImageY = 0;
            this.lblPlayersPc.LevelColor = System.Drawing.Color.Empty;
            this.lblPlayersPc.Location = new System.Drawing.Point(719, 12);
            this.lblPlayersPc.Name = "lblPlayersPc";
            this.lblPlayersPc.Size = new System.Drawing.Size(26, 16);
            this.lblPlayersPc.TabIndex = 24;
            this.lblPlayersPc.TextRight = "-";
            this.lblPlayersPc.Visible = false;
            // 
            // lblPlayersPs
            // 
            this.lblPlayersPs.DrawVisible = false;
            this.lblPlayersPs.PlatformIcon = Properties.Resources.ps_icon;
            this.lblPlayersPs.ImageHeight = 13;
            this.lblPlayersPs.ImageWidth = 13;
            this.lblPlayersPs.ImageX = 0;
            this.lblPlayersPs.ImageY = 0;
            this.lblPlayersPs.LevelColor = System.Drawing.Color.Empty;
            this.lblPlayersPs.Location = new System.Drawing.Point(605, 12);
            this.lblPlayersPs.Name = "lblPlayersPs";
            this.lblPlayersPs.Size = new System.Drawing.Size(26, 16);
            this.lblPlayersPs.TabIndex = 25;
            this.lblPlayersPs.TextRight = "-";
            this.lblPlayersPs.Visible = false;
            // 
            // lblPlayersXbox
            // 
            this.lblPlayersXbox.DrawVisible = false;
            this.lblPlayersXbox.PlatformIcon = Properties.Resources.xbox_icon;
            this.lblPlayersXbox.ImageHeight = 13;
            this.lblPlayersXbox.ImageWidth = 13;
            this.lblPlayersXbox.ImageX = 0;
            this.lblPlayersXbox.ImageY = 0;
            this.lblPlayersXbox.LevelColor = System.Drawing.Color.Empty;
            this.lblPlayersXbox.Location = new System.Drawing.Point(643, 12);
            this.lblPlayersXbox.Name = "lblPlayersXbox";
            this.lblPlayersXbox.Size = new System.Drawing.Size(26, 16);
            this.lblPlayersXbox.TabIndex = 26;
            this.lblPlayersXbox.TextRight = "-";
            this.lblPlayersXbox.Visible = false;
            // 
            // lblPlayersSwitch
            // 
            this.lblPlayersSwitch.DrawVisible = false;
            this.lblPlayersSwitch.PlatformIcon = Properties.Resources.switch_icon;
            this.lblPlayersSwitch.ImageHeight = 13;
            this.lblPlayersSwitch.ImageWidth = 13;
            this.lblPlayersSwitch.ImageX = 0;
            this.lblPlayersSwitch.ImageY = 0;
            this.lblPlayersSwitch.LevelColor = System.Drawing.Color.Empty;
            this.lblPlayersSwitch.Location = new System.Drawing.Point(681, 12);
            this.lblPlayersSwitch.Name = "lblPlayersSwitch";
            this.lblPlayersSwitch.Size = new System.Drawing.Size(26, 16);
            this.lblPlayersSwitch.TabIndex = 27;
            this.lblPlayersSwitch.TextRight = "-";
            this.lblPlayersSwitch.Visible = false;
            // 
            // lblRound
            // 
            this.lblRound.AutoEllipsis = true;
            this.lblRound.ImageHeight = 0;
            this.lblRound.ImageWidth = 0;
            this.lblRound.ImageX = 0;
            this.lblRound.ImageY = 0;
            this.lblRound.LevelColor = System.Drawing.Color.Empty;
            this.lblRound.Location = new System.Drawing.Point(270, 9);
            this.lblRound.Name = "lblRound";
            this.lblRound.Size = new System.Drawing.Size(281, 22);
            this.lblRound.TabIndex = 10;
            this.lblRound.Text = "ROUND 1 :";
            this.lblRound.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRound.TextRight = "N/A";
            this.lblRound.Visible = false;
            // 
            // lblWins
            // 
            this.lblWins.ImageHeight = 0;
            this.lblWins.ImageWidth = 0;
            this.lblWins.ImageX = 0;
            this.lblWins.ImageY = 0;
            this.lblWins.LevelColor = System.Drawing.Color.Empty;
            this.lblWins.Location = new System.Drawing.Point(22, 9);
            this.lblWins.Name = "lblWins";
            this.lblWins.Size = new System.Drawing.Size(242, 22);
            this.lblWins.TabIndex = 1;
            this.lblWins.Text = "WINS :";
            this.lblWins.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblWins.TextRight = "0 - 0.0%";
            this.lblWins.Visible = false;
            // 
            // lblFinish
            // 
            this.lblFinish.ImageHeight = 0;
            this.lblFinish.ImageWidth = 0;
            this.lblFinish.ImageX = 0;
            this.lblFinish.ImageY = 0;
            this.lblFinish.LevelColor = System.Drawing.Color.Empty;
            this.lblFinish.Location = new System.Drawing.Point(557, 55);
            this.lblFinish.Name = "lblFinish";
            this.lblFinish.Size = new System.Drawing.Size(225, 22);
            this.lblFinish.TabIndex = 20;
            this.lblFinish.Text = "FINISH :";
            this.lblFinish.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFinish.TextRight = "-";
            this.lblFinish.Visible = false;
            // 
            // Overlay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Magenta;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(786, 99);
            this.Controls.Add(this.picPositionNE);
            this.Controls.Add(this.picPositionNW);
            this.Controls.Add(this.picPositionSE);
            this.Controls.Add(this.picPositionSW);
            this.Controls.Add(this.picPositionLock);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lblProfile);
            this.Controls.Add(this.lblStreak);
            this.Controls.Add(this.lblFinals);
            this.Controls.Add(this.lblQualifyChance);
            this.Controls.Add(this.lblFastest);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblPingIcon);
            this.Controls.Add(this.lblCountryIcon);
            this.Controls.Add(this.lblPlayers);
            this.Controls.Add(this.lblPlayersPc);
            this.Controls.Add(this.lblPlayersPs);
            this.Controls.Add(this.lblPlayersXbox);
            this.Controls.Add(this.lblPlayersSwitch);
            this.Controls.Add(this.lblRound);
            this.Controls.Add(this.lblWins);
            this.Controls.Add(this.lblFinish);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "Overlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.KeyPreview = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Activated += new System.EventHandler(this.Overlay_GotFocus);
            this.Deactivate += new System.EventHandler(this.Overlay_LostFocus);
            this.Load += new System.EventHandler(this.Overlay_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Overlay_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Overlay_KeyUp);
            // this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseDown);
            //this.GotFocus += new System.EventHandler(this.Overlay_GotFocus);
            //this.LostFocus += new System.EventHandler(this.Overlay_LostFocus);
            // this.MouseEnter += new System.EventHandler(this.Overlay_MouseEnter);
            // this.MouseLeave += new System.EventHandler(this.Overlay_MouseLeave);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseWheel);
            this.Resize += new System.EventHandler(this.Overlay_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picPositionNE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionNW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionSE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionSW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPositionLock)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        private TransparentLabel lblRound;
        private TransparentLabel lblDuration;
        private TransparentLabel lblFinish;
        private TransparentLabel lblFastest;
        private TransparentLabel lblQualifyChance;
        private TransparentLabel lblWins;
        private TransparentLabel lblFinals;
        private TransparentLabel lblPlayers;
        private TransparentLabel lblPingIcon;
        private TransparentLabel lblCountryIcon;

        private TransparentLabel lblPlayersPc;
        private TransparentLabel lblPlayersPs;
        private TransparentLabel lblPlayersXbox;
        private TransparentLabel lblPlayersSwitch;
        
        private TransparentLabel lblStreak;
        private TransparentLabel lblFilter;
        private TransparentLabel lblProfile;
        
        private System.Windows.Forms.PictureBox picPositionNE;
        private System.Windows.Forms.PictureBox picPositionNW;
        private System.Windows.Forms.PictureBox picPositionSE;
        private System.Windows.Forms.PictureBox picPositionSW;
        private System.Windows.Forms.PictureBox picPositionLock;
    }
}