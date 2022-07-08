using System;
using System.Collections.Generic;
using System.Globalization;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Order;
using Address = Asda.Integration.Domain.Models.Order.Address;


namespace Asda.Integration.Api.Mappers
{
    public static class OrderMapper
    {
        public static Order MapToOrder(PurchaseOrder purchaseOrder)
        {
            var address = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address;
            var header = purchaseOrder.Request.OrderRequest.OrderRequestHeader;
            var order = new Order
            {
                DeliveryAddress = new Address
                {
                    Address1 = address.PostalAddress.Street,
                    Company = address.PostalAddress.DeliverTo,
                    Country = address.PostalAddress.Country.Text,
                    CountryCode = address.PostalAddress.Country.IsoCountryCode,
                    EmailAddress = address.Email,
                    FullName = address.Name.Text,
                    PhoneNumber = address.Phone.TelephoneNumber.Number,
                    PostCode = address.PostalAddress.PostalCode,
                    Town = address.PostalAddress.City
                },
                BillingAddress = new Address
                {
                    Address1 = address.PostalAddress.Street,
                    Company = address.PostalAddress.DeliverTo,
                    Country = address.PostalAddress.Country.Text,
                    CountryCode = address.PostalAddress.Country.IsoCountryCode,
                    EmailAddress = address.Email,
                    FullName = address.Name.Text,
                    PhoneNumber = address.Phone.TelephoneNumber.Number,
                    PostCode = address.PostalAddress.PostalCode,
                    Town = address.PostalAddress.City
                },
                ChannelBuyerName = "Asda",
                //From Asda specification we should use GBP here
                Currency = "GBP",
                DispatchBy = header.ReqShipDate,
                ReferenceNumber = header.OrderID.ToString(),
                SecondaryReferenceNumber = header.RequisitionID,
                PaidOn = header.OrderDate,
                PaymentStatus = PaymentStatus.PAID,
                ReceivedDate = header.OrderDate,
                ExtendedProperties = new List<OrderExtendedProperty>
                {
                    new()
                    {
                        Name = "PromisedDeliveryDate",
                        Type = "Info",
                        Value = header.PromisedDeliveryDate.ToString(CultureInfo.InvariantCulture)
                    },
                    new()
                    {
                        Name = "FulfilmentType",
                        Type = "Info",
                        Value = header.FulfilmentType
                    }
                }
            };
            foreach (var item in purchaseOrder.Request.OrderRequest.ItemOut)
            {
                order.OrderItems.Add(
                    new OrderItem
                    {
                        IsService = false,
                        ItemTitle = item.ItemDetail.Description.Text,
                        SKU = item.ItemID.AsdaItemID,
                        PricePerUnit = Convert.ToDecimal(item.ItemDetail.UnitPrice.Money.Text),
                        Qty = item.Quantity,
                        OrderLineNumber = item.LineNumber.ToString(),
                        TaxCostInclusive = true,
                        UseChannelTax = false
                    }
                );
            }

            return order;
        }
    }
}