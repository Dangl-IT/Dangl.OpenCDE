using Dangl.Identity.TestHost;
using Dangl.OpenCDE.Shared.Models.Controllers.FrontendConfig;
using Dangl.OpenCDE.TestUtilities;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Dangl.OpenCDE.TestUtilities.TestData;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration.Controllers
{
    public static class FrontendConfigControllerTests
    {
        public class GetFrontendConfig : ControllerTestBase<FrontendConfigGet>
        {
            public GetFrontendConfig(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = AppendQueryString($"/api/frontend-config");
                return new HttpRequestMessage(HttpMethod.Get, url);
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
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task CorrectResponse()
            {
                await MakeRequest();

                Assert.Null(_deserializedResponse.ApplicationInsightsInstrumentationKey);
                Assert.Equal("https://icons-dev.dangl-it.com", _deserializedResponse.DanglIconsBaseUrl);
                Assert.Equal(Clients.OpenCdeAppClient.ClientId, _deserializedResponse.DanglIdentityClientId);
                Assert.Equal(DanglIdentityTestServerManager.DANGL_IDENTITY_TESTHOST_BASE_ADDRESS, _deserializedResponse.DanglIdentityUrl);
                Assert.Equal("Production", _deserializedResponse.Environment);
            }
        }

        public class GetFrontendConfigScript : ControllerTestBase
        {
            public GetFrontendConfigScript(SqlServerDockerCollectionFixture fixture) : base(fixture)
            {
            }

            protected override HttpRequestMessage GetRequest()
            {
                var url = AppendQueryString($"/api/frontend-config/config.js");
                return new HttpRequestMessage(HttpMethod.Get, url);
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
                await MakeRequest();
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            public async Task CorrectResponse()
            {
                await MakeRequest();

                var responseString = await _response.Content.ReadAsStringAsync();
                Assert.StartsWith("(function()", responseString);
                Assert.EndsWith("();", responseString);
            }

            [Fact]
            public async Task IncludesNoCacheHeader_WhenNoTimestampQueryParameter()
            {
                await MakeRequest();

                var hasCacheControlHeader = _response.Headers.Contains("Cache-Control");
                Assert.False(hasCacheControlHeader);
            }

            [Fact]
            public async Task IncludesCacheHeader_WhenTimestampQueryParameterPresent()
            {
                _queryParams.Add("timestamp", "01");
                await MakeRequest();

                var hasCacheControlHeader = _response.Headers.Contains("Cache-Control");
                Assert.True(hasCacheControlHeader);
            }
        }
    }
}
