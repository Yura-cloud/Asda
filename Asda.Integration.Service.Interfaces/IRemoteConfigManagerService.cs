using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IRemoteConfigManagerService
    {
        RemoteFileStorageModel RemoteFileStorage { get; set; }
    }
}