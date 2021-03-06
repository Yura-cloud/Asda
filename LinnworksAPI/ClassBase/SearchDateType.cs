using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum SearchDateType
	{
		ALLDATES,
		RECEIVED,
		PROCESSED,
		PAYMENTRECEIVED,
		CANCELLED,
	}
}