using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using MetroFramework;

namespace FallGuysStats {
    public partial class UpdateProgress : MetroFramework.Forms.MetroForm {
        public Stats StatsForm { get; set; }
        public ZipWebClient ZipWebClient { get; set; }
        public string FileName { get; set; }
        private int percentage;

        public UpdateProgress() => this.InitializeComponent();

        private void Progress_Load(object sender, EventArgs e) {
            this.SetTheme(this.StatsForm.CurrentSettings.Theme == 0 ? MetroThemeStyle.Light : this.StatsForm.CurrentSettings.Theme == 1 ? MetroThemeStyle.Dark : MetroThemeStyle.Default);
            this.ChangeLanguage();
            this.DownloadNewVersion();
        }

        private void SetTheme(MetroThemeStyle theme) {
            this.Theme = theme;
            this.lblDownloadDescription.ForeColor = theme == MetroThemeStyle.Light ? Color.Black : Color.DarkGray;
            this.mpbProgressBar.Theme = theme;
        }
        
        private void web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            if (this.percentage != e.ProgressPercentage) {
                this.percentage = e.ProgressPercentage;
                this.mpbProgressBar.Value = e.ProgressPercentage;
                this.lblDownloadDescription.Text = Multilingual.GetWord("main_updating_program");
                this.Refresh();
            }
        }
        private void web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_complete");
            this.Refresh();
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
            ZipWebClient.DownloadProgressChanged += web_DownloadProgressChanged;
            ZipWebClient.DownloadFileCompleted += web_DownloadFileCompleted;
            ZipWebClient.DownloadFileAsync(new Uri("https://github.com/ShootMe/FallGuysStats/releases/latest/download/FallGuysStats.zip"), this.FileName);
        }

        private void ChangeLanguage() {
            this.lblDownloadDescription.Text = Multilingual.GetWord("main_update_program");
            this.lblDownloadDescription.Font = new Font(Overlay.GetDefaultFontFamilies(Stats.CurrentLanguage), 18, Stats.CurrentLanguage > 1 ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        }
    }
}