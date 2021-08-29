using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Foundations
{
    public class AuthGet
    {
        [JsonProperty("oauth2_auth_url")]
        public string OAuth2AuthUrl { get; set; }

        [JsonProperty("oauth2_token_url")]
        public string OAuth2TokenUrl { get; set; }
    
        [JsonProperty("oauth2_dynamic_client_reg_url")]
        public string OAuth2DynamicClientRegistrationUrl { get; set; }

        [JsonProperty("http_basic_supported")]
        public bool HttpBasicSupported { get; set; }

        [Required]
        [JsonProperty("supported_oauth2_flows")]
        public List<string> SupportedOAuth2Flows { get; set; }

        [JsonProperty("oauth2_required_scopes")]
        public string OAuth2RequiredScopes { get; set; }
    }
}
