using BMK.Models.DbEntities;
using BMK.UnitOfWork.Main;
using RxWeb.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Infrastructure.Logs
{
    public class LogException: ILogException
    {
        private IExceptionUow Uow { get; set; }
        private IUserClaim UserClaim { get; set; }
        public LogException(IExceptionUow uow,IUserClaim userClaim) 
        {
            Uow = uow;
            UserClaim = userClaim;
        }

        public async Task<string> Log(Exception exception, string url)
        {
            ExceptionLog log = new ExceptionLog
            {
                UsersId = UserClaim.UserId,
                Url = url,
                Message = exception.Message.ToString() ?? string.Empty,
                ExceptionType = exception.GetType().ToString() ?? string.Empty,
                ExceptionSource = exception.Source ?? string.Empty,
                StackTrace = exception.StackTrace ?? string.Empty,
                InnerException = (exception.InnerException != null) ? Convert.ToString(exception.InnerException) : string.Empty,
                ExceptionDate = DateTime.Now,
            };
            await Uow.RegisterNewAsync<ExceptionLog>(log);
            await Uow.CommitAsync();
            return string.Format("User : {0}<br/> Date & Time : {1}<br/> Error Log Id : {2}",
                    UserClaim.UserId,
                    Convert.ToString(DateTime.Now),
                    Convert.ToString(log.ExceptionLogsId));
        }
    }
    public interface ILogException
    {
        Task<string> Log(Exception exception, string url);
    }
}
