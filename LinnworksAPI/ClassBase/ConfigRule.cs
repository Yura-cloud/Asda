using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public class ConfigRule
	{
		public String FieldName { get; set; }

		public List<PropertyRule> Rules { get; set; }
	} 
}