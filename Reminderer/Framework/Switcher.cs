using System;
using System.Windows.Controls;
using Reminderer.Views.ScheduleListView;

namespace Reminderer.Framework
{
    public static class Switcher
    {
        public static MainWindow pageSwitcher;

        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }
        public static void Switch(UserControl newPage, object obj)
        {
            pageSwitcher.Navigate(newPage, obj);
        }
    }
}
