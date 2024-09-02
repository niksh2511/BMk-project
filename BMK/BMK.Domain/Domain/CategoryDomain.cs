using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;
using RxWeb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class CategoryDomain : ICategoryDomain
    {
        public IUserUow Uow { get; set; }
        public CategoryDomain(IUserUow uow) {
            Uow = uow; 
        }
        public async Task AddAsync(Category entity)
        {
            try
            {
                entity.Active = true;
                await Uow.RegisterNewAsync(entity);
                await Uow.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public HashSet<string> AddValidation(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Category parameters)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> DeleteValidation(Category parameters)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAsync(Category parameters)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetBy(Category parameters)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> UpdateValidation(Category entity)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICategoryDomain :ICoreDomain<Category, Category>
    {

    }
}
