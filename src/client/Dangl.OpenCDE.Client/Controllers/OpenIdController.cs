using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("openid-connect")]
    public class OpenIdController : Controller
    {
        private readonly OpenIdConnectResultPublisher _openIdConnectResultPublisher;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenIdConnectCache _openIdConnectCache;

        public OpenIdController(OpenIdConnectResultPublisher openIdConnectResultPublisher,
            IHttpClientFactory httpClientFactory,
            OpenIdConnectCache openIdConnectCache)
        {
            _openIdConnectResultPublisher = openIdConnectResultPublisher;
            _httpClientFactory = httpClientFactory;
            _openIdConnectCache = openIdConnectCache;
        }

        [HttpPost("")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> InitiateOpenIdConnectImplicitLoginAsync([FromBody] OpenIdConnectAuthenticationParameters authenticationParameters)
        {
            switch (authenticationParameters.Flow)
            {
                case OpenIdConnectFlowType.Implicit:
                    await InitiateImplicitAuthenticationFlowAsync(authenticationParameters);
                    break;

                case OpenIdConnectFlowType.AuthorizationCode:
                    await InitiateCodeAuthenticationFlowAsync(authenticationParameters);
                    break;

                default:
                    // Looks like we've encountered a non-supported flow here
                    await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(authenticationParameters.ClientState);
                    break;
            }

            return NoContent();
        }

        private async Task InitiateCodeAuthenticationFlowAsync(OpenIdConnectAuthenticationParameters authenticationParameters)
        {
            if (authenticationParameters.Flow != OpenIdConnectFlowType.AuthorizationCode)
            {
                await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(authenticationParameters.ClientState);
                return;
            }

            if (string.IsNullOrWhiteSpace(authenticationParameters.ClientConfiguration.ClientSecret))
            {
                await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(authenticationParameters.ClientState);
                return;
            }

            var authenticationUrl = authenticationParameters.ClientConfiguration.AuthorizeEndpoint;
            var callbackUrl = Url
                .Action("ProcessOpenIdConnectCallback", "OpenIdCallback", null, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            // We want to ensure to really just open an absolute uri, not any arbitrary command
            if (authenticationUrl.IsAbsoluteUri())
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var consentUrl = new RequestUrl(authenticationUrl)
                    .CreateAuthorizeUrl(clientId: authenticationParameters.ClientConfiguration.ClientId,
                    responseType: "code",
                    scope: $"{authenticationParameters.ClientConfiguration.RequiredScope} openid",
                    redirectUri: callbackUrl,
                    state: authenticationParameters.ClientState,
                    nonce: Guid.NewGuid().ToString());

                _openIdConnectCache.AuthenticationParametersByClientState.Add(authenticationParameters.ClientState, authenticationParameters);
                _openIdConnectCache.UsedRedirectUrisByClientState.Add(authenticationParameters.ClientState, callbackUrl);
                SystemBrowserService.OpenSystemBrowser(consentUrl);
            }
        }

        private async Task InitiateImplicitAuthenticationFlowAsync(OpenIdConnectAuthenticationParameters authenticationParameters)
        {
            if (authenticationParameters.Flow != OpenIdConnectFlowType.Implicit)
            {
                await _openIdConnectResultPublisher.InformClientsAboutAuthenticationFailureAsync(authenticationParameters.ClientState);
                return;
            }

            var authenticationUrl = authenticationParameters.ClientConfiguration.AuthorizeEndpoint;
            var callbackUrl = Url
                .Action("ProcessOpenIdConnectCallback", "OpenIdCallback", null, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            // We want to ensure to really just open an absolute uri, not any arbitrary command
            if (authenticationUrl.IsAbsoluteUri())
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var consentUrl = new RequestUrl(authenticationUrl)
                    .CreateAuthorizeUrl(clientId: authenticationParameters.ClientConfiguration.ClientId,
                    responseType: "id_token token",
                    scope: $"{authenticationParameters.ClientConfiguration.RequiredScope} openid",
                    redirectUri: callbackUrl,
                    state: authenticationParameters.ClientState,
                    nonce: Guid.NewGuid().ToString());

                SystemBrowserService.OpenSystemBrowser(consentUrl);
            }
        }
    }
}
