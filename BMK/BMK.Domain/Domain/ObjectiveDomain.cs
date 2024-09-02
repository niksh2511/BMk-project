using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;
using Microsoft.EntityFrameworkCore;
using RxWeb.Core.Data;
using RxWeb.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class ObjectiveDomain : IObjectiveDomain
    {
        public IObjectiveUow ObjectiveUow { get; set; }
        public ILogException LogException { get; set; }
        public IUserUow UserUow { get; set; }
        public IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        public IUserClaim UserClaim { get; set; }
        public ObjectiveDomain(IObjectiveUow objectiveUow, ILogException logException, IUserClaim userClaim, IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager)
        {
            ObjectiveUow = objectiveUow;
            LogException = logException;
            UserClaim = userClaim;
            UserUow = userUow;
            DbContextManager = dbContextManager;
        }

        public async Task<IEnumerable<Objective>> GetObjectivesByUserId(int userId)
        {
            return await ObjectiveUow.Repository<Objective>().Queryable().Where(x => x.UsersId == userId).Include(x=>x.ObjectiveComments).ThenInclude(x=>x.CommentByUser).ToListAsync();
        }

        public async Task<Response<Objective>> AddObjective(Objective objective)
        {
            var response = new Response<Objective>();
            var currentUser = UserClaim.UserId;

            var newObjective = objective;
            newObjective.Active = true;
            newObjective.CreatedDate = DateTime.Now;
            newObjective.CreatedBy = currentUser;
            newObjective.ModifiedDate = DateTime.Now;

            try
            {
                await ObjectiveUow.RegisterNewAsync(newObjective);
                var intres = await ObjectiveUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Created Objective Successfully";
            }
            catch(Exception ex)
            {
                await LogException.Log(ex, "api/Objectives/AddObjective");
                response.IsSucceed = false;
                response.Message = "Could not create objective.";
            }

            response.Data = newObjective;
            return response;
        }

        public async Task<Objective> GetObjectiveByObjectiveId(int id)
        {
            return await ObjectiveUow.Repository<Objective>().FindByKeyAsync(id);
        }

        public async Task<Response<Objective>> UpdateObjective(int id, Objective objective)
        {
            var response = new Response<Objective>();
            try
            {
                await ObjectiveUow.RegisterDirtyAsync(objective);
                var res = await ObjectiveUow.CommitAsync();
                response.IsSucceed= true;
                response.Message = "Objective Updated Successfully";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/Objective/UpdateObjective");
                response.IsSucceed = false;
                response.Message = "failed to update Objective";
            }
            response.Data = objective;
            return response;
        }

        public async Task<IEnumerable<object>> GetStatus()
        {
            return await ObjectiveUow.Repository<AppObject>().Queryable().Where(x => x.ObjCategory == "ObjectiveStatus").Select(x=> new { statusId=x.AppObjectsId, statusName= x.ObjValue}).ToListAsync();
        }

        public async Task<Response<ObjectiveComment>> AddCommentToObjective(ObjectiveComment comment)
        {
            var response = new Response<ObjectiveComment>();
            //var objective = await GetObjectiveByObjectiveId(Convert.ToInt32(comment.ObjectiveId));
            var resobj = new ObjectiveComment();
            try
            {
                comment.CreatedBy = comment.CommentByUserId;
                comment.CreatedDate = DateTime.Now;
                comment.Active = true;

                await ObjectiveUow.RegisterNewAsync(comment);
                var res = await ObjectiveUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Comment Added Successfully.";
                resobj = await ObjectiveUow.Repository<ObjectiveComment>().Queryable().Where(x => x.ObjectiveCommentsId == comment.ObjectiveCommentsId).Include(x => x.CommentByUser).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                response.IsSucceed = false;
                response.Message = "Failed to add Comment.";
            }
            response.Data = resobj;
            return response;
        }

        public async Task<Response<Objective>> CancelObjective(int id)
        {
            var response = new Response<Objective>();
            var objective = await GetObjectiveByObjectiveId(id);

            var currentDate = DateTime.Now;
            try
            {
                objective.ClosedDate = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);
                objective.Status = await ObjectiveUow.Repository<AppObject>().Queryable().Where(x => x.ObjCategory == "ObjectiveStatus" && x.ObjValue == "Cancelled").Select(x => x.AppObjectsId).FirstAsync();
                await ObjectiveUow.RegisterDirtyAsync(objective);
                var res = ObjectiveUow.CommitAsync();
                response.IsSucceed = true;
                response.Message = "Cancelled Objective Successfully";

            }
            catch(Exception ex )
            {
                response.IsSucceed = false;
                response.Message = "Failed to Cancel Objective.";
            }


            //response.Data = await GetObjectivesByUserId(Convert.ToInt32(objective.UsersId));
            response.Data = objective;
            return response;
        }
    }

    public interface IObjectiveDomain
    {
        Task<Response<ObjectiveComment>> AddCommentToObjective(ObjectiveComment comment);
        Task<Response<Objective>> AddObjective(Objective objective);
        Task<Response<Objective>> CancelObjective(int id);
        Task<Objective> GetObjectiveByObjectiveId(int id);
        Task<IEnumerable<Objective>> GetObjectivesByUserId(int userId);
        Task<IEnumerable<object>> GetStatus();
        Task<Response<Objective>> UpdateObjective(int id, Objective oldObjective);
    }
}
