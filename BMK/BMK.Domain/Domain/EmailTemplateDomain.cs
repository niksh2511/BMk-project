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
   public class EmailTemplateDomain : IEmailTemplateDomain
    {
        public IUserUow UserUow { get; set; }
        private IUserClaim UserClaim { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private ILogException LogException { get; set; }
        public EmailTemplateDomain(IUserUow userUow, IDbContextManager<MainSqlDbContext> dbContextManager, IUserClaim userClaim, ILogException logException)
        {
            UserUow = userUow;
            DbContextManager = dbContextManager;
            UserClaim = userClaim;
            LogException = logException;
        }
        public async Task<EmailTemplate> GetEmailTemplatesById(int id)
        {
            return await UserUow.Repository<EmailTemplate>().Queryable().Where(r => r.Active == true && r.EmailTemplatesId == id).FirstOrDefaultAsync();
        }

        public async Task<EmailTemplate> SaveEmailTemplate(EmailTemplate model)
        {
            var Exits = await UserUow.Repository<EmailTemplate>().Queryable().Where(x => x.EmailTemplatesId == model.EmailTemplatesId).FirstOrDefaultAsync();
            if (Exits != null)
            {
                model.CreatedBy = Exits.CreatedBy;
                model.CreatedDate = Exits.CreatedDate;
                model.ModifiedBy = UserClaim.UserId;
                model.ModifiedDate = DateTime.Now;
                await UserUow.RegisterDirtyAsync<EmailTemplate>(model);
            }

            await UserUow.CommitAsync();

            return model;
        }


    }
    public interface IEmailTemplateDomain
    {
        Task<EmailTemplate> GetEmailTemplatesById(int id);
        Task<EmailTemplate> SaveEmailTemplate(EmailTemplate model);


    }
}
