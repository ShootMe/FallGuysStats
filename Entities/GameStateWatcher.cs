using System;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class GameStateWatcher {
        private readonly object gameStateCheckLock = new object();
        private const int CheckDelay = 2000;

        private Task gameStateCheckTask;
        private bool running;
        private bool stop;

        public void Start() {
            lock (this.gameStateCheckLock) {
                if (this.running || (this.gameStateCheckTask != null && this.gameStateCheckTask.Status != TaskStatus.RanToCompletion)) { return; }

                this.stop = false;
                this.gameStateCheckTask = new Task(this.CheckGameState);
                this.gameStateCheckTask.Start();
            }
        }

        private async void CheckGameState() {
            this.running = true;
            while (!this.stop) {
                lock (this.gameStateCheckLock) {
                    if (!Stats.InShow) {
                        this.stop = true;
                        this.running = false;
                        return;
                    }
                    
                    if (Stats.IsClientRunning()) {
                        Stats.IsGameRunning = true;
                    } else {
                        Stats.IsGameRunning = false;
                        Stats.IsClientHasBeenClosed = true;
                        this.stop = true;
                        this.running = false;
                    }
                }

                await Task.Delay(CheckDelay);
            }
        }
    }
}