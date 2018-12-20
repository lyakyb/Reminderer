using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reminderer.CustomControl
{
    /// <summary>
    /// Interaction logic for RepeatingDayPicker.xaml
    /// </summary>
    public partial class RepeatingDayPicker : UserControl
    {
        public RepeatingDayPicker()
        {
            InitializeComponent();
            _selectedDays = new List<int>();
        }

        private List<int> _selectedDays;
        public List<int> SelectedDays
        {
            get { return (List<int>)this.GetValue(SelectedDaysProperty); }
            set
            {
                this.SetValue(SelectedDaysProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedDaysProperty = DependencyProperty.Register(
          "SelectedDays", typeof(List<int>), typeof(RepeatingDayPicker));

        private void dayClicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            int mappedVal;

            switch(btn.Name)
            {
                case "sunday":
                    mappedVal = 0;
                    break;
                case "monday":
                    mappedVal = 1;
                    break;
                case "tuesday":
                    mappedVal = 2;
                    break;
                case "wednesday":
                    mappedVal = 3;
                    break;
                case "thursday":
                    mappedVal = 4;
                    break;
                case "friday":
                    mappedVal = 5;
                    break;
                case "saturday":
                    mappedVal = 6;
                    break;
                default:
                    mappedVal = -1;
                    break;
            }

            if (mappedVal == -1) return;

            if (btn.IsChecked == true && !_selectedDays.Contains(mappedVal))
            {
                _selectedDays.Add(mappedVal);

            } else if (btn.IsChecked == false && _selectedDays.Contains(mappedVal))
            {
                _selectedDays.Remove(mappedVal);
            }

            SelectedDays = _selectedDays;
        }
    }
}
