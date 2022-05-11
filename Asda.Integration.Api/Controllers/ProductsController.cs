using System;
using System.Collections.Generic;
using Asda.Integration.Domain.Models.Products;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IProductService _productService;

        public ProductsController(IUserConfigAdapter userConfigAdapter, IProductService productService)
        {
            _userConfigAdapter = userConfigAdapter;
            _productService = productService;
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
            return _productService.SendInventoryUpdate(request);
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
    }
}