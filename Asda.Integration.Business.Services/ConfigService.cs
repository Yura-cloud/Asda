using System;
using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Payment;
using Asda.Integration.Domain.Models.Shipping;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asda.Integration.Business.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly ILogger<ConfigService> _logger;

        private readonly IConfigStages _configStages;

        public ConfigService(IUserConfigAdapter userConfigAdapter, ILogger<ConfigService> logger,
            IConfigStages configStages)
        {
            _userConfigAdapter = userConfigAdapter;
            _logger = logger;
            _configStages = configStages;
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
                var userConfig = _userConfigAdapter.LoadByUserId(request.LinnworksUniqueIdentifier);
                userConfig.AccountName = request.AccountName;
                _userConfigAdapter.Save(userConfig);

                return new AddNewUserResponse {AuthorizationToken = userConfig.AuthorizationToken};
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed while AddNewUserResponse, with message {e.Message}");
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
            catch (Exception ex)
            {
                _logger.LogError($"Failed while using ConfigDeleted action, with message {ex.Message}");
                return new BaseResponse {Error = ex.Message};
            }
        }

        public BaseResponse ConfigTest(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

                if (user == null)
                {
                    return new BaseResponse {Error = "User not found"};
                }

                return new BaseResponse() {Error = null};
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed while using ConfigTest action, with message {ex.Message}");
                return new BaseResponse {Error = ex.Message};
            }
        }

        public PaymentTagResponse PaymentTags(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    return new PaymentTagResponse {Error = "User not found"};
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
            catch (Exception ex)
            {
                return new PaymentTagResponse {Error = ex.Message};
            }
        }

        public ShippingTagResponse ShippingTags(BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);
                if (user == null)
                {
                    return new ShippingTagResponse {Error = "User not found"};
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
            catch (Exception ex)
            {
                return new ShippingTagResponse {Error = ex.Message};
            }
        }

        public UserConfigResponse UserConfig(BaseRequest request)
        {
            try
            {
                var userConfig = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

                return _configStages.StageResponse(userConfig, "User config is at invalid stage");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed while using UserConfig action, with message {ex.Message}");
                return new UserConfigResponse {Error = ex.Message};
            }
        }

        public UserConfigResponse SaveConfigSave(SaveUserConfigRequest request)
        {
            try
            {
                var userConfig = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

                if (request.StepName != userConfig.StepName)
                    return new UserConfigResponse
                        {Error = string.Format("Invalid step name expected {0}", userConfig.StepName)};

                return _userConfigAdapter.Save(userConfig, request.ConfigItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed while using SaveConfigSave action, with message {ex.Message}");
                return new UserConfigResponse {Error = ex.Message};
            }
        }
    }
}