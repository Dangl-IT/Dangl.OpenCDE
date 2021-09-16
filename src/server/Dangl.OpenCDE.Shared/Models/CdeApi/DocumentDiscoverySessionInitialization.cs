using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentDiscoverySessionInitialization
    {
        [Required, JsonProperty("select_documents_url")]
        public string SelectDocumentsUrl { get; set; }

        [Required, JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
