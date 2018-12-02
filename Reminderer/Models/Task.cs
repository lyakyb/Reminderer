using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Models
{
    public class Task
    {
        private string _description;
        private DateTime _desiredDateTime;
        private int _importance;
        private bool _shouldRemind;
        private bool _shouldRepeat;
        private List<char> _repeatingDays;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DateTime DesiredDateTime
        {
            get { return _desiredDateTime; }
            set { _desiredDateTime = value; }
        }

        public int Importance
        {
            get { return _importance; }
            set { _importance = value; }
        }

        public bool ShouldRemind
        {
            get { return _shouldRemind; }
            set { _shouldRemind = value; }
        }

        public bool ShouldRepeat
        {
            get { return _shouldRepeat; }
            set { _shouldRepeat = value; }
        }

        public List<char> RepeatingDays
        {
            get { return _repeatingDays; }
            set { _repeatingDays = value; }
        }

        public TimeSpan TimeUntilDesiredDate()
        {
            return DesiredDateTime.Subtract(DateTime.Now);
        }


    }
}
