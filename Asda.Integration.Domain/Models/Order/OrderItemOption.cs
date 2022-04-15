namespace Asda.Integration.Domain.Models.Order
{
    public class OrderItemOption
    {
        /// <summary>
        /// Unique per order option Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Option Value.
        /// </summary>
        public string Value { get; set; }
    }
}