using System;
using System.Collections.Generic;
using System.Linq;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Service.Intefaces;
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


        /// <summary>
        /// This call is made by Linnworks automation to get a list of orders since the last time it
        /// requested orders. These calls are made every 10 to 15 minutes usually. The request expects
        /// a page result back, if there are a lot of orders to return it is suggested to split the
        /// result into pages of 100 maximum.
        /// </summary>
        /// <param name="request"><see cref="OrdersRequest"/></param>
        /// <returns><see cref="OrdersResponse"/></returns>
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

        /// <summary>
        /// When an order is despatched in Linnworks this call is made to update the channel with the
        /// correct despatch details. Entire orders may be submitted or partial orders depending if
        /// the order has been split.
        /// </summary>
        /// <param name="request"><see cref="OrderDespatchRequest"/></param>
        /// <returns><see cref="OrderDespatchResponse"/></returns>
        [HttpPost]
        public OrderDespatchResponse Despatch([FromBody] OrderDespatchRequest request)
        {
            if (request.Orders == null || request.Orders.Count == 0)
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

                var shipmentConfirmations = ShipmentMapper.MapToShipmentConfirmations(request.Orders);
                _orderService.SendDispatchFile(shipmentConfirmations);
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
    }
}