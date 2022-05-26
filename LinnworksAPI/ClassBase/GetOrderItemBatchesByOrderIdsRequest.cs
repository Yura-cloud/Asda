using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class GetOrderItemBatchesByOrderIdsRequest
	{
		public List<Guid> pkOrderIds { get; set; }
	} 
}