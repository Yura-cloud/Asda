using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum ExecutionType
	{
		API,
		Scheduled,
		RulesEngine_Order,
	}
}