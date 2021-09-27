using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class SelectedDocuments
    {
        [Required, JsonProperty("document_references")]
        public List<DocumentReference> DocumentReferences { get; set; }
    }
}
