using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum OrderDetailLevel
	{
		FOLDER,
		NOTES,
		IDENTIFIERS,
		EXTENDEDPROPERTIES,
		BINRACKS,
	}
}