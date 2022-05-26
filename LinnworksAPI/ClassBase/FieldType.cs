using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum FieldType
	{
		Default,
		String,
		Int,
		Bool,
		Guid,
		Float,
		Double,
	}
}