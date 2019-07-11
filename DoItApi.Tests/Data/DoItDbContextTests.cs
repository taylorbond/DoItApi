using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Models;
using DoItApi.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;

namespace DoItApi.Tests.Data
{
    [TestFixture]
    public class DoItDbContextTests
    {
        private DoItDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DoItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new DoItDbContext(options);
        }

        [Test]
        public async Task DoItDbContextTests_SetAndGetTasks_SetsProperly()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertDateTimeOffset = new DateTimeOffset(DateTime.UtcNow.AddHours(5));
            var task = new DiaTask
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "myuserid",
                TaskDescription = "Here's my task description.",
                DueDateTime = dateTimeOffset,
                AlertTimes = new List<AlertTime> { new AlertTime { Id = Guid.NewGuid().ToString(), Time = alertDateTimeOffset } },
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid().ToString(), Text = "Hi" } }
            };
            _dbContext.Tasks = new InternalDbSet<DiaTask>(_dbContext);
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            _dbContext.Tasks.Should().Contain(task);
        }
    }
}