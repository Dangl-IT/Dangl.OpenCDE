using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class OpenIdConnectClientConfiguration
    {
        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        [Required]
        public string AuthorizeEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string RequiredScope { get; set; }
    }
}
