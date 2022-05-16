using System.Collections.Generic;
using System.IO;
using System.Net;
using LicenseManager;
using NUnit.Framework;

namespace LicenseManagerTests
{
    public class KeyHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CanSaveKeyPairToFile()
        {
            var licManager = new LicManager();
            var keys=licManager.CreateKeys("AnyPassword");
            KeyHandler.SaveKeys(keys);

            var fileExist =File.Exists("key.json");
            Assert.True(fileExist);
        }

        [Test]
        public void CanReadKeyPairFromFile()
        {
            var sut = KeyHandler.LoadKeys();
            
            Assert.NotNull(sut);
            Assert.IsInstanceOf<Dictionary<string,string>>(sut);
        }
    }
}