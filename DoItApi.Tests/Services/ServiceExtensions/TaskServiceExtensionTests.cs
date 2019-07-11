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
            var configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DoItDbContext"]).Returns("TestConnectionString");
            var configurationStub = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);

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