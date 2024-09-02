using RxWeb.Core.Data;
using BMK.UnitOfWork;
using BMK.BoundedContext.DbContext.Main;

namespace BMK.UnitOfWork.Main
{
    public class EventUow : BaseUow, IEventUow
    {
        public EventUow(IEventContext context, IRepositoryProvider repositoryProvider) : base(context, repositoryProvider) { }
    }

    public interface IEventUow : ICoreUnitOfWork { }
}


