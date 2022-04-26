using System;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Request = Asda.Integration.Domain.Models.Business.Acknowledgment.Request;


namespace Asda.Integration.Api.Mappers
{
    public class AcknowledgmentMapper
    {
        public static Acknowledgment MapToAcknowledgment(string referenceNumber)
        {
            var acknowledgment = new Acknowledgment
            {
                Request = new Request
                {
                    ConfirmationRequest = new ConfirmationRequest
                    {
                        ConfirmationHeader = new ConfirmationHeader
                        {
                            Type = "accept"
                        },
                        OrderReference = new OrderReference
                        {
                            OrderID = Convert.ToInt32(referenceNumber)
                        }
                    }
                },
            };
            return acknowledgment;
        }
    }
}