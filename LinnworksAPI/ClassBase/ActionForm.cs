using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class ActionForm
	{
		public String Caption { get; set; }

		public List<ActionFormControl> Controls { get; set; }
	} 
}