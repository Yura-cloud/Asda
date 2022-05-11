using System;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Api.Mappers
{
    public class CancellationMapper
    {
        public static Cancellation MapToCancellation(OrderCancellation orderCancellation, int itemNumber)
        {
            var cancellation = new Cancellation
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
                            Identity = "Linnworks"
                        }
                    }
                },
                Request = new Request
                {
                    ConfirmationRequest = new ConfirmationRequest
                    {
                        ConfirmationHeader = new ConfirmationHeader
                        {
                            Type = "detail"
                        },
                        OrderReference = new OrderReference
                        {
                            OrderID = Convert.ToInt32(orderCancellation.ReferenceNumber)
                        },
                        ConfirmationItem = new ConfirmationItem
                        {
                            LineNumber = itemNumber + 1,
                            Quantity = orderCancellation.Items[itemNumber].CancellationQuantity,
                            UnitOfMeasure = "EACH",
                            ConfirmationStatus = new ConfirmationStatus
                            {
                                Type = "reject",
                                Quantity = orderCancellation.Items[itemNumber].CancellationQuantity,
                                UnitOfMeasure = "EACH"
                            }
                        }
                    }
                }
            };
            return cancellation;
        }
    }
}