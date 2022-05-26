using System;
using System.Net;
using System.Web.Mvc;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using LinnworksAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IConfiguration _configuration;

        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<AuthTokenService> _logger;

        public AuthTokenService(IConfiguration configuration, ILogger<AuthTokenService> logger,
            IUserConfigAdapter userConfigAdapter)
        {
            _configuration = configuration;
            _logger = logger;
            _userConfigAdapter = userConfigAdapter;
        }

        public HttpStatusCodeResult SaveUserProfile(string token)
        {
            try
            {
                var session = GetSession(token);
                if (session.UserId == Guid.Empty)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                _userConfigAdapter.CreateNew(session.Email, session.UserId, session.UserName, new Guid(token));
            }
            catch (Exception e)
            {
                _logger.LogError($"Erorr, while using GetUsersInfo, with message {e.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private BaseSession GetSession(string token)
        {
            try
            {
                var authController = new AuthController(new ApiContext("https://api.linnworks.net"));
                var authorizeRequest = new AuthorizeByApplicationRequest
                {
                    ApplicationId = new Guid(_configuration["AuthorizationKeys:applicationId"]),
                    ApplicationSecret = new Guid(_configuration["AuthorizationKeys:secretKey"]),
                    Token = new Guid(token)
                };
                return authController.AuthorizeByApplication(authorizeRequest);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed while GetSession, with message {e.Message}");
            }

            return new BaseSession();
        }
    }
}