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
        private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatsDisplay));
      this.chkWins = new System.Windows.Forms.CheckBox();
      this.chkFinals = new System.Windows.Forms.CheckBox();
      this.chkShows = new System.Windows.Forms.CheckBox();
      this.graph = new FallGuysStats.Graph();
      ((System.ComponentModel.ISupportInitialize)(this.graph)).BeginInit();
      this.SuspendLayout();
      // 
      // chkWins
      // 
      this.chkWins.AutoSize = true;
      this.chkWins.Location = new System.Drawing.Point(18, 20);
      this.chkWins.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkWins.Name = "chkWins";
      this.chkWins.Size = new System.Drawing.Size(70, 24);
      this.chkWins.TabIndex = 1;
      this.chkWins.Text = "Wins";
      this.chkWins.UseVisualStyleBackColor = true;
      this.chkWins.CheckedChanged += new System.EventHandler(this.chkWins_CheckedChanged);
      // 
      // chkFinals
      // 
      this.chkFinals.AutoSize = true;
      this.chkFinals.Location = new System.Drawing.Point(102, 20);
      this.chkFinals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkFinals.Name = "chkFinals";
      this.chkFinals.Size = new System.Drawing.Size(77, 24);
      this.chkFinals.TabIndex = 2;
      this.chkFinals.Text = "Finals";
      this.chkFinals.UseVisualStyleBackColor = true;
      this.chkFinals.CheckedChanged += new System.EventHandler(this.chkFinals_CheckedChanged);
      // 
      // chkShows
      // 
      this.chkShows.AutoSize = true;
      this.chkShows.Location = new System.Drawing.Point(190, 20);
      this.chkShows.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.chkShows.Name = "chkShows";
      this.chkShows.Size = new System.Drawing.Size(83, 24);
      this.chkShows.TabIndex = 3;
      this.chkShows.Text = "Shows";
      this.chkShows.UseVisualStyleBackColor = true;
      this.chkShows.CheckedChanged += new System.EventHandler(this.chkShows_CheckedChanged);
      // 
      // graph
      // 
      this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.graph.BackColor = System.Drawing.Color.Transparent;
      this.graph.BackgroundColor = System.Drawing.Color.Transparent;
      this.graph.ErrorImage = null;
      this.graph.InitialImage = null;
      this.graph.Location = new System.Drawing.Point(0, 55);
      this.graph.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.graph.Name = "graph";
      this.graph.Opacity = 0;
      this.graph.Size = new System.Drawing.Size(921, 772);
      this.graph.TabIndex = 0;
      this.graph.TabStop = false;
      // 
      // StatsDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
      this.ClientSize = new System.Drawing.Size(921, 828);
      this.Controls.Add(this.chkShows);
      this.Controls.Add(this.chkFinals);
      this.Controls.Add(this.chkWins);
      this.Controls.Add(this.graph);
      this.ForeColor = System.Drawing.Color.Black;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "StatsDisplay";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Stats Display";
      this.Load += new System.EventHandler(this.StatsDisplay_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StatsDisplay_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.graph)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private Graph graph;
        private System.Windows.Forms.CheckBox chkWins;
        private System.Windows.Forms.CheckBox chkFinals;
        private System.Windows.Forms.CheckBox chkShows;
    }
}