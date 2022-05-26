using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class DeleteOrdersFromPickingWavesRequest
	{
        /// <summary>
        /// List of Linnworks OrderIds 
        /// </summary>
		public List<Int32> OrderIds { get; set; }
	} 
}