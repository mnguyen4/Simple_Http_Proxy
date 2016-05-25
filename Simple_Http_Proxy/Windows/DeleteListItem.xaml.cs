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
    /// Interaction logic for DeleteListItem.xaml
    /// </summary>
    public partial class DeleteListItem : Window
    {
        private string panelOp;
        private string listItem;

        public DeleteListItem(string panelOp, string listItem)
        {
            InitializeComponent();
            this.panelOp = panelOp;
            this.listItem = listItem;
        }

        /*
         * Event handler for window activated event.
         */
        private void onWindowActivated(object sender, EventArgs e)
        {
            confirmListItemLbl.Content = listItem;
        }

        /*
         * Event handler for confirm button click.
         */
        private void onConfirmDeleteItemBtnClicked(object sender, RoutedEventArgs e)
        {
            AppStorage storage = AppStorage.getInstance();
            if (Constant.BLACKLIST_OP.Equals(panelOp))
            {
                storage.removeBlacklistItem(listItem);
            }
            else if (Constant.WHITELIST_OP.Equals(panelOp))
            {
                storage.removeWhitelistItem(listItem);
            }
            this.Close();
        }

        /*
         * Event handler for cancel button click.
         */
        private void onCancelDeleteItemBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
