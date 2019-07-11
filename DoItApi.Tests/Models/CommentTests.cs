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
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Do this now.",
                UserId = "myuserid",
            };

            comment.Should().NotBeNull();
        }

        [Test]
        public void Comment_ModelCreated_GetWorks()
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Do this now.",
                UserId = "myuserid",
            };

            comment.Id.Should().NotBeNull();
            comment.Text.Should().NotBeNull();
            comment.UserId.Should().NotBeNull();
        }
    }
}