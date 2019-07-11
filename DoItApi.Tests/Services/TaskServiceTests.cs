using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Data;
using DoItApi.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DoItApi.Tests.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        private static DoItDbContext _dbContext;
        private static DiaTask _task;

        [SetUp]
        public static void Init()
        {
            var options = new DbContextOptionsBuilder<DoItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        
            _dbContext = new DoItDbContext(options);
        
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
        
            _dbContext.Tasks.Add(_task);
            _dbContext.SaveChanges();
        }

        [Test]
        public async Task TaskService_GetTasks_TasksReturned()
        {
            var taskService = new TaskService(_dbContext);

            var results = await taskService.GetTasks(null);

            results.Should().Contain(_task);
        }

        [Test]
        public void TaskService_TestName_Result()
        {
            var options = new DbContextOptionsBuilder<DoItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new DoItDbContext(options);

            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoTasksFoundException>(() => taskService.GetTasks(null));
        }
    }
}