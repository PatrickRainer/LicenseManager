using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Standard.Licensing.Security.Cryptography;

namespace LicenseManager
{
    public class KeyHandler
    {

        /// <summary>
        /// Save the key pair to a JSON-File
        /// First has to be named PrivateKey, second has to be named PublicKey
        /// </summary>
        /// <param name="keys"></param>
        public static void SaveKeys(Dictionary<string, string> keys)
        {
            var json = JsonConvert.SerializeObject(keys);

            File.WriteAllText("key.json", json);
        }

        public static Dictionary<string,string> LoadKeys()
        {
            var text = File.ReadAllText("key.json");
            var keyPair = JsonConvert.DeserializeObject(text, typeof(Dictionary<string, string>)) as Dictionary<string,string>;

            return keyPair;
        }

        public static bool DoesKeyFileExist()
        {
            return File.Exists("key.json");
        }
    }
}