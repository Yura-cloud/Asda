using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Intefaces
{
    public interface IXmlConvertor
    {
        PurchaseOrder GetPurchaseOrderFromXml(string path);
    }
}