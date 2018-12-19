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

        public void ScheduleTaskForInterval(DateTime dateTime, double interval, Action action)
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

            var timer = new Timer(x =>
            {
                action.Invoke();
            }, null, delay, TimeSpan.FromHours(interval));
            _timers.Add(timer);
        }
    }
}
