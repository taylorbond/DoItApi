using System;
using System.Collections.Generic;
using System.Security.Claims;
using DoItApi.Controllers;
using DoItApi.Data;
using DoItApi.Models;
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

        [OneTimeSetUp]
        public static void Init()
        {
            var options = new DbContextOptionsBuilder<DoItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DoItDbContext(options);

            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
            var task = new Task
            {
                Id = Guid.NewGuid().ToString(),
                UserId = null, // intentionally null since we can't set user claims.
                TaskDescription = "Here's my task description.",
                DueDateTime = dateTimeOffset,
                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = "Hi" } }
            };

            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();
        }

        [Test]
        public void GetTasks_ReturnsTasks_OkResultWithValuesReturned()
        {
            var controller = new TaskController(_dbContext);

            var response = controller.GetTasks();
            var result = (OkObjectResult) response;

            ((IEnumerable<Task>) result.Value).Should().NotBeNull();
        }

        [Test]
        public void GetTasks_ReturnsTasks_TasksReturned()
        {
            var controller = new TaskController(_dbContext);

            var response = controller.GetTasks();
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<Task>) result.Value;

            tasks.Should().HaveCount(1);
        }
    }
}