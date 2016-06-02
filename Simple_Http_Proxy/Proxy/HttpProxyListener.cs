﻿using Simple_Http_Proxy.Constants;
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
        private static HttpProxyListener instance;

        private HttpProxyListener()
        {
            // initialize the http listener
            listener = new HttpListener();
            configureHttpListener();
        }

        public static HttpProxyListener getInstance()
        {
            if (instance == null)
            {
                instance = new HttpProxyListener();
            }
            return instance;
        }

        /*
         * Start the HTTP listener.
         */
        public void startListener()
        {
            listener.Start();
            listener.BeginGetContext(new AsyncCallback(onRequestReceived), listener);
        }

        /*
         * Stop the HTTP listener.
         */
        public void stopListener()
        {
            listener.Stop();
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

        /*
         * Asynchronous function to handle requests.
         */
        private void onRequestReceived(IAsyncResult result)
        {
            var context = listener.EndGetContext(result);
            var request = context.Request;

            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(request.RawUrl);
            webRequest.UserAgent = request.UserAgent;
            webRequest.KeepAlive = request.KeepAlive;
            webRequest.Headers = (WebHeaderCollection)request.Headers;
            webRequest.Method = request.HttpMethod;

            Dictionary<string, object> proxyData = new Dictionary<string, object>();
            proxyData.Add(Constant.WEB_REQUEST, webRequest);
            proxyData.Add(Constant.ORIGINAL_CONTEXT, context);
            webRequest.BeginGetResponse(new AsyncCallback(onWebResponseReceived), proxyData);
        }

        /*
         * Asynchronous function to handle web response.
         */
        private void onWebResponseReceived(IAsyncResult result)
        {
            Dictionary<string, object> proxyData = (Dictionary<string, object>)result.AsyncState;
            HttpWebRequest webRequest = (HttpWebRequest)proxyData[Constant.WEB_REQUEST];
            var webResponse = webRequest.EndGetResponse(result);
            HttpListenerContext originalContext = (HttpListenerContext)proxyData[Constant.ORIGINAL_CONTEXT];
            var originalResponse = originalContext.Response;
            var webResponseStream = webResponse.GetResponseStream();
            webResponseStream.CopyTo(originalResponse.OutputStream);
            originalResponse.OutputStream.Close();
            listener.BeginGetContext(new AsyncCallback(onRequestReceived), listener);
        }
    }
}
