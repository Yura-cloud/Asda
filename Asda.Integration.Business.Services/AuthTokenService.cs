using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Interfaces;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using LinnworksAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<AuthTokenService> _logger;

        private readonly IRepository _tokenRepository;

        public AuthTokenService(IConfiguration configuration, ILogger<AuthTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _tokenRepository = new FileRepository(configuration["AppSettings:UserTokenLocation"]);
        }

        public HttpStatusCodeResult SaveUserTokenInfo(string token)
        {
            try
            {
                var session = GetSession(token);
                if (session.UserId == Guid.Empty)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var tokenModel = new TokenModel
                {
                    Token = new Guid(token),
                    Email = session.Email,
                    UserId = session.UserId
                };
                var tokenConfigJson = Newtonsoft.Json.JsonConvert.SerializeObject(tokenModel);
                if (!Directory.Exists(_configuration["AppSettings:UserTokenLocation"]))
                {
                    Directory.CreateDirectory(_configuration["AppSettings:UserTokenLocation"]);
                }

                _tokenRepository.Save(tokenModel.UserId.ToString("N"), tokenConfigJson);
            }
            catch (Exception e)
            {
                _logger.LogError($"userToken:{token}; Error, while using GetUsersInfo, with message {e.Message}");
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
                _logger.LogError($"userToken:{token}; Failed while GetSession, with message {e.Message}");
            }

            return new BaseSession();
        }
    }
}