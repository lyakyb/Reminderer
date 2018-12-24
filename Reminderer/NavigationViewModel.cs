using Reminderer.Framework;
using Reminderer.Models;
using Reminderer.Views;
using Reminderer.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;

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
            ViewModels.Add(new ScheduleListViewModel());
            ViewModels.Add(new ChoiceViewModel());
            ViewModels.Add(new AddEditViewModel());
            
            Mediator.Subscribe(Constants.ShowListView, ShowListView);
            Mediator.Subscribe(Constants.ShowAddEditView, ShowAddEditView);
            Mediator.Subscribe(Constants.ShowChoiceView, ShowChoiceView);
            Mediator.Subscribe(Constants.FireNotification, FireNotification);

            RemindererManager.Instance.LoadReminders();
            RemindererManager.Instance.LoadSchedules();
            Mediator.Broadcast(Constants.TasksUpdated);

            CurrentViewModel = ViewModels.First();
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
           // RemindererManager.Instance.UpdateTasks();
           // Mediator.Broadcast(Constants.TasksUpdated);
            ChangeViewModel(listVM);
        }

        public void ShowAddEditView(object obj)
        {
            AddEditViewModel addEditVM = (AddEditViewModel)ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(AddEditViewModel));
            if(obj.GetType().Name == "Task")
            {
                if (obj.GetType() == typeof(Reminder))
                {
                    addEditVM.NewTask = (Reminder)obj;
                } else
                {
                    addEditVM.NewTask = (Schedule)obj;
                }
                addEditVM.IsEditing = true;
            }else if (obj.GetType() == typeof(bool))
            {
                if((bool)obj)
                {
                    addEditVM.NewTask = new Reminder();
                }
                else
                {
                    addEditVM.NewTask = new Schedule();
                }
            }
            ChangeViewModel(addEditVM);
        }

        public void ShowChoiceView(object obj)
        {
            ChangeViewModel(ViewModels.FirstOrDefault(viewModel => viewModel.GetType() == typeof(ChoiceViewModel)));
        }

        public void FireNotification(object obj)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                NotificationWindow nw = new NotificationWindow();
                nw.DescriptionText = ((Task)obj).Description;
                nw.ExtraDetailText = ((Task)obj).ExtraDetail;
                nw.Topmost = true;
                nw.Show();

                SystemSounds.Hand.Play();
            });
        }
    }
}
