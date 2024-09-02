using RxWeb.Core;
using RxWeb.Core.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using BMK.UnitOfWork;
using BMK.Models.DbEntities;
using System.Security.Cryptography;
using System.Text;
using RxWeb.Core.Common;
using BMK.Models.Enums;
using Microsoft.Extensions.Configuration;
using BMK.UnitOfWork.Main;
using RxWeb.Core.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using User = BMK.Models.DbEntities.User;


namespace BMK.Domain.Domain
{
    public class UserDomain : IUserDomain
    {
        private IPasswordHash PasswordHash { get; set; }
        private IEmail Email { get; set; }
        private IUserClaim UserClaim { get; set; }
        private readonly IConfiguration Config;

        public UserDomain(IUserUow uow, IEmail email,
            IPasswordHash passwordHash, IConfiguration config, IUserClaim userClaim)
        {
            PasswordHash = passwordHash;
            Uow = uow;
            Email = email;
            Config = config;
            UserClaim = userClaim;
        }

        public async Task<object> GetAsync(User parameters)
        {
            var users = await Uow.Repository<User>().Queryable().Where(x => x.Active == true).ToListAsync() as object;

            return users;
        }

        public async Task<object> GetBy(User parameters)
        {
            var user = await Uow.Repository<User>().Queryable().Where(x => x.UsersId == parameters.UsersId).FirstOrDefaultAsync() as object;
            return user;
        }


        public HashSet<string> AddValidation(User entity)
        {
            var user = Uow.Repository<User>().FindBy(x => x.Email == entity.Email).FirstOrDefault();

            if (user != null && user.Active == true)
            {
                ValidationMessages.Add("Email is Already Registered");
            }
            return ValidationMessages;  
        }
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password of a given length (optional)
        public string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        public async Task AddAsync(User entity)
        {
            
            if (entity.IsO365user == true)
            {
                var invited = await InviteUserAsGuest($"{entity.FirstName} {entity.LastName}", entity.Email);
                if (invited)
                {
                    entity.Active = true;
                    entity.IsUserOtpRequired = false;
                    await Uow.RegisterNewAsync(entity);
                    await Uow.CommitAsync();

                    var emailTemplate = await Uow.Repository<EmailTemplate>().SingleOrDefaultAsync(e => e.EmailTemplatesId == (int)EmailTemplateEnum.O365WelcomeMail && e.Active == true);
                    emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##FirstName##", entity.FirstName);
                    emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Email##", entity.Email);
                    MailConfig mailConfig = new MailConfig
                    {
                        To = { entity.Email },
                        EmailFormat = EmailFormatType.Html,
                        From = Convert.ToString(Config["EmailSetting:FromMail"]),
                        Subject = emailTemplate.TemplateSubject,
                        Body = emailTemplate.TemplateBody,
                    };


                    bool isSuccess = await Email.SendAsync(mailConfig);
                }
                else
                {
                    ValidationMessages.Add("Inviting User For Microsoft365 Failed");
                }
            }
            else
            {
                var password = GeneratePassword();
                var result = PasswordHash.Encrypt(password);
                entity.Credential = Convert.ToHexString(result.Credential);
                entity.Salt = Convert.ToHexString(result.Salt);
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = UserClaim.UserId;
                entity.Active = true;
                await Uow.RegisterNewAsync(entity);
                await Uow.CommitAsync();
                var emailTemplate = await Uow.Repository<EmailTemplate>().SingleOrDefaultAsync(e => e.EmailTemplatesId == (int)EmailTemplateEnum.Logincredential && e.Active == true);
                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##FirstName##", entity.FirstName);
                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Email##", entity.Email);
                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Password##", password);
                MailConfig mailConfig = new MailConfig
                {
                    To = { entity.Email },
                    EmailFormat = EmailFormatType.Html,
                    From = Convert.ToString(Config["EmailSetting:FromMail"]),
                    Subject = emailTemplate.TemplateSubject,
                    Body = emailTemplate.TemplateBody,
                };


                bool isSuccess = await Email.SendAsync(mailConfig);
            }
            UserRole userrole = new UserRole { UsersId = entity.UsersId, RoleMasterId = entity.RoleId, Active = true, CreatedBy = UserClaim.UserId, CreatedDate = DateTime.Now };
            await Uow.RegisterNewAsync(userrole);
            await Uow.CommitAsync();

        }

        public HashSet<string> UpdateValidation(User entity)
        {
            return ValidationMessages;
        }

        public async Task UpdateAsync(User entity)
        {
            await Uow.RegisterDirtyAsync(entity);
            await Uow.CommitAsync();
        }

        public HashSet<string> DeleteValidation(User parameters)
        {
            return ValidationMessages;
        }

        public Task DeleteAsync(User parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<User> UpdatePassword(string password, int userId)
        {
            var passwordResult = PasswordHash.Encrypt(password);
            var user = await Uow.Repository<User>().Queryable().Where(u => u.UsersId == userId && u.Active == true && u.IsO365user == false).FirstOrDefaultAsync();

            user.Credential = Convert.ToHexString(passwordResult.Credential);
            user.Salt = Convert.ToHexString(passwordResult.Salt);
            //user.ModifiedBy = UserClaim.UserId;
            user.ModifiedDate = DateTime.Now;
            await UpdateAsync(user);
            return user;
        }

        public async Task<bool> InviteUserAsGuest(string name, string email)
        {
            // Replace with your application registration details
            string clientId = Config["AzureGraphApi:clientId"].ToString();
            string tenantId = Config["AzureGraphApi:tenantId"].ToString();
            string clientSecret = Config["AzureGraphApi:clientSecret"].ToString();
            IEnumerable<string> scopes = new List<string> { "https://graph.microsoft.com/.default" }; // Scope for inviting guests;


            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            var authResult = await confidentialClientApplication.AcquireTokenForClient(scopes)
                .ExecuteAsync();

            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            // Get authenticated GraphServiceClient
            var graphClient = new GraphServiceClient(httpClient);

            // Create invitation object
            var invitation = new Invitation
            {
                InvitedUserDisplayName = name,
                InvitedUserEmailAddress = email,
                InvitedUserMessageInfo = new InvitedUserMessageInfo
                {
                    // Optional message to include in the invitation email
                    CustomizedMessageBody = "You're invited to join as a guest user."
                },
                InviteRedirectUrl = "https://bmkportal.live1.dev.radixweb.net/sign-in",
                SendInvitationMessage = false
            };

            try
            {
                // Invite user as a guest
                var invite = await graphClient.Invitations.PostAsync(invitation);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public IUserUow Uow { get; set; }

        private HashSet<string> ValidationMessages { get; set; } = new HashSet<string>();

        private const string AllChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*_";

        public static string GeneratePassword(int length = 8)
        {

            // Shuffle the characters to ensure randomness
            var shuffledChars = new string(AllChars.OrderBy(x => Guid.NewGuid()).ToArray());

            // Take the first 'length' characters from the shuffled string
            return new string(shuffledChars.Take(length).ToArray());
        }
    }

    public interface IUserDomain : ICoreDomain<User, User>
    {
        public Task<User> UpdatePassword(string password, int userId);
    }
}
