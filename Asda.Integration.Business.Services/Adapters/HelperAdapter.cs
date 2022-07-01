using System;
using Asda.Integration.Domain.Models.Business;
using Renci.SshNet;

namespace Asda.Integration.Business.Services.Adapters
{
    public static class HelperAdapter
    {
        //This method throws an exception if something goes wrong, and then the user can see it in his UI
        public static void TestFtpConnection(FtpSettingsModel settings)
        {
            try
            {
                using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
                client.Connect();
            }
            catch (Exception e)
            {
                throw new Exception($"FTP connection test failed with an error message: {e.Message}");
            }
        }

        public static string CheckExistingFolders(FtpSettingsModel settings, RemoteFileStorageModel remoteFiles)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            if (!client.Exists(remoteFiles.OrdersPath))
            {
                var propertyName = nameof(remoteFiles.OrdersPath).Replace("Path", "");
                return $"Path to the {propertyName} does not exist on your FTP server!";
            }

            if (!client.Exists(remoteFiles.DispatchesPath))
            {
                var propertyName = nameof(remoteFiles.DispatchesPath).Replace("Path", "");
                return $"Path to the {propertyName} does not exist on your FTP server!";
            }

            if (!client.Exists(remoteFiles.AcknowledgmentsPath))
            {
                var propertyName = nameof(remoteFiles.AcknowledgmentsPath).Replace("Path", "");
                return $"Path to the {propertyName} does not exist on your FTP server!";
            }

            if (!client.Exists(remoteFiles.CancellationsPath))
            {
                var propertyName = nameof(remoteFiles.CancellationsPath).Replace("Path", "");
                return $"Path to the {propertyName} does not exist on your FTP server!";
            }

            if (!client.Exists(remoteFiles.SnapInventoriesPath))
            {
                var propertyName = nameof(remoteFiles.SnapInventoriesPath).Replace("Path", "");
                return $"Path to the {propertyName} does not exist on your FTP server!";
            }

            return string.Empty;
        }
    }
}