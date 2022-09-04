using Npgsql;

namespace Discount.Grpc.Extentions
{
    public static class HostExtentions
    {
        public static IHost MigrateDatabase<T>(this IHost host, int? retry = 0)
        {
            int retryCount = retry.Value;
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                try
                {
                    var connetion = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connetion.Open();
                    using var command = new NpgsqlCommand
                    {
                        Connection = connetion
                    };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"Create table Coupon(
                                              ID SERIAL PRIMARY KEY NOT NULL,
	                                          ProductName varchar(24) NOT NULL,
	                                          Description text,
	                                          Amount INT
                                            );";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(productname,Description,Amount) values ('IPhone X','IPhone Discount',150);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(productname,Description,Amount) values ('Samsung 10','Samsung Discount',110);";
                    command.ExecuteNonQuery();

                }
                catch (NpgsqlException e)
                {
                    if(retryCount < 50)
                    {
                        retryCount++;
                        Thread.Sleep(2000);
                        MigrateDatabase<T>(host, retryCount);
                    }
                   
                }
                return host;    
            }
        }

    }
}
