using Reminderer.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reminderer.Models;
using System.Windows.Input;
using System.ComponentModel;
using Reminderer.Framework;

namespace Reminderer.Views.AddEditView
{
    class AddEditViewModel : BaseViewModel
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

        public void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddCommand.RaiseCanExecuteChanged();
        }
        
        public AddEditViewModel()
        {
            AddCommand = new DelegateCommand(executeAddCommand, canExecuteAdd);
            NewTask = new Task();
        }

        public AddEditViewModel(Task task)
        {
            AddCommand = new DelegateCommand(executeAddCommand, canExecuteAdd);
            NewTask = task;
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
            DatabaseManager.ConnectToDatabase("test1");
            DatabaseManager.InsertTask(NewTask);                      
            DatabaseManager.DisconnectFromDatabase();

            Switcher.Switch(new Views.ScheduleListView.ScheduleListView());
            
        }
        private bool canExecuteAdd(object obj)
        {
            return NewTask != null && !string.IsNullOrEmpty(NewTask.Description) && !string.IsNullOrWhiteSpace(NewTask.Description) && NewTask.DesiredDateTime != null;
        }

    }
}
