using System;
using DIA.Core.Models;
using NUnit.Framework;
using FluentAssertions;

namespace DoItApi.Tests.Models
{
    [TestFixture]
    public class AlertTimeTests
    {
        [Test]
        public void AlertTime_ModelCreated_SetWorks()
        {
            var alertTime = new AlertTime
            {
                Id = Guid.NewGuid().ToString(),
                Time = new DateTimeOffset(DateTime.UtcNow)
            };

            alertTime.Should().NotBeNull();
        }

        [Test]
        public void AlertTime_ModelCreated_GetWorks()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            var alertTime = new AlertTime
            {
                Id = Guid.NewGuid().ToString(),
                Time = dateTimeOffset
            };

            alertTime.Id.Should().NotBeNull();
            alertTime.Time.Should().Be(dateTimeOffset);
        }
    }
}
