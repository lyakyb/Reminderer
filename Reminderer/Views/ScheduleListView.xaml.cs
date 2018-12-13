using Reminderer.Framework;
using System;
using System.Windows.Controls;
using Reminderer.Windows;

namespace Reminderer.Views.ScheduleListView
{
    /// <summary>
    /// Interaction logic for ScheduleListView.xaml
    /// </summary>
    public partial class ScheduleListView : UserControl, ISwitchable
    {
        public ScheduleListView()
        {
            InitializeComponent();
            this.DataContext = new ScheduleListViewModel();
        }

        private void addNewTaskButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new ChoiceView(), (this.DataContext as BaseViewModel).DatabaseManager);
        }

        #region ISwitchable Members
        public void UtilizeObject(object obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
