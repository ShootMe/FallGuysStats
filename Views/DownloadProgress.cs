using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using MetroFramework;

namespace FallGuysStats {
    public partial class DownloadProgress : MetroFramework.Forms.MetroForm {
        public ZipWebClient ZipWebClient { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }

        public DownloadProgress() {
            this.InitializeComponent();
        }

        private void Progress_Load(object sender, EventArgs e) {
            this.SetTheme(Stats.CurrentTheme);
            this.ChangeLanguage();
            this.DownloadNewVersion();
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.lblDownloadDescription.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.mpbProgressBar.Theme = theme;
        }
        
        private void zipWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            if (this.mpbProgressBar.Value != e.ProgressPercentage) {
                this.mpbProgressBar.Value = e.ProgressPercentage;
                this.mpbProgressBar.Invalidate();
            }
        }
        private void zipWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_complete");
            this.lblDownloadDescription.Refresh();
            string exeName = null;
            using (ZipArchive zipFile = new ZipArchive(new FileStream(this.FileName, FileMode.Open), ZipArchiveMode.Read)) {
                foreach (var entry in zipFile.Entries) {
                    if (entry.Name.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) > 0) {
                        exeName = entry.Name;
                    }
                    if (File.Exists(entry.Name)) {
                        File.Move(entry.Name, $"{entry.Name}.bak");
                    }
                    entry.ExtractToFile(entry.Name, true);
                }
            }
            File.Delete(this.FileName);
            Process.Start(new ProcessStartInfo(exeName));
            this.Close();
        }
        
        private void DownloadNewVersion() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_updating_program");
            this.ZipWebClient.DownloadProgressChanged += this.zipWebClient_DownloadProgressChanged;
            this.ZipWebClient.DownloadFileCompleted += this.zipWebClient_DownloadFileCompleted;
            this.ZipWebClient.DownloadFileAsync(new Uri(this.DownloadUrl), this.FileName);
        }

        private void ChangeLanguage() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_program");
            this.lblDownloadDescription.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
        }
    }
}