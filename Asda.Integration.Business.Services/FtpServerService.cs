using System;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Asda.Integration.Business.Services
{
    public class FtpServerService : IFtpServerService
    {
        private readonly ILogger<FtpServerService> _logger;
        public FtpSettingsModel FtpSettings { get; set; }
        public LocalSettingsModel LocalSettings { get; set; }


        public FtpServerService(IFtpConfigManagerService ftpConfig, ILocalConfigManagerService localLocalConfig,
            ILogger<FtpServerService> logger)
        {
            _logger = logger;
            FtpSettings = new FtpSettingsModel(ftpConfig.Port, ftpConfig.UserName, ftpConfig.Password,
                ftpConfig.Host, ftpConfig.ServerFilePath);
            LocalSettings = new LocalSettingsModel(localLocalConfig.LocalFilePath);
        }

        public void DownloadXmlFileFromServer()
        {
            try
            {
                using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                    FtpSettings.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    using var stream = System.IO.File.Create(LocalSettings.LocalFilePath);
                    client.DownloadFile(FtpSettings.ServerFilePath, stream);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with with GetXmlFileFromServer, with message {e.Message}";
                throw new Exception(message);
            }
        }
    }
}