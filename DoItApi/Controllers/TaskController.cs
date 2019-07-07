using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DoItApi.Data;
using DoItApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoItApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly DoItDbContext _doItDbContext;

        //public TaskController(DoItDbContext doItDbContext)
        //{
        //    this._doItDbContext = doItDbContext;
        //}

        [HttpGet]
        [Route("Tasks"), MapToApiVersion("1.0")]
        public IActionResult GetTasks()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //var tasks = _doItDbContext.Tasks;
            var tasks = new List<Task>();

            return Ok(userId);
        }
    }
}
