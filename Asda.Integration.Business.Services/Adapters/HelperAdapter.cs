using System;
using System.Text;
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

        public static string TestIfFoldersExist(FtpSettingsModel settings, RemoteFileStorageModel remoteFiles)
        {
            using var client = new SftpClient(settings.Host, settings.Port, settings.UserName, settings.Password);
            client.Connect();
            var pathsErrors = new StringBuilder();
            if (!client.Exists(remoteFiles.OrdersPath))
            {
                pathsErrors.Append("Orders, ");
            }

            if (!client.Exists(remoteFiles.DispatchesPath))
            {
                pathsErrors.Append("Dispatches, ");
            }

            if (!client.Exists(remoteFiles.AcknowledgmentsPath))
            {
                pathsErrors.Append("Acknowledgments, ");
            }

            if (!client.Exists(remoteFiles.CancellationsPath))
            {
                pathsErrors.Append("Cancellations, ");
            }

            if (!client.Exists(remoteFiles.SnapInventoriesPath))
            {
                pathsErrors.Append("SnapInventories, ");
            }

            if (pathsErrors.Length > 0)
            {
                pathsErrors.Append("folder(s) does not exist on your FTP server!");
            }

            return pathsErrors.ToString();
        }
    }
}