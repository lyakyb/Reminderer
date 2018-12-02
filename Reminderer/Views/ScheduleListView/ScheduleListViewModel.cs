using System.Collections.Generic;
using Reminderer.Models;
using System;

namespace Reminderer.Views.ScheduleListView
{
    class ScheduleListViewModel
    {
        private IList<Task> _scheduleList;

        public ScheduleListViewModel()
        {
            _scheduleList = new List<Task> { };
            _scheduleList.Add(new Task { Description = "Test 1", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { } });
            _scheduleList.Add(new Task { Description = "Test 2", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { } });
            _scheduleList.Add(new Task { Description = "Test 3", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { } });
            _scheduleList.Add(new Task { Description = "Test 4", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { } });
        }

        public IList<Task> Tasks
        {
            get { return _scheduleList; }
            set { _scheduleList = value; }
        }
    }
}
