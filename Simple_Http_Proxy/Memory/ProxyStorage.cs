using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Memory
{
    class ProxyStorage
    {
        public byte[] buffer { set; get; }
        public int read { set; get; }
        public TcpClient tcpClient { set; get; }
        public TcpClient tcpEndPoint { set; get; }
    }
}
