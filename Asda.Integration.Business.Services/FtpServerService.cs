using System;
using System.IO;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Renci.SshNet;

namespace Asda.Integration.Business.Services
{
    public class FtpServerService : IFtpServerService
    {
        public FtpSettingsModel FtpSettings { get; set; }


        public FtpServerService(IFtpConfigManagerService ftpConfig)
        {
            FtpSettings = new FtpSettingsModel(ftpConfig.Port, ftpConfig.UserName, ftpConfig.Password,
                ftpConfig.Host, ftpConfig.ServerFilePath);
        }

        public void DownloadXmlFileFromServer(string path)
        {
            try
            {
                using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                    FtpSettings.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    using var stream = System.IO.File.Create(path);
                    client.DownloadFile(FtpSettings.ServerFilePath, stream);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with with GetXmlFileFromServer, with message {e.Message}";
                throw new Exception(message);
            }
        }

        public void SentFileToServer(string localPath, string remotePath)
        {
            try
            {
                using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                    FtpSettings.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    using var s = File.OpenRead(localPath);
                    client.UploadFile(s, remotePath);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with with SentFileToServer, with message {e.Message}";
                throw new Exception(message);
            }
        }

        public void SentFilesToServerTest(string localPath, string remotePath)
        {
            try
            {
                using var client = new SftpClient(FtpSettings.Host, FtpSettings.Port, FtpSettings.UserName,
                    FtpSettings.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    var di = new DirectoryInfo(localPath);
                    foreach (var file in di.GetFiles())
                    {
                        using var fileStream = File.OpenRead(file.FullName);
                        client.UploadFile(fileStream, $"{remotePath}{file.Name}");
                    }
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with with SentFileToServer, with message {e.Message}";
                throw new Exception(message);
            }
        }
    }
}