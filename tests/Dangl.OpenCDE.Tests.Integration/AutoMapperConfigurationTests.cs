using AutoMapper;
using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration
{
    public class AutoMapperConfigurationTests : IntegrationTestBase
    {
        public AutoMapperConfigurationTests(SqlServerDockerCollectionFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void ValidateAutoMapperConfig()
        {
            using var scope = _testHelper.GetScope();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
