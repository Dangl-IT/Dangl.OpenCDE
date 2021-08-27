using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Dangl.OpenCDE.TestUtilities.BaseTests
{
    public abstract class IntegrationTestBase : IAsyncLifetime, IAssemblyFixture<SqlServerDockerCollectionFixture>
    {
        protected readonly SqlServerDockerCollectionFixture _fixture;
        protected TestHelper _testHelper;
        private static readonly SemaphoreSlim _sempahore = new SemaphoreSlim(40);

        protected IntegrationTestBase(SqlServerDockerCollectionFixture fixture)
        {
            _fixture = fixture;
        }

        public virtual async Task InitializeAsync()
        {
            if (!ShouldIgnoreParallelizationLimit())
            {
                await _sempahore.WaitAsync();
            }

            var sqlConnectionString = _fixture.GetSqlConnectionString();
            var masterSqlConnectionString = _fixture.GetMasterSqlConnectionString();
            var sqlServerDockerContainerId = _fixture.DockerContainerId;
            _testHelper = new TestHelper(sqlConnectionString, masterSqlConnectionString, sqlServerDockerContainerId);
            _testHelper.ConfigureTestServices = ConfigureTestServices;
            await _testHelper.InitializeTestServer();
        }

        public Action<IServiceCollection> ConfigureTestServices { get; set; }

        public virtual async Task DisposeAsync()
        {
            if (!ShouldIgnoreParallelizationLimit())
            {
                _sempahore.Release();
            }
            await _testHelper.CleanupTestsAndDropDatabaseAsync();
        }

        private static bool ShouldIgnoreParallelizationLimit()
        {
            return Environment.GetEnvironmentVariable("DANGL_OPENCDE_IGNORE_SQLSERVER_PARALLEL_LIMIT") == "true";
        }
    }
}
