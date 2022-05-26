using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class SearchOrdersResponse
	{
		public OrderViewIds[] OpenOrders { get; set; }

		public HashSet<Guid> ProcessedOrders { get; set; }
	} 
}