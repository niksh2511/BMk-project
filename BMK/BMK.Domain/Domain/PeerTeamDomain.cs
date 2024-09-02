using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.Models;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

using RxWeb.Core.Data;
using RxWeb.Core.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GroupType = BMK.Models.DbEntities.GroupType;
using User = BMK.Models.DbEntities.User;

namespace BMK.Domain.Domain
{
    public class PeerTeamDomain : IPeerTeamDomain
    {
        private IPeerTeamUow PeerTeamUow { get; set; }
        private IUserUow UserUow { get; set; }
        private ILogException LogException { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private IUserClaim UserClaim { get; set; }
        private Response<List<GroupType>> GroupTypeResponse { get; set; }
        public PeerTeamDomain(IPeerTeamUow peerTeamUow, ILogException logException, IUserClaim userClaim, IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager)
        {
            PeerTeamUow = peerTeamUow;
            LogException = logException;
            UserClaim = userClaim;
            UserUow = userUow;
            DbContextManager = dbContextManager;

        }

        public async Task<IEnumerable<GroupType>> GetGroupTypes()
        {

            return await PeerTeamUow.Repository<GroupType>().AllAsync();
            //try
            //{
            //    return await PeerTeamUow.Repository<GroupType>().AllAsync();
            //}
            //catch(Exception ex)
            //{
            //    await LogException.Log(ex, "api/PeerTeam/GetGroupTypes");
            //    //Response.IsSucceed = false;
            //    //Response.Message = "Error occur during saving account mapping";
            //    return await 
            //}
        }

        public async Task<Response<GroupType>> CreateGroupType(GroupType groupType)
        {
            var GroupTypeResponse = new Response<GroupType>();
            try
            {
                await PeerTeamUow.RegisterNewAsync(groupType);
                await PeerTeamUow.CommitAsync();
                GroupTypeResponse.IsSucceed = true;
                GroupTypeResponse.Message = "Successfully created Group Type.";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/PeerTeam/CreateGroupType");
                GroupTypeResponse.IsSucceed = false;
                GroupTypeResponse.Message = "Error occur during saving account mapping";
            }
            //GroupTypeResponse.Data = await GetAccountMapping();
            return GroupTypeResponse;
        }

        public async Task<GroupType> GetGroupTypeById(int id)
        {
            return await PeerTeamUow.Repository<GroupType>().FindByKeyAsync(id);
        }

        public async Task<IEnumerable<UserGroup>> GetUserGroups()
        {
            return await PeerTeamUow.Repository<UserGroup>().Queryable().Where(x => x.Active == true).ToListAsync();
        }

        public async Task<UserGroup> GetUserGroupById(int id)
        {
            var group = await PeerTeamUow.Repository<UserGroup>().FindByKeyAsync(id);
            if (group != null)
            {
                group.UserGroupsMembers = (ICollection<UserGroupsMember>)await GetMembersByGroupId(id);
            }
            return group;
        }

        public async Task<Response<UserGroup>> CreateUserGroup(UserGroup group)
        {
            var response = new Response<UserGroup>();
            try
            {
                group.CreatedBy = UserClaim.UserId;
                group.Active = true;
                group.CreatedDate = DateTime.Now;

                if (group.UserGroupsMembers.Count > 0)
                {
                    foreach (var member in group.UserGroupsMembers)
                    {
                        member.CreatedBy = UserClaim.UserId;
                        member.Active = true;
                        member.CreatedDate = DateTime.Now;
                    }
                }


                await PeerTeamUow.RegisterNewAsync(group);
                var intres = await PeerTeamUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Created User Group Successfully";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/PeerTeams/CreateUserGroup");
                response.IsSucceed = false;
                response.Message = "failed to create User Group";
            }
            response.Data = group;
            return response;
        }

        public async Task<Response<UserGroup>> AddUserToGroup(int userId, int groupId)
        {
            //test this
            var response = new Response<UserGroup>();
            var currentUser = UserClaim.UserId;

            if (currentUser == 10000 || currentUser == 10003)
            {
                try
                {
                    var group = await GetUserGroupById(groupId);
                    var user = UserUow.Repository<User>().FindByKey(userId);
                    if (group != null)
                    {
                        if (user != null)
                        {
                            UserGroupsMember member = new UserGroupsMember();
                            member.UserGroupsId = group.UserGroupsId;
                            member.UsersId = userId;
                            member.CreatedBy = currentUser;
                            member.CreatedDate = DateTime.Now;


                            await PeerTeamUow.RegisterNewAsync(member);
                            await PeerTeamUow.CommitAsync();

                            response.IsSucceed = true;
                            response.Data = group;
                        }
                        else
                        {
                            response.IsSucceed = false;
                            response.Message = "User not found";
                        }
                    }
                    else
                    {
                        response.IsSucceed = false;
                        response.Message = "Group not found";
                    }
                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/PeerTeams/AddUserToGroup");
                    response.IsSucceed = false;
                    response.Message = "Could not save user to group";
                }
            }

            else
            {
                response.IsSucceed = false;
                response.Message = "Unauthorized.";
            }

            return response;
        }
        public async Task<IEnumerable<VUserGroupsMember>> GetGroupsByUserId(int UsersId)
        {
            return await PeerTeamUow.Repository<VUserGroupsMember>().FindByAsync(x => x.UsersId == UsersId);
        }
        public async Task<IEnumerable<UserGroupsMember>> GetMembersByGroupId(int groupId)
        {
            return await PeerTeamUow.Repository<UserGroupsMember>().FindByAsync(x => x.UserGroupsId == groupId);
        }

        public async Task<Response<UserGroup>> UpdateUserGroup(int id, UserGroup group)
        {
            var response = new Response<UserGroup>();
            try
            {
                //group.UserGroupsId = id;
                group.ModifiedBy = UserClaim.UserId;
                group.Active = true;
                group.ModifiedDate = DateTime.Now;


                var existingUsers = await GetMembersByGroupId(group.UserGroupsId);
                if (existingUsers.Count() > 0)
                {
                    foreach (var user in existingUsers)
                    {
                        await PeerTeamUow.RegisterDeletedAsync(user);
                    }
                    await PeerTeamUow.CommitAsync();
                }


                foreach (var member in group.UserGroupsMembers)
                {
                    member.CreatedBy = UserClaim.UserId;
                    member.Active = true;
                    member.CreatedDate = DateTime.Now;
                }

                await PeerTeamUow.RegisterDirtyAsync(group);
                var intres = await PeerTeamUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Updated User Group Successfully";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/PeerTeams/UpdateUserGroup");
                response.IsSucceed = false;
                response.Message = "failed to update User Group";
            }
            response.Data = group;
            return response;
        }

        public async Task<Response<UserGroup>> DeleteUserGroup(int id)
        {
            var response = new Response<UserGroup>();
            var existingGroup = await GetUserGroupById(id);
            if (existingGroup == null)
            {
                response.IsSucceed = false;
                response.Message = "GroupNotFound";
            }
            else
            {
                try
                {
                    existingGroup.Active = false;
                    existingGroup.ModifiedBy = UserClaim.UserId;
                    existingGroup.ModifiedDate = DateTime.Now;

                    if (existingGroup.UserGroupsMembers.Count > 0)
                    {
                        foreach (var member in existingGroup.UserGroupsMembers)
                        {
                            member.Active = false;
                            member.ModifiedBy = UserClaim.UserId;
                            member.ModifiedDate = DateTime.Now;
                            await PeerTeamUow.RegisterDirtyAsync(member);
                        }
                    }

                    await PeerTeamUow.RegisterDirtyAsync(existingGroup);
                    var intres = await PeerTeamUow.CommitAsync();
                    response.IsSucceed = true;
                    response.Message = "Updated User Group Successfully";
                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/PeerTeams/DeleteUserGroup");
                    response.IsSucceed = false;
                    response.Message = "failed to delete User Group";
                }
            }

            return response;
        }

        public async Task<IEnumerable<UserGroup>> GetUserGroupsByMemberId(int id)
        {
            var groups = await PeerTeamUow.Repository<UserGroupsMember>().Queryable().Where(x => x.UsersId == id).Select(x => x.UserGroups).Where(x => x.Active == true).ToListAsync();

            foreach (var grp in groups)
            {
                if (grp != null)
                {
                    grp.UserGroupsMembers = (ICollection<UserGroupsMember>)await GetMembersByGroupId(grp.UserGroupsId);

                }
            }
            return groups;
        }

        public async Task<IEnumerable<MyPeerTeamModel>> GetPeerTeamsByUserId(int id)
        {
            var spParameters = new SqlParameter[1];
            List<MyPeerTeamModel> peerTeamModels = new List<MyPeerTeamModel>();
            spParameters[0] = new SqlParameter() { ParameterName = "UsersID", Value = id };
            var result = await DbContextManager.StoreProc<MyPeerTeamModel>("[dbo].spGetPeerTeamsDataByUserID", spParameters);
            if (result != null)
            {
                try
                {
                    peerTeamModels = result.ToList();
                    foreach (var item in peerTeamModels)
                    {
                        item.orgRevenue = await PeerTeamUow.Repository<vOrgRevenue>()
                            .Queryable()
                            .Where(x => x.organizationID == item.organizationId)
                            .OrderByDescending(x => x.year)
                            .Take(5)
                            .OrderBy(x => x.year) // To ensure the final result is ordered by year in ascending order
                            .ToListAsync();
                    }
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                peerTeamModels = new List<MyPeerTeamModel>();

            }
            return peerTeamModels;
        }

        public async Task<IEnumerable<MyPeerTeamModel>> GetPeerTeamById(int id)
        {
            var spParameters = new SqlParameter[1];
            List<MyPeerTeamModel> peerTeamModels = new List<MyPeerTeamModel>();
            spParameters[0] = new SqlParameter() { ParameterName = "UserGroupsID", Value = id };
            var result = await DbContextManager.StoreProc<MyPeerTeamModel>("[dbo].spGetPeerTeamsDataByUserGroupID", spParameters);
            if (result != null)
            {
                try
                {
                    peerTeamModels = result.ToList();
                    foreach (var item in peerTeamModels)
                    {
                        item.orgRevenue = await PeerTeamUow.Repository<vOrgRevenue>()?.Queryable()?.Where(x => x.organizationID == item.organizationId)?.OrderBy(x => x.year).ToListAsync();
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                peerTeamModels = new List<MyPeerTeamModel>();

            }
            return peerTeamModels;
        }
    }

    public interface IPeerTeamDomain
    {
        Task<Response<GroupType>> CreateGroupType(GroupType groupType);
        Task<UserGroup> GetUserGroupById(int id);
        Task<IEnumerable<UserGroup>> GetUserGroups();
        Task<GroupType> GetGroupTypeById(int id);
        Task<IEnumerable<GroupType>> GetGroupTypes();
        Task<Response<UserGroup>> CreateUserGroup(UserGroup userGroup);
        Task<Response<UserGroup>> AddUserToGroup(int userId, int groupId);
        Task<IEnumerable<UserGroupsMember>> GetMembersByGroupId(int groupId);

        Task<IEnumerable<VUserGroupsMember>> GetGroupsByUserId(int UserId);
        Task<Response<UserGroup>> UpdateUserGroup(int id, UserGroup userGroup);
        Task<Response<UserGroup>> DeleteUserGroup(int id);
        Task<IEnumerable<UserGroup>> GetUserGroupsByMemberId(int id);
        Task<IEnumerable<MyPeerTeamModel>> GetPeerTeamsByUserId(int id);
        Task<IEnumerable<MyPeerTeamModel>> GetPeerTeamById(int id);
    }

}
