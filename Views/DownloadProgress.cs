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
        public ZipWebClient ZipWebClient { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        private int _percentage;

        public DownloadProgress() => this.InitializeComponent();

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
            if (this._percentage != e.ProgressPercentage) {
                this._percentage = e.ProgressPercentage;
                this.mpbProgressBar.Value = e.ProgressPercentage;
                this.lblDownloadDescription.Text = Multilingual.GetWord("main_updating_program");
                this.Invalidate();
            }
        }
        private void zipWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
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
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_complete");
            this.Invalidate();
            File.Delete(this.FileName);
            Process.Start(new ProcessStartInfo(exeName));
            this.Close();
        }
        
        private void DownloadNewVersion() {
            this.ZipWebClient.DownloadProgressChanged += this.zipWebClient_DownloadProgressChanged;
            this.ZipWebClient.DownloadFileCompleted += this.zipWebClient_DownloadFileCompleted;
            this.ZipWebClient.DownloadFileAsync(new Uri(this.DownloadUrl), this.FileName);
        }

        private void ChangeLanguage() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_program");
            this.lblDownloadDescription.Font = new Font(Overlay.GetDefaultFontFamilies(Stats.CurrentLanguage), 18, Stats.CurrentLanguage > 1 ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        }
    }
}