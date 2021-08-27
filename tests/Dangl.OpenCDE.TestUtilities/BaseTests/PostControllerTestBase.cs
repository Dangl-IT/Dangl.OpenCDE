using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Dangl.OpenCDE.TestUtilities.BaseTests
{
    /// <summary>
    /// If <see cref="TPayload"/> is an <see cref="HttpContent"/>, it will be used directly.
    /// Otherwise, it will serialize the <see cref="_payload"/> as json with the <see cref="_jsonPostContentType"/>
    /// as Content-Type.
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class PostControllerTestBase<TPayload, TResponse> : ControllerTestBase<TResponse>
    {
        protected TPayload _payload;
        protected string _jsonPostContentType = "application/json";
        protected string _acceptContentType;

        protected PostControllerTestBase(SqlServerDockerCollectionFixture fixture) : base(fixture)
        {
        }

        protected abstract string GetUrl();

        protected override HttpRequestMessage GetRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrl());

            if (_payload is HttpContent payloadContent)
            {
                request.Content = payloadContent;
            }
            else
            {
                var jsonPayload = JsonConvert.SerializeObject(_payload);
                var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, _jsonPostContentType);
                request.Content = jsonContent;
            }

            if (!string.IsNullOrWhiteSpace(_acceptContentType))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(_acceptContentType));
            }

            return request;
        }
    }

    public abstract class PostControllerTestBase<TPayload> : ControllerTestBase
    {
        protected TPayload _payload;
        protected string _jsonPostContentType = "application/json";
        protected string _acceptContentType;

        protected PostControllerTestBase(SqlServerDockerCollectionFixture fixture) : base(fixture)
        {
        }

        protected abstract string GetUrl();

        protected override HttpRequestMessage GetRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrl());

            if (_payload is HttpContent payloadContent)
            {
                request.Content = payloadContent;
            }
            else
            {
                var jsonPayload = _payload == null ? string.Empty : JsonConvert.SerializeObject(_payload);
                var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, _jsonPostContentType);
                request.Content = jsonContent;
            }

            if (!string.IsNullOrWhiteSpace(_acceptContentType))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(_acceptContentType));
            }

            return request;
        }
    }
}
