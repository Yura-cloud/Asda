using System;
using System.Linq;
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
                            //From Asda specification "This is a required field by the cXML protocol but not validated or used by Asda."
                            //This value is from example
                            ShipmentID = "S89823-123",
                            //TO DO => asked Alexandra about details for this property. Waiting for the answer
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