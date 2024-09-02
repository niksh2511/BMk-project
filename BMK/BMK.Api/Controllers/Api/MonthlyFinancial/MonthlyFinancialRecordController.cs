using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BMK.Api.Controllers.Api.MonthlyFinancial
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyFinancialRecordController : ControllerBase
    {
        private IMonthlyFinancialRecordDomain MonthlyFinancialRecordDomain { get; set; }
        public MonthlyFinancialRecordController(IMonthlyFinancialRecordDomain monthlyFinancialRecordDomain)
        {
            MonthlyFinancialRecordDomain = monthlyFinancialRecordDomain;
        }

        [HttpGet("retrieve")]
        public async Task<IActionResult> Retrieve([FromQuery] int month, [FromQuery] int year)
        {
            if (month <= 0 || year <= 0)
            {
                return BadRequest("month and year not found when retrieve monthly financial record");
            }
            Response<MonthlyFinancialRecord> response = await MonthlyFinancialRecordDomain.Retrieve(month, year);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("getMapMasterFinancialType")]
        public async Task<IActionResult> GetMapMasterFinancialType()
        {
            return Ok(await MonthlyFinancialRecordDomain.GetMapMasterFinancialType());
        }

        [HttpGet("getManualEntryCalendar")]
        public async Task<IActionResult> GetManualEntryCalendar()
        {
            Response<List<ManualEntryCalendar>> response = await MonthlyFinancialRecordDomain.GetManualEntryCalendar();
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError, response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(MonthlyFinancialRecord monthlyFinancialRecord)
        {
            Response<object> response = await MonthlyFinancialRecordDomain.Add(monthlyFinancialRecord);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }
    }
}
