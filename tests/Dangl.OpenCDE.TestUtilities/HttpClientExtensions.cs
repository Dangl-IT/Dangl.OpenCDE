using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, T payload, string url)
        {
            return client.SendAsJsonAsync(payload, url, HttpMethod.Post);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, T payload, string url)
        {
            return client.SendAsJsonAsync(payload, url, HttpMethod.Put);
        }

        private static Task<HttpResponseMessage> SendAsJsonAsync<T>(this HttpClient client, T payload, string url, HttpMethod httpMethod)
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(httpMethod, url)
            {
                Content = jsonContent
            };
            return client.SendAsync(request);
        }
    }
}
