using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class ServerPingWatcher {
        public Stats StatsForm { get; set; }
        
        private const int UpdateDelay = 2000;

        private bool running;
        private bool stop;

        private readonly Ping pingSender = new Ping();
        private PingReply pingReply;

        private readonly Random random = new Random();
        private int randomElement;
        private readonly int[] moreDelayValues = { 0, 100, 200, 300, 400, 500 };
        private int addMoreRandomDelay;
        private bool toggleRequestCountryInfoApi;

        public void Start() {
            if (this.running) { return; }
            
            this.stop = false;
            Task.Run(this.CheckServerPing);
            this.toggleRequestCountryInfoApi = false;
            Task.Run(this.GetIpToCountryInfo);
        }

        private async void CheckServerPing() {
            this.running = true;
            while (!this.stop) {
                try {
                    TimeSpan timeDiff = DateTime.UtcNow - Stats.ConnectedToServerDate;
                    if (!Stats.IsDisplayOverlayPing || !Stats.ToggleServerInfo || timeDiff.TotalMinutes >= 40) {
                        Stats.LastServerPing = 0;
                        Stats.IsBadServerPing = false;
                        this.stop = true;
                        this.running = false;
                        return;
                    }
                    
                    this.pingReply = this.pingSender.Send(Stats.LastServerIp, 1000, new byte[32]);
                    if (this.pingReply != null && this.pingReply.Status == IPStatus.Success) {
                        Stats.LastServerPing = this.pingReply.RoundtripTime;
                        Stats.IsBadServerPing = false;
                    } else {
                        Stats.LastServerPing = this.pingReply?.RoundtripTime ?? 0;
                        Stats.IsBadServerPing = true;
                    }
                    
                    this.randomElement = this.random.Next(0, this.moreDelayValues.Length);
                    this.addMoreRandomDelay = this.moreDelayValues[this.randomElement];
                } catch {
                    Stats.LastServerPing = 0;
                    Stats.IsBadServerPing = true;
                    this.randomElement = this.random.Next(0, this.moreDelayValues.Length);
                    this.addMoreRandomDelay = this.moreDelayValues[this.randomElement];
                }
                await Task.Delay(UpdateDelay + addMoreRandomDelay);
            }
        }

        private void GetIpToCountryInfo() {
            while (!this.toggleRequestCountryInfoApi) {
                try {
                    this.toggleRequestCountryInfoApi = true;
                    Stats.LastCountryAlpha2Code = this.StatsForm.GetIpToCountryCode(Stats.LastServerIp).ToLower();
                                                    
                    if (this.StatsForm.CurrentSettings.NotifyServerConnected && !string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                        this.StatsForm.ShowNotification(Multilingual.GetWord("message_connected_to_server_caption"),
                            $"{Multilingual.GetWord("message_connected_to_server_prefix")}{Multilingual.GetCountryName(Stats.LastCountryAlpha2Code)}{Multilingual.GetWord("message_connected_to_server_suffix")}",
                            System.Windows.Forms.ToolTipIcon.Info, 2000);
                    }

                    if (string.IsNullOrEmpty(Stats.LastCountryAlpha2Code)) {
                        this.toggleRequestCountryInfoApi = false;
                    }
                } catch {
                    this.toggleRequestCountryInfoApi = false;
                    Stats.LastCountryAlpha2Code = string.Empty;
                }
            }
        }
    }
}