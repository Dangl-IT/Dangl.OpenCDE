using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class OpenIdConnectAuthenticationResult
    {
        [Required]
        public bool IsSuccess { get; set; }

        public string JwtToken { get; set; }

        public long ExpiresAt { get; set; }

        [Required]
        public string ClientState { get; set; }
    }
}
