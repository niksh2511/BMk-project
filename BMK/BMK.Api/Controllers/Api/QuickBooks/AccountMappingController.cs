using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BMK.Api.Controllers.Api.QuickBooks
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMappingController : ControllerBase
    {
        private IAccountMappingDomain AccountMappingDomain { get; set; }
        public AccountMappingController(IAccountMappingDomain accountMappingDomain)
        {
            AccountMappingDomain = accountMappingDomain;
        }

        [HttpGet("getAccountMappingType")]
        public async Task<IActionResult> GetAccountMappingType()
        {
            return Ok(await AccountMappingDomain.GetAccountMappingType());
        }

        [HttpGet("getAccountMapping")]
        public async Task<IActionResult> GetAccountMapping()
        {
            return Ok(await AccountMappingDomain.GetAccountMapping());
        }

        [HttpPost("saveAccountMapping")]
        public async Task<IActionResult> SaveAccountMapping(List<QbOrgAccountMapping> accountMappingList)
        {

            if (accountMappingList.Count() <= 0)
            {
                return BadRequest("Make some changes in account mapping to save mapping");
            }
            Response<List<AccountMapping>> response = await AccountMappingDomain.SaveAccountMapping(accountMappingList);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpPost("reProcessImportData")]
        public async Task<IActionResult> ReProcessImportData()
        {
            Response<object> response = await AccountMappingDomain.ReProcessImportData();
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("retriveUnMapAccount")]
        public async Task<IActionResult> RetriveUnMapAccount()
        {
            int response = await AccountMappingDomain.RetriveUnMapAccount();
            return Ok(response);
        }
    }
}
