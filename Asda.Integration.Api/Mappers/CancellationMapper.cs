using System;
using Asda.Integration.Domain.Models.Business.Acknowledgment;
using Asda.Integration.Domain.Models.Business.Cancellation;
using Asda.Integration.Domain.Models.Order;
using ConfirmationHeader = Asda.Integration.Domain.Models.Business.Acknowledgment.ConfirmationHeader;
using ConfirmationRequest = Asda.Integration.Domain.Models.Business.Acknowledgment.ConfirmationRequest;
using OrderReference = Asda.Integration.Domain.Models.Business.Acknowledgment.OrderReference;
using Request = Asda.Integration.Domain.Models.Business.Acknowledgment.Request;

namespace Asda.Integration.Api.Mappers
{
    public class CancellationMapper
    {
        public static Cancellation MapToCancellation(OrderCancellation orderCancellation)
        {
            var acknowledgment = new Acknowledgment
            {
                
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
            return null;
        }
    }
}