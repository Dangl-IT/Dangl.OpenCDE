using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentMetadataLinks
    {
        [Required, JsonProperty("self")]
        public LinkData Self { get; set; }

        [Required, JsonProperty("documentReference")]
        public LinkData DocumentReference { get; set; }
    }
}
