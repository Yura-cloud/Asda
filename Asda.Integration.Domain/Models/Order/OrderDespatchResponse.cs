using System.Collections.Generic;

namespace Asda.Integration.Domain.Models.Order
{
    public class OrderDespatchResponse : BaseResponse
    {
        /// <summary>
        /// Orders <see cref="OrderDespatchError"/>
        /// </summary>
        public List<OrderDespatchError> Orders { get; set; }
    }
}