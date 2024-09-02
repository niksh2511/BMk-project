using BMK.BoundedContext.SqlDbContext;
using BMK.Infrastructure.Logs;
using BMK.Models.DbEntities;
using BMK.Models.Enums;
using BMK.Models.Models;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using RxWeb.Core.Common;
using RxWeb.Core.Data;
using RxWeb.Core.Security;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class EventDomain : IEventDomain
    {
        private IEventUow Uow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IPeerTeamUow PeerTeamUow { get; set; }
        private IPeerTeamDomain PeerTeamDomain { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        private Response<Category> Response { get; set; }
        private Response<Event> Response1 { get; set; }
        private readonly IConfiguration Config;
        private IEmail Email { get; set; }
        public EventDomain(IEventUow uow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException, IConfiguration config, IEmail email)
        {
            Uow = uow;
            Config = config;
            Email = email;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;
            LogException = logException;
            Response = new Response<Category>();
            Response1 = new Response<Event>();
        }
        public async Task<IEnumerable<CategoryModel>> GetCategoryList()
        {
            var spParameters = new SqlParameter[1];
            spParameters[0] = new SqlParameter() { ParameterName = "userId", Value = UserClaim.UserId };
            var obj = await DbContextManager.StoreProc<CategoryModel>("[dbo].spGetCategoryList", spParameters);
            return obj;
        }
        public async Task<List<Category>> GetCategories()
        {
            var obj = await Uow.Repository<Category>().Queryable().Where(x => x.Active == true).ToListAsync();
            return obj;
        }
        public async Task<Category> GetCategoryById(int id)
        {
            var obj = await Uow.Repository<Category>().Queryable().Where(x => x.Active == true && x.CategoryId == id).SingleOrDefaultAsync();
            return obj;
        }

        public async Task<Response<Category>> SaveCategory(CategoryGroupModel catGroup)
        {

            try
            {
                var obj = Uow.Repository<Category>().Queryable().Where(x => x.CategoryName == catGroup.CategoryName).FirstOrDefaultAsync();
                if (obj.Result != null)
                {
                    Response.IsSucceed = false;
                    Response.Message = "Category is already exist.";
                }
                else
                {
                    Category category = new Category()
                    {
                        CategoryName = catGroup.CategoryName,
                        Active = true,
                        IsSendNotification = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = UserClaim.UserId,
                    };

                    await Uow.RegisterNewAsync(category);
                    await Uow.CommitAsync();

                    var groupTypes = catGroup.GroupTypes;
                    foreach (var item in groupTypes)
                    {
                        foreach (var res in item.UserGroups)
                        {
                            if (res.Access == 10051)
                            {
                                CategoryGroup categoryGroup = new CategoryGroup()
                                {
                                    CatgeoryId = category.CategoryId,
                                    CategoryName = category.CategoryName,
                                    UserGroupsId = res.UserGroupsId,
                                    GroupName = res.GroupName,
                                    Access = res.Access,
                                    Active = true,
                                    CreatedDate = DateTime.Now,
                                    CreatedBy = UserClaim.UserId,
                                };
                                await Uow.RegisterNewAsync(categoryGroup);
                            }
                        }
                    }

                   
                        CategoryGroup acceesForEveryone = new CategoryGroup()
                        {
                            CatgeoryId = category.CategoryId,
                            CategoryName = category.CategoryName,
                            UserGroupsId = 0,
                            GroupName = "Everyone",
                            Access = catGroup.Access.Everyone,
                            Active = true,
                            CreatedDate = DateTime.Now,
                            CreatedBy = UserClaim.UserId,
                        };
                        await Uow.RegisterNewAsync(acceesForEveryone);
                    

                    var data = await Uow.CommitAsync();

                    Response.IsSucceed = true;
                    Response.Message = "Category added Successfully";


                }

            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/CategoryController/SaveCategory");
                Response.IsSucceed = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public async Task<List<CategoryGroup>> GetCategoryGroups(int id)
        {
            var obj = await Uow.Repository<CategoryGroup>().Queryable().Where(x => x.Active == true && x.CatgeoryId == id).ToListAsync();
            return obj;

        }

        public async Task<List<VCategoryGroup>> GetCategoryGroupsList(int id)
        {
            var obj = await Uow.Repository<VCategoryGroup>().Queryable().Where(x => x.CatgeoryId == id).ToListAsync();
            return obj;
        }
        public async Task<Response<Category>> UpdateCategory([FromBody] CategoryGroupModel categoryGroupModel)
        {

            try
            {
                var cat = await Uow.Repository<Category>().Queryable().Where(x => x.CategoryName == categoryGroupModel.CategoryName && x.CategoryId != categoryGroupModel.CategoryId).FirstOrDefaultAsync();
                if (cat != null)
                {
                    Response.IsSucceed = false;
                    Response.Message = "Category is already exist.";
                }
                else
                {
                    var category = await Uow.Repository<Category>().Queryable().Where(x => x.CategoryId == categoryGroupModel.CategoryId && x.Active == true).SingleOrDefaultAsync();
                    if (category == null)
                    {
                        Response.IsSucceed = false;
                        Response.Message = "Category is not available.";
                    }
                    else
                    {

                        category.CategoryName = categoryGroupModel.CategoryName;
                        category.IsSendNotification = category.IsSendNotification;
                        category.ModifiedDate = DateTime.Now;
                        category.ModifiedBy = UserClaim.UserId;

                        await Uow.RegisterDirtyAsync(category);

                        var categoryGroup = await Uow.Repository<CategoryGroup>().Queryable().Where(x => x.Active == true && x.CatgeoryId == category.CategoryId).ToListAsync();
                        if (categoryGroup.Count > 0)
                        {
                            foreach (var groupType in categoryGroupModel.GroupTypes)
                            {
                                foreach (var userGroup in groupType.UserGroups)
                                {
                                    var catGrp = categoryGroup.Find(x => x.CatgeoryId == categoryGroupModel.CategoryId && x.UserGroupsId == userGroup.UserGroupsId && x.Active == true);
                                    if (catGrp != null)
                                    {
                                        catGrp.CategoryName = categoryGroupModel.CategoryName;
                                        catGrp.Access = userGroup.Access;
                                        catGrp.ModifiedDate = DateTime.Now;
                                        catGrp.ModifiedBy = UserClaim.UserId;
                                        await Uow.RegisterDirtyAsync(catGrp);
                                    }
                                    else
                                    {
                                        if (userGroup.Access == 10051)
                                        {
                                            var group = await Uow.Repository<UserGroup>().Queryable().Where(x => x.UserGroupsId == userGroup.UserGroupsId && x.Active == true).FirstOrDefaultAsync();
                                            if (group != null)
                                            {
                                                CategoryGroup newCategoryGroup = new CategoryGroup()
                                                {
                                                    CatgeoryId = categoryGroupModel.CategoryId,
                                                    CategoryName = categoryGroupModel.CategoryName,
                                                    UserGroupsId = userGroup.UserGroupsId,
                                                    GroupName = userGroup.GroupName,
                                                    Access = userGroup.Access,
                                                    Active = true,
                                                    CreatedDate = DateTime.Now,
                                                    CreatedBy = UserClaim.UserId,
                                                };
                                                await Uow.RegisterNewAsync(newCategoryGroup);
                                            }
                                        }
                                    }
                                }
                            }

                            // Additional logic to update "Everyone" and "Authenticated User"
                          
                            var everyoneGroup = categoryGroup.Find(x => x.GroupName == "Everyone" && x.Active == true);
                            if (everyoneGroup != null)
                            {
                                everyoneGroup.CategoryName = categoryGroupModel.CategoryName;
                                everyoneGroup.Access = categoryGroupModel.Access.Everyone;
                                everyoneGroup.ModifiedDate = DateTime.Now;
                                everyoneGroup.ModifiedBy = UserClaim.UserId;
                                await Uow.RegisterDirtyAsync(everyoneGroup);
                            }
                          

                            //var authenticatedUserGroup = categoryGroup.Find(x => x.GroupName == "Authenticated User" && x.Active == true);
                            //if (authenticatedUserGroup != null)
                            //{
                            //    authenticatedUserGroup.CategoryName = categoryGroupModel.CategoryName;
                            //    authenticatedUserGroup.Access = categoryGroupModel.Access.AuthenticatedUser;
                            //    authenticatedUserGroup.ModifiedDate = DateTime.Now;
                            //    authenticatedUserGroup.ModifiedBy = UserClaim.UserId;
                            //    await Uow.RegisterDirtyAsync(authenticatedUserGroup);
                            //}

                            await Uow.CommitAsync();
                            Response.IsSucceed = true;
                            Response.Message = "Category updated Successfully";
                        }
                        else
                        {
                            Response.IsSucceed = false;
                            Response.Message = "No groups has access with this category.";
                        }


                    }
                }

                return Response;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/CategoryController/UpdateCategory");
                Response.IsSucceed = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public async Task<Response<Category>> DeleteCategory(int categoryId)
        {
            if (UserClaim.RoleId == (int)RoleEnum.SuperAdmin || UserClaim.RoleId == (int)RoleEnum.Admin)
            {
                try
                {
                    var category = await Uow.Repository<Category>().Queryable().Where(x => x.CategoryId == categoryId).SingleOrDefaultAsync();
                    if (category == null)
                    {
                        Response.IsSucceed = false;
                        Response.Message = "Category is not available.";
                    }

                    category.Active = false;
                    await Uow.RegisterDirtyAsync(category);

                    var categoryGroups = await Uow.Repository<CategoryGroup>().Queryable().Where(x => x.CatgeoryId == categoryId && x.Active == true).ToListAsync();

                    foreach (var categoryGroup in categoryGroups)
                    {
                        categoryGroup.Active = false;
                        await Uow.RegisterDirtyAsync(categoryGroup);
                    }
                    await Uow.CommitAsync();
                    Response.IsSucceed = true;
                    Response.Message = "Category deleted successfully.";

                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/CategoryController/DeleteCategory");
                    Response.IsSucceed = false;
                    Response.Message = ex.Message;
                }
            }
            else
            {
                Response.IsSucceed = false;
                Response.Message = "Unauthorized!";
            }
            return Response;
        }

        public async Task<List<Event>> GetEvents()
        {
            var obj = await Uow.Repository<Event>().Queryable().Where(x => x.Active == true).ToListAsync();
            return obj;
        }

        public async Task<Response<Event>> SaveEvent(EventModel evnt)
        {
            if ((UserClaim.RoleId == (int)RoleEnum.SuperAdmin || UserClaim.RoleId == (int)RoleEnum.Admin))
            {
                try
                {
                    var obj = Uow.Repository<Event>().Queryable().Where(x => x.EventName == evnt.EventName || x.EventId == evnt.EventID).FirstOrDefaultAsync();
                    if (obj.Result != null)
                    {
                        Response1.IsSucceed = false;
                        Response1.Message = "Event is already exist.";
                    }
                    else
                    {
                        Event newEvent = new Event()
                        {
                            EventName = evnt.EventName,
                            Description = evnt.Description,
                            AllDayEvent = evnt.AllDayEvent,
                            StartDate = evnt.StartDate,
                            EndDate = evnt.EndDate,
                            Location = evnt.Location,
                            Active = true,
                            TimeZone = evnt.Timezone,
                            SentInvitation = evnt.SendInvitation,
                            CreatedBy = UserClaim.UserId,
                            ModifiedBy = UserClaim.UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };
                        await Uow.RegisterNewAsync(newEvent);
                        await Uow.CommitAsync();
                        for (int i = 0; i < evnt.Categories.Count; i++)
                        {
                            EventCategory eventCategory = new EventCategory()
                            {
                                Active = true,
                                CreatedBy = UserClaim.UserId,
                                ModifiedBy = UserClaim.UserId,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                EventId = newEvent.EventId,
                                CategoryId = evnt.Categories[i]
                            };
                            await Uow.RegisterNewAsync(eventCategory);
                        }
                        await Uow.CommitAsync();
                        Response1.IsSucceed = true;
                        Response1.Message = "Event created successfully.";

                    }
                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/EventController/SaveEvent");
                    Response1.IsSucceed = false;
                    Response1.Message = ex.Message;
                }
            }
            else
            {
                Response1.IsSucceed = false;
                Response1.Message = "Unauthorized.";
            }
            return Response1;

        }

        public async Task<Response<Event>> DeleteEvent(int eventId)
        {
            if (UserClaim.RoleId == (int)RoleEnum.SuperAdmin || UserClaim.RoleId == (int)RoleEnum.Admin)
            {
                try
                {
                    var deleteEvent = await Uow.Repository<Event>()
                        .Queryable()
                        .Where(x => x.EventId == eventId)
                        //.Include(x => x.EventCategories)
                        .FirstOrDefaultAsync();

                    if (deleteEvent == null)
                    {
                        Response.IsSucceed = false;
                        Response.Message = "Event is not available.";
                    }
                    else
                    {
                        deleteEvent.Active = false;
                        deleteEvent.ModifiedBy = UserClaim.UserId;
                        deleteEvent.ModifiedDate = DateTime.Now;

                        await Uow.RegisterDirtyAsync(deleteEvent);

                        var categories = await Uow.Repository<EventCategory>().Queryable().Where(x => x.EventId == eventId && x.Active == true).ToListAsync();

                        foreach (var item in categories)
                        {
                            //bool hasCategory = deleteEvent.EventCategories.Any(ec => ec.CategoryId == item.CategoryId);
                            //if (hasCategory)
                            //{
                            item.Active = false;
                            item.ModifiedBy = UserClaim.UserId;
                            item.ModifiedDate = DateTime.Now;
                            await Uow.RegisterDirtyAsync(item);
                            //}
                        }
                        await Uow.CommitAsync();
                    }
                    Response1.IsSucceed = true;
                    Response1.Message = "Category deleted successfully.";

                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/EventController/DeleteEvent");
                    Response1.IsSucceed = false;
                    Response1.Message = ex.Message;
                }
            }
            else
            {
                Response1.IsSucceed = false;
                Response1.Message = "Unauthorized!";
            }
            return Response1;
        }

        public async Task<List<EventCategoryModel>> GetEventCategoryList(int eventID, int userID)
        {
            var spParameters = new SqlParameter[2];
            spParameters[0] = new SqlParameter() { ParameterName = "eventID", Value = eventID };
            spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = userID };
            List<EventCategoryModel> eventCategories = new List<EventCategoryModel>();
            var result = await DbContextManager.StoreProc<SpResult>("[dbo].spGetEventCategoryList", spParameters);
            if (result != null)
            {
                var res = result.SingleOrDefault().Result;
                if (res != null)
                {
                    eventCategories = JsonConvert.DeserializeObject<List<EventCategoryModel>>(res);
                }
            }
            return eventCategories;
        }

        public async Task<bool> sendEmailNotification(int eventId)
        {
            var eventobj = await Uow.Repository<Event>().Queryable().Where(x => x.EventId == eventId && x.Active == true).FirstOrDefaultAsync();

            var EventCategoryobj = await Uow.Repository<EventCategory>().Queryable().Where(x => x.EventId == eventId && x.Active == true).ToListAsync();
            var uniqueEmails = new HashSet<string>();
            foreach (var item in EventCategoryobj)
            {
                var Categoryobj = await Uow.Repository<Category>().Queryable().Where(x => x.CategoryId == item.CategoryId && x.Active == true && x.IsSendNotification == true).FirstOrDefaultAsync();
                if (Categoryobj != null && Categoryobj.IsSendNotification == true)
                {
                    var CategoryGroupobj = await Uow.Repository<CategoryGroup>().Queryable().Where(x => x.Active == true && x.CatgeoryId == item.CategoryId && x.Access != (int)CategoryAccessEnum.None).ToListAsync();

                    foreach (var cat in CategoryGroupobj)
                    {
                        var userGroupsMemberObj = await Uow.Repository<UserGroupsMember>()
                                                        .Queryable()
                                                        .Where(x => x.Active == true && x.UserGroupsId == cat.UserGroupsId)
                                                        .Select(x => x.UsersId)
                                                        .Distinct()
                                                        .ToListAsync();
                        foreach (var user in userGroupsMemberObj)
                        {
                            var Userobj = await Uow.Repository<User>().Queryable().Where(x => x.Active == true && x.UsersId == user && !uniqueEmails.Contains(x.Email)).FirstOrDefaultAsync();
                            if (Userobj != null)
                            {
                                var emailTemplate = await Uow.Repository<EmailTemplate>().SingleOrDefaultAsync(e => e.EmailTemplatesId == (int)EmailTemplateEnum.EventNotification && e.Active == true);
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##username##", Userobj.FirstName + " " + Userobj.LastName);
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##EventName##", eventobj.EventName);
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Description##", eventobj.Description);
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Startdate##", Convert.ToString(eventobj.StartDate));
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##Enddate##", Convert.ToString(eventobj.EndDate));
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##location##", eventobj.Location);
                                emailTemplate.TemplateBody = emailTemplate.TemplateBody.Replace("##timezone##", eventobj.TimeZone);
                                MailConfig mailConfig = new MailConfig
                                {
                                    To = { Userobj.Email },
                                    EmailFormat = EmailFormatType.Html,
                                    From = Convert.ToString(Config["EmailSetting:FromMail"]),
                                    Subject = emailTemplate.TemplateSubject,
                                    Body = emailTemplate.TemplateBody,
                                };


                                bool isSuccess = await Email.SendAsync(mailConfig);
                                if (isSuccess == true)
                                    uniqueEmails.Add(Userobj.Email);
                            }
                        }
                    }
                }

            }

            return true;
        }

        public async Task<Response<Event>> UpdateEvent(int eventId, EventCategoryModel evnt)
        {
            if ((UserClaim.RoleId == (int)RoleEnum.SuperAdmin || UserClaim.RoleId == (int)RoleEnum.Admin))
            {
                try
                {
                    var existedEvent = await Uow.Repository<Event>().Queryable().FirstOrDefaultAsync(x => x.EventName == evnt.EventName && x.EventId != eventId);
                    if (existedEvent != null)
                    {
                        Response1.IsSucceed = false;
                        Response1.Message = "Category is already exist.";
                    }
                    else
                    {
                        var updateEvent = await Uow.Repository<Event>()
                        .Queryable()
                        .Where(x => x.EventId == evnt.EventID)
                        .Include(x => x.EventCategories)
                        .FirstOrDefaultAsync();

                        if (updateEvent != null)
                        {
                            updateEvent.EventName = evnt.EventName;
                            updateEvent.AllDayEvent = evnt.AllDayEvent;
                            updateEvent.StartDate = evnt.Startdate;
                            updateEvent.EndDate = evnt.Enddate;
                            updateEvent.Location = evnt.Location;
                            updateEvent.Description = evnt.Description;
                            updateEvent.SentInvitation = evnt.SentInvitation;
                            updateEvent.TimeZone = evnt.Timezone;
                            updateEvent.ModifiedBy = UserClaim.UserId;
                            updateEvent.ModifiedDate = DateTime.Now;
                            updateEvent.EventCategories.Clear();
                            await Uow.RegisterDirtyAsync(updateEvent);

                            var categories = await Uow.Repository<EventCategory>().Queryable().Where(x => x.EventId == evnt.EventID && x.Active == true).ToListAsync();
                            if (categories.Count > 0)
                            {
                                foreach (var category in categories)
                                {
                                    await Uow.RegisterDeletedAsync<EventCategory>(category);
                                }
                            }
                            foreach (var item in evnt.Category)
                            {

                                EventCategory eventCategory = new EventCategory()
                                {
                                    Active = true,
                                    CreatedBy = UserClaim.UserId,
                                    ModifiedBy = UserClaim.UserId,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    EventId = evnt.EventID,
                                    CategoryId = item.CategoryID
                                };
                                await Uow.RegisterNewAsync(eventCategory);

                            }

                            await Uow.CommitAsync();
                            Response1.IsSucceed = true;
                            Response1.Message = "Event updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    await LogException.Log(ex, "api/EventController/UpdateEvent");
                    Response1.IsSucceed = false;
                    Response1.Message = ex.Message;
                }
            }
            else
            {
                Response1.IsSucceed = false;
                Response1.Message = "Unauthorized.";
            }
            return Response1;
        }

        public async Task<Response<Category>> ManageNotification(int[] categoryList)
        {
            try
            {
                var currentCategories = await Uow.Repository<Category>()
                                      .Queryable()
                                      .Where(x => categoryList.Contains(x.CategoryId) && x.Active == true)
                                      .ToListAsync();

                var currentCategoryIds = currentCategories.Select(x => x.CategoryId).ToList();

                var previousCategories = await Uow.Repository<Category>()
                                          .Queryable()
                                          .Where(x => !currentCategoryIds.Contains(x.CategoryId) && x.Active == true)
                                          .ToListAsync();

                // Update categories in memory
                foreach (var category in currentCategories)
                {
                    category.IsSendNotification = true;
                    await Uow.RegisterDirtyAsync(category);
                }
                foreach (var category in previousCategories)
                {
                    category.IsSendNotification = false;
                    await Uow.RegisterDirtyAsync(category);
                }
                await Uow.CommitAsync();
                Response.IsSucceed = true;
                Response.Message = "Events notifications updated successfully!";

            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/CategoryController/ManageNotification");
                Response.IsSucceed = false;
                Response.Message = ex.Message;
            }

            return Response;
        }
    }
    public interface IEventDomain
    {
        Task<IEnumerable<CategoryModel>> GetCategoryList();
        Task<List<Category>> GetCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Response<Category>> SaveCategory(CategoryGroupModel catGroup);
        Task<Response<Category>> UpdateCategory(CategoryGroupModel categoryGroupModel);
        Task<Response<Category>> DeleteCategory(int categoryId);
        Task<List<CategoryGroup>> GetCategoryGroups(int id);
        Task<List<VCategoryGroup>> GetCategoryGroupsList(int id);
        Task<List<Event>> GetEvents();

        Task<Response<Event>> SaveEvent(EventModel evnt);
        Task<Response<Event>> UpdateEvent(int eventId, EventCategoryModel evnt);
        Task<Response<Event>> DeleteEvent(int eventId);
        Task<List<EventCategoryModel>> GetEventCategoryList(int eventID, int userID);
        Task<bool> sendEmailNotification(int eventId);

        Task<Response<Category>> ManageNotification(int[] catgoeyList);

    }
}

