using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class XmlConvertor : IXmlConvertor
    {
        private readonly ILogger<XmlConvertor> _logeer;

        public XmlConvertor(ILogger<XmlConvertor> logeer)
        {
            _logeer = logeer;
        }

        public PurchaseOrder GetPurchaseOrderFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(PurchaseOrder));
            try
            {
                using var reader = new StreamReader(path);
                return (PurchaseOrder) serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with with GetXmlFileFromServer, with message {e.Message}";
                throw new Exception(message);
            }
        }
    }
}