using System;
using Asda.Integration.Service.Intefaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class FtpConfigManagerService : IFtpConfigManagerService
    {
        private readonly IConfiguration _configuration;

        public FtpConfigManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int Port => Convert.ToInt32(_configuration.GetSection("FtpSettings").GetSection("Port").Value);
        public string UserName => _configuration.GetSection(("FtpSettings")).GetSection("UserName").Value;
        public string Password => _configuration.GetSection(("FtpSettings")).GetSection("Password").Value;
        public string Host => _configuration.GetSection(("FtpSettings")).GetSection("Host").Value;
        public string ServerFilePath => _configuration.GetSection(("FtpSettings")).GetSection("ServerFilePath").Value;
        public string LocalFilePath => _configuration.GetSection(("FtpSettings")).GetSection("LocalFilePath").Value;
    }
}