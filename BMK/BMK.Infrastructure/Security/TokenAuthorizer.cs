using BMK.Infrastructure.Singleton;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RxWeb.Core.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BMK.Infrastructure.Security
{
    public class TokenAuthorizer : ITokenAuthorizer
    {
        public TokenAuthorizer(IJwtTokenProvider tokenProvider, UserAccessConfigInfo userAccessConfigInfo)
        {
            TokenProvider = tokenProvider;
            UserAccessConfigInfo = userAccessConfigInfo;
        }

        public Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json;";
            return Task.CompletedTask;
        }

        public Task Challenge(JwtBearerChallengeContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json;";
            return Task.CompletedTask;
        }

        public Task MessageReceived(MessageReceivedContext context)
        {
            try
            {
                var principal = this.ValidateTokenAsync(context.HttpContext).Result;
                if (principal != null)
                {
                    context.Principal = principal;
                    context.Success();
                }
                else
                    context.Fail("Token Not Found");
            }
            catch (Exception ex)
            {
                context.Fail("Token Expired.");
                context.Response.ContentType = "application/json;";
            }    
            return Task.CompletedTask;
        }

        public Task TokenValidated(TokenValidatedContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(AUTHORIZATION_HEADER, out var token) && context.Request.Cookies.TryGetValue("request_identity", out var requestIdentity))
            {
                var loginUow = context.RequestServices.GetService(typeof(ILoginUow)) as ILoginUow;
                var dbToken = await UserAccessConfigInfo.GetTokenAsync(requestIdentity, loginUow);
                return string.IsNullOrEmpty(dbToken) ? null : TokenProvider.ValidateToken(requestIdentity, dbToken);
            }
            return null;
        }

        public ClaimsPrincipal AnonymousUserValidateToken(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(AUTHORIZATION_HEADER, out var token) && context.Request.Cookies.TryGetValue("anonymous", out var anonymousUser))
                return TokenProvider.ValidateToken(anonymousUser, token);
            return null;
        }

        private IJwtTokenProvider TokenProvider { get; set; }

        private UserAccessConfigInfo UserAccessConfigInfo { get; set; }

        private const string AUTHORIZATION_HEADER = "Authorization";

    }
}


