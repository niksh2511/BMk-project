using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RxWeb.Core.Data;
using RxWeb.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class QuickBookSummaryDomain : IQuickBookSummaryDomain
    {
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        private IQBUow Uow { get; set; }
        public QuickBookSummaryDomain(IDbContextManager<MainSqlDbContext> dbContextManager, ILogException logException, IQBUow uow)
        {
            DbContextManager = dbContextManager;
            LogException = logException;
            Uow = uow;
        }

        public async Task<Response<IEnumerable<QBSummary>>> RetrieveQBSummary()
        {
            var response = new Response<IEnumerable<QBSummary>>();
            try
            {
                var spParameters = new SqlParameter[0];
                IEnumerable<QBSummary> result = await DbContextManager.StoreProc<QBSummary>("[dbo].spQBImportSummary", spParameters);
                response.IsSucceed = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/QuickBookSummary/RetrieveQBSummary");
                response.IsSucceed = false;
                response.Message = "Error occur during Retrieve QuickBook Summary";
            }
            return response;
        }

        public async Task<Response<OrganizationQbLog>> RetrieveOrganizationQBSummary(int OrganizationId)
        {
            var response = new Response<OrganizationQbLog> { IsSucceed = true };
            response.Data = new OrganizationQbLog();
            try
            {
                var organization = Uow.Repository<Organization>().FirstOrDefault(o => o.OrganizationId == OrganizationId);
                if (organization != null)
                    response.Data.OrganizationName = organization.Name;

                var latestDateLog = Uow.Repository<QbProcessLog>().FindBy(log => log.OrganizationId == OrganizationId)
                                       .Max(log => log.LogDate);

                response.Data.OrganizationLogs = Uow.Repository<QbProcessLog>()
                                     .FindBy(log => log.OrganizationId == OrganizationId && EF.Functions.DateDiffDay(log.LogDate, latestDateLog) == 0 && string.IsNullOrEmpty(log.ResponseStream))
                                     .OrderBy(log => log.LogDate)
                                     .Select(log => new QbLogs
                                     {
                                         LogInfo = log.LogDate == null ? "" : log.LogDate?.ToString("yyyy-MM-dd HH:mm:ss") + " - " + log.LogComments
                                     }).ToList();
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, $"api/QuickBookSummary/RetrieveOrganizationQBSummary/{OrganizationId}");
                response.IsSucceed = false;
                response.Message = $"Error occur during Retrieve QuickBook Summary for Organization";
            }
            return response;
        }

    }

    public interface IQuickBookSummaryDomain
    {
        Task<Response<IEnumerable<QBSummary>>> RetrieveQBSummary();
        Task<Response<OrganizationQbLog>> RetrieveOrganizationQBSummary(int OrganizationId);
    }
}
