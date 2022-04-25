using System.Collections.Generic;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Order;

namespace SampleChannel.Helpers
{
    public class OrderControllerHelper
    {
        public static List<OrderDespatchError> AddReferenceNumbersFromRequest(List<OrderDespatch> despatchOrders)
        {
            var orders = new List<OrderDespatchError>();
            foreach (var orderDespatch in despatchOrders)
            {
                var order = new OrderDespatchError
                {
                    ReferenceNumber = orderDespatch.ReferenceNumber
                };
                orders.Add(order);
            }

            return orders;
        }
    }
}