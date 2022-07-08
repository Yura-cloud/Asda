using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;


namespace Asda.Integration.Api.Mappers
{
    public static class AcknowledgmentMapper
    {
        public static Acknowledgment MapToAcknowledgment(string referenceNumber)
        {
            var acknowledgment = new Acknowledgment
            {
                Request = new Request
                {
                    ConfirmationRequest = new ConfirmationRequest
                    {
                        ConfirmationHeader = new ConfirmationHeader {Type = "accept"},
                        OrderReference = new OrderReference {OrderID = referenceNumber}
                    }
                }
            };
            return acknowledgment;
        }
    }
}