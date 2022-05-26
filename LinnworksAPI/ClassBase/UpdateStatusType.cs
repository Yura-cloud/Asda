using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum UpdateStatusType
	{
		NoChange,
		Pending,
		SentNotConfirmed,
		Success,
		Error,
	}
}