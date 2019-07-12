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

        [HttpGet]
        [Route("{taskId}/comments"), MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCommentsAsync(string taskId)
        {
            try
            {
                var comments = await _taskService.GetCommentsAsync(UserId, taskId).ConfigureAwait(false);
                return Ok(comments);
            }
            catch (NoCommentsFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("{taskId}/comments"), MapToApiVersion("1.0")]
        public async Task<IActionResult> PostCommentsAsync(string taskId, [FromBody] Comment comment)
        {
            try
            {
                comment.UserId = UserId;
                await _taskService.AddCommentAsync(taskId, comment).ConfigureAwait(false);
                return Ok();
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut]
        [Route("{taskId}/comments/{commentId}"), MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateCommentAsync(string taskId, string commentId, [FromBody] Comment comment)
        {
            try
            {
                comment.UserId = UserId;
                comment.Id = commentId; // TODO: Is this needed, or will the client send the comment across with the Id already assigned?
                await _taskService.UpdateCommentAsync(taskId, comment).ConfigureAwait(false);
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
        [Route("{taskId}/comments/{commentId}"), MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteCommentAsync(string taskId, string commentId)
        {
            try
            {
                await _taskService.DeleteCommentAsync(taskId, commentId, UserId).ConfigureAwait(false);
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

        // TODO: Add get, post, update, delete Alerts

        [HttpGet]
        [Route("{taskId}/alerts"), MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAlertsAsync(string taskId)
        {
            try
            {
                var alerts = await _taskService.GetAlertsAsync(UserId, taskId).ConfigureAwait(false);
                return Ok(alerts);
            }
            catch (NoCommentsFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("{taskId}/alerts"), MapToApiVersion("1.0")]
        public async Task<IActionResult> PostAlertsAsync(string taskId, [FromBody] AlertTime alert)
        {
            try
            {
                alert.UserId = UserId;
                await _taskService.AddAlertAsync(taskId, alert).ConfigureAwait(false);
                return Ok();
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut]
        [Route("{taskId}/alerts/{alertId}"), MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateAlertAsync(string taskId, string alertId, [FromBody] AlertTime alert)
        {
            try
            {
                alert.UserId = UserId;
                alert.Id = alertId; // TODO: Is this needed, or will the client send the comment across with the Id already assigned?
                await _taskService.UpdateAlertAsync(taskId, alert).ConfigureAwait(false);
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
        [Route("{taskId}/alerts/{alertId}"), MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteAlertAsync(string taskId, string alertId)
        {
            try
            {
                await _taskService.DeleteAlertAsync(taskId, alertId, UserId).ConfigureAwait(false);
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
