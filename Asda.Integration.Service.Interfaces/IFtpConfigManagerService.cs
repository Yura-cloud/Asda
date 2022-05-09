using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpConfigManagerService
    {
        public FtpSettingsModel FtpSettings { get; }
    }
}