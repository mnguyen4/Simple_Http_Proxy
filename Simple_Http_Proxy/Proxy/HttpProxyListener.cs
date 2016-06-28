using log4net;
using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using Simple_Http_Proxy.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Simple_Http_Proxy.Proxy
{
    class HttpProxyListener
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof (HttpProxyListener));

        //private static HttpListener listener;
        private static TcpListener tcpListener;
        private static HttpProxyListener instance;
        private static Queue<HttpListenerContext> contextQueue;
        private static ManualResetEvent listeningEvent;
        private static Thread tcpListenerThread;

        private static string guid;
        private static bool isListening;

        private HttpProxyListener()
        {
            // initialize the http proxy listener
            contextQueue = new Queue<HttpListenerContext>();
            listeningEvent = new ManualResetEvent(false);
            isListening = false;
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
            /*
            listener.Start();
            guid = Guid.NewGuid().ToString();
            AppStorage storage = AppStorage.getInstance();
            foreach (var prefix in listener.Prefixes)
            {
                LOGGER.Info("Listener started on: " + prefix);
            }
            string appGuid = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            LOGGER.Info("Application GUID: " + appGuid);
            listener.BeginGetContext(new AsyncCallback(onRequestReceived), guid);
            */
            tcpListener.Start();
            isListening = true;
            tcpListenerThread = new Thread(() =>
            {
                while (isListening)
                {
                    listeningEvent.Reset();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(onRequestReceived), tcpListener);
                    listeningEvent.WaitOne();
                }
            });
            tcpListenerThread.Start();
        }

        /*
         * Stop the HTTP listener.
         */
        public void stopListener()
        {
            isListening = false;
            listeningEvent.Set();
            tcpListener.Stop();
            /*
            listener.Close();
            LOGGER.Info("Listener stopped.");
            */
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
            /*
            listener = new HttpListener();
            AppStorage storage = AppStorage.getInstance();
            listener.Prefixes.Add("http://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.PORT_TEXT) + "/");
            string enableSsl = storage.getPreference(Constant.SSL_CHECK);
            if (Constant.TRUE.Equals(enableSsl))
            {
                listener.Prefixes.Add("https://" + storage.getPreference(Constant.HOST_NAME_TEXT) + ":" + storage.getPreference(Constant.SSL_PORT_TEXT) + "/");
            }
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            */
            AppStorage storage = AppStorage.getInstance();
            tcpListener = new TcpListener(IPAddress.Parse(storage.getPreference(Constant.HOST_NAME_TEXT)), Int32.Parse(storage.getPreference(Constant.PORT_TEXT)));
        }

        /*
         * Asynchronous function to handle requests.
         */
        private void onRequestReceived(IAsyncResult result)
        {
            /*
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
            */
            var listener = (TcpListener)result.AsyncState;
            TcpClient tcpClient = null;
            try
            {
                tcpClient = listener.EndAcceptTcpClient(result);
                // start background thread to relay request
                Thread worker = new Thread(() => sendWebRequest(tcpClient));
                worker.IsBackground = true;
                worker.Start();
            } catch (Exception e)
            {
                LOGGER.Error("Failed to receive request.", e);
            } finally
            {
                // signal the listener to start listening again
                listeningEvent.Set();
            }
        }

        /*
         * Function to send web request.
         */
        private void sendWebRequest(TcpClient tcpClient)
        {
            /*
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
            */
            try
            {
                // get the connecting client IO stream
                NetworkStream clientStream = tcpClient.GetStream();
                StreamReader clientReader = new StreamReader(clientStream);
                StreamWriter clientWriter = new StreamWriter(clientStream);

                // get the request method
                string method = clientReader.ReadLine();
                if (method == null || method.Length == 0)
                {
                    return;
                }
                
            } catch (Exception e)
            {
                LOGGER.Error("Failed to relay request.", e);
            } finally
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    tcpClient.Close();
                }
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

        /*
         * Relay traffic from one TcpClient to another.
         */
        private void relayTcpTraffic(TcpClient from, TcpClient to)
        {
            // get the network streams
            NetworkStream fromStream = from.GetStream();
            NetworkStream toStream = to.GetStream();
            try
            {
                // copy data
                if (from.Connected && to.Connected)
                {
                    fromStream.CopyTo(toStream);
                }
            } catch(Exception e)
            {
                LOGGER.Error("Failed to relay request.", e);
            } finally
            {
                // clean up connections
                if (from.Connected)
                {
                    from.Close();
                }
                if (to.Connected)
                {
                    to.Close();
                }
            }
        }
    }
}
