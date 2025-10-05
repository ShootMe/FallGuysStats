using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using MetroFramework;

namespace FallGuysStats {
    public partial class DownloadProgress : MetroFramework.Forms.MetroForm {
        public string CurrentExeName { get; set; }
        public ZipWebClient ZipWebClient { get; set; }
        public string ZipFileName { get; set; }
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
            using (ZipArchive zipFile = new ZipArchive(new FileStream(this.ZipFileName, FileMode.Open), ZipArchiveMode.Read)) {
                foreach (var entry in zipFile.Entries) {
                    if (File.Exists($"{Stats.CURRENTDIR}{entry.Name}")) {
                        File.Move($"{Stats.CURRENTDIR}{entry.Name}", $"{Stats.CURRENTDIR}{entry.Name}.bak");
                    }
                    if (entry.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
                        if (!File.Exists($"{Stats.CURRENTDIR}{this.CurrentExeName}.bak")) {
                            File.Move($"{Stats.CURRENTDIR}{this.CurrentExeName}", $"{Stats.CURRENTDIR}{this.CurrentExeName}.bak");
                        }
                        entry.ExtractToFile($"{Stats.CURRENTDIR}{this.CurrentExeName}", true);
                    } else {
                        entry.ExtractToFile($"{Stats.CURRENTDIR}{entry.Name}", true);
                    }
                }
            }
            File.Delete(this.ZipFileName);
            Process.Start(new ProcessStartInfo($"{Stats.CURRENTDIR}{this.CurrentExeName}"));
            Process.GetCurrentProcess().Kill();
        }
        
        private void DownloadNewVersion() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_updating_program");
            this.ZipWebClient.DownloadProgressChanged += this.zipWebClient_DownloadProgressChanged;
            this.ZipWebClient.DownloadFileCompleted += this.zipWebClient_DownloadFileCompleted;
            this.ZipWebClient.DownloadFileAsync(new Uri(this.DownloadUrl), this.ZipFileName);
        }
        
        private void ChangeLanguage() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_program");
            this.lblDownloadDescription.Font = Overlay.GetDefaultFont(18, Stats.CurrentLanguage);
        }
    }
}