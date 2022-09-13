using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Dangl.OpenCDE.Shared.Utilities
{
    public static class HttpRequestHelper
    {
        public static HttpRequestMessage GetJsonPostRequest<T>(T payload, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = jsonContent;

            return request;
        }
    }
}
