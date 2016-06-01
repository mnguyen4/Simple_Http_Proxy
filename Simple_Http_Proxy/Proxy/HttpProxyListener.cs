using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Proxy
{
    class HttpProxyListener
    {
        private static HttpListener listener;

        public HttpProxyListener()
        {
            // initialize the http listener
            listener = new HttpListener();
            configureHttpListener();
        }

        /*
         * Initialize the HTTP listener with hostname and port(s).
         */
        private void configureHttpListener()
        {
            AppStorage storage = AppStorage.getInstance();
            listener.Prefixes.Add("http://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.PORT_TEXT));
            string enableSsl = storage.getPreference(Constant.SSL_CHECK);
            if (Constant.TRUE.Equals(enableSsl))
            {
                listener.Prefixes.Add("https://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.SSL_PORT_TEXT));
            }
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }
    }
}
