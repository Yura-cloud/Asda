using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Products;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;

        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IXmlService _xmlService;

        public ProductService(ILogger<ProductService> logger, IUserConfigAdapter userConfigAdapter,
            IXmlService xmlService)
        {
            _logger = logger;
            _userConfigAdapter = userConfigAdapter;
            _xmlService = xmlService;
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

                var inventoryItems = GetInventoryItems(request.Products);
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

        private List<InventorySnapshot> GetInventoryItems(ProductInventory[] requestProducts)
        {
            var inventoryItems = new List<InventorySnapshot>();
            foreach (var productInventory in requestProducts)
            {
                var inventoryItem = SnapInventoryMapping.MapToInventorySnapshot(productInventory);
                inventoryItems.Add(inventoryItem);
            }

            return inventoryItems;
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