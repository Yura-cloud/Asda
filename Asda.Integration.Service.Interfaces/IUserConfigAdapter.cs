using System;
using Asda.Integration.Domain.Models.User;


namespace Asda.Integration.Service.Interfaces
{
    public interface IUserConfigAdapter
    {
        /// <summary>
        /// Load a user configuration.
        /// </summary>
        /// <param name="authorizationToken">The authorization token for the user account.</param>
        /// <returns>The user configuration.</returns>
        public UserConfig LoadByToken(string authorizationToken);

        /// <summary>
        /// Delete a user configuration.
        /// </summary>
        /// <param name="authorizationToken">The authorization token for the user account.</param>
        public void Delete(string authorizationToken);

        /// <summary>
        /// Create a new user configuration.
        /// </summary>
        /// <param name="email">The email address for the account.</param>
        /// <param name="linnworksUniqueIdentifier">The unique identifier assigned by linnworks.</param>
        /// <param name="accountName">The name of the account.</param>
        /// <param name="appToken"></param>
        /// <returns>The newly created user configuration.</returns>
        public UserConfig CreateNew(string email, Guid linnworksUniqueIdentifier, string accountName, Guid appToken);

        /// <summary>
        /// Save configuration items in the user config.
        /// </summary>
        /// <param name="userConfig">The user config.</param>
        /// <param name="configItems"></param>
        /// <returns>The result of the action.</returns>
        public void FillUserConfig(UserConfig userConfig, ConfigItem[] configItems);

        void Save(UserConfig userConfig);
    }
}
