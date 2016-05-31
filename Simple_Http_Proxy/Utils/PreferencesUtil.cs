using Simple_Http_Proxy.Constants;
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
        private const string SETTINGS_ELEMENT = "appSettings";
        private const string KEY = "key";
        private const string VALUE = "value";

        /*
         * Load preferences from either a custom preferences.xml or from the App.config.
         */
        public static void loadPreferences()
        {
            AppStorage storage = AppStorage.getInstance();
            // read custom preferences if exists
            if (File.Exists(Constant.CONFIG_FILE_LOCATION)) {
                readPreferencesXml();
            }
            // read default preferences
            else
            {
                readDefaultPreferences();
            }
        }

        /*
         * Write preferences to a custom preferences.xml file.
         */
        public static void writePreferences()
        {
            AppStorage storage = AppStorage.getInstance();
            // if preferences file doesn't exist, create it
            if (!File.Exists(Constant.CONFIG_FILE_LOCATION))
            {
                XmlWriter preferenceWriter = XmlWriter.Create(Constant.CONFIG_FILE_LOCATION);
                preferenceWriter.WriteStartDocument();
                preferenceWriter.WriteStartElement("configuration");
                preferenceWriter.WriteStartElement("appSettings");
                foreach (string key in storage.getPreferences().Keys)
                {
                    preferenceWriter.WriteStartElement("add");
                    preferenceWriter.WriteAttributeString("key", key);
                    preferenceWriter.WriteAttributeString("value", storage.getPreference(key));
                    preferenceWriter.WriteEndElement();
                }
                preferenceWriter.WriteEndElement();
                preferenceWriter.WriteEndElement();
                preferenceWriter.WriteEndDocument();
                preferenceWriter.Close();
            }
            // otherwise, modify the existing one
            else
            {
                var preferencesMap = new ExeConfigurationFileMap();
                // set the location of the preferences file
                preferencesMap.ExeConfigFilename = Constant.CONFIG_FILE_LOCATION;
                // read the preferences
                Configuration preferencesFile = ConfigurationManager.OpenMappedExeConfiguration(preferencesMap, ConfigurationUserLevel.None);
                foreach (string key in storage.getPreferences().Keys)
                {
                    preferencesFile.AppSettings.Settings[key].Value = storage.getPreference(key);
                }
                preferencesFile.Save(ConfigurationSaveMode.Modified);
                // Force reread from disk on next access
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        /*
         * Delete the custom preferences file and use the default configurations.
         */
        public static void deletePreferencesXml()
        {
            if (File.Exists(Constant.CONFIG_FILE_LOCATION))
            {
                File.Delete(Constant.CONFIG_FILE_LOCATION);
            }
            readDefaultPreferences();
        }

        /*
         * Retrieve the setting value from preferences.xml.
         */
        private static void readPreferencesXml()
        {
            AppStorage storage = AppStorage.getInstance();
            var preferencesMap = new ExeConfigurationFileMap();
            preferencesMap.ExeConfigFilename = Constant.CONFIG_FILE_LOCATION;
            Configuration preferencesFile = ConfigurationManager.OpenMappedExeConfiguration(preferencesMap, ConfigurationUserLevel.None);
            foreach (string key in preferencesFile.AppSettings.Settings.AllKeys)
            {
                storage.setPreference(key, preferencesFile.AppSettings.Settings[key].Value);
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
