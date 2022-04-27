using System.Collections.Generic;

namespace Asda.Integration.Domain.Models.Order
{
    public class OrderCancellation
    {
        public List<OrderCancellationItem> Items { get; set; }
        public string ReferenceNumber { get; set; }
        public string ExternalReference { get; set; }
    }
}