using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Asda.Integration.Business.Services
{
    public class FtpService : IFtpService
    {
        private readonly ILogger<FtpService> _logger;

        public FtpService(ILogger<FtpService> logger)
        {
            _logger = logger;
        }

        public List<T> GetFiles<T>(SftpClient sftpClient, List<string> filesPath, string userToken)
        {
            var purchaseOrders = new List<T>();
            var serializer = new XmlSerializer(typeof(T));
            foreach (var filePath in filesPath)
            {
                try
                {
                    using var stream = sftpClient.OpenRead(filePath);
                    purchaseOrders.Add((T) serializer.Deserialize(stream));
                }
                catch (Exception e)
                {
                    var message = $"Failed while deserialize, order =>{filePath}, with message: {e.Message}";
                    _logger.LogError($"UserToken: {userToken}; Error: {message}");
                }
            }

            return purchaseOrders;
        }

        public List<SftpFile> GetAllSortedFilesInfo(SftpClient sftpClient, string path)
        {
            var files = sftpClient.ListDirectory(path)
                .Where(f => f.IsRegularFile)
                .OrderBy(f => f.LastWriteTimeUtc)
                .ToList();

            return files;
        }

        public List<XmlError> CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath,
            string userToken) where T : IGetFileName
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
                throw new Exception(
                    $"Failed while working with CreateFiles with message: No such folder: {remotePath}");
            }

            var filePath = string.Empty;
            var xmlErrors = new List<XmlError>();
            for (var i = 0; i < models.Count; i++)
            {
                try
                {
                    var fileName = models[i].GetFileName();
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

            return xmlErrors;
        }
    }
}