using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Licensing;
using Standard.Licensing.Validation;


namespace LicenseManager
{
    public class LicManager
    {
        public LicManager()
        {
            
        }

        public Dictionary<string,string> CreateKeys(string password)
        {
            var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            var privateKey = keyPair.ToEncryptedPrivateKeyString(password);
            var publicKey = keyPair.ToPublicKeyString();

            return new Dictionary<string, string>() { {"PrivateKey",privateKey}, { "PublicKey",publicKey } };
        }

        public string CreateLicense(string password, string privateKey, LicenseManager.Models.License licenseModel)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(privateKey))
            {
                return "Error, password or private key not provided";
            }


            try
            {
                var license = License.New()
                    .WithUniqueIdentifier(Guid.NewGuid())
                    .As(licenseModel.Type)
                    .ExpiresAt(licenseModel.Expiration)
                    .WithMaximumUtilization(licenseModel.Quantity)
                    .WithProductFeatures(licenseModel.ProductFeatures
                        .ToDictionary(feature => feature.Name, feature => feature.Text))
                    .LicensedTo(licenseModel.Customer.Name, licenseModel.Customer.Email)
                    .CreateAndSignWithPrivateKey(privateKey, password);

                return license.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ValidateLicense(string licenseKey,string publicKey)
        {
            if (string.IsNullOrEmpty(licenseKey)) return "No Licence Key passed";

            var license = License.Load(licenseKey);


            var validationFailures = license.Validate()
                .ExpirationDate()
                .When(lic => lic.Type == LicenseType.Trial)
                .And()
                .Signature(publicKey)
                .AssertValidLicense().ToList();

            if (!validationFailures.Any())
            {
                return "Validation successful";
            }
            else
            {
                var failureText = "";
                foreach (var failure in validationFailures)
                    failureText += failure.GetType().Name + ": " + failure.Message + " - " + failure.HowToResolve;

                return failureText;
            }


            // Optional instead of iterating through
            /*if (validationFailures.Any())
            // ...*/
        }
    }
}