using System.Collections.Generic;
using System.Linq;
using Asda.Integration.Domain.Models.Order;

namespace SampleChannel.Helpers
{
    public class OrderControllerHelper
    {
        public static List<OrderDespatchError> AddReferenceNumbersFromRequest(List<OrderDespatch> despatchOrders)
        {
            return despatchOrders.Select(o => new OrderDespatchError
            {
                ReferenceNumber = o.ReferenceNumber
            }).ToList();
        }
    }
}