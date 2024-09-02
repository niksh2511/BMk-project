using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RxWeb.Core.Security;

using System.Net;

namespace BMK.Api.Controllers.Api.OrganizationModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationSalaryController : ControllerBase
    {
        private IOrganizationSalaryDomain OrganizationSalaryDomain { get; set; }
        public IUserUow UserUow { get; set; }

        public OrganizationSalaryController(IUserUow userUow, IUserClaim userClaim, IOrganizationSalaryDomain organizationSalaryDomain)
        {
            OrganizationSalaryDomain = organizationSalaryDomain;
            UserUow = userUow;
        }

        [HttpGet("GetOrganizationSalary/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await OrganizationSalaryDomain.GetOrganizationSalary(id);
            return Ok(result);
        }
        [HttpGet("OrganizationSalarybyId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await OrganizationSalaryDomain.OrganizationSalaryById(id);
            return Ok(result);
        }

        [HttpPost("AddOrganizationSalary")]
        public async Task<IActionResult> SaveOrganizationSalary(OrganizationSalary model)
        { 
            Response<object> response = await OrganizationSalaryDomain.SaveOrganizationSalary(model);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpPost("DeleteOrganizationSalary/{id}")]
        public async Task<IActionResult> DeleteOrganizationSalary(int id, [FromBody] JsonPatchDocument Query)
        {
            Response<object> response = await OrganizationSalaryDomain.DeleteOrganizationSalary(id, Query);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpPost("UpdateOrganizationSalary/{id}")]
        public async Task<IActionResult> UpdateOrganizationSalary(int id,OrganizationSalary model)
        {
            model.OrganizationSalaryId = id;
            Response<object> response = await OrganizationSalaryDomain.UpdateOrganizationSalary(id,model);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpGet]
        [Route("SalaryAppObjects")]
        public async Task<IActionResult> GetOrganizationInfo()
        {
            var SalaryAppObjects = await UserUow.Repository<AppObject>().Queryable().ToListAsync();
            var SalarylookupData = new
            {
                salaryDepartment = SalaryAppObjects.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "SalaryDepartment"),
                salaryRole = SalaryAppObjects.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "SalaryRole"),
                salaryFTE = SalaryAppObjects.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "SalaryFTE"),
                salaryLevel = SalaryAppObjects.Select(v => new
                {
                    appobjectId = v.AppObjectsId,
                    appobjectCategory = v.ObjCategory,
                    appobjectValue = v.ObjValue,
                    active = v.Active
                }).Distinct().Where(x => x.active == true && x.appobjectCategory == "SalaryLevel")
            };


            return Ok(SalarylookupData);
        }

        [HttpGet("GetPSAInput/{id}")]
        public async Task<IActionResult> GetPSAInput(int id,int year)
        {
            var result = await OrganizationSalaryDomain.GetPSAInput(id,year);
            return Ok(result);
        }

        [HttpPost("UpdatePSAinput/{id}")]
        public async Task<IActionResult> UpdatePSAInput(int id, [FromBody] PsaInput[] models)
        {
            Response<object> response = await OrganizationSalaryDomain.UpdatePSAInput(id, models);
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
