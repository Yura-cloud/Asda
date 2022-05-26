using System.Linq;
using Asda.Integration.Domain.Models.Business;
using Renci.SshNet;

namespace Asda.Integration.Business.Services.Adapters
{
    public static class HelperAdapter
    {
        public static bool CanConnectToFtp(FtpSettingsModel settings)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            return client.IsConnected;
        }

        public static bool CheckExistingFolders(FtpSettingsModel settings, RemoteFileStorageModel remoteFiles)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            return CheckFolders(remoteFiles, client);
        }

        private static bool CheckFolders(RemoteFileStorageModel remoteFiles, SftpClient client)
        {
            var propertyInfos = remoteFiles.GetType().GetProperties();
            return propertyInfos.All(propertyInfo => client.Exists((string) propertyInfo.GetValue(remoteFiles)));
        }
    }
}