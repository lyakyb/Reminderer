
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Reminderer.Models
{
    public class Reminder : Task
    {
        public enum Days
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Everyday = 7,
            None = -1
        }

        public enum ReminderType
        {
            IsFromSavedTime = 0,
            IsAtSetInterval = 1,
            IsAtSpecificTime = 2,
            None = -1
        }

        public Reminder()
        {
            RepeatingDays = new List<Days>();
            DesiredDateTime = DateTime.Now;
            Type = ReminderType.None;
            ShouldRemind = true;
            ShouldRepeat = false;
        }

        private List<Days> _repeatingDays; // 0~6, Sun~Sat
        private bool _isFromSavedTime;
        private bool _isAtSetInterval;
        private bool _isAtSpecificTime;
        private bool _shouldRepeat;
        private ReminderType _type;

        public List<Days> RepeatingDays
        {
            get { return _repeatingDays; }
            set { _repeatingDays = value; OnPropertyChanged(); }
        }
        public bool IsFromSavedTime
        {
            get { return _isFromSavedTime; }
            set { _isFromSavedTime = value;
                Type = ReminderType.IsFromSavedTime;
                OnPropertyChanged(); }
        }
        public bool IsAtSetInterval
        {
            get { return _isAtSetInterval; }
            set { _isAtSetInterval = value;
                Type = ReminderType.IsAtSetInterval;
                OnPropertyChanged(); }
        }
        public bool IsAtSpecificTime
        {
            get { return _isAtSpecificTime; }
            set { _isAtSpecificTime = value;
                Type = ReminderType.IsAtSpecificTime;
                OnPropertyChanged(); }
        }
        public bool ShouldRepeat
        {
            get { return _shouldRepeat; }
            set { _shouldRepeat = value; OnPropertyChanged(); }
        }
        public ReminderType Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public override bool ShouldNotifyToday()
        {
            if (RepeatingDays.Count == 7 || RepeatingDays.Contains(DayOfWeekConverter(DateTime.Now.DayOfWeek)))
            {
                return true;
            }

            return false;
        }

        public override string DesiredDateTimeText
        {
            get
            {
                if (Type == ReminderType.IsFromSavedTime)
                {
                    var hourDiff = DesiredDateTime.Hour - DateTime.Now.Hour;
                    var minDiff = DesiredDateTime.Minute - DateTime.Now.Minute;
                    if (minDiff < 0) minDiff = 0;

                    return $"Notifying at {DesiredDateTime.ToLongTimeString()}, in {hourDiff}:{minDiff}";
                }
                else if (Type == ReminderType.IsAtSetInterval)
                {
                    return $"Notifying Every {DesiredDateTime.Hour} hours and {DesiredDateTime.Minute} minutes";
                }
                else if (Type == ReminderType.IsAtSpecificTime)
                {
                    string s = "Notifying";
                    if (DesiredDateTime.Hour > 12)
                    {
                        s = $"{s} at {DesiredDateTime.Hour - 12}:{DesiredDateTime.Minute} PM";
                    }
                    else if (DesiredDateTime.Hour == 12)
                    {
                        s = $"{s} at 12:{DesiredDateTime.Minute} PM";
                    }
                    else
                    {
                        s = $"{s} at {DesiredDateTime.Hour}:{DesiredDateTime.Minute} AM";
                    }
                    return s;
                }
                return "Type is not set";
            }
        }
        public override string RepeatingDaysText
        {
            get
            {
                if (RepeatingDays == null || RepeatingDays.Count == 0)
                    return "Does not repeat";

                string text = $"Repeats on: {repeatingDayConverter(RepeatingDays[0])}";

                for (int i = 1; i < RepeatingDays.Count; i++)
                {
                    text = $"{text} ,{repeatingDayConverter(RepeatingDays[i])}";
                }

                return text;
            }
        }

        public static Days DayOfWeekConverter(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return Days.Sunday;
                case DayOfWeek.Monday:
                    return Days.Monday;
                case DayOfWeek.Tuesday:
                    return Days.Tuesday;
                case DayOfWeek.Wednesday:
                    return Days.Wednesday;
                case DayOfWeek.Thursday:
                    return Days.Thursday;
                case DayOfWeek.Friday:
                    return Days.Friday;
                case DayOfWeek.Saturday:
                    return Days.Saturday;
                default:
                    return Days.None;
            }
        }
        public static Days StringToDaysConverter(string s)
        {
            switch (s)
            {
                case "Sunday":
                    return Days.Sunday;
                case "Monday":
                    return Days.Monday;
                case "Tuesday":
                    return Days.Tuesday;
                case "Wednesday":
                    return Days.Wednesday;
                case "Thursday":
                    return Days.Thursday;
                case "Friday":
                    return Days.Friday;
                case "Saturday":
                    return Days.Saturday;
                default:
                    return Days.None;
            }
        }
        private string repeatingDayConverter(Days day)
        {
            switch (day)
            {
                case Days.Sunday:
                    return "Sun";
                case Days.Monday:
                    return "Mon";
                case Days.Tuesday:
                    return "Tue";
                case Days.Wednesday:
                    return "Wed";
                case Days.Thursday:
                    return "Thur";
                case Days.Friday:
                    return "Fri";
                case Days.Saturday:
                    return "Sat";
                case Days.Everyday:
                    return "Everyday";
                case Days.None:
                    return "No repeat";
                default:
                    return "";
            }
        }
        
    }
}
