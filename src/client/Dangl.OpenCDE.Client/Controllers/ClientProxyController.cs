using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("client-proxy")]
    public class ClientProxyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetResponseViaBackend([FromQuery] string targetUrl, [FromQuery] string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await client.GetAsync(targetUrl);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponseContent = await response.Content.ReadAsStringAsync();
                return BadRequest(errorResponseContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return Content(responseContent, "application/json");
        }
    }
}
