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

        public string LocalFilePath => _configuration.GetSection(("FtpSettings")).GetSection("LocalFilePath").Value;
    }
}