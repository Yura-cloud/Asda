using System;
using System.Xml.Serialization;

namespace Asda.Integration.Domain.Models.Business.XML
{
    public class HeaderBase
    {
        public HeaderBase()
        {
            PayloadID = $"{Guid.NewGuid()}@linnworks.domain.com";
            Lang = "en";
            Text = "";
            Timestamp = DateTime.UtcNow;
            Header = new Header
            {
                From = new From
                {
                    Credential = new Credential {Domain = "AsdaOrganisation", Identity = "ASDA-123456-DC"}
                },
                To = new To
                {
                    Credential = new Credential {Domain = "AsdaOrganisation", Identity = "ASDA"}
                },
                Sender = new Sender
                {
                    Credential = new Credential {Domain = "Linnworks", Identity = "Linnworks"}
                }
            };
        }
        [XmlElement(ElementName = "Header")]
        public Header Header { get; set; }

        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }

        [XmlAttribute(AttributeName = "payloadID")]
        public string PayloadID { get; set; }

        [XmlAttribute(AttributeName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "From")]
    public class From
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }
    }

    [XmlRoot(ElementName = "To")]
    public class To
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }
    }

    [XmlRoot(ElementName = "Sender")]
    public class Sender
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }

        [XmlElement(ElementName = "UserAgent")]
        public string UserAgent { get; set; }
    }
    
    [XmlRoot(ElementName = "Header")]
    public class Header
    {
        [XmlElement(ElementName = "From")]
        public From From { get; set; }

        [XmlElement(ElementName = "To")]
        public To To { get; set; }

        [XmlElement(ElementName = "Sender")]
        public Sender Sender { get; set; }
    }

    [XmlRoot(ElementName = "Credential")]
    public class Credential
    {
        [XmlElement(ElementName = "Identity")]
        public string Identity { get; set; }

        [XmlAttribute(AttributeName = "domain")]
        public string Domain { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}