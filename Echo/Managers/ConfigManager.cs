using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;

namespace Echo.Managers
{
    class ConfigManager // Config file manager
    {
        public static List<string> ReadSettings()
        {
            List<string> data = new List<string> { };
            string version = ConfigurationManager.AppSettings.Get("version");       

            data.Add(version);

            return data;

        }

        public static string GetSetting(string setting)
        {
            return ConfigurationManager.AppSettings.Get(setting);
        }

        public static void UpdateSetting(string setting, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement element in xmlDoc.DocumentElement)
            {
                if (element.Name.Equals("appSettings"))
                {
                    foreach (XmlNode node in element.ChildNodes)
                    {
                        if (node.Attributes[0].Value.Equals(setting))
                        {
                            node.Attributes[1].Value = newValue;
                        }
                    }
                }
            }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}

