using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface ILocalConfigManagerService
    {
        public LocalFileStorageModel LocalFileStorage { get; }
    }
}