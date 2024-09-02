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
    public partial class LoginContext : BaseBoundedContext, ILoginContext
    {
        public LoginContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public DbSet<User> User { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<User> UserToken { get; set; }
        public DbSet<UserForgotPwdToken> UserForgotPwdToken { get; set; }
        public DbSet<Vuser> Vuser { get; set; }


        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vuser>(entity =>
            {
                entity.ToView("vusers");
            });

            //OnModelCreatingPartial(modelBuilder);
        }
        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


    public interface ILoginContext : IDbContext
    {
    }
}

