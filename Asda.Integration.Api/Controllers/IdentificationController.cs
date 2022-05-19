using System;
using Asda.Integration.Service.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentificationController : ControllerBase
    {
        private readonly IAuthTokenService _authTokenService;

        public IdentificationController(IAuthTokenService authTokenService)
        {
            _authTokenService = authTokenService;
        }

        [HttpGet]
        public IActionResult AuthToken(string token)
        {
            _authTokenService.GetUsersInfo(token);


            return Ok();
        }
    }
}