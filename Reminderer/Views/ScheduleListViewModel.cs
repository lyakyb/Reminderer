using System.Collections.Generic;
using Reminderer.Models;
using System;
using Reminderer.Framework;
using Reminderer.Commands;

namespace Reminderer.Views
{
    class ScheduleListViewModel : BaseViewModel, IRemindererViewModel
    {
        private IList<Task> _scheduleList;


        public ScheduleListViewModel()
        {
        }

        public ScheduleListViewModel(TaskDatabaseManager taskDatabaseManager):base(taskDatabaseManager)
        {
            var tasks = new List<Task> { };
            tasks.Add(new Task { Description = "Test 1", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 2", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 3", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            tasks.Add(new Task { Description = "Test 4", DesiredDateTime = new DateTime(), Importance = 1, ShouldRemind = true, ShouldRepeat = false, RepeatingDays = { 'm', 't', 'w' } });
            
            DatabaseManager.CreateNewDatabase("test1");
            DatabaseManager.ConnectToDatabase("test1");

            DatabaseManager.CreateTasksTable();
            DatabaseManager.InsertTasks(tasks);

            Tasks = DatabaseManager.LoadSavedTasks();

            DatabaseManager.DisconnectFromDatabase();

            NewTaskCommand = new DelegateCommand(executeNewTaskCommand, canExecuteNewTask);
        }


        private DelegateCommand _newTaskCommand;
        public DelegateCommand NewTaskCommand
        {
            get { return _newTaskCommand; }
            set { _newTaskCommand = value; }
        }
        private void executeNewTaskCommand(object obj)
        {
            Mediator.Broadcast(Constants.ShowChoiceView);
        }
        private bool canExecuteNewTask(object obj)
        {
            return true;
        }


        public IList<Task> Tasks
        {
            get { return _scheduleList; }
            set { _scheduleList = value; }
        }
    }
}
