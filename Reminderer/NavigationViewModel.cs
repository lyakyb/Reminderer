﻿using Reminderer.Framework;
using Reminderer.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer
{
    class NavigationViewModel : INotifyPropertyChanged
    {
        private List<IRemindererViewModel> _viewModels;
        public List<IRemindererViewModel> ViewModels
        {
            get
            {
                if (_viewModels == null)
                {
                    _viewModels = new List<IRemindererViewModel>();
                }
                return _viewModels;
            }
        }
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; OnPropertyChanged("CurrentViewModel"); }
        }

        public NavigationViewModel()
        {
            TaskDatabaseManager taskDatabaseManager = new TaskDatabaseManager();

            ViewModels.Add(new ScheduleListViewModel(taskDatabaseManager));
            ViewModels.Add(new ChoiceViewModel(taskDatabaseManager));
            ViewModels.Add(new AddEditViewModel(taskDatabaseManager));

            CurrentViewModel = ViewModels.First();

            Mediator.Subscribe(Constants.ShowListView, ShowListView);
            Mediator.Subscribe(Constants.ShowAddEditView, ShowAddEditView);
            Mediator.Subscribe(Constants.ShowChoiceView, ShowChoiceView);

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangeViewModel(IRemindererViewModel viewModel)
        {
            if (!ViewModels.Contains(viewModel))
                ViewModels.Add(viewModel);

            CurrentViewModel = ViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        public void ShowListView(object obj)
        {
            ChangeViewModel(ViewModels.FirstOrDefault(viewModel=>viewModel.GetType() == typeof(ScheduleListViewModel)));
        }

        public void ShowAddEditView(object obj)
        {
            AddEditViewModel addEditVM = (AddEditViewModel)ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(AddEditViewModel));
            ChangeViewModel(addEditVM);
            addEditVM.ReminderSelected = (bool)obj;
        }

        public void ShowChoiceView(object obj)
        {
            ChangeViewModel(ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(ChoiceViewModel)));
        }
    }
}