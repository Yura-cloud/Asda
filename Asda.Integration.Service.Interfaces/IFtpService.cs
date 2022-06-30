using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        List<PurchaseOrder> GetPurchaseOrderFromFtp(FtpSettingsModel ftpSettings, string path,
            string userToken, int pageNumber, out List<XmlError> xmlErrors, out bool lastPage);

        void CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath, string userToken,
            List<XmlError> xmlErrors) where T : IGetFileName;
    }
}