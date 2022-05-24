using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        List<PurchaseOrder> GetPurchaseOrderFromFtp(string path);
        List<XmlError> CreateFiles<T>(List<T> models, string remotePath);
    }
}