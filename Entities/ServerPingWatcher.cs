using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class ServerPingWatcher {
        private const int UpdateDelay = 2000;

        private bool running;
        private bool stop;

        private readonly Ping pingSender = new Ping();
        private PingReply pingReply;

        private readonly Random random = new Random();
        private int randomElement;
        private readonly int[] moreDelayValues = { 0, 100, 200, 300, 400, 500 };
        private int addMoreRandomDelay;

        public void Start() {
            if (this.running) { return; }
            
            this.stop = false;
            Task.Run(this.CheckServerPing);
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
    }
}