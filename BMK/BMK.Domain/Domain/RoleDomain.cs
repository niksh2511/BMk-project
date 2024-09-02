using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
    public class RoleDomain : IRoleDomain
    {
        public IUserUow UserUow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        public RoleDomain(IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException)
        {
            UserUow = userUow;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;
            LogException = logException;
        }
        public async Task<List<RolePermission>> GetRolePermission(int id)
        {
            var spParameters = new SqlParameter[1];
            List<RolePermission> rolePermission = new List<RolePermission>();
            spParameters[0] = new SqlParameter() { ParameterName = "roleMaserID", Value = id };
            var result = await DbContextManager.StoreProc<SpResult>("[dbo].spGetRolePermission", spParameters);
            if (result != null && result.SingleOrDefault().Result != null)
            {
                var res = result.SingleOrDefault().Result;
                rolePermission = JsonConvert.DeserializeObject<List<RolePermission>>(res);
            }
            else
            {
                rolePermission = new List<RolePermission>();

            }
            return rolePermission;
        }

        public async Task<RolePermissionModel> SaveRolePermission(RolePermissionModel model)
        {
            RoleMaster newmodel = new RoleMaster();
            newmodel.RoleName = model.Name;
            newmodel.RoleDesc = model.Desc;
            newmodel.Active = true;
            newmodel.RoleMasterId = model.RoleId;

            var roleExits = await UserUow.Repository<RoleMaster>().Queryable().Where(x => x.RoleMasterId == model.RoleId).FirstOrDefaultAsync();
            if (roleExits != null)
            {
                newmodel.CreatedBy = roleExits.CreatedBy;
                newmodel.CreatedDate = roleExits.CreatedDate;
                newmodel.ModifiedBy = UserClaim.UserId;
                newmodel.ModifiedDate = DateTime.Now;
                await UserUow.RegisterDirtyAsync<RoleMaster>(newmodel);
            }
            else
            {
                newmodel.CreatedBy = UserClaim.UserId;
                newmodel.CreatedDate = DateTime.Now;
                await UserUow.RegisterNewAsync<RoleMaster>(newmodel);
            }
            await UserUow.CommitAsync();

            foreach (var item in model.rolePermissions)
            {
                item.Active = true;
                item.RoleMaserId = newmodel.RoleMasterId;

                var obj = await UserUow.Repository<RolePermission>().Queryable().Where(x => x.RolePermissionId == item.RolePermissionId).FirstOrDefaultAsync();

                if (obj != null)
                {
                    item.CreatedBy = obj.CreatedBy;
                    item.CreatedDate = obj.CreatedDate;
                    item.ModifiedBy = UserClaim.UserId;
                    item.ModifiedDate = DateTime.Now;
                    item.Active = item.RolePermissionView == null || item.RolePermissionView == false ? false : true;
                    await UserUow.RegisterDirtyAsync<RolePermission>(item);
                }
                else
                {
                    item.CreatedBy = UserClaim.UserId;
                    item.CreatedDate = DateTime.Now;
                    if (item.RolePermissionView == true)
                    {
                        await UserUow.RegisterNewAsync<RolePermission>(item);

                    }
                }
            }
            await UserUow.CommitAsync();


            return model;
        }

        public async Task<Response<object>> DeleteRole(int id)
        {
            Response<object> response = new Response<object>();
            var data = await UserUow.Repository<RolePermission>().Queryable().Where(x => x.RoleMaserId == id).ToListAsync();

            var userExists = await UserUow.Repository<UserRole>().Queryable().Where(x => x.RoleMasterId == id).AnyAsync();
            if (userExists)
            {
                response.IsSucceed = false;
                response.Message = "We can not delete this Role.Dependent data found.";
            }
            else
            {

                foreach (var item in data)
                {
                    await UserUow.RegisterDeletedAsync<RolePermission>(item);
                }

                await UserUow.CommitAsync();

                var role = await UserUow.Repository<RoleMaster>().Queryable().Where(x => x.RoleMasterId == id).FirstOrDefaultAsync();

                await UserUow.RegisterDeletedAsync<RoleMaster>(role);
                await UserUow.CommitAsync();

                response.IsSucceed = true;
                response.Message = "Role Deleted Successfully";
            }

            return response;
        }
    }
    public interface IRoleDomain
    {
        Task<List<RolePermission>> GetRolePermission(int id);
        Task<RolePermissionModel> SaveRolePermission(RolePermissionModel model);

        Task<Response<object>> DeleteRole(int id);

    }
}
