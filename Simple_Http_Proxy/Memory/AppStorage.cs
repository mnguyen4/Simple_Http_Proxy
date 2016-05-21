using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Memory
{
    /*
     * AppStorage is a singleton class used to store application data.
     */
    class AppStorage
    {
        private static AppStorage instance;
        private Hashtable preferences;
        private HashSet<string> blacklist;
        private HashSet<string> whitelist;

        private AppStorage()
        {
            preferences = new Hashtable();
            blacklist = new HashSet<string>();
            whitelist = new HashSet<string>();
        }

        public static AppStorage getInstance()
        {
            if (instance == null)
            {
                instance = new AppStorage();
            }

            return instance;
        }

        public object getPreference(string key)
        {
            return preferences[key];
        }

        public void setPreference(string key, object value)
        {
            preferences[key] = value;
        }

        public bool addBlacklistItem(string item)
        {
            return blacklist.Add(item);
        }

        public bool removeBlacklistItem(string item)
        {
            return blacklist.Remove(item);
        }

        public bool isBlacklistItem(string item)
        {
            return blacklist.Contains(item);
        }

        public bool addWhitelistItem(string item)
        {
            return whitelist.Add(item);
        }

        public bool removeWhitelistItem(string item)
        {
            return whitelist.Remove(item);
        }

        public bool isWhitelistItem(string item)
        {
            return whitelist.Contains(item);
        }
    }
}
