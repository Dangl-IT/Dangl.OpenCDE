using Dangl.AspNetCore.FileHandling;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dangl.OpenCDE
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            ConfigureSerilog();
            try
            {
                Log.Information("Starting web host");
                var webHost = CreateHostBuilder(args).Build();

                using (var scope = webHost.Services.CreateScope())
                {
                    await InitializeDatabaseAsync(scope);
                    if (scope.ServiceProvider.GetRequiredService<IFileManager>() is AzureBlobFileManager azureBlobFileManager)
                    {
                        var azureInitializer = scope.ServiceProvider.GetRequiredService<Data.IO.AzureBlobStorageInitializer>();

                        await azureInitializer.EnsureAzureBlobContainersPresentAsync();
                        await azureInitializer.EnsureAzureBlobStorageHasCorsEnabledAsync();
                    }
                }

                await webHost.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseSerilog()
                .ConfigureLogging((c, lb) =>
                {
                    if (!c.HostingEnvironment.IsDevelopment())
                    {
                        lb.AddAzureWebAppDiagnostics();
                        lb.AddApplicationInsights();
                    }
                    lb.AddSerilog();
                });

        private static async Task InitializeDatabaseAsync(IServiceScope scope)
        {
            // This will try to reach the database and perform the migration multiple times in case
            // of a database that is not yet ready
            const int maxTimeoutInSeconds = 30;
            var start = DateTime.UtcNow;
            var initialized = false;
            while (!initialized && DateTime.UtcNow < start.AddSeconds(maxTimeoutInSeconds))
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<CdeDbContext>();
                    await context.Database.MigrateAsync();
                    initialized = true;
                }
                catch when (!Debugger.IsAttached)
                {
                    if (DateTime.UtcNow < start.AddSeconds(30))
                    {
                        await Task.Delay(1_000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private static void ConfigureSerilog()
        {
            const string logOutputTemplate = "[{Timestamp:HH:mm:ss} {MachineName} {Level:u3} {RequestId}] {SourceContext}{NewLine}    {Message:lj}{NewLine}{Exception}";
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug(outputTemplate: logOutputTemplate)
                .WriteTo.Console(outputTemplate: logOutputTemplate);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var appSettings = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build()
                .Get<OpenCdeSettings>();

            if (!string.IsNullOrWhiteSpace(appSettings?.StorageSettings?.AzureBlobStorageLogConnectionString))
            {
                loggerConfiguration.WriteTo.AzureBlobStorage(appSettings.StorageSettings.AzureBlobStorageLogConnectionString,
                    storageFileName: $"{{yyyy}}/{{MM}}/{{dd}}/{{HH}}/log-{environment}.txt",
                    outputTemplate: logOutputTemplate,
                    storageContainerName: "opencde",
                    writeInBatches: true,
                    period: TimeSpan.FromSeconds(15),
                    batchPostingLimit: 10);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
