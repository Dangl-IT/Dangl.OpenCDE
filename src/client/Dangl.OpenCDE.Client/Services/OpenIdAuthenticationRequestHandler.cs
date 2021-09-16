using Dangl.OpenCDE.Client.Hubs;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Services
{
    public class OpenIdAuthenticationRequestHandler
    {
        private readonly OpenIdConnectCache _openIdConnectCache;
        private readonly OpenIdConnectResultPublisher _openIdConnectResultPublisher;
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenIdAuthenticationRequestHandler(OpenIdConnectCache openIdConnectCache,
            OpenIdConnectResultPublisher openIdConnectResultPublisher,
            IHttpClientFactory httpClientFactory)
        {
            _openIdConnectCache = openIdConnectCache;
            _openIdConnectResultPublisher = openIdConnectResultPublisher;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> HandleOpenIdCodeResponse(HttpContext ctx,
            string code)
        {
            var state = ctx.Request.Query["state"].FirstOrDefault();

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
            return content;
        }
    }
}
