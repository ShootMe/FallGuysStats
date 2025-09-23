using System;
using System.Timers;

namespace FallGuysStats {
    public delegate void TimerCallbackDelegate(object sender, ElapsedEventArgs e);
    public class TimerAbsolute : Timer {
        private DateTime m_dueTime;
        private readonly TimerCallbackDelegate callback;

        public TimerAbsolute(TimerCallbackDelegate cb) : base() {
            this.callback = cb;
            this.Elapsed += this.ElapsedAction;
            this.AutoReset = true;
        }

        public void Start(double interval) {
            this.m_dueTime = DateTime.Now.AddMilliseconds(interval);
            // Timer tick is 1 second
            this.Interval = 1000;
            base.Start();
        }

        private void ElapsedAction(object sender, ElapsedEventArgs e) {
            if (DateTime.Now >= m_dueTime) {
                this.callback(sender, e);
                base.Stop();
            }
        }
    }
}