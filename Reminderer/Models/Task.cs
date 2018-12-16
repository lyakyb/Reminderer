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
        private string _description;
        private string _extraDetail;
        private DateTime _desiredDateTime;
        private int _importance;
        private bool _shouldRemind;
        private bool _shouldRepeat;
        private List<int> _repeatingDays; // 0~6, Sun~Sat
        private bool _isFromSavedTime;
        private bool _isAtSetInterval;
        private int _type;
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

        public int Type
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

        public bool FromSavedTime
        {
            get { return _isFromSavedTime; }
            set { _isFromSavedTime = value; OnPropertyChanged(); }
        }

        public bool IsAtSetInterval
        {
            get { return _isAtSetInterval; }
            set { _isAtSetInterval = value; OnPropertyChanged(); }
        }

        public string RepeatingDaysText
        {
            get
            {
                if (RepeatingDays == null || RepeatingDays.Count == 0)
                    return "-";

                string text = repeatingDayConverter(RepeatingDays.First());
                
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
