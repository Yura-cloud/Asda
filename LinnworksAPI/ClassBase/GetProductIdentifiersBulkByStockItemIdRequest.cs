using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class GetProductIdentifiersBulkByStockItemIdRequest
	{
		public IEnumerable<Guid> StockItemIds { get; set; }
	} 
}