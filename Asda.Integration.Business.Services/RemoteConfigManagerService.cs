using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class RemoteConfigManagerService : IRemoteConfigManagerService
    {
        private readonly IConfiguration _configuration;

        public RemoteFileStorageModel RemoteFileStorage { get; set; }

        public RemoteConfigManagerService(IConfiguration configuration)
        {
            RemoteFileStorage = new RemoteFileStorageModel(
                configuration.GetSection(("RemoteFileStorage")).GetSection("PurchaseOrdersPath").Value,
                configuration.GetSection(("RemoteFileStorage")).GetSection("DispatchPath").Value,
                configuration.GetSection(("RemoteFileStorage")).GetSection("AcknowledgmentPath").Value,
                configuration.GetSection(("RemoteFileStorage")).GetSection("CancellationPath").Value,
                configuration.GetSection(("RemoteFileStorage")).GetSection("SnapInventoryPath").Value
            );
        }
    }
}