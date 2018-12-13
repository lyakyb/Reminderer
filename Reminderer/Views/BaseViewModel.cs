using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Views
{
    class BaseViewModel
    {
        public TaskDatabaseManager DatabaseManager { get; set; }

        public BaseViewModel(TaskDatabaseManager databaseManager) : this()
        {
            DatabaseManager = databaseManager;
        }

        public BaseViewModel()
        {
        }
    }
}
