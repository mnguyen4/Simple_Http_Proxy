﻿using Simple_Http_Proxy.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Simple_Http_Proxy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;

        private void onApplicationStartup(object sender, StartupEventArgs e)
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("Images\\main.ico");
            notifyIcon.Visible = true;
            notifyIcon.Click += new EventHandler(onNotifyIconClicked);
        }

        private void onNotifyIconClicked(object sender, EventArgs e)
        {
            var mainWindow = App.Current.MainWindow;
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }

        private void onApplicationExit(object sender, ExitEventArgs e)
        {
            HttpProxyListener proxyListener = HttpProxyListener.getInstance();
            proxyListener.stopListener();
            notifyIcon.Dispose();
        }
    }
}
