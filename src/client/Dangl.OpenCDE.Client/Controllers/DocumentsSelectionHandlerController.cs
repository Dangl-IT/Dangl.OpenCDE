using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Client.Services;
using Dangl.OpenCDE.Shared.Models.CdeApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("documents-selection-handler")]
    public class DocumentsSelectionHandlerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DocumentsSelectionHandlerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPost("start-selection")]
        public async Task<IActionResult> PrepareDocumentSelectionAndOpenSystemBrowserAsync([FromBody] DocumentSelectionInitializationParameters
            parameters)
        {
            if (parameters == null)
            {
                return BadRequest();
            }

            var callbackUrl = Url.Action("HandleCdeCallback", "CdeServerCallback", new
            {
                state = parameters.ClientState
            }, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            var documentsUrl = parameters.OpenCdeBaseUrl
                .TrimEnd('/')
                + "/select-documents";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", parameters.AccessToken);

            var callbackParameters = JObject.FromObject(new DocumentDiscoveryPost
            {
                CallbackLink = new CallbackLink
                {
                    ExpiresIn = 3600,
                    Url = callbackUrl
                }
            });
            callbackParameters["callback_url"] = callbackUrl;

            var initialRequest = GetRequest(callbackParameters, documentsUrl);
            var response = await httpClient.SendAsync(initialRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<DocumentDiscoverySessionInitialization>(responseContent);

                // This is a workaround, since it's required at the moment by some vendors
                var documentSelectionUrl = data.SelectDocumentsUrl += $"&access_token={parameters.AccessToken}";

                SystemBrowserService.OpenSystemBrowser(documentSelectionUrl);
                return NoContent();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return BadRequest(responseContent);
            }
        }

        [ProducesResponseType(typeof(DocumentSelectionCallbackParameters), (int)HttpStatusCode.OK)]
        public IActionResult GetDocumentSelectionCallbackUrl([FromQuery] string clientState)
        {
            var callbackUrl = Url.Action("HandleCdeCallback", "CdeServerCallback", new
            {
                state = clientState
            }, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            return Ok(new DocumentSelectionCallbackParameters
            {
                CallbackUrl = callbackUrl
            });
        }

        [HttpPost("")]
        public IActionResult OpenCdeDocumentSelectionPage(SystemBrowserUrlOpenCommand page)
        {
            SystemBrowserService.OpenSystemBrowser(page.Url);
            return NoContent();
        }

        private HttpRequestMessage GetRequest<T>(T payload, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = jsonContent;

            return request;
        }
    }
}
