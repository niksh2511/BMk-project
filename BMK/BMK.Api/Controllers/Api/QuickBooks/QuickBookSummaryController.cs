using BMK.Domain.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BMK.Api.Controllers.Api.QuickBooks
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuickBookSummaryController : ControllerBase
    {
        private IQuickBookSummaryDomain QuickBookSummaryDomain { get; set; }
        public QuickBookSummaryController(IQuickBookSummaryDomain quickBookSummaryDomain)
        {
            QuickBookSummaryDomain = quickBookSummaryDomain;
        }

        [HttpGet("retrieveQBSummary")]
        public async Task<IActionResult> RetrieveQBSummary()
        {
            var result = await QuickBookSummaryDomain.RetrieveQBSummary();
            return StatusCode(result.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, result);
        }

        [HttpGet("retrieveOrganizationQBSummary/{OrganizationId}")]
        public async Task<IActionResult> RetrieveQBSummary(int OrganizationId)
        {
            var result = await QuickBookSummaryDomain.RetrieveOrganizationQBSummary(OrganizationId);
            return StatusCode(result.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, result);
        }
    }
}
