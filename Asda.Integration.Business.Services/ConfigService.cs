using System;
using Asda.Integration.Business.Services.Adapters;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Interfaces;
using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Payment;
using Asda.Integration.Domain.Models.Shipping;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<ConfigService> _logger;

        private readonly IConfigStages _configStages;

        private readonly IRepository _fileRepository;

        public ConfigService(IUserConfigAdapter userConfigAdapter, ILogger<ConfigService> logger,
            IConfigStages configStages, IConfiguration configuration)
        {
            _userConfigAdapter = userConfigAdapter;
            _logger = logger;
            _configStages = configStages;
            _fileRepository = new FileRepository(configuration["AppSettings:UserTokenLocation"]);
        }

        public AddNewUserResponse UpdateUserInfo(AddNewUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return new AddNewUserResponse {Error = "Invalid Email"};

            if (string.IsNullOrWhiteSpace(request.AccountName))
                return new AddNewUserResponse {Error = "Invalid AccountName"};

            if (request.LinnworksUniqueIdentifier == Guid.Empty)
                return new AddNewUserResponse {Error = "Invalid LinnworksUniqueIdentifier"};

            try
            {
                var file = _fileRepository.Load(request.LinnworksUniqueIdentifier.ToString("N"));
                var tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(file);
                var userConfig = _userConfigAdapter.CreateNew(request.Email, request.LinnworksUniqueIdentifier,
                    request.AccountName, tokenModel.Token);

                return new AddNewUserResponse {AuthorizationToken = userConfig.AuthorizationToken};
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"LinnworksUniqueIdentifier:{request.LinnworksUniqueIdentifier}; Failed while UpdateUserInfo with message {e.Message}");
                return new AddNewUserResponse {Error = e.Message};
            }
        }

        public BaseResponse ConfigDeleted(BaseRequest request)
        {
            try
            {
                _userConfigAdapter.Delete(request.AuthorizationToken);

                return new BaseResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken:{request.AuthorizationToken}; Failed while using ConfigDeleted with message {e.Message}");
                return new BaseResponse {Error = e.Message};
            }
        }

        public BaseResponse ConfigTest(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

                if (user == null)
                {
                    var message = "User not found";
                    _logger.LogError(
                        $"AuthorizationToken: {request.AuthorizationToken}; Failed while using ConfigTest with message: {message}");
                    return new BaseResponse {Error = message};
                }

                HelperAdapter.TestFtpConnection(user.FtpSettings);
                if (!HelperAdapter.CheckExistingFolders(user.FtpSettings, user.RemoteFileStorage, out var errorMessage))
                {
                    _logger.LogError(
                        $"AuthorizationToken: {request.AuthorizationToken}; Failed while using ConfigTest with message: {errorMessage}");
                    return new BaseResponse {Error = errorMessage};
                }

                return new BaseResponse {Error = null};
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken: {request.AuthorizationToken}; Failed while using ConfigTest with message {e.Message}");
                return new BaseResponse {Error = e.Message};
            }
        }

        public PaymentTagResponse PaymentTags(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var message = "User not found";
                    _logger.LogError(
                        $"AuthorizationToken: {request.AuthorizationToken}; Failed while using PaymentTags with message: {message}");
                    return new PaymentTagResponse {Error = message};
                }

                return new PaymentTagResponse
                {
                    PaymentTags = new[]
                    {
                        new PaymentTag {FriendlyName = "PayPal", Site = "", Tag = "paypal_verified"},
                        new PaymentTag {FriendlyName = "Credit Card - Master Card", Site = "", Tag = "mastercard"},
                        new PaymentTag {FriendlyName = "Credit Card - Visa", Site = "", Tag = "visa_credit"},
                        new PaymentTag {FriendlyName = "Credit Card - Unknown", Site = "", Tag = "credit_unknown"},
                        new PaymentTag {FriendlyName = "Bank payments", Site = "", Tag = "bank"},
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken: {request.AuthorizationToken}; Failed while using PaymentTags with message {e.Message}");
                return new PaymentTagResponse {Error = e.Message};
            }
        }

        public ShippingTagResponse ShippingTags(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var message = "User not found";
                    _logger.LogError(
                        $"AuthorizationToken: {request.AuthorizationToken}; Failed while using ShippingTags with message: {message}");
                    return new ShippingTagResponse {Error = message};
                }

                return new ShippingTagResponse
                {
                    ShippingTags = new[]
                    {
                        new ShippingTag {FriendlyName = "Royal Mail First Class", Site = "", Tag = "RM CLR01"},
                        new ShippingTag
                            {FriendlyName = "Royal Mail Special Delivery", Site = "", Tag = "RM_SpecialDelivery_9am"},
                        new ShippingTag {FriendlyName = "DPD - Next Day", Site = "", Tag = "dpd"},
                        new ShippingTag {FriendlyName = "Fedex - Ground", Site = "", Tag = "fedex_ground"},
                        new ShippingTag {FriendlyName = "Some other service", Site = "", Tag = "matrix_rate_10221"},
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken: {request.AuthorizationToken}; Failed while using ShippingTags with message: {e.Message}");
                return new ShippingTagResponse {Error = e.Message};
            }
        }

        public UserConfigResponse UserConfig(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    var message = "User config is at invalid stage";
                    _logger.LogError(
                        $"AuthorizationToken: {request.AuthorizationToken}; Failed while using UserConfig with message: {message}");
                    return new UserConfigResponse {Error = message};
                }

                return _configStages.StageResponse(user);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken: {request.AuthorizationToken}; Failed while using UserConfig with message: {e.Message}");
                return new UserConfigResponse {Error = e.Message};
            }
        }

        public UserConfigResponse SaveConfig(SaveUserConfigRequest request)
        {
            try
            {
                var userConfig = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

                if (request.StepName != userConfig.StepName)
                {
                    var message = $"Invalid step name expected {userConfig.StepName}";
                    _logger.LogError($"AuthorizationToken: {request.AuthorizationToken}; Error: {message}");
                    return new UserConfigResponse {Error = message};
                }

                _userConfigAdapter.FillUserConfig(userConfig, request.ConfigItems);
                _userConfigAdapter.Save(userConfig);
                return _configStages.StageResponse(userConfig);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"AuthorizationToken: {request.AuthorizationToken}; Failed while using SaveConfigSave with message: {e.Message}");
                return new UserConfigResponse {Error = e.Message};
            }
        }
    }
}