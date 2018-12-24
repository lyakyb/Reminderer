﻿using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reminderer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow app = new MainWindow();
            DatabaseManager dbM = new DatabaseManager("Test1");
            NavigationViewModel context = new NavigationViewModel(dbM);
            app.DataContext = context;
            app.Show();
        }
    }
}
