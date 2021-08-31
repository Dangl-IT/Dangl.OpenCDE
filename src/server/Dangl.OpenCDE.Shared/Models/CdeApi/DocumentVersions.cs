using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersions
    {
        [Required, JsonProperty("_links")]
        public DocumentVersionLinks Links { get; set; }

        [Required, JsonProperty("_embedded")]
        public DocumentVersionsEmbeddedReferences DocumentReferences { get; set; }
    }
}
