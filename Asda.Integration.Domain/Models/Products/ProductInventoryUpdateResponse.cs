using System.Collections.Generic;

namespace Asda.Integration.Domain.Models.Products
{
    public class ProductInventoryUpdateResponse : BaseResponse
    {
        public ProductInventoryUpdateResponse()
        {
            Products = new List<ProductInventoryResponse>();
        }

        /// <summary>
        /// List of products <see cref="ProductInventoryResponse"/>
        /// </summary>
        public List<ProductInventoryResponse> Products { get; set; }
    }
}