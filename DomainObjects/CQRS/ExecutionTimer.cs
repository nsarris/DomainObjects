using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS
{
    public class ExecutionTimer : IDisposable
    {
        public class Elapsed
        {
            public TimeSpan TimeSpan;
            public long Ticks;
            public long Milliseconds;
        }

        public class TimerStateEventArgs : EventArgs
        {
            public string Tag { get; set; }
            public Elapsed Elapsed { get; set; }
        }

        public delegate void TimerStateEventHandler(object sender, TimerStateEventArgs e);

        public event TimerStateEventHandler TimerStarted;
        public event TimerStateEventHandler TimerStopped;

        Stopwatch mainTimer = new Stopwatch();
        Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();

        public ExecutionTimer()
        {
            timers.Add("", mainTimer);
        }
        public void Start()
        {
            mainTimer.Start();
            OnTimerStarted("");
        }

        public void Stop()
        {
            mainTimer.Stop();
            OnTimerStopped("");
        }

        Stopwatch GetTimer(string Tag, bool Add = false)
        {
            if (string.IsNullOrEmpty(Tag))
                return mainTimer;

            Stopwatch w;
            if (!timers.TryGetValue(Tag, out w))
            {
                if (!Add) return null;
                w = new Stopwatch();
                timers.Add(Tag, w);
            }
            return w;
        }
        public void Start(string Tag = null)
        {
            GetTimer(Tag, true).Start();
            OnTimerStarted(Tag);
        }

        public void Stop(string Tag = null)
        {
            var w = GetTimer(Tag);
            if (w != null && w.IsRunning)
                w.Stop();
            OnTimerStopped(Tag);
        }

        public void Dispose()
        {
            if (mainTimer.IsRunning)
                mainTimer.Stop();
            foreach (var t in timers.Values.Where(t => t.IsRunning))
                t.Stop();
        }

        private void OnTimerStarted(string tag)
        {
            if (TimerStarted != null)
                TimerStarted(this, new TimerStateEventArgs() { Tag = tag, Elapsed = GetElapsed(tag) });
        }

        private void OnTimerStopped(string tag)
        {
            if (TimerStopped != null)
                TimerStopped(this, new TimerStateEventArgs() { Tag = tag, Elapsed = GetElapsed(tag) });
        }

        public TimeSpan ElapsedTimeSpan
        {
            get
            {
                return mainTimer.Elapsed;
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return mainTimer.ElapsedMilliseconds;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                return mainTimer.ElapsedTicks;
            }
        }

        public TimeSpan GetElapsedTimeSpan(string Tag)
        {
            var w = GetTimer(Tag);
            if (w != null)
                return w.Elapsed;
            else
                return default(TimeSpan);
        }
        public long GetElapsedMilliseconds(string Tag)
        {
            var w = GetTimer(Tag);
            if (w != null)
                return w.ElapsedMilliseconds;
            else
                return 0;
        }

        public long GetElapsedTicks(string Tag)
        {
            var w = GetTimer(Tag);
            if (w != null)
                return w.ElapsedTicks;
            else
                return 0;
        }

        public Elapsed GetElapsed(string tag)
        {
            var w = GetTimer(tag);
            if (w == null)
                return new Elapsed();
            else
                return new Elapsed()
                {
                    TimeSpan = w.Elapsed,
                    Ticks = w.ElapsedTicks,
                    Milliseconds = w.ElapsedMilliseconds
                };
        }

        public Dictionary<string, Elapsed> GetElapsedTimes()
        {
            return timers.ToDictionary(
                x => x.Key,
                x => new Elapsed()
                {
                    TimeSpan = x.Value.Elapsed,
                    Ticks = x.Value.ElapsedTicks,
                    Milliseconds = x.Value.ElapsedMilliseconds
                });
        }

        public void ResetTimers()
        {
            foreach (var t in timers.Values)
            {
                if (t.IsRunning)
                    t.Stop();
                t.Reset();
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine,
                timers.Select(x =>
                    (string.IsNullOrEmpty(x.Key) ? (timers.Count == 1 ? "" : "Main : ") : x.Key + " : ")
                    + x.Value.Elapsed.ToString()
                    + (x.Value.IsRunning ? " (Running)" : "")));
        }
    }
}
