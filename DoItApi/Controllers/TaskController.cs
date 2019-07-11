using System;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoItApi.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [Route(""), MapToApiVersion("1.0")]
        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetTasksAsync()
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync(UserId).ConfigureAwait(false);
                return Ok(tasks);
            }
            catch (NoTasksFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route(""), MapToApiVersion("1.0")]
        public async Task<IActionResult> PostTaskAsync(DiaTask task)
        {
            try
            {
                task.UserId = UserId;
                await _taskService.AddTaskAsync(task).ConfigureAwait(false);
                return Ok();
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut]
        [Route(""), MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateTaskAsync(DiaTask task)
        {
            try
            {
                task.UserId = UserId;
                await _taskService.UpdateTaskAsync(task).ConfigureAwait(false);
                return Ok();
            }
            catch (NoDatabaseObjectFoundException e)
            {
                return NotFound(e);
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route(""), MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteTaskAsync(string id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id, UserId).ConfigureAwait(false);
                return Ok();
            }
            catch (NoDatabaseObjectFoundException e)
            {
                return NotFound(e);
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }


    }
}
