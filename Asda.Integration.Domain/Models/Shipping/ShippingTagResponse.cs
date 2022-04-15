namespace Asda.Integration.Domain.Models.Shipping
{
    public class ShippingTagResponse : BaseResponse
    {
        /// <summary>
        /// List of shipping tags.
        /// </summary>
        public ShippingTag[] ShippingTags { get; set; }
    }
}