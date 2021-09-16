using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("openid-connect-callback")]
    public class OpenIdCallbackController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenIdConnectResultPublisher _openIdConnectResultPublisher;
        private readonly OpenIdConnectCache _openIdConnectCache;
        private readonly OpenIdAuthenticationRequestHandler _openIdAuthenticationRequestHandler;

        public OpenIdCallbackController(OpenIdConnectResultPublisher openIdConnectResultPublisher,
            IHttpClientFactory httpClientFactory,
            OpenIdConnectCache openIdConnectCache,
            OpenIdAuthenticationRequestHandler openIdAuthenticationRequestHandler)
        {
            _httpClientFactory = httpClientFactory;
            _openIdConnectResultPublisher = openIdConnectResultPublisher;
            _openIdConnectCache = openIdConnectCache;
            _openIdAuthenticationRequestHandler = openIdAuthenticationRequestHandler;
        }

        [HttpGet("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ProcessOpenIdConnectCallback([FromQuery] string serverResponse,
            [FromQuery] string code,
            [FromQuery] string error,
            [FromQuery] string state)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(state);
                var siteContent = "<p>Thank you! You can now close this window.</p>";
                var content = HtmlTemplateProvider.GetHtmlContent(string.Empty, siteContent, "Open ID Connect Authentication");
                return Content(content, "text/html");
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                // This means we've got a response to an authentication code flow request
                var openIdCodeResponse = await _openIdAuthenticationRequestHandler.HandleOpenIdCodeResponse(HttpContext, code);
                return Content(openIdCodeResponse, "text/html");
            }

            return await HandleOpenIdImplicitResponse(serverResponse);
        }

        private async Task<IActionResult> HandleOpenIdImplicitResponse(string serverResponse)
        {
            var redirectScript = string.Empty;
            string siteContent;
            if (string.IsNullOrWhiteSpace(serverResponse))
            {
                redirectScript = @"let hash = window.location.hash;
if (hash) {
  hash = hash.substring(1);
} else {
  hash = 'Error';
}
window.location.href = window.location.origin + window.location.pathname + '?serverResponse=' + encodeURIComponent(hash);";
                siteContent = "<p>Please wait, processing authentication...</p>";
            }
            else
            {
                var queryParams = HttpUtility.ParseQueryString(serverResponse);
                var query = queryParams
                    .Keys
                    .Cast<string>()
                    .ToDictionary(key => key, key => queryParams[key]);
                var state = string.Empty;
                query.TryGetValue("state", out state);

                if (query.TryGetValue("access_token", out var accessToken)
                    && query.TryGetValue("expires_in", out var expiresInRaw)
                    && int.TryParse(expiresInRaw, out var expiresIn))
                {
                    await _openIdConnectResultPublisher.InformClientsAboutAuthenticationSuccess(state, accessToken, expiresIn);
                }
                else
                {
                    await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(state);
                }

                siteContent = "<p>Thank you! You can now close this window.</p>";
            }

            var content = HtmlTemplateProvider.GetHtmlContent(redirectScript, siteContent, "Open ID Connect Authentication");

            return Content(content, "text/html");
        }
    }
}
