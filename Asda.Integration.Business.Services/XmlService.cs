using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services
{
    public class XmlService : IXmlService
    {
        private const string CancellationFileName = "Cancellation";

        private const string FileType = ".xml";

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

        public void CreateLocalDispatchXmlFile(List<ShipmentConfirmation> shipmentConfirmations, string path)
        {
            try
            {
                var writer = new XmlSerializer(typeof(List<ShipmentConfirmation>));

                var fileStream = File.Create(path);
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                writer.Serialize(fileStream, shipmentConfirmations, namespaces);
                fileStream.Close();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed while working with CreateLocalDispatchXmlFile, with message {e.Message}");
            }
        }

        public void CreateLocalAcknowledgmentXmlFile(Acknowledgment acknowledgment, string path)
        {
            try
            {
                var fileStream = File.Create(path);

                var writer = new XmlSerializer(typeof(Acknowledgment));
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                writer.Serialize(fileStream, acknowledgment, namespaces);
                fileStream.Close();
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed while working with CreateLocalAcknowledgmentXmlFile, with message {e.Message}");
            }
        }

        public void CreateLocalCancellationXmlFiles(List<Cancellation> cancellations, string path)
        {
            try
            {
                DeletePreviousFiles(path);
                foreach (var cancellation in cancellations)
                {
                    var fileStream = File.Create(Path.Combine(path, CancellationFileName,
                        $"_{cancellation.Request.ConfirmationRequest.ConfirmationItem}", FileType));

                    var writer = new XmlSerializer(typeof(Cancellation));
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    writer.Serialize(fileStream, cancellation, namespaces);
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed while working with CreateLocalCancellationsXmlFile, with message {e.Message}");
            }
        }

        private void DeletePreviousFiles(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}