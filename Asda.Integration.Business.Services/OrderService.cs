using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Asda.Integration.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IFtpService _ftpService;

        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<OrderService> _logger;

        private const int MaxOrdersPerPage = 50;

        public OrderService(IFtpService ftpService, IUserConfigAdapter userConfigAdapter, ILogger<OrderService> logger)
        {
            _ftpService = ftpService;
            _userConfigAdapter = userConfigAdapter;
            _logger = logger;
        }

        public OrdersResponse GetOrdersAndSendManifest(OrdersRequest request)
        {
            if (request.PageNumber <= 0)
            {
                var message = "Invalid page number";
                _logger.LogError(
                    $"UserToken: {request.AuthorizationToken}; Failed while working with GetOrdersAndSendManifest with message: {message}");
                return new OrdersResponse {Error = message};
            }

            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var errorMessage = $"User with AuthToken: {request.AuthorizationToken} - not found.";
                    _logger.LogError(
                        $"UserToken: {request.AuthorizationToken}; Failed while working with GetOrdersAndSendManifest with message: {errorMessage}");

                    return new OrdersResponse {Error = errorMessage};
                }

                using (var sftpClient = new SftpClient(user.FtpSettings.Host, user.FtpSettings.Port,
                           user.FtpSettings.UserName, user.FtpSettings.Password))
                {
                    sftpClient.Connect();
                    if (!sftpClient.IsConnected)
                    {
                        var message = "Client was not connected";
                        _logger.LogError($"Failed while working with GetPurchaseOrderFromFtp with message: {message}");
                        throw new Exception(message);
                    }

                    if (!sftpClient.Exists(user.RemoteFileStorage.OrdersPath))
                    {
                        var message = $"No such folder: {user.RemoteFileStorage.OrdersPath}";
                        _logger.LogError($"Failed while working with GetPurchaseOrderFromFtp with message: {message}");
                        throw new Exception($"{message}");
                    }

                    var filesInfo =
                        _ftpService.GetAllSortedFilesInfo(sftpClient, user.RemoteFileStorage.OrdersPath);
                    var filesPathsPerPage = GetFilesPathsPerPage(filesInfo, request.PageNumber);
                    var purchaseOrders =
                        _ftpService.GetFiles<PurchaseOrder>(sftpClient, filesPathsPerPage, request.AuthorizationToken);
                    var purchaseOrdersNew = purchaseOrders.Where(p =>
                        p.Request.OrderRequest.OrderRequestHeader.OrderDate.ToUniversalTime() >= request.UTCTimeFrom);
                    if (!purchaseOrdersNew.Any())
                    {
                        return new OrdersResponse() {Orders = Array.Empty<Order>(), HasMorePages = false};
                    }

                    var orders = purchaseOrdersNew.Select(OrderMapper.MapToOrder);
                    var acknowledgments =
                        orders.Select(o => AcknowledgmentMapper.MapToAcknowledgment(o.ReferenceNumber));
                    _ftpService.CreateFiles(acknowledgments.ToList(), user.FtpSettings,
                        user.RemoteFileStorage.AcknowledgmentsPath,
                        user.AuthorizationToken);

                    return new OrdersResponse
                    {
                        Orders = orders.ToArray(),
                        HasMorePages = request.PageNumber * MaxOrdersPerPage < filesInfo.Count
                    };
                }
            }
            catch (Exception e)
            {
                var message =
                    $"UserToken: {request.AuthorizationToken}; Failed while working with GetOrdersAndSendManifest with message: {e.Message}";
                _logger.LogError(message);
                return new OrdersResponse {Error = e.Message};
            }
        }

        public OrderDespatchResponse SendDispatch(OrderDespatchRequest request)
        {
            if (request?.Orders == null || request.Orders?.Count == 0)
            {
                var message = "Orders are Null or empty";
                _logger.LogError(
                    $"UserToken: {request.AuthorizationToken}; Failed while working with Dispatch Action, with message: {message}");
                return new OrderDespatchResponse {Error = message};
            }

            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var errorMessage = $"User with AuthToken: {request.AuthorizationToken} - not found.";
                    _logger.LogError(
                        $"UserToken: {request.AuthorizationToken}; Failed while working with SendDispatch with message: {errorMessage}");

                    return new OrderDespatchResponse {Error = errorMessage};
                }

                var shipmentConfirmations = request.Orders.Select(ShipmentMapper.MapToShipmentConfirmation).ToList();
                var xmlErrors =
                    _ftpService.CreateFiles(shipmentConfirmations, user.FtpSettings,
                        user.RemoteFileStorage.DispatchesPath,
                        user.AuthorizationToken);
                var response = new OrderDespatchResponse
                {
                    Orders = request.Orders.Select(o => new OrderDespatchError {ReferenceNumber = o.ReferenceNumber})
                        .ToList()
                };

                return !xmlErrors.Any() ? response : ErrorDispatchResponse(xmlErrors, request);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with SendDispatch with message {e.Message}";
                _logger.LogError($"UserToken: {request.AuthorizationToken}; {message}");

                return new OrderDespatchResponse {Error = e.Message};
            }
        }

        public OrderCancelResponse SendCanceledOrders(OrderCancelRequest request)
        {
            if (request?.Cancellation == null || request.Cancellation?.Items?.Count == 0)
            {
                var message = $"Items are Null or empty";
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken}; Failed while working with CancelOrders Action, with message: {message}");

                return new OrderCancelResponse {Error = message, HasError = true};
            }

            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var errorMessage = $"User with AuthToken: {request.AuthorizationToken} - not found.";
                    _logger.LogError(
                        $"UserToken: {request.AuthorizationToken}; Failed while working with SendCanceledOrders with message: {errorMessage}");

                    return new OrderCancelResponse {Error = errorMessage};
                }

                var cancellation = CancellationMapper.MapToCancellation(request.Cancellation);
                var xmlErrors =
                    _ftpService.CreateFiles(new List<Cancellation> {cancellation}, user.FtpSettings,
                        user.RemoteFileStorage.CancellationsPath, user.AuthorizationToken);

                return !xmlErrors.Any() ? new OrderCancelResponse {HasError = false} : ErrorCancelResponse(xmlErrors);
            }
            catch (Exception e)
            {
                var message =
                    $"UserToken: {request.AuthorizationToken}; Failed while working with SendCanceledOrders with message {e.Message}";
                _logger.LogError(message);
                return new OrderCancelResponse {Error = e.Message, HasError = true};
            }
        }

        private List<string> GetFilesPathsPerPage(List<SftpFile> files, int pageNumber)
        {
            var skip = (pageNumber - 1) * MaxOrdersPerPage;
            var take = skip + MaxOrdersPerPage > files.Count ? files.Count - skip : MaxOrdersPerPage;
            var filesPathsPerPage = files
                .Skip(skip)
                .Take(take);
            return filesPathsPerPage.Select(f => f.FullName).ToList();
        }

        private OrderCancelResponse ErrorCancelResponse(List<XmlError> xmlErrors)
        {
            var response = new OrderCancelResponse {HasError = true};
            var messages = new StringBuilder();
            foreach (var xmlError in xmlErrors)
            {
                messages.AppendLine(xmlError.Message);
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
    }
}