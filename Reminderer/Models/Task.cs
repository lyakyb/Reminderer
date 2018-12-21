using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Models
{
    public class Task : INotifyPropertyChanged
    {
        public enum TaskType {
            Schedule = 0,
            Reminder = 1
        }

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
        
        private string _description;
        private string _extraDetail;
        private DateTime _desiredDateTime;
        private int _importance;
        private bool _shouldRemind;
        private bool _shouldRepeat;
        private List<int> _repeatingDays; // 0~6, Sun~Sat
        private bool _isFromSavedTime;
        private bool _isAtSetInterval;
        private bool _isAtSpecificTime;
        private int _numDaysBeforeNotify;
        private TaskType _type;
        private int _taskId;

        public Task()
        {
            RepeatingDays = new List<int>();
            DesiredDateTime = DateTime.Today;
        }

        public Task(string description, string extraDetail, DateTime desiredDateTime, bool shouldRemind, bool shouldRepeat, List<int> repeatingDays)
        {
            Description = description;
            ExtraDetail = extraDetail;
            DesiredDateTime = desiredDateTime;
            ShouldRemind = shouldRemind;
            ShouldRepeat = shouldRepeat;
            RepeatingDays = repeatingDays;
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }
        
        public string ExtraDetail
        {
            get { return _extraDetail; }
            set { _extraDetail = value; OnPropertyChanged(); }
        }

        public TaskType Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public DateTime DesiredDateTime
        {
            get { return _desiredDateTime; }
            set { _desiredDateTime = value; OnPropertyChanged(); }
        }

        public int TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        public int Importance
        {
            get { return _importance; }
            set { _importance = value; OnPropertyChanged(); }
        }

        public bool ShouldRemind
        {
            get { return _shouldRemind; }
            set { _shouldRemind = value; OnPropertyChanged(); }
        }

        public bool ShouldRepeat
        {
            get { return _shouldRepeat; }
            set { _shouldRepeat = value; OnPropertyChanged(); }
        }

        public List<int> RepeatingDays
        {
            get { return _repeatingDays; }
            set { _repeatingDays = value; OnPropertyChanged(); }
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
                }else if (t.Days == 0 && t.Hours == 0)
                {
                    return $"{t.Minutes} minutes left";
                }else
                {
                    return $"D-{t.Days}";
                }
            }
        }

        public int ReminderSetting
        {
            get {
                if (IsFromSavedTime)
                {
                    return 1;
                }else if(IsAtSetInterval)
                {
                    return 2;
                } else if(IsAtSpecificTime)
                {
                    return 3;
                }
                return 0;
            }
        }
        public bool IsFromSavedTime
        {
            get { return _isFromSavedTime; }
            set { _isFromSavedTime = value; OnPropertyChanged(); }
        }
        public bool IsAtSetInterval
        {
            get { return _isAtSetInterval; }
            set { _isAtSetInterval = value; OnPropertyChanged(); }
        }
        public bool IsAtSpecificTime
        {
            get { return _isAtSpecificTime; }
            set { _isAtSpecificTime = value; OnPropertyChanged(); }
        }

        public int NumDaysBeforeNotify
        {
            get { return _numDaysBeforeNotify; }
            set { _numDaysBeforeNotify = value; OnPropertyChanged(); }
        }

        public bool ShouldNotifyToday()
        {
            if (RepeatingDays.Count == 7 || RepeatingDays.Contains(DayOfWeekConverter(DateTime.Now.DayOfWeek)))
            {
                return true;
            }

            return false;
        }

        private int DayOfWeekConverter(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 0;
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    return -1;
            }
        }

        public string DesiredDateTimeText
        {
            get
            {
                if (Type == TaskType.Schedule)
                {
                    return DesiredDateTime.ToLongDateString();
                }
                else
                {
                    if (IsFromSavedTime)
                    {
                        var hourDiff = DesiredDateTime.Hour - DateTime.Now.Hour;
                        var minDiff = DesiredDateTime.Minute - DateTime.Now.Minute;
                        if (minDiff < 0) minDiff = 0;

                        return $"Notifying in {hourDiff} hours and {minDiff}minutes";
                    }
                    else if (IsAtSetInterval)
                    {
                        return $"Notifies Every {DesiredDateTime.Hour} hours and {DesiredDateTime.Minute} minutes";                        
                    }
                    else if (IsAtSpecificTime)
                    {
                        string s = "Notifying";
                        if (DesiredDateTime.Hour > 12)
                        {
                            s = $"{s} at {DesiredDateTime.Hour - 12}:{DesiredDateTime.Minute} PM"; 
                        } else if (DesiredDateTime.Hour == 12)
                        {
                            s = $"{s} at 12:{DesiredDateTime.Minute} PM";
                        }
                        else 
                        {
                            s = $"{s} at {DesiredDateTime.Hour}:{DesiredDateTime.Minute} AM";
                        }
                        return s;
                    }

                                                                                           /*     Long date pattern: "dddd, MMMM dd, yyyy"
                                                                            Long date string:  "Wednesday, May 16, 2001"
                                                                            Long time string:  "3:02:15 AM"
                                                                            Short date string:  "5/16/2001"
                                                                            Short time string:  "3:02 AM"
                                                                                                    */
                }
                return "";
            }
        }
        public string RepeatingDaysText
        {
            get
            {
                if (Type == TaskType.Schedule && ShouldRemind && NumDaysBeforeNotify >= 0)
                {
                    return $"Notifying {NumDaysBeforeNotify} days before D-Day";
                }

                if (RepeatingDays == null || RepeatingDays.Count == 0)
                    return "Does not repeat";

                string text = $"Repeats on: {repeatingDayConverter(RepeatingDays.First())}";
                
                for (int i=1; i<RepeatingDays.Count; i++)
                {
                    text = $"{text} ,{repeatingDayConverter(RepeatingDays[i])}";
                }
                
                return text;
            }
        }

        private string repeatingDayConverter(int day)
        {
            switch (day)
            {
                case 0:
                    return "Sun";
                case 1:
                    return "Mon";
                case 2:
                    return "Tue";
                case 3:
                    return "Wed";
                case 4:
                    return "Thur";
                case 5:
                    return "Fri";
                case 6:
                    return "Sat";
                case 7:
                    return "Everyday";
                default:
                    return "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
