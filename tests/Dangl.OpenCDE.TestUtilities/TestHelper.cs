using Dangl.AspNetCore.FileHandling;
using Dangl.Identity.Client.Models;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.Identity.TestHost;
using Dangl.Identity.TestHost.SetupData;
using Dangl.OpenCDE.Data;
using Dangl.OpenCDE.TestUtilities.TestData;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public class TestHelper
    {
        private TestServer _testServer;
        private readonly string _databaseConnectionString;
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private readonly string _masterDatabaseConnectionString;
        private readonly string _sqlServerDockerContainerId;
        private readonly DanglIdentityTestServerManager _danglIdentityTestServerManager;

        public string DatabaseName => _databaseName;
        public string DatabaseConnectionString => _databaseConnectionString;
        public DanglIdentityTestServerManager DanglIdentityTestServerManager => _danglIdentityTestServerManager;

        public TestHelper(string databaseConnectionString,
            string masterDatabaseConnectionString,
            string sqlServerDockerContainerId,
            DanglIdentityTestServerManager danglIdentityTestServerManager)
        {
            _databaseConnectionString = databaseConnectionString
                .Replace(SqlServerDockerCollectionFixture.DATABASE_NAME_PLACEHOLDER, _databaseName);
            _masterDatabaseConnectionString = masterDatabaseConnectionString;
            _sqlServerDockerContainerId = sqlServerDockerContainerId;
            _danglIdentityTestServerManager = danglIdentityTestServerManager;
        }

        public Action<IServiceCollection> ConfigureTestServices { get; set; }

        public async Task InitializeTestServer()
        {
            if (_testServer != null)
            {
                return;
            }

            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Production")
                .ConfigureLogging(c => c.AddDebug())
                .ConfigureServices((_, services) =>
                {
                    services.ConfigureIntegrationTestServices(_databaseConnectionString, () => DanglIdentityTestServerManager.TestServer.CreateHandler());

                    ConfigureTestServices?.Invoke(services);
                })
                .Configure((ctx, app) => app.ConfigureIntegrationTestApp(ctx.HostingEnvironment));
            var testServer = new TestServer(webHostBuilder);
            await SeedDatabase(testServer);
            testServer.BaseAddress = new Uri("https://example.com");
            _testServer = testServer;
        }

        public async Task SeedDatabase(TestServer testServer)
        {
            await DockerSqlDatabaseUtilities
                .CopySeedDatabaseFilesForNewDatabase(_databaseName,
                _sqlServerDockerContainerId);

            var maxTries = 20;
            var currentTry = 0;
            var hasCreatedDb = false;
            while (!hasCreatedDb && currentTry < maxTries)
            {
                // There's a problem where SQL Server in Docker on a Linux host might sometimes fail to
                // create a database in parallelized workflows, such as in the integration tests.
                try
                {
                    await using var sqlConnection = new SqlConnection(_masterDatabaseConnectionString);
                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync($"CREATE DATABASE [{_databaseName}]" +
                        $" ON (FILENAME = '/var/opt/mssql/data/{_databaseName}.mdf')," +
                        $"    (FILENAME = '/var/opt/mssql/data/{_databaseName}_log.ldf')" +
                        $" FOR ATTACH");
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

            await DatabaseSeeder.GenerateSeedFilesAsync(testServer.Services);
        }

        public TestServer GetTestServer()
        {
            return _testServer;
        }

        public HttpMessageHandler GetHttpMessageHandler()
        {
            return GetTestServer().CreateHandler();
        }

        public IServiceScope GetScope()
        {
            return GetTestServer().Host.Services.CreateScope();
        }

        public CdeDbContext GetNewCdeDbContext()
        {
            return GetScope().ServiceProvider.GetRequiredService<CdeDbContext>();
        }

        public HttpClient GetAnonymousClient()
        {
            var httpClient = GetTestServer().CreateClient();

            // The default is 100 seconds, we're now using 5 minutes / 300 seconds
            // I've encountered issues at the CI server, when many builds are running in
            // parallel, then some long running tasks like generating reports could sometimes
            // fail the integration tests due to timeouts here
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            return httpClient;
        }

        public async Task<HttpClient> GetJwtAuthenticatedClientAsync(UserSetupDto user = null)
        {
            user ??= Users.User;
            var jwtToken = await GetJwtTokenAsync(user.Email, user.Password);
            var client = GetAnonymousClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.AccessToken);
            return client;
        }

        public async Task<HttpClient> GetJwtAuthenticatedClientCredentialsClientAsync(ClientSetupDto client)
        {
            var jwtToken = await DanglIdentityTestServerManager.GetJwtClientCredentialsGrantTokenAsync(client.ClientId, client.ClientSecret, IntegrationTestConstants.REQUIRED_SCOPE);
            var httpClient = GetAnonymousClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.AccessToken);
            return httpClient;
        }

        public InstanceInMemoryFileManager GetFileManager()
        {
            using var scope = GetScope();
            var serviceProvider = scope.ServiceProvider;
            var fileManager = serviceProvider.GetRequiredService<IFileManager>() as InstanceInMemoryFileManager;
            return fileManager;
        }

        public Task CleanupTestsAndDropDatabaseAsync()
        {
            if (Environment.GetEnvironmentVariable("DANGL_OPENCDE_IGNORE_SQLSERVER_PARALLEL_LIMIT") == "true")
            {
                // Just doing fire and forget to free up the test cases faster, otherwise each test would take around
                // 3 seconds longer to free up the database.
                Task.Run(async () =>
                {
                    await CleanupTestsAndDropDatabaseInternalAsync();
                });

                return Task.CompletedTask;
            }
            else
            {
                return CleanupTestsAndDropDatabaseInternalAsync();
            }
        }

        private async Task CleanupTestsAndDropDatabaseInternalAsync()
        {
            using var scope = GetScope();
            var serviceProvider = scope.ServiceProvider;
            var fileManager = serviceProvider.GetRequiredService<IFileManager>() as InstanceInMemoryFileManager;
            fileManager.ClearFiles();

            var integrationTestMemoryCache = scope.ServiceProvider.GetRequiredService<IUserInfoUpdaterCache>() as MockUserInfoUpdaterCache;
            integrationTestMemoryCache.Clear();

            await using var context = serviceProvider.GetRequiredService<CdeDbContext>();
            await context.Database.EnsureDeletedAsync();
            try
            {
                await DockerSqlDatabaseUtilities.RemoveDatabaseFilesInContainer(_databaseName, _sqlServerDockerContainerId);
            }
            catch
            {
                // We're ignoring errors here, the files are usually removed by SQL server itself, and latest
                // when the Docker container stops due to them being just in memory
            }
        }

        public async Task<TokenResponseGet> GetJwtTokenAsync(string userIdentifier, string password)
        {
            var client = GetAnonymousClient();
            var loginModel = new TokenLoginPost
            {
                Identifier = userIdentifier,
                Password = password
            };

            const string tokenUrl = "/identity/token-login";
            var tokenResponse = await client.PostAsJsonAsync(loginModel, tokenUrl);
            var responseString = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponseGet>(responseString);
            return token;
        }
    }
}
