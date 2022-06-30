using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Asda.Integration.Business.Services
{
    public class FtpService : IFtpService
    {
        private readonly ILogger<FtpService> _logger;

        private const int MaxOrders = 50;

        public FtpService(ILogger<FtpService> logger)
        {
            _logger = logger;
        }

        public List<PurchaseOrder> GetPurchaseOrderFromFtp(FtpSettingsModel ftpSettings, string path, string userToken,
            int pageNumber, out List<XmlError> xmlErrors, out bool lastPage)
        {
            using var client = new SftpClient(ftpSettings.Host, ftpSettings.Port, ftpSettings.UserName,
                ftpSettings.Password);
            client.Connect();
            if (!client.IsConnected)
            {
                var message = "Client was not connected";
                _logger.LogError($"Failed while working with GetPurchaseOrderFromFtp with message: {message}");
                throw new Exception(message);
            }

            if (!client.Exists(path))
            {
                var message = $"No such folder: {path}";
                _logger.LogError($"Failed while working with GetPurchaseOrderFromFtp with message: {message}");
                throw new Exception($"{message}");
            }

            var purchaseOrders = new List<PurchaseOrder>();
            var serializer = new XmlSerializer(typeof(PurchaseOrder));
            var files = client.ListDirectory(path).Where(f => f.IsRegularFile).ToList();
            xmlErrors = new List<XmlError>();
            //load certain amount of Orders
            var count = pageNumber * MaxOrders;
            if ((pageNumber - 1) * MaxOrders + MaxOrders > files.Count)
            {
                count = files.Count;
            }

            lastPage = count == files.Count;
            for (int i = (pageNumber - 1) * MaxOrders; i < count; i++)
            {
                try
                {
                    using var stream = client.OpenRead(files[i].FullName);
                    purchaseOrders.Add((PurchaseOrder) serializer.Deserialize(stream));
                }
                catch (Exception e)
                {
                    var message = $"Failed while deserialize, order =>{files[i].FullName}, with message: {e.Message}";
                    _logger.LogError($"UserToken: {userToken}; {message}");
                    xmlErrors.Add(new XmlError {Message = message});
                }
            }

            return purchaseOrders;
        }

        public void CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath, string userToken,
            List<XmlError> xmlErrors)
        {
            using var client = new SftpClient(ftpSettings.Host, ftpSettings.Port, ftpSettings.UserName,
                ftpSettings.Password);
            client.Connect();
            if (!client.IsConnected)
            {
                var message = "Client was not connected";
                _logger.LogError($"Failed while working with CreateFiles with message: {message}");
                throw new Exception(message);
            }

            if (!client.Exists(remotePath))
            {
                throw new Exception($"Failed while working with CreateFiles with message: No such folder: {remotePath}");
            }

            var filePath = string.Empty;
            for (var i = 0; i < models.Count; i++)
            {
                try
                {
                    var fileName = ((IGetFileName) models[i]).GetFileName();
                    filePath = $"{remotePath}/{fileName}";
                    var fileStream = client.Create(filePath);

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    var writer = new XmlSerializer(typeof(T));
                    writer.Serialize(fileStream, models[i], namespaces);
                    fileStream.Close();
                }
                catch (Exception e)
                {
                    var message = $"Failed while creating file => {filePath}, with message {e.Message}";
                    _logger.LogError($"UserToken: {userToken}; {message}");
                    xmlErrors.Add(new XmlError {Index = i, Message = message});
                }
            }
        }
    }
}