using BMK.BoundedContext.SqlDbContext;
using BMK.Models.DbEntities;
using BMK.Models.DbEntities.Main;
using BMK.Models.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RxWeb.Core.Data;
using RxWeb.Core.Data.BoundedContext;
using RxWeb.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.BoundedContext.DbContext.Main
{
    public class PeerTeamContext : BaseBoundedContext, IPeerTeamContext
    {
        public PeerTeamContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public virtual DbSet<GroupType> GroupTypes { get; set; }

        public virtual DbSet<UserGroup> UserGroups { get; set; }

        public virtual DbSet<UserGroupsMember> UserGroupsMembers { get; set; }

        public DbSet<VUserGroup> Vuser { get; set; }
        public DbSet<VUserGroupsMember> vUserGroupsMember { get; set; }
        public DbSet<vOrgRevenue> vOrgRevenue { get; set; }
        

        #endregion DbSets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VUserGroup>(entity =>
            {
                entity.ToView("vUserGroups");
            });

            modelBuilder.Entity<VUserGroupsMember>(entity =>
            {
                entity.ToView("vUserGroupsMember");
            });

        }
    }


    public interface IPeerTeamContext : IDbContext
    {
    }
}
