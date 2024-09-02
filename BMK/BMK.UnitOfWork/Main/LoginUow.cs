using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class LoginUow : BaseUow, ILoginUow
    {
        public LoginUow(ILoginContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface ILoginUow : ICoreUnitOfWork { }
}


