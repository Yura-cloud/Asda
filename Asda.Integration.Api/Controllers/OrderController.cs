using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Service.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using SampleChannel.Helpers;
using Address = Asda.Integration.Domain.Models.Order.Address;

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
        public OrdersResponse Orders([FromBody] OrdersRequest request)
        {
            if (request.PageNumber <= 0)
            {
                return new OrdersResponse {Error = "Invalid page number"};
            }

            var user = _userConfigAdapter.Load(request.AuthorizationToken);
            if (user == null)
            {
                _logger.LogError($"User with AuthToken: {request.AuthorizationToken} - not found.");
                return new OrdersResponse {Error = "User not found"};
            }

            try
            {
                var purchaseOrder = _orderService.GetPurchaseOrder();
                var order = Mapper.MapToOrder(purchaseOrder);

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
                return new OrderDespatchResponse {Error = "Invalid page number"};

            try
            {
                var user = this._userConfigAdapter.Load(request.AuthorizationToken);

                return new OrderDespatchResponse()
                {
                    Orders = new List<OrderDespatchError>()
                    {
                        new OrderDespatchError
                        {
                            ReferenceNumber = request.Orders.First().ReferenceNumber,
                            Error = "Despatch failed for some reason"
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new OrderDespatchResponse {Error = ex.Message};
            }
        }
    }
}