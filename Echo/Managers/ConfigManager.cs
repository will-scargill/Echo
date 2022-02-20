using System;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;

namespace Echo.Managers
{
    class ConfigManager // Config file manager
    {
        public static string GetSetting(string setting)
        {
            return ConfigurationManager.AppSettings.Get(setting);
        }

        public static void UpdateSetting(string setting, string newValue)
        {
            ConfigurationManager.AppSettings.Set(setting, newValue);
        }
    }
}
