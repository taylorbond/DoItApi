using DoItApi.Services;
using DoItApi.Services.ServiceExtensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace DoItApi.Tests.Services.ServiceExtensions
{
    [TestFixture]
    public class TaskServiceExtensionTests
    {
        [Test]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            //  Setting up the stuff required for Configuration.GetConnectionString("DefaultConnection")
            var databaseSectionHub = new Mock<IConfigurationSection>();
            databaseSectionHub.Setup(x => x["DoItDbContext"]).Returns("TestConnectionString");

            var auth0SectionHub = new Mock<IConfigurationSection>();
            auth0SectionHub.Setup(x => x["Authority"]).Returns("TesAuthority");
            auth0SectionHub.Setup(x => x["Audience"]).Returns("TestAudience");

            var configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(databaseSectionHub.Object);
            configurationStub.Setup(x => x.GetSection("Auth0Settings")).Returns(auth0SectionHub.Object);

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            
            target.ConfigureServices(services); //  Mimic internal asp.net core logic.
            services.AddTaskService(); 


            var serviceProvider = services.BuildServiceProvider();
            var controller = serviceProvider.GetService<ITaskService>();

            controller.Should().NotBeNull();
        }
    }
}