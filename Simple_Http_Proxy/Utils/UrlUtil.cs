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
            // url is already in domain name format
            if (Regex.IsMatch(url, @"^([A-Za-z0-9]+\.[A-Za-z0-9]+)$"))
            {
                return url;
            }
            // add protocol if the url doesn't contain it
            if (!url.Contains("http://"))
            {
                url = "http://" + url;
            }
            Uri uri = new Uri(url);
            string host = uri.Host;
            string[] hostParts = host.Split('.');
            int length = hostParts.Length;
            host = length > 2 ? hostParts[length - 2] + '.' + hostParts[length - 1] : host;
            return host;
        }
    }
}
