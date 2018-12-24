using Reminderer.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reminderer.Models;
using System.Windows.Input;
using System.ComponentModel;
using Reminderer.Framework;
using Reminderer.Repositories;

namespace Reminderer.Views
{
    class AddEditViewModel : BaseViewModel, IRemindererViewModel
    {
        public List<int> NumDaysOptions
        {
            get { return new List<int>{ 1,2,3,4,5,6,7}; }
        }
        private Task _newTask;
        public Task NewTask
        {
            get { return _newTask; }
            set {
                if (_newTask != null)
                    _newTask.PropertyChanged -= Task_PropertyChanged;
                _newTask = value;
                if (_newTask != null)
                    _newTask.PropertyChanged += Task_PropertyChanged;
                ReminderSelected = value.GetType() == typeof(Reminder) ? true : false;
            }
        }
        private int _desiredHour;
        public int DesiredHour { get { return _desiredHour; } set { _desiredHour = value; } }
        private int _desiredMinute;
        public int DesiredMinute { get { return _desiredMinute; } set { _desiredMinute = value; } }
        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set { _isEditing = value; }
        }
        private bool _reminderSelected;
        public bool ReminderSelected
        {
            get { return _reminderSelected; }
            set { _reminderSelected = value; }
        }

        private NotificationManager _notificationManager;

        public void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddCommand.RaiseCanExecuteChanged();
        }
        
        public AddEditViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;

            ReminderSelected = false;
            AddCommand = new DelegateCommand(executeAddCommand, canExecuteAdd);
            NewTask = new Task();
        }

        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
            set { _addCommand = value; }
        }
        private void executeAddCommand(object obj)
        {
            //adding logic
            NewTask.DesiredDateTime = NewTask.DesiredDateTime.AddHours(DesiredHour);
            NewTask.DesiredDateTime = NewTask.DesiredDateTime.AddMinutes(DesiredMinute);

            if (ReminderSelected)
            {
                if (!((Reminder)NewTask).ShouldRepeat)
                {
                   // ((Reminder)NewTask).RepeatingDays.RemoveRange(0, ((Reminder)NewTask).RepeatingDays.Count);
                }

                if (((Reminder)NewTask).Type == Reminder.ReminderType.IsFromSavedTime)
                {
                    NewTask.DesiredDateTime = DateTime.Now.AddHours(DesiredHour).AddMinutes(DesiredMinute);
                }
                if (((Reminder)NewTask).Type == Reminder.ReminderType.IsAtSetInterval)
                {
                    NewTask.DesiredDateTime = DateTime.Today.AddHours(DesiredHour).AddMinutes(DesiredMinute);
                }
                if (((Reminder)NewTask).Type == Reminder.ReminderType.IsAtSpecificTime)
                {
                    NewTask.DesiredDateTime = DateTime.Today.AddHours(DesiredHour).AddMinutes(DesiredMinute);
                }

                if (IsEditing)
                {
                    _notificationManager.EditReminder((Reminder)NewTask);
                } else
                {
                    _notificationManager.CreateReminder((Reminder)NewTask);
                }
            }
            else
            {
                if (IsEditing)
                {
                    _notificationManager.EditSchedule((Schedule)NewTask);
                }
                else
                {
                    _notificationManager.CreateSchedule((Schedule)NewTask);
                }
            }

            NewTask = new Task();
            DesiredHour = 0;
            DesiredMinute = 0;
            IsEditing = false;
            ReminderSelected = false;
            Mediator.Broadcast(Constants.ShowListView);    
        }

        private bool canExecuteAdd(object obj)
        {
            if (ReminderSelected)
            {
                return NewTask != null && !string.IsNullOrEmpty(NewTask.Description) && !string.IsNullOrWhiteSpace(NewTask.Description) && NewTask.DesiredDateTime != null && ((Reminder)NewTask).Type != Reminder.ReminderType.None;
                
            } else
            {
                return NewTask != null && !string.IsNullOrEmpty(NewTask.Description) && !string.IsNullOrWhiteSpace(NewTask.Description) && NewTask.DesiredDateTime != null;
            }


        }

    }
}
