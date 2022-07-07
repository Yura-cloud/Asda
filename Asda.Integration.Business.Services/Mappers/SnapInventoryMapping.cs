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