using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class StockItemChannelSkuResponse
	{
		public Guid StockItemId { get; set; }

		public List<StockItemChannelSKU> ChannelSkus { get; set; }
	} 
}