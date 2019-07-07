using System;
using System.Collections.Generic;
using DoItApi.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DoItApi.Tests.Models
{
    [TestFixture]
    public class TaskTests
    {
        [Test]
        public void Task_ModelCreated_SetWorks()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
            var task = new Task
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "myuserid",
                TaskDescription = "Here's my task description.",
                DueDateTime = dateTimeOffset,
                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = "Hi" } }
            };

            task.Should().NotBeNull();
        }

        [Test]
        public void Task_ModelCreated_GetWorks()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var task = new Task
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "myuserid",
                TaskDescription = "Here's my task description.",
                DueDateTime = dateTimeOffset
            };

            task.Id.Should().NotBeNull();
            task.UserId.Should().NotBeNull();
            task.TaskDescription.Should().NotBeNull().And.Should().NotBe(string.Empty);
            task.DueDateTime.Should().Be(dateTimeOffset);
            task.AlertTimes.Should().NotBeNull();
            task.Comments.Should().NotBeNull();
        }
    }
}