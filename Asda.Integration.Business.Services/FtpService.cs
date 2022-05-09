using System;
using System.IO;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Renci.SshNet;

namespace Asda.Integration.Business.Services
{
    public class FtpService : IFtpServerService
    {
        public FtpSettingsModel FtpSettings { get; set; }


        public FtpService(IFtpConfigManagerService ftpConfig)
        {
            FtpSettings = ftpConfig.FtpSettings;
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
                    using var stream = File.Create(path);
                    client.DownloadFile(FtpSettings.ServerFilePath, stream);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with GetXmlFileFromServer, with message {e.Message}";
                throw new Exception(message);
            }
        }

        public void SendFilesToServer(string localPath, string remotePath)
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
                        client.UploadFile(fileStream, $"{remotePath}/{file.Name}");
                    }
                }
            }
            catch (Exception e)
            {
                var message = $"Failed while working with SentFileToServer, with message {e.Message}";
                throw new Exception(message);
            }
        }
    }
}