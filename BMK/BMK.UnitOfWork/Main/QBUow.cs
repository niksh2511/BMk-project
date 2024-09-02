using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class QBUow : BaseUow, IQBUow
    {
        public QBUow(IQuickBooksContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface IQBUow : ICoreUnitOfWork { }
}


