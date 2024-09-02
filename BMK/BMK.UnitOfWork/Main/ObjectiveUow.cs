using BMK.BoundedContext.DbContext.Main;
using RxWeb.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.UnitOfWork.Main
{
    public class ObjectiveUow: BaseUow, IObjectiveUow
    {
        public ObjectiveUow(IObjectiveContext context, IRepositoryProvider repositoryProvider, IAuditLog auditLog = null) : base(context, repositoryProvider, auditLog) { }
    }

    public interface IObjectiveUow : ICoreUnitOfWork { }
}
