using Simple_Http_Proxy.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Simple_Http_Proxy.Utils
{
    class PreferencesUtil
    {
        private const string PREFERENCES_PATH = "cfg\\preferences.xml";
        private const string SETTINGS_ELEMENT = "appSettings";
        private const string KEY = "key";
        private const string VALUE = "value";

        /*
         * Load preferences front either a custom preferences.xml or from the App.config.
         */
        public static void loadPreferences()
        {
            // read custom preferences if exists
            if (File.Exists(PREFERENCES_PATH)) {
                readPreferencesXml();
            }
            // read default preferences
            else
            {
                readDefaultPreferences();
            }
        }

        /*
         * Retrieve the setting value from preferences.xml.
         */
         private static void readPreferencesXml()
        {
            AppStorage storage = AppStorage.getInstance();
            StreamReader reader = new StreamReader(new FileStream(PREFERENCES_PATH, FileMode.Open, FileAccess.Read, FileShare.Read));
            XmlDocument preferences = new XmlDocument();
            preferences.LoadXml(reader.ReadToEnd());
            reader.Close();
            XmlElement appSettings = preferences.GetElementById(SETTINGS_ELEMENT);
            foreach (XmlNode add in appSettings.ChildNodes)
            {
                storage.setPreference(add.Attributes["key"].Value, add.Attributes["value"].Value);
            }
        }

        /*
         * Retrieve the setting value from App.config.
         */
        private static void readDefaultPreferences()
        {
            AppStorage storage = AppStorage.getInstance();
            var appSettings = ConfigurationManager.AppSettings;
            foreach (string key in appSettings.AllKeys) {
                storage.setPreference(key, appSettings[key]);
            }
        }
    }
}
