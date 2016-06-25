using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Utils
{
    class HeaderUtil
    {
        public static bool isReservedHeader(string header)
        {
            bool isReserved = false;
            switch (header)
            {
                case "Connection":
                case "Content-Length":
                case "Date":
                case "Expect":
                case "Host":
                case "If-Modified-Since":
                case "Range":
                case "Transfer-Encoding":
                case "Proxy-Connection":
                case "Accept":
                case "Content-Type":
                case "Referrer":
                case "User-Agent":
                case "Keep-Alive":
                    isReserved = true;
                    break;
                default:
                    isReserved = false;
                    break;
            }
            return isReserved;
        }
    }
}
