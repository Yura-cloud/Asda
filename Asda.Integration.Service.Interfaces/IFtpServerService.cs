using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpServerService
    {
        FtpSettingsModel FtpSettings { get; set; }
        void DownloadXmlFileFromServer(string path);
        void SentFilesToServer(string localPath, string remotePath);
    }
}