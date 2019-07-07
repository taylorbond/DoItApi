using System.Linq;
using System.Security.Claims;
using DoItApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoItApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TaskController : ControllerBase
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
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var tasks = _doItDbContext.Tasks;

            return Ok(tasks);
        }
    }
}
