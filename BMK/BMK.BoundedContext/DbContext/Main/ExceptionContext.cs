using BMK.BoundedContext.SqlDbContext;
using BMK.Models.DbEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RxWeb.Core.Data;
using RxWeb.Core.Data.BoundedContext;
using RxWeb.Core.Data.Models;

namespace BMK.BoundedContext.DbContext.Main
{
    public partial class ExceptionContext : BaseBoundedContext, IExceptionContext
    {
        public ExceptionContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public DbSet<ExceptionLog> Exceptions { get; set; }


        #endregion DbSets

        
    }


    public interface IExceptionContext : IDbContext
    {
    }
}

