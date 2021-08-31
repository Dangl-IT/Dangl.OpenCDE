using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentMetadata
    {
        [Required, JsonProperty("_links")]
        public DocumentMetadataLinks Links { get; set; }

        [Required, JsonProperty("_metadata")]
        public List<DocumentMetadataEntry> Entries { get; set; }
    }
}
