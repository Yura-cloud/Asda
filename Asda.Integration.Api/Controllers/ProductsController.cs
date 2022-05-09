using System;
using System.Collections.Generic;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Products;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<ProductsController> _logger;

        private readonly IOrderService _orderService;

        public ProductsController(IUserConfigAdapter userConfigAdapter, ILogger<ProductsController> logger,
            IOrderService orderService)
        {
            _userConfigAdapter = userConfigAdapter;
            _logger = logger;
            _orderService = orderService;
        }


        [HttpPost]
        public ProductsResponse Products([FromBody] ProductsRequest request)
        {
            if (request.PageNumber <= 0)
                return new ProductsResponse {Error = "Invalid page number"};

            try
            {
                var user = this._userConfigAdapter.Load(request.AuthorizationToken);

                Random rand = new(DateTime.UtcNow.Millisecond);

                var products = new List<Product>();

                int productCount = 1000;
                if (request.PageNumber == 11)
                {
                    productCount = 22;
                }
                else if (request.PageNumber > 11)
                {
                    productCount = 0;
                }

                for (int i = 1; i <= productCount; i++)
                {
                    products.Add(new Product
                    {
                        SKU = string.Concat("ChannelProduct_", i * request.PageNumber),
                        Title = string.Concat("Channel Tile of product ChannelProduct_", i * request.PageNumber),
                        Price = (decimal) rand.NextDouble(),
                        Quantity = rand.Next(0, 100)
                    });
                }

                return new ProductsResponse
                {
                    Products = products.ToArray(),
                    HasMorePages = request.PageNumber < 11
                };
            }
            catch (Exception ex)
            {
                return new ProductsResponse {Error = ex.Message};
            }
        }
        
        [HttpPost]
        public ProductInventoryUpdateResponse InventoryUpdate([FromBody] ProductInventoryUpdateRequest request)
        {
            if (request.Products == null || request.Products.Length == 0)
            {
                _logger.LogError($"Error while updating inventory. There aren't any products");
                return new ProductInventoryUpdateResponse {Error = "Products not supplied"};
            }

            try
            {
                var user = _userConfigAdapter.Load(request.AuthorizationToken);

                if (user == null)
                {
                    _logger.LogError($"User with ID: {request.AuthorizationToken} - not found.");
                    return new ProductInventoryUpdateResponse {Error = "User not found"};
                }

                var inventoryItems = GetInventoryItems(request.Products);
                _orderService.SendSnapInventoriesFiles(inventoryItems);
                
                var response = FillInResponse(request);

                return response;
            }
            catch (Exception ex)
            {
                return new ProductInventoryUpdateResponse {Error = ex.Message};
            }
        }
        
        [HttpPost]
        public ProductPriceUpdateResponse PriceUpdate([FromBody] ProductPriceUpdateRequest request)
        {
            if (request.Products == null || request.Products.Length == 0)
                return new ProductPriceUpdateResponse {Error = "Products not supplied"};

            try
            {
                var user = this._userConfigAdapter.Load(request.AuthorizationToken);

                var response = new ProductPriceUpdateResponse();

                foreach (var product in request.Products)
                {
                    if (product.SKU == "MyNonExistantSKU")
                    {
                        response.Products.Add(
                            new ProductPriceResponse {SKU = product.SKU, Error = "SKU does not exist"});
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ProductPriceUpdateResponse {Error = ex.Message};
            }
        }

        private ProductInventoryUpdateResponse FillInResponse(ProductInventoryUpdateRequest request)
        {
            var response = new ProductInventoryUpdateResponse
            {
                Products = new List<ProductInventoryResponse>()
            };
            foreach (var productInventory in request.Products)
            {
                response.Products.Add(new ProductInventoryResponse
                {
                    SKU = productInventory.SKU
                });
            }

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
    }
}