using System;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Request = Asda.Integration.Domain.Models.Business.Acknowledgment.Request;


namespace Asda.Integration.Api.Mappers
{
    public static class AcknowledgmentMapper
    {
        public static Acknowledgment MapToAcknowledgment(string referenceNumber)
        {
            var acknowledgment = new Acknowledgment
            {
                PayloadID = $"{Guid.NewGuid()}@linnworks.domain.com",
                Lang = "en",
                Text = "",
                Timestamp = DateTime.Now,
                Header = new Header
                {
                    From = new From
                    {
                        Credential = new Credential
                        {
                            Domain = "AsdaOrganisation",
                            Identity = "ASDA-123456-DC"
                        }
                    },
                    To = new To
                    {
                        Credential = new Credential
                        {
                            Domain = "AsdaOrganisation",
                            Identity = "ASDA"
                        }
                    },
                    Sender = new Sender
                    {
                        Credential = new Credential
                        {
                            Domain = "Linnworks",
                            Identity = referenceNumber
                        }
                    }
                },
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