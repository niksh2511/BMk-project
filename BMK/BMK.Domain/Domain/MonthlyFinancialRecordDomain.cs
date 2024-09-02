using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Infrastructure.Model;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.TermStore;
using Newtonsoft.Json;
using RxWeb.Core.Data;
using RxWeb.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Fax;
using static System.Net.Mime.MediaTypeNames;

namespace BMK.Domain.Domain
{
    public class MonthlyFinancialRecordDomain : IMonthlyFinancialRecordDomain
    {
        private IAccountMappingUow Uow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        private IQuickBookDomain QuickBookDomain { get; set; }
        private IAccountMappingDomain AccountMappingDomain { get; set; }
        public MonthlyFinancialRecordDomain(IAccountMappingUow uow, IUserClaim userClaim, IDbContextManager<MainSqlDbContext> dbContextManager, ILogException logException, IQuickBookDomain quickBookDomain, IAccountMappingDomain accountMappingDomain)
        {
            Uow = uow;
            UserClaim = userClaim;
            DbContextManager = dbContextManager;
            LogException = logException;
            QuickBookDomain = quickBookDomain;
            AccountMappingDomain = accountMappingDomain;
        }

        public async Task<Response<MonthlyFinancialRecord>> Retrieve(int month, int year)
        {
            Response<MonthlyFinancialRecord> response = new Response<MonthlyFinancialRecord>();
            try
            {
                var spParameters = new SqlParameter[3];
                MonthlyFinancialRecord monthlyFinanicalRecord = new MonthlyFinancialRecord();
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "month", Value = month };
                spParameters[2] = new SqlParameter() { ParameterName = "year", Value = year };
                var result = await DbContextManager.StoreProc<SpResult>("[dbo].spRetrieveMonthlyFinanicalRecord", spParameters);
                if (result != null)
                {
                    var res = result.SingleOrDefault().Result;
                    if (res != null)
                    {
                        monthlyFinanicalRecord = JsonConvert.DeserializeObject<MonthlyFinancialRecord>(res);
                    }
                }
                response.IsSucceed = true;
                response.Data = monthlyFinanicalRecord;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/MonthlyFinancialRecord/retrive");
                response.IsSucceed = false;
                response.Message = "Error occur during retrive monthly finanical record";
            }
            return response;
        }

        public async Task<List<MasterFinancialType>> GetMapMasterFinancialType()
        {
            return await Uow.Repository<QbMapMasterFinRecord>().Queryable().Select(r => new MasterFinancialType { Id = r.QbMapMasterFinRecordId, Name = r.ExternalName }).ToListAsync();
        }
        public async Task<Response<List<ManualEntryCalendar>>> GetManualEntryCalendar()
        {
            Response<List<ManualEntryCalendar>> response = new Response<List<ManualEntryCalendar>>();
            try
            {
                var spParameters = new SqlParameter[2];
                List<ManualEntryCalendar> manualEntryCalendar = new List<ManualEntryCalendar>();
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "@userID", Value = UserClaim.UserId };
                var result = await DbContextManager.StoreProc<SpResult>("[dbo].spIECalendarData", spParameters);
                if (result != null)
                {
                    var res = result.SingleOrDefault().Result;
                    if (res != null)
                    {
                        manualEntryCalendar = JsonConvert.DeserializeObject<List<ManualEntryCalendar>>(res);
                    }
                }
                response.IsSucceed = true;
                response.Data = manualEntryCalendar;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/MonthlyFinancialRecord/GetManualEntryCalendar");
                response.IsSucceed = false;
                response.Message = "Error occur during retrive monthly finanical for calendar";
            }
            return response;
        }
        public async Task<Response<object>> Add(MonthlyFinancialRecord monthlyFinancialRecord)
        {
            string url = "api/MonthlyFinancialRecord/add";
            int userId = UserClaim.UserId;
            var currentDate = DateTime.Now;
            var response = new Response<object>();
            try
            {
                List<QbMapMonthlyFinRecord> qbMapMonthlyFinRecordList = setUpQbMapMasterFinRecord(monthlyFinancialRecord);
                if (qbMapMonthlyFinRecordList != null && qbMapMonthlyFinRecordList.Any())
                {

                    foreach (var monthlyFinRecord in qbMapMonthlyFinRecordList)
                    {
                        if (monthlyFinRecord.QbMapMonthlyFinRecordId == 0)
                        {
                            monthlyFinRecord.CreatedBy = userId;
                            monthlyFinRecord.CreatedDate = currentDate;
                            await Uow.RegisterNewAsync(monthlyFinRecord);
                        }
                        else if (monthlyFinRecord.QbMapMonthlyFinRecordId > 0)
                        {
                            monthlyFinRecord.ModifiedBy = userId;
                            monthlyFinRecord.ModifiedDate = currentDate;
                            await Uow.RegisterDirtyAsync(monthlyFinRecord);
                        }
                    }

                    await Uow.CommitAsync();
                    await OnMonthlyFinancialRecordChange(url);
                    response.Message = "Successfully saved monthly financial record";
                }
                else
                {
                    response.Message = "Make some changes monthly financial record to save";
                }
                response.IsSucceed = true;
                response.Message = "Successfully saved monthly financial record";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, url);
                response.IsSucceed = false;
                response.Message = "Error occur during saving monthly financial record";
            }
            return response;
        }

        private List<QbMapMonthlyFinRecord> setUpQbMapMasterFinRecord(MonthlyFinancialRecord monthlyFinancialRecord)
        {

            return monthlyFinancialRecord.parentCategory
                .Where(parentCategory => parentCategory?.sections != null && parentCategory.sections.Any())
                .SelectMany(parentCategory => parentCategory.sections)
                .Where(section => section?.childCategory != null && section.childCategory.Any())
                .SelectMany(section => section.childCategory)
                .Where(category => category != null)
                .Select(category => new QbMapMonthlyFinRecord
                {
                    OrganizationId = UserClaim.OrganizationId,
                    Active = true,
                    IsImported = monthlyFinancialRecord.isImported,
                    MonthYear = new DateOnly(monthlyFinancialRecord.Year, monthlyFinancialRecord.Month, 01),
                    Amount = Convert.ToDecimal(category.amount),
                    QbMapMasterFinRecordId = category.childCategoryId,
                    QbMapMonthlyFinRecordId = category.qbMapMonthlyFinRecordId,
                    CreatedBy = UserClaim.UserId,
                    CreatedDate = DateTime.Now
                })
                .ToList();

        }

        private async Task OnMonthlyFinancialRecordChange(string url)
        {
            await AccountMappingDomain.ReProcessImportData(url);
            await AccountMappingDomain.UpdateBMKTargetReport(url);
        }

    }
    public interface IMonthlyFinancialRecordDomain
    {
        Task<Response<MonthlyFinancialRecord>> Retrieve(int month, int year);
        Task<List<MasterFinancialType>> GetMapMasterFinancialType();
        Task<Response<object>> Add(MonthlyFinancialRecord monthlyFinancialRecord);
        Task<Response<List<ManualEntryCalendar>>> GetManualEntryCalendar();
    }
}
