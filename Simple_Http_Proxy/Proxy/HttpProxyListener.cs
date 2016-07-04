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
using System.Text.RegularExpressions;
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
            tcpListener.Start();
            isListening = true;
            // handle network events on a separate thread
            tcpListenerThread = new Thread(() =>
            {
                while (isListening)
                {
                    listeningEvent.Reset();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(onRequestReceived), tcpListener);
                    // wait for signal to continue with loop
                    listeningEvent.WaitOne();
                }
            });
            tcpListenerThread.Start();
            LOGGER.Info("Listener started.");
        }

        /*
         * Stop the HTTP listener.
         */
        public void stopListener()
        {
            isListening = false;
            listeningEvent.Set();
            tcpListener.Stop();
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
            AppStorage storage = AppStorage.getInstance();
            tcpListener = new TcpListener(IPAddress.Parse(storage.getPreference(Constant.HOST_NAME_TEXT)), Int32.Parse(storage.getPreference(Constant.PORT_TEXT)));
        }

        /*
         * Asynchronous function to handle requests.
         */
        private void onRequestReceived(IAsyncResult result)
        {
            var listener = (TcpListener)result.AsyncState;
            // signal the listener to start listening again
            listeningEvent.Set();
            TcpClient tcpClient = null;
            try
            {
                tcpClient = listener.EndAcceptTcpClient(result);
                // use a session storage to keep track of traffic
                ProxyStorage pStorage = new ProxyStorage();
                pStorage.buffer = new byte[2048];
                pStorage.tcpClient = tcpClient;

                // get the connecting client IO stream
                NetworkStream clientStream = tcpClient.GetStream();
                clientStream.BeginRead(pStorage.buffer, 0, pStorage.buffer.Length, new AsyncCallback(clientDataReceived), pStorage);
            } catch (Exception e)
            {
                LOGGER.Error("Failed to receive request.", e);
            }
        }

        /*
         * Asynchronous function to read data from client.
         */
        private void clientDataReceived(IAsyncResult result)
        {
            // get the request method
            // Ex: GET http://imgur.com/ HTTP/1.1
            var pStorage = (ProxyStorage)result.AsyncState;
            var clientStream = pStorage.tcpClient.GetStream();
            string data = "";
            byte[] dataBytes;

            // get data from asynchronous read
            pStorage.read = clientStream.EndRead(result);
            data += Encoding.ASCII.GetString(pStorage.buffer, 0, pStorage.read);
            // try to get the remaining data (if any)
            while (clientStream.DataAvailable)
            {
                pStorage.read = clientStream.Read(pStorage.buffer, 0, pStorage.buffer.Length);
                data += Encoding.ASCII.GetString(pStorage.buffer, 0, pStorage.read);
            }

            // parse the host name and try to create the TCP end point
            string method = data.Replace("\r\n", "\n").Split(new string[] { "\n" }, StringSplitOptions.None)[0];
            string[] methodParts = method.Split(' ');
            pStorage.tcpEndPoint = generateTcpEndPoint(methodParts);
            // no hostname specified
            if (pStorage.tcpEndPoint == null)
            {
                dataBytes = Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");
                clientStream.Write(dataBytes, 0, dataBytes.Length);
                pStorage.tcpClient.Close();
                return;
            }

            // send request to TCP end point
            NetworkStream endPointStream = pStorage.tcpEndPoint.GetStream();
            dataBytes = Encoding.ASCII.GetBytes(data);
            endPointStream.Write(dataBytes, 0, dataBytes.Length);
            endPointStream.Flush();
            // read response from TCP end point
            endPointStream.BeginRead(pStorage.buffer, 0, pStorage.buffer.Length, new AsyncCallback(endPointDataReceived), pStorage);
        }

        /*
         * Read TCP end point response and copy to client network stream.
         */
        private void endPointDataReceived(IAsyncResult result)
        {
            var pStorage = (ProxyStorage)result.AsyncState;
            var clientStream = pStorage.tcpClient.GetStream();
            var endPointStream = pStorage.tcpEndPoint.GetStream();

            // read the TCP end point response and write to client network stream
            pStorage.read = endPointStream.EndRead(result);
            clientStream.Write(pStorage.buffer, 0, pStorage.read);
            // read and copy any remaining data to client network stream
            while (endPointStream.DataAvailable)
            {
                pStorage.read = endPointStream.Read(pStorage.buffer, 0, pStorage.buffer.Length);
                clientStream.Write(pStorage.buffer, 0, pStorage.read);
            }
            clientStream.Flush();
            // clean up connection
            pStorage.tcpClient.Close();
            pStorage.tcpEndPoint.Close();
        }

        /*
         * Generate the proper TCP end point based on the client request.
         */
        private TcpClient generateTcpEndPoint(string[] methodParts)
        {
            if (methodParts.Length < 3)
            {
                return null;
            }

            TcpClient tcpEndPoint = null;
            string url = methodParts[1];
            Regex extraPattern = new Regex("http://|https://|\\?.*|/.*");

            // try to create a TcpClient for the end point
            try
            {
                if (url.StartsWith("http://"))
                {
                    url = extraPattern.Replace(url, "");
                    tcpEndPoint = new TcpClient(url, 80);
                }
                else if (url.StartsWith("https://"))
                {
                    url = extraPattern.Replace(url, "");
                    tcpEndPoint = new TcpClient(url, 443);
                }
                else if (UrlUtil.isDomainAndPort(url))
                {
                    string[] urlParts = methodParts[1].Split(':');
                    tcpEndPoint = new TcpClient(urlParts[0], Int32.Parse(urlParts[1]));
                }
            } catch (SocketException e)
            {
                LOGGER.Error("Failed to connect to " + url, e);
            }
            return tcpEndPoint;
        }
    }
}
