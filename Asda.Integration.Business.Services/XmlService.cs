using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services
{
    public class XmlService : IXmlService
    {
        private const string CancellationFileName = "Cancellation";

        private const string DispatchFileName = "DispatchConfirmation";

        private const string AcknowledgmentFileName = "Acknowledgment";

        private const string SnapShotFileName = "SnapShot";

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

        public void CreateLocalDispatchXmlFiles(List<ShipmentConfirmation> shipmentConfirmations, string path)
        {
            try
            {
                DeletePreviousFiles(path);
                for (var i = 0; i < shipmentConfirmations.Count; i++)
                {
                    var shipmentConfirmation = shipmentConfirmations[i];

                    var filePath = Path.Combine(path, $"{DispatchFileName}_{i + 1}{FileType}");
                    var fileStream = File.Create(filePath);

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    var writer = new XmlSerializer(typeof(ShipmentConfirmation));
                    writer.Serialize(fileStream, shipmentConfirmation, namespaces);
                    fileStream.Close();
                }
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
                DeletePreviousFiles(path);

                var filePath = Path.Combine(path, $"{AcknowledgmentFileName}{FileType}");
                var fileStream = File.Create(filePath);

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                var writer = new XmlSerializer(typeof(Acknowledgment));
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
                for (var i = 0; i < cancellations.Count; i++)
                {
                    var cancellation = cancellations[i];

                    var filePath = Path.Combine(path, $"{CancellationFileName}_{i + 1}{FileType}");
                    var fileStream = File.Create(filePath);

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    var writer = new XmlSerializer(typeof(Cancellation));
                    writer.Serialize(fileStream, cancellation, namespaces);
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed while working with CreateLocalCancellationsXmlFile, with message {e.Message}");
            }
        }

        public void CreateLocalSnapInventoriesXmlFiles(List<InventorySnapshot> inventorySnapshots,
            string path)
        {
            try
            {
                DeletePreviousFiles(path);
                for (var i = 0; i < inventorySnapshots.Count; i++)
                {
                    var inventorySnapshot = inventorySnapshots[i];
                    var filePath = Path.Combine(path, $"{SnapShotFileName}_{i + 1}{FileType}");
                    var fileStream = File.Create(filePath);

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    var writer = new XmlSerializer(typeof(InventorySnapshot));
                    writer.Serialize(fileStream, inventorySnapshot, namespaces);
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed while working with CreateLocalSnapInventoriesXmlFiles, with message {e.Message}");
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