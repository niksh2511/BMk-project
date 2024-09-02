using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BMK.Api.Controllers.Api.StateModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        public IUserUow Uow;

        public StateController(IUserUow uow)
        {
            Uow = uow;
        }

        [HttpGet]

        public async Task<IActionResult> GetStates()
        {
            var states = await Uow.Repository<State>().Queryable().ToListAsync();

            return Ok(states);
        }
    }
}
