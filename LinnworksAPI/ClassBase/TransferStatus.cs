using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum TransferStatus
	{
		Draft,
		Request,
		Accepted,
		Packing,
		InTransit,
		CheckingIn,
		Delivered,
	}
}