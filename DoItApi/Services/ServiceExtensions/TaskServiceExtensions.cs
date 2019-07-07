using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DoItApi.Services.ServiceExtensions
{
    public static class TaskServiceExtensions
    {
        public static IServiceCollection AddTaskService(this IServiceCollection services)
        {
            //https://andrewlock.net/configuring-environment-specific-services-in-asp-net-core-part-2/
            services.AddTransient<ITaskService, TaskService>();
            return services;
        }
    }
}