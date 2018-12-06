using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Reminderer.Views.AddEditView
{
    /// <summary>
    /// Interaction logic for AddEditView.xaml
    /// </summary>
    public partial class AddEditView : UserControl, ISwitchable
    {
        public AddEditView()
        {
            InitializeComponent();
            setListBoxItems();
        }

        private void setListBoxItems()
        {
            List<int> hourList = new List<int>();
            List<int> minuteList = new List<int>();
            for (int i=0; i<24; i++)
            {
                hourList.Add(i + 1);
            }
            for (int j=0; j<60; j++)
            {
                minuteList.Add(j);
            }

            hourListBox.ItemsSource = hourList;
            minuteListBox.ItemsSource = minuteList;
        }

        public void UtilizeObject(object obj)
        {
            throw new NotImplementedException();
        }

        private void addButton_click(object sender, System.Windows.RoutedEventArgs e)
        {
            return;
        }
    }
}
