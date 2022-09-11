using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extentions
{
    public static class HostExtentions
    {
        public static IHost MigrateDatabase<T> (this IHost  host
            , Action<T,IServiceProvider> seeder, int retry = 0) where T : DbContext
        {
            int retryCount = retry;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<T>>();
                var context = services.GetService<T>();

                try
                {
                    InvokeSeeder(seeder, context, services);
                }
                catch (SqlException ex)
               {

                    if(retryCount < 50)
                    {
                        retryCount++;
                        Thread.Sleep(2000);
                        MigrateDatabase<T>(host, seeder, retryCount);
                    }
                }

            }


                return host;
        }

        public static void InvokeSeeder<T>(Action<T,IServiceProvider> seeder,T context, IServiceProvider service) where T:DbContext
        {
            context.Database.Migrate();
            seeder(context, service);
        }
    }
}
