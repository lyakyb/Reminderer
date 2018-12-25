
using System;

namespace Reminderer.Models
{
    public class Schedule : Task
    {
        public Schedule()
        {
            DesiredDateTime = DateTime.Now;
        }

        private int _numDaysBeforeNotify;

        public int NumDaysBeforeNotify
        {
            get { return _numDaysBeforeNotify; }
            set { _numDaysBeforeNotify = value; OnPropertyChanged(); }
        }

        public TimeSpan TimeUntilDesiredDate()
        {
            return DesiredDateTime.Subtract(DateTime.Now);
        }
        public string TimeUntilDesiredDateText
        {
            get
            {
                var t = TimeUntilDesiredDate();
                if (t.Days == 0 && t.Hours != 0)
                {
                    return $"{t.Hours} hours left";
                }
                else if (t.Days == 0 && t.Hours == 0)
                {
                    return $"{t.Minutes} minutes left";
                }
                else
                {
                    return $"D-{t.Days + 1}";
                }
            }
        }

        public override bool ShouldNotifyToday()
        {
            if (NumDaysBeforeNotify == TimeUntilDesiredDate().Days + 1)
            {
                return true;
            }
            return false;
        }
        public override string DesiredDateTimeText => base.DesiredDateTimeText;
        public override string RepeatingDaysText
        {
            get
            {
                if (ShouldRemind && NumDaysBeforeNotify >= 0)
                {
                    return $"Notifying {NumDaysBeforeNotify} days before D-Day";
                }

                return "You did not request notification for this schedule";
            }
        }
    }
}
