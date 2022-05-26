using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class ShippingMethod
	{
        /// <summary>
        /// Courier name 
        /// </summary>
		public String Vendor { get; set; }

        /// <summary>
        /// List of postal services 
        /// </summary>
		public List<PostageService> PostalServices { get; set; }
	} 
}