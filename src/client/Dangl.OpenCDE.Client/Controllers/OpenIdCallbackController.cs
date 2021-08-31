using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Services;
using IdentityModel.Client;
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

        public OpenIdCallbackController(OpenIdConnectResultPublisher openIdConnectResultPublisher,
            IHttpClientFactory httpClientFactory,
            OpenIdConnectCache openIdConnectCache)
        {
            _httpClientFactory = httpClientFactory;
            _openIdConnectResultPublisher = openIdConnectResultPublisher;
            _openIdConnectCache = openIdConnectCache;
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
                return await HandleOpenIdCodeResponse(code);
            }

            return await HandleOpenIdImplicitResponse(serverResponse);
        }

        private async Task<IActionResult> HandleOpenIdCodeResponse(string code)
        {
            var state = HttpContext.Request.Query["state"].FirstOrDefault();

            var siteContent = "<p>Thank you! You can now close this window.</p>";
            if (string.IsNullOrWhiteSpace(state)
                || !_openIdConnectCache.AuthenticationParametersByClientState.ContainsKey(state))
            {
                await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(state);
            }
            else
            {
                var authenticationParameters = _openIdConnectCache.AuthenticationParametersByClientState[state];
                if (string.IsNullOrWhiteSpace(authenticationParameters.ClientConfiguration.ClientSecret))
                {
                    await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(state);
                }
                else
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var redirectUri = _openIdConnectCache.UsedRedirectUrisByClientState[state];
                    var codeResponse = await httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                    {
                        Address = authenticationParameters.ClientConfiguration.TokenEndpoint,
                        ClientId = authenticationParameters.ClientConfiguration.ClientId,
                        ClientSecret = authenticationParameters.ClientConfiguration.ClientSecret,
                        Code = code,
                        GrantType = "authorization_code",
                        RedirectUri = redirectUri
                    });

                    if (codeResponse.IsError)
                    {
                        await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(state);
                    }
                    else
                    {
                        await _openIdConnectResultPublisher
                            .InformClientsAboutAuthenticationSuccess(state, codeResponse.AccessToken, codeResponse.ExpiresIn);
                    }
                }
            }

            var content = HtmlTemplateProvider.GetHtmlContent(string.Empty, siteContent, "Open ID Connect Authentication");

            return Content(content, "text/html");
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
