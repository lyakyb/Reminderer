using Reminderer.Commands;
using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Views
{
    class ChoiceViewModel : BaseViewModel, IRemindererViewModel
    {
        public ChoiceViewModel(){}
        public ChoiceViewModel(TaskDatabaseManager taskDatabaseManager) : base(taskDatabaseManager)
        {
            ScheduleCommand = new DelegateCommand(executeScheduleCommand, canExecuteSchedule);
            ReminderCommand = new DelegateCommand(executeRemindercommand, canExecuteReminder);
        }

        private DelegateCommand _scheduleCommand;
        public DelegateCommand ScheduleCommand
        {
            get { return _scheduleCommand; }
            set { _scheduleCommand = value; }
        }
        private bool canExecuteSchedule(object obj)
        {
            return true;
        }
        private void executeScheduleCommand(object obj)
        {
            Console.WriteLine("ScheduleCommand fired");

            Mediator.Broadcast(Constants.ShowAddEditView, false);
        }

        private DelegateCommand _reminderCommand;
        public DelegateCommand ReminderCommand
        {
            get { return _reminderCommand; }
            set { _reminderCommand = value; }
        }
        private bool canExecuteReminder(object obj)
        {
            return true;
        }
        private void executeRemindercommand(object obj)
        {
            Console.WriteLine("Remindercommand fired");

            Mediator.Broadcast(Constants.ShowAddEditView, true);
        }

    }
}
