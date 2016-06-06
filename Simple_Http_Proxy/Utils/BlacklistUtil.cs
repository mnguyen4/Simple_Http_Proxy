using log4net;
using Simple_Http_Proxy.Constants;
using Simple_Http_Proxy.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Utils
{
    class BlacklistUtil
    {
        public static ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void readBlacklist()
        {
            AppStorage storage = AppStorage.getInstance();
            if (File.Exists(storage.getPreference(Constant.BLACK_LOCATION_TEXT)))
            {
                string blacklistItem = "";
                StreamReader blacklistFile = new StreamReader(storage.getPreference(Constant.BLACK_LOCATION_TEXT));
                while ((blacklistItem = blacklistFile.ReadLine()) != null)
                {
                    storage.addBlacklistItem(blacklistItem);
                }
                blacklistFile.Close();
            }
        }

        public static void writeBlackList()
        {
            AppStorage storage = AppStorage.getInstance();
            StreamWriter blacklistFile = new StreamWriter(storage.getPreference(Constant.BLACK_LOCATION_TEXT), false);
            try
            {
                foreach (string blacklistItem in storage.getBlacklist())
                {
                    blacklistFile.WriteLineAsync(blacklistItem);
                }
            }
            catch(Exception e) when (e is ObjectDisposedException || e is InvalidOperationException)
            {
                LOGGER.Error("Failed to write to blacklist file.", e);
            }
            finally
            {
                blacklistFile.Close();
            }
        }
    }
}
