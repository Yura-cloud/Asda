using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class GetAssignedOrderItemBatchesRequest
	{
		public List<Guid> OrderItemRows { get; set; }
	} 
}