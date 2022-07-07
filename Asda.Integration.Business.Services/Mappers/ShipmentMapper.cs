using System;
using System.Linq;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Api.Mappers
{
    public static class ShipmentMapper
    {
        public static ShipmentConfirmation MapToShipmentConfirmation(OrderDespatch orderDespatch)
        {
            var shipmentConfirmation = new ShipmentConfirmation
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
                    ShipNoticeRequest = new ShipNoticeRequest
                    {
                        ShipNoticeHeader = new ShipNoticeHeader
                        {
                            ShipmentID = "S89823-123",
                            CarrierId = orderDespatch.ShippingVendor ?? "toyou"
                        },
                        ShipControl = new ShipControl {ShipmentIdentifier = orderDespatch.TrackingNumber},
                        ShipNoticePortion = new ShipNoticePortion
                        {
                            OrderReference = new OrderReference {OrderID = orderDespatch.ReferenceNumber},
                            ShipNoticeItem = orderDespatch.Items
                                .Select(item => new ShipNoticeItem
                                {
                                    LineNumber = Convert.ToInt32(item.OrderLineNumber),
                                    Quantity = item.DespatchedQuantity,
                                    UnitOfMeasure = "EACH"
                                }).ToList()
                        }
                    }
                }
            };

            return shipmentConfirmation;
        }
    }
}