namespace Asda.Integration.Domain.Models
{
    public class BaseRequest
    {
        /// <summary>
        /// Authorization Token from the customers integration.
        /// </summary>
        public string AuthorizationToken { get; set; }
    }
}