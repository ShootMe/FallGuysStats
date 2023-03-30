﻿using System.Drawing;
using System.Windows.Forms;

namespace FallGuysStats {
    partial class Settings {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.lblLogPath = new MetroFramework.Controls.MetroLabel();
            this.lblLogPathNote = new MetroFramework.Controls.MetroLabel();
            this.txtLogPath = new MetroFramework.Controls.MetroTextBox();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.lblOverlayFont = new MetroFramework.Controls.MetroLabel();
            this.btnSelectFont = new MetroFramework.Controls.MetroButton();
            this.btnResetOverlayFont = new MetroFramework.Controls.MetroButton();
            this.grpOverlayFontExample = new System.Windows.Forms.GroupBox();
            this.lblOverlayFontExample = new System.Windows.Forms.Label();
            this.grpCycleQualifyGold = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowGold = new MetroFramework.Controls.MetroRadioButton();
            this.chkOnlyShowQualify = new MetroFramework.Controls.MetroRadioButton();
            this.chkCycleQualifyGold = new MetroFramework.Controls.MetroRadioButton();
            this.grpCycleFastestLongest = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowLongest = new MetroFramework.Controls.MetroRadioButton();
            this.chkOnlyShowFastest = new MetroFramework.Controls.MetroRadioButton();
            this.chkCycleFastestLongest = new MetroFramework.Controls.MetroRadioButton();
            this.chkHidePercentages = new MetroFramework.Controls.MetroCheckBox();
            this.chkHideWinsInfo = new MetroFramework.Controls.MetroCheckBox();
            this.lblOverlayBackground = new MetroFramework.Controls.MetroLabel();
            this.cboOverlayColor = new MetroFramework.Controls.MetroComboBox();
            this.lblOverlayColor = new MetroFramework.Controls.MetroLabel();
            this.lblOverlayOpacity = new MetroFramework.Controls.MetroLabel();
            this.trkOverlayOpacity = new MetroFramework.Controls.MetroTrackBar();
            this.chkFlipped = new MetroFramework.Controls.MetroCheckBox();
            this.chkShowTabs = new MetroFramework.Controls.MetroCheckBox();
            this.chkHideTimeInfo = new MetroFramework.Controls.MetroCheckBox();
            this.chkHideRoundInfo = new MetroFramework.Controls.MetroCheckBox();
            this.cboFastestFilter = new MetroFramework.Controls.MetroComboBox();
            this.lblFastestFilter = new MetroFramework.Controls.MetroLabel();
            this.cboQualifyFilter = new MetroFramework.Controls.MetroComboBox();
            this.lblQualifyFilter = new MetroFramework.Controls.MetroLabel();
            this.cboWinsFilter = new MetroFramework.Controls.MetroComboBox();
            this.lblWinsFilter = new MetroFramework.Controls.MetroLabel();
            this.chkOverlayOnTop = new MetroFramework.Controls.MetroCheckBox();
            this.chkPlayerByConsoleType = new MetroFramework.Controls.MetroCheckBox();
            this.chkColorByRoundType = new MetroFramework.Controls.MetroCheckBox();
            this.chkAutoChangeProfile = new MetroFramework.Controls.MetroCheckBox();
            this.lblCycleTimeSecondsTag = new MetroFramework.Controls.MetroLabel();
            this.lblCycleTimeSeconds = new MetroFramework.Controls.MetroLabel();
            this.txtCycleTimeSeconds = new MetroFramework.Controls.MetroTextBox();
            this.grpCycleWinFinalStreak = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowFinalStreak = new MetroFramework.Controls.MetroRadioButton();
            this.chkOnlyShowWinStreak = new MetroFramework.Controls.MetroRadioButton();
            this.chkCycleWinFinalStreak = new MetroFramework.Controls.MetroRadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOnlyShowPing = new MetroFramework.Controls.MetroRadioButton();
            this.chkOnlyShowPlayers = new MetroFramework.Controls.MetroRadioButton();
            this.chkCyclePlayersPing = new MetroFramework.Controls.MetroRadioButton();
            this.grpLaunchPlatform = new System.Windows.Forms.GroupBox();
            this.picPlatformCheck = new System.Windows.Forms.PictureBox();
            this.picEpicGames = new System.Windows.Forms.PictureBox();
            this.picSteam = new System.Windows.Forms.PictureBox();
            this.cboMultilingual = new MetroFramework.Controls.MetroComboBox();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.lblTheme = new MetroFramework.Controls.MetroLabel();
            this.chkChangeHoopsieLegends = new MetroFramework.Controls.MetroCheckBox();
            this.cboTheme = new MetroFramework.Controls.MetroComboBox();
            this.chkAutoUpdate = new MetroFramework.Controls.MetroCheckBox();
            this.lblPreviousWinsNote = new MetroFramework.Controls.MetroLabel();
            this.lblPreviousWins = new MetroFramework.Controls.MetroLabel();
            this.txtPreviousWins = new MetroFramework.Controls.MetroTextBox();
            this.lblGameExeLocation = new MetroFramework.Controls.MetroLabel();
            this.txtGameExeLocation = new MetroFramework.Controls.MetroTextBox();
            this.txtGameShortcutLocation = new MetroFramework.Controls.MetroTextBox();
            this.btnGameExeLocationBrowse = new MetroFramework.Controls.MetroButton();
            this.chkAutoLaunchGameOnStart = new MetroFramework.Controls.MetroCheckBox();
            this.picLanguageSelection = new System.Windows.Forms.PictureBox();
            this.chkIgnoreLevelTypeWhenSorting = new MetroFramework.Controls.MetroCheckBox();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.dlgOverlayFont = new System.Windows.Forms.FontDialog();
            this.platformToolTip = new MetroFramework.Components.MetroToolTip();
            this.tileProgram = new MetroFramework.Controls.MetroTile();
            this.panelProgram = new MetroFramework.Controls.MetroPanel();
            this.lblLanguage = new MetroFramework.Controls.MetroLabel();
            this.panelDisplay = new MetroFramework.Controls.MetroPanel();
            this.tileDisplay = new MetroFramework.Controls.MetroTile();
            this.panelOverlay = new MetroFramework.Controls.MetroPanel();
            this.cboOverlayBackground = new FallGuysStats.ImageComboBox();
            this.tileOverlay = new MetroFramework.Controls.MetroTile();
            this.tileFallGuys = new MetroFramework.Controls.MetroTile();
            this.panelFallGuys = new MetroFramework.Controls.MetroPanel();
            this.tileAbout = new MetroFramework.Controls.MetroTile();
            this.panelAbout = new MetroFramework.Controls.MetroPanel();
            this.lbltpl4 = new MetroFramework.Controls.MetroLink();
            this.lbltpl3 = new MetroFramework.Controls.MetroLink();
            this.lbltpl2 = new MetroFramework.Controls.MetroLink();
            this.lblthirdpartyLicences = new MetroFramework.Controls.MetroLabel();
            this.lbltpl1 = new MetroFramework.Controls.MetroLink();
            this.fglink2 = new MetroFramework.Controls.MetroLink();
            this.fglink1 = new MetroFramework.Controls.MetroLink();
            this.btnCheckUpdates = new MetroFramework.Controls.MetroButton();
            this.lblVersion = new MetroFramework.Controls.MetroLabel();
            this.lblLicence = new MetroFramework.Controls.MetroLabel();
            this.lblupdateNote = new MetroFramework.Controls.MetroLabel();
            this.grpOverlayFontExample.SuspendLayout();
            this.overlayOpacityToolTip = new MetroFramework.Components.MetroToolTip();
            this.grpOverlay.SuspendLayout();
            this.grpCycleQualifyGold.SuspendLayout();
            this.grpCycleFastestLongest.SuspendLayout();
            this.grpCycleWinFinalStreak.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpLaunchPlatform.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPlatformCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEpicGames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSteam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLanguageSelection)).BeginInit();
            this.panelProgram.SuspendLayout();
            this.panelDisplay.SuspendLayout();
            this.panelOverlay.SuspendLayout();
            this.panelFallGuys.SuspendLayout();
            this.panelAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLogPath
            // 
            this.lblLogPath.AutoSize = true;
            this.lblLogPath.Location = new System.Drawing.Point(8, 12);
            this.lblLogPath.Name = "lblLogPath";
            this.lblLogPath.Size = new System.Drawing.Size(84, 19);
            this.lblLogPath.TabIndex = 0;
            this.lblLogPath.Text = "Log File Path";
            // 
            // lblLogPathNote
            // 
            this.lblLogPathNote.AutoSize = true;
            this.lblLogPathNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblLogPathNote.Location = new System.Drawing.Point(8, 37);
            this.lblLogPathNote.Name = "lblLogPathNote";
            this.lblLogPathNote.Size = new System.Drawing.Size(572, 19);
            this.lblLogPathNote.TabIndex = 2;
            this.lblLogPathNote.Text = "* You should not need to set this. Only use when the program is not reading the c" +
    "orrect location.";
            // 
            // txtLogPath
            // 
            this.txtLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtLogPath.CustomButton.Image = null;
            this.txtLogPath.CustomButton.Location = new System.Drawing.Point(520, 2);
            this.txtLogPath.CustomButton.Name = "";
            this.txtLogPath.CustomButton.Size = new System.Drawing.Size(17, 17);
            this.txtLogPath.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtLogPath.CustomButton.TabIndex = 1;
            this.txtLogPath.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtLogPath.CustomButton.UseSelectable = true;
            this.txtLogPath.CustomButton.Visible = false;
            this.txtLogPath.Lines = new string[0];
            this.txtLogPath.Location = new System.Drawing.Point(98, 12);
            this.txtLogPath.MaxLength = 32767;
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.PasswordChar = '\0';
            this.txtLogPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtLogPath.SelectedText = "";
            this.txtLogPath.SelectionLength = 0;
            this.txtLogPath.SelectionStart = 0;
            this.txtLogPath.ShortcutsEnabled = true;
            this.txtLogPath.Size = new System.Drawing.Size(540, 22);
            this.txtLogPath.TabIndex = 1;
            this.txtLogPath.UseSelectable = true;
            this.txtLogPath.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtLogPath.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtLogPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLogPath_Validating);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSave.Location = new System.Drawing.Point(678, 600);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblOverlayFont
            // 
            this.lblOverlayFont.AutoSize = true;
            this.lblOverlayFont.Location = new System.Drawing.Point(15, 531);
            this.lblOverlayFont.Name = "lblOverlayFont";
            this.lblOverlayFont.Size = new System.Drawing.Size(133, 19);
            this.lblOverlayFont.TabIndex = 26;
            this.lblOverlayFont.Text = "Custom Overlay Font";
            // 
            // btnSelectFont
            // 
            this.btnSelectFont.Location = new System.Drawing.Point(153, 527);
            this.btnSelectFont.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelectFont.Name = "btnSelectFont";
            this.btnSelectFont.Size = new System.Drawing.Size(96, 25);
            this.btnSelectFont.TabIndex = 27;
            this.btnSelectFont.Text = "Select Font";
            this.btnSelectFont.UseSelectable = true;
            this.btnSelectFont.Click += new System.EventHandler(this.btnSelectFont_Click);
            // 
            // btnResetOverlayFont
            // 
            this.btnResetOverlayFont.Location = new System.Drawing.Point(253, 527);
            this.btnResetOverlayFont.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetOverlayFont.Name = "btnResetOverlayFont";
            this.btnResetOverlayFont.Size = new System.Drawing.Size(96, 25);
            this.btnResetOverlayFont.TabIndex = 28;
            this.btnResetOverlayFont.Text = "Reset Font";
            this.btnResetOverlayFont.UseSelectable = true;
            this.btnResetOverlayFont.Click += new System.EventHandler(this.btnResetOverlayFont_Click);
            // 
            // grpOverlayFontExample
            // 
            this.grpOverlayFontExample.Controls.Add(this.lblOverlayFontExample);
            this.grpOverlayFontExample.Location = new System.Drawing.Point(15, 566);
            this.grpOverlayFontExample.Margin = new System.Windows.Forms.Padding(2);
            this.grpOverlayFontExample.Name = "grpOverlayFontExample";
            this.grpOverlayFontExample.Padding = new System.Windows.Forms.Padding(2);
            this.grpOverlayFontExample.Size = new System.Drawing.Size(618, 75);
            this.grpOverlayFontExample.TabIndex = 35;
            this.grpOverlayFontExample.TabStop = false;
            this.grpOverlayFontExample.Text = "Example";
            // 
            // lblOverlayFontExample
            // 
            this.lblOverlayFontExample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOverlayFontExample.Location = new System.Drawing.Point(5, 14);
            this.lblOverlayFontExample.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOverlayFontExample.Name = "lblOverlayFontExample";
            this.lblOverlayFontExample.Size = new System.Drawing.Size(609, 53);
            this.lblOverlayFontExample.TabIndex = 0;
            this.lblOverlayFontExample.Text = "Round 3 : Freezy Peak";
            this.lblOverlayFontExample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpCycleQualifyGold
            // 
            this.grpCycleQualifyGold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleQualifyGold.Controls.Add(this.chkOnlyShowGold);
            this.grpCycleQualifyGold.Controls.Add(this.chkOnlyShowQualify);
            this.grpCycleQualifyGold.Controls.Add(this.chkCycleQualifyGold);
            this.grpCycleQualifyGold.Location = new System.Drawing.Point(15, 195);
            this.grpCycleQualifyGold.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleQualifyGold.Name = "grpCycleQualifyGold";
            this.grpCycleQualifyGold.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleQualifyGold.Size = new System.Drawing.Size(576, 40);
            this.grpCycleQualifyGold.TabIndex = 8;
            this.grpCycleQualifyGold.TabStop = false;
            // 
            // chkOnlyShowGold
            // 
            this.chkOnlyShowGold.AutoSize = true;
            this.chkOnlyShowGold.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowGold.Location = new System.Drawing.Point(385, 14);
            this.chkOnlyShowGold.Name = "chkOnlyShowGold";
            this.chkOnlyShowGold.Size = new System.Drawing.Size(87, 19);
            this.chkOnlyShowGold.TabIndex = 2;
            this.chkOnlyShowGold.Text = "Gold Only";
            this.chkOnlyShowGold.UseSelectable = true;
            // 
            // chkOnlyShowQualify
            // 
            this.chkOnlyShowQualify.AutoSize = true;
            this.chkOnlyShowQualify.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowQualify.Location = new System.Drawing.Point(238, 14);
            this.chkOnlyShowQualify.Name = "chkOnlyShowQualify";
            this.chkOnlyShowQualify.Size = new System.Drawing.Size(101, 19);
            this.chkOnlyShowQualify.TabIndex = 1;
            this.chkOnlyShowQualify.Text = "Qualify Only";
            this.chkOnlyShowQualify.UseSelectable = true;
            // 
            // chkCycleQualifyGold
            // 
            this.chkCycleQualifyGold.AutoSize = true;
            this.chkCycleQualifyGold.Checked = true;
            this.chkCycleQualifyGold.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkCycleQualifyGold.Location = new System.Drawing.Point(6, 14);
            this.chkCycleQualifyGold.Name = "chkCycleQualifyGold";
            this.chkCycleQualifyGold.Size = new System.Drawing.Size(146, 19);
            this.chkCycleQualifyGold.TabIndex = 0;
            this.chkCycleQualifyGold.TabStop = true;
            this.chkCycleQualifyGold.Text = "Cycle Qualify / Gold";
            this.chkCycleQualifyGold.UseSelectable = true;
            // 
            // grpCycleFastestLongest
            // 
            this.grpCycleFastestLongest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleFastestLongest.Controls.Add(this.chkOnlyShowLongest);
            this.grpCycleFastestLongest.Controls.Add(this.chkOnlyShowFastest);
            this.grpCycleFastestLongest.Controls.Add(this.chkCycleFastestLongest);
            this.grpCycleFastestLongest.Location = new System.Drawing.Point(15, 225);
            this.grpCycleFastestLongest.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleFastestLongest.Name = "grpCycleFastestLongest";
            this.grpCycleFastestLongest.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleFastestLongest.Size = new System.Drawing.Size(576, 40);
            this.grpCycleFastestLongest.TabIndex = 9;
            this.grpCycleFastestLongest.TabStop = false;
            // 
            // chkOnlyShowLongest
            // 
            this.chkOnlyShowLongest.AutoSize = true;
            this.chkOnlyShowLongest.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowLongest.Location = new System.Drawing.Point(385, 14);
            this.chkOnlyShowLongest.Name = "chkOnlyShowLongest";
            this.chkOnlyShowLongest.Size = new System.Drawing.Size(107, 19);
            this.chkOnlyShowLongest.TabIndex = 2;
            this.chkOnlyShowLongest.Text = "Longest Only";
            this.chkOnlyShowLongest.UseSelectable = true;
            // 
            // chkOnlyShowFastest
            // 
            this.chkOnlyShowFastest.AutoSize = true;
            this.chkOnlyShowFastest.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowFastest.Location = new System.Drawing.Point(238, 14);
            this.chkOnlyShowFastest.Name = "chkOnlyShowFastest";
            this.chkOnlyShowFastest.Size = new System.Drawing.Size(101, 19);
            this.chkOnlyShowFastest.TabIndex = 1;
            this.chkOnlyShowFastest.Text = "Fastest Only";
            this.chkOnlyShowFastest.UseSelectable = true;
            // 
            // chkCycleFastestLongest
            // 
            this.chkCycleFastestLongest.AutoSize = true;
            this.chkCycleFastestLongest.Checked = true;
            this.chkCycleFastestLongest.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkCycleFastestLongest.Location = new System.Drawing.Point(6, 14);
            this.chkCycleFastestLongest.Name = "chkCycleFastestLongest";
            this.chkCycleFastestLongest.Size = new System.Drawing.Size(166, 19);
            this.chkCycleFastestLongest.TabIndex = 0;
            this.chkCycleFastestLongest.TabStop = true;
            this.chkCycleFastestLongest.Text = "Cycle Fastest / Longest";
            this.chkCycleFastestLongest.UseSelectable = true;
            // 
            // chkHidePercentages
            // 
            this.chkHidePercentages.AutoSize = true;
            this.chkHidePercentages.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkHidePercentages.Location = new System.Drawing.Point(15, 109);
            this.chkHidePercentages.Name = "chkHidePercentages";
            this.chkHidePercentages.Size = new System.Drawing.Size(130, 19);
            this.chkHidePercentages.TabIndex = 3;
            this.chkHidePercentages.Text = "Hide Percentages";
            this.chkHidePercentages.UseSelectable = true;
            // 
            // chkHideWinsInfo
            // 
            this.chkHideWinsInfo.AutoSize = true;
            this.chkHideWinsInfo.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkHideWinsInfo.Location = new System.Drawing.Point(15, 19);
            this.chkHideWinsInfo.Name = "chkHideWinsInfo";
            this.chkHideWinsInfo.Size = new System.Drawing.Size(114, 19);
            this.chkHideWinsInfo.TabIndex = 0;
            this.chkHideWinsInfo.Text = "Hide Wins info";
            this.chkHideWinsInfo.UseSelectable = true;
            // 
            // lblOverlayBackground
            // 
            this.lblOverlayBackground.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblOverlayBackground.AutoSize = true;
            this.lblOverlayBackground.Location = new System.Drawing.Point(15, 335);
            this.lblOverlayBackground.Name = "lblOverlayBackground";
            this.lblOverlayBackground.Size = new System.Drawing.Size(120, 19);
            this.lblOverlayBackground.TabIndex = 18;
            this.lblOverlayBackground.Text = "Background Image";
            // 
            // cboOverlayColor
            // 
            this.cboOverlayColor.FormattingEnabled = true;
            this.cboOverlayColor.ItemHeight = 23;
            this.cboOverlayColor.Items.AddRange(new object[] {
            "Transparent",
            "Black",
            "Magenta",
            "Red",
            "Green",
            "Blue"});
            this.cboOverlayColor.Location = new System.Drawing.Point(168, 361);
            this.cboOverlayColor.Name = "cboOverlayColor";
            this.cboOverlayColor.Size = new System.Drawing.Size(240, 29);
            this.cboOverlayColor.TabIndex = 21;
            this.cboOverlayColor.UseSelectable = true;
            // 
            // lblOverlayColor
            // 
            this.lblOverlayColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblOverlayColor.AutoSize = true;
            this.lblOverlayColor.Location = new System.Drawing.Point(15, 366);
            this.lblOverlayColor.Name = "lblOverlayColor";
            this.lblOverlayColor.Size = new System.Drawing.Size(79, 19);
            this.lblOverlayColor.TabIndex = 20;
            this.lblOverlayColor.Text = "Background";
            // 
            // lblOverlayOpacity
            // 
            this.lblOverlayOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOverlayOpacity.AutoSize = true;
            this.lblOverlayOpacity.Location = new System.Drawing.Point(453, 195);
            this.lblOverlayOpacity.Name = "lblOverlayOpacity";
            this.lblOverlayOpacity.Size = new System.Drawing.Size(128, 19);
            this.lblOverlayOpacity.TabIndex = 20;
            this.lblOverlayOpacity.Text = "Background Opacity";
            // 
            // trkOverlayOpacity
            // 
            this.trkOverlayOpacity.BackColor = System.Drawing.Color.Transparent;
            this.trkOverlayOpacity.Location = new System.Drawing.Point(599, 193);
            this.trkOverlayOpacity.Name = "trkOverlayOpacity";
            this.trkOverlayOpacity.Size = new System.Drawing.Size(240, 29);
            this.trkOverlayOpacity.TabIndex = 22;
            this.overlayOpacityToolTip.SetToolTip(this.trkOverlayOpacity, "100");
            this.trkOverlayOpacity.Value = 100;
            this.trkOverlayOpacity.ValueChanged += new System.EventHandler(this.trkOverlayOpacity_ValueChanged);
            // 
            // chkFlipped
            // 
            this.chkFlipped.AutoSize = true;
            this.chkFlipped.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkFlipped.Location = new System.Drawing.Point(15, 396);
            this.chkFlipped.Name = "chkFlipped";
            this.chkFlipped.Size = new System.Drawing.Size(167, 19);
            this.chkFlipped.TabIndex = 23;
            this.chkFlipped.Text = "Flip display horizontally";
            this.chkFlipped.UseSelectable = true;
            // 
            // chkShowTabs
            // 
            this.chkShowTabs.AutoSize = true;
            this.chkShowTabs.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkShowTabs.Location = new System.Drawing.Point(15, 139);
            this.chkShowTabs.Name = "chkShowTabs";
            this.chkShowTabs.Size = new System.Drawing.Size(233, 19);
            this.chkShowTabs.TabIndex = 4;
            this.chkShowTabs.Text = "Show Tab for current filter / profile";
            this.chkShowTabs.UseSelectable = true;
            // 
            // chkHideTimeInfo
            // 
            this.chkHideTimeInfo.AutoSize = true;
            this.chkHideTimeInfo.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkHideTimeInfo.Location = new System.Drawing.Point(15, 79);
            this.chkHideTimeInfo.Name = "chkHideTimeInfo";
            this.chkHideTimeInfo.Size = new System.Drawing.Size(113, 19);
            this.chkHideTimeInfo.TabIndex = 2;
            this.chkHideTimeInfo.Text = "Hide Time info";
            this.chkHideTimeInfo.UseSelectable = true;
            // 
            // chkHideRoundInfo
            // 
            this.chkHideRoundInfo.AutoSize = true;
            this.chkHideRoundInfo.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkHideRoundInfo.Location = new System.Drawing.Point(15, 49);
            this.chkHideRoundInfo.Name = "chkHideRoundInfo";
            this.chkHideRoundInfo.Size = new System.Drawing.Size(124, 19);
            this.chkHideRoundInfo.TabIndex = 1;
            this.chkHideRoundInfo.Text = "Hide Round info";
            this.chkHideRoundInfo.UseSelectable = true;
            // 
            // cboFastestFilter
            // 
            this.cboFastestFilter.FormattingEnabled = true;
            this.cboFastestFilter.ItemHeight = 23;
            this.cboFastestFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
            this.cboFastestFilter.Location = new System.Drawing.Point(393, 75);
            this.cboFastestFilter.Name = "cboFastestFilter";
            this.cboFastestFilter.Size = new System.Drawing.Size(240, 29);
            this.cboFastestFilter.TabIndex = 17;
            this.cboFastestFilter.UseSelectable = true;
            // 
            // lblFastestFilter
            // 
            this.lblFastestFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFastestFilter.AutoSize = true;
            this.lblFastestFilter.Location = new System.Drawing.Point(248, 79);
            this.lblFastestFilter.Name = "lblFastestFilter";
            this.lblFastestFilter.Size = new System.Drawing.Size(138, 19);
            this.lblFastestFilter.TabIndex = 16;
            this.lblFastestFilter.Text = "Fastest / Longest Filter";
            // 
            // cboQualifyFilter
            // 
            this.cboQualifyFilter.FormattingEnabled = true;
            this.cboQualifyFilter.ItemHeight = 23;
            this.cboQualifyFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
            this.cboQualifyFilter.Location = new System.Drawing.Point(393, 45);
            this.cboQualifyFilter.Name = "cboQualifyFilter";
            this.cboQualifyFilter.Size = new System.Drawing.Size(240, 29);
            this.cboQualifyFilter.TabIndex = 15;
            this.cboQualifyFilter.UseSelectable = true;
            // 
            // lblQualifyFilter
            // 
            this.lblQualifyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQualifyFilter.AutoSize = true;
            this.lblQualifyFilter.Location = new System.Drawing.Point(262, 49);
            this.lblQualifyFilter.Name = "lblQualifyFilter";
            this.lblQualifyFilter.Size = new System.Drawing.Size(124, 19);
            this.lblQualifyFilter.TabIndex = 14;
            this.lblQualifyFilter.Text = "Qualify / Gold Filter";
            // 
            // cboWinsFilter
            // 
            this.cboWinsFilter.FormattingEnabled = true;
            this.cboWinsFilter.ItemHeight = 23;
            this.cboWinsFilter.Items.AddRange(new object[] {
            "All Time Stats",
            "Stats and Party Filter",
            "Season Stats",
            "Week Stats",
            "Day Stats",
            "Session Stats"});
            this.cboWinsFilter.Location = new System.Drawing.Point(393, 15);
            this.cboWinsFilter.Name = "cboWinsFilter";
            this.cboWinsFilter.Size = new System.Drawing.Size(240, 29);
            this.cboWinsFilter.TabIndex = 13;
            this.cboWinsFilter.UseSelectable = true;
            // 
            // lblWinsFilter
            // 
            this.lblWinsFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWinsFilter.AutoSize = true;
            this.lblWinsFilter.Location = new System.Drawing.Point(277, 19);
            this.lblWinsFilter.Name = "lblWinsFilter";
            this.lblWinsFilter.Size = new System.Drawing.Size(114, 19);
            this.lblWinsFilter.TabIndex = 12;
            this.lblWinsFilter.Text = "Wins / Final Filter ";
            this.lblWinsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkOverlayOnTop
            // 
            this.chkOverlayOnTop.AutoSize = true;
            this.chkOverlayOnTop.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOverlayOnTop.Location = new System.Drawing.Point(15, 421);
            this.chkOverlayOnTop.Name = "chkOverlayOnTop";
            this.chkOverlayOnTop.Size = new System.Drawing.Size(148, 19);
            this.chkOverlayOnTop.TabIndex = 22;
            this.chkOverlayOnTop.Text = "Always show on top";
            this.chkOverlayOnTop.UseSelectable = true;
            // 
            // chkPlayerByConsoleType
            // 
            this.chkPlayerByConsoleType.AutoSize = true;
            this.chkPlayerByConsoleType.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkPlayerByConsoleType.Location = new System.Drawing.Point(15, 446);
            this.chkPlayerByConsoleType.Name = "chkPlayerByConsoleType";
            this.chkPlayerByConsoleType.Size = new System.Drawing.Size(234, 19);
            this.chkPlayerByConsoleType.TabIndex = 24;
            this.chkPlayerByConsoleType.Text = "Display the Player by console type";
            this.chkPlayerByConsoleType.UseSelectable = true;
            // 
            // chkColorByRoundType
            // 
            this.chkColorByRoundType.AutoSize = true;
            this.chkColorByRoundType.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkColorByRoundType.Location = new System.Drawing.Point(15, 471);
            this.chkColorByRoundType.Name = "chkColorByRoundType";
            this.chkColorByRoundType.Size = new System.Drawing.Size(221, 19);
            this.chkColorByRoundType.TabIndex = 25;
            this.chkColorByRoundType.Text = "Display the Color by round type";
            this.chkColorByRoundType.UseSelectable = true;
            // 
            // chkAutoChangeProfile
            // 
            this.chkAutoChangeProfile.AutoSize = true;
            this.chkAutoChangeProfile.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkAutoChangeProfile.Location = new System.Drawing.Point(15, 496);
            this.chkAutoChangeProfile.Name = "chkAutoChangeProfile";
            this.chkAutoChangeProfile.Size = new System.Drawing.Size(221, 19);
            this.chkAutoChangeProfile.TabIndex = 26;
            this.chkAutoChangeProfile.Text = "Display the Color by round type";
            this.chkAutoChangeProfile.UseSelectable = true;
            // 
            // lblCycleTimeSecondsTag
            // 
            this.lblCycleTimeSecondsTag.AutoSize = true;
            this.lblCycleTimeSecondsTag.Location = new System.Drawing.Point(118, 170);
            this.lblCycleTimeSecondsTag.Name = "lblCycleTimeSecondsTag";
            this.lblCycleTimeSecondsTag.Size = new System.Drawing.Size(29, 19);
            this.lblCycleTimeSecondsTag.TabIndex = 7;
            this.lblCycleTimeSecondsTag.Text = "Sec";
            // 
            // lblCycleTimeSeconds
            // 
            this.lblCycleTimeSeconds.AutoSize = true;
            this.lblCycleTimeSeconds.Location = new System.Drawing.Point(15, 170);
            this.lblCycleTimeSeconds.Name = "lblCycleTimeSeconds";
            this.lblCycleTimeSeconds.Size = new System.Drawing.Size(73, 19);
            this.lblCycleTimeSeconds.TabIndex = 5;
            this.lblCycleTimeSeconds.Text = "Cycle Time";
            // 
            // txtCycleTimeSeconds
            // 
            // 
            // 
            // 
            this.txtCycleTimeSeconds.CustomButton.Image = null;
            this.txtCycleTimeSeconds.CustomButton.Location = new System.Drawing.Point(4, 1);
            this.txtCycleTimeSeconds.CustomButton.Name = "";
            this.txtCycleTimeSeconds.CustomButton.Size = new System.Drawing.Size(19, 19);
            this.txtCycleTimeSeconds.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtCycleTimeSeconds.CustomButton.TabIndex = 1;
            this.txtCycleTimeSeconds.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtCycleTimeSeconds.CustomButton.UseSelectable = true;
            this.txtCycleTimeSeconds.CustomButton.Visible = false;
            this.txtCycleTimeSeconds.Lines = new string[] {
        "5"};
            this.txtCycleTimeSeconds.Location = new System.Drawing.Point(90, 170);
            this.txtCycleTimeSeconds.MaxLength = 2;
            this.txtCycleTimeSeconds.Name = "txtCycleTimeSeconds";
            this.txtCycleTimeSeconds.PasswordChar = '\0';
            this.txtCycleTimeSeconds.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtCycleTimeSeconds.SelectedText = "";
            this.txtCycleTimeSeconds.SelectionLength = 0;
            this.txtCycleTimeSeconds.SelectionStart = 0;
            this.txtCycleTimeSeconds.ShortcutsEnabled = true;
            this.txtCycleTimeSeconds.Size = new System.Drawing.Size(24, 21);
            this.txtCycleTimeSeconds.TabIndex = 6;
            this.txtCycleTimeSeconds.Text = "5";
            this.txtCycleTimeSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCycleTimeSeconds.UseSelectable = true;
            this.txtCycleTimeSeconds.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtCycleTimeSeconds.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtCycleTimeSeconds.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycleTimeSeconds_Validating);
            // 
            // grpCycleWinFinalStreak
            // 
            this.grpCycleWinFinalStreak.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCycleWinFinalStreak.Controls.Add(this.chkOnlyShowFinalStreak);
            this.grpCycleWinFinalStreak.Controls.Add(this.chkOnlyShowWinStreak);
            this.grpCycleWinFinalStreak.Controls.Add(this.chkCycleWinFinalStreak);
            this.grpCycleWinFinalStreak.Location = new System.Drawing.Point(15, 255);
            this.grpCycleWinFinalStreak.Margin = new System.Windows.Forms.Padding(0);
            this.grpCycleWinFinalStreak.Name = "grpCycleWinFinalStreak";
            this.grpCycleWinFinalStreak.Padding = new System.Windows.Forms.Padding(2);
            this.grpCycleWinFinalStreak.Size = new System.Drawing.Size(576, 40);
            this.grpCycleWinFinalStreak.TabIndex = 10;
            this.grpCycleWinFinalStreak.TabStop = false;
            // 
            // chkOnlyShowFinalStreak
            // 
            this.chkOnlyShowFinalStreak.AutoSize = true;
            this.chkOnlyShowFinalStreak.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowFinalStreak.Location = new System.Drawing.Point(385, 14);
            this.chkOnlyShowFinalStreak.Name = "chkOnlyShowFinalStreak";
            this.chkOnlyShowFinalStreak.Size = new System.Drawing.Size(128, 19);
            this.chkOnlyShowFinalStreak.TabIndex = 2;
            this.chkOnlyShowFinalStreak.Text = "Final Streak Only";
            this.chkOnlyShowFinalStreak.UseSelectable = true;
            // 
            // chkOnlyShowWinStreak
            // 
            this.chkOnlyShowWinStreak.AutoSize = true;
            this.chkOnlyShowWinStreak.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowWinStreak.Location = new System.Drawing.Point(238, 14);
            this.chkOnlyShowWinStreak.Name = "chkOnlyShowWinStreak";
            this.chkOnlyShowWinStreak.Size = new System.Drawing.Size(124, 19);
            this.chkOnlyShowWinStreak.TabIndex = 1;
            this.chkOnlyShowWinStreak.Text = "Win Streak Only";
            this.chkOnlyShowWinStreak.UseSelectable = true;
            // 
            // chkCycleWinFinalStreak
            // 
            this.chkCycleWinFinalStreak.AutoSize = true;
            this.chkCycleWinFinalStreak.Checked = true;
            this.chkCycleWinFinalStreak.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkCycleWinFinalStreak.Location = new System.Drawing.Point(6, 14);
            this.chkCycleWinFinalStreak.Name = "chkCycleWinFinalStreak";
            this.chkCycleWinFinalStreak.Size = new System.Drawing.Size(168, 19);
            this.chkCycleWinFinalStreak.TabIndex = 0;
            this.chkCycleWinFinalStreak.TabStop = true;
            this.chkCycleWinFinalStreak.Text = "Cycle Win / Final Streak";
            this.chkCycleWinFinalStreak.UseSelectable = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkOnlyShowPing);
            this.groupBox1.Controls.Add(this.chkOnlyShowPlayers);
            this.groupBox1.Controls.Add(this.chkCyclePlayersPing);
            this.groupBox1.Location = new System.Drawing.Point(15, 285);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(576, 40);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // chkOnlyShowPing
            // 
            this.chkOnlyShowPing.AutoSize = true;
            this.chkOnlyShowPing.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowPing.Location = new System.Drawing.Point(385, 14);
            this.chkOnlyShowPing.Name = "chkOnlyShowPing";
            this.chkOnlyShowPing.Size = new System.Drawing.Size(85, 19);
            this.chkOnlyShowPing.TabIndex = 2;
            this.chkOnlyShowPing.Text = "Ping Only";
            this.chkOnlyShowPing.UseSelectable = true;
            // 
            // chkOnlyShowPlayers
            // 
            this.chkOnlyShowPlayers.AutoSize = true;
            this.chkOnlyShowPlayers.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkOnlyShowPlayers.Location = new System.Drawing.Point(238, 14);
            this.chkOnlyShowPlayers.Name = "chkOnlyShowPlayers";
            this.chkOnlyShowPlayers.Size = new System.Drawing.Size(101, 19);
            this.chkOnlyShowPlayers.TabIndex = 1;
            this.chkOnlyShowPlayers.Text = "Players Only";
            this.chkOnlyShowPlayers.UseSelectable = true;
            // 
            // chkCyclePlayersPing
            // 
            this.chkCyclePlayersPing.AutoSize = true;
            this.chkCyclePlayersPing.Checked = true;
            this.chkCyclePlayersPing.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkCyclePlayersPing.Location = new System.Drawing.Point(6, 14);
            this.chkCyclePlayersPing.Name = "chkCyclePlayersPing";
            this.chkCyclePlayersPing.Size = new System.Drawing.Size(144, 19);
            this.chkCyclePlayersPing.TabIndex = 0;
            this.chkCyclePlayersPing.TabStop = true;
            this.chkCyclePlayersPing.Text = "Cycle Players / Ping";
            this.chkCyclePlayersPing.UseSelectable = true;
            // 
            // grpLaunchPlatform
            // 
            this.grpLaunchPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLaunchPlatform.Controls.Add(this.picPlatformCheck);
            this.grpLaunchPlatform.Controls.Add(this.picEpicGames);
            this.grpLaunchPlatform.Controls.Add(this.picSteam);
            this.grpLaunchPlatform.Location = new System.Drawing.Point(8, 12);
            this.grpLaunchPlatform.Margin = new System.Windows.Forms.Padding(0);
            this.grpLaunchPlatform.Name = "grpLaunchPlatform";
            this.grpLaunchPlatform.Padding = new System.Windows.Forms.Padding(2);
            this.grpLaunchPlatform.Size = new System.Drawing.Size(95, 60);
            this.grpLaunchPlatform.TabIndex = 36;
            this.grpLaunchPlatform.TabStop = false;
            this.grpLaunchPlatform.Text = "Platform";
            // 
            // picPlatformCheck
            // 
            this.picPlatformCheck.BackColor = System.Drawing.Color.Transparent;
            this.picPlatformCheck.Image = global::FallGuysStats.Properties.Resources.checkmark_icon;
            this.picPlatformCheck.Location = new System.Drawing.Point(10, 0);
            this.picPlatformCheck.Name = "picPlatformCheck";
            this.picPlatformCheck.Size = new System.Drawing.Size(28, 22);
            this.picPlatformCheck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPlatformCheck.TabIndex = 0;
            this.picPlatformCheck.TabStop = false;
            // 
            // picEpicGames
            // 
            this.picEpicGames.BackColor = System.Drawing.Color.Transparent;
            this.picEpicGames.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picEpicGames.Image = global::FallGuysStats.Properties.Resources.epic_icon;
            this.picEpicGames.Location = new System.Drawing.Point(5, 17);
            this.picEpicGames.Name = "picEpicGames";
            this.picEpicGames.Size = new System.Drawing.Size(46, 38);
            this.picEpicGames.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEpicGames.TabIndex = 1;
            this.picEpicGames.TabStop = false;
            this.platformToolTip.SetToolTip(this.picEpicGames, "Epic Games");
            this.picEpicGames.Click += new System.EventHandler(this.launchPlatform_Click);
            // 
            // picSteam
            // 
            this.picSteam.BackColor = System.Drawing.Color.Transparent;
            this.picSteam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSteam.Image = global::FallGuysStats.Properties.Resources.steam_icon;
            this.picSteam.Location = new System.Drawing.Point(47, 18);
            this.picSteam.Name = "picSteam";
            this.picSteam.Size = new System.Drawing.Size(45, 34);
            this.picSteam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSteam.TabIndex = 2;
            this.picSteam.TabStop = false;
            this.platformToolTip.SetToolTip(this.picSteam, "Steam");
            this.picSteam.Click += new System.EventHandler(this.launchPlatform_Click);
            // 
            // cboMultilingual
            // 
            this.cboMultilingual.ItemHeight = 23;
            this.cboMultilingual.Items.AddRange(new object[] {
            "🇺🇸 English",
            "🇫🇷 Français",
            "🇰🇷 한국어",
            "🇯🇵 日本語",
            "🇨🇳 简体中文"});
            this.cboMultilingual.Location = new System.Drawing.Point(115, 122);
            this.cboMultilingual.Name = "cboMultilingual";
            this.cboMultilingual.Size = new System.Drawing.Size(105, 29);
            this.cboMultilingual.TabIndex = 99;
            this.cboMultilingual.UseSelectable = true;
            this.cboMultilingual.SelectedIndexChanged += new System.EventHandler(this.cboMultilingual_SelectedIndexChanged);
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(8, 92);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(49, 19);
            this.lblTheme.TabIndex = 5;
            this.lblTheme.Text = "Theme";
            // 
            // chkChangeHoopsieLegends
            // 
            this.chkChangeHoopsieLegends.AutoSize = true;
            this.chkChangeHoopsieLegends.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkChangeHoopsieLegends.Location = new System.Drawing.Point(8, 39);
            this.chkChangeHoopsieLegends.Name = "chkChangeHoopsieLegends";
            this.chkChangeHoopsieLegends.Size = new System.Drawing.Size(301, 19);
            this.chkChangeHoopsieLegends.TabIndex = 4;
            this.chkChangeHoopsieLegends.Text = "Rename Hoopsie Legends to Hoopsie Heroes";
            this.chkChangeHoopsieLegends.UseSelectable = true;
            // 
            // cboTheme
            // 
            this.cboTheme.FormattingEnabled = true;
            this.cboTheme.IntegralHeight = false;
            this.cboTheme.ItemHeight = 23;
            this.cboTheme.Items.AddRange(new object[] {
            "Light",
            "Dark"});
            this.cboTheme.Location = new System.Drawing.Point(63, 87);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(85, 29);
            this.cboTheme.TabIndex = 1;
            this.cboTheme.UseSelectable = true;
            this.cboTheme.SelectedIndexChanged += new System.EventHandler(this.cboTheme_SelectedIndexChanged);
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkAutoUpdate.Location = new System.Drawing.Point(8, 59);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(161, 19);
            this.chkAutoUpdate.TabIndex = 3;
            this.chkAutoUpdate.Text = "Auto Update Program";
            this.chkAutoUpdate.UseSelectable = true;
            // 
            // lblPreviousWinsNote
            // 
            this.lblPreviousWinsNote.AutoSize = true;
            this.lblPreviousWinsNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblPreviousWinsNote.Location = new System.Drawing.Point(145, 12);
            this.lblPreviousWinsNote.Name = "lblPreviousWinsNote";
            this.lblPreviousWinsNote.Size = new System.Drawing.Size(126, 19);
            this.lblPreviousWinsNote.TabIndex = 2;
            this.lblPreviousWinsNote.Text = "Before using tracker";
            // 
            // lblPreviousWins
            // 
            this.lblPreviousWins.AutoSize = true;
            this.lblPreviousWins.Location = new System.Drawing.Point(8, 12);
            this.lblPreviousWins.Name = "lblPreviousWins";
            this.lblPreviousWins.Size = new System.Drawing.Size(85, 19);
            this.lblPreviousWins.TabIndex = 0;
            this.lblPreviousWins.Text = "Previous Win";
            // 
            // txtPreviousWins
            // 
            // 
            // 
            // 
            this.txtPreviousWins.CustomButton.Image = null;
            this.txtPreviousWins.CustomButton.Location = new System.Drawing.Point(20, 1);
            this.txtPreviousWins.CustomButton.Name = "";
            this.txtPreviousWins.CustomButton.Size = new System.Drawing.Size(19, 19);
            this.txtPreviousWins.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtPreviousWins.CustomButton.TabIndex = 1;
            this.txtPreviousWins.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtPreviousWins.CustomButton.UseSelectable = true;
            this.txtPreviousWins.CustomButton.Visible = false;
            this.txtPreviousWins.Lines = new string[] {
        "0"};
            this.txtPreviousWins.Location = new System.Drawing.Point(99, 12);
            this.txtPreviousWins.MaxLength = 5;
            this.txtPreviousWins.Name = "txtPreviousWins";
            this.txtPreviousWins.PasswordChar = '\0';
            this.txtPreviousWins.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtPreviousWins.SelectedText = "";
            this.txtPreviousWins.SelectionLength = 0;
            this.txtPreviousWins.SelectionStart = 0;
            this.txtPreviousWins.ShortcutsEnabled = true;
            this.txtPreviousWins.Size = new System.Drawing.Size(40, 21);
            this.txtPreviousWins.TabIndex = 1;
            this.txtPreviousWins.Text = "0";
            this.txtPreviousWins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPreviousWins.UseSelectable = true;
            this.txtPreviousWins.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtPreviousWins.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtPreviousWins.Validating += new System.ComponentModel.CancelEventHandler(this.txtPreviousWins_Validating);
            // 
            // lblGameExeLocation
            // 
            this.lblGameExeLocation.AutoSize = true;
            this.lblGameExeLocation.Location = new System.Drawing.Point(8, 77);
            this.lblGameExeLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGameExeLocation.Name = "lblGameExeLocation";
            this.lblGameExeLocation.Size = new System.Drawing.Size(150, 19);
            this.lblGameExeLocation.TabIndex = 0;
            this.lblGameExeLocation.Text = "Game Shortcut Location";
            // 
            // txtGameExeLocation
            // 
            // 
            // 
            // 
            this.txtGameExeLocation.CustomButton.Image = null;
            this.txtGameExeLocation.CustomButton.Location = new System.Drawing.Point(369, 1);
            this.txtGameExeLocation.CustomButton.Name = "";
            this.txtGameExeLocation.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.txtGameExeLocation.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtGameExeLocation.CustomButton.TabIndex = 1;
            this.txtGameExeLocation.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtGameExeLocation.CustomButton.UseSelectable = true;
            this.txtGameExeLocation.CustomButton.Visible = false;
            this.txtGameExeLocation.Enabled = false;
            this.txtGameExeLocation.Lines = new string[0];
            this.txtGameExeLocation.Location = new System.Drawing.Point(162, 74);
            this.txtGameExeLocation.Margin = new System.Windows.Forms.Padding(2);
            this.txtGameExeLocation.MaxLength = 32767;
            this.txtGameExeLocation.Name = "txtGameExeLocation";
            this.txtGameExeLocation.PasswordChar = '\0';
            this.txtGameExeLocation.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtGameExeLocation.SelectedText = "";
            this.txtGameExeLocation.SelectionLength = 0;
            this.txtGameExeLocation.SelectionStart = 0;
            this.txtGameExeLocation.ShortcutsEnabled = true;
            this.txtGameExeLocation.Size = new System.Drawing.Size(393, 25);
            this.txtGameExeLocation.TabIndex = 37;
            this.txtGameExeLocation.UseSelectable = true;
            this.txtGameExeLocation.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtGameExeLocation.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // txtGameShortcutLocation
            // 
            // 
            // 
            // 
            this.txtGameShortcutLocation.CustomButton.Image = null;
            this.txtGameShortcutLocation.CustomButton.Location = new System.Drawing.Point(369, 1);
            this.txtGameShortcutLocation.CustomButton.Name = "";
            this.txtGameShortcutLocation.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.txtGameShortcutLocation.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtGameShortcutLocation.CustomButton.TabIndex = 1;
            this.txtGameShortcutLocation.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtGameShortcutLocation.CustomButton.UseSelectable = true;
            this.txtGameShortcutLocation.CustomButton.Visible = false;
            this.txtGameShortcutLocation.Enabled = false;
            this.txtGameShortcutLocation.Lines = new string[0];
            this.txtGameShortcutLocation.Location = new System.Drawing.Point(162, 74);
            this.txtGameShortcutLocation.Margin = new System.Windows.Forms.Padding(2);
            this.txtGameShortcutLocation.MaxLength = 32767;
            this.txtGameShortcutLocation.Name = "txtGameShortcutLocation";
            this.txtGameShortcutLocation.PasswordChar = '\0';
            this.txtGameShortcutLocation.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtGameShortcutLocation.SelectedText = "";
            this.txtGameShortcutLocation.SelectionLength = 0;
            this.txtGameShortcutLocation.SelectionStart = 0;
            this.txtGameShortcutLocation.ShortcutsEnabled = true;
            this.txtGameShortcutLocation.Size = new System.Drawing.Size(405, 25);
            this.txtGameShortcutLocation.TabIndex = 38;
            this.txtGameShortcutLocation.UseSelectable = true;
            this.txtGameShortcutLocation.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtGameShortcutLocation.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnGameExeLocationBrowse
            // 
            this.btnGameExeLocationBrowse.Location = new System.Drawing.Point(571, 74);
            this.btnGameExeLocationBrowse.Margin = new System.Windows.Forms.Padding(14, 11, 14, 11);
            this.btnGameExeLocationBrowse.Name = "btnGameExeLocationBrowse";
            this.btnGameExeLocationBrowse.Size = new System.Drawing.Size(62, 25);
            this.btnGameExeLocationBrowse.TabIndex = 2;
            this.btnGameExeLocationBrowse.Text = "Browse";
            this.btnGameExeLocationBrowse.UseSelectable = true;
            this.btnGameExeLocationBrowse.Click += new System.EventHandler(this.btnGameExeLocationBrowse_Click);
            // 
            // chkAutoLaunchGameOnStart
            // 
            this.chkAutoLaunchGameOnStart.AutoSize = true;
            this.chkAutoLaunchGameOnStart.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkAutoLaunchGameOnStart.Location = new System.Drawing.Point(8, 104);
            this.chkAutoLaunchGameOnStart.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoLaunchGameOnStart.Name = "chkAutoLaunchGameOnStart";
            this.chkAutoLaunchGameOnStart.Size = new System.Drawing.Size(226, 19);
            this.chkAutoLaunchGameOnStart.TabIndex = 3;
            this.chkAutoLaunchGameOnStart.Text = "Auto-launch Fall Guys on tracker";
            this.chkAutoLaunchGameOnStart.UseSelectable = true;
            // 
            // picLanguageSelection
            // 
            this.picLanguageSelection.Image = global::FallGuysStats.Properties.Resources.language_icon;
            this.picLanguageSelection.Location = new System.Drawing.Point(8, 122);
            this.picLanguageSelection.Name = "picLanguageSelection";
            this.picLanguageSelection.Size = new System.Drawing.Size(29, 29);
            this.picLanguageSelection.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLanguageSelection.TabIndex = 39;
            this.picLanguageSelection.TabStop = false;
            // 
            // chkIgnoreLevelTypeWhenSorting
            // 
            this.chkIgnoreLevelTypeWhenSorting.AutoSize = true;
            this.chkIgnoreLevelTypeWhenSorting.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chkIgnoreLevelTypeWhenSorting.Location = new System.Drawing.Point(8, 63);
            this.chkIgnoreLevelTypeWhenSorting.Margin = new System.Windows.Forms.Padding(2);
            this.chkIgnoreLevelTypeWhenSorting.Name = "chkIgnoreLevelTypeWhenSorting";
            this.chkIgnoreLevelTypeWhenSorting.Size = new System.Drawing.Size(216, 19);
            this.chkIgnoreLevelTypeWhenSorting.TabIndex = 0;
            this.chkIgnoreLevelTypeWhenSorting.Text = "Ignore Level Type when sorting";
            this.chkIgnoreLevelTypeWhenSorting.UseSelectable = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(778, 600);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // platformToolTip
            // 
            this.platformToolTip.Style = MetroFramework.MetroColorStyle.Blue;
            this.platformToolTip.StyleManager = null;
            this.platformToolTip.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // tileProgram
            // 
            this.tileProgram.ActiveControl = null;
            this.tileProgram.BackColor = System.Drawing.Color.LightGray;
            this.tileProgram.Location = new System.Drawing.Point(12, 73);
            this.tileProgram.Name = "tileProgram";
            this.tileProgram.Size = new System.Drawing.Size(200, 40);
            this.tileProgram.TabIndex = 9;
            this.tileProgram.Text = "Program";
            this.tileProgram.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileProgram.UseSelectable = true;
            this.tileProgram.Click += new System.EventHandler(this.changeTab);
            // 
            // panelProgram
            // 
            this.panelProgram.Controls.Add(this.chkAutoUpdate);
            this.panelProgram.Controls.Add(this.lblLogPath);
            this.panelProgram.Controls.Add(this.txtLogPath);
            this.panelProgram.Controls.Add(this.lblLogPathNote);
            this.panelProgram.HorizontalScrollbarBarColor = true;
            this.panelProgram.HorizontalScrollbarHighlightOnWheel = false;
            this.panelProgram.HorizontalScrollbarSize = 10;
            this.panelProgram.Location = new System.Drawing.Point(29, 15);
            this.panelProgram.Name = "panelProgram";
            this.panelProgram.Size = new System.Drawing.Size(647, 98);
            this.panelProgram.TabIndex = 10;
            this.panelProgram.VerticalScrollbarBarColor = true;
            this.panelProgram.VerticalScrollbarHighlightOnWheel = false;
            this.panelProgram.VerticalScrollbarSize = 10;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(43, 126);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(66, 19);
            this.lblLanguage.TabIndex = 100;
            this.lblLanguage.Text = "Language";
            // 
            // panelDisplay
            // 
            this.panelDisplay.Controls.Add(this.lblLanguage);
            this.panelDisplay.Controls.Add(this.lblTheme);
            this.panelDisplay.Controls.Add(this.lblPreviousWins);
            this.panelDisplay.Controls.Add(this.chkIgnoreLevelTypeWhenSorting);
            this.panelDisplay.Controls.Add(this.txtPreviousWins);
            this.panelDisplay.Controls.Add(this.chkChangeHoopsieLegends);
            this.panelDisplay.Controls.Add(this.lblPreviousWinsNote);
            this.panelDisplay.Controls.Add(this.cboMultilingual);
            this.panelDisplay.Controls.Add(this.picLanguageSelection);
            this.panelDisplay.Controls.Add(this.cboTheme);
            this.panelDisplay.HorizontalScrollbarBarColor = true;
            this.panelDisplay.HorizontalScrollbarHighlightOnWheel = false;
            this.panelDisplay.HorizontalScrollbarSize = 10;
            this.panelDisplay.Location = new System.Drawing.Point(29, 119);
            this.panelDisplay.Name = "panelDisplay";
            this.panelDisplay.Size = new System.Drawing.Size(647, 162);
            this.panelDisplay.TabIndex = 11;
            this.panelDisplay.VerticalScrollbarBarColor = true;
            this.panelDisplay.VerticalScrollbarHighlightOnWheel = false;
            this.panelDisplay.VerticalScrollbarSize = 10;
            // 
            // tileDisplay
            // 
            this.tileDisplay.ActiveControl = null;
            this.tileDisplay.BackColor = System.Drawing.Color.LightGray;
            this.tileDisplay.Location = new System.Drawing.Point(12, 119);
            this.tileDisplay.Name = "tileDisplay";
            this.tileDisplay.Size = new System.Drawing.Size(200, 40);
            this.tileDisplay.TabIndex = 12;
            this.tileDisplay.Text = "Display";
            this.tileDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileDisplay.UseSelectable = true;
            this.tileDisplay.Click += new System.EventHandler(this.changeTab);
            // 
            // panelOverlay
            // 
            this.panelOverlay.AutoScroll = true;
            this.panelOverlay.Controls.Add(this.grpOverlayFontExample);
            this.panelOverlay.Controls.Add(this.lblOverlayFont);
            this.panelOverlay.Controls.Add(this.btnSelectFont);
            this.panelOverlay.Controls.Add(this.btnResetOverlayFont);
            this.panelOverlay.Controls.Add(this.chkAutoChangeProfile);
            this.panelOverlay.Controls.Add(this.chkColorByRoundType);
            this.panelOverlay.Controls.Add(this.chkPlayerByConsoleType);
            this.panelOverlay.Controls.Add(this.chkOverlayOnTop);
            this.panelOverlay.Controls.Add(this.chkFlipped);
            this.panelOverlay.Controls.Add(this.lblOverlayColor);
            this.panelOverlay.Controls.Add(this.cboOverlayColor);
            this.panelOverlay.Controls.Add(this.lblOverlayBackground);
            this.panelOverlay.Controls.Add(this.cboOverlayBackground);
            this.panelOverlay.Controls.Add(this.grpCycleQualifyGold);
            this.panelOverlay.Controls.Add(this.chkHidePercentages);
            this.panelOverlay.Controls.Add(this.grpCycleFastestLongest);
            this.panelOverlay.Controls.Add(this.chkHideWinsInfo);
            this.panelOverlay.Controls.Add(this.chkHideRoundInfo);
            this.panelOverlay.Controls.Add(this.chkHideTimeInfo);
            this.panelOverlay.Controls.Add(this.chkShowTabs);
            this.panelOverlay.Controls.Add(this.cboFastestFilter);
            this.panelOverlay.Controls.Add(this.lblWinsFilter);
            this.panelOverlay.Controls.Add(this.cboQualifyFilter);
            this.panelOverlay.Controls.Add(this.lblFastestFilter);
            this.panelOverlay.Controls.Add(this.lblCycleTimeSecondsTag);
            this.panelOverlay.Controls.Add(this.cboWinsFilter);
            this.panelOverlay.Controls.Add(this.lblCycleTimeSeconds);
            this.panelOverlay.Controls.Add(this.txtCycleTimeSeconds);
            this.panelOverlay.Controls.Add(this.lblQualifyFilter);
            this.panelOverlay.Controls.Add(this.grpCycleWinFinalStreak);
            this.panelOverlay.Controls.Add(this.groupBox1);
            this.panelOverlay.HorizontalScrollbar = true;
            this.panelOverlay.HorizontalScrollbarBarColor = true;
            this.panelOverlay.HorizontalScrollbarHighlightOnWheel = false;
            this.panelOverlay.HorizontalScrollbarSize = 10;
            this.panelOverlay.Location = new System.Drawing.Point(756, 15);
            this.panelOverlay.Name = "panelOverlay";
            this.panelOverlay.Size = new System.Drawing.Size(647, 500);
            this.panelOverlay.TabIndex = 13;
            this.panelOverlay.VerticalScrollbar = true;
            this.panelOverlay.VerticalScrollbarBarColor = true;
            this.panelOverlay.VerticalScrollbarHighlightOnWheel = false;
            this.panelOverlay.VerticalScrollbarSize = 10;
            // 
            // cboOverlayBackground
            // 
            this.cboOverlayBackground.BorderColor = System.Drawing.Color.Gray;
            this.cboOverlayBackground.ButtonColor = System.Drawing.Color.DarkGray;
            this.cboOverlayBackground.DropDownHeight = 500;
            this.cboOverlayBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOverlayBackground.FormattingEnabled = true;
            this.cboOverlayBackground.IntegralHeight = false;
            this.cboOverlayBackground.ItemHeight = 12;
            this.cboOverlayBackground.Location = new System.Drawing.Point(168, 335);
            this.cboOverlayBackground.Name = "cboOverlayBackground";
            this.cboOverlayBackground.Size = new System.Drawing.Size(240, 20);
            this.cboOverlayBackground.TabIndex = 19;
            this.cboOverlayBackground.GotFocus += new System.EventHandler(this.cboOverlayBackground_GotFocus);
            this.cboOverlayBackground.LostFocus += new System.EventHandler(this.cboOverlayBackground_LostFocus);
            this.cboOverlayBackground.MouseEnter += new System.EventHandler(this.cboOverlayBackground_MouseEnter);
            this.cboOverlayBackground.MouseLeave += new System.EventHandler(this.cboOverlayBackground_MouseLeave);
            // 
            // tileOverlay
            // 
            this.tileOverlay.ActiveControl = null;
            this.tileOverlay.BackColor = System.Drawing.Color.LightGray;
            this.tileOverlay.Location = new System.Drawing.Point(12, 165);
            this.tileOverlay.Name = "tileOverlay";
            this.tileOverlay.Size = new System.Drawing.Size(200, 40);
            this.tileOverlay.TabIndex = 14;
            this.tileOverlay.Text = "Overlay";
            this.tileOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileOverlay.UseSelectable = true;
            this.tileOverlay.Click += new System.EventHandler(this.changeTab);
            // 
            // tileFallGuys
            // 
            this.tileFallGuys.ActiveControl = null;
            this.tileFallGuys.BackColor = System.Drawing.Color.LightGray;
            this.tileFallGuys.Location = new System.Drawing.Point(12, 211);
            this.tileFallGuys.Name = "tileFallGuys";
            this.tileFallGuys.Size = new System.Drawing.Size(200, 40);
            this.tileFallGuys.TabIndex = 15;
            this.tileFallGuys.Text = "Launch FallGuys";
            this.tileFallGuys.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileFallGuys.UseSelectable = true;
            this.tileFallGuys.Click += new System.EventHandler(this.changeTab);
            // 
            // panelFallGuys
            // 
            this.panelFallGuys.Controls.Add(this.chkAutoLaunchGameOnStart);
            this.panelFallGuys.Controls.Add(this.btnGameExeLocationBrowse);
            this.panelFallGuys.Controls.Add(this.txtGameShortcutLocation);
            this.panelFallGuys.Controls.Add(this.txtGameExeLocation);
            this.panelFallGuys.Controls.Add(this.lblGameExeLocation);
            this.panelFallGuys.Controls.Add(this.grpLaunchPlatform);
            this.panelFallGuys.HorizontalScrollbarBarColor = true;
            this.panelFallGuys.HorizontalScrollbarHighlightOnWheel = false;
            this.panelFallGuys.HorizontalScrollbarSize = 10;
            this.panelFallGuys.Location = new System.Drawing.Point(29, 286);
            this.panelFallGuys.Name = "panelFallGuys";
            this.panelFallGuys.Size = new System.Drawing.Size(647, 136);
            this.panelFallGuys.TabIndex = 16;
            this.panelFallGuys.VerticalScrollbarBarColor = true;
            this.panelFallGuys.VerticalScrollbarHighlightOnWheel = false;
            this.panelFallGuys.VerticalScrollbarSize = 10;
            // 
            // tileAbout
            // 
            this.tileAbout.ActiveControl = null;
            this.tileAbout.BackColor = System.Drawing.Color.LightGray;
            this.tileAbout.Location = new System.Drawing.Point(12, 257);
            this.tileAbout.Name = "tileAbout";
            this.tileAbout.Size = new System.Drawing.Size(200, 40);
            this.tileAbout.TabIndex = 17;
            this.tileAbout.Text = "About";
            this.tileAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tileAbout.UseSelectable = true;
            this.tileAbout.Click += new System.EventHandler(this.changeTab);
            // 
            // panelAbout
            // 
            this.panelAbout.AutoScroll = true;
            this.panelAbout.Controls.Add(this.lblupdateNote);
            this.panelAbout.Controls.Add(this.lbltpl4);
            this.panelAbout.Controls.Add(this.lbltpl3);
            this.panelAbout.Controls.Add(this.lbltpl2);
            this.panelAbout.Controls.Add(this.lblthirdpartyLicences);
            this.panelAbout.Controls.Add(this.lbltpl1);
            this.panelAbout.Controls.Add(this.fglink2);
            this.panelAbout.Controls.Add(this.fglink1);
            this.panelAbout.Controls.Add(this.btnCheckUpdates);
            this.panelAbout.Controls.Add(this.lblVersion);
            this.panelAbout.Controls.Add(this.lblLicence);
            this.panelAbout.HorizontalScrollbar = true;
            this.panelAbout.HorizontalScrollbarBarColor = true;
            this.panelAbout.HorizontalScrollbarHighlightOnWheel = false;
            this.panelAbout.HorizontalScrollbarSize = 10;
            this.panelAbout.Location = new System.Drawing.Point(29, 428);
            this.panelAbout.Name = "panelAbout";
            this.panelAbout.Size = new System.Drawing.Size(647, 500);
            this.panelAbout.TabIndex = 18;
            this.panelAbout.VerticalScrollbar = true;
            this.panelAbout.VerticalScrollbarBarColor = true;
            this.panelAbout.VerticalScrollbarHighlightOnWheel = false;
            this.panelAbout.VerticalScrollbarSize = 10;
            // 
            // lbltpl4
            // 
            this.lbltpl4.Location = new System.Drawing.Point(8, 670);
            this.lbltpl4.Name = "lbltpl4";
            this.lbltpl4.Size = new System.Drawing.Size(121, 23);
            this.lbltpl4.TabIndex = 13;
            this.lbltpl4.Text = "ScottPlot";
            this.lbltpl4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbltpl4.UseSelectable = true;
            this.lbltpl4.Click += new System.EventHandler(this.link_Click);
            // 
            // lbltpl3
            // 
            this.lbltpl3.Location = new System.Drawing.Point(8, 641);
            this.lbltpl3.Name = "lbltpl3";
            this.lbltpl3.Size = new System.Drawing.Size(121, 23);
            this.lbltpl3.TabIndex = 12;
            this.lbltpl3.Text = "MetroModernUI";
            this.lbltpl3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbltpl3.UseSelectable = true;
            this.lbltpl3.Click += new System.EventHandler(this.link_Click);
            // 
            // lbltpl2
            // 
            this.lbltpl2.Location = new System.Drawing.Point(8, 612);
            this.lbltpl2.Name = "lbltpl2";
            this.lbltpl2.Size = new System.Drawing.Size(121, 23);
            this.lbltpl2.TabIndex = 11;
            this.lbltpl2.Text = "Fody";
            this.lbltpl2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbltpl2.UseSelectable = true;
            this.lbltpl2.Click += new System.EventHandler(this.link_Click);
            // 
            // lblthirdpartyLicences
            // 
            this.lblthirdpartyLicences.AutoSize = true;
            this.lblthirdpartyLicences.Location = new System.Drawing.Point(8, 563);
            this.lblthirdpartyLicences.Name = "lblthirdpartyLicences";
            this.lblthirdpartyLicences.Size = new System.Drawing.Size(121, 19);
            this.lblthirdpartyLicences.TabIndex = 9;
            this.lblthirdpartyLicences.Text = "Thirdparty Licences";
            // 
            // lbltpl1
            // 
            this.lbltpl1.Location = new System.Drawing.Point(8, 585);
            this.lbltpl1.Name = "lbltpl1";
            this.lbltpl1.Size = new System.Drawing.Size(121, 23);
            this.lbltpl1.TabIndex = 8;
            this.lbltpl1.Text = "Costura.Fody";
            this.lbltpl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbltpl1.UseSelectable = true;
            this.lbltpl1.Click += new System.EventHandler(this.link_Click);
            // 
            // fglink2
            // 
            this.fglink2.Location = new System.Drawing.Point(204, 34);
            this.fglink2.Name = "fglink2";
            this.fglink2.Size = new System.Drawing.Size(83, 23);
            this.fglink2.TabIndex = 7;
            this.fglink2.Text = "Issue Tracker";
            this.fglink2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fglink2.UseSelectable = true;
            this.fglink2.Click += new System.EventHandler(this.link_Click);
            // 
            // fglink1
            // 
            this.fglink1.Location = new System.Drawing.Point(9, 34);
            this.fglink1.Name = "fglink1";
            this.fglink1.Size = new System.Drawing.Size(187, 23);
            this.fglink1.TabIndex = 6;
            this.fglink1.Text = "GitHub (ShootMe/FallGuysStats)";
            this.fglink1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fglink1.UseSelectable = true;
            this.fglink1.Click += new System.EventHandler(this.link_Click);
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.AutoSize = true;
            this.btnCheckUpdates.Location = new System.Drawing.Point(13, 90);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(51, 23);
            this.btnCheckUpdates.TabIndex = 3;
            this.btnCheckUpdates.Text = "Update";
            this.btnCheckUpdates.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheckUpdates.UseSelectable = true;
            this.btnCheckUpdates.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblVersion.Location = new System.Drawing.Point(8, 12);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(166, 25);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Fall Guys Stats v1.140";
            // 
            // lblLicence
            // 
            this.lblLicence.AutoSize = true;
            this.lblLicence.Location = new System.Drawing.Point(9, 126);
            this.lblLicence.Name = "lblLicence";
            this.lblLicence.Size = new System.Drawing.Size(577, 418);
            this.lblLicence.TabIndex = 5;
            this.lblLicence.Text = resources.GetString("lblLicence.Text");
            // 
            // lblupdateNote
            // 
            this.lblupdateNote.AutoSize = true;
            this.lblupdateNote.Location = new System.Drawing.Point(13, 68);
            this.lblupdateNote.Name = "lblupdateNote";
            this.lblupdateNote.Size = new System.Drawing.Size(0, 0);
            this.lblupdateNote.TabIndex = 14;
            // overlayOpacityToolTip
            // 
            this.overlayOpacityToolTip.Style = MetroFramework.MetroColorStyle.Blue;
            this.overlayOpacityToolTip.StyleManager = null;
            this.overlayOpacityToolTip.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(876, 650);
            this.Controls.Add(this.panelOverlay);
            this.Controls.Add(this.panelFallGuys);
            this.Controls.Add(this.panelDisplay);
            this.Controls.Add(this.panelAbout);
            this.Controls.Add(this.tileAbout);
            this.Controls.Add(this.tileFallGuys);
            this.Controls.Add(this.tileOverlay);
            this.Controls.Add(this.tileDisplay);
            this.Controls.Add(this.panelProgram);
            this.Controls.Add(this.tileProgram);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.ForeColor = System.Drawing.Color.Black;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 18);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.grpOverlayFontExample.ResumeLayout(false);
            this.grpCycleQualifyGold.ResumeLayout(false);
            this.grpCycleQualifyGold.PerformLayout();
            this.grpCycleFastestLongest.ResumeLayout(false);
            this.grpCycleFastestLongest.PerformLayout();
            this.grpCycleWinFinalStreak.ResumeLayout(false);
            this.grpCycleWinFinalStreak.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpLaunchPlatform.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPlatformCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEpicGames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSteam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLanguageSelection)).EndInit();
            this.panelProgram.ResumeLayout(false);
            this.panelProgram.PerformLayout();
            this.panelDisplay.ResumeLayout(false);
            this.panelDisplay.PerformLayout();
            this.panelOverlay.ResumeLayout(false);
            this.panelOverlay.PerformLayout();
            this.panelFallGuys.ResumeLayout(false);
            this.panelFallGuys.PerformLayout();
            this.panelAbout.ResumeLayout(false);
            this.panelAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        private MetroFramework.Controls.MetroLabel lblTheme;

        #endregion

        private MetroFramework.Controls.MetroLabel lblLogPath;
        private MetroFramework.Controls.MetroLabel lblLogPathNote;
        private MetroFramework.Controls.MetroTextBox txtLogPath;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroLabel lblCycleTimeSecondsTag;
        private MetroFramework.Controls.MetroLabel lblCycleTimeSeconds;
        private MetroFramework.Controls.MetroTextBox txtCycleTimeSeconds;
        private MetroFramework.Controls.MetroLabel lblPreviousWinsNote;
        private MetroFramework.Controls.MetroLabel lblPreviousWins;
        private MetroFramework.Controls.MetroTextBox txtPreviousWins;
        private MetroFramework.Controls.MetroCheckBox chkOverlayOnTop;
        private MetroFramework.Controls.MetroCheckBox chkPlayerByConsoleType;
        private MetroFramework.Controls.MetroCheckBox chkColorByRoundType;
        private MetroFramework.Controls.MetroCheckBox chkAutoChangeProfile;
        private MetroFramework.Controls.MetroComboBox cboFastestFilter;
        private MetroFramework.Controls.MetroLabel lblFastestFilter;
        private MetroFramework.Controls.MetroComboBox cboQualifyFilter;
        private MetroFramework.Controls.MetroLabel lblQualifyFilter;
        private MetroFramework.Controls.MetroComboBox cboWinsFilter;
        private MetroFramework.Controls.MetroComboBox cboMultilingual;
        private MetroFramework.Controls.MetroLabel lblWinsFilter;
        private MetroFramework.Controls.MetroCheckBox chkHideTimeInfo;
        private MetroFramework.Controls.MetroCheckBox chkHideRoundInfo;
        private MetroFramework.Controls.MetroCheckBox chkShowTabs;
        private MetroFramework.Controls.MetroComboBox cboTheme;
        private MetroFramework.Controls.MetroCheckBox chkAutoUpdate;
        private FallGuysStats.ImageComboBox cboOverlayBackground;
        private MetroFramework.Controls.MetroLabel lblOverlayBackground;
        private MetroFramework.Controls.MetroComboBox cboOverlayColor;
        private MetroFramework.Controls.MetroLabel lblOverlayColor;
        private MetroFramework.Controls.MetroLabel lblOverlayOpacity;
        private MetroFramework.Controls.MetroTrackBar trkOverlayOpacity;
        private MetroFramework.Controls.MetroCheckBox chkFlipped;
        private MetroFramework.Controls.MetroCheckBox chkHideWinsInfo;
        private MetroFramework.Controls.MetroCheckBox chkHidePercentages;
        private MetroFramework.Controls.MetroCheckBox chkChangeHoopsieLegends;
        private MetroFramework.Controls.MetroLabel lblGameExeLocation;
        private System.Windows.Forms.PictureBox picLanguageSelection;
        private MetroFramework.Controls.MetroTextBox txtGameExeLocation;
        private MetroFramework.Controls.MetroTextBox txtGameShortcutLocation;
        private MetroFramework.Controls.MetroButton btnGameExeLocationBrowse;
        private MetroFramework.Controls.MetroCheckBox chkAutoLaunchGameOnStart;
        private MetroFramework.Controls.MetroCheckBox chkIgnoreLevelTypeWhenSorting;
        private MetroFramework.Controls.MetroButton btnCancel;
        private System.Windows.Forms.GroupBox grpCycleQualifyGold;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowGold;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowQualify;
        private MetroFramework.Controls.MetroRadioButton chkCycleQualifyGold;
        private System.Windows.Forms.GroupBox grpCycleFastestLongest;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowLongest;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowFastest;
        private MetroFramework.Controls.MetroRadioButton chkCycleFastestLongest;
        private System.Windows.Forms.GroupBox grpCycleWinFinalStreak;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowFinalStreak;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowWinStreak;
        private MetroFramework.Controls.MetroRadioButton chkCycleWinFinalStreak;
        private System.Windows.Forms.GroupBox groupBox1;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowPing;
        private MetroFramework.Controls.MetroRadioButton chkOnlyShowPlayers;
        private MetroFramework.Controls.MetroRadioButton chkCyclePlayersPing;
        private MetroFramework.Controls.MetroLabel lblOverlayFont;
        private MetroFramework.Controls.MetroButton btnSelectFont;
        private System.Windows.Forms.FontDialog dlgOverlayFont;
        private System.Windows.Forms.Label lblOverlayFontExample;
        private System.Windows.Forms.GroupBox grpOverlayFontExample;
        private System.Windows.Forms.GroupBox grpLaunchPlatform;
        private System.Windows.Forms.PictureBox picSteam;
        private System.Windows.Forms.PictureBox picEpicGames;
        private System.Windows.Forms.PictureBox picPlatformCheck;
        private MetroFramework.Controls.MetroButton btnResetOverlayFont;
        private MetroFramework.Components.MetroToolTip platformToolTip;
        private MetroFramework.Controls.MetroTile tileProgram;
        private MetroFramework.Controls.MetroPanel panelProgram;
        private MetroFramework.Controls.MetroLabel lblLanguage;
        private MetroFramework.Controls.MetroPanel panelDisplay;
        private MetroFramework.Controls.MetroTile tileDisplay;
        private MetroFramework.Controls.MetroPanel panelOverlay;
        private MetroFramework.Controls.MetroTile tileOverlay;
        private MetroFramework.Controls.MetroTile tileFallGuys;
        private MetroFramework.Controls.MetroPanel panelFallGuys;
        private MetroFramework.Controls.MetroTile tileAbout;
        private MetroFramework.Controls.MetroPanel panelAbout;
        private MetroFramework.Controls.MetroLabel lblVersion;
        private MetroFramework.Controls.MetroButton btnCheckUpdates;
        private MetroFramework.Controls.MetroLabel lblLicence;
        private MetroFramework.Controls.MetroLink fglink1;
        private MetroFramework.Controls.MetroLabel lblthirdpartyLicences;
        private MetroFramework.Controls.MetroLink lbltpl1;
        private MetroFramework.Controls.MetroLink fglink2;
        private MetroFramework.Controls.MetroLink lbltpl2;
        private MetroFramework.Controls.MetroLink lbltpl4;
        private MetroFramework.Controls.MetroLink lbltpl3;
        private MetroFramework.Controls.MetroLabel lblupdateNote;
        private MetroFramework.Components.MetroToolTip overlayOpacityToolTip;
    }
}
