using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum StockAllocationType
	{
		NotAllocated,
		Insuffient,
		Partial,
		Full,
		OverAllocated,
	}
}