using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum ReturnsRefundsSearchDateType
	{
		ALLDATES,
		RECEIVED,
		PROCESSED,
		BOOKED,
		ACTIONED,
	}
}