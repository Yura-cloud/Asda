using Asda.Integration.Service.Intefaces;
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

    }
}