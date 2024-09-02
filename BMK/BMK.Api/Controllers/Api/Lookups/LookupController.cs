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
    public class LookupController : ControllerBase
    {

        public IUserUow Uow;
        public LookupController(IUserUow uow)
        {
            Uow = uow;
        }

        [HttpGet]
        [Route("GetAppObjects/{ObjCategory}")]
        public async Task<IActionResult> GetAppObjects(string ObjCategory)
        {
            var appObjects = await Uow.Repository<AppObject>().Queryable().Where(x => x.ObjCategory == ObjCategory).ToListAsync();

            return Ok(appObjects);
        }
    }
}
