using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersions
    {
        [Required, JsonProperty("links")]
        public DocumentVersionLinks Links { get; set; }

        [Required, JsonProperty("embedded")]
        public DocumentVersionsEmbeddedReferences DocumentReferences { get; set; }
    }
}
