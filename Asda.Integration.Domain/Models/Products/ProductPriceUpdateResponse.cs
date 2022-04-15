using System.Collections.Generic;

namespace Asda.Integration.Domain.Models.Products
{
    public class ProductPriceUpdateResponse : BaseResponse
    {
        public ProductPriceUpdateResponse()
        {
            this.Products = new List<ProductPriceResponse>();
        }

        /// <summary>
        /// List of responses <see cref="ProductPriceResponse"/>
        /// </summary>
        public List<ProductPriceResponse> Products { get; set; }
    }
}