using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Reminderer.Views
{
    /// <summary>
    /// Interaction logic for AddEditView.xaml
    /// </summary>
    /// 

      

    public partial class AddEditView : UserControl, ISwitchable
    {
        public bool TaskSelected { get; set; }

        public AddEditView()
            : this(false) { }

        public AddEditView(bool taskSelected)
        {
            TaskSelected = taskSelected;
            InitializeComponent();
        }

        public void UtilizeObject(object obj)
        {
            if (obj.GetType() == typeof(TaskDatabaseManager))
                (DataContext as BaseViewModel).DatabaseManager = (TaskDatabaseManager)obj;
        }

    }
}
