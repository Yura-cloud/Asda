using System;
using System.Collections.Generic;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleChannel.Helpers;

namespace Asda.Integration.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IOrderService _orderService;

        private readonly ILogger<OrderController> _logger;

        public OrderController(IUserConfigAdapter userConfigAdapter, IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _userConfigAdapter = userConfigAdapter;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public OrdersResponse PullOrdersFromAsda([FromBody] OrdersRequest request)
        {
            if (request.PageNumber <= 0)
            {
                return new OrdersResponse {Error = "Invalid page number"};
            }

            try
            {
                var user = _userConfigAdapter.Load(request.AuthorizationToken);
                if (user == null)
                {
                    _logger.LogError($"User with AuthToken: {request.AuthorizationToken} - not found.");
                    return new OrdersResponse {Error = "User not found"};
                }

                var purchaseOrder = _orderService.GetPurchaseOrder();
                var order = OrderMapper.MapToOrder(purchaseOrder);

                var acknowledgment = AcknowledgmentMapper.MapToAcknowledgment(order.ReferenceNumber);
                _orderService.SendAcknowledgmentFile(acknowledgment);

                return new OrdersResponse
                {
                    Orders = new[] {order},
                    HasMorePages = false
                };
            }
            catch (Exception ex)
            {
                var message = $"Failed while working with Orders Action, with message: \n {ex.Message}";
                _logger.LogError(message);
                return new OrdersResponse {Error = message};
            }
        }

        [HttpPost]
        public OrderDespatchResponse Dispatch([FromBody] OrderDespatchRequest request)
        {
            if (request?.Orders == null || request.Orders?.Count == 0)
            {
                var message = $"Failed while working with Despatch Action, with message: Orders are Null or empty";
                _logger.LogError(message);
                return new OrderDespatchResponse {Error = message};
            }

            try
            {
                var user = _userConfigAdapter.Load(request.AuthorizationToken);
                if (user == null)
                {
                    var message = $"User with AuthToken: {request.AuthorizationToken} - not found.";
                    _logger.LogError(message);
                    return new OrderDespatchResponse {Error = message};
                }

                var shipmentConfirmations = GetShipmentConfirmations(request);
                _orderService.SendDispatchFiles(shipmentConfirmations);
                return new OrderDespatchResponse();
            }
            catch (Exception ex)
            {
                var message = $"Failed while working with Despatch Action, with message {ex.Message}";
                _logger.LogError(message);

                return new OrderDespatchResponse
                    {Error = message, Orders = OrderControllerHelper.AddReferenceNumbersFromRequest(request.Orders)};
            }
        }

        [HttpPost]
        public OrderCancelResponse CanceledOrders([FromBody] OrderCancelRequest request)
        {
            if (request?.Cancellation == null || request.Cancellation?.Items?.Count == 0)
            {
                var message = $"Failed while working with CancelOrders Action, with message: Items are Null or empty";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = message, HasError = true};
            }

            try
            {
                var user = _userConfigAdapter.Load(request.AuthorizationToken);
                if (user == null)
                {
                    var message = $"User with AuthToken: {request.AuthorizationToken} - not found.";
                    _logger.LogError(message);
                    return new OrderCancelResponse() {Error = message, HasError = true};
                }

                var cancellations = GetCancellations(request);
                _orderService.SendCancellationsFile(cancellations);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with CancelOrders Action, with message {e.Message}";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = message, HasError = true};
            }

            return new OrderCancelResponse();
        }

        private List<ShipmentConfirmation> GetShipmentConfirmations(OrderDespatchRequest request)
        {
            var shipmentConfirmations = new List<ShipmentConfirmation>();
            foreach (var orderDespatch in request.Orders)
            {
                var shipmentConfirmation = ShipmentMapper.MapToShipmentConfirmation(orderDespatch);
                shipmentConfirmations.Add(shipmentConfirmation);
            }

            return shipmentConfirmations;
        }

        private List<Cancellation> GetCancellations(OrderCancelRequest request)
        {
            var cancellations = new List<Cancellation>();
            for (int i = 0; i < request.Cancellation.Items.Count; i++)
            {
                var cancellation = CancellationMapper.MapToCancellation(request.Cancellation, i);
                cancellations.Add(cancellation);
            }

            return cancellations;
        }
    }
}