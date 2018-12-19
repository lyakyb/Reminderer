using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System;

namespace Reminderer.Windows
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private string _descriptionText;
        private string _extraDetailText;
        public string DescriptionText { get { return _descriptionText; } set { _descriptionText = value; DescriptionTextBlock.Text = value; } }
        public string ExtraDetailText { get { return _extraDetailText; } set { _extraDetailText = value; ExtraDetailTextBlock.Text = value; } }

        public NotificationWindow()
        {
            InitializeComponent();
            adjustDimensions();

            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void adjustDimensions()
        {
            Rectangle res = Screen.PrimaryScreen.Bounds;
            this.Width = res.Width * 0.172;
            this.Height = res.Height * 0.185;
            this.UpdateLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            var timer = new System.Threading.Timer(delegate{
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            }, null, 5000, Timeout.Infinite);
        }

        private void closeButton_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}

