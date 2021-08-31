using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentMetadataEntry
    {
        [Required, JsonProperty("name")]
        public string Name { get; set; }

        [Required, JsonProperty("value")]
        public string Value { get; set; }

        [Required, JsonProperty("type")]
        public DocumentMetadataDataType Type { get; set; }
    }
}
