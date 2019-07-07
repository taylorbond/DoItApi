using System.Linq;
using DoItApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoItApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TaskController : BaseController
    {
        private readonly DoItDbContext _doItDbContext;

        public TaskController(DoItDbContext doItDbContext)
        {
            this._doItDbContext = doItDbContext;
        }

        [HttpGet]
        [Route("Tasks"), MapToApiVersion("1.0")]
        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)]
        public IActionResult GetTasks()
        {
            var tasks = _doItDbContext.Tasks.Include("Comments").Include("AlertTimes").Where(x => x.UserId == UserId);

            return Ok(tasks);
        }
    }
}
