using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentDiscoverySessionInitialization
    {
        [Required]
        public string SelectDocumentsUrl { get; set; }

        [Required]
        public int ExpiresIn { get; set; }
    }
}
