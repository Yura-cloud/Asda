using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum EvaluatorGroup
	{
		BasicEquality,
		Range,
		Set,
		NumberEquality,
		StringEquality,
	}
}