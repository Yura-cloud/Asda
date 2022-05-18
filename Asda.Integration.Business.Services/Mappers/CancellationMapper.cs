using System;
using System.Collections.Generic;
using System.Linq;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Api.Mappers
{
    public class CancellationMapper
    {
        public static Cancellation MapToCancellation(OrderCancellation orderCancellation)
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
                        }
                    }
                }
            };
            var confirmationItems = orderCancellation.Items
                .Select(item => new ConfirmationItem
                {
                    LineNumber = Convert.ToInt32(item.OrderLineNumber),
                    Quantity = item.CancellationQuantity,
                    UnitOfMeasure = "EACH",
                    ConfirmationStatus = new ConfirmationStatus
                    {
                        Type = "reject",
                        Quantity = item.CancellationQuantity,
                        UnitOfMeasure = "EACH"
                    }
                }).ToList();
            cancellation.Request.ConfirmationRequest.ConfirmationItem = confirmationItems;
            
            return cancellation;
        }
    }
}