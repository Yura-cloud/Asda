using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Service.Intefaces
{
    public interface IAuthTokenService
    {
        void GetUsersInfo(string token);
    }
}