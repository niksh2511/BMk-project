using BMK.Infrastructure.Security;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxWeb.Core.Common;
using RxWeb.Core.Security.Cryptography;
using RxWeb.Core.Security.Filters;
using Twilio.TwiML.Messaging;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Nodes;
using Twilio.Rest.Verify.V2.Service;
using RxWeb.Core.Security;
using System.Security.Claims;
using BMK.Infrastructure.Security.Authorization;
using BMK.Infrastructure.Singleton;

namespace BMK.Api.Controllers.Api.Core
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private ILoginUow LoginUow { get; set; }
        private IApplicationTokenProvider ApplicationTokenProvider { get; set; }
        private IPasswordHash PasswordHash { get; set; }
        private ITextSms TextSms { get; set; } //for twilio sms service
        private UserAccessConfigInfo UserAccessConfig { get; set; }
        private IUserClaim UserClaim { get; set; }

        public AuthenticationController(ILoginUow loginUow, IApplicationTokenProvider tokenProvider, IPasswordHash passwordHash, ITextSms textSms, UserAccessConfigInfo userAccessConfig, IUserClaim userClaim)
        {
            ApplicationTokenProvider = tokenProvider;
            PasswordHash = passwordHash;
            TextSms = textSms;
            LoginUow = loginUow;
            UserAccessConfig = userAccessConfig;
            UserClaim = userClaim;
        }

        [HttpGet]
        [AllowAnonymous]
        [AllowRequest(MaxRequestCountPerIp = 100)]
        public async Task<IActionResult> Get()
        {
            //THIS METHOD IS TEMPORARILY FOR GENERATING HASH AND SALT FOR PASSWORD
            //CONTENT FOR THIS METHOD WILL BE REMOVED LATER

            var password = PasswordHash.Encrypt("Radixweb@123");
            var Credential = Convert.ToHexString(password.Credential);
            var Salt = Convert.ToHexString(password.Salt);
            var user = await LoginUow.Repository<User>().FirstOrDefaultAsync(t => t.Email == "abhishek.patel@radixweb.com");
            PasswordHash.VerifySignature("Radixweb@2", user.Credential, user.Salt);
            //var token = await ApplicationTokenProvider.GetTokenAsync(new vUser { UserId = 0, ApplicationTimeZoneName = string.Empty, LanguageCode = string.Empty });
            //return Ok(token);
            return Ok("test");
        }

        [Route("VerifyUser")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUser(AuthenticationModel authentication)
        {
            UserAuthenticateModel userAuthenticateModel = new UserAuthenticateModel();
            Vuser user = await LoginUow.Repository<Vuser>().SingleOrDefaultAsync(t => t.Email.ToLower() == authentication.Email.ToLower() && t.Active == true && t.IsO365user == false);
            if (user == null)
                return BadRequest("The email is not registered with the BMK Community");

            if (string.IsNullOrEmpty(user.MobilePhone) || string.IsNullOrEmpty(user.CountryCode))
                return BadRequest("Please ask the administrator to set a mobile number for the user");

            if (!PasswordHash.VerifySignature(authentication.Password, user.Credential, user.Salt))
                return BadRequest("The password entered is incorrect");

            VerificationResource? res = null;
            if (user.IsUserOtpRequired)
            {
                SmsConfig smsConfig = new SmsConfig { To = user.CountryCode + user.MobilePhone };
                res = await TextSms.SendAsync(smsConfig);
            }

            if (!user.IsUserOtpRequired)
            {
                userAuthenticateModel = await GetUserAuthenticateInfo(user, "You have logged in successfully");
                return Ok(userAuthenticateModel);
            }
            else if (res != null)
            {
                userAuthenticateModel.Message = "OTP sent successfully";
                userAuthenticateModel.IsUserVerified = true;
                userAuthenticateModel.IsUserOtpRequired = user.IsUserOtpRequired;
                return Ok(userAuthenticateModel);
            }
            else
            {
                string errorMessage = "An error occurred while sending the OTP. Please try again.";
                // Log the exception here if needed
                return BadRequest(errorMessage);
            }

        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AuthenticationModel authentication)
        {
            if (string.IsNullOrEmpty(authentication.VerificationCode))
                return BadRequest("Enter vaild one time password(OTP)");
            UserAuthenticateModel userAuthenticateModel = new UserAuthenticateModel();
            Vuser user = await LoginUow.Repository<Vuser>().SingleOrDefaultAsync(t => t.Email.ToLower() == authentication.Email.ToLower() && t.Active == true && t.IsO365user == false);
            if (user == null)
                return BadRequest("The email is not registered with the BMK Community");

            if (string.IsNullOrEmpty(user.MobilePhone) || string.IsNullOrEmpty(user.CountryCode))
                return BadRequest("Ask the administrator to set a mobile number for the user recevice one time password(OTP)");

            string mobilePhone = user.CountryCode + user.MobilePhone;
            string verificationCodeResponse = await TextSms.VerfiyAsync(mobilePhone, authentication.VerificationCode);
            if (verificationCodeResponse == "invalid" || verificationCodeResponse == "expired")
            {
                userAuthenticateModel.IsUserVerified = false;
                userAuthenticateModel.Message = "The one-time password (OTP) is invalid";
            }
            else
            {
                userAuthenticateModel = await GetUserAuthenticateInfo(user, "You have logged in successfully");
            }

            return Ok(userAuthenticateModel);
        }

        [Route("SwitchUser/{email}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SwitchUser(string email)
        {
            UserAuthenticateModel userAuthenticateModel = new UserAuthenticateModel();
            Vuser user = await LoginUow.Repository<Vuser>().SingleOrDefaultAsync(t => t.Email.ToLower() == email.ToLower() && t.Active == true);

            
            if (user == null)
            {
                userAuthenticateModel.Message = "The email is not registered with the BMK Community";
                userAuthenticateModel.IsUserVerified = false;
                return Ok(userAuthenticateModel);
            }
            else
            {
                userAuthenticateModel = await GetUserAuthenticateInfo(user, "You have logged in successfully");
            }

            return Ok(userAuthenticateModel);
        }

        [Route("verifyO365")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyO365([FromBody] O365VerificationModel jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt.IdToken);

                if (token != null)
                {
                    //decode token, extract email from token, then check if email exists in database
                    var keyId = token.Header.Kid;
                    var audience = token.Audiences.ToList();
                    var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();

                    var email = (claims[10]).Value;
                    var user = await GetUserByEmailAsync(email);
                    if (user != null)
                    {
                        return Ok(await GetUserAuthenticateInfo(user, "User Verified via O365"));
                    }
                    else
                    {
                        return BadRequest("User Not Found in System.");
                    }
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("There was some issue parsing the Token.");
            }

        }

        private async Task<Vuser> GetUserByEmailAsync(string email)
        {
            Vuser user = await LoginUow.Repository<Vuser>().SingleOrDefaultAsync(t => t.Email == email && t.Active == true && t.IsO365user == true);
            return user;
        }

        private async Task<UserAuthenticateModel> GetUserAuthenticateInfo(Vuser user, string message)
        {
            var userAuthenticateModel = new UserAuthenticateModel();
            userAuthenticateModel.UserId = user.UsersId;
            userAuthenticateModel.IsUserVerified = true;
            userAuthenticateModel.IsUserOtpRequired = user.IsUserOtpRequired;
            userAuthenticateModel.Email = user.Email;
            userAuthenticateModel.UserName = user.Firstname + " " + user.LastName;
            userAuthenticateModel.Role = user.RoleName;
            userAuthenticateModel.RoleId = Convert.ToInt32(user.RoleMasterId);
            userAuthenticateModel.Organization = user.Name;
            userAuthenticateModel.OrganizationId = user.OrganizationId;
            userAuthenticateModel.Message = message;
            userAuthenticateModel.ProfilePicture = user.ProfilePicture;
            userAuthenticateModel.AccessModules = await UserAccessConfig.FirstTimeCacheAccessInfoAsync(LoginUow, user.UsersId);
            userAuthenticateModel.AuthToken = await ApplicationTokenProvider.GetTokenAsync(user);
            return userAuthenticateModel;
        }
    }
}

