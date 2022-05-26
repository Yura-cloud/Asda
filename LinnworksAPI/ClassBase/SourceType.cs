using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum SourceType
	{
		netcore10,
		netcore21,
		netcore31,
	}
}