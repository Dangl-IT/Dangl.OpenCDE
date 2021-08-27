using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.TestUtilities
{
    public class SqlServerDockerCollectionFixture : IAsyncLifetime
    {
        public const string DATABASE_NAME_PLACEHOLDER = "@@databaseName@@";
        public const string INITIAL_SEED_DATABASE_NAME = "TestSeed";
        private string _dockerContainerId;
        private string _dockerSqlPort;

        public string DockerContainerId => _dockerContainerId;

        public string GetSqlConnectionString()
        {
            return $"Data Source=localhost,{_dockerSqlPort};" +
                $"Initial Catalog={DATABASE_NAME_PLACEHOLDER};" +
                "Integrated Security=False;" +
                "User ID=SA;" +
                $"Password={DockerSqlDatabaseUtilities.SQLSERVER_SA_PASSWORD}";
        }

        public string GetMasterSqlConnectionString()
        {
            return $"Data Source=localhost,{_dockerSqlPort};" +
                "Integrated Security=False;" +
                "User ID=SA;" +
                $"Password={DockerSqlDatabaseUtilities.SQLSERVER_SA_PASSWORD}";
        }

        public async Task InitializeAsync()
        {
            (_dockerContainerId, _dockerSqlPort, _) = await DockerSqlDatabaseUtilities.EnsureDockerStartedAndGetContainerIdAndPortAsync();
            await InitializeSeedDatabaseAsync();
        }

        /// <summary>
        /// This method creates an initial seed database with the test data and then detaches
        /// it from the server, so to initialize other test databases we can just copy the
        /// database files and don't have to seed every new database.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeSeedDatabaseAsync()
        {
            var databaseConnectionString = GetSqlConnectionString()
                .Replace(DATABASE_NAME_PLACEHOLDER, INITIAL_SEED_DATABASE_NAME);
            var masterDatabaseConnectionString = GetMasterSqlConnectionString();
            var services = new ServiceCollection();
            var environmentMock = new Mock<IWebHostEnvironment>();
            services.ConfigureIntegrationTestServices(databaseConnectionString);
            var serviceProvider = services.BuildServiceProvider();
            await DatabaseInitializer.InitializeDatabase(serviceProvider,
                masterDatabaseConnectionString,
                INITIAL_SEED_DATABASE_NAME);

            await using var sqlConnection = new SqlConnection(masterDatabaseConnectionString);
            await sqlConnection.OpenAsync();
            await sqlConnection.ExecuteAsync($"ALTER DATABASE {INITIAL_SEED_DATABASE_NAME} SET RECOVERY SIMPLE");
            await sqlConnection.ExecuteAsync($"ALTER DATABASE {INITIAL_SEED_DATABASE_NAME} SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            await sqlConnection.ExecuteAsync($"EXEC sp_detach_db @dbname = N'{INITIAL_SEED_DATABASE_NAME}'");
        }

        public Task DisposeAsync()
        {
            return DockerSqlDatabaseUtilities.EnsureDockerStoppedAndRemovedAsync(_dockerContainerId);
        }
    }
}
