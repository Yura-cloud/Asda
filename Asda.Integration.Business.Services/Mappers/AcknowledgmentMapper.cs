using System;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;


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
                Timestamp = DateTime.UtcNow,
                Header = new Header
                {
                    From = new From
                    {
                        Credential = new Credential {Domain = "AsdaOrganisation", Identity = "ASDA-123456-DC"}
                    },
                    To = new To
                    {
                        Credential = new Credential {Domain = "AsdaOrganisation", Identity = "ASDA"}
                    },
                    Sender = new Sender
                    {
                        Credential = new Credential {Domain = "Linnworks", Identity = "Linnworks"}
                    }
                },
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