using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
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

        public List<PurchaseOrder> GetPurchaseOrderFromFtp(FtpSettingsModel ftpSettings, string path)
        {
            try
            {
                using var client = new SftpClient(ftpSettings.Host, ftpSettings.Port, ftpSettings.UserName,
                    ftpSettings.Password);
                client.Connect();
                if (!client.IsConnected)
                {
                    var message = $"Failed while working with Ftp, client was not connected";
                    _logger.LogError(message);
                    throw new Exception(message);
                }

                var purchaseOrders = new List<PurchaseOrder>();
                var serializer = new XmlSerializer(typeof(PurchaseOrder));
                var files = client.ListDirectory(path);
                foreach (var sftpFile in files)
                {
                    if (sftpFile.Name != "." && sftpFile.Name != "..")
                    {
                        using var stream = client.OpenRead(sftpFile.FullName);
                        purchaseOrders.Add((PurchaseOrder) serializer.Deserialize(stream));
                    }
                }

                return purchaseOrders;
            }

            catch (Exception e)
            {
                var message = $"Failed while working with GetPurchaseOrderFromFtp, with message {e.Message}";
                throw new Exception(message);
            }
        }

        public List<XmlError> CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath)
        {
            var errorsXml = new List<XmlError>();
            using var client = new SftpClient(ftpSettings.Host, ftpSettings.Port, ftpSettings.UserName,
                ftpSettings.Password);
            client.Connect();
            if (client.IsConnected)
            {
                DeleteFiles(remotePath, client);
                for (var i = 0; i < models.Count; i++)
                {
                    try
                    {
                        var fileName = FileNamingHelper.GetFileName(models[i]);
                        var filePath = $"{remotePath}/{fileName}";
                        var fileStream = client.Create(filePath);

                        var namespaces = new XmlSerializerNamespaces();
                        namespaces.Add("", "");

                        var writer = new XmlSerializer(typeof(T));
                        writer.Serialize(fileStream, models[i], namespaces);
                        fileStream.Close();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Failed while creating file in {remotePath}, with message {e.Message}");
                        errorsXml.Add(new XmlError {Index = i, Message = e.Message});
                    }
                }
            }

            return errorsXml;
        }


        private void DeleteFiles(string remotePath, SftpClient client)
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