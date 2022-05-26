using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using SampleChannel.Helpers;

namespace Asda.Integration.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IFtpService _ftp;

        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<OrderService> _logger;

        public OrderService(IFtpService ftp, IUserConfigAdapter userConfigAdapter, ILogger<OrderService> logger)
        {
            _ftp = ftp;
            _userConfigAdapter = userConfigAdapter;
            _logger = logger;
        }

        public OrdersResponse GetOrdersAndSendManifest(OrdersRequest request)
        {
            if (request.PageNumber <= 0)
            {
                return new OrdersResponse {Error = "Invalid page number"};
            }

            try
            {
                if (UserUnauthorizedTest(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrdersResponse {Error = errorMessage};
                }

                var purchaseOrders = GetPurchaseOrder(user.FtpSettings, user.RemoteFileStorage.OrderPath);
                if (purchaseOrders == null)
                {
                    return new OrdersResponse();
                }

                var orders = purchaseOrders.Select(OrderMapper.MapToOrder);
                var acknowledgments = orders.Select(o => AcknowledgmentMapper.MapToAcknowledgment(o.ReferenceNumber));
                var xmlErrors = _ftp.CreateFiles(acknowledgments.ToList(), user.FtpSettings,
                    user.RemoteFileStorage.AcknowledgmentPath);
                if (!xmlErrors.Any())
                {
                    return new OrdersResponse {Orders = orders.ToArray(), HasMorePages = false,};
                }

                var errors = xmlErrors
                    .Select(e => e.Message)
                    .Aggregate(string.Empty, (current, next) => current + next);
                return new OrdersResponse {Error = errors};
            }
            catch (Exception e)
            {
                var message = $"Failed while working with Orders Action, with message: \\n {e.Message}";
                _logger.LogError(message);
                return new OrdersResponse {Error = message};
            }
        }

        public OrderDespatchResponse SendDispatch(OrderDespatchRequest request)
        {
            if (OrdersEmpty(request, out var ordersEmptyResponse))
            {
                return ordersEmptyResponse;
            }

            try
            {
                if (UserUnauthorizedTest(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrderDespatchResponse {Error = errorMessage};
                }

                var shipmentConfirmations = request.Orders.Select(ShipmentMapper.MapToShipmentConfirmation).ToList();
                var xmlErrors = _ftp.CreateFiles(shipmentConfirmations, user.FtpSettings,
                    user.RemoteFileStorage.DispatchPath);
                return !xmlErrors.Any()
                    ? new OrderDespatchResponse()
                    : ErrorDispatchResponse(xmlErrors, shipmentConfirmations);
            }
            catch (Exception ex)
            {
                var message = $"Failed while working with Dispatch Action, with message {ex.Message}";
                _logger.LogError(message);

                return new OrderDespatchResponse
                    {Error = message, Orders = OrderControllerHelper.AddReferenceNumbersFromRequest(request.Orders)};
            }
        }

        public OrderCancelResponse SendCanceledOrders(OrderCancelRequest request)
        {
            if (ItemsEmpty(request, out var itemsEmptyResponse))
            {
                return itemsEmptyResponse;
            }

            try
            {
                if (UserUnauthorizedTest(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrderCancelResponse() {Error = errorMessage, HasError = true};
                }

                var cancellation = CancellationMapper.MapToCancellation(request.Cancellation);
                var xmlErrors = _ftp.CreateFiles(new List<Cancellation> {cancellation}, user.FtpSettings,
                    user.RemoteFileStorage.CancellationPath);

                return !xmlErrors.Any() ? new OrderCancelResponse() : ErrorCancelResponse(xmlErrors);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with CancelOrders Action, with message {e.Message}";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = message, HasError = true};
            }
        }

        private List<PurchaseOrder> GetPurchaseOrder(FtpSettingsModel ftpSettings, string orderPath)
        {
            return _ftp.GetPurchaseOrderFromFtp(ftpSettings, orderPath);
        }

        private OrderCancelResponse ErrorCancelResponse(List<XmlError> xmlErrors)
        {
            var response = new OrderCancelResponse {HasError = true};

            var messages = new StringBuilder();
            foreach (var xmlError in xmlErrors)
            {
                messages.Append(xmlError.Message).AppendLine();
            }

            response.Error = messages.ToString();
            return response;
        }

        private OrderDespatchResponse ErrorDispatchResponse(List<XmlError> xmlErrors,
            List<ShipmentConfirmation> shipmentConfirmations)
        {
            var response = new OrderDespatchResponse {Orders = new List<OrderDespatchError>()};

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

        private bool UserUnauthorizedTest(string token, out string errorMessage, out UserConfig user)
        {
            user = _userConfigAdapter.LoadByToken(token);
            if (user == null)
            {
                errorMessage = $"User with AuthToken: {token} - not found.";
                _logger.LogError(errorMessage);
                return true;
            }

            errorMessage = string.Empty;
            return false;
        }

        private bool OrdersEmpty(OrderDespatchRequest request, out OrderDespatchResponse response)
        {
            if (request?.Orders == null || request.Orders?.Count == 0)
            {
                var message = $"Failed while working with Dispatch Action, with message: Orders are Null or empty";
                _logger.LogError(message);

                response = new OrderDespatchResponse {Error = message};
                return true;
            }

            response = null;
            return false;
        }

        private bool ItemsEmpty(OrderCancelRequest request, out OrderCancelResponse sendCanceledOrders)
        {
            if (request?.Cancellation == null || request.Cancellation?.Items?.Count == 0)
            {
                var message = $"Failed while working with CancelOrders Action, with message: Items are Null or empty";
                _logger.LogError(message);

                sendCanceledOrders = new OrderCancelResponse {Error = message, HasError = true};
                return true;
            }

            sendCanceledOrders = null;
            return false;
        }
    }
}