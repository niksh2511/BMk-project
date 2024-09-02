using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BMK.Api.Controllers.Api.Lookups
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationLookupController : ControllerBase
    {
        public IUserUow Uow;
        public OrganizationLookupController(IOrganizationDoomain organization, IUserUow uow)
        {
            Uow = uow;
        }

        [HttpGet]
        [Route("organizationinfo")]
        public async Task<IActionResult> GetOrganizationInfo()
        {
            var organizationInfo = await Uow.Repository<AppObject>().Queryable().ToListAsync();
            var lookupData = new
            {
                PSA = organizationInfo.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "PSA"),
                RMM = organizationInfo.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "RMM"),
                SalesPayroll = organizationInfo.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "Sales Payroll Tracking"),
                AdminPayroll = organizationInfo.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "Admin Payroll Tracking")
            };


            return Ok(lookupData);
        }

    }
}
