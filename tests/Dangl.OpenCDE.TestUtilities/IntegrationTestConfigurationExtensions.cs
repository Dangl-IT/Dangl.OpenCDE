using Dangl.AspNetCore.FileHandling;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.Identity.TestHost;
using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Data;
using Dangl.OpenCDE.TestUtilities.TestData;
using Dapper.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dangl.OpenCDE.TestUtilities
{
    public static class IntegrationTestConfigurationExtensions
    {
        public static IServiceCollection ConfigureIntegrationTestServices(this IServiceCollection services,
            string databaseConnectionString)
        {
            services.AddDbContext<CdeDbContext>(sqlBuilder =>
                sqlBuilder.UseSqlServer(databaseConnectionString, options => options.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));

            var appSettings = new OpenCdeSettings
            {
                StorageSettings = new StorageSettings
                {
                    UseCustomFileManager = true
                },
                DanglIdentitySettings = new DanglIdentitySettings
                {
                    BaseUri = DanglIdentityTestServerManager.DANGL_IDENTITY_TESTHOST_BASE_ADDRESS,
                    ClientId = Clients.OpenCdeAppClient.ClientId,
                    ClientSecret = Clients.OpenCdeAppClient.ClientSecret,
                    RequiredScope = IntegrationTestConstants.REQUIRED_SCOPE,
                    CustomBackchannelHttpMessageHandlerFactory = () => TestHelper.DanglIdentityTestServerManager.TestServer.CreateHandler(),
                    UseDefaultInMemoryUserUpdaterCache = false
                },
                AppBaseUrl = "https://localhost",
                DanglIconsBaseUrl = "https://icons-dev.dangl-it.com"
            };

            services.AddOpenCdeServices(appSettings);

            services.AddDbConnectionFactory(_ => new SqlConnection(databaseConnectionString));
            services.AddTransient<IDapperSqlConnectionProvider, DapperSqlConnectionProvider>();

            // These services are provided specifically for testing to ensure there's no context overlap between different test runs
            services.AddSingleton<IUserInfoUpdaterCache, MockUserInfoUpdaterCache>();
            services.AddSingleton<IFileManager, InstanceInMemoryFileManager>();

            return services;
        }

        public static IApplicationBuilder ConfigureIntegrationTestApp(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.ConfigureOpenCdeApp(environment, new DanglIdentitySettings
            {
                BaseUri = DanglIdentityTestServerManager.DANGL_IDENTITY_TESTHOST_BASE_ADDRESS,
                RequiredScope = IntegrationTestConstants.REQUIRED_SCOPE,
                ClientId = Clients.OpenCdeAppClient.ClientId
            });

            return app;
        }
    }
}
