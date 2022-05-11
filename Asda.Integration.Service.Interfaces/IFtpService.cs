using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        void DownloadXmlFileFromFtp(string path);
        List<XmlError> CreateFiles<T>(List<T> models, string remotePath);
    }
}