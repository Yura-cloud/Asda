using System;
using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Domain.Models.Order;

namespace Asda.Integration.Api.Mappers
{
    public static class ShipmentMapper
    {
        public static List<ShipmentConfirmation> MapToShipmentConfirmations(List<OrderDespatch> orderDespatches)
        {
            var shipmentConfirmations = new List<ShipmentConfirmation>();
            foreach (var orderDespatch in orderDespatches)
            {
                var shipmentConfirmation = new ShipmentConfirmation
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
                                Identity = orderDespatch.ReferenceNumber
                            }
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
                            ShipControl = new ShipControl
                            {
                                ShipmentIdentifier = orderDespatch.TrackingNumber
                            },
                            ShipNoticePortion = new ShipNoticePortion
                            {
                                OrderReference = new OrderReference
                                {
                                    DocumentReference = new DocumentReference
                                    {
                                        PayloadID = orderDespatch.ReferenceNumber
                                    }
                                }
                            }
                        }
                    }
                };
                var shipNoticeItems = new List<ShipNoticeItem>();
                foreach (var item in orderDespatch.Items)
                {
                    var shipNoteiceItem = new ShipNoticeItem
                    {
                        LineNumber = Convert.ToInt32(item.OrderLineNumber),
                        Quantity = item.DespatchedQuantity,
                        UnitOfMeasure = "EACH"
                    };
                    shipNoticeItems.Add(shipNoteiceItem);
                }

                shipmentConfirmation.Request.ShipNoticeRequest.ShipNoticePortion.ShipNoticeItem = shipNoticeItems;
                shipmentConfirmations.Add(shipmentConfirmation);
            }

            return shipmentConfirmations;
        }
    }
}