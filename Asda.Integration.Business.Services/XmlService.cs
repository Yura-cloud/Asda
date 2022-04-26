using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Asda.Integration.Domain.Models.Business.ShipmentConfirmation;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Business.Services
{
    public class XmlService : IXmlService
    {


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
        public  void CreateLocalDispatchXmlFile(List<ShipmentConfirmation> shipmentConfirmations, string path)
        {
            try
            {
                var writer = new XmlSerializer(typeof(List<ShipmentConfirmation>));
                
                var file = File.Create(path);

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                writer.Serialize(file, shipmentConfirmations, ns);
                file.Close();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed while working with CreateLocalDispatchXmlFile, with message {e.Message}");
            }
        }
        public  void CreateLocalAcknowledgmentXmlFile(Acknowledgment acknowledgment, string path)
        {
            try
            {
                var writer = new XmlSerializer(typeof(Acknowledgment));
                
                var file = File.Create(path);

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                writer.Serialize(file, acknowledgment, ns);
                file.Close();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed while working with CreateLocalAcknowledgmentXmlFile, with message {e.Message}");
            }
        }
    }
}