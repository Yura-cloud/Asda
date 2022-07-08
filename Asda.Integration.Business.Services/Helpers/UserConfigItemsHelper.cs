using System.Linq;
using Asda.Integration.Domain.Models.User;

namespace Asda.Integration.Business.Services.Helpers
{
    public static class UserConfigItemsHelper
    {
        public static ConfigItem[] GetFtpSettingsConfigItems(UserConfig userConfig)
        {
            return new[]
            {
                new ConfigItem
                {
                    ConfigItemId = "Host",
                    Description = "FTP Host",
                    GroupName = "FTP Settings",
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
                    GroupName = "FTP Settings",
                    MustBeSpecified = true,
                    Name = "Port",
                    ReadOnly = false,
                    SelectedValue = userConfig.FtpSettings.Port.ToString(),
                    SortOrder = 2,
                    ValueType = ConfigValueType.INT
                },
                new ConfigItem
                {
                    ConfigItemId = "Password",
                    Description = "FTP Password",
                    GroupName = "FTP Settings",
                    MustBeSpecified = true,
                    Name = "Password",
                    ReadOnly = false,
                    SelectedValue = userConfig.FtpSettings.Password,
                    SortOrder = 3,
                    ValueType = ConfigValueType.PASSWORD
                },
                new ConfigItem
                {
                    ConfigItemId = "UserName",
                    Description = "FTP User Name",
                    GroupName = "FTP Settings",
                    MustBeSpecified = true,
                    Name = "User Name",
                    ReadOnly = false,
                    SelectedValue = userConfig.FtpSettings.UserName,
                    SortOrder = 4,
                    ValueType = ConfigValueType.STRING
                }
            };
        }

        public static ConfigItem[] GetFoldersPathsConfigItems(UserConfig userConfig)
        {
            return new[]
            {
                new ConfigItem
                {
                    ConfigItemId = "Orders",
                    Description = "Where the App will take purchase orders from",
                    GroupName = "Folders",
                    MustBeSpecified = true,
                    Name = "Orders",
                    ReadOnly = false,
                    SelectedValue = userConfig.RemoteFileStorage.OrdersPath,
                    SortOrder = 1,
                    ValueType = ConfigValueType.STRING
                },
                new ConfigItem
                {
                    ConfigItemId = "Dispatches",
                    Description = "Where dispatch files will be uploaded",
                    GroupName = "Folders",
                    MustBeSpecified = true,
                    Name = "Dispatches",
                    ReadOnly = false,
                    SelectedValue = userConfig.RemoteFileStorage.DispatchesPath,
                    SortOrder = 2,
                    ValueType = ConfigValueType.STRING
                },
                new ConfigItem
                {
                    ConfigItemId = "Acknowledgments",
                    Description = "Where acknowledgment files will be uploaded",
                    GroupName = "Folders",
                    MustBeSpecified = true,
                    Name = "Acknowledgments",
                    ReadOnly = false,
                    SelectedValue = userConfig.RemoteFileStorage.AcknowledgmentsPath,
                    SortOrder = 3,
                    ValueType = ConfigValueType.STRING
                },
                new ConfigItem
                {
                    ConfigItemId = "Cancellations",
                    Description = "Where cancellations files will be uploaded",
                    GroupName = "Folders",
                    MustBeSpecified = true,
                    Name = "Cancellations",
                    ReadOnly = false,
                    SelectedValue = userConfig.RemoteFileStorage.CancellationsPath,
                    SortOrder = 4,
                    ValueType = ConfigValueType.STRING
                },
                new ConfigItem
                {
                    ConfigItemId = "SnapInventories",
                    Description = "Where snapInventories files will be uploaded",
                    GroupName = "Folders",
                    MustBeSpecified = true,
                    Name = "SnapInventories",
                    ReadOnly = false,
                    SelectedValue = userConfig.RemoteFileStorage.SnapInventoriesPath,
                    SortOrder = 5,
                    ValueType = ConfigValueType.STRING
                }
            };
        }

        public static ConfigItem[] GetAllUserConfigItems(UserConfig userConfig)
        {
            var allUserConfigItems = new[]
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
            };

            return allUserConfigItems.Concat(GetFoldersPathsConfigItems(userConfig))
                .Concat(GetFtpSettingsConfigItems(userConfig))
                .ToArray();
        }
    }
}