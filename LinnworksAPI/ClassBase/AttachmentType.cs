using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum AttachmentType
	{
		INVOICE,
		PICKLIST,
		PACKLIST,
		SHIPPINGLABEL,
	}
}