using Echo.Models;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;

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
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[setting].Value = newValue;
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        public static Dictionary<string, List<object>> ReadPresetFile()
        {
            try
            {
                string text = System.IO.File.ReadAllText(@"echo_stored_presets.json");
                Dictionary<string, List<object>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<object>>>(text);
                
                if (result is null)
                {
                    return new Dictionary<string, List<object>>();
                } else
                {
                    return result;
                }
            } catch(System.IO.FileNotFoundException)
            {
                File.Create(@"echo_stored_presets.json");
                return new Dictionary<string, List<object>>();
            }         
        }

        public static void AddPreset(string ip, int port, string name, string pass)
        {
            Dictionary<string, List<object>> currentPresets = ReadPresetFile();

            currentPresets[name] = new List<object> { ip, port, pass };

            string jsonData = JsonConvert.SerializeObject(currentPresets);
            System.IO.File.WriteAllText(@"echo_stored_presets.json", jsonData);
        }

        public static void DeletePreset(ServerPresetViewModel vm)
        {
            ServerPreset preset = vm.GetPreset();
            Dictionary<string, List<object>> currentPresets = ReadPresetFile();
            if (currentPresets is null)
            {
                return;
            } else
            {
                currentPresets.Remove(preset._serverName);
                string jsonData = JsonConvert.SerializeObject(currentPresets);
                System.IO.File.WriteAllText(@"echo_stored_presets.json", jsonData);
            }
        }

        public static string GetAppDataPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string fp = Path.Combine(appDataPath, "echo");

            return fp;
        }
    }
}
