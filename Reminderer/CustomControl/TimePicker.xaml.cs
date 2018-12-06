﻿using System;
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
            MinuteVal = 0;
            HourVal = 0;
            _hourBtnFocused = _minBtnFocused = false;
        }

        public int HourVal
        {
            get { return _hourVal; }
            set
            {
                _hourVal = value;
                hourNumber.Content = _hourVal.ToString();
            }
        }

        public int MinuteVal
        {
            get { return _minuteVal; }
            set
            {
                _minuteVal = value;
                minuteNumber.Content = _minuteVal.ToString();
            }
        }

        private void buttonPressed(object sender, RoutedEventArgs e)
        {
            //Check if the sender is either Hour or Minute button
            if (sender == hourButton || sender == hourNumber)
            {
                _hourBtnFocused = true;
                _minBtnFocused = false;
                minuteButton.Background = Brushes.LightSlateGray;
                minuteNumber.Background = Brushes.LightSlateGray;
                hourButton.Background = Brushes.White;
                hourNumber.Background = Brushes.White;
            }
            else if (sender == minuteButton || sender == minuteNumber)
            {
                _hourBtnFocused = false;
                _minBtnFocused = true;
                minuteButton.Background = Brushes.White;
                minuteNumber.Background = Brushes.White;
                hourButton.Background = Brushes.LightSlateGray;
                hourNumber.Background = Brushes.LightSlateGray;
            } else
            {
                _hourBtnFocused = false;
                _minBtnFocused = false;
                minuteButton.Background = hourButton.Background = Brushes.White;
            }
        }

        private void incBtnPressed(object sender, RoutedEventArgs e)
        {
            if (_hourBtnFocused)
            {
                HourVal = HourVal + 1 == 24 ? 23 : HourVal + 1;
            } else if (_minBtnFocused)
            {
                MinuteVal = MinuteVal + 1 == 60 ? 0 : MinuteVal + 1;
            }
        }

        private void decBtnPressed(object sender, RoutedEventArgs e)
        {
            if (_hourBtnFocused)
            {
                HourVal = HourVal - 1 < 0 ? 0 : HourVal - 1;
            }
            else if (_minBtnFocused)
            {
                MinuteVal = MinuteVal - 1 < 0 ? 59 : MinuteVal - 1;
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

        

    }
}
