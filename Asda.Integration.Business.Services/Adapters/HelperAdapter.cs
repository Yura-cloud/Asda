using System;
using System.Linq;
using Asda.Integration.Domain.Models.Business;
using Renci.SshNet;

namespace Asda.Integration.Business.Services.Adapters
{
    public static class HelperAdapter
    {
        public static void CanConnectToFtp(FtpSettingsModel settings)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
        }

        public static bool CheckExistingFolders(FtpSettingsModel settings, RemoteFileStorageModel remoteFiles,out string errorMessage)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            var propertyInfos = remoteFiles.GetType().GetProperties();
            errorMessage = string.Empty;
            foreach (var propertyInfo in propertyInfos)
            {
                if (!client.Exists((string) propertyInfo.GetValue(remoteFiles)))
                {
                    errorMessage = $"Path to the {propertyInfo.Name.Replace("Path","")}, does not exist on your FTP server!";
                    break;
                }
            }

            return errorMessage == string.Empty;
        }
    }
}