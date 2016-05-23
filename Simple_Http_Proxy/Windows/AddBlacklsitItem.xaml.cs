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
    public partial class AddBlacklsitItem : Window
    {
        public AddBlacklsitItem()
        {
            InitializeComponent();
        }

        /*
         * Event handler for cancel button.
         */
        private void onBlacklistItemCancelBtnClicked(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
