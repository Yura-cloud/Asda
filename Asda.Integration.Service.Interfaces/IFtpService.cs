using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Service.Intefaces;
using Renci.SshNet.Sftp;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        void CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath, string userToken,
            List<XmlError> xmlErrors) where T : IGetFileName;

        List<SftpFile> GetAllSftpFiles(FtpSettingsModel ftpSettings,string path);

        List<PurchaseOrder> GetPurchaseOrderFromFtp(FtpSettingsModel ftpSettings,
            List<SftpFile> files, string userToken, out List<XmlError> xmlErrors);
    }
}