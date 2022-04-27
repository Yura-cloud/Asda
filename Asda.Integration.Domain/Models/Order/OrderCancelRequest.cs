namespace Asda.Integration.Domain.Models.Order
{
    public class OrderCancelRequest : BaseRequest
    {
        public OrderCancellation Cancellation { get; set; }
    }
}
