using BMK.Domain.Domain;
using BMK.Models;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RxWeb.Core.AspNetCore;
using RxWeb.Core.Security;
using SendGrid.Helpers.Mail;

namespace BMK.Api.Controllers.Api.OrganizationModule
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : BaseCoreDomainController<Organization, Organization>
    {
        public IUserUow Uow;
        private IUserClaim UserClaim { get; set; }
        //private BMKDbContext Context { get; set; }
        public OrganizationController(IOrganizationDoomain domain, IUserUow uow, IUserClaim userCliam) : base(domain)
        {
            Uow = uow;
            UserClaim = userCliam;
            //Context = context;
        }


        [HttpGet]
        [Route("GetOrgs")]
        public async Task<object> GetOrgsLookUp()
        {
            var organizations = await Uow.Repository<Organization>()
                                  .Queryable()
                                  .Select(org => new { org.OrganizationId, org.Name, org.Active })
                                  .Where(x => x.Active == true)
                                  .ToListAsync();

            return organizations;

        }

        [HttpGet]
        [Route("GetOrganization/{id}")]
        public async Task<IActionResult> GetOrganization(int id)
        {
            Organization organization = await Uow.Repository<Organization>().FirstOrDefaultAsync(x => x.OrganizationId == id);
            if (organization != null)
            {
                organization.OrganizationPortalSettings = await Uow.Repository<OrganizationPortalSetting>().Queryable().Where(x => x.OrganizationId == id).ToListAsync();
            }
            return Ok(organization);
        }

        [HttpGet]
        [Route("GetVOrganizations")]
        public async Task<object> GetOrganization()
        {
            var usersQuery = Uow.Repository<Vorganizaion>().Queryable();
            usersQuery = usersQuery.Where(x => x.Active == true);

            var users = await usersQuery.ToListAsync();
            return users;
        }



        [HttpPatch]
        [Route("RemoveOrganization/{id}")]
        public async Task<IActionResult> RemoveOrganization(int id, [FromBody] JsonPatchDocument organizationQuery)
        {
            try
            {
                var organization = await Uow.Repository<Organization>().Queryable().Where(x => x.OrganizationId == id).FirstOrDefaultAsync();
                if (organization == null)
                {
                    return BadRequest();
                }
                organizationQuery.ApplyTo(organization);
                organization.ModifiedBy = UserClaim.UserId;
                organization.ModifiedDate = DateTime.Now;
                await Uow.RegisterDirtyAsync(organization);
                await Uow.CommitAsync();
                var deleteOrguser = Uow.Repository<User>().All().Where(x => x.OrganizationId == id).AsEnumerable();
                if (deleteOrguser.Any())
                {
                    var now = DateTime.Now;
                    foreach (var user in deleteOrguser)
                    {
                        user.Active = false;
                        user.ModifiedDate = now;
                        user.ModifiedBy = UserClaim.UserId;
                    }
                    await Uow.RegisterDirtyAsync(deleteOrguser);
                    await Uow.CommitAsync();
                    return Ok("Success");
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }


        [HttpPut]
        [Route("UpdateOrganization/{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, [FromBody] Organization organization)
        {
            try
            {
                Organization oldOrganization = new Organization();
                oldOrganization = await Uow.Repository<Organization>().FirstOrDefaultAsync(x => x.OrganizationId == id);
                if (oldOrganization != null && oldOrganization.OrganizationId <= 0)
                {
                    return BadRequest();
                }

       
                oldOrganization.StaffSize = organization.StaffSize;
                oldOrganization.Name = organization.Name;
                oldOrganization.Rmm = organization.Rmm;
                oldOrganization.Address = organization.Address;
                oldOrganization.Website = organization.Website;
                oldOrganization.TargetEhr = organization.TargetEhr;
                oldOrganization.OtherTools = organization.Name;
                oldOrganization.AdminPayrollTracking = organization.AdminPayrollTracking;
                oldOrganization.AnnualRevenue = organization.AnnualRevenue;
                oldOrganization.City = organization.City;
                oldOrganization.Phone = organization.Phone;
                oldOrganization.Psa = organization.Psa;
                oldOrganization.StatesId = organization.StatesId;
                oldOrganization.ExcludeFromAverages = organization.ExcludeFromAverages;
                oldOrganization.TargetNetIncome = organization.TargetNetIncome;
                oldOrganization.TargetAgp = organization.TargetAgp;
                oldOrganization.SalesPayrollTracking = organization.SalesPayrollTracking;
                oldOrganization.CountryCode = organization.CountryCode;
                oldOrganization.ModifiedDate = DateTime.Now;
                oldOrganization.ModifiedBy = UserClaim.UserId;
                oldOrganization.Zipcode = organization.Zipcode;
                await Uow.RegisterDirtyAsync(oldOrganization);
                if (organization.OrganizationPortalSettings.Any())
                {
                    await Uow.RegisterDirtyAsync(organization.OrganizationPortalSettings.AsEnumerable());
                }
                await Uow.CommitAsync();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet]
        [Route("GetOrganizationPortalSettings/{id}")]
        public async Task<IActionResult> GetOrganizationPortalSetting(int id)
        {
            var organization = await Uow.Repository<OrganizationPortalSetting>().Queryable().Where(x => x.OrganizationId == id).ToListAsync();

            return Ok(organization);
        }

        [HttpGet("GetOrganizationByUserId/{id}")]
        public async Task<IActionResult> GetOrganizationByUserId(int id)
        {
            var orgId = await Uow.Repository<Models.DbEntities.User>().Queryable().Where(x => x.UsersId == id).Select(x => x.OrganizationId).FirstOrDefaultAsync() ;

            //this will not work. need to implement db view vOrganizationForPeerTeamProfile here
            var org = await Uow.Repository<VOrganizationForPeerTeamProfile>().Queryable().Where(x => x.OrganizationId == orgId).FirstOrDefaultAsync();
            var vOrg = new PeerTeamOrganizationModel();
            if (org != null)
            {
                vOrg.organizationID = org.OrganizationId;
                if (org.StaffSize != null)
                {
                    vOrg.staffSize = (int)org.StaffSize;
                }
                else
                {
                    vOrg.staffSize = 0;
                }
                vOrg.organizationName = org.Name;
                vOrg.PSA = org.Psa;
                vOrg.RMM = org.Rmm;
                vOrg.otherServices = org.OtherTools;
                vOrg.Salaries = await Uow.Repository<VOrganizationSalary>().Queryable().Where(x => x.OrganizationId == orgId).ToListAsync();
            }
            return Ok(vOrg);
        }


        [HttpGet]
        [Route("GetOrganizationStaffSizeAndAnnualRevenue/{id}")]
        public async Task<IActionResult> GetOrganizationStaffSizeAndAnnualRevenue(int id)
        {
            var staffSize = await Uow.Repository<OrganizationSalary>()
                                  .Queryable()
                                  .Where(x => x.OrganizationId == id)
                                  .CountAsync();

            var annualRevenue = await Uow.Repository<BmkTargetReport>()
                                .Queryable()
                                .Where(x => x.OrganizationId == id && x.Year == DateTime.Now.Year && x.Month == "Tot"
                                && x.RptHead == "Total Revenue" && x.RptLine == "Total Revenue" && x.CategoryId==10010)
                                .Select(x => x.Amount).FirstOrDefaultAsync();

            if (annualRevenue == null)
            {
                annualRevenue = 0;
            }
            return Ok(new { staffSize= staffSize, annualRevenue = annualRevenue });
        }
    }
}
