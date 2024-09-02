using BMK.Infrastructure.Security;
using BMK.Infrastructure.Singleton;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RxWeb.Core.Security;

namespace BMK.Api.Controllers.Api.Core
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private ILoginUow LoginUow { get; set; }

        private IUserClaim UserClaim { get; set; }
        private UserAccessConfigInfo UserAccessConfig { get; set; }

        private IApplicationTokenProvider ApplicationTokenProvider { get; set; }

        public AuthorizeController(ILoginUow loginUow, UserAccessConfigInfo userAccessConfig, IUserClaim userClaim, IApplicationTokenProvider applicationTokenProvider)
        {
            LoginUow = loginUow;
            UserAccessConfig = userAccessConfig;
            UserClaim = userClaim;
            ApplicationTokenProvider = applicationTokenProvider;
        }

        [HttpGet(ACCESS)]
        public async Task<IActionResult> Get()
        {
            var accessModules = await UserAccessConfig.GetFullInfoAsync(UserClaim.UserId, LoginUow);
            return Ok(JsonConvert.SerializeObject(accessModules));
        }

        [HttpPost(LOGOUT)]
        public async Task<IActionResult> LogOut()
        {
            await ApplicationTokenProvider.RemoveTokenAsync();
            return Ok(LOGOUT);
        }

        [HttpPost(REFRESH)]
        public async Task<IActionResult> Refresh(UserConfig userConfig)
        {
            var user = await LoginUow.Repository<Vuser>().SingleAsync(t => t.UsersId == UserClaim.UserId);
            var token = await ApplicationTokenProvider.RefereshTokenAsync(user, userConfig);
            return Ok(token);
        }

        const string LOGOUT = "logout";

        const string ACCESS = "access";

        const string REFRESH = "refresh";
    }
}

