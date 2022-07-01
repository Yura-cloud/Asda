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

        public static bool CheckExistingFolders(FtpSettingsModel settings, RemoteFileStorageModel remoteFiles,
            out string errorMessage)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            errorMessage = string.Empty;
            string propertyName;

            if (!client.Exists(remoteFiles.OrdersPath))
            {
                propertyName = nameof(remoteFiles.OrdersPath);
                errorMessage =
                    $"Path to the {propertyName.Replace("Path", "")} does not exist on your FTP server!";
                return false;
            }

            if (!client.Exists(remoteFiles.DispatchesPath))
            {
                propertyName = nameof(remoteFiles.DispatchesPath);
                errorMessage =
                    $"Path to the {propertyName.Replace("Path", "")} does not exist on your FTP server!";
                return false;
            }

            if (!client.Exists(remoteFiles.AcknowledgmentsPath))
            {
                propertyName = nameof(remoteFiles.AcknowledgmentsPath);
                errorMessage =
                    $"Path to the {propertyName.Replace("Path", "")} does not exist on your FTP server!";
                return false;
            }

            if (!client.Exists(remoteFiles.CancellationsPath))
            {
                propertyName = nameof(remoteFiles.CancellationsPath);
                errorMessage =
                    $"Path to the {propertyName.Replace("Path", "")} does not exist on your FTP server!";
                return false;
            }


            if (!client.Exists(remoteFiles.SnapInventoriesPath))
            {
                propertyName = nameof(remoteFiles.SnapInventoriesPath);
                errorMessage =
                    $"Path to the {propertyName.Replace("Path", "")} does not exist on your FTP server!";
                return false;
            }


            return errorMessage == string.Empty;
        }
    }
}