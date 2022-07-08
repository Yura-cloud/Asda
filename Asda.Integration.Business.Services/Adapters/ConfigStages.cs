using System;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services.Adapters
{
    public class ConfigStages : IConfigStages
    {
        public UserConfigResponse StageResponse(UserConfig userConfig, string errorMessage = "")
        {
            var configStage = Enum.Parse(typeof(ConfigStagesEnum), userConfig.StepName);
            return configStage switch
            {
                ConfigStagesEnum.AddFtpSettings => GetFtpSettings(userConfig),
                ConfigStagesEnum.AddFoldersNames => GetFoldersNames(userConfig),
                ConfigStagesEnum.UserConfig => GetConfigStep(userConfig),
                _ => new UserConfigResponse {Error = errorMessage}
            };
        }

        private static UserConfigResponse GetFtpSettings(UserConfig userConfig)
        {
            return new UserConfigResponse
            {
                StepName = ConfigStagesEnum.AddFtpSettings.ToString(),
                AccountName = "AccountName",
                WizardStepTitle = "Adding FTP Settings",
                WizardStepDescription = "Here you should input your FTP details",
                ConfigItems = UserConfigItemsHelper.GetFtpSettingsConfigItems(userConfig)
            };
        }

        private static UserConfigResponse GetFoldersNames(UserConfig userConfig)
        {
            return new UserConfigResponse
            {
                StepName = ConfigStagesEnum.AddFoldersNames.ToString(),
                AccountName = "AccountName",
                WizardStepTitle = "Adding names of the working folders",
                WizardStepDescription = "For example: Linnworks/Orders",
                ConfigItems = UserConfigItemsHelper.GetFoldersPathsConfigItems(userConfig)
            };
        }

        private static UserConfigResponse GetConfigStep(UserConfig userConfig)
        {
            // We don't return API Credentials, if they're wrong or invalid we go back to starting stage.
            return new UserConfigResponse
            {
                StepName = ConfigStagesEnum.UserConfig.ToString(),
                AccountName = "Example account name",
                WizardStepTitle = "User Configuration",
                WizardStepDescription = "User Config",
                ConfigItems = UserConfigItemsHelper.GetAllUserConfigItems(userConfig)
            };
        }
    }
}