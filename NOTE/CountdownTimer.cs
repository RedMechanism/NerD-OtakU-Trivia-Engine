using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NOTE
{
    public class CountdownTimer
    {
        #region Constructor
        readonly DispatcherTimer timer = new DispatcherTimer();
        public CountdownTimer(TimeSpan duration)
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => TimerTick();

            Duration = duration;

            Reset();
        }
        #endregion

        #region Event Handlers
        private void TimerTick()
        {

            CurrentTime = CurrentTime - TimeSpan.FromSeconds(1);

            OnTick();
            if (Complete)
            {
                Stop();
                CurrentTime = TimeSpan.Zero;
                OnCompleted();
            }
        }

        public delegate void TimerTickHandler(TimeSpan timerValue);
        public event TimerTickHandler TickEvent;

        protected void OnTimerValueChanged(TimeSpan timerValue)
        {
            if (TickEvent != null)
            {
                TickEvent(timerValue);
            }
        }
        #endregion

        #region Timer Properties

        private TimeSpan currentTime;

        public TimeSpan CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    OnTimerValueChanged(this.currentTime);
                }
            }
        }

        public bool Complete
        {
            get
            {
                return CurrentTime <= TimeSpan.Zero;
            }
        }

        private TimeSpan duration;

        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    Reset();
                }
            }
        }

        public enum TimerState
        {
            Running,
            Stopped,
            Complete
        }

        public TimerState Status { get; set; }
        #endregion

        #region Methods

        public void Start()
        {
            timer.Start();
            OnStart();
        }

        public void Stop()
        {
            timer.Stop();
            OnStop();
        }

        public void Reset()
        {
            Stop();
            CurrentTime = Duration;
            OnReset();
        }

        #region State Uodate Methods
        private void OnTick()
        {
            Status = TimerState.Running;

        }

        private void OnCompleted()
        {
            Status = TimerState.Complete;
        }

        private void OnReset()
        {
            Status = TimerState.Stopped;
        }

        private void OnStart()
        {
            Status = TimerState.Running;
        }

        private void OnStop()
        {
            Status = TimerState.Stopped;
        }

        public TimeSpan TimerValue
        {
            get
            {
                return CurrentTime;
            }
        }

        #endregion
        #endregion
    }
}
