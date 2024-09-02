using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.DbEntities.Main;
using BMK.Models.Enums;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RxWeb.Core.Security;

using System.Linq;
using System.Net;

namespace BMK.Api.Controllers.Api.UserModule
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoleMasterController : ControllerBase
    {
        private IUserUow UserUow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IRoleDomain RoleDomain { get; set; }
        public RoleMasterController(IUserUow userUow, IUserClaim userClaim, IRoleDomain roleDomain)
        {
            UserUow = userUow;
            RoleDomain = roleDomain;
            UserClaim = userClaim;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await UserUow.Repository<RoleMaster>().Queryable().Select(r => new { RoleId = r.RoleMasterId, Name = r.RoleName, Desc = r.RoleDesc }).ToListAsync();

            return Ok(roles);

        }


        [HttpGet("GetRolePermission/{id}")]
        public async Task<IActionResult> GetRolePermission(int id)
        {
            return Ok(await RoleDomain.GetRolePermission(id));
        }

        [HttpPost("SaveRolePermission")]
        public async Task<IActionResult> SaveRolePermission(RolePermissionModel model)
        {

            var result = await RoleDomain.SaveRolePermission(model);
            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            Response<object> response = await RoleDomain.DeleteRole(id);
            return StatusCode( (int)HttpStatusCode.OK , response);


        }
        
    }
}
