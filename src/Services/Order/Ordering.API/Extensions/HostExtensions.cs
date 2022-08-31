using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static WebApplication MigrateDatabase<TContext>(this WebApplication app, 
                                                               Action<TContext, IServiceProvider> seeder,
                                                               int? retry = 0)
                                                                where TContext : DbContext                  
        {
            int retryForAvailability = retry.Value;

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Migrating Order Database");
                    InvokeSeeder(seeder, services, context);
                }
                catch (SqlException ex)
                {
                    logger.LogInformation(ex, "an error occured while migrating the database..");
                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(app, seeder, retryForAvailability);
                    }
                }
            }

                
            return app;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, 
                                                IServiceProvider services, TContext? context)
                                                where TContext: DbContext
        {

            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
