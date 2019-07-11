using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Controllers;
using DoItApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace DoItApi.Tests.Controllers
{
    [TestFixture]
    public class TaskControllerTests
    {
        private static DiaTask _task;

        [SetUp]
        public void Init()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
            _task = new DiaTask
            {
                Id = Guid.NewGuid().ToString(),
                UserId = null, // intentionally null since we can't set user claims.
                TaskDescription = "Here's my task description.",
                DueDateTime = dateTimeOffset,
                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = "Hi" } }
            };
        }

        [Test]
        public async Task GetTasks_ReturnsTasks_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (OkObjectResult) response;

            ((IEnumerable<DiaTask>) result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task GetTasks_ReturnsTasks_TasksReturned()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksAsync(null)).ReturnsAsync(new List<DiaTask> { _task });
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<DiaTask>)result.Value;

            tasks.Should().HaveCount(1);
        }

        [Test]
        public async Task TaskController_GetTasksThrowsNoTaskFoundException_ReturnsNoContent()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksAsync(null)).ThrowsAsync(new NoTasksFoundException());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (NoContentResult) response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetTasksThrowsException_ReturnsBadRequest()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksAsync(null)).ThrowsAsync(new Exception());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostTask_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostTask(_task);
            var result = (OkObjectResult) response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostTask_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddTaskAsync(It.IsAny<DiaTask>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostTask(_task);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }
    }
}