using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class AccountMappingUow : BaseUow, IAccountMappingUow
    {
        public AccountMappingUow(IAccountMappingContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface IAccountMappingUow : ICoreUnitOfWork { }
}


