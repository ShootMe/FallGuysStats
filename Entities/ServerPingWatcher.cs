using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class ServerPingWatcher {
        private const int CheckDelay = 2000;

        private Task task;
        private bool running;
        private bool stop;

        private readonly Ping pingSender = new Ping();
        private PingReply pingReply;

        private readonly Random random = new Random();
        private int randomElement;
        private readonly int[] moreDelayValues = { 200, 400, 600, 800, 1000 };
        private int addMoreRandomDelay;

        public void Start() {
            if (this.running) { return; }
        
            this.stop = false;
            this.task = new Task(this.CheckServerPing);
            this.task.Start();
        }

        private async void CheckServerPing() {
            this.running = true;
            while (!this.stop) {
                TimeSpan timeDiff = DateTime.UtcNow - Stats.ConnectedToServerDate;
                if (!Stats.IsDisplayOverlayPing || !Stats.IsConnectedToServer || Stats.IsClientHasBeenClosed || timeDiff.TotalMinutes >= 40) {
                    Stats.LastServerPing = 0;
                    Stats.IsBadServerPing = false;
                    this.stop = true;
                    this.running = false;
                    return;
                }

                try {
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

                await Task.Delay(CheckDelay + this.addMoreRandomDelay);
            }
        }
    }
}