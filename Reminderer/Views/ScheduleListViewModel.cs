using System.Collections.Generic;
using Reminderer.Models;
using System;
using Reminderer.Framework;

namespace Reminderer.Views.ScheduleListView
{
    class ScheduleListViewModel
    {
        private IList<Task> _scheduleList;

        public ScheduleListViewModel()
        {
            var tasks = new List<Task> { };
            tasks.Add(new Task { Description = "Test 1", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 2", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 3", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 4", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });

            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.CreateNewDatabase("test1");
            databaseManager.ConnectToDatabase("test1");

            databaseManager.CreateTasksTable("tasks_test_1");
            databaseManager.Insert("tasks_test_1", tasks);

            _scheduleList = databaseManager.LoadSavedTasks("tasks_test_1");
        }

        public IList<Task> Tasks
        {
            get { return _scheduleList; }
            set { _scheduleList = value; }
        }
    }
}
