using System.Windows;
using System.Drawing;
using System.Windows.Forms;

namespace Reminderer.Windows
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
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
        }
    }
}

