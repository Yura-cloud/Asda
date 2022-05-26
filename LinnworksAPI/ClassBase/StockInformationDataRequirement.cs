using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum StockInformationDataRequirement
	{
		StockLevels,
		Pricing,
		Supplier,
		ShippingInformation,
		ChannelTitle,
		ChannelDescription,
		ChannelPrice,
		ExtendedProperties,
		Images,
	}
}