using System;
using System.Collections.Generic;
using System.Globalization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Order;
using Address = Asda.Integration.Domain.Models.Order.Address;


namespace Asda.Integration.Api.Mappers
{
    public static class OrderMapper
    {
        public static Order MapToOrder(PurchaseOrder purchaseOrder)
        {
            var order = new Order
            {
                DeliveryAddress = new Address
                {
                    Address1 =
                        purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Street,
                    Company = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .DeliverTo,
                    Country = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Country
                        .Text,
                    CountryCode = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .Country.IsoCountryCode,
                    EmailAddress = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email,
                    FullName = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Name.Text,
                    PhoneNumber = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone
                        .TelephoneNumber.Number,
                    PostCode = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .PostalCode,
                    Town = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.City
                },
                BillingAddress = new Address
                {
                    Address1 =
                        purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Street,
                    Company = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .DeliverTo,
                    Country = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Country
                        .Text,
                    CountryCode = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .Country.IsoCountryCode,
                    EmailAddress = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email,
                    FullName = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Name.Text,
                    PhoneNumber = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone
                        .TelephoneNumber.Number,
                    PostCode = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress
                        .PostalCode,
                    Town = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.City,
                },
                ChannelBuyerName = "Asda",
                Currency = "GBP",
                DispatchBy = purchaseOrder.Request.OrderRequest.OrderRequestHeader.ReqShipDate,
                ReferenceNumber = purchaseOrder.Request.OrderRequest.OrderRequestHeader.OrderID.ToString(),
                SecondaryReferenceNumber = purchaseOrder.Request.OrderRequest.OrderRequestHeader.RequisitionID,
                PaidOn = purchaseOrder.Request.OrderRequest.OrderRequestHeader.OrderDate,
                PaymentStatus = PaymentStatus.PAID,
                ReceivedDate = purchaseOrder.Request.OrderRequest.OrderRequestHeader.OrderDate,
            };
            foreach (var item in purchaseOrder.Request.OrderRequest.ItemOut)
            {
                order.OrderItems.Add(
                    new OrderItem
                    {
                        IsService = false,
                        ItemTitle = item.ItemDetail.Description.Text,
                        SKU = item.ItemID.SupplierProductID.ToString(),
                        PricePerUnit = Convert.ToDecimal(item.ItemDetail.UnitPrice.Money.Text),
                        Qty = item.Quantity,
                        OrderLineNumber = item.LineNumber.ToString(),
                        TaxCostInclusive = true,
                        UseChannelTax = false
                    }
                );
            }

            var extendedProperties = new List<OrderExtendedProperty>
            {
                new()
                {
                    Name = "PromisedDeliveryDate",
                    Type = "Info",
                    Value = purchaseOrder.Request.OrderRequest.OrderRequestHeader.PromisedDeliveryDate.ToString(
                        CultureInfo
                            .InvariantCulture)
                },
                new()
                {
                    Name = "FulfilmentType",
                    Type = "Info",
                    Value = purchaseOrder.Request.OrderRequest.OrderRequestHeader.FulfilmentType
                }
            };
            order.ExtendedProperties.AddRange(extendedProperties);

            return order;
        }
    }
}