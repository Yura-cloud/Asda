﻿namespace Asda.Integration.Domain.Models.Products
{
    public class ProductsRequest : BaseRequest
    {
        public ProductsRequest()
        {
            PageNumber = 1;
        }

        /// <summary>
        /// Current page number in the fetch sequence, starts from 1.
        /// </summary>
        public int PageNumber { get; set; }
    }
}