using System;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services
{
    public class FtpConfigManagerService : IFtpConfigManagerService
    {
        public FtpSettingsModel FtpSettings { get; set; }

        public FtpConfigManagerService(IConfiguration configuration)
        {
            FtpSettings =
                new FtpSettingsModel(
                    Convert.ToInt32(configuration.GetSection("FtpSettings").GetSection("Port").Value),
                    configuration.GetSection(("FtpSettings")).GetSection("UserName").Value,
                    configuration.GetSection(("FtpSettings")).GetSection("Password").Value,
                    configuration.GetSection(("FtpSettings")).GetSection("Host").Value);
        }
    }
}