using System;
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
                Request = new Request
                {
                    ConfirmationRequest = new ConfirmationRequest
                    {
                        ConfirmationHeader = new ConfirmationHeader {Type = "detail"},
                        OrderReference = new OrderReference {OrderID = orderCancellation.ReferenceNumber},
                        ConfirmationItem = orderCancellation.Items
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
                            }).ToList()
                    }
                }
            };
            return cancellation;
        }
    }
}