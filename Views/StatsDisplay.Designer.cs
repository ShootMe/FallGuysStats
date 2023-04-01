using System.Drawing;
using System.Windows.Forms;

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
            this.picGraph = new System.Windows.Forms.PictureBox();
            this.tgGraph = new MetroFramework.Controls.MetroToggle();
            this.chkWins = new MetroFramework.Controls.MetroCheckBox();
            this.chkFinals = new MetroFramework.Controls.MetroCheckBox();
            this.chkShows = new MetroFramework.Controls.MetroCheckBox();
            this.formsPlot = new ScottPlot.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // picGraph
            // 
            this.picGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picGraph.BackColor = System.Drawing.Color.Transparent;
            this.picGraph.Image = global::FallGuysStats.Properties.Resources.scatter_plot_icon;
            this.picGraph.Location = new System.Drawing.Point(882, 33);
            this.picGraph.Name = "picGraph";
            this.picGraph.Size = new System.Drawing.Size(20, 20);
            this.picGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGraph.TabIndex = 0;
            // 
            // tgGraph
            // 
            this.tgGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tgGraph.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgGraph.DisplayStatus = false;
            this.tgGraph.ForeColor = System.Drawing.Color.Red;
            this.tgGraph.Location = new System.Drawing.Point(848, 37);
            this.tgGraph.Name = "tgGraph";
            this.tgGraph.Size = new System.Drawing.Size(25, 16);
            this.tgGraph.TabIndex = 1;
            this.tgGraph.Text = "Off";
            this.tgGraph.UseCustomBackColor = true;
            this.tgGraph.UseCustomForeColor = true;
            this.tgGraph.UseSelectable = true;
            this.tgGraph.UseStyleColors = true;
            this.tgGraph.CheckStateChanged += new System.EventHandler(this.tgGraph_CheckStateChanged);
            // 
            // chkWins
            // 
            this.chkWins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWins.AutoSize = true;
            this.chkWins.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWins.ForeColor = System.Drawing.Color.Red;
            this.chkWins.Location = new System.Drawing.Point(961, 38);
            this.chkWins.Name = "chkWins";
            this.chkWins.Size = new System.Drawing.Size(49, 15);
            this.chkWins.TabIndex = 2;
            this.chkWins.Text = "Wins";
            this.chkWins.UseCustomForeColor = true;
            this.chkWins.UseSelectable = true;
            this.chkWins.CheckedChanged += new System.EventHandler(this.chkWins_CheckedChanged);
            // 
            // chkFinals
            // 
            this.chkFinals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFinals.AutoSize = true;
            this.chkFinals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkFinals.ForeColor = System.Drawing.Color.Green;
            this.chkFinals.Location = new System.Drawing.Point(1053, 38);
            this.chkFinals.Name = "chkFinals";
            this.chkFinals.Size = new System.Drawing.Size(53, 15);
            this.chkFinals.TabIndex = 3;
            this.chkFinals.Text = "Finals";
            this.chkFinals.UseCustomForeColor = true;
            this.chkFinals.UseSelectable = true;
            this.chkFinals.CheckedChanged += new System.EventHandler(this.chkFinals_CheckedChanged);
            // 
            // chkShows
            // 
            this.chkShows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShows.AutoSize = true;
            this.chkShows.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkShows.ForeColor = System.Drawing.Color.Blue;
            this.chkShows.Location = new System.Drawing.Point(1147, 38);
            this.chkShows.Name = "chkShows";
            this.chkShows.Size = new System.Drawing.Size(57, 15);
            this.chkShows.TabIndex = 4;
            this.chkShows.Text = "Shows";
            this.chkShows.UseCustomForeColor = true;
            this.chkShows.UseSelectable = true;
            this.chkShows.CheckedChanged += new System.EventHandler(this.chkShows_CheckedChanged);
            // 
            // formsPlot
            // 
            this.formsPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.formsPlot.Location = new System.Drawing.Point(0, 58);
            this.formsPlot.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.formsPlot.Name = "formsPlot";
            this.formsPlot.Size = new System.Drawing.Size(1280, 680);
            this.formsPlot.TabIndex = 5;
            this.formsPlot.MouseLeave += new System.EventHandler(this.formsPlot_MouseLeave);
            this.formsPlot.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlot_MouseMove);
            // 
            // StatsDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1294, 720);
            this.Controls.Add(this.formsPlot);
            this.Controls.Add(this.picGraph);
            this.Controls.Add(this.tgGraph);
            this.Controls.Add(this.chkShows);
            this.Controls.Add(this.chkFinals);
            this.Controls.Add(this.chkWins);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(15, 15);
            this.MinimumSize = new System.Drawing.Size(720, 360);
            this.Name = "StatsDisplay";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 0);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Stats Display";
            this.Load += new System.EventHandler(this.StatsDisplay_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StatsDisplay_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        #endregion
        private ScottPlot.FormsPlot formsPlot;
        private System.Windows.Forms.PictureBox picGraph;
        private MetroFramework.Controls.MetroToggle tgGraph;
        private MetroFramework.Controls.MetroCheckBox chkWins;
        private MetroFramework.Controls.MetroCheckBox chkFinals;
        private MetroFramework.Controls.MetroCheckBox chkShows;
    }
}