using Reminderer.Framework;
using Reminderer.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Views
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        private NotificationManager _notificationManager;

        public BaseViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        public BaseViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
