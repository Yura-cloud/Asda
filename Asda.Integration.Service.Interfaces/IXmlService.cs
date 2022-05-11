using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;

namespace Asda.Integration.Service.Interfaces
{
    public interface IXmlService
    {
        PurchaseOrder GetPurchaseOrderFromXml(string path);
        List<XmlError> CreateXmlFilesOnFtp<T>(List<T> list, XmlModelType modelType);

    }
}