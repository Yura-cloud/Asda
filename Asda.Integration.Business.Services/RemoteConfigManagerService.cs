using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class RemoteConfigManagerService : IRemoteConfigManagerService
    {
        private readonly IConfiguration _configuration;

        public RemoteConfigManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DispatchPath => _configuration.GetSection(("RemoteFileStorage")).GetSection("DispatchPath").Value;

        public string AcknowledgmentPath =>
            _configuration.GetSection(("RemoteFileStorage")).GetSection("AcknowledgmentPath").Value;

        public string CancellationPath =>
            _configuration.GetSection(("RemoteFileStorage")).GetSection("CancellationPath").Value;

        public string SnapInventoryPath =>
            _configuration.GetSection(("RemoteFileStorage")).GetSection("SnapInventoryPath").Value;
    }
}