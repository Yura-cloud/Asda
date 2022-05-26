using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class GetBatchInventoryByIdRequest
	{
		public List<Int32> BatchInventoryIds { get; set; }

		public Boolean LoadRelatedInventoryLines { get; set; }
	} 
}