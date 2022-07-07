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