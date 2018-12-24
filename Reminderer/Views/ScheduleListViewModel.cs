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
        private IList<Reminder> _reminders;
        private IList<Schedule> _schedules;
        
        public ScheduleListViewModel()
        {
            NewTaskCommand = new DelegateCommand(executeNewTaskCommand, canExecuteNewTask);
            EditCommand = new DelegateCommand(executeEditCommand, canExecuteEditCommand);
            DeleteCommand = new DelegateCommand(executeDeleteCommand, canExecuteDeleteCommand);

            tasksUpdated();

            Mediator.Subscribe(Constants.TasksUpdated, tasksUpdated);
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

        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
            set { _deleteCommand = value; }
        }
        private void executeDeleteCommand(object obj)
        {
            if (obj == null) return;
            if (obj.GetType() == typeof(Reminder))
            {
                RemindererManager.Instance.DeleteReminder((Reminder)obj);
            }
            else if (obj.GetType() == typeof(Schedule))
            {
                RemindererManager.Instance.DeleteSchedule((Schedule)obj);
            }
        }

        private bool canExecuteDeleteCommand(object obj)
        {
            return true;
        }

        private void tasksUpdated(object obj = null)
        {
            Schedules = RemindererManager.Instance.Schedules;
            Reminders = RemindererManager.Instance.Reminders;
        }


        public IList<Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; }
        }

        public IList<Reminder> Reminders
        {
            get { return _reminders; }
            set { _reminders = value; OnPropertyChanged("Reminders"); }
        }

        public IList<Schedule> Schedules
        {
            get { return _schedules; }
            set { _schedules = value; OnPropertyChanged("Schedules"); }
        }
    }
}
