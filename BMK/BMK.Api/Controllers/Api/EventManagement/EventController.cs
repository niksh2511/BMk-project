using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.Models;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.AgreementAcceptances.Item;
using System.Net;

namespace BMK.Api.Controllers.Api.EventManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {

        private IEventDomain EventDomain { get; set; }
        private IEventUow Uow { get; set; }
        public EventController(IEventDomain eventDomain, IEventUow uow)
        {
            EventDomain = eventDomain;
            Uow = uow;
        }

        [HttpGet("getEvents")]
        public async Task<IActionResult> GetEvents()
        {
            return Ok(await EventDomain.GetEvents());
        }

        [HttpPost("saveEvent")]
        public async Task<IActionResult> SaveEvent([FromBody] EventModel events)
        {
            Response<Event> response = await EventDomain.SaveEvent(events);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("getEventCategoryList/{userID}")]
        public async Task<IActionResult> GetEventCategoryList(int userID)
        {
            return Ok(await EventDomain.GetEventCategoryList(0,userID));
        }

        [HttpGet("sendEmailNotification/{eventId}")]
        public async Task<IActionResult> sendEmailNotification(int eventId)
        {
            return Ok(await EventDomain.sendEmailNotification(eventId));
        }

        [HttpGet("getEventById/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            return Ok(await EventDomain.GetEventCategoryList(id,0));
        }

        [HttpPut("updateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventCategoryModel model)
        {
            Response<Event> response = await EventDomain.UpdateEvent(id,model);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("removeEvent/{id}")]
        public async Task<IActionResult> RemoveEvent(int id)
        {
            Response<Event> response = await EventDomain.DeleteEvent(id);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }
    }
}
