using Reminderer.Framework;
using Reminderer.Models;
using Reminderer.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
            ScheduleListViewModel listVM = (ScheduleListViewModel)ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(ScheduleListViewModel));
            ChangeViewModel(listVM);
            listVM.UpdateTasks();
        }

        public void ShowAddEditView(object obj)
        {
            AddEditViewModel addEditVM = (AddEditViewModel)ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(AddEditViewModel));
            if(obj.GetType().Name == "Task")
            {
                addEditVM.NewTask = (Task)obj;
                addEditVM.IsEditing = true;
            }else if (obj.GetType() == typeof(bool))
            {
                addEditVM.ReminderSelected = (bool)obj;
            }
            ChangeViewModel(addEditVM);
        }

        public void ShowChoiceView(object obj)
        {
            ChangeViewModel(ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(ChoiceViewModel)));
        }
    }
}
