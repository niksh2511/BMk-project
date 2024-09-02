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
    public class EventContext : BaseBoundedContext, IEventContext
    {
        public EventContext(MainSqlDbContext sqlDbContext, IOptions<DatabaseConfig> databaseConfig, IHttpContextAccessor contextAccessor, ITenantDbConnectionInfo tenantDbConnection) : base(sqlDbContext, databaseConfig.Value, contextAccessor, tenantDbConnection) { }

        #region DbSets
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserGroupsMember> UserGroupsMember { get; set; }
        public virtual DbSet<GroupType> GroupTypes { get; set; }
        public virtual DbSet<CategoryGroup> CategoryGroups { get; set; }
        public virtual DbSet<EventCategory> EventCategories { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<VCategoryGroup> vCategoryGroup { get; set; }

        #endregion DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VCategoryGroup>(entity =>
            {
                entity.ToView("vCategoryGroups");
            });
        }
    }


    public interface IEventContext : IDbContext
    {
    }
}

