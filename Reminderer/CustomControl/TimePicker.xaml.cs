using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        private ContentControl _focusedButton;
        private bool _hourBtnFocused;
        private bool _minBtnFocused;
        private int _minuteVal;
        private int _hourVal;

        public TimePicker()
        {
            InitializeComponent();
            _minuteVal = 0;
            _hourVal = 0;
            hourNumberText.Text = "0";
            minuteNumberText.Text = "0";
            _hourBtnFocused = _minBtnFocused = false;
        }

        //public int HourVal
        //{
        //    get { return _hourVal; }
        //    set
        //    {
        //        _hourVal = value;
        //        hourNumberText.Text = _hourVal.ToString();
        //    }
        //}
        //public int MinuteVal
        //{
        //    get { return _minuteVal; }
        //    set
        //    {
        //        _minuteVal = value;
        //        minuteNumberText.Text = _minuteVal.ToString();
        //    }
        //}

        private void lostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void buttonPressed(object sender, RoutedEventArgs e)
        {
            //Check if the sender is either Hour or Minute button
            if (sender == hourButton || sender == hourNumber)
            {
                _hourBtnFocused = true;
                _minBtnFocused = false;
            }
            else if (sender == minuteButton || sender == minuteNumber)
            {
                _hourBtnFocused = false;
                _minBtnFocused = true;
            } else
            {
                _hourBtnFocused = false;
                _minBtnFocused = false;
            }
        }

        private void incBtnPressed(object sender, RoutedEventArgs e)
        {
            if (_hourBtnFocused)
            {
                HourValue = HourValue + 1 == 24 ? 23 : HourValue + 1;
            } else if (_minBtnFocused)
            {
                MinuteValue = MinuteValue + 1 == 60 ? 0 : MinuteValue + 1;
            }
        }

        private void decBtnPressed(object sender, RoutedEventArgs e)
        {
            if (_hourBtnFocused)
            {
                HourValue = HourValue - 1 < 0 ? 0 : HourValue - 1;
            }
            else if (_minBtnFocused)
            {
                MinuteValue = MinuteValue - 1 < 0 ? 59 : MinuteValue - 1;
            }
        }

        private void mouseWheelShifted(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                incBtnPressed(sender, e);
            }
            else
            {
                decBtnPressed(sender, e);
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {

        }


        public int HourValue
        {
            get { return (int)this.GetValue(HourValueProperty); }
            set
            {
                this.SetValue(HourValueProperty, value);
                _hourVal = value;
                hourNumberText.Text = _hourVal.ToString();
            }
        }
        public static readonly DependencyProperty HourValueProperty = DependencyProperty.Register(
          "HourValue", typeof(int), typeof(TimePicker), new PropertyMetadata(0));

        public int MinuteValue
        {
            get { return (int)this.GetValue(MinuteValueProperty); }
            set
            {
                this.SetValue(MinuteValueProperty, value);
                _minuteVal = value;
                minuteNumberText.Text = _minuteVal.ToString();
            }
        }
        public static readonly DependencyProperty MinuteValueProperty = DependencyProperty.Register(
          "MinuteValue", typeof(int), typeof(TimePicker), new PropertyMetadata(0));

    }
}
