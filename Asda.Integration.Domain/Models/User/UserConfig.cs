using System;
using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Domain.Models.User
{
    public class UserConfig : BaseRequest
    {
        public UserConfig()
        {
            IsOauth = true;
            StepName = ConfigStagesEnum.AddFtpSettings.ToString();
        }

        public UserConfig(string authorizationToken)
        {
            AuthorizationToken = authorizationToken;
        }
        public Guid AppToken { get; set; }
        public string Email { get; set; }
        public Guid LinnworksUniqueIdentifier { get; set; }

        public bool IsComplete { get; set; }
        public string StepName { get; set; }
        public string AccountName { get; set; }

        public bool IsConfigActive { get; set; }
        public string Location { get; set; }
        public bool IsOauth { get; set; }
        public FtpSettingsModel FtpSettings { get; set; }
        public RemoteFileStorageModel RemoteFileStorage { get; set; }
    }
}