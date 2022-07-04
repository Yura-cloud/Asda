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
                return GetErrorMessage(nameof(remoteFiles.OrdersPath));
            }

            if (!client.Exists(remoteFiles.DispatchesPath))
            {
                return GetErrorMessage(nameof(remoteFiles.DispatchesPath));
            }

            if (!client.Exists(remoteFiles.AcknowledgmentsPath))
            {
                return GetErrorMessage(nameof(remoteFiles.AcknowledgmentsPath));
            }

            if (!client.Exists(remoteFiles.CancellationsPath))
            {
                return GetErrorMessage(nameof(remoteFiles.CancellationsPath));
            }

            if (!client.Exists(remoteFiles.SnapInventoriesPath))
            {
                return GetErrorMessage(nameof(remoteFiles.SnapInventoriesPath));
            }

            return string.Empty;
        }

        private static string GetErrorMessage(string folderPath)
        {
            var propertyName = folderPath.Replace("Path", "");
            return $"Path to the {propertyName} does not exist on your FTP server!";
        }
    }
}