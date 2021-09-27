using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentMetadata
    {
        [Required, JsonProperty("links")]
        public DocumentMetadataLinks Links { get; set; }

        [Required, JsonProperty("metadata")]
        public List<DocumentMetadataEntry> Entries { get; set; }
    }
}
