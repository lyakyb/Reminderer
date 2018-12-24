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
        private bool _shouldRemind;
        private bool _notificationOn;
        private int _taskId;

        public Task()
        {
            DesiredDateTime = DateTime.Today;
        }

        public Task(string description, string extraDetail, DateTime desiredDateTime, bool shouldRemind)
        {
            Description = description;
            ExtraDetail = extraDetail;
            DesiredDateTime = desiredDateTime;
            ShouldRemind = shouldRemind;
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
        public int Id
        {
            get { return _taskId; }
            set { _taskId = value; }
        }
        public bool ShouldRemind
        {
            get { return _shouldRemind; }
            set { _shouldRemind = value; OnPropertyChanged(); }
        }
        public bool NotificationOn
        {
            get { return _notificationOn; }
            set { _notificationOn = value; OnPropertyChanged(); }
        }



        public virtual bool ShouldNotifyToday()
        {
            if (DesiredDateTime.Day == DateTime.Now.Day)
            {
                return true;
            }
            return false;
        }
        public virtual string DesiredDateTimeText
        {
            get { return DesiredDateTime.ToLongDateString(); }
        }
        public virtual string RepeatingDaysText
        {
            get
            {
                return "You should never see this msg";

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


//Task
//Reminder        Taskid  Desc    ExtraDesc   DDT Remind    Repeat  Type RptDays
//Schedule        Taskid  Desc    ExtraDesc   DDT Remind	#DaysNot		
