using Reminderer.Framework;
using Reminderer;
using System;
using System.Windows;
using System.Windows.Controls;
using Reminderer.Views;
using Reminderer.CustomControl;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Reminderer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



            var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/icon.png")).Stream;
            var iconBitmap = new Bitmap(iconStream);

            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon()),
                Visible = true
            };
            notifyIcon.DoubleClick += delegate
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
            
            cm.MenuItems.Add("Show Reminder/Schedule List", (s, e) =>
            {
                Mediator.Broadcast(Constants.ShowListView);
                this.Show();
                this.WindowState = WindowState.Normal;
            });
            cm.MenuItems.Add("Exit", (s,e)=>
            {
                System.Windows.Application.Current.Shutdown();
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            });

            notifyIcon.ContextMenu = cm;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.WindowState = WindowState.Minimized;

            base.OnClosing(e);
        }

    }
}
