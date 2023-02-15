using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Data;
using Dapper.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dangl.OpenCDE
{
    public class Startup
    {
        public Startup(IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var openCdeSettings = Configuration.Get<OpenCdeSettings>();
            if (openCdeSettings.StorageSettings == null)
            {
                openCdeSettings.StorageSettings = new StorageSettings();
            }

            var sqlServerConnectionString = Configuration.GetConnectionString("SqlServer");
            openCdeSettings.Validate();
            services.AddOpenCdeServices(openCdeSettings);

            if (!string.IsNullOrWhiteSpace(openCdeSettings.ApplicationInsightsInstrumentationKey))
            {
                services.AddApplicationInsightsTelemetry();
            }

            services.AddDbContext<CdeDbContext>(sqlBuilder =>
                sqlBuilder.UseSqlServer(sqlServerConnectionString, options => options.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));

            if (!openCdeSettings.StorageSettings.UseCustomFileManager)
            {
                if (!string.IsNullOrWhiteSpace(openCdeSettings.StorageSettings.AzureBlobFileManagerConnectionString))
                {
                    services.AddAzureBlobFileManager(openCdeSettings.StorageSettings.AzureBlobFileManagerConnectionString);
                    services.AddTransient<Data.IO.AzureBlobStorageInitializer>(_ => new Data.IO.AzureBlobStorageInitializer(openCdeSettings.StorageSettings.AzureBlobFileManagerConnectionString));
                }
                else
                {
                    throw new Exception("Failed to instantiate correct storage from given options, neither Azure nor a custom file manager was specified.");
                }
            }

            services.AddDbConnectionFactory(_ => new SqlConnection(sqlServerConnectionString));
            services.AddTransient<IDapperSqlConnectionProvider, DapperSqlConnectionProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var openCdeSettings = Configuration.Get<OpenCdeSettings>();
            app.ConfigureOpenCdeApp(env, openCdeSettings.DanglIdentitySettings);
        }
    }
}
