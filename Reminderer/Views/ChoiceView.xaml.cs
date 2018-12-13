using Reminderer.Framework;
using System;
using System.Windows.Controls;

namespace Reminderer.Views
{
    /// <summary>
    /// Interaction logic for ChoiceView.xaml
    /// </summary>
    public partial class ChoiceView : UserControl, ISwitchable
    {
        public ChoiceView()
        {
            InitializeComponent();
        }

        public void UtilizeObject(object obj)
        {
            if (obj.GetType() == typeof(TaskDatabaseManager))
                (DataContext as BaseViewModel).DatabaseManager = (TaskDatabaseManager)obj;
        }

        private void reminderChosen(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new AddEditView.AddEditView(true), (this.DataContext as BaseViewModel).DatabaseManager);
        }

        private void scheduleChosen(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new AddEditView.AddEditView(false), (this.DataContext as BaseViewModel).DatabaseManager);
        }
    }
}
