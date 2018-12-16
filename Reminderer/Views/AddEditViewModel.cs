using Reminderer.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reminderer.Models;
using System.Windows.Input;
using System.ComponentModel;
using Reminderer.Framework;

namespace Reminderer.Views
{
    class AddEditViewModel : BaseViewModel, IRemindererViewModel
    {
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
            }
        }

        private int _desiredHour;
        public int DesiredHour { get { return _desiredHour; } set { _desiredHour = value;  } }
        private int _desiredMinute;
        public int DesiredMinute { get { return _desiredMinute; } set { _desiredMinute = value; } }

        private bool _reminderSelected;
        public bool ReminderSelected
        {
            get { return _reminderSelected; }
            set { _reminderSelected = value; Console.WriteLine($"reminderSelected: {value}"); }
        }

        public void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddCommand.RaiseCanExecuteChanged();
        }
        
        public AddEditViewModel()
        {
        }
        public AddEditViewModel(TaskDatabaseManager taskDatabaseManager) : base(taskDatabaseManager) {
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
            NewTask.Type = ReminderSelected ? 1 : 0;

            DatabaseManager.ConnectToDatabase("test1");
            DatabaseManager.InsertTask(NewTask);                      
            DatabaseManager.DisconnectFromDatabase();

            NewTask = new Task();
            Mediator.Broadcast(Constants.ShowListView);    
        }

        private bool canExecuteAdd(object obj)
        {
            return NewTask != null && !string.IsNullOrEmpty(NewTask.Description) && !string.IsNullOrWhiteSpace(NewTask.Description) && NewTask.DesiredDateTime != null;
        }

    }
}
