using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration
{
    public class SimpleAuthToken
    {
        [Required]
        public string Jwt { get; set; }

        [Required]
        public long ExpiresAt { get; set; }
    }
}
