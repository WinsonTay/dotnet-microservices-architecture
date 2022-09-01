using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var hostingEnvironment = builder.Environment.EnvironmentName;
            builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Configuration.AddJsonFile($"ocelot.{hostingEnvironment}.json",true,true);

            var app = builder.Build();
            app.UseRouting();
           
            app.MapGet("/", () => "Hello World!");
            app.UseOcelot().Wait();
            app.Run();
        }
    }
}