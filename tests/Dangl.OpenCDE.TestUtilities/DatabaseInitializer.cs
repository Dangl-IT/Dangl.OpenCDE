using Dangl.OpenCDE.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public static class DatabaseInitializer
    {
        public static async Task CreateEmptyDatabaseAsync(string masterDatabaseConnectionString, string databaseName)
        {
            await using var sqlConnection = new SqlConnection(masterDatabaseConnectionString);
            await sqlConnection.OpenAsync();
            await sqlConnection.ExecuteAsync($"CREATE DATABASE [{databaseName}];");
            await sqlConnection.ExecuteAsync($"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;");
        }
        
        public static async Task InitializeDatabase(IServiceProvider serviceProvider, string masterDatabaseConnectionString, string databaseName)
        {
            var maxTries = 20;
            var currentTry = 0;

            var hasCreatedDb = false;
            while (!hasCreatedDb && currentTry < maxTries)
            {
                // There's a problem where SQL Server in Docker on a Linux host might sometimes fail to
                // create a database in parallelized workflows, such as in the integration tests.
                try
                {
                    await using var sqlConnection = new SqlConnection(masterDatabaseConnectionString);
                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync($"CREATE DATABASE [{databaseName}];");
                    await sqlConnection.ExecuteAsync($"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;");
                    hasCreatedDb = true;
                }
                catch
                {
                    currentTry++;
                    if (currentTry >= maxTries)
                    {
                        throw;
                    }
                    await Task.Delay(100);
                }
            }

            currentTry = 0;
            while (currentTry < maxTries)
            {
                try
                {
                    using var serviceScope = serviceProvider.CreateScope();
                    var scopedServiceProvider = serviceScope.ServiceProvider;
                    await using var dbContext = scopedServiceProvider.GetRequiredService<CdeDbContext>();
                    await dbContext.Database.MigrateAsync();
                    var seeder = new DatabaseSeeder(serviceProvider);
                    await seeder.SeedDatabaseAsync();
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    currentTry++;
                    if (currentTry >= maxTries)
                    {
                        throw;
                    }
                    else
                    {
                        if (currentTry > 5)
                        {
                            await Task.Delay(500);
                        }
                        // Ensure the database is in an empty state
                        try
                        {
                            using var serviceScope = serviceProvider.CreateScope();
                            var dapperSqlProvider = serviceScope.ServiceProvider.GetRequiredService<IDapperSqlConnectionProvider>();
                            var databaseCleaner = new DatabaseCleaner(dapperSqlProvider);
                            await databaseCleaner.DropAllTablesInDatabaseAsync();
                        }
                        catch
                        {
                            // But ignore failures here
                        }
                    }
                }
            }
        }
    }
}
