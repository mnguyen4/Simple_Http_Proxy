using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Http_Proxy.Constants
{
    class Constant
    {
        // Preferences constants
        public const string HOST_NAME_TEXT = "hostnameTxt";
        public const string PORT_TEXT = "portTxt";
        public const string SSL_CHECK = "sslChk";
        public const string SSL_PORT_TEXT = "sslPortTxt";
        public const string BLACK_LOCATION_TEXT = "blackLocationTxt";
        public const string WHITE_LOCATION_TEXT = "whiteLocationTxt";

        // Preferences XML constants
        public const string ADD = "add";
        public const string APP_SETTINGS = "appSettings";
        public const string CONFIGURATION = "configuration";
        public const string KEY = "key";
        public const string VALUE = "value";

        // Proxy constants
        public const string WEB_REQUEST = "webRequest";
        public const string ORIGINAL_CONTEXT = "originalContext";

        // Context menu constants
        public const string CLOSE = "Close";

        // Common constants
        public const string BLACKLIST_OP = "blacklist";
        public const string WHITELIST_OP = "whitelist";
        public const string TRUE = "true";
        public const string FALSE = "false";

        // Misc constants
        public const string CONFIG_FILE_LOCATION = "cfg\\preferences.xml";
        public const string MAIN_ICON_LOCATION = "Images\\main.ico";
    }
}
