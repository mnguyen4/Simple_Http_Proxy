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
    }
}
