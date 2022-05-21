using System;
using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Products;

namespace Asda.Integration.Api.Mappers
{
    public static class SnapInventoryMapping
    {
        public static InventorySnapshot MapToInventorySnapshot(ProductInventory productInventory)
        {
            var inventorySnapshot = new InventorySnapshot
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
                    InventorySnapshotRequest = new InventorySnapshotRequest
                    {
                        InventorySnapshotRequestHeader = new InventorySnapshotRequestHeader
                        {
                            SnapshotDate = DateTime.Now.ToString(),
                            Description = "",
                            ListId = ""
                        },
                        Records = new Records
                        {
                            Record = new List<Record>
                            {
                                new()
                                {
                                    ProductId = productInventory.SKU,
                                    AllocationQty = productInventory.Quantity.ToString()
                                }
                            }
                        }
                    }
                }
            };
            return inventorySnapshot;
        }
    }
}