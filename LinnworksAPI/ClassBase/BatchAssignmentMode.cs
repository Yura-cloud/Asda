using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum BatchAssignmentMode
	{
		ALL,
		AUTO_ONLY,
		UNASSIGNED_ONLY,
	}
}