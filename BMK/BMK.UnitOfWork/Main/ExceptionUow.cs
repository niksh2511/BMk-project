using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class ExceptionUow : BaseUow, IExceptionUow
    {
        public ExceptionUow(IExceptionContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface IExceptionUow : ICoreUnitOfWork { }
}


