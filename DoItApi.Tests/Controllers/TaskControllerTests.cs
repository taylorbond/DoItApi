﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private static DoItDbContext _dbContext;
        private static DiaTask _task;

        // TODO: Move to TaskService Tests
//        [OneTimeSetUp]
//        public static void Init()
//        {
//            var options = new DbContextOptionsBuilder<DoItDbContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;
//
//            _dbContext = new DoItDbContext(options);
//
//            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
//            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
//            _task = new DiaTask
//            {
//                Id = Guid.NewGuid().ToString(),
//                UserId = null, // intentionally null since we can't set user claims.
//                TaskDescription = "Here's my task description.",
//                DueDateTime = dateTimeOffset,
//                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
//                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = "Hi" } }
//            };
//
//            _dbContext.Tasks.Add(_task);
//            _dbContext.SaveChanges();
//        }

        [Test]
        public async Task GetTasks_ReturnsTasks_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasks();
            var result = (OkObjectResult) response;

            ((IEnumerable<DiaTask>) result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task GetTasks_ReturnsTasks_TasksReturned()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasks(null)).ReturnsAsync(new List<DiaTask>{_task});
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasks();
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<DiaTask>) result.Value;

            tasks.Should().HaveCount(1);
        }
    }
}