using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class GetInventoryItemsCompositionByIdsResponse
	{
		public Dictionary<Guid,List<StockItemComposition>> InventoryItemsCompositionByIds { get; set; }
	} 
}