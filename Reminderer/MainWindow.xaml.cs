using Reminderer.Framework;
using Reminderer;
using System;
using System.Windows;
using System.Windows.Controls;
using Reminderer.Views.ScheduleListView;

namespace Reminderer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new ScheduleListView());

        }
        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }
        public void Navigate(UserControl nextPage, object obj)
        {
            this.Content = nextPage;
            ISwitchable switcher = nextPage as ISwitchable;

            if (switcher != null)
            {
                switcher.UtilizeObject(obj);
            } else
            {
                throw new ArgumentException($"Destination page is not Iswitchable {nextPage.Name.ToString()}");
            }
        }
    }
}
