using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
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
                var errorsIndex = _orderService.CreateXmlFilesOnFtp(new List<Acknowledgment> {acknowledgment},
                    XmlModelType.Acknowledgment);

                if (!errorsIndex.Any())
                {
                    return new OrdersResponse
                    {
                        Orders = new[] {order},
                        HasMorePages = false,
                    };
                }

                var message = $"There was en Error in this OrderReferenceNumber => {order.ReferenceNumber}";
                return new OrdersResponse {Error = message};
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
                var xmlErrors = _orderService.CreateXmlFilesOnFtp(shipmentConfirmations, XmlModelType.Dispatch);
                if (!xmlErrors.Any())
                {
                    return new OrderDespatchResponse();
                }

                var response = new OrderDespatchResponse
                {
                    Orders = new List<OrderDespatchError>()
                };

                foreach (var xmlError in xmlErrors)
                {
                    var error = new OrderDespatchError
                    {
                        ReferenceNumber = shipmentConfirmations[xmlError.Index].Request.ShipNoticeRequest
                            .ShipNoticePortion
                            .OrderReference.OrderID.ToString(),
                        Error = xmlError.Message
                    };
                    response.Orders.Add(error);
                }

                return response;
            }
            catch (Exception ex)
            {
                var message = $"Failed while working with Dispatch Action, with message {ex.Message}";
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
                var xmlErrors = _orderService.CreateXmlFilesOnFtp(cancellations, XmlModelType.Cancellations);

                if (!xmlErrors.Any())
                {
                    return new OrderCancelResponse();
                }

                var response = new OrderCancelResponse
                {
                    HasError = true
                };

                var messages = new StringBuilder();
                foreach (var xmlError in xmlErrors)
                {
                    messages.Append(xmlError.Message).AppendLine();
                }

                response.Error = messages.ToString();
                return response;
            }
            catch (Exception e)
            {
                var message = $"Failed while working with CancelOrders Action, with message {e.Message}";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = message, HasError = true};
            }
        }

        private List<ShipmentConfirmation> GetShipmentConfirmations(OrderDespatchRequest request)
        {
            var shipmentConfirmations = new List<ShipmentConfirmation>();
            foreach (var orderDispatch in request.Orders)
            {
                var shipmentConfirmation = ShipmentMapper.MapToShipmentConfirmation(orderDispatch);
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