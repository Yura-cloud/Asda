using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum StateType
	{
		AVAILABLE,
		LOCKED_BASIC,
		LOCKED_FULL,
		MAINTENANCE,
	}
}