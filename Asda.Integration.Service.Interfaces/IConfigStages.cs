using Asda.Integration.Domain.Models.User;

namespace Asda.Integration.Service.Interfaces
{
    public interface IConfigStages
    {
        public UserConfigResponse StageResponse(UserConfig userConfig, string errorMessage = "");
    }
}