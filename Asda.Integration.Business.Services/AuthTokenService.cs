using System;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using LinnworksAPI;
using LinnworksMacroHelpers;
using LinnworksMacroHelpers.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private LinnworksMacroBase LinnWorks { get; }

        private readonly IConfiguration _configuration;

        private readonly ILogger<AuthTokenService> _logger;

        private readonly IUserConfigAdapter _userConfigAdapter;

        public AuthTokenService(IConfiguration configuration, ILogger<AuthTokenService> logger,
            IUserConfigAdapter userConfigAdapter)
        {
            _configuration = configuration;
            _logger = logger;
            _userConfigAdapter = userConfigAdapter;
            LinnWorks = new LinnworksMacroBase();
        }


        public void GetUsersInfo(string token)
        {
            try
            {
                _logger.LogInformation($"This is Token => {token}");
                
                _logger.LogInformation("Iniasializing Linnworks");
                LinnWorks.Api = InitializeHelper.GetApiManagerForPullOrders(_configuration, token);
                    //LinnWorks.Api.api

                _logger.LogInformation("Get Stock");
                var res = LinnWorks.Api.Inventory.GetStockLocations();
                
                _logger.LogInformation("Get count of Stock");
                _logger.LogError(res.Count.ToString());

                var authorizeRequest = new AuthorizeByApplicationRequest
                {
                    ApplicationId = new Guid(_configuration["AuthorizationKeys:applicationId"]),
                    ApplicationSecret = new Guid(_configuration["AuthorizationKeys:secretKey"]),
                    Token = new Guid(token)
                };
                _logger.LogInformation($"GetSession");
                var session = LinnWorks.Api.Auth.AuthorizeByApplication(authorizeRequest);
                
                _logger.LogInformation($"Info from session {session.Email}");
                
                _logger.LogError(session?.Email);
               
            }
            catch (Exception e)
            {
                _logger.LogInformation("Was An Error!");
                _logger.LogError(e.Message);
            }
        }
    }
}