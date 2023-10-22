using System;
using System.Threading.Tasks;

namespace FallGuysStats {
    public class GameStateWatcher {
        private readonly object lockObject = new object();
        private const int CheckDelay = 2000;

        private Task task;
        private bool running;
        private bool stop;

        public void Start() {
            if (this.running || (this.task != null && this.task.Status != TaskStatus.RanToCompletion)) { return; }

            this.stop = false;
            this.task = new Task(this.CheckGameState);
            this.task.Start();
        }

        private async void CheckGameState() {
            this.running = true;
            while (!this.stop) {
                lock (this.lockObject) {
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