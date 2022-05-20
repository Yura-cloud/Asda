using System.Web.Mvc;
using Asda.Integration.Service.Intefaces;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace Asda.Integration.Api.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]/[action]")]
    public class IdentificationController : ControllerBase
    {
        private readonly IAuthTokenService _authTokenService;

        public IdentificationController(IAuthTokenService authTokenService)
        {
            _authTokenService = authTokenService;
        }

        [HttpGet]
        public HttpStatusCodeResult AuthToken(string token)
        {
           return _authTokenService.GetUsersInfo(token);
        }
    }
}