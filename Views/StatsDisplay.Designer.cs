using System.Drawing;

namespace FallGuysStats {
    partial class StatsDisplay {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatsDisplay));
            this.graph = new FallGuysStats.Graph();
            this.chkWins = new MetroFramework.Controls.MetroCheckBox();
            this.chkFinals = new MetroFramework.Controls.MetroCheckBox();
            this.chkShows = new MetroFramework.Controls.MetroCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.graph)).BeginInit();
            this.SuspendLayout();
            // 
            // graph
            // 
            this.graph.BackColor = System.Drawing.Color.Transparent;
            this.graph.BackgroundColor = System.Drawing.Color.Transparent;
            this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph.ErrorImage = null;
            this.graph.InitialImage = null;
            this.graph.Location = new System.Drawing.Point(20, 60);
            this.graph.Name = "graph";
            this.graph.Opacity = 0;
            this.graph.Size = new System.Drawing.Size(1240, 460);
            this.graph.TabIndex = 0;
            this.graph.TabStop = false;
            // 
            // chkWins
            // 
            this.chkWins.AutoSize = true;
            this.chkWins.ForeColor = System.Drawing.Color.Red;
            this.chkWins.Location = new System.Drawing.Point(958, 38);
            this.chkWins.Name = "chkWins";
            this.chkWins.Size = new System.Drawing.Size(84, 15);
            this.chkWins.TabIndex = 1;
            this.chkWins.Text = "Streak Wins";
            this.chkWins.UseCustomForeColor = true;
            this.chkWins.UseSelectable = true;
            this.chkWins.CheckedChanged += new System.EventHandler(this.chkWins_CheckedChanged);
            // 
            // chkFinals
            // 
            this.chkFinals.AutoSize = true;
            this.chkFinals.ForeColor = System.Drawing.Color.Green;
            this.chkFinals.Location = new System.Drawing.Point(1054, 38);
            this.chkFinals.Name = "chkFinals";
            this.chkFinals.Size = new System.Drawing.Size(88, 15);
            this.chkFinals.TabIndex = 2;
            this.chkFinals.Text = "Streak Finals";
            this.chkFinals.UseCustomForeColor = true;
            this.chkFinals.UseSelectable = true;
            this.chkFinals.CheckedChanged += new System.EventHandler(this.chkFinals_CheckedChanged);
            // 
            // chkShows
            // 
            this.chkShows.AutoSize = true;
            this.chkShows.ForeColor = System.Drawing.Color.Blue;
            this.chkShows.Location = new System.Drawing.Point(1151, 38);
            this.chkShows.Name = "chkShows";
            this.chkShows.Size = new System.Drawing.Size(57, 15);
            this.chkShows.TabIndex = 3;
            this.chkShows.Text = "Shows";
            this.chkShows.UseCustomForeColor = true;
            this.chkShows.UseSelectable = true;
            this.chkShows.CheckedChanged += new System.EventHandler(this.chkShows_CheckedChanged);
            // 
            // StatsDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1280, 540);
            this.Controls.Add(this.chkShows);
            this.Controls.Add(this.chkFinals);
            this.Controls.Add(this.chkWins);
            this.Controls.Add(this.graph);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "StatsDisplay";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Stats Display";
            this.Load += new System.EventHandler(this.StatsDisplay_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StatsDisplay_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.graph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private FallGuysStats.Graph graph;
        private MetroFramework.Controls.MetroCheckBox chkWins;
        private MetroFramework.Controls.MetroCheckBox chkFinals;
        private MetroFramework.Controls.MetroCheckBox chkShows;
    }
}