using System;
using System.Linq;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Interfaces;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Asda.Integration.Business.Services.Adapters
{
    public class UserConfigAdapter : IUserConfigAdapter
    {
        private readonly IRepository _fileRepository;

        public UserConfigAdapter(IConfiguration configuration)
        {
            _fileRepository = new FileRepository(configuration["AppSettings:UserStoreLocation"]);
        }

        /// <inheritdoc />
        public UserConfig LoadByToken(string authorizationToken)
        {
            if (string.IsNullOrWhiteSpace(authorizationToken))
            {
                throw new ArgumentNullException("authorizationToken");
            }

            if (!_fileRepository.FileExists(authorizationToken))
            {
                return null;
            }

            var userConfigJson = _fileRepository.Load(authorizationToken);
            var userConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConfig>(userConfigJson);
            return userConfig;
        }

        /// <inheritdoc />
        public void Delete(string authorizationToken)
        {
            if (_fileRepository.FileExists(authorizationToken))
            {
                _fileRepository.Delete(authorizationToken);
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        /// <inheritdoc />
        public UserConfig CreateNew(string email, Guid linnworksUniqueIdentifier, string accountName, Guid appToken)
        {
            var userConfig = new UserConfig
            {
                AuthorizationToken = Guid.NewGuid().ToString("N"),
                Email = email,
                LinnworksUniqueIdentifier = linnworksUniqueIdentifier,
                AccountName = accountName,
                AppToken = appToken,
                FtpSettings = new FtpSettingsModel(),
                RemoteFileStorage = new RemoteFileStorageModel()
            };
            Save(userConfig);
            return userConfig;
        }

        /// <inheritdoc />
        public void FillUserConfig(UserConfig userConfig, ConfigItem[] configItems)
        {
            var step = Enum.Parse(typeof(ConfigStagesEnum), userConfig.StepName);
            switch (step)
            {
                case ConfigStagesEnum.AddFtpSettings:
                    ReadFtpSettings(userConfig, configItems);
                    //This method throws an exception if something goes wrong, and then the user can see it in his UI
                    HelperAdapter.TestFtpConnection(userConfig.FtpSettings);
                    userConfig.StepName = ConfigStagesEnum.AddFoldersNames.ToString();
                    break;
                case ConfigStagesEnum.AddFoldersNames:
                    ReadFoldersNames(userConfig, configItems);
                    var errorMessage =
                        HelperAdapter.TestIfFoldersExist(userConfig.FtpSettings, userConfig.RemoteFileStorage);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        throw new Exception(errorMessage);
                    }

                    userConfig.StepName = ConfigStagesEnum.UserConfig.ToString();
                    break;
                case ConfigStagesEnum.UserConfig:
                    ReadAllUserConfigSettings(userConfig, configItems);
                    break;
            }
        }

        private static void ReadAllUserConfigSettings(UserConfig userConfig, ConfigItem[] configItems)
        {
            ReadFoldersNames(userConfig, configItems);
            ReadFtpSettings(userConfig, configItems);
            //Location Name
            userConfig.Location =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Location")?.SelectedValue.Trim();
        }

        private static void ReadFoldersNames(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.RemoteFileStorage.OrdersPath =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Orders")?.SelectedValue.Trim();
            userConfig.RemoteFileStorage.DispatchesPath =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Dispatches")?.SelectedValue.Trim();
            userConfig.RemoteFileStorage.AcknowledgmentsPath =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Acknowledgments")?.SelectedValue.Trim();
            userConfig.RemoteFileStorage.CancellationsPath =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Cancellations")?.SelectedValue.Trim();
            userConfig.RemoteFileStorage.SnapInventoriesPath =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "SnapInventories")?.SelectedValue.Trim();
        }

        private static void ReadFtpSettings(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.FtpSettings.Host =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Host")?.SelectedValue.Trim();
            userConfig.FtpSettings.Port = configItems?.FirstOrDefault(i => i.ConfigItemId == "Port");
            userConfig.FtpSettings.Password =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "Password")?.SelectedValue.Trim();
            userConfig.FtpSettings.UserName =
                configItems?.FirstOrDefault(i => i.ConfigItemId == "UserName")?.SelectedValue.Trim();
        }

        public void Save(UserConfig userConfig)
        {
            var userConfigJson = Newtonsoft.Json.JsonConvert.SerializeObject(userConfig);
            _fileRepository.Save(userConfig.AuthorizationToken, userConfigJson);
        }
    }
}