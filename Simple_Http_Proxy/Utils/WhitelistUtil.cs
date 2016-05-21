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
    class WhitelistUtil
    {
        public static void readWhitelist()
        {
            AppStorage storage = AppStorage.getInstance();
            if (File.Exists((string)storage.getPreference(Constant.WHITE_LOCATION_TEXT)))
            {
                string whitelistItem = "";
                StreamReader whitelistFile = new StreamReader((string)storage.getPreference(Constant.WHITE_LOCATION_TEXT));
                while ((whitelistItem = whitelistFile.ReadLine()) != null)
                {
                    storage.addWhitelistItem(whitelistItem);
                }
                whitelistFile.Close();
            }
        }
    }
}
