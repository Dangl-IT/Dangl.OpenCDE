using System;
using System.Threading.Tasks;
using CommandLine;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.OpenCDE.Data;
using Dangl.OpenCDE.Data.IO;
using Dangl.OpenCDE.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dangl.OpenCDE.DataSeed
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<SeedOptions>(args)
                .MapResult(async seedOptions =>
                {
                    if (string.IsNullOrWhiteSpace(seedOptions.AzureBlobFileManagerConnectionString))
                    {
                        Console.WriteLine("Please provide storage options for Azure Blob Storage");
                        return;
                    }

                    await SeedDatabaseAsync(seedOptions);
                },
                errors =>
                {
                    return Task.CompletedTask;
                })
                .ConfigureAwait(false);
        }

        private static async Task SeedDatabaseAsync(SeedOptions seedOptions)
        {
            var services = new ServiceCollection();

            if (!string.IsNullOrWhiteSpace(seedOptions.AzureBlobFileManagerConnectionString))
            {
                services.AddAzureBlobFileManager(seedOptions.AzureBlobFileManagerConnectionString);
            }
            else
            {
                throw new NotImplementedException("There is no seed code for this storage provider");
            }

            services.AddTransient<AzureBlobStorageInitializer>(x => new AzureBlobStorageInitializer(seedOptions.AzureBlobFileManagerConnectionString));

            services.AddDbContext<CdeDbContext>(sqlBuilder =>
                sqlBuilder.UseSqlServer(seedOptions.SqlConnectionString, options => options.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));

            var serviceProvider = services.BuildServiceProvider();

            var seeder = new DatabaseSeeder(serviceProvider);
            await seeder.SeedDatabaseAsync();
        }
    }
}
