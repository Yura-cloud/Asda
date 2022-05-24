using System;
using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Payment;
using Asda.Integration.Domain.Models.Shipping;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Asda.Integration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IUserConfigAdapter _userConfigAdapter;

        private readonly IConfigStages _configStages;

        private readonly ILogger<ConfigController> _logger;

        public ConfigController(IUserConfigAdapter userConfigAdapter, IConfigStages configStages,
            ILogger<ConfigController> logger)
        {
            _userConfigAdapter = userConfigAdapter;
            _configStages = configStages;
            _logger = logger;
        }

        [HttpPost("AddNewUser")]
        public AddNewUserResponse UpdateUserInfo([FromBody] AddNewUserRequest request)
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

        [HttpPost("ConfigDeleted")]
        public BaseResponse ConfigDeleted([FromBody] BaseRequest request)
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

        [HttpPost("ConfigTest")]
        public BaseResponse ConfigTest([FromBody] BaseRequest request)
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


        /// <summary>
        /// This call is expected to return an array of shipping methods friendly names and their tags
        /// to generate a pre-populated list in the config shipping mapping screen.
        /// </summary>
        /// <param name="request"><see cref="BaseRequest"/></param>
        /// <returns><see cref="PaymentTagResponse"/></returns>
        [HttpPost("PaymentTags")]
        public PaymentTagResponse PaymentTags([FromBody] BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

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

        /// <summary>
        /// This call is expected to return an array of shipping methods friendly names and their tags
        /// to generate a pre-populated list in the config shipping mapping screen.
        /// </summary>
        /// <param name="request"><see cref="BaseRequest"/></param>
        /// <returns><see cref="ShippingTagResponse"/></returns>
        [HttpPost("ShippingTags")]
        public ShippingTagResponse ShippingTags([FromBody] BaseRequest request)
        {
            try
            {
                var user = _userConfigAdapter.LoadByToken(request.AuthorizationToken);

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

        /// <summary>
        /// This request is made in two situations:
        /// 
        /// Firstly when a customer is going through the integration wizard to integrate the channel.
        /// To complete the wizard returns "UserConfig" as the step name and this will indicate the
        /// wizard is complete.
        /// 
        /// The second instance is when the config is loaded the call is made to load any dynamic
        /// ConfigItems that may be required to show on the Linnworks config UI.SaveConfigEndpoint
        /// will be called on each wizard step and when the config is saved.
        /// 
        /// If the config is loaded and the StepName is not "UserConfig" it will load the config
        /// wizard and take them through the stages until "UserConfig" is returned.This can be
        /// especially useful if the user is required to go through additional steps down the line.
        /// </summary>
        /// <param name="request"><see cref="BaseRequest"/></param>
        /// <returns><see cref="UserConfigResponse"/></returns>
        [HttpPost("UserConfig")]
        public UserConfigResponse UserConfig([FromBody] BaseRequest request)
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

        /// <summary>
        /// This request is made in two situations:
        /// 
        /// At the end of every config wizard step as a customer enters / edits the fields and on the
        /// config screen if custom config items are supplied back when the step name is "UserConfig".
        /// 
        /// Linnworks will provide the entire object that was provided back with the only field ever
        /// changing being the SelectedValue.This is passed back cast as string as fields may be of
        /// many different types.
        /// </summary>
        /// <param name="request"><see cref="SaveUserConfigRequest"/></param>
        /// <returns><see cref="UserConfigResponse"/></returns>
        [HttpPost("SaveConfigSave")]
        public UserConfigResponse SaveConfigSave([FromBody] SaveUserConfigRequest request)
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