using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Utils
{
    class UrlUtil
    {
        /*
         * Parse the domain name from the URL string.
         */
        public static string parseDomainName(string url)
        {
            // url is already in domain name format or in IP format
            if (Regex.IsMatch(url, @"^([A-Za-z0-9]+\.[A-Za-z0-9]+)$|^(([0-9]{1,3}\.){3}[0-9]{1,3})$"))
            {
                return url;
            }
            // remove protocol string if exists
            Regex protocolPattern = new Regex("http://|https://");
            if (protocolPattern.IsMatch(url))
            {
                url = protocolPattern.Replace(url, "");
            }
            // remove parameter string if exists
            Regex queryPattern = new Regex("/.*|\\?.*");
            if (queryPattern.IsMatch(url))
            {
                url = queryPattern.Replace(url, "");
            }
            string[] hostParts = url.Split('.');
            int length = hostParts.Length;
            url = length > 2 ? hostParts[length - 2] + '.' + hostParts[length - 1] : url;
            return url;
        }
    }
}
