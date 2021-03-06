﻿using log4net;
using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Utils
{
    class WhitelistUtil
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof (WhitelistUtil));

        public static void readWhitelist()
        {
            AppStorage storage = AppStorage.getInstance();
            if (File.Exists(storage.getPreference(Constant.WHITE_LOCATION_TEXT)))
            {
                string whitelistItem = "";
                StreamReader whitelistFile = new StreamReader(storage.getPreference(Constant.WHITE_LOCATION_TEXT));
                while ((whitelistItem = whitelistFile.ReadLine()) != null)
                {
                    storage.addWhitelistItem(whitelistItem);
                }
                whitelistFile.Close();
            }
        }

        public static void writeWhitelist()
        {
            AppStorage storage = AppStorage.getInstance();
            StreamWriter whitelistFile = new StreamWriter(storage.getPreference(Constant.WHITE_LOCATION_TEXT), false);
            try
            {
                foreach (string whitelistItem in storage.getWhitelist())
                {
                    whitelistFile.WriteLineAsync(whitelistItem);
                }
            }
            catch (Exception e) when (e is ObjectDisposedException || e is InvalidOperationException)
            {
                LOGGER.Error("Failed to write to whitelist file.", e);
            }
            finally
            {
                whitelistFile.Close();
            }
        }

        public static bool isWhitelistedDomain(HttpListenerRequest request)
        {
            bool isWhitelisted = false;
            string requestUrl = UrlUtil.parseDomainName(request.RawUrl);
            AppStorage storage = AppStorage.getInstance();
            isWhitelisted = storage.isWhitelistItem(requestUrl);
            if (!isWhitelisted)
            {
                requestUrl = UrlUtil.parseDomainName(request.UrlReferrer.OriginalString);
                isWhitelisted = storage.isWhitelistItem(requestUrl);
            }
            return isWhitelisted;
        }
    }
}
