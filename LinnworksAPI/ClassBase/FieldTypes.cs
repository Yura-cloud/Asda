using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum FieldTypes
	{
		Decimal,
		Text,
		List,
		Date,
		Boolean,
		None,
		Button,
		HtmlList,
		Integer,
	}
}