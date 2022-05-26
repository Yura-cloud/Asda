using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LinnworksAPI
{ 
    [JsonConverter(typeof(StringEnumConverter))]
	public enum PickingWaveOrderState
	{
		Unpicked,
		PartialPicked,
		Picked,
		Processed,
		LockedOrParked,
		Cancelled,
		Deleted,
	}
}