﻿using System;
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
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route(""), MapToApiVersion("1.0")]
        public async Task<IActionResult> PostTask(DiaTask task)
        {
            try
            {
                await _taskService.AddTaskAsync(task).ConfigureAwait(false);
                return Ok(task.DueDateTime.ToString());
            }
            catch (DatabaseUpdateException e)
            {
                return BadRequest(e);
            }
        }
    }
}
