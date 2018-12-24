using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Reminderer.Framework
{
    public class TimerService
    {
        private static readonly TimerService _instance = new TimerService();
        private List<Timer> _timers = new List<Timer>();

        private TimerService()
        {            
        }
        static TimerService() { }
        public static TimerService instance { get { return _instance; } }

        public void ScheduleTaskForInterval(int hour, int min, double interval, Action action)
        {
        }

        public Timer ScheduleTaskFromNow(int hour, int min, Action action)
        {
            TimeSpan delaySpan = TimeSpan.FromMinutes(hour * 60 + min);
            var timer = new Timer(x =>
            {
                action.Invoke();
            }, null, delaySpan, TimeSpan.FromMilliseconds(-1));
            _timers.Add(timer);
            return timer;
        }

        public Timer ScheduleTaskForInterval(DateTime dateTime, double interval, Action action)
        {
            DateTime now = DateTime.Now;
            TimeSpan delay = new TimeSpan();
            if (now > dateTime)
            {
                delay = TimeSpan.Zero;
            } else
            {
                delay = dateTime - now;
            }
            delay = TimeSpan.FromMinutes(delay.TotalMinutes);
            Console.WriteLine($"delay in minutes: {delay.TotalMinutes}");
            TimeSpan intervalSpan = interval == 0 ? TimeSpan.FromMilliseconds(-1) : TimeSpan.FromMinutes(interval);
            var timer = new Timer(x =>
            {
                action.Invoke();
            }, null, delay, intervalSpan);
            _timers.Add(timer);
            return timer;
        }

    }
}
