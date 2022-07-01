using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.Payment;
using Asda.Integration.Domain.Models.Shipping;
using Asda.Integration.Domain.Models.User;

namespace Asda.Integration.Service.Intefaces
{
    public interface IConfigService
    {
        AddNewUserResponse UpdateUserInfo(AddNewUserRequest request);
        BaseResponse ConfigDeleted(BaseRequest request);
        BaseResponse ConfigTest(BaseRequest request);
        PaymentTagResponse PaymentTags(BaseRequest request);
        ShippingTagResponse ShippingTags(BaseRequest request);
        UserConfigResponse UserConfig(BaseRequest request);
        UserConfigResponse SaveConfig(SaveUserConfigRequest request);
    }
}