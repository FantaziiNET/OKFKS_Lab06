using Microsoft.AspNetCore.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            builder.Services.AddOcelot();

            var app = builder.Build();

            app.UseRouting();
            app.MapControllers();
            app.UseOcelot().Wait();

            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config =>
                    {
                        config.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
