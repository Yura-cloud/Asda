using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
        PurchaseOrder GetPurchaseOrder();

        void SendDispatchFile(List<ShipmentConfirmation> shipmentConfirmations);
        public void SendAcknowledgmentFile(Acknowledgment acknowledgment);
    }
}