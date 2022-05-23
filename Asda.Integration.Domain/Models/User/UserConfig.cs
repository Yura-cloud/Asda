using System;

namespace Asda.Integration.Domain.Models.User
{
    public class UserConfig : BaseRequest
    {
        public UserConfig()
        {
            IsOauth = true;
            StepName = ConfigStagesEnum.AddCredentials.ToString();
        }

        public UserConfig(string authorizationToken)
        {
            AuthorizationToken = authorizationToken;
        }

        public string Email { get; set; }

        public Guid LinnworksUniqueIdentifier { get; set; }

        public bool IsComplete { get; set; }
        public string StepName { get; set; }
        public string AccountName { get; set; }
        
        public bool IsConfigActive { get; set; }
        public string Location { get; set; }

        public bool IsOauth { get; set; }

        public bool PriceIncTax { get; set; }
    }
}