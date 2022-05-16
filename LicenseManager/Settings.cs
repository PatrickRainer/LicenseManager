using System.Collections.Generic;
using LicenseManager.Models;

namespace LicenseManager
{
    public class Settings
    {
        public string LicenseOutputPath { get; set; }

        //TODO: Fill by Settings
        public ICollection<Feature> AvailableFeatures { get; set; } = new List<Feature>()
        {
            new Feature() { Name = "Core", Text = "Yes" }
        };
    }
}