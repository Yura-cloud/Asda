using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Payment;
using Asda.Integration.Domain.Models.Shipping;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Microsoft.AspNetCore.Mvc;


namespace Asda.Integration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpPost("AddNewUser")]
        public AddNewUserResponse AddNewUser([FromBody] AddNewUserRequest request)
        {
            return _configService.AddNewUser(request);
        }

        [HttpPost("ConfigDeleted")]
        public BaseResponse ConfigDeleted([FromBody] BaseRequest request)
        {
            return _configService.ConfigDeleted(request);
        }

        [HttpPost("ConfigTest")]
        public BaseResponse ConfigTest([FromBody] BaseRequest request)
        {
            return _configService.ConfigTest(request);
        }

        [HttpPost("PaymentTags")]
        public PaymentTagResponse PaymentTags([FromBody] BaseRequest request)
        {
            return _configService.PaymentTags(request);
        }

        [HttpPost("ShippingTags")]
        public ShippingTagResponse ShippingTags([FromBody] BaseRequest request)
        {
            return _configService.ShippingTags(request);
        }

        [HttpPost("UserConfig")]
        public UserConfigResponse UserConfig([FromBody] BaseRequest request)
        {
            return _configService.UserConfig(request);
        }

        [HttpPost("SaveConfig")]
        public UserConfigResponse SaveConfig([FromBody] SaveUserConfigRequest request)
        {
            return _configService.SaveConfig(request);
        }
    }
}