using Dangl.OpenCDE.Shared;
using Dangl.OpenCDE.Shared.Models.Controllers.Status;
using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration.Controllers
{
    public static class StatusControllerTests
    {
        public class GetStatus : ControllerTestBase<StatusGet>
        {
            private bool _sendHttpHead = false;

            public GetStatus(SqlServerDockerCollectionFixture fixture)
                : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                return new HttpRequestMessage(_sendHttpHead ? HttpMethod.Head : HttpMethod.Get, "/api/status");
            }

            [Fact]
            public async Task OkForAnonymousUser()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task OkForAuthenticatedUser()
            {
                _client = await _testHelper.GetJwtAuthenticatedClientAsync();
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task CorrectResponse()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                Assert.True(_deserializedResponse.IsHealthy);
                Assert.Equal(VersionsService.Version, _deserializedResponse.Version);
                Assert.Equal("Production", _deserializedResponse.Environment);
            }

            [Fact]
            public async Task ResponseIsCamelCased()
            {
                _client = _testHelper.GetAnonymousClient();
                await MakeRequest();
                var responseString = await _response.Content.ReadAsStringAsync();
                Assert.Contains("isHealthy", responseString);
                Assert.DoesNotContain("IsHealthy", responseString);
            }

            [Fact]
            public async Task CanMakeHeadRequest_WithAuthenticatedClient()
            {
                _client = await _testHelper.GetJwtAuthenticatedClientAsync();
                _sendHttpHead = true;
                await MakeRequest();
                Assert.True(_response.IsSuccessStatusCode);
            }

            [Fact]
            public async Task CanMakeHeadRequest_WithNotAuthenticatedClient()
            {
                _client = _testHelper.GetAnonymousClient();
                _sendHttpHead = true;
                await MakeRequest();
                Assert.True(_response.IsSuccessStatusCode);
            }
        }
    }
}
