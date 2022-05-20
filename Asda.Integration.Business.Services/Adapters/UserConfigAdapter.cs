using System;
using System.Linq;
using Asda.Integration.Business.Services.Config;
using Asda.Integration.Domain.Interfaces;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Asda.Integration.Business.Services.Adapters
{
    public class UserConfigAdapter : IUserConfigAdapter
    {
        private readonly IRepository _fileRepository;

        private readonly IConfigStages _configStages;

        private readonly ILogger<UserConfigAdapter> _logger;

        public UserConfigAdapter(IOptions<AppSettings> config, IConfigStages configStages,
            ILogger<UserConfigAdapter> logger)
        {
            _fileRepository = config.Value.FileRepository;
            _configStages = configStages;
            _logger = logger;
        }

        /// <inheritdoc />
        public UserConfig LoadByToken(string authorizationToken)
        {
            if (string.IsNullOrWhiteSpace(authorizationToken))
                throw new ArgumentNullException("authorizationToken");

            if (_fileRepository.FileExists(authorizationToken))
            {
                string json = _fileRepository.LoadByToken(authorizationToken);
                var userConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConfig>(json);
                return userConfig;
            }
            else
            {
                throw new Exception("User not found");
            }
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
                AccountName = accountName
            };
            this.Save(userConfig);
            return userConfig;
        }

        /// <inheritdoc />
        public UserConfigResponse Save(UserConfig userConfig, ConfigItem[] configItems)
        {
            var step = Enum.Parse(typeof(ConfigStagesEnum), userConfig.StepName);
            switch (step)
            {
                case ConfigStagesEnum.AddCredentials:
                    userConfig.StepName = ConfigStagesEnum.OrderSetup.ToString();
                    break;
                case ConfigStagesEnum.OrderSetup:
                    userConfig.StepName = ConfigStagesEnum.UserConfig.ToString();
                    break;
                case ConfigStagesEnum.UserConfig:
                    userConfig.Location = configItems.FirstOrDefault(i => i.ConfigItemId == "Location");
                    break;
            }

            Save(userConfig);

            return _configStages.StageResponse(userConfig);
        }

        public void Save(UserConfig userConfig)
        {
            var output = Newtonsoft.Json.JsonConvert.SerializeObject(userConfig);

            this._fileRepository.Save(userConfig.AuthorizationToken, output);
        }
    }
}