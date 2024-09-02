using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.Enums.Main;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RxWeb.Core.Common;
using RxWeb.Core.Security.Cryptography;
using System.Net.Mail;
using System.Web;

namespace BMK.Api.Controllers.Api.UserModule
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/")]
    public class ForgotPasswordController : ControllerBase
    {
        private ILoginUow LoginUow { get; set; }
        private IAesEncryption AesEncryption { get; set; }
        private IUserDomain UserDomain { get; set; }
        private IEmail Email { get; set; }

        private readonly IConfiguration Config;

        public ForgotPasswordController(ILoginUow loginUow, IAesEncryption aesEncryption, IConfiguration config, IEmail email, IUserDomain userDomain)
        {
            LoginUow = loginUow;
            AesEncryption = aesEncryption;
            Config = config;
            Email = email;
            UserDomain = userDomain;
        }

        [Route("forgot-password")]
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmail(ForgetPasswordModel forgetPasswordModel)
        {
            Vuser user = await LoginUow.Repository<Vuser>().SingleOrDefaultAsync(u => u.Email.ToLower() == forgetPasswordModel.Email.ToLower() && u.Active == true);
            if (user != null)
            {
                if ((bool)user.IsO365user)
                    return BadRequest("The Office 365 user can reset their password through the Office 365 portal.");
                var userForgotPwdToken = new UserForgotPwdToken
                {
                    UsersId = user.UsersId,
                    Active = true,
                    CreatedBy = user.UsersId,
                    VerificationKey = AesEncryption.GenerateActivationKey()
                };

                var encryptVerificationKey = AesEncryption.Encrypt(userForgotPwdToken.VerificationKey);

                var emailTemplate = await LoginUow.Repository<EmailTemplate>().SingleOrDefaultAsync(e => e.EmailTemplatesId == (int)EmailTemplateEnum.ForgetPassword && e.Active == true);
                string resetPasswordUrl = Convert.ToString(Config["BmkSetting:ResetPasswordUrl"]) + HttpUtility.UrlEncode(encryptVerificationKey);
                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##URL##", resetPasswordUrl);
                MailConfig mailConfig = new MailConfig
                {
                    To = { user.Email },
                    EmailFormat = EmailFormatType.Html,
                    From = Convert.ToString(Config["EmailSetting:FromMail"]),
                    Subject = emailTemplate.TemplateSubject,
                    Body = emailTemplate.TemplateBody,
                };


                userForgotPwdToken.IsEmailSent = await Email.SendAsync(mailConfig);
                await LoginUow.RegisterNewAsync(userForgotPwdToken);
                await LoginUow.CommitAsync();

                return Ok("A password reset email has been sent successfully");
            }
            else
            {
                return BadRequest("The email is not registered with the BMK Community");
            }
        }

        [Route("reset-password")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPasswordModel forgetPasswordModel)
        {
            if (forgetPasswordModel.ConfirmPassword != forgetPasswordModel.Password)
                return BadRequest("Password and confirm password do not match");
            if (string.IsNullOrEmpty(forgetPasswordModel.VerificationKey))
                return BadRequest("You have already reset your password once, please proceed to the 'Forgot Password' page and generate a new reset password link");

            string decryptedVerificationKey = AesEncryption.Decrypt(forgetPasswordModel.VerificationKey);
            UserForgotPwdToken userForgotPwdToken = await LoginUow.Repository<UserForgotPwdToken>().SingleOrDefaultAsync(u => u.VerificationKey == decryptedVerificationKey && u.Active == true);
            if (userForgotPwdToken == null)
                return BadRequest("You have already reset your password once, please proceed to the 'Forgot Password' page and generate a new reset password link");
            await FindActiveForgetPasswordKeyAndUpdate(userForgotPwdToken.UsersId, userForgotPwdToken.UserForgotPwdTokenId);
            var user = await UserDomain.UpdatePassword(forgetPasswordModel.Password, (int)userForgotPwdToken.UsersId);

            var emailTemplate = await LoginUow.Repository<EmailTemplate>().SingleOrDefaultAsync(e => e.EmailTemplatesId == (int)EmailTemplateEnum.ResetPasswordSuccessfully && e.Active == true);
            //MailConfig mailConfig = new MailConfig
            //{
            //    To = { user.Email },
            //    EmailFormat = EmailFormatType.Html,
            //    From = Convert.ToString(Config["EmailSetting:FromMail"]),
            //    Subject = emailTemplate.TemplateSubject,
            //    Body = emailTemplate.TemplateBody,
            //};
            //var isSucess = await Email.SendAsync(mailConfig);
            //if (isSucess)

            userForgotPwdToken.Active = false;
            userForgotPwdToken.ModifiedBy = user.UsersId;
            userForgotPwdToken.ModifiedDate = DateTime.UtcNow;
            await LoginUow.RegisterDirtyAsync(userForgotPwdToken);
            await LoginUow.CommitAsync();


            return Ok("Password reset successfully");
        }

        private async Task FindActiveForgetPasswordKeyAndUpdate(int? usersId, int id)
        {
            var userForgotPwdTokens = LoginUow.Repository<UserForgotPwdToken>().All().Where(u => u.UsersId == usersId && u.UserForgotPwdTokenId != id).AsEnumerable();
            if (userForgotPwdTokens.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var userForgotPwdToken in userForgotPwdTokens)
                {
                    userForgotPwdToken.Active = false;
                    userForgotPwdToken.ModifiedBy = userForgotPwdToken.UsersId;
                    userForgotPwdToken.ModifiedDate = now;
                }
                await LoginUow.RegisterDirtyAsync(userForgotPwdTokens);
                await LoginUow.CommitAsync();
            }

        }
    }
}