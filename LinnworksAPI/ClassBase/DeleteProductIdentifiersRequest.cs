using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class DeleteProductIdentifiersRequest
	{
		public IEnumerable<Int64> ProductIdentifierIds { get; set; }
	} 
}