using System;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services.Adapters
{
    public class ConfigStages : IConfigStages
    {
        public UserConfigResponse StageResponse(UserConfig userConfig, string errorMessage = "")
        {
            switch (Enum.Parse(typeof(ConfigStagesEnum), userConfig.StepName))
            {
                case ConfigStagesEnum.AddFtpSettings:
                    return GetFtpSettings(userConfig);
                case ConfigStagesEnum.AddFoldersNames:
                    return GetFoldersNames(userConfig);
                case ConfigStagesEnum.UserConfig:
                    return GetConfigStep(userConfig);
            }

            return new UserConfigResponse {Error = errorMessage};
        }

        private static UserConfigResponse GetFtpSettings(UserConfig userConfig)
        {
            return new UserConfigResponse
            {
                StepName = "AddFtpSettings",
                AccountName = "AccountName",
                WizardStepTitle = "Add FTP Settings",
                WizardStepDescription = "This is where you add your FTP credentials",
                ConfigItems = new[]
                {
                    new ConfigItem
                    {
                        ConfigItemId = "Host",
                        Description = "FTP Host",
                        GroupName = "Ftp Settings",
                        MustBeSpecified = true,
                        Name = "Host",
                        ReadOnly = false,
                        SelectedValue = userConfig.FtpSettings.Host,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "Port",
                        Description = "FTP Port",
                        GroupName = "Ftp Settings",
                        MustBeSpecified = true,
                        Name = "Port",
                        ReadOnly = false,
                        SelectedValue = userConfig.FtpSettings.Port.ToString(),
                        SortOrder = 1,
                        ValueType = ConfigValueType.INT
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "Password",
                        Description = "FTP Password",
                        GroupName = "Ftp Settings",
                        MustBeSpecified = true,
                        Name = "Password",
                        ReadOnly = false,
                        SelectedValue = userConfig.FtpSettings.Password,
                        SortOrder = 1,
                        ValueType = ConfigValueType.PASSWORD
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "UserName",
                        Description = "FTP User Name",
                        GroupName = "Ftp Settings",
                        MustBeSpecified = true,
                        Name = "User Name",
                        ReadOnly = false,
                        SelectedValue = userConfig.FtpSettings.UserName,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                }
            };
        }

        private static UserConfigResponse GetFoldersNames(UserConfig userConfig)
        {
            return new UserConfigResponse
            {
                StepName = "AddFoldersNames",
                AccountName = "AccountName",
                WizardStepTitle = "Add names of the working folders",
                WizardStepDescription = "For example: Linnworks/Orders",
                ConfigItems = new[]
                {
                    new ConfigItem
                    {
                        ConfigItemId = "Orders",
                        Description = "Where this App takes purchase orders",
                        GroupName = "FoldersName",
                        MustBeSpecified = true,
                        Name = "Orders",
                        ReadOnly = false,
                        SelectedValue = userConfig.RemoteFileStorage.OrderPath,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "Dispatches",
                        Description = "Where the dispatch files will be uploaded",
                        GroupName = "FoldersName",
                        MustBeSpecified = true,
                        Name = "Dispatches",
                        ReadOnly = false,
                        SelectedValue = userConfig.RemoteFileStorage.DispatchPath,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "Acknowledgments",
                        Description = "Where the acknowledgment files will be uploaded",
                        GroupName = "FoldersName",
                        MustBeSpecified = true,
                        Name = "Acknowledgments",
                        ReadOnly = false,
                        SelectedValue = userConfig.RemoteFileStorage.AcknowledgmentPath,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "Cancellations",
                        Description = "Where the cancellations files will be uploaded",
                        GroupName = "FoldersName",
                        MustBeSpecified = true,
                        Name = "Cancellations",
                        ReadOnly = false,
                        SelectedValue = userConfig.RemoteFileStorage.CancellationPath,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    new ConfigItem
                    {
                        ConfigItemId = "SnapInventories",
                        Description = "Where the snapInventories files will be uploaded",
                        GroupName = "FoldersName",
                        MustBeSpecified = true,
                        Name = "SnapInventories",
                        ReadOnly = false,
                        SelectedValue = userConfig.RemoteFileStorage.SnapInventoryPath,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    },
                    
                }
            };
        }

        private static UserConfigResponse GetConfigStep(UserConfig userConfig)
        {
            // We don't return API Credentials, if they're wrong or invalid we go back to starting stage.
            return new UserConfigResponse
            {
                StepName = "UserConfig",
                AccountName = "Example account name",
                WizardStepTitle = "UserConfig",
                WizardStepDescription = "User Config",
                ConfigItems = new[]
                {
                    new ConfigItem
                    {
                        ConfigItemId = "Location",
                        Description = "Defines location of SnapShot",
                        GroupName = "Inventory",
                        MustBeSpecified = true,
                        Name = "Sync Inventory from Location",
                        ReadOnly = false,
                        SelectedValue = userConfig.Location,
                        SortOrder = 1,
                        ValueType = ConfigValueType.STRING
                    }
                }
            };
        }
    }
}