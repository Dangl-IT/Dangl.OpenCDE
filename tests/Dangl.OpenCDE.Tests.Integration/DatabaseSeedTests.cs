using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Dangl.OpenCDE.TestUtilities.TestData;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration
{
    public class DatabaseSeedTests : IntegrationTestBase
    {
            public DatabaseSeedTests(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            [Fact]
            public async Task GeneratesUsers()
            {
                await using var context = _testHelper.GetNewCdeDbContext();
                Assert.Equal(Users.SeedValues.Count, await context.Users.CountAsync());
            }

            [Fact]
            public async Task GeneratesProjects()
            {
                await using var context = _testHelper.GetNewCdeDbContext();
                Assert.Equal(Projects.Values.Count, await context.Projects.CountAsync());
            }

            [Fact]
            public async Task GeneratesFileMimeTypes()
            {
                await using var context = _testHelper.GetNewCdeDbContext();
                Assert.Equal(CdeAppFileMimeTypes.Values.Count, await context.FileMimeTypes.CountAsync());
            }

            [Fact]
            public async Task GeneratesFiles()
            {
                await using var context = _testHelper.GetNewCdeDbContext();
                Assert.Equal(CdeAppFiles.Values.Count, await context.Files.CountAsync());
            }

            [Fact]
            public async Task GeneratesDocuments()
            {
                await using var context = _testHelper.GetNewCdeDbContext();
                Assert.Equal(Documents.Values.Count, await context.Documents.CountAsync());
            }
    }
}
