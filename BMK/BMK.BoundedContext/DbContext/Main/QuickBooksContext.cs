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
    public class QuickBooksContext : BaseBoundedContext, IQuickBooksContext
    {
        public QuickBooksContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public DbSet<QbTokenDetail> QbTokenDetails { get; set; }
        public DbSet<QbExceptionLog> QbExceptionLogs { get; set; }
        public virtual DbSet<QbProcessLog> QbProcessLogs { get; set; }
        public virtual DbSet<Vuser> Vuser { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vuser>(entity =>
            {
                entity.ToView("vusers");
            });

            //
            modelBuilder.Entity<QbProcessLog>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
        }
    }


    public interface IQuickBooksContext : IDbContext
    {
    }
}

