using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class UserUow : BaseUow, IUserUow
    {
        public UserUow(IUserContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface IUserUow : ICoreUnitOfWork { }
}


