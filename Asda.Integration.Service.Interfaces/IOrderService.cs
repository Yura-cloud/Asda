using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Asda.Integration.Domain.Models.Business.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
        PurchaseOrder GetPurchaseOrder();

        void SendDispatchFile(List<ShipmentConfirmation> shipmentConfirmations);
        public void SendAcknowledgmentFile(Acknowledgment acknowledgment);
    }
}