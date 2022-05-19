using System.Web.Mvc;

namespace Asda.Integration.Service.Intefaces
{
    public interface IAuthTokenService
    {
        HttpStatusCodeResult GetUsersInfo(string token);
    }
}