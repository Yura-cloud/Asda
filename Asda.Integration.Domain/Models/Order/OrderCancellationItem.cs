namespace Asda.Integration.Domain.Models.Order
{
    public class OrderCancellationItem
    {
        public string SKU { get; set; }
        public string OrderLineNumber { get; set; }
        public int CancellationQuantity { get; set; }
        public string Reason { get; set; }
        public string SecondaryReason { get; set; }
    }
}