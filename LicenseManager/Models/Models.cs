// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(License));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (License)serializer.Deserialize(reader);
// }

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Standard.Licensing;

namespace LicenseManager.Models
{
    [XmlRoot(ElementName="License")]
    public class License { 

        [XmlElement(ElementName="Id")] 
        public string Id { get; set; } 

        [XmlElement(ElementName="Type")] 
        public LicenseType Type { get; set; } 

        [XmlElement(ElementName="Expiration")] 
        public DateTime Expiration { get; set; } 

        [XmlElement(ElementName="Quantity")] 
        public int Quantity { get; set; } 

        [XmlElement(ElementName="ProductFeatures")] 
        public ICollection<Feature> ProductFeatures { get; set; } 

        [XmlElement(ElementName="Customer")] 
        public Customer Customer { get; set; } 

        [XmlElement(ElementName="Signature")] 
        public string Signature { get; set; } 
    }

    [XmlRoot(ElementName="Feature")]
    public class Feature { 

        [XmlAttribute(AttributeName="name")] 
        public string Name { get; set; } 

        [XmlText] 
        public string Text { get; set; } 
    }

    [XmlRoot(ElementName="Customer")]
    public class Customer { 

        [XmlElement(ElementName="Name")] 
        public string Name { get; set; } 

        [XmlElement(ElementName="Email")] 
        public string Email { get; set; } 
    }
}