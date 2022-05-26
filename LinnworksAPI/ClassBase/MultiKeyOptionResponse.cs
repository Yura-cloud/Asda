using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class MultiKeyOptionResponse
	{
        /// <summary>
        /// Option field 
        /// </summary>
		public String Field { get; set; }

        /// <summary>
        /// List of options 
        /// </summary>
		public List<String> Options { get; set; }
	} 
}