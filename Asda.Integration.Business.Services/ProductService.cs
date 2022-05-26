using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Products;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using LinnworksAPI;
using LinnworksMacroHelpers;
using LinnworksMacroHelpers.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;

        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IConfiguration _configuration;

        private readonly IFtpService _ftp;

        private LinnworksMacroBase LinnWorks { get; }

        public ProductService(ILogger<ProductService> logger, IUserConfigAdapter userConfigAdapter,
            IConfiguration configuration, IFtpService ftp)
        {
            _logger = logger;
            _userConfigAdapter = userConfigAdapter;
            _configuration = configuration;
            _ftp = ftp;
            LinnWorks = new LinnworksMacroBase();
        }

        public ProductInventoryUpdateResponse SendInventoryUpdate(ProductInventoryUpdateRequest request)
        {
            if (ProductsEmpty(request, out var productEmptyResponse))
            {
                return productEmptyResponse;
            }

            try
            {
                if (UserUnauthorized(request, out var userUnauthorizedResponse, out var user))
                {
                    return userUnauthorizedResponse;
                }

                LinnWorks.Api = InitializeHelper.GetApiManagerForPullOrders(_configuration, request.AuthorizationToken);
                var stockItemsLevel = GetStockItemsLevel(request, _userConfigAdapter);
                var inventoryItems = stockItemsLevel.Select(SnapInventoryMapping.MapToInventorySnapshot).ToList();
                var xmlErrors = _ftp.CreateFiles(inventoryItems, user.FtpSettings,
                    user.RemoteFileStorage.SnapInventoryPath);

                var response = new ProductInventoryUpdateResponse
                {
                    Products = request.Products.Select(p => new ProductInventoryResponse {SKU = p.SKU}).ToList()
                };
                return !xmlErrors.Any() ? response : ErrorResponse(xmlErrors, response);
            }
            catch (Exception e)
            {
                string message = $"Failed while ProductInventoryUpdateResponse, with message {e.Message}";
                _logger.LogError(message);
                return new ProductInventoryUpdateResponse {Error = message};
            }
        }

        private List<StockItemLevel> GetStockItemsLevel(ProductInventoryUpdateRequest request,
            IUserConfigAdapter userConfigAdapter)
        {
            var userLocations = GetLocations();
            var user = userConfigAdapter.LoadByToken(request.AuthorizationToken);
            if (!userLocations.Select(l => l.LocationName).Contains(user.Location))
            {
                var message = $"There is no such location name like - {user.Location}";
                _logger.LogError(message);
                throw new Exception(message);
            }

            var itemsFullInfo = GetItemsFullInfo(request);
            var stockItemLevels = itemsFullInfo.StockItemsFullExtended
                .Select(i => i.StockLevels)
                .SelectMany(x => x).ToList()
                .Where(x => x.Location.LocationName == user.Location)
                .ToList();
            //change sku to channel sku
            foreach (var stockItemLevel in stockItemLevels)
            {
                stockItemLevel.SKU = request.Products
                    .FirstOrDefault(p => p.Reference == stockItemLevel.StockItemId.ToString())
                    ?.SKU;
            }

            return stockItemLevels;
        }

        private List<InventoryStockLocation> GetLocations()
        {
            try
            {
                return LinnWorks.Api.Inventory.GetStockLocations();
            }
            catch (Exception e)
            {
                var message = $"Failed while GetStockLocations, with message {e.Message}";
                _logger.LogError(message);
                throw new Exception(message);
            }
        }

        private GetStockItemsFullByIdsResponse GetItemsFullInfo(ProductInventoryUpdateRequest request)
        {
            try
            {
                var itemsRequest = new GetStockItemsFullByIdsRequest
                {
                    StockItemIds = request.Products.Select(p => new Guid(p.Reference)).ToList(),
                    DataRequirements = new List<StockItemFullExtendedDataRequirement>
                    {
                        StockItemFullExtendedDataRequirement.StockLevels
                    }
                };
                var itemsFullInfo = LinnWorks.Api.Stock.GetStockItemsFullByIds(itemsRequest);
                return itemsFullInfo;
            }
            catch (Exception e)
            {
                var message = $"Failed while GetStockItemsFullByIds, with message {e.Message}";
                _logger.LogError(message);
                throw new Exception(message);
            }
        }


        private static ProductInventoryUpdateResponse ErrorResponse(List<XmlError> xmlErrors,
            ProductInventoryUpdateResponse response)
        {
            var messages = new StringBuilder();
            foreach (var xmlError in xmlErrors)
            {
                messages.Append(xmlError.Message).AppendLine();
            }

            response.Error = messages.ToString();
            return response;
        }

        private bool UserUnauthorized(ProductInventoryUpdateRequest request,
            out ProductInventoryUpdateResponse response, out UserConfig user)
        {
            user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

            if (user == null)
            {
                _logger.LogError($"User with ID: {request.AuthorizationToken} - not found.");
                {
                    response = new ProductInventoryUpdateResponse {Error = "User not found"};
                    return true;
                }
            }

            response = null;
            return false;
        }

        private bool ProductsEmpty(ProductInventoryUpdateRequest request,
            out ProductInventoryUpdateResponse response)
        {
            if (request.Products == null || request.Products.Length == 0)
            {
                _logger.LogError($"Error while updating inventory. There aren't any products");
                {
                    response = new ProductInventoryUpdateResponse {Error = "Products not supplied"};
                    return true;
                }
            }

            response = null;
            return false;
        }
    }
}