using log4net;
using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using Simple_Http_Proxy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Simple_Http_Proxy.Proxy
{
    class HttpProxyListener
    {
        private static ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static HttpListener listener;
        private static HttpProxyListener instance;
        private static Queue<HttpListenerContext> contextQueue;

        private static string guid;

        private HttpProxyListener()
        {
            // initialize the http proxy listener
            contextQueue = new Queue<HttpListenerContext>();
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
            guid = Guid.NewGuid().ToString();
            AppStorage storage = AppStorage.getInstance();
            LOGGER.Info("Listener started: " + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.PORT_TEXT));
            listener.BeginGetContext(new AsyncCallback(onRequestReceived), guid);
        }

        /*
         * Stop the HTTP listener.
         */
        public void stopListener()
        {
            listener.Close();
            LOGGER.Info("Listener stopped.");
        }

        /*
         * Restart the HTTP listener.
         */
        public void restartListener()
        {
            stopListener();
            configureHttpListener();
            startListener();
        }

        /*
         * Initialize the HTTP listener with hostname and port(s).
         */
        private void configureHttpListener()
        {
            listener = new HttpListener();
            AppStorage storage = AppStorage.getInstance();
            listener.Prefixes.Add("http://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.PORT_TEXT) + "/");
            string enableSsl = storage.getPreference(Constant.SSL_CHECK);
            if (Constant.TRUE.Equals(enableSsl))
            {
                listener.Prefixes.Add("https://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.SSL_PORT_TEXT) + "/");
            }
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }

        /*
         * Asynchronous function to handle requests.
         */
        private void onRequestReceived(IAsyncResult result)
        {
            string resultGuid = result.AsyncState.ToString();
            try
            {
                var context = listener.EndGetContext(result);
                lock (contextQueue)
                {
                    contextQueue.Enqueue(context);
                }
                // start background thread to relay request
                Thread worker = new Thread(sendWebRequest);
                worker.IsBackground = true;
                worker.Start();
                listener.BeginGetContext(new AsyncCallback(onRequestReceived), guid);
            }
            // catch exception thrown when old listener context is disposed
            catch (Exception e) when (!listener.IsListening || !guid.Equals(resultGuid))
            {
                LOGGER.Error("Listener context for GUID " + resultGuid + " ended.", e);
            }
        }

        /*
         * Function to send web request.
         */
        private void sendWebRequest()
        {
            HttpListenerContext context;
            lock (contextQueue)
            {
                context = contextQueue.Dequeue();
            }

            var request = context.Request;
            // url is in blacklist
            if (BlacklistUtil.isBlacklistedDomain(request.RawUrl) && !WhitelistUtil.isWhitelistedDomain(request))
            {
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.OutputStream.Close();
            }
            // url is not in blacklist
            else
            {
                HttpWebRequest webRequest = HttpWebRequest.CreateHttp(request.RawUrl);
                webRequest.UserAgent = request.UserAgent;
                webRequest.Method = request.HttpMethod;
                webRequest.ContentType = request.ContentType;
                webRequest.CookieContainer = new CookieContainer();
                // copy over cookies
                foreach (Cookie cookie in request.Cookies)
                {
                    // fix empty domain cookies
                    if (String.IsNullOrEmpty(cookie.Domain))
                    {
                        cookie.Domain = request.Url.Host;
                    }
                    webRequest.CookieContainer.Add(cookie);
                }
                // copy over POST body
                if (request.HttpMethod.Equals(Constant.POST))
                {
                    request.InputStream.CopyTo(webRequest.GetRequestStream());
                    request.InputStream.Close();
                }

                Dictionary<string, object> proxyData = new Dictionary<string, object>();
                proxyData.Add(Constant.WEB_REQUEST, webRequest);
                proxyData.Add(Constant.ORIGINAL_CONTEXT, context);
                webRequest.BeginGetResponse(new AsyncCallback(onWebResponseReceived), proxyData);
            }
        }

        /*
         * Asynchronous function to handle web response.
         */
        private void onWebResponseReceived(IAsyncResult result)
        {
            Dictionary<string, object> proxyData = (Dictionary<string, object>)result.AsyncState;
            HttpWebRequest webRequest = (HttpWebRequest)proxyData[Constant.WEB_REQUEST];
            HttpListenerContext originalContext = (HttpListenerContext)proxyData[Constant.ORIGINAL_CONTEXT];
            var originalResponse = originalContext.Response;
            // fix for domain name resolve errors
            try
            {
                var webResponse = webRequest.EndGetResponse(result);
                originalResponse.ContentType = webResponse.ContentType;
                originalResponse.ContentLength64 = webResponse.ContentLength;
                // copy over headers
                foreach(string key in webResponse.Headers.AllKeys)
                {
                    // omit existing heaers and reserved headers
                    if (!originalResponse.Headers.AllKeys.Contains<string>(key) && !HeaderUtil.isReservedHeader(key))
                    {
                        originalResponse.Headers[key] = webResponse.Headers[key];
                    }
                }
                var webResponseStream = webResponse.GetResponseStream();
                webResponseStream.CopyTo(originalResponse.OutputStream);
            } catch (Exception e)
            {
                originalResponse.StatusCode = (int)HttpStatusCode.NotFound;
                LOGGER.Error("Web request failed.", e);
            }
            originalResponse.OutputStream.Close();
        }
    }
}
