using Dangl.Data.Shared;
using Dangl.Identity.TestHost.SetupData;
using Dangl.OpenCDE.TestUtilities.TestData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities.BaseTests
{
    /// <summary>
    /// If no <see cref="_client"/> is specified, it will use the default authenticated one.
    /// </summary>
    public abstract class ControllerTestBase<TResponse> : ControllerTestBase
    {
        protected ControllerTestBase(SqlServerDockerCollectionFixture fixture)
            : base(fixture)
        {
        }

        protected TResponse _deserializedResponse;

        protected override async Task<HttpResponseMessage> MakeRequest()
        {
            var baseResponse = await base.MakeRequest();
            if (_response.IsSuccessStatusCode)
            {
                var responseString = await _response.Content.ReadAsStringAsync();
                _deserializedResponse = JsonConvert.DeserializeObject<TResponse>(responseString);
            }

            return baseResponse;
        }
    }

    /// <summary>
    /// If no <see cref="_client"/> is specified, it will use the default authenticated one.
    /// </summary>
    public abstract class ControllerTestBase : IntegrationTestBase
    {
        protected HttpResponseMessage _response;
        protected ApiError _apiError;
        protected HttpClient _client;
        protected UserSetupDto _authenticatedUser = Users.User;
        protected readonly Dictionary<string, object> _queryParams = new Dictionary<string, object>();

        protected ControllerTestBase(SqlServerDockerCollectionFixture fixture)
            : base(fixture)
        {
        }

        protected abstract HttpRequestMessage GetRequest();

        protected virtual async Task<HttpResponseMessage> MakeRequest()
        {
            if (_client == null)
            {
                _client = await _testHelper.GetJwtAuthenticatedClientAsync(_authenticatedUser);
            }

            var response = await _client.SendAsync(GetRequest());
            _response = response;
            if (!_response.IsSuccessStatusCode)
            {
                try
                {
                    _apiError = await GetApiError<ApiError>();
                }
                catch
                {
                    /* Do nothing */
                }
            }

            return response;
        }

        protected async Task<TError> GetApiError<TError>()
        {
            var responseString = await _response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TError>(responseString);
        }

        protected string AppendQueryString(string url)
        {
            if (!_queryParams.Any())
            {
                return url;
            }

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            foreach (var kv in _queryParams)
            {
                queryString[kv.Key] = kv.Value.ToString();
            }

            return $"{url}?{queryString}";
        }
    }
}
