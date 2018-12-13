using System.Collections.Generic;
using Reminderer.Models;
using System;
using Reminderer.Framework;

namespace Reminderer.Views.ScheduleListView
{
    class ScheduleListViewModel : BaseViewModel
    {
        private IList<Task> _scheduleList;

        public ScheduleListViewModel()
        {
            var tasks = new List<Task> { };
            tasks.Add(new Task { Description = "Test 1", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 2", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 3", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 4", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });

            DatabaseManager = new TaskDatabaseManager();
            DatabaseManager.CreateNewDatabase("test1");
            DatabaseManager.ConnectToDatabase("test1");

            DatabaseManager.CreateTasksTable();
            DatabaseManager.InsertTasks(tasks);

            _scheduleList = DatabaseManager.LoadSavedTasks();

            DatabaseManager.DisconnectFromDatabase();
        }

        



        public IList<Task> Tasks
        {
            get { return _scheduleList; }
            set { _scheduleList = value; }
        }
    }
}
