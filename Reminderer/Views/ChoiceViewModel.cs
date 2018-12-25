using Reminderer.Commands;
using Reminderer.Framework;
using Reminderer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Views
{
    class ChoiceViewModel : BaseViewModel, IRemindererViewModel
    {
        private NotificationManager _notificationManager;

        public ChoiceViewModel(NotificationManager notificationManager){
            _notificationManager = notificationManager;

            ScheduleCommand = new DelegateCommand(executeScheduleCommand, canExecuteSchedule);
            ReminderCommand = new DelegateCommand(executeRemindercommand, canExecuteReminder);
            BackCommand = new DelegateCommand(executeBackCommand, canExecuteBack);

        }

        private DelegateCommand _backCommand;
        public DelegateCommand BackCommand
        {
            get { return _backCommand; }
            set { _backCommand = value; }
        }
        private bool canExecuteBack(object obj)
        {
            return true;
        }
        private void executeBackCommand(object obj)
        {
            Mediator.Broadcast(Constants.ShowListView);
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
            Mediator.Broadcast(Constants.ShowAddEditView, true);
        }

    }
}
