using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Views
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        public TaskDatabaseManager DatabaseManager { get; set; }

        public BaseViewModel(TaskDatabaseManager databaseManager) : this()
        {
            DatabaseManager = databaseManager;
        }

        public BaseViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
