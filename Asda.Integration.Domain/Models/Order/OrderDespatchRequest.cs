using System.Collections.Generic;
using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Api.Mappers
{
    public class OrderDespatchRequest : BaseRequest
    {
        public OrderDespatchRequest()
        {
            this.Orders = new List<OrderDespatch>();
        }

        /// <summary>
        /// List of despatch orders <see cref="OrderDespatch"/>.
        /// </summary>
        public List<OrderDespatch> Orders { get; set; }
    }
}