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
            _dbContext = GenerateDbContext();

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

            var results = await taskService.GetTasksAsync(null);

            results.Should().Contain(_task);
        }

        [Test]
        public void TaskService_GetTasks_ThrowsNoTasksFoundException()
        {
            _dbContext = GenerateDbContext();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoTasksFoundException>(() => taskService.GetTasksAsync(null));
        }

        [Test]
        public async Task TaskService_AddTask_TaskAdded()
        {
            var task = GenerateRandomTask();
            var taskService = new TaskService(_dbContext);

            await taskService.AddTaskAsync(task);

            _dbContext.Tasks.Should().Contain(task);
        }

        [Test]
        public async Task TaskService_AddTask_ThrowsDatabaseUpdateException()
        {
            var task = GenerateRandomTask();
            _dbContext.Tasks = null;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.AddTaskAsync(task));
        }

        [Test]
        public async Task TaskService_UpdateTask_UpdatesTaskInDatabase()
        {
            var newTask = GenerateRandomTask();
            newTask.Id = _task.Id;

            var taskService = new TaskService(_dbContext);

            await taskService.UpdateTaskAsync(newTask);

            var foundTask = await _dbContext.Tasks.FindAsync(newTask.Id).ConfigureAwait(false);
            foundTask.TaskDescription.Should().Be(newTask.TaskDescription);
        }

        [Test]
        public async Task TaskService_UpdateTaskWhenTaskIsRemoved_ThrowsNoDatabaseObjectFoundException()
        {
            var task = GenerateRandomTask();
            task.Id = _task.Id;
            _dbContext.Tasks.Remove(_task);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateTaskAsync(task));
        }

        [Test]
        public void TaskService_UpdateTaskWhenTasksTableIsNull_ThrowsNoDatabaseObjectFoundException()
        {
            var task = GenerateRandomTask();
            task.Id = _task.Id;
            _dbContext.Tasks = null;
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateTaskAsync(task));
        }

        [Test]
        public async Task TaskService_DeleteTask_DeletesTask()
        {
            var taskService = new TaskService(_dbContext);

            await taskService.DeleteTaskAsync(_task.Id, null);

            _dbContext.Tasks.Should().NotContain(_task);
        }

        [Test]
        public async Task TaskService_DeleteTaskWhenTaskIsRemoved_ThrowsNoDatabaseObjectFoundException()
        {
            _dbContext.Remove(_task);
            await _dbContext.SaveChangesAsync();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.DeleteTaskAsync(_task.Id, null));
        }

        [Test]
        public void TaskService_DeleteTaskWhenTasksTableIsNull_ThrowsDatabaseUpdateException()
        {
            _dbContext.Tasks = null;
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.DeleteTaskAsync(_task.Id, null));
        }

        private static DoItDbContext GenerateDbContext()
        {
            var options = new DbContextOptionsBuilder<DoItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DoItDbContext(options);
        }

        private DiaTask GenerateRandomTask()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
            return new DiaTask
            {
                Id = Guid.NewGuid().ToString(),
                UserId = null, // intentionally null since we can't set user claims.
                TaskDescription = $"{Guid.NewGuid().ToString()} description.",
                DueDateTime = dateTimeOffset,
                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = $"{Guid.NewGuid().ToString()} comment" } }
            };
        }
    }
}