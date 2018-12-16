using Reminderer.Framework;
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
        public TaskDatabaseManager DatabaseManager { get; set; }

        public BaseViewModel(TaskDatabaseManager databaseManager) : this()
        {
            DatabaseManager = databaseManager;
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
