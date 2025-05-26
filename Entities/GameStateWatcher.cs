﻿using System.Threading.Tasks;

namespace FallGuysStats {
    public class GameStateWatcher {
        private const int CheckDelay = 2000;

        private Task task;
        private bool running;
        private bool stop;

        public void Start() {
            if (this.running) return;

            this.stop = false;
            this.task = new Task(this.CheckGameState);
            this.task.Start();
        }
        
        public async Task Stop() {
            this.stop = true;
            while (this.running || this.task == null || this.task.Status == TaskStatus.Created) {
                await Task.Delay(50);
            }
            await Task.Run(() => this.task?.Wait());
        }

        private async void CheckGameState() {
            this.running = true;
            while (!this.stop) {
                if (!Stats.InShow) {
                    this.stop = true;
                    this.running = false;
                    return;
                }

                if (Utils.IsProcessRunning("FallGuys_client_game")) {
                    Stats.IsGameRunning = true;
                } else {
                    Stats.IsGameRunning = false;
                    Stats.IsClientHasBeenClosed = true;
                    this.stop = true;
                    this.running = false;
                }

                await Task.Delay(CheckDelay);
            }
        }
    }
}