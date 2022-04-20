using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
        PurchaseOrder GetPurchaseOrder();
    }
}