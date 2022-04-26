using Asda.Integration.Service.Intefaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class LocalConfigManagerService : ILocalConfigManagerService
    {
        private readonly IConfiguration _configuration;

        public LocalConfigManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string OrderPath => _configuration.GetSection(("LocalFileStorage")).GetSection("OrderPath").Value;

        public string DispatchPath => _configuration.GetSection(("LocalFileStorage")).GetSection("DispatchPath").Value;

        public string AcknowledgmentPath =>
            _configuration.GetSection(("LocalFileStorage")).GetSection("AcknowledgmentPath").Value;
    }
}