using Dangl.OpenCDE.Shared.Models.Controllers.Projects;
using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Dangl.OpenCDE.TestUtilities.TestData;
using FluentAssertions;
using LightQuery.Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration.Controllers
{
    public static class ProjectsControllerTests
    {
        public class GetAllProjects : ControllerTestBase<PaginationResult<ProjectGet>>
        {
            public GetAllProjects(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = AppendQueryString("/api/projects");
                return new HttpRequestMessage(HttpMethod.Get, url);
            }

            [Fact]
            public async Task UnauthorizedForAnonymousUser()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.Unauthorized, _response.StatusCode);
            }

            [Fact]
            public async Task OkForAuthenticatedUser()
            {
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task CorrectResponse()
            {
                await MakeRequest();
                Assert.NotEmpty(_deserializedResponse.Data);
                Assert.Equal(1, _deserializedResponse.Page);
                Assert.Equal(50, _deserializedResponse.PageSize);
                var totalCount = Projects.Values.Count;
                Assert.Equal(totalCount, _deserializedResponse.TotalCount);
                Assert.Equal(totalCount, _deserializedResponse.Data.Count);
            }

            [Fact]
            public async Task CanPage()
            {
                _queryParams.Add("pageSize", "10");
                await MakeRequest();
                Assert.Equal(10, _deserializedResponse.PageSize);
            }

            [Fact]
            public async Task CanSortByName()
            {
                _queryParams.Add("sort", "name desc");
                await MakeRequest();

                _deserializedResponse.Data.Should().BeInDescendingOrder(p => p.Name);
            }

            [Fact]
            public async Task CanFilterByProjectName()
            {
                _queryParams.Add("filter", Projects.Project01.Name);
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Projects.Project01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByProjectName_TwoWords()
            {
                _queryParams.Add("filter", "Exp ansion");
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Projects.Project01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByProjectName_ThreeWords()
            {
                _queryParams.Add("filter", "Ex pan sion");
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Projects.Project01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByProjectName_Uppercase()
            {
                _queryParams.Add("filter", Projects.Project01.Name.ToUpper());
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Projects.Project01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByProjectName_Lowercase()
            {
                _queryParams.Add("filter", Projects.Project01.Name.ToLower());
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Projects.Project01.Id, _deserializedResponse.Data[0].Id);
            }
        }

        public class GetProjectByIdAsync : ControllerTestBase<ProjectGet>
        {
            private Guid _projectId = Projects.Project01.Id;

            public GetProjectByIdAsync(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                return new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{_projectId}");
            }

            [Fact]
            public async Task UnauthorizedForAnonymousUser()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.Unauthorized, _response.StatusCode);
            }

            [Fact]
            public async Task OkForAuthenticatedUser()
            {
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task CorrectResponse()
            {
                await MakeRequest();
                Assert.Equal(Projects.Project01.Name, _deserializedResponse.Name);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public async Task OkForArchivedProject()
            {
                _projectId = Projects.Project03.Id;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }
        }

        public class CreateProjectAsync : PostControllerTestBase<ProjectPost, ProjectGet>
        {
            public CreateProjectAsync(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
                _payload = new ProjectPost
                {
                    Name = "Generated Project",
                    Description = "Standard"
                };
            }

            protected override string GetUrl()
            {
                return "/api/projects";
            }

            [Fact]
            public async Task UnauthorizedForAnonymousUser()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.Unauthorized, _response.StatusCode);
            }

            [Fact]
            public async Task CreatedForAuthenticatedUser()
            {
                await MakeRequest();
                Assert.Equal(HttpStatusCode.Created, _response.StatusCode);
            }

            [Fact]
            public async Task GeneratesIdenticonId()
            {
                await MakeRequest();
                Assert.NotEqual(Guid.Empty, _deserializedResponse.IdenticonId);
            }

            [Fact]
            public async Task BadRequestForMissingName()
            {
                _payload.Name = string.Empty;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }

            [Fact]
            public async Task CreatesInDatabase()
            {
                await MakeRequest();
                await using var context = _testHelper.GetNewCdeDbContext();
                var projectExists = await context.Projects.AnyAsync(p => p.Id == _deserializedResponse.Id);
                Assert.True(projectExists);
            }
        }
    }
}
