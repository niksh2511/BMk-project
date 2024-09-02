using BMK.Models;
using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RxWeb.Core;
using RxWeb.Core.Security;
using RxWeb.Core.Security.Cryptography;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{

    public interface IOrganizationDoomain : ICoreDomain<Organization, Organization> { }
  

    public class OrganizationDomain : IOrganizationDoomain
    {
        private IUserClaim UserClaim { get; set; }

        public OrganizationDomain(IUserUow uow, IUserClaim userCliam)
        {
            Uow = uow;
            UserClaim = userCliam;
        }
        public IUserUow Uow { get; set; }
        private HashSet<string> ValidationMessages { get; set; } = new HashSet<string>();

        public async Task AddAsync(Organization entity)
        {
            try
            {
                entity.Active = true;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = UserClaim.UserId;
                await Uow.RegisterNewAsync(entity);
                await Uow.CommitAsync();
                }
            catch(Exception ex)
            {
                    
            }
           
        }

        public HashSet<string> AddValidation(Organization entity)
        {
            return ValidationMessages;
        }

        public Task DeleteAsync(Organization parameters)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> DeleteValidation(Organization parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetAsync(Organization parameters)
        {
            var organization = await Uow.Repository<Organization>().Queryable().Where(x=>x.Active == true).ToListAsync();

            return organization;
        }
       

        public Task<object> GetBy(Organization parameters)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Organization entity)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> UpdateValidation(Organization entity)
        {
            throw new NotImplementedException();
        }
    }
}
