using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML;
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

        private readonly IXmlService _xmlService;

        private LinnworksMacroBase LinnWorks { get; }

        public ProductService(ILogger<ProductService> logger, IUserConfigAdapter userConfigAdapter,
            IXmlService xmlService, IConfiguration configuration)
        {
            _logger = logger;
            _userConfigAdapter = userConfigAdapter;
            _xmlService = xmlService;
            _configuration = configuration;
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
                if (UserUnauthorized(request, out var userUnauthorizedResponse))
                {
                    return userUnauthorizedResponse;
                }

                var stockItemsLevel = GetStockItemsLevel(request, _configuration, _userConfigAdapter);

                var inventoryItems = request.Products.Select(SnapInventoryMapping.MapToInventorySnapshot).ToList();
                //var inventoryItems = SnapInventoryMapping.MapToInventorySnapshot(stockItemsLevel);
                var xmlErrors = _xmlService.CreateXmlFilesOnFtp(inventoryItems);

                var response = new ProductInventoryUpdateResponse
                {
                    Products = request.Products.Select(p => new ProductInventoryResponse {SKU = p.SKU}).ToList()
                };
                return !xmlErrors.Any() ? response : ErrorResponse(xmlErrors, response);
            }
            catch (Exception ex)
            {
                return new ProductInventoryUpdateResponse {Error = ex.Message};
            }
        }

        private List<StockItemLevel> GetStockItemsLevel(ProductInventoryUpdateRequest request,
            IConfiguration configuration,
            IUserConfigAdapter userConfigAdapter)
        {
            LinnWorks.Api = InitializeHelper.GetApiManagerForPullOrders(configuration, request.AuthorizationToken);
            var itemsRequest = new GetStockItemsFullByIdsRequest
            {
                StockItemIds = request.Products.Select(p => new Guid(p.Reference)).ToList(),
                DataRequirements = new List<StockItemFullExtendedDataRequirement>
                {
                    StockItemFullExtendedDataRequirement.StockLevels
                }
            };
            var itemFull = LinnWorks.Api.Stock.GetStockItemsFullByIds(itemsRequest);
            var user = userConfigAdapter.LoadByToken(request.AuthorizationToken);
            var stockItemLevels = itemFull.StockItemsFullExtended
                .Select(i => i.StockLevels)
                .SelectMany(x => x).ToList()
                .Where(x => x.Location.LocationName == user.Location)
                .ToList();
            return stockItemLevels;
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
            out ProductInventoryUpdateResponse response)
        {
            var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

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