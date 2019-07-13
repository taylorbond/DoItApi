using DoItApi.Settings;
using FluentAssertions;
using NUnit.Framework;

namespace DoItApi.Tests.Settings
{
    [TestFixture]
    public class Auth0SettingsTests
    {
        [Test]
        public void Auth0Settings_SetData_DataSet()
        {
            var settings = new Auth0Settings
            {
                Audience = "test audience",
                Authority = "test authority"
            };

            settings.Audience.Should().NotBeNull();
            settings.Authority.Should().NotBeNull();
        }
    }
}