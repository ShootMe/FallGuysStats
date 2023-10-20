using System;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class GameStateWatcher {
        private readonly object gameStateCheckLock = new object();
        private const int CheckDelay = 2000;

        private bool running;
        private bool stop;

        public event Action<string> OnError;

        public void Start() {
            if (this.running) { return; }

            this.stop = false;
            Task.Run(this.CheckGameState);
        }

        private async void CheckGameState() {
            this.running = true;
            while (!this.stop) {
                if (!Stats.InShow) {
                    this.stop = true;
                    this.running = false;
                    return;
                }
                
                lock (this.gameStateCheckLock) {
                    try {
                        if (Stats.IsClientRunning()) {
                            Stats.IsGameRunning = true;
                        } else {
                            Stats.IsGameRunning = false;
                            Stats.IsGameHasBeenClosed = true;
                            this.stop = true;
                            this.running = false;
                        }
                    } catch (Exception ex) {
                        this.OnError?.Invoke(ex.ToString());
                    }
                }

                await Task.Delay(CheckDelay);
            }
        }
    }
}