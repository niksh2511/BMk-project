using BMK.BoundedContext.SqlDbContext;
using BMK.Models.DbEntities;
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
    public class ObjectiveContext : BaseBoundedContext, IObjectiveContext
    {
        public ObjectiveContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public virtual DbSet<Objective> Objectives { get; set; }

        public virtual DbSet<ObjectiveComment> ObjectiveComments { get; set; }

        public DbSet<VUserGroup> Vuser { get; set; }

        #endregion DbSets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VUserGroup>(entity =>
            {
                entity.ToView("vUserGroups");
            });

        }
    }

    public interface IObjectiveContext : IDbContext
    {
    }
}
