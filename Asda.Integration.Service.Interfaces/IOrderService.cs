using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.ShipmentConfirmation;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
        PurchaseOrder GetPurchaseOrder();

        void SentDispatchFile(List<ShipmentConfirmation> shipmentConfirmations);
    }
}