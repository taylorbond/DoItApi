using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Comment _comment;
        private AlertTime _alert;

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

            _comment = new Comment {Id = Guid.NewGuid().ToString(), Text = "Hi2"};
            _alert = new AlertTime{Time = DateTimeOffset.UtcNow.AddHours(5)};
        }

        #region TaskTests
        [Test]
        public async Task TaskController_GetTasks_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (OkObjectResult)response;

            ((IEnumerable<DiaTask>)result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetTasks_TasksReturned()
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
        public async Task TaskController_GetTasksThrowsNoTaskFoundException_ReturnsNotFound()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksAsync(null)).ThrowsAsync(new NoTasksFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksAsync();
            var result = (NotFoundResult)response;

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
        public async Task TaskController_GetTasksWithDetails_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksWithDetailsAsync();
            var result = (OkObjectResult)response;

            ((IEnumerable<DiaTask>)result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetTasksWithDetails_TasksReturned()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksWithDetailsAsync(null)).ReturnsAsync(new List<DiaTask> { _task });
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksWithDetailsAsync();
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<DiaTask>)result.Value;

            tasks.Should().HaveCount(1);
        }

        [Test]
        public async Task TaskController_GetTasksWithDetailsThrowsNoTaskFoundException_ReturnsNotFound()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksWithDetailsAsync(null)).ThrowsAsync(new NoTasksFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksWithDetailsAsync();
            var result = (NotFoundResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetTasksWithDetailsThrowsException_ReturnsBadRequest()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetTasksWithDetailsAsync(null)).ThrowsAsync(new Exception());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetTasksWithDetailsAsync();
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostTask_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostTaskAsync(_task);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostTask_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddTaskAsync(It.IsAny<DiaTask>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostTaskAsync(_task);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateTask_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            _task.TaskDescription = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateTaskAsync(_task);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateTaskNoDatabaseObject_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateTaskAsync(It.IsAny<DiaTask>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);
            _task.TaskDescription = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateTaskAsync(_task);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateTaskDatabaseUpdateException_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateTaskAsync(It.IsAny<DiaTask>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);
            _task.TaskDescription = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateTaskAsync(_task);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [TestCase("", ExpectedResult = true)]
        [TestCase("     ", ExpectedResult = true)]
        [TestCase(null, ExpectedResult = true)]
        public async Task<bool> TaskController_UpdateTaskNoIdOnTask_BadRequest(string id)
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            _task.Id = id;

            var response = await controller.UpdateTaskAsync(_task);
            var result = (BadRequestObjectResult)response;

            return result != null;
        }

        [Test]
        public async Task TaskController_DeleteTask_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteTaskAsync(_task.Id);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteTask_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteTaskAsync(_task.Id, null))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteTaskAsync(_task.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteTask_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteTaskAsync(_task.Id, null))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteTaskAsync(_task.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskComplete_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskComplete(_task.Id);
            var result = (OkResult) response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskComplete_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x =>
                    x.MarkTaskCompleteStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException("can't find it..."));
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskComplete(_task.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskComplete_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x =>
                    x.MarkTaskCompleteStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskComplete(_task.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskIncomplete_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskIncomplete(_task.Id);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskIncomplete_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x =>
                    x.MarkTaskCompleteStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException("can't find it..."));
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskIncomplete(_task.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_MarkTaskIncomplete_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x =>
                    x.MarkTaskCompleteStatusAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.MarkTaskIncomplete(_task.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }
        #endregion

        #region CommentTests


        [Test]
        public async Task TaskController_GetComments_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetCommentsAsync(_task.Id);
            var result = (OkObjectResult)response;

            ((IEnumerable<Comment>)result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetComments_CommentsReturned()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetCommentsAsync(null, _task.Id)).ReturnsAsync(_task.Comments);
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetCommentsAsync(_task.Id);
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<Comment>)result.Value;

            tasks.Should().HaveCount(1);
        }

        [Test]
        public async Task TaskController_GetCommentsThrowsNoCommentsFoundException_ReturnsNotFound()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetCommentsAsync(null, _task.Id)).ThrowsAsync(new NoCommentsFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetCommentsAsync(_task.Id);
            var result = (NotFoundResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetCommentsThrowsException_ReturnsBadRequest()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetCommentsAsync(null, _task.Id)).ThrowsAsync(new Exception());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetCommentsAsync(_task.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostComments_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostCommentsAsync(_task.Id, _comment);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostComments_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostCommentsAsync(_task.Id, _comment);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostCommentsButTaskNotfound_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException("can't find it..."));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostCommentsAsync(_task.Id, _comment);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateComments_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");

            currentComment.Text = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateCommentAsync(_task.Id, currentComment.Id, currentComment);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateCommentsNoDatabaseObject_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");

            currentComment.Text = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateCommentAsync(_task.Id, currentComment.Id, currentComment);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateCommentsDatabaseUpdateException_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");

            currentComment.Text = $"{_task.Id} newly updated description.";

            var response = await controller.UpdateCommentAsync(_task.Id, currentComment.Id, currentComment);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [TestCase("", ExpectedResult = true)]
        [TestCase("     ", ExpectedResult = true)]
        [TestCase(null, ExpectedResult = true)]
        public async Task<bool> TaskController_UpdateCommentsNoIdOnComments_BadRequest(string id)
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");
            currentComment.Id = id;

            var response = await controller.UpdateCommentAsync(_task.Id, currentComment.Id, currentComment);
            var result = (BadRequestObjectResult)response;

            return result != null;
        }

        [TestCase("", ExpectedResult = true)]
        [TestCase("     ", ExpectedResult = true)]
        [TestCase(null, ExpectedResult = true)]
        public async Task<bool> TaskController_UpdateCommentsNoIdOnTask_BadRequest(string id)
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");
            _task.Id = id;

            var response = await controller.UpdateCommentAsync(_task.Id, currentComment.Id, currentComment);
            var result = (BadRequestObjectResult)response;

            return result != null;
        }

        [Test]
        public async Task TaskController_DeleteComments_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentComment = _task.Comments.FirstOrDefault();
            if (currentComment == null) Assert.Fail("currentComment isn't available.");

            var response = await controller.DeleteCommentAsync(_task.Id, currentComment.Id);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteComment_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteCommentAsync(_task.Id, _comment.Id, null))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteCommentAsync(_task.Id, _comment.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteComment_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteCommentAsync(_task.Id,It.IsAny<string>(), null))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteCommentAsync(_task.Id, _comment.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        #endregion

        #region AlertTests

        [Test]
        public async Task TaskController_GetAlerts_OkResultWithValuesReturned()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetAlertsAsync(_task.Id);
            var result = (OkObjectResult)response;

            ((IEnumerable<AlertTime>)result.Value).Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetAlerts_AlertsReturned()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetAlertsAsync(null, _task.Id)).ReturnsAsync(_task.AlertTimes);
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetAlertsAsync(_task.Id);
            var result = (OkObjectResult)response;
            var tasks = (IEnumerable<AlertTime>)result.Value;

            tasks.Should().HaveCount(1);
        }

        [Test]
        public async Task TaskController_GetAlertsThrowsNoAlertsFoundException_ReturnsNotFound()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetAlertsAsync(null, _task.Id)).ThrowsAsync(new NoAlertsFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetAlertsAsync(_task.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_GetAlertsThrowsException_ReturnsBadRequest()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.GetAlertsAsync(null, _task.Id)).ThrowsAsync(new Exception());
            var controller = new TaskController(taskService.Object);

            var response = await controller.GetAlertsAsync(_task.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostAlerts_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostAlertsAsync(_task.Id, _alert);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostAlerts_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostAlertsAsync(_task.Id, _alert);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostAlertsButTaskNotFound_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException("can't find it..."));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostAlertsAsync(_task.Id, _alert);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_PostAlertsButBadArgument_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.AddAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new ArgumentException("uh oh"));
            var controller = new TaskController(taskService.Object);

            var response = await controller.PostAlertsAsync(_task.Id, _alert);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateAlerts_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");

            currentAlert.Time = DateTimeOffset.UtcNow.AddHours(2);

            var response = await controller.UpdateAlertAsync(_task.Id, currentAlert.Id, currentAlert);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateAlertsNoDatabaseObject_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");

            currentAlert.Time = DateTimeOffset.UtcNow.AddHours(2);

            var response = await controller.UpdateAlertAsync(_task.Id, currentAlert.Id, currentAlert);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_UpdateAlertsDatabaseUpdateException_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");

            currentAlert.Time = DateTimeOffset.UtcNow.AddHours(2);

            var response = await controller.UpdateAlertAsync(_task.Id, currentAlert.Id, currentAlert);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        [TestCase("", ExpectedResult = true)]
        [TestCase("     ", ExpectedResult = true)]
        [TestCase(null, ExpectedResult = true)]
        public async Task<bool> TaskController_UpdateAlertsNoIdOnAlerts_NotFound(string id)
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new NoDatabaseObjectFoundException("sorry"));
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");
            currentAlert.Id = id;

            var response = await controller.UpdateAlertAsync(_task.Id, currentAlert.Id, currentAlert);
            var result = (NotFoundObjectResult)response;

            return result != null;
        }

        [TestCase("", ExpectedResult = true)]
        [TestCase("     ", ExpectedResult = true)]
        [TestCase(null, ExpectedResult = true)]
        public async Task<bool> TaskController_UpdateAlertsNoIdOnTask_BadRequest(string id)
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.UpdateAlertAsync(It.IsAny<string>(), It.IsAny<AlertTime>()))
                .ThrowsAsync(new ArgumentException("sorry"));
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");
            _task.Id = id;

            var response = await controller.UpdateAlertAsync(_task.Id, currentAlert.Id, currentAlert);
            var result = (BadRequestObjectResult)response;

            return result != null;
        }

        [Test]
        public async Task TaskController_DeleteAlerts_OkResult()
        {
            var taskService = new Mock<ITaskService>();
            var controller = new TaskController(taskService.Object);
            var currentAlert = _task.AlertTimes.FirstOrDefault();
            if (currentAlert == null) Assert.Fail("currentAlert isn't available.");

            var response = await controller.DeleteAlertAsync(_task.Id, currentAlert.Id);
            var result = (OkResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteAlert_NotFoundResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteAlertAsync(_task.Id, _comment.Id, null))
                .ThrowsAsync(new NoDatabaseObjectFoundException(It.IsAny<string>()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteAlertAsync(_task.Id, _comment.Id);
            var result = (NotFoundObjectResult)response;

            result.Should().NotBeNull();
        }

        [Test]
        public async Task TaskController_DeleteAlert_BadRequestResult()
        {
            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.DeleteAlertAsync(_task.Id, It.IsAny<string>(), null))
                .ThrowsAsync(new DatabaseUpdateException(new Exception()));
            var controller = new TaskController(taskService.Object);

            var response = await controller.DeleteAlertAsync(_task.Id, _comment.Id);
            var result = (BadRequestObjectResult)response;

            result.Should().NotBeNull();
        }

        #endregion
    }
}