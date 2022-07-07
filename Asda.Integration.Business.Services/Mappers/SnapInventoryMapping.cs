using System;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using LinnworksAPI;

namespace Asda.Integration.Api.Mappers
{
    public static class SnapInventoryMapping
    {
        public static InventorySnapshot MapToInventorySnapshot(StockItemLevel stockItemLevel)
        {
            var inventorySnapshot = new InventorySnapshot
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
                            Record = new Record
                            {
                                ProductId = stockItemLevel.SKU,
                                AllocationQty = stockItemLevel.Available.ToString(),
                                OrderedQty = stockItemLevel.InOrders.ToString()
                            }
                        }
                    }
                }
            };
            return inventorySnapshot;
        }
    }
}