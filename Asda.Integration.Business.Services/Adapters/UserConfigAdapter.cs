using System;
using System.Linq;
using Asda.Integration.Business.Services.Config;
using Asda.Integration.Domain.Interfaces;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace Asda.Integration.Business.Services.Adapters
{
    public class UserConfigAdapter : IUserConfigAdapter
    {
        private readonly IRepository _fileRepository;

        private readonly IConfigStages _configStages;

        public UserConfigAdapter(IOptions<AppSettings> config, IConfigStages configStages)
        {
            _fileRepository = config.Value.FileRepository;
            _configStages = configStages;
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

            string json = _fileRepository.LoadByToken(authorizationToken);
            var userConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConfig>(json);
            return userConfig;
        }

        public UserConfig LoadByUserId(Guid userId)
        {
            if (!_fileRepository.DirectoryExists())
            {
                throw new Exception("Directory with users not found");
            }

            var files = _fileRepository.LoadAll();
            foreach (var file in files)
            {
                var fileConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConfig>(file);
                if (fileConfig?.LinnworksUniqueIdentifier == userId)
                {
                    return fileConfig;
                }
            }

            throw new Exception("Please reinstall App by link!");
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
        public UserConfig CreateNew(string email, Guid linnworksUniqueIdentifier, string accountName, Guid token)
        {
            var userConfig = new UserConfig
            {
                AuthorizationToken = token.ToString("N"),
                Email = email,
                LinnworksUniqueIdentifier = linnworksUniqueIdentifier,
                AccountName = accountName,
                FtpSettings = new FtpSettingsModel(),
                RemoteFileStorage = new RemoteFileStorageModel()
            };
            Save(userConfig);
            return userConfig;
        }

        /// <inheritdoc />
        public UserConfigResponse Save(UserConfig userConfig, ConfigItem[] configItems)
        {
            var step = Enum.Parse(typeof(ConfigStagesEnum), userConfig.StepName);
            switch (step)
            {
                case ConfigStagesEnum.AddFtpSettings:
                    ReadFtpSettings(userConfig, configItems);
                    HelperAdapter.CanConnectToFtp(userConfig.FtpSettings);
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
                    break;
            }

            Save(userConfig);
            return _configStages.StageResponse(userConfig);
        }

        private void ReadFoldersNames(UserConfig userConfig, ConfigItem[] configItems)
        {
            userConfig.RemoteFileStorage.OrdersPath = configItems.FirstOrDefault(i => i.ConfigItemId == "Orders");
            userConfig.RemoteFileStorage.DispatchesPath = configItems.FirstOrDefault(i => i.ConfigItemId == "Dispatches");
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
            var output = Newtonsoft.Json.JsonConvert.SerializeObject(userConfig);

            _fileRepository.Save(userConfig.AuthorizationToken, output);
        }
    }
}