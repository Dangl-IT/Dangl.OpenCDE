using Dangl.OpenCDE.Client.Models;
using System.Collections.Generic;

namespace Dangl.OpenCDE.Client.Services
{
    public class OpenIdConnectCache
    {
        public Dictionary<string, OpenIdConnectAuthenticationParameters> AuthenticationParametersByClientState { get; } = new Dictionary<string, OpenIdConnectAuthenticationParameters>();
        public Dictionary<string, string> UsedRedirectUrisByClientState { get; set; } = new Dictionary<string, string>();
    }
}
