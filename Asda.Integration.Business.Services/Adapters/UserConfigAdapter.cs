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
                throw new ArgumentNullException("authorizationToken");

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
                    if (!HelperAdapter.CheckExistingFolders(userConfig.FtpSettings, userConfig.RemoteFileStorage,
                            out var errorMessage))
                    {
                        throw new Exception(errorMessage);
                    }

                    userConfig.StepName = ConfigStagesEnum.UserConfig.ToString();
                    break;
                case ConfigStagesEnum.UserConfig:
                    userConfig.Location = configItems.FirstOrDefault(i => i.ConfigItemId == "Location");
                    FillInRemoteFilesStorage(userConfig, configItems);
                    FillInFtpSettings(userConfig, configItems);
                    break;
            }
        }

        private void FillInFtpSettings(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.FtpSettings.Host = ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Host")).Trim();
            userConfig.FtpSettings.Port = (int) configItems.FirstOrDefault(i => i.ConfigItemId == "Port");
            userConfig.FtpSettings.Password =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Password")).Trim();
            userConfig.FtpSettings.UserName =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "UserName")).Trim();
        }

        private static void FillInRemoteFilesStorage(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.RemoteFileStorage.OrdersPath =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Orders")).Trim();
            userConfig.RemoteFileStorage.DispatchesPath =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Dispatches")).Trim();
            userConfig.RemoteFileStorage.AcknowledgmentsPath =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Acknowledgments")).Trim();
            userConfig.RemoteFileStorage.CancellationsPath =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "Cancellations")).Trim();
            userConfig.RemoteFileStorage.SnapInventoriesPath =
                ((string) configItems.FirstOrDefault(i => i.ConfigItemId == "SnapInventories")).Trim();
        }

        private void ReadFoldersNames(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.RemoteFileStorage.OrdersPath = configItems.FirstOrDefault(i => i.ConfigItemId == "Orders");
            userConfig.RemoteFileStorage.DispatchesPath =
                configItems.FirstOrDefault(i => i.ConfigItemId == "Dispatches");
            userConfig.RemoteFileStorage.AcknowledgmentsPath =
                configItems.FirstOrDefault(i => i.ConfigItemId == "Acknowledgments");
            userConfig.RemoteFileStorage.CancellationsPath =
                configItems.FirstOrDefault(i => i.ConfigItemId == "Cancellations");
            userConfig.RemoteFileStorage.SnapInventoriesPath =
                configItems.FirstOrDefault(i => i.ConfigItemId == "SnapInventories");
        }

        private void ReadFtpSettings(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.FtpSettings.Host = configItems.FirstOrDefault(i => i.ConfigItemId == "Host");
            userConfig.FtpSettings.Port = configItems.FirstOrDefault(i => i.ConfigItemId == "Port");
            userConfig.FtpSettings.Password = configItems.FirstOrDefault(i => i.ConfigItemId == "Password");
            userConfig.FtpSettings.UserName = configItems.FirstOrDefault(i => i.ConfigItemId == "UserName");
        }

        public void Save(UserConfig userConfig)
        {
            var userConfigJson = Newtonsoft.Json.JsonConvert.SerializeObject(userConfig);

            _fileRepository.Save(userConfig.AuthorizationToken, userConfigJson);
        }
    }
}