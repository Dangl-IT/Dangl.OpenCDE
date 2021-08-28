using Dangl.OpenCDE.Shared.Models.Controllers.Documents;
using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Dangl.OpenCDE.TestUtilities.TestData;
using FluentAssertions;
using LightQuery.Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration.Controllers
{
    public static class DocumentsControllerTests
    {
        public class GetAllDocumentsForProjectAsync : ControllerTestBase<PaginationResult<DocumentGet>>
        {
            private Guid _projectId = Projects.Project01.Id;

            public GetAllDocumentsForProjectAsync(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = AppendQueryString($"/api/projects/{_projectId}/documents");
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
                var totalCount = Documents.Values
                    .Where(d => d.ProjectId == _projectId)
                    .Count();
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
            public async Task CanFilterByDocument()
            {
                _queryParams.Add("filter", Documents.Project01_Document01.Name);
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Documents.Project01_Document01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByDocumentName_TwoWords()
            {
                _queryParams.Add("filter", "Pic ture");
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Documents.Project01_Document01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByDocumentName_ThreeWords()
            {
                _queryParams.Add("filter", "Pi ct ure");
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Documents.Project01_Document01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByDocumentName_Uppercase()
            {
                _queryParams.Add("filter", Documents.Project01_Document01.Name.ToUpper());
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Documents.Project01_Document01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task CanFilterByDocumentName_Lowercase()
            {
                _queryParams.Add("filter", Documents.Project01_Document01.Name.ToLower());
                await MakeRequest();

                Assert.Single(_deserializedResponse.Data);
                Assert.Equal(Documents.Project01_Document01.Id, _deserializedResponse.Data[0].Id);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class GetDocumentById : ControllerTestBase<DocumentGet>
        {
            private Guid _projectId = Projects.Project01.Id;
            private Guid _documentId = Documents.Project01_Document01.Id;

            public GetDocumentById(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = AppendQueryString($"/api/projects/{_projectId}/documents/{_documentId}");
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
                Assert.Equal(_documentId, _deserializedResponse.Id);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public async Task NotFoundForInvalidDocumentId()
            {
                _documentId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public async Task NotFoundForDocumentAndProjectIdMismatch()
            {
                _documentId = Documents.Project02_Document01.Id;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class DownloadDocumentAsync : ControllerTestBase
        {
            // Info: For testing, we're not creating an Azurite container, so we're just using the
            // regular InstanceInMemoryFileHandler to give every test run a fresh file manager.
            // This leads to us not testing the SAS download feature of Azure Blob storage, so
            // in case this project ever grows to more than just a small demo app for the
            // OpenCDE Document API, we should reconsider that approach

            private Guid _projectId = Projects.Project01.Id;
            private Guid _documentId = Documents.Project01_Document01.Id;

            public DownloadDocumentAsync(SqlServerDockerCollectionFixture fixture)
                : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = $"/api/projects/{_projectId}/documents/{_documentId}/content";
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

                Assert.Equal("image/jpeg", _response.Content.Headers.ContentType.ToString());
                Assert.Equal(TestFilesFactory.GetTestfileAsStream(TestFile.construction_site_picture).Length, (await _response.Content.ReadAsStreamAsync()).Length);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public async Task BadRequestForInvalidDocumentId()
            {
                _documentId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }

            [Fact]
            public async Task OkForMissmatchBetweenProjectAndDocumentId()
            {
                // We're currently not checking the project id internally,
                // so the request just goes through😀
                _documentId = Documents.Project02_Document01.Id;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task BadRequestForDocumentWithoutContent()
            {
                _documentId = Documents.Project01_Document05.Id;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }
        }

        public class UploadDocumentMetadataForProjectAsync : PostControllerTestBase<DocumentPost, DocumentGet>
        {
            private Guid _projectId = Projects.Project01.Id;

            public UploadDocumentMetadataForProjectAsync(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
                _payload = new DocumentPost
                {
                    Name = "Generated Document",
                    Description = "Standard"
                };
            }

            protected override string GetUrl()
            {
                return $"/api/projects/{_projectId}/documents";
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
            public async Task IndicatesThatDocumentIsNotPersisted()
            {
                await MakeRequest();
                Assert.False(_deserializedResponse.ContentAvailable);
            }

            [Fact]
            public async Task BadRequestForMissingName()
            {
                _payload.Name = string.Empty;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public async Task CreatesInDatabase()
            {
                await MakeRequest();
                await using var context = _testHelper.GetNewCdeDbContext();
                var documentExists = await context.Documents.AnyAsync(d => d.Id == _deserializedResponse.Id);
                Assert.True(documentExists);
            }
        }

        public class UploadDocumentContentAsync : PostControllerTestBase<HttpContent, DocumentGet>
        {
            private Stream _testFileStream;
            private string _mimeType = "application/octet-stream";
            private string _fileName = "picture.jpg";
            private TestFile _testFile = TestFile.construction_site_picture;

            private Guid _projectId = Projects.Project01.Id;
            private Guid _documentId = Documents.Project01_Document05.Id;

            public UploadDocumentContentAsync(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                _testFileStream = TestFilesFactory.GetTestfileAsStream(_testFile);
                var multiPartFormContent = new MultipartFormDataContent();
                var testFileStream = _testFileStream;
                var streamContent = new StreamContent(testFileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_mimeType);
                multiPartFormContent.Add(streamContent, "document", _fileName);
                _payload = multiPartFormContent;
                return base.GetRequest();
            }

            protected override string GetUrl()
            {
                return $"api/projects/{_projectId}/documents/{_documentId}/content";
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

                Assert.True(_deserializedResponse.ContentAvailable);
                var expectedLength = TestFilesFactory.GetTestfileAsStream(_testFile).Length;
                Assert.Equal(expectedLength, _deserializedResponse.FileSizeInBytes);
                Assert.Equal("picture.jpg", _deserializedResponse.FileName);
            }

            [Fact]
            public async Task BadRequestForDocumentWithContentPresent()
            {
                _documentId = Documents.Project01_Document01.Id;
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }

            [Fact]
            public async Task BadRequestForInvalidDocumentId()
            {
                _documentId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }

            [Fact]
            public async Task NotFoundForInvalidProjectId()
            {
                _projectId = Guid.NewGuid();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }
    }
}
