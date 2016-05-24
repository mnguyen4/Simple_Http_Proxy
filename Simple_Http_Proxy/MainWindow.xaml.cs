﻿using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using Simple_Http_Proxy.Utils;
using Simple_Http_Proxy.Windows;
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
            storage = AppStorage.getInstance();
            readPreferencesAndLists();
        }

        /*
         * Handler for window activated event.
         */
        private void onWindowActivated(object sender, EventArgs e)
        {
            blacklistTabInit();
            whitelistTabInit();
            preferenceTabInit();
        }

        /*
         * Initialize blacklist tab components.
         */
        private void blacklistTabInit()
        {
            // clear listbox before adding items to it
            blackList.Items.Clear();
            // populate listbox with blacklist items
            foreach (string item in storage.getBlacklist())
            {
                ListBoxItem blacklistItem = new ListBoxItem();
                blacklistItem.Content = item;
                blackList.Items.Add(blacklistItem);
            }
            blackList.SelectedIndex = 0;
        }

        /*
         * Initialize whitelist tab components.
         */
        private void whitelistTabInit()
        {
            // TODO: Add whitelist tab component init.
        }

        /*
         * Initialize preference tab components.
         */
        private void preferenceTabInit()
        {
            // Initialize Preferences tab
            hostnameTxt.Text = storage.getPreference(Constant.HOST_NAME_TEXT);
            portTxt.Text = storage.getPreference(Constant.PORT_TEXT);
            sslChk.IsChecked = "true".Equals(storage.getPreference(Constant.SSL_CHECK));
            // fix for uncheck event not firing on app startup when IsChecked is set to false
            if (sslChk.IsChecked == false && sslPortTxt.IsEnabled)
            {
                sslChkChanged(sslChk, new RoutedEventArgs(CheckBox.UncheckedEvent));
            }
            sslPortTxt.Text = storage.getPreference(Constant.SSL_PORT_TEXT);
            blackLocationTxt.Text = storage.getPreference(Constant.BLACK_LOCATION_TEXT);
            whiteLocationTxt.Text = storage.getPreference(Constant.WHITE_LOCATION_TEXT);
        }

        /*
         * Read preferences, blacklist, and whitelist from files.
         */
        private void readPreferencesAndLists()
        {
            PreferencesUtil.loadPreferences();
            BlacklistUtil.readBlacklist();
            WhitelistUtil.readWhitelist();
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

        /*
         * Event handler for blacklist add button.
         */
        private void onBlacklistAddBtnClicked(object sender, RoutedEventArgs e)
        {
            AddListItem addBlacklistItemWindow = new AddListItem(Constant.BLACKLIST_OP);
            addBlacklistItemWindow.Show();
        }

        /*
         * Event handler for blacklist edit button.
         */
        private void onBlacklistEditBtnClicked(object sender, RoutedEventArgs e)
        {
            string selectedItem = ((ListBoxItem)blackList.SelectedItem).Content.ToString();
            EditListItem editBlacklistItemWindow = new EditListItem(Constant.BLACKLIST_OP, selectedItem);
            editBlacklistItemWindow.Show();
        }
    }
}
