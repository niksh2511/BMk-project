using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RxWeb.Core.Security;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace BMK.Infrastructure.Singleton
{
    public class UserAccessConfigInfo
    {
        public UserAccessConfigInfo()
        {
            AccessInfo = new ConcurrentDictionary<int, Dictionary<int, Dictionary<string, bool>>>();
            Tokens = new ConcurrentDictionary<string, string>();
        }
        private ConcurrentDictionary<int, Dictionary<int, Dictionary<string, bool>>> AccessInfo { get; set; }

        public ConcurrentDictionary<string, string> Tokens { get; set; }

        public async Task<Dictionary<int, Dictionary<string, bool>>> GetFullInfoAsync(int userId, ILoginUow loginUow)
        {
            Dictionary<int, Dictionary<string, bool>> modules = null;
            if (!AccessInfo.TryGetValue(userId, out modules))
            {
                await CacheAccessInfoAsync(loginUow, userId);
                AccessInfo.TryGetValue(userId, out modules);
            }
            return modules;
        }

        public async Task<bool> GetAccessInfoAsync(int userId, int applicationModuleId, string action, ILoginUow loginUow, bool cachedCall = false)
        {
            Dictionary<int, Dictionary<string, bool>> moduleIds;
            if (AccessInfo.TryGetValue(userId, out moduleIds))
            {
                Dictionary<string, bool> actionAccess;
                if (moduleIds.TryGetValue(applicationModuleId, out actionAccess))
                {
                    bool value;
                    if (actionAccess.TryGetValue(action, out value))
                        return value;
                }
            }
            else
            {
                if (!cachedCall)
                {
                    await CacheAccessInfoAsync(loginUow, userId);
                    return await GetAccessInfoAsync(userId, applicationModuleId, action, loginUow);
                }
            }
            return false;
        }

        public void SaveAccessInfo(int userId, Dictionary<int, Dictionary<string, bool>> value)
        {
            AccessInfo.AddOrUpdate(userId, value, (x, y) => value);
        }

        private async Task CacheAccessInfoAsync(ILoginUow loginUow, int userId)
        {
            var rolePermission = await (
                       from UserRole in loginUow.Repository<UserRole>().Queryable()
                       join RolePermission in loginUow.Repository<RolePermission>().Queryable()
                       on UserRole.RoleMasterId equals RolePermission.RoleMaserId // Filter by OrganizationID
                       where UserRole.UsersId == userId
                            && UserRole.Active == true
                            && RolePermission.Active == true
                       select new
                       {
                           RolePermission.RolePermissionView,
                           RolePermission.RolePermissionAdd,
                           RolePermission.RolePermissionUpdate,
                           RolePermission.RolePermissionDelete,
                           RolePermission.ModuleId
                       }
                   ).ToListAsync();
            //var moduleAccess = new Dictionary<int, Dictionary<string, bool>>();
            var moduleAccess = rolePermission.ToDictionary(
                access => (int)access.ModuleId,
                access => new Dictionary<string, bool>
                {
                    { GET, Convert.ToBoolean(access.RolePermissionView) },
                    { POST, Convert.ToBoolean(access.RolePermissionAdd) },
                    { PUT, Convert.ToBoolean(access.RolePermissionUpdate) },
                    { PATCH, Convert.ToBoolean(access.RolePermissionUpdate) },
                    { DELETE, Convert.ToBoolean(access.RolePermissionDelete) }
                }
            );

            foreach (var access in rolePermission)
            {
                var actionAccess = new Dictionary<string, bool>();
                actionAccess.Add(GET, access.RolePermissionView == true);
                actionAccess.Add(POST, access.RolePermissionAdd == true);
                actionAccess.Add(PUT, access.RolePermissionUpdate == true);
                actionAccess.Add(PATCH, access.RolePermissionUpdate == true);
                actionAccess.Add(DELETE, access.RolePermissionDelete == true);
                if (!moduleAccess.ContainsKey((int)access.ModuleId))
                    moduleAccess.Add((int)access.ModuleId, actionAccess);
            }
            SaveAccessInfo(userId, moduleAccess);
        }
        public async  Task<Dictionary<int, Dictionary<string, bool>>> FirstTimeCacheAccessInfoAsync(ILoginUow loginUow, int userId)
        {
            var rolePermission = await (
                       from UserRole in loginUow.Repository<UserRole>().Queryable()
                       join RolePermission in loginUow.Repository<RolePermission>().Queryable()
                       on UserRole.RoleMasterId equals RolePermission.RoleMaserId // Filter by OrganizationID
                       where UserRole.UsersId == userId
                            && UserRole.Active == true
                            && RolePermission.Active == true
                       select new
                       {
                           RolePermission.RolePermissionView,
                           RolePermission.RolePermissionAdd,
                           RolePermission.RolePermissionUpdate,
                           RolePermission.RolePermissionDelete,
                           RolePermission.ModuleId
                       }
                   ).ToListAsync();
            //var moduleAccess = new Dictionary<int, Dictionary<string, bool>>();
            var moduleAccess = rolePermission.ToDictionary(
                access => (int)access.ModuleId,
                access => new Dictionary<string, bool>
                {
                    { GET, Convert.ToBoolean(access.RolePermissionView) },
                    { POST, Convert.ToBoolean(access.RolePermissionAdd) },
                    { PUT, Convert.ToBoolean(access.RolePermissionUpdate) },
                    { PATCH, Convert.ToBoolean(access.RolePermissionUpdate) },
                    { DELETE, Convert.ToBoolean(access.RolePermissionDelete) }
                }
            );
             Task.Run(() => SaveAccessInfo(userId, moduleAccess));
            return moduleAccess;
        }

        public async Task<string> GetTokenAsync(string securityKey, ILoginUow loginUow)
        {
            string token;
            if (!Tokens.TryGetValue(securityKey, out token))
            {
                var applicationUserToken = await loginUow.Repository<UserToken>().SingleOrDefaultAsync(t => t.SecurityKey == securityKey);
                if (applicationUserToken != null)
                    Tokens.AddOrUpdate(applicationUserToken.SecurityKey, applicationUserToken.AuthToken, (x, y) => applicationUserToken.AuthToken);
                return applicationUserToken == null ? string.Empty : applicationUserToken.AuthToken;
            }
            return token;
        }

        public async Task SaveTokenAsync(int userId, string audience, KeyValuePair<string, string> token, ILoginUow loginUow)
        {
            await RemoveTokenAsync(userId, loginUow);
            var userToken = new UserToken
            {
                CreatedDate = DateTime.UtcNow,
                UsersId = userId,
                AuthToken = token.Value,
                CreatedBy = userId,
                Active = true,
                SecurityKey = token.Key,
            };
            await loginUow.RegisterNewAsync<UserToken>(userToken);
            await loginUow.CommitAsync();
            Tokens.AddOrUpdate(token.Key, token.Value, (x, y) => token.Value);
        }

        public async Task RemoveTokenAsync(int userId, ILoginUow loginUow)
        {
            var applicationUserTokens = await loginUow.Repository<UserToken>().FindByAsync(t => t.UsersId == userId);
            foreach (var applicationUserToken in applicationUserTokens)
            {
                await loginUow.RegisterDeletedAsync<UserToken>(applicationUserToken);
                string token;
                Tokens.TryRemove(applicationUserToken.SecurityKey, out token);
            }
            Dictionary<int, Dictionary<string, bool>> moduleIds;
            AccessInfo.TryRemove(userId, out moduleIds);
            await loginUow.CommitAsync();

        }

        const string GET = "get";

        const string POST = "post";

        const string PUT = "put";

        const string PATCH = "patch";

        const string DELETE = "delete";
    }
}

