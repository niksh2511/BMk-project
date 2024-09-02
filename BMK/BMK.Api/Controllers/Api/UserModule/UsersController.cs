using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.Enums;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RxWeb.Core.AspNetCore;
using RxWeb.Core.Security;
using RxWeb.Core.Security.Cryptography;

using JsonPatchDocument = Microsoft.AspNetCore.JsonPatch.JsonPatchDocument;

namespace BMK.Api.Controllers.Api.UserModule
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseCoreDomainController<User, User>
    {
        public IUserUow Uow;
        private IUserClaim UserClaim { get; set; }
        private IPasswordHash PasswordHash { get; set; }
        private IPeerTeamUow PeerTeamUow { get; set; }
        private readonly IBlobService _blobService;
        public UsersController(IPeerTeamUow peerTeamUow, IPasswordHash passwordHash, IUserDomain domain, IUserUow uow, IUserClaim userCliam, IBlobService blobService) : base(domain)
        {
            Uow = uow;
            UserClaim = userCliam;
            PasswordHash = passwordHash;
            PeerTeamUow = peerTeamUow;
            _blobService = blobService;
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await Uow.Repository<User>().Queryable().Where(x => x.UsersId == id).FirstOrDefaultAsync();
            if (user != null)
            {
                var role = await Uow.Repository<UserRole>().Queryable().Where(x => x.UsersId == id).FirstOrDefaultAsync();
                if (role != null)
                {
                    user.RoleId = role.RoleMasterId != null ? (int)role.RoleMasterId : 0;
                }
            }

            return Ok(user);
        }


        [HttpPatch]
        [Route("RemoveUser/{id}")]
        public async Task<IActionResult> RemoveUser(int id, [FromBody] JsonPatchDocument userQuery)
        {
            try
            {
                var user = await Uow.Repository<User>().Queryable().Where(x => x.UsersId == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest();
                }
                userQuery.ApplyTo(user);
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = UserClaim.UserId;
                await Uow.RegisterDirtyAsync(user);
                await Uow.CommitAsync();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet]
        [Route("GetVUser")]
        public async Task<object> GetAsync()
        {
            var usersQuery = Uow.Repository<Vuser>().Queryable();
            usersQuery = usersQuery.Where(x => x.Active == true && (UserClaim.RoleId == (int)RoleEnum.SuperAdmin || x.OrganizationId == UserClaim.OrganizationId));

            var users = await usersQuery.ToListAsync();
            return users;
        }

        [HttpPut]
        [Route("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                var oldUser = await Uow.Repository<User>().Queryable().Where(x => x.UsersId == id).FirstOrDefaultAsync();
                var oldUserRole = await Uow.Repository<UserRole>().Queryable().Where(x => x.UsersId == id).FirstOrDefaultAsync();
                if (oldUser == null)
                {
                    return BadRequest();
                }
                if (oldUser.Email != user.Email)
                {
                    var ifAlreadyUser = await Uow.Repository<User>().Queryable().Where(x => x.Email == user.Email && x.Active == true).FirstOrDefaultAsync();
                    if (ifAlreadyUser != null)
                    {
                        return Ok("Email already used");
                    }
                }
                if (!string.IsNullOrEmpty(user.Password))
                {
                    var result = PasswordHash.Encrypt(user.Password);
                    oldUser.Credential = Convert.ToHexString(result.Credential);
                    oldUser.Salt = Convert.ToHexString(result.Salt);
                }
                oldUser.Email = user.Email;
                oldUser.FirstName = user.FirstName;
                oldUser.LastName = user.LastName;
                oldUser.WorkPhone = user.WorkPhone;
                oldUser.MobilePhone = user.MobilePhone;
                oldUser.OrganizationId = user.OrganizationId;
                oldUser.IsUserOtpRequired = user.IsUserOtpRequired;
                oldUser.RoleId = user.RoleId;
                oldUser.Title = user.Title;
                oldUser.ProfilePicture = user.ProfilePicture;
                oldUser.BmkExecutiveReports = user.BmkExecutiveReports;
                oldUser.ModifiedDate = DateTime.Now;
                oldUser.ModifiedBy = UserClaim.UserId;
                if (oldUserRole != null)
                {
                    oldUserRole.RoleMasterId = user.RoleId;
                    await Uow.RegisterDirtyAsync(oldUserRole);
                }
                await Uow.RegisterDirtyAsync(oldUser);
              await Uow.CommitAsync();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpPut]
        [Route("UpdateUserGroupMapping/{id}")]
        public async Task<IActionResult> UpdateUserGroupMapping(int id, [FromBody] List<UserGroupsMember> model)
        {
            try
            {
                var List = await PeerTeamUow.Repository<UserGroupsMember>().FindByAsync(x => x.UsersId == id);

                foreach (var item in List)
                {
                    await PeerTeamUow.RegisterDeletedAsync<UserGroupsMember>(item);
                }
                await PeerTeamUow.CommitAsync();

                foreach (var item1 in model)
                {
                    item1.CreatedDate = DateTime.Now;
                    item1.CreatedBy = UserClaim.UserId;
                    item1.Active = true;
                    item1.UsersId = id;
                    await PeerTeamUow.RegisterNewAsync<UserGroupsMember>(item1);
                }
                await PeerTeamUow.CommitAsync();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [AllowAnonymous]
        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = image.OpenReadStream();
            var result = await _blobService.UploadFileAsync(stream, image.FileName);


            //if (image == null || image.Length == 0)
            //    return BadRequest("No file uploaded.");

            //byte[] bytes;
            //using (Stream fs = image.OpenReadStream())
            //{
            //    using (BinaryReader br = new BinaryReader(fs))
            //    {
            //        bytes = br.ReadBytes((Int32)fs.Length);
            //    }
            //}
            //var base64str = Convert.ToBase64String(bytes);
            return Ok(new { base64str = result });
        }

    }
}
