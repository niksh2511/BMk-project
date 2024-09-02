using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BMK.Api.Controllers.Api.BMKMetting
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BmkScheduleMeetingController : ControllerBase
    {
        private IBmkScheduleMeetingDomain BmkScheduleMeetingDomain { get; set; }
        public BmkScheduleMeetingController(IBmkScheduleMeetingDomain bmkScheduleMeetingDomain)
        {
            BmkScheduleMeetingDomain = bmkScheduleMeetingDomain;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await BmkScheduleMeetingDomain.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] JObject bmkmemberMeeting)
        {
            var bmkMember = bmkmemberMeeting.ToObject<BmkMemberMeeting>();
            var validationMessage = await BmkScheduleMeetingDomain.AddValidation(bmkMember);
            if (validationMessage.Count > 0)
                return UnprocessableEntity(validationMessage);
            return Ok(await BmkScheduleMeetingDomain.AddAsync(bmkMember));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] JObject bmkmemberMeeting)
        {
            var bmkMember = bmkmemberMeeting.ToObject<BmkMemberMeeting>();
            var validationMessage = await BmkScheduleMeetingDomain.UpdateValidation(bmkMember);
            if (validationMessage.Count > 0)
                return UnprocessableEntity(validationMessage);
            return Ok(await BmkScheduleMeetingDomain.UpdateAsync(bmkMember));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] JsonPatchDocument<BmkMemberMeeting> patchDocument)
        {
            var bmkmemberMeeting = await BmkScheduleMeetingDomain.GetByAsync(id);
            patchDocument.ApplyTo(bmkmemberMeeting);
            return Ok(await BmkScheduleMeetingDomain.UpdateAsync(bmkmemberMeeting));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(BmkMemberMeeting bmkmemberMeeting)
        {
            return Ok(await BmkScheduleMeetingDomain.DeleteAsync(bmkmemberMeeting));
        }
    }

}
