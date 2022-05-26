using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum SearchFieldTypes
	{
		Source,
		SubSource,
		ItemIdentifier,
	}
}