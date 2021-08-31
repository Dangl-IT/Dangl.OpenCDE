using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class OpenIdConnectAuthenticationParameters
    {
        [Required]
        public OpenIdConnectFlowType Flow { get; set; }

        [Required]
        public string ClientState { get; set; }

        [Required]
        public OpenIdConnectClientConfiguration ClientConfiguration { get; set; }
    }
}
