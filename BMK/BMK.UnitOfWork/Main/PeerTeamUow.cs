using BMK.BoundedContext.DbContext.Main;
using RxWeb.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.UnitOfWork.Main
{
    public class PeerTeamUow : BaseUow, IPeerTeamUow
    {
        public PeerTeamUow(IPeerTeamContext context, IRepositoryProvider repositoryProvider, IAuditLog auditLog = null) : base(context, repositoryProvider, auditLog) { }
    }

    public interface IPeerTeamUow : ICoreUnitOfWork { }
}
