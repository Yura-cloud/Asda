using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
        PurchaseOrder GetPurchaseOrder();

        void SendDispatchFiles(List<ShipmentConfirmation> shipmentConfirmations);
        void SendAcknowledgmentFile(Acknowledgment acknowledgment);
        void SendCancellationsFiles(List<Cancellation> cancellations);
        void SendSnapInventoriesFiles(List<InventorySnapshot> inventorySnapshots );
    }
}