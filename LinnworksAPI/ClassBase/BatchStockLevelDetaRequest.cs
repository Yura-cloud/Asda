using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class BatchStockLevelDetaRequest
	{
		public List<BatchStockLevelDelta> StockLevelDeltas { get; set; }

		public Guid StockLocationId { get; set; }
	} 
}