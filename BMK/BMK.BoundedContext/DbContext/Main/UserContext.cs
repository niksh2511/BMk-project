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
    public class UserContext : BaseBoundedContext, IUserContext
    {
        public UserContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMaster> RoleMasters { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<BmkTarget> BmkTarget { get; set; }
        public DbSet<Vuser> Vuser { get; set; }
        public DbSet<VOrganizationSalary> VOrganizationSalary { get; set; }
        public DbSet<OrganizationSalary> OrganizationSalaries { get; set; }
        public DbSet<PsaInput> PsaInput { get; set; }
        public DbSet<BmkMemberMeeting> BmkMemberMeetings { get; set; }
        public DbSet<QbMapAccountCategory> QbMapAccountCategory { get; set; }
        
        public virtual DbSet<VOrganizationForPeerTeamProfile> VOrganizationForPeerTeamProfiles { get; set; }
        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vuser>(entity =>
            {
                entity.ToView("vusers");
            });
            modelBuilder.Entity<Vorganizaion>(entity =>
            {
                entity.ToView("vorganizaions");
            });
            modelBuilder.Entity<VOrganizationForPeerTeamProfile>(entity =>
            {
                entity.ToView("vOrganizationForPeerTeamProfile");
            });
        }
    }


    public interface IUserContext : IDbContext
    {
    }
}

