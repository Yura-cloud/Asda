using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpServerService
    {
        FtpSettingsModel FtpSettings { get; set; }
        void DownloadXmlFileFromServer(string path);
        void SendFilesToServer(string localPath, string remotePath);
        List<XmlError> CreateFiles<T>(List<T> models, string remotePath);
    }
}