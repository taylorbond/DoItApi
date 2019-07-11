using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Controllers;
using DoItApi.Data;
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
            taskService.Setup(x => x.GetTasks(null)).ReturnsAsync(new List<DiaTask> { _task });
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
            taskService.Setup(x => x.GetTasks(null)).ThrowsAsync(new NoTasksFoundException());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (NoContentResult) response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetTasksThrowsException_ReturnsBadRequest()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasks(null)).ThrowsAsync(new Exception());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }
    }
}