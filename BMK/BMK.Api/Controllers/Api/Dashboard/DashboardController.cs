using BMK.Domain.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BMK.Api.Controllers.Api.Dashboard
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IPowerBIDomain _powerBIDomain;

        public DashboardController(IPowerBIDomain powerBIDomain)
        {
            _powerBIDomain = powerBIDomain;
        }

        [HttpGet("getEmbedToken")]
        public async Task<IActionResult> GetEmbedToken()
        {
            var response = await _powerBIDomain.GetEmbedToken();
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError, response);
        }
    }
}
