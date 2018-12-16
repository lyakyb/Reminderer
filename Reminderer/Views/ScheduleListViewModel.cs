using System.Collections.Generic;
using Reminderer.Models;
using System;
using Reminderer.Framework;
using Reminderer.Commands;

namespace Reminderer.Views
{
    class ScheduleListViewModel : BaseViewModel, IRemindererViewModel
    {
        private IList<Task> _tasks;
        private IList<Task> _reminders;
        private IList<Task> _schedules;
        
        
        public ScheduleListViewModel()
        {
        }

        public ScheduleListViewModel(TaskDatabaseManager taskDatabaseManager):base(taskDatabaseManager)
        {
            DatabaseManager.CreateNewDatabase("test1");
            DatabaseManager.ConnectToDatabase("test1");

            DatabaseManager.CreateTasksTable();

            UpdateTasks();
            DatabaseManager.DisconnectFromDatabase();

            NewTaskCommand = new DelegateCommand(executeNewTaskCommand, canExecuteNewTask);
            EditCommand = new DelegateCommand(executeEditCommand, canExecuteEditCommand);
        }

        public void UpdateTasks()
        {
            Reminders = new List<Task>();
            Schedules = new List<Task>();

            DatabaseManager.ConnectToDatabase("test1");
            Tasks = DatabaseManager.LoadSavedTasks();
            foreach (Task t in Tasks)
            {
                if (t.Type == 0)
                {
                    Schedules.Add(t);
                } else
                {
                    Reminders.Add(t);
                }
            }
            DatabaseManager.DisconnectFromDatabase();
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

        private DelegateCommand _editCommand;
        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
            set { _editCommand = value; }
        }
        private void executeEditCommand(object obj)
        {
            if (obj == null) return;
            Mediator.Broadcast(Constants.ShowAddEditView, obj);
        }
        private bool canExecuteEditCommand(object obj)
        {
            return true;
        }


        public IList<Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; }
        }

        public IList<Task> Reminders
        {
            get { return _reminders; }
            set { _reminders = value; }
        }

        public IList<Task> Schedules
        {
            get { return _schedules; }
            set { _schedules = value; }
        }
    }
}
