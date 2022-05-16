using System;
using System.IO;
using Newtonsoft.Json;

namespace LicenseManager
{
    public class SettingsManager
    {
        const string SettingsPath = "settings.json";
        public Settings Settings { get; private set; } = new();

        public SettingsManager()
        {
            if (!File.Exists(SettingsPath))
            {
                using (var fileStream = File.Create(SettingsPath))
                {
                }

                // Set standard Output path
                Settings.LicenseOutputPath = Directory.GetCurrentDirectory() + @"\output";
                Directory.CreateDirectory(Settings.LicenseOutputPath);
                SaveSettingsToFile();
            }
            else
            {
                ReadSettingsFromFile();
            }
        }

        void ReadSettingsFromFile()
        {
            var file = File.ReadAllText(SettingsPath);

            var json = JsonConvert.DeserializeObject<Settings>(file);
            Settings = json;
        }

        void SaveSettingsToFile()
        {
            var json = JsonConvert.SerializeObject(Settings);

            File.WriteAllText(SettingsPath, json);
        }

        public string GetOutputPath()
        {
            return Settings.LicenseOutputPath;
        }

        public string SetOutputPath(string value)
        {
            try
            {
                Settings.LicenseOutputPath = value;
                SaveSettingsToFile();

                return "Success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}