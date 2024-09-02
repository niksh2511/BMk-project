using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BMK.Models;
using BMK.BoundedContext.Singleton;
using RxWeb.Core.Data;
using RxWeb.Core.Data.Models;
using RxWeb.Core.Data.BoundedContext;
using BMK.Models.DbEntities;
using BMK.BoundedContext.SqlDbContext;

namespace BMK.BoundedContext.DbContext.Main
{
    public class AccountMappingContext : BaseBoundedContext, IAccountMappingContext
    {
        public AccountMappingContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public virtual DbSet<QbExceptionLog> QbExceptionLogs { get; set; }

        public virtual DbSet<QbMapAccountCategory> QbMapAccountCategories { get; set; }

        public virtual DbSet<QbMapMasterAccountList> QbMapMasterAccountLists { get; set; }

        public virtual DbSet<QbMapMasterFinRecord> QbMapMasterFinRecords { get; set; }

        public virtual DbSet<QbMapMonthlyFinRecord> QbMapMonthlyFinRecords { get; set; }

        public virtual DbSet<QbOrgAccountBalance> QbOrgAccountBalances { get; set; }

        public virtual DbSet<QbOrgAccountList> QbOrgAccountLists { get; set; }

        public virtual DbSet<QbOrgAccountMapping> QbOrgAccountMappings { get; set; }
        public virtual DbSet<QbProcessLog> QbProcessLogs { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<QbTokenDetail>(entity =>
            //{
            //    entity.ToView("qbTokenDetail");
            //});

        }
    }


    public interface IAccountMappingContext : IDbContext
    {
    }
}

