using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Service.Intefaces
{
    public interface IOrderService
    {
    
        OrdersResponse GetOrdersAndSendManifest(OrdersRequest request);
        OrderDespatchResponse SendDispatch(OrderDespatchRequest request);
        OrderCancelResponse SendCanceledOrders(OrderCancelRequest request);
    }
}