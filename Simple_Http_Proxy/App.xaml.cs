using Simple_Http_Proxy.Proxy;
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
        private void onApplicationExit(object sender, ExitEventArgs e)
        {
            HttpProxyListener proxyListener = HttpProxyListener.getInstance();
            proxyListener.stopListener();
        }
    }
}
