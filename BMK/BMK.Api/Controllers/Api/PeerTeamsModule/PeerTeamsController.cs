using BMK.Domain.Domain;
using BMK.Models.DbEntities;
using BMK.Models.Models;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxWeb.Core.AspNetCore;
using System.Net;

namespace BMK.Api.Controllers.Api.PeerTeamsModule
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PeerTeamsController : ControllerBase
    {

        public IPeerTeamDomain PeerTeamDomain;

        public PeerTeamsController(IPeerTeamDomain domain)
        {
            PeerTeamDomain = domain;
        }

        [HttpGet("GetGroupTypes")]
        public async Task<IActionResult> GetGroupTypes()
        {
            return Ok(await PeerTeamDomain.GetGroupTypes());
        }

        //[HttpPost("createGroupType")]
        //public async Task<IActionResult> createGroupType([FromBody] GroupType groupType)
        //{
        //    return Ok(await PeerTeamDomain.CreateGroupType(groupType));
        //}

        [HttpGet("GetGroupTypeById")]
        public async Task<IActionResult> GetGroupTypeById(int id)
        {
            return Ok(await PeerTeamDomain.GetGroupTypeById(id));
        }


        [HttpGet("GetUserGroups")]
        public async Task<IActionResult> GetUserGroups()
        {
            return Ok(await PeerTeamDomain.GetUserGroups());
        }

        [HttpGet("GetUserGroupById/{id}")]
        public async Task<IActionResult> GetUserGroupById(int id)
        {
            return Ok(await PeerTeamDomain.GetUserGroupById(id));
        }

        [HttpGet("GetMembersByGroupId/{id}")]
        public async Task<IActionResult> GetMembersByGroupId(int id)
        {
            return Ok(await PeerTeamDomain.GetMembersByGroupId(id));
        }
        [HttpGet("GetGroupsByUserId/{id}")]
        public async Task<IActionResult> GetGroupsByUserId(int id)
        {
            return Ok(await PeerTeamDomain.GetGroupsByUserId(id));
        }

        [HttpPost("CreateUserGroup")]
        public async Task<IActionResult> CreateUserGroup([FromBody] UserGroup userGroup)
        {
            Response<UserGroup> response = await PeerTeamDomain.CreateUserGroup(userGroup);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpPost("AddUserToGroup")]
        public async Task<IActionResult> AddUserToGroup([FromBody] AddUserToGroupModel addUserToGroupModel)
        {
            Response<UserGroup> response = await PeerTeamDomain.AddUserToGroup(addUserToGroupModel.userId, addUserToGroupModel.groupId);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }


        [HttpPut("UpdateUserGroup/{id}")]
        public async Task<IActionResult> UpdateUserGroup(int id,[FromBody] UserGroup userGroup)
        {
            var oldGroup = await PeerTeamDomain.GetUserGroupById(id);

            if (oldGroup == null)
            {
                return BadRequest();
            }
            oldGroup.GroupName = userGroup.GroupName;
            oldGroup.GroupDesc = userGroup.GroupDesc;
            if (userGroup.GroupTypesId != null && oldGroup.GroupTypesId != userGroup.GroupTypesId)
            {
                oldGroup.GroupTypesId = userGroup.GroupTypesId;
            }
            oldGroup.UserGroupsMembers = userGroup.UserGroupsMembers;

            Response<UserGroup> response = await PeerTeamDomain.UpdateUserGroup(id,oldGroup);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }


        [HttpPatch("DeleteUserGroup/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            Response<UserGroup> response = await PeerTeamDomain.DeleteUserGroup(id);
            return StatusCode(response.IsSucceed ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, response);
        }

        [HttpGet("GetUserGroupsByMemberId/{id}")]
        public async Task<IActionResult> GetUserGroupsByMemberId(int id)
        {
            return Ok(await PeerTeamDomain.GetPeerTeamsByUserId(id));
        }

        [HttpGet("GetPeerTeamById/{id}")]
        public async Task<IActionResult> GetPeerTeamById(int id)
        {
            return Ok(await PeerTeamDomain.GetPeerTeamById(id));
        }
    }
}
