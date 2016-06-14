using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
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
using System.Windows.Shapes;

namespace Simple_Http_Proxy.Windows
{
    /// <summary>
    /// Interaction logic for AddBlacklsitItem.xaml
    /// </summary>
    public partial class AddListItem : Window
    {
        // Differentiate between blacklist add and whitelist add.
        private string panelOp;

        public AddListItem(string panelOp)
        {
            InitializeComponent();
            this.panelOp = panelOp;
            initializeListeners();
        }

        /*
         * Initialize listeners for UI components.
         */
        private void initializeListeners()
        {
            listDomainTxt.KeyDown += new KeyEventHandler(onListDomainTxtKeyPressed);
            listItemAddBtn.Click += new RoutedEventHandler(onListItemAddBtnClicked);
            listItemCancelBtn.Click += new RoutedEventHandler(onListItemCancelBtnClicked);
        }

        /*
         * Event handler for add button
         */
        private void onListItemAddBtnClicked(object sender, RoutedEventArgs e)
        {
            string listItem = listDomainTxt.Text;
            listItem = UrlUtil.parseDomainName(listItem);
            if (listItem != null && listItem.Length > 0)
            {
                AppStorage storage = AppStorage.getInstance();
                if (Constant.BLACKLIST_OP.Equals(panelOp))
                {
                    storage.addBlacklistItem(listItem);
                    BlacklistUtil.writeBlackList();
                }
                else if (Constant.WHITELIST_OP.Equals(panelOp))
                {
                    storage.addWhitelistItem(listItem);
                    WhitelistUtil.writeWhitelist();
                }
                this.Close();
            }
        }

        /*
         * Event handler for cancel button.
         */
        private void onListItemCancelBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
         * Key press event handler for list domain textbox.
         */
        private void onListDomainTxtKeyPressed(object sender, KeyEventArgs e)
        {
            // if key pressed is enter, trigger add button click
            if (e.Key.Equals(Key.Enter))
            {
                onListItemAddBtnClicked(listItemAddBtn, new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
