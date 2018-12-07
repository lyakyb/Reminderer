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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reminderer.CustomControl
{
    /// <summary>
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        private bool _toggled;
        private double _deltaX;
        private Color _onColor;
        private Color _offColor;

        const string DEFAULT_OFF_COLOR_HEX_VALUE = "#b0b2b1";
        const string DEFAULT_ON_COLOR_HEX_VALUE = "#89ffa5";



        private Thickness _offPosMargin;
        private Thickness _onPosMargin;

        public ToggleSwitch()
        {
            _toggled = false;
            InitializeComponent();
            this.SetOffColorFromHexString(DEFAULT_OFF_COLOR_HEX_VALUE);
            this.SetOnColorFromHexString(DEFAULT_ON_COLOR_HEX_VALUE);

            Loaded += delegate
            {
                calculateDimensions();
                innerCircle.Fill = new SolidColorBrush(this.OffColor);
            };
        }

        private void calculateDimensions()
        {
            innerCircle.Width = background.ActualWidth * 0.97;
            innerCircle.Height = background.ActualHeight * 0.8;
            innerCircle.RadiusX = innerCircle.ActualWidth * 0.4;
            innerCircle.RadiusY = innerCircle.ActualHeight * 1;
            
            Console.WriteLine($"bgWidth: {background.ActualWidth.ToString()} , bgHeight: {background.ActualHeight.ToString()}");

            toggleEllipse.Width = innerCircle.Height * 0.95;
            toggleEllipse.Height = innerCircle.Height * 0.95;
            Thickness newMargin = new Thickness(innerCircle.Width / 2 - toggleEllipse.Width / 2, innerCircle.ActualHeight - toggleEllipse.Height, innerCircle.Width / 2 - toggleEllipse.Width / 2, innerCircle.ActualHeight - toggleEllipse.Height);
           
            newMargin.Left = newMargin.Left - toggleEllipse.Width * 0.5;
            newMargin.Right = newMargin.Right + toggleEllipse.Width * 0.5;
            toggleEllipse.Margin = _offPosMargin = newMargin;

            _onPosMargin = newMargin;
            _onPosMargin.Left = _onPosMargin.Left + toggleEllipse.Width * 0.5 * 2;
            _onPosMargin.Right = _onPosMargin.Right - toggleEllipse.Width * 0.5 * 2;
            _deltaX = toggleEllipse.Width * 0.5;
        }
        
        private void clicked(object sender, RoutedEventArgs e)
        {
            if (_toggled)
            {
                DoubleAnimation db = new DoubleAnimation();
                db.From = _deltaX * 2;
                db.To = innerCircle.Margin.Left;
                db.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                TranslateTransform tt = new TranslateTransform();
                toggleEllipse.RenderTransform = tt;

                tt.BeginAnimation(TranslateTransform.XProperty, db);
               
                ColorAnimation color = new ColorAnimation();
                color.From = (Color)ColorConverter.ConvertFromString(DEFAULT_ON_COLOR_HEX_VALUE);
                color.To = (Color)ColorConverter.ConvertFromString(DEFAULT_OFF_COLOR_HEX_VALUE);

                innerCircle.Fill = new SolidColorBrush((Color)color.From);
                color.Duration = new Duration(TimeSpan.FromSeconds(0.5));

                innerCircle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, color);
            } else
            {
                DoubleAnimation db = new DoubleAnimation();
                db.From = innerCircle.Margin.Left;
                db.To = _deltaX * 2;
                db.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                TranslateTransform tt = new TranslateTransform();
                toggleEllipse.RenderTransform = tt;

                tt.BeginAnimation(TranslateTransform.XProperty, db);

                ColorAnimation color = new ColorAnimation();
                color.From = (Color)ColorConverter.ConvertFromString(DEFAULT_OFF_COLOR_HEX_VALUE);
                color.To = (Color)ColorConverter.ConvertFromString(DEFAULT_ON_COLOR_HEX_VALUE);

                innerCircle.Fill = new SolidColorBrush((Color)color.From);
                color.Duration = new Duration(TimeSpan.FromSeconds(0.5));

                innerCircle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, color);
            }
            _toggled = !_toggled;

        }


        #region Members
        public void SetOnColorFromHexString(string hexString)
        {
            this.OnColor = (Color)ColorConverter.ConvertFromString(hexString);
        }
        public void SetOffColorFromHexString(string hexString)
        {
            this.OffColor = (Color)ColorConverter.ConvertFromString(hexString);
        }
        public Color OnColor
        {
            get { return _onColor; }
            set { _onColor = value; }
        }
        public Color OffColor
        {
            get { return _offColor; }
            set { _offColor = value; }
        }

        #endregion


    }
}
