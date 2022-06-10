using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Asda.Integration.Business.Services.Helpers;
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

        public FtpService(ILogger<FtpService> logger)
        {
            _logger = logger;
        }

        public List<PurchaseOrder> GetPurchaseOrderFromFtp(FtpSettingsModel ftpSettings, string path, string userToken,
            out List<XmlError> xmlErrors)
        {
            using var client = new SftpClient(ftpSettings.Host, ftpSettings.Port, ftpSettings.UserName,
                ftpSettings.Password);
            client.Connect();
            if (!client.IsConnected)
            {
                var message = $"Failed while working with GetPurchaseOrderFromFtp, client was not connected";
                _logger.LogError($"UserToken: {userToken}; {message}");
                throw new Exception(message);
            }

            var purchaseOrders = new List<PurchaseOrder>();
            var serializer = new XmlSerializer(typeof(PurchaseOrder));
            if (!client.Exists(path))
            {
                throw new Exception($"No such folder: {path}");
            }
            var files = client.ListDirectory(path);
            xmlErrors = new List<XmlError>();
            foreach (var sftpFile in files)
            {
                if (sftpFile.IsRegularFile)
                {
                    try
                    {
                        using var stream = client.OpenRead(sftpFile.FullName);
                        purchaseOrders.Add((PurchaseOrder) serializer.Deserialize(stream));
                    }
                    catch (Exception e)
                    {
                        var message = $"Failed while deserialize, order =>{sftpFile.FullName}, with message: {e.Message}";
                        _logger.LogError($"UserToken: {userToken}; {message}");
                        xmlErrors.Add(new XmlError()
                        {
                            Message = message
                        });
                    }
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
                var message = $"Failed while working with CreateFiles, client was not connected";
                _logger.LogError($"UserToken: {userToken}; {message}");
                throw new Exception(message);
            }
            if (!client.Exists(remotePath))
            {
                throw new Exception($"No such folder: {remotePath}");
            }
            UpdateFiles(remotePath, client);
            var filePath = string.Empty;
            for (var i = 0; i < models.Count; i++)
            {
                try
                {
                    var fileName = ((IGetFileName)models[i]).GetFileName();
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

        private void UpdateFiles(string remotePath, SftpClient client)
        {
            var files = client.ListDirectory(remotePath);
            foreach (var sftpFile in files)
            {
                if (sftpFile.Name is not ("." or ".."))
                {
                    client.DeleteFile(sftpFile.FullName);
                }
            }
        }
    }
}