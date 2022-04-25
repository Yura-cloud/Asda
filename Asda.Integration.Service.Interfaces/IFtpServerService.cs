using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Intefaces
{
    public interface IFtpServerService
    {
        FtpSettingsModel FtpSettings { get; set; }
        void DownloadXmlFileFromServer(string path);
        void SentFileToServer(string localPath, string remotePath);
    }
}