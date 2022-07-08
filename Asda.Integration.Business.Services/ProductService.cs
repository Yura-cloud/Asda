using System;
using System.Collections.Generic;
using System.Linq;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Products;
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
            if (request.Products == null || request.Products.Length == 0)
            {
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken}; Error while updating inventory. There aren't any products");
                {
                    return new ProductInventoryUpdateResponse {Error = "Products not supplied"};
                }
            }

            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    _logger.LogError($"User with ID: {request.AuthorizationToken} - not found.");
                    {
                        return new ProductInventoryUpdateResponse {Error = "User not found"};
                    }
                }

                var xmlErrors = GetXmlErrorsIfItemIdIsNotGuid(request);
                if (xmlErrors.Count == request.Products.Length)
                {
                    var message = "All items do not have their id";
                    _logger.LogError($"userToken: {request.AuthorizationToken}; Error: {message}");
                    {
                        return new ProductInventoryUpdateResponse {Error = message};
                    }
                }

                if (xmlErrors.Count != 0)
                {
                    request.Products = request.Products
                        .Where(p => xmlErrors.All(e => e.SKU != p.SKU))
                        .ToArray();
                }

                LinnWorks.Api = InitializeHelper.GetApiManagerForPullOrders(_configuration, user.AppToken);

                // we need StockItemsLevel because client needs Available and InOrders values
                var stockItemsLevel = GetStockItemsLevel(request, _userConfigAdapter);
                if (stockItemsLevel.Count != request.Products.Length)
                {
                    AddToErrorsItemsWithWrongItemId(request, stockItemsLevel, xmlErrors);
                }

                var inventoryItems = stockItemsLevel.Select(SnapInventoryMapping.MapToInventorySnapshot).ToList();
                xmlErrors.AddRange(_ftp.CreateFiles(inventoryItems, user.FtpSettings,
                    user.RemoteFileStorage.SnapInventoriesPath, user.AuthorizationToken));
                var response = new ProductInventoryUpdateResponse
                {
                    Products = request.Products.Select(p =>
                            new ProductInventoryResponse {SKU = p.SKU})
                        .ToList()
                };
                return !xmlErrors.Any() ? response : ErrorResponse(xmlErrors, inventoryItems);
            }
            catch (Exception e)
            {
                var message =
                    $"UserToken: {request.AuthorizationToken}; Failed while ProductInventoryUpdateResponse, with message {e.Message}";
                _logger.LogError(message);
                return new ProductInventoryUpdateResponse {Error = e.Message};
            }
        }

        private void AddToErrorsItemsWithWrongItemId(ProductInventoryUpdateRequest request,
            List<StockItemLevel> stockItemsLevel, List<XmlError> xmlErrors)
        {
            var productsWithFailedItemId = request.Products
                .Where(p => stockItemsLevel.All(s => s.SKU != p.SKU));
            foreach (var productInventory in productsWithFailedItemId)
            {
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken}; WrongIdNumber, SKU: {productInventory.SKU}");
                xmlErrors.Add(new XmlError
                {
                    SKU = productInventory.SKU,
                    Message = "WrongIdNumber"
                });
            }
        }

        private List<XmlError> GetXmlErrorsIfItemIdIsNotGuid(ProductInventoryUpdateRequest request)
        {
            var itemsWithNonCorrectId = request.Products
                .Where(p => !Guid.TryParse(p.Reference, out var guidOut));
            foreach (var productInventory in itemsWithNonCorrectId)
            {
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken}; Item id is empty or not Guid, SKU: {productInventory.SKU}");
            }

            var xmlErrors = itemsWithNonCorrectId
                .Select(p => new XmlError
                {
                    Message = "Item id is empty or not Guid",
                    SKU = p.SKU
                }).ToList();

            return xmlErrors;
        }

        private List<StockItemLevel> GetStockItemsLevel(ProductInventoryUpdateRequest request,
            IUserConfigAdapter userConfigAdapter)
        {
            var userLocations = GetLocations(request.AuthorizationToken);
            var user = userConfigAdapter.LoadByToken(request.AuthorizationToken);
            if (!userLocations.Select(l => l.LocationName).Contains(user.Location))
            {
                var message = $"There is no such location name like - {user.Location}";
                _logger.LogError($"userToken: {request.AuthorizationToken}; {message}");
                throw new Exception(message);
            }

            var itemsFullInfo = GetItemsFullInfo(request);
            var stockItemLevels = itemsFullInfo.StockItemsFullExtended
                .Select(i => i.StockLevels)
                .SelectMany(x => x).ToList()
                .Where(x => x.Location.LocationName == user.Location)
                .ToList();
            //change sku to channel sku for clients ItemUpdate file
            foreach (var stockItemLevel in stockItemLevels)
            {
                stockItemLevel.SKU = request.Products
                    .FirstOrDefault(p => p.Reference == stockItemLevel.StockItemId.ToString())?.SKU;
            }

            return stockItemLevels;
        }

        private List<InventoryStockLocation> GetLocations(string userToken)
        {
            try
            {
                return LinnWorks.Api.Inventory.GetStockLocations();
            }
            catch (Exception e)
            {
                var message = $"Failed while GetStockLocations, with message {e.Message}";
                _logger.LogError($"UserToken: {userToken}; {message}");
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
                _logger.LogError($"userToken: {request.AuthorizationToken}; {message}");
                throw new Exception(message);
            }
        }

        private static ProductInventoryUpdateResponse ErrorResponse(IEnumerable<XmlError> xmlErrors,
            List<InventorySnapshot> products)
        {
            var response = new ProductInventoryUpdateResponse
            {
                Products = xmlErrors.Select(e => new ProductInventoryResponse
                {
                    //we get a SKU in xmlError if ItemId is not correct,or we have index of a product
                    //from CreateFiles() if something went wrong 
                    SKU = e.SKU ?? products[e.Index].Request.InventorySnapshotRequest.Records.Record.ProductId,
                    Error = e.Message
                }).ToList()
            };
            return response;
        }
    }
}