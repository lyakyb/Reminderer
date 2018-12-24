using Reminderer.Models;
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
            _selectedDays = new List<Reminder.Days>();
        }

        private IList<Reminder.Days> _selectedDays;
        public IList<Reminder.Days> SelectedDays
        {
            get { return (List<Reminder.Days>)this.GetValue(SelectedDaysProperty); }
            set
            {
                this.SetValue(SelectedDaysProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedDaysProperty = DependencyProperty.Register(
          "SelectedDays", typeof(List<Reminder.Days>), typeof(RepeatingDayPicker));

        private void dayClicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            Reminder.Days mappedVal;

            switch(btn.Name)
            {
                case "sunday":
                    mappedVal = Reminder.Days.Sunday;
                    break;
                case "monday":
                    mappedVal = Reminder.Days.Monday;
                    break;
                case "tuesday":
                    mappedVal = Reminder.Days.Tuesday;
                    break;
                case "wednesday":
                    mappedVal = Reminder.Days.Wednesday;
                    break;
                case "thursday":
                    mappedVal = Reminder.Days.Thursday;
                    break;
                case "friday":
                    mappedVal = Reminder.Days.Friday;
                    break;
                case "saturday":
                    mappedVal = Reminder.Days.Saturday;
                    break;
                default:
                    mappedVal = Reminder.Days.None;
                    break;
            }

            if (mappedVal == Reminder.Days.None) return;

            if (btn.IsChecked == true && !_selectedDays.Contains(mappedVal))
            {
                _selectedDays.Add(mappedVal);

            } else if (btn.IsChecked == false && _selectedDays.Contains(mappedVal))
            {
                _selectedDays.Remove(mappedVal);
            }

            _selectedDays = _selectedDays.OrderBy(x =>(int)x).ToList();
            SelectedDays = _selectedDays;
            Console.WriteLine($"selectedDays: {SelectedDays}");
        }
    }
}
