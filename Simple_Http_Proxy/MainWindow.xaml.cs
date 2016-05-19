﻿using Simple_Http_Proxy.Memory;
using Simple_Http_Proxy.Utils;
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

namespace Simple_Http_Proxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppStorage storage;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * Handler for window loaded event.
         */
        private void windowContentLoaded(object sender, RoutedEventArgs e)
        {
            storage = AppStorage.getInstance();
            readPreferences();
            additionalInit();
        }

        /*
         * Initialize additional components.
         */
        private void additionalInit()
        {
            hostnameTxt.Text = (string)storage.getPreference(hostnameTxt.Name);
            portTxt.Text = (string)storage.getPreference(portTxt.Name);
            sslChk.IsChecked = "true".Equals((string)storage.getPreference(sslChk.Name));
            // fix for uncheck event not firing on app startup when IsChecked is set to false
            if (sslChk.IsChecked == false && sslPortTxt.IsEnabled)
            {
                sslChkChanged(sslChk, new RoutedEventArgs(CheckBox.UncheckedEvent));
            }
            sslPortTxt.Text = (string)storage.getPreference(sslPortTxt.Name);
            blackLocationTxt.Text = (string)storage.getPreference(blackLocationTxt.Name);
            whiteLocationTxt.Text = (string)storage.getPreference(whiteLocationTxt.Name);
        }
        
        /*
         * Read preferences from preference file.
         */
        private void readPreferences()
        {
            PreferencesUtil.loadPreferences();
        }

        /*
         * Event handler for sslChk.
         */
        private void sslChkChanged(object sender, RoutedEventArgs e)
        {
            // enable sslPortTxt if sslChk is checked
            if (sslChk.IsChecked ?? true)
            {
                sslPortTxt.IsEnabled = true;
            }
            // disable sslPortTxt
            else
            {
                sslPortTxt.IsEnabled = false;
            }
        }
    }
}
