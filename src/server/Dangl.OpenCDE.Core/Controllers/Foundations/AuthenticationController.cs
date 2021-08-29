using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Shared.Models.Foundations;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers.Foundations
{
    [Route("foundation/1.0/auth")]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly OpenCdeSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationController(OpenCdeSettings settings,
            IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(AuthGet), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAuthenticationMetadataAsync()
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var danglIdentityBaseUrl = _settings.DanglIdentitySettings.BaseUri;
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(danglIdentityBaseUrl);

            var authenticationMetadata = new AuthGet
            {
                OAuth2AuthUrl = discoveryDocument.AuthorizeEndpoint,
                OAuth2TokenUrl = discoveryDocument.TokenEndpoint,
                OAuth2DynamicClientRegistrationUrl = null,
                HttpBasicSupported = false,
                SupportedOAuth2Flows = new List<string>
                {
                    "authorization_code_grant",
                    "implicit_grant"
                },
                OAuth2RequiredScopes = _settings.DanglIdentitySettings.RequiredScope
            };

            return Ok(authenticationMetadata);
        }
    }
}
