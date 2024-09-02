using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.Data.SqlClient;
using Microsoft.Graph.Models;

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
    public class BmkTargetsDomain : IBmkTargetsDomain
    {
        public IUserUow UserUow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        public BmkTargetsDomain(IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException)
        {
            UserUow = userUow;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;
            LogException = logException;
        }


        public async Task<List<BmkTarget>> SaveTargets(List<BmkTarget> model)
        {
            foreach (var item in model)
            {
                item.Active = true;
                if (item.BmkTargetId > 0)
                {

                    await UserUow.RegisterDirtyAsync<BmkTarget>(item);
                }
                else
                {
                    await UserUow.RegisterNewAsync<BmkTarget>(item);
                }
            }
            await UserUow.CommitAsync();


            return model;
        }

        public async Task<List<ReportHead>> GetReportData(int organizationID, int year, int deptId)
        {
            var result = new List<ReportHead>();
            try
            {
                var spParameters = new SqlParameter[3];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = organizationID };
                spParameters[1] = new SqlParameter() { ParameterName = "year", Value = year };
                spParameters[2] = new SqlParameter() { ParameterName = "catID", Value = deptId };
                var resultobj = await DbContextManager.StoreProc<SpResult>("[dbo].spGetBMKTargetReport", spParameters);
                if (resultobj.SingleOrDefault() != null)
                {
                    result = JsonConvert.DeserializeObject<List<ReportHead>>(resultobj.SingleOrDefault().Data);
                }

            }
            catch (Exception ex) { }
            return result;
        }

    }
    public interface IBmkTargetsDomain
    {

        Task<List<BmkTarget>> SaveTargets(List<BmkTarget> model);
        Task<List<ReportHead>> GetReportData(int organizationID, int year, int deptId);


    }
    public class BmkTargetReportModel
    {
        public List<ReportHead> DataList { get; set; }
    }
    public class ReportHead
    {
        public string rptHead { get; set; }
        public List<MonthData> data { get; set; }
    }
    public class MonthData
    {
        public string rptLine { get; set; }
        public int rptSeqs { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }
        public string Total { get; set; }
        public string Avg { get; set; }
        public string GroupAvg { get; set; }
        public string BMKAvg { get; set; }
        public bool isSeparater { get; set; }
        public bool isBgColor { get; set; }
        public string ToolTip { get; set; }
    }
}
