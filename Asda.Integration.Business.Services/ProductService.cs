using System;
using System.Collections.Generic;
using System.Linq;
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

                if (AllItemsWithoutId(request, out var xmlErrors, out var wrongIdResponse))
                {
                    return wrongIdResponse;
                }

                if (xmlErrors.Count != 0)
                {
                    request.Products = request.Products
                        .Where(p => !xmlErrors.Select(e => e.SKU).Contains(p.SKU))
                        .ToArray();
                }

                LinnWorks.Api = InitializeHelper.GetApiManagerForPullOrders(_configuration, request.AuthorizationToken);
                var stockItemsLevel = GetStockItemsLevel(request, _userConfigAdapter);
                if (stockItemsLevel.Count != request.Products.Length)
                {
                    AddToErrorsItemsWithWrongItemId(request, stockItemsLevel, xmlErrors);
                }

                //further we are working with List<StockItemLevel> not List<request.Products>
                var inventoryItems = stockItemsLevel.Select(SnapInventoryMapping.MapToInventorySnapshot).ToList();

                _ftp.CreateFiles(inventoryItems, user.FtpSettings, user.RemoteFileStorage.SnapInventoriesPath,
                    user.AuthorizationToken, xmlErrors);
                var response = new ProductInventoryUpdateResponse
                {
                    Products = request.Products.Select(p => new ProductInventoryResponse
                    {
                        SKU = p.SKU
                    }).ToList()
                };
                return !xmlErrors.Any() ? response : ErrorResponse(xmlErrors, request.Products);
            }
            catch (Exception e)
            {
                var message = $"Failed while ProductInventoryUpdateResponse, with message {e.Message}";
                _logger.LogError(message);
                return new ProductInventoryUpdateResponse {Error = message};
            }
        }

        private void AddToErrorsItemsWithWrongItemId(ProductInventoryUpdateRequest request,
            List<StockItemLevel> stockItemsLevel, List<XmlError> xmlErrors)
        {
            var productsWithFailedItemId = request.Products
                .Where(p => !stockItemsLevel.Select(o => o.SKU).Contains(p.SKU));
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

        private bool AllItemsWithoutId(ProductInventoryUpdateRequest request, out List<XmlError> xmlErrors,
            out ProductInventoryUpdateResponse wrongIdResponse)
        {
            xmlErrors = GetXmlErrorsIfItemIdIsNotGuid(request);
            if (xmlErrors.Count == request.Products.Length)
            {
                var message = $"All items do not have their id";
                _logger.LogError($"userToken: {request.AuthorizationToken}; {message}");
                {
                    wrongIdResponse = new ProductInventoryUpdateResponse()
                    {
                        Error = message
                    };
                    return true;
                }
            }

            wrongIdResponse = null;
            return false;
        }

        private List<XmlError> GetXmlErrorsIfItemIdIsNotGuid(ProductInventoryUpdateRequest request)
        {
            var itemsWithNonCorrectId = request.Products
                .Where(p => !Guid.TryParse(p.Reference, out var guidOut));
            foreach (var productInventory in itemsWithNonCorrectId)
            {
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken};Item id is not correct or empty, SKU: {productInventory.SKU}");
            }

            var xmlErrors = itemsWithNonCorrectId
                .Select(p => new XmlError
                {
                    Message = "Item id is not correct or empty",
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
            //change sku to channel sku
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
            ProductInventory[] products)
        {
            var response = new ProductInventoryUpdateResponse
            {
                Products = xmlErrors.Select(e => new ProductInventoryResponse
                {
                    //we get SKU in xmlError if ItemId is not correct,otherwise we have index of product
                    //from CreateFiles() if something went wrong 
                    SKU = e.SKU ?? products[e.Index].SKU,
                    Error = e.Message
                }).ToList()
            };
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
                _logger.LogError(
                    $"userToken: {request.AuthorizationToken}; Error while updating inventory. There aren't any products");
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