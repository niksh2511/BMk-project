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
using static BMK.Domain.Domain.EventDomain;

namespace BMK.Api.Controllers.Api.EventManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private IEventDomain EventDomain {  get; set; }
        private IEventUow Uow { get; set; }
        public CategoryController(IEventDomain eventDomain, IEventUow uow)
        {
            EventDomain = eventDomain;
            Uow = uow;
        }

        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await EventDomain.GetCategories());
        }

        [HttpGet("getCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            return Ok(await EventDomain.GetCategoryList());
        }

        [HttpGet("getCategory/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            return Ok(await EventDomain.GetCategoryById(id));
        }

        [HttpPost("saveCategory")]
        public async Task<IActionResult> SaveCategory([FromBody] CategoryGroupModel category)
        {   
            Response<Category> response = await EventDomain.SaveCategory(category);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("getCategoryGroups/{id}")]
        public async Task<IActionResult> GetCategoryGroups(int id)
        {
            var obj = await EventDomain.GetCategoryGroups(id);
            return Ok(obj);
        }

        [HttpGet("getCategoryGroupsList/{id}")]
        public async Task<IActionResult> GetCategoryGroupsList(int id)
        {
            var obj = await EventDomain.GetCategoryGroupsList(id);
            return Ok(obj);
        }
        [HttpPut("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryGroupModel model)
        {
            Response<Category> response = await EventDomain.UpdateCategory(model);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("removeCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Response<Category> response = await EventDomain.DeleteCategory(id);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpPost("manageNotifications")]
        public async Task<IActionResult> ManageNottification([FromBody] int[] ids)
        {
            Response<Category> response = await EventDomain.ManageNotification(ids);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }
    }
}
