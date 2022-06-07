using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;

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
                _logger.LogError($"UserToken: {request.AuthorizationToken}, Invalid page number");
                return new OrdersResponse {Error = "Invalid page number"};
            }

            try
            {
                if (UserUnauthorized(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrdersResponse {Error = errorMessage};
                }

                var purchaseOrders = _ftp.GetPurchaseOrderFromFtp(user.FtpSettings, user.RemoteFileStorage.OrdersPath,
                    user.AuthorizationToken, out var xmlErrors);
                if (purchaseOrders == null)
                {
                    return new OrdersResponse();
                }

                var orders = purchaseOrders.Select(OrderMapper.MapToOrder);
                var acknowledgments = orders.Select(o => AcknowledgmentMapper.MapToAcknowledgment(o.ReferenceNumber));
                _ftp.CreateFiles(acknowledgments.ToList(), user.FtpSettings, user.RemoteFileStorage.AcknowledgmentsPath,
                    user.AuthorizationToken, xmlErrors);

                return new OrdersResponse {Orders = orders.ToArray(), HasMorePages = false,};
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
                if (UserUnauthorized(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrderDespatchResponse {Error = errorMessage};
                }

                var shipmentConfirmations = request.Orders.Select(ShipmentMapper.MapToShipmentConfirmation).ToList();
                var xmlErrors = new List<XmlError>();
                _ftp.CreateFiles(shipmentConfirmations, user.FtpSettings, user.RemoteFileStorage.DispatchesPath,
                    user.AuthorizationToken, xmlErrors);
                var response = new OrderDespatchResponse
                {
                    Orders = request.Orders.Select(o => new OrderDespatchError {ReferenceNumber = o.ReferenceNumber})
                        .ToList()
                };

                return !xmlErrors.Any() ? response : ErrorDispatchResponse(xmlErrors, request);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with Dispatch Action, with message {e.Message}";
                _logger.LogError($"UserToken: {request.AuthorizationToken}; {message}");

                return new OrderDespatchResponse {Error = message};
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
                if (UserUnauthorized(request.AuthorizationToken, out var errorMessage, out var user))
                {
                    return new OrderCancelResponse() {Error = errorMessage, HasError = true};
                }

                var cancellation = CancellationMapper.MapToCancellation(request.Cancellation);
                var xmlErrors = new List<XmlError>();
                _ftp.CreateFiles(new List<Cancellation> {cancellation}, user.FtpSettings,
                    user.RemoteFileStorage.CancellationsPath, user.AuthorizationToken, xmlErrors);

                return !xmlErrors.Any() ? new OrderCancelResponse {HasError = false} : ErrorCancelResponse(xmlErrors);
            }
            catch (Exception e)     
            {
                var message = $"Failed while working with CancelOrders Action, with message {e.Message}";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = message, HasError = true};
            }
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

        private OrderDespatchResponse ErrorDispatchResponse(List<XmlError> xmlErrors, OrderDespatchRequest request)
        {
            var response = new OrderDespatchResponse
            {
                Orders = xmlErrors.Select(e => new OrderDespatchError
                {
                    ReferenceNumber = request.Orders[e.Index].ReferenceNumber,
                    Error = e.Message
                }).ToList()
            };
            return response;
        }

        private bool UserUnauthorized(string token, out string errorMessage, out UserConfig user)
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
                _logger.LogError($"UserToken: {request.AuthorizationToken}; {message}");

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
                _logger.LogError($"userToken: {request.AuthorizationToken}; {message}");

                sendCanceledOrders = new OrderCancelResponse {Error = message, HasError = true};
                return true;
            }

            sendCanceledOrders = null;
            return false;
        }
    }
}