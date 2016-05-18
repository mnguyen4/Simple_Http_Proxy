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

        private AppStorage()
        {
            preferences = new Hashtable();
        }

        public static AppStorage getInstance()
        {
            if (instance == null)
            {
                instance = new AppStorage();
            }

            return instance;
        }

        public Object getPreference(string key)
        {
            return preferences[key];
        }

        public void setPreference(string key, Object value)
        {
            preferences[key] = value;
        }
    }
}
