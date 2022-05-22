using System;
using Asda.Integration.Domain.Models.Products;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IProductService _productService;

        public ProductController(IUserConfigAdapter userConfigAdapter, IProductService productService)
        {
            _userConfigAdapter = userConfigAdapter;
            _productService = productService;
        }


        [HttpPost]
        public ProductsResponse Products([FromBody] ProductsRequest request)
        {
            try
            {
                return new ProductsResponse();
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
            return new ProductPriceUpdateResponse {Error = "At this moment PriceUpdate is not implemented"};
        }
    }
}