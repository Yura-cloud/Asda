using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum HistoryType
	{
		RETURNS,
		REFUNDS,
		CANCELLATIONS,
	}
}