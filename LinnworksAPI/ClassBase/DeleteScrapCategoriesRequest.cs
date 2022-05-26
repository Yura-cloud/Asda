using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class DeleteScrapCategoriesRequest
	{
		public IEnumerable<Int32> CategoryIds { get; set; }
	} 
}