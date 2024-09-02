using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.DbEntities.Main;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using RxWeb.Core.Data;
using RxWeb.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Graph.Constants;

namespace BMK.Domain.Domain
{
    public class AccountMappingDomain : IAccountMappingDomain
    {
        private IAccountMappingUow Uow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        private Response<List<AccountMapping>> Response { get; set; }
        public AccountMappingDomain(IAccountMappingUow uow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException)
        {
            Uow = uow;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;
            LogException = logException;
            Response = new Response<List<AccountMapping>>();
        }
        public async Task<IEnumerable<AccountMappingType>> GetAccountMappingType()
        {
            var spParameters = new SqlParameter[0];
            return await DbContextManager.StoreProc<AccountMappingType>("[dbo].spGetAccountType", spParameters);
        }

        public async Task<List<AccountMapping>> GetAccountMapping()
        {
            var spParameters = new SqlParameter[1];
            List<AccountMapping> accountMapping = new List<AccountMapping>();
            spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
            var result = await DbContextManager.StoreProc<SpResult>("[dbo].spGetAccountMappingList", spParameters);
            if (result != null)
            {
                var res = result.SingleOrDefault().Result;
                if (res != null)
                {
                    accountMapping = JsonConvert.DeserializeObject<List<AccountMapping>>(res);
                }
            }
            return accountMapping;
        }

        public async Task<Response<List<AccountMapping>>> SaveAccountMapping(IEnumerable<QbOrgAccountMapping> orgAccountMappingList)
        {
            try
            {
                int userId = UserClaim.UserId;
                var currentDate = DateTime.Now;
                var isDeleted = await RemoveMappingFlagChangedOrgAccountMapping(orgAccountMappingList);
                foreach (var accountMapping in orgAccountMappingList)
                {
                    if (accountMapping.QbOrgAccountMappingId == 0)
                    {
                        accountMapping.CreatedBy = userId;
                        accountMapping.CreatedDate = currentDate;
                        await Uow.RegisterNewAsync(accountMapping);
                    }
                    else if (accountMapping.QbOrgAccountMappingId > 0)
                    {
                        accountMapping.ModifiedBy = userId;
                        accountMapping.ModifiedDate = currentDate;
                        await Uow.RegisterDirtyAsync(accountMapping);
                    }
                }
                await Uow.CommitAsync();
                Response.IsSucceed = true;
                Response.Message = "Account mappings have been saved successfully";

            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/AccountMapping/saveAccountMapping");
                Response.IsSucceed = false;
                Response.Message = "Error occur during saving account mapping";
            }
            Response.Data = await GetAccountMapping();
            return Response;
        }

        public async Task<Response<object>> ReProcessImportData(string url = null)
        {
            Response<object> response = new Response<object>();
            try
            {
                var spParameters = new SqlParameter[2];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                await DbContextManager.StoreProc<object>("[dbo].spQBMasterMonthlyFinUpdate", spParameters);
                response.IsSucceed = true;
                response.Message = "Re-processed import data successfully";
                await UpdateBMKTargetReport("api/AccountMapping/reProcessImportData");
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, url ?? "api/AccountMapping/reProcessImportData");
                response.IsSucceed = false;
                response.Message = "Error Occur during re-processing data";
            }
            return response;
        }

        private async Task<bool> RemoveMappingFlagChangedOrgAccountMapping(IEnumerable<QbOrgAccountMapping> orgAccountMappingList)
        {
            var distinctQbOrgAccountListIDs = orgAccountMappingList.Where(a => a.IsMappingFlagChanged).Select(a => a.QbOrgAccountListId).Distinct().AsEnumerable();
            if (distinctQbOrgAccountListIDs.Count() > 0)
            {
                var removeAccountMappings = (
                        from accountMapping in Uow.Repository<QbOrgAccountMapping>().Queryable()
                        join accountList in Uow.Repository<QbOrgAccountList>().Queryable().Where(d => d.OrganizationId == UserClaim.OrganizationId && distinctQbOrgAccountListIDs.Contains(d.QbOrgAccountListId))
                        on accountMapping.QbOrgAccountListId equals accountList.QbOrgAccountListId// Filter by OrganizationID
                        select accountMapping
                    );
                var result = removeAccountMappings.AsEnumerable();
                await Uow.RegisterDeletedAsync(result);
                await Uow.CommitAsync();
            }
            return true;
        }

        public async Task UpdateBMKTargetReport(string url)
        {
            try
            {
                var spParameters = new SqlParameter[2];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                await DbContextManager.StoreProc<object>("[dbo].spUpdateBMKTargetReport", spParameters);
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, url);
            }
        }
        public async Task<int> RetriveUnMapAccount()
        {
            try
            {
                var organizationId = UserClaim.OrganizationId;

                var unmappedCount = (from a in Uow.Repository<QbOrgAccountList>().Queryable()
                                     where a.OrganizationId == organizationId
                                     join m in Uow.Repository<QbOrgAccountMapping>().Queryable()
                                     on a.QbOrgAccountListId equals m.QbOrgAccountListId into gj
                                     from subm in gj.DefaultIfEmpty()
                                     where subm == null
                                     && Uow.Repository<QbOrgAccountBalance>().Queryable()
                                         .Any(b => b.QbOrgAccountListId == a.QbOrgAccountListId && b.QbBalanceAmount != 0)
                                     group a by a.OrganizationId into g
                                     select new
                                     {
                                         OrganizationId = g.Key,
                                         UnmappedCount = g.Count()
                                     }).FirstOrDefault();

                return unmappedCount != null ? unmappedCount.UnmappedCount : 0;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/AccountMapping/retriveUnMapAccount");
                return 0;
            }
        }
    }
    public interface IAccountMappingDomain
    {
        Task<IEnumerable<AccountMappingType>> GetAccountMappingType();
        Task<List<AccountMapping>> GetAccountMapping();
        Task<Response<List<AccountMapping>>> SaveAccountMapping(IEnumerable<QbOrgAccountMapping> orgAccountMappingList);
        Task<Response<object>> ReProcessImportData(string url = null);
        Task<int> RetriveUnMapAccount();
        Task UpdateBMKTargetReport(string url);
    }
}
