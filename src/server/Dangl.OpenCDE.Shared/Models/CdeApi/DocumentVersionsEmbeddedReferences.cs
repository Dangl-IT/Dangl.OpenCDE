using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersionsEmbeddedReferences
    {
        [Required, JsonProperty("documentReferenceList")]
        public List<DocumentReference> DocumentReferences { get; set; }
    }
}
