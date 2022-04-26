using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Asda.Integration.Domain.Models.Business.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IXmlService
    {
        PurchaseOrder GetPurchaseOrderFromXml(string path);
        void CreateLocalDispatchXmlFile(List<ShipmentConfirmation> shipmentConfirmations, string path);
        void CreateLocalAcknowledgmentXmlFile(Acknowledgment acknowledgment, string path);
    }
}