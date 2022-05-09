using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class LocalConfigManagerService : ILocalConfigManagerService
    {
        public LocalFileStorageModel LocalFileStorage { get; }

        public LocalConfigManagerService(IConfiguration configuration)
        {
            LocalFileStorage = new LocalFileStorageModel(
                configuration.GetSection(("LocalFileStorage")).GetSection("OrderPath").Value,
                configuration.GetSection(("LocalFileStorage")).GetSection("DispatchPath").Value,
                configuration.GetSection(("LocalFileStorage")).GetSection("AcknowledgmentPath").Value,
                configuration.GetSection(("LocalFileStorage")).GetSection("CancellationPath").Value,
                configuration.GetSection(("LocalFileStorage")).GetSection("SnapInventoryPath").Value
            );
        }
    }
}