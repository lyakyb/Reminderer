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

        public DateTime DesiredDateTime
        {
            get { return _desiredDateTime; }
            set { _desiredDateTime = value; OnPropertyChanged(); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
