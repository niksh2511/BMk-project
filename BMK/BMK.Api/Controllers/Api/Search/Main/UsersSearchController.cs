using BMK.BoundedContext.SqlDbContext;
using BMK.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RxWeb.Core.Data;

namespace BMK.Api.Controllers.Api.Search.Main
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchUsersController : ControllerBase
    {
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        public SearchUsersController(IDbContextManager<MainSqlDbContext> dbContextManager)
        {
            DbContextManager = dbContextManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Dictionary<string, string> searchParams)
        {
            var spParameters = new SqlParameter[1];
            spParameters[0] = new SqlParameter() { ParameterName = "Query", Value = searchParams["query"] };
            var result = await DbContextManager.StoreProc<StoreProcResult>("[dbo].spSearchUsers", spParameters);
            return Ok(result.SingleOrDefault()?.Result);
        }
    }
}
