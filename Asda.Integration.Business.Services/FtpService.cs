using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Asda.Integration.Business.Services
{
    public class FtpService : IFtpService
    {
        public FtpSettingsModel FtpSettings { get; set; }

        private readonly ILogger<FtpService> _logger;

        public FtpService(IFtpConfigManagerService ftpConfig, ILogger<FtpService> logger)
        {
            _logger = logger;
            FtpSettings = ftpConfig.FtpSettings;
        }

        public void DownloadXmlFileFromFtp(string path)
        {
            try
            {
                using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                    FtpSettings.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    using var stream = File.Create(path);
                    client.DownloadFile(FtpSettings.ServerFilePath, stream);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with DownloadXmlFileFromFtp, with message {e.Message}";
                throw new Exception(message);
            }
        }

        public List<XmlError> CreateFiles<T>(List<T> models, string remotePath)
        {
            var errorsXml = new List<XmlError>();
            using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                FtpSettings.Password);
            client.Connect();
            if (client.IsConnected)
            {
                for (var i = 0; i < models.Count; i++)
                {
                    try
                    {
                        var model = models[i];
                        var filePath = $"{remotePath}/file_{i + 1}.xml";
                        var fileStream = client.Create(filePath);

                        var namespaces = new XmlSerializerNamespaces();
                        namespaces.Add("", "");

                        var writer = new XmlSerializer(typeof(T));
                        writer.Serialize(fileStream, model, namespaces);
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
    }
}