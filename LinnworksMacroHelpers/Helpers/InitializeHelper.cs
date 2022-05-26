using LinnworksAPI;
using System;
using Microsoft.Extensions.Configuration;

namespace LinnworksMacroHelpers.Helpers
{
    public static class InitializeHelper
    {
        public static ApiObjectManager GetApiManagerForPullOrders(IConfiguration configuration, string token)
        {
            var auth = Authorize(new Guid(configuration["AuthorizationKeys:applicationId"]),
                new Guid(configuration["AuthorizationKeys:secretKey"]), new Guid(token));
            var context = new ApiContext(auth.Token, auth.Server);

            return new ApiObjectManager(context);
        }

        public static ApiObjectManager GetApiManagerForCanceledOrders(IConfiguration configuration, string token)
        {
            var auth = Authorize(new Guid(configuration["AuthorizationKeysCanceledOrders:applicationId"]),
                new Guid(configuration["AuthorizationKeysCanceledOrders:secretKey"]), new Guid(token));

            var context = new ApiContext(auth.Token, auth.Server);

            return new ApiObjectManager(context);
        }

        private static BaseSession Authorize(Guid applicationId, Guid secretKey, Guid token)
        {
            var controller = new AuthController(new ApiContext("https://api.linnworks.net"));

            return controller.AuthorizeByApplication(new AuthorizeByApplicationRequest
            {
                ApplicationId = applicationId,
                ApplicationSecret = secretKey,
                Token = token
            });
        }
    }
}