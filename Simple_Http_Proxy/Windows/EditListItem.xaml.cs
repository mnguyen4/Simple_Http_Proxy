using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
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
    /// Interaction logic for EditListItem.xaml
    /// </summary>
    public partial class EditListItem : Window
    {
        // Differentiate between blacklist add and whitelist edit.
        private string panelOp;
        private string oldItem;

        public EditListItem(string panelOp, string oldItem)
        {
            InitializeComponent();
            this.panelOp = panelOp;
            this.oldItem = oldItem;
        }

        /*
         * Event handler for window activated.
         */
        private void onWindowActivated(object sender, EventArgs e)
        {
            listDomainTxt.Text = oldItem;
        }

        /*
         * Event handler for confirm button.
         */
        private void onListItemConfirmBtnClicked(object sender, RoutedEventArgs e)
        {
            string listItem = listDomainTxt.Text;
            if (listItem != null && listItem.Length > 0 && !oldItem.Equals(listItem))
            {
                AppStorage storage = AppStorage.getInstance();
                if (Constant.BLACKLIST_OP.Equals(panelOp))
                {
                    storage.removeBlacklistItem(oldItem);
                    storage.addBlacklistItem(listItem);
                }
                else if (Constant.WHITELIST_OP.Equals(panelOp))
                {
                    storage.removeWhitelistItem(oldItem);
                    storage.addWhitelistItem(listItem);
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
    }
}
