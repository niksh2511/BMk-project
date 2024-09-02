using BMK.Domain.Domain;
using BMK.Infrastructure.Singleton;
using BMK.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxWeb.Core.Security;
using System.Net;
using System.Text;

namespace BMK.Api.Controllers.Api.QuickBooks
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuickBooksController : ControllerBase
    {
        private readonly IConfiguration Config;
        private readonly IUserClaim UserClaim;
        //private readonly HttpClient _httpClient;
        private IQuickBookDomain QuickBookDomain { get; set; }
        private ISessionProvider SessionProvider { get; set; }
        public QuickBooksController(IConfiguration config, IUserClaim userClaim, IQuickBookDomain quickBookDomain, ISessionProvider serviceProvider)
        {
            Config = config;
            QuickBookDomain = quickBookDomain;
            SessionProvider = serviceProvider;
            UserClaim = userClaim;
        }

        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            return Ok(await QuickBookDomain.Connect());
        }

        [HttpGet("status")]
        public async Task<IActionResult> QbStatus()
        {
            return Ok(await QuickBookDomain.CheckQbStatus());
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, [FromQuery] string realmId)
        {

            var res = await QuickBookDomain.Callback(code, state, realmId, HttpContext.Request.Path.Value);
            return Redirect(Convert.ToString(Config["QuickBooksOAuth:RedirectUrl"]) + res.Message);
        }

        [HttpGet("retrieveQbData")]
        public async Task<IActionResult> RetrieveQbData()
        {
            Response<object> qbResponse = await QuickBookDomain.RetrieveQbData();
            return StatusCode(qbResponse.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, qbResponse.Message);
        }

        [HttpPost("downloadQwcFile")]
        public async Task<IActionResult> DownloadQwcFile()
        {

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "QBWC", "BMKCommunity.qwc");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Error Occur During Downloading Quickbooks Web Connector definition file.");
            }

            string fileContent;
            using (var reader = new StreamReader(filePath))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            // Update the username in the file content
            fileContent = fileContent.Replace("{userName}", UserClaim.Email);
            fileContent = fileContent.Replace("{ownerId}", Guid.NewGuid().ToString("B"));
            fileContent = fileContent.Replace("{fileId}", Guid.NewGuid().ToString("B"));
            fileContent = fileContent.Replace("{appUrl}", Convert.ToString(Config["AppUrl"]));
            fileContent = fileContent.Replace("{qbAppUrl}", Convert.ToString(Config["QuickBooksOAuth:QBAppUrl"]));

            var updatedFileBytes = Encoding.UTF8.GetBytes(fileContent);

            var memory = new MemoryStream(updatedFileBytes);

            return File(memory, "application/octet-stream", "BMKCommunity.qwc");
        }

        [HttpDelete("deleteAllFinancialData")]
        public async Task<IActionResult> DeleteAllFinancialData([FromQuery]string deleteType)
        {
            Response<object> qbResponse = await QuickBookDomain.DeleteAllFinancialData(deleteType);
            return StatusCode(qbResponse.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, qbResponse);

        }
    }

}
