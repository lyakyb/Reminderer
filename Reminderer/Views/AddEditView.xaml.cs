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
