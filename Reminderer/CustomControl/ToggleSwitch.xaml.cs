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
        private string _offText;
        private string _onText;

        const string DEFAULT_OFF_COLOR_HEX_VALUE = "#b0b2b1";
        const string DEFAULT_ON_COLOR_HEX_VALUE = "#89ffa5";
        const string DEFAULT_ONTEXT_TEXT = "YES";
        const string DEFAULT_OFFTEXT_TEXT = "NO";



        private Thickness _offPosMargin;
        private Thickness _onPosMargin;

        public ToggleSwitch()
        {
            _toggled = false;
            InitializeComponent();
            this.SetOffColorFromHexString(DEFAULT_OFF_COLOR_HEX_VALUE);
            this.SetOnColorFromHexString(DEFAULT_ON_COLOR_HEX_VALUE);
            this.OnText = DEFAULT_ONTEXT_TEXT;
            this.OffText = DEFAULT_OFFTEXT_TEXT;

            Loaded += delegate
            {
                calculateDimensions();
                innerCircle.Fill = new SolidColorBrush(this.OffColor);
            };
        }

        private void calculateDimensions()
        {
            innerCircle.Width = background.ActualWidth;
            innerCircle.Height = background.ActualHeight * 0.8;
            innerCircle.RadiusX = innerCircle.Width * 0.1;
            innerCircle.RadiusY = innerCircle.Height * 0.5;

            toggleEllipse.Width = innerCircle.Height * 0.9;
            toggleEllipse.Height = innerCircle.Height * 0.9;

            double desiredGap = innerCircle.Height * 0.1;

            Thickness currentMargin = innerCircle.Margin;
            double offMarginLeft = desiredGap;
            double offMarginRight = innerCircle.Width - toggleEllipse.Width - desiredGap;
            double onMarginLeft = innerCircle.Width - desiredGap;
            double onMarginRight = desiredGap;

            _offPosMargin = new Thickness(offMarginLeft, desiredGap, offMarginRight, desiredGap);
            _onPosMargin = new Thickness(onMarginLeft, desiredGap, onMarginRight, desiredGap);

            toggleEllipse.Margin = _offPosMargin;

            _deltaX = onMarginLeft - onMarginRight - toggleEllipse.Width;

            Console.WriteLine($"innerCircle width: {innerCircle.Width} and height: {innerCircle.Height}");
            Console.WriteLine($"ellipse width: {toggleEllipse.Width}");
            Console.WriteLine($"currentMargin: {toggleEllipse.Margin}");
            Console.WriteLine($"desiredGap: {desiredGap}");
            Console.WriteLine($"deltaX: {_deltaX}");

            textDetail.Text = this.OffText;
        }

        private void clicked(object sender, RoutedEventArgs e)
        {
            if (_toggled)
            {
                DoubleAnimation db = new DoubleAnimation();
                db.From = _deltaX;
                db.To = 0;
                db.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                TranslateTransform tt = new TranslateTransform();
                toggleEllipse.RenderTransform = tt;

                tt.BeginAnimation(TranslateTransform.XProperty, db);

                Console.WriteLine($"TO: {db.To}");
                ColorAnimation color = new ColorAnimation();
                color.From = this.OnColor;
                color.To = this.OffColor;

                innerCircle.Fill = new SolidColorBrush((Color)color.From);
                color.Duration = new Duration(TimeSpan.FromSeconds(0.5));

                innerCircle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, color);
                textDetail.Text = this.OffText;
            } else
            {
                DoubleAnimation db = new DoubleAnimation();
                db.From = 0;
                db.To = _deltaX;
                db.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                TranslateTransform tt = new TranslateTransform();
                toggleEllipse.RenderTransform = tt;

                tt.BeginAnimation(TranslateTransform.XProperty, db);

                ColorAnimation color = new ColorAnimation();
                color.From = this.OffColor;
                color.To = this.OnColor;

                innerCircle.Fill = new SolidColorBrush((Color)color.From);
                color.Duration = new Duration(TimeSpan.FromSeconds(0.5));

                innerCircle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, color);
                textDetail.Text = this.OnText;
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
   
        public string OffText
        {
            get { return (string)this.GetValue(OffTextProperty); }
            set { this.SetValue(OffTextProperty, value);
                _offText = value;
            }
        }
        public static readonly DependencyProperty OffTextProperty = DependencyProperty.Register(
          "OffText", typeof(string), typeof(ToggleSwitch));

        public string OnText
        {
            get { return (string)this.GetValue(OnTextProperty); }
            set { this.SetValue(OnTextProperty, value);
                _onText = value;
            }
        }
        public static readonly DependencyProperty OnTextProperty = DependencyProperty.Register(
          "OnText", typeof(string), typeof(ToggleSwitch));

        public Boolean IsToggled
        {
            get { return (Boolean)this.GetValue(ToggleProperty); }
            set { this.SetValue(ToggleProperty, value); }
        }
        public static readonly DependencyProperty ToggleProperty = DependencyProperty.Register(
          "IsToggled", typeof(Boolean), typeof(ToggleSwitch));


        #endregion


    }
}
