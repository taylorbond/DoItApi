using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Data;
using DoItApi.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
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

        #region TaskTests


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
        public async Task TaskService_GetTasksWithDetails_TasksReturned()
        {
            var taskService = new TaskService(_dbContext);

            var results = await taskService.GetTasksWithDetailsAsync(null);

            results.Should().Contain(_task);
        }

        [Test]
        public void TaskService_GetTasksWithDetails_ThrowsNoTasksFoundException()
        {
            _dbContext = GenerateDbContext();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoTasksFoundException>(() => taskService.GetTasksWithDetailsAsync(null));
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
        public void TaskService_UpdateTask_ThrowsDatabaseUpdateException()
        {
            var newTask = GenerateRandomTask();
            newTask.Id = _task.Id;

            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.UpdateTaskAsync(null));
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

            _dbContext.Tasks.FindAsync(_task.Id).Result.IsDeleted.Should().Be(true);
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

        [Test]
        public void TaskService_DeleteTask_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.DeleteTaskAsync(null, null));
        }

        [Test]
        public async Task TaskService_MarkTaskCompleted_TaskMarkedCompleted()
        {
            var taskService = new TaskService(_dbContext);

            await taskService.MarkTaskCompleteStatusAsync(_task.Id, null, true);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id).IsCompleted.Should().BeTrue();
        }

        [Test]
        public async Task TaskService_MarkTaskIncomplete_TaskMarkedIncomplete()
        {
            var taskService = new TaskService(_dbContext);
            var newTask = new DiaTask {Id = "myId", IsCompleted = true};
            _dbContext.Tasks.Add(newTask);
            await _dbContext.SaveChangesAsync();

            await taskService.MarkTaskCompleteStatusAsync(newTask.Id, null, false);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == newTask.Id).IsCompleted.Should().BeFalse();
        }

        [Test]
        public void TaskService_MarkTaskCompleteThatDoesNotExist_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() =>
                taskService.MarkTaskCompleteStatusAsync("whatTask?", null, true));
        }

        [Test]
        public void TaskService_MarkTaskComplete_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() =>
                taskService.MarkTaskCompleteStatusAsync("whatTask?", null, true));
        }

        #endregion

        #region CommentTests


        [Test]
        public async Task TaskService_GetComments_CommentsReturned()
        {
            var taskService = new TaskService(_dbContext);

            var results = await taskService.GetCommentsAsync(null, _task.Id);

            results.Should().Contain(_task.Comments.FirstOrDefault());
        }

        [Test]
        public void TaskService_GetComments_ThrowsNoDatabaseObjectFoundException()
        {
            _dbContext = GenerateDbContext();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.GetCommentsAsync(null, "abcde"));
        }

        [Test]
        public async Task TaskService_GetComments_ThrowsNoCommentsFoundException()
        {
            _dbContext = GenerateDbContext();
            var newTask = new DiaTask { Id = "abcde" };
            _dbContext.Tasks.Add(newTask);
            await _dbContext.SaveChangesAsync();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoCommentsFoundException>(() => taskService.GetCommentsAsync(null, newTask.Id));
        }

        [Test]
        public async Task TaskService_AddComment_CommentAddedInDatabase()
        {
            var comment = new Comment
            {
                Text = "My new comment",
                UserId = null
            };
            var taskService = new TaskService(_dbContext);

            await taskService.AddCommentAsync(_task.Id, comment);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id).Comments.Should().Contain(comment);
        }

        [Test]
        public void TaskService_AddComment_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.AddCommentAsync("bob", new Comment()));
        }

        [Test]
        public void TaskService_AddComment_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.AddCommentAsync("bob", new Comment()));
        }

        [Test]
        public async Task TaskService_UpdateComment_UpdatesCommentInDatabase()
        {
            var comment = _task.Comments.FirstOrDefault();
            if (comment == null) Assert.Fail("Comment is not present.");
            comment.Text = "Hello world!";

            var taskService = new TaskService(_dbContext);

            await taskService.UpdateCommentAsync(_task.Id, comment);

            var foundComment = _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id)?.Comments.FirstOrDefault();
            foundComment.Text.Should().Be(comment.Text);
        }

        [Test]
        public void TaskService_UpdateCommentTaskNotFound_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);
            var newTask = new DiaTask { Id = "abcde" };

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateCommentAsync(newTask.Id, new Comment()));
        }

        [Test]
        public void TaskService_UpdateCommentButCommentNotFound_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            var newTask = GenerateRandomTask();
            newTask.Id = _task.Id;

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateCommentAsync(newTask.Id, newTask.Comments.FirstOrDefault()));
        }

        [Test]
        public void TaskService_UpdateComment_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.UpdateCommentAsync(_task.Id, _task.Comments.FirstOrDefault()));
        }

        [Test]
        public async Task TaskService_DeleteComment_DeletesCommentFromDatabase()
        {
            var comment = _task.Comments.FirstOrDefault();
            if (comment == null) Assert.Fail("Comment is not present.");
            var taskService = new TaskService(_dbContext);

            await taskService.DeleteCommentAsync(_task.Id, comment.Id, null);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id).Comments.FirstOrDefault(x => x.Id == comment.Id)
                .IsDeleted.Should().Be(true);
        }

        [Test]
        public void TaskService_DeleteCommentForTaskThatDoesNotExist_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() =>
                taskService.DeleteCommentAsync("abcde", "12345", null));
        }

        [Test]
        public void TaskService_DeleteCommentThatDoesNotExist_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() =>
                taskService.DeleteCommentAsync(_task.Id, "12345", null));
        }

        [Test]
        public void TaskService_DeleteComment_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() =>
                taskService.DeleteCommentAsync(_task.Id, "12345", null));
        }

        #endregion

        #region AlertTests


        [Test]
        public async Task TaskService_GetAlerts_AlertsReturned()
        {
            var taskService = new TaskService(_dbContext);

            var results = await taskService.GetAlertsAsync(null, _task.Id);

            results.Should().Contain(_task.AlertTimes.FirstOrDefault());
        }

        [Test]
        public void TaskService_GetAlerts_ThrowsNoDatabaseObjectFoundException()
        {
            _dbContext = GenerateDbContext();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.GetAlertsAsync(null, "abcde"));
        }

        [Test]
        public async Task TaskService_GetAlerts_ThrowsNoAlertsFoundException()
        {
            _dbContext = GenerateDbContext();
            var newTask = new DiaTask { Id = "abcde" };
            _dbContext.Tasks.Add(newTask);
            await _dbContext.SaveChangesAsync();
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoAlertsFoundException>(() => taskService.GetAlertsAsync(null, newTask.Id));
        }

        [Test]
        public async Task TaskService_AddAlert_AlertAddedInDatabase()
        {
            var alertTime = new AlertTime
            {
                Time = DateTimeOffset.UtcNow.AddHours(5)
            };
            var taskService = new TaskService(_dbContext);

            await taskService.AddAlertAsync(_task.Id, alertTime);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id).AlertTimes.Should().Contain(alertTime);
        }

        [Test]
        public void TaskService_AddAlert_ThrowsArgumentException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<ArgumentException>(() => taskService.AddAlertAsync("bob", new AlertTime()));
        }

        [Test]
        public void TaskService_AddAlert_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.AddAlertAsync("bob", new AlertTime{Time = DateTimeOffset.UtcNow.AddHours(2)}));
        }

        [Test]
        public void TaskService_AddAlert_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.AddAlertAsync("bob", new AlertTime { Time = DateTimeOffset.UtcNow.AddHours(2) }));
        }

        [Test]
        public async Task TaskService_UpdateAlert_UpdatesAlertInDatabase()
        {
            var alertTime = _task.AlertTimes.FirstOrDefault();
            if (alertTime == null) Assert.Fail("Alert is not present.");
            alertTime.Time = DateTimeOffset.UtcNow.AddHours(2);

            var taskService = new TaskService(_dbContext);

            await taskService.UpdateAlertAsync(_task.Id, alertTime);

            var foundAlert = _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id)?.AlertTimes.FirstOrDefault();
            foundAlert.Time.Should().Be(alertTime.Time);
        }

        [Test]
        public void TaskService_UpdateAlert_ThrowsArgumentException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<ArgumentException>(() => taskService.UpdateAlertAsync("bob", new AlertTime()));
        }

        [Test]
        public void TaskService_UpdateAlertTaskNotFound_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);
            var newTask = new DiaTask { Id = "abcde" };

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateAlertAsync(newTask.Id, new AlertTime { Time = DateTimeOffset.UtcNow.AddHours(2) }));
        }

        [Test]
        public void TaskService_UpdateAlertButAlertNotFound_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            var newTask = GenerateRandomTask();
            newTask.Id = _task.Id;

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() => taskService.UpdateAlertAsync(newTask.Id, newTask.AlertTimes.FirstOrDefault()));
        }

        [Test]
        public void TaskService_UpdateAlert_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() => taskService.UpdateAlertAsync(_task.Id, _task.AlertTimes.FirstOrDefault()));
        }

        [Test]
        public async Task TaskService_DeleteAlert_DeletesAlertFromDatabase()
        {
            var alertTime = _task.AlertTimes.FirstOrDefault();
            if (alertTime == null) Assert.Fail("Alert is not present.");
            var taskService = new TaskService(_dbContext);

            await taskService.DeleteAlertAsync(_task.Id, alertTime.Id, null);

            _dbContext.Tasks.FirstOrDefault(x => x.Id == _task.Id).AlertTimes.FirstOrDefault(x => x.Id == alertTime.Id)
                .IsDeleted.Should().Be(true);
        }

        [Test]
        public void TaskService_DeleteAlertForTaskThatDoesNotExist_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() =>
                taskService.DeleteAlertAsync("abcde", "12345", null));
        }

        [Test]
        public void TaskService_DeleteAlertThatDoesNotExist_ThrowsNoDatabaseObjectFoundException()
        {
            var taskService = new TaskService(_dbContext);

            Assert.ThrowsAsync<NoDatabaseObjectFoundException>(() =>
                taskService.DeleteAlertAsync(_task.Id, "12345", null));
        }

        [Test]
        public void TaskService_DeleteAlert_ThrowsDatabaseUpdateException()
        {
            var taskService = new TaskService(null);

            Assert.ThrowsAsync<DatabaseUpdateException>(() =>
                taskService.DeleteAlertAsync(_task.Id, "12345", null));
        }

        #endregion
        
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