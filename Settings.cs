using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostKit
{
    public class Settings
    {
        public int FavWorld { get; set; } = 1;
        public DetailSetting FavDetailSettings { get; set; } = DetailSetting.HIGH;

        public bool ShowChat = true;

        private static readonly string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data", "settings.json");

        public static Settings Load()
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
            }
            return new Settings();
        }

        public void Save()
        {
            string directory = Path.GetDirectoryName(settingsPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }
    }
}
