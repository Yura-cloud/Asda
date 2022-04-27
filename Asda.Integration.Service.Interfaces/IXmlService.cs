using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IXmlService
    {
        PurchaseOrder GetPurchaseOrderFromXml(string path);
        void CreateLocalDispatchXmlFile(List<ShipmentConfirmation> shipmentConfirmations, string path);
        void CreateLocalAcknowledgmentXmlFile(Acknowledgment acknowledgment, string path);
    }
}