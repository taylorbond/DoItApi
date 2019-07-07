using System;
using DIA.Core.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DoItApi.Tests.Models
{
    [TestFixture]
    public class CommentTests
    {

        [Test]
        public void Comment_ModelCreated_SetWorks()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Do this now.",
                UserId = "myuserid",
                InsertedTime = dateTimeOffset,
                UpdatedTime = dateTimeOffset
            };

            comment.Should().NotBeNull();
        }

        [Test]
        public void Comment_ModelCreated_GetWorks()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Do this now.",
                UserId = "myuserid",
                InsertedTime = dateTimeOffset,
                UpdatedTime = dateTimeOffset
            };

            comment.Id.Should().NotBeNull();
            comment.Text.Should().NotBeNull();
            comment.UserId.Should().NotBeNull();
            comment.InsertedTime.Should().Be(dateTimeOffset);
            comment.UpdatedTime.Should().Be(dateTimeOffset);
        }
    }
}