using DoItApi.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DoItApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();

            var host = CreateWebHostBuilder(args).Build();
            //using (var scope = host.Services.CreateScope())
            //{
            //    // Retrieve your DbContext isntance here
            //    var dbContext = scope.ServiceProvider.GetService<DoItDbContext>();

            //    // place your DB seeding code here
            //    //DbSeeder.Seed(dbContext);
            //}
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
