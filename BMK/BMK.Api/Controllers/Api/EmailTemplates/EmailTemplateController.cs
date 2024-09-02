using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RxWeb.Core.Security;

namespace BMK.Api.Controllers.Api.EmailTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTemplateController : ControllerBase
    {
        private IUserUow UserUow { get; set; }
        private IEmailTemplateDomain EmailTemplateDomain { get; set; }
        public EmailTemplateController(IUserUow userUow, IUserClaim userClaim, IEmailTemplateDomain emailTemplateDomain)
        {
            UserUow = userUow;
            EmailTemplateDomain = emailTemplateDomain;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var templates = await UserUow.Repository<EmailTemplate>().Queryable().Where(r => r.Active == true).ToListAsync();

            return Ok(templates);

        }


        [HttpGet("GetEmailTemplatesById/{id}")]
        public async Task<IActionResult> GetEmailTemplatesById(int id)
        {
            return Ok(await EmailTemplateDomain.GetEmailTemplatesById(id));
        }

        [HttpPost("saveEmailTemplate")]
        public async Task<IActionResult> saveEmailTemplate(EmailTemplate model)
        {

            var result = await EmailTemplateDomain.SaveEmailTemplate(model);
            return Ok(result);
        }
    }
}
