using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.EntityFrameworkCore;

using RxWeb.Core.Data;

using RxWeb.Core.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxWeb.Core.AspNetCore;
using RxWeb.Core.Security.Cryptography;

using JsonPatchDocument = Microsoft.AspNetCore.JsonPatch.JsonPatchDocument;

namespace BMK.Domain.Domain
{
    public class OrganizationSalaryDomain : IOrganizationSalaryDomain
    {
        public IUserUow UserUow { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        public IUserClaim UserClaim { get; set; }
        public OrganizationSalaryDomain(IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException)
        {
            UserUow = userUow;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;

        }

        public async Task<Response<object>> DeleteOrganizationSalary(int id, [FromBody] JsonPatchDocument salaryQuery)
        {
            Response<object> response = new Response<object>();
            var salaryExists = await UserUow.Repository<OrganizationSalary>().Queryable().Where(x => x.OrganizationSalaryId == id).FirstOrDefaultAsync();
            if (salaryExists == null)
            {
                response.IsSucceed = false;
                response.Message = "Organization salary already deleted";
            }
            else
            {
                salaryQuery.ApplyTo(salaryExists);
                //salaryExists.ModifyBy = UserClaim.UserId;
                //salaryExists.ModifyDate = DateTime.Now;
                await UserUow.RegisterDirtyAsync(salaryExists);
                await UserUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Salary Deleted Successfully";
            }
            return response;
        }

        public async Task<List<VOrganizationSalary>> GetOrganizationSalary(int id)
        {
            List<VOrganizationSalary> organizationSalaries = new List<VOrganizationSalary>();
            var result = await UserUow.Repository<VOrganizationSalary>().Queryable().Where(x => x.OrganizationId == id).ToListAsync();
            return result;
        }

        public async Task<Response<object>> SaveOrganizationSalary(OrganizationSalary model)
        {
            Response<object> response = new Response<object>();
            model.Active = true;
            model.CreatedBy = UserClaim.UserId;
            model.CreatedDate = DateTime.Now;
            await UserUow.RegisterNewAsync<OrganizationSalary>(model);
            await UserUow.CommitAsync();
            response.IsSucceed = true;
            response.Message = "Salary Added Successfully";
            return response;
        }

        public async Task<Response<object>> UpdateOrganizationSalary(int id, OrganizationSalary model)
        {
            Response<object> response = new Response<object>();
            //var oldModel = await UserUow.Repository<OrganizationSalary>().Queryable().Where(x => x.OrganizationSalaryId == id).SingleOrDefaultAsync();
            //model.ModifyBy = UserClaim.UserId;
            //model.ModifyDate = DateTime.Now;
            await UserUow.RegisterDirtyAsync(model);
            await UserUow.CommitAsync();
            response.IsSucceed = true;
            response.Message = "Salary Updated Successfully";
            return response;
        }

        public async Task<OrganizationSalary> OrganizationSalaryById(int id)
        {
            OrganizationSalary organizationSalary = new OrganizationSalary();
            organizationSalary = await UserUow.Repository<OrganizationSalary>().Queryable().Where(x => x.OrganizationSalaryId == id).FirstOrDefaultAsync();
            return organizationSalary;
        }

        public async Task<List<PsaInput>> GetPSAInput(int id, int year)
        {
            List<PsaInput> organizationSalaries = new List<PsaInput>();
            try
            {
                var result = await UserUow.Repository<PsaInput>().Queryable().Where(x => x.OrganizationId == id && x.Year == year).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<Response<object>> UpdatePSAInput(int organizationid, IEnumerable<PsaInput> psaInputs)
        {
            Response<object> response = new Response<object>();

            var recordToAdd = psaInputs.Where(x => x.Psaid == 0);
            var recordToUpdate = psaInputs.Where(x => x.Psaid != 0);

            if (recordToAdd.Any())
            {
                await UserUow.RegisterNewAsync(recordToAdd);
            }
            if (recordToUpdate.Any())
            {
                await UserUow.RegisterDirtyAsync(recordToUpdate);
            }

            await UserUow.CommitAsync();
            response.IsSucceed = true;
            response.Message = "Monthly PSA inputs updated successfully";
            return response;
        }
    }

    public interface IOrganizationSalaryDomain
    {
        Task<List<VOrganizationSalary>> GetOrganizationSalary(int id);
        Task<OrganizationSalary> OrganizationSalaryById(int id);
        Task<Response<object>> SaveOrganizationSalary(OrganizationSalary model);
        Task<Response<object>> DeleteOrganizationSalary(int id, JsonPatchDocument Query);
        Task<Response<object>> UpdateOrganizationSalary(int id, OrganizationSalary organizationSalary);
        Task<List<PsaInput>> GetPSAInput(int id, int year);
        Task<Response<object>> UpdatePSAInput(int organizationid, IEnumerable<PsaInput> psaInputs);
    }
}
