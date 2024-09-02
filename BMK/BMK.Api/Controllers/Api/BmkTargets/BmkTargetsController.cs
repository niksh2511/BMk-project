using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RxWeb.Core.Security;


namespace BMK.Api.Controllers.Api.BmkTargets
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BmkTargetsController : ControllerBase
    {
        private IUserUow UserUow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IBmkTargetsDomain TargetDomain { get; set; }
        public BmkTargetsController(IUserUow userUow, IUserClaim userClaim, IBmkTargetsDomain targetDomain)
        {
            UserUow = userUow;
            TargetDomain = targetDomain;
            UserClaim = userClaim;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await UserUow.Repository<BmkTarget>().Queryable().Where(r => r.Active == true).ToListAsync();

            return Ok(res);

        }
        [HttpGet("getAccountCategory")]
        public async Task<IActionResult> getAccountCategory()
        {
            var res = await UserUow.Repository<QbMapAccountCategory>().Queryable().Where(r => r.Active == true && r.IsDept == true).OrderBy(r=>r.CategoryName).ToListAsync();
            return Ok(res);
        }

        [HttpGet("GetBmkTargetReportData/{organizationID}/{year}/{deptId}")]
        public async Task<IActionResult> GetBmkTargetReportData(int organizationID, int year, int deptId)
        {
            var result = await TargetDomain.GetReportData(organizationID, year, deptId);
            return Ok(result);

        }
        [HttpPost("SaveTargets")]
        public async Task<IActionResult> SaveTargets(List<BmkTarget> model)
        {

            var result = await TargetDomain.SaveTargets(model);
            return Ok(result);
        }
    }
}
