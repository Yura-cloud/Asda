using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum DisplayType
	{
		FreeText,
		AutoComplete,
		Dropdown,
		None,
	}
}