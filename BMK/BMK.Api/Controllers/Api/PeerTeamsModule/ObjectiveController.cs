using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace BMK.Api.Controllers.Api.PeerTeamsModule
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectiveController : ControllerBase
    {
        public IObjectiveDomain ObjectiveDomain { get; set; }

        public ObjectiveController(IObjectiveDomain objectiveDomain)
        {
            ObjectiveDomain = objectiveDomain;
        }

        [HttpGet("GetObjectivesByUserId/{id}")]
        public async Task<IActionResult> GetObjectivesByUserId(int id)
        {
            return Ok(await ObjectiveDomain.GetObjectivesByUserId(id));
        }

        [HttpPost("AddObjective")]
        public async Task<IActionResult> AddObjective([FromBody] Objective objective)
        {
            return Ok(await ObjectiveDomain.AddObjective(objective));
        }

        [HttpGet("GetObjectiveByObjectiveId/{id}")]
        public async Task<IActionResult> GetObjectiveByObjectiveId(int id)
        {
            return Ok(await ObjectiveDomain.GetObjectiveByObjectiveId(id));
        }

        [HttpPut("UpdateObjective/{id}")]
        public async Task<IActionResult> UpdateObjective(int id, [FromBody] Objective objective)
        {
            var oldObjective = await ObjectiveDomain.GetObjectiveByObjectiveId(id);
            if(oldObjective == null) {
                return BadRequest();
            }
            else
            {
                oldObjective.Description = objective.Description;
                oldObjective.Notes= objective.Notes;
                oldObjective.Completion = objective.Completion;
                oldObjective.Deadline = objective.Deadline;
                oldObjective.Status = objective.Status;
                oldObjective.ModifiedBy = objective.ModifiedBy;
                oldObjective.ModifiedDate = DateTime.Now;
                oldObjective.CreatedDate = oldObjective.CreatedDate;
                oldObjective.CreatedBy = oldObjective.CreatedBy;

                if (objective.Completion == 100)
                {
                    var currentDate = DateTime.Now;
                    oldObjective.ClosedDate = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);
                }
                else
                {
                    oldObjective.ClosedDate = null;
                }
            }

            Response<Objective> response = await ObjectiveDomain.UpdateObjective(id, oldObjective);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("GetStatus")]
        public async Task<IActionResult> GetStatus()
        {
            return Ok(await ObjectiveDomain.GetStatus());
        }

        [HttpPost("AddCommentToObjective")]
        public async Task<IActionResult> AddComment(ObjectiveComment comment)
        {
            return Ok(await ObjectiveDomain.AddCommentToObjective(comment));
        }

        [HttpPut("CancelObjective/{id}")]
        public async Task<IActionResult> CancelObjective(int id)
        {
            Response<Objective> response = await ObjectiveDomain.CancelObjective(id);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response );
        }
    }
}
