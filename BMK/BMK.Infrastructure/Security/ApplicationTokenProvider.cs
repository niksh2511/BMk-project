using BMK.Infrastructure.Model;
using BMK.Infrastructure.Singleton;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Http;
using RxWeb.Core.Security;
using System.Security.Claims;

namespace BMK.Infrastructure.Security
{
    public class ApplicationTokenProvider : IApplicationTokenProvider
    {
        private ILoginUow LoginUow { get; set; }
        private UserAccessConfigInfo UserAccessConfig { get; set; }
        private IJwtTokenProvider TokenProvider { get; set; }

        private IUserClaim UserClaim { get; set; }

        private IHttpContextAccessor ContextAccessor { get; set; }
        private ISessionProvider SessionProvider { get; set; }

        public ApplicationTokenProvider(IJwtTokenProvider tokenProvider, UserAccessConfigInfo userAccessConfig, ILoginUow loginUow, IUserClaim userClaim, IHttpContextAccessor contextAccessor, ISessionProvider sessionProvider)
        {
            TokenProvider = tokenProvider;
            UserAccessConfig = userAccessConfig;
            LoginUow = loginUow;
            UserClaim = userClaim;
            ContextAccessor = contextAccessor;
            SessionProvider = sessionProvider;
        }
        public async Task<string> GetTokenAsync(Vuser user)
        {
            var expirationTime = user.UsersId == 0 ? DateTime.Now.AddDays(1) : DateTime.Now.AddHours(8);
            var token = TokenProvider.WriteToken(new[]{
                new Claim(
                    ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Anonymous, (user.UsersId == 0).ToString()),
                new Claim(ClaimTypes.Role,user.RoleMasterId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("organizationId", user.OrganizationId.ToString())
                    }, "BMK Community", "User", expirationTime);
            if (user.UsersId != 0)
            {
                await UserAccessConfig.SaveTokenAsync(user.UsersId, "web", token, LoginUow);
                SetUserInfo(user);
            }
            this.AddCookie(user, token.Key);
            return token.Value;
        }

        public async Task<string> RefereshTokenAsync(Vuser user, UserConfig userConfig)
        {
            if (!string.IsNullOrEmpty(userConfig.LanguageCode))
            {
                var userRecord = await LoginUow.Repository<User>().SingleAsync(t => t.UsersId == user.UsersId);
                await LoginUow.RegisterDirtyAsync<User>(userRecord);
                await LoginUow.CommitAsync();
            }
            await UserAccessConfig.RemoveTokenAsync(user.UsersId, LoginUow);
            return await GetTokenAsync(user);
        }

        public async Task RemoveTokenAsync()
        {
            RemoveCookie();
            await UserAccessConfig.RemoveTokenAsync(UserClaim.UserId, LoginUow);
        }

        private void AddCookie(Vuser user, string value)
        {
            var cookieName = user.UsersId == 0 ? ANONYMOUS : REQUEST_IDENTITY;
            if (cookieName == REQUEST_IDENTITY && ContextAccessor.HttpContext.Request.Cookies.ContainsKey(ANONYMOUS))
                ContextAccessor.HttpContext.Response.Cookies.Delete(ANONYMOUS);
            ContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value);
        }
        private void RemoveCookie()
        {
            ContextAccessor.HttpContext.Response.Cookies.Delete(REQUEST_IDENTITY);
            ContextAccessor.HttpContext.Response.Cookies.Delete(USER_INFO);
        }

        private void SetUserInfo(Vuser user)
        {
            ContextAccessor.HttpContext.Response.Cookies.Delete(USER_INFO);
            UserInfo userInfo = new UserInfo
            {
                UserId = user.UsersId,
                RoleId = Convert.ToInt32(user.RoleMasterId),
                Email = user.Email,
                OrganizationId = Convert.ToInt32(user.OrganizationId),
            };
            SessionProvider.SetObject(USER_INFO, userInfo);
        }

        private const string REQUEST_IDENTITY = "request_identity";
        private const string ANONYMOUS = "anonymous";
        private const string USER_INFO = "user_info";
    }

    public interface IApplicationTokenProvider
    {
        Task<string> GetTokenAsync(Vuser user);

        Task<string> RefereshTokenAsync(Vuser user, UserConfig userConfig);

        Task RemoveTokenAsync();
    }
}



